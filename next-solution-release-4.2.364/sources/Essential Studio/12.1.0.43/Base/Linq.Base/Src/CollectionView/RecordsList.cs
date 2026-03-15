#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Windows.Data;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Globalization;
    using System.Reflection;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using Syncfusion.Windows.Data;
#if !SILVERLIGHT
    using Syncfusion.ComponentModel;
#endif

    /// <summary>
    /// Implements <see cref="IRecordsList"/> interface to enumerate the records
    /// data structure for <see cref="CollectionViewAdv"/>.
    /// </summary>
    public class EnumerableRecordsWrapper : IRecordsList
    {
        /// <summary>
        /// Create a new instance of <see cref="EnumerableRecordsWrapper"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public static EnumerableRecordsWrapper CreateNew(IEnumerable source, CollectionViewAdv view)
        {
            var wrapper = new EnumerableRecordsWrapper(source, view);
            return wrapper;
        }

        private ObservableCollection<RecordEntry> internalList;
        private ListIndexer<RecordEntry> indicesList;
        private bool isLegacyDataTable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableRecordsWrapper"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        protected EnumerableRecordsWrapper(CollectionViewAdv view)
        {
            this.View = view;
#if !SILVERLIGHT
            this.isLegacyDataTable = view.IsLegacyDataTable;
#endif
            this.TableSummaries = new List<SummaryRecordEntry>();
            this.internalList = new ObservableCollection<RecordEntry>();
            this.indicesList = new ListIndexer<RecordEntry>(this.internalList);
            this.WireEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableRecordsWrapper"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="view">The view.</param>
        public EnumerableRecordsWrapper(IEnumerable source, CollectionViewAdv view)
            : this(view)
        {
            this.View = view;
            this.SuspendUpdates();
            this.View.FilteredRecord = new List<RecordEntry>();
            if (!this.View.IsGrouping)
            {
                IEnumerator enumerator = null;
                if (source != null)
                    enumerator = source.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        if (this.View.PassesFilter(enumerator.Current))
                        {


                            //This code for paging support. This code take the current page record
                            if (this.View.EnablePaging && this.View.PagedSource != null)
                            {
                                if (this.View.PagedSource.Contains(enumerator.Current))
                                {
                                    this.Add(this.CreateRecord(enumerator.Current));
                                }

                            }

                            else
                            {
                                this.Add(this.CreateRecord(enumerator.Current));
                            }
                        }
                        else
                        {
                            this.View.FilteredRecord.Add(this.CreateRecord(enumerator.Current));
                        }
                    }
                }
            }
            else
            {
                this.EnsureRecordsFromTopLevelGroup();
            }
            this.ResumeUpdates();
        }

        /// <summary>
        /// Populates the records from top level group.
        /// </summary>
        private void EnsureRecordsFromTopLevelGroup()
        {
            this.PopulateRecordsFromGroup(this.View.TopLevelGroup.Groups);
        }

        private void PopulateRecordsFromGroup(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    foreach (var record in group.Records)
                    {
                        if (this.View.PassesFilter(record.Data))
                        {
                            this.Add(record);
                        }
                        else
                        {
                            this.View.FilteredRecord.Add(record);
                        }
                    }
                }
                else
                {
                    this.PopulateRecordsFromGroup(group.Groups);
                }
            }
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected virtual RecordEntry CreateRecord(object data)
        {
            RecordEntry record = this.View.CreateRecordEntry(data);
            if (record == null)
            {
                record = new RecordEntry(null, -1, data);
            }

            return record;
        }

        private void WireEvents()
        {
#if !SILVERLIGHT
            //if (!this.View.IsLegacyDataTable)
            {
                this.internalList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
#else
            this.internalList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
#endif
        }

        private void UnwireEvents()
        {
#if !SILVERLIGHT
            //if (!this.View.IsLegacyDataTable)
            {
                this.internalList.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
#else
            this.internalList.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
#endif
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        private bool isNotifyPropertyTagged = false;

        /// <summary>
        /// Called when collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
//            switch (e.Action)
//            {
//                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
//                    {
//#if !SILVERLIGHT
//                        var pd = this.View.GetItemProperties();
//#endif
//                        foreach (RecordEntry recordEntry in e.NewItems)
//                        {
//#if !SILVERLIGHT
//                            foreach (var mappingname in this.ComplexProperties)
//                            {
//                                var kvp = pd.GetPropertyDescriptor(recordEntry.Data, mappingname.ToString());
//                                var record = new RecordEntry(null, -1, kvp.Value);

//                                //The below code is commented because while using complex property binding in the GridDataVisiblecolumn, this condition is checked everytime.so the loading time is very high.
//                                //because everytime new record is creating, so always this will return true. so no need to check this If clause. (Please Refer #80817)

//                                //if (!this.complexRecordList.Contains(record))
//                                this.complexRecordList.Add(record);
//                                this.AddNotifyListener(record);
//                            }
//#endif
//                            this.AddNotifyListener(recordEntry);

//                        }
//                    }
//                    break;
//                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
//                    {
//                        foreach (RecordEntry recordEntry in e.OldItems)
//                        {
//#if !SILVERLIGHT
//                            foreach (RecordEntry recEntry in e.OldItems)
//                            {
//                                var pdc = this.View.GetItemProperties();
//                                foreach (var mappingname in this.ComplexProperties)
//                                {
//                                    var kvp = pdc.GetPropertyDescriptor(recEntry.Data, mappingname.ToString());
//                                    var recEntryinList = this.complexRecordList.Where(d => d.Data == kvp.Value).FirstOrDefault();
//                                    if (recEntryinList != null && recEntryinList != (default(RecordEntry)))
//                                    {
//                                        var notify = recEntryinList.Data as INotifyPropertyChanged;
//                                        if (notify != null)
//                                        {
//                                            PropertyChangedEventManager.RemoveListener(notify, this.View, string.Empty);
//                                            this.complexRecordList.Remove(recEntryinList);
//                                        }
//                                    }
//                                }
//                            }
//#endif
//                            RemoveNotifyListener(recordEntry);
//                        }
//                    }
//                    break;
//            }

            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void AddNotifyListener(RecordEntry recordEntry)
        {
            var notifyPropertyChanged = recordEntry.Data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                if (!this.isNotifyPropertyTagged)
                {
                    this.isNotifyPropertyTagged = true;
                }
#if !SILVERLIGHT
                PropertyChangedEventManager.AddListener(notifyPropertyChanged, this.View, string.Empty);
#else
                notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
#endif
            }
#if !SILVERLIGHT
            else
            {
                if (recordEntry.Data is ICustomTypeDescriptor)
                {
                    foreach (PropertyDescriptor property in this.View.GetItemProperties())
                    {
                        property.AddValueChanged(recordEntry.Data, OnPropertyChanged);
                    }
                }
            }
            var notifyPropertyChanging = recordEntry.Data as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
            {
                PropertyChangingEventManager.AddListener(notifyPropertyChanging, this.View, string.Empty);
            }
#endif
        }

        private void RemoveNotifyListener(RecordEntry recordEntry)
        {
            var notifyPropertyChanged = recordEntry.Data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
#if !SILVERLIGHT
                PropertyChangedEventManager.RemoveListener(notifyPropertyChanged, this.View, string.Empty);
#else
                notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
#endif
            }
#if !SILVERLIGHT
            else
            {
                if (recordEntry.Data is ICustomTypeDescriptor)
                {
                    foreach (PropertyDescriptor property in this.View.GetItemProperties())
                    {
                        property.RemoveValueChanged(recordEntry.Data, OnPropertyChanged);
                    }
                }
            }
            var notifyPropertyChanging = recordEntry.Data as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
            {
                PropertyChangingEventManager.RemoveListener(notifyPropertyChanging, this.View, string.Empty);
            }
#endif
        }

#if !SILVERLIGHT
        private void OnPropertyChanged(object sender, EventArgs e)
        {
            var propertyChangedHandler = this.View as IWeakEventListener;
            if (propertyChangedHandler != null)
                propertyChangedHandler.ReceiveWeakEvent(typeof (PropertyChangedEventManager), sender, e);
        }
#endif

#if SILVERLIGHT

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyChangedHandler = this.View as IPropertyChangedEventHandler;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler.OnPropertyChanged(sender, e);
            }
        }

