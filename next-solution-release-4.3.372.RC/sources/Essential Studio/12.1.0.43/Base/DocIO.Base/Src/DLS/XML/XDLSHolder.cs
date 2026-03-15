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
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for DLSXmlHolder.
    /// </summary>
    public class XDLSHolder
    {
        #region Class members
        private int m_id = -1;
        private Dictionary<string, object> m_hashElements = null;
        private Dictionary<string, object> m_hashRefElements = null;
        private bool m_bCleared = true;
        private bool m_bGenID = false;
        private bool m_bSkipMe = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="XDLSHolder"/> is cleared.
        /// </summary>
        /// <value>if it cleared, set to <c>true</c>.</value>
        public bool Cleared
        {
            get
            {
                return m_bCleared;
            }
            set
            {
                if (value != m_bCleared)
                {
                    if (value)
                    {
                        Clear();
                    }
                    else
                    {
                        m_bCleared = false;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable ID].
        /// </summary>
        /// <value>If it is enable ID, set to <c>true</c>.</value>
        public bool EnableID
        {
            get
            {
                return m_bGenID;
            }
            set
            {
                m_bGenID = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [skip me].
        /// </summary>
        /// <value>If it specifies to skip, set to <c>true</c>.</value>
        public bool SkipMe
        {
            get
            {
                return m_bSkipMe;
            }
            set
            {
                m_bSkipMe = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="XDLSHolder"/> class.
        /// </summary>
        public XDLSHolder()
        {
        }
        #endregion

        #region Class public methods
//#if !SILVERLIGHT
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="value">The value.</param>
        public void AddElement(string tagName, object value)
        {
            if (m_hashElements == null)
            {
                m_hashElements = new Dictionary<string, object>();
                //m_hashElements = new List<object>();
            }

            m_hashElements[tagName] = value;
        }
        /// <summary>
        /// Adds the ref element.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="value">The value.</param>
        public void AddRefElement(string tagName, object value)
        {
            if (m_hashRefElements == null)
            {
                m_hashRefElements = new Dictionary<string, object>();
            }

            m_hashRefElements[tagName] = value;
        }
//#endif
        /// <summary>
        /// Writes the holder.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteHolder(IXDLSContentWriter writer)
        {
            if (m_hashElements != null)
            {
                foreach (string keyTagName in m_hashElements.Keys)
                {
                    writer.WriteChildElement(keyTagName, m_hashElements[keyTagName]);
                }
            }

            if (m_hashRefElements != null)
            {
                foreach (string keyTagName in m_hashRefElements.Keys)
                {
                    IXDLSSerializable refElement = m_hashRefElements[keyTagName] as IXDLSSerializable;
                    if (refElement != null)
                    {
                        writer.WriteChildRefElement(keyTagName, refElement.XDLSHolder.ID);
                    }
                    else
                    {
                        //writer.WriteChildRefElement( keyTagName, -1 );
                    }
                }
            }
        }
        /// <summary>
        /// Reads the holder.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public bool ReadHolder(IXDLSContentReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                string tagName = reader.TagName;

                if (m_hashElements != null && m_hashElements.ContainsKey( tagName ))
                {
                    object value = m_hashElements[tagName];

                    if (value != null)
                    {
                        IXDLSFactory factory = value as IXDLSFactory;
                        if (factory != null)
                        {
                            value = factory.Create(reader);
                            m_hashElements[tagName] = value;
                        }

                        return reader.ReadChildElement(value);
                    }
                }

                if (m_hashRefElements != null)
                {
                    if (m_hashRefElements.ContainsKey(tagName))
                    {
                        string sRef = reader.GetAttributeValue("ref");

                        if (sRef == null)
                        {
                            m_hashRefElements[reader.TagName] = -1;
                        }
                        else
                        {
                            m_hashRefElements[reader.TagName] = XmlConvert.ToInt32(sRef);
                        }

                        return false;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// After the deserialization.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void AfterDeserialization(IXDLSSerializable owner)
        {
            if (m_hashElements != null)
            {
                // Recursive calls AfterDeserialization method for all subitems
                foreach (string keyTagName in m_hashElements.Keys)
                {
                    IXDLSSerializable dlsSer = m_hashElements[keyTagName] as IXDLSSerializable;

                    if (dlsSer != null)
                    {
                        dlsSer.XDLSHolder.AfterDeserialization(dlsSer);
                    }
                    else
                    {
                        IXDLSSerializableCollection dlsSerColl =
                          m_hashElements[keyTagName] as IXDLSSerializableCollection;

                        if (dlsSerColl != null)
                        {
                            //for( int i = 0; i < dlsSerColl.Count; i++ )
                            foreach (IXDLSSerializable dlsSerItem in dlsSerColl)
                            {
                                //IXDLSSerializable dlsSerItem = dlsSerColl[ i ] as IXDLSSerializable;

                                if (dlsSerItem != null)
                                {
                                    dlsSerItem.XDLSHolder.AfterDeserialization(dlsSerItem);
                                }
                            }
                        }
                    }
                }
            }

            // Restore references
            if (m_hashRefElements != null)
            {
                foreach (string keyTagName in m_hashRefElements.Keys)
                {
                    int refValue = -1;

                    if (m_hashRefElements[keyTagName] != null)
                    {
                        refValue = (int)m_hashRefElements[keyTagName];
                    }

                    owner.RestoreReference(keyTagName, refValue);
                }
            }
            Clear();
        }
        /// <summary>
        /// Before the serialization.
        /// </summary>
        public void BeforeSerialization()
        {
            if (m_hashElements != null)
            {
                // Recursive calls BeforeSerialization method for all subitems
                foreach (string keyTagName in m_hashElements.Keys)
                {
                    IXDLSSerializable dlsSer = m_hashElements[keyTagName] as IXDLSSerializable;

                    if (dlsSer != null)
                    {
                        dlsSer.XDLSHolder.Cleared = true;
                        dlsSer.XDLSHolder.BeforeSerialization();
                    }
                    else
                    {
                        IXDLSSerializableCollection dlsSerColl =
                          m_hashElements[keyTagName] as IXDLSSerializableCollection;

                        if (dlsSerColl != null)
                        {
                            int id = 0;
                            //for( int i = 0; i < dlsSerColl.Count; i++ )
                            foreach (IXDLSSerializable dlsSerItem in dlsSerColl)
                            {
                                //IXDLSSerializable dlsSerItem = dlsSerColl[ i ] as IXDLSSerializable;

                                if (dlsSerItem != null)
                                {
                                    dlsSerItem.XDLSHolder.Cleared = true;
                                    dlsSerItem.XDLSHolder.ID = id; // Update item ID.
                                    dlsSerItem.XDLSHolder.BeforeSerialization();
                                    id++;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Clear()
        {
            if (m_hashElements != null)
            {
                m_hashElements.Clear();
            }

            if (m_hashRefElements != null)
            {
                m_hashRefElements.Clear();
            }

            m_bCleared = true;
        }
        #endregion
    }
}