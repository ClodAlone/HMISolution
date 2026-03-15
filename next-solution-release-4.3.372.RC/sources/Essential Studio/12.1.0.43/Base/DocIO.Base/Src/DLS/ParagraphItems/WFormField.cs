#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WFormField.
    /// </summary>
    public abstract class WFormField
      : WField
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_VALUE = 25;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected FormFieldType m_curFormFieldType;
        private short m_params;
        private string m_title;
        private string m_help;
        private string m_tooltip;
        private string m_macroOnStart;
        private string m_macroOnEnd;
        /// <summary>
        /// Specifies whether the field has Form field data.
        /// </summary>
        private bool m_bHasFFData = true;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets type of this form field.
        /// </summary>
        public FormFieldType FormFieldType
        {
            get
            {
                return m_curFormFieldType;
            }
        }
        /// <summary>
        /// Gets/sets form field title name (bookmark name).
        /// </summary>
        public string Name
        {
            get
            {
                return m_title;
            }
            set
            {
                ApplyNewBookmarkName(m_title, value);
                m_title = value;
            }
        }
        /// <summary>
        /// Gets/sets form field help.
        /// </summary>
        public string Help
        {
            get
            {
                return m_help;
            }
            set
            {
                m_help = value;
                m_params = (short)BaseWordRecord.SetBitsByMask(m_params, 0x80, 7, 1);
            }
        }
        /// <summary>
        /// Gets or sets the status bar help.
        /// </summary>
        /// <value>The status bar help.</value>
        public string StatusBarHelp
        {
            get
            {
                return m_tooltip;
            }
            set
            {
                m_tooltip = value;
                m_params = (short)BaseWordRecord.SetBitsByMask(m_params, 0x100, 8, 1);
            }
        }
        /// <summary>
        /// Gets / sets the name of macros on start
        /// </summary>
        public string MacroOnStart
        {
            get
            {
                return m_macroOnStart;
            }
            set
            {
                m_macroOnStart = value;
            }
        }
        /// <summary>
        /// Gets / sets the name of macros on end
        /// </summary>
        public string MacroOnEnd
        {
            get
            {
                return m_macroOnEnd;
            }
            set
            {
                m_macroOnEnd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Value
        {
            get
            {
                return ((m_params & 0x7c) >> 2);
            }
            set
            {
                m_params = (short)((m_params & -125) | (value << 2));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Params
        {
            get
            {
                return m_params;
            }
            set
            {
                m_params = (short)value;
            }
        }
        /// <summary>
        /// Get/sets Enabled property (true if form field enabled).
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (m_params & 0x200) == 0;
            }
            set
            {
                int enabledValue = value ? 0 : 1;
                m_params = (short)BaseWordRecord.SetBitsByMask(m_params, 0x200, 9, enabledValue);
            }
        }
        /// <summary>
        /// Gets/sets calculate on exit property.
        /// </summary>
        public bool CalculateOnExit
        {
            get
            {
                return (m_params & 0x4000) == 0x4000;
            }
            set
            {
                m_params = (value) ? (short)BaseWordRecord.SetBitsByMask(m_params, 0x4000, 14, 1) :
                                       (short)BaseWordRecord.SetBitsByMask(m_params, 0x4000, 14, 0);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has form field data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has form field data; otherwise, <c>false</c>.
        /// </value>
        internal bool HasFFData
        {
            get
            {
                return m_bHasFFData;
            }
            set
            {
                m_bHasFFData = value;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WFormField"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WFormField(IWordDocument doc)
            : base(doc)
        {
            m_paraItemType = ParagraphItemType.FormField;
            m_title = string.Empty;
            m_help = string.Empty;
            m_tooltip = string.Empty;
            m_macroOnStart = string.Empty;
            m_macroOnEnd = string.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WFormField"/> class.
        /// </summary>
        /// <param name="formField">The form field.</param>
        /// <param name="doc">The doc.</param>
        protected WFormField(WFormField formField, IWordDocument doc)
            : this(doc)
        {
            this.Help = formField.Help;
            this.MacroOnEnd = formField.MacroOnEnd;
            this.MacroOnStart = formField.MacroOnStart;
            this.Params = formField.Params;
            this.Name = formField.Name;
            this.StatusBarHelp = formField.StatusBarHelp;
            this.Value = formField.Value;
            this.FieldType = formField.FieldType;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WFormField ff = (WFormField)base.CloneImpl();
            ff.CharacterFormat.ImportContainer(CharacterFormat);

            return ff;
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos"></param>
        internal override void Attach(WParagraph paragraph, int itemPos)
        {
            base.Attach(paragraph, itemPos);

            if (!Document.IsOpening)
            {
                AttachForTextBody(paragraph.Owner as WTextBody);
            }
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            base.Detach();

            DetachForTextBody(OwnerParagraph.Owner as WTextBody);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Attaches for text body.
        /// </summary>
        private void AttachForTextBody(WTextBody textBody)
        {
            if (textBody != null && textBody.IsFormFieldsCreated)
            {
                textBody.FormFields.Add(this);

                if (textBody is WTableCell)
                {
                    WTable table = textBody.Owner.Owner as WTable;

                    if (table != null)
                    {
                        AttachForTextBody(table.Owner as WTextBody);
                    }
                }
            }
        }
        /// <summary>
        /// Detaches for text body.
        /// </summary>
        private void DetachForTextBody(WTextBody textBody)
        {
            if (textBody != null)
            {
                if (textBody.IsFormFieldsCreated)
                {
                    textBody.FormFields.Remove(this);
                }

                if (textBody is WTableCell)
                {
                    DetachForTextBody(textBody.Owner.Owner.Owner as WTextBody);
                }
            }
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FormFieldParamsAttr))
            {
                Params = reader.ReadInt(XDLSConstants.FormFieldParamsAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldTitleAttr))
            {
                m_title = reader.ReadString(XDLSConstants.FormFieldTitleAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldHelpAttr))
            {
                m_help = reader.ReadString(XDLSConstants.FormFieldHelpAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldTooltipAttr))
            {
                m_tooltip = reader.ReadString(XDLSConstants.FormFieldTooltipAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldMacroOnStartAttr))
            {
                m_macroOnStart = reader.ReadString(XDLSConstants.FormFieldMacroOnStartAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldMacroOnEndAttr))
            {
                m_macroOnEnd = reader.ReadString(XDLSConstants.FormFieldMacroOnEndAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.FormFieldParamsAttr, m_params);
            writer.WriteValue(XDLSConstants.FormFieldTitleAttr, m_title);
            writer.WriteValue(XDLSConstants.FormFieldHelpAttr, m_help);
            writer.WriteValue(XDLSConstants.FormFieldTooltipAttr, m_tooltip);
            writer.WriteValue(XDLSConstants.FormFieldMacroOnStartAttr, m_macroOnStart);
            writer.WriteValue(XDLSConstants.FormFieldMacroOnEndAttr, m_macroOnEnd);
        }
//#endif
        #endregion

        #region Class helper methods
        /// <summary>
        /// Applies the new name of the bookmark.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        private void ApplyNewBookmarkName(string oldName, string newName)
        {
            if (this.Document != null && this.Document.IsOpening)
                return;

            if (this.OwnerParagraph != null)
            {
                CheckFormFieldName(newName);

                bool bookmarkFound = false;

                if (this.Document != null)
                {
                    bookmarkFound = ApplyInDocBkmkColl(oldName, newName);
                }
                if (!bookmarkFound)
                {
                    ApplyInOwnerParaColl(oldName, newName);
                }

                if (OwnerParagraph.OwnerTextBody != null && OwnerParagraph.OwnerTextBody.IsFormFieldsCreated)
                {
                    OwnerParagraph.OwnerTextBody.FormFields.CorrectName(oldName, newName);
                }
            }
        }
        /// <summary>
        /// Applies new bookmark name in bookmark collection of document.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <returns></returns>
        private bool ApplyInDocBkmkColl(string oldName, string newName)
        {
            bool bookmarkFound = false;
            BookmarkCollection bookmarks = this.Document.Bookmarks;

            if (bookmarks.Count > 0)
            {
                Bookmark bookmark = bookmarks[oldName];
                if (bookmark != null && bookmark.BookmarkStart != null && bookmark.BookmarkEnd != null)
                {
                    bookmark.BookmarkStart.SetName(newName);
                    bookmark.BookmarkEnd.SetName(newName);
                    bookmarkFound = true;
                }
            }

            return bookmarkFound;
        }
        /// <summary>
        /// Applies the in owner para collection.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        private void ApplyInOwnerParaColl(string oldName, string newName)
        {
            BookmarkStart bkmkStart = null;
            BookmarkEnd bkmkEnd = null;

            foreach (IParagraphItem item in this.OwnerParagraph.Items)
            {
                if (item is BookmarkStart && (item as BookmarkStart).Name == oldName)
                {
                    bkmkStart = item as BookmarkStart;
                }
                else if (item is BookmarkEnd && (item as BookmarkEnd).Name == oldName)
                {
                    bkmkEnd = item as BookmarkEnd;
                    if (bkmkStart != null) break;
                }
            }

            if (bkmkStart != null && bkmkEnd != null)
            {
                bkmkStart.SetName(newName);
                bkmkEnd.SetName(newName);
            }
        }
        /// <summary>
        /// Checks if collection of bookmarks in document contains bookmark with specified name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        private void CheckFormFieldName(string newName)
        {
            Bookmark bookmark = this.Document.Bookmarks[newName];

            if (bookmark != null)
            {
                throw new ArgumentException("Bookmark with name \"" + newName + "\" already exists.");
            }

            foreach (WSection section in this.Document.Sections)
            {
                if (section.Body.FormFields != null && section.Body.FormFields.ContainsName(newName))
                {
                    throw new ArgumentException("Form field with name \"" + newName + "\" already exists.");
                }
            }
        }
        #endregion
    }
}
