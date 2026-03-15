#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Syncfusion.UI.Xaml.Grid
{
    #region Event Arguments & Handlers

    #region RecordDeleting Event Handler
    /// <summary>
    /// Delegate for Record Deleting Event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs">RecordDeletingEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void RecordDeletingEventHandler(object sender, RecordDeletingEventArgs args);

    /// <summary>
    /// Class containing arguments for RecordsDeleting Event
    /// </summary>
    /// <remarks></remarks>
    public class RecordDeletingEventArgs : GridCancelEventArgs
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Deleting Items in the SfDataGrid.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public List<object> Items
        {
            get;
            set;
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="N:Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs">RecordDeletingEventArgs</see> namespace. 
        /// </summary>
        /// <param name="originalSource"></param>
        /// <remarks></remarks>
        public RecordDeletingEventArgs(object originalSource)
            : base(originalSource)
        {

        }
        #endregion
    }
    #endregion

    #region RecordDeleted Event Handler
    /// <summary>
    /// Delegate for Record Deleted Event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs">RecordDeletedEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void RecordDeletedEventHandler(object sender, RecordDeletedEventArgs args);
    public class RecordDeletedEventArgs : GridEventArgs
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Deleted Items in the SfDataGrid.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public List<object> Items
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the CurrentCellIndex in the SfDataGrid.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int SelectedIndex
        {
            get;
            set;
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="N:Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs">RecordDeletedEventArgs</see> namespace. 
        /// </summary>
        /// <param name="originalSource"></param>
        /// <remarks></remarks>
        public RecordDeletedEventArgs(object originalSource)
            : base(originalSource)
        {

        }
        #endregion
    }
    #endregion
    #endregion
}
