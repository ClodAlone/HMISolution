#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Syncfusion.UI.Xaml.Grid
{
    public delegate void GridSortColumnsChangedEventHandler(object sender, GridSortColumnsChangedEventArgs e);

    [ClassReference(IsReviewed = false)]
    public sealed class GridSortColumnsChangedEventArgs : GridEventArgs
    {
        public GridSortColumnsChangedEventArgs(IList<SortColumnDescription> addedItems, IList<SortColumnDescription> removedItems, NotifyCollectionChangedAction action, object originalSource)
            : base(originalSource)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
            this.Action = action;
        }
        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList<SortColumnDescription> AddedItems { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList<SortColumnDescription> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }
    }

    public delegate void GridSortColumnsChangingEventHandler(object sender, GridSortColumnsChangingEventArgs e);

    [ClassReference(IsReviewed = false)]
    public sealed class GridSortColumnsChangingEventArgs : GridCancelEventArgs
    {
        public GridSortColumnsChangingEventArgs(IList<SortColumnDescription> addedItems, IList<SortColumnDescription> removedItems, NotifyCollectionChangedAction action, object originalSource)
            : base(originalSource)
        {
            this.AddedItems = addedItems;
            this.RemovedItems = removedItems;
            this.Action = action;
        }
        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>The added items.</value>
        public IList<SortColumnDescription> AddedItems { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>The removed items.</value>
        public IList<SortColumnDescription> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether SfDataGrid should scroll to current selected item while sorting.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool CancelScroll { get; set; }
    }
}
