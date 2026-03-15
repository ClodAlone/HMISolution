//-------------------------------------------------------------------------------------------------
// <copyright file="Events.cs" company="syncfusion">
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
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    // eva QueryRecordMeetsFilterCriteria SyncfusionHandled Record record bool result

    /// <summary>
    /// Represents the method that handles <see cref="Engine.QueryRecordMeetsFilterCriteria"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="QueryRecordMeetsFilterCriteriaEventArgs"/> that contains the event data.</param>
    public delegate void QueryRecordMeetsFilterCriteriaEventHandler(object sender, QueryRecordMeetsFilterCriteriaEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="Engine.QueryRecordMeetsFilterCriteria"/> event which
    /// occurs when a record is checked whether it meets filter criteria and should appear visible in the tables DisplayElements.
    /// </summary>
    public sealed class QueryRecordMeetsFilterCriteriaEventArgs : SyncfusionHandledEventArgs
    {
        Record record;
        bool result;

        /// <summary>
        /// Initializes the new object with a record and result.
        /// </summary>
        /// <param name="record">The record to be tested.</param>
        /// <param name="result">The default value for the <see cref="Result"/>.</param>
        public QueryRecordMeetsFilterCriteriaEventArgs(Record record, bool result)
        {
            this.record = record;
            this.result = result;
        }

        /// <summary>
        /// Gets the record to be tested.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// Gets / sets the result. True if record meets criteria; False otherwise.
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    // eva DescriptorPropertyChanged Syncfusion string property EventArgs inner

    /// <summary>
    /// Represents a method that handles a PropertyChanged or PropertyChanging event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void DescriptorPropertyChangedEventHandler(object sender, DescriptorPropertyChangedEventArgs e);

    /// <summary>
    /// Provides data for PropertyChanged or PropertyChanging events which
    /// occur when a property is changed.
    /// </summary>
    public sealed class DescriptorPropertyChangedEventArgs : SyncfusionEventArgs
    {
        string property;
        EventArgs inner;

        /// <summary>
        /// Initializes the new object with a property name and an inner EventArgs object.
        /// </summary>
        /// <param name="property">The name of the property was changed.</param>
        /// <param name="inner">An inner EventArgs object with more detailed information about a nested event. If for example the fields collection 
        /// is changed, the table object will raise a PropertyChanged event and use the original PropertyListChangedEventArgs as inner property.</param>
        public DescriptorPropertyChangedEventArgs(string property, EventArgs inner)
        {
            this.property = property;
            this.inner = inner;
        }

        /// <summary>
        /// Initializes the new object with a property name.
        /// </summary>
        /// <param name="property">The name of the property was changed.</param>
        public DescriptorPropertyChangedEventArgs(string property)
        {
            this.property = property;
        }

        /// <summary>
        /// The name of the property was changed.
        /// </summary>
        [TraceProperty(true)]
        public string PropertyName
        {
            get
            {
                return property;
            }
        }

        /// <summary>
        /// Gets the inner EventArgs object with more detailed information about a nested event. If for example the fields collection 
        /// is changed, the Table object will raise a PropertyChanged event and use the original PropertyListChangedEventArgs as inner property.
        /// </summary>
        [TraceProperty(true)]
        public EventArgs Inner
        {
            get
            {
                return inner;
            }
        }

        /// <summary>
        /// Checks if this object contains event data about a "Relations" property of a TableDescriptor and if the event 
        /// was raised from a nested ChildTableDescriptor within the Relation collection. <para/>
        /// In such case, the method returns the EventArgs
        /// for the original event within that nested ChildTableDescriptor. <para/>
        /// For example, if a column was changed in a nested table descriptor, this method will return a reference to the TableDescriptor and the 
        /// ColumnsChanged event data. You can analyze the event data whether just a width for the column has changed or
        /// if other settings were changed.
        /// </summary>
        /// <param name="td">Table Descriptor.</param>
        /// <returns>The event args.</returns>
        public DescriptorPropertyChangedEventArgs GetNestedChildTableDescriptorEvent(ref TableDescriptor td)
        {
            DescriptorPropertyChangedEventArgs e = this;

            if (e.PropertyName != "Relations")
            {
                return e;
            }

            ListPropertyChangedEventArgs rla = e.Inner as ListPropertyChangedEventArgs;
            if (rla != null)
            {
                //// relationListArgs.Item: RelationDescriptor
                //// relationListArgs.Inner: DescriptorPropertyChangedEventArgs (reason why RelationDescriptor was changed)

                if (rla.Action == ListPropertyChangedType.ItemPropertyChanged)
                {
                    RelationDescriptor rd = (RelationDescriptor)rla.Item;

                    DescriptorPropertyChangedEventArgs rlai = rla.Inner as DescriptorPropertyChangedEventArgs;
                    if (rlai.PropertyName == "ChildTableDescriptor")
                    {
                        td = rd.ChildTableDescriptor;

                        return ((DescriptorPropertyChangedEventArgs)rlai.Inner).GetNestedChildTableDescriptorEvent(ref td);
                    }
                }
            }

            return e;
        }
    }

    ////    public sealed class ElementEventArgs : SyncfusionCancelEventArgs 
    ////    {
    ////        Element element;
    ////    
    ////        public ElementEventArgs(Element element) 
    ////        {
    ////            this.element = element;
    ////        }
    ////    
    ////        [TraceProperty(true)]
    ////        public Element Element
    ////        {
    ////            get
    ////            {
    ////                return element;
    ////            }
    ////            set
    ////            {
    ////                element = value;
    ////            }
    ////        }
    ////    }
    ////    
    ////    public delegate void ElementEventHandler(object sender, ElementEventArgs e);

    /// <summary>
    /// Provides data for events that occur on a record level such as RecordCollapsing, RecordCollapsed,
    /// RecordExpanding, and RecordExpanded.
    /// </summary>
    public sealed class RecordEventArgs : SyncfusionCancelEventArgs
    {
        Record record;
        bool raiseDisplayElementChanged = true;

        /// <summary>
        /// Initializes a new object with a reference to a record.
        /// </summary>
        /// <param name="record">The affected record.</param>
        public RecordEventArgs(Record record)
        {
            this.record = record;
        }

        /// <summary>
        /// Gets the affected record.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        [TraceProperty(true)]
        internal bool RaiseDisplayElementChanged
        {
            get
            {
                return raiseDisplayElementChanged;
            }

            set
            {
                raiseDisplayElementChanged = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="RecordEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void RecordEventHandler(object sender, RecordEventArgs e);

    /// <summary>
    /// Provides data for events that occur on a group level such as GroupCollapsing, GroupCollapsed,
    /// GroupExpanding, and GroupExpanded.
    /// </summary>
    public sealed class GroupEventArgs : SyncfusionCancelEventArgs
    {
        Group group;

        /// <summary>
        /// Initializes a new object with a reference to a group.
        /// </summary>
        /// <param name="group">The affected group.</param>
        public GroupEventArgs(Group group)
        {
            this.group = group;
        }

        /// <summary>
        /// Gets the affected group.
        /// </summary>
        [TraceProperty(true)]
        public Group Group
        {
            get
            {
                return group;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="GroupEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void GroupEventHandler(object sender, GroupEventArgs e);

    /// <summary>
    /// Specifies how the record list changed.
    /// </summary>
    /// <remarks>
    /// Used by the Action property of the RecordChangedEventArgs class to indicate the way records in a table change.
    /// </remarks>
    public enum RecordChangedType
    {
        /// <summary>
        /// A record was added to the datasource.
        /// </summary>
        Added,

        /// <summary>
        /// A record was removed from the datasource.
        /// </summary>
        Removed,

        /// <summary>
        /// One or multiple record fields were changed.
        /// </summary>
        Changed,

        /// <summary>
        /// A record was moved to a new position in the underlying data source.
        /// </summary>
        Moved
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="RecordChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void RecordChangedEventHandler(object sender, RecordChangedEventArgs e);
    
    /// <summary>
    /// Provides data for SourceListRecordChanged and SourceListRecordChanging events that occur 
    /// when a record in the underlying data source was added, removed, or changed.
    /// </summary>
    public sealed class RecordChangedEventArgs : SyncfusionCancelEventArgs
    {
        Record record;
        RecordChangedType action;
        int newIndex;
        int oldIndex;
        bool raiseDisplayElementChanged = true;
        Group group;
        Group addedGroup;
        Group removedGroup;
        bool sortAffected = true;
        bool visibiltyAffected = false;
        bool groupsAffected = false;
        TableListChangedEventArgs inner;
        object reserved;
        bool isNestedRelationParentKeyFieldAffected = false;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="record">The affected record.</param>
        /// <param name="action">Specifies how the record list changed.</param>
        /// <param name="newIndex">The new record index in the underlying data source; can be -1.</param>
        /// <param name="oldIndex">The old record index in the underlying data source; can be -1.</param>
        public RecordChangedEventArgs(Record record, RecordChangedType action, int newIndex, int oldIndex)
        {
            this.record = record;
            this.action = action;
            this.newIndex = newIndex;
            this.oldIndex = oldIndex;
        }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="record">The affected record.</param>
        /// <param name="action">Specifies how the record list changed.</param>
        /// <param name="newIndex">The new record index in the underlying data source; can be -1.</param>
        /// <param name="oldIndex">The old record index in the underlying data source; can be -1.</param>
        /// <param name="group">When a record is removed and a parent group needs to be removed, gets the group that
        /// is going to be removed.</param>
        public RecordChangedEventArgs(Record record, RecordChangedType action, int newIndex, int oldIndex, Group group)
        {
            this.record = record;
            this.action = action;
            this.newIndex = newIndex;
            this.oldIndex = oldIndex;
            this.group = group;
        }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="record">The affected record.</param>
        /// <param name="action">Specifies how the record list changed.</param>
        /// <param name="newIndex">The new record index in the underlying data source; can be -1.</param>
        /// <param name="oldIndex">The old record index in the underlying data source; can be -1.</param>
        /// <param name="group">When a record is removed and a parent group needs to be removed, gets the group that
        /// is going to be removed.</param>
        /// <param name="sortAffected">Specifies if the sorted position of the changed record was changed.</param>
        public RecordChangedEventArgs(Record record, RecordChangedType action, int newIndex, int oldIndex, Group group, bool sortAffected)
        {
            this.record = record;
            this.action = action;
            this.newIndex = newIndex;
            this.oldIndex = oldIndex;
            this.group = group;
            this.sortAffected = sortAffected;
        }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="record">The affected record.</param>
        /// <param name="action">Specifies how the record list changed.</param>
        /// <param name="newIndex">The new record index in the underlying data source; can be -1.</param>
        /// <param name="oldIndex">The old record index in the underlying data source; can be -1.</param>
        /// <param name="group">When a record is removed and a parent group needs to be removed, gets the group that
        /// is going to be removed.</param>
        /// <param name="sortAffected">Specifies if the sorted position of the changed record was changed.</param>
        public RecordChangedEventArgs(Record record, RecordChangedType action, int newIndex, int oldIndex, Group group, bool sortAffected, TableListChangedEventArgs inner)
        {
            this.record = record;
            this.action = action;
            this.newIndex = newIndex;
            this.oldIndex = oldIndex;
            this.group = group;
            this.sortAffected = sortAffected;
            this.TableListChangedEventArgs = inner;
        }

        /// <summary>
        ///  Gets / sets if a property was changed that is the parent field of a master details relation.
        /// </summary>
        public bool IsNestedRelationParentKeyFieldAffected
        {
            get
            {
                return this.isNestedRelationParentKeyFieldAffected;
            }

            set
            {
                this.isNestedRelationParentKeyFieldAffected = value;
            }
        }

        /// <summary>
        /// Gets / sets if a DisplayElementChanged event should be raised after this event's handler returns.
        /// </summary>
        [TraceProperty(true)]
        public bool RaiseDisplayElementChanged
        {
            get
            {
                return raiseDisplayElementChanged;
            }

            set
            {
                raiseDisplayElementChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets the TableListChanged Event Args.
        /// </summary>
        public TableListChangedEventArgs TableListChangedEventArgs
        {
            get
            {
                return inner;
            }

            set
            {
                inner = value;
            }
        }

        /// <summary>
        /// Gets if the sorted position of the changed record was changed.
        /// </summary>
        [TraceProperty(true)]
        public bool SortedPositionChanged
        {
            get
            {
                return sortAffected;
            }
        }
        
        /// <summary>
        /// Gets if the changed record visibility was changed (filter criteria meet / not meet criteria).
        /// </summary>
        [TraceProperty(true)]
        public bool VisibilityChanged
        {
            get
            {
                return this.visibiltyAffected;
            }

            set
            {
                this.visibiltyAffected = value;
            }
        }

        /// <summary>
        /// Specfies if the changed record forced changes to parent groups (move record to new group, create/delete group).
        /// </summary>
        [TraceProperty(true)]
        public bool GroupsChanged
        {
            get
            {
                return this.groupsAffected;
            }

            set
            {
                this.groupsAffected = value;
            }
        }

        /// <summary>
        /// The affected record.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// Returns the group which visibility is affected when a record is removed or added (if the affected
        /// record that is hidden is the only record or if a new group becomes visible which can also happen
        /// when a child record meets or fails to meet filter criteria) 
        /// </summary>
        [TraceProperty(true)]
        public Group Group
        {
            get
            {
                return group;
            }

            set
            {
                group = value;
            }
        }

        /// <summary>
        /// Returns the top-most group when a record was added with a new category
        /// </summary>
        [TraceProperty(true)]
        public Group AddedGroup
        {
            get
            {
                return addedGroup;
            }

            set
            {
                addedGroup = value;
            }
        }

        /// <summary>
        /// When a record is removed and a parent group needs to be removed, gets the group that
        /// is going to be removed.
        /// </summary>
        [TraceProperty(true)]
        public Group RemovedGroup
        {
            get
            {
                return removedGroup;
            }

            set
            {
                removedGroup = value;
            }
        }

        /// <summary>
        /// Specifies how the record list changed.
        /// </summary>
        [TraceProperty(true)]
        public RecordChangedType Action
        {
            get
            {
                return action;
            }
        }

        /// <summary>
        /// The new record index in the underlying datasource; can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int NewIndex
        {
            get
            {
                return newIndex;
            }
        }

        /// <summary>
        /// The old record index in the underlying datasource; can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int OldIndex
        {
            get
            {
                return oldIndex;
            }
        }

        /// <exclude/>
        /// <summary>
        /// Used internally for view information when removing records.
        /// </summary>
        public object Reserved
        {
            get
            {
                return reserved;
            }

            set
            {
                reserved = value;
            }
        }
    }

    /// <summary>
    /// Provides data for the ExceptionRaised event that 
    /// occurs when an unknown exception has been cached while modifying underlying data in the datasource.
    /// </summary>
    /// <remarks>
    /// If necessary, you can rethrow the exception in your event handler.
    /// </remarks>
    public sealed class ExceptionRaisedEventArgs : TableEventArgs
    {
        string method;
        Exception exception;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="table">The table object that cached the exception.</param>
        /// <param name="method">The name of the method that cached the exception.</param>
        /// <param name="exception">The exception.</param>
        public ExceptionRaisedEventArgs(Table table, string method, Exception exception)
            : base(table)
        {
            this.method = method;
            this.exception = exception;
        }

        /// <summary>
        /// The name of the method that cached the exception.
        /// </summary>
        [TraceProperty(true)]
        public string Method
        {
            get
            {
                return method;
            }
        }

        /// <summary>
        /// The exception.
        /// </summary>
        [TraceProperty(true)]
        public Exception Exception
        {
            get
            {
                return exception;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="ExceptionRaisedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void ExceptionRaisedEventHandler(object sender, ExceptionRaisedEventArgs e);

    /// <summary>
    /// Represents a method that handles an event with <see cref="SelectedRecordsChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void SelectedRecordsChangedEventHandler(object sender, SelectedRecordsChangedEventArgs e);

    /// <summary>
    /// Specifies the kind of action for SelectedRecordsChangedEventArgs
    /// </summary>
    public enum SelectedRecordsChangedType
    {
        /// <summary>
        /// A record is/was added to the SelectedRecords collection 
        /// </summary>
        Added,

        /// <summary>
        /// A record is/was removed from the SelectedRecords collection 
        /// </summary>
        Removed,

        /// <summary>
        /// The SelectedRecords collection is/was cleared.
        /// </summary>
        Reset
    }

    /// <summary>
    /// Provides event data for the <see cref="Syncfusion.Grouping.Table.SelectedRecordsChanged"/> and <see cref="Syncfusion.Grouping.Table.SelectedRecordsChanging"/>
    /// event which occur when the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection is/was modified.
    /// </summary>
    public sealed class SelectedRecordsChangedEventArgs : SyncfusionCancelEventArgs
    {
        Table table;
        SelectedRecordsChangedType action;
        SelectedRecord selectedRecord;

        /// <summary>
        /// Initializes a new SelectedRecordsChangedEventArgs object.
        /// </summary>
        /// <param name="table">The table the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection belongs to.</param>
        /// <param name="action">Specifies the kind of action that occured:
        /// <list type="bullet">
        /// <item><term>Added: A record is/was added to the SelectedRecords collection</term></item>
        /// <item><term>Deleted: A record is/was removed from the SelectedRecords collection </term></item>
        /// <item><term>Reset: The SelectedRecords collection is/was cleared.</term></item>
        /// </list></param>
        /// <param name="selectedRecord">The affected record</param>
        public SelectedRecordsChangedEventArgs(Table table, SelectedRecordsChangedType action, SelectedRecord selectedRecord)
        {
            this.table = table;
            this.action = action;
            this.selectedRecord = selectedRecord;
        }

        /// <summary>
        /// The table the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection belongs to.
        /// </summary>
        [TraceProperty(true)]
        public Table Table
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        /// Specifies the kind of action that occured:
        /// <list type="bullet">
        /// <item><term>Added: A record is/was added to the SelectedRecords collection</term></item>
        /// <item><term>Deleted: A record is/was removed from the SelectedRecords collection </term></item>
        /// <item><term>Reset: The SelectedRecords collection is/was cleared.</term></item>
        /// </list>
        /// </summary>
        [TraceProperty(true)]
        public SelectedRecordsChangedType Action
        {
            get
            {
                return action;
            }
        }

        /// <summary>
        /// The affected record
        /// </summary>
        [TraceProperty(true)]
        public SelectedRecord SelectedRecord
        {
            get
            {
                return selectedRecord;
            }
        }
    }

    /// <summary>
    /// Provides data for the SourceListListChanged event that
    /// occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
    /// of an attached source list. More detailed <see cref="Syncfusion.Grouping.Table.SourceListRecordChanged"/> events will be
    /// raised after this event.
    /// </summary>
    /// <remarks>
    /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
    /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
    /// event.
    /// </remarks>
    public class TableListChangedEventArgs : ListChangedEventArgs
    {
        Table table;
        bool shouldInvalidateCounters = true;
        bool shouldInvalidateSummaries = true;
        bool shouldResetCurrentRecord = true;
        bool navigateCurrentRecordWhenDeleted = false;
        bool shouldInvalidateScreen = true;
        bool shouldInvalidateGroupSortOrder = true;
        bool shouldReevaluateSortPosition = false;
        bool shouldIgnoreReset = false;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="table">The table object.</param>
        /// <param name="listChangedType">Gets the way that the list changed.</param>
        /// <param name="newIndex">Gets the new index of the item in the list.</param>
        /// <param name="oldIndex">Gets the old index of the item in the list.</param>
        public TableListChangedEventArgs(Table table, ListChangedType listChangedType, int newIndex, int oldIndex)
            : base(listChangedType, newIndex, oldIndex)
        {
            this.table = table;
        }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="table">The table object.</param>
        /// <param name="listChangedType">Gets the way that the list changed.</param>
        /// <param name="newIndex">Gets the new index of the item in the list.</param>
        /// <param name="propDesc">The PropertyDescriptor describing the item.</param>
        public TableListChangedEventArgs(Table table, ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc)
            : base(listChangedType, newIndex, propDesc)
        {
            this.table = table;
        }

        /// <summary>
        /// Creates a TableListChangedEventArgs object.
        /// </summary>
        /// <param name="table">The table object.</param>
        /// <param name="listChangedType">Gets the way that the list changed.</param>
        /// <param name="newIndex">Gets the new index of the item in the list.</param>
        /// <param name="oldIndex">Gets the old index of the item in the list.</param>
        /// <param name="propDesc">The PropertyDescriptor describing the item.</param>
        /// <returns>The event args.</returns>
        public static TableListChangedEventArgs Create(Table table, ListChangedType listChangedType, int newIndex, int oldIndex, PropertyDescriptor propDesc)
        {
            if (propDesc != null)
            {
                return new TableListChangedEventArgs(table, listChangedType, newIndex, propDesc);
            }
            else
            {
                return new TableListChangedEventArgs(table, listChangedType, newIndex, oldIndex);
            }
        }

        /// <summary>
        /// The table object.
        /// </summary>
        [TraceProperty(true)]
        public Table Table
        {
            get
            {
                return table;
            }

            set
            {
                table = value;
            }
        }

        /// <summary>
        /// Lets you specify whether counters need to be marked dirty when
        /// a ListChanged event is handled. By default, the table does not know
        /// whether fields that are changes in a record will affect the counter
        /// logic and therefore will always mark all counters dirty from the bottom up. 
        /// This has performance implications for the next time you need position information
        /// about a record (or simply when you call InvalidateRange and the YAmount
        /// counter is accessed).
        /// </summary>
        /// <remarks>
        /// When counters are not marked dirty, subsequent operations that need record position
        /// or y-amount position will be much faster. Counters only need to be marked dirty when there is a chance
        /// that the sort order is affected or if filter / hidden state of a record is 
        /// affected or if custom counters are used.
        /// </remarks>
        public bool ShouldInvalidateCounters
        {
            get
            {
                return shouldInvalidateCounters;
            }

            set
            {
                shouldInvalidateCounters = value;
            }
        }

        /// <summary>
        /// Lets you specify whether the grid should repaint the record when changes to the record
        /// were made in the underlying datasource and a ListChangedType.ItemChanged notification was
        /// raised. The default value for ShouldInvalidateScreen is true.
        /// </summary>
        public bool ShouldInvalidateScreen
        {
            get
            {
                return shouldInvalidateScreen;
            }

            set
            {
                shouldInvalidateScreen = value;
            }
        }

        /// <summary>
        /// Lets you change whether the a ListChangedType.Reset notification should be ignored. If
        /// you set ShouldIgnoreReset = true the table will not set itsself dirty and also not repaint the screen.
        /// Normally, a listener on a DataView needs to completely reinitialize itsself when ListChangedType.Reset 
        /// is raised beause the underlying datasource might have been sorted or othwerwise changes. Sometimes
        /// however you may want to call CurrencyManager.Reset or call DataTable.AcceptChanges without 
        /// reinitializing the grid. In such case this property will help you avoid delays. The default value for ShouldIgnoreReset is false.
        /// </summary>
        /// <remarks>
        /// There is an exception when the grid will alwyas handle the Reset event and ignores this setting
        /// when the grid detects that the record count it has cached differs from the record count of the 
        /// underlying source list. In that case the table must be marked dirty to avoid the data and the
        /// table to go out of sync.
        /// </remarks>
        public bool ShouldIgnoreReset
        {
            get { return shouldIgnoreReset; }
            set { shouldIgnoreReset = value; }
        }
        
        /// <summary>
        /// Lets you specify whether summaries need to be marked dirty when
        /// a ListChanged event is handled. By default, the table does not know
        /// whether fields that are changed in a record will affect the summaries
        /// logic and therefore will always mark all summaries dirty from the bottom up. 
        /// </summary>
        public bool ShouldInvalidateSummaries
        {
            get
            {
                return shouldInvalidateSummaries;
            }

            set
            {
                shouldInvalidateSummaries = value;
            }
        }

        /// <summary>
        /// Lets you specify whether the current record should be reset
        /// when an ItemChanged notification is received for the current record.
        /// By default, the table resets the current record, but you can
        /// avoid this action if you set this property to False.
        /// </summary>
        public bool ShouldResetCurrentRecord
        {
            get
            {
                return shouldResetCurrentRecord;
            }

            set
            {
                shouldResetCurrentRecord = value;
            }
        }

        /// <summary>
        /// Lets you specify whether the current record should be moved to
        /// the previous visible record when an ItemDeleted notification is 
        /// received for the current record.
        /// <para/>
        /// By default, the table deactivates the current record. You can
        /// avoid this action and force the record to moved if you set this 
        /// property to True.
        /// </summary>
        public bool NavigateCurrentRecordWhenDeleted
        {
            get
            {
                return navigateCurrentRecordWhenDeleted;
            }

            set
            {
                navigateCurrentRecordWhenDeleted = value;
            }
        }
        
        /// <summary>
        /// Lets you specify if changes to the current record affect the sort order
        /// of the parent group  <para/>
        /// This property only has an affect when a record is changed and you have specified
        /// a SortColumnDescriptor.GroupSortOrderComparer for the grouped column, e.g. if
        /// you want groups to be sorted by the value of a summary column.
        /// <para/>
        /// If you know that changes to a specific record won't affect the sort order you 
        /// can avoid unnencessary resorting of the parent group by setting this property to false.
        /// </summary>
        public bool ShouldInvalidateGroupSortOrder
        {
            get
            {
                return shouldInvalidateGroupSortOrder;
            }

            set
            {
                shouldInvalidateGroupSortOrder = value;
            }
        }

        /// <summary>
        /// Lets you specify if changes to the current record affect the sort position
        /// of the record.<para/>
        /// Set this property if you sorted or grouped by an expression field that is dependant
        /// on changes in other values of the record. The engine does not track dependencies among
        /// fields. 
        /// </summary>
        public bool ShouldReevaluateSortPosition
        {
            get
            {
                return shouldReevaluateSortPosition;
            }

            set
            {
                shouldReevaluateSortPosition = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="TableListChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void TableListChangedEventHandler(object sender, TableListChangedEventArgs e);

    /// <summary>
    /// Provides data for events that occur on a table such as SourceListChanged, CategorizedRecords,
    /// CurrentRecordManagerReset, and others.
    /// </summary>
    public class TableEventArgs : SyncfusionCancelEventArgs
    {
        Table table;

        /// <summary>
        /// Initializes a new object with a reference to a table.
        /// </summary>
        /// <param name="table">The affected table.</param>
        public TableEventArgs(Table table)
        {
            this.table = table;
        }

        /// <summary>
        /// The table object.
        /// </summary>
        [TraceProperty(true)]
        public Table Table
        {
            get
            {
                return table;
            }

            set
            {
                table = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="TableEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void TableEventHandler(object sender, TableEventArgs e);

    ////    public sealed class NestedTableEventArgs : SyncfusionCancelEventArgs 
    ////    {
    ////        NestedTable nestedTable;
    ////    
    ////        public NestedTableEventArgs(NestedTable nestedTable) 
    ////        {
    ////            this.nestedTable = nestedTable;
    ////        }
    ////    
    ////        [TraceProperty(true)]
    ////        public NestedTable NestedTable
    ////        {
    ////            get
    ////            {
    ////                return nestedTable;
    ////            }
    ////        }
    ////    }
    ////
    ////    ///// <summary>
    ////    ///// Represents a method that handles an event with <see cref="NestedTableEventArgs"/>.
    ////    ///// </summary>
    ////    ///// <param name="sender">The source of the event.</param>
    ////    ///// <param name="e">The event data.</param>
    ////    public delegate void NestedTableEventHandler(object sender, NestedTableEventArgs e);

    #region CurrentRecordContextChangeEvents

    /// <summary>
    /// Specifies how the current record context changes.
    /// </summary>
    /// <remarks>
    /// This enum is used by the <see cref="CurrentRecordContextChangeEventArgs.Action"/> property
    /// of the <see cref="CurrentRecordContextChangeEventArgs"/> class.</remarks>
    public enum CurrentRecordAction
    {
        /// <summary>
        /// <see cref="CurrentRecordManager.BeginEdit"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        BeginEditCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.BeginEdit"/> is complete and returns.
        /// </summary>
        BeginEditComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.EndEdit"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        EndEditCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.EndEdit"/> is complete and returns.
        /// </summary>
        EndEditComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.CancelEdit"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        CancelEditCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.CancelEdit"/> is complete and returns.
        /// </summary>
        CancelEditComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.Navigate"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        NavigateCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.Navigate"/> is complete and returns.
        /// </summary>
        NavigateComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.LeaveRecord"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        LeaveRecordCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.LeaveRecord"/> is complete and returns.
        /// </summary>
        LeaveRecordComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.EnterRecord"/> was called. You can cancel this operation by
        /// setting the Cancel property.
        /// </summary>
        EnterRecordCalled,

        /// <summary>
        /// <see cref="CurrentRecordManager.EnterRecord"/> is complete and returns.
        /// </summary>
        EnterRecordComplete,

        /// <summary>
        /// <see cref="CurrentRecordManager.CurrentField"/> was moved.
        /// </summary>
        CurrentFieldChanged
    }

    /// <summary>
    /// Provides data for the CurrentRecordContextChange event that
    /// occurs before and after the status of the current record is changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
    /// to get information on which current record state was changed.
    /// </summary>
    public sealed class CurrentRecordContextChangeEventArgs : CancelEventArgs
    {
        Table table;
        Element record;
        bool success;
        CurrentRecordAction action;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="action">Specifies how the current record context changes.</param>
        /// <param name="table">The affected table.</param>
        /// <param name="record">The affected record.</param>
        /// <param name="success">Indicates if the operation was completed successfully or failed.</param>
        public CurrentRecordContextChangeEventArgs(CurrentRecordAction action, Table table, Element record, bool success)
        {
            this.table = table;
            this.record = record;
            this.success = success;
            this.action = action;
        }

        /// <summary>
        /// Specifies how the current record context changes.
        /// </summary>
        public CurrentRecordAction Action
        {
            get
            {
                return action;
            }
        }

        /// <summary>
        /// The affected table.
        /// </summary>
        public Table Table
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        /// The affected record.
        /// </summary>
        public Element Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// Indicates if the operation was completed successfully or failed.
        /// </summary>
        public bool Success
        {
            get
            {
                return success;
            }
        }

        /// <summary>Returns string representation of the event args object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Action = " + Action.ToString() + " " + "Success = " + Success.ToString());
            if (Record != null)
            {
                sb.Append(" Record = " + Record.ToString());
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a method that handles an event with <see cref="CurrentRecordContextChangeEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void CurrentRecordContextChangeEventHandler(object sender, CurrentRecordContextChangeEventArgs e);

    #endregion

    /// <summary>
    /// Represents a method that handles an event with <see cref="DisplayElementChangingEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void DisplayElementChangingEventHandler(object sender, DisplayElementChangingEventArgs e);

    /// <summary>
    /// Represents a method that handles an event with <see cref="DisplayElementChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void DisplayElementChangedEventHandler(object sender, DisplayElementChangedEventArgs e);

    // eva DisplayElementChanging SyncfusionCancel Element element int oldCount int newCount bool repaintElement bool syncCurrentRecordPos bool leaveCurrentRecord
    // eva DisplayElementChanged SyncfusionSuccess Element element int oldCount int newCount bool repaintElement bool syncCurrentRecordPos bool leaveCurrentRecord

    /// <summary>
    /// A base class for EventArgs with an <see cref="AllowCancel"/> property.
    /// </summary>
    public class SyncfusionAllowCancelEventArgs : SyncfusionCancelEventArgs
    {
        bool allowCancel = true;

        /// <summary>
        /// Indicates if the event supports canceling the current operation that raised the event.
        /// </summary>
        [TraceProperty(true)]
        public bool AllowCancel
        {
            get
            {
                return allowCancel;
            }

            set
            {
                allowCancel = value;
            }
        }
    }
    
    /// <summary>
    /// Provides data for the DisplayElementChanging event that
    /// occurs before display elements in a table are changed. A GridGroupingControl
    /// listens to this event and deactivates the current cell and / or saves the cell's contents if necessary.
    /// </summary>
    public sealed class DisplayElementChangingEventArgs : SyncfusionAllowCancelEventArgs
    {
        Element element;
        int oldCount;
        int newCount;
        bool repaintElement;
        bool syncCurrentRecordPos;
        bool leaveCurrentRecord;
        bool scrollCurrentRecordInView;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        /// <param name="scrollCurrentRecordInView">Indicates if current record should be scrolled into view.</param>
        public DisplayElementChangingEventArgs(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord, bool scrollCurrentRecordInView)
        {
            this.element = element;
            this.oldCount = oldCount;
            this.newCount = newCount;
            this.repaintElement = repaintElement;
            this.syncCurrentRecordPos = syncCurrentRecordPos;
            this.leaveCurrentRecord = leaveCurrentRecord;
            this.scrollCurrentRecordInView = scrollCurrentRecordInView;
        }

        /// <summary>
        /// The affected element can be the whole table.
        /// </summary>
        [TraceProperty(true)]
        public Element Element
        {
            get
            {
                return element;
            }
        }

        /// <summary>
        /// The old display element count of the affected element. Can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int OldCount
        {
            get
            {
                return oldCount;
            }
        }

        /// <summary>
        /// The new display element count of the affected element. Can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int NewCount
        {
            get
            {
                return newCount;
            }
        }

        /// <summary>
        /// Indicates if element needs repainting.
        /// </summary>
        [TraceProperty(true)]
        public bool RepaintElement
        {
            get
            {
                return repaintElement;
            }
        }

        /// <summary>
        /// Indicates if current record position should be saved and restored.
        /// </summary>
        [TraceProperty(true)]
        public bool SyncCurrentRecordPos
        {
            get
            {
                return syncCurrentRecordPos;
            }
        }

        /// <summary>
        /// Indicates if current record should be deactivated.
        /// </summary>
        [TraceProperty(true)]
        public bool LeaveCurrentRecord
        {
            get
            {
                return leaveCurrentRecord;
            }

            set
            {
                leaveCurrentRecord = value;
            }
        }

        /// <summary>
        /// Indicates if current record should be scrolled into view.
        /// </summary>
        [TraceProperty(true)]
        public bool ScrollCurrentRecordInView
        {
            get
            {
                return this.scrollCurrentRecordInView;
            }
        }
    }

    /// <summary>
    /// Provides data for the DisplayElementChanged event that
    /// occurs after display elements in a table are changed. A GridGroupingControl
    /// listens to this event and repaints affected elements if necessary.
    /// </summary>
    public sealed class DisplayElementChangedEventArgs : SyncfusionSuccessEventArgs
    {
        Element element;
        int oldCount;
        int newCount;
        bool repaintElement;
        bool syncCurrentRecordPos;
        bool leaveCurrentRecord;
        bool scrollCurrentRecordInView;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="element">The affected element can be the whole table.</param>
        /// <param name="oldCount">The old display element count of the affected element. Can be -1.</param>
        /// <param name="newCount">The new display element count of the affected element. Can be -1.</param>
        /// <param name="repaintElement">Indicates if element needs repainting.</param>
        /// <param name="syncCurrentRecordPos">Indicates if current record position should be saved and restored.</param>
        /// <param name="leaveCurrentRecord">Indicates if current record should be deactivated.</param>
        /// <param name="scrollCurrentRecordInView">Indicates if current record should be scrolled into view.</param>
        public DisplayElementChangedEventArgs(Element element, int oldCount, int newCount, bool repaintElement, bool syncCurrentRecordPos, bool leaveCurrentRecord, bool scrollCurrentRecordInView)
        {
            this.element = element;
            this.oldCount = oldCount;
            this.newCount = newCount;
            this.repaintElement = repaintElement;
            this.syncCurrentRecordPos = syncCurrentRecordPos;
            this.leaveCurrentRecord = leaveCurrentRecord;
            this.scrollCurrentRecordInView = scrollCurrentRecordInView;
        }
        
        /// <summary>
        /// The affected element can be the whole table.
        /// </summary>
        [TraceProperty(true)]
        public Element Element
        {
            get
            {
                return element;
            }
        }

        /// <summary>
        /// The old display element count of the affected element. Can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int OldCount
        {
            get
            {
                return oldCount;
            }
        }

        /// <summary>
        /// The new display element count of the affected element. Can be -1.
        /// </summary>
        [TraceProperty(true)]
        public int NewCount
        {
            get
            {
                return newCount;
            }

            set
            {
                newCount = value;
            }
        }

        /// <summary>
        /// Indicates if element needs repainting.
        /// </summary>
        [TraceProperty(true)]
        public bool RepaintElement
        {
            get
            {
                return repaintElement;
            }
        }

        /// <summary>
        /// Indicates if current record position should be saved and restored.
        /// </summary>
        [TraceProperty(true)]
        public bool SyncCurrentRecordPos
        {
            get
            {
                return syncCurrentRecordPos;
            }
        }

        /// <summary>
        /// Indicates if current record should be deactivated.
        /// </summary>
        [TraceProperty(true)]
        public bool LeaveCurrentRecord
        {
            get
            {
                return leaveCurrentRecord;
            }
        }

        /// <summary>
        /// Indicates if current record should be scrolled into view.
        /// </summary>
        [TraceProperty(true)]
        public bool ScrollCurrentRecordInView
        {
            get
            {
                return this.scrollCurrentRecordInView;
            }
        }
    }
    
    // eva FieldValue Syncfusion FieldDescriptor field Record record object value

    /// <summary>
    /// Represents a method that handles an event with <see cref="FieldValueEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void FieldValueEventHandler(object sender, FieldValueEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="Engine.QueryValue"/> and <see cref="Engine.SaveValue"/> events that
    /// occur when a value for a field descriptor and record is returned or when a value for a field descriptor and record is saved. See the Grid\Grouping\Samples\CustomSummary
    /// sample how to use this event with unbound field descriptors.
    /// </summary>
    public sealed class FieldValueEventArgs : SyncfusionEventArgs
    {
        FieldDescriptor field;
        Record record;
        object _value;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="field">The affected field.</param>
        /// <param name="record">The affected record.</param>
        /// <param name="value">The default value.</param>
        public FieldValueEventArgs(FieldDescriptor field, Record record, object value)
        {
            this.field = field;
            this.record = record;
            this._value = value;
        }

        /// <summary>
        /// The affected field.
        /// </summary>
        [TraceProperty(true)]
        public FieldDescriptor Field
        {
            get
            {
                return field;
            }
        }

        /// <summary>
        /// The affected record.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// The table descriptor.
        /// </summary>
        [TraceProperty(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return field.TableDescriptor;
            }
        }

        /// <summary>
        /// Gets / sets the result value or value to be saved into the datasource.
        /// </summary>
        [TraceProperty(true)]
        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }
    
    // eva CustomCount Syncfusion Record record double customCount

    /// <summary>
    /// Represents a method that handles an event with <see cref="CustomCountEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void CustomCountEventHandler(object sender, CustomCountEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="Table.QueryCustomCount"/> and <see cref="Table.QueryVisibleCustomCount"/> events that
    /// occur when the custom counter value for a record is queried. See the Grid\Grouping\Samples\CustomSummary
    /// example.
    /// </summary>
    public sealed class CustomCountEventArgs : SyncfusionEventArgs
    {
        Record record;
        double customCount;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="record">The record for which the custom counter value should be returned.</param>
        /// <param name="customCount">The default value for the custom counter value.</param>
        public CustomCountEventArgs(Record record, double customCount)
        {
            this.record = record;
            this.customCount = customCount;
        }

        /// <summary>
        /// The record for which the custom counter value should be returned.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// Gets / sets the custom counter value.
        /// </summary>
        [TraceProperty(true)]
        public double CustomCount
        {
            get
            {
                return customCount;
            }

            set
            {
                customCount = value;
            }
        }
    }
    
    /// <summary>
    /// Represents a method that handles the cancelable <see cref="Engine.RecordValueChanging"/> event of a <see cref="Engine"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void RecordValueChangingEventHandler(object sender, RecordValueChangingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="Engine.RecordValueChanging"/> event of a <see cref="Engine"/>
    /// which occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
    /// </summary>
    /// <remarks>
    /// You can cancel saving the value when you set e.Cancel = True, or you can replace e.NewValue with a different value.
    /// </remarks>
    public sealed class RecordValueChangingEventArgs : SyncfusionCancelEventArgs
    {
        Record record;
        FieldDescriptor fieldDescriptor;
        object newValue;
        string column;

        /// <summary>
        /// Initializes the event data.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="column">The column.</param>
        /// <param name="fieldDescriptor">The field.</param>
        /// <param name="newValue">The new value to save.</param>
        public RecordValueChangingEventArgs(Record record, string column, FieldDescriptor fieldDescriptor, object newValue)
        {
            this.record = record;
            this.column = column;
            this.fieldDescriptor = fieldDescriptor;
            this.newValue = newValue;
        }

        /// <summary>
        /// The record.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// The column.
        /// </summary>
        [TraceProperty(true)]
        public string Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// The field.
        /// </summary>
        [TraceProperty(true)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                return fieldDescriptor;
            }
        }

        /// <summary>
        /// The new value to save.
        /// </summary>
        [TraceProperty(true)]
        public object NewValue
        {
            get
            {
                return newValue;
            }

            set
            {
                newValue = value;
            }
        }
    }

    // eva RecordValueChanged Syncfusion Record record ColumnDescriptor column FieldDescriptor fieldDescriptor

    /// <summary>
    /// Represents a method that handles the <see cref="Engine.RecordValueChanged"/> event of a <see cref="Engine"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void RecordValueChangedEventHandler(object sender, RecordValueChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="Engine.RecordValueChanged"/> event of a <see cref="Engine"/>
    /// which occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned.
    /// </summary>
    public sealed class RecordValueChangedEventArgs : SyncfusionEventArgs
    {
        Record record;
        string column;
        FieldDescriptor fieldDescriptor;

        /// <summary>
        /// Initializes the event data.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="column">The column.</param>
        /// <param name="fieldDescriptor">The field.</param>
        public RecordValueChangedEventArgs(Record record, string column, FieldDescriptor fieldDescriptor)
        {
            this.record = record;
            this.column = column;
            this.fieldDescriptor = fieldDescriptor;
        }

        /// <summary>
        /// The record.
        /// </summary>
        [TraceProperty(true)]
        public Record Record
        {
            get
            {
                return record;
            }
        }

        /// <summary>
        /// The column.
        /// </summary>
        [TraceProperty(true)]
        public string Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// The field.
        /// </summary>
        [TraceProperty(true)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                return fieldDescriptor;
            }
        }
    }
}