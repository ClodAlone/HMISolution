#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.ComponentModel;
    using System.Collections;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Data;
    using System.Collections.Specialized;
    using System.Windows;


    /// <summary>
    /// Delegate for <see cref="GridDataTable.InitializingNewItem"/> event.
    /// </summary>
    public delegate void GridDataInitializingNewItemEventHandler(object sender, GridDataInitializingNewItemEventArgs args);


    /// <summary>
    /// Delegate for <see cref="GridDataCurrentRecordManager.CurrentRecordSelectionChanging"/> event.
    /// </summary>
    public delegate void GridDataCurrentRecordSelectionChangingEventHandler(object sender, GridDataCurrentRecordSelectionChangingEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataCurrentRecordManager.CurrentRecordSelectionChanged"/> event.
    /// </summary>
    public delegate void GridDataCurrentRecordSelectionChangedEventHandler(object sender, GridDataCurrentRecordSelectionChangedEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataControl.RowValueCommitting"/>
    /// </summary>
    public delegate void GridDataRowValueCommittingEventHandler(object sender, GridDataRowValueCommittingEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataControl.RowValueCommitted"/> event.
    /// </summary> 
    public delegate void GridDataRowValueCommittedEventHandler(object sender, GridDataRowValueCommittedEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataControl.RowValueCommittingCancelled"/> event.
    /// </summary>  
    public delegate void GridDataRowValueCommittingCancelledEventHandler(object sender, GridDataRowValueCommittingCancelledEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataControl.RowValidating"/> event.
    /// </summary>
    public delegate void GridDataRowValidatingEventHandler(object sender, GridDataRowValidatingEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataTable.CurrentCellValidating"/> event.
    /// </summary>
    public delegate void GridDataCurrentCellValidatingEventHandler(object sender, GridDataCurrentCellValidatingEventArgs args);


    /// <summary>
    /// Delegate for <see cref="GridDataTableModel.QueryUnboundCellInfo"/> event.
    /// </summary>
    public delegate void GridDataQueryUnboundCellInfoEventHandler(object sender, GridQueryCellInfoEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataCurrentRecordManager.RecordAdding"/> event.
    /// </summary>
    public delegate void GridDataNewRecordAddingEventHandler(object sender, GridDataNewRecordAddingEventArgs args);

    /// <summary>
    /// Delegate for <see cref="GridDataCurrentRecordManager.RecordAdded"/> event.
    /// </summary>
    public delegate void GridDataNewRecordAddedEventHandler(object sender, GridDataNewRecordAddedEventArgs args);

    /// <summary>
    /// Provides object for initializing new item
    /// </summary>
    public class GridDataInitializingNewItemEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new item
        /// </summary>
        public object NewItem { get; set; }
    }

    /// <summary>
    /// Provides data for validating new record.
    /// </summary>
    public class GridDataNewRecordAddingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataNewRecordAddingEventArgs"/> class.
        /// </summary>
        /// <param name="data">Record Data.</param>

        public GridDataNewRecordAddingEventArgs(object data)
            : base(false)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the RecordEntry.
        /// </summary>
        /// <value>Record Entry.</value>
        public object Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets handle
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the new record added.
    /// </summary>
    public class GridDataNewRecordAddedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataNewRecordAddedEventArgs"/> class.
        /// </summary>
        /// <param name="data">Record Entry.</param>
        public GridDataNewRecordAddedEventArgs(RecordEntry record)
        {
            this.Record = record;
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>Record Data.</value>
        public RecordEntry Record
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides data for canceling record selection changed event.
    /// </summary>
    public class GridDataCurrentRecordSelectionChangingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataCurrentRecordSelectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public GridDataCurrentRecordSelectionChangingEventArgs(int recordIndex) :
            base(false)
        {
            this.RecordIndex = recordIndex;
        }

        /// <summary>
        /// Gets or sets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides data for current record selection changed event.
    /// </summary>
    public class GridDataCurrentRecordSelectionChangedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Gets the new index.
        /// </summary>
        /// <value>The new index.</value>
        public int NewIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the old index.
        /// </summary>
        /// <value>The old index.</value>
        public int OldIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides a generic handler for holding simple data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridDataValueEventArgs<T> : SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataValueEventArgs&lt;T&gt;"/> class.
        /// </summary>
        public GridDataValueEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataValueEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public GridDataValueEventArgs(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides a cancellable generic data to handle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridDataValueCancelEventArgs<T> : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataValueCancelEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public GridDataValueCancelEventArgs(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get;
            private set;
        }
    }
  
    public class GridDataRowValueCommittingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public GridDataRecord Record
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the edited values.
        /// </summary>
        /// <value>The edited values.</value>
        public Dictionary<string, object> EditedValues
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get;
            internal set;
        }
    }
    
    public class GridDataRowValueCommittingCancelledEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public GridDataRecord Record
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get;
            internal set;
        }
    }
   
    public class GridDataRowValueCommittedEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public GridDataRecord Record
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the new values.
        /// </summary>
        /// <value>The new values.</value>
        public Dictionary<string, object> NewValues
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get;
            internal set;
        }
    }   

    public class GridDataRowValidatingEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataRowValidatingEventArgs"/> class.
        /// </summary>
        public GridDataRowValidatingEventArgs()
        {           
        }

        /// <summary>
        /// Gets or sets the new values.
        /// </summary>
        /// <value>The new values.</value>
        public Dictionary<string,object> NewValues
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get;
           internal set;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides a cancellable data handler for validating current cell changes.
    /// </summary>
    public class GridDataCurrentCellValidatingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the modified value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.CurrentCellChanged"/> event.
    /// </summary>
    public delegate void GridDataCurentCellChangedEventHandler(object sender, GridDataCurrentCellChanged args);

    /// <summary>
    /// Provides the class to hold data when current cell is changed.
    /// </summary>
    public class GridDataCurrentCellChanged : SyncfusionEventArgs
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <value>The current value.</value>
        public object CurrentValue
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate handler for <see cref="GridDataTable.GroupExpanding"/> event.
    /// </summary>
    public delegate void GroupExpandingEventHandler(object sender, GroupExpandingEventArgs args);

    /// <summary>
    /// Provides a cancelleable data when group is expanding.
    /// </summary>
    public class GroupExpandingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate handler for <see cref="GridDataTable.GroupExpanded"/> event.
    /// </summary>
    public delegate void GroupExpandedEventHandler(object sender, GroupExpandedEventArgs args);

    /// <summary>
    /// Provides the data when group is expanded.
    /// </summary>
    public class GroupExpandedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate handler for <see cref="GridDataTable.GroupCollapsing"/> event.
    /// </summary>
    public delegate void GroupCollapsingEventHandler(object sender, GroupCollapsingEventArgs args);

    /// <summary>
    /// Provides a cancelleable data when group is collapsing.
    /// </summary>
    public class GroupCollapsingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate handler for <see cref="GridDataTable.GroupCollapsed"/> event.
    /// </summary>
    public delegate void GroupCollapsedEventHandler(object sender, GroupCollapsedEventArgs args);

    /// <summary>
    /// Provides the data when the group is collapsed.
    /// </summary>
    public class GroupCollapsedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Provides the delegate handler for <see cref="GridDataTableModel.QueryUnboundColumnValue"/> event.
    /// </summary>
    public delegate void GridDataQueryUnboundColumnCellEventHandler(object sender, GridDataQueryUnboundColumnCellEventArgs args);

    /// <summary>
    /// Provides the Unbound column value data that can be handled by the subscriber.
    /// </summary>
    public sealed class GridDataQueryUnboundColumnCellEventArgs : SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataQueryUnboundColumnCellEventArgs"/> class.
        /// </summary>
        public GridDataQueryUnboundColumnCellEventArgs()
        {
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <value>The cell.</value>
        public RowColumnIndex Cell
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="GridDataStyleInfo"/> style.
        /// </summary>
        /// <value>The style.</value>
        public GridDataStyleInfo Style
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets <see cref="GridDataUnboundVisibleColumn"/> value.
        /// </summary>
        /// <value>The unbound column.</value>
        public GridDataUnboundVisibleColumn UnboundColumn
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the record entry.
        /// </summary>
        /// <value>The record entry.</value>
        public GridDataRecord RecordEntry
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get
            {
                if (this.RecordEntry != null)
                {
                    return this.RecordEntry.Data;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Represents a method that handles <see cref="GridDataTableModel.FilterChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellClickEventArgs"/> that contains the event data.</param>
    public delegate void GridFilterEventHandler(object sender, GridFilterEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridDataTableModel.FilterChanging"/> event.
    /// </summary>
    /// <remarks>GridFilterEventArgs is a custom event argument class used by the <see cref="GridDataTableModel.FilterChanging"/>
    /// event when the user clicks inside a cell.</remarks>
    /// <seealso cref="GridControlBase.RaiseGridCellClick"/>
    public class GridFilterEventArgs : SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellClickEventArgs"/>.
        /// </summary>
        public GridFilterEventArgs(GridDataVisibleColumn column, FilterPredicate filterpredicate)
        {
            this.column = column;
            this.filterpredicate = filterpredicate;
        }

        private GridDataVisibleColumn column;
        public GridDataVisibleColumn Column
        {
            get
            {
                return column;
            }
        }

        private FilterPredicate filterpredicate;
        public FilterPredicate FilterPredicate
        {
            get
            {
                return filterpredicate;
            }
        }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.RecordDeleting"/> event.
    /// </summary>
    public delegate void GridDataRecordDeletingEventHandler(object sender, GridDataRecordDeletingEventArgs args);

    /// <summary>
    /// Provides a cancelleable data when the record is deleted.
    /// </summary>
    public sealed class GridDataRecordDeletingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataRecordDeletingEventArgs"/> class.
        /// </summary>
        public GridDataRecordDeletingEventArgs()
        {
            Handled = false;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }

        public List<object> Records { get; internal set; }

        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex { get; internal set; }

        /// <summary>
        /// Handles MessageBox display while deleting
        /// </summary>
        public bool Handled { get; set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.RecordDeleted"/> event.
    /// </summary>
    public delegate void GridDataRecordDeletedEventHandler(object sender, GridDataRecordDeletedEventArgs args);

    /// <summary>
    /// Provides a cancelleable data when the record is deleted.
    /// </summary>
    public sealed class GridDataRecordDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataRecordDeletedEventArgs"/> class.
        /// </summary>
        public GridDataRecordDeletedEventArgs()
        {
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }

        public List<object> Records { get; internal set; }
        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex { get; internal set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.RecordExpanding"/> event.
    /// </summary>
    public delegate void GridDataRecordExpandingEventHandler(object sender, GridDataRecordExpandingEventArgs args);


    /// <summary>
    /// Provides ItemsSource, Record when the record is expanded.
    /// </summary>
    public sealed class GridDataRecordExpandingEventArgs : SyncfusionCancelEventArgs
    {
        public GridDataRecordExpandingEventArgs()
        {
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }


        /// <summary>
        /// Gets or sets Handled 
        /// </summary>        
        public bool Handled { get; set; }



        /// <summary>
        /// Gets or sets the ChildItemsSource.
        /// </summary>
        /// <value>ChildItemsSource.</value>
        public object ChildItemsSource { get; set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.DetailsViewExpanding"/> event.
    /// </summary>
    public delegate void GridDataDetailsViewExpandingEventHandler(object sender, GridDataDetailsViewExpandingEventArgs args);

    /// <summary>
    /// Provides DataContext, Template when the details view is expanded.
    /// </summary>
    public sealed class GridDataDetailsViewExpandingEventArgs : SyncfusionCancelEventArgs
    {
        public GridDataDetailsViewExpandingEventArgs(GridDataRecord record)
        {
            this.Record = record;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public GridDataRecord Record { get; private set; }

        /// <summary>
        /// Gets or sets Handled 
        /// </summary>        
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the ChildItemsSource.
        /// </summary>
        /// <value>ChildItemsSource.</value>
        public object DetailsViewDataContext
        {
            set
            {
                Record.DetailsViewDataContext = value;
            }
            get
            {
                return Record.DetailsViewDataContext;
            }
        }

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        public DataTemplate DetailsViewTemplate
        {
            set
            {
                Record.DetailsViewTemplate = value;
            }
            get
            {
                return Record.DetailsViewTemplate;
            }
        }
    }

    /// <summary>
    /// Provides the delegate for <see cref="GridDataTable.DetailsViewExpanding"/> event.
    /// </summary>
    public delegate void GridDataDetailsViewExpandedEventHandler(object sender, GridDataDetailsViewExpandedEventArgs args);

    /// <summary>
    /// Provides DataContext, Template when the details view is expanded.
    /// </summary>
    public sealed class GridDataDetailsViewExpandedEventArgs : SyncfusionEventArgs
    {
        public GridDataDetailsViewExpandedEventArgs(GridDataRecord record)
        {
            this.Record = record;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public GridDataRecord Record { get; private set; }

        /// <summary>
        /// Gets or sets the ChildItemsSource.
        /// </summary>
        /// <value>ChildItemsSource.</value>
        public object DetailsViewDataContext
        {
            get
            {
                return Record.DetailsViewDataContext;
            }
        }

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        public DataTemplate DetailsViewTemplate
        {
            get
            {
                return Record.DetailsViewTemplate;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.SelectionChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridDataRecordsSelectionChangedEventHandler(object sender, GridDataRecordsSelectionChangedEventArgs e);

    /// <summary>
    /// Provides a routing data that can be handled when the record's selection is changed.
    /// </summary>
    public sealed class GridDataRecordsSelectionChangedEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Record Selection Changed Event
        /// </summary>
        /// <param name="oldItems">removed Items</param>
        /// <param name="newItems">added Items</param>
        public GridDataRecordsSelectionChangedEventArgs(IList oldItems, IList newItems)
        {
            this.RemovedItems = oldItems;
            this.AddedItems = newItems;
        }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList RemovedItems { get; private set; }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList AddedItems { get; private set; }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.SelectionChanging"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GridDataRecordSelectionChangingEventHandler(object sender, GridDataRecordSelectionChangingEventArgs e);

    /// <summary>
    /// Arguments for record selection changing
    /// </summary>
    public sealed class GridDataRecordSelectionChangingEventArgs : SyncfusionRoutedEventArgs
    {
        public GridDataRecordSelectionChangingEventArgs()
        {

        }

        /// <summary>
        /// Gets the CurrentSelectedItem
        /// </summary>
       // [Nullable]
        public object NewItem { get; internal set; }

        /// <summary>
        /// Gets the PreviousSelectedItem
        /// </summary>
        public object OldItem { get; internal set; }

        /// <summary>
        /// Gets the index of the NewItem
        /// </summary>
        public RowColumnIndex NewIndex { get; internal set; }

        /// <summary>
        /// Gets the index of the OldItem
        /// </summary>
        public RowColumnIndex OldIndex { get; internal set; }

        /// <summary>
        /// Gets the index of the Old Record
        /// </summary>
        public int NewRecordIndex { get; internal set; }

        /// <summary>
        /// Gets the index of the New Record
        /// </summary>
        public int OldRecordIndex { get; internal set; }

        /// <summary>
        /// Allows user to Cancel the record selection changing
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the reason for selection changing
        /// </summary>
        public GridSelectionReason  Reason { get; internal set; }

    }

  

    public delegate void GridDataOnDemandPageLoadingEventHandler(object sender, GridDataOnDemandPageLoadingEventArgs e);
    public sealed class GridDataOnDemandPageLoadingEventArgs : SyncfusionRoutedEventArgs
    {
        public GridDataOnDemandPageLoadingEventArgs()
        {

        }      
        public GridDataOnDemandPageLoadingEventArgs(int PagedRows,int MaximumRows)
        {
            this.PagedRows = PagedRows;
            this.MaximumRows = MaximumRows;
        }
       
        public int PagedRows { get; internal set; }
        public int MaximumRows { get; internal set; }

    }
  

    /// <summary>
    /// Provides a canceleable data that can be handled when sort columns are changing.
    /// </summary>
    public sealed class GridDataSortColumnsChangingEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSortColumnsChangingEventArgs"/> class.
        /// </summary>
        /// <param name="addedItems">The added items.</param>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="action">The action.</param>
        public GridDataSortColumnsChangingEventArgs(IList<GridDataSortColumn> addedItems, IList<GridDataSortColumn> removedItems, NotifyCollectionChangedAction action)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
            this.Action = action;
        }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList<GridDataSortColumn> AddedItems { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList<GridDataSortColumn> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Changing Event, Added Items {0} : Removed Items {1} : Action {2}", AddedItems != null ? AddedItems.Count : 0, RemovedItems != null ? RemovedItems.Count : 0, Action));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridDataTable.SortColumnsChanging"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void GridDataSortColumnsChangingEventHandler(object sender, GridDataSortColumnsChangingEventArgs args);

    public sealed class GridDataSortColumnsChangedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSortColumnsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="addedItems">The added items.</param>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="action">The action.</param>
        public GridDataSortColumnsChangedEventArgs(IList<GridDataSortColumn> addedItems, IList<GridDataSortColumn> removedItems, NotifyCollectionChangedAction action)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
            this.Action = action;
        }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList<GridDataSortColumn> AddedItems { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList<GridDataSortColumn> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Changed Event, Added Items {0} : Removed Items {1} : Action {2}", AddedItems != null ? AddedItems.Count : 0, RemovedItems != null ? RemovedItems.Count : 0, Action));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridDataTable.SortColumnsChanged"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void GridDataSortColumnsChangedEventHandler(object sender, GridDataSortColumnsChangedEventArgs args);

    public sealed class GridDataGroupedColumnsChangedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupedColumnsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="addedItems">The added items.</param>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="action">The action.</param>
        public GridDataGroupedColumnsChangedEventArgs(IList<GridDataGroupColumn> addedItems, IList<GridDataGroupColumn> removedItems, NotifyCollectionChangedAction action)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
            this.Action = action;
        }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList<GridDataGroupColumn> AddedItems { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList<GridDataGroupColumn> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Changed Event, Added Items {0} : Removed Items {1} : Action {2}", AddedItems != null ? AddedItems.Count : 0, RemovedItems != null ? RemovedItems.Count : 0, Action));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridDataTable.GroupedColumnsChanged"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void GridDataGroupedColumnsChangedEventHandler(object sender, GridDataGroupedColumnsChangedEventArgs args);

    public delegate void QueryVisibleColumnInfoEventHandler(object sender, QueryVisibleColumnInfoArgs args);

    public sealed class QueryVisibleColumnInfoArgs : EventArgs
    {
        public GridDataVisibleColumn VisibleColumn { get; set; }
    }
}
