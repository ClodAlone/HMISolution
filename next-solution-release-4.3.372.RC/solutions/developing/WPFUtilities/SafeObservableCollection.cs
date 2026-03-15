using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Utilities;
using System.Windows;
using System.Collections;
using System.Windows.Data;
#endif

namespace Utilities
{
    public interface CollectionUpdaterInterface
    {
        void BeginUpdate();
        void EndUpdate();
    }

    public class CollectionUpdater : IDisposable
    {
        CollectionUpdaterInterface list;
        public CollectionUpdater(CollectionUpdaterInterface l)
        {
            list = l;
            list.BeginUpdate();
        }

        public void Dispose()
        {
            if (list != null)
            {
                list.EndUpdate();
                list = null;
            }
        }
    }

    public class SafeObservableCollection<T> : ObservableCollection<T>, CollectionUpdaterInterface
    {
#if !WINDOWS_UWP && !NET_STANDARD
        // Dispatcher dispatcher;
        Object _syncObj;
        // readonly Object lockObject = new Object();
        // bool bNeedUpdate;
#endif
        void InitializeDispatcher(Object lockobject = null)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            // dispatcher = Dispatcher.CurrentDispatcher;
            if (lockobject != null)
                _syncObj = lockobject;
            else if (_syncObj == null)
                _syncObj = new Object();
            if (Dispatcher.FromThread(Thread.CurrentThread) != null)
                BindingOperations.EnableCollectionSynchronization(this, _syncObj);
#endif
        }

        public SafeObservableCollection()
        {
            InitializeDispatcher();
        }

        public SafeObservableCollection(Object lockObject)
        {
            InitializeDispatcher(lockObject);
        }

        public SafeObservableCollection(List<T> list,Object lockObject)
            : base(list)
        {
            InitializeDispatcher(lockObject);
        }

        public SafeObservableCollection(IEnumerable<T> collection) 
            : base(collection)
        {
            InitializeDispatcher();
        }

        public SafeObservableCollection(List<T> list)
            : base(list)
        {
            InitializeDispatcher();
        }

            #region MonitorQueue
            //private long _operationsQueueCount = 0;
            //private ConcurrentDictionary<DispatcherOperation, object> _operations;
            //private IDisposable observableTimer;
            //private void MonitorDispatcherQueue(long l)
            //{
            //    if (_operationsQueueCount != 0)
            //        Debug.WriteLine(String.Format("Dispatcher Operations In Queue {0}, ", _operationsQueueCount));

            //    if (_operationsQueueCount > 100)
            //    {
            //        Debug.WriteLine("Pushing all Dispatcher operations");
            //        var disp = _operations.Keys.First();
            //        if (disp.Dispatcher.Thread != null && disp.Dispatcher.Thread.IsAlive)
            //            disp.Dispatcher.DoEvents();
            //        else
            //        {
            //            lock (lockObject)
            //            {
            //                var currentqueue = Interlocked.Decrement(ref _operationsQueueCount);

            //                if (currentqueue == 0 && observableTimer != null)
            //                {
            //                    observableTimer.Dispose();
            //                    observableTimer = null;
            //                }

            //                object t;
            //                _operations.TryRemove(disp, out t);
            //            }
            //        }
            //        // Application.Current.DoEvents();
            //       //  _operations.Clear();
            //        // Interlocked.Exchange(ref _operationsQueueCount, 0);
            //    }
            //}
            #endregion

            //List<NotifyCollectionChangedEventArgs> pendingChanges = new List<NotifyCollectionChangedEventArgs>();
            //public override event NotifyCollectionChangedEventHandler CollectionChanged;

            ////readonly Dictionary<Dispatcher, DispatcherTimer> mapTimers = new Dictionary<Dispatcher, DispatcherTimer>();
            ////readonly Dictionary<Dispatcher, List<NotifyCollectionChangedEventArgs>> mapLists = new Dictionary<Dispatcher, List<NotifyCollectionChangedEventArgs>>();
            //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            //{
            //    lock (_syncObj)
            //    {
            //        if (IsUpdating)
            //        {
            //            bNeedUpdate = true;
            //            pendingChanges.Add(e);
            //            // break the workflow to reduce the UI communication
            //            return;
            //        }

            //        var list = pendingChanges.ToList();
            //        pendingChanges.Clear();
            //        list.ForEach(value =>
            //        {
            //            OnCollectionChanged(value);
            //        });
            //    }

