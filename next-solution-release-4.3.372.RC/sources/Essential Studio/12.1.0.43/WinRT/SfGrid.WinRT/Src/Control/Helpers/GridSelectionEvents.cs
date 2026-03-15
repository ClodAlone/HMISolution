#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Collections.Generic;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    #region Event args & Handlers

    /// <summary>
    /// Delegate for Selection Changing Event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridSelectionChangingEventArgs">GridSelectionChangingEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void GridSelectionChangingEventHandler(object sender, GridSelectionChangingEventArgs e);

    /// <summary>
    /// Class which contains the arguments for SelectionChanging Event.
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public class GridSelectionChangingEventArgs : GridCancelEventArgs
    {
        public GridSelectionChangingEventArgs(object originalSource)
            : base(originalSource)
        {

        }

        /// <summary>
        /// List of items which are going to add in SelectedItems
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IList<object> AddedItems { get; internal set; }

        /// <summary>
        /// List of items which are removed from SelectedItems.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IList<object> RemovedItems { get; internal set; }

        public IList<int> AddedIndexs { get; internal set; }

        public IList<int> RemovedIndexs { get; internal set; }
    }

    /// <summary>
    /// Delegate for SelectionChanged event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs">GridSelectionChangedEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void GridSelectionChangedEventHandler(object sender, GridSelectionChangedEventArgs e);

    /// <summary>
    /// Class which contains the Sarguments for SelectionChanged event.
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public class GridSelectionChangedEventArgs : GridEventArgs
    {

        public GridSelectionChangedEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        /// <summary>
        /// List of items which are added to SelectedItems
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IList<object> AddedItems { get; internal set; }

        /// <summary>
        /// List of items which are removed from SelectedItems.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IList<object> RemovedItems { get; internal set; }

        public IList<int> AddedIndexs { get; internal set; }

        public IList<int> RemovedIndexs { get; internal set; }
    }

    #endregion
}
