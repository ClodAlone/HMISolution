#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using System;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
#else
using System;
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using System.Windows;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{

    /// <summary>
    /// Represents a method that handles a cancelable <see cref="GridControlBase.CurrentCellActivating"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellActivatingEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellActivatingEventHandler(object sender, GridCurrentCellActivatingEventArgs e);


    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CurrentCellActivating"/> event
    /// before the grid activates the specified cell as current cell.
    /// </summary>
    /// <remarks>
    /// You can disallow the activation of specific cells at run-time when
    /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.RowIndex"/>
    /// and <see cref="GridCurrentCellActivatingEventArgs.ColIndex"/> to activate
    /// a different cell.
    /// <para/>
    /// You can determine if <see cref="GridCurrentCell.Activate"/>
    /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
    /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
    /// <para/>
    /// Once the current cell has been activated, a <see cref="GridControlBase.CurrentCellActivated"/> event
    /// is raised or a <see cref="GridControlBase.CurrentCellActivateFailed"/> if activating the specified
    /// cell failed.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellActivatingEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellActivating"/>
    /// <seealso cref="GridCurrentCell.Activate"/>
    /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
    /// 

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellActivatingEventArgs : SyncfusionCancelRoutedEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridCurrentCellActivatingEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            this.CellRowColumnIndex = cellRowColumnIndex;
            this.ActivateOptions = activateOptions;
        }


        
        public RowColumnIndex CellRowColumnIndex
        {
            get;
            set;
        }


        
        public GridActivateCurrentCellOptions ActivateOptions
        {
            get;
            set;
        }
    }
    
    /// <summary>
    /// Represents a method that handles a cancelable <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellActivateFailedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellActivateFailedEventHandler(object sender, GridCurrentCellActivateFailedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
    /// </summary>
    /// <remarks>
    /// The GridCurrentCellActivateFailedEventArgs is a custom event argument class used by the
    /// GridCurrentCell.CurrentCellActivateFailed"/> event that notifies you
    /// that the current cell could not be activated at the specified cell position.
    /// <para/>
    /// <see cref="GridCurrentCell.ErrorMessage"/> may hold an error message
    /// why the operation failed.
    /// </remarks>
    /// <seealso cref="GridCurrentCellActivateFailedEventHandler"/>
    /// <seealso cref="GridCurrentCell.Activate"/>
    /// <seealso cref="GridCurrentCell.MoveTo"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellActivateFailedEventArgs : EventArgs
    {
        /// <internalonly/>
        internal RowColumnIndex cellRowColumnIndex;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridCurrentCellActivateFailedEventArgs(RowColumnIndex cellRowColumnIndex)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                return cellRowColumnIndex;
            }
            set
            {
                cellRowColumnIndex = value;
            }
        }
    }


    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.CurrentCellDeactivated"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">An <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellDeactivatedEventHandler(object sender, GridCurrentCellDeactivatedEventArgs e);


    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CurrentCellDeactivated"/> event.
    /// </summary>
    /// <remarks>
    /// The GridCurrentCellDeactivatedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellDeactivated"/> event that notifies you
    /// that the current cell has been deactivated at the specified cell position.
    /// </remarks>
    /// <seealso cref="GridCurrentCellDeactivatedEventHandler"/>
    /// <seealso cref="GridCurrentCell.Activate"/>
    /// <seealso cref="GridCurrentCell.MoveTo"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellDeactivatedEventArgs : GridCellEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCurrentCellDeactivatedEventArgs(RowColumnIndex cellRowColumnIndex)
            : base(cellRowColumnIndex)
        {
        }
    }

    
    /// <summary>
    /// Represents a method that handles events associated with a specific cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellEventArgs"/> that contains the event data.</param>
    public delegate void GridCellEventHandler(object sender, GridCellEventArgs e);

    /// <summary>
    /// Holds row and column coordinates for events associated with a specific cell.
    /// </summary>
    /// <remarks>
    /// Directly used by <see cref="GridCellModelBase.ActiveTextChanged"/>, <see cref="GridCellButton.Clicked"/>, <see cref="GridCellButton.HoveringChanged"/>,
    /// <see cref="GridCellButton.MouseDownChanged"/>, and <see cref="GridCellButton.PushedChanged"/>.
    /// <para/>
    /// Used also as base class for several other events related to a specific cell.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellEventArgs : EventArgs
    {
        /// <internalonly/>
        internal RowColumnIndex cellRowColumnIndex;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridCellEventArgs(RowColumnIndex cellRowColumnIndex)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
        }


        
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                return cellRowColumnIndex;
            }
            set
            {
                cellRowColumnIndex = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CurrentCellMoved"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellMovedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMovedEventHandler(object sender, GridCurrentCellMovedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CurrentCellMoved"/> event
    /// after the current cell was succesfully moved to a new position.
    /// </summary>
    /// <remarks>
    /// GridCurrentCellMovedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellMoved"/> event when the current cell
    /// has been succesfully moved to a new position.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellMovedEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoved"/>
    /// <seealso cref="GridCurrentCell.MoveTo"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellMovedEventArgs : SyncfusionRoutedEventArgs
    {
        GridActivateCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        public GridCurrentCellMovedEventArgs(GridActivateCurrentCellOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.
        /// </summary>
        
        public GridActivateCurrentCellOptions Options
        {
            get
            {
                return options;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CurrentCellMoveFailed"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMoveFailedEventHandler(object sender, GridCurrentCellMoveFailedEventArgs e);


    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CurrentCellMoveFailed"/> event
    /// when the current cell fails to be moved to a new position.
    /// </summary>
    /// <remarks>
    /// GridCurrentCellMoveFailedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellMoveFailed"/> event when the current cell 
    /// fails to be moved to a new position.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// <para/>
    /// <see cref="GridCurrentCell.ErrorMessage"/> may hold an error message
    /// why the operation failed.
    /// </remarks>
    /// <see cref="GridCurrentCellMoveFailedEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoveFailed"/>
    /// <seealso cref="GridCurrentCell.MoveTo"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellMoveFailedEventArgs : GridCellEventArgs
    {
        GridActivateCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        public GridCurrentCellMoveFailedEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options)
            : base(cellRowColumnIndex)
        {
            this.options = options;
        }

        /// <summary>
        /// The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.
        /// </summary>
        
        public GridActivateCurrentCellOptions Options
        {
            get
            {
                return options;
            }
        }
    }


    /// <summary>
    /// Represents a method that handles the cancelable <see cref="GridControlBase.CurrentCellMoving"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellMovingEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMovingEventHandler(object sender, GridCurrentCellMovingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CurrentCellMoving"/> event
    /// before the current cell is moved to a new position as a result from a <see cref="GridCurrentCell.MoveTo"/>
    /// method call.
    /// </summary>
    /// <remarks>
    /// The GridCurrentCellMovingEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellMoving"/> event when the current cell
    /// is about to be moved to a new position.
    /// You can disallow the activataion of specific cells at run-time when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.
    /// <para/>
    /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.RowIndex"/>
    /// and <see cref="GridCurrentCellActivatingEventArgs.ColIndex"/> to activate
    /// a different cell.
    /// <para/>
    /// You can also modify the <see cref="GridCurrentCellActivatingEventArgs.Options"/>.
    /// <para/>
    /// Once the current cell has been moved, a <see cref="GridControlBase.CurrentCellMoved"/> event
    /// is raised or a <see cref="GridControlBase.CurrentCellMoveFailed"/> if moving to the specified
    /// target cell failed.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellMovingEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoving"/>
    /// <seealso cref="GridCurrentCell.MoveTo"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrentCellMovingEventArgs : GridCurrentCellActivatingEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        public GridCurrentCellMovingEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options)
            : base(cellRowColumnIndex, options)
        {
        }
    }

}
