using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections;

namespace Utilities
{
    /// <summary>
    /// Represents a list that allows cross thread collection and property binding.
    /// Use AcquireLock for multithreaded scenarios.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableList<T> : IList<T>
    {
        /// <summary>
        /// The dispatcher that is used to notify the UI thread of changes
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// The list used by a worker thread
        /// </summary>
        private List<T> _list;

        /// <summary>
        /// The ObservableCollection that UI controls should bind to
        /// </summary>
        private ObservableCollection<T> _observableCollection;

        //Change callbacks
        private delegate void InsertItemCallback(int index, T item);
        private delegate void RemoveAtCallback(int index);
        private delegate void SetItemCallback(int index, T item);
        private delegate void AddCallback(T item);
        private delegate void ClearCallback();
        private delegate void RemoveCallback(T item);

        //property changed callback
        private delegate void PropertyChangedCallback(T item, PropertyChangedEventArgs e);

        /// <summary>
        /// Creates a new instance of the ObservableBackgroundList class
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to notify the UI thread of changes</param>
        public ObservableList(Dispatcher dispatcher)
        {
            if (dispatcher == null)
                this._dispatcher = Dispatcher.CurrentDispatcher;
            else
                this._dispatcher = dispatcher;
            this._list = new List<T>();
            this._observableCollection = new ObservableCollection<T>();
        }

        /// <summary>
        /// Gets the ObservableCollection that UI controls should bind to
        /// </summary>
        public ObservableCollection<T> ObservableCollection
        {
            get
            {
                if (this._dispatcher.CheckAccess() == false)
                {
                    throw new InvalidOperationException("ObservableCollection only accessible from UI thread");
                }
                return this._observableCollection;
            }
        }

        #region ICollection<T> Members

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire List.
        /// </summary>
        /// <param name="item">The object to locate in the List</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire List
        /// if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return this._list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, T item)
        {
            this._list.Insert(index, item);
            this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                new InsertItemCallback(InsertItemFromDispatcherThread),
                index,
                new object[] { item }
            );
        }

        /// <summary>
        /// Removes the element at the specified index of the List
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
            this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                new RemoveAtCallback(RemoveAtFromDispatcherThread),
                index
            );
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                return this._list[index];
            }
            set
            {
                this._list[index] = value;
                this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                    new SetItemCallback(SetItemFromDispatcherThread),
                    index,
                    new object[] { value }
                );
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an object to the end of the List.
        /// </summary>
        /// <param name="item">The object to be added to the end of the List</param>
        public void Add(T item)
        {
            this._list.Add(item);
            this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                new AddCallback(AddFromDispatcherThread),
                item
            );
        }

        /// <summary>
        /// Removes all elements from the List
        /// </summary>
        public void Clear()
        {
            this._list.Clear();
            this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                new ClearCallback(ClearFromDispatcherThread)
            );
        }

        /// <summary>
        /// Determines whether an element is in the List
        /// </summary>
        /// <param name="item">The object to locate in the List</param>
        /// <returns>true if item is found in the List; otherwise, false</returns>
        public bool Contains(T item)
        {
            return this._list.Contains(item);
        }

        /// <summary>
        /// Copies the entire List to a compatible one-dimensional
        /// array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.List<T>. The System.Array must have
        /// zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements actually contained in the List.
        /// </summary>
        public int Count
        {
            get
            {
                return this._list.Count;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the List.
        /// </summary>
        /// <param name="item">The object to remove from the List.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also
        ///     returns false if item was not found in the List</returns>
        public bool Remove(T item)
        {
            bool result = this._list.Remove(item);

            //only remove the item from the UI collection if it is removed from the worker collection
            if (result == true)
            {
                this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                    new RemoveCallback(RemoveFromDispatcherThread),
                    item
                );
            }
            return result;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the List
        /// </summary>
        /// <returns>A System.Collections.Generic.List<T>.Enumerator for the System.Collections.Generic.List<T>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the List
        /// </summary>
        /// <returns>Am Enumerator for the List.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Attaches listener for the CollectionItemNotifyPropertyChanged event
        /// </summary>
        /// <param name="source">The event source</param>
        private void StartListening(T source)
        {
            ICollectionItemNotifyPropertyChanged item = source as ICollectionItemNotifyPropertyChanged;
            if (item != null)
            {
                item.CollectionItemPropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(item_CollectionItemPropertyChanged);
            }
        }

        /// <summary>
        /// Removes listener for the CollectionItemNotifyPropertyChanged event
        /// </summary>
        /// <param name="source">The event source</param>
        private void StopListening(T source)
        {
            ICollectionItemNotifyPropertyChanged item = source as ICollectionItemNotifyPropertyChanged;
            if (item != null)
            {
                item.CollectionItemPropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(item_CollectionItemPropertyChanged);
            }
        }

        /// <summary>
        /// Handles the CollectionItemNotifyPropertyChanged event by posting to the UI thread to
        /// raise the PropertyChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_CollectionItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this._dispatcher.BeginInvoke(DispatcherPriority.Send,
                new PropertyChangedCallback(PropertyChangedFromDispatcherThread),
                sender,
                new object[] { e }
            );
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void PropertyChangedFromDispatcherThread(T source, PropertyChangedEventArgs e)
        {
            ICollectionItemNotifyPropertyChanged item = source as ICollectionItemNotifyPropertyChanged;
            item.NotifyPropertyChanged(e);
        }

        //change callbacks
        private void InsertItemFromDispatcherThread(int index, T item)
        {
            this._observableCollection.Insert(index, item);
            StartListening(item);
        }

        private void RemoveAtFromDispatcherThread(int index)
        {
            StopListening(this._observableCollection[index]);
            this._observableCollection.RemoveAt(index);
        }

        private void SetItemFromDispatcherThread(int index, T item)
        {
            StopListening(this._observableCollection[index]);
            this._observableCollection[index] = item;
            StartListening(item);
        }
        private void AddFromDispatcherThread(T item)
        {
            this._observableCollection.Add(item);
            StartListening(item);
        }
        private void ClearFromDispatcherThread()
        {
            foreach (T item in this._observableCollection)
            {
                StopListening(item);
            }
            this._observableCollection.Clear();
        }
        private void RemoveFromDispatcherThread(T item)
        {
            StopListening(item);
            this._observableCollection.Remove(item);
        }

        /// <summary>
        /// Acquires an object that locks on the collection. The lock is released when the object is disposed
        /// </summary>
        /// <returns>A disposable object that unlocks the collection when disposed</returns>
        public TimedLock AcquireLock()
        {
            return TimedLock.Lock(((ICollection)this._list).SyncRoot);
        }
    }
}
