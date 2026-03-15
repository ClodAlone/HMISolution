//-------------------------------------------------------------------------------------------------
// <copyright file="Groups.cs" company="syncfusion">
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
using System.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;
using Syncfusion.ComponentModel;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A Read-only collection of <see cref="Group"/> elements that are children of a <see cref="GroupsDetails"/> section.
    /// An instance of this collection is returned by the <see cref="GroupsDetails.Groups"/> property
    /// of a <see cref="GroupsDetails"/> object. The <see cref="Group.Groups"/> property of a <see cref="Group"/> does also
    /// return an instance of this collection if the group's details section contains groups (and not records). Otherwise an
    /// empty collection is returned.
    /// </summary>
    public class GroupsInDetailsCollection : IList, IDisposable
    {
        internal GroupsDetails _groupWithGroups;

        /// <summary>
        /// A Read-only empty collection.
        /// </summary>
        public static GroupsInDetailsCollection Empty = new GroupsInDetailsCollection(null);

        internal GroupsInDetailsCollection(GroupsDetails groupWithGroups)
        {
            _groupWithGroups = groupWithGroups;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            _groupWithGroups = null;
        }

        /// <summary>
        /// Gets the group at the specified non-zero based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Group this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _groupWithGroups.EnsureInitialized(this);
                ElementTreeTableEntry entry = this._groupWithGroups.GroupDisplayEntries[index];
                if (entry == null)
                {
                    return null;
                }

                return (Group)entry.Element;
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        /// <overload>
        /// Searches for the specified category and returns the zero-based index of the occurrence.
        /// </overload>
        /// <summary>
        /// Searches for the specified category and returns the zero-based index of the occurrence.
        /// </summary>
        /// <param name="category">The category key.</param>
        /// <returns>
        /// The zero-based index of the occurrence of the category within the entire <see cref="GroupsInDetailsCollection"/>, if found; otherwise, -1.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// public Form1()
        /// {
        ///     //
        ///     // Required for Windows Form Designer support
        ///     //
        ///     InitializeComponent();
        /// <para/>
        ///     this.groupingGrid1.ShowNavigationBar = true;
        ///     this.groupingGrid1.TableControl.HorizontalScrollTips = false;
        ///     this.groupingGrid1.BorderStyle = BorderStyle.FixedSingle;
        ///     this.groupingGrid1.FilterRuntimeProperties = true;
        ///  <para/>
        ///     DataSet ds = new DataSet();
        ///     ReadXml(ds, @"Data\Expand.xml");
        ///  <para/>
        ///     ds.Tables[1].TableName = "Products";
        ///     ds.Tables[2].TableName = "OrderDetails";
        ///     ds.Tables[3].TableName = "Suppliers";
        ///  <para/>
        ///     ds.Relations.Add(
        ///         ds.Tables[0].Columns["CategoryID"],
        ///         ds.Tables[1].Columns["CategoryID"]);
        ///     ds.Relations[0].RelationName = "Category_Products";
        ///  <para/>
        ///     ds.Relations.Add(
        ///         ds.Tables[1].Columns["ProductID"],
        ///         ds.Tables[2].Columns["ProductID"]);
        ///     ds.Relations[1].RelationName = "Products_OrderDetails";
        ///  <para/>
        ///     this.groupingGrid1.DataSource = ds.Tables[0];
        ///     Table categoriesTable = groupingGrid1.GetTable("Categories");
        ///     Console.WriteLine(categoriesTable.ToString());
        ///  <para/>
        ///     Table productsTable = categoriesTable.RelatedTables["Products"];
        ///     Console.WriteLine(productsTable.ToString());
        ///  <para/>
        ///     ChildTable product1 = (ChildTable) productsTable.TopLevelGroup.Groups["1"];
        ///     Console.WriteLine(product1.ToString());
        ///     Console.WriteLine(product1.Records[0].ToString());
        ///  <para/>
        ///     ChildTable product21 = (ChildTable) productsTable.TopLevelGroup.Groups["8"];
        ///     Console.WriteLine(product21.ToString());
        ///     Console.WriteLine(product21.Records[0].ToString());
        /// }
        /// </code>
        /// <code lang="VB">
        ///  <para/>
        ///  <para/>
        /// Public Sub New() '
        ///     '
        ///     ' Required for Windows Form Designer support
        ///     '
        ///     InitializeComponent()
        ///  <para/>
        ///     Me.groupingGrid1.ShowNavigationBar = True
        ///     Me.groupingGrid1.TableControl.HorizontalScrollTips = False
        ///     Me.groupingGrid1.BorderStyle = BorderStyle.FixedSingle
        ///     Me.groupingGrid1.FilterRuntimeProperties = True
        ///  <para/>
        ///     Dim ds As New DataSet()
        ///     ReadXml(ds, "Data\Expand.xml")
        /// <para/> 
        ///     ds.Tables(1).TableName = "Products"
        ///     ds.Tables(2).TableName = "OrderDetails"
        ///     ds.Tables(3).TableName = "Suppliers"
        ///  <para/>
        ///     ds.Relations.Add(ds.Tables(0).Columns("CategoryID"), ds.Tables(1).Columns("CategoryID"))
        ///     ds.Relations(0).RelationName = "Category_Products"
        ///  <para/>
        ///     ds.Relations.Add(ds.Tables(1).Columns("ProductID"), ds.Tables(2).Columns("ProductID"))
        ///     ds.Relations(1).RelationName = "Products_OrderDetails"
        /// <para/> 
        ///     Me.groupingGrid1.DataSource = ds.Tables(0)
        ///     Dim categoriesTable As Table = groupingGrid1.GetTable("Categories")
        ///     Console.WriteLine(categoriesTable.ToString())
        ///     Dim productsTable As Table = categoriesTable.RelatedTables("Products")
        ///     Console.WriteLine(productsTable.ToString())
        ///     Dim product1 As ChildTable = CType(productsTable.TopLevelGroup.Groups("1"), ChildTable)
        ///     Console.WriteLine(product1.ToString())
        ///     Console.WriteLine(product1.Records(0).ToString())
        ///     Dim product21 As ChildTable = CType(productsTable.TopLevelGroup.Groups("8"), ChildTable)
        ///     Console.WriteLine(product21.ToString())
        ///     Console.WriteLine(product21.Records(0).ToString())
        /// End Sub 'New
        /// </code>
        /// </example>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int FindGroup(object category)
        {
            if (_groupWithGroups != null && _groupWithGroups.ParentGroup != null)
            {
                _groupWithGroups.EnsureInitialized(this, true);

                int c = this._groupWithGroups.ParentTable.GetVisibleCount();
                ////Console.WriteLine(this._groupWithGroups.ParentTable);
                Type type = this._groupWithGroups.columnDescriptors[0].FieldDescriptor.GetPropertyType();
                object adjCat = category is DBNull || category == null ? category : NullableHelper.ChangeType(category, type);
                object key = new object[] { adjCat };

                if (this._groupWithGroups.sortOrderTreeTable != null)
                {
                    // Lookup category in group tree sorted by category, get Group and then lookup 
                    // index in SortSummaryTree
                    GroupCategoryTreeTableEntry gentry = (GroupCategoryTreeTableEntry)this._groupWithGroups.categoryTreeTable.inner.FindKey(key);

                    if (gentry == null || gentry.Element == null)
                    {
                        return -1;
                    }

                    return this._groupWithGroups.sortOrderTreeTable.TreeTable.IndexOf(gentry.Element.GroupSortOrderEntry);
                }
                else
                {
                    return this._groupWithGroups.categoryTreeTable.inner.IndexOfKey(key);
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified category and returns the zero-based index of the occurrence.
        /// </summary>
        /// <param name="categoryKeys">The array of objects that identify the category.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        /// <returns>returns the zero-based index of the occurrence</returns>
        public int FindGroup(object[] categoryKeys)
        {
            if (categoryKeys == null)
            {
                categoryKeys = new object[] { null };
            }

            if (_groupWithGroups != null && _groupWithGroups.ParentGroup != null)
            {
                _groupWithGroups.EnsureInitialized(this, true);

                int c = this._groupWithGroups.ParentTable.GetVisibleCount();
                ////Console.WriteLine(this._groupWithGroups.ParentTable);

                if (this._groupWithGroups.sortOrderTreeTable != null)
                {
                    // Lookup category in group tree sorted by category, get Group and then lookup 
                    // index in SortSummaryTree
                    GroupCategoryTreeTableEntry gentry = (GroupCategoryTreeTableEntry)this._groupWithGroups.GroupCategoryTreeTable.TreeTable.FindKey(categoryKeys);

                    if (gentry == null || gentry.Element == null)
                    {
                        return -1;
                    }

                    return this._groupWithGroups.sortOrderTreeTable.TreeTable.IndexOf(gentry.Element.GroupSortOrderEntry);
                }
                else
                {
                    return this._groupWithGroups.categoryTreeTable.inner.IndexOfKey(categoryKeys);
                }
            }

            return -1;
        }

        // TODO: FindHighestSmallerOrEqualKey

        /// <summary>
        /// Returns the group that matches the category or the nearest group with a category key that 
        /// is smaller than the specified category key.
        /// </summary>
        /// <param name="category">The category key.</param>
        /// <returns>
        /// The zero-based index of the occurrence of the category within the entire <see cref="GroupsInDetailsCollection"/>, if found; otherwise, -1.
        /// </returns>
        public Group FindHighestSmallerOrEqualKey(object category)
        {
            if (_groupWithGroups != null && _groupWithGroups.ParentGroup != null)
            {
                _groupWithGroups.EnsureInitialized(this, true);

                int c = this._groupWithGroups.ParentTable.GetVisibleCount();
                Type type = this._groupWithGroups.columnDescriptors[0].FieldDescriptor.GetPropertyType();
                object key = NullableHelper.ChangeType(category, type);
                GroupCategoryTreeTableEntry gte = (GroupCategoryTreeTableEntry)_groupWithGroups.categoryTreeTable.TreeTable.FindHighestSmallerOrEqualKey(new object[] { key });
                if (gte == null)
                {
                    return null;
                }

                return gte.Element;
            }

            return null;
        }

        // TODO: FindLowestHigherOrEqualKey

        /// <summary>
        /// Returns the group that matches the category or the nearest group with a category key that 
        /// is higher than the specified category key.
        /// </summary>
        /// <param name="category">The category key.</param>
        /// <returns>
        /// The zero-based index of the occurrence of the category within the entire <see cref="GroupsInDetailsCollection"/>, if found; otherwise, -1.
        /// </returns>
        public Group FindLowestHigherOrEqualKey(object category)
        {
            Group g = FindHighestSmallerOrEqualKey(category);
            if (g.Category.Equals(category))
            {
                return g;
            }

            GroupCategoryTreeTableEntry gte = g.GroupCategoryEntry;
            ITreeTable tree = gte.Tree;
            GroupCategoryTreeTableEntry next = tree.GetNextEntry(gte) as GroupCategoryTreeTableEntry;
            if (next == null)
            {
                return null;
            }

            return next.Element;
        }

        /// <summary>
        /// Searches for the specified category and returns the found group.
        /// </summary>
        /// <param name="category">Category index.</param>
        /// <example>
        /// <code lang="C#">
        /// public Form1()
        /// {
        ///     //
        ///     // Required for Windows Form Designer support
        ///     //
        ///     InitializeComponent();
        /// <para/>
        ///     this.groupingGrid1.ShowNavigationBar = true;
        ///     this.groupingGrid1.TableControl.HorizontalScrollTips = false;
        ///     this.groupingGrid1.BorderStyle = BorderStyle.FixedSingle;
        ///     this.groupingGrid1.FilterRuntimeProperties = true;
        ///  <para/>
        ///     DataSet ds = new DataSet();
        ///     ReadXml(ds, @"Data\Expand.xml");
        /// <para/> 
        ///     ds.Tables[1].TableName = "Products";
        ///     ds.Tables[2].TableName = "OrderDetails";
        ///     ds.Tables[3].TableName = "Suppliers";
        /// <para/> 
        ///     ds.Relations.Add(
        ///         ds.Tables[0].Columns["CategoryID"],
        ///         ds.Tables[1].Columns["CategoryID"]);
        ///     ds.Relations[0].RelationName = "Category_Products";
        /// <para/> 
        ///     ds.Relations.Add(
        ///         ds.Tables[1].Columns["ProductID"],
        ///         ds.Tables[2].Columns["ProductID"]);
        ///     ds.Relations[1].RelationName = "Products_OrderDetails";
        /// <para/> 
        ///     this.groupingGrid1.DataSource = ds.Tables[0];
        ///     Table categoriesTable = groupingGrid1.GetTable("Categories");
        ///     Console.WriteLine(categoriesTable.ToString());
        ///  <para/>
        ///     Table productsTable = categoriesTable.RelatedTables["Products"];
        ///     Console.WriteLine(productsTable.ToString());
        ///  <para/>
        ///     ChildTable product1 = (ChildTable) productsTable.TopLevelGroup.Groups["1"];
        ///     Console.WriteLine(product1.ToString());
        ///     Console.WriteLine(product1.Records[0].ToString());
        ///  <para/>
        ///     ChildTable product21 = (ChildTable) productsTable.TopLevelGroup.Groups["8"];
        ///     Console.WriteLine(product21.ToString());
        ///     Console.WriteLine(product21.Records[0].ToString());
        /// }
        /// </code>
        /// <code lang="VB">
        ///  <para/>
        ///  <para/>
        /// Public Sub New() '
        ///     '
        ///     ' Required for Windows Form Designer support
        ///     '
        ///     InitializeComponent()
        ///  <para/>
        ///     Me.groupingGrid1.ShowNavigationBar = True
        ///     Me.groupingGrid1.TableControl.HorizontalScrollTips = False
        ///     Me.groupingGrid1.BorderStyle = BorderStyle.FixedSingle
        ///     Me.groupingGrid1.FilterRuntimeProperties = True
        /// <para/> 
        ///     Dim ds As New DataSet()
        ///     ReadXml(ds, "Data\Expand.xml")
        ///  <para/>
        ///     ds.Tables(1).TableName = "Products"
        ///     ds.Tables(2).TableName = "OrderDetails"
        ///     ds.Tables(3).TableName = "Suppliers"
        ///  <para/>
        ///     ds.Relations.Add(ds.Tables(0).Columns("CategoryID"), ds.Tables(1).Columns("CategoryID"))
        ///     ds.Relations(0).RelationName = "Category_Products"
        ///  <para/>
        ///     ds.Relations.Add(ds.Tables(1).Columns("ProductID"), ds.Tables(2).Columns("ProductID"))
        ///     ds.Relations(1).RelationName = "Products_OrderDetails"
        ///  <para/>
        ///     Me.groupingGrid1.DataSource = ds.Tables(0)
        ///     Dim categoriesTable As Table = groupingGrid1.GetTable("Categories")
        ///     Console.WriteLine(categoriesTable.ToString())
        ///     Dim productsTable As Table = categoriesTable.RelatedTables("Products")
        ///     Console.WriteLine(productsTable.ToString())
        ///     Dim product1 As ChildTable = CType(productsTable.TopLevelGroup.Groups("1"), ChildTable)
        ///     Console.WriteLine(product1.ToString())
        ///     Console.WriteLine(product1.Records(0).ToString())
        ///     Dim product21 As ChildTable = CType(productsTable.TopLevelGroup.Groups("8"), ChildTable)
        ///     Console.WriteLine(product21.ToString())
        ///     Console.WriteLine(product21.Records(0).ToString())
        /// End Sub 'New
        /// </code>
        /// </example>
        public Group this[string category]
        {
            get
            {
                if (category == null)
                {
                    throw new ArgumentNullException("category");
                }

                int index = FindGroup(category);
                if (index == -1)
                {
                    throw new ArgumentException(category.ToString() + " not found in " + this._groupWithGroups.columnDescriptors[0].Name);
                }

                return this[index];
            }
        }

        /// <summary>
        /// Searches for the specified category and returns True if it was found.
        /// </summary>
        /// <param name="category">Category key.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        /// <returns>returns True if it was found</returns>
        public bool Contains(string category)
        {
            return FindGroup(category) != -1;
        }

        /// <summary>
        /// Searches for the specified category and returns the zero-based index of the occurrence or -1 if not found.
        /// </summary>
        /// <param name="category">Category key.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        /// <returns>returns the zero-based index of the occurrence</returns>
        public int IndexOf(string category)
        {
            return FindGroup(category);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(Group value)
        {
            if (value == null)
            {
                return false;
            }

            if (_groupWithGroups == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            return value.ParentElement == _groupWithGroups;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int IndexOf(Group value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            if (value.GroupSortOrderEntry != null)
            {
                return value.GroupSortOrderEntry.GetPosition(); ////InnerElementPosition;
            }

            return value.GroupCategoryEntry.GetPosition();
        }

        /// <summary>
        /// Removes the group.
        /// </summary>
        /// <param name="value">Group to remove.</param>
        public void Remove(Group value)
        {
            if (value.GroupCategoryEntry != null)
            {
                value.GroupCategoryEntry.ElementTreeTable.Remove(value.GroupCategoryEntry);
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void CopyTo(Group[] array, int index)
        {
            int n = 0;
            foreach (Group group in this)
            {
                array[index + n] = group;
                n++;
            }
        }

        ////        public GroupsInDetailsCollection SyncRoot
        ////        {
        ////            get
        ////            {
        ////                return null;
        ////            }
        ////        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public GroupsInDetailsCollectionEnumerator GetEnumerator()
        {
            if (_groupWithGroups != null)
            {
                _groupWithGroups.EnsureInitialized(this);
            }

            return new GroupsInDetailsCollectionEnumerator(this);
        }

        #region IList Members

        /// <summary>
        /// Returns True because this collection is always Read-only.
        /// </summary>
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

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">The index of the list</param>
        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Remove(object value)
        {
            Remove((Group)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((Group)value);
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Group)value);
        }

        int IList.Add(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_groupWithGroups == null)
                {
                    return 0;
                }

                _groupWithGroups.EnsureInitialized(this);
                return _groupWithGroups.categoryTreeTable.TreeTable.GetCount();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Group[])array, index);
        }

        object ICollection.SyncRoot
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
    }

    /// <summary>
    /// Enumerator class for <see cref="Group"/> elements of a <see cref="GroupsInDetailsCollection"/>.
    /// </summary>
    public class GroupsInDetailsCollectionEnumerator : IEnumerator
    {
        Group _cursor, _next;
        GroupsInDetailsCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GroupsInDetailsCollectionEnumerator(GroupsInDetailsCollection collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0];
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0];
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public Group Current
        {
            get
            {
                return _cursor;
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
            if (_next == null)
            {
                return false;
            }

            _cursor = _next;

            _next = (Group)ElementHelper.GetNextSibling(_next);

            return _cursor != null;
        }
        #endregion
    }
}
