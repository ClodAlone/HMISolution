#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{

    /// <summary>
    /// Represents a method that handles a cancelable <see cref="GridControlBase.CurrentCellActivating"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="GridCurrentCellActivatingEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellActivatingEventHandler(object sender, GridCurrentCellActivatingEventArgs args);


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
    public class GridCurrentCellActivatingEventArgs : SyncfusionCancelRoutedEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="cellRowColumnIndex">The current cell's row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="activateOptions">A <see cref="GridActivateCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>
        public GridCurrentCellActivatingEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.CellRowColumnIndex = cellRowColumnIndex;
            this.ActivateOptions = activateOptions;
        }

        /// <summary>
        /// The row and column indices of the current cell.
        /// </summary>
        [TraceProperty(true)]
        public RowColumnIndex CellRowColumnIndex
        {
            get;
            set;
        }

        /// <summary>
        /// A <see cref="GridActivateCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.
        /// </summary>
        [TraceProperty(true)]
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
    /// <param name="args">An <see cref="GridCurrentCellActivateFailedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellActivateFailedEventHandler(object sender, GridCurrentCellActivateFailedEventArgs args);


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
    public class GridCurrentCellActivateFailedEventArgs : SyncfusionRoutedEventArgs
    {
        /// <internalonly/>
        internal RowColumnIndex cellRowColumnIndex;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="cellRowColumnIndex">The current cell's row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>
        public GridCurrentCellActivateFailedEventArgs(RowColumnIndex cellRowColumnIndex,
            RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// The row and column indices of current cell.
        /// </summary>
        [TraceProperty(true)]
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
    /// <param name=" args">An <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellDeactivatedEventHandler(object sender, GridCurrentCellDeactivatedEventArgs args);


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
    public class GridCurrentCellDeactivatedEventArgs : GridCellEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="cellRowColumnIndex">The current cell's row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>
        public GridCurrentCellDeactivatedEventArgs(RowColumnIndex cellRowColumnIndex,
            RoutedEvent routedEvent,
            object source)
            : base(cellRowColumnIndex, routedEvent, source)
        {
        }
    }



    /// <summary>
    /// Represents a method that handles events associated with a specific cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="GridCellEventArgs"/> that contains the event data.</param>
    public delegate void GridCellEventHandler(object sender, GridCellEventArgs args);

    /// <summary>
    /// Holds row and column coordinates for events associated with a specific cell.
    /// </summary>
    /// <remarks>
    /// Directly used by <see cref="GridCellModelBase.ActiveTextChanged"/>, <see cref="GridCellButton.Clicked"/>, <see cref="GridCellButton.HoveringChanged"/>,
    /// <see cref="GridCellButton.MouseDownChanged"/>, and <see cref="GridCellButton.PushedChanged"/>.
    /// <para/>
    /// Used also as base class for several other events related to a specific cell.
    /// </remarks>
    public class GridCellEventArgs : SyncfusionRoutedEventArgs
    {
        /// <internalonly/>
        internal RowColumnIndex cellRowColumnIndex;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridCellEventArgs(RowColumnIndex cellRowColumnIndex,
            RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// The row and column indices of the cell.
        /// </summary>
        [TraceProperty(true)]
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
    /// <param name="args">An <see cref="GridCurrentCellMovedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMovedEventHandler(object sender, GridCurrentCellMovedEventArgs args);

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
    public class GridCurrentCellMovedEventArgs : SyncfusionRoutedEventArgs
    {
        GridActivateCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>
        public GridCurrentCellMovedEventArgs(GridActivateCurrentCellOptions options,
            RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
            this.options = options;
        }

        /// <summary>
        /// The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.
        /// </summary>
        [TraceProperty(true)]
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
    /// <param name="args">An <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMoveFailedEventHandler(object sender, GridCurrentCellMoveFailedEventArgs args);


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
    public class GridCurrentCellMoveFailedEventArgs : GridCellEventArgs
    {
        GridActivateCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="cellRowColumnIndex">The current cell's row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>        
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        public GridCurrentCellMoveFailedEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options, RoutedEvent routedEvent, object source)
            : base(cellRowColumnIndex, routedEvent, source)
        {
            this.options = options;
        }

        /// <summary>
        /// The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.
        /// </summary>
        [TraceProperty(true)]
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
    /// <param name="args">An <see cref="GridCurrentCellMovingEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellMovingEventHandler(object sender, GridCurrentCellMovingEventArgs args);

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
    public class GridCurrentCellMovingEventArgs : GridCurrentCellActivatingEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="cellRowColumnIndex">The current cell's row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo"/>
        /// operation.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The event source.</param>
        public GridCurrentCellMovingEventArgs(RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options, RoutedEvent routedEvent, object source)
            : base(cellRowColumnIndex, options, routedEvent, source)
        {
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CurrentCellShowingDropDown"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellShowingDropDownEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellShowingDropDownEventHandler(object sender, GridCurrentCellShowingDropDownEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CurrentCellShowingDropDown"/> event when
    /// the grid drops-down the drop-down portion of the current cell.
    /// </summary>
    /// <para/>
    /// To abort the drop-down operation, set <see cref="CancelEventArgs.Cancel"/> to True.
    /// <para/>
    /// If you need to get access to the cell renderer, you can use the <see cref="GridCurrentCell.Renderer"/>
    /// property of the <see cref="GridControlBase.CurrentCell"/> object. The <see cref="GridControlBase.CurrentCell"/> object
    /// also holds style information and row and column index. See the cell renderer for properties to access
    /// the drop-down container and drop-down part.
    /// </remarks>
    public sealed class GridCurrentCellShowingDropDownEventArgs : SyncfusionCancelRoutedEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="size">The suggested size for the drop-down control. You can change this
        /// size in your event handler.</param>
        public GridCurrentCellShowingDropDownEventArgs(bool isDropDownOpen, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.isDropDownOpen = isDropDownOpen;
        }

        private bool isDropDownOpen = false;
        public bool IsDropDownOpen
        {
            get
            {
                return this.isDropDownOpen;
            }
        }
    }
}
