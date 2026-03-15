#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.Collections
{
    public class MetaPropertiesImpl 
        : CollectionBaseEx<IMetaProperty>
        ,IMetaProperties
    {
        #region Class members
        private Dictionary<string, IMetaProperty> m_hashNameToIMetaProperty = new Dictionary<string, IMetaProperty>();
        private WorkbookImpl m_book;
        private WorksheetImpl m_worksheet;
        private string m_SchemaXml;
        private string m_ItemName;
        private bool m_isValid = true;

        #endregion
        #region Class constructors
        /// <summary>
        /// Creates new empty collection.
        /// </summary>
        /// <param name="application">Application object for the collection.</param>
        /// <param name="parent">Parent object for the new collection.</param>
        public MetaPropertiesImpl(IApplication application, object parent)
            : base(application, parent)
        {
            
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// ItemName
        /// </summary>
        internal string ItemName
        {
            get
            {
                return m_ItemName;
            }
            set
            {
                m_ItemName = value;
            }
        }
        /// <summary>
        /// ItemName
        /// </summary>
         internal bool IsValid
        {
            get
            {
                return m_isValid;
            }
            set
            {
                m_isValid = value;
            }
        }
        #endregion


        #region IMetaProperties Members

        /// <summary>
        /// SchemaXml
        /// </summary>
        public string SchemaXml
        {
            get 
            { 
                return this.m_SchemaXml; 
            }
            set
            {
                m_SchemaXml = value;
            }
        }
        /// <summary>
        /// Triggered when new items are added to collection
        /// </summary>
        protected override void OnInsertComplete(int index, IMetaProperty value)
        {
            MetaPropertyImpl name = (MetaPropertyImpl)value;
            base.OnInsertComplete(index, value);

            m_hashNameToIMetaProperty[name.InternalName] = value;
        }
        #endregion

        /// <summary>
        /// Checks whether collection contains named range.
        /// </summary>
        /// <param name="name">Name of the named range to search.</param>
        /// <returns>True if collection contains such named range; otherwise returns False.</returns>
        public IMetaProperty GetItemByInternalName(string name)
        {
            if (m_hashNameToIMetaProperty.ContainsKey(name))
            {
                return m_hashNameToIMetaProperty[name];
            }
            return null;
        }
    }
}
