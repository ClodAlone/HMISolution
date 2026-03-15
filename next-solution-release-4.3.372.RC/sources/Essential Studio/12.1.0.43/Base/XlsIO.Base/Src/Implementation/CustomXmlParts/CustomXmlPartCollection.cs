#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Collections
{
    public class CustomXmlPartCollection
        : CollectionBaseEx<ICustomXmlPart>
        , ICustomXmlPartCollection
    {
        /// <summary>
        /// Dictionary with CustomXml data, key - property name/id, value - property value.
        /// </summary>
        private Dictionary<string, ICustomXmlPart> m_propertiesHash = new Dictionary<string, ICustomXmlPart>();
        /// <summary>
        /// Parent workbook for the collection.
        /// </summary>
        private WorkbookImpl m_book;
        /// <summary>
        /// Parent workbook for the collection.
        /// </summary>
        private object m_parent;

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomXmlPartCollection(IApplication application, object parent)
            : base(application, parent)
        {
            SetParent();
            m_parent = parent;
        }
        #endregion



        #region ICustomXmlPartCollection Members
        /// <summary>
        /// CustomXmlPart Count
        /// </summary>
        int ICustomXmlPartCollection.Count
        {
            get
            {
                return List.Count;
            }
        }
        /// <summary>
        /// Retruns CustomXmlPart at corresponding index positions
        /// </summary>
        public new ICustomXmlPart this[int index]
        {
            get
            {

                if (index < 0 || index >= List.Count)
                    throw new ArgumentOutOfRangeException(
                      string.Format("index is {0}, Count is {1}", index, List.Count));

                return (ICustomXmlPart)List[index];
            }
            
        }
        /// <summary>
        /// Represents the collection count
        /// </summary>
        public int Count
        {
            get
            {
                return List.Count;
            }
        }
        /// <summary>
        /// Creates new instance of clone object
        /// </summary>
        public ICustomXmlPartCollection Clone()
        {
            return (ICustomXmlPartCollection)base.Clone(m_parent);
        }
        /// <summary>
        /// Returns CustomXmlPart for corresponding id
        /// </summary>
        public ICustomXmlPart GetById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (m_propertiesHash.ContainsKey(id))
                return m_propertiesHash[id];
            else
                return null;
        }
        /// <summary>
        /// Removes specified name from the collection.
        /// </summary>
        /// <param name="index">Name of the object to remove.</param>
        void ICustomXmlPartCollection.RemoveAt(int index)
        {
            if (index < 0 || index > Count - 1)
                throw new ArgumentOutOfRangeException("index", "Value cannot be less than 0 and greater than Count - 1.");

            ICustomXmlPart toDel = List[index];
            List.RemoveAt(index);
            m_propertiesHash.Remove(toDel.Id);
        }
        /// <summary>
        /// Clear all items in the collections
        /// </summary>
        void ICustomXmlPartCollection.Clear()
        {
            List.Clear();
            m_propertiesHash.Clear();
        }

        #endregion

       

        #region ICustomXmlPartCollection Members

        /// <summary>
        /// Adds new CustomXmlpart to the collections
        /// </summary>
        public ICustomXmlPart Add(ICustomXmlPart customXmlPart)
        {
            CustomXmlPart xmlPart = customXmlPart as CustomXmlPart;

            if(m_propertiesHash.ContainsKey(xmlPart.Id))
            {
                 throw new ArgumentException( "ID of the CustomXmlPart object must be unique." );
            }

            AddLocal(customXmlPart, true);

            return customXmlPart;
        }
        /// <summary>
        /// Cretes new CustomXmlParts through specified arugments
        /// </summary>
        public ICustomXmlPart Add(string id, byte[] XmlData)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (XmlData == null)
                throw new ArgumentNullException("XmlData");

            if (id.Length == 0)
                throw new ArgumentException("id");

            if(XmlData.Length==0)
                throw new ArgumentNullException("XmlData");

            CustomXmlPart customxmlpart = new CustomXmlPart(Application, this, id, XmlData, List.Count);

            Add(customxmlpart);

            return customxmlpart;

        }
        /// <summary>
        /// Creates new instance of CusomXmlPart from ID
        /// </summary>
        public ICustomXmlPart Add(string id)
        {
            if (id == null)
                throw new ArgumentNullException("ID");

            if (id.Length == 0)
                throw new ArgumentException("ID");

            CustomXmlPart customXmlPart = new CustomXmlPart(Application, m_book,id,List.Count);

            Add(customXmlPart);

            return customXmlPart;
        }

        /// <summary>
        /// Adds into list and hashtable, for local named ranges.
        /// </summary>
        /// <param name="name">Name to add.</param>
        /// <param name="bAddInGlobalNamesHash">Indicates is adds in global names hash.</param>
        public void AddLocal(ICustomXmlPart name, bool bAddInGlobalNamesHash)
        {
            //((CustomXmlPart)name).SetIndex(Count);

            if (bAddInGlobalNamesHash)
            {
                base.Add(name);
            }
            else
            {
                InnerList.Add(name);
            }

        }
        /// <summary>
        /// Performs additional processes after inserting a new element into the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert value.</param>
        /// <param name="value">The new value of the element at the index.</param>
        protected override void OnInsertComplete(int index, ICustomXmlPart customXmlPart)
        {
            CustomXmlPart name = (CustomXmlPart)customXmlPart;
            base.OnInsertComplete(index, customXmlPart);
            m_propertiesHash[name.Id] = name;
        }
        /// <summary>
        /// Sets parent workbook value.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// When  parent workbook cannot be found.
        /// </exception>
        private void SetParent()
        {
            m_book = FindParent(typeof(WorkbookImpl)) as WorkbookImpl;

            if (m_book == null)
                throw new ArgumentNullException("NamesCollection has no parent Workbook.");
        }
       
        /// <summary>
        /// Removes specified name from the collection.
        /// </summary>
        /// <param name="name">Name of the object to remove.</param>
        public void Remove(string id)
        {
            ICustomXmlPart toDel;

            if (m_propertiesHash.TryGetValue(id,out toDel))
            {
                m_propertiesHash.Remove(id);

                List.Remove(toDel);
            }
        }

        #endregion



        

    }
}
