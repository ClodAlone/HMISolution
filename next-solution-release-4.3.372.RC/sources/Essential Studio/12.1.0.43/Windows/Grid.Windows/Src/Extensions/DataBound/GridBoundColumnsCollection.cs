//-------------------------------------------------------------------------------------------------
// <copyright file="GridBoundColumnsCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using System.Text;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Represents a collection of <see cref="GridBoundColumn"/> objects in the <see cref="GridDataBoundGrid"/> control.
    /// </summary>
    /// <remarks>
    /// On the <see cref="GridDataBoundGrid"/> you access the GridBoundColumnsCollection through the <see cref="GridDataBoundGrid.GridBoundColumns"/> property.
    /// <para/>
    /// The GridBoundColumnsCollection uses standard <see cref="Add"/> and <see cref="Remove"/>
    /// methods to manipulate the collection.
    /// Use the Contains method to determine if a specific property value exists in the collection.
    /// <para/>
    /// Additionally, use the IndexOf method to determine the index of any <see cref="GridBoundColumn"/> object
    /// within the collection.
    /// </remarks>
    [ListBindableAttribute(false)]
    [Editor(typeof(GridBoundColumnsCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridBoundColumnsCollection : BaseCollection, IList, ICollection, IEnumerable, ICloneable
    {
        /// <summary>
        /// Occurs when members in this collection have been added or removed.
        /// </summary>
        public event CollectionChangeEventHandler CollectionChanged;

        private ArrayList items = new ArrayList();
        private ICurrencyManagerSource owner = null;
        private bool isDefault = false;

        /// <override/>
        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Creates a new <see cref="GridBoundColumnsCollection"/> and creates copies of all members in this collection.
        /// </summary>
        /// <returns>A <see cref="GridBoundColumnsCollection"/> object.</returns>
        public virtual object Clone()
        {
            GridBoundColumnsCollection clone = new GridBoundColumnsCollection(this.owner, isDefault);
            foreach (GridBoundColumn item in this)
            {
                clone.Add((GridBoundColumn)item); ////.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Creates a new item for this collection. Override this method if you want
        /// to support derived column types. See <see cref="GridModelDataBinder.BoundColumnsCollectionType"/> for a sample.
        /// </summary>
        /// <param name="pd">The property descriptor with information about the column.</param>
        /// <returns>A <see cref="GridBoundColumn"/> or derived class object.</returns>
        public virtual GridBoundColumn CreateBoundColumn(PropertyDescriptor pd)
        {
            return new GridBoundColumn(pd);
        }

        /// <overload>
        /// Gets a specified <see cref="GridBoundColumn"/> in the GridBoundColumnsCollection. 
        /// </overload>
        /// <summary>
        /// Gets a specified <see cref="GridBoundColumn"/> in the <see cref="GridBoundColumnsCollection"/>. 
        /// </summary>
        public GridBoundColumn this[int index]
        {
            get
            {
                return (GridBoundColumn)items[index];
            }
        }

        /// <summary>
        /// Gets a specified <see cref="GridBoundColumn"/> in the GridBoundColumnsCollection. 
        /// </summary>
        public GridBoundColumn this[string columnName]
        {
            get
            {
                int i = items.Count;
                for (int j = 0; j < i; j++)
                {
                    GridBoundColumn GridBoundColumn = (GridBoundColumn)items[j];
                    if (String.Compare(GridBoundColumn.MappingName, columnName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        return GridBoundColumn;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a specified <see cref="GridBoundColumn"/> in the GridBoundColumnsCollection. 
        /// </summary>
        public GridBoundColumn this[PropertyDescriptor propDesc]
        {
            get
            {
                int i = items.Count;
                for (int j = 0; j < i; j++)
                {
                    GridBoundColumn GridBoundColumn = (GridBoundColumn)items[j];
                    if (propDesc.Equals(GridBoundColumn.PropertyDescriptor))
                    {
                        return GridBoundColumn;
                    }
                }

                return null;
            }
        }

        internal GridModelDataBinder OwnerDB
        {
            get
            {
                return owner as GridModelDataBinder;
            }
        }

        /// <summary>
        /// Gets the owner that also provides a reference to a CurrencyManager.
        /// </summary>
        public ICurrencyManagerSource Owner
        {
            get
            {
                return owner;
            }
        }

        int IList.Add(object value)
        {
            return Add((GridBoundColumn)value);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return items.IndexOf(value);
        }

        /// <summary>
        /// Inserts a column at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="column">The column.</param>
        public void Insert(int index, GridBoundColumn column)
        {
            column.SetOwner(this.owner);
            column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
            column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
            items.Insert(index, column);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridBoundColumn)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridBoundColumn)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return items[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   <para>Copies the values to a one-dimensional <see cref="System.Array" /> instance at the
        /// specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the <see cref="GridCellModelCollection" />.</param>
        /// <param name="index">The index in the array where copying begins.</param>
        public void CopyTo(GridBoundColumn[] array, int index)
        {
            ((ICollection)this).CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get
            {
                return items.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridBoundColumnsCollection"/>.
        /// </summary>
        /// <param name="table">A reference to the <see cref="GridModelDataBinder"/>
        /// this collection belongs to.</param>
        public GridBoundColumnsCollection(ICurrencyManagerSource table)
        {
            owner = table;
        }

        internal GridBoundColumnsCollection(ICurrencyManagerSource table, bool isDefault)
            : this(table)
        {
            this.isDefault = isDefault;
        }

        internal GridBoundColumn MapColumnStyleToPropertyName(string mappingName)
        {
            int i = items.Count;
            for (int j = 0; j < i; j++)
            {
                GridBoundColumn GridBoundColumn = (GridBoundColumn)items[j];
                if (String.Compare(GridBoundColumn.MappingName, mappingName, true, CultureInfo.InvariantCulture) == 0)
                {
                    return GridBoundColumn;
                }
            }

            return null;
        }

        internal void CheckForMappingNameDuplicates(GridBoundColumn column)
        {
            if (GridUtil.IsEmpty(column.MappingName))
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (((GridBoundColumn)items[i]).MappingName.Equals(column.MappingName) && column != items[i])
                {
                    throw new ArgumentException("DuplicateMappingName", "column");
                }
            }
        }

        private void ColumnStyleMappingNameChanged(object sender, EventArgs pcea)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        private void ColumnStylePropDescChanged(object sender, EventArgs pcea)
        {
            ////OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (GridBoundColumn)sender));
        }

        /// <summary>
        /// Adds a <see cref="GridBoundColumn"/> to the collection.
        /// </summary>
        /// <param name="column">The <see cref="GridBoundColumn"/> to add.</param>
        /// <returns>The index of the new <see cref="GridBoundColumn"/> object.</returns>
        public virtual int Add(GridBoundColumn column)
        {
            if (isDefault)
            {
                throw new ArgumentException("DataGridDefaultColumnCollectionChanged");
            }

            CheckForMappingNameDuplicates(column);
            column.SetOwner(this.owner);
            column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
            column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);
            int i = items.Add(column);
            if (!inAddRange)
            {
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
            }

            return i;
        }

        bool inAddRange = false;

        /// <summary>
        /// Adds an array of <see cref="GridBoundColumn"/> objects to the collection.
        /// </summary>
        /// <param name="columns">An array of DataGridColumnStyle objects to add to the collection. </param>
        public void AddRange(GridBoundColumn[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            inAddRange = true;
            for (int i = 0; i < (int)columns.Length; i++)
            {
                Add(columns[i]);
            }

            inAddRange = false;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, null));
        }

        internal void AddDefaultColumn(GridBoundColumn column)
        {
            items.Add(column);
            column.SetOwner(this.owner);
        }

        internal void ResetDefaultColumnCollection()
        {
            items.Clear();
        }

        /// <summary>
        /// Clears the collection of <see cref="GridBoundColumn"/> objects.
        /// </summary>
        public void Clear()
        {
            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GridBoundColumnsCollection"/> contains a specific <see cref="GridBoundColumn"/> associated with the specified PropertyDescriptor.
        /// </summary>
        /// <param name="propDesc">The <see cref="PropertyDescriptor"/> associated with the desired <see cref="GridBoundColumn"/>.</param>
        /// <returns>True if the collection contains the <see cref="GridBoundColumn"/>; otherwise, False.</returns>
        public bool Contains(PropertyDescriptor propDesc)
        {
            return this[propDesc] != null;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GridBoundColumnsCollection"/> contains a specific <see cref="GridBoundColumn"/>.
        /// </summary>
        /// <param name="column">The <see cref="GridBoundColumn"/> to find.</param>
        /// <returns>True if the collection contains the <see cref="GridBoundColumn"/>; otherwise, False.</returns>
        public bool Contains(GridBoundColumn column)
        {
            return items.IndexOf(column) == -1 == false;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GridBoundColumnsCollection"/> contains a specific <see cref="GridBoundColumn"/> with a specific name.
        /// </summary>
        /// <param name="name">The <see cref="GridBoundColumn.MappingName"/> of the desired <see cref="GridBoundColumn"/>.</param>
        /// <returns>True if the collection contains the <see cref="GridBoundColumn"/>; otherwise, False.</returns>
        public bool Contains(string name)
        {
            IEnumerator iEnumerator = items.GetEnumerator();
            while (iEnumerator.MoveNext())
            {
                if (String.Compare(((GridBoundColumn)iEnumerator.Current).MappingName, name, true, CultureInfo.InvariantCulture) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the index of a specified <see cref="GridBoundColumn"/>.
        /// </summary>
        /// <param name="element">The <see cref="GridBoundColumn"/> to find.</param>
        /// <returns>The zero-based index of the <see cref="GridBoundColumn"/> within the <see cref="GridBoundColumnsCollection"/> or -1 if no corresponding <see cref="GridBoundColumn"/> exists.</returns>
        public int IndexOf(GridBoundColumn element)
        {
            int i = items.Count;
            for (int j = 0; j < i; j++)
            {
                GridBoundColumn GridBoundColumn = (GridBoundColumn)items[j];
                if (element == GridBoundColumn)
                {
                    return j;
                }
            }

            return -1;
        }

        /// <summary>
        /// Raises a <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="ccevent">Event data.</param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
        {
#if DEBUG
            if (Switches.GridBoundColumnEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ccevent.Action, ccevent.Element);
            }
#else
            ;
#endif
            if (CollectionChanged != null)
            {
                CollectionChanged(this, ccevent);
            }
        }

        /// <summary>
        /// Removes the specified <see cref="GridBoundColumn"/> from the <see cref="GridBoundColumnsCollection"/>.
        /// </summary>
        /// <param name="column">The <see cref="GridBoundColumn"/> to remove.</param>
        public void Remove(GridBoundColumn column)
        {
            if (isDefault)
            {
                throw new ArgumentException("DataGridDefaultColumnCollectionChanged");
            }

            int i = -1;
            int j = items.Count;
            for (int k = 0; k < j; k++)
            {
                if (items[k] == column)
                {
                    i = k;
                    break;
                }
            }

            if (i == -1)
            {
                throw new InvalidOperationException("DataGridColumnCollectionMissing");
            }

            RemoveAt(i);
        }

        /// <summary>
        /// Removes the <see cref="GridBoundColumn"/> with the specified index from the <see cref="GridBoundColumnsCollection"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="GridBoundColumn"/>.</param>
        public void RemoveAt(int index)
        {
            if (isDefault)
            {
                throw new ArgumentException("DataGridDefaultColumnCollectionChanged");
            }

            GridBoundColumn column = (GridBoundColumn)items[index];
            column.MappingNameChanged -= new EventHandler(ColumnStyleMappingNameChanged);
            column.PropertyDescriptorChanged -= new EventHandler(ColumnStylePropDescChanged);
            column.SetOwner(this.owner);
            items.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));
        }

        /// <summary>
        /// Resets all <see cref="GridBoundColumn.PropertyDescriptor"/> for all <see cref="GridBoundColumn"/> objects
        /// in the collection.
        /// </summary>
        public void ResetPropertyDescriptors()
        {
            for (int i = 0; i < base.Count; i++)
            {
                this[i].PropertyDescriptor = null;
            }
        }
    }
}
