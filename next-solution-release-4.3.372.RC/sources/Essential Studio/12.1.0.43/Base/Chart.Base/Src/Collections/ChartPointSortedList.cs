#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// 
    /// </summary>
    [DocumentationExclude]
    sealed class ChartPointSortedList : ICollection, IEnumerable
    {
        #region Constants
        private const int c_nDEFAULT_ARRAY_SIZE = 32;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_nCapacity;
        /// <summary>
        /// Store array of ChartPointWithIndexs
        /// </summary>
        private ChartPointWithIndex[] m_array;
        /// <summary>
        /// Store count of ChartPointWithIndexs
        /// </summary>
        private int m_count;
        #endregion

        #region Class initialize methods
        /// <summary>
        /// Creates instance of the ChartPointWithIndexArray.
        /// </summary>
        public ChartPointSortedList()
            : this(c_nDEFAULT_ARRAY_SIZE)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointSortedList"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ChartPointSortedList(int capacity)
        {
            m_nCapacity = capacity;
            m_array = new ChartPointWithIndex[m_nCapacity];
            m_count = 0;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Add ChartPointWithIndex to collection
        /// </summary>
        /// <param name="item">ChartPointWithIndex to add</param>
        /// <returns>Index of added ChartPointWithIndex</returns>
        public int Add(ChartPointWithIndex item)
        {
            this.ProvideSpaceFor(1);

            int index = GetInsertIndex(item, 0, Count);

            if (index < 0)
            {
                throw new IndexOutOfRangeException("Insert position");
            }

            Insert(index, item);

            return m_count++;
        }

        /// <summary>
        /// Clear collection.
        /// </summary>
        public void Clear()
        {
            m_array = new ChartPointWithIndex[m_nCapacity];
            m_count = 0;
        }
        /// <summary>
        /// Gets value indicates is ChartPointWithIndex in collection
        /// </summary>
        /// <param name="point">ChartPointWithIndex to check</param>
        /// <returns>TRUE in point belongs to collection otherwise FALSE.</returns>
        public bool Contains(ChartPointWithIndex point)
        {
            return (this.IndexOf(point) != -1);
        }
        /// <summary>
        /// Remove point by its index in collection
        /// </summary>
        /// <param name="index">Index of point</param>
        public void RemoveAt(int index)
        {
            this.ValidateIndex(index);
            m_count--;
            Array.Copy(m_array, index + 1, m_array, index, m_count - index);
        }
        /// <summary>
        /// Gets or sets collection capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return m_array.Length;
            }
            set
            {
                if (value < m_count)
                {
                    value = m_count;
                }
                if (value < m_nCapacity)
                {
                    value = m_nCapacity;
                }
                if (m_array.Length != value)
                {
                    ChartPointWithIndex[] temp = new ChartPointWithIndex[value];
                    Array.Copy(m_array, 0, temp, 0, m_count);
                    m_array = temp;
                }
            }
        }
        /// <summary>
        /// Gets or sets ChartPointWithIndex by its index.
        /// </summary>
        public ChartPointWithIndex this[int index]
        {
            get
            {
                return m_array[index];
            }
            set
            {
                m_array[index] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChartPointWithIndex[] ToArray()
        {
            ChartPointWithIndex[] array = new ChartPointWithIndex[m_count];
            Array.Copy(m_array, array, m_count);
            return array;
        }
        #endregion

        #region IEnumerator implementation
        /// <summary>
        /// Gets IEnumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        #region ICollection implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        void ICollection.CopyTo(Array array, int start)
        {
            if (m_count > ((array.GetUpperBound(0) + 1) - start))
            {
                throw new ArgumentException("Destination array was not long enough.");
            }

            Array.Copy(m_array, 0, array, start, m_count);
        }
        /// <summary>
        /// Gets value indicates that collection is synchronized.
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return m_array.IsSynchronized;
            }
        }
        /// <summary>
        /// Gets SyncRoot.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return m_array.SyncRoot;
            }
        }
        /// <summary>
        /// Gets count of elements in collection.
        /// </summary>
        public int Count
        {
            get
            {
                return m_count;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets index of point in collection.
        /// </summary>
        /// <param name="item">Node to get index</param>
        /// <returns>Index of the point in collection. If point collection doesn't constrain point return -1</returns>
        private int IndexOf(ChartPointWithIndex item)
        {
            for (int i = 0; i < m_count; i++)
            {
                if (m_array[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void Insert(int index, ChartPointWithIndex value)
        {
            this.ProvideSpaceFor(1);

            if (index < Count)
            {
                Array.Copy(m_array, index, m_array, index + 1, Count - index);
            }

            m_array[index] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private int GetInsertIndex(ChartPointWithIndex point, int start, int end)
        {
            if (m_array == null)
            {
                throw new ArgumentNullException("array");
            }

            if ((start < 0) || (end < 0))
            {
                throw new ArgumentOutOfRangeException();
            }

            if ((Count - start) < end)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }

            int left = start;
            int right = (start + Count) - 1;

            while (left <= right)
            {
                int middle = (left + right) >> 1;

                if (m_array[middle].Point.X < point.Point.X)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return left;
        }
        /// <summary>
        /// Checks if collection can constrain Count + nItems. If not
        /// increase collection by collection capacity.
        /// </summary>
        /// <param name="nItems">Count of points to check collection size</param>
        private void ProvideSpaceFor(int nItems)
        {
            if ((m_count + nItems) >= this.Capacity)
            {
                if ((m_count + nItems) < (2 * this.Capacity))
                {
                    this.Capacity *= 2;
                }
                else
                {
                    this.Capacity = m_count + nItems;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void ValidateIndex(int index)
        {
            if ((index < 0) || (index >= m_count))
            {
                throw new ArgumentOutOfRangeException("Index was out of range.  Must be non-negative and less than the size of the collection.", index, "Specified argument was out of the range of valid values.");
            }
        }
        #endregion

        #region Class internal declarations
        /// <summary>
        /// 
        /// </summary>
        public class Enumerator : IEnumerator
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private ChartPointSortedList m_collection;
            /// <summary>
            /// 
            /// </summary>
            private int m_index;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="tc"></param>
            public Enumerator(ChartPointSortedList tc)
            {
                m_collection = tc;
                m_index = -1;
            }
            #endregion

            #region IEnumerator implementation
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                m_index++;
                return (m_index < m_collection.Count);
            }

            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                m_index = -1;
            }

            /// <summary>
            /// 
            /// </summary>
            public ChartPointWithIndex Current
            {
                get
                {
                    return m_collection[m_index];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
            #endregion
        }
        #endregion
    }
}
