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

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a subset from collection of specified type entities.
    /// </summary>
    public abstract class EntitySubsetCollection : IEntityCollectionBase
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private EntityCollection m_coll;
        private EntityType m_type;
        private int m_lastIndex = -1;
        private int m_lastBaseIndex = -1;
        private int m_count = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public WordDocument Document
        {
            get
            {
                return m_coll.Document;
            }
        }
        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public Entity Owner
        {
            get
            {
                return m_coll.Owner;
            }
        }
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_count;
            }
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Entity"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Entity this[int index]
        {
            get
            {
                if (m_coll.Count < 1)
                    throw new ArgumentOutOfRangeException("index");
                ClearIndexes();
                return GetByIndex(index);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySubsetCollection"/> class.
        /// </summary>
        /// <param name="coll">The collection.</param>
        /// <param name="type">The type.</param>
        internal EntitySubsetCollection(EntityCollection coll, EntityType type)
        {
            m_coll = coll;
            m_type = type;
            UpdateCount();
            coll.ChangeItemsHandlers.Add(new EntityCollection.ChangeItems(BaseCollChangeItems));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Removes all entities 
        /// </summary>
        public void Clear()
        {
            m_coll.InternalClearBy(m_type);
            m_count = 0;
            m_lastIndex = -1;
            m_lastBaseIndex = -1;
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new SubSetEnumerator(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the entity to the end of collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal int InternalAdd(Entity entity)
        {
            CheckType(entity);
            m_coll.Add(entity);

            return m_count - 1;
        }
        /// <summary>
        /// Determines whether a entity is in the collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal bool InternalContains(Entity entity)
        {
            CheckType(entity);
            return m_coll.Contains(entity);
        }
        /// <summary>
        /// Returns the zero-based index of the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal int InternalIndexOf(Entity entity)
        {
            CheckType(entity);
            ClearIndexes();
            for (int i = 0; i < Count; i++)
            {
                if (GetByIndex(i) == entity)
                {
                    return i;
                }
            }

            return -1;
        }
        /// <summary>
        /// Inserts a entity into the collection at the specified index. 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal int InternalInsert(int index, Entity entity)
        {
            int bIndex = GetBaseIndex(index);
            m_coll.Insert(index, entity);
            bIndex++;
            return bIndex;
        }
        /// <summary>
        /// Removes the entity from the collection.
        /// </summary>
        /// <param name="entity"></param>
        internal void InternalRemove(Entity entity)
        {
            CheckType(entity);
            m_coll.Remove(entity);
        }
        /// <summary>
        /// Removes the entity at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void InternalRemoveAt(int index)
        {
            int bIndex = GetBaseIndex(index);
            m_coll.RemoveAt(bIndex);
        }
        /// <summary>
        /// Gets the entity by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected Entity GetByIndex(int index)
        {
            //m_lastBaseIndex - index from the textbodycollection
            //m_lastIndex - index from the innercollection
            if (m_lastBaseIndex < 0 || index == m_lastIndex)
            {
                m_lastBaseIndex = GetBaseIndex(index);
                m_lastIndex = index;
            }
            else
            {
                bool direct = m_lastIndex < index;

                while (index != m_lastIndex)
                {
                    m_lastBaseIndex = m_coll.GetNextOrPrevIndex(m_lastBaseIndex, m_type, direct);
                    m_lastIndex += direct ? 1 : -1;
                }
            }
            return m_coll[m_lastBaseIndex];
        }
        /// <summary>
        /// Gets the base index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private int GetBaseIndex(int index)
        {
            int j = 0;

            for (int i = 0, length = m_coll.Count; i < length; i++)
            {
                Entity en = (m_coll as IEntityCollectionBase)[i];

                if (en.EntityType == m_type)
                {
                    if (j == index)
                        return i;

                    j++;
                }
            }

            return -1;
        }
        /// <summary>
        /// Updates the count.
        /// </summary>
        private void UpdateCount()
        {
            int lastIndex = -1;
            m_count = 0;

            while (true)
            {
                lastIndex = m_coll.GetNextOrPrevIndex(lastIndex, m_type, true);

                if (lastIndex >= 0)
                {
                    m_count++;
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Checks the type.
        /// </summary>
        private void CheckType(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.EntityType != m_type)
                throw new ArgumentException("Invalid entity type");
        }
        /// <summary>
        /// Calls when in base collection changed items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="entity">The entity.</param>
        private void BaseCollChangeItems(EntityCollection.ChangeItemsType type, Entity entity)
        {
            switch (type)
            {
                case EntityCollection.ChangeItemsType.Add:
                    if (entity.EntityType == m_type)
                    {
                        m_count++;
                    }
                    break;
                case EntityCollection.ChangeItemsType.Remove:
                    if (entity.EntityType == m_type)
                    {
                        m_count--;
                    }
                    break;
                case EntityCollection.ChangeItemsType.Clear:
                    m_count = 0;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Clears the indexes.
        /// </summary>
        internal void ClearIndexes()
        {
            m_lastIndex = -1;
            m_lastBaseIndex = -1;
        }
        #endregion

        #region Internal declaration
        /// <summary>
        /// Represents a internal enumerator for EntitySubSetCollection. 
        /// </summary>
        public class SubSetEnumerator : IEnumerator
        {
            #region Fields
            private int m_currIndex = -1;
            private EntitySubsetCollection m_enColl;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="SubSetEnumerator"/> class.
            /// </summary>
            /// <param name="enColl">The entities collection.</param>
            public SubSetEnumerator(EntitySubsetCollection enColl)
            {
                m_enColl = enColl;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            public object Current
            {
                get
                {
                    if (m_currIndex < 0)
                        return null;
                    else
                        return m_enColl.m_coll[m_currIndex];
                }
            }
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                int nextIndex = m_enColl.m_coll.GetNextOrPrevIndex(m_currIndex, m_enColl.m_type, true);

                if (nextIndex < 0)
                    return false;

                m_currIndex = nextIndex;
                return true;
            }
            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_currIndex = -1;
            }
            #endregion
        }
        #endregion
    }
}