            //    var eh = CollectionChanged;
            //    if (eh != null)
            //    {
            //        using (BlockReentrancy())
            //        {
            //            NotifyCollectionChangedEventHandler eventHandler =
            //                    this.CollectionChanged;
            //            if (eventHandler == null)
            //            {
            //                return;
            //            }

            //            // Walk thru invocation list.
            //            Delegate[] delegates = eventHandler.GetInvocationList();

            //            var disp = (from NotifyCollectionChangedEventHandler nh in delegates
            //                        let dpo = nh.Target as DispatcherObject
            //                        where dpo != null
            //                        select dpo.Dispatcher).FirstOrDefault();

            //            //if (disp == null)
            //            //    disp = dispatcher;

            //            foreach (NotifyCollectionChangedEventHandler handler in delegates)
            //            {
            //                // If the subscriber is a DispatcherObject and different thread.
            //                //var d = handler.Target as DispatcherObject;
            //                //if (d != null)
            //                //    disp = d.Dispatcher;

            //                if (disp != null && disp.Thread.IsAlive && !disp.CheckAccess())
            //                {
            //                    //if (e.Action == NotifyCollectionChangedAction.Reset)
            //                    //{
            //                    //    disp.Invoke(DispatcherPriority.DataBind,
            //                    //        (Action)(() => OnCollectionChanged(e)));
            //                    //}
            //                    //else
            //                    AddPendingOperation(e, disp);
            //                }
            //                else
            //                {
            //                    if (e.Action == NotifyCollectionChangedAction.Add)
            //                        e = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems);
            //                    else if (e.Action == NotifyCollectionChangedAction.Remove)
            //                        e = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems);

            //                    // Execute handler as is.
            //                    handler(this, e);
            //                }
            //            }
            //        }
            //    }
            //}

            //private void AddPendingOperation(NotifyCollectionChangedEventArgs e, Dispatcher disp)
            //{
            //    var operation = disp.BeginInvoke(DispatcherPriority.DataBind,
            //                                            (Action)(() => OnCollectionChanged(e)));
            //    operation.Completed += (s, o) =>
            //    {
            //        lock (lockObject)
            //        {
            //            var currentqueue = Interlocked.Decrement(ref _operationsQueueCount);

            //            if (currentqueue == 0 && observableTimer != null)
            //            {
            //                observableTimer.Dispose();
            //                observableTimer = null;
            //            }

            //            object t;
            //            var s1 = (DispatcherOperation)s;
            //            _operations.TryRemove(s1, out t);
            //        }
            //    };

            //    var current = Interlocked.Increment(ref _operationsQueueCount);
            //    if (current == 1)
            //    {
            //        if (_operations == null)
            //            _operations = new ConcurrentDictionary<DispatcherOperation, object>();
            //        //if (observableTimer == null)
            //        //    observableTimer = System.Linq.Observable.Interval(TimeSpan.FromMilliseconds(2000)).Subscribe(MonitorDispatcherQueue);
            //    }

            //    _operations.TryAdd(operation, e);
            //}


            // ----------------------------------------------------------------------
        public bool IsUpdating
        {
            get { return updateCount > 0; }
        } // IsSetuping

        // ----------------------------------------------------------------------
        // see http://mokosh.co.uk/post/2009/08/04/how-to-sort-observablecollection/
        private List<T> ItemsAsList
        {
            get { return Items as List<T>; }
        } // ItemsAsList

        // ----------------------------------------------------------------------
        bool lockHolder = false;
        public virtual void BeginUpdate()
        {
            //lock (_syncObj)
            //{
            //    lockHolder = updateCount == 0;
            //    updateCount++;
            //}

            //if (lockHolder)
            //{
            //    System.Diagnostics.Debug.WriteLine("Acquiring _syncObj on {0}", Thread.CurrentThread.ManagedThreadId);
            //    // Monitor.Enter(_syncObj);
            //    System.Diagnostics.Debug.WriteLine("Acquired _syncObj on {0}", Thread.CurrentThread.ManagedThreadId);
            //}
        } // BeginUpdate

        // ----------------------------------------------------------------------
        public virtual void EndUpdate()
        {
            //lock (_syncObj)
            //{
            //    if (updateCount == 0 || !lockHolder)
            //        return;
            //    updateCount--;
            //    if (updateCount > 0)
            //        return;
            //    lockHolder = false;
            //}

            //System.Diagnostics.Debug.WriteLine("UpdatingCollection _syncObj on {0}", Thread.CurrentThread.ManagedThreadId);
            //UpdateCollectionChanged();
            //System.Diagnostics.Debug.WriteLine("Leaving _syncObj on {0}", Thread.CurrentThread.ManagedThreadId);
            //// Monitor.Exit(_syncObj);
            //System.Diagnostics.Debug.WriteLine("Left _syncObj on {0}", Thread.CurrentThread.ManagedThreadId);
        } // EndUpdate

