#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
#if WinRT
using Windows.UI.Xaml.Data;

#elif WPF
using System.Windows;
#endif

namespace Syncfusion.Data
{
    public interface ICollectionViewAdv : ICollectionView, INotifyPropertyChanged, ISupportInitialize,
                                          IDisposable, IPropertyChangedEventHandler, IEditableCollectionView
#if !WinRT
        ,ISupportIncrementalLoading
#endif
    {

#if !WP
        bool IsDynamicBound { get; set; }
#endif
        /// <summary>
        /// Gets the records list structure.
        /// </summary>
        /// <value>The records.</value>
        IRecordsList Records { get; }

        /// <summary>
        /// Gets the top level group.
        /// </summary>
        /// <value>The top level group.</value>
        TopLevelGroup TopLevelGroup { get; }

        /// <summary>
        /// Gets or sets the caption summary row.
        /// </summary>
        /// <value>The caption summary row.</value>
        ISummaryRow CaptionSummaryRow { get; set; }

        /// <summary>
        /// Gets the summary rows.
        /// </summary>
        /// <value>The summary rows.</value>
        ObservableCollection<ISummaryRow> SummaryRows { get; }

        /// <summary>
        /// Gets the table summary rows.
        /// </summary>
        /// <value>The table summary rows.</value>
        ObservableCollection<ISummaryRow> TableSummaryRows { get; }

        /// <summary>
        /// Defines the custom group comparer to enable customization of the group sort order.
        /// </summary>
        IComparer<Group> GroupComparer { get; set; }

        /// <summary>
        /// Gets the sort comparers for the sort description.
        /// </summary>
        SortComparers SortComparers { get; }

        /// <summary>
        /// Creates the record entry.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry CreateRecordEntry(object data);

#if SILVERLIGHT
        void Remove(object item);
#endif

        /// <summary>
        /// Gets the property access provider.
        /// </summary>
        /// <returns></returns>
        IPropertyAccessProvider GetPropertyAccessProvider();

        /// <summary>
        /// Record Property Changed event handler
        /// </summary>
        event PropertyChangedEventHandler RecordPropertyChanged;
        event NotifyCollectionChangedEventHandler SourceCollectionChanged;
#if WPF
        bool IsLegacyDataTable { get; }
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

        bool FilterRecord(object record);

        void RefreshFilter();

        /// <summary>
        /// Gets or sets the filter predicates.
        /// </summary>
        /// <value>The filter predicates.</value>
        ObservableCollection<IFilterDefinition> FilterPredicates { get; set; }

        RecordEntry GetRecordAt(int index);

        void OnCollectionChanged(NotifyCollectionChangedEventArgs args);
        //WinRT//

        LiveDataUpdateMode LiveDataUpdateMode { get; set; }

#if WinRT
        ObservableCollection<GroupDescription> GroupDescriptions { get; }

        SortDescriptionCollection SortDescriptions { get; }

        IEnumerable SourceCollection { get; }

        IDisposable DeferRefresh();

        void Refresh();

        CultureInfo Culture { get; set; }

        Predicate<object> Filter { get; set; }

        bool CanFilter { get; }
#endif

        bool AutoExpandGroups { get; set; }
    }

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

    public interface IPropertyChangedEventHandler
    {
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }

#if !WinRT

    public interface ISupportIncrementalLoading
    {
        void LoadMoreItemsAsync(uint count);

        bool HasMoreItems { get; }
    }

#endif

#if !WPF
    // Summary:
    //     Defines methods and properties that a System.Windows.Data.CollectionView
    //     implements to provide editing capabilities to a collection.
    public interface IEditableCollectionView
    {
        // Summary:
        //     Gets a value that indicates whether a new item can be added to the collection.
        //
        // Returns:
        //     true if a new item can be added to the collection; otherwise, false.
        bool CanAddNew { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the collection view can discard pending
        //     changes and restore the original values of an edited object.
        //
        // Returns:
        //     true if the collection view can discard pending changes and restore the original
        //     values of an edited object; otherwise, false.
        bool CanCancelEdit { get; }
        //
        // Summary:
        //     Gets a value that indicates whether an item can be removed from the collection.
        //
        // Returns:
        //     true if an item can be removed from the collection; otherwise, false.
        bool CanRemove { get; }
        //
        // Summary:
        //     Gets the item that is being added during the current add transaction.
        //
        // Returns:
        //     The item that is being added if System.ComponentModel.IEditableCollectionView.IsAddingNew
        //     is true; otherwise, null.
        object CurrentAddItem { get; }
        //
        // Summary:
        //     Gets the item in the collection that is being edited.
        //
        // Returns:
        //     The item in the collection that is being edited if System.ComponentModel.IEditableCollectionView.IsEditingItem
        //     is true; otherwise, null.
        object CurrentEditItem { get; }
        //
        // Summary:
        //     Gets a value that indicates whether an add transaction is in progress.
        //
        // Returns:
        //     true if an add transaction is in progress; otherwise, false.
        bool IsAddingNew { get; }
        //
        // Summary:
        //     Gets a value that indicates whether an edit transaction is in progress.
        //
        // Returns:
        //     true if an edit transaction is in progress; otherwise, false.
        bool IsEditingItem { get; }
        //
        // Summary:
        //     Gets or sets the position of the new item placeholder in the collection view.
        //
        // Returns:
        //     One of the enumeration values that specifies the position of the new item
        //     placeholder in the collection view.
        NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }

        // Summary:
        //     Adds a new item to the collection.
        //
        // Returns:
        //     The new item that is added to the collection.
        object AddNew();
        //
        // Summary:
        //     Ends the edit transaction and, if possible, restores the original value to
        //     the item.
        void CancelEdit();
        //
        // Summary:
        //     Ends the add transaction and discards the pending new item.
        void CancelNew();
        //
        // Summary:
        //     Ends the edit transaction and saves the pending changes.
        void CommitEdit();
        //
        // Summary:
        //     Ends the add transaction and saves the pending new item.
        void CommitNew();
        //
        // Summary:
        //     Begins an edit transaction of the specified item.
        //
        // Parameters:
        //   item:
        //     The item to edit.
        void EditItem(object item);

#if !WinRT
        //
        // Summary:
        //     Removes the item at the specified position from the collection.
        //
        // Parameters:
        //   index:
        //     The position of the item to remove.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0 or greater than the number of items in the collection
        //     view.
        void RemoveAt(int index);
#endif
    }

    // Summary:
    //     Provides data for the System.ComponentModel.INotifyPropertyChanging.PropertyChanging
    //     event.
    public class PropertyChangingEventArgs : EventArgs
    {
        // Summary:
        //     Initializes a new instance of the Syncfusion.Data.PropertyChangingEventArgs
        //     class.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property whose value is changing.
        public PropertyChangingEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        // Summary:
        //     Gets the name of the property whose value is changing.
        //
        // Returns:
        //     The name of the property whose value is changing.
        public virtual string PropertyName
        {
            get;
            internal set;
        }
    }


    

    /// <summary>
    /// Represents the method that will handle the Syncfusion.Data.INotifyPropertyChanging.PropertyChanging
    /// event of an Syncfusion.Data.INotifyPropertyChanging interface.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="T:Syncfusion.Data.PropertyChangingEventArgs">PropertyChangingEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);


    // Summary:
    //     Notifies clients that a property value is changing.
    public interface INotifyPropertyChanging
    {
        // Summary:
        //     Occurs when a property value is changing.
        event PropertyChangingEventHandler PropertyChanging;
    }
#endif

}