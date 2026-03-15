#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides data for the <see cref="ChartListChangeHandler"/> delegate.
    /// </summary>
    public class ChartListChangeArgs : EventArgs
    {
        #region Members
        private int m_index;
        private object[] m_oldItems;
        private object[] m_newItems;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of the items affected by the change.
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets array of items has removed from list.
        /// </summary>
        public object[] OldItems
        {
            get
            {
                return m_oldItems;
            }
        }

        /// <summary>
        /// Gets array of items has added to list.
        /// </summary>
        public object[] NewItems
        {
            get
            {
                return m_newItems;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartListChangeArgs"/> class. 
        /// </summary>
        /// <param name="index">Index of the items affected by the change.</param>
        /// <param name="oldItems">Array of items has added to list.</param>
        /// <param name="newItems">Array of items has removed from list.</param>
        public ChartListChangeArgs(int index, object[] oldItems, object[] newItems)
        {
            m_index = index;
            m_oldItems = oldItems;
            m_newItems = newItems;
        }
        #endregion
    }

    /// <summary>
    /// Represents the method that will handle an event that has <see cref="ChartListChangeArgs"/> data. 
    /// </summary>
    /// <param name="list">The source of the event.</param>
    /// <param name="args">Instance of <see cref="ChartListChangeArgs"/> class.</param>
    public delegate void ChartListChangeHandler(ChartBaseList list, ChartListChangeArgs args);

    /// <summary>
    /// Provides the base class for a strongly typed collection. 
    /// This class has <see cref="ChartBaseList.Changed"/> event.
    /// </summary>
    public class ChartBaseList : IList
    {
        #region Helper classes
        /// <summary>
        /// Internal enumerator for <see cref="ChartBaseList"/> class.
        /// </summary>
        public class ChartBaseEnumerator : IEnumerator
        {
            #region Members
            private int m_currIndex = -1;
            private ChartBaseList m_list;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the current element in the collection. 
            /// </summary>
            public object Current
            {
                get
                {
                    return m_list.List[m_currIndex];
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the ChartBaseEnumerator class.
            /// </summary>
            /// <param name="list">Instance of <see cref="ChartBaseList"/> class.</param>
            public ChartBaseEnumerator(ChartBaseList list)
            {
                m_list = list;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                return ++m_currIndex < m_list.Count;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection. 
            /// </summary>
            public void Reset()
            {
                m_currIndex = -1;
            }
            #endregion
        }
        #endregion

        #region Constants
        private const int DEF_ITEMS_SIZE = 0x04;
        #endregion

        #region Members
        private bool m_freezeEvent = false;
        private int m_count = 0;
        private object[] m_items;
        private object m_syncRoot = null;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the collection is changed.
        /// </summary>
        public event ChartListChangeHandler Changed;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChartBaseList.Changed"/> will be raised.
        /// </summary>
        protected bool FreezeEvent
        {
            get
            {
                return m_freezeEvent;
            }

            set
            {
                m_freezeEvent = value;
            }
        }

        /// <summary>
        /// Gets an IList containing the list of elements in the ChartBaseList instance.
        /// </summary>
        protected IList List
        {
            get
            {
                return this as IList;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ChartBaseList has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ChartBaseList is read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ChartBaseList is synchronized (thread safe). 
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ChartBaseList.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                if (m_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref m_syncRoot, new object(), null);
                }

                return m_syncRoot;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the ChartBaseList instance. This property cannot be overridden.
        /// </summary>
        public int Count
        {
            get
            {
                return m_count;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ChartBaseList class that is empty and has the default initial capacity. 
        /// </summary>
        public ChartBaseList()
        {
            m_items = new object[DEF_ITEMS_SIZE];
        }

        /// <summary>
        /// Initializes a new instance of the ChartBaseList class that is empty and has the specified initial capacity. 
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ChartBaseList(int capacity)
        {
            m_items = new object[capacity];
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Removes all items from the ChartBaseList.
        /// </summary>
        public void Clear()
        {
            object[] objs = new object[m_count];
            Array.Copy(m_items, 0, objs, 0, m_count);

            for (int i = 0; i < m_count; i++)
            {
                m_items[i] = null;
            }

            m_count = 0;

            OnChanged(new ChartListChangeArgs(0, objs, null));
        }

        /// <summary>
        /// Removes the ChartBaseList item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < m_count)
            {
                ChartListChangeArgs args = new ChartListChangeArgs(index, new object[] { m_items[index] }, null);
                NativeRemoveAt(index);
                OnChanged(args);
            }
        }

        /// <summary>
        /// Copies the entire ChartBaseList to a compatible one-dimensional Array, starting at the specified index of the target array. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ChartBaseList. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            Array.Copy(m_items, index, array, 0, m_count - index);
        }

        /// <summary>
        /// Sorts the elements in the entire ChartBaseList using the specified comparer
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer"/> implementation to use when comparing elements.</param>
        public void Sort(IComparer comparer)
        {
            Array.Sort(m_items, 0, m_count, comparer);
        }

        /// <summary>
        /// Copies the elements of the ChartBaseList to a new <see cref="Object"/> array.
        /// </summary>
        /// <returns>An <see cref="Object"/> array containing copies of the elements of the ChartBaseList.</returns>
        public object[] ToArray()
        {
            object[] result = new object[m_count];
            Array.Copy(m_items, 0, result, 0, m_count);
            return result;
        }

        /// <summary>
        /// Copies the elements of the ChartBaseList to a new array of the specified element type. 
        /// </summary>
        /// <param name="type">The element <see cref="Type"/> of the destination array to create and copy elements to.</param>
        /// <returns>An array of the specified element type containing copies of the elements of the ChartBaseList.</returns>
        public Array ToArray(Type type)
        {
            Array result = Array.CreateInstance(type, m_count);
            Array.Copy(m_items, 0, result, 0, m_count);
            return result;
        }

        /// <summary>
        /// Returns an enumerator for the entire ChartBaseList. 
        /// </summary>
        /// <returns>An IEnumerator for the entire ChartBaseList.</returns>
        public IEnumerator GetEnumerator()
        {
            return new ChartBaseEnumerator(this);
        }
        #endregion

        #region IList implementation
        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= m_count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return m_items[index];
            }

            set
            {
                if (this.Validate(value))
                {
                    if (index < 0 || index >= m_count)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    if (m_items[index] != value)
                    {
                        ChartListChangeArgs args = new ChartListChangeArgs(index, new object[] { m_items[index] }, new object[] { value });
                        m_items[index] = value;
                        OnChanged(args);
                    }
                }
            }
        }

        /// <summary>
        /// Adds an item to the ChartBaseList.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to add to the ChartBaseList.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        int IList.Add(object value)
        {
            int index = -1;

            if (this.Validate(value))
            {
                index = m_count;
                ChartListChangeArgs args = new ChartListChangeArgs(index, null, new object[] { value });
                NativeInsert(index, value);
                OnChanged(args);
            }

            return index;
        }

        /// <summary>
        /// Inserts an item to the ChartBaseList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The <see cref="Object"/> to insert into the ChartBaseList.</param>
        void IList.Insert(int index, object value)
        {
            if (this.Validate(value))
            {
                if (index <= m_count)
                {
                    ChartListChangeArgs args = new ChartListChangeArgs(index, null, new object[] { value });
                    NativeInsert(index, value);
                    OnChanged(args);
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the ChartBaseList.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to remove from the ChartBaseList. </param>
        void IList.Remove(object value)
        {
            int index = Array.IndexOf(m_items, value, 0, m_count);

            if (index >= 0 && index < m_count)
            {
                ChartListChangeArgs args = new ChartListChangeArgs(index, new object[] { value }, null);
                NativeRemoveAt(index);
                OnChanged(args);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the ChartBaseList.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to locate in the ChartBaseList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        int IList.IndexOf(object value)
        {
            return Array.IndexOf(m_items, value, 0, m_count);
        }

        /// <summary>
        /// Determines whether the ChartBaseList contains a specific value. 
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to locate in the ChartBaseList.</param>
        /// <returns>true if the <see cref="Object"/> is found in the IList; otherwise, false</returns>
        bool IList.Contains(object value)
        {
            return Array.IndexOf(m_items, value, 0, m_count) != -1;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs additional custom processes when validating a value
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>If is true, value is approved.</returns>
        protected virtual bool Validate(object obj)
        {
            return true;
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="args">Argument.</param>
        protected virtual void OnChanged(ChartListChangeArgs args)
        {
            if (!m_freezeEvent && Changed != null)
            {
                Changed(this, args);
            }
        }

        /// <summary>
        /// Inserts an item to the internal array at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted. </param>
        /// <param name="obj">The <see cref="Object"/> to insert into the internal array.</param>
        private void NativeInsert(int index, object obj)
        {
            m_count++;

            if (m_count > m_items.Length)
            {
                object[] items = m_items;
                m_items = new object[2 * m_items.Length];
                Array.Copy(items, 0, m_items, 0, items.Length);
            }

            if (index < m_count)
            {
                for (int i = m_count - 1; i > index; )
                {
                    m_items[i] = m_items[--i];
                }
            }

            m_items[index] = obj;
        }

        /// <summary>
        /// Removes an the internal array item at the specified index. 
        /// </summary>
        /// <param name="index">Index of item.</param>
        private void NativeRemoveAt(int index)
        {
            if (index != m_count)
            {
                for (int i = index, c = m_count - 1; i < c; )
                {
                    m_items[i] = m_items[++i];
                }
            }

            m_count--;
            m_items[m_count] = null;
        }
        #endregion
    }
}
