// <copyright file="DictionaryList.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This class implements a custom dictionary that supports IList interface.
    /// </summary>
    public class DictionaryList
        : ListDictionary, IList
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryList"/> class.
        /// </summary>
        public DictionaryList()
        {
        }
        #endregion

        #region IList Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        int IList.Add(object value)
        {
            DictionaryEntry entry = (DictionaryEntry)value;
            Add(entry.Key, entry.Value);
            return Count - 1;
        }

        /// <summary>
        /// Removes all entries from the <see cref="T:System.Collections.Specialized.ListDictionary"/>.
        /// </summary>
        void IList.Clear()
        {
            Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            foreach (DictionaryEntry entry in Values)
            {
                if (entry.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            int ret = -1;
            foreach (DictionaryEntry entry in Values)
            {
                ++ret;
                if (entry.Value.Equals(value))
                {
                    return ret;
                }
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
        void IList.Insert(int index, object value)
        {
            object[] array = new object[base.Count];
            base.CopyTo(array, 0);
            base.Clear();

            int i = 0;
            while (i++ < index)
            {
                ((IList)this).Add(array[i]);
            }

            ((IList)this).Add(value);

            for (int j = i; j < array.Length; ++j)
            {
                ((IList)this).Add(array[j]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>This property always returns false.</returns>
        bool IList.IsFixedSize
        {
            get 
            {
                return base.IsFixedSize; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>This property always returns false.</returns>
        bool IList.IsReadOnly
        {
            get { return base.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            DictionaryEntry entry = (DictionaryEntry)value;
            base.Remove(entry.Key);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.RemoveAt(int index)
        {
            base.Remove(GetEntryAt(index).Key);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        object IList.this[int index]
        {
            get
            {
                return GetEntryAt(index);
            }

            set
            {
                DictionaryEntry entry = GetEntryAt(index);
                entry.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key"> The key of Dictionary entry.</param>
        new public object this[object key]
        {
            get
            {
                if (base.Contains(key))
                {
                    DictionaryEntry entry = (DictionaryEntry)base[key];
                    return entry.Value;
                }

                return null;
            }

            set
            {
                if (base.Contains(key))
                {
                    DictionaryEntry entry = (DictionaryEntry)base[key];
                    entry.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets the entry at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>DictionaryEntry enumerator</returns>
        private DictionaryEntry GetEntryAt(int index)
        {
            if (index < 0 || index >= base.Count)
            {
                throw new ArgumentOutOfRangeException("index is invalid");
            }

            int i = index;
            IDictionaryEnumerator enumerator = base.GetEnumerator();
            do
            {
                enumerator.MoveNext();
            } 
            while (i-- != 0);

            return (DictionaryEntry)enumerator.Value;
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the <see cref="T:System.Collections.Specialized.ListDictionary"/> entries to a one-dimensional <see cref="T:System.Array"/> instance at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the <see cref="T:System.Collections.DictionaryEntry"/> objects copied from <see cref="T:System.Collections.Specialized.ListDictionary"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or- <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.-or- The number of elements in the source <see cref="T:System.Collections.Specialized.ListDictionary"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception>
        /// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.Specialized.ListDictionary"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            base.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Specialized.ListDictionary"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Specialized.ListDictionary"/>.</returns>
        int ICollection.Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>This property always returns false.</returns>
        bool ICollection.IsSynchronized
        {
            get { return base.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.ListDictionary"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.ListDictionary"/>.</returns>
        object ICollection.SyncRoot
        {
            get { return base.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }
        #endregion
    }
}
