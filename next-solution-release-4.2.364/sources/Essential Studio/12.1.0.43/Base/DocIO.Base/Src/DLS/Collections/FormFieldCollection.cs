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

#region File using directives
using System;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of form fields.
    /// </summary>
    public class FormFieldCollection : CollectionImpl
    {
        #region Fields
        private Dictionary<string, WFormField> m_dictionary = new Dictionary<string, WFormField>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WFormField"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WFormField this[int index]
        {
            get
            {
                return (WFormField)InnerList[index];
            }
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WFormField"/> by specified form field name.
        /// </summary>
        /// <value></value>
        public WFormField this[string formFieldName]
        {
            get
            {
                return GetByName(formFieldName);
            }
        }
        /// <summary>
        /// Gets dictionary with form field items.
        /// </summary>
        internal Dictionary<string, WFormField> FormFieldDictonary
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FormFieldCollection"/> class.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        internal FormFieldCollection(WTextBody textBody)
            : base(textBody.Document, textBody)
        {
            Populate(textBody);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the specified collection contains item with specified name.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public bool ContainsName(string itemName)
        {
            return m_dictionary.ContainsKey(itemName);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Corrects the name.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        internal void CorrectName(string oldName, string newName)
        {
            WFormField ff = m_dictionary[oldName];
            m_dictionary.Remove(oldName);
            m_dictionary.Add(newName, ff);

            WTableCell cell = OwnerBase as WTableCell;

            if (cell != null && cell.OwnerRow != null && cell.OwnerRow.OwnerTable != null)
            {
                WTextBody textBody = cell.OwnerRow.OwnerTable.OwnerTextBody;

                if (textBody != null && textBody.IsFormFieldsCreated)
                {
                    textBody.FormFields.CorrectName(oldName, newName);
                }
            }
        }
        /// <summary>
        /// Adds the specified formField.
        /// </summary>
        /// <param name="ff">The formField.</param>
        internal void Add(WFormField ff)
        {
            InnerList.Add(ff);
            if (ff.Name != null && ff.Name != string.Empty && !m_dictionary.ContainsKey(ff.Name))
            {
                m_dictionary.Add(ff.Name, ff);
            }
        }
        /// <summary>
        /// Removes the specified formField.
        /// </summary>
        /// <param name="ff">The formField.</param>
        internal void Remove(WFormField ff)
        {
            InnerList.Remove(ff);
            if (ff.Name != null && ff.Name != string.Empty && m_dictionary.ContainsKey(ff.Name))
            {
                m_dictionary.Remove(ff.Name);
                Bookmark bkmk = m_doc.Bookmarks.FindByName(ff.Name);
                if(bkmk!=null)
                    m_doc.Bookmarks.Remove(bkmk);
            }
        }
        /// <summary>
        /// Populates this coollection of form fields from parent WTextBody.
        /// </summary>
        private void Populate(WTextBody textBody)
        {
            foreach (TextBodyItem item in textBody.ChildEntities)
            {
                switch (item.EntityType)
                {
                    case EntityType.Paragraph:
                        PopulateFromParagraph((WParagraph)item);
                        break;

                    case EntityType.Table:
                        PopulateFromTable((WTable)item);
                        break;
                }
            }
        }
        /// <summary>
        /// Populates from paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void PopulateFromParagraph(WParagraph para)
        {
            foreach (ParagraphItem item in para.Items)
            {
                if (item.EntityType == EntityType.TextFormField ||
                  item.EntityType == EntityType.CheckBox ||
                  item.EntityType == EntityType.DropDownFormField)
                {
                    Add((WFormField)item);
                }

                switch (item.EntityType)
                {
                    case EntityType.TextBox:
                        Populate((item as WTextBox).TextBoxBody);
                        break;
                    case EntityType.Footnote:
                        Populate((item as WFootnote).TextBody);
                        break;
                    case EntityType.Comment:
                        Populate((item as WComment).TextBody);
                        break;
                }
            }
        }
        /// <summary>
        /// Populates from table.
        /// </summary>
        /// <param name="table">The table.</param>
        private void PopulateFromTable(WTable table)
        {
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    Populate(cell);
                }
            }
        }
        /// <summary>
        /// Gets form field by name.
        /// </summary>
        /// <param name="formFieldName">Name of the form field.</param>
        /// <returns></returns>
        private WFormField GetByName(string formFieldName)
        {
            if (m_dictionary.ContainsKey(formFieldName))
            {
                return m_dictionary[formFieldName];
            }
            return null;
        }
        #endregion
    }
}
