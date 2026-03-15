#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Represents a collection of <see cref="Node"/> and <see cref="LineConnector"/> objects.
    /// </summary>
    public class CollectionExt : IList, INotifyCollectionChanged
    {
        /// <summary>
        /// Indicates whether the value is cleared.
        /// </summary>
        private static bool mcleared = false;

        /// <summary>
        /// Refers to the internal data (nodes and connectors)objects.
        /// </summary>
        private ObservableCollection<object> dataInternal = new ObservableCollection<object>();

        /// <summary>
        /// Refers to the Source collection object.
        /// </summary>
        private IEnumerable source;

        /// <summary>
        /// Object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        private object syncRoot = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExt"/> class .
        /// </summary>
        public CollectionExt()
        {
            this.HookCollection(this.dataInternal);
        }

        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the count of the items in the collection.
        /// </summary>        
        public int Count
        {
            get
            {
                if (this.source != null)
                {
                    ICollection collection = this.source as ICollection;
                    if (collection == null)
                    {
                        throw new InvalidOperationException("Can not get the count of the items in a source collection.");
                    }

                    return collection.Count;
                }

                return this.dataInternal.Count;
            }
        }       

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return this.ValidateIListSupport().IsFixedSize;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                IList data = this.Data as IList;
                return (data != null) ? data.IsReadOnly : false;
            }
        }

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).</summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets custom source collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get
            {
                return this.source;
            }

            set
            {
                if (value != null && this.dataInternal.Count > 0)
                {
                    throw new InvalidOperationException("Source collection can not be used while collection contains elements itself.");
                }

                if (this.source != null)
                {
                    this.UnhookCollection(this.source);
                }

                if (value != null && this.source == null)
                {
                    this.UnhookCollection(this.dataInternal);
                }

                this.source = value;

                if (this.source != null)
                {
                    this.HookCollection(this.source);
                }
                else
                {
                    this.HookCollection(this.dataInternal);
                }

                if (this.CollectionChanged != null)
                {
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
        public object SyncRoot
        {
            get
            {
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CollectionExt"/> is cleared.
        /// </summary>
        /// <value><c>true</c> if cleared; otherwise, <c>false</c>.</value>
        internal static bool Cleared
        {
            get { return mcleared; }
            set { mcleared = value; }
        }

        /// <summary>
        /// Gets the source collection or internal data list  depending on the presence of the source collection.
        /// </summary>
        protected IEnumerable Data
        {
            get
            {
                return this.source ?? this.dataInternal;
            }
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        public object this[int index]
        {
            get
            {
                return this.GetItemAt(index);
            }

            set
            {
                this.CheckEmptySource();
                this.ValidateIListSupport()[index] = value;
            }
        }

        internal int AddIfDoesNotExist(object value)
        {
            if (!this.Contains(value))
                return this.Add(value);
            else
                return -1;
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public int Add(object value)
        {
            this.CheckEmptySource();
            return this.ValidateIListSupport().Add(value);
        }
        
        /// <summary>Removes all items from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {
            this.CheckEmptySource();
            CollectionExt.Cleared = true;
            this.ValidateIListSupport().Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.</summary>
        /// <returns>true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public bool Contains(object value)
        {
            return this.ValidateIListSupport().Contains(value);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException">array is null. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        public void CopyTo(Array array, int index)
        {
            foreach (UIElement element in this)
            {
                array.SetValue(element, index++);
            }
        }

        /// <summary>Creates a new view on the collection that implements this interface. Typically, user code does not call this method.</summary>
        /// <returns>The newly created view.</returns>
        public ICollectionView CreateView()
        {
            return null;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            if (this.source != null)
            {
                return this.source.GetEnumerator();
            }

            return this.dataInternal.GetEnumerator();
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public int IndexOf(object value)
        {
            return this.ValidateIListSupport().IndexOf(value);
        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.</summary>
        /// <param name="index">The zero-based index at which value should be inserted. </param>
        /// <param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception>
        public void Insert(int index, object value)
        {
            this.CheckEmptySource();
            this.ValidateIListSupport().Insert(index, value);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void Remove(object value)
        {
            this.CheckEmptySource();
            this.ValidateIListSupport().Remove(value);
        }

        /// <summary>Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void RemoveAt(int index)
        {
            this.CheckEmptySource();
            this.ValidateIListSupport().RemoveAt(index);
        }

        /// <summary>
        /// Checks whether the source is not set and raises an exception if the collection source is not null.
        /// </summary>
        private void CheckEmptySource()
        {
            if (this.SourceCollection != null)
            {
                throw new InvalidOperationException("SourceCollection is used.");
            }
        }

        /// <summary>
        /// Get an item by index from the internal or external source through the IList or IEnumerable interfaces.
        /// </summary>
        /// <param name="index">The integer value representing the index</param>
        /// <returns>The item at the specified index.</returns>
        private object GetItemAt(int index)
        {
            IEnumerable data = this.Data;
            IList list = data as IList;

            if (list != null)
            {
                return list[index];
            }

            ICollection collection = data as ICollection;

            if (collection != null && collection.Count <= index)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            IEnumerator enumerator = data.GetEnumerator();

            if (enumerator == null)
            {
                throw new InvalidOperationException("Can not get an enumerator from the source collection.");
            }

            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }

            return enumerator.Current;
        }

        /// <summary>
        /// Subscribes to the CollectionChanged event.
        /// </summary>
        /// <param name="collection">Represents the IEnumerable collection.</param>
        private void HookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;

            if (changes != null)
            {
                changes.CollectionChanged += this.OnChangesCollectionChanged;
            }
        }        

        /// <summary>
        /// Reraises notifications on collection change.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        /// <remarks>Notifies of the collection change.</remarks>
        private void OnChangesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }        

        /// <summary>
        /// Unsubscribes from the CollectionChanged event.
        /// </summary>
        /// <param name="collection">Represents the IEnumerable collection.</param>
        private void UnhookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;
            if (changes != null)
            {
                changes.CollectionChanged -= this.OnChangesCollectionChanged;
            }
        }

        /// <summary>
        /// Checks whether current source supports the IList interface.
        /// </summary>
        /// <returns>The collection of objects in <see cref="IList"/></returns>
        private IList ValidateIListSupport()
        {
            IList result = this.Data as IList;

            if (result == null)
            {
                throw new InvalidOperationException("Operation invalid, source collection should support IList.");
            }

            return result;
        }
        #region Class Fields

        #endregion

        #region Properties

        #endregion

        #region Events

        #endregion

        #region Initialization

        #endregion

        #region Public methods

        #endregion

        #region Implementation

        #endregion
    }
}
