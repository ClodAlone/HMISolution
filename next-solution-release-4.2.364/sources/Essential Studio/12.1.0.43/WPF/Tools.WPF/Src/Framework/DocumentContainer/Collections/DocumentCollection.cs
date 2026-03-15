// <copyright file="DocumentCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region fileusing
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Collection, used to store documents.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DocumentCollection : IList, INotifyCollectionChanged, ICollectionViewFactory,INotifyPropertyChanged
    {
        #region Private members
        /// <summary>
        /// Presents Internal data
        /// </summary>
        private readonly ObservableCollection<object> m_dataInternal = new ObservableCollection<object>();

        /// <summary>
        /// Presents sync root
        /// </summary>
        private object m_syncRoot = null;
        
        /// <summary>
        /// Presents source
        /// </summary>
        private IEnumerable m_source;

        private int iCount = 0;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets custom source collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get
            {
                return m_source;
            }

            set
            {
                if (m_source != value)
                {
                    if (value != null && m_dataInternal.Count > 0)
                    {
                        throw new InvalidOperationException("Source collection can not be used while collection contains elements itself.");
                    }

                    if (m_source != null)
                    {
                        UnhookCollection(m_source);
                    }

                    if (value != null && m_source == null)
                    {
                        UnhookCollection(m_dataInternal);
                    }

                    m_source = value;

                    if (m_source != null)
                    {
                        HookCollection(m_source);
                    }
                    else
                    {
                        HookCollection(m_dataInternal);
                    }

                    if (CollectionChanged != null)
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                }
            }
        }

        /// <summary>
        /// Gets source collection or internal data list - depending on the presence of the source collection.
        /// </summary>
        /// <value>The data source.</value>
        protected IEnumerable Data
        {
            get
            {
                return m_source ?? m_dataInternal;
            }
        }

        /// <summary>
        /// Gets count of the items in collection.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.</returns>
        public int Count
        {
            get
            {
                return iCount;
            }
            internal set
            {
                iCount = value;
                OnPropertyChanged("Count");
            }
        }

        private int GetItemsCount()
        {
            if (m_source != null)
            {
                ICollection collection = m_source as ICollection;

                if (collection == null)
                {
                    throw new InvalidOperationException("Can not get the count of the items in a source collection.");
                }

                return collection.Count;
            }

            return m_dataInternal.Count;
        }
        #endregion

        #region Support INotifyPropertyChanged
       
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCollection"/> class.
        /// </summary>
        public DocumentCollection()
        {
            HookCollection(m_dataInternal);
        }
        #endregion

        #region Public methods
        /// <summary>Adds an item to the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public int Add(object value)
        {
            CheckEmptySource();

            return ValidateIListSupport().Add(value);
        }
        
        /// <summary>Removes all items from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {
            CheckEmptySource();

            ValidateIListSupport().Clear();
        }
        
        /// <summary>Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.</summary>
        /// <param name="index">The zero-based index at which value should be inserted. </param>
        /// <param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception>
        public void Insert(int index, object value)
        {
            CheckEmptySource();

            ValidateIListSupport().Insert(index, value);
        }
        
        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return ValidateIListSupport().IsFixedSize;
            }
        }
        
        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                IList data = Data as IList;

                return (data != null) ? data.IsReadOnly : false;
            }
        }
        
        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void Remove(object value)
        {
            CheckEmptySource();
            ValidateIListSupport().Remove(value);
        }
        
        /// <summary>Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void RemoveAt(int index)
        {
            CheckEmptySource();
            ValidateIListSupport().RemoveAt(index);
        }
        
        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        public object this[int index]
        {
            get
            {
                return GetItemAt(index);
            }

            set
            {
                CheckEmptySource();

                ValidateIListSupport()[index] = value;
            }
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
        
        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).</summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }
        
        /// <summary>Creates a new view on the collection that implements this interface. Typically, user code does not call this method.</summary>
        /// <returns>The newly created view.</returns>
        public ICollectionView CreateView()
        {
            throw new InvalidOperationException("DocumentCollection class does not support collection views.");
        }
        
        /// <summary>Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.</summary>
        /// <returns>true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public bool Contains(object value)
        {
            return ValidateIListSupport().Contains(value);
        }
        
        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public int IndexOf(object value)
        {
            return ValidateIListSupport().IndexOf(value);
        }
        
        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            if (m_source != null)
            {
                return m_source.GetEnumerator();
            }

            return m_dataInternal.GetEnumerator();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Subscribes to the CollectionChanged event.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void HookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;

            if (changes != null)
            {
                changes.CollectionChanged += OnChangesCollectionChanged;
            }
        }

        /// <summary>
        /// Unsubscribe from the CollectionChanged event.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void UnhookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;

            if (changes != null)
            {
                changes.CollectionChanged -= OnChangesCollectionChanged;
            }
        }

        /// <summary>
        /// Called when [changes collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnChangesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                Count = GetItemsCount();
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Checks whether current source supports the IList interface.
        /// </summary>
        /// <returns>Ilist value</returns>
        private IList ValidateIListSupport()
        {
            IList result = Data as IList;

            if (result == null)
            {
                throw new InvalidOperationException("Operation invalid, source collection should support IList.");
            }

            return result;
        }

        /// <summary>
        /// Get an item by index from the internal or external source through the IList or IEnumerable interfaces.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>object type value</returns>
        private object GetItemAt(int index)
        {
            IEnumerable data = Data;

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
        /// Checks whether the source is not set and raises an exception if the collection source is not null.
        /// </summary>
        private void CheckEmptySource()
        {
            if (SourceCollection != null)
            {
                throw new InvalidOperationException("SourceCollection is used.");
            }
        }
        #endregion
    }
}
