#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Linq.Expressions;
    using Syncfusion.Windows.Data;
    using System.Windows;
using System.Windows.Data;

    /// <summary>
    /// ICollectionViewAdv is an extended interface from ICollectionView that implements
    /// support for Grouping structure, Summaries, Table Summaries, Filters.
    /// </summary>
    public interface ICollectionViewAdv : IDisposable, ICollectionView, INotifyPropertyChanged  
#if !SILVERLIGHT
, IEditableCollectionView
        , ISupportInitialize
        , IWeakEventListener
#else
        , IEditableCollectionView
        , ISupportInitialize
        , IPropertyChangedEventHandler
#endif

    {
        /// <summary>
        /// Gets the records list structure.
        /// </summary>
        /// <value>The records.</value>
        IRecordsList Records
        {
            get;
        }

        bool PassesFilter(object record);

        //This code for Paging
#if !SILVERLIGHT
        bool EnablePaging
#else
        new bool EnablePaging
#endif
        {
            get;
            set;
        }
        void RefreshFilters();

#if !SILVERLIGHT
        bool IsViewLevelPaging
#else
        new bool IsViewLevelPaging
#endif
        {
            get;
            set;
        }

#if !SILVERLIGHT
        PagedCollectionView PagedSource
#else
        new PagedCollectionView PagedSource
#endif
        {
            get;
            set;
        }


        List<RecordEntry> FilteredRecord
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the top level group.
        /// </summary>
        /// <value>The top level group.</value>
        TopLevelGroup TopLevelGroup
        {
            get;
        }

        /// <summary>
        /// Gets or sets the caption summary row.
        /// </summary>
        /// <value>The caption summary row.</value>
        ISummaryRow CaptionSummaryRow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the relations.
        /// </summary>
        /// <value>The relations.</value>
        ObservableCollection<IRelationDefinition> Relations
        {
            get;
        }

        /// <summary>
        /// Gets the summary rows.
        /// </summary>
        /// <value>The summary rows.</value>
        ObservableCollection<ISummaryRow> SummaryRows
        {
            get;
        }

        /// <summary>
        /// Gets or sets the filter predicates.
        /// </summary>
        /// <value>The filter predicates.</value>
        ObservableCollection<IFilterDefinition> FilterPredicates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the table summary rows.
        /// </summary>
        /// <value>The table summary rows.</value>
        ObservableCollection<ISummaryRow> TableSummaryRows
        {
            get;
        }


        /// <summary>
        /// Gets the sort comparers for the sort description.
        /// </summary>
        Dictionary<string, IComparer<object>> SortComparers
        {
            get;
        }

        /// <summary>
        /// Defines the custom group comparer to enable customization of the group sort order.
        /// </summary>
        IComparer<Group> GroupComparer { get; set; }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void OnCollectionChanged(NotifyCollectionChangedEventArgs args);

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        void AddListener(ICollectionView collectionView);

        /// <summary>
        /// Gets the state of the edit item.
        /// </summary>
        /// <value>The state of the edit item.</value>
        IEditItemState EditItemState { get; }

        /// <summary>
        /// Gets the state of the current edit item.
        /// </summary>
        /// <value>The state of the current edit item.</value>
        IEditItemState CurrentEditItemState { get; }

        /// <summary>
        /// Creates the record entry.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry CreateRecordEntry(object data);

        /// <summary>
        /// Gets the property access provider.
        /// </summary>
        /// <returns></returns>
        IPropertyAccessProvider GetPropertyAccessProvider();

        /// <summary>
        /// Record Property Changed event handler
        /// </summary>
        event PropertyChangedEventHandler RecordPropertyChanged;
#if !SILVERLIGHT
        /// <summary>
        /// Gets the item properties.
        /// </summary>
        /// <returns></returns>
        PropertyDescriptorCollection GetItemProperties();
#else
        /// <summary>
        /// Gets the item properties.
        /// </summary>
        /// <returns></returns>
        PropertyInfoCollection GetItemProperties();
#endif

        /// <summary>
        /// Enable customization for runtime dynamic objects in LINQ queries.
        /// </summary>
        /// <param name="expressionFunctor"></param>
        void SetCustomExpressionFunc(IUnboundExpressionFunc expressionFunctor);

        /// <summary>
        /// Enable groupe expand state persistance. This will affect performance if there are large number of groups.
        /// </summary>
        bool PersistGroupsExpandedState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is groups expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is groups expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsGroupsExpanded { get; set; }

        /// <summary>
        /// Suspends this instance.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        void Resume();

        /// <summary>
        /// Refresh toplevel groups --Added for the fix SD8300.
        /// </summary>
        void RefreshFiltering();
    }

    public interface ISortDirection
    {
        ListSortDirection SortDirection
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Returns an Expression to embed in LINQ operation queries implemented by <see cref="ICollectionViewAdv"/>.
    /// </summary>
    public interface IUnboundExpressionFunc
    {
        /// <summary>
        /// Custom Functor to enable runtime customized objects over default operations other than LINQ queries.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        Func<string, object, object> GetFunc(string propertyName);

        /// <summary>
        /// Custom Functor to enable runtime customized objects over default operations other than LINQ queries.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        Func<string, object, object> GetTypeFunc(string propertyName);

        /// <summary>
        /// Custom expression functor to enable runtime customized objects into LINQ queries.
        /// </summary>
        /// <param name="propertyName"></param>
        Expression<Func<string, object, object>> GetExpressionFunc(string propertyName);

        /// <summary>
        /// Custom expression functor to enable runtime customized objects into LINQ queries.
        /// </summary>
        /// <param name="propertyName"></param>
        Expression<Func<string, object, object>> GetTypeExpressionFunc(string propertyName);
    }

    /// <summary>
    /// Implements an IList structure to contain the list of RecordEntry.
    /// </summary>
    public interface IRecordsList : IRecordsEntryList
    {
        /// <summary>
        /// Gets the table summaries.
        /// </summary>
        /// <value>The table summaries.</value>
        IList<SummaryRecordEntry> TableSummaries
        {
            get;
        }

        /// <summary>
        /// Returns the index for the underlying record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        int IndexOfRecord(object data);


        /// <summary>
        /// Gets the item at index specified.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        /// <returns></returns>
        object GetItemAt(int recordIndex);


        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Data.RecordEntry"/> for the underlying business object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry GetRecord(object data);


        /// <summary>
        /// Creates the <see cref="Syncfusion.Windows.Data.RecordEntry"/> for the business object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry CreateRecordEntry(object data);

        /// <summary>
        /// Dispose all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        void ReomveAll();
    }

    /// <summary>
    /// Exposes method to Get/Set from the underlying object.
    /// </summary>
    public interface IPropertyAccessProvider
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        object GetValue(object record, string propName);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool SetValue(object record, string propName, object value);
    }

