//-------------------------------------------------------------------------------------------------
// <copyright file="GridCurrentCellEvents.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;

using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.CurrentCellValidateString"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellValidateStringEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellValidateStringEventHandler(object sender, GridCurrentCellValidateStringEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CurrentCellValidateString"/> event.
    /// </summary>
    /// <remarks>
    /// The GridCurrentCellValidateStringEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellValidateString"/> event that notifies you
    /// when the user types text into the current cell.<para/>
    /// To restrict user input into the current cell while entering text, you can set <see cref="CancelEventArgs.Cancel"/>
    /// to True.
    /// </remarks>
    public class GridCurrentCellValidateStringEventArgs : SyncfusionCancelEventArgs
    {
        string text;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="text">The text that will be entered into the current cell.</param>
        public GridCurrentCellValidateStringEventArgs(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Gets the text that will be entered into the current cell.
        /// </summary>
        [TraceProperty(true)]
        public string Text
        {
            get
            {
                return text;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.CurrentCellInitializeControlText"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellInitializeControlTextEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellInitializeControlTextEventHandler(object sender, GridCurrentCellInitializeControlTextEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CurrentCellInitializeControlText"/> event.
    /// </summary>
    /// <remarks>
    /// The GridCellInitializeControlTextEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellInitializeControlText"/> event that notifies you
    /// that the current cell is initialized with text to be displayed in the associated control, e.g. a text box control.
    /// </remarks>
    public class GridCurrentCellInitializeControlTextEventArgs : SyncfusionCancelEventArgs
    {
        int rowIndex;
        int colIndex;
        GridStyleInfo style;
        object cellValue;
        string controlText;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex"> The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style"> Style information.</param>
        /// <param name="cellValue">The cell value that is assigned to the cell renderer.</param>
        /// <param name="controlText">The text that should be displayed in the active cell.</param>
        public GridCurrentCellInitializeControlTextEventArgs(int rowIndex, int colIndex, GridStyleInfo style, object cellValue, string controlText)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.style = style;
            this.cellValue = cellValue;
            this.controlText = controlText;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        /// <summary>
        /// Gets Style information.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets the cell value that is assigned to the cell renderer.
        /// </summary>
        [TraceProperty(true)]
        public object CellValue
        {
            get
            {
                return cellValue;
            }
        }

        /// <summary>
        /// Gets or sets the text that should be displayed in the active cell.
        /// </summary>
        [TraceProperty(true)]
        public string ControlText
        {
            get
            {
                return controlText;
            }

            set
            {
                controlText = value;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.CurrentCellDeactivated"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
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
    /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
    /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    public class GridCurrentCellDeactivatedEventArgs : GridCellEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCurrentCellDeactivatedEventArgs(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
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
    /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
    /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    public class GridCurrentCellActivateFailedEventArgs : GridCellEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCurrentCellActivateFailedEventArgs(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
        }
    }

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
    /// You can determine if <see cref="GridCurrentCell.Activate(int, int)"/>
    /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
    /// <para/>
    /// Once the current cell has been activated, a <see cref="GridControlBase.CurrentCellActivated"/> event
    /// is raised or a <see cref="GridControlBase.CurrentCellActivateFailed"/> if activating the specified
    /// cell failed.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellActivatingEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellActivating"/>
    /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
    /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
    public class GridCurrentCellActivatingEventArgs : SyncfusionCancelEventArgs
    {
        /// <internalonly/>
        internal int rowIndex;

        /// <internalonly/>
        internal int colIndex;

        /// <internalonly/>
        internal GridSetCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.</param>
        public GridCurrentCellActivatingEventArgs(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
            : base(false)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.options = options;
        }

        /// <summary>
        /// Gets or sets the options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.
        /// </summary>
        [TraceProperty(true)]
        public GridSetCurrentCellOptions Options
        {
            get
            {
                return options;
            }

            set
            {
                options = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
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
    /// before the current cell is moved to a new position as a result from a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    /// method call.
    /// </summary>
    /// <remarks>
    /// The GridCurrentCellMovingEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellMoving"/> event when the current cell
    /// is about to be moved to a new position.
    /// You can disallow the activation of specific cells at run-time when
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
    /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellMovingEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoving"/>
    /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    public class GridCurrentCellMovingEventArgs : GridCurrentCellActivatingEventArgs
    {
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.</param>
        public GridCurrentCellMovingEventArgs(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
            : base(rowIndex, colIndex, options)
        {
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
    /// after the current cell was successfully moved to a new position.
    /// </summary>
    /// <remarks>
    /// GridCurrentCellMovedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CurrentCellMoved"/> event when the current cell
    /// has been successfully moved to a new position.
    /// <para/>
    /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// </remarks>
    /// <seealso cref="GridCurrentCellMovedEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoved"/>
    /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    public class GridCurrentCellMovedEventArgs : SyncfusionEventArgs
    {
        GridSetCurrentCellOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.</param>
        public GridCurrentCellMovedEventArgs(GridSetCurrentCellOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.
        /// </summary>
        [TraceProperty(true)]
        public GridSetCurrentCellOptions Options
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
    /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
    /// order of events that you receive when the current cell is moved.
    /// <para/>
    /// <see cref="GridCurrentCell.ErrorMessage"/> may hold an error message
    /// why the operation failed.
    /// </remarks>
    /// <see cref="GridCurrentCellMoveFailedEventHandler"/>
    /// <seealso cref="GridControlBase.CurrentCellMoveFailed"/>
    /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
    public class GridCurrentCellMoveFailedEventArgs : SyncfusionEventArgs
    {
        GridSetCurrentCellOptions options;
        int rowIndex;
        int colIndex;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">The options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.</param>
        public GridCurrentCellMoveFailedEventArgs(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
            this.options = options;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets or sets the options specified for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// operation.
        /// </summary>
        [TraceProperty(true)]
        public GridSetCurrentCellOptions Options
        {
            get
            {
                return options;
            }

            set
            {
                options = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.DrawCurrentCellBorder"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCurrentCellBorderEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCurrentCellBorderEventHandler(object sender, GridDrawCurrentCellBorderEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.DrawCurrentCellBorder"/> event when
    /// the grid draws a border around the current cell.
    /// </summary>
    /// <remarks>
    /// By default, the grid will draw a black rectangle around the current cell. If the grid does not
    /// have focus, <see cref="GridFocused"/> is False and the grid will draw a dashed border.
    /// <para/>
    /// If you handle this event and implement your own drawing for the current cell border, you should
    /// set <see cref="CancelEventArgs.Cancel"/> to True.
    /// </remarks>
    public sealed class GridDrawCurrentCellBorderEventArgs : GridCellCancelEventArgs 
    {
        Graphics g;
        Rectangle bounds;
        bool gridFocused;
        GridStyleInfo style;
        GridShowCurrentCellBorder showBorder;
    
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="g"> The graphics context.</param>
        /// <param name="bounds">The bounds of the cell rectangle.</param>
        /// <param name="gridFocused">Indicates if grid has focus.</param>
        /// <param name="style">The style information for the current cell.</param>
        /// <param name="showBorder">The settings how and when the current cell border should be drawn.</param>
        public GridDrawCurrentCellBorderEventArgs(int rowIndex, int colIndex, Graphics g, Rectangle bounds, bool gridFocused, GridStyleInfo style, GridShowCurrentCellBorder showBorder) 
            : base(rowIndex, colIndex)
        {
            this.g = g;
            this.bounds = bounds;
            this.gridFocused = gridFocused;
            this.style = style;
            this.showBorder = showBorder;
        }
    
        /// <summary>
        /// Gets the graphics context.
        /// </summary>
        [TraceProperty(true)]
        public Graphics Graphics
        {
            get
            {
                return g;
            }
        }
    
        /// <summary>
        /// Gets the bounds of the cell rectangle.
        /// </summary>
        [TraceProperty(true)]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }
    
        /// <summary>
        /// Gets a value indicating whether grid has focus.
        /// </summary>
        [TraceProperty(true)]
        public bool GridFocused
        {
            get
            {
                return gridFocused;
            }
        }
    
        /// <summary>
        /// Gets the style information for the current cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }
    
        /// <summary>
        /// Gets the settings how and when the current cell border should be drawn.
        /// </summary>
        [TraceProperty(true)]
        public GridShowCurrentCellBorder ShowBorder
        {
            get
            {
                return showBorder;
            }
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
    /// <remarks>
    /// The event will provide a suggested size of the drop-down control. You can change
    /// the default size in your event handler by changing the <see cref="GridCurrentCellShowingDropDownEventArgs.Size"/>
    /// property.
    /// <para/>
    /// Please note however that some drop-down controls might override the suggested height with their own
    /// preferred height. The <see cref="GridDropDownGridListControlPart"/> and <see cref="GridComboBoxListBoxPart"/>
    /// methods both provide a <see cref="GridComboBoxListBoxPart.DropDownRows"/> property that defines the
    /// number of visible rows. 
    /// <para/>
    /// To abort the drop-down operation, set <see cref="CancelEventArgs.Cancel"/> to True.
    /// <para/>
    /// If you need to get access to the cell renderer, you can use the <see cref="GridCurrentCell.Renderer"/>
    /// property of the <see cref="GridControlBase.CurrentCell"/> object. The <see cref="GridControlBase.CurrentCell"/> object
    /// also holds style information and row and column index. See the cell renderer for properties to access
    /// the drop-down container and drop-down part.
    /// </remarks>
    public sealed class GridCurrentCellShowingDropDownEventArgs : SyncfusionCancelEventArgs 
    {
        Size size;
        
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="size">The suggested size for the drop-down control. You can change this
        /// size in your event handler.</param>
        public GridCurrentCellShowingDropDownEventArgs(Size size) 
        {
            this.size = size;
        }
        
        /// <summary>
        /// Gets or sets the suggested size for the drop-down control. You can change this
        /// size in your event handler.
        /// </summary>
        [TraceProperty(true)]
        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
    }  
}