        // ----------------------------------------------------------------------
        //public virtual void AddAll(IEnumerable<T> items)
        //{
        //    if (items == null)
        //    {
        //        throw new ArgumentNullException("items");
        //    }

        //    foreach (T item in items)
        //    {
        //        Add(item);
        //    }
        //} // AddAll

        // ----------------------------------------------------------------------
        //public virtual void Replace(T oldItem, T newItem)
        //{
        //    int index = IndexOf(oldItem);
        //    if (index < 0)
        //    {
        //        throw new ArgumentException("oldItem");
        //    }

        //    Items[index] = newItem;
        //    DisposeItem(oldItem);
        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        //        NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        //} // Replace

        // ----------------------------------------------------------------------
        //public virtual void Sort()
        //{
        //    ItemsAsList.Sort();
        //    // UpdateCollectionChanged();
        //} // Sort

        // ----------------------------------------------------------------------
        //public virtual void Sort(Comparison<T> comparison)
        //{
        //    if (comparison == null)
        //    {
        //        throw new ArgumentNullException("comparison");
        //    }

        //    ItemsAsList.Sort(comparison);
        //    // UpdateCollectionChanged();
        //} // Sort

        // ----------------------------------------------------------------------
        //public virtual void Sort(IComparer<T> comparer)
        //{
        //    if (comparer == null)
        //    {
        //        throw new ArgumentNullException("comparer");
        //    }

        //    ItemsAsList.Sort(comparer);
        //    // UpdateCollectionChanged();
        //} // Sort

        // ----------------------------------------------------------------------
        //public virtual void Sort(int index, int count, IComparer<T> comparer)
        //{
        //    if (comparer == null)
        //    {
        //        throw new ArgumentNullException("comparer");
        //    }

        //    ItemsAsList.Sort(index, count, comparer);
        //    // UpdateCollectionChanged();
        //} // Sort

        // ----------------------------------------------------------------------
        //public void Dispose()
        //{
        //    if (updateCount > 0)
        //    {
        //        throw new InvalidOperationException("unbalanced collection dispose");
        //    }
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //} // Dispose

        // ----------------------------------------------------------------------
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //if (observableTimer != null)
        //        //{
        //        //    observableTimer.Dispose();
        //        //    observableTimer = null;
        //        //}

        //        DisposeItems(this);
        //    }
        //} // Dispose

        // ----------------------------------------------------------------------
        //protected override void ClearItems()
        //{
        //    DisposeItems(this);
        //    base.ClearItems();
        //} // ClearItems

        // ----------------------------------------------------------------------
        //protected override void RemoveItem(int index)
        //{
        //    DisposeItem(this[index]);
        //    base.RemoveItem(index);
        //} // RemoveItem

        // ----------------------------------------------------------------------
        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    if (IsUpdating)
        //    {
        //        // break the workflow to reduce the UI communication
        //        return;
        //    }

        //    base.OnCollectionChanged(e);
        //} // OnCollectionChanged

        // ----------------------------------------------------------------------
        //protected void UpdateCollectionChanged()
        //{
        //    /*
        //    if (IsUpdating)
        //    {
        //        bNeedUpdate = true;
        //        return;
        //    }
        //    if (!bNeedUpdate)
        //        return;
        //    bNeedUpdate = false;
        //     */
        //    // OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //    lock (_syncObj)
        //    {
        //        var list = pendingChanges.ToList();
        //        pendingChanges.Clear();
        //        list.ForEach(value =>
        //            {
        //                OnCollectionChanged(value);
        //            });
        //    }
        //} // UpdateCollectionChanged

        // ----------------------------------------------------------------------
        //private static void DisposeItems(IEnumerable items)
        //{
        //    foreach (T item in items)
        //    {
        //        DisposeItem(item);
        //    }
        //} // DisposeItems

        // ----------------------------------------------------------------------
        //private static void DisposeItem(T item)
        //{
        //    IDisposable disposable = item as IDisposable;
        //    if (disposable != null)
        //    {
        //        disposable.Dispose();
        //    }
        //} // DisposeItem

        // ----------------------------------------------------------------------
        // members
        private int updateCount;
    }
}