#if !SILVERLIGHT && SyncfusionFramework4_0
    /// <summary>
    /// Implement this interface to instruct the QueryableCollectionView derived view for generating PLINQ query expression trees.
    /// </summary>
    public interface IParallelizableView
    {
        bool UsePLINQ { get; set; }
    }
#endif
    //Temp Added
#if !SILVERLIGHT
    public sealed class PageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructor that takes the target page index
        /// </summary>
        /// <param name="newPageIndex">Index of the requested page</param>
        public PageChangingEventArgs(int newPageIndex)
        {
            this.NewPageIndex = newPageIndex;
        }

        /// <summary>
        /// Gets the index of the requested page
        /// </summary>
        public int NewPageIndex
        {
            get;
            private set;
        }
    }
    public interface IPagedCollectionView
    {
        /// <summary>
        /// Raised when a page index change completed.
        /// </summary>
        event EventHandler<EventArgs> PageChanged;

        /// <summary>
        /// Raised when a page index change is requested.
        /// </summary>
        event EventHandler<PageChangingEventArgs> PageChanging;

        /// <summary>
        /// Gets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        bool CanChangePage
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether a page index change is in process or not.
        /// </summary>
        bool IsPageChanging
        {
            get;
        }

        /// <summary>
        /// Gets the number of known items in the view before paging is applied.
        /// </summary>
        int ItemCount
        {
            get;
        }

        /// <summary>
        /// Gets the current page we are on. (zero based)
        /// </summary>
        int PageIndex
        {
            get;
        }

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total number of items in the view before paging is applied,
        /// or -1 if that total number is unknown.
        /// </summary>
        int TotalItemCount
        {
            get;
        }

        /// <summary>
        /// Moves to the first page.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToFirstPage();

        /// <summary>
        /// Moves to the last page.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToLastPage();

        /// <summary>
        /// Moves to the page after the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToNextPage();

        /// <summary>
        /// Moves to the page before the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToPreviousPage();

        /// <summary>
        /// Moves to page <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page to which to move.</param>
        /// <returns>Whether or not the move was successful.</returns>
        bool MoveToPage(int pageIndex);
    }
#endif
    //Till This

