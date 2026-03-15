#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Collection of the cached objects.
    /// </summary>
    internal class PdfCacheCollection
    {
        #region Fields
        /// <summary>
        /// Stores the similar objects.
        /// </summary>
        private List<List<object>> m_referenceObjects;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        private Dictionary<Font, int> m_fontOffsets;
        private Dictionary<Font,byte[]> m_fontData;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCacheCollection"/> class.
        /// </summary>
        public PdfCacheCollection()
        {
            m_referenceObjects = new List<List<object>>();
#if !SILVERLIGHT && !NETFX_CORE && !WP
            m_fontData = new Dictionary<Font,byte[]>();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="System.Collections.ArrayList"/> at the specified index.
        /// </summary>
        /// <value></value>
        private List<object> this[int index]
        {
            get
            {
                List<object> obj = m_referenceObjects[index];
                return obj;
            }
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the font offset table.
        /// </summary>
        /// <value>The font offset table.</value>
        internal Dictionary<Font, int> FontOffsetTable
        {
            get
            {
                if (m_fontOffsets == null)
                    m_fontOffsets = new Dictionary<Font, int>();

                return m_fontOffsets;
            }
        }

        /// <summary>
        /// Gets the font data.
        /// </summary>
        /// <value>The font data.</value>
        internal Dictionary<Font, byte[]> FontData
        {
            get
            {
                return m_fontData;
            }
        }
#endif
        #endregion

        #region Public methods
        /// <summary>
        /// Searches for the similar cached object. If is not found - adds the object to the cache.
        /// </summary>
        /// <param name="obj">Object to search for.</param>
        /// <returns>Cached similar object if found, null otherwise.</returns>
        public IPdfCache Search(IPdfCache obj)
        {
            IPdfCache result = null;

            List<object> group = GetGroup(obj);
            if (group == null)
            {
                group = CreateNewGroup();
            }
            else if (group.Count > 0)
            {
                result = (IPdfCache)group[0];
            }

            group.Add(obj);

            return result;
        }

        /// <summary>
        /// Checks whether a cache contains a group of such objects.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>true if contains, False otherwise.</returns>
        public bool Contains(IPdfCache obj)
        {
            bool contains = false;

            if (obj != null)
            {
                contains = (GetGroup(obj) != null);
            }

            return contains;
        }

        /// <summary>
        /// Returns number of cached object in a group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns number of cached object in a group if found, 0 otherwise.</returns>
        public int GroupCount(IPdfCache obj)
        {
            int count = 0;
            if (obj != null)
            {
                List<object> group = GetGroup(obj);
                if (group != null)
                {
                    count = group.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Removes the object from a cache.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void Remove(IPdfCache obj)
        {
            if (obj != null)
            {
                List<object> group = GetGroup(obj);
                if (group != null)
                {
                    group.Remove(obj);
                    if (group.Count == 0)
                    {
                        RemoveGroup(group);
                    }
                }
            }
        }

        /// <summary>
        /// Cleares cache.
        /// </summary>
        public void Clear()
        {
            if (m_referenceObjects != null)
            {
                for (int i = 0, len = m_referenceObjects.Count; i < len; i++)
                {
                    List<object> group = (List<object>)m_referenceObjects[i];
                    group.Clear();
                }

                m_referenceObjects.Clear();
            }

#if !SILVERLIGHT && !NETFX_CORE && !WP
           if(m_fontOffsets != null)
                m_fontOffsets.Clear();

            if(m_fontData != null)
                m_fontData.Clear();
#endif
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <returns>A created group.</returns>
        private List<object> CreateNewGroup()
        {
            List<object> group = new List<object>();
            m_referenceObjects.Add(group);

            return group;
        }

        /// <summary>
        /// Searches for a corresponding group.
        /// </summary>
        /// <param name="result">A representative of a group.</param>
        /// <returns>A group if found, Null otherwise.</returns>
        private List<object> GetGroup(IPdfCache result)
        {
            List<object> group = null;

            if (result != null)
            {
                for (int i = 0, len = m_referenceObjects.Count; i < len; i++)
                {
                    List<object> tGroup = m_referenceObjects[i];
                    if (tGroup.Count > 0)
                    {
                        IPdfCache representative = (IPdfCache)tGroup[0];

                        if (result.EqualsTo(representative))
                        {
                            group = tGroup;
                            break;
                        }
                    }
                    else
                    {
                        RemoveGroup(tGroup);
                    }
                }
            }

            return group;
        }

        /// <summary>
        /// Remove a group from the storage.
        /// </summary>
        /// <param name="group">A group of the objects.</param>
        private void RemoveGroup(List<object> group)
        {
            if (group != null)
            {
                m_referenceObjects.Remove(group);
            }
        }
        #endregion
    }
}
