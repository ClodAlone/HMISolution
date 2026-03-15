#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

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


namespace Syncfusion.Collections
{
	/// <summary>
	/// An IBindingList that wraps a DataTable and provides optimized access to the rows of the datatable. Assign
	/// this list to a grid as DataSource to improve performance when inserting records into an existing
	/// table with many records.
	/// </summary>
	public class DataTableWrapperList : IBindingList, ITypedList
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

		/// <summary>
		/// Initializes a new object and attaches it to the wrapped DataTable.
		/// </summary>
		/// <param name="dt">The DataTable to be wrapped.</param>
		public DataTableWrapperList(DataTable dt)
		{
			this.dt = dt;
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			IBindingList bindingList = (IBindingList) dt.DefaultView;
			bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
#endif

			dt.RowChanging += new DataRowChangeEventHandler(dt_RowChanging);
			dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);

		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 ) 
		static MethodInfo mIndexOf;
#endif
		public static int ChangedRowIndex = -1;

		public static int GetDataTableRowIndexOf(DataRow row)
		{
			int rowIndex;

			//using (MeasureTime.Measure("GetDataTableRowIndexOf"))
			{
				if (ChangedRowIndex != -1)
					rowIndex = ChangedRowIndex;
				else
				{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 ) 
					rowIndex = row.Table.Rows.IndexOf(row);
#else
					if (mIndexOf == null)
						mIndexOf = typeof(DataRowCollection).GetMethod("IndexOf", BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Public);
					rowIndex = (int) mIndexOf.Invoke(row.Table.Rows, new object[] { row });
#endif
				}
			}
			return rowIndex;
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
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
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    int c = dt.Rows.Count-1;
                    if (ListChanged != null)
                        ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, c));
#endif
					break;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                case DataRowAction.Change:
					if (ListChanged != null)
					{
						int rowIndex = GetDataTableRowIndexOf(e.Row);
						ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, rowIndex));
					}
                    break;

                default:
                    if (ListChanged != null)
                        ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
                    break;
#endif
			}
		}

		private void dt_RowChanging(object sender, DataRowChangeEventArgs e)
		{
			switch (e.Action)
			{
				case DataRowAction.Add:
					inRowAdd = true;
					rowAdd = e.Row;
					break;
					//				case DataRowAction.Change:
					//				case DataRowAction.Commit:
					//				case DataRowAction.Delete:
					//				case DataRowAction.Rollback:
					//				case DataRowAction.Nothing:
					//					break;
			}
			//			Console.WriteLine("{0}: {1}", e.Action, GetRowId(e.Row));
		}

		#region IBindingList Members

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <param name="property"></param>
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
		/// <param name="property"></param>
		/// <param name="key"></param>
		/// <returns></returns>
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
		/// <returns></returns>
		public object AddNew()
		{
			addRow = dt.NewRow();
            dt.Rows.Add(addRow);
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
		/// <param name="property"></param>
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
					return rowAdd;
				return dt.Rows[index];
			}
			set
			{
				throw new NotSupportedException("Read-only collection");
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
			dt.Rows.InsertAt((DataRow) value, index);
		}

		/// <summary>
		/// Removes the specified row object from the collection.
		/// </summary>
		/// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
		/// in the collection, the method will do nothing.</param>
		public void Remove(object value)
		{
			dt.Rows.Remove((DataRow) value);
		}

		/// <summary>
		/// Always False, the method is not implemented yet.
		/// </summary>
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
			dt.Rows.Add((DataRow) value);
			return dt.Rows.Count-1;
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
					return dt.Rows.Count+1;
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
		/// <returns></returns>
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
		/// <param name="listAccessors"></param>
		/// <returns></returns>
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (properties == null)
			{
				ArrayList pds = new ArrayList();
				foreach (DataColumn column in this.dt.Columns)
				{
					pds.Add(new DataTableColumnPropertyDescriptor(column));
				}
				properties = new PropertyDescriptorCollection((PropertyDescriptor[]) pds.ToArray(typeof(PropertyDescriptor)));
			}
			return properties;
		}

		/// <summary>
		/// Returns DataTable.TableName.
		/// </summary>
		/// <param name="listAccessors"></param>
		/// <returns></returns>
		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return dt.TableName;
		}

		#endregion

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
		/// <param name="dataColumn"></param>
		public DataTableColumnPropertyDescriptor(DataColumn dataColumn)
			: base(dataColumn.ColumnName, null)
		{
			this.column = dataColumn;
		}

		/// <override/>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}


		/// <override/>
		public override void SetValue(object component, object value)
		{
			DataRow row = (DataRow) component;
			row[column] = value;
			this.OnValueChanged(row, EventArgs.Empty);
		}


		/// <override/>
		public override void ResetValue(object component)
		{
			DataRow row = (DataRow) component;
			row[column] = DBNull.Value;
			this.OnValueChanged(row, EventArgs.Empty);
		}


		/// <override/>
		public override object GetValue(object component)
		{
			DataRow row = (DataRow) component;
			return row[column];
		}


		/// <override/>
		public override bool CanResetValue(object component)
		{
			DataRow row = (DataRow) component;
			return row[column] != DBNull.Value;
		}


		/// <override/>
		public override Type PropertyType
		{
			get
			{
				return this.column.DataType;
			}
		}

		/// <override/>
		public override bool IsReadOnly
		{
			get
			{
				return this.column.ReadOnly;
			}
		}

		/// <override/>
		public override Type ComponentType
		{
			get
			{
				return typeof(System.Data.DataRow);
			}
		}

		/// <override/>
		public override bool IsBrowsable
		{
			get
			{
				return this.column.ColumnMapping != MappingType.Hidden;
			}
		}

		/// <override/>
		public override int GetHashCode()
		{
			return this.Column.GetHashCode();
		}


		/// <override/>
		public override bool Equals(object other)
		{
			if ((other is DataTableColumnPropertyDescriptor))
			{
				DataTableColumnPropertyDescriptor other0 = (DataTableColumnPropertyDescriptor) other;
				return (other0.Column == this.Column);
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