#if SILVERLIGHT
    public interface IEditableCollectionView
    {
        bool EnablePaging
        {
            get;
            set;
        }
        bool IsViewLevelPaging
        {
            get;
            set;
        }

        PagedCollectionView PagedSource
       {
           get;
           set;
       }


        //Till This
        // Summary:
        //     Gets a value indicating whether the view supports System.ComponentModel.IEditableCollectionView.AddNew().
        bool CanAddNew { get; }
        //
        // Summary:
        //     Gets a value indicating whether the view supports the notion of "pending
        //     changes" on the current edit item. This may vary, depending on the view and
        //     the particular item. For example, a view might return true if the current
        //     edit item implements IEditableObject, or if the view has special knowledge
        //     about the item that it can use to support rollback of pending changes.
        bool CanCancelEdit { get; }
        //
        // Summary:
        //     Gets a value indicating whether the view supports System.ComponentModel.IEditableCollectionView.Remove(System.Object)
        //     and System.ComponentModel.IEditableCollectionView.RemoveAt(System.Int32).
        bool CanRemove { get; }
        //
        // Summary:
        //     Gets the new item when an System.ComponentModel.IEditableCollectionView.AddNew()
        //     transaction is in progress.  Otherwise it returns null.
        object CurrentAddItem { get; }
        //
        // Summary:
        //     Gets the affected item when an System.ComponentModel.IEditableCollectionView.EditItem(System.Object)
        //     transaction is in progress.  Otherwise it returns null.
        object CurrentEditItem { get; }
        //
        // Summary:
        //     Gets a value indicating whether an System.ComponentModel.IEditableCollectionView.AddNew()
        //     transaction is in progress.
        bool IsAddingNew { get; }
        //
        // Summary:
        //     Gets a value indicating whether an System.ComponentModel.IEditableCollectionView.EditItem(System.Object)
        //     transaction is in progress.
        bool IsEditingItem { get; }
        //
        // Summary:
        //     Gets or sets whether to include a placeholder for a new item, and if so,
        //     where to put it.
        NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }

        // Summary:
        //     Add a new item to the underlying collection. Returns the new item.  After
        //     calling AddNew and changing the new item as desired, either System.ComponentModel.IEditableCollectionView.CommitNew()
        //     or System.ComponentModel.IEditableCollectionView.CancelNew() should be called
        //     to complete the transaction.
        //
        // Returns:
        //     The new item that gets created
        object AddNew();
        //
        // Summary:
        //     Complete the transaction started by System.ComponentModel.IEditableCollectionView.EditItem(System.Object).
        //      The pending changes (if any) to the item are discarded.
        void CancelEdit();
        //
        // Summary:
        //     Complete the transaction started by System.ComponentModel.IEditableCollectionView.AddNew().
        //     The new item is removed from the collection.
        void CancelNew();
        //
        // Summary:
        //     Complete the transaction started by System.ComponentModel.IEditableCollectionView.EditItem(System.Object).
        //      The pending changes (if any) to the item are committed.
        void CommitEdit();
        //
        // Summary:
        //     Complete the transaction started by System.ComponentModel.IEditableCollectionView.AddNew().
        //     The new item remains in the collection, and the view's sort, filter, and
        //     grouping specifications (if any) are applied to the new item.
        void CommitNew();
        //
        // Summary:
        //     Begins an editing transaction on the given item. The transaction is completed
        //     by calling either System.ComponentModel.IEditableCollectionView.CommitEdit()
        //     or System.ComponentModel.IEditableCollectionView.CancelEdit(). Any changes
        //     made to the item during the transaction are considered "pending", provided
        //     that the view supports the notion of "pending changes" for the given item.
        //
        // Parameters:
        //   item:
        //     Object we wnat to edit
        void EditItem(object item);
        //
        // Summary:
        //     Remove the given item from the underlying collection.
        //
        // Parameters:
        //   item:
        //     Object we want to remove
        void Remove(object item);
        //
        // Summary:
        //     Remove the item at the given index from the underlying collection.  The index
        //     is interpreted with respect to the view (not with respect to the underlying
        //     collection).
        //
        // Parameters:
        //   index:
        //     Index of item to remove
        void RemoveAt(int index);
    }
    public enum NewItemPlaceholderPosition
    {
        None,
        AtBeginning,
        AtEnd
    }

#if SyncfusionFramework3_5 && !SyncfusionFramework4_0
    // Summary:
    //     Specifies that this object supports a simple, transacted notification for
    //     batch initialization.
    public interface ISupportInitialize
    {
        // Summary:
        //     Signals the object that initialization is starting.
        void BeginInit();
        //
        // Summary:
        //     Signals the object that initialization is complete.
        void EndInit();
    }
#endif

    public interface IPropertyChangedEventHandler
    {
        void OnPropertyChanged(object sender, EventArgs e);
    }
#endif
}
