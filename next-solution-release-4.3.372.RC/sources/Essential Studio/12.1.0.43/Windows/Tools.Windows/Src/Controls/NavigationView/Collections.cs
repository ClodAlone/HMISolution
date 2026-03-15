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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    /// <summary>
    /// Base observable generic collection.
    /// </summary>
    /// <typeparam name="T">Observable CollectionBase</typeparam>
    [DefaultEvent("CollectionChanged")]
    public class ObservableCollectionBase<T> :
        Collection<T>
    {
        #region Events

        /// <summary>
        /// Occurs when collection is changed.
        /// </summary>
        [Description("Occurs when collection is changed.")]
        public event CollectionChangeEventHandler CollectionChanged;

        #endregion

        #region Overrides

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            FireCollectionChangedEvent(CollectionChangeAction.Add, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void RemoveItem(int index)
        {
            T item = this[index];

            base.RemoveItem(index);
            FireCollectionChangedEvent(CollectionChangeAction.Remove, item);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];

            base.SetItem(index, item);
            FireCollectionChangedEvent(CollectionChangeAction.Refresh, item);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollectionBase"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ObservableCollectionBase"/>.
        /// The collection itself cannot be <c>null</c> reference (<c>Nothing</c> in Visual Basic).</param>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            InsertRange(this.Count, collection);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollectionBase"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="ObservableCollectionBase"/>.
        /// The collection itself cannot be <c>null</c> null reference (<c>Nothing</c> in Visual Basic).</param>
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            else if (index > this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            IEnumerator<T> enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Insert(index++, enumerator.Current);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Fires <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="action">One of the <see cref="CollectionChangeAction"/>values that specifies how the collection changed.</param>
        /// <param name="item">An <see cref="T"/> that specifies the instance of the collection where the change occurred.</param>
        protected void FireCollectionChangedEvent(CollectionChangeAction action, T item)
        {
            if (this.CollectionChanged != null)
            {
                CollectionChangeEventArgs e = new CollectionChangeEventArgs(action, item);
                this.CollectionChanged(this, e);
            }
        }

        #endregion
    }

    /// <summary>
    /// Observable generic collection.
    /// </summary>
    /// <remarks>Item must implement <see cref="INotifyPropertyChanged"/>.</remarks>
    /// <typeparam name="U">Observable Collection</typeparam>
    [DefaultEvent("CollectionChanged")]
    public class ObservableCollection<U> :
        ObservableCollectionBase<U> where U : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when property of item within the collection is changed.
        /// </summary>
        [Description("Occurs when property of item within the collection is changed.")]
        public event PropertyChangedEventHandler ItemPropertyChanged;

        #endregion

        #region Overrides

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void InsertItem(int index, U item)
        {
            if (Contains(item))
            {
                throw new ArgumentException("Collection already contains the item.");
            }

            AdwiseEvents(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void RemoveItem(int index)
        {
            U item = this[index];

            UnadwiseEvents(item);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.</exception>
        protected override void SetItem(int index, U item)
        {
            U oldItem = this.Items[index];

            UnadwiseEvents(oldItem);
            base.SetItem(index, item);
            AdwiseEvents(item);
        }

        #endregion

        #region Implementation

        private void AdwiseEvents(INotifyPropertyChanged item)
        {
            if (item != null)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertyChanged);
            }
        }

        private void UnadwiseEvents(INotifyPropertyChanged item)
        {
            if (item != null)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertyChanged);
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ItemPropertyChanged != null)
            {
                this.ItemPropertyChanged(sender, e);
            }
        }

        #endregion
    }

    /// <summary>
    /// Stores collection of <see cref="Bar"/> instances.
    /// </summary>
    public class BarCollection :
        ObservableCollection<Bar>
    {
        #region Overrides

        /// <summary>
        /// Adds the range an array of <see cref="Bar"/>s.
        /// </summary>
        /// <param name="bars">An array of <see cref="Bar"/>s objects representing the bars to add to the collection. </param>
        public void AddRange(Bar[] bars)
        {
            base.AddRange(bars);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the <see cref="Bar"/> by its text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Index of found <see cref="Bar"/> or -1 otherwise.</returns>
        public int FindByText(string text)
        {
            int foundAt = -1;

            for (int i = 0; i < this.Count; i++)
            {
                Bar bar = this[i];

                if (bar.Text == text)
                {
                    foundAt = i;
                    break;
                }
            }

            return foundAt;
        }

        #endregion
    }
}
