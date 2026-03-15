using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace WPFUtilities
{
    internal class Notify<T>
    {
        #region Declarations
        readonly NotifyCollectionChangedAction action;
        readonly List<T> changedItems;
        #endregion

        #region Constructors
        public Notify(NotifyCollectionChangedAction action)
        {
            this.action = action;
            this.changedItems = new List<T>();
        }
        #endregion

        #region Properties
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return action;
            }
        }

        public System.Collections.IList ChangedItems
        {
            get
            {
                return changedItems;
            }
        }
        #endregion
    }

    public class ObservableUpdateCollection<T> : ObservableCollection<T>
    {
        #region Declarations
        List<NotifyCollectionChangedEventArgs> list;
        long nSuppressCounter;

        readonly bool bCheckDisposableElement;
        #endregion

        #region Constructors
        public ObservableUpdateCollection(bool bCheckDisposableElement = false)
        {
            this.bCheckDisposableElement = bCheckDisposableElement;
        }

        public ObservableUpdateCollection(List<T> list, bool bCheckDisposableElement = false) 
            : base(list)
        {
            this.bCheckDisposableElement = bCheckDisposableElement;
        }

        public ObservableUpdateCollection(IEnumerable<T> collection, bool bCheckDisposableElement = false) 
            : base(collection)
        {
            this.bCheckDisposableElement = bCheckDisposableElement;
        }
        #endregion

        #region Public Methods
        public void BeginUpdate()
        {
            var counter = Interlocked.Increment(ref nSuppressCounter);
            System.Diagnostics.Debug.Assert(counter > 0);
        }

        public void EndUpdate()
        {
            var counter = Interlocked.Decrement(ref nSuppressCounter);
            System.Diagnostics.Debug.Assert(counter >= 0);
            if (counter == 0)
                NotifyCollectionChanges();
        }
        #endregion

        #region Private Methods
        void NotifyCollectionChanges()
        {
            using (BlockReentrancy())
            {
                if (list != null)
                {
                    Notify<T> notify = null;
                    while (list.Count > 0)
                    {
                        var e = list[0];
                        list.RemoveAt(0);

                        if (notify != null && notify.Action != e.Action)
                        {
                            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(notify.Action, notify.ChangedItems));
                            notify = null;
                        }
                        
                        if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Remove)
                        {
                            base.OnCollectionChanged(e);
                        }
                        else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count > 0)
                        {
                            if (notify == null)
                                notify = new Notify<T>(e.Action);
                            foreach (T item in e.NewItems)
                                notify.ChangedItems.Add(item);
                        }
                        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count > 0)
                        {
                            if (notify == null)
                                notify = new Notify<T>(e.Action);
                            foreach (T item in e.OldItems)
                                notify.ChangedItems.Add(item);
                        }
                    }

                    if (notify != null)
                        base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(notify.Action, notify.ChangedItems));
                }
            }
        }
        #endregion

        #region Overrides
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Interlocked.Read(ref nSuppressCounter) == 0)
                base.OnCollectionChanged(e);
            else
            {
                if (list == null)
                    list = new List<NotifyCollectionChangedEventArgs>();
                list.Add(e);
            }
        }

        protected override void ClearItems()
        {
            if (bCheckDisposableElement)
            {
                foreach (var removedItem in Items)
                {
                    if (removedItem is IDisposable)
                        (removedItem as IDisposable).Dispose();
                }
            }

            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            if (bCheckDisposableElement)
            {
                T removedItem = this[index];
                if (removedItem is IDisposable)
                {
                    if (removedItem is IDisposable)
                        (removedItem as IDisposable).Dispose();
                }
            }
            
            base.RemoveItem(index);
        }
        #endregion
    }
}
