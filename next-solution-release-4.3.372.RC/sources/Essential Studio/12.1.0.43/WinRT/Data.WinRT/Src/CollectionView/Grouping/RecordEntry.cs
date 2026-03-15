#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
#if !WP7
    using Syncfusion.Dynamic;
#endif
#if WinRT
    using Syncfusion.Data.Extensions;
#endif
#if WPF
    using System.Data;
#endif
    /// <summary>
    /// Contains the underlying business object bound to <see cref="ICollectionViewAdv" /> instance. 
    /// Nested records can be specified / controlled using the PopulateChildView method.
    /// </summary>
    public class RecordEntry : NodeEntry, IComparable<RecordEntry>
    {
        private int uniqueKeyIdentifier = int.MinValue;
        // a unique identity provider variable, this just maintains a unique id for this record entry, used in BinarySearch for faster indexOf operations.
        private static int IDProvider = int.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        /// <param name="data">The data.</param>
        public RecordEntry(NodeEntry parent, int level, object data)
            : base(parent, level)
        {
            this.Data = data;
            this.IsRecords = true;
            // this.isExpanded = false;
            this.uniqueKeyIdentifier = IDProvider++;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.data = null;
                if (ChildViews != null)
                {
                    foreach (var item in ChildViews)
                    {
                        item.Value.Dispose();
                    }
                    this.ChildViews.Clear();
                    this.ChildViews = null;
                }
            }
        }

        private object data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        private bool isExpanded;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (this.isExpanded != value)
                {
                    if (value)
                    {
                        this.OnExpanded();
                    }
                    else
                    {
                        this.OnCollapsed();
                    }
                }
            }
        }

        /// <summary>
        /// Called when RecordEntry.IsExpanded = true;
        /// </summary>
        protected virtual void OnExpanded()
        {
            if (this.ChildViews == null)
                this.ChildViews = new Dictionary<string, NestedRecordEntry>();
            this.isExpanded = true;
        }

        /// <summary>
        /// Sets IsExpanded = false without any calls to the events or overrides.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            this.isExpanded = false;
        }

        /// <summary>
        /// Gets or sets the child views.
        /// </summary>
        /// <value>The child views.</value>
        public Dictionary<string, NestedRecordEntry> ChildViews
        {
            get;
            set;
        }

        /// <summary>
        /// Populates the child view.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="level">The level.</param>
        /// <param name="relationName">Name of the relation.</param>
        public void PopulateChildView(ICollectionViewAdv collectionView, int level, string relationName)
        {
            var nestedRecordEntry = new NestedRecordEntry(this, this.Level) { View = collectionView, NestedLevel = level };
            this.ChildViews.Add(relationName, nestedRecordEntry);
        }

        /// <summary>
        /// Populates the child view.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="level">The level.</param>
        /// <param name="relationName">Name of the relation.</param>
        /// <param name="isNestedExpanded">to determine nested record needs to expand or not</param>
        public void PopulateChildView(ICollectionViewAdv collectionView, int level, string relationName, bool isNestedExpanded)
        {
            var nestedRecordEntry = new NestedRecordEntry(this, this.Level) { View = collectionView, NestedLevel = level, IsNestedLevelExpanded = isNestedExpanded };
            this.ChildViews.Add(relationName, nestedRecordEntry);
        }
#if DEBUG
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
#if WP7
            if (this.data != null)
#else
            if (this.data != null && !DynamicHelper.CheckIsDynamicObject(data.GetType()))
#endif
            {
#if WPF
                var dataRowView = this.data as DataRowView;
                if (dataRowView != null)
                {
                    foreach (DataColumn dc in dataRowView.Row.Table.Columns)
                    {
                        sb.Append(string.Format("{0} : {1} ", dc.ColumnName, dataRowView[dc.ColumnName]));
                    }
                    return sb.ToString();
                }
#endif
                var pInfo = this.data.GetType().GetProperties();
                foreach (var pI in pInfo)
                {
                    sb.Append(string.Format("{0} : {1} ", pI.Name, pI.GetValue(this.data, null)));
                }
            }

            return sb.ToString();
        }
