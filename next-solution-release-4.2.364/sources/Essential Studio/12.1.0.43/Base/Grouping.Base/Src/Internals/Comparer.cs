//-------------------------------------------------------------------------------------------------
// <copyright file="Comparer.cs" company="syncfusion">
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
using System.Data;

using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping.Internals
{
    /// <exclude/>
    internal sealed class TableRecordDataComparer : IComparer
    {
        IComparer _inner;
        bool isSorted;
        SortColumnDescriptor[] arrayOfColumnDescriptors;
        Table table;
        TableDescriptor td;
        PropertyDescriptor[] arrayOfPropertyDescriptor;

        public TableRecordDataComparer(Table table, IComparer inner)
        {
            this.table = table;
            this.td = table.TableDescriptor;
            _inner = inner;
            td.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
        }

        public int Compare(object x, object y)
        {
            ////            int cmp;
            ////            for (int n = 0; n < arrayOfPropertyDescriptor.Length; n++)
            ////            {
            ////                object cx = ((Record) x).sortKeys[n];
            ////                object cy = ((Record) y).sortKeys[n];
            ////                cmp = SortColumnComparer._Compare(arrayOfColumnDescriptors[n], cx, cy);
            ////                ////cmp = ((IComparable) cx).CompareTo(cy);
            ////                if (cmp != 0)
            ////                    return cmp;
            ////            }
            ////            return 0;
            return SortColumnComparer.Compare(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor, x, y);
        }
    }

    /// <exclude/>
    internal sealed class TablePrimaryKeyRecordDataComparer : IComparer
    {
        IComparer _inner;
        bool isSorted;
        SortColumnDescriptor[] arrayOfColumnDescriptors;
        Table table;
        TableDescriptor td;
        PropertyDescriptor[] arrayOfPropertyDescriptor;

        public TablePrimaryKeyRecordDataComparer(Table table, IComparer inner)
        {
            this.table = table;
            this.td = table.TableDescriptor;
            _inner = inner;
            td.GetPrimaryKeySortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
        }

        public int Compare(object x, object y)
        {
            return PrimaryKeyColumnComparer.Compare(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor, x, y);
        }
    }

    /// <exclude/>
    internal class RecordDataComparer : IComparer
    {
        IComparer _inner;

        public RecordDataComparer(IComparer inner)
        {
            _inner = inner;
        }

        public int Compare(object x, object y)
        {
            Record ry = (Record)y;
            if (x is Record)
            {
                Record rx = (Record)x;

                int cmp = 0;
                if (rx.ParentTableDescriptor.IsSorted)
                {
                    cmp = _inner.Compare(rx, ry);
                }

                if (cmp != 0)
                {
                    return cmp;
                }

                return rx.sourceIndex - ry.sourceIndex;
            }
            else 
            {
                //// This branch is used when called from RecordsInDetailsCollection.FindRecord.
                object[] ax = (object[])x;
                int n = 0;
                foreach (SortColumnDescriptor columnDescriptor in ry.ParentTableDescriptor.SortedColumns)
                {
                    if (n >= ax.Length)
                    {
                        break;
                    }

                    object cx = NullableHelper.ChangeType(ax[n], columnDescriptor.FieldDescriptor.GetPropertyType());
                    object cy = ry.GetValue(columnDescriptor);
                    int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);
                    if (cmp != 0)
                    {
                        return cmp;
                    }

                    n++;
                }

                return 0;
            }
        }
    }

    /// <exclude/>
    internal class PrimaryKeyRecordDataComparer : IComparer
    {
        IComparer _inner;

        public PrimaryKeyRecordDataComparer(IComparer inner)
        {
            _inner = inner;
        }

        public int Compare(object x, object y)
        {
            Record ry = (Record)y;
            if (x is Record)
            {
                Record rx = (Record)x;

                int cmp = ((SortColumnComparer)_inner).ComparePrimaryKey(rx, ry);
                if (cmp != 0)
                {
                    return cmp;
                }

                return rx.sourceIndex - ry.sourceIndex;
            }
            else 
            {
                //// This branch is used when called from *.FindRecord.
                object[] ax = (object[])x;
                int n = 0;
                foreach (SortColumnDescriptor columnDescriptor in ry.ParentTableDescriptor.PrimaryKeyColumns)
                {
                    if (n >= ax.Length)
                    {
                        break;
                    }

                    object cx = !(ax[n] == null || ax[n] is DBNull) ? NullableHelper.ChangeType(ax[n], columnDescriptor.FieldDescriptor.GetPropertyType()) : null;
                    object cy = ry.GetValue(columnDescriptor);
                    int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);
                    if (cmp != 0)
                    {
                        return cmp;
                    }

                    n++;
                }

                return 0;
            }
        }
    }

    /// <exclude/>
    internal interface IGroupByCategorizer
    {
        object GetGroupByCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, Record record);
        int CompareCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, object category, Record record);
        object[] GetCategoryForeignKeyParentIds(SortColumnDescriptor columnDescriptor, bool isForeignKey, Record record, out bool foreignKeyFieldsFound);
    }

    /// <exclude/>
    internal class GroupedColumnCategorizer : IGroupByCategorizer
    {
        SortColumnDescriptorCollection groupedColumns;

        public GroupedColumnCategorizer(SortColumnDescriptorCollection groupedColumns)
        {
            this.groupedColumns = groupedColumns;
        }

        public object GetGroupByCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, Record record)
        {
            if (columnDescriptor.Categorizer != null)
            {
                return columnDescriptor.Categorizer.GetGroupByCategoryKey(columnDescriptor, isForeignKey, record);
            }

            return record.GetValue(columnDescriptor);
        }

        public object[] GetCategoryForeignKeyParentIds(SortColumnDescriptor columnDescriptor, bool isForeignKey, Record record, out bool foreignKeyFieldsFound)
        {
            foreignKeyFieldsFound = false;

            // Resolve dependencies to other fields in the record if you group by a foreign key display name,
            // unbound field or a nested property field.
            FieldDescriptor fd = columnDescriptor.FieldDescriptor;
            if (fd == null)
            {
                return null;
            }

            if (fd.IsRelatedField())
            {
                foreignKeyFieldsFound = true;
                RelationDescriptor rd = fd.relation;
                if (rd.RelationKeys.Count > 0)
                {
                    // ForeignKeyReference
                    object[] cv = new object[rd.RelationKeys.Count];
                    for (int n = 0; n < cv.Length; n++)
                    {
                        cv[n] = record.GetValue(rd.RelationKeys[n].ParentKeyField);
                    }

                    return cv;
                }
                else if (rd.MappingName != string.Empty)
                {
                    // ListItemReference
                    object[] cv = new object[1];
                    cv[0] = record.GetValue(rd.MappingName);
                    return cv;
                }
            }
            else if (fd.ReferencedFields != null && fd.ReferencedFields != string.Empty)
            {
                // UnboundFields or nested property fields.
                foreignKeyFieldsFound = true;
                string rf = fd.ReferencedFields;
                string[] parts = rf.Split(';');
                object[] cv = new object[parts.Length];
                for (int n = 0; n < parts.Length; n++)
                {
                    cv[n] = record.GetValue(parts[n]);
                }

                return cv;
            }

            //// Note: There could be further dependencies. A referenced field or parent key field itsself could 
            //// be a unbound field. Recursive reference checking would be needed for that, but since this is a
            //// a extremely rare scenario this case was not implemented here. 
            ////
            //// Note: See the AddNewRecord.GetDefaultValue method for extracting the default values from the
            //// groups categories.

            return null;
        }

        public int CompareCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, object category, Record record)
        {
            if (columnDescriptor.Categorizer != null)
            {
                return columnDescriptor.Categorizer.CompareCategoryKey(columnDescriptor, isForeignKey, category, record);
            }

            object x = category;
            object y = record.GetValue(columnDescriptor);

            return SortColumnComparer._Compare(columnDescriptor, x, y);
        }
    }

    /// <exclude/>
    internal class GroupedColumnsComparer : IComparer
    {
        SortColumnDescriptorCollection columnDescriptors;

        public GroupedColumnsComparer(SortColumnDescriptorCollection columnDescriptors)
        {
            this.columnDescriptors = columnDescriptors;
        }

        public int Compare(object x, object y)
        {
            for (int n = 0; n < columnDescriptors.Count; n++)
            {
                SortColumnDescriptor columnDescriptor = columnDescriptors[n];
                ////object cx = x != null ? ((object[]) x)[n] : null;
                ////object cy = y != null ? ((object[]) y)[n] : null;
                object cx = ((object[])x)[n];
                object cy = ((object[])y)[n];

                int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return 0;
        }
    }

    /// <exclude/>
    internal sealed class SortColumnComparer : IComparer
    {
        TableDescriptor table;

        public SortColumnComparer(TableDescriptor table)
        {
            this.table = table;
        }

        public int Compare(object x, object y)
        {
            SortColumnDescriptor[] arrayOfColumnDescriptors;
            PropertyDescriptor[] arrayOfPropertyDescriptor;
            bool isSorted;

            table.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
            return SortColumnComparer.Compare(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor, x, y);
        }

        public int ComparePrimaryKey(object x, object y)
        {
            SortColumnDescriptor[] arrayOfColumnDescriptors;
            PropertyDescriptor[] arrayOfPropertyDescriptor;
            bool isSorted;

            table.GetPrimaryKeySortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

            Record ry = (Record)y;
            Record rx = (Record)x;
            int count = arrayOfColumnDescriptors.Length;
            for (int n = 0; n < count; n++)
            {
                object cx, cy;
                SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                cx = rx.primaryKeys[n];
                cy = ry.primaryKeys[n];
                int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return rx.sourceIndex - ry.sourceIndex;
        }

        public static int Compare(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, PropertyDescriptor[] arrayOfPropertyDescriptor, object x, object y)
        {
            return CompareColumns.CompareSortKeys(isSorted, arrayOfColumnDescriptors, x, y);
        }

        internal static int _Compare(SortColumnDescriptor columnDescriptor, object x, object y)
        {
            int cmp;
            if (columnDescriptor.Comparer != null)
            {
                cmp = columnDescriptor.Comparer.Compare(x, y);
            }
            else
            {
                cmp = _Compare(x, y);
            }

            if (columnDescriptor.SortDirection == ListSortDirection.Descending)
            {
                return -cmp;
            }

            return cmp;
        }

        internal static int _Compare(object x, object y)
        {
            int cmp = 0;
            bool xIsNull = x == null || x is DBNull;
            bool yIsNull = y == null || y is DBNull;

            if (yIsNull && xIsNull)
            {
                cmp = 0;
            }
            else if (xIsNull)
            {
                cmp = -1;
            }
            else if (yIsNull)
            {
                cmp = 1;
            }
            else if (x is IComparable && x.GetType() == y.GetType())
            {
                cmp = ((IComparable)x).CompareTo(y);
            }

            return cmp;
        }
    }

    /// <exclude/>
    internal sealed class PrimaryKeyColumnComparer : IComparer
    {
        TableDescriptor table;

        public PrimaryKeyColumnComparer(TableDescriptor table)
        {
            this.table = table;
        }

        public int Compare(object x, object y)
        {
            SortColumnDescriptor[] arrayOfColumnDescriptors;
            PropertyDescriptor[] arrayOfPropertyDescriptor;
            bool isSorted;

            table.GetPrimaryKeySortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
            return PrimaryKeyColumnComparer.Compare(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor, x, y);
        }

        public static int Compare(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, PropertyDescriptor[] arrayOfPropertyDescriptor, object x, object y)
        {
            return CompareColumns.ComparePrimaryKeys(isSorted, arrayOfColumnDescriptors, x, y);
        }
    }

    /// <exclude/>
    internal class GroupsDetailsSortColumnsComparer : IComparer
    {
        SortColumnDescriptor[] arrayOfColumnDescriptors;
        PropertyDescriptor[] arrayOfPropertyDescriptor;

        public GroupsDetailsSortColumnsComparer(SortColumnDescriptorCollection columnDescriptors)
        {
            if (columnDescriptors == null)
            {
                throw new ArgumentNullException("columnDescriptors");
            }

            arrayOfColumnDescriptors = new SortColumnDescriptor[columnDescriptors.Count];
            columnDescriptors.CopyTo(this.arrayOfColumnDescriptors, 0);

            arrayOfPropertyDescriptor = new PropertyDescriptor[arrayOfColumnDescriptors.Length];
            int count = arrayOfColumnDescriptors.Length;
            for (int n = 0; n < count; n++)
            {
                SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                if (columnDescriptor != null)
                {
                    FieldDescriptor fd = columnDescriptor.FieldDescriptor;
                    if (fd != null && fd.IsPropertyField())
                    {
                        arrayOfPropertyDescriptor[n] = fd.GetPropertyDescriptor();
                    }
                }
            }
        }

        public int Compare(object x, object y)
        {
            for (int n = 0; n < arrayOfColumnDescriptors.Length; n++)
            {
                SortColumnDescriptor columnDescriptor = this.arrayOfColumnDescriptors[n];
                if (columnDescriptor.Comparer != null)
                {
                    Record rx = ((Group)x).GetFirstRecord();
                    Record ry = ((Group)y).GetFirstRecord();
                    if (arrayOfPropertyDescriptor[n] == null || columnDescriptor.FieldDescriptor.IsComplexPropertyField())
                    {
                        x = rx.GetValue(columnDescriptor);
                        y = ry.GetValue(columnDescriptor);
                    }
                    else
                    {
                        x = arrayOfPropertyDescriptor[n].GetValue(rx.GetData());
                        y = arrayOfPropertyDescriptor[n].GetValue(ry.GetData());
                    }
                }
                else
                {
                    x = ((Group)x).CategoryKeys[n];
                    y = ((Group)y).CategoryKeys[n];
                }

                int cmp = SortColumnComparer._Compare(columnDescriptor, x, y);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return 0;
        }
    }

    /// <exclude/>
    internal class RecordsDetailsSortColumnComparer : IComparer
    {
        SortColumnDescriptor[] arrayOfColumnDescriptors;
        PropertyDescriptor[] arrayOfPropertyDescriptor;
        bool isSorted;
        Table table;
        TableDescriptor td;

        public RecordsDetailsSortColumnComparer(Table table, SortColumnDescriptorCollection columnDescriptors)
        {
            if (columnDescriptors == null)
            {
                throw new ArgumentNullException("columnDescriptors");
            }

            this.table = table;
            this.td = table.TableDescriptor;
            isSorted = table.ParentTableDescriptor.IsSorted;

            arrayOfColumnDescriptors = new SortColumnDescriptor[columnDescriptors.Count];
            columnDescriptors.CopyTo(this.arrayOfColumnDescriptors, 0);

            arrayOfPropertyDescriptor = new PropertyDescriptor[arrayOfColumnDescriptors.Length];
            int count = arrayOfColumnDescriptors.Length;
            for (int n = 0; n < count; n++)
            {
                SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                if (columnDescriptor != null)
                {
                    FieldDescriptor fd = columnDescriptor.FieldDescriptor;
                    if (fd != null && fd.IsPropertyField())
                    {
                        arrayOfPropertyDescriptor[n] = fd.GetPropertyDescriptor();
                    }
                }
            }
        }

        public int Compare(object x, object y)
        {
            Record ry = (Record)y;
            Record rx = (Record)x;

            int cmp = 0;
            if (isSorted)
            {
                ////cmp = _inner.Compare(rx, ry);
                int count = arrayOfColumnDescriptors.Length;
                for (int n = 0; n < count; n++)
                {
                    object cx, cy;
                    SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                    if (arrayOfPropertyDescriptor[n] == null || columnDescriptor.FieldDescriptor.IsComplexPropertyField())
                    {
                        cx = rx.GetValue(columnDescriptor);
                        cy = ry.GetValue(columnDescriptor);
                    }
                    else
                    {
                        cx = arrayOfPropertyDescriptor[n].GetValue(rx.GetData());
                        cy = arrayOfPropertyDescriptor[n].GetValue(ry.GetData());
                    }

                    cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }

                return rx.sourceIndex - ry.sourceIndex;
            }

            return rx.sourceIndex - ry.sourceIndex;
        }
    }
}

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Provides utilities for comparing two records or columns.
    /// </summary>
    public class CompareColumns
    {
        /// <summary>
        /// Compares the sort keys for the two records specified with x and y.
        /// </summary>
        /// <param name="isSorted">Spedifes whether the values are sorted.</param>
        /// <param name="arrayOfColumnDescriptors">Array of sort column descriptors.</param>
        /// <param name="x">First record to compare.</param>
        /// <param name="y">Second record to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater
        /// than zero x is greater than y.
        /// </returns>
        public static int CompareSortKeys(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, object x, object y)
        {
            Record ry = (Record)y;
            Record rx = (Record)x;

            if (isSorted)
            {
                ////cmp = _inner.Compare(rx, ry);
                int count = arrayOfColumnDescriptors.Length;
                for (int n = 0; n < count; n++)
                {
                    object cx, cy;
                    SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                    cx = rx.sortKeys[n];
                    cy = ry.sortKeys[n];
                    int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
            }

            return rx.sourceIndex - ry.sourceIndex;
        }

        /// <summary>
        /// Compares two nullbale objects.
        /// </summary>
        /// <param name="columnDescriptor">Sort column descriptor.</param>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greatervthan
        /// zero x is greater than y.
        /// </returns>
        public static int CompareNullableObjects(SortColumnDescriptor columnDescriptor, object x, object y)
        {
            return SortColumnComparer._Compare(columnDescriptor, x, y);
        }

        /// <summary>
        /// Compare two nullable objects.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Value Condition Less than zero x is less than y. Zero x equals y. Greatervthan zero x is greater than y.</returns>
        public static int CompareNullableObjects(object x, object y)
        {
            return SortColumnComparer._Compare(x, y);
        }

        /// <summary>
        /// Compares the primary keys for the two records specified with x and y.
        /// </summary>
        /// <param name="isSorted">Spedifes whether the values are sorted.</param>
        /// <param name="arrayOfColumnDescriptors">Array of sort column descriptors.</param>
        /// <param name="x">First record to compare.</param>
        /// <param name="y">Second record to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater
        /// than zero x is greater than y.
        /// </returns>
        public static int ComparePrimaryKeys(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, object x, object y)
        {
            Record ry = (Record)y;
            Record rx = (Record)x;

            if (isSorted)
            {
                ////cmp = _inner.Compare(rx, ry);
                int count = arrayOfColumnDescriptors.Length;
                for (int n = 0; n < count; n++)
                {
                    object cx, cy;
                    SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                    cx = rx.primaryKeys[n];
                    cy = ry.primaryKeys[n];
                    int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
            }

            return rx.sourceIndex - ry.sourceIndex;
        }
    }
}