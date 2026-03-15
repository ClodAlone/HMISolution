//-------------------------------------------------------------------------------------------------
// <copyright file="GridInterfaces.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a <see cref="CreateCellModel"/> method that instantiates <see cref="GridCellModelBase"/> 
    /// objects on demand when a cell is touched that has a <see cref="GridStyleInfo.CellType"/> that is 
    /// not found in the <see cref="GridModel.CellModels"/> collection.
    /// </summary>
    public interface IGridCellModelFactory
    {
        /// <summary>
        /// Instantiates <see cref="GridCellModelBase"/> objects on demand when a cell is touched that has a <see cref="GridStyleInfo.CellType"/> that is 
        /// not found in the <see cref="GridModel.CellModels"/> collection.
        /// </summary>
        /// <returns>returns GridCellModelBase</returns>
        GridCellModelBase CreateCellModel(string controlId, GridModel pGrid);

        /// <summary>
        /// Gets a value indicating whether true when the grid is allowed to replace this factory with a derived factory at any time.
        /// </summary>
        bool IsDefault { get; }
    }
    
    /// <summary>
    /// Provides functionality to an object to return a list that can <see cref="GridModel"/>.
    /// </summary>
    public interface IGridModelSource
    {
        /// <summary>
        /// Gets a reference to a <see cref="GridModel"/>.
        /// </summary>
        GridModel Model { get; }
    }

    /// <summary>
    /// Provides a one-stop place to subscribe to row count, column count, and QueryCellInfo events.
    /// </summary>
    /// <remarks>
    /// You should implement <see cref="IGridModelDataProvider"/> if you want to receive
    /// <see cref="GridModel.QueryRowCount"/>, <see cref="GridModel.QueryColCount"/>,
    /// <see cref="GridModel.QueryCellInfo"/>, and <see cref="GridModel.SaveCellInfo"/> events.
    /// <para/>
    /// The methods in this interface are called before the named events are raised and thus
    /// give you a chance to control the events behavior before other subscribers can handle it.
    /// <para/>
    /// You should assign a reference of your object to <see cref="GridModel.DataProvider"/> in order
    /// to receive the method calls.
    /// </remarks>
    public interface IGridModelDataProvider
    {
        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryRowCount"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridRowColCountEventArgs"/> that contains the event data.</param>
        void QueryRowCount(GridRowColCountEventArgs e);

        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryColCount"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridRowColCountEventArgs"/> that contains the event data.</param>
        void QueryColCount(GridRowColCountEventArgs e);

        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridQueryCellInfoEventArgs"/> that contains the event data.</param>
        void QueryCellInfo(GridQueryCellInfoEventArgs e); 
        
        /// <summary>
        /// Method handler for the <see cref="GridModel.SaveCellInfo"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridSaveCellInfoEventArgs"/> that contains the event data.</param>
        void SaveCellInfo(GridSaveCellInfoEventArgs e);

        /// <summary>
        /// Returns the column index for a column that matches a given name.
        /// </summary>
        /// <param name="name">The name of the field to be matched.</param>
        /// <returns>The column index in the grid; -1 if not found.</returns>
        int NameToColIndex(string name);

        /// <summary>
        /// Returns the row index for a row that matches a given name.
        /// </summary>
        /// <param name="name">The name of the row to be matched.</param>
        /// <returns>The row index in the grid; -1 if not found.</returns>
        int NameToRowIndex(string name);
    }

    /// <summary>
    /// See <see cref="GridViewLayout.VisitVisibleCells"/>.
    /// </summary>
    /// <returns>returns GridRangeInfo</returns>
    public delegate GridRangeInfo GridRowColRangeInfoHandler(int row, int col);

    /// <summary>
    /// Provides the <see cref="GetAllowFixFocus"/> method which is called for the active <see cref="IMouseController"/>
    /// from within the grids <see cref="Control.OnMouseDown"/> handler to determine if the grid should set focus to the
    /// active current cells <see cref="Control"/>.
    /// </summary>
    public interface IGridFocusHelper
    {
        /// <summary>
        /// Implement this method in your <see cref="IMouseController"/> and return False if it would interfere with your 
        /// controller's state when the current cell is focused and possibly scrolled into view.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> that indicates if the grid is allowed to set the focus onto the current cells <see cref="Control"/>.
        /// </returns>
        bool GetAllowFixFocus();
    }

    /// <summary>
    /// Provides support for hosting a windowless grid control. Such windowless controls
    /// are used inside GridGroupingControl for nested tables.
    /// </summary>
    public interface IGridWindowlessSite
    {
        /// <summary>
        /// Returns the actual parent that has a window handle.
        /// </summary>
        /// <returns>A parent control with window handle.</returns>
        Control GetWindow();

        /// <summary>
        /// Returns the visible bounds of the parent control.
        /// </summary>
        /// <returns>A rectangle with coordinates of the parent control.</returns>
        Rectangle GetVisibleBounds();
    }

    /// <summary>
    /// Adds support for using the grid control as a windowless control. Such windowless controls
    /// are used inside GridGroupingControl for nested tables. A windowless control
    /// has no window handle. It only forwards paint and other window operations
    /// to the parent control. The parent control will forward mouse and keyboard messages
    /// to the windowless control.
    /// </summary>
    public interface IGridWindowlessObject: IFindParentForm
    {
        /// <summary>
        /// Gets or sets a value indicating whether the control is used in windowless mode.
        /// </summary>
        bool IsWindowless { get; set; }

        /// <summary>
        /// Gets or sets the parent control that implements IGridWindowlessSite.
        /// </summary>
        IGridWindowlessSite ParentSite { get; set; }
    }

    /// <summary>
    /// Provides the <see cref="DrawSelectionFrame"/> method which is internally used for drawing of Excel-like selection frame.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface IGridDrawSelectionFrame
    {
        /// <summary>
        /// Internally used for drawing of Excel-like selection frame.
        /// </summary>
        /// <param name="pGrid">The grid control</param>
        /// <param name="g">The graphics</param>
        /// <param name="bDrawOld">The draw old</param>
        /// <param name="pNewRange">The GridRangeInfo</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void DrawSelectionFrame(GridControlBase pGrid, Graphics g, bool bDrawOld, GridRangeInfo pNewRange);
    }

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface IGridPaintSelectCells
    {
        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        void UpdateSelectRange(GridRangeInfo range, GridRangeInfoList pOldRangeList);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        void PrepareClearSelection();

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        void PrepareChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange);
    }
}