#endif

        #region IComparable<RecordEntry> Members

        public int CompareTo(RecordEntry other)
        {
            return this.uniqueKeyIdentifier - other.uniqueKeyIdentifier;
        }

        #endregion
    }

    /// <summary>
    /// Contains a list of nested records for each <see cref="Syncfusion.Windows.Data.RecordEntry"/> and nested <see cref="ICollectionViewAdv" /> instance.
    /// </summary>
    public class NestedRecordEntry : NodeEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NestedRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public NestedRecordEntry(NodeEntry parent, int level)
            : base(parent, level)
        {
        }

        /// <summary>
        /// Gets or sets the nested level.
        /// </summary>
        /// <value>The nested level.</value>
        public int NestedLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether nested record is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if nested record is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsNestedLevelExpanded
        {
            get;
            set;
        }
        private ICollectionViewAdv nestedView;
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public ICollectionViewAdv View
        {
            get
            {
                return this.nestedView;
            }
            set
            {
                this.nestedView = value;
            }
        }

        /// <summary>
        /// Gets the nested records.
        /// </summary>
        /// <value>The nested records.</value>
        public IList<RecordEntry> NestedRecords
        {
            get
            {
                if (this.nestedView != null)
                {
                    return this.nestedView.Records;
                }
                return null;
            }
        }
    }


    public interface IRecordsEntryList : IList<RecordEntry>, INotifyCollectionChanged, IDisposable
    {
        void SuspendUpdates();
        void ResumeUpdates();
        bool IsInSuspend { get; }
        /// <summary>
        /// Returns the index for the underlying record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        int IndexOfRecord(object data);
    }

    #region Indexer List

    public class ListIndexer<T> where T : RecordEntry, IComparable<T>, IDisposable
    {
        private List<T> internalList;
        private IList<T> referenceList;
        public ListIndexer(IList<T> list)
        {
            this.internalList = new List<T>();
            this.InitializeInternalList(list);
            if (list is INotifyCollectionChanged)
            {
                var notifyCollection = list as INotifyCollectionChanged;
                notifyCollection.CollectionChanged += OnCollectionChanged;
            }
            else
            {
                throw new InvalidOperationException("IList<> does not implements INotifyCollectionChanged");
            }
            this.referenceList = list;
        }

        public void Dispose()
        {
            if (this.referenceList != null)
            {
                this.UnwireReferenceList();
            }

            this.internalList.Clear();
            this.internalList = null;
            this.referenceList = null;
        }

        public void Suspend()
        {
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }
        }

        public void Resume()
        {
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
                this.internalList.Clear();
                //this.UnwireReferenceList();
                this.InitializeInternalList(this.referenceList);
            }
        }

        public bool IsInSuspend
        {
            get;
            private set;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSuspend)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int loc = e.NewStartingIndex;
                        foreach (T o in e.NewItems)
                        {
                            ProcessAdd(o, loc++);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (T o in e.OldItems)
                        {
                            ProcessRemove(o);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (T o in e.OldItems)
                        {
                            ProcessRemove(o);
                        }
                        int loc = e.NewStartingIndex;
                        foreach (T o in e.NewItems)
                        {
                            ProcessAdd(o, loc++);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        //this.UnwireReferenceList();
                        this.InitializeInternalList(this.referenceList);
                    }
                    break;
            }
        }

        private void UnwireReferenceList()
        {
            var notifyCollectionChanged = this.referenceList as INotifyCollectionChanged;
            notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
        }

        private void InitializeInternalList(IList<T> list)
        {
            if (this.internalList != null && this.internalList.Count > 0)
            {
                this.internalList.Clear();
            }
            for (int i = 0; i < list.Count; ++i)
            {
                T record = list[i];
                this.internalList.Add(record);
            }
            this.internalList = this.internalList.OrderBy(x => x.Data != null ? x.Data.GetHashCode() : -1).ToList();
        }

        private void ProcessRemove(T record)
        {
            int loc = Find(record);
            internalList.RemoveAt(loc);
        }

        private void ProcessAdd(T record, int location)
        {
            int loc = Find(record);
            if (loc < 0)
            {
                internalList.Insert(-(loc + 1), record);
            }
        }

        /// <summary>
        /// Finds the location of item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int Find(object item)
        {
            int loc = -1;

            if (internalList.Count > 0)
            {
                int c = -1;
                int top = 0;
                int bot = internalList.Count - 1;
                int mid = 0;
                int prevMid = 0;
                var compareHashCode = item != null ? item.GetHashCode() : 0;
                while (top <= bot && c != 0)
                {
                    mid = (top + bot) / 2;
                    var data = internalList[mid].Data;
                    c = (data != null ? data.GetHashCode() : 0).CompareTo(compareHashCode);
                    if (c > 0)
                    {
                        prevMid = mid;
                        bot = mid - 1;
                    }
                    else if (c < 0)
                    {
                        top = mid + 1;
                        prevMid = top;
                    }
                    else
                    {
                        if (!data.Equals(item))
                        {
                            c = -1;
                            top = mid + 1;
                            prevMid = top;
                        }
                    }
                }
                if (c != 0)
                {
                    mid = -prevMid - 1;
                }
                loc = mid;
            }

            return loc;
        }

        public RecordEntry GetRecordEntry(object item)
        {
            int loc = Find(item);
            if (loc > -1 && loc < internalList.Count)
            {
                return this.internalList[loc];
            }
            return null;
        }

        public int Find(T item)
        {
            return Find(item.Data);
        }

        public override string ToString()
        {
            return string.Format("Count = {0}", this.internalList.Count);
        }
    }

    internal class IndexObject<T> : IComparable<T> where T : RecordEntry, IComparable<T>
    {
        public IndexObject(int loc, T val)
        {
            this.Location = loc;
            this.Value = val;
        }

        public int Location { get; set; }

        public T Value { get; set; }

        #region IComparable  Members

        public int CompareTo(T other)
        {
            return this.CompareTo(other);
        }

        #endregion

        internal class IndexObjectComparer : IComparer<IndexObject<T>>
        {
            #region IComparer<IndexObject<T>> Members

            public int Compare(IndexObject<T> x, IndexObject<T> y)
            {
                return x.Value.Data.GetHashCode().CompareTo(y.Value.Data.GetHashCode());
            }

            #endregion
        }

    }

    #endregion

    public class RecordsEntryList : IRecordsEntryList
    {
        #region cTor

        private ObservableCollection<RecordEntry> internalList;
        private ListIndexer<RecordEntry> indicesList;

        public RecordsEntryList()
        {
            this.internalList = new ObservableCollection<RecordEntry>();
            this.indicesList = new ListIndexer<RecordEntry>(this.internalList);
        }

        public virtual void Dispose()
        {
            this.internalList.Clear();
            this.indicesList.Dispose();
        }

        #endregion

        public void SuspendUpdates()
        {
            if (!this.IsInSuspend)
            {
                this.indicesList.Suspend();
            }
        }

        public void ResumeUpdates()
        {
            if (this.IsInSuspend)
            {
                this.indicesList.Resume();
            }
        }

        public bool IsInSuspend
        {
            get
            {
                return this.indicesList.IsInSuspend;
            }
        }

        #region IList<RecordEntry> Members

        public virtual int IndexOf(RecordEntry item)
        {
            var index = this.internalList.IndexOf(item);
            return index;
        }

        public virtual int IndexOfRecord(object data)
        {
            int index = -1;
            if (data is RecordEntry)
                index = this.internalList.IndexOf((RecordEntry)data);
            else
            {
                var re = this.indicesList.GetRecordEntry(data);
                if (re != null)
                    index = this.internalList.IndexOf(re);
            }
            return index;
        }

        public void Insert(int index, RecordEntry item)
        {
            this.internalList.Insert(index, item);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove);
        }

        public virtual RecordEntry this[int index]
        {
            get
            {
                var record = this.internalList[index];
                return record;
            }
            set
            {
                this.internalList[index] = value;
                RaiseCollectionChanged(NotifyCollectionChangedAction.Replace);
            }
        }

        #endregion

        #region ICollection<RecordEntry> Members

        public void Add(RecordEntry item)
        {
            this.internalList.Add(item);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
        }

        public void Clear()
        {
            this.internalList.Clear();
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(RecordEntry item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(RecordEntry[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(RecordEntry item)
        {
            var recult = this.internalList.Remove(item);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove);
            return recult;
        }

        #endregion

        #region IEnumerable<RecordEntry> Members

        public IEnumerator<RecordEntry> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
        }
    }
}
