//-------------------------------------------------------------------------------------------------
// <copyright file="GroupTypedListRecordsCollection.cs" company="syncfusion">
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

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GroupTypedListRecordsCollection : ITypedList, IBindingList, IDisposable
    {
        internal Group group;
        FlattenedFilteredRecordsInGroupCollection inner;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public GroupTypedListRecordsCollection(Group group)
        {
            this.group = group;
            inner = new FlattenedFilteredRecordsInGroupCollection(group);
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public Group Group
        {
            get
            {
                return group;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public Record this[int index]
        {
            get
            {
                return inner[index];
            }

            set
            {
                ////                Record r = value as Record;
                ////                Record f = inner[index];
                ////                f.BeginEdit();
                ////                foreach (FieldDescriptor fd in group.TableDescriptor.Fields)
                ////                    f.SetValue(fd, r.GetValue(fd));
                ////                f.EndEdit();
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void CopyTo(Record[] array, int index)
        {
            inner.CopyTo(array, index);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns FlattenedRecordsInGroupCollectionEnumerator</returns>
        /// <internalonly/>
        public FlattenedRecordsInGroupCollectionEnumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        #region ITypedList Members
        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects to find in the collection as bindable. This can be null.</param>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </returns>
        /// <internalonly/>
        public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return ((ITypedList)group.ParentTableDescriptor).GetItemProperties(listAccessors);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        /// <internalonly/>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return group.CategoriesToString();
        }

        #endregion

        #region IBindingList Members
        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void AddIndex(PropertyDescriptor property)
        {
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool AllowNew
        {
            get
            {
                return false; ////group.ParentTableDescriptor.AllowNew && group.ParentTable.SourceListAllowNew;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
            SortColumnDescriptorCollection sc = new SortColumnDescriptorCollection();
            sc.Add(property.Name, direction);
            group.ParentTableDescriptor.SortedColumns.InitializeFrom(sc);
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public PropertyDescriptor SortProperty
        {
            get
            {
                if (group.ParentTableDescriptor.SortedColumns.Count > 0)
                {
                    return this.GetItemProperties(null)[group.ParentTableDescriptor.SortedColumns[0].Name];
                }

                return null;
            }
        }

        /*
        public int Find(PropertyDescriptor property, object key)
        {
            if (group.ParentTableDescriptor.PrimaryKeyColumns.Count == 1 && group.ParentTableDescriptor.PrimaryKeyColumns[0].Name == property.Name)
            {
                int n = group.ParentTable.PrimaryKeySortedRecords.FindRecord(key);
                if (n != -1)
                {
                    Record r = group.ParentTable.PrimaryKeySortedRecords[n];
                    if (inner.CheckParentGroup(r))
                        return inner.IndexOf(r);
                }
            }
            else if (group.ParentTableDescriptor.SortedColumns.Count == 1 && group.ParentTableDescriptor.SortedColumns[0].Name == property.Name
                && group.Records.Count > 0)
                return group.Records.FindRecord(key);
            else
            {
                FieldDescriptor fd = group.ParentTableDescriptor.Fields[property.Name];
                if (fd != null)
                {
                    foreach (Record r in inner)
                    {
                        if (r.GetValue(fd) == key)
                            return inner.IndexOf(r);
                    }
                }
            }

            return -1;
        }*/

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// The index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor"/>.
        /// </returns>
        /// <internalonly/>
        public int Find(PropertyDescriptor property, object key)
        {
            if (property == null)
            {
                foreach (Record r in inner)
                {
                    object val = r.GetData();
                    if (val == key)
                    {
                        return inner.IndexOf(r);
                    }
                }

                return -1;
            }

            SortColumnDescriptorCollection pks = group.ParentTableDescriptor.PrimaryKeyColumns;
            int pkCount = pks.Count;
            if (pkCount == 1 && pks[0].Name == property.Name)
            {
                int n = group.ParentTable.PrimaryKeySortedRecords.FindRecord(key);
                if (n != -1)
                {
                    Record r = group.ParentTable.PrimaryKeySortedRecords[n];
                    if (inner.CheckParentGroup(r))
                    {
                        return inner.IndexOf(r);
                    }
                }
            }
            else if (pkCount > 1 && pks[pkCount - 1].Name == property.Name)
            {
                object[] keys = new object[pkCount];
                group.CategoryKeys.CopyTo(keys, 0);
                keys[pkCount - 1] = key;
                int n = group.ParentTable.PrimaryKeySortedRecords.FindRecord(keys);
                if (n != -1)
                {
                    Record r = group.ParentTable.PrimaryKeySortedRecords[n];
                    if (inner.CheckParentGroup(r))
                    {
                        return inner.IndexOf(r);
                    }
                }
            }
            else if (group.ParentTableDescriptor.SortedColumns.Count == 1 && group.ParentTableDescriptor.SortedColumns[0].Name == property.Name
                && group.Records.Count > 0)
            {
                return group.Records.FindRecord(key);
            }
            else
            {
                FieldDescriptor fd = group.ParentTableDescriptor.Fields[property.Name];
                if (fd != null)
                {
                    Type type = fd.GetPropertyType();
                    key = NullableHelper.ChangeType(key, type);
                    foreach (Record r in inner)
                    {
                        object val = r.GetValue(fd);
                        if (SortColumnComparer._Compare(key, val) == 0)
                        {
                            return inner.IndexOf(r);
                        }
                    }
                }
                else if (property is TableRecordDataPropertyDescriptor)
                {
                    foreach (Record r in inner)
                    {
                        if (r.GetData() == key)
                        {
                            return inner.IndexOf(r);
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool SupportsSorting
        {
            get
            {
                return true;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool IsSorted
        {
            get
            {
                return group.ParentTableDescriptor.SortedColumns.Count > 0;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool AllowRemove
        {
            get
            {
                return false; ////group.ParentTableDescriptor.AllowRemove && group.SourceListAllowRemove;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool SupportsSearching
        {
            get
            {
                return true;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public System.ComponentModel.ListSortDirection SortDirection
        {
            get
            {
                if (group.ParentTableDescriptor.SortedColumns.Count > 0)
                {
                    return group.ParentTableDescriptor.SortedColumns[0].SortDirection;
                }

                return ListSortDirection.Ascending;
            }
        }

        event System.ComponentModel.ListChangedEventHandler IBindingList.ListChanged
        {
            add { }
            remove { }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool SupportsChangeNotification
        {
            get
            {
                return false;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void RemoveSort()
        {
            group.ParentTableDescriptor.SortedColumns.Clear();
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>The item added to the list.</returns>
        /// <internalonly/>
        public object AddNew()
        {
            return null; //// group.ParentTable.AddNewRecord;
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool AllowEdit
        {
            get
            {
                return false; ////group.TableDescriptor.AllowEdit && group.SourceListAllowEdit;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void RemoveIndex(PropertyDescriptor property)
        {
            group.ParentTableDescriptor.PrimaryKeyColumns.Remove(property.Name);
        }

        #endregion

        #region IList Members

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void RemoveAt(int index)
        {
            ////            Record r = group.Records[index];
            ////            r.Delete();
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void Insert(int index, object value)
        {
            ////            Record r = value as Record;
            ////            Record f = group.AddNewRecord;
            ////            f.BeginEdit();
            ////            foreach (FieldDescriptor fd in group.TableDescriptor.Fields)
            ////                f.SetValue(fd, r.GetValue(fd));
            ////            f.EndEdit();
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void Remove(object value)
        {
            ////            ((Record) value).Delete();
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        /// <internalonly/>
        public bool Contains(object value)
        {
            return inner.Contains((Record)value);
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void Clear()
        {
            ////            group.SourceList.Clear();
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <internalonly/>
        public int IndexOf(object value)
        {
            return inner.IndexOf((Record)value);
        }

        /// <summary>For internal use.</summary>
        /// <returns>returns -1</returns>
        /// <internalonly/>
        public int Add(object value)
        {
            return -1;
            ////            Record r = value as Record;
            ////            Record f = group.AddNewRecord;
            ////            f.BeginEdit();
            ////            foreach (FieldDescriptor fd in group.TableDescriptor.Fields)
            ////                f.SetValue(fd, r.GetValue(fd));
            ////            f.EndEdit();
            ////            return group.GetRecordCount();
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }
        
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Record[])array, index);
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public void Dispose()
        {
            this.group = null;
        }

        #endregion
    }
}