#endif
        /// <summary>
        /// Creates the record entry.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public RecordEntry CreateRecordEntry(object data)
        {
            return this.CreateRecord(data);
        }

        private RecordEntry GetRecord(object record, Group group)
        {
            RecordEntry item = null;
            foreach (var innergroup in group.Groups)
            {
                if (this.CheckKey(innergroup, record))
                {
                    if (innergroup.IsBottomLevel)
                    {
                        foreach (var originalrecord in innergroup.Records)
                        {

                            if (originalrecord.Data == record)
                            {
                                item = originalrecord;
                                break;
                            }
                        }
                    }
                    else
                    {
                        item = GetRecord(record, innergroup);
                    }

                    if (item != null)
                    {
                        break;
                    }
                }

            }
            return item;
        }

        /// <summary>
        /// Adds the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Add(object data)
        {
            var record = this.CreateRecord(data);
            this.Add(record);
        }

        private bool CheckKey(Group group, object record)
        {
            var pgd = this.View.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            //var itemProperties = this.View.GetItemProperties();            
            var provider = this.View.GetPropertyAccessProvider();
            if (provider == null)
            {
                return false;
            }

            var value = provider.GetValue(record, pgd.PropertyName);
            if (!(group.Key is DBNull) && group.Key != null)
            {
                var c = ((IComparable)group.Key).CompareTo((IComparable)value);
                if (c == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public CollectionViewAdv View
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the table summaries.
        /// </summary>
        /// <value>The table summaries.</value>
        public IList<SummaryRecordEntry> TableSummaries
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the index for the underlying record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int IndexOfRecord(object data)
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

        /// <summary>
        /// Gets the item at index specified.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        /// <returns></returns>
        public object GetItemAt(int recordIndex)
        {
            if (!(recordIndex < 0))
            {
                var item = this[recordIndex];
                if (item != null)
                {
                    return item.Data;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Data.RecordEntry"/> for the underlying business object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public RecordEntry GetRecord(object data)
        {
            return this.indicesList.GetRecordEntry(data);

//            foreach (RecordEntry record in this.internalList)
//            {
//#if !SILVERLIGHT
//                if (this.isLegacyDataTable)
//                {
//                    if (((System.Data.DataRowView)record.Data).Row == (((System.Data.DataRowView)data).Row))
//                    {
//                        return record;
//                    }
//                }
//                else
//                {
//#endif
//                    if (record.Data == data)
//                    {
//                        return record;
//                    }
//#if !SILVERLIGHT
//                }
//#endif
//            }
//            return null;
        }


        #region IList<RecordEntry> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(RecordEntry item)
        {
            return this.internalList.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, RecordEntry item)
        {
            this.internalList.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Data.RecordEntry"/> at the specified index.
        /// </summary>
        /// <value></value>
        public RecordEntry this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<RecordEntry> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(RecordEntry item)
        {
            this.internalList.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            if (this.isNotifyPropertyTagged)
            {
                for (int i = 0; i < this.internalList.Count; i++)//var rec in this.internalList)
                {
                    var rec = this.internalList[i];
                    rec.Dispose();
                }
                this.internalList.Clear();
            }
        }

        public void RemoveNotifyListener()
        {
            if (this.isNotifyPropertyTagged)
            {
                foreach (var rec in this.internalList)
                    this.RemoveNotifyListener(rec);
            }
        }

        /// <summary>
        /// Dispose all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void ReomveAll()
        {
            if (this.isNotifyPropertyTagged)
            {
                foreach (var rec in this.internalList)
                {
                    rec.Dispose();
                }
                this.internalList.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(RecordEntry item)
        {
            return this.internalList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(RecordEntry[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return this.internalList.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(RecordEntry item)
        {
            return this.internalList.Remove(item);
        }

        #endregion

        #region IEnumerable<RecordEntry> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<RecordEntry> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
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
            return ((IEnumerable)this.internalList).GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.internalList != null)
            {
                this.Clear();
                this.UnwireEvents();
                this.internalList = null;

                this.indicesList.Dispose();
                this.indicesList = null;
            }

            if (this.TableSummaries != null)
            {
                this.TableSummaries.Clear();
                this.TableSummaries = null;
            }

            if (View != null)
                View = null;
          
        }

        #endregion

        #region IRecordsEntryList Members

        public void SuspendUpdates()
        {
            this.indicesList.Suspend();
        }

        public void ResumeUpdates()
        {
            this.indicesList.Resume();
        }

        public bool IsInSuspend
        {
            get { return this.indicesList.IsInSuspend; }
        }

        #endregion
    }

#if SILVERLIGHT
    public sealed class Comparer : IComparer
    {
        // Fields
        public static readonly Comparer Default = new Comparer(CultureInfo.CurrentCulture);
        public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);
        private CompareInfo m_compareInfo;

        // Methods
        private Comparer()
        {
            this.m_compareInfo = null;
        }

        public Comparer(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            this.m_compareInfo = culture.CompareInfo;
        }

        public int Compare(object a, object b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a == null)
            {
                return -1;
            }
            if (b == null)
            {
                return 1;
            }
            if (this.m_compareInfo != null)
            {
                string str = a as string;
                string str2 = b as string;
                if ((str != null) && (str2 != null))
                {
                    return this.m_compareInfo.Compare(str, str2);
                }
            }
            IComparable comparable = a as IComparable;
            if (comparable == null)
            {
                throw new ArgumentException("Object should implement IComparable");
            }
            return comparable.CompareTo(b);
        }
    }

#endif

    internal class SortFieldComparer : IComparer<object>
    {
        // Fields
        private Comparer _comparer;
        private SortPropertyInfo[] _fields;
        internal SortDescriptionCollection SortFields;
        private Func<object, string, object> propertyInfoFunc;
        private Dictionary<string, IComparer<object>> comparers;

        internal SortFieldComparer(SortDescriptionCollection sortFields, Dictionary<string, IComparer<object>> comparers, CultureInfo culture, Func<object, string, object> propertyInfoFunc)
        {
            this.SortFields = sortFields;
            this.propertyInfoFunc = propertyInfoFunc;
            this._fields = this.CreatePropertyInfo(this.SortFields);
            this._comparer = ((culture == null) || (culture == CultureInfo.InvariantCulture)) ? Comparer.DefaultInvariant : ((culture == CultureInfo.CurrentCulture) ? Comparer.Default : new Comparer(culture));
            this.comparers = comparers;
        }

        public int Compare(object x, object y)
        {
            var o1 = x as RecordEntry;
            var o2 = y as RecordEntry;
            var data1 = o1 != null ? o1.Data : x;
            var data2 = o2 != null ? o2.Data : y;
            int num = 0;
            for (int i = 0;i < this._fields.Length;i++)
            {
#if !SILVERLIGHT
                if (data1 is System.Data.DataRowView)
                {
                    System.Data.DataRowView d1 = data1 as System.Data.DataRowView;
                    System.Data.DataRowView d2 = data2 as System.Data.DataRowView;

                    if (d1.Row.RowState == System.Data.DataRowState.Detached && d2.Row.RowState == System.Data.DataRowState.Detached)
                    {
                        num = 0;
                        break;
                    }
                    else if (d1.Row.RowState == System.Data.DataRowState.Detached)
                    {
                        num = -1;
                        break;
                    }
                    else if (d2.Row.RowState == System.Data.DataRowState.Detached)
                    {
                        num = 1;
                        break;
                    }
                }
#endif
                object a = this.propertyInfoFunc(data1, this._fields[i].propName);
                object b = this.propertyInfoFunc(data2, this._fields[i].propName);

                IComparer<object> customComparer = null;
                this.comparers.TryGetValue(this._fields[i].propName, out customComparer);

                if (customComparer != null)
                {
                    num = customComparer.Compare(data1, data2);
                }
                else
                {
                    if (a == null || a == DBNull.Value)
                    {
                        num = (b == null || b is DBNull) ? 0 : -1;
                    }
                    else if (b == null || b is DBNull)
                    {
                        num = 1; //a cannot be null or dbnull herer
                    }
                    else
                    {
                        num = this._comparer.Compare(a, b);
                    }
                }

                if (this._fields[i].descending)
                {
                    num = -num;
                }

                //if (data1 == data2)
                //{
                //    return -1;
                //}
                if (num != 0)
                    break;
                else
                    continue;

                //if (num != 0)
                //{
                //    return num;
                //}
            }
            return num;
        }

        private SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortFields)
        {
            SortPropertyInfo[] infoArray = new SortPropertyInfo[sortFields.Count];
            for (int i = 0;i < sortFields.Count;i++)
            {
                //#if !SILVERLIGHT
                //                PropertyDescriptor pd = null;
                //#else
                //                PropertyInfo pd = null;
                //#endif
                //                if (string.IsNullOrEmpty(description.PropertyName))
                //                {
                //                    pd = null;
                //                }
                //                else if(this._pdc != null)
                //                {
                //                    SortDescription description2 = sortFields[i];
                //                    pd = this._pdc.GetPropertyDescriptor(description2.PropertyName);//.Find(description2.PropertyName, false); //new PropertyPath(description2.PropertyName, new object[0]);
                //                }
                SortDescription description = sortFields[i];
                infoArray[i].propName = description.PropertyName;
                infoArray[i].descending = description.Direction == ListSortDirection.Descending;
            }
            return infoArray;
        }

        // Nested Types
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        private struct SortPropertyInfo
        {
            internal string propName;
            internal bool descending;
            //internal object GetValue(object o)
            //{
            //    if (this.info == null)
            //    {
            //        return o;
            //    }

            //    return this.info.GetValue(o);
            //}
        }
    }
}
