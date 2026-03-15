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

#if !SyncfusionFramework1_0 && !SyncfusionFramework1_1
#define Generics
#endif

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;

#if Generics
using System.Collections.Generic;
#endif
#endregion

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Optimized version of SortedList collection. Instead of keeping two
    /// arrays, one for keys and one for values, the values array on
    /// Hashtable collection are changed. Performance of this collection is better than that of SortedList.
    /// </summary>
    internal class SortedListEx :
        IDictionary,
        ICloneable
    {
        #region Constants
        /// <summary>
        /// Default capacity of internal buffers.
        /// </summary>
        private const int _defaultCapacity = 16;
        #endregion

        #region Fields
        /// <summary>
        /// Array which store keys in sorted order.
        /// </summary>
        private object[] keys;

        /// <summary>
        /// Collection stores values.
        /// </summary>
        private Dictionary<object,object> values;

        /// <summary>
        /// Size of collection.
        /// </summary>
        private int _size;

        /// <summary>
        /// Version of collection data.
        /// </summary>
        private int version;

        /// <summary>
        /// Default comparer for keys.
        /// </summary>
        private IComparer comparer;

        /// <summary>
        /// List of keys.
        /// </summary>
        private KeyList keyList;

        /// <summary>
        /// List of values.
        /// </summary>
        private ValueList valueList;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        public SortedListEx()
        {
            keys = new Object[_defaultCapacity];
            values = new Dictionary<object, object>(_defaultCapacity);
            //comparer = Comparer.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public SortedListEx(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException("initialCapacity");
            }

            keys = new Object[initialCapacity];
            values = new Dictionary<object,object>(initialCapacity);
            //comparer = Comparer.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public SortedListEx(IComparer comparer)
            : this()
        {
            if (comparer != null)
            {
                this.comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="capacity">The capacity.</param>
        public SortedListEx(IComparer comparer, int capacity)
            : this(comparer)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        public SortedListEx(IDictionary d)
            : this(d, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="comparer">The comparer.</param>
        public SortedListEx(IDictionary d, IComparer comparer)
            : this(comparer, (d != null ? d.Count : 0))
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }

            throw new NotImplementedException();
            // Create copy of dictionary values.
            d.Keys.CopyTo(keys, 0);
            //values = new Dictionary<object, object>(d);

            Array.Sort(keys, comparer);
            _size = d.Count;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public virtual int Capacity
        {
            get
            {
                return keys.Length;
            }

            set
            {
                if (value != keys.Length)
                {
                    if (value < _size)
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }

                    if (value > 0)
                    {
                        object[] newKeys = new object[value];

                        if (_size > 0)
                        {
                            Array.Copy(keys, 0, newKeys, 0, _size);
                        }

                        keys = newKeys;
                    }
                    else
                    {
                        keys = new object[_defaultCapacity];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public virtual int Count
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        public virtual ICollection Keys
        {
            get
            {
                return GetKeyList();
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        public virtual ICollection Values
        {
            get
            {
                return GetValueList();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object is read-only; otherwise, false.
        /// </returns>
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.
        /// </returns>
        public virtual bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        public virtual object this[object key]
        {
            get
            {
                return values[key];
            }

            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (values.ContainsKey(key))
                {
                    values[key] = value;
                }
                else
                {
                    Add(key, value);
                }

                version++;
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Returns a synchronized (thread-safe) wrapper for the SortedList.
        /// </summary>
        /// <param name="list">The SortedList to synchronize.</param>
        /// <returns>A synchronized (thread-safe) wrapper for the SortedList.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// When list is null.
        /// </exception>
        public static SortedListEx Synchronized(SortedListEx list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            return new SyncSortedListEx(list);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an element with the provided key and value to the list.
        /// </summary>
        /// <param name="key">The Object to use as the key of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        /// <exception cref="System.ArgumentNullException">
        /// When key is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// When list already contains specified key.
        /// </exception>
        public virtual void Add(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (values.ContainsKey(key))
            {
                throw new ArgumentException("Duplicated");
            }

            int index = Array.BinarySearch(keys, 0, _size, key, comparer);
            Insert(~index, key, value);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public virtual void Clear()
        {
            version++;
            _size = 0;

            keys = new Object[_defaultCapacity];
            values = new Dictionary<object, object>(_defaultCapacity);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the current instance.</returns>
        public virtual object Clone()
        {
            SortedListEx sl = new SortedListEx(_size);
            Array.Copy(keys, 0, sl.keys, 0, _size);
            sl.values = new Dictionary<object, object>(values);
            sl._size = _size;
            sl.version = version;
            sl.comparer = comparer;

            // Don't copy keyList or the valueList.
            return sl;
        }

        /// <summary>
        /// Clone current instance.
        /// </summary>
        /// <returns>Returns clone of current object.</returns>
        public SortedListEx CloneAll()
        {
            int iLen = Count;
            SortedListEx result = new SortedListEx(iLen + 1);

            for (int i = 0; i < iLen; i++)
            {
                object o = GetByIndex(i);

                o = ((ICloneable)o).Clone();
                result.Add(GetKey(i), o);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the list contains an element with the specified key.
        /// </summary>
        /// <param name="key">Key of the element to search.</param>
        /// <returns>True if list contains specified key.</returns>
        public virtual bool Contains(object key)
        {
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the list contains an element with the specified key.
        /// </summary>
        /// <param name="key">Key of the element to search.</param>
        /// <returns>True if list contains specified key.</returns>
        public virtual bool ContainsKey(object key)
        {
            // This is a SPEC'ed duplicate of Contains().
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the list contains the specified value.
        /// </summary>
        /// <param name="value">Value of the element to search.</param>
        /// <returns>True if list contains specified value.</returns>
        public virtual bool ContainsValue(object value)
        {
            return values.ContainsValue(value);
        }

        /// <summary>
        /// Copies all the elements of the SortedListEx to the specified one-dimensional Array
        /// starting at the specified destination Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the
        /// elements copied from the current list.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If specified array is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If rank of the array is not 1 or there are not enough elements.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If specified arrayIndex is less than zero.
        /// </exception>
        public virtual void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException();
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < Count; i++)
            {
                DictionaryEntry entry = new DictionaryEntry(keys[i], values[keys[i]]);
                array.SetValue(entry, i + arrayIndex);
            }
        }

        /// <summary>
        /// Gets the value at the specified index of the SortedListEx.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>The value at the specified index of the SortedListEx.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual object GetByIndex(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return values[keys[index]];
        }

        /// <summary>
        /// Gets the key at the specified index of the SortedListEx.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>The key at the specified index of the SortedListEx.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual object GetKey(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return keys[index];
        }

        /// <summary>
        /// Gets the keys in the SortedListEx.
        /// </summary>
        /// <returns>An IList containing the keys in the SortedListEx.</returns>
        public virtual IList GetKeyList()
        {
            if (keyList == null) keyList = new KeyList(this);
            {
                return keyList;
            }
        }

        /// <summary>
        /// Gets the values in the SortedListEx.
        /// </summary>
        /// <returns>An IList containing the values in the SortedListEx.</returns>
        public virtual IList GetValueList()
        {
            if (valueList == null)
            {
                valueList = new ValueList(this);
            }
            else
            {
                valueList.UpdateValues();
            }

            return valueList;
        }

        /// <summary>
        /// Returns the zero-based index of the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The zero-based index of key, if key is found; otherwise, -1.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// If specified key is null.
        /// </exception>
        public virtual int IndexOfKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            int ret = Array.BinarySearch(keys, 0, _size, key, comparer);

            return ret >= 0 ? ret : -1;
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified value.
        /// </summary>
        /// <param name="value">The value to locate (can be NULL).</param>
        /// <returns>
        /// The zero-based index of the first occurrence of value, if value is found;
        /// otherwise, -1.
        /// </returns>
        public virtual int IndexOfValue(object value)
        {
            object key = null;
            IDictionaryEnumerator enm = values.GetEnumerator();
            enm.Reset();

            while (enm.MoveNext())
            {
                if (enm.Value.Equals(value))
                {
                    key = enm.Key;
                    break;
                }
            }

            if (key == null) return -1;
            {
                return Array.IndexOf(keys, key, 0, _size);
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            _size--;
            object key = keys[index];
            if (index < _size)
            {
                Array.Copy(keys, index + 1, keys, index, _size - index);
            }
            keys[_size] = null;
            values.Remove(key);
            version++;
        }

        /// <summary>
        ///Removes the element with the specified key from SortedListEx.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public virtual void Remove(object key)
        {
            int i = IndexOfKey(key);
            if (i >= 0)
            {
                RemoveAt(i);
            }
        }

        /// <summary>
        /// Replaces the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index at which to save value.</param>
        /// <param name="value">The Object to save into. Can be NULL.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual void SetByIndex(int index, object value)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            values[keys[index]] = value;

            version++;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements.
        /// </summary>
        public virtual void TrimToSize()
        {
            Capacity = _size;
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator that can iterate through the SortedListEx.
        /// </summary>
        /// <returns>An IDictionaryEnumerator for the SortedListEx.</returns>
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return new SortedListExEnumerator(this, 0, _size, SortedListExEnumerator.DictEntry);
        }

        /// <summary>
        /// Returns an IEnumerator that can iterate through the SortedListEx.
        /// </summary>
        /// <returns>An IEnumerator for the SortedListEx.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedListExEnumerator(this, 0, _size, SortedListExEnumerator.DictEntry);
        }

        #endregion

        #region Private helper methods
        /// <summary>
        /// Inserts element with specified key and value at specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert element at.</param>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        private void Insert(int index, object key, object value)
        {
            if (_size == keys.Length)
            {
                EnsureCapacity(_size + 1);
            }

            if (index < _size)
            {
                Array.Copy(keys, index, keys, index + 1, _size - index);
            }

            keys[index] = key;
            values[key] = value;

            _size++;
            version++;
        }

        /// <summary>
        /// Ensures that the capacity of this instance is at least the specified value.
        /// </summary>
        /// <param name="min">The minimum capacity to ensure.</param>
        private void EnsureCapacity(int min)
        {
            int newCapacity = keys.Length == 0 ? 16 : keys.Length * 2;
            if (newCapacity < min)
            {
                newCapacity = min;
            }

            Capacity = newCapacity;
        }
        #endregion

        #region Internal classes declarations
        private class SyncSortedListEx : SortedListEx
        {
            #region Fields
            /// <summary>
            /// Wrapped SortedListEx.
            /// </summary>
            private SortedListEx _list;
            /// <summary>
            /// Sync object.
            /// </summary>
            private object _root;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="SyncSortedListEx"/> class.
            /// </summary>
            /// <param name="list">The list.</param>
            internal SyncSortedListEx(SortedListEx list)
            {
                _list = list;
                _root = list.SyncRoot;
            }
            #endregion

            #region Class Properties
            /// <summary>
            /// Capacity of internal buffers.
            /// </summary>
            public override int Capacity
            {
                get { lock (_root) { return _list.Capacity; } }
            }

            /// <summary>
            /// Size of the collection. Read-only.
            /// </summary>
            public override int Count
            {
                get { lock (_root) { return _list.Count; } }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public override object SyncRoot
            {
                get { return _root; }
            }

            /// <summary>
            /// Returns True if list is readonly, False otherwise. Read-only.
            /// </summary>
            public override bool IsReadOnly
            {
                get { return _list.IsReadOnly; }
            }

            /// <summary>
            /// Returns True if collection has fixed size, False otherwise.
            /// </summary>
            public override bool IsFixedSize
            {
                get { return _list.IsFixedSize; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public override bool IsSynchronized
            {
                get { return true; }
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            /// <value>key</value>
            public override object this[object key]
            {
                get
                {
                    lock (_root)
                    {
                        return _list[key];
                    }
                }

                set
                {
                    lock (_root)
                    {
                        _list[key] = value;
                    }
                }
            }

            #endregion

            #region Public Methods
            /// <summary>
            /// Adds an element with the provided key and value to the list.
            /// </summary>
            /// <param name="key">The Object to use as the key of the element to add.</param>
            /// <param name="value">The Object to use as the value of the element to add.</param>
            public override void Add(object key, object value)
            {
                lock (_root)
                {
                    _list.Add(key, value);
                }
            }

            /// <summary>
            /// Removes all the elements from the collection.
            /// </summary>
            public override void Clear()
            {
                lock (_root)
                {
                    _list.Clear();
                }
            }

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>A new object that is a copy of the current instance.</returns>
            public override object Clone()
            {
                lock (_root)
                {
                    return _list.Clone();
                }
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public override bool Contains(object key)
            {
                lock (_root)
                {
                    return _list.Contains(key);
                }
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public override bool ContainsKey(object key)
            {
                lock (_root)
                {
                    return _list.ContainsKey(key);
                }
            }

            /// <summary>
            /// Determines whether the list contains the specified value.
            /// </summary>
            /// <param name="value">Value of the element to search.</param>
            /// <returns>True if list contains specified value.</returns>
            public override bool ContainsValue(object value)
            {
                lock (_root)
                {
                    return _list.ContainsValue(value);
                }
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="index">The index in array at which copying begins.</param>
            public override void CopyTo(Array array, int index)
            {
                lock (_root)
                {
                    _list.CopyTo(array, index);
                }
            }

            /// <summary>
            /// Gets the value at the specified index of the list.
            /// </summary>
            /// <param name="index">The zero-based index of the value to get.</param>
            /// <returns>The value at the specified index of the SortedListEx.</returns>
            public override object GetByIndex(int index)
            {
                lock (_root)
                {
                    return _list.GetByIndex(index);
                }
            }

            /// <summary>
            /// Returns an IDictionaryEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IDictionaryEnumerator for the list.</returns>
            public override IDictionaryEnumerator GetEnumerator()
            {
                lock (_root)
                {
                    return _list.GetEnumerator();
                }
            }

            /// <summary>
            /// Gets the key at the specified index of the list.
            /// </summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at the specified index of the list.</returns>
            public override object GetKey(int index)
            {
                lock (_root)
                {
                    return _list.GetKey(index);
                }
            }

            /// <summary>
            /// Gets the keys in the list.
            /// </summary>
            /// <returns>An IList containing the keys in the list.</returns>
            public override IList GetKeyList()
            {
                lock (_root)
                {
                    return _list.GetKeyList();
                }
            }

            /// <summary>
            /// Gets the values in the list.
            /// </summary>
            /// <returns>An IList containing the values in the list.</returns>
            public override IList GetValueList()
            {
                lock (_root)
                {
                    return _list.GetValueList();
                }
            }

            /// <summary>
            /// Returns the zero-based index of the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The zero-based index of key, if key is found; otherwise, -1.</returns>
            public override int IndexOfKey(object key)
            {
                lock (_root)
                {
                    return _list.IndexOfKey(key);
                }
            }

            /// <summary>
            /// Returns the zero-based index of the first occurrence of the specified value.
            /// </summary>
            /// <param name="value">The value to locate (can be NULL).</param>
            /// <returns>
            /// The zero-based index of the first occurrence of value, if value is found;
            /// otherwise, -1.
            /// </returns>
            public override int IndexOfValue(object value)
            {
                lock (_root)
                {
                    return _list.IndexOfValue(value);
                }
            }

            /// <summary>
            /// Removes the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public override void RemoveAt(int index)
            {
                lock (_root)
                {
                    _list.RemoveAt(index);
                }
            }

            /// <summary>
            ///Removes the element with the specified key from list.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            public override void Remove(object key)
            {
                lock (_root)
                {
                    _list.Remove(key);
                }
            }

            /// <summary>
            /// Replaces the value at a specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public override void SetByIndex(int index, object value)
            {
                lock (_root)
                {
                    _list.SetByIndex(index, value);
                }
            }

            /// <summary>
            /// Sets the capacity to the actual number of elements.
            /// </summary>
            public override void TrimToSize()
            {
                lock (_root)
                {
                    _list.TrimToSize();
                }
            }
            #endregion
        }

        private class SortedListExEnumerator : IDictionaryEnumerator, ICloneable
        {
            #region Constants
            /// <summary>
            /// If it is assumed to getObjectRetType, Current will return key
            /// of the current element.
            /// </summary>
            internal const int Keys = 1;

            /// <summary>
            /// If it is assumed to getObjectRetType, Current will return value
            /// of the current element.
            /// </summary>
            internal const int Values = 2;

            /// <summary>
            /// If it is assumed to getObjectRetType, Current will return both -
            /// key and value (as DictionaryEntry).
            /// </summary>
            internal const int DictEntry = 3;
            #endregion

            #region Fields
            /// <summary>
            /// List for which is this enumerator.
            /// </summary>
            private SortedListEx SortedListEx;

            /// <summary>
            /// Key of the current element.
            /// </summary>
            private object key;

            /// <summary>
            /// Values of the current element.
            /// </summary>
            private object value;

            /// <summary>
            /// Index of current element.
            /// </summary>
            private int index;

            /// <summary>
            /// Starting index for the enumerator.
            /// </summary>
            private int startIndex;

            /// <summary>
            /// Ending index for this enumerator.
            /// </summary>
            private int endIndex;
            /// <summary>
            /// Version of collection data.
            /// </summary>
            private int version;

            /// <summary>
            /// True if current element is correct, False otherwise
            /// (before beginning or after end).
            /// </summary>
            private bool current;
            /// <summary>
            /// Specifies what should return method Current (Key, Value or both).
            /// </summary>
            private int getObjectRetType;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Creates enumerator for specified list, starting from specified index
            /// and with specified count.
            /// </summary>
            /// <param name="SortedListEx">list for which to create enumerator.</param>
            /// <param name="index">Starting index.</param>
            /// <param name="count">Number of elements to enumerate.</param>
            /// <param name="getObjRetType">Type of enumerating values (keys, value, DicEntry).</param>
            internal SortedListExEnumerator(SortedListEx SortedListEx, int index, int count,
                int getObjRetType)
            {
                this.SortedListEx = SortedListEx;
                this.index = index;
                startIndex = index;
                endIndex = index + count;
                version = SortedListEx.version;
                getObjectRetType = getObjRetType;
                current = false;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>Copy of the current instance.</returns>
            public object Clone()
            {
                return MemberwiseClone();
            }

            /// <summary>
            /// Gets the key of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The key of the current element of the enumeration.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of the dictionary or after the last entry.
            /// </exception>
            public virtual object Key
            {
                get
                {
                    if (version != SortedListEx.version)
                    {
                        throw new InvalidOperationException();
                    }

                    if (current == false)
                        throw new InvalidOperationException();

                    return key;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if the enumerator was successfully advanced to the next element;
            /// False if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">
            /// When the current version is not equal to the SortedListEx version.
            /// </exception>
            public virtual bool MoveNext()
            {
                if (version != SortedListEx.version)
                {
                    throw new InvalidOperationException();
                }

                if (index < endIndex)
                {
                    key = SortedListEx.keys[index];
                    value = SortedListEx.values[key];
                    index++;
                    current = true;
                    return true;
                }
                key = null;
                value = null;
                current = false;
                return false;
            }


            /// <summary>
            /// Gets both the key and the value of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:System.Collections.DictionaryEntry"/> containing both the key and the value of the current dictionary entry.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of the dictionary or after the last entry.
            /// </exception>
            public virtual DictionaryEntry Entry
            {
                get
                {
                    if (version != SortedListEx.version)
                    {
                        throw new InvalidOperationException();
                    }

                    if (current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return new DictionaryEntry(key, value);
                }
            }

            /// <summary>
            /// Gets The current element in the collection. Read-only.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            /// If current is false.
            /// </exception>
            public virtual object Current
            {
                get
                {
                    if (current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    if (getObjectRetType == Keys)
                    {
                        return key;
                    }
                    else if (getObjectRetType == Values)
                    {
                        return value;
                    }
                    else
                    {
                        return new DictionaryEntry(key, value);
                    }
                }
            }


            /// <summary>
            /// Gets the value of the current dictionary entry.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The value of the current element of the enumeration.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of the dictionary or after the last entry.
            /// </exception>
            public virtual object Value
            {
                get
                {
                    if (version != SortedListEx.version)
                    {
                        throw new InvalidOperationException();
                    }

                    if (current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return value;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public virtual void Reset()
            {
                if (version != SortedListEx.version)
                {
                    throw new InvalidOperationException();
                }

                index = startIndex;
                current = false;
                key = null;
                value = null;
            }
            #endregion
        }

        private class KeyList : IList
        {
            #region Fields
            /// <summary>
            /// List for which this collection was created.
            /// </summary>
            private SortedListEx SortedListEx;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Creates KeyList for specified SortedListEx.
            /// </summary>
            /// <param name="SortedListEx">SortedListEx for which KeyList must be created.</param>
            internal KeyList(SortedListEx SortedListEx)
            {
                this.SortedListEx = SortedListEx;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
            /// </returns>
            public virtual int Count
            {
                get { return SortedListEx._size; }
            }


            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
            /// </summary>
            /// <value></value>
            /// <returns>true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.
            /// </returns>
            public virtual bool IsReadOnly
            {
                get { return true; }
            }


            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
            /// </summary>
            /// <value></value>
            /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
            /// </returns>
            public virtual bool IsFixedSize
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public virtual bool IsSynchronized
            {
                get { return SortedListEx.IsSynchronized; }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public virtual object SyncRoot
            {
                get { return SortedListEx.SyncRoot; }
            }

            /// <summary>
            /// Adds an element with the provided key to the list.
            /// </summary>
            /// <param name="key">The Object to use as the key of the element to add.</param>
            public virtual int Add(object key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all elements from the collection.
            /// </summary>
            public virtual void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public virtual bool Contains(object key)
            {
                return SortedListEx.Contains(key);
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="arrayIndex">The index in array at which copying begins.</param>
            /// <exception cref="System.ArgumentException">
            /// If array is null or rank of the array is not 1.
            /// </exception>
            public virtual void CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException();

                // Defer error checking to Array.Copy.
                Array.Copy(SortedListEx.keys, 0, array, arrayIndex, SortedListEx.Count);
            }

            /// <summary>
            /// Insert the value at the specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public virtual void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the key at the specified index.
            /// </summary>
            public virtual object this[int index]
            {
                get
                {
                    return SortedListEx.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Returns an IEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IEnumerator for the list.</returns>
            public virtual IEnumerator GetEnumerator()
            {
                return new SortedListExEnumerator(SortedListEx, 0, SortedListEx.Count, SortedListExEnumerator.Keys);
            }

            /// <summary>
            /// Returns the zero-based index of the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The zero-based index of the key, if the key is found; otherwise, -1.</returns>
            public virtual int IndexOf(object key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                int i = Array.BinarySearch(SortedListEx.keys, 0,
                    SortedListEx.Count, key, SortedListEx.comparer);
                if (i >= 0) return i;
                return -1;
            }

            /// <summary>
            ///Removes the element with the specified key from list.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            public virtual void Remove(object key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the element at the specified index from the list.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public virtual void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
            #endregion
        }

        private class ValueList : IList
        {
            #region Fields
            /// <summary>
            /// List for which this collection was created.
            /// </summary>
            private SortedListEx SortedListEx;

            /// <summary>
            /// Array list that contain values.
            /// </summary>
            private Array vals;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates ValueList for specified SortedListEx.
            /// </summary>
            /// <param name="SortedListEx">SortedListEx for which ValueList must be created.</param>
            internal ValueList(SortedListEx SortedListEx)
            {
                this.SortedListEx = SortedListEx;
                UpdateValues();
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Re-read values from the list.
            /// </summary>
            public virtual void UpdateValues()
            {
                int iCount = SortedListEx.Count;

                vals = new object[iCount];
                throw new NotImplementedException();
                //SortedListEx.values.Values.CopyTo(vals, 0);

                object[] keys = new object[iCount];
                SortedListEx.values.Keys.CopyTo(keys, 0);
#if NETFX_CORE || WP
                SortedDictionary<object[], Array> strDic = new SortedDictionary<object[], Array>();
                strDic.Add(keys, vals);
#else
                Array.Sort(keys, vals, SortedListEx.comparer);
#endif
            }

            /// <summary>
            /// Gets the size of the collection. Read-only.
            /// </summary>
            public virtual int Count
            {
                get { return SortedListEx._size; }
            }

            /// <summary>
            /// Returns True if list is readonly, False otherwise. Read-only.
            /// </summary>
            public virtual bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if collection has fixed size, False otherwise.
            /// </summary>
            public virtual bool IsFixedSize
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public virtual bool IsSynchronized
            {
                get { return SortedListEx.IsSynchronized; }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public virtual object SyncRoot
            {
                get { return SortedListEx.SyncRoot; }
            }

            /// <summary>
            /// Adds an element with the provided key to the list.
            /// </summary>
            /// <param name="key">The Object to use as the key of the element to add.</param>
            public virtual int Add(object key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all elements from the collection.
            /// </summary>
            public virtual void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified value.
            /// </summary>
            /// <param name="value">Value to search.</param>
            /// <returns>True if list contains specified value.</returns>
            public virtual bool Contains(object value)
            {
                return SortedListEx.ContainsValue(value);
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="arrayIndex">The index in array at which copying begins.</param>
            public virtual void CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException();

                // Defer error checking to Array.Copy.
                Array.Copy(vals, 0, array, arrayIndex, SortedListEx.Count);
            }

            /// <summary>
            /// Insert the value at the specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public virtual void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the value at the specified index.
            /// </summary>
            public virtual object this[int index]
            {
                get
                {
                    return SortedListEx.GetByIndex(index);
                }
                set
                {
                    SortedListEx.SetByIndex(index, value);
                }
            }

            /// <summary>
            /// Returns an IEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IEnumerator for the list.</returns>
            public virtual IEnumerator GetEnumerator()
            {
                return new SortedListExEnumerator(SortedListEx, 0, SortedListEx.Count, SortedListExEnumerator.Values);
            }

            /// <summary>
            /// Returns the zero-based index of the specified value.
            /// </summary>
            /// <param name="value">The value to locate.</param>
            /// <returns>The zero-based index of the value, if the value is found, otherwise -1.</returns>
            public virtual int IndexOf(object value)
            {
                return Array.IndexOf(vals, value, 0, SortedListEx.Count);
            }

            /// <summary>
            ///Removes the specified value from list.
            /// </summary>
            /// <param name="value">The value to remove.</param>
            public virtual void Remove(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the element at the specified index from the list.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public virtual void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
            #endregion
        }
        #endregion
    }

#if Generics
    /// <summary>
    /// Optimized version of SortedList collection. Instead of keeping two
    /// arrays, one for keys and one for values, the values array on
    /// Hashtable collection are changed. Performance of this collection is better than that of SortedList.
    /// </summary>
    internal class SortedListEx<TKey, TValue> :
        IDictionary<TKey, TValue>,
        ICloneable
    {
        #region Constants
        /// <summary>
        /// Default capacity of internal buffers.
        /// </summary>
        private const int _defaultCapacity = 16;
        #endregion

        #region Fields
        /// <summary>
        /// Array which store keys in sorted order.
        /// </summary>
        private TKey[] m_keys;

        /// <summary>
        /// Collection stores values.
        /// </summary>
        private Dictionary<TKey, TValue> m_values;

        /// <summary>
        /// Size of collection.
        /// </summary>
        private int m_size;

        /// <summary>
        /// Version of collection data.
        /// </summary>
        private int m_version;

        /// <summary>
        /// Default comparer for keys.
        /// </summary>
        private IComparer<TKey> m_comparer;

        /// <summary>
        /// List of keys.
        /// </summary>
        private KeyList m_keyList;

        /// <summary>
        /// List of values.
        /// </summary>
        private ValueList m_valueList;

        #endregion

        #region Class Properties
        /// <summary>
        /// Capacity of internal buffers.
        /// </summary>
        public virtual int Capacity
        {
            get
            {
                return m_keys.Length;
            }

            set
            {
                if (value != m_keys.Length)
                {
                    if (value < m_size)
                        throw new ArgumentOutOfRangeException("value");

                    if (value > 0)
                    {
                        TKey[] newKeys = new TKey[value];

                        if (m_size > 0)
                        {
                            Array.Copy(m_keys, 0, newKeys, 0, m_size);
                        }

                        m_keys = newKeys;
                    }
                    else
                    {
                        m_keys = new TKey[_defaultCapacity];
                    }
                }
            }
        }

        /// <summary>
        /// Size of the collection. Read-only.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// List of keys. Read-only.
        /// </summary>
        public virtual IList<TKey> Keys
        {
            get
            {
                return GetKeyList();
            }
        }
        /// <summary>
        /// List of keys. Read-only.
        /// </summary>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return GetKeyList();
            }
        }

        /// <summary>
        /// List of values. Read-only.
        /// </summary>
        public virtual IList<TValue> Values
        {
            get
            {
                return GetValueList();
            }
        }
        /// <summary>
        /// List of values. Read-only.
        /// </summary>
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return GetValueList();
            }
        }

        /// <summary>
        /// Returns True if list is readonly, False otherwise. Read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns True if collection has fixed size, False otherwise.
        /// </summary>
        public virtual bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns True if the collection is synchronized, False otherwise.
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the object that can be used to synchronize access to the collection.
        /// Read-only.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        public virtual TValue this[TKey key]
        {
            get
            {
                return m_values[key];
            }

            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (m_values.ContainsKey(key))
                {
                    m_values[key] = value;
                }
                else
                {
                    Add(key, value);
                }

                m_version++;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public SortedListEx()
        {
            m_keys = new TKey[_defaultCapacity];
            m_values = new Dictionary<TKey, TValue>(_defaultCapacity);
            m_comparer = Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public SortedListEx(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("initialCapacity");

            m_keys = new TKey[initialCapacity];
            m_values = new Dictionary<TKey, TValue>(initialCapacity);
            m_comparer = Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public SortedListEx(IComparer<TKey> comparer)
            : this()
        {
            if (comparer != null) m_comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="capacity">The capacity.</param>
        public SortedListEx(IComparer<TKey> comparer, int capacity)
            : this(comparer)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        public SortedListEx(IDictionary<TKey, TValue> d)
            : this(d, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="comparer">The comparer.</param>
        public SortedListEx(IDictionary<TKey, TValue> d, IComparer<TKey> comparer)
            : this(comparer, (d != null ? d.Count : 0))
        {
            if (d == null)
                throw new ArgumentNullException("d");

            // Create copy of dictionary values.
            d.Keys.CopyTo(m_keys, 0);
            m_values = new Dictionary<TKey, TValue>(d);

            Array.Sort(m_keys, comparer);
            m_size = d.Count;
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Returns a synchronized (thread-safe) wrapper for the SortedList.
        /// </summary>
        /// <param name="list">The SortedList to synchronize.</param>
        /// <returns>A synchronized (thread-safe) wrapper for the SortedList.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// When list is null.
        /// </exception>
        public static SortedListEx<TKey, TValue> Synchronized(SortedListEx<TKey, TValue> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            return new SyncSortedListEx(list);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an element with the provided key and value to the list.
        /// </summary>
        /// <param name="key">The Object to use as the key of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        /// <exception cref="System.ArgumentNullException">
        /// When key is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// When list already contains specified key.
        /// </exception>
        public virtual void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (m_values.ContainsKey(key))
            {
                throw new ArgumentException("Duplicated");
            }

            int index = Array.BinarySearch(m_keys, 0, m_size, key, m_comparer);
            Insert(~index, key, value);
        }
        /// <summary>
        /// Adds the specified pair.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public virtual void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public virtual void Clear()
        {
            m_version++;
            m_size = 0;

            m_keys = new TKey[_defaultCapacity];
            m_values = new Dictionary<TKey, TValue>(_defaultCapacity);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the current instance.</returns>
        public virtual object Clone()
        {
            SortedListEx<TKey, TValue> sl = new SortedListEx<TKey, TValue>(m_size);
            Array.Copy(m_keys, 0, sl.m_keys, 0, m_size);
            sl.m_values = new Dictionary<TKey, TValue>(m_values);
            sl.m_size = m_size;
            sl.m_version = m_version;
            sl.m_comparer = m_comparer;

            // Don't copy keyList or the valueList.
            return sl;
        }
        /// <summary>
        /// Clone current instance.
        /// </summary>
        /// <returns>Returns clone of current object.</returns>
        public SortedListEx<TKey, TValue> CloneAll()
        {
            int iLen = Count;
            SortedListEx<TKey, TValue> result = new SortedListEx<TKey, TValue>(iLen + 1);

            for (int i = 0; i < iLen; i++)
            {
                TValue o = GetByIndex(i);

                o = (TValue)((ICloneable)o).Clone();
                result.Add(GetKey(i), o);
            }

            return result;
        }
        /// <summary>
        /// Determines whether the list contains an element with the specified key.
        /// </summary>
        /// <param name="key">Key of the element to search.</param>
        /// <returns>True if list contains specified key.</returns>
        public virtual bool Contains(TKey key)
        {
            return m_values.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the list contains an element with the specified key.
        /// </summary>
        /// <param name="key">Key of the element to search.</param>
        /// <returns>True if list contains specified key.</returns>
        public virtual bool ContainsKey(TKey key)
        {
            // This is a SPEC'ed duplicate of Contains().
            return m_values.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the list contains the specified value.
        /// </summary>
        /// <param name="value">Value of the element to search.</param>
        /// <returns>True if list contains specified value.</returns>
        public virtual bool ContainsValue(TValue value)
        {
            return m_values.ContainsValue(value);
        }
        /// <summary>
        /// Determines whether [contains] [the specified pair].
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>
        /// 	if it contains the specified pair, set to <c>true</c> .
        /// </returns>
        public virtual bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            bool result = false;

            if (ContainsKey(pair.Key))
            {
                result = (pair.Value.Equals(this[pair.Key]));
            }

            return result;
        }

        /// <summary>
        /// Copies all the elements of the SortedListEx to the specified one-dimensional Array
        /// starting at the specified destination Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the
        /// elements copied from the current list.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If specified array is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If rank of the array is not 1 or there are not enough elements.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If specified arrayIndex is less than zero.
        /// </exception>
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException();
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < Count; i++)
            {
                TKey key = m_keys[i];
                KeyValuePair<TKey, TValue> entry = new KeyValuePair<TKey, TValue>(key, m_values[key]);
                array.SetValue(entry, i + arrayIndex);
            }
        }

        /// <summary>
        /// Gets the value at the specified index of the SortedListEx.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>The value at the specified index of the SortedListEx.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual TValue GetByIndex(int index)
        {
            if (index < 0 || index >= m_size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return m_values[m_keys[index]];
        }

        /// <summary>
        /// Gets the key at the specified index of the SortedListEx.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>The key at the specified index of the SortedListEx.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual TKey GetKey(int index)
        {
            if (index < 0 || index >= m_size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return m_keys[index];
        }

        /// <summary>
        /// Gets the keys in the SortedListEx.
        /// </summary>
        /// <returns>An IList containing the keys in the SortedListEx.</returns>
        public virtual IList<TKey> GetKeyList()
        {
            if (m_keyList == null) m_keyList = new KeyList(this);
            return m_keyList;
        }

        /// <summary>
        /// Gets the values in the SortedListEx.
        /// </summary>
        /// <returns>An IList containing the values in the SortedListEx.</returns>
        public virtual IList<TValue> GetValueList()
        {
            if (m_valueList == null)
            {
                m_valueList = new ValueList(this);
            }
            else
            {
                m_valueList.UpdateValues();
            }

            return m_valueList;
        }

        /// <summary>
        /// Returns the zero-based index of the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The zero-based index of key, if key is found; otherwise, -1.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// If specified key is null.
        /// </exception>
        public virtual int IndexOfKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            int ret = Array.BinarySearch(m_keys, 0, m_size, key, m_comparer);

            return ret >= 0 ? ret : -1;
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified value.
        /// </summary>
        /// <param name="value">The value to locate (can be NULL).</param>
        /// <returns>
        /// The zero-based index of the first occurrence of value, if value is found;
        /// otherwise, -1.
        /// </returns>
        public virtual int IndexOfValue(TValue value)
        {
            TKey key;

            IEnumerator<KeyValuePair<TKey, TValue>> enm = m_values.GetEnumerator();
            enm.Reset();

            while (enm.MoveNext())
            {
                if (enm.Current.Value.Equals(value))
                {
                    key = enm.Current.Key;
                    return Array.IndexOf(m_keys, key, 0, m_size);
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= m_size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            m_size--;
            TKey key = m_keys[index];
            if (index < m_size)
            {
                Array.Copy(m_keys, index + 1, m_keys, index, m_size - index);
            }
            m_keys[m_size] = default(TKey);
            m_values.Remove(key);
            m_version++;
        }

        /// <summary>
        ///Removes the element with the specified key from SortedListEx.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public virtual bool Remove(TKey key)
        {
            int i = IndexOfKey(key);
            bool result = false;

            if (i >= 0)
            {
                RemoveAt(i);
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Removes the specified pair.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns></returns>
        public virtual bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            bool result = false;

            if (Contains(pair))
            {
                if (pair.Value.Equals(this[pair.Key]))
                {
                    Remove(pair.Key);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Replaces the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index at which to save value.</param>
        /// <param name="value">The Object to save into. Can be NULL.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When index is less than zero or greater than size of the list.
        /// </exception>
        public virtual void SetByIndex(int index, TValue value)
        {
            if (index < 0 || index >= m_size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            m_values[m_keys[index]] = value;

            m_version++;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements.
        /// </summary>
        public virtual void TrimToSize()
        {
            Capacity = m_size;
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator that can iterate through the SortedListEx.
        /// </summary>
        /// <returns>An IDictionaryEnumerator for the SortedListEx.</returns>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new SortedListExEnumerator(this, 0, m_size);
        }

        /// <summary>
        /// Returns an IEnumerator that can iterate through the SortedListEx.
        /// </summary>
        /// <returns>An IEnumerator for the SortedListEx.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();

            //new SortedListExEnumerator( this, 0, m_size, SortedListExEnumerator.DictEntry );
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            bool result = false;

            if (ContainsKey(key))
            {
                value = this[key];
                result = true;
            }
            else
            {
                value = default(TValue);
            }

            return result;
        }

        #endregion

        #region Private helper methods
        /// <summary>
        /// Inserts element with specified key and value at specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert element at.</param>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        private void Insert(int index, TKey key, TValue value)
        {
            if (m_size == m_keys.Length)
            {
                EnsureCapacity(m_size + 1);
            }

            if (index < m_size)
            {
                Array.Copy(m_keys, index, m_keys, index + 1, m_size - index);
            }

            m_keys[index] = key;
            m_values[key] = value;

            m_size++;
            m_version++;
        }

        /// <summary>
        /// Ensures that the capacity of this instance is at least the specified value.
        /// </summary>
        /// <param name="min">The minimum capacity to ensure.</param>
        private void EnsureCapacity(int min)
        {
            int newCapacity = m_keys.Length == 0 ? 16 : m_keys.Length * 2;
            if (newCapacity < min)
            {
                newCapacity = min;
            }
            Capacity = newCapacity;
        }
        #endregion

        #region Internal classes declarations
        private class SyncSortedListEx : SortedListEx<TKey, TValue>
        {
            #region Fields
            /// <summary>
            /// Wrapped SortedListEx.
            /// </summary>
            private SortedListEx<TKey, TValue> m_list;

            /// <summary>
            /// Sync object.
            /// </summary>
            private object m_root;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates SyncSortedListEx for specified SortedListEx.
            /// </summary>
            /// <param name="list">SortedListEx that will be wrapped.</param>
            internal SyncSortedListEx(SortedListEx<TKey, TValue> list)
            {
                m_list = list;
                m_root = list.SyncRoot;
            }
            #endregion

            #region Class Properties
            /// <summary>
            /// Capacity of internal buffers.
            /// </summary>
            public override int Capacity
            {
                get { lock (m_root) { return m_list.Capacity; } }
            }

            /// <summary>
            /// Size of the collection. Read-only.
            /// </summary>
            public override int Count
            {
                get { lock (m_root) { return m_list.Count; } }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public override object SyncRoot
            {
                get { return m_root; }
            }

            /// <summary>
            /// Returns True if list is readonly, False otherwise. Read-only.
            /// </summary>
            public override bool IsReadOnly
            {
                get { return m_list.IsReadOnly; }
            }

            /// <summary>
            /// Returns True if collection has fixed size, False otherwise.
            /// </summary>
            public override bool IsFixedSize
            {
                get { return m_list.IsFixedSize; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public override bool IsSynchronized
            {
                get { return true; }
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            public override TValue this[TKey key]
            {
                get
                {
                    lock (m_root)
                    {
                        return m_list[key];
                    }
                }

                set
                {
                    lock (m_root)
                    {
                        m_list[key] = value;
                    }
                }
            }

            #endregion

            #region Public Methods
            /// <summary>
            /// Adds an element with the provided key and value to the list.
            /// </summary>
            /// <param name="key">The Object to use as the key of the element to add.</param>
            /// <param name="value">The Object to use as the value of the element to add.</param>
            public override void Add(TKey key, TValue value)
            {
                lock (m_root)
                {
                    m_list.Add(key, value);
                }
            }

            /// <summary>
            /// Removes all the elements from the collection.
            /// </summary>
            public override void Clear()
            {
                lock (m_root)
                {
                    m_list.Clear();
                }
            }

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>A new object that is a copy of the current instance.</returns>
            public override object Clone()
            {
                lock (m_root)
                {
                    return m_list.Clone();
                }
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public override bool Contains(TKey key)
            {
                lock (m_root)
                {
                    return m_list.Contains(key);
                }
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public override bool ContainsKey(TKey key)
            {
                lock (m_root)
                {
                    return m_list.ContainsKey(key);
                }
            }

            /// <summary>
            /// Determines whether the list contains the specified value.
            /// </summary>
            /// <param name="value">Value of the element to search.</param>
            /// <returns>True if list contains specified value.</returns>
            public override bool ContainsValue(TValue value)
            {
                lock (m_root)
                {
                    return m_list.ContainsValue(value);
                }
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="index">The index in array at which copying begins.</param>
            public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
            {
                lock (m_root)
                {
                    m_list.CopyTo(array, index);
                }
            }

            /// <summary>
            /// Gets the value at the specified index of the list.
            /// </summary>
            /// <param name="index">The zero-based index of the value to get.</param>
            /// <returns>The value at the specified index of the SortedListEx.</returns>
            public override TValue GetByIndex(int index)
            {
                lock (m_root)
                {
                    return m_list.GetByIndex(index);
                }
            }

            /// <summary>
            /// Returns an IDictionaryEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IDictionaryEnumerator for the list.</returns>
            public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                lock (m_root)
                {
                    return m_list.GetEnumerator();
                }
            }

            /// <summary>
            /// Gets the key at the specified index of the list.
            /// </summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at the specified index of the list.</returns>
            public override TKey GetKey(int index)
            {
                lock (m_root)
                {
                    return m_list.GetKey(index);
                }
            }

            /// <summary>
            /// Gets the keys in the list.
            /// </summary>
            /// <returns>An IList containing the keys in the list.</returns>
            public override IList<TKey> GetKeyList()
            {
                lock (m_root)
                {
                    return m_list.GetKeyList();
                }
            }

            /// <summary>
            /// Gets the values in the list.
            /// </summary>
            /// <returns>An IList containing the values in the list.</returns>
            public override IList<TValue> GetValueList()
            {
                lock (m_root)
                {
                    return m_list.GetValueList();
                }
            }

            /// <summary>
            /// Returns the zero-based index of the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The zero-based index of key, if key is found; otherwise, -1.</returns>
            public override int IndexOfKey(TKey key)
            {
                lock (m_root)
                {
                    return m_list.IndexOfKey(key);
                }
            }

            /// <summary>
            /// Returns the zero-based index of the first occurrence of the specified value.
            /// </summary>
            /// <param name="value">The value to locate (can be NULL).</param>
            /// <returns>
            /// The zero-based index of the first occurrence of value, if value is found;
            /// otherwise, -1.
            /// </returns>
            public override int IndexOfValue(TValue value)
            {
                lock (m_root)
                {
                    return m_list.IndexOfValue(value);
                }
            }

            /// <summary>
            /// Removes the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public override void RemoveAt(int index)
            {
                lock (m_root)
                {
                    m_list.RemoveAt(index);
                }
            }

            /// <summary>
            ///Removes the element with the specified key from list.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            public override bool Remove(TKey key)
            {
                bool result = false;
                lock (m_root)
                {
                    result = m_list.Remove(key);
                }

                return result;
            }

            /// <summary>
            /// Replaces the value at a specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public override void SetByIndex(int index, TValue value)
            {
                lock (m_root)
                {
                    m_list.SetByIndex(index, value);
                }
            }

            /// <summary>
            /// Sets the capacity to the actual number of elements.
            /// </summary>
            public override void TrimToSize()
            {
                lock (m_root)
                {
                    m_list.TrimToSize();
                }
            }
            #endregion
        }

        private class SortedListExEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, ICloneable
        {
            #region Fields
            /// <summary>
            /// List for which is this enumerator.
            /// </summary>
            private SortedListEx<TKey, TValue> m_sortedListEx;

            /// <summary>
            /// Key of the current element.
            /// </summary>
            private TKey m_key;

            /// <summary>
            /// Values of the current element.
            /// </summary>
            private TValue m_value;

            /// <summary>
            /// Index of current element.
            /// </summary>
            private int m_index;

            /// <summary>
            /// Starting index for the enumerator.
            /// </summary>
            private int m_startIndex;

            /// <summary>
            /// Ending index for this enumerator.
            /// </summary>
            private int m_endIndex;

            /// <summary>
            /// Version of collection data.
            /// </summary>
            private int m_version;

            /// <summary>
            /// True if current element is correct, False otherwise
            /// (before beginning or after end).
            /// </summary>
            private bool m_current;

            /// <summary>
            /// Shows if the object was disposed.
            /// </summary>
            private bool m_isDisposed = false;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Creates enumerator for specified list, starting from specified index
            /// and with specified count.
            /// </summary>
            /// <param name="sortedListEx">list for which to create enumerator.</param>
            /// <param name="index">Starting index.</param>
            /// <param name="count">Number of elements to enumerate.</param>
            internal SortedListExEnumerator(SortedListEx<TKey, TValue> sortedListEx, int index, int count)
            {
                m_sortedListEx = sortedListEx;
                m_index = index;
                m_startIndex = index;
                m_endIndex = index + count;
                m_version = sortedListEx.m_version;
                m_current = false;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>Copy of the current instance.</returns>
            public object Clone()
            {
                return MemberwiseClone();
            }

            /// <summary>
            /// Returns key of the current element. Read-only.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            /// When the current version is not equal to the SortedListEx version
            /// or if current is false.
            /// </exception>
            public virtual TKey Key
            {
                get
                {
                    if (m_version != m_sortedListEx.m_version)
                    {
                        throw new InvalidOperationException();
                    }

                    if (m_current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return m_key;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if the enumerator was successfully advanced to the next element;
            /// False if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">
            /// When the current version is not equal to the SortedListEx version.
            /// </exception>
            public virtual bool MoveNext()
            {
                if (m_version != m_sortedListEx.m_version)
                {
                    throw new InvalidOperationException();
                }

                if (m_index < m_endIndex)
                {
                    m_key = m_sortedListEx.m_keys[m_index];
                    m_value = m_sortedListEx.m_values[m_key];
                    m_index++;
                    m_current = true;
                    return true;
                }
                m_key = default(TKey);
                m_value = default(TValue);
                m_current = false;
                return false;
            }

            /// <summary>
            /// The current element in the collection. Read-only.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            /// If current is false.
            /// </exception>
            public virtual KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (m_current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return new KeyValuePair<TKey, TValue>(m_key, m_value);
                }
            }

            /// <summary>
            /// Returns value for the current element.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            /// When the current version is not equal to the SortedListEx version
            /// or if current is false.
            /// </exception>
            public virtual TValue Value
            {
                get
                {
                    if (m_version != m_sortedListEx.m_version)
                    {
                        throw new InvalidOperationException();
                    }

                    if (m_current == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return m_value;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before
            /// the first element in the collection.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            /// When the current version is not equal to the SortedListEx version.
            /// </exception>
            public virtual void Reset()
            {
                if (m_version != m_sortedListEx.m_version)
                {
                    throw new InvalidOperationException();
                }

                m_index = m_startIndex;
                m_current = false;
                m_key = default(TKey);
                m_value = default(TValue);
            }
            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (!m_isDisposed)
                {
                    m_isDisposed = true;

                    m_sortedListEx = null;
                    m_current = false;
                }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            #endregion
        }

        private class KeyList : IList<TKey>
        {
            #region Fields
            /// <summary>
            /// List for which this collection was created.
            /// </summary>
            private SortedListEx<TKey, TValue> m_sortedListEx;
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Creates KeyList for specified SortedListEx.
            /// </summary>
            /// <param name="sortedListEx">The sorted list ex.</param>
            internal KeyList(SortedListEx<TKey, TValue> sortedListEx)
            {
                m_sortedListEx = sortedListEx;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Size of the collection. Read-only.
            /// </summary>
            public virtual int Count
            {
                get { return m_sortedListEx.m_size; }
            }

            /// <summary>
            /// Returns True if list is readonly, False otherwise. Read-only.
            /// </summary>
            public virtual bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if collection has fixed size, False otherwise.
            /// </summary>
            public virtual bool IsFixedSize
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public virtual bool IsSynchronized
            {
                get { return m_sortedListEx.IsSynchronized; }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public virtual object SyncRoot
            {
                get { return m_sortedListEx.SyncRoot; }
            }

            /// <summary>
            /// Adds an element with the provided key to the list.
            /// </summary>
            /// <param name="key">The Object to use as the key of the element to add.</param>
            public virtual void Add(TKey key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all elements from the collection.
            /// </summary>
            public virtual void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified key.
            /// </summary>
            /// <param name="key">Key of the element to search.</param>
            /// <returns>True if list contains specified key.</returns>
            public virtual bool Contains(TKey key)
            {
                return m_sortedListEx.Contains(key);
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="arrayIndex">The index in array at which copying begins.</param>
            /// <exception cref="System.ArgumentException">
            /// If array is null or rank of the array is not 1.
            /// </exception>
            public virtual void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException();

                // Defer error checking to Array.Copy.
                Array.Copy(m_sortedListEx.m_keys, 0, array, arrayIndex, m_sortedListEx.Count);
            }

            /// <summary>
            /// Insert the value at the specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public virtual void Insert(int index, TKey value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the key at the specified index.
            /// </summary>
            public virtual TKey this[int index]
            {
                get
                {
                    return m_sortedListEx.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Returns an IEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IEnumerator for the list.</returns>
            public virtual IEnumerator<TKey> GetEnumerator()
            {
                int index = 0;
                TKey current;
                while (index < m_sortedListEx.m_size)
                {
                    current = m_sortedListEx.m_keys[index];
                    yield return current;
                    ++index;
                }
            }
            /// <summary>
            /// Returns an IEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IEnumerator for the list.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Returns the zero-based index of the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The zero-based index of the key, if the key is found; otherwise, -1.</returns>
            public virtual int IndexOf(TKey key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                int i = Array.BinarySearch(m_sortedListEx.m_keys, 0,
                    m_sortedListEx.Count, key, m_sortedListEx.m_comparer);
                if (i >= 0)
                {
                    return i;
                }
                return -1;
            }

            /// <summary>
            ///Removes the element with the specified key from list.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            public virtual bool Remove(TKey key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the element at the specified index from the list.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public virtual void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
            #endregion
        }

        private class ValueList : IList<TValue>
        {
            #region Fields
            /// <summary>
            /// List for which this collection was created.
            /// </summary>
            private SortedListEx<TKey, TValue> m_sortedListEx;

            /// <summary>
            /// Array list that contain values.
            /// </summary>
            private TValue[] m_values;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates ValueList for specified SortedListEx.
            /// </summary>
            /// <param name="sortedListEx">The sorted list ex.</param>
            internal ValueList(SortedListEx<TKey, TValue> sortedListEx)
            {
                m_sortedListEx = sortedListEx;
                UpdateValues();
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Re-read values from the list.
            /// </summary>
            public virtual void UpdateValues()
            {
                int iCount = m_sortedListEx.Count;

                m_values = new TValue[iCount];
                m_sortedListEx.m_values.Values.CopyTo(m_values, 0);

                TKey[] keys = new TKey[iCount];
                m_sortedListEx.m_values.Keys.CopyTo(keys, 0);

#if NETFX_CORE || WP
                SortedDictionary<TKey[], TValue[]> srtdDic = new SortedDictionary<TKey[], TValue[]>();
                srtdDic.Add(keys,m_values);
#else
                Array.Sort(keys, m_values, m_sortedListEx.m_comparer);
#endif
            }

            /// <summary>
            /// Size of the collection. Read-only.
            /// </summary>
            public virtual int Count
            {
                get { return m_sortedListEx.m_size; }
            }

            /// <summary>
            /// Returns True if list is readonly, False otherwise. Read-only.
            /// </summary>
            public virtual bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if collection has fixed size, False otherwise.
            /// </summary>
            public virtual bool IsFixedSize
            {
                get { return true; }
            }

            /// <summary>
            /// Returns True if the collection is synchronized, False otherwise.
            /// </summary>
            public virtual bool IsSynchronized
            {
                get { return m_sortedListEx.IsSynchronized; }
            }

            /// <summary>
            /// Returns the object that can be used to synchronize access to the collection.
            /// Read-only.
            /// </summary>
            public virtual object SyncRoot
            {
                get { return m_sortedListEx.SyncRoot; }
            }

            /// <summary>
            /// Adds an element with the provided key to the list.
            /// </summary>
            /// <param name="value">The Object to use as the key of the element to add.</param>
            public virtual void Add(TValue value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all elements from the collection.
            /// </summary>
            public virtual void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether the list contains an element with the specified value.
            /// </summary>
            /// <param name="value">Value to search.</param>
            /// <returns>True if list contains specified value.</returns>
            public virtual bool Contains(TValue value)
            {
                return m_sortedListEx.ContainsValue(value);
            }

            /// <summary>
            /// Copies all the elements of the list to the specified one-dimensional Array
            /// starting at the specified destination Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the
            /// elements copied from the current list.</param>
            /// <param name="arrayIndex">The index in array at which copying begins.</param>
            public virtual void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException();

                // Defer error checking to Array.Copy.
                Array.Copy(m_values, 0, array, arrayIndex, m_sortedListEx.Count);
            }

            /// <summary>
            /// Insert the value at the specific index.
            /// </summary>
            /// <param name="index">The zero-based index at which to save value.</param>
            /// <param name="value">The Object to save into. Can be NULL.</param>
            public virtual void Insert(int index, TValue value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the value at the specified index.
            /// </summary>
            public virtual TValue this[int index]
            {
                get
                {
                    return m_sortedListEx.GetByIndex(index);
                }

                set
                {
                    m_sortedListEx.SetByIndex(index, value);
                }
            }
            /// <summary>
            /// Returns an IEnumerator that can iterate through the list.
            /// </summary>
            /// <returns>An IEnumerator for the list.</returns>
            public virtual IEnumerator<TValue> GetEnumerator()
            {
                int index = 0;
                TValue current;
                TKey[] keys = m_sortedListEx.m_keys;

                while (index < m_sortedListEx.m_size)
                {
                    current = m_sortedListEx.m_values[keys[index]];
                    yield return current;
                    ++index;
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            /// <summary>
            /// Returns the zero-based index of the specified value.
            /// </summary>
            /// <param name="value">The value to locate.</param>
            /// <returns>The zero-based index of the value, if the value is found, otherwise -1.</returns>
            public virtual int IndexOf(TValue value)
            {
                return Array.IndexOf(m_values, value, 0, m_sortedListEx.Count);
            }

            /// <summary>
            ///Removes the specified value from list.
            /// </summary>
            /// <param name="value">The value to remove.</param>
            public virtual bool Remove(TValue value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the element at the specified index from the list.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            public virtual void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
            #endregion
        }
        #endregion
    }

#endif
    /// <summary>
    /// Comparer for strings.
    /// </summary>
    internal class StringComparer :
#if Generics
 IComparer<string>,
#endif
 IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compare two strings.
        /// </summary>
        /// <param name="x">String to compare.</param>
        /// <param name="y">String to compare.</param>
        /// <returns>Returns compared results.</returns>
        public int Compare(object x, object y)
        {
            string str1 = x as string;
            string str2 = y as string;

            if (str1 != null && str2 != null)
            {
                return String.CompareOrdinal(str1, str2);
            }

            return 0;
        }
        #endregion

#if Generics
        #region IComparer<string> Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero. x is less than y.Zero x equals
        /// y.Greater than zero x is greater than y.
        /// </returns>
        public int Compare(string x, string y)
        {
            if (x != null && y != null)
            {
                return String.CompareOrdinal(x, y);
            }

            return 0;
        }
        #endregion
#endif
    }
}
