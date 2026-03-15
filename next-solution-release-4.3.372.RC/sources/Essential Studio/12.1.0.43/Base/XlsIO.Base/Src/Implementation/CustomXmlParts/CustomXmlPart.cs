#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation
{
    public class CustomXmlPart : CommonObject,ICustomXmlPart
    {
        private CustomXmlSchemaCollection m_schemacollection = new CustomXmlSchemaCollection();
        private string m_Id = "";
        private byte[] m_data;
        /// <summary>
        /// Parent workbook for this object.
        /// </summary>
        private WorkbookImpl m_book;
        /// <summary>
        /// Parent worksheet for this object.
        /// </summary>
        private WorksheetImpl m_worksheet;
        /// <summary>
        /// Index of the Name object in the Workbook's CustomXml Parts collection.
        /// </summary>
        private int m_index = -1;

        /// <summary>
        /// Creates new CustomXml object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXml object.</param>
        /// <param name="parent">Parent object for the new CustomXml object.</param>
        /// <param name="id">Name of the new Customxml object.</param>
        /// <param name="index">Current index.</param>
        public CustomXmlPart(IApplication application, object parent, string id, int index)
            : this(application, parent, id, index, false)
        {
        }
        /// <summary>
        /// Creates new CustomXml object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXml object.</param>
        /// <param name="parent">Parent object for the new CustomXml object.</param>
        /// <param name="id">Name of the new CustomXml object.</param>
        /// <param name="index">Current index.</param>
        /// <param name="bIsLocal">Indicates whether CustomXml object is local.</param>
        public CustomXmlPart(IApplication application, object parent, string id, int index, bool bIsLocal)
            : this(application, parent)
        {
            m_index = index;
            m_Id = id;
        }
        /// <summary>
        /// Creates a new CustomXml object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXml object.</param>
        /// <param name="parent">Parent object for the new CustomXml object.</param>
        public CustomXmlPart(IApplication application, object parent)
            : base(application, parent)
        {
            SetParents();
        }
        /// <summary>
        /// Creates a new CustomXml object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXml object.</param>
        /// <param name="parent">Parent object for the new CustomXml object.</param>
        /// <param name="name">Name of the new CustomXml object.</param>
        /// <param name="schemas">Schemas to be added
        /// Range that will be associated with the specified name.
        /// </param>
        /// <param name="index"></param>
        public CustomXmlPart(IApplication application, object parent, string id, ICustomXmlSchemaCollection schemas
      , int index)
            : this(application, parent, id, schemas, index, false)
        {
        }
        /// <summary>
        /// Creates a new CustomXml object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXml object.</param>
        /// <param name="parent">Parent object for the new CustomXml object.</param>
        /// <param name="id">Name of the new CustomXml object.</param>
        /// <param name="data">Xml Data in byte Array</param>
        public CustomXmlPart(IApplication application, object parent, string id, byte[] data
      , int index)
            : this(application, parent, id, data, index, false)
        {
        }
        /// <summary>
        /// Creates a new CustomXmlPart object.
        /// </summary>
        /// <param name="application">Application object for the new CustomXmlPart object.</param>
        /// <param name="parent">Parent object for the new CustomXmlPart object.</param>
        /// <param name="id">ID of the new CustomXmlPart object.</param>
        /// <param name="schemas">schemas of the customxml data</param>
        /// <param name="index"></param>
        /// <param name="bIsLocal"></param>
        public CustomXmlPart(IApplication application, object parent, string id, ICustomXmlSchemaCollection customXmlSchemaCollection
      , int index, bool bIsLocal)
            : this(application, parent)
        {
            m_index = index;
            SetParents();
            m_Id = id;
            Schemas = customXmlSchemaCollection;
        }
        /// <summary>
        /// Creates a new customxml object.
        /// </summary>
        /// <param name="application">Application object for the new customxml object.</param>
        /// <param name="parent">Parent object for the new customxml object.</param>
        /// <param name="id">ID of the new customxml object.</param>
        /// <param name="range">Xml Data in byte array</param>
        /// <param name="index"></param>
        /// <param name="bIsLocal"></param>
        public CustomXmlPart(IApplication application, object parent, string id, byte[] data
      , int index, bool bIsLocal)
            : this(application, parent)
        {
            m_index = index;
            m_data = data;
            SetParents();
            m_Id = id;
        }
        public override void Dispose()
        {
            base.Dispose();
            m_book = null;
        }
        
        #region ICustomXmlPart Members

        /// <summary>
        /// XmlData in Byte array
        /// </summary>
        public byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        /// <summary>
        /// Unique ID for Xml Data
        /// </summary>
        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }
        /// <summary>
        /// Xml Schemas collection for CustomXml data
        /// </summary>
        public ICustomXmlSchemaCollection Schemas
        {
            get { return m_schemacollection; }
            set { m_schemacollection = value as CustomXmlSchemaCollection; }
        }
        /// <summary>
        /// create new clone object
        /// </summary>
        public ICustomXmlPart Clone()
        {
            CustomXmlPart part = (CustomXmlPart)base.MemberwiseClone();
            part.m_schemacollection = (CustomXmlSchemaCollection)m_schemacollection.Clone();
            return part;
        }
        /// <summary>
        /// Sets index of the named range and raise event.
        /// </summary>
        /// <param name="index">New index.</param>
        public void SetIndex(int index)
        {
            SetIndex(index, true);
        }
        /// <summary>
        /// Sets index of the named range.
        /// </summary>
        /// <param name="index">New index.</param>
        /// <param name="bRaiseEvent">Indicates whether events should be raised.</param>
        public void SetIndex(int index, bool bRaiseEvent)
        {
            if (index != m_index)
            {
                int oldIndex = m_index;
                m_index = index;

            }
        }
        /// <summary>
        /// Sets parent workbook and worksheet.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// When parent workbook or worksheet cannot be found.
        /// </exception>
        private void SetParents()
        {
            m_worksheet = FindParent(typeof(WorksheetImpl)) as WorksheetImpl;

            if (m_worksheet != null)
            {
                m_book = m_worksheet.Workbook as WorkbookImpl;
            }
            else
            {
                m_book = FindParent(typeof(WorkbookImpl)) as WorkbookImpl;

                if (m_book == null)
                    throw new ArgumentNullException("IName has no parent workbook");
            }
        }

        #endregion
    }
}
