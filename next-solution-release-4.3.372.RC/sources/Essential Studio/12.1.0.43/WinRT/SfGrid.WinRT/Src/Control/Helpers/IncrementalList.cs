#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif
#if WinRT
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml.Data;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class IncrementalList<T> : IList<T>, ISupportIncrementalLoading, INotifyCollectionChanged
    {
        #region Members

        bool isBusy = false;
        List<T> InternalList;
#if WinRT
        Func<CancellationToken, uint, int, Task<IList<T>>> LoadMoreItems;
#else
        Action<uint, int> LoadMoreItems;
#endif

        #endregion

        #region Public Members

        public int MaxItemCount { get; set; }

        #endregion

        #region Ctor

#if WinRT
        public IncrementalList(Func<CancellationToken, uint, int, Task<IList<T>>> loadeMoreItemsFunc)
        {
            InternalList = new List<T>();
            LoadMoreItems = loadeMoreItemsFunc;
        }
#else
        public IncrementalList(Action<uint, int> loadeMoreItemsFunc)
        {
            InternalList = new List<T>();
            LoadMoreItems = loadeMoreItemsFunc;
        }
#endif

        #endregion

        #region ISupportIncreamentaLoading Members

        public bool HasMoreItems
        {
            get { return InternalList.Count < MaxItemCount; }
        }

#if WinRT
        IAsyncOperation<LoadMoreItemsResult> result;
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            try
            {
                if (isBusy)
                    return result;
                isBusy = true;
                result=AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
                return result;
            }
            finally
            {
                
            }

        }
#else
        public void LoadMoreItemsAsync(uint count)
        {
            if (isBusy)
                return;
            isBusy = true;
            var baseIndex = this.InternalList.Count;
            LoadMoreItems(count, baseIndex);
        }
#endif

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return InternalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InternalList.Insert(index, item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            if (index < -1 || index > this.InternalList.Count)
                throw new ArgumentOutOfRangeException();
            var removedItem = InternalList[index];
            InternalList.RemoveAt(index);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        public T this[int index]
        {
            get
            {
                return InternalList[index];
            }
            set
            {
                var oldValue = InternalList[index];
                InternalList[index] = value;
#if !SILVERLIGHT && !WP
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));
#else
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,value,oldValue,index));
#endif
            }
        }

        public void Add(T item)
        {
            InternalList.Add(item);
#if !SILVERLIGHT && !WP
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
#else
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item,InternalList.Count-1));
#endif

        }

        public void Clear()
        {
            InternalList.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return InternalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return InternalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            var index= this.InternalList.IndexOf(item);
            var result = InternalList.Remove(item);
            if (result)
            {
#if !SILVERLIGHT && !WP7
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
#else
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
#endif
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (InternalList as IEnumerable).GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChaged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        #endregion

        #region Private Methods
        
      

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.InternalList[i + baseIndex], i + baseIndex);
                RaiseCollectionChanged(args);
            }
        }

#if !WinRT
        public void LoadItems(IEnumerable<T> items)
        {
            if (items != null)
            {
                int baseIndex = InternalList.Count;
                InternalList.AddRange(items);
                NotifyOfInsertedItems(baseIndex, items.Count());
                isBusy = false;
            }
        }

#else
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {                
                var baseIndex = this.InternalList.Count;
                var items = await LoadMoreItems(c, count, baseIndex);
                InternalList.AddRange(items);
                NotifyOfInsertedItems(baseIndex, items.Count);
                return new LoadMoreItemsResult() { Count = (uint)items.Count };
            }
            finally
            {
                isBusy = false;
            }
        }
#endif

        #endregion
    }
}

