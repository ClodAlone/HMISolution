//-------------------------------------------------------------------------------------------------
// <copyright file="CollectionWrapper.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;
using System.ComponentModel;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// This is a wrapper around <see cref="System.Collections.ArrayList"/>. It is used to hold a set of non-generic collections.
    /// </summary>
    public class CollectionWrapper : IList, ITypedList
    {
        ICollection inner;
        ArrayList cache;
        IEnumerator enumerator;

        /// <summary>
        /// Constructor for CollectionWrapper.
        /// </summary>
        /// <param name="inner">The collection.</param>
        public CollectionWrapper(ICollection inner)
        {
            this.inner = inner;
            this.cache = new ArrayList();
        }

        #region IList Members

        /// <summary>
        /// Adds new item.
        /// </summary>
        /// <param name="value">Item value.</param>
        /// <returns>Returns -1.</returns>
        public int Add(object value)
        {
            return -1;
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Determines whether the list contains the specified item.
        /// </summary>
        /// <param name="value">The Item value.</param>
        /// <returns>True if it contains the item specified; False otherwise.</returns>
        public bool Contains(object value)
        {
            return cache.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the given item in the list.
        /// </summary>
        /// <param name="value">Item value.</param>
        /// <returns>Item Index.</returns>
        public int IndexOf(object value)
        {
            return cache.IndexOf(value);
        }

        /// <summary>
        /// Inserts a value into the list at the specified index.
        /// </summary>
        /// <param name="index">The Index.</param>
        /// <param name="value">The Value.</param>
        public void Insert(int index, object value)
        {
        }

        /// <summary>
        /// Determines if the list size is fixed. Returns true.
        /// </summary>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Specifies if the list is read-only. Returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Removes the specified value from the list.
        /// </summary>
        /// <param name="value">The object value</param>
        public void Remove(object value)
        {
        }

        /// <summary>
        /// Removes the value at the specified index.
        /// </summary>
        /// <param name="index">The Index.</param>
        public void RemoveAt(int index)
        {
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The Index.</param>
        public object this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index", "maximum value is " + Count.ToString());
                }

                if (enumerator == null)
                {
                    enumerator = inner.GetEnumerator();
                }

                while (index >= cache.Count)
                {
                    enumerator.MoveNext();
                    cache.Add(enumerator.Current);
                }

                return cache[index];
            }

            set
            {
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array.
        /// </summary>
        /// <param name="array">One-dimensional array.</param>
        /// <param name="index">Start index.</param>
        public void CopyTo(Array array, int index)
        {
            object obj = this[Count - 1];
            cache.CopyTo(array, index);
        }

        /// <summary>
        /// Gets number of elements in the list.
        /// </summary>
        public int Count
        {
            get { return inner.Count; }
        }

        /// <summary>
        /// Gets a value that indicates whether access to this collection is synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Collection Wrapper enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return new CollectionWrapperEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Serves as enumerator for <see cref="CollectionWrapper"/>.
        /// </summary>
        public class CollectionWrapperEnumerator : IEnumerator
        {
            int _cursor = -1, _next = -1;
            CollectionWrapper _coll;

            /// <summary>
            /// Initalizes the enumerator and attaches it to the collection.
            /// </summary>
            /// <param name="collection">The parent collection to enumerate.</param>
            public CollectionWrapperEnumerator(CollectionWrapper collection)
            {
                _coll = collection;
                _next = _coll.Count > 0 ? 0 : -1;
            }

            #region IEnumerator Members

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public virtual void Reset()
            {
                _cursor = -1;
                _next = _coll.Count > 0 ? 0 : -1;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public object Current
            {
                get
                {
                    return _coll[_cursor];
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                if (_next == -1 || _next >= _coll.Count)
                {
                    return false;
                }

                _cursor = _next;

                _next++;
                if (_next >= _coll.Count)
                {
                    _next = -1;
                }

                return _cursor != -1;
            }
            #endregion
        }

        #region ITypedList Members

        /// <summary>
        /// Returns the <see cref="PropertyDescriptorCollection"/> that represents the properties on each item used to bind data. 
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="PropertyDescriptor"/> objects to find in the collection as bindable.</param>
        /// <returns>The <see cref="PropertyDescriptorCollection"/>.</returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection pdc = Syncfusion.Collections.ListUtil.GetItemProperties(inner);
            if (pdc.Count == 0 && Count > 0)
            {
                ICustomTypeDescriptor ctp = this[0] as ICustomTypeDescriptor;
                if (ctp != null)
                {
                    pdc = ctp.GetProperties(null);
                }
            }
            ////  pdc = Syncfusion.Collections.ListUtil.GetItemProperties(this[0]);
            return pdc;
        }

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="PropertyDescriptor"/> objects, for which the list name is returned.</param>
        /// <returns>The list name.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            if (inner is ITypedList)
            {
                return ((ITypedList)inner).GetListName(listAccessors);
            }

            return string.Empty;
        }
        #endregion
    }
}
