// <copyright file="SearchNodeArray.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region File using derectives

using System;
using System.Collections;
using Syncfusion.Windows.Diagram;

#endregion

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Collection of search nodes.
    /// </summary>    
    internal sealed class SearchNodeArray : ICollection, IEnumerable
    {
        #region Constants
        private const int c_nDEFAULT_ARRAY_SIZE = 32;
        #endregion

        #region Class members
        private int m_nCapacity;

        /// <summary>
        /// Store array of SearchNodes
        /// </summary>
        private SearchNode[] m_array;

        /// <summary>
        /// Store count of SearchNodes
        /// </summary>
        private int m_count;
        private PriorityComparer m_comparer;
        #endregion

        #region Class initialize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchNodeArray"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public SearchNodeArray(SearchNode node)
            : this(node, c_nDEFAULT_ARRAY_SIZE)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchNodeArray"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="capacity">The capacity.</param>
        public SearchNodeArray(SearchNode node, int capacity)
        {
            if (node != null)
            {
                m_comparer = new PriorityComparer(node);
            }
            else
            {
                m_comparer = null;
            }

            m_nCapacity = capacity;
            this.m_array = new SearchNode[m_nCapacity];
            this.m_count = 0;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Add SearchNode to collection
        /// </summary>
        /// <param name="item">SearchNode to add</param>
        /// <returns>Index of added SearchNode</returns>
        public int Add(SearchNode item)
        {
            this.ProvideSpaceFor(1);

            if (m_comparer != null)
            {
                int index = GetInsertIndex(item, 0, Count);

                if (index < 0)
                {
                    throw new IndexOutOfRangeException("Insert position");
                }

                Insert(index, item);
            }
            else
            {
                this.m_array[this.m_count] = item;
            }

            return this.m_count++;
        }

        /// <summary>
        /// Clear collection.
        /// </summary>
        public void Clear()
        {
            this.m_array = new SearchNode[m_nCapacity];
            this.m_count = 0;
        }

        /// <summary>
        /// Gets value indicates is SearchNode in collection
        /// </summary>
        /// <param name="node">SearchNode to check</param>
        /// <returns>TRUE in node belongs to collection otherwise FALSE.</returns>
        public bool Contains(SearchNode node)
        {
            return (this.IndexOf(node) != -1);
        }

        /// <summary>
        /// Remove node by its index in collection
        /// </summary>
        /// <param name="index">Index of node</param>
        public void RemoveAt(int index)
        {
            this.ValidateIndex(index);
            this.m_count--;
            Array.Copy(this.m_array, index + 1, this.m_array, index, this.m_count - index);
        }

        /// <summary>
        /// Gets or sets collection capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.m_array.Length;
            }
            set
            {
                if (value < this.m_count)
                {
                    value = this.m_count;
                }
                if (value < m_nCapacity)
                {
                    value = m_nCapacity;
                }
                if (this.m_array.Length != value)
                {
                    SearchNode[] temp = new SearchNode[value];
                    Array.Copy(this.m_array, 0, temp, 0, this.m_count);
                    this.m_array = temp;
                }
            }
        }

        /// <summary>
        /// Gets or sets SearchNode by its index.
        /// </summary>
        /// <param name="index">The index.</param>
        public SearchNode this[int index]
        {
            get
            {
                return this.m_array[index];
            }
            set
            {
                this.m_array[index] = value;
            }
        }
        #endregion

        #region IEnumerator implementation
        /// <summary>
        /// Gets IEnumerator.
        /// </summary>
        /// <returns>The IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        #region ICollection implementation
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="start">The start.</param>
        void ICollection.CopyTo(Array array, int start)
        {
            if (this.m_count > ((array.GetUpperBound(0) + 1) - start))
            {
                throw new ArgumentException("Destination array was not long enough.");
            }

            Array.Copy(this.m_array, 0, array, start, this.m_count);
        }

        /// <summary>
        /// Gets a value indicating whether collection is synchronized.
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return this.m_array.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets SyncRoot.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return this.m_array.SyncRoot;
            }
        }

        /// <summary>
        /// Gets count of elements in collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.m_count;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets index of node in collection.
        /// </summary>
        /// <param name="item">Node to get index</param>
        /// <returns>Index of the node in collection. If node collection doesn't constrain node return -1</returns>
        private int IndexOf(SearchNode item)
        {
            for (int i = 0; i < this.m_count; i++)
            {
                if (this.m_array[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Insert(int index, SearchNode value)
        {
            this.ProvideSpaceFor(1);

            if (index < Count)
            {
                Array.Copy(this.m_array, index, this.m_array, index + 1, Count - index);
            }

            m_array[index] = value;
        }

        /// <summary>
        /// Gets the index of the insert.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>The index to insert the node.</returns>
        private int GetInsertIndex(SearchNode node, int start, int end)
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

                if (m_comparer.Compare(m_array[middle], node) < 0)
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
        /// <param name="nItems">Count of nodes to check collection size</param>
        private void ProvideSpaceFor(int nItems)
        {
            if ((this.m_count + nItems) >= this.Capacity)
            {
                if ((this.m_count + nItems) < (2 * this.Capacity))
                {
                    this.Capacity *= 2;
                }
                else
                {
                    this.Capacity = this.m_count + nItems;
                }
            }
        }

        /// <summary>
        /// Validates the index.
        /// </summary>
        /// <param name="index">The index.</param>
        private void ValidateIndex(int index)
        {
            if ((index < 0) || (index >= m_count))
            {
                throw new ArgumentOutOfRangeException(); //"Index was out of range.  Must be non-negative and less than the size of the collection.", index, "Specified argument was out of the range of valid values.");
            }
        }
        #endregion

        #region Class internal declarations
        /// <summary>
        /// The node search enumerator.
        /// </summary>
        public class Enumerator : IEnumerator
        {
            #region Class members
            private SearchNodeArray m_collection;
            private int m_index;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> class.
            /// </summary>
            /// <param name="tc">The search node array.</param>
            public Enumerator(SearchNodeArray tc)
            {
                this.m_collection = tc;
                this.m_index = -1;
            }
            #endregion

            #region IEnumerator implementation
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                this.m_index++;
                return (m_index < m_collection.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                this.m_index = -1;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
            public SearchNode Current
            {
                get
                {
                    return this.m_collection[this.m_index];
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
            #endregion
        }

        /// <summary>
        /// Priority comparer.
        /// </summary>
        class PriorityComparer
        {
            #region Class members
            private SearchNode m_nodeEnd;
            #endregion

            #region Initialize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="PriorityComparer"/> class.
            /// </summary>
            /// <param name="nodeEnd">The node end.</param>
            public PriorityComparer(SearchNode nodeEnd)
            {
                if (nodeEnd == null) throw new ArgumentNullException("nodeEnd");

                m_nodeEnd = nodeEnd;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Compares the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>1, if x is greater than y.</returns>
            public int Compare(SearchNode x, SearchNode y)
            {
                if (x == null) throw new ArgumentNullException("x");
                if (y == null) throw new ArgumentNullException("y");

                double fF1 = x.GetMoveCost() + x.GetHeuristicCost(m_nodeEnd);
                double fF2 = y.GetMoveCost() + y.GetHeuristicCost(m_nodeEnd);
                int nReturnValue = 0;

                if (fF1 < fF2)
                {
                    nReturnValue = -1;
                }
                else if (fF1 > fF2)
                {
                    nReturnValue = 1;
                }

                return nReturnValue;
            }

            #endregion
        }
        #endregion
    }
}
