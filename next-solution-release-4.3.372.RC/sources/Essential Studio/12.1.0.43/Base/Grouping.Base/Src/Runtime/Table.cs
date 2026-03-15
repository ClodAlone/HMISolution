//-------------------------------------------------------------------------------------------------
// <copyright file="Table.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#define USETHRAEDING
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
using System.Collections.Generic;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Defines an interface for an object that handles events raised by <see cref="Table"/> objects.
    /// </summary>
    public interface ITableEventsTarget
    {
        /// <summary>
        /// Occurs when an unknown exception has been catched while modifying underlying data in the datasource.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        void OnExceptionRaised(ExceptionRaisedEventArgs e);

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        void OnGroupCollapsing(GroupEventArgs e);

        /// <summary>
        /// Occurs after a group is collapsed.
        /// </summary>
        void OnGroupCollapsed(GroupEventArgs e);

        /// <summary>
        /// Occurs before a group is expanded.
        /// </summary>
        void OnGroupExpanding(GroupEventArgs e);

        /// <summary>
        /// Occurs after a group is expanded.
        /// </summary>
        void OnGroupExpanded(GroupEventArgs e);

        /// <summary>
        /// Occurs before a record with nested tables is collapsed.
        /// </summary>
        void OnRecordCollapsing(RecordEventArgs e);

        /// <summary>
        /// Occurs after a record with nested tables is collapsed.
        /// </summary>
        void OnRecordCollapsed(RecordEventArgs e);

        /// <summary>
        /// Occurs before a record with nested tables is expanded.
        /// </summary>
        void OnRecordExpanding(RecordEventArgs e);

        /// <summary>
        /// Occurs after a record with nested tables is expanded.
        /// </summary>
        void OnRecordExpanded(RecordEventArgs e);

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        void OnRecordDeleting(RecordEventArgs e);

        /// <summary>
        /// Occurs after a record is deleted.
        /// </summary>
        void OnRecordDeleted(RecordEventArgs e);

        /// <summary>
        /// Occurs before and after the status of the current record was changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information which current record state was changed.
        /// </summary>
        void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e);

        /// <summary>
        /// Occurs when the <see cref="CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> was called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and resets any "Current Cell" state when this
        /// event was raised.
        /// </remarks>
        void OnCurrentRecordManagerReset(TableEventArgs e);

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event will force a repaint of the specified summary if it is visible
        /// when this event was raised.
        /// </remarks>
        void OnGroupSummaryInvalidated(GroupEventArgs e);

        // ListChanged

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="Table.SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer a chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        void OnSourceListListChanged(TableListChangedEventArgs e);

        /// <summary>
        /// Occurs after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event right after the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        void OnSourceListListChangedCompleted(TableListChangedEventArgs e);

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed, or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        void OnSourceListRecordChanging(RecordChangedEventArgs e);

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="Table.SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        void OnSourceListRecordChanged(RecordChangedEventArgs e);

        /// <summary>
        /// Occurs when a new group was added in a table after the table was categorized and when a record was changed. The event does not
        /// occur during categorization of the table. See the <see cref="Table.CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        void OnGroupAdded(GroupEventArgs e);

        /// <summary>
        /// Occurs when a group was removed from a table after the table was categorized and when a record was changed. The event does not
        /// occur during categorization of the table. See the <see cref="Table.CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        void OnGroupRemoving(GroupEventArgs e);

        // Counters, summaries, sorting:

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup  events are fired in such case when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        void OnSortingItemsInGroup(GroupEventArgs e);

        /// <summary>
        /// Occurs after the records for a group were sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup  events are fired in such case when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        void OnSortedItemsInGroup(GroupEventArgs e);

        /// <summary>
        /// Occurs when the <see cref="Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> is called
        /// and before all counters are marked dirty.
        /// </summary>
        void OnInvalidatingCounters(TableEventArgs e);

        /// <summary>
        /// Occurs when the <see cref="Table.InvalidateSummariesTopDown"/> of a <see cref="Table"/> is called
        /// and before all summaries in that table are marked dirty.
        /// </summary>
        void OnInvalidatingSummaries(TableEventArgs e);

        /// <summary>
        /// Occurs before records are categorized after a table is marked dirty (<see cref="Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Table.TableDirty"/> is set to True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element.EnsureInitialized"/> of the <see cref="Table"/>
        /// will start categorization.
        /// </remarks>
        void OnCategorizingRecords(TableEventArgs e);

        /// <summary>
        /// Occurs after records are categorized after a table is marked dirty (<see cref="Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Table.TableDirty"/> is set to True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element.EnsureInitialized"/> will start
        /// categorization.
        /// </remarks>
        void OnCategorizedRecords(TableEventArgs e);

        /// <summary>
        /// Occurs after the data source is replaced.
        /// </summary>
        void OnTableSourceListChanged(TableEventArgs e);

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
        /// </summary>
        void OnRecordValueChanging(RecordValueChangingEventArgs e);

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned.
        /// </summary>
        void OnRecordValueChanged(RecordValueChangedEventArgs e);

        /// <summary>
        /// When number of visible elements are changed.
        /// </summary>
        void OnDisplayElementChanging(DisplayElementChangingEventArgs e);

        /// <summary>
        /// After number of visible elements were changed.
        /// </summary>
        void OnDisplayElementChanged(DisplayElementChangedEventArgs e);

        /// <summary>
        /// Occurs before the <see cref="Table.SelectedRecords"/> collection was modified.
        /// </summary>
        void OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e);

        /// <summary>
        /// Occurs after the <see cref="Table.SelectedRecords"/> collection was modified.
        /// </summary>
        void OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e);
    }

    /// <summary>
    /// Provides a method to access a <see cref="Table"/> object.
    /// </summary>
    public interface ITableSource
    {
        /// <summary>
        /// Returns a <see cref="Table"/>.
        /// </summary>
        /// <returns>returns Table value</returns>
        Table GetTable();
    }

    /// <summary>
    /// The Table class provides a flattened view of grouped and hierarchical records and
    /// manages all the records from the underlying source list.
    /// </summary>
    /// <remarks>
    /// The table manages all the records from the underlying source list. The source list can be any
    /// IList collection. If it implements IBindingList, the table will listen to the ListChangedEvent
    /// and update its internal data whenever changes are made to the source list.<para/>
    /// <para/>
    /// When the Table is initialized for the first time it will loop through all items in the source
    /// list and create a record object. The record object will be initialized with an index to the underlying
    /// source list item so that it can locate the item when you call its Record.GetData() property.
    /// Record.GetData() looks up the item in the underlying source list and returns a reference to it.<para/>
    /// <para/>
    /// Once a record object has been initialized, it will stay attached to the underlying item. When
    /// records are inserted or moved, this relation will stay intact. The only time this link between
    /// record and the underlying item is released is when the underlying item gets deleted from the
    /// source list. In that event, the record object gets disposed. A record's sorted record index
    /// within the table can at any time be determined with the Table.Records.IndexOf(record) method.
    /// This method will always return the accurate index respecting any insertions, movements, or changes
    /// that were made in the source list.
    /// <para/>
    /// When looping through the source list and intializing the records, the table also checks
    /// if categorization of data is necessary. When the TableDescriptor.RelationChildColumns or
    /// TableDescriptor.GroupedColumns collection are not empty, groups will be created for
    /// each new category.
    /// <para/>
    /// A table has one main group, the <see cref="TopLevelGroup"/>. The <see cref="TopLevelGroup"/>
    /// gives you access to a list of child groups and child records.
    /// <para/>
    /// The order of records and groups is defined by the <see cref="TableDescriptor"/> with schema
    /// information about the table. Once you make changes to the <see cref="TableDescriptor"/>, the
    /// table will be updated on demand the first time you try to access elements of the table after
    /// the change.
    /// <para/>
    /// There are multiple ways to get access to a specific record: <para/>
    /// <list type="bullet">
    /// <item><term>
    /// The <see cref="Table.UnsortedRecords"/> collection of the <see cref="Table"/> class provides access to the records
    /// in the same order as they appear in the datasource. The <see cref="UnsortedRecordsCollection.IndexOf"/> method
    /// of a <see cref="UnsortedRecordsCollection"/> determines the index of any record in the underlying datasource.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.Records"/> collection of the <see cref="Table"/> class provides access to the records
    /// in the order as they were sorted in the engine. The <see cref="RecordsInTableCollectionBase.IndexOf"/> method
    /// of a <see cref="RecordsInTableCollection"/> determines the index of any record in the Table.Records collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.FilteredRecords"/> collection of the <see cref="Table"/> class provides access to records that
    /// meet filter criteria in the order they were sorted in the engine. The <see cref="RecordsInTableCollectionBase.IndexOf"/> method
    /// of a <see cref="RecordsInTableCollection"/> determines the index of any record in the Table.FilteredRecords collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Group.Records"/> collection of the <see cref="Group"/> class provides access to the records
    /// in the order they appear in the group. The <see cref="RecordsInDetailsCollection.IndexOf"/> method
    /// of a <see cref="RecordsInDetailsCollection"/> determines the index of any record in the Group.Records collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Group.FilteredRecords"/> collection of the <see cref="Group"/> class provides access to the records
    /// in the order they appear in the group. The <see cref="FilteredRecordsInDetailsCollection.IndexOf"/> method
    /// of a <see cref="FilteredRecordsInDetailsCollection"/> determines the index of any record in the Group.FilteredRecords collection.
    /// </term></item>
    /// </list>
    /// Any element in the table can be accessed using its element or display index. There are four
    /// collections that let you access elements:
    /// <list type="bullet">
    /// <item><term>
    /// The <see cref="Table.DisplayElements"/> collection of the <see cref="Table"/> class provides access to the visible
    /// display elements. The <see cref="RuntimeElementsInTableCollection.IndexOf"/> method
    /// of a <see cref="DisplayElementsInTableCollection"/> determines the visible index of any element in the table. <para/>
    /// With a GridGroupingControl, the index of display elements is the same as the row index in the grid. Therefore,
    /// given a row index of the grid you can easily determine the element that is to be shown at the specific row. Vice versa,
    /// if you do have an element, you can easily determine its row index with the <see cref="RuntimeElementsInTableCollection.IndexOf"/> method.
    /// The <see cref="Table.DisplayElements"/> collection does not step into a <see cref="NestedTable"/> element.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.NestedDisplayElements"/> collection of the <see cref="Table"/> class provides access to the visible
    /// display elements and nested elements inside nested tables. The <see cref="RuntimeElementsInTableCollection.IndexOf"/> method
    /// of a <see cref="DisplayElementsInTableCollection"/> determines the visible index of any element in the table. <para/>
    /// The <see cref="Table.NestedDisplayElements"/> collection steps into a <see cref="NestedTable"/> element.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.Elements"/> collection returns all elements of the <see cref="Table"/>
    /// in the order as they appear if they would be visible. Any element is returned, also records that are not visible
    /// in the DisplayElements collection because a parent group was collapsed are returned.
    /// The <see cref="RuntimeElementsInTableCollection.IndexOf"/> method
    /// of a <see cref="ElementsInTableCollection"/> determines the index of any element in the Table.Elements collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.NestedElements"/> collection returns all elements of the <see cref="Table"/>
    /// in the order as they appear if they would be visible and also steps into nested tables.
    /// The <see cref="RuntimeElementsInTableCollection.IndexOf"/> method
    /// of a <see cref="ElementsInTableCollection"/> determines the index of any element in the Table.NestedElements collection.
    /// </term></item>
    /// </list>
    /// A table has a <see cref="CurrentRecord"/> or <see cref="CurrentElement"/> which identifies the current record. The current
    /// record can be edited and navigated with the <see cref="CurrentRecordManager"/>.<para/>
    /// A table raises various events when changes are made to a record or when groups or records are collapsed or expanded. All
    /// events will bubble up to the parent engine object. <para/>
    /// If an engine displays hierarchical data, a <see cref="Table"/> is created for each table of the
    /// datasource. With a DataSet for example, a <see cref="Table"/> is created for each DataTable in the DataSet. <para/>
    /// If a <see cref="Table"/> is a child table in a master-details relation, the table is grouped by the foreign key column.
    /// That way the parent table can get access to a group of records in the child table using the foreign key.
    /// </remarks>
    public class Table : Element, IContainerElement, IDisposable
    {
        #region Fields
        IList sourceList;
        IBindingList bindingList;
        ArrayList sourceListSortArray;
        UnsortedRecordsTree unsortedRecordsTree;
        UnsortedRecordsCollection _unsortedRecords = null;
        PrimaryKeySortedRecordsTree primaryKeySortedRecordsTree;
        PrimaryKeySortedRecordsCollection _primaryKeySortedRecords = null;
        DisplayElementsInTableCollection _displayElements = null;
        DisplayElementsInTableCollection _nestedDisplayElements = null;
        ElementsInTableCollection _elements = null;
        ElementsInTableCollection _nestedElements = null;
        FilteredRecordsInTableCollection _filteredRecords = null;
        RecordsInTableCollection _sortedRecords = null;
        bool inSetSourceList;
        bool inSourceListChanged;
        ChildTable filteredChildTable;
        GroupCategoryTreeTable groupCategoryTable;
        ChildTable _topLevelGroup;
        Table relationParentTable = null;
        bool isDirty = true;
        bool _isSummaryDirty = false;
        bool _isRecordFiltersDirty = true;
        TableDescriptor _tableDescriptor;
        int defaultRecordPreviewRowHeight = 40;
        int defaultRecordRowHeight = 18;
        int defaultCaptionRowHeight = 22;
        int defaultColumnHeaderRowHeight = 22;
        int defaultEmptySectionHeight = 10;
        int defaultGroupFooterSectionHeight = 10;
        int defaultGroupPreviewSectionHeight = 40;
        int defaultGroupHeaderSectionHeight = 10;
        int defaultFilterBarRowHeight = -1;
        int defaultSummaryRowHeight = -1;
        int defaultIndentWidth = 18;
        int defaultRowHeaderWidth = 18;
        int defaultTableIndentWidth = 18;
        bool inInitialize;
        int _sourceListVersion = 0;
        Record lastChangedRecord = null;
        static bool traceRelatedTables = false;
        ArrayList relatedTables = new ArrayList();
        RelationDescriptorCollection relationDescriptors = null;
        TableCollection _relatedTables;
        internal bool wasItemChanged = false;
        int oldCount = 0;
        int lastAddNewIndex = -1;
        bool immediateUpdateSummaries = false;
        CurrentRecordManager currentRecordManager = null;
        bool lastSorted = false;
        bool lastPKSorted = false;
        int collapsing_CollapseCount = 0;
        int _primaryKeyColumnsVersion = -1;
        ////        static bool traceVisibleCount = false;
        internal WeakReference flattenedRecordsGroup = new WeakReference(null);
        internal WeakReference flattenedRecordsGroupCollection = new WeakReference(null);
        internal WeakReference flattenedFilteredRecordsGroup = new WeakReference(null);
        internal WeakReference flattenedFilteredRecordsGroupCollection = new WeakReference(null);
        internal WeakReference groupTypedListRecordsGroup = new WeakReference(null);
        internal WeakReference groupTypedListRecordsCollection = new WeakReference(null);

        internal bool hasGroupSortOrderEntry = false;

        Engine engine;

        /// <summary>Returns grouping engine.</summary>
        /// <override/>
        public override Engine Engine
        {
            get
            {
                return engine;
            }
        }

        int tableNo;
        bool supportCustomCounters = false;
        ListChangedEventArgs lastListChangedEventArgs = null;
        bool dataSourceRaisesTwoItemAddedEvents = false;

        static int tableCounter = 0;
        int tableId = 0;
        #endregion
        #region Construct
        /// <summary>
        /// Initializes a new table object that belongs to a <see cref="TableDescriptor"/> and optionally belongs to a parent table.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor with schema information about the table.</param>
        /// <param name="relationParentTable">The parent table of this table; NULL if this table is not a child table of a relation.</param>
        public Table(TableDescriptor tableDescriptor, Table relationParentTable)
        {
            this.engine = tableDescriptor.Engine;
            groupCategoryTable = new GroupCategoryTreeTable(this);
            _tableDescriptor = tableDescriptor;
            this.relationParentTable = relationParentTable;
            WireTableDescriptor();
            tableEmptySummaries = new TableEmptySummaries(this);
            ////UFD:tableEmptyUnfilteredSummaries = new TableEmptyUnfilteredSummaries(this);

            unsortedRecordsTree = new UnsortedRecordsTree(this);
            _unsortedRecords = new UnsortedRecordsCollection(unsortedRecordsTree, this);
            string pnam = relationParentTable != null ? relationParentTable.TableDescriptor.Name : string.Empty;
            tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(tableDescriptor.Name, pnam, tableId);
            }

            tableNo = TableDescriptor.Engine.tableNoCounter;
            TableDescriptor.Engine.tableNoCounter++;
        }

        /// <summary>
        /// Returns a unique id for the table in the parents engine object. Each nested
        /// table will have a different id.
        /// </summary>
        public int TableNo
        {
            get
            {
                return tableNo;
            }
        }
        #endregion
        #region ITableEventsTarget
        private ITableEventsTarget eventsTarget;

        /// <summary>
        /// Gets / sets an object that handles events raised by this object.
        /// </summary>
        public ITableEventsTarget TableEventsTarget
        {
            get
            {
                return eventsTarget;
            }

            set
            {
                eventsTarget = value;
            }
        }
        #endregion
        #region WireTableDescriptor
        /// <summary>
        /// Wires events for the <see cref="TableDescriptor"/>
        /// </summary>
        protected virtual void WireTableDescriptor()
        {
            if (TableDescriptor != null)
            {
                TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
                TableDescriptor.PropertyChanging += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
                TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                TableDescriptor.RelationChildColumns.Changed += new ListPropertyChangedEventHandler(RelationChildColumns_Changed);
                TableDescriptor.SortedColumns.Changed += new ListPropertyChangedEventHandler(SortedColumns_Changed);
                TableDescriptor.Summaries.Changed += new ListPropertyChangedEventHandler(SummaryDescriptors_Changed);
                TableDescriptor.RecordFilters.Changed += new ListPropertyChangedEventHandler(RecordFilters_Changed);
                TableDescriptor.RecordFilters.PropertyChanged += new DescriptorPropertyChangedEventHandler(RecordFilters_PropertyChanged);
                TableDescriptor.ItemPropertiesChanged += new EventHandler(TableDescriptor_ItemPropertiesChanged);
                TableDescriptor.ExpressionFields.Changed += new ListPropertyChangedEventHandler(ExpressionFields_Changed);
                TableDescriptor.Fields.Changed += new ListPropertyChangedEventHandler(Fields_Changed);
                TableDescriptor.AllowNewChanged += new EventHandler(TableDescriptor_AllowNewChanged);
                if (TableDescriptor.Engine != null)
                {
                    TableDescriptor.Engine.SourceListChanged += new EventHandler(Engine_SourceListChanged);
                    TableDescriptor.Engine.PropertyChanging += new DescriptorPropertyChangedEventHandler(Engine_PropertyChanging);
                    TableDescriptor.Engine.PropertyChanged += new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
                }

                TableDescriptor.Disposed += new EventHandler(TableDescriptor_Disposed);
            }
        }

        /// <summary>
        /// Unwires events for the <see cref="TableDescriptor"/>
        /// </summary>
        protected virtual void UnwireTableDescriptor()
        {
            if (TableDescriptor != null)
            {
                TableDescriptor.PropertyChanged -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
                TableDescriptor.PropertyChanging -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
                TableDescriptor.GroupedColumns.Changed -= new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                TableDescriptor.RelationChildColumns.Changed -= new ListPropertyChangedEventHandler(RelationChildColumns_Changed);
                TableDescriptor.SortedColumns.Changed -= new ListPropertyChangedEventHandler(SortedColumns_Changed);
                TableDescriptor.Summaries.Changed -= new ListPropertyChangedEventHandler(SummaryDescriptors_Changed);
                TableDescriptor.RecordFilters.Changed -= new ListPropertyChangedEventHandler(RecordFilters_Changed);
                TableDescriptor.RecordFilters.PropertyChanged -= new DescriptorPropertyChangedEventHandler(RecordFilters_PropertyChanged);
                TableDescriptor.ItemPropertiesChanged -= new EventHandler(TableDescriptor_ItemPropertiesChanged);
                TableDescriptor.ExpressionFields.Changed -= new ListPropertyChangedEventHandler(ExpressionFields_Changed);
                TableDescriptor.Fields.Changed -= new ListPropertyChangedEventHandler(Fields_Changed);
                TableDescriptor.AllowNewChanged -= new EventHandler(TableDescriptor_AllowNewChanged);
                if (TableDescriptor.Engine != null)
                {
                    TableDescriptor.Engine.SourceListChanged -= new EventHandler(Engine_SourceListChanged);
                    TableDescriptor.Engine.PropertyChanging -= new DescriptorPropertyChangedEventHandler(Engine_PropertyChanging);
                    TableDescriptor.Engine.PropertyChanged -= new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
                }

                TableDescriptor.Disposed -= new EventHandler(TableDescriptor_Disposed);
            }
        }

        private void TableDescriptor_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion
        #region DisposableObject overrides
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(_tableDescriptor != null ? _tableDescriptor.Name : string.Empty, tableId);
            }

            if (disposing)
            {
                UnwireDataTable(dt);
                UnwireGroupingColumnChanging(igcc);

                if (this._topLevelGroup != null)
                {
                    this._topLevelGroup.Dispose();
                }

                this._topLevelGroup = null;

                if (this.unsortedRecordsTree != null)
                {
                    unsortedRecordsTree.Dispose();
                }

                this.unsortedRecordsTree = null;

                if (this._selectedRecords != null)
                {
                    _selectedRecords.Dispose();
                }

                this._selectedRecords = null;

                if (this.primaryKeySortedRecordsTree != null)
                {
                    primaryKeySortedRecordsTree.Dispose();
                }

                this.primaryKeySortedRecordsTree = null;

                if (this._displayElements != null)
                {
                    this._displayElements.Dispose();
                }

                this._displayElements = null;

                if (this._elements != null)
                {
                    this._elements.Dispose();
                }

                this._elements = null;

                if (this._filteredRecords != null)
                {
                    this._filteredRecords.Dispose();
                }

                this._filteredRecords = null;

                if (this._nestedDisplayElements != null)
                {
                    this._nestedDisplayElements.Dispose();
                }

                this._nestedDisplayElements = null;

                if (this._nestedElements != null)
                {
                    this._nestedElements.Dispose();
                }

                this._nestedElements = null;

                if (this._sortedRecords != null)
                {
                    this._sortedRecords.Dispose();
                }

                this._sortedRecords = null;

                if (this._unsortedRecords != null)
                {
                    this._unsortedRecords.Dispose();
                }

                this._unsortedRecords = null;

                if (this._primaryKeySortedRecords != null)
                {
                    this._primaryKeySortedRecords.Dispose();
                }

                this._primaryKeySortedRecords = null;

                UnwireList();

                if (this.relatedTables != null)
                {
                    Table[] tables = new Table[this.relatedTables.Count];
                    relatedTables.CopyTo(tables, 0);
                    foreach (Table table in tables)
                    {
                        if (table != null)
                        {
                            table.Disposed -= new EventHandler(relatedTable_Disposed);
                            table.Dispose();
                        }
                    }
                }

                this.relatedTables = null;

                if (this._relatedTables != null)
                {
                    this._relatedTables.Dispose();
                }

                this._relatedTables = null;

                this.bindingList = null;
                this.bindingSourceList = null;

                if (currentRecordManager != null)
                {
                    this.currentRecordManager.Dispose();
                }

                this.currentRecordManager = null;

                this.eventsTarget = null;
                this.filteredChildTable = null;
                if (this.groupCategoryTable != null)
                {
                    this.groupCategoryTable.Dispose();
                    this.groupCategoryTable = null;
                }

                UnwireTableDescriptor();

                this.lastChangedRecord = null;
                this.lastSortedList = null;
                this.recordComparer = null;
                this.relationDescriptors = null;  // don't dispose - only remove ref
                this.relationParentTable = null;
                this.sourceList = null;
                this.sourceListSortArray = null;
                this.lastPKSortedList = null;

                this.engine = null;
            }

            base.Dispose(disposing);
        }

        #endregion
        #region Element Overrides
        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.VisibleCount;
        }

        /// <summary>Gets the element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.YAmountCount;
        }

        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.ElementCount;
        }

        /// <overload>
        /// <summary>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> of the
        /// parent table descriptor.</summary>
        /// </overload>
        /// <returns>Summary information.</returns>
        /// <override/>
        public ITreeTableSummary[] GetSummaries()
        {
            return base.GetSummaries(this);
        }

        /*UFD:public ITreeTableSummary[] GetUnfilteredSummaries()
        {
            bool summaryChanged;
            return this.GetUnfilteredSummaries(out summaryChanged);
        }*/

        /// <override/>
        /// <summary>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
        /// parent table descriptor.
        /// </summary>
        /// <param name="parentTable">A reference to the parent table of this element.</param>
        /// <param name="summaryChanged">Returns True if changes were detected.</param>
        /// <returns>An array of <see cref="ITreeTableSummary"/> objects.</returns>
        public override ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            if (TopLevelGroup == null)
            {
                summaryChanged = false;
                return null;
            }

            return TopLevelGroup.GetSummaries(parentTable, out summaryChanged);
        }

        /// <summary>
        /// Gets an array of empty <see cref="ITreeTableSummary"/> objects. For each SummaryDescriptor in this
        /// collection, an ITreeTableSummary is created by calling the SummaryDescriptor.CreateSummary method
        /// and passing in NULL as record.
        /// </summary>
        /// <returns>An array of ITreeTableSummary objects, one for each SummaryDescriptor in this collection.</returns>
        public ITreeTableSummary[] GetEmptySummaries()
        {
            return TableDescriptor.Summaries.EmptySummaries;
        }

        /*UFD:
                /// <summary>
                /// Gets an array of empty <see cref="ITreeTableSummary"/> objects. For each SummaryDescriptor in this
                /// collection, an ITreeTableSummary is created by calling the SummaryDescriptor.CreateSummary method
                /// and passing in NULL as record.
                /// </summary>
                /// <returns>An array of ITreeTableSummary objects, one for each SummaryDescriptor in this collection.</returns>
                public ITreeTableSummary[] GetEmptyUnfilteredSummaries()
                {
                    return TableDescriptor.UnfilteredSummaries.EmptySummaries;
                }


                internal TableEmptyUnfilteredSummaries tableEmptyUnfilteredSummaries;

                internal class TableEmptyUnfilteredSummaries : ITreeTableEmptySummaryArraySource, ITableSource
                {
                    Table table;

                    public TableEmptyUnfilteredSummaries(Table table)
                    {
                        this.table = table;
                    }

                    public Table GetTable()
                    {
                        return table;
                    }

                    public ITreeTableSummary[] GetEmptySummaries()
                    {
                        return table.GetEmptyUnfilteredSummaries();
                    }
                }

                /// <summary>
                /// Gets summary information for this element and child elements. The summaries
                /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
                /// parent table descriptor.
                /// </summary>
                /// <param name="parentTable">A reference to the parent table of this element.</param>
                /// <param name="summaryChanged">Returns True if changes are detected.</param>
                /// <returns>An array of <see cref="ITreeTableSummary"/> objects.</returns>
                public virtual ITreeTableSummary[] GetUnfilteredSummaries(out bool summaryChanged)
                {
                    UnsortedRecordsTree e = this.unsortedRecordsTree;
                    if (e != null)
                    {
                        //EnsureInitialized(this);
                        ITreeTableSummary[] ss = e.GetSummaries(this.tableEmptyUnfilteredSummaries, out summaryChanged);
                        if (ss == null)
                            return this.GetEmptyUnfilteredSummaries();

                        ITreeTableSummary[] summaries = new ITreeTableSummary[ss.Length];
                        ss.CopyTo(summaries, 0);
                        return summaries;
                    }
                    summaryChanged = false;
                    return this.GetEmptyUnfilteredSummaries();
                }
        */

        internal TableEmptySummaries tableEmptySummaries;

        internal class TableEmptySummaries : ITreeTableEmptySummaryArraySource, ITableSource
        {
            Table table;

            public TableEmptySummaries(Table table)
            {
                this.table = table;
            }

            public Table GetTable()
            {
                return table;
            }

            public ITreeTableSummary[] GetEmptySummaries()
            {
                return table.GetEmptySummaries();
            }
        }

        /// <summary>Gets the number of filterd records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return TreeEntries.FilteredRecordCount;
        }

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return TreeEntries.RecordCount;
        }

        /// <summary>Walks down to the child branches and resets the counters.</summary>
        /// <param name="notifyCounterSource">If true, notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            this.OnInvalidatingCounters(new TableEventArgs(this));

            TreeEntries.InvalidateCounterTopDown(notifyCounterSource);
            _isRecordFiltersDirty = false;
        }

        /// <summary>Resets the summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            this.OnInvalidatingSummaries(new TableEventArgs(this));

            TreeEntries.InvalidateSummariesTopDown();
            ////UFD: unsortedRecordsTree.InvalidateSummariesTopDown();
            _isSummaryDirty = false;
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Resets the summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
        }

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry()
        {
            return null;
        }

        internal void TouchDisplayElementsInRelatedTables()
        {
            // the following line fixes an issue when the very first time a nested table is expanded,
            // sometimes the GetFilterChildTable is not cached correctly.
            Table[] tables = new Table[this.relatedTables.Count];
            relatedTables.CopyTo(tables, 0); // trigger SynchronizeRelatedTables
            foreach (Table t in tables)
            {
                Element el = t.DisplayElements[0];
            }
        }

        /// <summary>
        /// This virtual method is called from <see cref="OnEnsureInitialized"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="EnsureInitialized"/> call.</param>
        /// <returns>
        /// True if changes were detected and the object was updated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            if (this.inInitialize)
            {
                return false;
            }

            if (this.TreeEntries.Count == 0)
            {
            }

            if (!isDirty && (_isRecordFiltersDirty || _isSummaryDirty))
            {
                //// Check if virtual mode status is changed
                bool setVirtualMode = false;
                bool setWithoutCounter = false;
                bool setRecordsAsDisplayElements = false;

                CheckOptimizations(out setVirtualMode, out setWithoutCounter, out setRecordsAsDisplayElements);

                isDirty = setVirtualMode != VirtualMode;
                ////|| setWithoutCounter != WithoutCounter;
                this.virtualMode = setVirtualMode;

                if (setWithoutCounter != WithoutCounter)
                {
                    _isRecordFiltersDirty = true;
                    if (_topLevelGroup != null)
                    {
                        _topLevelGroup.Details.TreeEntries.WithoutCounter = setWithoutCounter;
                    }

                    this.WithoutCounter = setWithoutCounter;
                    _isRecordFiltersDirty = true;
                }
            }

            if (isDirty)
            {
                isDirty = false;
                _isSummaryDirty = false;
                _isRecordFiltersDirty = false;
                ClearCollectionCaches();
                this.CategorizeElements();
                ////TouchDisplayElementsInRelatedTables();

                return true;
            }

            if (sender is PrimaryKeySortedRecordsCollection)
            {
                if (_primaryKeyColumnsVersion != TableDescriptor.PrimaryKeyColumns.Version)
                {
                    this.InitUnsortedRecords(false);
                    this.InitPrimaryKeys();
                }
            }

            bool b = _isRecordFiltersDirty;

            if (_isRecordFiltersDirty && !(sender is UnsortedRecordsCollection))
            {
                this.InvalidateCounterTopDown(true);

                // Hide current record if filter was changed
                if (CurrentElement != null && CurrentElement.GetVisibleCount() == 0)
                {
                    CurrentRecordManager.InternalSetCurrentRecord(null);
                }

                ClearCollectionCaches();
                _isSummaryDirty = true;
            }

            if (_isSummaryDirty && !(sender is UnsortedRecordsCollection))
            {
                ////if (!b)
                ////    this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, false, false);
                if (this.TableDescriptor.Summaries.Count > 0)
                {
                    ////UFD: || this.TableDescriptor.UnfilteredSummaries.Count > 0

                    this.InvalidateSummariesTopDown();
                }
                else
                {
                    _isSummaryDirty = false;
                }

                ClearCollectionCaches();
                ////this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, true);
                ////TouchDisplayElementsInRelatedTables();

                return false;
            }

            return false;
        }

        /// <summary>Gets the parent table descriptor.</summary>
        /// <override/>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override TableDescriptor ParentTableDescriptor
        {
            get
            {
                return _tableDescriptor;
            }
        }

        /// <summary>
        /// Returns a string holding the element.
        /// </summary>
        /// <returns>
        /// String representation of the current object.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;

            if (this._tableDescriptor != null && this._tableDescriptor.Name != null)
            {
                return GetType().Name + "(" + TableDescriptor.Name.ToString() + ", " + tableId.ToString() + isdisposed + ")";
            }
            else
            {
                return GetType().Name + "(" + tableId.ToString() + isdisposed + ")";
            }
        }

        ////int fieldsVersion = -1;

        bool BaseEnsureInitialized(object sender, bool notifyParent)
        {
            if (IsDisposed)
            {
                return false;
            }

            if (notifyParent)
            {
                if (this.RelationParentTable != null)
                {
                    this.RelationParentTable.EnsureInitialized(sender, notifyParent);
                }
            }

            //// Avoid surprises in InitTopLevelGroup ..
            int v = 0;
            TableDescriptor td = this.TableDescriptor;
            v = td.ItemPropertiesVersion;
            if (td.IsDisposed)
            {
                return false;
            }
            ////            v = td.ExpressionFields.version;
            ////            if (td.IsDisposed)
            ////                return false;
            ////            v = td.UnboundFields.version;
            ////            if (td.IsDisposed)
            ////                return false;
            ////            v = td.Relations.Version;
            ////            if (td.IsDisposed)
            ////                return false;
            ////            v = td.Engine.SourceListVersion;
            ////            if (td.IsDisposed)
            ////                return false;
            ////
            ////            foreach (Table t in this.relatedTables)
            ////            {
            ////                if (t.TableDescriptor.IsDisposed)
            ////                    throw new InvalidOperationException();
            ////
            ////                t.EnsureInitialized(sender, false);
            ////            }
            ////
            ////            if (IsDisposed)
            ////                return false;

            EnsureSourceList();

            if (IsDisposed)
            {
                return false;
            }

            ////            if (fieldsVersion != TableDescriptor.Fields.Version)
            ////            {
            ////                fieldsVersion = TableDescriptor.Fields.Version;
            ////                TableDirty = true;
            ////            }
            ////            object obj = this.TableDescriptor.Engine.GetSourceList();
            if (this.inInitialize)
            {
                return true;
            }

            return base.EnsureInitialized(sender, notifyParent && this.ParentRecord == null);
        }

        bool sourceListWarningDisplayed = false;

        IEnumerable originalSourceList = null;

        public IEnumerable OriginalSourceList
        {
            get { return originalSourceList; }
        }

        internal void EnsureSourceList()
        {
            IEnumerable list = SourceList;
            if (originalSourceList != null)
            {
                list = originalSourceList;
            }

            if (Engine != null)
            {
                RelationDescriptor parentRelation = this.TableDescriptor.ParentRelation;
                if (parentRelation == null)
                {
                    list = Engine.GetSourceList();
                }
                else if (parentRelation.RelationKind != RelationKind.UniformChildList && parentRelation.ChildTableName != string.Empty)
                {
                    SourceListSetEntry entry = Engine.SourceListSet[parentRelation.ChildTableName];
                    if (entry != null)
                    {
                        list = entry.List;
                    }
                    else if (!sourceListWarningDisplayed)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(
                            "Possible incorrect RelationDescriptor.ChildTableName for Relation {0}.\r\nEngine.SourceListSet[{1}] returned null.\r\n",
                            parentRelation.Name,
                            parentRelation.ChildTableName);
                        sb.AppendFormat("Did you forget to call Engine.SourceListSet.Add(new SourceListSetEntry({0}, list)\r\n", parentRelation.ChildTableName);
                        sb.Append("or did you specify a wrong ChildTableName?\r\n");

                        sb.Append("Valid ChildTableName entries are: ");
                        foreach (SourceListSetEntry e in Engine.SourceListSet)
                        {
                            sb.Append("'" + e.Name + "' ");
                        }

#if DEBUG
                        Debug.WriteLine("Warning: " + sb.ToString());
#else
                        Console.WriteLine("Warning: " + sb.ToString());
#endif
                        sourceListWarningDisplayed = true;

                        if (Engine.ThrowExceptionIfSourceListSetEntryNotFound && !Engine.GetDesignMode())
                        {
                            throw new InvalidOperationException(sb.ToString());
                        }
                    }
                }
            }

            bool diff = false;
            if (originalSourceList != null)
            {
                diff = !Object.ReferenceEquals(originalSourceList, list);
            }
            else
            {
                diff = !Object.ReferenceEquals(SourceList, list);
            }

            if (diff)
            {
                originalSourceList = null;
                if (list is IList || list == null)
                {
                    SourceList = (IList)list;
                }
                else
                {
                    // Create a sourcelist with items.
                    ArrayList al = new ArrayList();
                    if (TableDescriptor.RelationChildColumns.Count == 0)
                    {
                        foreach (object item in list)
                        {
                            al.Add(item);
                        }
                    }

                    originalSourceList = list;
                    SourceList = al;
                }
            }
        }

        #endregion
        #region IElementTreeTableSource Implementation
        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            return groupCategoryTable;
        }
        #endregion
        #region IContainerElement Implementation
        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }
        #endregion
        #region Parents
        /// <summary>
        /// Gets the parent table of this table or NULL if this table is not a child table of a relation.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Table RelationParentTable
        {
            get
            {
                return relationParentTable;
            }
        }

        /// <summary>
        /// The TableDescriptor with schema information about this table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return _tableDescriptor;
            }
        }

        #endregion
        #region Version
        /// <summary>
        /// Gets the <see cref="Syncfusion.Grouping.Engine.Version"/> of the <see cref="Engine"/>.
        /// </summary>
        public int EngineVersion
        {
            get
            {
                if (this._tableDescriptor != null && this._tableDescriptor.Engine != null)
                {
                    return this._tableDescriptor.Engine.version;
                }

                return -1;
            }

            set
            {
                if (this._tableDescriptor != null && this._tableDescriptor.Engine != null)
                {
                    this._tableDescriptor.Engine.version = value;
                }
            }
        }

        /// <summary>
        /// Increases the <see cref="EngineVersion"/>. Collection caches will be cleared since
        /// they compare the cache version counter with the engine version counter.
        /// </summary>
        public void ClearCollectionCaches()
        {
            this.EngineVersion++;
            ////sourceListVersion++;
        }

        /// <summary>
        /// Sets the table dirty (<see cref="TableDirty"/>) and increses the engine and sourcelist
        /// version counter.
        /// </summary>
        public void Reload()
        {
            isDirty = true;
            EngineVersion++;
            SourceListVersion++;
        }

        #endregion
        #region ExceptionRaised Event
        internal void NotifyException(string method, Exception ex)
        {
            if (this.inCurrentRecordEndEditOrLeave)
            {
                throw ex;
            }

            OnExceptionRaised(new ExceptionRaisedEventArgs(this, method, ex));
        }

        /// <summary>
        /// Occurs when an unknown exception has been cached while modifying underlying data in the datasource.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs when an unknown exception has been cached while modifying underlying data in the datasource.")]
        public event ExceptionRaisedEventHandler ExceptionRaised;

        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ExceptionRaisedEventArgs" /> that contains the event data.</param>
        protected virtual void OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnExceptionRaised(e);
            }

            if (ExceptionRaised != null)
            {
                ExceptionRaised(this, e);
            }
        }

        #endregion
        #region Events
        /// <summary>
        /// Occurs before a group is expanded.
        /// </summary>
        [Description("Occurs before a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanding;

        /// <summary>
        /// Occurs after a group is expanded.
        /// </summary>
        [Description("Occurs after a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanded;

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsing;

        /// <summary>
        /// Occurs after a group is collapsed.
        /// </summary>
        [Description("Occurs after a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsed;

        /// <summary>
        /// Occurs before a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs before a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanding;

        /// <summary>
        /// Occurs after a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs after a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanded;

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        [Description("Occurs before a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleting;

        /// <summary>
        /// Occurs after a record is deleted.
        /// </summary>
        [Description("Occurs after a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleted;

        /// <summary>
        /// Occurs before a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs before a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsing;

        /// <summary>
        /// Occurs after a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs after a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsed;

        /// <summary>
        /// Occurs before records are categorized after a table is marked dirty (<see cref="Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Table.EnsureInitialized"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs before records are categorized after a table is marked dirty")]
        public event TableEventHandler CategorizingRecords;

        /// <summary>
        /// Occurs after records are categorized after a table is marked dirty (<see cref="Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Table.EnsureInitialized"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after records are categorized after a table is marked dirty")]
        public event TableEventHandler CategorizedRecords;

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group is sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Description("Occurs before the records for a group are sorted.")]
        [Category("Table")]
        public event GroupEventHandler SortingItemsInGroup;

        /// <summary>
        /// Occurs after the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group is sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after the records for a group are sorted.")]
        public event GroupEventHandler SortedItemsInGroup;

        ////public event GroupEventHandler GroupSummaryChanged;

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and will force a repaint of the specified summary if it is visible
        /// when this event was raised.
        /// </remarks>
        [Description("Occurs when a summary has been marked dirty.")]
        [Category("Table")]
        public event GroupEventHandler GroupSummaryInvalidated;

        /// <summary>
        /// Occurs when a new group is added in a table after the table was categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a new group is added in a table after the table was categorized and when a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupAdded;

        /// <summary>
        /// Occurs when a group was removed from a table after the table was categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a group was removed from a table after the table was categorized and when a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupRemoving;

        /// <summary>
        /// Occurs when a record in the underlying data source was added, removed, or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying data source was added, removed, or changed and before the table is updated.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanging;

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer a chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanged;

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer a chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChanged;

        /// <summary>
        /// Occurs right after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer a chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event right after the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs after the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChangedCompleted;

        /// <summary>
        /// Occurs when the <see cref="Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> is called
        /// and before all counters are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateCounterTopDown method of a Table is called.")]
        public event TableEventHandler InvalidatingCounters;

        /// <summary>
        /// Occurs when the <see cref="Table.InvalidateSummariesTopDown"/> of a <see cref="Table"/> is called
        /// and before all summaries in that table are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateSummariesTopDown of a table is called.")]
        public event TableEventHandler InvalidatingSummaries;

        /// <summary>
        /// Raises the <see cref="InvalidatingCounters"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingCounters(TableEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnInvalidatingCounters(e);
            }

            if (InvalidatingCounters != null)
            {
                InvalidatingCounters(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="InvalidatingSummaries"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingSummaries(TableEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnInvalidatingSummaries(e);
            }

            if (InvalidatingSummaries != null)
            {
                InvalidatingSummaries(this, e);
            }
        }

        internal bool NotifyBeginEditCalled()
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.BeginEditCalled, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyBeginEditComplete(bool success)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.BeginEditComplete, this, CurrentElement, success);
            OnCurrentRecordContextChange(e);
        }

        internal bool NotifyEndEditCalled()
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.EndEditCalled, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyEndEditComplete(bool success)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.EndEditComplete, this, CurrentElement, success);
            OnCurrentRecordContextChange(e);
        }

        internal bool NotifyCancelEditCalled()
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.CancelEditCalled, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyCancelEditComplete(bool success)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.CancelEditComplete, this, CurrentElement, success);
            OnCurrentRecordContextChange(e);
        }

        internal bool NotifyNavigateCalled(Element record)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.NavigateCalled, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyNavigateComplete(bool success, Element previousRecord)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.NavigateComplete, this, previousRecord, success);
            OnCurrentRecordContextChange(e);
        }

        internal bool NotifyCurrentFieldChanged()
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.CurrentFieldChanged, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal bool NotifyLeaveRecordCalled()
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.LeaveRecordCalled, this, CurrentElement, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyLeaveRecordComplete(bool success, Element previousRecord)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.LeaveRecordComplete, this, previousRecord, success);
            OnCurrentRecordContextChange(e);
        }

        internal bool NotifyEnterRecordCalled(Element record)
        {
            if (FilteredChildTable != null && record.ParentTable == this && record.ParentChildTable != this.FilteredChildTable)
            {
                NestedTable parentNestedTable = FilteredChildTable.ParentNestedTable;
                if (!(CurrentRecordManager.NavigateTo(parentNestedTable) == parentNestedTable))
                {
                    return false;
                }
            }

            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.EnterRecordCalled, this, record, true);
            OnCurrentRecordContextChange(e);
            return !e.Cancel;
        }

        internal void NotifyEnterRecordComplete(bool success)
        {
            CurrentRecordContextChangeEventArgs e = new CurrentRecordContextChangeEventArgs(CurrentRecordAction.EnterRecordComplete, this, CurrentElement, success);
            OnCurrentRecordContextChange(e);
        }

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSourceListRecordChanged(e);
            }
#if DEBUG

            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Action, e.OldIndex, e.NewIndex, e.Record);
            }
#else

            ;
#endif

            if (SourceListRecordChanged != null)
            {
                SourceListRecordChanged(this, e);
            }

            if (e.RaiseDisplayElementChanged)
            {
                if (e.Record != null && !e.Record.GetVisibleInHierarchy())
                {
                    return;
                }

                bool inAddNewBeginEdit = e.Action == RecordChangedType.Added && CurrentRecordManager.InBeginEdit;
                if (inAddNewBeginEdit || (e.Action == RecordChangedType.Changed && e.NewIndex == e.OldIndex))
                {
                    this.EngineTable.RaiseDisplayElementChanged(e.Record, -1, -1, true, false, false);
                }
                else
                {
                    bool syncCurrentRecordPos = false; /*CurrentElement != null && !(
                        CurrentRecordManager.InBeginEdit||
                        CurrentRecordManager.InCancelEdit||
                        CurrentRecordManager.InEndEdit||
                        CurrentRecordManager.InEnterRecord||
                        CurrentRecordManager.InLeaveRecord||
                        CurrentRecordManager.InNavigate);*/

                    if (e.Action == RecordChangedType.Removed)
                    {
                        this.EngineTable.RaiseDisplayElementChanged(this, 1, 0, true, syncCurrentRecordPos, false);
                    }
                    else
                    {
                        this.EngineTable.RaiseDisplayElementChanged(this, 0, 1, true, syncCurrentRecordPos, false);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            bool inAddNewBeginEdit = e.Action == RecordChangedType.Added && CurrentRecordManager.InBeginEdit;

            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSourceListRecordChanging(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Action, e.OldIndex, e.NewIndex, e.Record);
            }
#else

            ;
#endif
            if (SourceListRecordChanging != null)
            {
                SourceListRecordChanging(this, e);
            }

            if (e.RaiseDisplayElementChanged)
            {
                if (inAddNewBeginEdit || (e.Action == RecordChangedType.Changed && e.NewIndex == e.OldIndex))
                {
                    this.EngineTable.RaiseDisplayElementChanging(e.Record, -1, -1, true, false, false);
                }
                else
                {
                    bool syncCurrentRecordPos = false; /*CurrentElement != null && !(
                        CurrentRecordManager.InBeginEdit||
                        CurrentRecordManager.InCancelEdit||
                        CurrentRecordManager.InEndEdit||
                        CurrentRecordManager.InEnterRecord||
                        CurrentRecordManager.InLeaveRecord||
                        CurrentRecordManager.InNavigate);*/

                    if (e.Action == RecordChangedType.Removed)
                    {
                        this.EngineTable.RaiseDisplayElementChanging(this, 1, 0, true, syncCurrentRecordPos, false);
                    }
                    else
                    {
                        this.EngineTable.RaiseDisplayElementChanging(this, 0, 1, true, syncCurrentRecordPos, false);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSourceListListChanged(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.ListChangedType, e.NewIndex, e.OldIndex);
            }
#else

            ;
#endif
            if (SourceListListChanged != null)
            {
                SourceListListChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="SourceListListChangedCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSourceListListChangedCompleted(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.ListChangedType, e.NewIndex, e.OldIndex);
            }
#else
            ;
#endif
            if (SourceListListChangedCompleted != null)
            {
                SourceListListChangedCompleted(this, e);
            }
        }

        ////        void RaiseSummaryChanged(Group g)
        ////        {
        ////            OnGroupSummaryChanged(new GroupEventArgs(g));
        ////        }

        internal void RaiseGroupSummaryInvalidated(Group g)
        {
            OnGroupSummaryInvalidated(new GroupEventArgs(g));
        }

        ////
        ////        /// <summary>
        ////        /// Raises the <see cref="GroupSummaryChanged"/> event.
        ////        /// </summary>
        ////        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnGroupSummaryChanged(GroupEventArgs e)
        ////        {
        ////            //            TraceUtil.TraceCurrentMethodInfo(e.Group);
        ////            ITreeTableSummary[] summaries = e.Group.GetSummaries();
        ////
        ////#if DEBUG
        ////            if (Switches.GroupingEngine.TraceVerbose && summaries != null && summaries.Length > 0)
        ////            {
        ////                foreach (ITreeTableSummary summary in summaries)
        ////                    Trace.WriteLine(summary.ToString());
        ////            }
        ////#endif
        ////            if (GroupSummaryChanged != null)
        ////                GroupSummaryChanged(this, e);
        ////        }

        /// <summary>
        /// Raises the <see cref="GroupSummaryInvalidated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupSummaryInvalidated(e);
            }

            if (GroupSummaryInvalidated != null)
            {
                GroupSummaryInvalidated(this, e);
            }
        }

        internal void RaiseGroupAdded(GroupEventArgs e)
        {
            OnGroupAdded(e);
        }

        /// <summary>
        /// Raises the <see cref="GroupAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupAdded(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupAdded(e);
            }

            ////this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, false, false);
            ////TraceUtil.TraceCurrentMethodInfo(e.Group);
            if (GroupAdded != null)
            {
                GroupAdded(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GroupRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupRemoving(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupRemoving(e);
            }

            ////this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, false, false);

            if (GroupRemoving != null)
            {
                GroupRemoving(this, e);
            }
        }

        ////        /// <summary>
        ////        /// Raises the <see cref="GroupRemoving"/> event.
        ////        ///// </summary>
        ////        ///// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnGroupRemoving(GroupEventArgs e)
        ////        {
        ////            ////            TraceUtil.TraceCurrentMethodInfo(e.Group);
        ////            if (GroupRemoving != null)
        ////                GroupRemoving(this, e);
        ////
        ////            bool leaveRec = this.IsSameGroup(e.Group, CurrentElement);//// && CurrentElement != e.Group.Caption;
        ////            if (leaveRec)
        ////                CurrentElement = null;
        ////            bool syncRec = CurrentElement != null; ////DisplayElements.IndexOf(CurrentElement) >= collapseFrom;
        ////
        ////            ////this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, syncRec, false);
        ////        }

        internal bool RaiseGroupCollapsing(Group group, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize || group.SectionEntries.IsInitializing)
            {
                return true;
            }

            GroupEventArgs e = new GroupEventArgs(group);
            OnGroupCollapsing(e);

            if (raiseDisplayElementChangeEvents && group.GetVisibleInHierarchy() && !e.Cancel)
            {
                int rowIndex = DisplayElements.IndexOf(group);
                int collapseCount = group.GetVisibleCount();
                collapsing_CollapseCount = collapseCount;
                ////int collapseTo = rowIndex+collapseCount-1;
                int newCount = group.Caption.GetVisibleCount();
                int collapseFrom = rowIndex + newCount;
                bool leaveRec = this.IsSameGroup(group, CurrentElement) && CurrentElement != group.Caption;
                if (leaveRec && CurrentRecordManager.ForceShowCurrentRecord)
                {
                    CurrentRecordManager.NavigateTo(group.Caption, false, false);
                }

                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= collapseFrom;
                this.EngineTable.RaiseDisplayElementChanging(group, collapseCount, newCount, true, syncRec, false, false);
            }

            return !e.Cancel;
        }

        internal void RaiseGroupCollapsed(Group group, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize || group.SectionEntries.IsInitializing)
            {
                return;
            }

            if (raiseDisplayElementChangeEvents && group.GetVisibleInHierarchy())
            {
                int rowIndex = DisplayElements.IndexOf(group);
                int collapseCount = collapsing_CollapseCount; ////group.GetVisibleCount();
                int newCount = group.Caption.GetVisibleCount();
                this.EngineTable.RaiseDisplayElementChanged(group, collapseCount, newCount, true, true, false, false);
            }

            OnGroupCollapsed(new GroupEventArgs(group));
        }

        internal bool RaiseGroupExpanding(Group group, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize || group.SectionEntries.IsInitializing)
            {
                return true;
            }

            GroupEventArgs e = new GroupEventArgs(group);
            OnGroupExpanding(e);

            if (e.Cancel)
            {
                return false;
            }

            // Populate group on demand when SourceList
            // is a IPassThroughGroupingResult
            PopulatePassThroughGroupIfEmpty(group);

            if (raiseDisplayElementChangeEvents && group.GetVisibleInHierarchy() && !e.Cancel)
            {
                int expandFrom = DisplayElements.IndexOf(group) + group.Caption.GetVisibleCount();
                int expandCount = group.Caption.GetVisibleCount() + group.Details.TreeEntries.VisibleCount;
                int oldCount = group.Caption.GetVisibleCount();
                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= expandFrom;
                this.EngineTable.RaiseDisplayElementChanging(group, oldCount, expandCount, true, syncRec, false, false);
            }

            return !e.Cancel;
        }

        internal void RaiseGroupExpanded(Group group, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize || group.SectionEntries.IsInitializing)
            {
                return;
            }

            if (raiseDisplayElementChangeEvents && group.GetVisibleInHierarchy())
            {
                int expandFrom = DisplayElements.IndexOf(group) + 1;
                int expandCount = group.GetVisibleCount();
                int oldCount = group.Caption.GetVisibleCount();
                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= expandFrom;
                this.EngineTable.RaiseDisplayElementChanged(group, oldCount, expandCount, true, true, false, false);
            }

            OnGroupExpanded(new GroupEventArgs(group));
        }

        /// <summary>
        /// Raises the <see cref="GroupCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsing(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupCollapsing(e);
            }
#if DEBUG

            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Group);
            }
#else
            ;
#endif
            if (GroupCollapsing != null)
            {
                GroupCollapsing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GroupCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsed(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupCollapsed(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Group);
            }
#else

            ;
#endif
            if (GroupCollapsed != null)
            {
                GroupCollapsed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GroupExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanding(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupExpanding(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Group);
            }
#else
            ;
#endif
            if (GroupExpanding != null)
            {
                GroupExpanding(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GroupExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanded(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGroupExpanded(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Group);
            }
#else
            ;
#endif
            if (GroupExpanded != null)
            {
                GroupExpanded(this, e);
            }
        }

        internal bool RaiseRecordCollapsing(Record record, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize)
            {
                return true;
            }

            RecordEventArgs e = new RecordEventArgs(record);
            OnRecordCollapsing(e);

            if (!e.Cancel && raiseDisplayElementChangeEvents && record.GetVisibleInHierarchy())
            {
                int rowIndex = DisplayElements.IndexOf(record);
                int collapseCount = record.GetVisibleCount();
                collapsing_CollapseCount = collapseCount;
                int newCount = record.GetRecordRowsVisibleCount();
                int collapseFrom = rowIndex + newCount;
                bool leaveRec = false;
                if (CurrentElement != null && this.CurrentRecord != record)
                {
                    leaveRec = this.CurrentElement.ParentRecord == record;
                }

                if (leaveRec)
                {
                    CurrentRecordManager.NavigateTo(record, false, false);
                }

                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= collapseFrom;
                this.EngineTable.RaiseDisplayElementChanging(record, collapseCount, newCount, true, syncRec, false, false);
            }

            return !e.Cancel;
        }

        internal void RaiseRecordCollapsed(Record record, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize)
            {
                return;
            }

            if (raiseDisplayElementChangeEvents && record.GetVisibleInHierarchy())
            {
                int rowIndex = DisplayElements.IndexOf(record);
                int collapseCount = collapsing_CollapseCount;
                int newCount = record.GetVisibleCount();
                this.EngineTable.RaiseDisplayElementChanged(record, collapseCount, newCount, true, false, false, false);
            }

            OnRecordCollapsed(new RecordEventArgs(record));
        }

        internal bool RaiseRecordExpanding(Record record, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize)
            {
                return true;
            }

            RecordEventArgs e = new RecordEventArgs(record);
            OnRecordExpanding(e);

            if (e.Cancel)
            {
                return false;
            }

            PopulateRecordChildTablesIfEmpty(record);

            if (!e.Cancel && e.RaiseDisplayElementChanged && raiseDisplayElementChangeEvents && record.GetVisibleInHierarchy())
            {
                int expandFrom = DisplayElements.IndexOf(record) + record.GetRecordRowsVisibleCount();
                int expandCount = record.GetNestedTablesVisibleCount();
                int oldCount = record.GetVisibleCount();
                int newCount = expandCount + record.GetRecordRowsVisibleCount();
                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= expandFrom;
                this.EngineTable.RaiseDisplayElementChanging(record, oldCount, newCount, true, syncRec, false, false);
            }

            return !e.Cancel;
        }

        internal void RaiseRecordExpanded(Record record, bool raiseDisplayElementChangeEvents)
        {
            if (this.inInitialize)
            {
                return;
            }

            RecordEventArgs e = new RecordEventArgs(record);
            OnRecordExpanded(e);

            if (e.RaiseDisplayElementChanged && raiseDisplayElementChangeEvents && record.GetVisibleInHierarchy())
            {
                int expandFrom = DisplayElements.IndexOf(record) + record.GetDisplayElementOffsetOfFirstNestedTable();
                int expandCount = record.GetNestedTablesVisibleCount();
                int oldCount = record.GetRecordRowsVisibleCount();
                int newCount = expandCount + record.GetRecordRowsVisibleCount();
                bool syncRec = DisplayElements.IndexOf(CurrentElement) >= expandFrom;
                this.EngineTable.RaiseDisplayElementChanged(record, oldCount, newCount, true, true, false, false);
            }
        }

        internal bool RaiseRecordDeleting(Record record)
        {
            RecordEventArgs e = new RecordEventArgs(record);
            OnRecordDeleting(e);

            return !e.Cancel;
        }

        internal void RaiseRecordDeleted(Record record)
        {
            RecordEventArgs e = new RecordEventArgs(record);
            OnRecordDeleted(e);
        }

        /// <summary>
        /// Raises the <see cref="RecordCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsing(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordCollapsing(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordCollapsing != null)
            {
                RecordCollapsing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsed(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordCollapsed(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordCollapsed != null)
            {
                RecordCollapsed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanding(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordExpanding(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordExpanding != null)
            {
                RecordExpanding(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanded(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordExpanded(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordExpanded != null)
            {
                RecordExpanded(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleting(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordDeleting(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordDeleting != null)
            {
                RecordDeleting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordDeleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleted(RecordEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordDeleted(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Record);
            }
#else
            ;
#endif
            if (RecordDeleted != null)
            {
                RecordDeleted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        void BaseCategorizingRecords(TableEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCategorizingRecords(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
            ;
#endif
            if (CategorizingRecords != null)
            {
                CategorizingRecords(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CategorizedRecords"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizedRecords(TableEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCategorizedRecords(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
            ;
#endif
            if (CategorizedRecords != null)
            {
                CategorizedRecords(this, e);
            }
        }

        ////        ///// <summary>
        ////        ///// Raises the <see cref="PrepareApplyFilter"/> event.
        ////        ///// </summary>
        ////        ///// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnPrepareApplyFilter(TableEventArgs e)
        ////        {
        ////            ////            TraceUtil.TraceCurrentMethodInfo();
        ////            if (PrepareApplyFilter != null)
        ////                PrepareApplyFilter(this, e);
        ////
        ////            bool leaveRec = CurrentRecord != null && !ParentTableDescriptor.RecordFilters.CompareRecord(CurrentRecord);
        ////            if (leaveRec)
        ////                CurrentRecordManager.LeaveRecord(false);////= this.TopLevelGroup.Caption;
        ////
        ////            ////EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, true, leaveRec);
        ////        }

        ////        ///// <summary>
        ////        ///// Raises the <see cref="FilterCompleted"/> event.
        ////        ///// </summary>
        ////        ///// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnFilterCompleted(TableEventArgs e)
        ////        {
        ////            ////EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, false);
        ////
        ////            ////            TraceUtil.TraceCurrentMethodInfo();
        ////            if (FilterCompleted != null)
        ////                FilterCompleted(this, e);
        ////        }

        internal void RaiseSortingItemsInGroup(Group group)
        {
            OnSortingItemsInGroup(new GroupEventArgs(group));
        }

        /// <summary>
        /// Raises the <see cref="SortingItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSortingItemsInGroup(e);
            }

            if (SortingItemsInGroup != null)
            {
                SortingItemsInGroup(this, e);
            }
        }

        internal void RaiseSortedItemsInGroup(Group group)
        {
            OnSortedItemsInGroup(new GroupEventArgs(group));
        }

        /// <summary>
        /// Raises the <see cref="SortedItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSortedItemsInGroup(e);
            }

            if (SortedItemsInGroup != null)
            {
                SortedItemsInGroup(this, e);
            }
        }

        ////        ///// <summary>
        ////        ///// Raises the <see cref="PrepareInvalidateSummariesTopDown"/> event.
        ////        ///// </summary>
        ////        ///// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnPrepareInvalidateSummariesTopDown(TableEventArgs e)
        ////        {
        ////            ////            TraceUtil.TraceCurrentMethodInfo();
        ////            if (PrepareInvalidateSummariesTopDown != null)
        ////                PrepareInvalidateSummariesTopDown(this, e);
        ////        }

        ////        ///// <summary>
        ////        ///// Raises the <see cref="summariesCompleted"/> event.
        ////        ///// </summary>
        ////        ///// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        ////        protected virtual void OnSummariesRefreshed(TableEventArgs e)
        ////        {
        ////            ////            TraceUtil.TraceCurrentMethodInfo();
        ////            if (SummariesRefreshed != null)
        ////                SummariesRefreshed(this, e);
        ////        }

        #endregion
        #region CurrentRecordContextChange Event

        /// <summary>
        /// Occurs before and after the status of the current record was changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information which current record state was changed.
        /// </summary>
        [Description("Occurs before and after the status of the current record was changed.")]
        [Category("Table")]
        public event CurrentRecordContextChangeEventHandler CurrentRecordContextChange;

        /// <summary>
        /// Raises the <see cref="CurrentRecordContextChange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordContextChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentRecordContextChange(e);
            }
#if DEBUG
            if (Switches.GroupingEngine.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
            ;
#endif
            if (CurrentRecordContextChange != null)
            {
                CurrentRecordContextChange(this, e);
            }
        }
        #endregion
        #region CurrentRecordManagerReset Event

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> is called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and resets any "Current Cell" state when this
        /// event is raised.
        /// </remarks>
        [Description("Occurs when the CurrentRecordManager.Reset method of the CurrentRecordManager is called.")]
        [Category("Table")]
        public event EventHandler CurrentRecordManagerReset;

        /// <summary>
        /// Raises the <see cref="CurrentRecordManagerReset"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentRecordManagerReset(e);
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (CurrentRecordManagerReset != null)
            {
                CurrentRecordManagerReset(this, e);
            }
        }

        internal void RaiseCurrentRecordManagerReset()
        {
            OnCurrentRecordManagerReset(new TableEventArgs(this));
        }

        #endregion
        #region Edit Record
        /// <summary>
        /// Navigates to the <see cref="AddNewRecord"/> and calls <see cref="BeginEdit"/>.
        /// </summary>
        public void AddNew()
        {
            CurrentRecordManager.AddNew();
        }

        /// <summary>
        /// Switches the current record into edit mode. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        public void BeginEdit()
        {
            CurrentRecordManager.BeginEdit();
        }

        /// <summary>
        /// Ends edit mode for the current record. If changes are detected, they will be saved to the
        /// underlying data source. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        public void EndEdit()
        {
            CurrentRecordManager.EndEdit();
        }

        /// <summary>
        /// Cancels editing for the current record. Changes in the current record are
        /// discarded. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        public void CancelEdit()
        {
            CurrentRecordManager.CancelEdit();
        }

        #endregion
        #region IsSame
        /// <summary>
        /// Determines if the row at the specified displayElementIndex is a child of the specified record.
        /// </summary>
        /// <param name="r">The record to be tested.</param>
        /// <param name="displayElementIndex">The row index to be tested.</param>
        /// <returns>True if the row belongs to the record; False otherwise.</returns>
        public bool IsSameRecord(Record r, int displayElementIndex)
        {
            if (r == null)
            {
                return false;
            }

            int displayElementIndex1 = this.DisplayElements.IndexOf(r);
            return displayElementIndex >= displayElementIndex1 && displayElementIndex < displayElementIndex1 + r.GetRecordRowsVisibleCount();
        }

        /// <summary>
        /// If the specified element is a <see cref="Record"/>, it returns the element; if the element is a <see cref="NestedTable"/>, it
        /// returns the <see cref="Element.ParentRecord"/>.
        /// </summary>
        /// <param name="el">The element whose record should be returned.</param>
        /// <returns>Returns Record.</returns>
        public Record GetRecordOrParentRecord(Element el)
        {
            Record r = null;
            if (el is Record)
            {
                r = (Record)el;
            }
            else if (el is NestedTable)
            {
                r = ((NestedTable)el).ParentRecord;
            }

            return r;
        }

        /// <summary>
        /// Determines if the specified element is a child or grandchild of a group.
        /// </summary>
        /// <param name="g">The parent group.</param>
        /// <param name="r">The element to be tested.</param>
        /// <returns>true if element is contained in group; false otherwise.</returns>
        public bool IsSameGroup(Group g, Element r)
        {
            if (r == null)
            {
                return false;
            }

            Group p = r.ParentGroup;
            while (p != null)
            {
                if (p == g)
                {
                    return true;
                }

                p = p.ParentGroup;
            }

            return false;
        }
        #endregion
        #region Current Record
        /// <summary>
        /// Gets a reference to the <see cref="Syncfusion.Grouping.CurrentRecordManager"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CurrentRecordManager CurrentRecordManager
        {
            get
            {
                if (currentRecordManager == null)
                {
                    currentRecordManager = new CurrentRecordManager(this);
                }

                return currentRecordManager;
            }
        }

        AddNewRecord _addNewRecord;

        /// <summary>
        /// Gets a reference to the <see cref="Syncfusion.Grouping.AddNewRecord"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Record AddNewRecord
        {
            get
            {
                if (_addNewRecord != null && _addNewRecord.ParentElement != null)
                {
                    return _addNewRecord;
                }

                Group topLevelGroup = TopLevelGroup;
                AddNewRecordSection addNewSection = null;
                if (topLevelGroup != null)
                {
                    foreach (Section section in topLevelGroup.Sections)
                    {
                        if (section is AddNewRecordSection)
                        {
                            addNewSection = (AddNewRecordSection)section;
                            if (topLevelGroup.IsChildVisible(section))
                            {
                                _addNewRecord = (AddNewRecord)addNewSection.Records[0];
                                return _addNewRecord;
                            }
                        }
                    }

                    if (addNewSection != null)
                    {
                        _addNewRecord = (AddNewRecord)addNewSection.Records[0];
                        return _addNewRecord;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Determines if table has an active current element that is a <see cref="Record"/> (and not a <see cref="NestedTable"/>).
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool HasCurrentRecord
        {
            get
            {
                return currentRecordManager != null && CurrentRecordManager.HasCurrentRecord;
            }
        }

        /// <summary>
        /// Gets / sets the current record. When current element is not a <see cref="Record"/>, NULL is returned, (e.g. if
        /// element is a <see cref="NestedTable"/>).
        /// Setting the current record will trigger a <see cref="Syncfusion.Grouping.CurrentRecordManager.Navigate"/> call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Record CurrentRecord
        {
            get
            {
                return CurrentRecordManager.CurrentRecord;
            }

            set
            {
                CurrentRecordManager.CurrentRecord = value;
            }
        }

        /// <summary>
        /// Deativates the current record, saves pending changes.
        /// </summary>
        public void ResetCurrentRecord()
        {
            if (HasCurrentRecord)
            {
                CurrentRecord = null;
            }
        }

        /// <summary>
        /// Gets / sets the current element. Setting the current element will trigger a <see cref="Syncfusion.Grouping.CurrentRecordManager.Navigate"/> call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Element CurrentElement
        {
            get
            {
                return CurrentRecordManager.CurrentElement;
            }

            set
            {
                CurrentRecordManager.CurrentElement = value;
            }
        }

        /// <summary>
        /// Determines if table has an active current element (either a <see cref="Record"/> or <see cref="NestedTable"/>).
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool HasCurrentElement
        {
            get
            {
                return CurrentRecordManager.HasCurrentElement;
            }
        }

        /// <summary>
        /// Returns the current element of this table. If it is a NestedTable element, it will return the current element of the related table.
        /// </summary>
        /// <remarks>
        /// This method only returns the current element, it does not change the <see cref="FilteredChildTable"/> property.
        /// </remarks>
        /// <returns>The current element</returns>
        public Element GetInnerMostCurrentElement()
        {
            Element el = this.CurrentElement;
            while (el is NestedTable)
            {
                NestedTable nt = (NestedTable)el;
                if (nt.ParentTable == null)
                {
                    return null;
                }

                ChildTable ct = nt.ChildTable;
                if (ct != null)
                {
                    Table relatedTable = ct.ParentTable;
                    if (relatedTable != null && relatedTable.HasCurrentElement)
                    {
                        el = relatedTable.CurrentElement;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return el;
        }

        /// <summary>
        /// Determines if the specified row index is child row of the current record.
        /// </summary>
        /// <param name="displayElementIndex">The row index to be tested.</param>
        /// <returns>True if the row belongs to the current record record; False otherwise.</returns>
        public bool IsCurrentRecord(int displayElementIndex)
        {
            return IsSameRecord(this.CurrentRecord, displayElementIndex);
        }

        /// <summary>
        /// Navigates the record up or down, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="step">The number of records to move up. Positive step will move the record up, negative steps will move the record down.</param>
        /// <returns>The current record after navigation.</returns>
        public Record NavigateCurrentRecordUp(int step)
        {
            return CurrentRecordManager.Navigate(-step);
        }

        /// <summary>
        /// Navigates the record up or down, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="step">The number of records to advance. Positive steps will move the record down, negative steps will move the record up.</param>
        /// <returns>The current record after navigation.</returns>
        public Record NavigateCurrentRecordDown(int step)
        {
            return CurrentRecordManager.Navigate(step);
        }

        #endregion
        #region Expand Groups and Record
        /// <summary>
        /// Expands the top-level group and all nested groups.
        /// </summary>
        public void ExpandAllGroups()
        {
            Element current = this.CurrentElement;
            this.CurrentRecordManager.Reset();
            this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, true, false, false);
            TopLevelGroup.ExpandAllGroups(false);
            TopLevelGroup.SetExpanded(true, true, false);
            this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, false, true);
            if (current != null)
            {
                this.CurrentElement = current;
            }
        }

        /// <summary>
        /// Expands all records in the top-level group and all nested groups.
        /// </summary>
        public void ExpandAllRecords()
        {
            ////Element current = this.CurrentElement;
            ////this.CurrentRecordManager.Reset();
            this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, true, false, false);
            TopLevelGroup.ExpandAllRecords(false);
            TopLevelGroup.SetExpanded(true, true, false);
            this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, false, true);
            ////if (current != null)
            ////    this.CurrentElement = current;
        }

        /// <summary>
        /// Collapses the top-level group and all nested groups.
        /// </summary>
        public void CollapseAllGroups()
        {
            ////this.CurrentRecordManager.Reset();
            this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, true, false, false);
            TopLevelGroup.CollapseAllGroups(false);
            TopLevelGroup.SetExpanded(false, true, false);
            this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, false, true);
        }

        /// <summary>
        /// Collapses all records in the top-level group and all nested groups.
        /// </summary>
        public void CollapseAllRecords()
        {
            CollapseAllRecords(true);
        }

        internal void CollapseAllRecords(bool raiseDisplayElementChanged)
        {
            if (raiseDisplayElementChanged)
            {
                ////this.CurrentRecordManager.Reset();
                this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, true, true, false, false);
            }

            bool hasUniformChildList = false;
            foreach (Table relatedTable in RelatedTables)
            {
                RelationDescriptor rd = relatedTable.TableDescriptor.ParentRelation;
                if (rd.RelationKind == RelationKind.RelatedMasterDetails)
                {
                    relatedTable.CollapseAllRecords(false);
                    relatedTable.TopLevelGroup.CollapseAllGroups(false);
                }

                if (rd.RelationKind == RelationKind.UniformChildList)
                {
                    hasUniformChildList = true;
                }
            }

            foreach (Record r in UnsortedRecords)
            {
                r.SetExpanded(false, false, false);

                // In version 4.2 we do not create Tables any more for each nested table.
                // Instead the ChildTable.SourceList will point to the nested collection
                // and listen to ListChanged events. 
                if (Engine.UseOldUniformChildListRelation && hasUniformChildList)
                {
                    foreach (NestedTable nt in r.NestedTables)
                    {
                        RelationDescriptor rd = nt.ChildTable.ParentTableDescriptor.ParentRelation;
                        if (rd.RelationKind == RelationKind.UniformChildList)
                        {
                            Table relatedTable = nt.ChildTable.ParentTable;
                            relatedTable.CollapseAllRecords(false);
                            relatedTable.TopLevelGroup.CollapseAllGroups(false);
                            relatedTable.TopLevelGroup.SetExpanded(false, false, false);
                        }
                    }
                }
            }

            if (raiseDisplayElementChanged)
            {
                CountersDirty = true;

                this.EngineTable.RaiseDisplayElementChanged(this, -1, -1, true, true, false, true);
            }
        }

        /// <summary>
        /// Expands all parent groups and grandparent groups of the element until it becomes shown
        /// in the DisplayElements collection.
        /// </summary>
        /// <param name="r">The element to be shown.</param>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void ShowRecord(Element r, bool raiseDisplayElementChangeEvents)
        {
            if (/*r.ParentElement.IsChildVisible(r) &&*/ !r.GetVisibleInHierarchy())
            {
                if (r is Record)
                {
                    ShowRecord((Record)r, raiseDisplayElementChangeEvents);
                }
                else if (r is NestedTable)
                {
                    NestedTable nt = (NestedTable)r;
                    if (nt.ChildTable != null)
                    {
                        // Expand the child table
                        nt.ChildTable.SetExpanded(true, true, raiseDisplayElementChangeEvents);
                    }
                    // and record
                    nt.ParentRecord.SetExpanded(true, true, raiseDisplayElementChangeEvents);
                    // and the parent groups the parent record belongs to
                    ShowRecord(nt.ParentRecord, raiseDisplayElementChangeEvents);
                }
            }
        }

        /// <summary>
        /// Expands all parent groups and grandparent groups of the element until it becomes shown
        /// in the DisplayElements collection.
        /// </summary>
        /// <param name="r">The element to be shown.</param>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void ShowRecord(Record r, bool raiseDisplayElementChangeEvents)
        {
            Group g = r.ParentGroup;
            while (g != null && !r.GetVisibleInHierarchy())
            {
                g.SetExpanded(true, true, raiseDisplayElementChangeEvents);
                if (g is ChildTable)
                {
                    NestedTable nt = ((ChildTable)g).ParentNestedTable;
                    if (nt == null)
                    {
                        return;
                    }

                    r = nt.ParentRecord;
                    r.SetExpanded(true, true, raiseDisplayElementChangeEvents);
                    ShowRecord(r, raiseDisplayElementChangeEvents);
                    break;
                }

                g = g.ParentGroup;
            }
        }

        #endregion
        #region Collections
        /// <summary>
        /// Provides a flat collection of visible elements in the table. All records, groups,
        /// and sections are only returned by this collection if they are expanded
        /// and meet filter criteria. The collection steps into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DisplayElementsInTableCollection NestedDisplayElements
        {
            get
            {
                if (_nestedDisplayElements == null)
                {
                    _nestedDisplayElements = new DisplayElementsInTableCollection(this, true);
                }

                return _nestedDisplayElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of visible elements in the table. All records, groups,
        /// and sections are only returned by this collection if they are expanded
        /// and meet filter criteria. The collection does not step into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DisplayElementsInTableCollection DisplayElements
        {
            get
            {
                if (_displayElements == null)
                {
                    _displayElements = new DisplayElementsInTableCollection(this);
                }

                return _displayElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of all elements in the table. All records, groups,
        /// and sections are returned by this collection no matter if they were expanded
        /// or meet filter criteria. The collection steps into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ElementsInTableCollection NestedElements
        {
            get
            {
                if (_nestedElements == null)
                {
                    _nestedElements = new ElementsInTableCollection(this, true);
                }

                return _nestedElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of all elements in the table. All records, groups,
        /// and sections are returned by this collection no matter if they were expanded
        /// or meet filter criteria. The collection does not step into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ElementsInTableCollection Elements
        {
            get
            {
                if (_elements == null)
                {
                    _elements = new ElementsInTableCollection(this);
                }

                return _elements;
            }
        }

        /// <summary>
        /// A Read-only collection of sorted <see cref="Record"/> elements that meet filter criteria and are children of a <see cref="Table"/>.
        /// See <see cref="RecordFilterDescriptorCollection"/> or <see cref="Syncfusion.Grouping.TableDescriptor.RecordFilters"/> for filter criteria.
        /// An instance of this collection is returned by the <see cref="Table.FilteredRecords"/> property
        /// of a <see cref="Table"/> object. <para/>
        /// The collection
        /// provides support for determining a record's position in the grouped table using the <see cref="RecordsInTableCollectionBase.IndexOf"/>
        /// method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FilteredRecordsInTableCollection FilteredRecords
        {
            get
            {
                if (_filteredRecords == null)
                {
                    _filteredRecords = new FilteredRecordsInTableCollection(this);
                }

                return _filteredRecords;
            }
        }

        /// <summary>
        /// A Read-only collection of sorted <see cref="Record"/> elements that are children of a <see cref="Table"/>.
        /// An instance of this collection is returned by the <see cref="Table.Records"/> property
        /// of a <see cref="Table"/> object. This collection contains all records, it is not filtered. <para/>
        /// The collection
        /// provides support for determining a record's position in the grouped table using the <see cref="RecordsInTableCollectionBase.IndexOf"/>
        /// method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RecordsInTableCollection Records
        {
            get
            {
                if (_sortedRecords == null)
                {
                    _sortedRecords = new RecordsInTableCollection(this);
                }

                return _sortedRecords;
            }
        }

        /// <summary>
        /// A collection of selected <see cref="Record"/> elements that are children of a <see cref="Table"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SelectedRecordsCollection SelectedRecords
        {
            get
            {
                if (_selectedRecords == null)
                {
                    _selectedRecords = new SelectedRecordsCollection(this);
                }

                return _selectedRecords;
            }
        }

        SelectedRecordsCollection _selectedRecords;

        /// <summary>
        /// Occurs before the <see cref="Table.SelectedRecords"/> collection is modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanging;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanging != null)
            {
                SelectedRecordsChanging(this, e);
            }
        }

        internal void RaiseSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectedRecordsChanging(e);
            }

            OnSelectedRecordsChanging(e);
        }

        /// <summary>
        /// Occurs after the <see cref="Table.SelectedRecords"/> collection was modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanged;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanged != null)
            {
                SelectedRecordsChanged(this, e);
            }
        }

        internal void RaiseSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectedRecordsChanged(e);
            }

            OnSelectedRecordsChanged(e);
        }

        /// <summary>
        /// A collection of unsorted <see cref="Record"/> elements that are children of a <see cref="Table"/> and
        /// represent the original records in the same order as the underlying data source. The collection
        /// provides support for determining a record's underlying position in the datasource using the <see cref="UnsortedRecordsCollection.IndexOf"/>
        /// method.<para/>
        /// An instance of this collection is returned by the <see cref="Table.UnsortedRecords"/> property
        /// of a <see cref="Table"/> object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public UnsortedRecordsCollection UnsortedRecords
        {
            get
            {
                return _unsortedRecords;
            }
        }

        /// <summary>
        /// A collection of "sorted by PrimaryKey" <see cref="Record"/> elements that are children of a <see cref="Table"/> and
        /// represent the original records in the same order as the underlying data source. The collection
        /// provides support for determining a record's underlying position in the datasource using the <see cref="PrimaryKeySortedRecordsCollection.IndexOf"/>
        /// method.<para/>
        /// An instance of this collection is returned by the <see cref="Table.PrimaryKeySortedRecords"/> property
        /// of a <see cref="Table"/> object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public PrimaryKeySortedRecordsCollection PrimaryKeySortedRecords
        {
            get
            {
                if (_primaryKeySortedRecords == null)
                {
                    if (primaryKeySortedRecordsTree == null)
                    {
                        primaryKeySortedRecordsTree = new PrimaryKeySortedRecordsTree(this);
                    }

                    _primaryKeySortedRecords = new PrimaryKeySortedRecordsCollection(primaryKeySortedRecordsTree, this);
                }

                return _primaryKeySortedRecords;
            }
        }
        #endregion
        #region TopLevelGroup
        /// <summary>
        /// Gets the main top-level group for the table. The <see cref="Table.TopLevelGroup"/> is a <see cref="ChildTable"/>
        /// which is derived from <see cref="Group"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ChildTable TopLevelGroup
        {
            get
            {
                EnsureInitialized(this);
                return _topLevelGroup;
            }
        }

        internal void SetTopLevelGroup(ChildTable value)
        {
            if (this._topLevelGroup != null)
            {
                this._topLevelGroup.Dispose();
            }

            _topLevelGroup = value;
            this.groupCategoryTable.Clear();
            GroupCategoryTreeTableEntry entry = new GroupCategoryTreeTableEntry();
            entry.Element = value;
            entry.Tree = this.TreeEntries.TreeTable;
            _topLevelGroup.GroupCategoryEntry = entry;
            this.groupCategoryTable.Add(entry);
            // Console.WriteLine(_topLevelGroup.Sections.Count);
        }

        #endregion
        #region DefaultYAmountChanged Event

        /// <summary>
        /// Occurs when the <see cref="RaiseDefaultYAmountChanged"/> is called.
        /// </summary>
        [Browsable(false)]
        public event EventHandler DefaultYAmountChanged;

        /// <summary>
        /// Raises the <see cref="DefaultYAmountChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDefaultYAmountChanged(EventArgs e)
        {
            if (DefaultYAmountChanged != null)
            {
                DefaultYAmountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DefaultYAmountChanged"/> event.
        /// </summary>
        public void RaiseDefaultYAmountChanged()
        {
            InvalidateCounterTopDown(true);
            OnDefaultYAmountChanged(EventArgs.Empty);
        }

        #endregion
        #region Default Metrics

        /// <summary>
        /// Gets / sets the default height (YAmount) of summary rows.
        /// </summary>
        public virtual int DefaultSummaryRowHeight
        {
            get
            {
                return defaultSummaryRowHeight == -1 ? DefaultRecordRowHeight : defaultSummaryRowHeight;
            }

            set
            {
                if (defaultSummaryRowHeight != value)
                {
                    defaultSummaryRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of record rows.
        /// </summary>
        public virtual int DefaultRecordRowHeight
        {
            get
            {
                return defaultRecordRowHeight;
            }

            set
            {
                if (defaultRecordRowHeight != value)
                {
                    defaultRecordRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of record preview rows.
        /// </summary>
        public virtual int DefaultRecordPreviewRowHeight
        {
            get
            {
                return defaultRecordPreviewRowHeight;
            }

            set
            {
                if (defaultRecordPreviewRowHeight != value)
                {
                    defaultRecordPreviewRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of group caption rows.
        /// </summary>
        public virtual int DefaultCaptionRowHeight
        {
            get
            {
                return defaultCaptionRowHeight;
            }

            set
            {
                if (defaultCaptionRowHeight != value)
                {
                    defaultCaptionRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of column header rows.
        /// </summary>
        public virtual int DefaultColumnHeaderRowHeight
        {
            get
            {
                return defaultColumnHeaderRowHeight;
            }

            set
            {
                if (defaultColumnHeaderRowHeight != value)
                {
                    defaultColumnHeaderRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of empty section rows.
        /// </summary>
        public virtual int DefaultEmptySectionHeight
        {
            get
            {
                return defaultEmptySectionHeight;
            }

            set
            {
                if (defaultEmptySectionHeight != value)
                {
                    defaultEmptySectionHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of group header sections.
        /// </summary>
        public virtual int DefaultGroupHeaderSectionHeight
        {
            get
            {
                return defaultGroupHeaderSectionHeight;
            }

            set
            {
                if (defaultGroupHeaderSectionHeight != value)
                {
                    defaultGroupHeaderSectionHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of group footer sections.
        /// </summary>
        public virtual int DefaultGroupFooterSectionHeight
        {
            get
            {
                return defaultGroupFooterSectionHeight;
            }

            set
            {
                if (defaultGroupFooterSectionHeight != value)
                {
                    defaultGroupFooterSectionHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of group preview rows.
        /// </summary>
        public virtual int DefaultGroupPreviewSectionHeight
        {
            get
            {
                return defaultGroupPreviewSectionHeight;
            }

            set
            {
                if (defaultGroupPreviewSectionHeight != value)
                {
                    defaultGroupPreviewSectionHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default height (YAmount) of filter bar rows.
        /// </summary>
        public virtual int DefaultFilterBarRowHeight
        {
            get
            {
                return defaultFilterBarRowHeight == -1 ? DefaultRecordRowHeight : defaultFilterBarRowHeight;
            }

            set
            {
                if (defaultFilterBarRowHeight != value)
                {
                    defaultFilterBarRowHeight = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default width of group indents.
        /// </summary>
        public virtual int DefaultIndentWidth
        {
            get
            {
                return this.defaultIndentWidth;
            }

            set
            {
                if (this.defaultIndentWidth != value)
                {
                    this.defaultIndentWidth = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default width of row headers.
        /// </summary>
        public virtual int DefaultRowHeaderWidth
        {
            get
            {
                return this.defaultRowHeaderWidth;
            }

            set
            {
                if (this.defaultRowHeaderWidth != value)
                {
                    this.defaultRowHeaderWidth = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        /// <summary>
        /// Gets / sets the default width of table indents.
        /// </summary>
        public virtual int DefaultTableIndentWidth
        {
            get
            {
                return this.defaultTableIndentWidth;
            }

            set
            {
                if (this.defaultTableIndentWidth != value)
                {
                    this.defaultTableIndentWidth = value;
                    RaiseDefaultYAmountChanged();
                }
            }
        }

        #endregion
        #region Dirty Markers
        /// <summary>
        /// Gets / sets a value indicating if the table is marked as dirty. If a table is marked dirty, any subsequent
        /// access to child elements (and a resulting <see cref="Syncfusion.Grouping.Element.EnsureInitialized"/> call) will trigger
        /// recategorization of all records in the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool TableDirty
        {
            get
            {
                return isDirty;
            }

            set
            {
                if (value && !isDirty)
                {
                    if (SettingTableDirty != null)
                    {
                        SettingTableDirty(this, EventArgs.Empty);
                    }

                    ////                    TraceUtil.TraceCurrentMethodInfo(value);
                    ////                    TraceUtil.TraceCalledFrom(10);
                }

                isDirty = value;
            }
        }

        /// <summary>
        /// A debug helper to be able to see call stack when table is set dirty.
        /// </summary>
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public event EventHandler SettingTableDirty;

        /// <summary>
        /// Gets / sets a value indicating if the counters are marked as dirty. If a table has counters marked dirty,
        /// any subsequent access to child elements (and a resulting <see cref="Syncfusion.Grouping.Element.EnsureInitialized"/> call) will trigger
        /// a <see cref="InvalidateCounterTopDown"/> call and force reevaluation of all counters for all elements in the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CountersDirty
        {
            get
            {
                return _isRecordFiltersDirty;
            }

            set
            {
                ////                if (value && !_isRecordFiltersDirty)
                ////                {
                ////                    TraceUtil.TraceCurrentMethodInfo(value);
                ////TraceUtil.TraceCalledFrom(10);
                ////                }
                _isRecordFiltersDirty = value;
                /*if (value)
                {
                    Table t = RelationParentTable;
                    if (t != null)
                        t.CountersDirty = value;
                }*/
            }
        }

        /// <summary>
        /// Gets / sets a value indicating if summaries are marked as dirty. If a table has summaries marked dirty,
        /// any subsequent access to child elements (and a resulting <see cref="Syncfusion.Grouping.Element.EnsureInitialized"/> call) will trigger
        /// a <see cref="InvalidateSummariesTopDown"/> call and force reevaluation of all summaries for all elements in the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SummariesDirty
        {
            get
            {
                return _isSummaryDirty;
            }

            set
            {
                ////                if (value && !_isSummaryDirty)
                ////                {
                ////                    TraceUtil.TraceCurrentMethodInfo(value);
                ////                    TraceUtil.TraceCalledFrom(10);
                ////                }
                _isSummaryDirty = value;
            }
        }
        #endregion
        #region SourceListVersion
        /// <summary>
        /// The source list version. The version is increased each time a change in a record in the datasource
        /// is detected.
        /// </summary>
        public int SourceListVersion
        {
            get
            {
                return this._sourceListVersion;
            }

            set
            {
                if (this._sourceListVersion != value)
                {
                    this.sourceListSortArray = null;
                    this._sourceListVersion = value;
                }
            }
        }

        #endregion
        #region SourceList
        /// <summary>
        /// Determines if the source list allows adding new records.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SourceListAllowNew
        {
            get
            {
                return this.IsNewUniformChildListRelation()
                || (this.sourceList != null && !this.sourceList.IsFixedSize)
                    || (this.bindingList != null && this.bindingList.AllowNew);
            }
        }

        /// <summary>
        /// Determines if the source list allows removing records.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SourceListAllowRemove
        {
            get
            {
                return this.IsNewUniformChildListRelation()
                || (this.sourceList != null && !this.sourceList.IsFixedSize)
                    || (this.bindingList != null && this.bindingList.AllowRemove);
            }
        }

        /// <summary>
        /// Determines if the source list allows editing records.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SourceListAllowEdit
        {
            get
            {
                return this.IsNewUniformChildListRelation()
                || (this.sourceList != null && !this.sourceList.IsReadOnly)
                    || (this.bindingList != null && this.bindingList.AllowEdit);
            }
        }

        /// <summary>
        /// Determines if a source has been attached to the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool HasSourceList
        {
            get
            {
                return sourceList != null;
            }
        }

        object bindingSourceList = null;

        /// <summary>
        /// Gets / sets a reference to the source list.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IList SourceList
        {
            get
            {
                return sourceList;
            }

            set
            {
                if (sourceList != value)
                {
                    if (Engine.HelpTracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo(this);
                        TraceUtil.TraceCalledFrom(20);
                    }

                    inSetSourceList = true;
                    UnwireList();

                    sourceListSortArray = null;
                    if (this.CurrentRecordManager.HasCurrentRecord)
                    {
                        CurrentRecordManager.Reinititalize();
                    }

                    if (this.sourceList != null)
                    {
                        this.filteredChildTable = null;
                        this.currentRecordManager.Reset();
                    }

                    sourceList = value;
#if SyncfusionFramework2_0
                    if (value is System.Windows.Forms.BindingSource)
                    {
                        bindingSourceList = ((System.Windows.Forms.BindingSource)value).List;
                    }
#endif
                    SourceListVersion++;
                    this.filteredChildTable = null;
                    this.currentRecordManager.Reset();
                    bindingList = sourceList as IBindingList;
                    WireList();

                    if (TableDescriptor.GetList() == null)
                    {
                        TableDescriptor.SetItemProperties(sourceList);
                    }

                    if (_selectedRecords != null)
                    {
                        _selectedRecords._inner.Clear();
                    }

                    unsortedRecordsTree.Clear();
                    isDirty = true;

                    cacheRecordData = this.ShouldCacheRecordData();

                    inSetSourceList = false;
                    try
                    {
                        inSourceListChanged = true;
                        OnSourceListChanged(new TableEventArgs(this));

                        if (this.relatedTables != null && this.relatedTables.Count > 0)
                        {
                            Table[] tables = new Table[this.relatedTables.Count];
                            relatedTables.CopyTo(tables, 0);
                            foreach (Table table in tables)
                            {
                                table.Disposed -= new EventHandler(relatedTable_Disposed);
                                this.OnRemovingRelatedTable(new TableEventArgs(table));
                                table.Dispose();
                            }
                        }

                        foreach (RelationDescriptor rd in Relations._inner)
                        {
                            rd.Dispose();
                        }

                        Relations.Clear();
                        relatedTables.Clear();
                        RelatedTables.relationDescriptorVersion = -1;
                        RelatedTables.tableSourceListVersion = -1;
                    }
                    finally
                    {
                        inSourceListChanged = false;
                    }
                }
            }
        }

        /// <summary>
        /// Returns True when SourceList setter was called and False after SourceList setter returns.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetSourceList
        {
            get
            {
                return inSetSourceList;
            }
        }

        /// <summary>
        /// Returns True when the SourceListChanged event is raised and False after it returns.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSourceListChanged
        {
            get
            {
                return inSourceListChanged;
            }
        }

        /// <summary>
        /// Method is called when the table tries to determine whether a record's underlying data row can be cached
        /// within <see cref="Record"/> objects.
        /// </summary>
        /// <returns>True if record's underlying data row can be cached
        /// within <see cref="Record"/> objects; False otherwise.</returns>
        protected virtual bool ShouldCacheRecordData() // called when SourceList is changed
        {
            return ((SourceList is IGroupingList) && ((IGroupingList)SourceList).AllowItemReference) || SourceList is System.Data.DataView;
        }

        internal bool cacheRecordData = false;

        internal object GetSourceListItem(int n)
        {
            if (isPassThroughGrouping)
            {
                throw new InvalidOperationException("Does not work for Passthrough Grouping");
            }
#if MEASURE
            using (MeasureTime.Measure("GetSourceListItem.sourceList.Count"))
#endif
            {
                if (sourceList == null || n >= sourceList.Count || isPassThroughGrouping)
                {
                    return null;
                }
            }
#if MEASURE
            using (MeasureTime.Measure("GetSourceListItem.sourceList[n]"))
#endif
            {
                return sourceList[n];
            }
        }

        ////        protected virtual IList GetSourceListBase()
        ////        {
        ////            return null;
        ////        }
        #endregion
        #region SourceListChanged Event
        /// <summary>
        /// Occurs after the datasource is replaced.
        /// </summary>
        [Browsable(false)]
        public event TableEventHandler SourceListChanged;

        /// <exclude/>
        /// <summary>
        /// Raises the <see cref="SourceListChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected void BaseSourceListChanged(TableEventArgs e)
        {
            this.CurrentRecordManager.Reset();
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnTableSourceListChanged(e);
            }

            if (SourceListChanged != null)
            {
                SourceListChanged(this, e);
            }
        }

        #endregion
        #region RecordValueChange Event
        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
        /// </summary>
        public event RecordValueChangingEventHandler RecordValueChanging;

        /// <summary>
        /// Raises the <see cref="RecordValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordValueChanging(e);
            }
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.ControlBaseEvents.TraceVerbose, Name, e);
            if (RecordValueChanging != null)
            {
                RecordValueChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangingEventArgs" /> that contains the event data.</param>
        public void RaiseRecordValueChanging(RecordValueChangingEventArgs e)
        {
            OnRecordValueChanging(e);
        }

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returns.
        /// </summary>
        public event RecordValueChangedEventHandler RecordValueChanged;

        /// <summary>
        /// Raises the <see cref="RecordValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnRecordValueChanged(e);
            }
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.ControlBaseEvents.TraceVerbose, Name, e);
            if (RecordValueChanged != null)
            {
                RecordValueChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RecordValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangedEventArgs" /> that contains the event data.</param>
        public void RaiseRecordValueChanged(RecordValueChangedEventArgs e)
        {
            OnRecordValueChanged(e);
        }
        #endregion
        #region WireList
        /// <summary>
        /// Is called to unwire events from source list before source list is replaced.
        /// </summary>
        protected virtual void UnwireList()
        {
            ////TraceUtil.TraceCurrentMethodInfo(Info);
            if (this.bindingList != null)
            {
                this.bindingList.ListChanged -= new ListChangedEventHandler(onListChanged);
            }

            if (this.SourceList is IListChangedSource)
            {
                ((IListChangedSource)SourceList).ListChanged -= new ListChangedEventHandler(onListChanged);
            }
        }

        bool bindingList_IsSorted = false;
        PropertyDescriptor bindingList_SortProperty = null;
        ListSortDirection bindingList_SortDirection = ListSortDirection.Ascending;
        string dataViewSort = string.Empty;
        string dataViewRowFilter = string.Empty;

#if SyncfusionFramework2_0
        ListSortDescriptionCollection bindingListViewSortDescriptions = new ListSortDescriptionCollection();
        string bindingListViewFilter = string.Empty;
#endif

        /// <summary>
        /// Detect and save changes in sort or filter criteria of underlying datasource. 
        /// </summary>
        /// <param name="dataSource">The datasource</param>
        /// <returns>true if changes were detected; false otherwise.</returns>
        bool CompareAndCacheSortDescription(object dataSource)
        {
            bool changed = false;

            IBindingList bindingList = dataSource as IBindingList;
            if (bindingList != null && bindingList.SupportsSorting)
            {
                changed =
                    bindingList_IsSorted != bindingList.IsSorted
                    || bindingList_SortProperty != bindingList.SortProperty
                    || bindingList_SortDirection != bindingList.SortDirection;

                bindingList_IsSorted = bindingList.IsSorted;
                bindingList_SortProperty = bindingList.SortProperty;
                bindingList_SortDirection = bindingList.SortDirection;
            }

#if SyncfusionFramework2_0
            IBindingListView bindingListView = dataSource as IBindingListView;
            if (bindingListView != null)
            {
                changed |= bindingListViewFilter != bindingListView.Filter;
                bindingListViewFilter = bindingListView.Filter;

                if (bindingListView.IsSorted)
                {
                    changed |= bindingListViewSortDescriptions.Count != bindingListView.SortDescriptions.Count;
                    if (!changed)
                    {
                        for (int n = 0; !changed && n < bindingListViewSortDescriptions.Count; n++)
                        {
                            changed |=
                                 bindingListViewSortDescriptions[n].PropertyDescriptor != bindingListView.SortDescriptions[n].PropertyDescriptor
                                 || bindingListViewSortDescriptions[n].SortDirection != bindingListView.SortDescriptions[n].SortDirection;
                        }
                    }

                    ListSortDescription[] sorts = new ListSortDescription[bindingListView.SortDescriptions.Count];
                    bindingListView.SortDescriptions.CopyTo(sorts, 0);
                    bindingListViewSortDescriptions = new ListSortDescriptionCollection(sorts);
                }
            }
#endif

            DataView dv = dataSource as DataView;
            if (dv != null)
            {
                changed |=
                    dataViewSort != dv.Sort
                    || dataViewRowFilter != dv.RowFilter;
                dataViewSort = dv.Sort;
                dataViewRowFilter = dv.RowFilter;
            }

            return changed;
        }

        /// <summary>
        /// Is called to wire events to source list after source list is attached.
        /// </summary>
        protected virtual void WireList()
        {
            CompareAndCacheSortDescription(SourceList);

            ////TraceUtil.TraceCurrentMethodInfo(Info);
            if (this.bindingList != null)
            {
                this.bindingList.ListChanged += new ListChangedEventHandler(onListChanged);
            }
            else if (this.SourceList is IListChangedSource && ((IListChangedSource)SourceList).SupportsListChanged)
            {
                ((IListChangedSource)SourceList).ListChanged += new ListChangedEventHandler(onListChanged);
            }
        }

        #endregion
        #region Synchronize Table with ListChanged event
        /// <summary>
        /// Gets a reference to the most recently changed record.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Record LastChangedRecord
        {
            get
            {
                return lastChangedRecord;
            }

            set
            {
                lastChangedRecord = value;
            }
        }


        /// <exclude/>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int LastAddNewIndex
        {
            get { return lastAddNewIndex; }
        }

        /// <summary>
        /// Used by ChildTable in UniformChildList relations to forward IBindingList.ListChanged event from a nested collection.
        /// </summary>
        /// <param name="sourceList">The source list.</param>       
        /// <param name="ct">The group.</param>
        /// <param name="e">The event data.</param>
        public void SimulateListChangedWithSourceList(IList sourceList, Group ct, ListChangedEventArgs e)
        {
            if (this.sourceList == null)
            {
                this.sourceList = sourceList;
                onListChanged(ct, e);
                this.sourceList = null;
            }
        }

        /// <summary>
        /// Fakes an IBindingList.ListChanged event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SimulateListChanged(ListChangedEventArgs e)
        {
            onListChanged(this, e);
        }

        internal void SimulateListChanged(Record record)
        {
            int index = UnsortedRecords.IndexOf(record);
            ListChangedType action = ListChangedType.ItemChanged;
            if (index == -1)
            {
                action = ListChangedType.ItemAdded;
                index = UnsortedRecords.Count - 1;
            }

            onListChanged(this, new ListChangedEventArgs(action, index, index));
        }

        bool inSourceListListChanged = false;

        /// <summary>
        /// Returns True while handling.
        /// </summary>
        public bool InSourceListListChangedHandler
        {
            get
            {
                return inSourceListListChanged;
            }
        }

        internal bool inItemDeleted = false;

        internal void onListChanged(object sender, ListChangedEventArgs e)
        {
            wasItemChanged = true;
            bindingList_ListChanged(sender, e);
        }

        /// <summary>
        /// Handles the IBindingList.ListChanged event of the source list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (!Engine.UseOldListChangedHandler)
            {
                NewBindingList_ListChanged(sender, e);
                return;
            }

            // Do not call UnsortedRecords.Count when TableDirty = true. Otherwise it will trigger
            // CategorizeElements and with CategorizeElements that one record that is modified here
            // has already been changed.
#if MEASURE
            using (MeasureTime.Measure("bindingList_ListChanged"))
#endif
            {
                inSourceListListChanged = true;

                lastChangedRecord = null;

                int savedCategorizeElementsVersion = this.categorizeElementsVersion;

                Group senderChildTable = sender as Group;
                int newIndex = e.NewIndex;
                int oldIndex = e.OldIndex;
                if (senderChildTable != null && senderChildTable.UnsortedEntry != null)
                {
                    if (newIndex != -1)
                    {
                        newIndex += ((Group)sender).UnsortedEntry.GetPosition();
                    }

                    if (oldIndex != -1)
                    {
                        oldIndex += ((Group)sender).UnsortedEntry.GetPosition();
                    }
                }

                TableListChangedEventArgs te = TableListChangedEventArgs.Create(this, e.ListChangedType, e.NewIndex, e.OldIndex, e.PropertyDescriptor);

                try
                {
                    if (this.ParentTable != null)
                    {
                        this.ParentTable.OnRelatedTableSourceListListChanged(te);
                    }

#if MEASURE
                    using (MeasureTime.Measure("bindingList_ListChanged.OnSourceListListChanged"))
#endif
                    {
                        OnSourceListListChanged(te);
                    }

                    if (savedCategorizeElementsVersion != this.categorizeElementsVersion)
                    {
                        oldCount = sourceList.Count;
                        return;  // nothing further to do any more - the whole table has been recategorized with new data
                    }

                    ////TraceUtil.TraceCurrentMethodInfo(Switches.GroupingEngine.TraceVerbose, e.ListChangedType, e.NewIndex, e.OldIndex, inInitialize);

                    ////EngineVersion++;
#if DEBUG
                    if (Switches.GroupingEngine.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(e.ListChangedType, e.NewIndex, e.OldIndex, inInitialize);
                    }
#else
                ;
#endif
                    ////Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, CurrentRecordManager.ToString());

                    UnsortedRecordsTreeEntry unsortedEntry;
                    PrimaryKeySortedRecordsTreeEntry primaryKeyEntry;
                    SortedRecordsTreeTableEntry sortedEntry;
                    Record record;
                    Group group;

                    switch (e.ListChangedType)
                    {
                        case ListChangedType.PropertyDescriptorAdded:
                        case ListChangedType.PropertyDescriptorChanged:
                        case ListChangedType.PropertyDescriptorDeleted:
                            {
                                SourceListVersion++;
                                TableDescriptor.SetItemProperties(this.SourceList);
                                this.lastAddNewIndex = -1;
                                break;
                            }

                        case ListChangedType.Reset:
                            {
                                try
                                {
                                    CurrentRecordManager.Reset();

                                    SourceListVersion++;
                                    TableDescriptor.SetItemProperties(this.SourceList);
                                    // TODO: need to find out if in such case the whole list needs to be reinitialized
                                    if (!this.isDirty && (this.UnsortedRecords.Count != this.SourceList.Count || Engine.TableDirtyOnSourceListReset))
                                    {
                                        isDirty = true;
                                        this.SelectedRecords.Clear();
                                    }

                                    if (!te.ShouldIgnoreReset)
                                    {
                                        // But resetting counters should allways make sense, e.g. if List was sorted,
                                        // AdjustRecordRowCount should be called for all records.
                                        CountersDirty = true; // TODO: check if this collapses records ... User's don't want that to happen!
                                        this.RaiseDisplayElementChanged(this, -1, -1, true, false, false);
                                    }

                                    this.lastAddNewIndex = -1;
                                }
                                catch (Exception ex)
                                {
                                    TraceUtil.TraceExceptionCatched(ex);
                                }

                                break;
                            }

                        case ListChangedType.ItemMoved:
                            {
                                //// TestCase: sort a DataView, add or change a record. Then you'll get a ListChangedType.ItemMoved notification.
                                //// Don't bother updating sourceListSortArray - only update unsortedRecordsTree,

                                if (this.isDirty)
                                {
                                    SourceListVersion++;
                                    return; // need to reinitialize anyway ...
                                }

                                if (e.OldIndex < 0)
                                {
                                    goto case ListChangedType.ItemAdded;
                                }
                                else if (e.NewIndex < 0)
                                {
                                    goto case ListChangedType.ItemDeleted;
                                }

                                int selectedRecordsIndexOf = this.SelectedRecords.FindRecord(e.OldIndex);
                                if (selectedRecordsIndexOf != -1)
                                {
                                    this.SelectedRecords.RemoveAt(selectedRecordsIndexOf);
                                }

                                SourceListVersion++;
                                this.sourceListSortArray = null; // can be rebuilt from unsortedRecordsTree

                                record = UnsortedRecords[e.OldIndex];
                                unsortedEntry = record.UnsortedEntry;
                                ////EngineTable.OnSourceListRecordChanging(new RecordChangedEventArgs(unsortedEntry.Record, RecordChangedType.Moved, e.OldIndex, e.NewIndex));
                                ////UFD:unsortedEntry.InvalidateSummariesBottomUp(false);
                                if (record != null && record.IsCurrent)
                                {
                                    CurrentRecordManager.Reset();
                                }

                                if (VirtualMode)
                                {
                                    //// Update virtualEntryCache in SortedRecordsTreeTable
                                    this.TopLevelGroup.Details.TreeEntries.RemoveAt(e.OldIndex);
                                    ClearCollectionCaches();
                                    record.sourceIndex = e.NewIndex;
                                    ((VirtualSortedRecordsTreeTableEntry)record.SortedEntry).VPosition = e.NewIndex;
                                    this.TopLevelGroup.Details.TreeEntries.Insert(e.NewIndex, record.SortedEntry);
                                }
                                else
                                {
                                    this.unsortedRecordsTree.Remove(unsortedEntry);
                                    ClearCollectionCaches();
                                    record.sourceIndex = e.NewIndex;
                                    this.unsortedRecordsTree.Insert(e.NewIndex, unsortedEntry);
                                }

                                ////UFD:unsortedEntry.InvalidateSummariesBottomUp(false);
                                ClearCollectionCaches();
                                ////sourceListVersion++;
                                EngineTable.OnSourceListRecordChanging(new RecordChangedEventArgs(record, RecordChangedType.Moved, e.OldIndex, e.NewIndex, null));

                                if (selectedRecordsIndexOf != -1)
                                {
                                    this.SelectedRecords.Add(unsortedEntry.Record);
                                }

                                if (!VirtualMode)
                                {
                                    goto case ListChangedType.ItemChanged;
                                }

                                EngineTable.OnSourceListRecordChanged(new RecordChangedEventArgs(record, RecordChangedType.Moved, e.OldIndex, e.NewIndex, null, true, te));
                                this.lastAddNewIndex = -1;
                                break;
                            }

                        case ListChangedType.ItemChanged:
                            {
                                if (this.isDirty)
                                {
                                    SourceListVersion++;
                                    return; // need to reinitialize anyway ...
                                }

                                RecordChangedType rt = RecordChangedType.Changed;
                                if (e.ListChangedType == ListChangedType.ItemMoved)
                                {
                                    rt = RecordChangedType.Moved;
                                }

                                Debug.Assert(e.NewIndex != -1);
                                if (newIndex >= 0 && newIndex < this.UnsortedRecords.Count)
                                {
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.unsortedRecordsTree[newIndex]"))
#endif
                                    {
                                        record = UnsortedRecords[newIndex];
                                        unsortedEntry = record.UnsortedEntry;
                                    }

                                    record.filterState = -1;

                                    // New for version 4.2 UniformChildListRelation                           
                                    /*if (senderChildTable != null && senderChildTable.UnsortedEntry == null)
                                    {
                                        senderChildTable.UnsortedEntry = unsortedEntry;
                                        newIndex = UnsortedRecords.Count;
                                    }*/
                                    bool didMeetFilterCriteria = VirtualMode || record.GetSavedMeetsFilterCriteria();

                                    bool isCurrent = record.IsCurrent;

                                    if (record.SortedEntry == null || record.SortedEntry.SortedRecordsTreeTable == null)
                                    {
                                        // newIndex >= this.Records.Count
                                        // special case: user added a record in AddNew Record and then EndEdit is called.
                                        rt = RecordChangedType.Added;
                                        if (CurrentElement is AddNewRecord)
                                        {
                                            CurrentElement.InvalidateCounterBottomUp();
                                            CurrentElement.InvalidateSummariesBottomUp();
                                            if (!(sender is Group))
                                            {
                                                record.AdjustRecordRowCount();
                                            }

                                            CurrentRecordManager.ResetCurrentRecord(record);
                                        }
                                    }

#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.CurrentRecordManager.Reset"))
#endif
                                    {
                                        if (isCurrent)
                                        {
                                            CurrentRecordManager.NotifyCurrentRecordListChanged(te);
                                        }
                                    }
                                    
                                    ////oldIndex = DisplayElements.IndexOf(record);

                                    SortColumnDescriptor[] arrayOfColumnDescriptors;
                                    PropertyDescriptor[] arrayOfPropertyDescriptor;
                                    bool isSorted;
                                    if (VirtualMode)
                                    {
                                        isSorted = false;
                                        arrayOfColumnDescriptors = null;
                                        arrayOfPropertyDescriptor = null;
                                    }
                                    else
                                    {
                                        TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
                                    }

                                    SortColumnDescriptor[] arrayOfPKColumnDescriptors;
                                    PropertyDescriptor[] arrayOfPKPropertyDescriptor;
                                    bool isPKSorted;
                                    TableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);

                                    int sortedPositionChanged = -1;
                                    int primaryKeyChanged = -1;

                                    object[] oldSortKeys = record.sortKeys;
                                    object[] oldPrimaryKeys = record.primaryKeys;

#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.isSortedPositionChanged"))
#endif
                                    {
                                        if (sender is Group)
                                        {
                                            // New for version 4.2 UniformChildListRelation
                                            record.sourceIndex = newIndex;
                                            record.sourceListVersion = this.SourceListVersion;
                                            record.SetData(((Group)sender).SourceList[e.NewIndex], isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                                        }
                                        else
                                        {
                                            record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                                        }

                                        if (isSorted && rt != RecordChangedType.Added)
                                        {
                                            for (int n = 0; n < arrayOfColumnDescriptors.Length; n++)
                                            {
                                                object cx, cy;
                                                SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                                                cx = n < oldSortKeys.Length ? oldSortKeys[n] : null;
                                                cy = n < record.sortKeys.Length ? record.sortKeys[n] : null;
                                                if (SortColumnComparer._Compare(columnDescriptor, cx, cy) != 0)
                                                {
                                                    sortedPositionChanged = n;
                                                    break;
                                                }
                                            }
                                            // find out how many groups are changed
                                        }

                                        record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                                        if (isPKSorted && rt != RecordChangedType.Added)
                                        {
                                            for (int n = 0; n < arrayOfPKColumnDescriptors.Length; n++)
                                            {
                                                object cx, cy;
                                                SortColumnDescriptor columnDescriptor = arrayOfPKColumnDescriptors[n];
                                                cx = oldPrimaryKeys != null && n < oldPrimaryKeys.Length ? oldPrimaryKeys[n] : null;
                                                cy = n < record.primaryKeys.Length ? record.primaryKeys[n] : null;
                                                if (SortColumnComparer._Compare(columnDescriptor, cx, cy) != 0)
                                                {
                                                    primaryKeyChanged = n;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    primaryKeyEntry = record.PrimaryKeySortedEntry;
                                    if (primaryKeyChanged != -1 || rt == RecordChangedType.Added)
                                    {
                                        if (primaryKeyEntry != null && primaryKeyEntry.Tree != null)
                                        {
                                            primaryKeyEntry.Tree.Remove(primaryKeyEntry);
                                            primaryKeyEntry.Parent = null;
                                        }

                                        if (this.primaryKeySortedRecordsTree != null)
                                        {
                                            primaryKeyEntry = new PrimaryKeySortedRecordsTreeEntry();
                                            record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                                            record.PrimaryKeySortedEntry = primaryKeyEntry;

                                            primaryKeyEntry.Record = record;
                                            primaryKeyEntry.Tree = primaryKeySortedRecordsTree.TreeTable;
                                            primaryKeySortedRecordsTree.Add(primaryKeyEntry); // will be inserted according to sort position
                                        }
                                    }

                                    group = record.ParentGroup;
                                    sortedEntry = record.SortedEntry;
                                    Group addedGroup = null;

                                    TableDescriptor.RecordFilters.ResetCache();

                                    if (sortedPositionChanged != -1 || rt == RecordChangedType.Added || rt == RecordChangedType.Moved
                                        || te.ShouldReevaluateSortPosition)
                                    {
                                        int gindex = -1;
                                        if (isSorted)
                                        {
                                            gindex = arrayOfColumnDescriptors.Length - 1 - sortedPositionChanged - TableDescriptor.SortedColumns.Count;
                                        }
                                        ////sortedPositionChanged -= TableDescriptor.SortedColumns.Count;

                                        Group obsoleteGroup = null;
                                        if (sortedPositionChanged >= 0 || te.ShouldReevaluateSortPosition)
                                        {
                                            if (record.MeetsFilterCriteria())
                                            {
#if MEASURE
                                            using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.DetectObsoleteGroup"))
#endif
                                                {
                                                    Group g = group;

                                                    while (gindex >= 0)
                                                    {
                                                        if (g == null || g is ChildTable || g.Details.TreeEntries.Count > 1)
                                                        {
                                                            break;
                                                        }

                                                        obsoleteGroup = g;
                                                        g = g.ParentGroup;
                                                        gindex--;
                                                    }
                                                }
                                            }
#if MEASURE
                                        using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.OnSourceListRecordChanging"))
#endif
                                            {
                                                EngineTable.OnSourceListRecordChanging(new RecordChangedEventArgs(record, rt, -1, newIndex, obsoleteGroup));
                                                if (obsoleteGroup != null)
                                                {
                                                    this.OnGroupRemoving(new GroupEventArgs(obsoleteGroup));
                                                }
                                            }
                                        }

                                        ////#if MEASURE
                                        ////                                    using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.record.InvalidateCounter"))
                                        ////#endif
                                        ////                                    {
                                        ////                                        //record.InvalidateCounterTopDown(true); // force filter reevaluate
                                        ////                                        record.InvalidateCounter();
                                        ////                                        record.InvalidateCounterBottomUp();
                                        ////                                        //if (this.TableDescriptor.Summaries.Count > 0)
                                        ////                                        record.InvalidateSummariesBottomUp();
                                        ////                                    }

                                        if (group != null && (te.ShouldInvalidateGroupSortOrder || rt == RecordChangedType.Added))
                                        {
                                            GroupsDetails gd = group.ParentSection as GroupsDetails;
                                            while (gd != null)
                                            {
                                                gd.GroupSortOrderDirty = true;
                                                gd = gd.ParentSection as GroupsDetails;
                                            }
                                        }
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.sortedEntry.Tree.Remove"))
#endif
                                        {
                                            //// Remove from old group
                                            if (sortedEntry != null && sortedEntry.Tree != null)
                                            {
                                                sortedEntry.Tree.Remove(sortedEntry);
                                                sortedEntry.Parent = null;
                                                ClearCollectionCaches();
                                                ////sourceListVersion++;
                                            }
                                        }

                                        //// Remove group
                                        if (obsoleteGroup != null)
                                        {
#if MEASURE
                                        using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.remove.obsoleteGroup"))
#endif
                                            {
                                                group = obsoleteGroup.ParentGroup; //// for counter invalidation
                                                GroupCategoryTreeTableEntry gentry = obsoleteGroup.GroupCategoryEntry;
                                                if (gentry != null)
                                                {
                                                    gentry.Tree.Remove(gentry);
                                                }

                                                group.InvalidateCounterBottomUp();
                                                group.InvalidateCounterTopDown(false);
                                                group.InvalidateSummariesBottomUp();
                                                group.InvalidateSummary();
                                                group.TreeEntries.InvalidateSummariesTopDown(false);
                                                obsoleteGroup.Dispose();
                                            }
                                        }
                                        else if (group != null)
                                        {
#if MEASURE
                                        using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.group.InvalidateCounter"))
#endif
                                            {
                                                group.InvalidateCounterBottomUp();
                                                // TODO: if this is too aggresive and not enough is refreshed then call
                                                // group.InvalidateCounterTopDown(true); instead.
                                                group.InvalidateCounterTopDown(false);
                                                group.InvalidateSummariesBottomUp();
                                                group.InvalidateSummary();
                                                group.TreeEntries.InvalidateSummariesTopDown(false);
                                            }
                                        }

#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.ItemChanged.InsertSortedRecordsTreeEntry"))
#endif
                                        {
                                            // Find the right group where to insert the node.
                                            addedGroup = InsertSortedRecordsTreeEntry(sortedEntry);
                                        }

                                        ClearCollectionCaches();
                                        ////sourceListVersion++;
                                    }
                                    else 
                                    {
                                        //// only values that do not affect sort order were changed.
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.EngineTable.OnSourceListRecordChanging"))
#endif
                                        {
                                            EngineTable.OnSourceListRecordChanging(new RecordChangedEventArgs(record, rt, newIndex, newIndex));
                                        }

#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.record.InvalidateCounter"))
#endif
                                        {
                                            bool meetFilterChanged = didMeetFilterCriteria != record.MeetsFilterCriteria();

                                            ////record.InvalidateCounterTopDown(true); // force filter reevaluate
                                            if (!this.CountersDirty && (meetFilterChanged || te.ShouldInvalidateCounters))
                                            {
                                                record.InvalidateCounter();
                                                record.InvalidateCounterBottomUp();
                                            }

                                            ////if (this.TableDescriptor.Summaries.Count > 0)
                                            if (!this.SummariesDirty && te.ShouldInvalidateSummaries && this.TableDescriptor.Summaries.Count > 0)
                                            {
                                                record.InvalidateSummariesBottomUp();
                                            }
                                        }
                                    }

                                    bool groupSortOrder = false;
                                    group = record.ParentGroup;
                                    if (group != null && (te.ShouldInvalidateGroupSortOrder || rt == RecordChangedType.Added))
                                    {
                                        GroupsDetails gd = group.ParentSection as GroupsDetails;
                                        while (gd != null)
                                        {
                                            groupSortOrder |= gd.HasGroupSortOrder;
                                            gd.GroupSortOrderDirty = true;
                                            gd = gd.ParentSection as GroupsDetails;
                                        }
                                    }

                                    ////UFD:unsortedEntry.InvalidateSummariesBottomUp(false);

                                    ////newIndex = DisplayElements.IndexOf(record);
                                    lastChangedRecord = record;
                                    ////                            if (record == CurrentRecord && CurrentRecordManager.InEndEdit)
                                    ////                                CurrentRecordManager.ResetEditingInternal();

#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.OnSourceListRecordChanged"))
#endif
                                    {
                                        EngineTable.OnSourceListRecordChanged(new RecordChangedEventArgs(record, rt, newIndex, newIndex, addedGroup, sortedPositionChanged != -1 || groupSortOrder || didMeetFilterCriteria != record.MeetsFilterCriteria() || te.ShouldReevaluateSortPosition, te));
                                    }

                                    // force summaries for all parent groups to be refreshed
                                    if (immediateUpdateSummaries)
                                    {
                                        GetSummaries(this);
                                    }

                                    this.lastAddNewIndex = -1;
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.CurrentRecord = record"))
#endif
                                    {
                                        ////                                    if (isCurrent && !this.CurrentRecordManager.InEndEdit)
                                        ////                                        CurrentRecord = record;
                                    }
                                }

                                break;
                            }

                        case ListChangedType.ItemDeleted:
                            {
                                if (this.isDirty)
                                {
                                    SourceListVersion++;
                                    return; // need to reinitialize anyway ...
                                }

                                inItemDeleted = true;
                                newIndex = e.NewIndex;
                                ////if (unsortedRecordsTree.Count != this.sourceList.Count+1)
                                ////    Debugger.Break();
                                ////Debug.Assert(e.NewIndex != -1);
                                if (newIndex == -1)
                                {
                                    newIndex = unsortedRecordsTree.Count - 1;
                                }
                                else if (newIndex >= unsortedRecordsTree.Count || newIndex < 0)
                                {
                                    this.isDirty = true;
                                    goto case ListChangedType.Reset;
                                }

                                if (newIndex != -1)
                                {
                                    this.sourceListSortArray = null; // can be rebuilt from unsortedRecordsTree
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.1_unsortedRecordsTree[newIndex]"))
#endif

                                    int selectedRecordsIndexOf = this.SelectedRecords.FindRecord(newIndex);
                                    if (selectedRecordsIndexOf != -1)
                                    {
                                        this.SelectedRecords.RemoveAt(selectedRecordsIndexOf);
                                    }

                                    unsortedEntry = this.unsortedRecordsTree[newIndex];
                                    sortedEntry = unsortedEntry.Record.SortedEntry;
                                    group = unsortedEntry.Record.ParentGroup;

                                    if (group != null)
                                    {
                                        GroupsDetails gd = group.ParentSection as GroupsDetails;
                                        while (gd != null)
                                        {
                                            gd.GroupSortOrderDirty = true;
                                            gd = gd.ParentSection as GroupsDetails;
                                        }
                                    }

                                    record = sortedEntry.Element;
#if MEASURE
                                //                                using (MeasureTime.Measure("bindingList_ListChanged.2_EnsureInitialized"))
#endif
                                    ////                                {
                                    //// When deleting a record after sorting  and parent group was not updated
                                    //// but it causes a big problem with records showing empty values!
                                    ////record.EnsureInitialized(this, true);
                                    ////                                }

                                    primaryKeyEntry = record.PrimaryKeySortedEntry;
                                    if (primaryKeyEntry != null && primaryKeyEntry.Tree != null)
                                    {
                                        primaryKeyEntry.Tree.Remove(primaryKeyEntry);
                                        primaryKeyEntry.Parent = null;
                                    }

                                    record.PrimaryKeySortedEntry = null;
                                    
                                    if (sortedEntry.Tree != null)
                                    {
                                        ////oldIndex = DisplayElements.IndexOf(record);
                                        ////this.OnSourceListRecordRemoved(new RecordEventArgs(record));

#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.3_IsCurrent"))
#endif
                                        {
                                            if (record.IsCurrent)
                                            {
                                                CurrentRecordManager.Reset();

                                                if (te.NavigateCurrentRecordWhenDeleted)
                                                {
                                                    //// Move current record out of the way if it is about to be deleted.
                                                    Record r = Records[newIndex - 1];
                                                    ////                                            if (r.ParentGroup != record.ParentGroup)
                                                    ////                                            {
                                                    ////                                                if (newIndex+1 < Records.Count)
                                                    ////                                                {
                                                    ////                                                    Record r2 = Records[newIndex+1];
                                                    ////                                                    if (r2.ParentGroup == record.ParentGroup)
                                                    ////                                                        r = r2;
                                                    ////                                                }
                                                    ////                                            }
                                                    CurrentRecordManager.NavigateTo(r);
                                                }
                                            }
                                        }

                                        Group obsoleteGroup = null;
                                        if (record.MeetsFilterCriteria())
                                        {
#if MEASURE
                                        using (MeasureTime.Measure("bindingList_ListChanged.4GetFilteredRecordCount"))
#endif
                                            {
                                                if (!(group is ChildTable)
                                                    && group.Details.TreeEntries.Count == 1)
                                                {
                                                    obsoleteGroup = group;
                                                    Group g = group.ParentGroup;

                                                    // Check if one or more parent groups need to be removed.
                                                    while (g != null
                                                        && !(g is ChildTable)
                                                        && g.Groups.Count == 1)
                                                    {
                                                        obsoleteGroup = g;
                                                        g = g.ParentGroup;
                                                    }
                                                }
                                            }
                                        }

                                        RecordChangedEventArgs removingEventArgs;
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.5OnSourceListRecordChanging"))
#endif
                                        {
                                            removingEventArgs = new RecordChangedEventArgs(record, RecordChangedType.Removed, oldIndex, newIndex, obsoleteGroup);
                                            EngineTable.OnSourceListRecordChanging(removingEventArgs);
                                            if (obsoleteGroup != null)
                                            {
                                                this.OnGroupRemoving(new GroupEventArgs(obsoleteGroup));
                                            }
                                        }
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.6InvalidateCounterBottomUp"))
#endif
                                        {
                                            if (obsoleteGroup == null)
                                            {
                                                //// Remove from group and unsorted tree
                                                record.InvalidateCounterBottomUp();
                                                ////if (this.TableDescriptor.Summaries.Count > 0)
                                                record.InvalidateSummariesBottomUp();
                                            }
                                        }

                                        record.ParentElement = null;
                                        record.SortedEntry = null;
                                        record.UnsortedEntry = null;
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.record.SetData"))
#endif
                                        {
                                            record.SetData(null, false, null, null);
                                        }
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.7Remove(sortedEntry)"))
#endif
                                        {
                                            // Remove record
                                            sortedEntry.Tree.Remove(sortedEntry);
                                        }
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.8Remove(gentry)"))
#endif
                                        {
                                            // Remove group
                                            if (obsoleteGroup != null)
                                            {
                                                group = obsoleteGroup.ParentGroup; // for counter invalidation
                                                GroupCategoryTreeTableEntry gentry = obsoleteGroup.GroupCategoryEntry;
                                                if (gentry != null)
                                                {
                                                    gentry.Tree.Remove(gentry);
                                                }

                                                group.InvalidateCounterBottomUp();
                                                group.InvalidateCounterTopDown(false);
                                                group.InvalidateSummariesBottomUp();
                                                group.InvalidateSummary();
                                                group.TreeEntries.InvalidateSummariesTopDown(false);
                                                obsoleteGroup.Dispose();
                                            }

                                            record.Dispose();
                                        }
#if MEASURE
                                    //                                    using (MeasureTime.Measure("bindingList_ListChanged.9InvalidateCounterBottomUp"))
#endif
                                        ////                                    {
                                        ////                                        group.InvalidateCounterBottomUp();
                                        ////                                    }
                                        //// TODO: if this is too aggresive and not enough is refreshed then call
                                        ////    group.InvalidateCounterTopDown(true); //instead.
#if MEASURE
                                    //                                    using (MeasureTime.Measure("bindingList_ListChanged.91InvalidateCounterTopDown"))
#endif
                                        ////                                    {
                                        ////                                        group.InvalidateCounterTopDown(false);
                                        ////                                        group.Details.InvalidateCounterTopDown(false);
                                        ////                                    }
#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.92Remove(unsortedEntry)"))
#endif
                                        {
                                            ////UFD:unsortedEntry.InvalidateSummariesBottomUp(false);
                                            unsortedEntry.Tree.Remove(unsortedEntry);
                                        }

                                        SourceListVersion++;
                                        ClearCollectionCaches();
                                        ////sourceListVersion++;

#if MEASURE
                                    using (MeasureTime.Measure("bindingList_ListChanged.OnSourceListRecordChanged"))
#endif
                                        {
                                            RecordChangedEventArgs removedEventArgs = new RecordChangedEventArgs(null, RecordChangedType.Removed, oldIndex, newIndex, null);
                                            removedEventArgs.Reserved = removingEventArgs.Reserved;
                                            EngineTable.OnSourceListRecordChanged(removedEventArgs);
                                        }
                                        // force summaries for all parent groups to be refreshed
                                        if (immediateUpdateSummaries)
                                        {
                                            GetSummaries(this);
                                        }
                                    }
                                    else
                                    {
                                        // this is from an AddNew and record never was added to a group
                                        unsortedEntry.Tree.Remove(unsortedEntry);
                                        ClearCollectionCaches();
                                    }

                                    this.lastAddNewIndex = -1;
                                }

                                inItemDeleted = false;
                                break;
                            }

                        case ListChangedType.ItemAdded:
                            {
                                SourceListVersion++;

                                if (this.isDirty)
                                {
                                    return; // need to reinitialize anyway ...
                                }

                                //// When CurrencyManager.AddNew is called we'll get two ListChangedType.ItemAdded
                                //// notifications, both with the same e.NewIndex. One at the time AddNew
                                //// is called, the other when EndEdit is called.

                                if (CurrentRecordManager.InBeginEdit)
                                {
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.InBeginEdit"))
#endif
                                    {
                                        // This is the AddNew case: Add an unsorted record but do not add it to sorted records
                                        // that are displayed in engine.
                                        this.sourceListSortArray = null; // can be rebuilt from unsortedRecordsTree

                                        unsortedEntry = new UnsortedRecordsTreeEntry();
                                        sortedEntry = new SortedRecordsTreeTableEntry();

                                        // New for version 4.2 UniformChildListRelation                           
                                        if (senderChildTable != null && senderChildTable.UnsortedEntry == null)
                                        {
                                            senderChildTable.UnsortedEntry = unsortedEntry;
                                            newIndex = UnsortedRecords.Count;
                                        }

                                        SortColumnDescriptor[] arrayOfColumnDescriptors;
                                        PropertyDescriptor[] arrayOfPropertyDescriptor;
                                        bool isSorted;
                                        if (VirtualMode)
                                        {
                                            isSorted = false;
                                            arrayOfColumnDescriptors = null;
                                            arrayOfPropertyDescriptor = null;
                                        }
                                        else
                                        {
                                            TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
                                        }

                                        record = TableDescriptor.CreateRecord(this);
                                        record.sourceIndex = newIndex;
                                        record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);

                                        record.UnsortedEntry = unsortedEntry;

                                        unsortedEntry.Record = record;
                                        unsortedEntry.Tree = unsortedRecordsTree.TreeTable;

                                        // New for version 4.2 UniformChildListRelation
                                        if (this.IsNewUniformChildListRelation())
                                        {
                                            if (sender is Group)
                                            {
                                                Group ct = (Group)sender;
                                                Record fr = ct.GetFirstRecord();
                                                int n;
                                                if (fr != null)
                                                {
                                                    n = ct.ParentTable.Records.IndexOf(fr);
                                                    n += e.NewIndex;
                                                    unsortedRecordsTree.Insert(n, unsortedEntry);
                                                }
                                                else
                                                {
                                                    n = unsortedRecordsTree.Add(unsortedEntry);
                                                }

                                                if (ct is ChildTable)
                                                {
                                                    record.Parent = ((ChildTable)ct).ParentNestedTable.ParentRecord;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            unsortedRecordsTree.Add(unsortedEntry);
                                        }

                                        ClearCollectionCaches();
                                        ////sourceListVersion++;

                                        sortedEntry.Element = record;
                                        record.SortedEntry = sortedEntry;

                                        //// InsertSortedRecordsTreeEntry needs to fill out missing parts:
                                        ////sortedEntry.Tree = detailSectionWithRecords.RecordTreeEntries.TreeTable;
                                        ////record.ParentElement = detailSectionWithRecords;
                                        this.lastAddNewIndex = newIndex;
                                    }
                                }
                                else if (newIndex != -1 && newIndex == this.lastAddNewIndex && (sender is Group || newIndex == this.SourceList.Count - 1))
                                {
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.gotoItemChanged"))
#endif
                                    {
                                        goto case ListChangedType.ItemChanged;
                                    }
                                }
                                else if (newIndex != -1)
                                {
                                    //// This is the EndEdit case: (oldCount should be e.NewIndex+1)
                                    //// Here we add the record to the sorted records shown in the engine.
                                    this.sourceListSortArray = null; // can be rebuilt from unsortedRecordsTree

                                    ////Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, UnsortedRecords.Count);

                                    primaryKeyEntry = (primaryKeySortedRecordsTree != null) ? new PrimaryKeySortedRecordsTreeEntry() : null;

                                    unsortedEntry = new UnsortedRecordsTreeEntry();
                                    sortedEntry = new SortedRecordsTreeTableEntry();

                                    //// New for version 4.2 UniformChildListRelation                           
                                    /*if (senderChildTable != null && senderChildTable.UnsortedEntry == null)
                                    {
                                        senderChildTable.UnsortedEntry = unsortedEntry;
                                        newIndex = UnsortedRecords.Count;
                                    }*/
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.CreateRecord"))
#endif
                                    {
                                        record = TableDescriptor.CreateRecord(this);
                                    }

                                    SortColumnDescriptor[] arrayOfColumnDescriptors;
                                    PropertyDescriptor[] arrayOfPropertyDescriptor;
                                    bool isSorted;
                                    if (VirtualMode)
                                    {
                                        isSorted = false;
                                        arrayOfColumnDescriptors = null;
                                        arrayOfPropertyDescriptor = null;
                                    }
                                    else
                                    {
                                        TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
                                    }

                                    SortColumnDescriptor[] arrayOfPKColumnDescriptors;
                                    PropertyDescriptor[] arrayOfPKPropertyDescriptor;
                                    bool isPKSorted;
                                    TableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);
                                    record.PrimaryKeySortedEntry = primaryKeyEntry;

                                    record.UnsortedEntry = unsortedEntry;
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.UpdateSortInfo"))
#endif
                                    {
                                        record.sourceIndex = newIndex;
                                        if (!VirtualMode)
                                        {
                                            record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                                        }
                                    }

#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.OnSourceListRecordChanging"))
#endif
                                    {
                                        EngineTable.OnSourceListRecordChanging(new RecordChangedEventArgs(record, RecordChangedType.Added, -1, newIndex));
                                    }

#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.unsortedRecordsTree.Insert"))
#endif
                                    {
                                        unsortedEntry.Record = record;
                                        unsortedEntry.Tree = unsortedRecordsTree.TreeTable;
                                        if (VirtualMode)
                                        {
                                            TreeEntries.InvalidateCounterTopDown(false);
                                        }
                                        else
                                        {
                                            unsortedRecordsTree.Insert(newIndex, unsortedEntry);
                                        }
                                        ////UFD:unsortedEntry.InvalidateSummariesBottomUp(false);
                                    }
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.unsortedRecordsTree.ClearCollectionCaches"))
#endif
                                    {
                                        ClearCollectionCaches();
                                    }

                                    if (primaryKeyEntry != null)
                                    {
                                        primaryKeyEntry.Record = record;
                                        primaryKeyEntry.Tree = primaryKeySortedRecordsTree.TreeTable;
                                        record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                                        primaryKeySortedRecordsTree.Add(primaryKeyEntry);
                                    }

                                    ////sourceListVersion++;

                                    sortedEntry.Element = record;
                                    record.SortedEntry = sortedEntry;
                                    record.added = true;
                                    record.sourceIndex = newIndex;
                                    //// InsertSortedRecordsTreeEntry needs to fill out missing parts:
                                    ////sortedEntry.Tree = detailSectionWithRecords.RecordTreeEntries.TreeTable;
                                    ////record.ParentElement = detailSectionWithRecords;

                                    //// Find the right group where to insert the node.

                                    Group newGroup = InsertSortedRecordsTreeEntry(sortedEntry);
                                    ////OnSourceListRecordAdded(new RecordEventArgs(record));
                                    ////    newIndex = DisplayElements.IndexOf(record);
                                    ////record.added = true;
                                    
                                    if (newGroup != null)
                                    {
                                        GroupsDetails gd = newGroup.ParentSection as GroupsDetails;
                                        while (gd != null)
                                        {
                                            gd.GroupSortOrderDirty = true;
                                            gd = gd.ParentSection as GroupsDetails;
                                        }
                                    }

                                    ////Trace.WriteLine(DisplayElements[newIndex+1]);
#if MEASURE
                                using (MeasureTime.Measure("bindingList_ListChanged.unsortedRecordsTree.OnSourceListRecordChanged"))
#endif
                                    {
                                        EngineTable.OnSourceListRecordChanged(new RecordChangedEventArgs(record, RecordChangedType.Added, -1, newIndex, newGroup));
                                        lastChangedRecord = record;
                                        this.lastAddNewIndex = newIndex;
                                    }

                                    //// force summaries for all parent groups to be refreshed
                                    if (immediateUpdateSummaries)
                                    {
                                        GetSummaries(this);
                                    }

                                    ////Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, UnsortedRecords.Count);
                                    //// TODO: raise ItemAdded event
                                }

                                break;
                            }
                    }

                    ////this.OnRecordsSynchronized(te);
                    if (sourceList != null)
                    {
                        oldCount = sourceList.Count;
                    }
                    ////else
                    ////    oldCount = -1;
                }
                finally
                {
                    if (this.ParentTable != null)
                    {
                        this.ParentTable.OnRelatedTableSourceListListChangedCompleted(te);
                    }

                    OnSourceListListChangedCompleted(te);
                    this.inSourceListListChanged = false;
                }
            }
        }

        #endregion
        #region Initialize

        /// <summary>
        /// Returns True while records are categorized; False after.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InInitialize
        {
            get
            {
                return inInitialize;
            }
        }

        private int categorizeElementsVersion;

        /// <summary>
        /// Property CategorizeElementsVersion (int).
        /// </summary>
        public int CategorizeElementsVersion
        {
            get
            {
                return this.categorizeElementsVersion;
            }

            set
            {
                this.categorizeElementsVersion = value;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is new uniform child list relation]; otherwise, <c>false</c>.
        /// </returns>
        /// <exclude/>
        public bool IsNewUniformChildListRelation()
        {
            TableDescriptor td = TableDescriptor;
            RelationDescriptor parentRelation = td.ParentRelation;
            return !Engine.UseOldUniformChildListRelation && parentRelation != null && parentRelation.RelationKind == RelationKind.UniformChildList;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is lazy uniform child list relation]; otherwise, <c>false</c>.
        /// </returns>
        /// <exclude/>
        public bool IsLazyUniformChildListRelation()
        {
            return IsNewUniformChildListRelation() && Engine.UseLazyUniformChildListRelation;
        }

#if PERF
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public int StartTicks;
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public int EndTicks;
#endif

        #region Check for PassThroughGrouping
        IPassThroughGroupingResult ptg;

        /// <summary>
        /// Gets the PassThroughGroupingResult for the table.
        /// </summary>
        public IPassThroughGroupingResult PassThroughGroupingResult
        {
            get { return ptg; }
        }

        bool isPassThroughGrouping;

        public bool IsPassThroughGrouping
        {
            get { return isPassThroughGrouping; }
        }

        /// <summary>
        /// Record.AdjustRecordRowCount will check this property before creating
        /// and populating child tables for nested tables. Property is false
        /// when the nested collections should only be accessed and popuplated on demand.
        /// </summary>
        internal bool AllowInitChildTableInAdjustRecordRowCount
        {
            get { return !(isPassThroughGrouping || IsLazyUniformChildListRelation()); }
        }

        void CheckForPassThroughGrouping()
        {
            ptg = originalSourceList as IPassThroughGroupingResult;
            isPassThroughGrouping = false;
            string[] groupByColumns = null;
            if (ptg != null)
            {
                groupByColumns = ptg.GroupByColumns;
                isPassThroughGrouping = groupByColumns.Length > 0 || TableDescriptor.RelationChildColumns.Count > 0;
            }

            if (isPassThroughGrouping)
            {
                // Prevent Modified flag being set in TableDescriptor.GroupedColumns 
                // by temoparily setting InsideCollectionEditor = true
                TableDescriptor.GroupedColumns.InsideCollectionEditor = true;
                TableDescriptor.GroupedColumns.Reset();

                foreach (string column in groupByColumns)
                {
                    TableDescriptor.GroupedColumns.Add(column);
                }

                TableDescriptor.GroupedColumns.InsideCollectionEditor = false;

                // Allow each Record to maintain reference to its Data instead
                // of calling Table.GetSourceListItem (which would does not work
                // in the passthrough grouping case).
                cacheRecordData = true;
            }
        }
        #endregion

        void CategorizeElements()
        {
            #region Tracing
            if (inSourceListListChanged)
            {
                Debug.WriteLine("Categorization of Elements has been triggered from within an IBindingList.ListChanged event.");
                // In this case the bindingList_ListChanged routine will immediately return. No need to handle ItemDeleted, Inserted etc.
                // since CategorizeElements already updates the table.
            }

            if (Engine.HelpTracing)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
            #endregion

            #region Save SelectedRecords
            // Save positions of current record and selected records in case we switch VirtualMode
            int currentRecordSourceIndex = -1;
            int[] selectedRecordIndexes = new int[this.SelectedRecords._inner.GetCount()];
            for (int n = 0; n < selectedRecordIndexes.Length; n++)
            {
                selectedRecordIndexes[n] = -1;
            }

            #endregion

            #region Init
            TableDescriptor td = TableDescriptor;
            td.GroupedColumns.CheckCollection();
            categorizeElementsVersion++;
            inInitialize = true;
            bool engineTableinInitialize = this.EngineTable.inInitialize;
            this.EngineTable.inInitialize = true;

            RelationDescriptor parentRelation = td.ParentRelation;
            hasGroupSortOrderEntry = false;
            #endregion

            try
            {
                #region Init Continued
#if PERF
                int sticks = Environment.TickCount-StartTicks;
                int ticks = Environment.TickCount;
#endif
                this.OnCategorizingRecords(new TableEventArgs(this));

                ClearCollectionCaches();

                // Avoid surprises in InitTopLevelGroup ..
                int v = 0;
                v = td.ItemPropertiesVersion;
                if (td.IsDisposed)
                {
                    return;
                }

                // TopLevel Node
                EnsureSourceList();   // won't do anything for RelationKind.UniformChildList

                if (td.IsDisposed || Engine == null)
                {
                    return;
                }

                CheckForPassThroughGrouping();

                #region VirtualMode
                bool setVirtualMode, setWithoutCounter, setRecordsAsDisplayElements;
                CheckOptimizations(out setVirtualMode, out setWithoutCounter, out setRecordsAsDisplayElements);

                if (Engine == null)
                {
                    return;
                }

                // Save indexes for selected records when toggling VirtualMode
                if (VirtualMode != setVirtualMode)
                {
                    // Virtual Mode support - swap current and selected records without raising events
                    if (CurrentRecord != null)
                    {
                        currentRecordSourceIndex = CurrentRecord.GetSourceIndex();
                    }

                    for (int n = 0; n < selectedRecordIndexes.Length; n++)
                    {
                        if (this.SelectedRecords.GetInnerItem(n) != null)
                        {
                            Record r = this.SelectedRecords.GetInnerItem(n).Record;
                            if (r != null)
                            {
                                selectedRecordIndexes[n] = r.GetSourceIndex();
                            }
                        }
                    }
                }

                bool savedWithoutCounter = this.WithoutCounter;
                bool savedVirtualMode = this.VirtualMode;

                this.WithoutCounter = setWithoutCounter;
                this.VirtualMode = setVirtualMode;
                this.RecordsAsDisplayElements = setRecordsAsDisplayElements;
                #endregion

                InitTopLevelGroup();  // creates a ChildTable with no records for RelationKind.UniformChildList

                if (td.IsDisposed)
                {
                    return;
                }

                InvalidateCounterTopDown(true);
                #endregion

                #region VirtualMode
                // Virtual Mode support - swap current and selected records without raising events
                if (VirtualMode)
                {
                    if (TableDescriptor.SortedColumns.Count > 0)
                    {
                        SortSourceListArray();
                    }

                    if (currentRecordSourceIndex != -1)
                    {
                        CurrentRecordManager.InternalSetCurrentRecord(Records[currentRecordSourceIndex]);
                    }

                    for (int n = 0; n < selectedRecordIndexes.Length; n++)
                    {
                        if (selectedRecordIndexes[n] != -1 && this.SelectedRecords.GetInnerItem(n) != null)
                        {
                            this.SelectedRecords.GetInnerItem(n).InternalSetRecord(Records[selectedRecordIndexes[n]]);
                        }
                    }

                    return;
                }
                #endregion

                #region Set Sorting Flags
                isSorting = true;
                foreach (FieldDescriptor fd in this.TableDescriptor.Fields)
                {
                    fd.InitializeMapping(this.TableDescriptor);
                    fd.IsSorting = true;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.RelationChildColumns)
                {
                    sd.IsSorting = true;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.GroupedColumns)
                {
                    sd.IsSorting = true;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.SortedColumns)
                {
                    sd.IsSorting = true;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.PrimaryKeyColumns)
                {
                    sd.IsSorting = true;
                }
                #endregion

                TableDescriptor.InitSortByDisplayMemberCols();

                TableDescriptor.EnsureSummaryDescriptors();
                v = TableDescriptor.Summaries.Version;

                #region Threading
#if USETHRAEDING
                Thread countThread1 = null;
                Thread sumThread1 = null;
                Thread countThread2 = null;
                Thread sumThread2 = null;
                if (this.allowThreading)
                {
                    startCountSignal = new ManualResetEvent(false);
                    countGroupsList = new ArrayList();

                    countThread1 = new Thread(new ThreadStart(CounterThreadProc));
                    countThread1.Name = "CountThread1";
                    countThread1.Start();

                    if (this.TableDescriptor.Summaries.Count > 0)
                    {
                        sumThread1 = new Thread(new ThreadStart(SummariesThreadProc));
                        sumThread1.Name = "SumThread1";
                        sumThread1.Start();
                    }
                }
#endif

#if PERF
                int ticksInit = Environment.TickCount;
                int tInit = ticksInit-ticks;
#endif
                #endregion

                if (IsLazyUniformChildListRelation())
                {
                }
                else if (isPassThroughGrouping)
                {
                    #region PassThroughGrouping
                    // Clear PK, SourceListArray
                    if (ParentTableDescriptor.RelationChildColumns.Count == 0)
                    {
                        // Note: Primary Keys will not be initialized in this case.
                        PopulatePassThroughGroup(TopLevelGroup, SourceList, null);
                    }
                    #endregion
                }
                else
                {
                    #region BoundToSourceList
                    // UniformChildList: Parent Table will initialize unsorted record for deeper level. Deeper levels skip records.

                    // Initialize Unsorted List and a shadowed arraylist for sorting.
                    InitUnsortedRecords(true);

#if PERF
                int ticksInitUnsorted = Environment.TickCount;
                int tInitUnsorted = ticksInitUnsorted-ticksInit;
#endif
                    // UniformChildList: No Primery Keys fors for deeper level. 

                    // PrimaryKeys.
                    InitPrimaryKeys();

                    // UniformChildList: No sorting here for deeper level - sorting will be parent record by parent record,
                    // same with categorization.

                    // Sort
                    SortSourceListArray();

#if PERF
                int ticksSort = Environment.TickCount;
                int tSort = ticksSort-ticksInitUnsorted;
#endif
                    TableDescriptor.GroupedColumns.CheckCollection();

                    Table[] tables = new Table[this.relatedTables.Count];
                    relatedTables.CopyTo(tables, 0); // trigger SynchronizeRelatedTables
                    foreach (Table relatedTable in tables)
                    {
                        if (relatedTable.isDirty)
                        {
                            relatedTable.CategorizeElements();
                        }
                        else
                        {
                            relatedTable.EnsureInitialized(this);
                        }
                    }
#if PERF
                int ticksRelated = Environment.TickCount;
                int tRelated = ticksRelated-ticksSort;
#endif

                    ////LockOutEnsureInitialized = true;
                    //// Loop through sorted list, create groups.
                    InitGroups();
                    ////LockOutEnsureInitialized = false;
#if TESTING
                Console.WriteLine("InitGroups done");
                for (int n = 0; n < this.sourceListSortArray.Count; n++)
                    if (((Record) sourceListSortArray[n]).ParentGroup == null)
                        Debugger.Break();
                int c = UnsortedRecords.Count;
                for (int n = 0; n < c; n++)
                    if (this.UnsortedRecords[n].ParentGroup == null)
                        Debugger.Break(); 
#endif

                    // New in version 4.2: UniformChildList Relations
                    if (this.RecordToSourceListCollection.Count > 0)
                    {
                        foreach (RecordToSourceList rsl in this.RecordToSourceListCollection)
                        {
                            RelationDescriptor rd = TableDescriptor.Relations[rsl.relationName];
                            ChildTable ct = rsl.record.GetRelatedChildTable(rd);
                            ct.SourceList = rsl.sourceList;
                            ct.UnsortedEntry = rsl.UnsortedEntry;
                        }

                        this.RecordToSourceListCollection.Clear();
                    }
                    #endregion
                }

                #region ResetSortingFlags
#if PERF
                int ticksInitGroups = Environment.TickCount;
                int tInitGroups = ticksInitGroups-ticksRelated;
#endif
                foreach (FieldDescriptor fd in this.TableDescriptor.Fields)
                {
                    fd.IsSorting = false;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.RelationChildColumns)
                {
                    sd.IsSorting = false;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.GroupedColumns)
                {
                    sd.IsSorting = false;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.SortedColumns)
                {
                    sd.IsSorting = false;
                }

                foreach (SortColumnDescriptor sd in this.TableDescriptor.PrimaryKeyColumns)
                {
                    sd.IsSorting = false;
                }

                isSorting = false;

                ////this.InvalidateCounterTopDown(false);
                ////this.TopLevelGroup.InvalidateCounterTopDown(false);

                this.isDirty = false;
                #endregion

                #region Treading
#if PERF
                int ticksTotal = Environment.TickCount;
                int tTotal = ticksTotal-ticks;
#endif
#if USETHRAEDING
                if (this.allowThreading)
                {
                    lock (countGroupsList.SyncRoot)
                    {
                        countGroupsList.Add(this.TopLevelGroup);
                        startCountSignal.Set();
                    }

                    Thread.Sleep(25);

                    if (sumThread1 == null)
                    {
                        sumsCounted = countGroupsList.Count;
                        sumsCounter = countGroupsList.Count;
                    }
                    int version = 0;
                    while ((groupsCounted != countGroupsList.Count
                        || sumsCounted != countGroupsList.Count) && version<=countGroupsList.Count)
                    {
                        if (groupsCounter != countGroupsList.Count
                            && sumsCounter == countGroupsList.Count)
                        {
                            countThread2 = new Thread(new ThreadStart(CounterThreadProc));
                            countThread2.Name = "CountThread2";
                            countThread2.Start();
                            startCountSignal.Set();
                        }
                        else if (groupsCounter == countGroupsList.Count
                            && sumsCounter != countGroupsList.Count)
                        {
                            sumThread2 = new Thread(new ThreadStart(SummariesThreadProc));
                            sumThread2.Name = "SumThread2";
                            sumThread2.Start();
                            startCountSignal.Set();
                        }

                        Thread.Sleep(25);
                        version++;
                    }

                    countThread1.Abort();
                    if (countThread2 != null)
                    {
                        countThread2.Abort();
                    }

                    if (sumThread1 != null)
                    {
                        sumThread1.Abort();
                    }

                    if (sumThread2 != null)
                    {
                        sumThread2.Abort();
                    }

                    countGroupsList = null;
                }
#endif
                #endregion

                #region VirtualMode
                if (!VirtualMode && savedVirtualMode)
                {
                    // switch from Virtual Mode to Non-Virtual mode
                    if (currentRecordSourceIndex != -1)
                    {
                        Record r = UnsortedRecords[currentRecordSourceIndex];
                        if (r.MeetsFilterCriteria())
                        {
                            CurrentRecordManager.InternalSetCurrentRecord(r);
                        }
                        else
                        {
                            CurrentRecordManager.InternalSetCurrentRecord(null);
                        }
                    }

                    for (int n = 0; n < selectedRecordIndexes.Length; n++)
                    {
                        if (selectedRecordIndexes[n] != -1 && this.SelectedRecords.GetInnerItem(n) != null)
                        {
                            this.SelectedRecords.GetInnerItem(n).InternalSetRecord(UnsortedRecords[selectedRecordIndexes[n]]);
                        }
                    }
                }

                #endregion
#if PERF
                int tWaitCount = Environment.TickCount-ticksTotal;

                CategorizePerformanceInfo = String.Format("StartTicks {0}, InitTopLevelGroup {1}, InitUnsortedRecords {2}, SortSourceListArray {3}, RelatedTables {4}, InitGroups {5}, Total {6}, WaitCount {7}",
                    sticks, tInit, tInitUnsorted, tSort, tRelated, tInitGroups, tTotal, tWaitCount);

                EndTicks = Environment.TickCount;
#endif
            }
            finally
            {
                this.EngineTable.inInitialize = engineTableinInitialize;
                inInitialize = false;
            }

            #region Relation Parent

            Table relationParentTable = this.RelationParentTable;
            while (relationParentTable != null)
            {
                if (!relationParentTable.InInitialize)
                {
                    relationParentTable.InvalidateCounterTopDown(true);

                    // New in version 4.2: UniformChildList Relations
                    ArrayList al = relationParentTable.RecordToSourceListCollection;
                    if (al.Count > 0)
                    {
                        foreach (RecordToSourceList rsl in al)
                        {
                            RelationDescriptor rd = this.ParentTableDescriptor.ParentRelation;
                            ChildTable ct = rsl.record.GetRelatedChildTable(rd);
                            ct.SourceList = rsl.sourceList;
                            ct.UnsortedEntry = rsl.UnsortedEntry;
                            ct.InvalidateCounterTopDown(true);
                        }

                        al.Clear();
                    }
                }

                relationParentTable = relationParentTable.RelationParentTable;
            }
            #endregion

            // Fix a problem that sometimes Table.NestedDisplayElements.Count only returns 3
            // Same occues with TopLevelGroup.NestedDisplayElements.Count 
            FixTopLevelCounters();

            this.OnCategorizedRecords(new TableEventArgs(this));
        }

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public void FixTopLevelCounters()
        {
            this.TopLevelGroup.InvalidateCounter();
            this.TreeEntries.InvalidateCounterTopDown(false);
            this.TopLevelGroup.TreeEntries.InvalidateCounterTopDown(false);
        }
#if PERF
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public string CategorizePerformanceInfo = "";
#endif
        /// <summary>
        /// For internal use.
        /// </summary>      
        /// <returns>returns Element</returns>
        /// <exclude/>
        public Element FixVirtualMode(Element el)
        {
            // When switching between virtual mode and non-virtual mode, entries get deleted
            //
            // If element is a record we can get the record from the source index cached in the record element.
            //
            if (el != null)
            {
                ElementTreeTableEntry entry = el.GetElementEntry();
                if (entry != null)
                {
                    if (entry.Element != null)
                    {
                        if (el.ParentSection != null && !el.ParentSection.IsDisposed)
                        {
                            return el;
                        }
                    }

                    Record r = el as Record;
                    if (r != null && r.GetSourceIndex() != -1)
                    {
                        return r.ParentTable.UnsortedRecords[r.GetSourceIndex()];
                    }
                }
            }

            return null;
        }

        void InitTopLevelGroup()
        {
            TableDescriptor.GroupedColumns.CheckCollection();

            ////            if (TableDescriptor.ItemProperties.Count == 0)
            ////            {
            ////                SetTopLevelGroup(TableDescriptor.CreateChildTable(this, true, this, SortColumnDescriptorCollection.Empty));
            ////                ((Element) TopLevelGroup).ParentElement = this;
            ////                //this.isDirty = true;
            ////                return;
            ////            }

            if (this.IsDisposed)
            {
                return;
            }

            if (this.TableDescriptor.IsDisposed)
            {
                return;
            }

            ////this.TableDescriptor.Disposed += new EventHandler(TableDescriptor_Disposed2);

            bool shouldGroupHaveRecords = TableDescriptor.GroupedColumns.Count + TableDescriptor.RelationChildColumns.Count == 0
                && !IsNewUniformChildListRelation();

            if (this.TableDescriptor.IsDisposed)
            {
                return;
            }

            if (TableDescriptor.RelationChildColumns.Count > 0 || IsNewUniformChildListRelation())
            {
                SetTopLevelGroup(TableDescriptor.CreateChildTable(this, shouldGroupHaveRecords, this, ParentTableDescriptor.RelationChildColumns.GetShadowedCopy()));
            }
            else if (TableDescriptor.GroupedColumns.Count > 0)
            {
                SetTopLevelGroup(TableDescriptor.CreateChildTable(this, shouldGroupHaveRecords, this, TableDescriptor.GroupedColumns[0].GetShadowedCopy()));
            }
            else
            {
                SetTopLevelGroup(TableDescriptor.CreateChildTable(this, shouldGroupHaveRecords, this, this.TableDescriptor.SortedColumns.GetShadowedCopy()));
            }

            if (PassThroughGroupingResult != null)
            {
                TopLevelGroup.PassThroughItem = PassThroughGroupingResult.GetTotals();
            }

            DetailsSection details = TopLevelGroup.Details;

            if (shouldGroupHaveRecords)
            {
                SortedRecordsTreeTable sortedRecordsTreeTable = ((RecordsDetails)details).RecordTreeEntries;
                sortedRecordsTreeTable.Comparer = new RecordDataComparer(Comparer);

                sortedRecordsTreeTable.WithoutCounter = WithoutCounter;
                sortedRecordsTreeTable.VirtualMode = VirtualMode;

                if (VirtualMode)
                {
                    TreeTableWithCounterBranch branch = new TreeTableWithCounterBranch(sortedRecordsTreeTable.TreeTable);
                    branch.Left = new ElementTreeTableEntry();
                    branch.Right = new ElementTreeTableEntry();
                    sortedRecordsTreeTable.inner.Root = branch;
                }
            }

            if (Engine.AllowedOptimizations != EngineOptimizations.None)
            {
                if (AllowTracing)
                {
                    Trace.WriteLine(String.Format("Optimizations for {0}", TableDescriptor.Name));
                    Trace.WriteLine(String.Format("VirtualMode: {0}", VirtualMode));
                    Trace.WriteLine(String.Format("WithoutCounter: {0}", WithoutCounter));
                    Trace.WriteLine(String.Format("RecordAsDisplayElements: {0}", RecordsAsDisplayElements));
                }
            }

            if (this.RelationParentTable == null)
            {
                TopLevelGroup.SetExpanded(true, false, false);
            }

            ((Element)TopLevelGroup).ParentElement = this;
            TableDescriptor.GroupedColumns.CheckCollection();

            ////this.TableDescriptor.Disposed -= new EventHandler(TableDescriptor_Disposed2);
        }

        static bool allowTracing = false;

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public static bool AllowTracing
        {
            get { return Table.allowTracing; }
            set { Table.allowTracing = value; }
        }

        /// <summary>
        /// Set TableDirty to true for all the tables(Including nested table) when item changed
        /// </summary>
        [Browsable(false),DefaultValue(false)]
        public bool TableDirtyOnItemChanged
        {
            get { return this.tableDirtyOnItemChanged; }
            set
            {
                if (this.tableDirtyOnItemChanged != value)
                {
                    this.tableDirtyOnItemChanged = value;
                }
            }
        }
        private bool tableDirtyOnItemChanged = false;

        void CheckOptimizations(out bool virtualMode, out bool withoutCounter, out bool setRecordsAsDisplayElements)
        {
            virtualMode = false;
            withoutCounter = false;
            setRecordsAsDisplayElements = false;

            if (Engine != null && (Engine.AllowedOptimizations & EngineOptimizations.DisableCounters) != 0)
            {
                if (TableDescriptor.GroupedColumns.Count == 0 && !IsPassThroughGrouping
                    && TableDescriptor.RecordFilters.Count == 0
                    && TableDescriptor.Relations.NestedCount == 0
                    && TableDescriptor.RelationChildColumns.Count == 0)
                {
                    withoutCounter = true;
                }
            }

            if (Engine != null && (Engine.AllowedOptimizations & EngineOptimizations.VirtualMode) != 0)
            {
                //// only enable virtual mode when trees have not been populated yet
                ////if (this.VirtualMode || this.unsortedRecordsTree.Count == 0)
                {
                    if (TableDescriptor.GroupedColumns.Count == 0 && !IsPassThroughGrouping
                        && TableDescriptor.RecordFilters.Count == 0
                        && TableDescriptor.Relations.NestedCount == 0
                        && TableDescriptor.RelationChildColumns.Count == 0
                        && TableDescriptor.Summaries.Count == 0)
                    {
                        bool noSort = TableDescriptor.SortedColumns.Count == 0;
                        if (!noSort)
                        {
                            // Can use virtual mode if underlying datasource does the sorting.
                            IGroupingList gl = SourceList as IGroupingList;
                            noSort = gl != null && gl.SupportsGroupSorting;
                        }

                        if (!noSort && (Engine.AllowedOptimizations & EngineOptimizations.PassThroughSort) != 0)
                        {
                            IBindingList bl = SourceList as IBindingList;
                            if (bl != null && bl.SupportsSorting && (this.TableDescriptor.SortedColumns.Count == 1 || bl is DataView))
                            {
                                // Make sure these are not expression fields, foreign key fields etc. Only properties
                                // of the underlying datasource can be used for pass-through sorting.
                                noSort = true;
                                foreach (SortColumnDescriptor sd in this.TableDescriptor.SortedColumns)
                                {
                                    noSort &= sd.FieldDescriptor.GetSimplePropertyDescriptor() != null;
                                }
                            }
                        }

                        if (noSort)
                        {
                            virtualMode = true;
                            withoutCounter = true;
                        }
                    }
                }
            }

            if (Engine != null && (Engine.AllowedOptimizations & EngineOptimizations.RecordsAsDisplayElements) != 0)
            {
                if (TableDescriptor.RowsPerRecord == 1
                    && TableDescriptor.Relations.NestedCount == 0
                    && TableDescriptor.PreviewRowsPerRecord == 0)
                {
                    setRecordsAsDisplayElements = true;
                }
            }
        }

        bool recordsAsDisplayElements;

        /// <summary>
        /// Set this True if you do not want the engine to treat Record and ColumnHeaderSection
        /// elements as ContainerElements and instead have these elements be returned as
        /// a display element in the Table.DisplayElements collection.
        /// </summary>
        /// <remarks>
        /// With a GridGroupingControl, you must not change this property since a GridGroupingControl
        /// relies on the behavior that a record is not a display element but a container for rows
        /// and nested tables.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool RecordsAsDisplayElements
        {
            get
            {
                return recordsAsDisplayElements;
            }

            set
            {
                recordsAsDisplayElements = value;
            }
        }

        /*private void TableDescriptor_Disposed2(object sender, EventArgs e)
        {
            TraceUtil.TraceCalledFrom(10);
        }*/

        bool withoutCounter = false;

        public bool WithoutCounter
        {
            get
            {
                return withoutCounter;
            }

            set
            {
                if (withoutCounter != value)
                {
                    ////    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("WithoutCounter"));
                    withoutCounter = value;
                    ////    Table.TableDirty = true;
                    ////    Table.ClearCollectionCaches();
                    ////    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("WithoutCounter"));
                }
            }
        }

        bool virtualMode = true;

        public bool VirtualMode
        {
            get
            {
                return virtualMode;
            }

            set
            {
                if (virtualMode != value)
                {
                    virtualMode = value;
                }
            }
        }

        internal class RecordToSourceList
        {
            public Record record;
            public string relationName;
            public IList sourceList;
            internal UnsortedRecordsTreeEntry UnsortedEntry;
        }

        internal ArrayList RecordToSourceListCollection = new ArrayList();
        bool allowRebuildfromUnsortedRecordsTree = true;

        void InitUnsortedRecords(bool forceParentElement)
        {
#if TESTING
            Console.WriteLine("InitUnsortedRecords");
#endif
            int sourceIndex = 0;
            IList unsortedList = this.SourceList;
            int count = unsortedList != null ? unsortedList.Count : 0;
            if (count == 0 && Engine.GetDesignMode())
            {
                unsortedList = new object[1];
                count = 1;
            }

            this.TableDescriptor.ResetSortInfoCache();

            bool isSorted;
            SortColumnDescriptor[] arrayOfColumnDescriptors;
            PropertyDescriptor[] arrayOfPropertyDescriptor;
            this.TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

            SortColumnDescriptor[] arrayOfPKColumnDescriptors;
            PropertyDescriptor[] arrayOfPKPropertyDescriptor;
            bool isPKSorted;
            TableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);

            if (IsLazyUniformChildListRelation())
            {
                return;
            }
            else if (this.IsNewUniformChildListRelation())
            {
#if TESTING
                Console.WriteLine("InitUnsortedRecords-IsNewUniformChildListRelation");
#endif
                RelationDescriptor rd = this.TableDescriptor.ParentRelation;
                string mappingName = rd.MappingName;
                string relationName = rd.Name;
                PropertyDescriptor pd = this.TableDescriptor.ParentTableDescriptor.ItemProperties[mappingName];

                int n = 0;
                count = 0;
                ////foreach (Record parentRecord in this.RelationParentTable.UnsortedRecords)
                ////{
                ////    object obj = pd.GetValue(parentRecord.GetData());
                ////    while (obj is IListSource && ((IListSource) obj).ContainsListCollection)
                ////        obj = ((IListSource) obj).GetList();
                ////    IList sourceList = obj as IList;
                ////    if (sourceList != null)
                ////        count += sourceList.Count;
                ////}

                if (sourceListSortArray == null || sourceListSortArray.Count != count
                     || (sourceListSortArray.Count > 0 && ((Record)sourceListSortArray[0]).IsDisposed))
                {
                    ////parentRecord.AdjustRecordRowCount();
                    ////NestedTable nt = parentRecord.NestedTables[relationName];
                    ////ChildTable ct = nt.ChildTable;
                    ////IEnumerable list = ct.SourceList;
                    ////ArrayList sourceListSortArray2 = ct.SourceListSortArray;
                    ////if (ct.SourceListSortArray == null)
                    ////{
                    ////.GetValue(parentRecord.GetData()) as IEnumerable;
                    this.cacheRecordData = true;
                    sourceListSortArray = new ArrayList(count);
                    unsortedRecordsTree.Clear();

                    foreach (Record parentRecord in this.RelationParentTable.UnsortedRecords)
                    {
                        parentRecord.InvalidateCounter();
                        object obj = pd.GetValue(parentRecord.GetData());
                        while (obj is IListSource && ((IListSource)obj).ContainsListCollection)
                        {
                            obj = ((IListSource)obj).GetList();
                        }

                        IEnumerable list = obj as IEnumerable;
                        ////n = 0;
                        ////sourceListSortArray2 = new ArrayList();
                        bool first = true;
                        UnsortedRecordsTreeEntry firstUnsortedEntry = null;

                        if (list != null)
                        {
                            foreach (object item in list)
                            {
                                UnsortedRecordsTreeEntry unsortedEntry = new UnsortedRecordsTreeEntry();
                                Record record = TableDescriptor.CreateRecord(this);
                                record.UnsortedEntry = unsortedEntry;

                                if (this.IsNewUniformChildListRelation())
                                {
                                    record.sourceIndex = n;  //// prevent data from being set to null.
                                }

                                record.sourceListVersion = this.SourceListVersion;

                                record.Parent = parentRecord; //// must be set before UpdateSortInfo is called!
                                record.SetData(item, isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                                record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                                record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                                sourceListSortArray.Add(record);

                                unsortedEntry.Record = record;
                                unsortedEntry.Tree = unsortedRecordsTree.TreeTable;
                                unsortedRecordsTree.Add(unsortedEntry);
                                n++;

                                if (first)
                                {
                                    firstUnsortedEntry = unsortedEntry;
                                    first = false;
                                }
                            }
                        }

                        RecordToSourceList rsl = new RecordToSourceList();
                        rsl.record = parentRecord;
                        rsl.relationName = relationName;
                        if (list is IList)
                            rsl.sourceList = (IList)list;
                        else
                        {
                            IList lt = new List<object>();
                            foreach (object ob in list)
                            {
                                lt.Add(ob);
                            }
                            rsl.sourceList = lt;
                        }
                        rsl.UnsortedEntry = firstUnsortedEntry;
                        parentRecord.ParentTable.RecordToSourceListCollection.Add(rsl);
                        ////}
                        ////else
                        ////    InitChildNodeEntry(this.TopLevelGroup.Details as GroupsDetails, ct.GroupCategoryEntry, new object[0]);
                    }

                    ////NestedTable nt = parentRecord.NestedTables[relationName];
                    ////ChildTable ct = nt.ChildTable;
                    //// Sort the list
                    ////if (TableDescriptor.IsSorted)
                    ////{
                    ////    if (Comparer != null)
                    ////        sourceListSortArray.Sort(new TableRecordDataComparer(this, Comparer));
                    ////}
                    ////InitGroups(ct, sourceListSortArray2);
                }
            }
            else if (sourceListSortArray == null || sourceListSortArray.Count != count
                || (sourceListSortArray.Count > 0 && ((Record)sourceListSortArray[0]).IsDisposed))
            {
#if TESTING
                Console.WriteLine("InitUnsortedRecords-aaa");
#endif
                //// TODO: Check performance if it would be better to interate through
                //// unsortedRecordsTree and rebuild sourceListSortArray from unsortedRecordsTree
                sourceListSortArray = new ArrayList(count);

                //// Make sure unsortedRecordsTree is reset if underlying datasource list changes because
                //// then it needs to be rebuild!
                bool rebuildfromUnsortedRecordsTree = allowRebuildfromUnsortedRecordsTree && !this.IsNewUniformChildListRelation() && unsortedRecordsTree != null && unsortedRecordsTree.Count == count;
                if (rebuildfromUnsortedRecordsTree)
                {
#if TESTING
                    Console.WriteLine("InitUnsortedRecords-rebuildfromUnsortedRecordsTree");
#endif
                    foreach (UnsortedRecordsTreeEntry unsortedEntry in unsortedRecordsTree)
                    {
                        Record record = unsortedEntry.Record;
                        record.ParentElement = this;
                        record.sourceIndex = sourceIndex++;
                        record.ResetValues();
                        record.EnsureValues();
                        record.InvalidateCounterTopDown(true);
                        record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                        record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                        sourceListSortArray.Add(record);
                    }
                }
                else
                {
#if TESTING
                    Console.WriteLine("InitUnsortedRecords-clear");
#endif
                    unsortedRecordsTree.Clear();
                    unsortedRecordsTree.BeginInit();
                    for (int n = 0; n < count; n++)
                    {
                        UnsortedRecordsTreeEntry unsortedEntry = new UnsortedRecordsTreeEntry();
                        Record record = TableDescriptor.CreateRecord(this);
                        record.UnsortedEntry = unsortedEntry;

                        record.sourceIndex = n;
                        record.sourceListVersion = this.SourceListVersion;

                        record.EnsureValues();
                        record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                        record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);

                        sourceListSortArray.Add(record);

                        unsortedEntry.Record = record;
                        unsortedEntry.Tree = unsortedRecordsTree.TreeTable;
                        unsortedRecordsTree.Add(unsortedEntry);
                    }

                    unsortedRecordsTree.EndInit();
                }
            }
            else
            {
#if TESTING
                Console.WriteLine("InitUnsortedRecords-sourceListSortArray");
#endif
                foreach (Record record in sourceListSortArray)
                {
                    if (forceParentElement)
                    {
                        record.ParentElement = this;
                    }

                    ////record.sourceIndex = sourceIndex++;
                    record.ResetValues();
                    record.EnsureValues();
                    record.InvalidateCounterTopDown(true);
                    record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                    record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                }
            }

            oldCount = unsortedList != null ? unsortedList.Count : 0;
            allowRebuildfromUnsortedRecordsTree = true;
        }

        bool isSorting = false;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSorting
        {
            get
            {
                return isSorting;
            }
        }

#if PERF
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public int SortTicks;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public int SortPKTicks;
#endif

        void InitPrimaryKeys()
        {
            if (TableDescriptor.PrimaryKeyColumns.Count > 0 || (lastPKSorted && Object.ReferenceEquals(lastPKSortedList, SourceList)))
            {
#if PERF
                int ticks = Environment.TickCount;
#endif
                if (primaryKeySortedRecordsTree == null)
                {
                    primaryKeySortedRecordsTree = new PrimaryKeySortedRecordsTree(this);
                }
                else
                {
                    primaryKeySortedRecordsTree.Clear();
                }

                IGroupingList gl = SourceList as IGroupingList;
                ArrayList pksourceListSortArray = (ArrayList)sourceListSortArray.Clone();
                pksourceListSortArray.Sort(new TablePrimaryKeyRecordDataComparer(this, this.Comparer));
                lastPKSorted = TableDescriptor.PrimaryKeyColumns.Count > 0;
                lastPKSortedList = SourceList;
#if PERF
                SortPKTicks = Environment.TickCount-ticks;
#endif

                primaryKeySortedRecordsTree.BeginInit();

                // Get first record, initialize all groups.
                Record record;
                if (pksourceListSortArray.Count > 0)
                {
                    record = (Record)pksourceListSortArray[0];
                    TableDescriptor td = ParentTableDescriptor;
                    int count = sourceListSortArray.Count;
                    for (int n = 0; n < count; n++)
                    {
                        record = (Record)pksourceListSortArray[n];
                        ////Console.WriteLine(record.Info);

                        PrimaryKeySortedRecordsTreeEntry primaryKeyEntry = new PrimaryKeySortedRecordsTreeEntry();
                        primaryKeyEntry.Tree = primaryKeySortedRecordsTree.TreeTable;
                        primaryKeyEntry.Record = record;
                        record.PrimaryKeySortedEntry = primaryKeyEntry;
                        primaryKeySortedRecordsTree.Add(primaryKeyEntry);
                    }
                }

                primaryKeySortedRecordsTree.EndInit();

                pksourceListSortArray = null;
            }
            else
            {
                lastPKSorted = false;
                lastPKSortedList = null;
            }

            _primaryKeyColumnsVersion = TableDescriptor.PrimaryKeyColumns.Version;
        }

        void SortSourceListArray()
        {
            if (TableDescriptor.IsSorted || (lastSorted && Object.ReferenceEquals(lastSortedList, SourceList)))
            {
#if PERF
                int ticks = Environment.TickCount;
#endif
                IGroupingList gl = SourceList as IGroupingList;
                IBindingList bl = SourceList as IBindingList;
                if (gl != null && gl.SupportsGroupSorting)
                {
                    gl.ApplySort(ParentTableDescriptor.RelationChildColumns, this.TableDescriptor.GroupedColumns, this.TableDescriptor.SortedColumns);
                }
                else if (VirtualMode && bl != null && bl.SupportsSorting && (this.TableDescriptor.SortedColumns.Count == 1 || bl is DataView))
                {
                    if (bl is DataView)
                    {
                        DataView dv = bl as DataView;
                        bool first = true;
                        StringBuilder sb = new StringBuilder();
                        foreach (SortColumnDescriptor sd in this.TableDescriptor.SortedColumns)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                sb.Append(", ");
                            }

                            sb.Append(sd.FieldDescriptor.MappingName);
                            if (sd.SortDirection == ListSortDirection.Descending)
                            {
                                sb.Append(" DESC");
                            }
                        }

                        string sort = sb.ToString();

                        string dvSort = dv.Sort.Trim(new char[] { '[', ']' });
                        if (sort != dvSort)
                        {
                            dv.Sort = sort;
                        }
                        else
                        {
                            CurrentRecordManager.Reset();
                        }
                    }
                    else
                    {
                        bl.ApplySort(TableDescriptor.SortedColumns[0].FieldDescriptor.GetPropertyDescriptor(), TableDescriptor.SortedColumns[0].SortDirection);
                    }
                }
                else if (Comparer != null)
                {
                    sourceListSortArray.Sort(new TableRecordDataComparer(this, Comparer));
                }
                else if (ChildTableGroupLevelCount > 0)
                {
                    throw new InvalidOperationException("A Comparer must be specified if ChildTableGroupLevelCount is greater than 0.");
                }

                lastSorted = TableDescriptor.IsSorted;
                lastSortedList = SourceList;
#if PERF
                SortTicks = Environment.TickCount-ticks;
#endif
            }
            else
            {
                lastSorted = false;
                lastSortedList = null;
            }
        }

        IList lastSortedList = null;
        IList lastPKSortedList = null;

        #endregion
        #region Insert Record

        internal Group InsertSortedRecordsTreeEntry(SortedRecordsTreeTableEntry sortedEntry)
        {
#if MEASURE
            using (MeasureTime.Measure("InsertSortedRecordsTreeEntry"))
#endif
            {
                IGroupByCategorizer groupByCategorizer = this.GroupByCategorizer;
                Record record = sortedEntry.Element;
                Group group = this.TopLevelGroup;
                int currentLevel = 0;
                Stack groupStack = new Stack();
                Group newGroup = null;

                while (group != null)
                {
                    DetailsSection detailSection = group.Details;
                    detailSection.EnsureInitialized(this, false);
                    if (detailSection.HasRecords)
                    {
                        RecordsDetails detailSectionWithRecords = (RecordsDetails)detailSection;
                        sortedEntry.Tree = detailSectionWithRecords.RecordTreeEntries.TreeTable;
                        if (record.sourceIndex == -1)
                        {
                            record.sourceIndex = UnsortedRecords.Count;
                        }

                        record.SortedEntry = sortedEntry;
                        record.ParentElement = detailSectionWithRecords;
#if DEBUG
                        Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, "Record Adding");
#endif

#if MEASURE
                        using (MeasureTime.Measure("InsertSortedRecordsTreeEntry.detailSectionWithRecords.RecordTreeEntries.Add"))
#endif
                        {
                            detailSectionWithRecords.RecordTreeEntries.Add(sortedEntry);
                            if (Engine.UseLazyUniformChildListRelation && !TableDescriptor.IsSorted &&
                                record.sourceIndex == sourceList.Count - 1 &&
                                sortedEntry.GetPosition() < record.sourceIndex)
                            {
                                this.Reload();
                            }
                            ////record.InvalidateCounterTopDown(true);
                        }

#if DEBUG
                        if (this.ParentTable == null)
                        {
                            Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, "Record Added - Calling record.InvalidateCounterBottomUp");
                        }
#endif

#if MEASURE
                        using (MeasureTime.Measure("InsertSortedRecordsTreeEntry.record.InvalidateCounterBottomUp"))
#endif
                        {
                            detailSectionWithRecords.InvalidateCounterBottomUp();
                            detailSectionWithRecords.InvalidateSummariesBottomUp();
                        }

                        break;
                    }
                    else
                    {
                        Group foundGroup = null;
                        GroupsDetails detailSectionWithGroups = (GroupsDetails)detailSection;
                        GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
                        GroupCategoryTreeTableEntry entry; ////int pos = 0;

                        object[] categoryKeys = null;

                        categoryKeys = GetGroupByCategoryKeys(groupByCategorizer, group, record);

#if MEASURE
                        using (MeasureTime.Measure("InsertSortedRecordsTreeEntry.detailSectionWithGroups.GroupCategoryTreeTable.inner.AddIfNotExists"))
#endif
                        {
                            entry = (GroupCategoryTreeTableEntry)detailSectionWithGroups.GroupCategoryTreeTable.TreeTable.AddIfNotExists(categoryKeys, childNodeEntry);
                        }

                        if (entry != childNodeEntry)
                        {
                            foundGroup = entry.Element;
                        }

#if MEASURE
                        using (MeasureTime.Measure("InsertSortedRecordsTreeEntry.foundGroup"))
#endif
                        {
                            if (foundGroup == null)
                            {
                                // new category
                                Group groupByNode;
                                bool shouldGroupHaveRecords = currentLevel == ChildTableGroupLevelCount - 1;
                                if (group.IsMainGroup && TableDescriptor.RelationChildColumns.Count > 0)
                                {
                                    groupByNode = TableDescriptor.CreateChildTable(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.RelationChildColumns);
                                }
                                else if (group.GroupLevel + 1 < this.TableDescriptor.GroupedColumns.Count)
                                {
                                    groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, this.TableDescriptor.GroupedColumns[group.GroupLevel + 1].GetShadowedCopy());
                                }
                                else
                                {
                                    groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, this.TableDescriptor.SortedColumns.GetShadowedCopy());
                                }

                                if (shouldGroupHaveRecords)
                                {
                                    ((RecordsDetails)groupByNode.Details).RecordTreeEntries.Comparer = new RecordDataComparer(Comparer);
                                }

                                childNodeEntry.Tree = detailSectionWithGroups.GroupCategoryTreeTable.TreeTable;
                                childNodeEntry.Element = groupByNode;
                                groupByNode.GroupCategoryEntry = childNodeEntry;
                                groupByNode.ParentElement = detailSectionWithGroups;
                                ClearCollectionCaches();
                                groupByNode.CategoryKeys = categoryKeys;

                                bool foreignKeyFieldsFound;
                                object[] categoryForeignKeyParentIds = GetCategoryForeignKeyParentIds(groupByCategorizer, group, record, out foreignKeyFieldsFound);
                                if (foreignKeyFieldsFound)
                                {
                                    groupByNode.CategoryForeignKeyParentIds = categoryForeignKeyParentIds;
                                }

                                groupStack.Push(groupByNode);

                                //// Get the category iten that identifies this group. Use groupByNode.Level to determine
                                //// which column is used for this group...

                                if (newGroup == null)
                                {
                                    newGroup = groupByNode;
                                }

                                //// Continue with while loop until record is added.
                                group = groupByNode;
                            }
                            else
                            {
                                group = foundGroup;
                            }
                        }

                        ////group.InvalidateCounterBottomUp();
                        ////Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, group.GetVisibleCount());

                        currentLevel++;
                    }
                } //// end while

#if MEASURE
                using (MeasureTime.Measure("InsertSortedRecordsTreeEntry.OnGroupAdded"))
#endif
                {
                    while (groupStack.Count > 0)
                    {
                        this.OnGroupAdded(new GroupEventArgs((Group)groupStack.Pop()));
                    }
                }

                return newGroup;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns ChildTable</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public ChildTable AddChildTableIfNotExists(object[] categoryKeys)
        {
            GroupsDetails detailSectionWithGroups = (GroupsDetails)TopLevelGroup.Details;
            GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
            GroupCategoryTreeTableEntry entry = (GroupCategoryTreeTableEntry)detailSectionWithGroups.GroupCategoryTreeTable.TreeTable.AddIfNotExists(categoryKeys, childNodeEntry);

            if (entry.Element == null)
            {
                return InitChildNodeEntry(detailSectionWithGroups, childNodeEntry, categoryKeys);
            }

            return entry.Element as ChildTable;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns ChildTable</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public ChildTable AddChildTable()
        {
            GroupsDetails detailSectionWithGroups = (GroupsDetails)TopLevelGroup.Details;
            GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
            detailSectionWithGroups.GroupCategoryTreeTable.TreeTable.Add(childNodeEntry);
            return InitChildNodeEntry(detailSectionWithGroups, childNodeEntry, new object[0]);
        }

        internal ChildTable InitChildNodeEntry(GroupsDetails detailSectionWithGroups, GroupCategoryTreeTableEntry childNodeEntry, object[] categoryKeys)
        {
            bool shouldGroupHaveRecords = TableDescriptor.GroupedColumns.Count == 0;
            ChildTable groupByNode = TableDescriptor.CreateChildTable(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.RelationChildColumns);
            if (shouldGroupHaveRecords)
            {
                ((RecordsDetails)groupByNode.Details).RecordTreeEntries.Comparer = new RecordDataComparer(Comparer);
            }

            childNodeEntry.Tree = detailSectionWithGroups.GroupCategoryTreeTable.TreeTable;
            childNodeEntry.Element = groupByNode;
            groupByNode.GroupCategoryEntry = childNodeEntry;
            groupByNode.ParentElement = detailSectionWithGroups;
            ClearCollectionCaches();
            groupByNode.CategoryKeys = categoryKeys;

            this.OnGroupAdded(new GroupEventArgs(groupByNode));

            groupByNode.InvalidateCounterBottomUp();
            groupByNode.InvalidateSummariesBottomUp();

            return groupByNode;
        }

        void SynchronizeGroup(Group group)
        {
            Group obsoleteGroup = null;
            if (!(group is ChildTable)
                && group.Details.TreeEntries.Count == 0)
            {
                obsoleteGroup = group;
                Group g = group.ParentGroup;

                // Check if one or more parent groups need to be removed.
                while (g != null
                    && !(g is ChildTable)
                    && g.Groups.Count == 1)
                {
                    obsoleteGroup = g;
                    g = g.ParentGroup;
                }
            }

            if (obsoleteGroup != null)
            {
                this.OnGroupRemoving(new GroupEventArgs(obsoleteGroup));
                GroupCategoryTreeTableEntry entry = obsoleteGroup.GroupCategoryEntry;
                entry.Tree.Remove(entry);
            }
        }

        #endregion
        #region Remove Record
        internal object deletedrecord = null;
        /// <summary>
        /// Removes a record from the datasource.
        /// </summary>
        /// <param name="r">The record to delete.</param>
        public void DeleteRecord(Record r)
        {
            if (this.TableDescriptor.AllowRemove && this.SourceListAllowRemove)
            {
                if (this.RaiseRecordDeleting(r))
                {
                    int pos = this.UnsortedRecords.IndexOf(r);
                    if (pos >= 0 && this.SourceList != null)
                    {
                        deletedrecord = this.SourceList[pos];
                        this.SourceList.RemoveAt(pos);
                    }
                    else if (pos >= 0 && r.ParentChildTable != null && r.ParentChildTable.SourceList != null)
                    {
                        ChildTable table = r.ParentChildTable;
                        pos = table.SourceList.IndexOf(r.GetData());
                        if (pos > -1)
                        {
                            deletedrecord = table.SourceList[pos];
                            table.SourceList.Remove(table.SourceList[pos]);
                        }
                        else
                        {
                            return; //do not raise RecordDeleted event...
                        }
                    }

                    this.RaiseRecordDeleted(r);
                }
            }
        }
        #endregion
        #region Insert and Filter Child Table
        /// <summary>
        /// Returns the sum of TableDescriptor.RelationChildColumns.Count and TableDescriptor.GroupedColumns.Count.
        /// </summary>
        int ChildTableGroupLevelCount
        {
            get
            {
                return ParentTableDescriptor.RelationChildColumns.Count + this.TableDescriptor.GroupedColumns.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="FilteredChildTable"/>, if any or the <see cref="TopLevelGroup"/> if <see cref="FilteredChildTable"/> is null.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ChildTable FilteredChildTableOrTopLevelGroup
        {
            get
            {
                ChildTable ct = this.FilteredChildTable;
                if (ct == null && !this.inSetSourceList && !this.IsDisposed && !Engine.InInitializeFrom)
                {
                    ct = this.TopLevelGroup;
                }

                return ct;
            }
        }

        /// <summary>
        /// Gets / sets a ChildTable. Setting this property forces the DisplayElements collection
        /// to return elements only for a specific child table. This property
        /// is used by the GridGroupingControl to quickly switch the context of table between child tables
        /// of different parent records when hierarchical data are displayed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        ////[EditorBrowsable(EditorBrowsableState.Never)]
        public ChildTable FilteredChildTable
        {
            get
            {
                if (filteredChildTable != null && filteredChildTable.IsDisposed)
                {
                    return null;
                }

                return filteredChildTable;
            }

            set
            {
                if (value != null && value.IsDisposed)
                {
                    throw new InvalidOperationException();
                }

                if (value != null && value.IsMainGroup)
                {
                    value = null;
                }

                if (filteredChildTable != value)
                {
                    ////this.ClearCollectionCaches();
                    ////if (filteredChildTable != null && value != null)
                    ////{
                    ////    if (filteredChildTable.ToString().EndsWith("45") && value.ToString().EndsWith("46"))
                    ////        Console.WriteLine(FilteredChildTable.ToString());
                    ////}
                    if (value != null)
                    {
                        if (value.ParentTable == null)
                        {
                            return;
                        }

                        if (value.ParentTable != this)
                        {
                            throw new ArgumentException("Expected a ChildTable that is a child element of this table.");
                        }
                    }

                    filteredChildTable = value;
                    DisplayElements.cachedFilteredChildTable = null;
                    NestedDisplayElements.cachedFilteredChildTable = null;
                    ////                    if (value != null && SharedBaseAssembly.DumpFilteredChildTable)
                    ////                    {
                    ////                        NestedTable nt = value.ParentNestedTable;
                    ////                        int rowIndex = nt.Engine.Table.NestedDisplayElements.IndexOf(nt);
                    ////                        ////if (rowIndex == 24)
                    ////                        ////    Debugger.Break();
                    ////                        TraceUtil.TraceCurrentMethodInfo("Row " + rowIndex, value.Info);
                    ////                        TraceUtil.TraceCalledFrom(10);
                    ////                    }
                }
            }
        }

        internal ChildTable AddEmptyChildTableWithGroups(GroupCategoryTreeTableEntry childNodeEntry, object[] categoryKeys)
        {
            Record addNew = AddNewRecord;
            if (addNew == null)
            {
                throw new NotSupportedException();
            }

            int foreignKeyLevelCount = ParentTableDescriptor.RelationChildColumns.Count > 0 ? 1 : 0;
            int groupLevelCount = foreignKeyLevelCount + this.TableDescriptor.GroupedColumns.Count;

            GroupsDetails detailSectionWithGroups = (GroupsDetails)TopLevelGroup.Details;
            bool shouldGroupHaveRecords = TableDescriptor.GroupedColumns.Count == 0;

            Group startGroup = TableDescriptor.CreateChildTable(detailSectionWithGroups, shouldGroupHaveRecords, this, ParentTableDescriptor.RelationChildColumns);
            if (shouldGroupHaveRecords)
            {
                ((RecordsDetails)startGroup.Details).RecordTreeEntries.Comparer = new RecordDataComparer(Comparer);
            }

            //// Insert new ChildTable table
            childNodeEntry.Tree = detailSectionWithGroups.categoryTreeTable.TreeTable;
            childNodeEntry.Element = startGroup;
            startGroup.GroupCategoryEntry = childNodeEntry;
            startGroup.ParentElement = detailSectionWithGroups;
            ClearCollectionCaches();
            startGroup.CategoryKeys = categoryKeys;
            startGroup.Details.BeginInit();

            ChildTable childTable = (ChildTable)startGroup;

            Group group = startGroup;
            int currentLevel = startGroup.ChildTableGroupLevel;
            detailSectionWithGroups = startGroup.Details as GroupsDetails;
            while (++currentLevel < groupLevelCount)
            {
                Group groupByNode;
                shouldGroupHaveRecords = currentLevel == groupLevelCount - 1;

                groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.GroupedColumns[group.GroupLevel + 1].GetShadowedCopy());

                if (shouldGroupHaveRecords)
                {
                    ((RecordsDetails)groupByNode.Details).RecordTreeEntries.Comparer = new RecordDataComparer(Comparer);
                }

                groupByNode.Details.BeginInit();
                childNodeEntry = new GroupCategoryTreeTableEntry();
                childNodeEntry.Tree = detailSectionWithGroups.categoryTreeTable.TreeTable;
                childNodeEntry.Element = groupByNode;
                groupByNode.GroupCategoryEntry = childNodeEntry;
                groupByNode.ParentElement = detailSectionWithGroups;
                detailSectionWithGroups.GroupCategoryTreeTable.Add(childNodeEntry);

                // Get the category iten that identifies this group. Use groupByNode.Level to determine
                // which column is used for this group...
                groupByNode.CategoryKeys = GetGroupByCategoryKeys(GroupByCategorizer, group, addNew);

                detailSectionWithGroups = groupByNode.Details as GroupsDetails;

                group = groupByNode;
            }

            while (group != startGroup)
            {
                group.Details.EndInit();
                RaiseGroupAdded(new GroupEventArgs(group));
                group = group.ParentGroup;
            }

            startGroup.Details.EndInit();
            RaiseGroupAdded(new GroupEventArgs(startGroup));

            return childTable;
        }

        #endregion
        #region Insert Groups
        int foreignKeyLevelCount = -1;
        int groupLevelCount = -1;

        void InitGroups()
        {
            InitGroups(this.TopLevelGroup, sourceListSortArray, this.allowThreading);
        }

        void InitGroups(Group topLevel, ArrayList sourceListSortArray, bool allowThreading)
        {
            topLevel.Details.BeginInit();
            recordComparer = new RecordDataComparer(Comparer);

            // Get first record, initialize all groups.
            Record record;
            if (sourceListSortArray.Count > 0)
            {
                record = (Record)sourceListSortArray[0];
                TableDescriptor td = ParentTableDescriptor;
                foreignKeyLevelCount = td.RelationChildColumns.Count > 0 ? 1 : 0;
                groupLevelCount = foreignKeyLevelCount + this.TableDescriptor.GroupedColumns.Count;
                Group currentGroupWithRecords = BeginGroups(topLevel, record);
                RecordsDetails detailSectionWithRecords = (RecordsDetails)currentGroupWithRecords.Details;
#if TESTING
                if (detailSectionWithRecords.ParentGroup == null)
                    Debugger.Break();
#endif
                int count = sourceListSortArray.Count;
                for (int n = 0; n < count; n++)
                {
                    record = (Record)sourceListSortArray[n];
                    if (topLevel != currentGroupWithRecords && ChildTableGroupLevelCount > 0)
                    {
                        Group notEqualCategory = CompareCategoryKeys(currentGroupWithRecords, record);

                        if (notEqualCategory != null)
                        {
                            Group group = notEqualCategory.ParentGroup;
                            Group g = EndGroups(currentGroupWithRecords, group);
                            ////Console.WriteLine(g.Info);

#if USETHRAEDING
                            if (allowThreading && g != null)
                            {
                                //// Queue the task.
                                ////lock (countGroupsList.SyncRoot)
                                {
                                    lock (countGroupsList.SyncRoot)
                                    {
                                        countGroupsList.Add(g);
                                    }
                                }

                                //// Inform counter thread that countGroupsList is ready to be processed
                                startCountSignal.Set();
                            }
#endif

                            currentGroupWithRecords = BeginGroups(group, record);
                            detailSectionWithRecords = (RecordsDetails)currentGroupWithRecords.Details;
#if TESTING
                            if (detailSectionWithRecords.ParentGroup == null)
                                Debugger.Break();
#endif
                        }
                    }

                    SortedRecordsTreeTableEntry sortedEntry = new SortedRecordsTreeTableEntry();
                    sortedEntry.Tree = detailSectionWithRecords.RecordTreeEntries.TreeTable;
                    sortedEntry.Element = record;
                    record.SortedEntry = sortedEntry;
                    record.ParentElement = detailSectionWithRecords;
                    ////int nestedTableCount = record.NestedTables.Count; // force initialization of NestedTables
                    detailSectionWithRecords.RecordTreeEntries.Add(sortedEntry);
                    ////Trace.WriteLine(record.ParentElement.GetType().Name + ": " + record.Info);

                    if (this.relatedTables.Count > 0)
                    {
                        record.InvalidateCounter();
                        record.AdjustRecordRowCount();
                    }
                }

                EndGroups(currentGroupWithRecords, topLevel);
            }

            topLevel.Details.EndInit();
        }

        void AddPassThroughGroupsWithoutDetails(Group group, IEnumerable items)
        {
            IPassThroughGroupingResult ptg = PassThroughGroupingResult;
            group.Details.BeginInit();
            string[] groupByColumns = ptg.GroupByColumns;
            int level = group.ChildTableGroupLevel + 1;
            if (group is ChildTable)
            {
                level = 0;
            }

            bool shouldGroupHaveRecords = groupByColumns.Length - 1 == level;
            GroupsDetails detailSectionWithGroups = (GroupsDetails)group.Details;
            foreach (object item in items)
            {
                Group groupByNode;
                if (!shouldGroupHaveRecords)
                {
                    groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.GroupedColumns[0].GetShadowedCopy());
                }
                else
                {
                    groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.SortedColumns.GetShadowedCopy());
                }

                if (shouldGroupHaveRecords)
                {
                    ((RecordsDetails)groupByNode.Details).RecordTreeEntries.Comparer = recordComparer;
                }

                GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
                childNodeEntry.Tree = detailSectionWithGroups.categoryTreeTable.TreeTable;
                childNodeEntry.Element = groupByNode;
                groupByNode.GroupCategoryEntry = childNodeEntry;
                groupByNode.ParentElement = detailSectionWithGroups;
                detailSectionWithGroups.GroupCategoryTreeTable.Add(childNodeEntry);

                //// Get the category iten that identifies this group. Use groupByNode.Level to determine
                //// which column is used for this group...
                //// groupByNode.CategoryKeys = GetGroupByCategoryKeys(GroupByCategorizer, group, record);
                groupByNode.CategoryKeys = new object[] { ptg.GetGroupByKey(groupByColumns[level], item) };
                groupByNode.PassThroughItem = item;

                //// GetCategoryForeignKeyParentIds depends on record which I do not have here. This can 
                //// therefore not be supported for now.
                ////bool foreignKeyFieldsFound;
                ////object[] categoryForeignKeyParentIds = GetCategoryForeignKeyParentIds(GroupByCategorizer, group, record, out foreignKeyFieldsFound);
                ////if (foreignKeyFieldsFound)
                ////    groupByNode.CategoryForeignKeyParentIds = categoryForeignKeyParentIds;
            }

            group.Details.EndInit();
        }

        /// <summary>
        /// Checks whether the record has nested tables that have not been 
        /// populated. If this is the case the method will retrieve the nested collection items
        /// from the property (if UniformChildList) or from the IPassThroughGroupingResult.GetNestedItems
        /// method, create the child list, set its SourceList and add entries to the child lists details
        /// section.
        /// </summary>
        /// <param name="record">The Record.</param>
        public void PopulateRecordChildTablesIfEmpty(Record record)
        {
            Table table = record.ParentTable;

            // Populate nested childtable on demand when SourceList
            // is a IPassThroughGroupingResult
            int relCount = 0;
            if (!(record is AddNewRecord))
            {
                relCount = table.RelatedTables.Count;  // Note: also includes foreign key relations!
            }

            for (int i = 0; i < relCount; i++)
            {
                Table relatedTable = table.RelatedTables[i];
                if (relatedTable.IsPassThroughGrouping || relatedTable.IsLazyUniformChildListRelation())
                {
                    NestedTable nt = record.NestedTables[i];

                    // If child table is not populated then create and initialize it.
                    if (nt.ChildTable == null)
                    {
                        RelationDescriptor rd = table.ParentTableDescriptor.Relations[i];
                        ChildTable childTable = record.GetRelatedChildTable(rd);
                        nt.ChildTable = childTable;

                        if (rd.RelationKind == RelationKind.UniformChildList)
                        {
                            // this.cacheRecordData = true;
                            PropertyDescriptor pd = table.TableDescriptor.ItemProperties[rd.MappingName];
                            // Call back to let query execute and get nested items.
                            IEnumerable items = (IEnumerable)pd.GetValue(record.GetData());

                            relatedTable.PopulatePassThroughGroup(childTable, items, record);
                        }
                        else
                        {
                            IPassThroughGroupingResult ptg = relatedTable.PassThroughGroupingResult;

                            // Call back to let query execute and get nested items.
                            object totals;
                            IEnumerable items = ptg.GetNestedItems(rd, childTable, childTable.CategoryKeys, out totals);
                            childTable.PassThroughItem = totals;

                            relatedTable.PopulatePassThroughGroup(childTable, items, record);
                        }

                        relatedTable.OnPopulatedChildTable(childTable);
                    }
                }
            }
        }

        protected internal virtual void OnPopulatedChildTable(ChildTable childTable)
        {
        }

        /// <summary>
        /// Populates groups on demand.
        /// </summary>
        /// <param name="group">The Group.</param>
        public void PopulatePassThroughGroupIfEmpty(Group group)
        {
            if (group.SourceList != null || group.Details.GetChildCount() > 0 || ptg == null)
            {
                return;
            }

            IEnumerable items = ptg.GetItems(group, group.PassThroughItem);
            PopulatePassThroughGroup(group, items, null);
        }

        /// <summary>
        /// Populates groups on demand.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <param name="items">Items for a given group.</param>
        /// <param name="parentRecord">Parent record.</param>
        public void PopulatePassThroughGroup(Group group, IEnumerable items, Record parentRecord)
        {
            if (items == null)
            {
                return;
            }

            IList list = GetIListWrapper(items);
            if (list != null)
            {
                group.SourceList = list;

                if (group.Details is RecordsDetails || IsNewUniformChildListRelation())
                {
                    UnsortedRecordsTreeEntry firstUnsortedEntry;
                    ArrayList records;

                    // Create UnsortedRecordEntry for each item, append to UnsortedRecordsList
                    GenerateUnsortedRecords(list, parentRecord, out records, out firstUnsortedEntry);

                    // Sort records
                    records.Sort(new TableRecordDataComparer(this, Comparer));

                    // Now categorize records and add them to group
                    InitGroups(group, records, false);

                    group.InvalidateSummariesTopDown();
                    group.UnsortedEntry = firstUnsortedEntry;
                }
                else
                {
                    AddPassThroughGroupsWithoutDetails(group, list);
                }
            }
        }

        private void GenerateUnsortedRecords(IEnumerable list, Record parentRecord, out ArrayList records, out UnsortedRecordsTreeEntry firstUnsortedEntry)
        {
            firstUnsortedEntry = null;
            records = new ArrayList();

            bool first = true;
            int n = unsortedRecordsTree.Count;

            bool isSorted;
            SortColumnDescriptor[] arrayOfColumnDescriptors;
            PropertyDescriptor[] arrayOfPropertyDescriptor;
            this.TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

            SortColumnDescriptor[] arrayOfPKColumnDescriptors;
            PropertyDescriptor[] arrayOfPKPropertyDescriptor;
            bool isPKSorted;
            TableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);

            foreach (object item in list)
            {
                UnsortedRecordsTreeEntry unsortedEntry = new UnsortedRecordsTreeEntry();
                Record record = TableDescriptor.CreateRecord(this);
                record.UnsortedEntry = unsortedEntry;

                record.sourceIndex = n;  // prevent data from being set to null.
                record.sourceListVersion = this.SourceListVersion;

                ////this.cacheRecordData = true;
                record.Parent = parentRecord; //// must be set before UpdateSortInfo is called!
                record.SetData(item, isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);

                unsortedEntry.Record = record;
                unsortedEntry.Tree = unsortedRecordsTree.TreeTable;
                unsortedRecordsTree.Add(unsortedEntry);
                n++;

                if (first)
                {
                    firstUnsortedEntry = unsortedEntry;
                    first = false;
                }

                records.Add(record);
            }
        }

        internal IList GetIListWrapper(IEnumerable list)
        {
            if (list is IList)
            {
                return (IList)list;
            }

            if (list == null)
                return null;
            IEnumerator enumerator = list.GetEnumerator();
            if (enumerator.MoveNext())
            {
                ArrayList al = new ArrayList();
                do
                {
                    al.Add(enumerator.Current);
                }
                while (enumerator.MoveNext());
                return al;
            }

            return null;
        }

        RecordDataComparer recordComparer;

        Group BeginGroups(Group startGroup, Record record)
        {
            if (startGroup.Details.HasRecords)
            {
                return startGroup;
            }

            ////int foreignKeyLevelCount = td.RelationChildColumns.Count > 0 ? 1 : 0;
            ////int groupLevelCount = foreignKeyLevelCount + this.TableDescriptor.GroupedColumns.Count;
            TableDescriptor td = ParentTableDescriptor;
            GroupsDetails detailSectionWithGroups = (GroupsDetails)startGroup.Details;
            Group group = startGroup;
            int currentLevel = startGroup.ChildTableGroupLevel;
            while (++currentLevel < groupLevelCount)
            {
                Group groupByNode;
                bool shouldGroupHaveRecords = currentLevel == groupLevelCount - 1;
                if (group.ParentElement == this && TableDescriptor.RelationChildColumns.Count > 0)
                {
                    groupByNode = TableDescriptor.CreateChildTable(detailSectionWithGroups, shouldGroupHaveRecords, this, td.RelationChildColumns.GetShadowedCopy());
                }
                else
                {
                    if (!shouldGroupHaveRecords)
                    {
                        groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.GroupedColumns[currentLevel].GetShadowedCopy());
                    }
                    else
                    {
                        groupByNode = TableDescriptor.CreateGroup(detailSectionWithGroups, shouldGroupHaveRecords, this, TableDescriptor.SortedColumns.GetShadowedCopy());
                    }
                    ////                    Console.WriteLine(GroupedColumns[group.GroupLevel+1].Name);
                }

                if (shouldGroupHaveRecords)
                {
                    ((RecordsDetails)groupByNode.Details).RecordTreeEntries.Comparer = recordComparer;
                }

                groupByNode.Details.BeginInit();
                GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
                childNodeEntry.Tree = detailSectionWithGroups.categoryTreeTable.TreeTable;
                childNodeEntry.Element = groupByNode;
                groupByNode.GroupCategoryEntry = childNodeEntry;
                groupByNode.ParentElement = detailSectionWithGroups;
                detailSectionWithGroups.GroupCategoryTreeTable.Add(childNodeEntry);

                //// Get the category iten that identifies this group. Use groupByNode.Level to determine
                //// which column is used for this group...
                groupByNode.CategoryKeys = GetGroupByCategoryKeys(GroupByCategorizer, group, record);
                bool foreignKeyFieldsFound;
                object[] categoryForeignKeyParentIds = GetCategoryForeignKeyParentIds(GroupByCategorizer, group, record, out foreignKeyFieldsFound);
                if (foreignKeyFieldsFound)
                {
                    groupByNode.CategoryForeignKeyParentIds = categoryForeignKeyParentIds;
                }

                detailSectionWithGroups = groupByNode.Details as GroupsDetails;

                group = groupByNode;

                ////Console.WriteLine("BeginGroup " + groupByNode.ToString());
            }

            return group;
        }

        Group EndGroups(Group groupByNode, Group parent)
        {
            Group g = null;
            while (groupByNode != parent)
            {
                ////                Console.WriteLine("EndGroup " + groupByNode.Category);
                ////                groupByNode.Sections.Add(Engine.CreateEmptySection());
                groupByNode.Details.EndInit();
                g = groupByNode;
                groupByNode = groupByNode.ParentGroup;
            }

            return g;
        }

#if USETHRAEDING
        bool allowThreading = false;
        int groupsCounted = 0;
        int sumsCounted = 0;
        int groupsCounter = 0;
        int sumsCounter = 0;
        ArrayList countGroupsList;
        ManualResetEvent startCountSignal;

        /// <internalonly/>
        /// <summary>
        /// Experimental only! Set this to True if you want count to be calculated in a separate thread when all records
        /// are categorized. Set this flag only if you have a real multi-processor system. With single
        /// processor, systems calculating counts in a separate thread slows categorizaton down.
        /// </summary>
        public bool AllowThreading
        {
            get
            {
                return allowThreading;
            }

            set
            {
                allowThreading = value;
            }
        }

        void CounterThreadProc()
        {
            startCountSignal.WaitOne();

            while (true)
            {
                Group g = null;
                if (countGroupsList != null)
                {
                    lock (countGroupsList.SyncRoot)
                    {
                        if (groupsCounter < countGroupsList.Count)
                        {
                            g = (Group)countGroupsList[groupsCounter];
                            groupsCounter++;
                        }
                    }
                }

                if (g != null)
                {
                    try
                    {
                        g.GetCounter();
                        ////if (g.IsTopLevelGroup)
                        g.GetVisibleCount();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Debugger.Break();
                    }
                    finally
                    {
                        Interlocked.Increment(ref groupsCounted);
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        void SummariesThreadProc()
        {
            startCountSignal.WaitOne();

            bool changed;
            while (true)
            {
                Group g = null;
                if (countGroupsList != null)
                {
                    lock (countGroupsList.SyncRoot)
                    {
                        if (sumsCounter < countGroupsList.Count)
                        {
                            g = (Group)countGroupsList[sumsCounter];
                            sumsCounter++;
                        }
                    }
                }

                if (g != null)
                {
                    try
                    {
                        g.GetSummaries(this, out changed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Debugger.Break();
                    }
                    finally
                    {
                        Interlocked.Increment(ref sumsCounted);
                    }
                }
                else
                {
                    Thread.Sleep(25);
                }
            }
        }
#endif

        #endregion
        #region CategoryKeys
        Group CompareCategoryKeys(Group group, Record record)
        {
            Group groupByNode = group;
            Group notEqualCategory = null;
            IGroupByCategorizer groupByCategorizer = this.GroupByCategorizer;
            for (; groupByNode != this.TopLevelGroup; groupByNode = groupByNode.ParentGroup)
            {
                bool equalsCategory = true;
                bool isForeignKeys = groupByNode is ChildTable;
                SortColumnDescriptorCollection columns;
                if (isForeignKeys)
                {
                    columns = ParentTableDescriptor.RelationChildColumns;
                    for (int n = 0; n < columns.Count; n++)
                    {
                        equalsCategory &= 0 == groupByCategorizer.CompareCategoryKey(columns[n], isForeignKeys, groupByNode.CategoryKeys[n], record);
                    }
                }
                else
                {
                    equalsCategory = 0 == groupByCategorizer.CompareCategoryKey(TableDescriptor.GroupedColumns[groupByNode.GroupLevel], false, groupByNode.CategoryKeys[0], record);
                }

                if (!equalsCategory)
                {
                    notEqualCategory = groupByNode; //// this groupByNode has different category
                }
            }

            return notEqualCategory; //// record fits
        }

        object[] GetGroupByCategoryKeys(IGroupByCategorizer groupByCategorizer, Group parentGroup, Record record)
        {
            object[] categoryKeys;
            if (parentGroup.IsMainGroup && TableDescriptor.RelationChildColumns.Count > 0)
            {
                categoryKeys = new Object[TableDescriptor.RelationChildColumns.Count];
                for (int n = 0; n < TableDescriptor.RelationChildColumns.Count; n++)
                {
                    categoryKeys[n] = groupByCategorizer.GetGroupByCategoryKey(TableDescriptor.RelationChildColumns[n], true, record);
                }
            }
            else if (TableDescriptor.GroupedColumns.Count > 0) 
            {
                //// == 0 might indicate a DataTable in a Dataset.
                categoryKeys = new object[1];
                categoryKeys[0] = groupByCategorizer.GetGroupByCategoryKey(TableDescriptor.GroupedColumns[parentGroup.GroupLevel + 1], false, record);
            }
            else
            {
                categoryKeys = new object[0];
            }

            return categoryKeys;
        }

        object[] GetCategoryForeignKeyParentIds(IGroupByCategorizer groupByCategorizer, Group parentGroup, Record record, out bool foreignKeyFieldsFound)
        {
            foreignKeyFieldsFound = false;

            object[] categoryKeys;
            if (parentGroup.IsMainGroup && TableDescriptor.RelationChildColumns.Count > 0)
            {
                bool foreignKeyFieldsFound2;
                categoryKeys = new Object[TableDescriptor.RelationChildColumns.Count];
                for (int n = 0; n < TableDescriptor.RelationChildColumns.Count; n++)
                {
                    categoryKeys[n] = groupByCategorizer.GetCategoryForeignKeyParentIds(TableDescriptor.RelationChildColumns[n], true, record, out foreignKeyFieldsFound2);
                    foreignKeyFieldsFound |= foreignKeyFieldsFound2;
                }
            }
            else if (TableDescriptor.GroupedColumns.Count > 0) 
            {
                //// == 0 might indicate a DataTable in a Dataset.
                categoryKeys = new object[1];
                categoryKeys[0] = groupByCategorizer.GetCategoryForeignKeyParentIds(TableDescriptor.GroupedColumns[parentGroup.GroupLevel + 1], false, record, out foreignKeyFieldsFound);
            }
            else
            {
                categoryKeys = new object[0];
            }

            return categoryKeys;
        }

        IGroupByCategorizer GroupByCategorizer
        {
            get
            {
                return TableDescriptor.GroupByCategorizer;
            }
        }

        IComparer Comparer
        {
            get
            {
                return TableDescriptor.Comparer;
            }
        }

        #endregion
        #region Synchronize Related Tables with TableDescriptor.Relations
        //// shadowed RelationDescriptorCollection
        RelationDescriptorCollection Relations
        {
            get
            {
                if (relationDescriptors == null)
                {
                    relationDescriptors = new RelationDescriptorCollection(null);
                }

                relationDescriptors.Table = this;
                return relationDescriptors;
            }
        }

        bool inSynchronizeRelatedTables = false;

        internal void SynchronizeRelatedTables()
        {
            if (inSynchronizeRelatedTables)
            {
                return;
            }

            inSynchronizeRelatedTables = true;
            bool hooked = false;
            try
            {
                this.EnsureSourceList();

                if (this.IsDisposed)
                {
                    return;
                }

                RelatedTables.relationDescriptorVersion = TableDescriptor.Relations.Version;
                RelatedTables.tableSourceListVersion = SourceListVersion;

                lastRelations_ChangedRelation = null;  //// avoid repeated calling for
                Relations.Table = null;
                ////Relations.ParentTableDescriptor = this.ParentTableDescriptor;
                Relations.Changed += new ListPropertyChangedEventHandler(Relations_Changed);
                hooked = true;
                Relations.InitializeFrom(TableDescriptor.Relations);
                Relations.Changed -= new ListPropertyChangedEventHandler(Relations_Changed);
                hooked = false;
                Relations.Table = this;
                lastRelations_ChangedRelation = null;

                int count = Math.Min(this._relatedTables.Count, Relations.Count);
                while (count > Relations.Count)
                {
                    Console.WriteLine("Related tables were out of sync for " + this.TableDescriptor.Name + ". Removing " + this._relatedTables[this._relatedTables.Count - 1]);
                    this._relatedTables.RemoveAt(this._relatedTables.Count - 1);
                }

                for (int n = 0; n < count; n++)
                {
                    Table relatedTable = (Table)this._relatedTables[n];
                    RelationDescriptor rd = TableDescriptor.Relations[n];
                    if (relatedTable == null || relatedTable.IsDisposed || !Object.ReferenceEquals(relatedTable.TableDescriptor, rd.ChildTableDescriptor))
                    {
                        if (relatedTable != null && !relatedTable.IsDisposed)
                        {
                            relatedTable.Dispose();
                        }

                        this._relatedTables[n] = relatedTable = Engine.CreateTable(rd.ChildTableDescriptor, this);
                    }

                    ////                    if (rd.ChildTableName != "")
                    ////                    {
                    ////                        SourceListSetEntry sourceListEntry = this.Engine.SourceListSet[rd.ChildTableName];
                    ////                        if (sourceListEntry != null)
                    ////                        {
                    ////                            relatedTable.SourceList = sourceListEntry.List;
                    ////                        }
                    ////                    }
                }

                //// TODO: What if order of relations has changed ...
                ////            ArrayList r = new ArrayList();
                ////            foreach (RelationDescriptor rd in Relations)
            }
            finally
            {
                inSynchronizeRelatedTables = false;
                if (hooked)
                {
                    Relations.Changed -= new ListPropertyChangedEventHandler(Relations_Changed);
                }
            }
        }

        Table GetRelatedTable(int n)
        {
            if (n < RelatedTables.Count)
            {
                return (Table)relatedTables[n];
            }

            return null;
        }

        ////        Table GetRelatedTable(RelationDescriptor rd)
        ////        {
        ////            return RelatedTables[rd.Name];
        ////        }

        RelationDescriptor lastRelations_ChangedRelation = null;

        Table CreateRelatedtable(RelationDescriptor rd)
        {
            lastRelations_ChangedRelation = rd;

            Table relatedTable;
            relatedTable = TableDescriptor.CreateRelatedTable(rd.ChildTableDescriptor, this);
            ////            SourceListSetEntry sourceListEntry = this.Engine.SourceListSet[rd.ChildTableName];
            ////            if (sourceListEntry != null)
            ////            {
            ////                relatedTable.SourceList = sourceListEntry.List;
            ////                if (traceRelatedTables) TraceUtil.TraceCurrentMethodInfo(this, relatedTable);
            ////            }
            return relatedTable;
        }

        private void Relations_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            EngineVersion++;
            if (traceRelatedTables)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }

            RelationDescriptor rd = (RelationDescriptor)e.Item;
            Table relatedTable;
            switch (e.Action)
            {
                case ListPropertyChangedType.Add:
                    rd = this.ParentTableDescriptor.Relations[rd.Name];
                    rd.SetCollection(ParentTableDescriptor.Relations);
                    ////if (rd != null)
                    {
                        relatedTable = CreateRelatedtable(rd);
                        relatedTables.Add(relatedTable);
                        OnAddedRelatedTable(new TableEventArgs(relatedTable));
                    }

                    break;

                case ListPropertyChangedType.Insert:
                    rd = this.ParentTableDescriptor.Relations[rd.Name];
                    if (rd != null)
                    {
                        rd.SetCollection(ParentTableDescriptor.Relations);
                        relatedTable = CreateRelatedtable(rd);
                        relatedTables.Insert(e.Index, relatedTable);
                        OnAddedRelatedTable(new TableEventArgs(relatedTable));
                    }

                    break;

                case ListPropertyChangedType.Remove:
                    relatedTable = (Table)relatedTables[e.Index];
                    OnRemovingRelatedTable(new TableEventArgs(relatedTable));
                    relatedTables.RemoveAt(e.Index);
                    relatedTable.Dispose();
                    break;

                case ListPropertyChangedType.ItemPropertyChanged:
                    if (e.Property != "ChildTableDescriptor")
                    {
                        goto case ListPropertyChangedType.ItemChanged;
                    }

                    break;

                case ListPropertyChangedType.ItemChanged:
                    rd = this.ParentTableDescriptor.Relations[rd.Name];
                    if (rd != null && rd != lastRelations_ChangedRelation)
                    {
                        rd.SetCollection(ParentTableDescriptor.Relations);
                        int indx = e.Index;
                        ////                    if (e.Index == -1)
                        ////                        indx = relatedTables.IndexOf(e.Item);
                        if (indx != -1 && indx < relatedTables.Count)
                        {
                            relatedTable = (Table)relatedTables[indx];
                            OnRemovingRelatedTable(new TableEventArgs(relatedTable));
                            if (relatedTable != null)
                            {
                                relatedTable.Dispose();
                            }

                            relatedTable = CreateRelatedtable(rd);
                            relatedTables[indx] = relatedTable;
                            OnAddedRelatedTable(new TableEventArgs(relatedTable));
                        }
                    }

                    break;

                case ListPropertyChangedType.Refresh:
                    //// that case should not happen ...
                    Debug.Assert(false);
                    ////                    this.relatedTables.Clear();
                    ////                    for (int n = 0; n < this.ParentTableDescriptor.relatedTables.Count; n++)
                    ////                    {
                    ////                        rd = (RelationDescriptor) this.relatedTables[n];
                    ////                        relatedTable = CreateRelatedtable(rd);
                    ////                        relatedTables.Add(relatedTable);
                    ////                    }
                    break;
            }
        }

        private void relatedTable_Disposed(object sender, EventArgs e)
        {
            Table table = (Table)sender;
            table.Disposed -= new EventHandler(relatedTable_Disposed);

            if (relatedTables == null)
            {
                return;
            }

            int index = relatedTables.IndexOf(table);
            OnRemovingRelatedTable(new TableEventArgs(table));
            if (index != -1)
            {
                relatedTables[index] = null;
            }
        }

        #endregion
        #region Related Tables Collection and Events

        /// <summary>
        /// Occurs after the related table was removed from the <see cref="RelatedTables"/> collection.
        /// </summary>
        [Browsable(false)]
        public event TableEventHandler RemovingRelatedTable;

        /// <summary>
        /// Raises the  <see cref="RemovingRelatedTable"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnRemovingRelatedTable(TableEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            e.Table.Disposed -= new EventHandler(relatedTable_Disposed);

            if (RemovingRelatedTable != null)
            {
                RemovingRelatedTable(this, e);
            }
        }

        /// <summary>
        /// Occurs after the related table was added to the <see cref="RelatedTables"/> collection.
        /// </summary>
        [Browsable(false)]
        public event TableEventHandler AddedRelatedTable;

        /// <summary>
        /// Raises the  <see cref="AddedRelatedTable"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnAddedRelatedTable(TableEventArgs e)
        {
            if (traceRelatedTables)
            {
                TraceUtil.TraceCurrentMethodInfo(this, e);
            }

            e.Table.Disposed += new EventHandler(relatedTable_Disposed);

            if (AddedRelatedTable != null)
            {
                AddedRelatedTable(this, e);
            }
        }

        ////internal TableCollection _foreignKeyTables;
        
        /// <summary>
        /// Gets a collection of related tables based on the TableDescriptor.Relations
        /// defined for this table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableCollection RelatedTables
        {
            get
            {
                if (_relatedTables == null)
                {
                    ////TraceUtil.TraceCurrentMethodInfo();
                    ////this.EnsureInitialized(this);
                    ////this.SynchronizeRelatedTables();
                    _relatedTables = new TableCollection(this, relatedTables);
                }

                ////int c = _relatedTables.Count;
                return _relatedTables;
            }
        }

        #endregion
        #region DisplayElementChanging
        /// <summary>
        /// When number of visible elements are changing.
        /// </summary>
        [Browsable(false)]
        public event DisplayElementChangingEventHandler DisplayElementChanging;

        /// <summary>
        /// When number of visible elements were changed.
        /// </summary>
        [Browsable(false)]
        public event DisplayElementChangedEventHandler DisplayElementChanged;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDisplayElementChanging(e);
            }

            if (DisplayElementChanging != null)
            {
                DisplayElementChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDisplayElementChanged(e);
            }

            if (DisplayElementChanged != null)
            {
                DisplayElementChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        /// <param name="scroll">Indicates if current record should be scrolled into view.</param>
        /// <returns>true if the event should be canceled; otherwise, false</returns>
        public bool RaiseDisplayElementChanging(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord, bool scroll)
        {
            ////fix - commented out EngineVersion++. At the time DisplayElementChanging is raised the engine
            ////should not have been touched yet.
            ////EngineVersion++;
            DisplayElementChangingEventArgs e = new DisplayElementChangingEventArgs(element, oldCount, newCount, repaintElement, syncCurrentRecordPos, leaveCurrentRecord, scroll);
            this.OnDisplayElementChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        /// <param name="scroll">Indicates if current record should be scrolled into view.</param>
        public void RaiseDisplayElementChanged(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord, bool scroll)
        {
            EngineVersion++;
            DisplayElementChangedEventArgs e = new DisplayElementChangedEventArgs(element, oldCount, newCount, repaintElement, syncCurrentRecordPos, leaveCurrentRecord, scroll);
            this.OnDisplayElementChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        /// <returns>true if the event should be canceled; otherwise, false</returns>
        public bool RaiseDisplayElementChanging(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord)
        {
            return RaiseDisplayElementChanging(element, oldCount, newCount, repaintElement, syncCurrentRecordPos, leaveCurrentRecord, true);
        }

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        public void RaiseDisplayElementChanged(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord)
        {
            RaiseDisplayElementChanged(element, oldCount, newCount, repaintElement, syncCurrentRecordPos, leaveCurrentRecord, true);
        }
        #endregion
        #region Changing Event Handlers

        bool inCurrentRecordEndEditOrLeave = false;

        bool CurrentRecordEndEditOrLeave()
        {
            if (this.CurrentElement != null && this.CurrentRecordManager.IsEditing)
            {
                inCurrentRecordEndEditOrLeave = true;
                try
                {
                    TraceUtil.TraceCalledFromIf(Switches.CurrentRecord.TraceVerbose, 10, this, CurrentRecordManager);
                    this.CurrentRecordManager.EndEdit();
                    if (this.CurrentRecordManager.IsEditing)
                    {
                        return false;
                    }
                }
                finally
                {
                    inCurrentRecordEndEditOrLeave = false;
                }
            }

            return true;
        }

        /// <exclude/>
        protected virtual void Engine_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(e);

            if (e.PropertyName == "TableDescriptor")
            {
                TableDescriptor td = Engine.TableDescriptor; 
                //// No need to repaint (and close dropdowns) when user sorts a foreign key table.
                DescriptorPropertyChangedEventArgs inner = ((DescriptorPropertyChangedEventArgs)e.Inner).GetNestedChildTableDescriptorEvent(ref td);
                if (td != null)
                {
                    if (!td.IsChildOf(this.TableDescriptor))
                    {
                        return;
                    }

                    RelationDescriptor rd = td.ParentRelation;
                    if (rd.RelationKind == RelationKind.ForeignKeyReference
                        || rd.RelationKind == RelationKind.ListItemReference
                        || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                    {
                        return;        //// ForeignListItems
                    }
                }
            }

            this.EngineTable.RaiseDisplayElementChanging(this, -1, -1, false, true, false, false);
            RaiseDisplayElementChanging(this, -1, -1, true, true, false, false);
            CurrentRecordEndEditOrLeave();
        }

        /// <exclude/>
        protected virtual void Engine_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CounterLogic" || e.PropertyName == "AllowedOptimization")
            {
                TableDirty = true;

                // also, make sure UnsortedRecords.RecordParts are recreated.
                sourceListSortArray = null;
                allowRebuildfromUnsortedRecordsTree = false;
            }

            if (e.PropertyName == "TableDescriptor")
            {
                TableDescriptor tableDescriptor = Engine.TableDescriptor;
                e = (DescriptorPropertyChangedEventArgs)e.Inner;

                if (e.PropertyName == "RecordFilters")
                {
                    if (tableDescriptor != this.TableDescriptor
                        && !tableDescriptor.IsChildOf(this.TableDescriptor))
                    {
                        return;
                    }

                    CountersDirty = true;
                    this.SummariesDirty = true;
                }
            }
        }

        ////        private void Engine_SourceListChanging(object sender, CancelEventArgs e)
        ////        {
        ////            if (CurrentRecordManager.IsEditing)
        ////                CurrentRecordManager.CancelEdit();
        ////
        ////            this.RaiseDisplayElementChanging(this, -1, -1, true, true, true);
        ////        }

        #endregion
        #region Changed Event Handlers
        private void Fields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.CountersDirty = true;
            //// If Fields are used for relation or grouping setting TableDirty would be better,
            //// but in such cases the TableDescriptor_ItemPropertiesChanged will take care on this.
            //// In other cases just setting CountersDirty is more efficient.
        }

        private void ExpressionFields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.CountersDirty = true;
            //// See also comment in Fields_Changed about TableDirty
        }

        private void TableDescriptor_RowsPerRecordChanged(object sender, EventArgs e)
        {
            this.CountersDirty = true;
        }

        private void TableDescriptor_ItemPropertiesChanged(object sender, EventArgs e)
        {
            this.isDirty = true;
        }

        private void RecordFilters_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this._isRecordFiltersDirty = true;
        }

        private void RecordFilters_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (sender == this.TableDescriptor.RecordFilters)
            {
                this._isRecordFiltersDirty = true;
            }
        }

        private void SummaryDescriptors_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (sender == this.TableDescriptor.Summaries)
            {
                if (!this._isSummaryDirty)
                {
                    if (e.Action == ListPropertyChangedType.ItemPropertyChanged)
                    {
                        if (e.Property != "Name")
                        {
                            this._isSummaryDirty = true;
                        }
                    }
                    else
                    {
                        this._isSummaryDirty = true;
                    }

                    this.ClearCollectionCaches();
                }
            }
        }

        private void RelationChildColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (sender == this.TableDescriptor.RelationChildColumns)
            {
                Table table = this;
                while (table != null)
                {
                    table.isDirty = true;
                    table = table.RelationParentTable;
                }
            }
        }

        private void GroupedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (sender == this.TableDescriptor.GroupedColumns)
            {
                bool dirty = false;
                IGroupingList gl = SourceList as IGroupingList;
                if (gl != null && gl.GroupingSortBehavior == GroupingSortBehavior.Table)
                {
                    dirty = true;
                }
                else if (e.Action == ListPropertyChangedType.ItemPropertyChanged)
                {
                    //// if only sort order is affected no need to completely refresh the table.
                    //// || e.Property == "Comparer")
                    if (e.Property == "Name")
                    {
                        dirty = true;
                    }

                    //// TODO: optimize behavior when only SortDirecting has changed - just have to walk the elements once then for each group ...
                    if (e.Property == "SortDirection")
                    {
                        return;
                    }
                }
                else
                {
                    dirty = true;
                }

                Table relationParentTable = RelationParentTable;
                while (relationParentTable != null)
                {
                    relationParentTable.isDirty |= dirty;
                    if (!relationParentTable.InInitialize)
                    {
                        relationParentTable.InvalidateCounterTopDown(true);
                    }

                    relationParentTable = relationParentTable.RelationParentTable;
                }

                this.isDirty |= dirty;
                this.CountersDirty = true;
            }
        }

        private void SortedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (sender == this.TableDescriptor.SortedColumns)
            {
                IGroupingList gl = SourceList as IGroupingList;
                if (gl != null && gl.GroupingSortBehavior == GroupingSortBehavior.Table)
                {
                    if (e.Action == ListPropertyChangedType.ItemPropertyChanged)
                    {
                        // if only sort order is affected no need to completely refresh the table.
                        if (e.Property == "ColumnName" || e.Property == "Comparer")
                        {
                            this.isDirty = true;
                        }
                    }
                }

                this.ClearCollectionCaches();
            }
        }

        private void TableDescriptor_AllowNewChanged(object sender, EventArgs e)
        {
            if (this.sourceList != null && this._topLevelGroup != null)
            {
                this.CountersDirty = true;
            }
            else
            {
                this.ClearCollectionCaches();
            }
        }

        private void Engine_SourceListChanged(object sender, EventArgs e)
        {
            this._relatedTables = null;
            this.isDirty = true;
            SourceListVersion++;
        }

        #endregion
        #region DEBUG
        [Conditional("DEBUG")]
        void DumpDisplayElementCount()
        {
#if DEBUG
            if (!Switches.GroupingEngine.TraceVerbose)
            {
                return;
            }
#endif
            int recnum = -1;
            Table pt = this.ParentTable;
            if (pt != null)
            {
                recnum = pt.Records.IndexOf(this.ParentRecord);
            }

            Trace.WriteLine(String.Format("{0} Record, {1} Display Elements, 2 Source List", recnum, this.DisplayElements.Count));
            Trace.Indent();

            int total = 0;
            foreach (Record r in this.Records)
            {
                total += r.GetVisibleCount();
            }

            Trace.Unindent();
            Trace.WriteLine(String.Format("Total {0} Display Elements", total));
        }

        #endregion
        #region CustomCount

        // event CustomCount QueryCustomCount

        /// <summary>
        /// Occurs when the custom counter value for a record is queried. See the Grid\Grouping\Samples\CustomSummary
        /// example.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the custom counter value for a record is queried.")]
        public event CustomCountEventHandler QueryCustomCount;

        /// <summary>
        /// Raises the <see cref="QueryCustomCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CustomCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCustomCount(CustomCountEventArgs e)
        {
            if (QueryCustomCount != null)
            {
                QueryCustomCount(this, e);
            }
        }

        internal void RaiseQueryCustomCount(CustomCountEventArgs e)
        {
            OnQueryCustomCount(e);
        }

        // event CustomCount QueryVisibleCustomCount

        /// <summary>
        /// Occurs when the visible custom counter value for a record is queried. See the Grid\Grouping\Samples\CustomSummary
        /// example.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the visible custom counter value for a record is queried.")]
        public event CustomCountEventHandler QueryVisibleCustomCount;

        /// <summary>
        /// Raises the <see cref="QueryVisibleCustomCount"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CustomCountEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryVisibleCustomCount(CustomCountEventArgs e)
        {
            if (QueryVisibleCustomCount != null)
            {
                QueryVisibleCustomCount(this, e);
            }
        }

        internal void RaiseQueryVisibleCustomCount(CustomCountEventArgs e)
        {
            OnQueryVisibleCustomCount(e);
        }

        #endregion
        #region RelatedTableSourceListListChanged
        /// <summary>
        /// This virtual method is called before a related child or grandchild table raises a <see cref="SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// The method calls the <see cref="SourceListVersion"/> of this table.
        /// </remarks>
        public virtual void OnRelatedTableSourceListListChanged(TableListChangedEventArgs e)
        {
            this.SourceListVersion++;
            if (this.ParentTable != null)
            {
                this.ParentTable.OnRelatedTableSourceListListChanged(e);
            }
        }

        /// <summary>
        /// This virtual method is called before a related child or grandchild table raises a <see cref="SourceListListChangedCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// The method calls the <see cref="SourceListVersion"/> of this table.
        /// </remarks>
        public virtual void OnRelatedTableSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (this.ParentTable != null)
            {
                this.ParentTable.OnRelatedTableSourceListListChangedCompleted(e);
            }
        }
        #endregion
        #region ColumnChanging and RowRemoving Event Support
        //// Allow this table to listen to DataTable.ColumnChanging events. When
        //// a ColumnChanging event is handled the table will automatically add ChangedFieldInfo
        //// objects with information about the new and old value of the column. The
        //// ChangedFieldInfo objects will then be checked in the ListChanged event handler.

        IGroupingColumnChanging igcc;
        DataTable dt;
        ////#if SyncfusionFramework2_0
        ////        Record lastColumnChangedRecord = null;
        ////#endif

        /// <summary>
        /// Raises the <see cref="SourceListChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListChanged(TableEventArgs e)
        {
            UnwireDataTable(dt);
            UnwireGroupingColumnChanging(igcc);

            dt = null;

            // Check first if source implement IGroupingColumnChanging interface
            igcc = SourceList as IGroupingColumnChanging;
            if (igcc != null)
            {
                WireGroupingColumnChanging(igcc);
            }
            else
            {
                // Otherwise check if it is a DataTable.
                dt = GetDataTable(this.SourceList);
                WireDataTable(dt);
            }

            // Temporary enable auto-population 
            EnableOneTimePopulate();

            if (GetDataTable(this.SourceList) != null)
            {
                this.dataSourceRaisesTwoItemAddedEvents = true;
            }

            BaseSourceListChanged(e);
        }

        void WireGroupingColumnChanging(IGroupingColumnChanging igcc)
        {
            if (igcc == null)
            {
                return;
            }

            igcc.ColumnChanging += new GroupingColumnChangeEventHandler(igcc_ColumnChanging);
            igcc.RowRemoving += new GroupingRowEventHandler(igcc_RowRemoving);
        }

        void UnwireGroupingColumnChanging(IGroupingColumnChanging igcc)
        {
            if (igcc == null)
            {
                return;
            }

            igcc.ColumnChanging -= new GroupingColumnChangeEventHandler(igcc_ColumnChanging);
            igcc.RowRemoving -= new GroupingRowEventHandler(igcc_RowRemoving);
        }

        void WireDataTable(DataTable dt)
        {
            if (dt == null)
            {
                return;
            }

            dt.ColumnChanging += new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowDeleting += new DataRowChangeEventHandler(dt_RowDeleting);
        }

        void UnwireDataTable(DataTable dt)
        {
            if (dt == null)
            {
                return;
            }

            dt.ColumnChanging -= new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowDeleting -= new DataRowChangeEventHandler(dt_RowDeleting);
        }

        DataTable GetDataTable(object datasource)
        {
            if (datasource is DataTable)
            {
                return (DataTable)datasource;
            }
            else if (datasource is DataTableList)
            {
                return ((DataTableList)datasource).DataTable;
            }
            else if (datasource is DataView)
            {
                return ((DataView)datasource).Table;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the table has subscribed to DataTable.ColumnChanging
        /// or IGroupingColumnChanging events of the datasource.
        /// </summary>
        public bool HasColumnChangeListener
        {
            get
            {
                return dt != null || igcc != null;
            }
        }

        private void dt_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            if (TableDirty)
            {
                return;
            }

            string name = e.Column.ColumnName;
            ////#if SyncfusionFramework2_0
            ////            int newIndex = e.Row.Table.Rows.IndexOf(e.Row);
            ////            if (newIndex != -1 && lastColumnChangedRecord == null)
            ////                lastColumnChangedRecord = this.UnsortedRecords[newIndex];
            ////#endif
            ChangedFieldInfo ci = new ChangedFieldInfo(this.TableDescriptor, name, e.Row[e.Column], e.ProposedValue);
            AddChangedField(ci);
        }

        void igcc_ColumnChanging(object sender, GroupingColumnChangeEventArgs e)
        {
            if (TableDirty)
            {
                return;
            }

            string name = e.Column;
            ChangedFieldInfo ci = new ChangedFieldInfo(this.TableDescriptor, name, e.OldValue, e.ProposedValue);
            AddChangedField(ci);
        }

        void igcc_RowRemoving(object sender, GroupingRowEventArgs e)
        {
            PrepareRemoving(e.Row);
        }

        protected virtual void dt_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            PrepareRemoving(e.Row);
        }

        void PrepareRemoving(object row)
        {
            if (TableDirty)
            {
                return;
            }

            OnPrepareRemoving(row);
        }

        /// <summary>
        /// This virtual method gets called before a row is removed from
        /// the underlying datasource.
        /// </summary>
        /// <param name="row">The row value</param>
        protected virtual void OnPrepareRemoving(object row)
        {
            //// Save values for all fields where we need to be able to access the
            //// old value (e.g. Delta for TotalSummaries).
            ////TableDescriptor td = TableDescriptor;
            ////IManualTotalSummaryArraySource tsa = this.TopLevelGroup as IManualTotalSummaryArraySource;
            ////if (tsa != null)
            ////{
            ////    foreach (string name in this.totalSummaries)
            ////    {
            ////        FieldDescriptor fd = td.Fields[name];
            ////        if (fd.IsPropertyField())
            ////        {
            ////            PropertyDescriptor pd = fd.GetPropertyDescriptor();
            ////            object value = GetValue(row, pd);

            ////            ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name, value, null);
            ////            this.AddChangedField(ci);
            ////        }
            ////    }
            ////}
            ////foreach (int index in this.summaryColumnFields.Keys)
            ////{
            ////    FieldDescriptor fd = td.Fields[index];
            ////    if (fd.IsPropertyField())
            ////    {
            ////        PropertyDescriptor pd = fd.GetPropertyDescriptor();
            ////        object value = GetValue(row, pd);

            ////        ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name, value, null);
            ////        this.AddChangedField(ci);
            ////    }
            ////}
        }

        void PrepareItemAdded(object row)
        {
            if (TableDirty)
            {
                return;
            }

            OnPrepareItemAdded(row);
        }

        /// <summary>
        /// This virtual method is called when a row was added in the underlying datasource.
        /// </summary>
        /// <param name="row">The row value</param>
        protected virtual void OnPrepareItemAdded(object row)
        {
            //// Get new values for which delta information is needed
            ////IManualTotalSummaryArraySource tsa = this.TopLevelGroup as IManualTotalSummaryArraySource;
            ////if (tsa != null)
            ////{
            ////    TableDescriptor td = TableDescriptor;

            ////    foreach (string name in this.totalSummaries)
            ////    {
            ////        FieldDescriptor fd = td.Fields[name];
            ////        if (fd.IsPropertyField())
            ////        {
            ////            PropertyDescriptor pd = fd.GetPropertyDescriptor();
            ////            object value = GetValue(row, pd);

            ////            ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name, null, value);
            ////            this.AddChangedField(ci);
            ////        }
            ////    }
            ////}
            TableDescriptor td = TableDescriptor;
            foreach (FieldDescriptor fd in td.Fields)
            {
                if (fd.IsPropertyField())
                {
                    PropertyDescriptor pd = fd.GetPropertyDescriptor();
                    //object value = GetValue(row, pd);
                    //ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name, null, value);

                    // Fix: SD 3729 GridGroupingControl binded to IList datasource not getting updated on adding record at runtime
                    ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name);
                    this.AddChangedField(ci);
                }
            }
        }

        /// <summary>
        /// A helper method that calls pd.GetValue(row) or gets the value directly
        /// from a DataRow using its name.
        /// </summary>
        /// <param name="row">The Data row.</param>
        /// <param name="pd">The Property descriptor.</param>
        /// <returns>returns Value.</returns>
        public static object GetValue(object row, PropertyDescriptor pd)
        {
            if (row is DataRow)
            {
                // PropertyDescriptor is for the DataRowView - but here
                // we only have access to the DataRow.
                return ((DataRow)row)[pd.Name];
            }
            else
            {
                return pd.GetValue(row);
            }
        }

        #endregion
        #region Changed Fields Optimization
        internal int listChangedRecordIndex;
        internal Hashtable changedFields = new Hashtable();
        ChangedFieldInfoCollection changedFieldsArray = new ChangedFieldInfoCollection();

        Hashtable last_changedFields;
        ChangedFieldInfoCollection last_changedFieldsArray;
        int last_ChangedIndex = -1;

        /// <summary>
        /// The Collection with detected changes in the datasource when a ListChanged event
        /// is handled.
        /// </summary>
        public ChangedFieldInfoCollection ChangedFieldsArray
        {
            get
            {
                return changedFieldsArray;
            }
        }

        /// <summary>
        /// Call this method to add ChangedFieldInfo
        /// objects with information about the new and old value of the column. The
        /// ChangedFieldInfo objects will then be checked in the LIstChanged event handler.
        /// </summary>
        /// <param name="ci">The changed field information.</param>
        public void AddChangedField(ChangedFieldInfo ci)
        {
            if (TableDirty)
            {
                return;
            }

            if (!changedFields.Contains(ci.FieldIndex))
            {
                changedFields[ci.FieldIndex] = ci;
                changedFieldsArray.Add(ci);

                TableDescriptor td = TableDescriptor;
                foreach (int dependantIndex in GetDependantFields(ci.FieldIndex))
                {
                    if (!changedFields.Contains(dependantIndex))
                    {
                        AddChangedField(new ChangedFieldInfo(td, td.Fields[dependantIndex].Name));
                    }
                }
            }
            else
            {
                // Combine ChangeField
                ChangedFieldInfo ci0 = (ChangedFieldInfo)changedFields[ci.FieldIndex];
                ci0.NewValue = ci.NewValue;
            }
        }

        /// <summary>
        /// The Collection with detected changes in the datasource when a ListChanged event
        /// is handled.
        /// </summary>
        /// <returns>A collection of changed fields.</returns>
        public ChangedFieldInfoCollection GetChangedFields()
        {
            return changedFieldsArray;
        }
        #endregion
        #region Optimize EnsureInitialized

        bool nextTimeEnsureInitialized = false;
        bool skipEnsureInitialized = false;

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            EnableOneTimePopulate();
        }

        private void TableDescriptor_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            EnableOneTimePopulate();
        }

        internal void EnableOneTimePopulate()
        {
            sortFields = null;
            pksortFields = null;

            // Collections check repeatedly whether their version still matches
            // the engine version and the version of base collections (e.g. VisibleColumns checks Columns)
            // This check is really only needed after we know that the schema was changed,
            // the following code will force the check being done only once again.
            if (TableDescriptor != null)
            {
                this.TableDescriptor.EnableOneTimePopulate();
                OnEnableOneTimePopulate();
                nextTimeEnsureInitialized = true;
                skipEnsureInitialized = false;
            }
        }

        /// <exclude/>
        protected virtual void OnEnableOneTimePopulate()
        {
        }

        bool lockOutEnsureInitialized = false;

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public bool LockOutEnsureInitialized
        {
            get
            {
                return lockOutEnsureInitialized;
            }

            set
            {
                lockOutEnsureInitialized = value;
            }
        }

        /// <override/>
        /// <summary>
        /// Ensures the object, nested objects, and parent elements reflect any changes made to the engine or table descriptor. This is
        /// an integral part of the engine's "on-demand execution" of schema changes. Before elements
        /// in the engine are accessed, they call <see cref="EnsureInitialized"/>. If changes were
        /// previously made that affect the queried element, all changes will be applied at this time.
        /// </summary>
        /// <param name="sender">The object that triggered the call.</param>
        /// <param name="notifyParent">Specifes if the parent elements <see cref="EnsureInitialized"/>
        /// should also be called.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        public override bool EnsureInitialized(object sender, bool notifyParent)
        {
            Table engineTable = Engine.Table;
            if (engineTable.LockOutEnsureInitialized)
            {
                return false;
            }

            if (SummariesDirty || CountersDirty || TableDirty)
            {
                return BaseEnsureInitialized(sender, notifyParent);
            }

            if (this.inListChanged)
            {
                return false;
            }

            // If there was no change to the schema make EnsureInitialized return right 
            // away to avoid redundant calls.
            if (nextTimeEnsureInitialized)
            {
                skipEnsureInitialized = true;
            }
            else if (skipEnsureInitialized)
            {
                return false;
            }

            nextTimeEnsureInitialized = false;

            return BaseEnsureInitialized(sender, notifyParent);
        }
        #endregion
        #region NewBindingList_ListChanged

        bool inListChanged = false;

        GroupingListChangedEvents allowListChangedEvents =
            GroupingListChangedEvents.None
            | GroupingListChangedEvents.SourceListListChanged
            | GroupingListChangedEvents.SourceListListChangedCompleted
            | GroupingListChangedEvents.SourceListRecordChanging
            | GroupingListChangedEvents.SourceListRecordChanged;

        void NotifyCurrentRecordListChanged(TableListChangedEventArgs te)
        {
            CurrentRecordManager.NotifyCurrentRecordListChanged(te);
        }

        private ITableEventsTarget listChangedEventsTarget;

        void NewBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            // On demand initialization of sortFields and other look up tables after a 
            // schema change (PropertyDescriptorChanged resets the hash tables)
            EnsureSortFields();

            try
            {
                inListChanged = true;

                if (Engine.RaiseSourceListChangedEventsOnEngineOnly)
                {
                    listChangedEventsTarget = Engine as ITableEventsTarget;  // setting this to null costs about  1sec in realtimesummary test...
                }
                else
                {
                    listChangedEventsTarget = null;
                }

                Table parentTable = ParentTable;
                TableListChangedEventArgs te = TableListChangedEventArgs.Create(this, e.ListChangedType, e.NewIndex, e.OldIndex, e.PropertyDescriptor);

                te.ShouldInvalidateSummaries = false;
                te.ShouldReevaluateSortPosition = false;
                te.ShouldInvalidateGroupSortOrder = false;
                te.ShouldInvalidateCounters = false;

                // Check if CategorizeElements is called from SourceListListChanged event handler.
                try
                {
                    int savedCategorizeElementsVersion = this.CategorizeElementsVersion;

                    if ((allowListChangedEvents & GroupingListChangedEvents.SourceListListChanged) != 0)
                    {
                        // Note: Table.OnRelatedTableSourceListListChanged sets
                        // Table.sourceListVersion++;
                        if (parentTable != null)
                        {
                            parentTable.OnRelatedTableSourceListListChanged(te);
                        }

                        // Note: TableControl.Table sets MarkResync false if Table.TableDirty
                        if (listChangedEventsTarget != null)
                        {
                            listChangedEventsTarget.OnSourceListListChanged(te);
                        }
                        else
                        {
                            OnSourceListListChanged(te);
                            ////isGroupSortBySummaryOrderAffected = te.ShouldInvalidateGroupSortOrder;
                        }
                    }

                    if (savedCategorizeElementsVersion != this.CategorizeElementsVersion)
                    {
                        ////oldCount = sourceList.Count;
                        return;  //// nothing further to do any more - the whole table has been recategorized with new data
                    }
                    //// Check if CategorizeElements is called from SourceListListChanged event handler.
                } 
                catch (Exception ex)
                {
                    Syncfusion.Diagnostics.TraceUtil.TraceExceptionCatched(ex);
                }

                ////                StringBuilder changeFieldsString = new StringBuilder();
                ////                if (this.changedFieldsArray != null)
                ////                {
                ////                    foreach (ChangedFieldInfo ci in changedFieldsArray)
                ////                        changeFieldsString.AppendFormat("{0}: {1} -> {2}", ci.Name, ci.OldValue, ci.NewValue);
                ////                }
                ////
                ////                Console.WriteLine(e.ListChangedType.ToString() + ": " + changeFieldsString.ToString());

                ChildTable senderChildTable = sender as ChildTable;
                int newIndex = e.NewIndex;
                int oldIndex = e.OldIndex;
                AdjustIndexForChildTable(sender, ref newIndex, ref oldIndex);

                try
                {
                    switch (e.ListChangedType)
                    {
                        case ListChangedType.ItemChanged:
                            bool dontTouchLastChangedFields = false;
                            if (e.PropertyDescriptor != null)
                            {
                                //// This usually is expression column in a DataTable.
                                string name = e.PropertyDescriptor.Name;
                                int fieldIndex = TableDescriptor.Fields.IndexOf(name);
                                if (fieldIndex == -1 && dt != null && this.changedFields.Count == 0)
                                {
                                    return;
                                }
                                else
                                {
                                    ChangedFieldInfo ci = new ChangedFieldInfo(this.TableDescriptor, name);

                                    //// Setting ci.NewValue ensures that the value gets applied when later 
                                    //// record.UpdateValues is called. Without setting the newValue the UpdateValues 
                                    //// in GridRecord with CacheRecordValues set to true would otherwise skip setting 
                                    //// the fieldvalues to the new value. This fixes problems
                                    //// seen in incidents 48085 and 47989 with items implementing INotifyPropertyChanged 
                                    //// in a BindingList<> or List<> of objects.
                                    if (e.NewIndex >= 0 && e.NewIndex < SourceList.Count)
                                    {
                                        ci.NewValue = e.PropertyDescriptor.GetValue(SourceList[e.NewIndex]);
                                        // Fix for defect #12120: I also need to save old value so that
                                        // blinking code can look for deltas.
                                        Record r = UnsortedRecords[e.NewIndex];
                                        if (e.NewIndex == lastAddNewIndex && e.ListChangedType != ListChangedType.ItemChanged)
                                            ci.OldValue = ci.NewValue; //r.GetValue(TableDescriptor.Fields[fieldIndex]);
                                        else
                                        {
                                            if (fieldIndex == -1)
                                            {
                                                return;
                                            }
                                            else
                                            {
                                                ci.OldValue = r.GetValue(TableDescriptor.Fields[fieldIndex]);
                                            }
                                        }
                                    }

                                    AddChangedField(ci);
                                    dontTouchLastChangedFields = true;
                                }
                            }

                            //// When expression fields in DataRow are changed there are no
                            //// ColumnChanging notifications raised but instead the ListChanged
                            //// event is raised for every expression field that is dependant
                            //// on the column changed. I therefore do reuse the changeFieldsArray
                            //// here for those subsequent ListChanged notifications that are 
                            //// raised for each expression field.
                            if (dt != null && e.NewIndex == this.last_ChangedIndex
                                && this.changedFields.Count == 0)
                            {
                                this.changedFields = this.last_changedFields;
                                this.changedFieldsArray = this.last_changedFieldsArray;
                            }

                            if (TableDirty)
                            {
                                this.lastAddNewIndex = -1;
                                this.SourceListVersion++;
                                this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            }
                            else if ((newIndex == UnsortedRecords.Count
                                    || senderChildTable != null && e.NewIndex == senderChildTable.Records.Count))
                            {
                                //// Custom Collection implementations might raise ItemAdded event which 
                                //// is ignored the first time when CurrentRecordManager.BeginEdit is called
                                //// and then later raise ItemChanged events when EndEdit is called. 

                                //// work around quirky behavior of custom collection implementations, sometimes
                                //// ItemChanged events can be raised while record is in edit mode. In that case
                                //// ignore ItemChange events. A final ItemChanged or ItemAdded event should be raised when
                                //// EndEdit is called or if that is not the case then the SaveRecordHelper method
                                //// will call SimulateListChanged with an ItemAdded notification.
                                if (CurrentRecordManager.InEndEdit || !CurrentRecordManager.IsEditing)
                                    OnSourceListItemAdded(sender, e, te);
                                this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            }
                            else
                            {
                                if (this.RelationParentTable != null && this.RelationParentTable.TableDirtyOnItemChanged)
                                {
                                    this.tableDirtyOnItemChanged = true;
                                }
                                if (this.tableDirtyOnItemChanged && this.relatedTables != null)
                                {
                                    this.TableDirty = true;
                                    foreach (Table relatedTable in this.RelatedTables)
                                        relatedTable.TableDirty = true;
                                }
                                OnSourceListItemChanged(sender, e, te);
                            }

                            //// Save changed fields information in case expresion fields in underlying
                            //// DataRow are also affected and DataView will raise more ListChanged.ItemChanged
                            //// notification for each expression field.
                            if (!dontTouchLastChangedFields)
                            {
                                this.last_changedFields = this.changedFields;
                                this.last_changedFieldsArray = this.changedFieldsArray;
                                this.last_ChangedIndex = e.NewIndex;
                            }

                            break;

                        case ListChangedType.ItemDeleted:
                            this.lastAddNewIndex = -1;
                            this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            if (TableDirty)
                            {
                                this.SourceListVersion++;
                            }
                            else
                            {
                                OnSourceListItemDeleted(sender, e, te);
                            }

                            break;

                        case ListChangedType.ItemAdded:
                            this.lastAddNewIndex = -1;
                            this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            if (TableDirty)
                            {
                                this.SourceListVersion++;
                            }
                            else
                            {
                                OnSourceListItemAdded(sender, e, te);
                            }

                            break;

                        case ListChangedType.ItemMoved:
                            this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            if (TableDirty)
                            {
                                this.SourceListVersion++;
                            }
                            else
                            {
                                OnSourceListItemMoved(sender, e, te);
                            }

                            this.lastAddNewIndex = -1;
                            break;

                        case ListChangedType.Reset:
                            this.lastAddNewIndex = -1;
                            this.sourceListSortArray = null; //// can be rebuilt from unsortedRecordsTree
                            OnSourceListReset(sender, e, te);
                            break;

                        case ListChangedType.PropertyDescriptorChanged:
                        case ListChangedType.PropertyDescriptorDeleted:
                        case ListChangedType.PropertyDescriptorAdded:
                            this.lastAddNewIndex = -1;
                            OnSourceListPropertyDescriptorChanged(sender, e, te);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Syncfusion.Diagnostics.TraceUtil.TraceExceptionCatched(ex);
                }

                this.lastListChangedEventArgs = e;

                try
                {
                    //// SourceListListChangedCompleted is raised as very last statement.
                    if ((allowListChangedEvents & GroupingListChangedEvents.SourceListListChangedCompleted) != 0)
                    {
                        if (parentTable != null)
                        {
                            parentTable.OnRelatedTableSourceListListChangedCompleted(te);
                        }

                        if (listChangedEventsTarget != null)
                        {
                            listChangedEventsTarget.OnSourceListListChangedCompleted(te);
                        }
                        else
                        {
                            OnSourceListListChangedCompleted(te);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Syncfusion.Diagnostics.TraceUtil.TraceExceptionCatched(ex);
                }
            }
            finally
            {
                inListChanged = false;

                //// Clear out ChangeFieldInfo (Record.GetOldValue will return null from now on until new ChangedFieldInfo are set).
                changedFields = new Hashtable();
                changedFieldsArray = new ChangedFieldInfoCollection();
            }
        }

        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// PropertyDescriptorChanged. The method is only called if UseOldListChangedHandler is false.
        /// </summary>   
        protected virtual void OnSourceListPropertyDescriptorChanged(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            SourceListVersion++;
            TableDescriptor.SetItemProperties(this.SourceList);
        }

        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// Reset. The method is only called if UseOldListChangedHandler is false.
        /// </summary>        
        protected virtual void OnSourceListReset(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            SourceListVersion++;
            TableDescriptor.SetItemProperties(this.SourceList);
            bool clear = false;

#if SyncfusionFramework2_0
            if (this.SourceList is System.Windows.Forms.BindingSource)
            {
                object bl = ((System.Windows.Forms.BindingSource)SourceList).List;

                if (!Object.ReferenceEquals(bl, this.bindingSourceList))
                {
                    bindingSourceList = bl;
                    clear = true;
                }
            }
#endif

            if (CompareAndCacheSortDescription(SourceList))
            {
                clear = true;
            }

            // need to find out if in such case the whole list needs to be reinitialized
            if (!TableDirty && (clear || this.UnsortedRecords.Count != this.SourceList.Count || Engine.TableDirtyOnSourceListReset))
            {
                // this collapses all records ... If user's don't want that to happen
                // you need to override OnSourceListReset
                SelectedRecords.DisableEvents();
                SelectedRecords.Clear();
                CurrentRecordManager.Reset();
                SelectedRecords.EnableEvents();
                TableDirty = true;
                if (Engine.AllowSetRelatedTablesDirty)
                {
                    SetRelatedTablesDirty(this);
                }
            }

            if (clear)
            {
                sourceListSortArray = null;
                unsortedRecordsTree.Clear();
            }

            if (!te.ShouldIgnoreReset)
            {
                // But resetting counters should allways make sense, e.g. if List was sorted,
                // AdjustRecordRowCount should be called for all records.
                CountersDirty = true;
            }
        }


        private void SetRelatedTablesDirty(Table t)
        {
            t.TableDirty = true;
            t.SelectedRecords.InternalClear();
            if (t.RelatedTables != null)
            {
                foreach (Table t1 in t.RelatedTables)
                {
                    SetRelatedTablesDirty(t1);
                }
            }
        }

        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// ItemAdded. The method is only called if UseOldListChangedHandler is false.
        /// </summary>       
        protected virtual void OnSourceListItemAdded(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            //// When DataTable.NewRow or CurrencyManager.AddNew is called two ListChangedType.ItemAdded
            //// notifications are raised, both with the same e.NewIndex. One at the time AddNew
            //// is called, the other when EndEdit is called.

            if (CurrentRecordManager.InBeginEdit)
            {
                return;
            }

            if (dataSourceRaisesTwoItemAddedEvents)
            {
                bool isBeginEdit =
                    lastListChangedEventArgs == null
                    || lastListChangedEventArgs.ListChangedType != ListChangedType.ItemAdded
                    || e.NewIndex != lastListChangedEventArgs.NewIndex;

                if (isBeginEdit)
                {
                    if (changedFields.Count == 0)  
                    {
                        // instead of checking for InBeginEdit ...
                        return;
                    }
                }
            }

            TableDescriptor td = TableDescriptor;
            RecordChangedEventArgs rcea = null;
            bool ispkSortOrderAffected = false;
            bool isSortOrderAffected = te.ShouldReevaluateSortPosition || e.ListChangedType == ListChangedType.ItemMoved;
            bool isSummaryAffected = te.ShouldInvalidateSummaries;
            bool isGroupSortBySummaryOrderAffected = te.ShouldInvalidateGroupSortOrder;
            bool meetFilterChanged;
            bool isCurrent;
            Record r;

            //// CurrentRecord.EndEdit returns this later for AddNew case
            //// LastChangedRecord = null;

            ChildTable senderChildTable = sender as ChildTable;
            int newIndex = e.NewIndex;
            int oldIndex = e.OldIndex;
            AdjustIndexForChildTable(sender, ref newIndex, ref oldIndex);

            //// Let's Record.GetOldValue pick up ChangedFieldInfo values and make sure
            //// it is for the same record (matches listChangedRecordIndex).
            this.listChangedRecordIndex = newIndex;

            //// Create a new record, add it to UnsortedRecords collection
            r = TableDescriptor.CreateRecord(this);

            object data = SourceList[e.NewIndex];
            r.SetSourceIndex(newIndex, SourceListVersion, data);

            int unsortedRecordsCount = UnsortedRecords.Count;
            if (senderChildTable != null && senderChildTable.GetChildListPosition() != -1 && e.NewIndex == senderChildTable.SourceList.Count - 1)
            {
                int pos = senderChildTable.GetChildListPosition() + senderChildTable.GetRecordCount();
                this.UnsortedRecords.Insert(pos, r);
                for (int n = e.NewIndex; n < unsortedRecordsCount + 1; n++)
                {
                    UnsortedRecords[n].sourceIndex = n;
                }

                SourceListVersion++;
            }
            else if (e.NewIndex < UnsortedRecords.Count)
            {
                UnsortedRecords.Insert(e.NewIndex, r);
                for (int n = e.NewIndex; n < unsortedRecordsCount + 1; n++)
                {
                    UnsortedRecords[n].sourceIndex = n;
                }

                //// DataView raises a ItemAdded with subsequent ItemMoved event when data view is sorted
                //// the lastAddNewIndex will be checked in the OnSourceListItemChanged and ItemMoved
                //// handler so that the subsequents events can be ignored.
                this.lastAddNewIndex = e.NewIndex;
            }
            else
            {
                UnsortedRecords.Add(r);
                this.lastAddNewIndex = e.NewIndex;
            }

            unsortedRecordsCount++; 
            if (senderChildTable != null && senderChildTable.UnsortedEntry == null)
            {
                senderChildTable.UnsortedEntry = r.UnsortedEntry;
            }

            // CurrentRecordManager.EndEdit will check LastChangedRecord to highlight
            // current record after saving changes.
            LastChangedRecord = r;

            // Determine if any fields that affect sort order or summaries were changed.
            isSortOrderAffected = this.sortFields.Count > 0;
            ispkSortOrderAffected = this.pksortFields.Count > 0;
            isSummaryAffected = this.summaryFields.Count > 0;
            te.ShouldInvalidateGroupSortOrder = isGroupSortBySummaryOrderAffected = this.groupSortBySummaryFields.Count > 0;

            // UniformChildList relation
            if (senderChildTable != null)
            {
                isSortOrderAffected = true;
                r.Parent = senderChildTable.ParentNestedTable.ParentRecord; // must be set before UpdateSortInfo is called!
                r.ParentElement = senderChildTable.Details;
            }

            // Generate missing ChangedFieldInfo objects
            PrepareItemAdded(data);

            // Record has optional support for caching values.
            r.EnsureValues();

            // Is it the current record
            isCurrent = CurrentRecordManager.InEndEdit;

            // Refresh records internal state
            {
                // Clean out values for UpdateSortInfo / UpdatePrimaryKeys
                // and MeetsFilterCriteria()
                r.InvalidateCounter();
                r.InvalidateSummary();

                if (isSortOrderAffected && TableDescriptor.IsSorted)
                {
                    r.UpdateSortInfo(true, TableDescriptor.GetSortDescriptors(), null);
                }

                if (ispkSortOrderAffected)
                {
                    r.UpdatePrimaryKeys(true, TableDescriptor.GetPrimaryKeySortDescriptors(), null);
                }
            }

            // Check new visibility of record after change.
            meetFilterChanged = r.MeetsFilterCriteria();

            // Fix primary key sort position.
            if (ispkSortOrderAffected)
            {
                PrimaryKeySortedRecords.Add(r);
            }

            // Raise SourceListRecordChanging before counters or summaries were invalidated.
            rcea = new RecordChangedEventArgs(r, RecordChangedType.Added, newIndex, oldIndex, null, true, te);
            rcea.VisibilityChanged = meetFilterChanged;
            rcea.GroupsChanged = true;
            rcea.RaiseDisplayElementChanged = false;

            RaiseSourceListRecordChanging(rcea);

            // Force related ChildTables to be created.
            r.GetCounter();

            // Add nested records if an item is added that contains itself a nested collection
            AddNestedUniformChildListItems(r);

            // Inserted record into matching group at correct sort position. If needed a new group
            // is added that fits records category.
            Group newGroup = r.ReinsertRecord();
            rcea.AddedGroup = newGroup;

            // Fix ManualTotalSummary of parent groups.
            OnRecordChanged(r.ParentSection, false, true); // added

            // Reset counters for the new parent elements of the record
            InvalidateCountersAndSummaries(r.ParentSection, te.ShouldInvalidateScreen);
            ////ChildTable.InvalidateCounter already invalidates senderChildTable.ParentNestedTable-->
            ////if (senderChildTable != null)
            ////    InvalidateCountersAndSummaries(senderChildTable.ParentNestedTable);
            //// <--

            //// Support for Sort by Summary in caption - groups need to be reordered
            if (isGroupSortBySummaryOrderAffected)
            {
                InvalidateParentGroupSortOrder(r.ParentGroup);
            }

            //// DisplayElements caches will check for EngineVersion
            Engine.BumpVersion();

            //// This is a new record and it is visible - get top-most group in parent nodes that only has one filtered record
            if (meetFilterChanged)
            {
                rcea.Group = GetParentGroupWithSingleFilteredRecord(r);
            }

            //// SourceListRecordChanged is raised after counters or summaries were invalidated.
            RaiseSourceListRecordChanged(rcea);
        }

        static void AddNestedUniformChildListItems(Record parentRecord)
        {
            //// Check if the record has any nested collections. If yes,
            //// recursively add records for items in nested collections.

            TableDescriptor td = parentRecord.ParentTableDescriptor;
            foreach (RelationDescriptor rd in td.Relations)
            {
                if (rd.RelationKind == RelationKind.UniformChildList)
                {
                    string mappingName = rd.MappingName;
                    string relationName = rd.Name;
                    PropertyDescriptor pd = td.ItemProperties[mappingName];
                    AddNestedUniformChildListItems(rd, pd, parentRecord);
                }
            }
        }

        static void AddNestedUniformChildListItems(RelationDescriptor rd, PropertyDescriptor pd, Record parentRecord)
        {
            //// rd.RelationKind must be RelationKind.UniformChildList!

            //// Get nested collection from item
            object obj = pd.GetValue(parentRecord.GetData());
            while (obj is IListSource && ((IListSource)obj).ContainsListCollection)
            {
                obj = ((IListSource)obj).GetList();
            }

            IEnumerable list = obj as IEnumerable;

            //// Nothing to do when list is empty
            if (!list.GetEnumerator().MoveNext())
            {
                return;
            }

            bool first = true;
            UnsortedRecordsTreeEntry firstUnsortedEntry = null;

            //// Get child table where nested records should be added
            ChildTable ct = parentRecord.NestedTables[rd.Name].ChildTable;

            // Fix: SD4186 - Records Add/Delete problem with UseLazyUniformChildListRelation setting
            if (ct != null)
            {
                Table relatedTable = ct.ParentTable;

                //// Get current total number of records in table before adding new records
                int n = relatedTable.UnsortedRecords.Count;

                //// Schema details to be stored in new records.
                SortColumnDescriptor[] arrayOfColumnDescriptors;
                PropertyDescriptor[] arrayOfPropertyDescriptor;
                bool isSorted;
                relatedTable.TableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

                SortColumnDescriptor[] arrayOfPKColumnDescriptors;
                PropertyDescriptor[] arrayOfPKPropertyDescriptor;
                bool isPKSorted;
                relatedTable.TableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);

                //// List of newly created records
                ArrayList sourceListSortArray = new ArrayList();

                //// Create and add UnsortedRecordsTreeEntry foreach item in nested collection
                foreach (object item in list)
                {
                    UnsortedRecordsTreeEntry unsortedEntry = new UnsortedRecordsTreeEntry();
                    Record record = relatedTable.TableDescriptor.CreateRecord(relatedTable);
                    record.UnsortedEntry = unsortedEntry;

                    ////if (this.IsNewUniformChildListRelation())
                    record.sourceIndex = n;  //// prevent data from being set to null.
                    record.sourceListVersion = relatedTable.SourceListVersion;

                    record.Parent = parentRecord; //// must be set before UpdateSortInfo is called!
                    record.SetData(item, isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                    record.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                    record.UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                    sourceListSortArray.Add(record);

                    unsortedEntry.Record = record;
                    unsortedEntry.Tree = relatedTable.unsortedRecordsTree.TreeTable;
                    relatedTable.unsortedRecordsTree.Add(unsortedEntry);
                    n++;

                    if (first)
                    {
                        firstUnsortedEntry = unsortedEntry;
                        first = false;
                    }

                    record.EnsureSortedEntry(record);
                }

                //// Assign the nested collection to child table.
                ct.SourceList = (IList)list;
                ct.UnsortedEntry = firstUnsortedEntry;

                //// Now insert records with correct sort and group order.
                foreach (Record r in sourceListSortArray)
                {
                    relatedTable.InsertSortedRecordsTreeEntry(r.SortedEntry);

                    // Do the same recursively if this record has nested collections.
                    AddNestedUniformChildListItems(r);
                }
            }
        }

        Group GetParentGroupWithSingleFilteredRecord(Record r)
        {
            return GetParentGroup(r, 1);
        }

        Group GetParentGroupWithNoFilteredRecord(Record r)
        {
            return GetParentGroup(r, 0);
        }

        Group GetParentGroup(Record r, int filterCount)
        {
            Group result = null;
            Group parentGroup = r.ParentGroup;
            while (parentGroup != null && !(parentGroup is ChildTable))
            {
                // check if parent group only has one filtered record. 
                int parentGroupFilterCount = parentGroup.GetFilteredRecordCount();
                if (parentGroupFilterCount == filterCount)
                {
                    result = parentGroup;
                    parentGroup = parentGroup.ParentGroup;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// ItemMoved. The method is only called if UseOldListChangedHandler is false.
        /// </summary>      
        protected virtual void OnSourceListItemMoved(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            TableDescriptor td = TableDescriptor;

            ChildTable senderChildTable = sender as ChildTable;
            int newIndex = e.NewIndex;
            int oldIndex = e.OldIndex;
            AdjustIndexForChildTable(sender, ref newIndex, ref oldIndex);

            // DataView raises a ItemAdded with subsequent ItemChanged and ItemMoved event when data view is sorted
            if (senderChildTable == null && lastAddNewIndex == e.NewIndex)
            {
                // In the ItemAdded event the e.NewIndex already had the correct index and was
                // inserted at the right position. Doing it here again would mess up UnsortedRecords
                // order.
                lastAddNewIndex = -1;
                return;
            }

            lastAddNewIndex = -1;

            // First, synchronize the UnsortedRecords collection with change in underlying datasource.
            Record r = UnsortedRecords[oldIndex];

#if TESTING
            if (r.ParentGroup == null)
                Debugger.Break();
#endif
            UnsortedRecords.Remove(r);
            UnsortedRecords.Insert(newIndex, r);
            ClearCollectionCaches();

            // If this was the first record in a uniform child list
            // the child tables offset needs to be moved to the next 
            // record. 
            if (senderChildTable != null)
            {
                if (e.OldIndex == 0)
                {
                    senderChildTable.SetChildListPosition(oldIndex);
                }
                else if (e.NewIndex == 0)
                {
                    senderChildTable.SetChildListPosition(newIndex);
                }
            }

            //// Ensure record.GetSourceIndex is updated with new position

            if (newIndex != -1 && oldIndex != -1)
            {
                int to = Math.Max(newIndex, oldIndex);
                for (int n = Math.Min(newIndex, oldIndex); n <= to; n++)
                {
                    UnsortedRecords[n].sourceIndex = n;
                }
            }

            // newIndex is passed on to ListItemChanged, sort order is affected 
            // if no other sort criteria have higher precedence (IsSortOrderAffected
            // method will check GetSourceIndex() in such case).
            OnSourceListItemChanged(sender, e, te);
        }

        internal bool IsInSourceListItemDeleted = false;
        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// ItemDeleted. The method is only called if UseOldListChangedHandler is false.
        /// </summary>       
        protected virtual void OnSourceListItemDeleted(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            TableDescriptor td = TableDescriptor;
            RecordChangedEventArgs rcea = null;
            Group obsoleteGroup = null;
            bool ispkSortOrderAffected = false;
            bool isSortOrderAffected = te.ShouldReevaluateSortPosition;
            bool isSummaryAffected = te.ShouldInvalidateSummaries;
            bool isGroupSortBySummaryOrderAffected = te.ShouldInvalidateGroupSortOrder;
            bool didMeetFilterCriteria;
            bool isCurrent;
            Record r;
            DetailsSection oldParentSection = null;
            IsInSourceListItemDeleted = true;
            // CurrentRecord.EndEdit returns this later for AddNew case
            //// LastChangedRecord = null;

            ChildTable senderChildTable = sender as ChildTable;
            int newIndex = e.NewIndex;
            int oldIndex = e.OldIndex;
            AdjustIndexForChildTable(sender, ref newIndex, ref oldIndex);

            //// Prepare record, investigate changes
            { ////if (CurrentRecordManager.CurrentElement is AddNewRecord
                ////&& CurrentRecordManager.InCancelEdit)
                if (newIndex >= UnsortedRecords.Count)               
                {
                    return;
                }

                r = UnsortedRecords[newIndex];

#if TESTING
                if (r.ParentGroup == null)
                    Debugger.Break();
#endif
                //// Check previous visibility of record before change.
                didMeetFilterCriteria = WithoutCounter || r.GetSavedMeetsFilterCriteria();

                //// Is it the current record
                isCurrent = r.IsCurrent;

                // Determine if any fields that affect sort order or summaries were changed.
                isSortOrderAffected = this.sortFields.Count > 0;
                ispkSortOrderAffected = this.pksortFields.Count > 0;
                isSummaryAffected = this.summaryFields.Count > 0;
                te.ShouldInvalidateGroupSortOrder = isGroupSortBySummaryOrderAffected = this.groupSortBySummaryFields.Count > 0;
            } // Prepare record, investigate changes

            // Fix primary key sort position.
            if (ispkSortOrderAffected)
            {
                PrimaryKeySortedRecords.Remove(r);
            }

            // Remove record from group
            {
                //// Reset counters for parent elements of the record
                oldParentSection = (DetailsSection)r.ParentSection;

                //// Get group that only shows this one record.
                Group hidingGroup = GetParentGroupWithSingleFilteredRecord(r);

                //// Record will be removed from group. Check if old group
                //// only had that one record and should be deleted.
                oldParentSection = DeleteEmptyGroups(oldParentSection, out obsoleteGroup);

                //// In original grid code:
                //// TableControl.Table_SourceListRecordChanging will handle on RecordChangedType.Removed case,
                //// looks for e.Group 

                //// Table.OnSourceListRecordChanging will check for e.RaiseDisplayElementChanged and
                //// raise EngineTable.RaiseDisplayElementChanging. Avoid this by setting
                //// GridGroupingControl.UseCustomUpdateOnListChanged = true;

                EngineTable.LockOutEnsureInitialized = true;

                Record nextRecord = r.GetNextRecord();

                rcea = new RecordChangedEventArgs(r, RecordChangedType.Removed, newIndex, oldIndex, hidingGroup, false, te);
                rcea.VisibilityChanged = didMeetFilterCriteria;
                rcea.GroupsChanged = true;
                rcea.RaiseDisplayElementChanged = false;
                rcea.RemovedGroup = obsoleteGroup;

                RaiseSourceListRecordChanging(rcea);

                SelectedRecords.DisableEvents();

                //// Remove any selected records
                if (r.IsSelected() && SelectedRecords.Count <= 1)
                {
                    r.SetSelected(false);
                }
                else
                {
                    r.SetSelectedRecursive(false);
                }

                // Give current record a chance to reset itsself
                if (isCurrent)
                {
                    NotifyCurrentRecordListChanged(te);
                    if (te.NavigateCurrentRecordWhenDeleted)
                    {
                        // Move current record out of the way if it is about to be deleted.
                        CurrentRecordManager.NavigateTo(nextRecord);
                    }
                }

                // Remove the group but raise event first.
                if (obsoleteGroup != null)
                {
                    RaiseGroupRemoving(obsoleteGroup);
                    ((GroupsDetails)oldParentSection).Groups.Remove(obsoleteGroup);
                }

                // Remove record from group (as a result it will also disappear from Table.Records collection)
                r.DetachFromGroup();

                // Recursivle remove group from ChildTable collection of related table.
                RemoveUniformChildListChildTable(r);

                // Remove record from Unsorted records collection
                UnsortedRecords.Remove(r);

                // Do not dispose record at this time. The current object is still being used
                // by GridGroupingControlOptimizeListChanged.
                // However, be carefull to not use this element anywhere else, especially
                // using it as current record at this could cause severe side effects.

                // If this was the first record in a uniform child list
                // the child tables offset needs to be moved to the next 
                // record. 
                if (senderChildTable != null && e.NewIndex == 0)
                {
                    if (senderChildTable.SourceList.Count == 0)
                    {
                        senderChildTable.SetChildListPosition(-1);
                    }
                    else
                    {
                        senderChildTable.SetChildListPosition(newIndex);
                    }
                }

                SelectedRecords.EnableEvents();

                // Fix ManualTotalSummary of parent groups.
                OnRecordChanged(oldParentSection, true, false); // remove

                // Reset counters for the parent elements of the record
                InvalidateCountersAndSummaries(oldParentSection, te.ShouldInvalidateScreen);

                EngineTable.LockOutEnsureInitialized = false;

                // DisplayElements caches will check for EngineVersion
                Engine.BumpVersion();

                SourceListVersion++; // force GetSourceIndex of record to replace Record.sourceIndex,
                // SourceListVersion also ensures that SelectedRecords are kept in sync.
            } // Check if fields of SortedColumns, GroupedColumns or RelationChildColumns affected?

            // Support for Sort by Summary in caption - groups need to be reordered
            if (isGroupSortBySummaryOrderAffected || (didMeetFilterCriteria && groupSortBySummaryFields.Count > 0))
            {
                te.ShouldInvalidateGroupSortOrder = isGroupSortBySummaryOrderAffected = true;
                InvalidateParentGroupSortOrder(r.ParentGroup);
                if (oldParentSection != null)
                {
                    InvalidateParentGroupSortOrder(oldParentSection.ParentGroup);
                }
            }

            // SourceListRecordChanging was raised before counters or summaries were invalidated.

            // SourceListRecordChanged is now raised after counters or summaries were invalidated.
            RaiseSourceListRecordChanged(rcea);

            r.Dispose();
            deletedrecord = null;
            if (obsoleteGroup != null)
            {
                obsoleteGroup.Dispose();
            }
        }

        static void RemoveUniformChildListChildTable(Record record)
        {
            //// Check if the record has any nested collections. If yes,
            //// recursively remove child tables in nested collections.

            TableDescriptor td = record.ParentTableDescriptor;
            if (td.Engine.UseOldUniformChildListRelation)
            {
                return;
            }

            foreach (RelationDescriptor rd in td.Relations)
            {
                if (rd.RelationKind == RelationKind.UniformChildList)
                {
                    // Get child table where nested records should be added
                    ChildTable ct = record.NestedTables[rd.Name].ChildTable;
                    if (ct != null)
                    {
                        foreach (Record r in ct.Records)
                        {
                            // Remove record from group (as a result it will also disappear from Table.Records collection)
                            r.DetachFromGroup();

                            // Remove group from ChildTable collection.
                            RemoveUniformChildListChildTable(r);
                        }

                        GroupsDetails gd = ct.ParentSection as GroupsDetails;
                        if (gd != null)
                        {
                            gd.Groups.Remove(ct);
                        }
                    }
                }
            }
        }

        void SetGroup(RecordChangedEventArgs rcea, Group group)
        {
            System.Reflection.FieldInfo fi = typeof(RecordChangedEventArgs).GetField("group", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            fi.SetValue(rcea, group);
        }

        /// <summary>
        /// Virtual method is called when a ListChanged event is handled and ListChangedType is 
        /// ItemChanged. The method is only called if UseOldListChangedHandler is false.
        /// </summary>       
        protected virtual void OnSourceListItemChanged(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            TableDescriptor td = TableDescriptor;
            RecordChangedEventArgs rcea = null;
            Group obsoleteGroup = null;
            bool ispkSortOrderAffected = false;
            bool isSortOrderAffected = !VirtualMode && (te.ShouldReevaluateSortPosition || e.ListChangedType == ListChangedType.ItemMoved);
            bool isSummaryAffected = !VirtualMode && te.ShouldInvalidateSummaries;
            bool isGroupSortBySummaryOrderAffected = !VirtualMode && te.ShouldInvalidateGroupSortOrder;
            bool isNestedRelationParentKeyFieldAffected = false;
            bool meetFilterChanged;
            bool didMeetFilterCriteria;
            bool isCurrent;
            bool isOnlyValueChange = false;
            Record r;
            DetailsSection oldParentSection = null;
            Group hidingGroup = null;

            //// CurrentRecord.EndEdit returns this later for AddNew case
            //// LastChangedRecord = null;

            int newIndex = e.NewIndex;
            int oldIndex = e.OldIndex;
            AdjustIndexForChildTable(sender, ref newIndex, ref oldIndex);

            //// Let's Record.GetOldValue pick up ChangedFieldInfo values and make sure
            //// it is for the same record (matches listChangedRecordIndex).
            this.listChangedRecordIndex = newIndex;

            //// Prepare record, investigate changes
            {
                r = UnsortedRecords[newIndex];

#if TESTING
                if (r.ParentGroup == null)
                    Debugger.Break();
#endif
                //// Check previous visibility of record before change.
                didMeetFilterCriteria = WithoutCounter || r.GetSavedMeetsFilterCriteria();

                //// Get group that only shows this one record.
                if (didMeetFilterCriteria)
                {
                    hidingGroup = GetParentGroupWithSingleFilteredRecord(r);
                }

                //// Record has optional support for caching values.
                {
                    r.EnsureValues();

                    if (this.changedFields.Count > 0)
                    {
                        r.UpdateValues(this.changedFieldsArray);
                    }
                    else
                    {
                        ChangedFieldInfoCollection cf = r.CompareAndUpdateValues();
                        if (cf != null)
                        {
                            for (int n = 0; n < cf.Count; n++)
                            {
                                AddChangedField(cf[n]);
                            }
                        }
                    }

                    //// let users not worry when overriding CompareAndUpdateValues ...
                    if (changedFieldsArray == null)
                    {
                        changedFieldsArray = new ChangedFieldInfoCollection();
                    }
                }

                //// Is it the current record
                isCurrent = r.IsCurrent;

                //// A BindingList in .NET 2.0 immeditaley raises a ItemChanged event after each
                //// change even if BeginEdit was called. The following code checks to make
                //// sure we proceed only if the record was modified outside the grid or
                //// if the CurrentRecordManager called EndEdit.
                if (isCurrent && Syncfusion.Grouping.Engine.AllowSkipItemChangedWhileEditing)
                {
                    if (!CurrentRecordManager.InEndEdit && CurrentRecordManager.isSetModifiedValue)
                    {
                        return;
                    }

                    if (CurrentRecordManager.InCancelEdit && CurrentRecordManager.isResetModifiedValue)
                    {
                        return;
                    }
                }

                //// Clear out "lastRecord" cache in RecordFilters
                td.RecordFilters.ResetCache();

                //// Determine if any fields that affect sort order or summaries were changed.
                {
                    if (changedFieldsArray.Count == 0)
                    {
                        //// DataView raises a ItemAdded with subsequent ItemChanged and ItemMoved event when data view is sorted
                        if (lastAddNewIndex != -1 && lastAddNewIndex == e.NewIndex)
                        {
                            return;
                        }

                        //// this is slow but in this case there is no ChangeFieldInfo information given
                        //// we have to assume worst case.
                        isSortOrderAffected |= this.sortFields.Count > 0;
                        ispkSortOrderAffected |= this.pksortFields.Count > 0;
                        isSummaryAffected |= this.summaryFields.Count > 0;
                        isGroupSortBySummaryOrderAffected |= this.groupSortBySummaryFields.Count > 0;
                    }
                    else
                    {
                        foreach (ChangedFieldInfo ci in this.changedFieldsArray)
                        {
                            isSortOrderAffected |= sortFields.Contains(ci.FieldIndex);
                            ispkSortOrderAffected |= pksortFields.Contains(ci.FieldIndex);
                            isSummaryAffected |= summaryFields.Contains(ci.FieldIndex);
                            isGroupSortBySummaryOrderAffected |= groupSortBySummaryFields.Contains(ci.FieldIndex) || groupSortBySummaryFields.Contains(-1);
                            ////isNestedRelationParentKeyFieldAffected |= relationParentKeyFields.Contains(ci.FieldIndex);

                            if (relationParentKeyFields.Contains(ci.FieldIndex))
                            {
                                //// Prevent Counter logic from getting out of sync
                                isNestedRelationParentKeyFieldAffected = true;
                                NestedTable nt = r.NestedTables[((RelationDescriptor)relationParentKeyFields[ci.FieldIndex]).Name];
                                if (nt != null && nt.ChildTable != null)
                                {
                                    nt.ChildTable.InvalidateCounterBottomUp();
                                }
                            }
                        }
                    }

                    if (dt != null || lastAddNewIndex != e.NewIndex)
                        lastAddNewIndex = -1;
                }

                if (isNestedRelationParentKeyFieldAffected)
                {
                    //// Prevent Counter logic from getting out of sync
                    r.InvalidateCounterBottomUp();
                    r.InvalidateCounter();
                }

                //// Refresh records internal state
                {
                    //// Clean out values for UpdateSortInfo / UpdatePrimaryKeys
                    //// and MeetsFilterCriteria()
                    r.InvalidateCounter();
                    r.InvalidateSummary();

                    if (isSortOrderAffected && TableDescriptor.IsSorted)
                    {
                        r.UpdateSortInfo(true, TableDescriptor.GetSortDescriptors(), null);
                    }

                    if (ispkSortOrderAffected)
                    {
                        r.UpdatePrimaryKeys(true, TableDescriptor.GetPrimaryKeySortDescriptors(), null);
                    }
                }

                //// Check new visibility of record after change.
                meetFilterChanged = didMeetFilterCriteria != r.MeetsFilterCriteria();
            } //// Prepare record, investigate changes

            //// Fix primary key sort position.
            if (ispkSortOrderAffected)
            {
                Record previous = PrimaryKeySortedRecords.GetPrevious(r);
                Record next = PrimaryKeySortedRecords.GetNext(r);

                //// Check if any of the columns that was changed affects the sort position of the records.
                ispkSortOrderAffected = IsSortOrderAffected(r, previous, next, td.GetPrimaryKeySortDescriptors(), this.changedFields);

                //// in that case fix the primary key sort position.
                if (ispkSortOrderAffected)
                {
                    PrimaryKeySortedRecords.FixPrimaryKeyPosition(r);
                }
            } //// Fix primary key sort position.

            bool matchesGroupCategories = true;

            //// Check if fields of SortedColumns, GroupedColumns or RelationChildColumns were affected.
            if (isSortOrderAffected)
            {
                //// Compare if record still matches group catecory criteria
                matchesGroupCategories = CompareGroupCategories(r.ParentGroup, r);

                //// still the same group ...
                if (matchesGroupCategories)
                {
                    //// only if parent group is expanded OR sort order is up to date
                    //// we need to check if change affects sort position of record.

                    RecordsDetails rd = (RecordsDetails)r.ParentSection;
                    if (rd.CompareSortedColumns())
                    {
                        isSortOrderAffected = false;
                    }
                    else
                    {
                        Record previous = r.GetPreviousRecord();
                        Record next = r.GetNextRecord();

                        //// Check if any of the columns that was changed affects the sort position of the records.
                        isSortOrderAffected = IsSortOrderAffected(r, previous, next, td.GetSortDescriptors(), this.changedFields);
                    }
                }
                else
                {
                    isGroupSortBySummaryOrderAffected |= groupSortBySummaryFields.Count > 0;
                }
            } //// Check if fields of SortedColumns, GroupedColumns or RelationChildColumns affected?

            te.ShouldInvalidateGroupSortOrder = isGroupSortBySummaryOrderAffected;

            if (isSortOrderAffected || !matchesGroupCategories)
            {
                //// Reset counters for parent elements of the record
                oldParentSection = (DetailsSection)r.ParentSection;

                if (!matchesGroupCategories)
                {
                    //// Record will be moved to a new group. Check if old group
                    //// only had that one record and should be deleted.
                    oldParentSection = DeleteEmptyGroups(oldParentSection, out obsoleteGroup);
                }

                if (!matchesGroupCategories || (meetFilterChanged && didMeetFilterCriteria))
                {
                    //// keep hidingGroup calculated earlier.
                }
                else
                {
                    hidingGroup = null;
                }

                //// In original grid code:
                //// Table.OnSourceListRecordChanging will check for e.RaiseDisplayElementChanged and
                //// raise EngineTable.RaiseDisplayElementChanging. Avoid this by setting
                //// GridGroupingControl.UseCustomUpdateOnListChanged = true;

                rcea = new RecordChangedEventArgs(r, RecordChangedType.Changed, newIndex, oldIndex, hidingGroup, true, te);
                rcea.IsNestedRelationParentKeyFieldAffected = isNestedRelationParentKeyFieldAffected;
                rcea.VisibilityChanged = meetFilterChanged;
                rcea.GroupsChanged = !matchesGroupCategories || isGroupSortBySummaryOrderAffected;
                rcea.RaiseDisplayElementChanged = false;

                RaiseSourceListRecordChanging(rcea);

                //// Give current record a chance to reset itsself
                if (isCurrent)
                {
                    NotifyCurrentRecordListChanged(te);
                }

                //// Remove the group but raise event first.
                if (obsoleteGroup != null)
                {
                    RaiseGroupRemoving(obsoleteGroup);
                    ((GroupsDetails)oldParentSection).Groups.Remove(obsoleteGroup);
                }

                if (matchesGroupCategories)
                {
                    //// Remove record from group and immediately reinsert it (at correct sort position / category).
                    r.DetachFromGroup();
                    ////r.GetElementEntry().InvalidateCounterBottomUp(true);
                    r.InvalidateCounter();

                    //// DisplayElements caches will check for EngineVersion
                    Engine.BumpVersion();

                    //// Give notification for Insert/Remove scrollwindow logic that it is now
                    //// safe to remove record from view. (but note: custom counters and summaries
                    //// have not been invalidated yet.)
                    ////RaiseSourceListItemChangedRecordDetached(r);
                    RaiseSourceListRecordChanged(rcea);

                    //// Record stays in same group, just insert at new sort position
                    r.AttachToGroup();

                    //// Fix ManualTotalSummary of parent groups.
                    OnRecordChanged(oldParentSection, false, false); //// delta only
                    //// oldParentSection.InvalidateCounterTopDown(true);

                    //// in case DisplayElements were accessed from RecordChanged
                    Engine.BumpVersion();

                    //// if support for custom counters
                    if (supportCustomCounters || meetFilterChanged)
                    {
                        InvalidateCounters(oldParentSection);
                    }

                    //// Reset summaries for the old parent elements of the record
                    //// Invalidate summaries if record was not hidden
                    if ((isSummaryAffected && didMeetFilterCriteria) || meetFilterChanged || this.summaryIgnoreRecordFilterCriteria)
                    {
                        InvalidateSummaries(oldParentSection, te.ShouldInvalidateScreen);
                    }

                    if (meetFilterChanged && !didMeetFilterCriteria)
                    {
                        rcea.Group = GetParentGroupWithSingleFilteredRecord(r);
                    }
                    else
                    {
                        rcea.Group = null;
                    }

                    rcea.GroupsChanged = rcea.Group != null;
                }
                else
                {
                    //// Remove record from group and immediately reinsert it (at correct sort position / category).
                    r.DetachFromGroup();
                    r.InvalidateCounter();
                    ////r.GetElementEntry().InvalidateCounterBottomUp(true);

                    //// Fix ManualTotalSummary of old parent groups.
                    OnRecordChanged(oldParentSection, true, false); //// remove

                    //// Reset counters for the old parent elements of the record
                    InvalidateCountersAndSummaries(oldParentSection, te.ShouldInvalidateScreen);

                    //// DisplayElements caches will check for EngineVersion
                    Engine.BumpVersion();

                    //// Give notification for Insert/Remove scrollwindow logic that it is now
                    //// safe to remove record from view. In this case SourceListRecordChanged event
                    //// is raised twice. Once before reinserting record and another one after
                    //// record was reinserted.
                    ////RaiseSourceListItemChangedRecordDetached(r);
                    RaiseSourceListRecordChanged(rcea);

                    //// Record moves to another group, will be inserted into new
                    //// group at correct sort position. If needed a new group
                    //// is added that fits records category.
                    Group newGroup = r.ReinsertRecord();
                    rcea.AddedGroup = newGroup;
                    ////SetGroup(rcea, newGroup);

                    //// Fix ManualTotalSummary of parent groups.
                    OnRecordChanged(r.ParentSection, false, true); //// added

                    //// Reset counters for the new parent elements of the record
                    InvalidateCountersAndSummaries(r.ParentSection, te.ShouldInvalidateScreen);

                    if (r.MeetsFilterCriteria())
                    {
                        rcea.Group = GetParentGroupWithSingleFilteredRecord(r);
                    }
                    else
                    {
                        rcea.Group = null;
                    }

                    rcea.GroupsChanged = true;
                    this.isDirty = true;
                }

                //// DisplayElements caches will check for EngineVersion
                Engine.BumpVersion();
            }          
            else if (meetFilterChanged)
            {  
                //// Show or hide record
                rcea = new RecordChangedEventArgs(r, RecordChangedType.Changed, newIndex, oldIndex, null, false, te);
                rcea.IsNestedRelationParentKeyFieldAffected = isNestedRelationParentKeyFieldAffected;
                rcea.VisibilityChanged = true;
                rcea.GroupsChanged = isGroupSortBySummaryOrderAffected;
                if (didMeetFilterCriteria)
                {
                    rcea.Group = GetParentGroupWithSingleFilteredRecord(r);
                }

                rcea.GroupsChanged = isGroupSortBySummaryOrderAffected || rcea.Group != null;

                RaiseSourceListRecordChanging(rcea);

                //// Give current record a chance to reset itsself
                if (isCurrent)
                {
                    NotifyCurrentRecordListChanged(te);
                }

                //// DisplayElements caches will check for EngineVersion
                Engine.BumpVersion();

                r.InvalidateCounter();

                //// Fix ManualTotalSummary of parent groups.
                OnRecordChanged(r.ParentSection, didMeetFilterCriteria, !didMeetFilterCriteria); //// either add or remove

                InvalidateCountersAndSummaries(r, te.ShouldInvalidateScreen);

                //// Again - just in case that user access DisplayElements while invalidating group summaries
                Engine.BumpVersion();

                if (!didMeetFilterCriteria)
                {
                    rcea.Group = GetParentGroupWithSingleFilteredRecord(r);
                }

                rcea.GroupsChanged = isGroupSortBySummaryOrderAffected || rcea.Group != null;
            }
            else 
            {
                //// same group, no sort order change, no visibility change
                isOnlyValueChange = true;
            }

            //// same group, no sort order change, no visibility change
            if (isOnlyValueChange)
            {
                rcea = new RecordChangedEventArgs(r, RecordChangedType.Changed, newIndex, oldIndex, null, isNestedRelationParentKeyFieldAffected, te);
                rcea.GroupsChanged = isGroupSortBySummaryOrderAffected;
                rcea.IsNestedRelationParentKeyFieldAffected = isNestedRelationParentKeyFieldAffected;

                RaiseSourceListRecordChanging(rcea);

                //// Give current record a chance to reset itsself
                if (isCurrent)
                {
                    NotifyCurrentRecordListChanged(te);
                }

                //// Fix ManualTotalSummary of parent groups.
                OnRecordChanged(r.ParentSection, false, false); //// delta only

                if (supportCustomCounters)
                {
                    InvalidateCounters(r.ParentSection);
                }

                //// Invalidate summaries if record is not hidden
                if (isSummaryAffected && (didMeetFilterCriteria || this.summaryIgnoreRecordFilterCriteria))
                {
                    InvalidateSummaries(r, false);
                }
            }

            //// Support for Sort by Summary in caption - groups need to be reordered
            if (isGroupSortBySummaryOrderAffected || (didMeetFilterCriteria && groupSortBySummaryFields.Count > 0))
            {
                isGroupSortBySummaryOrderAffected = true;
                InvalidateParentGroupSortOrder(r.ParentGroup);
                if (oldParentSection != null)
                {
                    InvalidateParentGroupSortOrder(oldParentSection.ParentGroup);
                }
            }

            //// SourceListRecordChanging was raised before counters or summaries were invalidated.
 //// just a double check that there was no missing if-else branch.
            if (rcea == null)
            {
                throw new InvalidOperationException("SourceListRecordChanging event was not raised ...");
            }

            //// SourceListRecordChanged is now raised after counters or summaries were invalidated.
            RaiseSourceListRecordChanged(rcea);
        }

        private void AdjustIndexForChildTable(object sender, ref int newIndex, ref int oldIndex)
        {
            ChildTable senderChildTable = sender as ChildTable;
            if (senderChildTable != null)
            {
                int offset = senderChildTable.GetChildListPosition();
                if (offset != -1)
                {
                    if (newIndex != -1)
                    {
                        newIndex += offset;
                    }

                    if (oldIndex != -1)
                    {
                        oldIndex += offset;
                    }
                }
            }
        }

        void RaiseSourceListRecordChanging(RecordChangedEventArgs rcea)
        {
            if ((allowListChangedEvents & GroupingListChangedEvents.SourceListRecordChanging) != 0)
            {
                if (eventsTarget != null)
                {
                    eventsTarget.OnSourceListRecordChanging(rcea);
                }
                else
                {
                    OnSourceListRecordChanging(rcea);
                }
            }
        }

        void RaiseSourceListRecordChanged(RecordChangedEventArgs rcea)
        {
            if ((allowListChangedEvents & GroupingListChangedEvents.SourceListRecordChanged) != 0)
            {
                if (eventsTarget != null)
                {
                    eventsTarget.OnSourceListRecordChanged(rcea);
                }
                else
                {
                    OnSourceListRecordChanged(rcea);
                }
            }
        }

        void RaiseGroupRemoving(Group obsoleteGroup)
        {
            GroupEventArgs ge = new GroupEventArgs(obsoleteGroup);
            if (eventsTarget != null)
            {
                eventsTarget.OnGroupRemoving(ge);
            }
            else
            {
                OnGroupRemoving(ge);
            }
        }

        void InvalidateParentGroupSortOrder(Group group)
        {
            if (group != null)
            {
                GroupsDetails gd = group.ParentSection as GroupsDetails;
                while (gd != null)
                {
                    gd.GroupSortOrderDirty = true;
                    gd = gd.ParentSection as GroupsDetails;
                }
            }
        }

        /// <summary>
        /// This virtual method is called from the new ListChanged handler when a record was
        /// added, removed or changed. It is called after the record was detached or attached
        /// to a new group and before counters in parent elements are marked. Override this
        /// method if you want to update for example your custom manual summaries in parent groups
        /// as shown in ManualSummaries example.
        /// </summary>      
        protected virtual void OnRecordChanged(Element r, bool isObsoleteRecord, bool isAddedRecord)
        {
        }

        /// <summary>
        /// Check if any of the columns that was changed affects the sort position of the records.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is sort order affected] [the specified r]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSortOrderAffected(Record r, Record previous, Record next, SortColumnDescriptor[] sortDescriptors)
        {
            return IsSortOrderAffected(r, previous, next, sortDescriptors, null);
        }

        /// <summary>
        /// Check if any of the columns that was changed affects the sort position of the records.
        /// </summary>      
        /// <returns>
        /// <c>true</c> if [is sort order affected] [the specified r]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSortOrderAffected(Record r, Record previous, Record next, SortColumnDescriptor[] sortDescriptors, Hashtable changedFields)
        {
            TableDescriptor td = TableDescriptor;
            int cmp = 0;

            foreach (SortColumnDescriptor sd in sortDescriptors)
            {
                int fieldIndex = td.Fields.IndexOf(sd.FieldDescriptor);
                if (changedFields != null && !changedFields.Contains(fieldIndex))
                {
                    continue;
                }

                // If no ChangeFieldInfo was given we loop through all sort fields.

                // CompareColumnSortOrder returns:
                // -1 if column is not equal to previous or next value and sort order is valid;
                // 0 if column is equal to previous or next value;
                // 1 if column is not equal to previous or next value and sort order is invalid;
                cmp = CompareColumnSortOrder(r, previous, next, sd);

                if (cmp < 0)
                {
                    return false;
                }
                else if (cmp > 0)
                {
                    return true;
                }

                //// continue looking if cmp == 0
            }

            //// finally, check source index.

            if (previous != null && r.GetSourceIndex() < previous.GetSourceIndex())
            {
                return true;
            }

            if (next != null && r.GetSourceIndex() > next.GetSourceIndex())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the changed value of the column affects the sort position of the record.
        /// </summary>
        /// <param name="r">The current record</param>
        /// <param name="previous">The previous record</param>
        /// <param name="next">The next record</param>
        /// <param name="sd">The column to be tested</param>
        /// <returns>-1 if column is not equal to previous or next value and sort order is valid;
        /// 0 if column is equal to previous or next value;
        /// 1 if column is not equal to previous or next value and sort order is invalid;.</returns>
        public int CompareColumnSortOrder(Record r, Record previous, Record next, SortColumnDescriptor sd)
        {
            object value = r.GetValue(sd.Name);

            int result = -1;

            if (previous != null)
            {
                object prevValue = previous.GetValue(sd.Name);
                int cmp = CompareColumns.CompareNullableObjects(value, prevValue);
                if ((sd.SortDirection == ListSortDirection.Ascending && cmp < 0)
                    || (sd.SortDirection == ListSortDirection.Descending && cmp > 0))
                {
                    return 1;   // sort order is invalid
                }
                else if (cmp == 0)
                {
                    result = 0; // value is equal to previous value
                }
            }

            if (next != null)
            {
                object nextValue = next.GetValue(sd.Name);
                int cmp = CompareColumns.CompareNullableObjects(value, nextValue);
                //// TODO: check support for custom comparer ...
                ////SortColumnDescriptor sd = (SortColumnDescriptor) this.sortFields[sd.FieldIndex];
                ////int cmp = sd != null
                ////    ? SortColumnComparer.CompareNullableObjects(sd, value, nextValue)
                ////    : SortColumnComparer.CompareNullableObjects(value, nextValue);
                if ((sd.SortDirection == ListSortDirection.Ascending && cmp > 0)
                    || (sd.SortDirection == ListSortDirection.Descending && cmp < 0))
                {
                    return 1;   //// sort order is invalid
                }
                else if (cmp == 0)
                {
                    result = 0; //// value is equal to next value
                }
            }

            return result;
        }

        /// <summary>
        /// Record will be moved to a new group. Check if old group
        /// only had that one record and should be deleted. The method
        /// does not delete the group, only returns the group to be deleted.
        /// </summary>
        /// <param name="oldParentSection">Old parent section.</param>
        /// <param name="groupToDelete">Returns the group to delete.</param>
        /// <returns>Old group.</returns>
        public DetailsSection DeleteEmptyGroups(DetailsSection oldParentSection, out Group groupToDelete)
        {
            RecordsDetails rd = (RecordsDetails)oldParentSection;

            if (!(rd.ParentGroup is ChildTable) && rd.GetChildCount() <= 1)
            {
                groupToDelete = oldParentSection.ParentGroup;

                // Check also for parent groups that become obsolete.
                GroupsDetails ds = (GroupsDetails)groupToDelete.ParentSection;
                while (ds != null && !(ds.ParentGroup is ChildTable) && ds.Groups.Count <= 1)
                {
                    groupToDelete = ds.ParentGroup;
                    ds = (GroupsDetails)groupToDelete.ParentSection;
                }

                // Remove group and its child elements.
                oldParentSection = ds; // Use for InvalidateCountersAndSummaries below.
            }
            else
            {
                groupToDelete = null;
            }

            return oldParentSection;
        }

        /// <summary>
        /// Compare if record still matches group catecory criteria
        /// </summary>
        /// <param name="g">The Group.</param>
        /// <param name="r">The Record.</param>
        /// <returns>True if the record matches the group category; False otherwise.</returns>
        public bool CompareGroupCategories(Group g, Record r)
        {
            ArrayList al = new ArrayList();
            while (g != null)
            {
                al.InsertRange(0, g.CategoryKeys);
                g = g.ParentGroup;
            }

            bool matchesGroupCategories = true;
            for (int n = 0; matchesGroupCategories && n < al.Count; n++)
            {
                matchesGroupCategories &= CompareColumns.CompareNullableObjects(al[n], r.SortKeys[n]) == 0;
            }

            return matchesGroupCategories;
        }

        /// <summary>
        /// Invalidate summaries and counters for parent elements
        /// of this elements bottom up.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="forceCaptions">If true, resets the captions.</param>
        public void InvalidateCountersAndSummaries(Element el, bool forceCaptions)
        {
            // Important: First counters must be set dirty.
            InvalidateCounters(el);
            InvalidateSummaries(el, forceCaptions);
        }

        /// <summary>
        /// Invalidate counters for parent elements
        /// of this elements bottom up.
        /// </summary>
        /// <param name="el">The element</param>
        public void InvalidateCounters(Element el)
        {
            if (el == null)
            {
                return;
            }

            if (!CountersDirty)
            {
                el.InvalidateCounterBottomUp();
                el.InvalidateCounter();
            }
        }

        /// <summary>
        /// Invalidate summaries for parent elements
        /// of this elements bottom up.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="forceCaptions">If true, resets the captions.</param>
        public void InvalidateSummaries(Element el, bool forceCaptions)
        {
            if (el == null)
            {
                return;
            }

            if (!SummariesDirty && this.TableDescriptor.Summaries.Count > 0)
            {
                el.InvalidateSummariesBottomUp();
                el.InvalidateSummary();
            }
            else if (forceCaptions)
            {
                // Invalidate captions (Itemcount)
                Group g = el.ParentGroup;
                while (g != null)
                {
                    RaiseGroupSummaryInvalidated(g);
                    g = g.ParentGroup;
                }
            }
        }
        #endregion
        #region SortFields
        Hashtable sortFields = null;
        Hashtable pksortFields = null;
        Hashtable groupSortBySummaryFields = null;
        Hashtable summaryFields = null;
        Hashtable relationParentKeyFields = null;
        ////Hashtable summaryColumnFields = null;
        Hashtable dependencyFields = null;
        bool summaryIgnoreRecordFilterCriteria = false;
        private int startIndex;
        private int endIndex;

        public int StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                if (startIndex != value)
                {
                    this.startIndex = value;
                }
            }
        }

        public int EndIndex
        {
            get
            {
                return this.endIndex;
            }
            set
            {
                if (endIndex != value)
                {
                    this.endIndex = value;
                }
            }
        }
        void EnsureSortFields()
        {
            if (sortFields != null)
            {
                return;
            }

            TableDescriptor td = this.TableDescriptor;

            int fieldCount = td.Fields.Count; //// trigger EnsureInitialized.
            int pkCount = td.PrimaryKeyColumns.Count; //// trigger EnsureInitialized.

            supportCustomCounters = this.Engine.CounterLogic == EngineCounters.All && !WithoutCounter;
            pksortFields = new Hashtable();
            sortFields = new Hashtable();
            summaryFields = new Hashtable();
            groupSortBySummaryFields = new Hashtable();
            dependencyFields = new Hashtable();
            relationParentKeyFields = new Hashtable();
            ////summaryColumnFields = new Hashtable();

            foreach (SortColumnDescriptor sd in td.RelationChildColumns)
            {
                sortFields[td.Fields.IndexOf(sd.Name)] = sd;
            }

            foreach (SortColumnDescriptor sd in td.GroupedColumns)
            {
                sortFields[td.Fields.IndexOf(sd.Name)] = sd;
            }

            foreach (SortColumnDescriptor sd in td.SortedColumns)
            {
                sortFields[td.Fields.IndexOf(sd.Name)] = sd;
            }

            foreach (SortColumnDescriptor sd in td.PrimaryKeyColumns)
            {
                pksortFields[td.Fields.IndexOf(sd.Name)] = sd;
            }

            foreach (RelationDescriptor rd in td.Relations)
            {
                if (rd.RelationKind == RelationKind.RelatedMasterDetails)
                {
                    foreach (RelationKeyDescriptor rkd in rd.RelationKeys)
                    {
                        relationParentKeyFields[td.Fields.IndexOf(rkd.ParentKeyFieldName)] = rd;
                    }
                }
            }

            summaryIgnoreRecordFilterCriteria = false;
            foreach (SummaryDescriptor summary in td.Summaries)
            {
                summaryFields[td.Fields.IndexOf(summary.MappingName)] = summary;
                summaryIgnoreRecordFilterCriteria |= summary.IgnoreRecordFilterCriteria;
            }

            OnEnsureSortFields();

            // Sort By Summary In Caption ...
            foreach (SortColumnDescriptor sd in td.GroupedColumns)
            {
                ////GroupSortOrderSummaryComparer cmpObj = sd.GroupSortOrderComparer as GroupSortOrderSummaryComparer;
                ////if (cmpObj != null)
                ////{
                ////    SummaryDescriptor summary = td.Summaries[cmpObj.SummaryDescriptorName];
                ////    groupSortBySummaryFields[td.Fields.IndexOf(summary.MappingName)] = sd;
                ////}
                IGroupSortOrderComparer gsoc = sd.GroupSortOrderComparer as IGroupSortOrderComparer;
                if (gsoc != null)
                {
                    string[] fields = gsoc.GetDependantFields(TableDescriptor);
                    foreach (string mappingName in fields)
                    {
                        groupSortBySummaryFields[td.Fields.IndexOf(mappingName)] = gsoc;
                    }
                }
            }

            ////
            //// Dependant fields
            ////
            //// - A Unbound field that depends on others
            //// - A expression field that depends on others
            //// - A foreign table display field that depends on a foreign key.
            ////
            //// Loop through all fields, get list of fields it depends on.
            //// The loop through the list of fields it depends on and
            //// create a quick lookup from that field to the original field.
            ////
            //// Example:
            //// expr1.ReferencedFields("field1;field2")
            //// expr2.ReferencedFields("field2;field3")
            ////
            //// will result in
            //// dependencyFields[field1] = expr1
            //// dependencyFields[field2] = expr1;expr2
            //// dependencyFields[field3] = expr2
            ////

            Hashtable tempdependencyFields = new Hashtable();
            foreach (FieldDescriptor fd in td.Fields)
            {
                int n = td.Fields.IndexOf(fd);
                string rf = fd.ReferencedFields;
                if (rf.Length == 0)
                {
                    continue;
                }

                foreach (string s in rf.Split(';'))
                {
                    Hashtable fields;
                    int index = td.Fields.IndexOf(s);
                    if (tempdependencyFields.Contains(index))
                    {
                        fields = (Hashtable)tempdependencyFields[index];
                    }
                    else
                    {
                        fields = new Hashtable();
                        tempdependencyFields[index] = fields;
                    }

                    fields[n] = fd;
                }
            }

            // Convert Hashtables to int[] arrays.
            foreach (DictionaryEntry de in tempdependencyFields)
            {
                Hashtable fields;
                fields = (Hashtable)de.Value;
                int[] parts = new int[fields.Count];
                fields.Keys.CopyTo(parts, 0);
                this.dependencyFields[de.Key] = parts;
            }
        }

        /// <exclude/>
        protected virtual void OnEnsureSortFields()
        {
            ////foreach (GridSummaryRowDescriptor summaryRow in td.SummaryRows)
            ////{
            ////    foreach (GridSummaryColumnDescriptor summaryColumn in summaryRow.SummaryColumns)
            ////    {
            ////        int fieldIndex = td.Fields.IndexOf(summaryColumn.DataMember);
            ////        GridSummaryColumnDescriptorCollection al;
            ////        if (summaryColumnFields.Contains(fieldIndex))
            ////            al = summaryColumnFields[fieldIndex] as GridSummaryColumnDescriptorCollection;
            ////        else
            ////        {
            ////            al = new GridSummaryColumnDescriptorCollection();
            ////            summaryColumnFields[fieldIndex] = al;
            ////        }
            ////        al.Add(summaryColumn);
            ////    }
            ////}
        }

        /// <summary>
        /// Raises the <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizingRecords(TableEventArgs e)
        {
            this.EnsureSortFields();

            BaseCategorizingRecords(e);
        }

        /// <summary>
        /// Returns an array of field indexes that are dependant on changes to this field because
        /// they reference this field (e.g. an unbound field that is based on this value or
        /// a Expression field that references this field).
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>Array of field indexes.</returns>
        public int[] GetDependantFields(int fieldIndex)
        {
            if (dependencyFields.Contains(fieldIndex))
            {
                return (int[])dependencyFields[fieldIndex];
            }

            return new int[0];
        }

        ////public GridSummaryColumnDescriptorCollection GetSummaryColumnCollection(int fieldIndex)
        ////{
        ////    if (summaryColumnFields.Contains(fieldIndex))
        ////        return summaryColumnFields[fieldIndex] as GridSummaryColumnDescriptorCollection;

        ////    return new GridSummaryColumnDescriptorCollection();
        ////}
        #endregion
    }

    enum GroupingListChangedEvents
    {
        /// <summary>
        /// Represents None
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents SourceListListChanged
        /// </summary>
        SourceListListChanged = 0x1,

        /// <summary>
        /// Represents SourceListListChangedCompleted
        /// </summary>
        SourceListListChangedCompleted = 0x2,

        /// <summary>
        /// Represents SourceListRecordChanging 
        /// </summary>
        SourceListRecordChanging = 0x4,

        /// <summary>
        /// Represents SourceListRecordChanged
        /// </summary>
        SourceListRecordChanged = 0x8
    }

#if MEASURE
    public class MeasureTime : IDisposable
    {
        static Hashtable times = new Hashtable();

        int ticks = 0;
        string id = "";

        public MeasureTime(string id)
        {
            this.id = id;
            ticks = Environment.TickCount;
        }

        public static MeasureTime Measure(string id)
        {
            return new MeasureTime(id);
        }

        public void Dispose()
        {
            int time = Environment.TickCount-ticks;
            if (times.ContainsKey(id))
                times[id] = ((int) times[id]) + time;
            else
                times[id] = time;
        }

        public static string DumpTimes()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("MeasureTime results:");
            ArrayList al = new ArrayList();
            foreach (DictionaryEntry d in times)
            {
                al.Add(String.Format("{0}: {1}\r\n", d.Key, d.Value));
            }
            al.Sort();
            foreach (string s in al)
                sb.Append(s);
            times.Clear();
            return sb.ToString();
        }

    }
#endif

    /// <summary>
    /// Provides a <see cref="GetTable"/> method that returns a <see cref="Syncfusion.Grouping.Table"/>
    /// </summary>
    public interface ITableProvider
    {
        /// <summary>
        /// Returns a <see cref="Table"/>
        /// </summary>
        /// <returns>returns Table</returns>
        Table GetTable();
    }
    
#if SyncfusionFramework2_0
    /// <exclude/>
    public class Int32ToStringDictionary : System.Collections.Generic.Dictionary<int, string>
    {
    }
#else
    /// <exclude/>
    public class Int32ToStringDictionary : Hashtable
    {
        public string this[int index]
        {
            get
            {
                return (string) base[index];
            }
            set
            {
                base[index] = value;
            }
        }
    }
#endif
    
    /// <summary>
    /// The Collection with detected changes in the datasource when a ListChanged event
    /// is handled.
    /// </summary>
    public class ChangedFieldInfoCollection : ArrayList
    {
        /// <summary>
        /// Gets or sets the ChangeFieldInfo at given index.
        /// </summary>
        /// <param name="index">Index to identify the requested item.</param>
        public new ChangedFieldInfo this[int index]
        {
            get
            {
                return (ChangedFieldInfo)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }

    /// <summary>
    /// Provides details about the changes made to a column at the time
    /// the ListChanged event is handled in the engine. 
    /// </summary>
    public class ChangedFieldInfo
    {
        TableDescriptor tableDescriptor;
        string name;
        double delta;
        object oldValue;
        object newValue;
        int fieldIndex = -1;
        bool hasDelta = false;
        bool hasValue = false;
        
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Grouping.ChangedFieldInfo">ChangedFieldInfo</see> class.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="name">Field name.</param>
        public ChangedFieldInfo(TableDescriptor tableDescriptor, string name)
        {
            this.name = name;
            this.tableDescriptor = tableDescriptor;
            this.hasValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Grouping.ChangedFieldInfo">ChangedFieldInfo</see> class. 
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="name">The Field name.</param>
        /// <param name="oldValue">The Old value.</param>
        /// <param name="newValue">The New value</param>
        /// <param name="delta">Difference between new and old values.</param>
        public ChangedFieldInfo(TableDescriptor tableDescriptor, string name, object oldValue, object newValue, double delta)
        {
            this.name = name;
            this.tableDescriptor = tableDescriptor;
            this.delta = delta;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.hasDelta = true;
            this.hasValue = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Grouping.ChangedFieldInfo">ChangedFieldInfo</see> class. 
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="name">Field name.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public ChangedFieldInfo(TableDescriptor tableDescriptor, string name, object oldValue, object newValue)
        {
            this.name = name;
            this.tableDescriptor = tableDescriptor;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.hasValue = true;
        }

        /// <summary>
        /// Gets the table descriptor.
        /// </summary>
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the difference between new and old values.
        /// </summary>
        public double Delta
        {
            get
            {
                if (!hasDelta)
                {
                    delta = Convert.ToDouble(newValue) - Convert.ToDouble(oldValue);
                    hasDelta = true;
                }

                return delta;
            }
        }

        /// <summary>
        /// Gets or sets old value.
        /// </summary>
        public object OldValue
        {
            get
            {
                return oldValue;
            }

            set
            {
                oldValue = value;
                hasDelta = false;
                hasValue = true;
            }
        }

        /// <summary>
        /// Gets or sets new value.
        /// </summary>
        public object NewValue
        {
            get
            {
                return newValue;
            }

            set
            {
                newValue = value;
                hasDelta = false;
                hasValue = true;
            }
        }

        /// <summary>
        /// Specifies whether it has a value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return hasValue;
            }
        }

        /// <summary>
        /// Sets old and new values.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public void SetValues(object oldValue, object newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.hasValue = true;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        public int FieldIndex
        {
            get
            {
                if (fieldIndex == -1)
                {
                    fieldIndex = TableDescriptor.Fields.IndexOf(Name);
                }

                return fieldIndex;
            }
        }
    }
    
    /// <summary>
    /// Defines an interface to be used for the <see cref="SortColumnDescriptor.GroupSortOrderComparer"/>
    /// of a <see cref="SortColumnDescriptor"/>. The GetDependantFields method is called from the engine
    /// to determine the fields a GroupSortOrder is dependant on. When the engine gets notified of changes
    /// from the underlying datasource it will loop through the modified fields of a record and see if any of the modified
    /// field names matches the names returned by this methods and in such case recalculate the group sort order.
    /// GetDependantFields is only called once when the table is initialized and the results array is then 
    /// internally cached.
    /// </summary>
    public interface IGroupSortOrderComparer : IComparer
    {
        /// <summary>
        /// The GetDependantFields method is called from the engine
        /// to determine the fields a GroupSortOrder is dependant on. When the engine gets notified of changes
        /// from the underlying datasource it will loop through the modified fields of a record and see if any of the modified
        /// field names matches the names returned by this methods and in such case recalculate the group sort order.
        /// GetDependantFields is only called once when the table is initialized and the results array is then 
        /// internally cached.
        /// </summary>
        /// <param name="td">The TableDescriptor</param>
        /// <returns>returns dependant fields</returns>
        string[] GetDependantFields(TableDescriptor td);
    }
}