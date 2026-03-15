//-------------------------------------------------------------------------------------------------
// <copyright file="SourceListSet.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Provides access to the underlying source lists for entries in the <see cref="Engine.SourceListSet"/>.
    /// SourceListSetEntry classes are used by the <see cref="RelationDescriptor"/> to look up related lists
    /// for a parent table at run-time and to determine schema information for the table descriptor of the
    /// related table. If the engine's datasource is a DataView or DataTable, SourceListSetEntry entries are
    /// populated from the DataTable and DataView objects within the DataViewManager or DataSet.
    /// </summary>
    public class SourceListSetEntry : IDisposable
    {
        private string name = string.Empty;
        private IEnumerable list;

        /// <summary>
        /// Initializes a new empty SourceListSetEntry.
        /// </summary>
        public SourceListSetEntry()
        {
        }

        /// <summary>
        /// Initializes a new SourceListSetEntry with a name and the list that is referenced.
        /// </summary>
        /// <param name="name">The name of the list. This name is used to look up the entry.</param>
        /// <param name="list">The list this item references and provides access for.</param>
        public SourceListSetEntry(string name, IEnumerable list)
        {
            if (name == null)
            {
                name = string.Empty;
            }

            this.list = list;
            this.name = name;
        }

        /// <summary>
        /// Initializes a new SourceListSetEntry with a name and the list that is referenced.
        /// </summary>
        /// <param name="list">The list this item references and provides access for. The name of the list is determined
        /// by testing the list for implementation of ITypedList interface which then has a GetName property.</param>
        public SourceListSetEntry(IEnumerable list)
        {
            this.list = list;
            this.name = GetListName(list);
            if (name == null)
            {
                name = string.Empty;
            }
        }

        /// <summary>
        /// The name of the list. This name is used to look up the entry.
        /// </summary>
        public string Name
        {
            get
            {                
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    if (name == null)
                    {
                        name = string.Empty;
                    }

                    this.RaiseNameChanged();
                }
            }
        }

        internal bool ShouldSerializeName()
        {
            return this.name != GetListName(list);
        }

        /// <summary>
        /// Resets the name of the list back to its original value. The original name of the list is determined
        /// by testing the list for implementation of the ITypedList interface which then has a GetName property.
        /// </summary>
        public void ResetName()
        {
            this.name = GetListName(list);
        }

        internal void RaiseNameChanged()
        {
            OnNameChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the <see cref="Name"/> is changed.
        /// </summary>
        public event EventHandler NameChanged;

        string GetListName(object list)
        {
            if (this.list is ITypedList)
            {
                string s = ((ITypedList)list).GetListName(null);
                return s == null ? string.Empty : s;
            }

            return string.Empty;
        }

        /// <summary>
        /// Raises the  <see cref="name"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnNameChanged(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(this, e);
            if (NameChanged != null)
            {
                NameChanged(this, e);
            }
        }

        /// <summary>
        /// The list that this entry references and provides access for.
        /// </summary>
        public IEnumerable List
        {
            get
            {
                return this.list;
            }

            set
            {
                if (this.list != value)
                {
                    if (!this.ShouldSerializeList())
                    {
                        this.name = GetListName(value);
                    }

                    this.list = value;
                    this.RaiseListChanged();
                }
            }
        }

        internal bool ShouldSerializeList()
        {
            return false;  //// not supported
        }

        /// <summary>
        /// Resets the <see cref="List"/> to NULL.
        /// </summary>
        public void ResetList()
        {
            this.list = null;
        }

        internal void RaiseListChanged()
        {
            OnListChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the <see cref="List"/> is replaced.
        /// </summary>
        public event EventHandler ListChanged;

        /// <summary>
        /// Raises the <see cref="list"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnListChanged(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(this, e);
            if (ListChanged != null)
            {
                ListChanged(this, e);
            }
        }
        #region IDisposable Members

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            this.list = null;
        }
        #endregion
    }

    /// <summary>
    /// This collection is only meant to be initialized at run-time. It does not
    /// support design-time. It also does not raise events when it is changed;
    /// it should be setup only once after Engine.SetSourceList is called.
    /// It listens to the engine.SourceListChanged event and checks if it can get to
    /// a "DataSet". If that is the case, then the DataSet.Tables collection is 
    /// copied over.
    /// </summary>
    [ListBindableAttribute(false)]
    public class SourceListSet : ICollection, IDisposable, IStandardValuesProvider
    {
        internal Hashtable inner;
        Engine engine;
        int version;
        DataSet dataSet;
        bool clearAllWhenEngineSourceListChanged = true;
        bool entriesAddedBeforeSetDataSource = false;
        bool firstTimereinitializingWarning = true;
        
        /// <summary>
        /// A Read-only empty collection.
        /// </summary>
        public static readonly SourceListSet Empty = new SourceListSet(null);

        /// <summary>
        /// Initializes a new SourceListSet that belongs to the specified engine.
        /// </summary>
        /// <param name="engine">The engine this set belongs to.</param>
        public SourceListSet(Engine engine)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            inner = new Hashtable();
            this.engine = engine;
            if (engine != null)
            {
                engine.SourceListChanged += new EventHandler(engine_SourceListChanged);
            }

            version = 0;
        }

        /// <summary>
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection was modified.
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// The underlying DataSet
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                return dataSet;
            }
        }

        /// <summary>
        /// Populates the entries from a DataSet.
        /// </summary>
        /// <param name="ds">The DataSet.</param>
        public void InitializeFromDataSet(DataSet ds)
        {
            if (ds == null)
            {
                return;
            }

            if (clearAllWhenEngineSourceListChanged)
            {
                Clear();
            }

            foreach (DataTable dt in ds.Tables)
            {
                if (this.engine.AllowSwapDataViewWithDataTableList)
                {
                    this.Add(new SourceListSetEntry(dt.TableName, new DataTableList(dt)));
                }
                else
                {
                    this.Add(new SourceListSetEntry(dt.TableName, dt.DefaultView));
                }
            }

            ds.Tables.CollectionChanged += new CollectionChangeEventHandler(Tables_CollectionChanged);
            ds.Disposed += new EventHandler(ds_Disposed);
            this.dataSet = ds;
            version++;
        }

        /// <summary>
        /// Gets / sets the entry with the specified name.
        /// </summary>
        public SourceListSetEntry this[string name]
        {
            get
            {
                SourceListSetEntry entry = (SourceListSetEntry)inner[name];
                return entry;
            }

            set
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                version++;
                inner[name] = value;
            }
        }

        /// <summary>
        /// Determines if the element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            return inner.ContainsKey(name);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(SourceListSetEntry value)
        {
            if (value == null)
            {
                return false;
            }

            return inner.ContainsValue(value);
        }

        /// <summary>
        /// Returns a strong-typed enumerator.
        /// </summary>
        /// <returns>A strong-typed enumerator.</returns>
        public SourceListSetEnumerator GetEnumerator()
        {
            return new SourceListSetEnumerator(this);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(SourceListSetEntry value)
        {
            version++;
            inner.Remove(value);
        }

        /// <summary>
        /// Adds a SourceListSetEntry to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(SourceListSetEntry value)
        {
            if (!engine.HasSourceList())
            {
                this.entriesAddedBeforeSetDataSource = true;
            }

            ////TraceUtil.TraceCurrentMethodInfo();
            inner[value.Name] = value;
            return inner.Count;
        }

        /// <summary>
        /// Creates a SourceListSetEntry and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the entry.</param>
        /// <param name="list">The IList.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, IEnumerable list)
        {
            return Add(new SourceListSetEntry(name, list));
        }

        /// <summary>
        /// Creates a SourceListSetEntry and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the entry.</param>
        /// <param name="list">The IListSource.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, IListSource list)
        {
            return Add(new SourceListSetEntry(name, list.GetList()));
        }

        /// <summary>
        /// Creates a SourceListSetEntry and adds it to the end of the collection.
        /// </summary>
        /// <param name="list">The IList.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(IEnumerable list)
        {
            return Add(new SourceListSetEntry(list));
        }

        /// <summary>
        /// Creates a SourceListSetEntry and adds it to the end of the collection.
        /// </summary>
        /// <param name="list">The IListSource.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(IListSource list)
        {
            return Add(new SourceListSetEntry(list.GetList()));
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            version++;
            inner.Clear();
            this.UnwireDataset(this.dataSet);
            this.dataSet = null;
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(SourceListSetEntry[] array, int index)
        {
            inner.CopyTo(array, index);
        }

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((SourceListSetEntry[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// When you assign a new datasource to the Engine, the SourceListSet still holds references
        /// to the previous list. When this property is True, the SourceListSet will check if the new
        /// datasource has an entry in the SourceListSet. If it is a new DataSource, the SourceListSet 
        /// will clear itself and then add the new datasource. This is necessary to avoid memory leaks. If 
        /// the clear causes problems (e.g. a Relation does not show up), you can force the SourceListSet
        /// never to clear its contents when the Engine.SetSourceList is called or Engine.DataSource is changed.
        /// </summary>
        [DefaultValue(true)]
        public bool ClearAllWhenEngineSourceListChanged
        {
            get
            {
                return clearAllWhenEngineSourceListChanged;
            }

            set
            {
                clearAllWhenEngineSourceListChanged = value;
            }
        }

        private void engine_SourceListChanged(object sender, EventArgs e)
        {
            IEnumerable list = engine.GetSourceList();

            foreach (SourceListSetEntry entry in this)
            {
                if (entry.List == list)
                {
                    return; // no need to clear out entries - this table is already registered 
                }
            }

            if (this.entriesAddedBeforeSetDataSource)
            {
                this.entriesAddedBeforeSetDataSource = false;
                return;
            }

            if (this.Count > 0 && !firstTimereinitializingWarning)
            {
                firstTimereinitializingWarning = false;
                Console.WriteLine("engine_SourceListChanged reinitializing SourceListSet");
            }

            // Auto initialize if list is DataSet.
            if (list is DataTable)
            {
                this.InitializeFromDataSet(((DataTable)list).DataSet);
            }
            else if (list is DataView)
            {
                DataView dv = (DataView)list;
                DataViewManager dm = dv.DataViewManager;
                if (dm != null)
                {
                    this.InitializeFromDataSet(dm.DataSet);
                }
                else if (dv.Table != null)
                {
                    this.InitializeFromDataSet(dv.Table.DataSet);
                    Add(new SourceListSetEntry(list));
                }
                else
                {
                    Add(new SourceListSetEntry(list));
                }
            }
            else if (list is DataViewManager)
            {
                this.InitializeFromDataSet(((DataViewManager)list).DataSet);
            }
            else if (list is DataSet)
            {
                this.InitializeFromDataSet(((DataSet)list));
            }
            else if (list is DataTableList)
            {
                this.InitializeFromDataSet(((DataTableList)list).DataTable.DataSet);
            }
            else
            {
                // A new main list has been specified. It only makes sense to assume
                // that other entries are obsolete now.

                // NOTE: Make a documentation note that when changing Engine.SourceList
                // will clear the sourcelist collection. You first have to set Engine.SourceList
                // and then add any SourceList entries
                if (clearAllWhenEngineSourceListChanged)
                {
                    Clear();
                }

                if (list != null)
                {
                    Add(new SourceListSetEntry(list));
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and entries.
        /// </summary>
        public void Dispose()
        {
            if (engine != null)
            {
                engine.SourceListChanged -= new EventHandler(engine_SourceListChanged);
            }

            this.engine = null;
            foreach (SourceListSetEntry entry in this.inner.Values)
            {
                entry.Dispose();
            }

            this.inner.Clear();
            UnwireDataset(this.dataSet);
            this.dataSet = null;

            GC.SuppressFinalize(this);
        }
        #endregion

        #region IStandardValuesProvider Members

        /// <summary>
        /// Provide collection of choices for RelationDescriptor.ChildTableName.
        /// </summary>
        /// <param name="pd">The property descriptor.</param>
        /// <returns>A list of choices for child table name.</returns>
        public ICollection GetStandardValues(PropertyDescriptor pd)
        {
            string[] values = new string[this.Count];
            inner.Keys.CopyTo(values, 0);
            return values;
        }
        #endregion

        private void Tables_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            DataTable dt = e.Element as DataTable;
            if (dt != null)
            {
                this.Add(new SourceListSetEntry(dt.TableName, dt.DefaultView));
            }
        }

        private void ds_Disposed(object sender, EventArgs e)
        {
            DataSet ds = (DataSet)sender;
            UnwireDataset(ds);
        }

        void UnwireDataset(DataSet ds)
        {
            if (ds != null)
            {
                ds.Disposed -= new EventHandler(ds_Disposed);
                ds.Tables.CollectionChanged -= new CollectionChangeEventHandler(Tables_CollectionChanged);
            }
        }
    }

    /// <summary>
    /// Enumerator class for <see cref="SourceListSetEntry"/> elements of a <see cref="SourceListSet"/>.
    /// </summary>
    public class SourceListSetEnumerator : IEnumerator
    {
        IDictionaryEnumerator inner;
        SourceListSet _coll;

        /// <summary>
        /// Initializes an enumerator for a <see cref="SourceListSet"/>.
        /// </summary>
        /// <param name="collection">The <see cref="SourceListSet"/> to enumerate.</param>
        public SourceListSetEnumerator(SourceListSet collection)
        {
            _coll = collection;
            inner = _coll.inner.GetEnumerator();
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            inner.Reset();
        }

        object IEnumerator.Current
        {
            get
            {
                return inner.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public SourceListSetEntry Current
        {
            get
            {
                return (SourceListSetEntry)((DictionaryEntry)inner.Current).Value;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return inner.MoveNext();
        }
        #endregion
    }
}
