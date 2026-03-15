//-------------------------------------------------------------------------------------------------
// <copyright file="DataTableList.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Data;
using System.Reflection;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// An IBindingList that wraps a DataTable and provides optimized access to the rows of the datatable.
    /// The Engine will access a DataTable through this wrapper class instead of accessing records through the DataTable.DefaultView
    /// to increase performance when adding, removing, and changing records when <see cref="Engine.AllowSwapDataViewWithDataTableList"/>
    /// is enabled.
    /// </summary>
    public class DataTableList : IBindingList, ITypedList, IGroupingList, IGroupingColumnChanging, IDisposable
    {
        DataTable dt;
        bool inRowAdd = false;
        DataRow rowAdd = null;

        /// <summary>
        /// The underlying DataTable.
        /// </summary>
        public DataTable DataTable
        {
            get
            {
                return dt;
            }
        }

#if !SyncfusionFramework2_0
        IBindingList bindingList;
#endif

        /// <summary>
        /// Initializes a new object and attaches it to the wrapped DataTable.
        /// </summary>
        /// <param name="dt">The DataTable to be wrapped.</param>
        public DataTableList(DataTable dt)
        {
            this.dt = dt;
#if !SyncfusionFramework2_0
            bindingList = (IBindingList) dt.DefaultView;
            bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
#endif

            dt.RowChanging += new DataRowChangeEventHandler(dt_RowChanging);
            dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);
            dt.ColumnChanging += new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowDeleting += new DataRowChangeEventHandler(dt_RowDeleting);
            dt.RowDeleted += new DataRowChangeEventHandler(dt_RowDeleted);
        }

        void dt_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
#if SyncfusionFramework2_0
            if (ListChanged != null)
            {
                int rowIndex = deleteRowIndex;
                ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, rowIndex));
            }
#endif
        }

        void dt_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
#if SyncfusionFramework2_0
            deleteRowIndex = GetDataTableRowIndexOf(e.Row);
#endif
            if (this.RowRemoving != null)
            {
                this.RowRemoving(this, new GroupingRowEventArgs(e.Row));
            }
        }

        public void Dispose()
        {
#if !SyncfusionFramework2_0
            if (bindingList != null)
                bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
#endif

            if (dt != null)
            {
                dt.RowChanging -= new DataRowChangeEventHandler(dt_RowChanging);
                dt.RowChanged -= new DataRowChangeEventHandler(dt_RowChanged);
                dt.ColumnChanging -= new DataColumnChangeEventHandler(dt_ColumnChanging);
                dt.RowDeleting -= new DataRowChangeEventHandler(dt_RowDeleting);
                dt.RowDeleted -= new DataRowChangeEventHandler(dt_RowDeleted);
            }
        }

        void dt_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            if (this.ColumnChanging != null)
            {
                GroupingColumnChangeEventArgs ge = new GroupingColumnChangeEventArgs(e.Row, e.Column.ColumnName, e.Row[e.Column], e.ProposedValue);
                this.ColumnChanging(this, ge);
                e.ProposedValue = ge.ProposedValue;
            }
        }

#if !SyncfusionFramework2_0 
        static MethodInfo mIndexOf;
#endif
        public static int ChangedRowIndex = -1;

        /// <summary>
        /// Returns the row index.
        /// </summary>
        /// <param name="row">Given row.</param>
        /// <returns>Row index.</returns>
        public static int GetDataTableRowIndexOf(DataRow row)
        {
            int rowIndex;

            ////using (MeasureTime.Measure("GetDataTableRowIndexOf"))
            {
                if (ChangedRowIndex != -1)
                {
                    rowIndex = ChangedRowIndex;
                }
                else
                {
#if SyncfusionFramework2_0
                    rowIndex = row.Table.Rows.IndexOf(row);
#else
                    if (mIndexOf == null)
                        mIndexOf = typeof(DataRowCollection).GetMethod("IndexOf", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    rowIndex = (int) mIndexOf.Invoke(row.Table.Rows, new object[] { row });
#endif
                }
            }

            return rowIndex;
        }
#if !SyncfusionFramework2_0
        private void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }
#endif
        private void dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Add:
                    inRowAdd = false;
                    rowAdd = e.Row;
#if SyncfusionFramework2_0
                    int c = dt.Rows.Count - 1;
                    if (ListChanged != null)
                    {
                        ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, c));
                    }
#endif
                    break;
#if SyncfusionFramework2_0
                case DataRowAction.Change:
                    if (ListChanged != null)
                    {
                        int rowIndex = GetDataTableRowIndexOf(e.Row);
                        ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, rowIndex));
                    }

                    break;

                default:
                    if (ListChanged != null)
                    {
                        ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
                    }

                    break;
#endif
            }
        }

        int deleteRowIndex;
        private void dt_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Add:
                    inRowAdd = true;
                    rowAdd = e.Row;
                    break;
                ////                case DataRowAction.Change:
                ////                case DataRowAction.Commit:
                ////                case DataRowAction.Delete:
                ////                case DataRowAction.Rollback:
                ////                case DataRowAction.Nothing:
                ////                    break;
            }
            ////            Console.WriteLine("{0}: {1}", e.Action, GetRowId(e.Row));
        }

        #region IBindingList Members

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddIndex(PropertyDescriptor property)
        {
        }

        /// <summary>
        /// Always True.
        /// </summary>
        public bool AllowNew
        {
            get
            {
                return true;
            }
        }

        void System.ComponentModel.IBindingList.ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Not implemented. Returns -1.
        /// </summary>
        /// <param name="property">The PropertyDescriptor</param>
        /// <param name="key">The key value</param>
        /// <returns>returns -1.</returns>
        public int Find(PropertyDescriptor property, object key)
        {
            return -1;
        }

        /// <summary>
        /// Always False.
        /// </summary>
        public bool SupportsSorting
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Always False.
        /// </summary>
        public bool IsSorted
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Always True.
        /// </summary>
        public bool AllowRemove
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Always False.
        /// </summary>
        public bool SupportsSearching
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public System.ComponentModel.ListSortDirection SortDirection
        {
            get
            {
                return ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Broadcasts the <see cref="IBindingList.ListChanged"/> event.
        /// </summary>
        public event System.ComponentModel.ListChangedEventHandler ListChanged;

        /// <summary>
        /// Always True.
        /// </summary>
        public bool SupportsChangeNotification
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void RemoveSort()
        {
        }

        DataRow addRow = null;

        /// <summary>
        /// Adds a new row (calling DataTable.NewRow) and returns the new object.
        /// </summary>
        /// <returns>The new row.</returns>
        public object AddNew()
        {
            addRow = dt.NewRow();
            return addRow;
        }

        /// <summary>
        /// Always True.
        /// </summary>
        public bool AllowEdit
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="property">The propertydescriptor</param>
        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        #endregion

        #region IList Members

        /// <summary>
        /// Always False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the element at the zero-based index. 
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        public object this[int index]
        {
            get
            {
                if (inRowAdd && index == dt.Rows.Count)
                {
                    return rowAdd;
                }

                return dt.Rows[index];
            }

            set
            {
                throw new NotSupportedException("Read-only collection.");
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            dt.Rows.RemoveAt(index);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, object value)
        {
            dt.Rows.InsertAt((DataRow)value, index);
        }

        /// <summary>
        /// Removes the specified row object from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(object value)
        {
            dt.Rows.Remove((DataRow)value);
        }

        /// <summary>
        /// Always False, the method is not implemented yet.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return false;
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            dt.Rows.Clear();
        }

        /// <summary>
        /// Always -1, the method is not implemented.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return -1;
        }

        /// <summary>
        /// Adds a row object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(object value)
        {
            dt.Rows.Add((DataRow)value);
            return dt.Rows.Count - 1;
        }

        /// <summary>
        /// Always False since this collection has no fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members
        
        /// <summary>
        /// Not implemented.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return dt.Rows.IsSynchronized;
            }
        }

        /// <summary>
        /// Returns the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get
            {
                if (inRowAdd)
                {
                    return dt.Rows.Count + 1;
                }

                return dt.Rows.Count;
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(Array array, int index)
        {
            dt.Rows.CopyTo(array, index);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return dt.Rows.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return dt.Rows.GetEnumerator();
        }

        #endregion

        #region ITypedList Members

        PropertyDescriptorCollection properties = null;

        /// <summary>
        /// Returns a PropertyDescriptorCollection that represents the DataTable.Columns collection. 
        /// </summary>
        /// <param name="listAccessors">Array of <see cref="PropertyDescriptor"/> objects.</param>
        /// <returns>Property descriptor collection.</returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (properties == null)
            {
                ArrayList pds = new ArrayList();
                foreach (DataColumn column in this.dt.Columns)
                {
                    pds.Add(new DataTableColumnPropertyDescriptor(column));
                }

                properties = new PropertyDescriptorCollection((PropertyDescriptor[])pds.ToArray(typeof(PropertyDescriptor)));
            }

            return properties;
        }

        /// <summary>
        /// Returns DataTable.TableName.
        /// </summary>
        /// <param name="listAccessors">Array of <see cref="PropertyDescriptor"/> objects.</param>
        /// <returns>Table name.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return dt.TableName;
        }

        #endregion

        #region IGroupingList Members

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="relationChildColumns">The RelationChildColumnDescriptorCollection</param>
        /// <param name="groupColumns">The group columns</param>
        /// <param name="sortColumns">The sort columns</param>
        public void ApplySort(RelationChildColumnDescriptorCollection relationChildColumns, SortColumnDescriptorCollection groupColumns, SortColumnDescriptorCollection sortColumns)
        {
        }

        /// <summary>
        /// Returns True.
        /// </summary>
        public bool AllowItemReference
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool SupportsGroupSorting
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public Syncfusion.Grouping.GroupingSortBehavior GroupingSortBehavior
        {
            get
            {
                return Syncfusion.Grouping.GroupingSortBehavior.GroupByGroup;
            }
        }

        #endregion

        #region IGroupingColumnChanging Members

        /// <summary>
        /// Occurs when a grouping column changes.
        /// </summary>
        public event GroupingColumnChangeEventHandler ColumnChanging;

        /// <summary>
        /// Occurs when a row gets removed.
        /// </summary>
        public event GroupingRowEventHandler RowRemoving;

        #endregion
    }

    public interface IGroupingColumnChanging
    {
        event GroupingColumnChangeEventHandler ColumnChanging;
        event GroupingRowEventHandler RowRemoving;
    }

    public delegate void GroupingColumnChangeEventHandler(object sender, GroupingColumnChangeEventArgs e);
    
    /// <summary>
    /// Provides data for the GroupingColumnChanging event.
    /// </summary>
    public class GroupingColumnChangeEventArgs : EventArgs
    {
        object row;
        string column;
        object value;
        object oldValue;

        /// <summary>
        /// Initializes a new instance of the GroupingColumnChangeEventArgs class.
        /// </summary>
        /// <param name="row">The Row of the column with the changing value.</param>
        /// <param name="column">The Column with the changing value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="value">The new value.</param>
        public GroupingColumnChangeEventArgs(object row, string column, object oldValue, object value)
        {
            this.row = row;
            this.column = column;
            this.value = value;
            this.oldValue = oldValue;
        }

        /// <summary>
        /// Gets Column with a changing value.
        /// </summary>
        public string Column 
        { 
            get { return column; } 
        }

        /// <summary>
        /// Gets or sets the proposed new value for the column.
        /// </summary>
        public object ProposedValue
        { 
            get 
            { 
                return this.value; 
            } 
            
            set 
            { 
                this.value = value; 
            } 
        }

        /// <summary>
        /// Gets the row of the column with a changing value.
        /// </summary>
        public object Row 
        { 
            get { return row; } 
        }

        /// <summary>
        /// Gets old value of the column.
        /// </summary>
        public object OldValue 
        {
            get { return oldValue; } 
        }
    }

    public delegate void GroupingRowEventHandler(object sender, GroupingRowEventArgs e);

    /// <summary>
    /// Provides data for the GroupingColumnChanging event.
    /// </summary>
    public class GroupingRowEventArgs : EventArgs
    {
        object row;

        /// <summary>
        /// Initializes a new instance of the GroupingColumnChangeEventArgs class.
        /// </summary>
        /// <param name="row">The Row of the column with the changing value.</param>
        public GroupingRowEventArgs(object row)
        {
            this.row = row;
        }

        /// <summary>
        /// Gets the row of the column with a changing value.
        /// </summary>
        public object Row 
        { 
            get 
            {
                return row; 
            } 
        }
    }

    /// <summary>
    /// A custom PropertyDescriptor that is used within a <see cref="DataTableWrapperList"/> to access
    /// a DataColumn.
    /// </summary>
    public class DataTableColumnPropertyDescriptor : PropertyDescriptor
    {
        private DataColumn column;

        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a DataColumn.
        /// </summary>
        /// <param name="dataColumn">The data column.</param>
        public DataTableColumnPropertyDescriptor(DataColumn dataColumn)
            : base(dataColumn.ColumnName, null)
        {
            this.column = dataColumn;
        }

        /// <summary>Indicates whether the component object can be serialized.</summary>
        /// <param name="component">The component object.</param>
        /// <returns>returns False.</returns>
        /// <override/>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>Sets a value to the specificed component.</summary>
        /// <param name="component">The component object.</param>
        /// <param name="value">New value.</param>
        /// <override/>
        public override void SetValue(object component, object value)
        {
            DataRow row = (DataRow)component;
            row[column] = value;
            this.OnValueChanged(row, EventArgs.Empty);
        }

        /// <summary>Resets the property value of a givne component.</summary>
        /// <param name="component">The component object.</param>
        /// <override/>
        public override void ResetValue(object component)
        {
            DataRow row = (DataRow)component;
            row[column] = DBNull.Value;
            this.OnValueChanged(row, EventArgs.Empty);
        }

        /// <summary>Gets the current value of the property on a component.</summary>
        /// <param name="component">The componenet object.</param>
        /// <returns>The property value.</returns>
        /// <override/>
        public override object GetValue(object component)
        {
            DataRow row = (DataRow)component;
            return row[column];
        }

        /// <summary>Indicates whether resetting an object resets its value.</summary>
        /// <param name="component">The object to test its reset capability.</param>
        /// <returns>True if it can be reset, False otherwise.</returns>
        /// <override/>
        public override bool CanResetValue(object component)
        {
            DataRow row = (DataRow)component;
            return row[column] != DBNull.Value;
        }

        /// <summary>Returns the property type.</summary>
        /// <override/>
        public override Type PropertyType
        {
            get
            {
                return this.column.DataType;
            }
        }

        /// <summary>Indicates whether the member is read-only.</summary>
        /// <override/>
        public override bool IsReadOnly
        {
            get
            {
                return this.column.ReadOnly;
            }
        }

        /// <summary>Returns the type of component.</summary>
        /// <override/>
        public override Type ComponentType
        {
            get
            {
                return typeof(System.Data.DataRow);
            }
        }

        /// <summary>Indicates whether the member is browsable.</summary>
        /// <override/>
        public override bool IsBrowsable
        {
            get
            {
                return this.column.ColumnMapping != MappingType.Hidden;
            }
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return this.Column.GetHashCode();
        }

        /// <summary>Indicates whether the object is equivalent to the given object.</summary>
        /// <param name="other">Another object to compare.</param>
        /// <returns>True if they are equivalent; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object other)
        {
            if (other is DataTableColumnPropertyDescriptor)
            {
                DataTableColumnPropertyDescriptor other0 = (DataTableColumnPropertyDescriptor)other;
                return other0.Column == this.Column;
            }

            return false;
        }
        
        /// <summary>
        /// Returns the DataColumn.
        /// </summary>
        public DataColumn Column
        {
            get
            {
                return this.column;
            }
        }
    }
}
