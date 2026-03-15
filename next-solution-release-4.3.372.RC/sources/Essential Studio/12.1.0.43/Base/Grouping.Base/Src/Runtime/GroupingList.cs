//-------------------------------------------------------------------------------------------------
// <copyright file="GroupingList.cs" company="syncfusion">
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

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Defines the sort behavior of the table.
    /// </summary>
    /// <remarks>
    /// The engine has a built-in optimization when you change sort columns that allows it to perform the sorting on an on-demand basis 
    /// group-by-group. Suppose you have a table with 200 different countries and you change the sort order of the cities. It is not 
    /// necessary to sort the whole table. Instead, the individual groups can be sorted when they are scrolled into view. Such 
    /// on-demand sorting can only be done with internal sort routines and not by letting your datasource do the sorting.
    /// <para/>
    /// If you return <see cref="GroupByGroup"/>, you allow this optimization. IGroupingList.ApplySort will not be called when the table is already 
    /// sorted by grouped or relationchild columns.
    /// <para/>
    /// If you return <see cref="Table"/>, you do not allow this optimization. In that case, the whole table will be sorted and 
    /// IGroupingList.ApplySort is called.
    /// </remarks>
    public enum GroupingSortBehavior
    {
        /// <summary>
        /// Do not allow group-by-group sort optimization. In that case, the whole table will be sorted and 
        /// IGroupingList.ApplySort is called.
        /// </summary>
        Table,

        /// <summary>
        /// Allow group-by-group sort optimization. IGroupingList.ApplySort will not be called when the table is already 
        /// sorted by grouped or relationchild columns.
        /// </summary>
        GroupByGroup,
    }

    /// <summary>
    /// This interface lets you implement customized sorting routines for the engine as shown in 
    /// the Grid / Grouping / GroupingPerf sample.
    /// </summary>
    public interface IGroupingList
    {
        /// <summary>
        /// When you allow item references, the Record.GetData() call will cache and save a reference to the data 
        /// object for that specific row (e.g. with a DataTable, the data object is a DataRow). If you do not 
        /// allow item references, Record.GetData() will always get the data from the datasource and will not cache it. 
        /// Depending on how fast lookups are in your datasource, AllowItemReference might speed things up a bit.
        /// </summary>
        /// <remarks>
        /// The default behavior in the engine is that if the datasource is a DataView, the engine does internally have 
        /// AllowItemReference = True in the grouping engine. 
        /// For other datasources, we usually don�t because we do not know if is safe to keep references to the rows.
        /// <para/>
        /// The engine has an internal version field for the source list. When the list is sorted, this version field 
        /// is increased. That means no matter what you specify for AllowItemReference, the Record.GetData() will ask your
        /// Datasource for a reference and not rely on its cache. A ListChanged event also increases that version field. 
        /// So, after a ListChanged event any subsequent call to Record.GetData will result in querying your datasource 
        /// the first time. When Record.GetData() is called a second time on a record, it will return the cached row.
        /// </remarks>
        bool AllowItemReference { get; }

        /// <summary>
        /// Defines the sort behavior of the table.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization when you change sort columns that allows it to perform the sorting on an on-demand basis 
        /// group-by-group. Suppose you have a table with 200 different countries and you change the sort order of the cities. It is not 
        /// necessary to sort the whole table. Instead, the individual groups can be sorted when they are scrolled into view. Such 
        /// on-demand sorting can only be done with internal sort routines and not by letting your datasource do the sorting.
        /// <para/>
        /// If you return <see cref="Syncfusion.Grouping.GroupingSortBehavior.GroupByGroup"/>, you allow this optimization. IGroupingList.ApplySort will not be called when the table is already 
        /// sorted by grouped or relationchild columns.
        /// <para/>
        /// If you return <see cref="Syncfusion.Grouping.GroupingSortBehavior.Table"/>, you do not allow this optimization. In that case, the whole table will be sorted and 
        /// IGroupingList.ApplySort is called.
        /// </remarks>
        GroupingSortBehavior GroupingSortBehavior { get; }

        /// <summary>
        /// Specifies if the list's <see cref="ApplySort"/> rotuine should be called instead of having the engine
        /// sort the data. Return True if you want to provide your own customized sort routine.
        /// </summary>
        bool SupportsGroupSorting { get; }

        /// <summary>
        /// Sorts the records in the table in the order specifed by the column collections passed in as arguments.
        /// </summary>
        /// <param name="relationChildColumns">This has the highest sort precedence. RelationChildColumns will be given when you have a master-details 
        /// relation between two tables. The child table must be sorted by the columns that are used to identify a record. 
        /// These columns match the foreign key columns of the parent table.</param>
        /// <param name="groupColumns">This has the higher sort precedence than sortColumns. Group columns are the columns that the table is grouped by.</param>
        /// <param name="sortColumns">This has the lowest sort precedence.</param>
        void ApplySort(RelationChildColumnDescriptorCollection relationChildColumns, SortColumnDescriptorCollection groupColumns, SortColumnDescriptorCollection sortColumns);
    }

    /// <summary>
    /// A stripped down version of IBindingList. The interface only has the ListChanged event to notify the
    /// engine about changes in a bound datasource.
    /// </summary>
    public interface IListChangedSource
    {
        /// <summary>
        /// Gets whether this list supports the <see cref="ListChanged"/> event.
        /// </summary>
        bool SupportsListChanged { get; }

        /// <summary>
        /// Occurs when the list changes or an item in the list changes. See <see cref="IBindingList.ListChanged"/> in the <see cref="IBindingList"/>
        /// interface.
        /// </summary>
        event ListChangedEventHandler ListChanged;
    }
}