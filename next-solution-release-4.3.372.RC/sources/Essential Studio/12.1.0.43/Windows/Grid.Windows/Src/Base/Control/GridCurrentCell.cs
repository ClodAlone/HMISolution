//-------------------------------------------------------------------------------------------------
// <copyright file="GridCurrentCell.cs" company="syncfusion">
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
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Used by GridSelectCellsMouseController.
    /// </summary>
    /// <returns>returns boolean value</returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    public delegate bool GridCurrentCellMoveDelegateHandler(GridDirectionType direction, int num, bool extendSelection);

    /// <summary>
    /// Manages the current cell for a grid. Provides methods for activating, deactivating, and
    /// moving the current cell.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="GridControlBase.CurrentCell"/> property of the <see cref="GridControlBase"/>
    /// class to get access to the <see cref="GridCurrentCell"/> instance associated with a grid.
    /// <para/>
    /// The <see cref="GridCurrentCell"/> class raises events in the associated grid to give you
    /// a chance to adjust the current cell's behavior at any stage.
    /// </remarks>
    /// <example>
    /// The following example shows how you can customize the behavior of the current cell
    /// and highlight the whole row of the current cell instead of just the current cell itself:
    /// <code lang="C#">
    ///         /// Current cell will be moving from one position to another.
    ///         protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
    ///         {
    ///             e.Options |= GridSetCurrentCellOptions.BeginEndUpdate;
    ///             // Instead of GridSetCurrentCellOptions.BeginEndUpdate we could also
    ///             // sandwich the call in a Begin/EndUpdate pair ourselves ...
    ///             //BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
    ///         }
    /// <para/>
    ///         /// Completes a current cell's MoveTo operation indicating success.
    ///         protected override void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
    ///         {
    ///             base.OnCurrentCellMoved(e);
    ///             //EndUpdate();
    ///         }
    /// <para/>
    ///         /// Completes a current cell's MoveTo operation indicating failure.
    ///         protected override void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
    ///         {
    ///             base.OnCurrentCellMoveFailed(e);
    ///             //EndUpdate();
    ///         }
    /// <para/>
    ///         /// Highlight the current row.
    ///         protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
    ///         {
    ///             if (e.RowIndex > this.Model.Rows.HeaderCount &amp;&amp; e.ColIndex > this.Model.Cols.HeaderCount
    ///                 &amp;&amp; CurrentCell.HasCurrentCellAt(e.RowIndex))
    ///             {
    ///                 e.Style.Interior = new BrushInfo(SystemColors.Highlight);
    ///                 e.Style.TextColor = SystemColors.HighlightText;
    ///                 e.Style.Font.Bold = true;
    ///             }
    ///             base.OnPrepareViewStyleInfo(e);
    ///         }
    /// <para/>
    ///         /// Refresh the whole row for the old position of the current cell when it is moved to
    ///         /// a new row or when current cell is deactivated stand-alone.
    ///         protected override void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
    ///         {
    ///             // Check if Deactivate is called stand-alone or called from MoveTo and row is moving.
    ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex)
    ///             {
    ///                 RefreshRange(GridRangeInfo.Row(e.RowIndex), GridRangeOptions.MergeAllSpannedCells);
    ///             }
    ///             base.OnCurrentCellDeactivated(e);
    ///         }
    /// <para/>
    ///         /// Refresh the whole row for the new current cell position when the current cell is moved
    ///         /// to a new row or when current cell is activated stand-alone (and there was no activate current cell).
    ///         protected override void OnCurrentCellActivated(EventArgs e)
    ///         {
    ///             // Check if Activate is called stand-alone or called from MoveTo and row is moving
    ///             base.OnCurrentCellActivated(e);
    ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex
    ///                 || !CurrentCell.MoveFromActiveState)
    ///             {
    ///                 RefreshRange(GridRangeInfo.Row(CurrentCell.RowIndex), GridRangeOptions.MergeAllSpannedCells);
    ///             }
    ///         }
    /// </code>
    /// </example>
    public class GridCurrentCell : GridSubComponent
    {
        // Fields
        private int rowIndex = -1;
        private int colIndex = -1;
        private int keyMoveRowIndex = -1;
        private int keyMoveColIndex = -1;
        internal bool notifyChangingCalled = false;
        private string validationErrorText = string.Empty;

        private GridCellRendererBase cellRenderer
        {
            get
            {
                return (GridCellRendererBase)weakRefCellRenderer.Target;
            }

            set
            {
                weakRefCellRenderer = new WeakReference(value);
            }
        }

        private GridCellModelBase cellModel
        {
            get
            {
                if (cellRenderer != null)
                {
                    return cellRenderer.cellModel;
                }

                return null;
            }
        }

        WeakReference weakRefCellRenderer = new WeakReference(null);

        private bool savedModified = false;
        private bool isModified = false;
        private bool isEditing = false;
        private bool inDeactivate = false;
        private bool inActivate = false;
        private bool inActivated = false;
        private bool inDeactivateFailed = false;
        private bool inActivateFailed = false;
        private bool inShowDropDown = false;
        private bool inConfirmChanges = false;
        private bool inAcceptedChanges = false;
        ////        private bool inRaiseCurrentCellMoved = false;
        private int suspendEvents = 0;
        private bool moveToDone = false;

        private int moveToRowIndex = -1;
        private int moveToColIndex = -1;
        private GridSetCurrentCellOptions moveToOptions;
        private bool moveFromActiveState = false;
        private int moveFromRowIndex = -1;
        private int moveFromColIndex = -1;
        private bool inMoveTo = false;
        internal bool updateInInitialize = true;
        ////private int reason; //// reserved for later use - example: keyboard, mouse etc.
        bool hidden = false;
        bool staticDrawing = false;
        GridControlBase grid;
        private GridModel gridModel
        {
            get
            {
                return grid.Model;
            }
        }

        bool focusRendererOnBeginEdit = true;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool defaultUpdateFlag = true;

        int locked = 0;
        internal bool noScrollInView = false;
        bool raiseExceptionIfNestedActiveOrDeactivate = false;

        bool inEndEdit = false;
        bool inBeginEdit = false;

        bool activateOnGotFocus = false;
        bool isValid = true;
        bool isChanging = false;

        internal bool m_bIgnoreFocus = false;
        string errorMessage = string.Empty;
        internal bool isError = false;
        Exception exception = null;
        private  bool showErrorMessageBox = true;
        private bool showErrorIcon = true;
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool FocusRendererOnBeginEdit
        {
            get
            {
                return focusRendererOnBeginEdit;
            }

            set
            {
                focusRendererOnBeginEdit = value;
            }
        }

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether StaticDrawing. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StaticDrawing
        {
            get
            {
                return staticDrawing;
            }

            set
            {
                staticDrawing = value;
            }
        }

        Control staticRenderControl;

        /// <internalonly/>
        /// <summary>Gets or sets StaticRenderControl. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control StaticRenderControl
        {
            get
            {
                return staticRenderControl;
            }

            set
            {
                staticRenderControl = value;
            }
        }

        //// TODO: Implmenetation

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether InternalHide. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InternalHide
        {
            get
            {
                return hidden;
            }

            set
            {
                hidden = value;
            }
        }

        /// <summary>
        /// Initializes the object, attaches it to a grid, and subscribes to events.
        /// </summary>
        /// <param name="grid">The parent grid for this object.</param>
        public GridCurrentCell(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Gets a string with debug information.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CC: { ");
            sb.Append(string.Concat("Act "));
            if (this.HasCurrentCell)
            {
                sb.Append(this.RangeInfo.ToString() + " ");
                if (this.isModified)
                {
                    sb.Append("Mod ");
                }

                if (this.IsEditing)
                {
                    sb.Append("Edi ");
                }

                if (this.HasControlFocus)
                { 
                    sb.Append("Foc ");
                }
            }

            if (this.inMoveTo)
            {
                sb.Append("InMoveTo ");
                sb.Append(string.Concat(this.moveFromActiveState.ToString(), ", ", this.moveFromRowIndex.ToString(), ", ", this.moveFromColIndex.ToString(), ", "));
                sb.Append(string.Concat("-> ", this.moveToOptions.ToString(), ", ", this.moveToRowIndex.ToString(), ", ", this.moveToColIndex.ToString(), ", "));
            }

            if (this.inActivate)
            {
                sb.Append("InAct ");
            }

            if (this.inActivateFailed)
            {
                sb.Append("InActFail ");
            }

            if (this.inDeactivate)
            {
                sb.Append("InDeact ");
            }

            if (this.inDeactivateFailed)
            {
                sb.Append("InDeactFail ");
            }

            if (this.HasCurrentCell && this.inConfirmChanges)
            {
                sb.Append("InConf ");
            }

            if (this.IsLocked)
            {
                sb.Append("Lock ");
            }

            ////sb.Append(string.Concat("KeyMove: ", this.keyMoveRowIndex.ToString(), ", ", this.keyMoveColIndex.ToString()));

            ////            sb.Append(string.Concat("Act: ", this.HasCurrentCell.ToString(), ", "));
            ////            if (this.HasCurrentCell)
            ////            {
            ////                sb.Append(string.Concat("Pos: ", this.RangeInfo.ToString(), ", "));
            ////                sb.Append(string.Concat("IsModified: ", this.isModified.ToString(), ", "));
            ////                sb.Append(string.Concat("IsEditing: ", this.isEditing.ToString(), ", "));
            ////                sb.Append(string.Concat("HasFocus: ", this.HasControlFocus.ToString(), ", "));
            ////            }
            ////            if (this.inMoveTo)
            ////            {
            ////                sb.Append(string.Concat("InMoveTo: ", this.inMoveTo.ToString(), ", "));
            ////                sb.Append(string.Concat("MoveFrom: ", this.moveFromActiveState.ToString(), ", ", this.moveFromRowIndex.ToString(), ", ", this.moveFromColIndex.ToString(), ", "));
            ////                sb.Append(string.Concat("MoveTo: ", this.moveToOptions.ToString(), ", ", this.moveToRowIndex.ToString(), ", ", this.moveToColIndex.ToString(), ", "));
            ////            }
            ////            if (this.inActivate)
            ////                sb.Append(string.Concat("InActivate: ", this.inActivate.ToString(), ", "));
            ////            if (this.inActivateFailed)
            ////                sb.Append(string.Concat("InActivateFailed: ", this.inActivateFailed.ToString(), ", "));
            ////            if (this.inDeactivate)
            ////                sb.Append(string.Concat("InDeactivate: ", this.inDeactivate.ToString(), ", "));
            ////            if (this.inDeactivateFailed)
            ////                sb.Append(string.Concat("InDeactivateFailed: ", this.inDeactivateFailed.ToString(), ", "));
            ////            if (this.HasCurrentCell && this.inConfirmChanges)
            ////                sb.Append(string.Concat("InConfirmChanges: ", this.inConfirmChanges.ToString(), ", "));
            ////            sb.Append(string.Concat("KeyMove: ", this.keyMoveRowIndex.ToString(), ", ", this.keyMoveColIndex.ToString()));
            sb.Append(string.Concat("}"));
            return sb.ToString();
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.cellRenderer = null;
                this.cellRenderer = null;
                this.isEditing = false;
                this.isModified = false;
            }

            base.Dispose(disposing);
        }

        internal void gridModel_CellModelsChanged(object sender, CollectionChangeEventArgs e)
        {
            this.cellRenderer = null;
        }

        string lastSyncText
        {
            get
            {
                if (gridModel == null)
                {
                    return string.Empty;
                }

                return gridModel.lastSyncText;
            }

            set
            {
                if (gridModel != null)
                { 
                    gridModel.lastSyncText = value;
                }
            }
        }

        internal void gridModel_SynchronizingCurrentCell(object sender, GridCellEventArgs e)
        {
            if (!this.HasControlFocus && !this.IsInMoveTo)
            {
                if (e.RowIndex >= 0 && e.ColIndex >= 0)
                {
                    MoveTo(e.RowIndex, e.ColIndex, GridSetCurrentCellOptions.NoSelectRange | GridSetCurrentCellOptions.NoSetFocus | GridSetCurrentCellOptions.NoSyncCurrentCell);
                }
                ////else
                ////    Deactivate(true);
            }
        }

        internal void GridModelChanged(object sender, EventArgs e)
        {
            this.cellRenderer = null;
            this.colIndex = 0;
            this.rowIndex = 0;
            this.keyMoveRowIndex = 0;
            this.keyMoveColIndex = 0;
        }

        /// <summary>
        /// Gets or sets ExternalMove. Used by GridSelectCellsMouseController.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridCurrentCellMoveDelegateHandler ExternalMove
        {
            get
            {
                return grid.externalMove;
            }

            set
            {
                grid.externalMove = value;
            }
        }

        /// <summary>
        /// Lets you temporarily "lock" the current cell. While a current cell
        /// is locked, any attempts to move, deactivate, save, or activate the current
        /// cell will fail.
        /// </summary>
        public void Lock()
        {
            ////TraceUtil.TraceCurrentMethodInfo(locked, this.grid);
            ////TraceUtil.TraceCalledFrom(3);
            locked++;
        }

        /// <summary>
        /// Unlocks a temporarily "locked" current cell with <see cref="Lock"/>.
        /// </summary>
        public void Unlock()
        {
            if (locked > 0)
            {
                locked--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current cell's state is "locked". If the current cell
        /// is locked, any attempts to move, deactivate, save, or activate the current
        /// cell will fail.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return locked > 0 || Grid.IsDisposed;
            }
        }

        /// <summary>
        /// Gets or sets the row and column index of the current cell as a <see cref="GridRangeInfo"/>.
        /// </summary>
        /// <remarks>
        /// Changing this property will trigger a call to <see cref="MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>. When the
        /// value is <see cref="GridRangeInfo.Empty"/>, the current cell will be deactivated.
        /// </remarks>
        public GridRangeInfo RangeInfo
        {
            get
            {
                int rowIndex, colIndex;
                if (GetCurrentCell(out rowIndex, out colIndex))
                {
                    return GridRangeInfo.Cell(rowIndex, colIndex);
                }

                return GridRangeInfo.Empty;
            }

            set
            {
                if (value.IsEmpty)
                {
                    MoveTo(-1, -1);
                }
                else if (value.Width == 1 && value.Height == 1)
                {
                    MoveTo(value.Top, value.Left);
                }
                else
                { 
                    throw new InvalidEnumArgumentException("value");
                }
            }
        }

        /// <summary>
        /// Invalidates the grid window area where the current cell is positioned.
        /// </summary>
        public void Invalidate()
        {
            int rowIndex, colIndex;
            if (!InternalHide && GetCurrentCell(out rowIndex, out colIndex)
                && Grid.Visible && Grid.IsVisibleCell(rowIndex, colIndex) && Renderer != null && Renderer.ShouldRefreshCurrentCell())
            {
                Grid.InvalidateRange(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current cell is shown in the current view. True if scrolled into view; False if outside current view.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                int rowIndex, colIndex;
                if (!InternalHide && GetCurrentCell(out rowIndex, out colIndex))
                {
                    return Grid.IsVisibleCell(rowIndex, colIndex);
                }

                return false;
            }
        }

        /// <overload>
        /// Moves the current cell down and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveDown()
        {
            MoveDown(1, false);
        }

        /// <summary>
        /// Moves the current cell down to the next enabled row after skipping a specified number of rows.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <genoverload/>
        public void MoveDown(int num)
        {
            MoveDown(num, false);
        }

        /// <summary>
        /// Moves the current cell down to the next enabled row after skipping a specified number of rows and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MoveDown(int num, bool extendSelection)
        {
            Move(GridDirectionType.Down, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell up and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell up to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveUp()
        {
            MoveUp(1, false);
        }

        /// <summary>
        /// Moves the current cell up to the next enabled row after skipping a specified number of rows.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <genoverload/>
        public void MoveUp(int num)
        {
            MoveUp(num, false);
        }

        /// <summary>
        /// Moves the current cell up to the next enabled row after skipping a specified number of rows and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of rows to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MoveUp(int num, bool extendSelection)
        {
            Move(GridDirectionType.Up, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell left and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell left to the next enabled column.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveLeft()
        {
            MoveLeft(1, false);
        }

        /// <summary>
        /// Moves the current cell left to the next enabled column after skipping a specified number of columns.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <genoverload/>
        public void MoveLeft(int num)
        {
            MoveLeft(num, false);
        }

        /// <summary>
        /// Moves the current cell left to the next enabled column after skipping a specified number of columns and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MoveLeft(int num, bool extendSelection)
        {
            Move(GridDirectionType.Left, num, extendSelection);
        }
        
        /// <overload>
        /// Moves the current cell right and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell right to the next enabled column.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MoveRight()
        {
            MoveRight(1, false);
        }

        /// <summary>
        /// Moves the current cell right to the next enabled column after skipping a specified number of columns.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <genoverload/>
        public void MoveRight(int num)
        {
            MoveRight(num, false);
        }

        /// <summary>
        /// Moves the current cell right to the next enabled column after skipping a specified number of columns and selects the
        /// cells.
        /// </summary>
        /// <param name="num">The number of columns to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MoveRight(int num, bool extendSelection)
        {
            Move(GridDirectionType.Right, num, extendSelection);
        }

        /// <overload>
        /// Moves the current cell up one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell up one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MovePageUp()
        {
            MovePageUp(false);
        }

        /// <summary>
        /// Moves the current cell up one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MovePageUp(bool extendSelection)
        {
            Move(GridDirectionType.PageUp, 1, extendSelection);
        }

        /// <overload>
        /// Moves the current cell down one page and optionally selects cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        public void MovePageDown()
        {
            MovePageDown(false);
        }

        /// <summary>
        /// Moves the current cell down one page to the next enabled row.
        /// </summary>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <genoverload/>
        public void MovePageDown(bool extendSelection)
        {
            Move(GridDirectionType.PageDown, 1, extendSelection);
        }

        private bool isWrapCell;        
        /// <overload>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </overload>
        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <param name="wrapCell">Indicates if grid should move to beginning of next row when at end of row or vice versa.</param>
        /// <returns>True if the current cell was moved to a new position; False otherwise (e.g. if current cell is at first row
        /// and you tried to move up).</returns>
        public bool Move(GridDirectionType direction, int num, bool extendSelection, bool wrapCell)
        {
            if (IsLocked)
            {
                return false;
            }

            ////            if (Grid.IsRightToLeft())
            ////            {
            ////                switch (direction)
            ////                {
            ////                    case GridDirectionType.Left:
            ////                        direction = GridDirectionType.Right;
            ////                        break;
            ////                    case GridDirectionType.MostLeft:
            ////                        direction = GridDirectionType.MostRight;
            ////                        break;
            ////                    case GridDirectionType.MostRight:
            ////                        direction = GridDirectionType.MostLeft;
            ////                        break;
            ////                    case GridDirectionType.Right:
            ////                        direction = GridDirectionType.Left;
            ////                        break;
            ////                }
            ////            }

            GridMoveCurrentCellDirectionEventArgs qme = new GridMoveCurrentCellDirectionEventArgs(direction, num, extendSelection, this.RowIndex, this.ColIndex);
            Grid.RaiseMoveCurrentCellDirection(qme);
            if (qme.Handled)
            {
                if (!qme.Result)
                {
                    if (this.Exception != null)
                    {
                        Grid.CancelUpdate();
                        this.DisplayWarningText(this.ErrorMessage);
                    }
                }

                return qme.Result;
            }

            if (wrapCell && grid.CurrentCell.HasCurrentCell)
            {
                GridWrapCellBehavior wrapFlags = Grid.Model.Options.WrapCellBehavior;
                int ri = this.RowIndex;
                int ci = this.ColIndex;
                int lastRowCount = this.gridModel.RowCount;
                int lastColCount = this.gridModel.ColCount;
                int topRow = 1;
                int firstCol = 1;
                if (direction == GridDirectionType.Right)
                {
                    bool isLastCol = ci == gridModel.ColCount;
                    if (!isLastCol)
                    {
                        ci++;
                        isLastCol = !Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref ri, ref ci);
                    }

                    if (isLastCol)
                    {
                        bool isLastRow = ri == gridModel.RowCount;
                        if (!isLastRow)
                        {
                            ri++;
                            ci = this.ColIndex;
                            isLastRow = !Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref ri, ref ci);
                        }

                        if (isLastRow)
                        {
                            if (wrapFlags == GridWrapCellBehavior.WrapGrid)
                            {
                                return Move(GridDirectionType.TopLeft, 1, false, false);
                            }
                            else if (wrapFlags == GridWrapCellBehavior.NextControlInForm)
                            {
                                GridWrapCellNextControlInFormEventArgs e = new GridWrapCellNextControlInFormEventArgs(true, false);
                                Grid.RaiseWrapCellNextControlInForm(e);
                                if (!e.Cancel &&
                                    (!e.MoveTopLeft || Move(GridDirectionType.TopLeft, 1, false, false)))
                                {
                                    Form f = FindFormHelper.FindForm(Grid);
                                    if (f != null)
                                    {
                                        if (!grid.AllowSelectNextControlinProcessDialogKey)
                                        {
                                            return grid.ProcessTabKeyMovingFocus(true);
                                        }

                                        if (f.SelectNextControl(Grid, true, true, false, true) && f.ActiveControl != null)
                                        {
                                            if (f.ActiveControl == Grid && f.SelectNextControl(Grid, true, true, false, true) && f.ActiveControl != null)
                                            {
                                                f.ActiveControl.Focus();
                                            }

                                            f.ActiveControl.Focus();
                                        }
                                    }

                                    return true;
                                }
                            }

                            return false;
                        }
                        else
                        {
                            GridCellActivateAction savedCaa = grid.Model.Options.ActivateCurrentCellBehavior;
                            grid.Model.Options.activateCurrentCellBehavior = GridCellActivateAction.None;
                            grid.BeginUpdateModel(BeginUpdateOptions.InvalidateAndScroll, true);
                            try
                            {
                                if (Move(GridDirectionType.Down, 1, false, false))
                                {
                                    Move(GridDirectionType.MostLeft, 1, false, false);
                                }
                            }
                            finally
                            {
                                grid.EndUpdateModel(defaultUpdateFlag, true);

                                grid.Model.Options.activateCurrentCellBehavior = savedCaa;
                                if ((savedCaa & GridCellActivateAction.SetCurrent) != 0
                                    && HasCurrentCell)
                                {
                                    BeginEdit();
                                }
                            }

                            return true;
                        }
                    }
                }
                else if (direction == GridDirectionType.Left)
                {
                    bool isFirstCol = ci == gridModel.Cols.HeaderCount + 1;
                    if (!isFirstCol)
                    {
                        ci--;
                        isFirstCol = !Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref ri, ref ci);
                    }

                    if (isFirstCol)
                    {
                        bool isFirstRow = ri == gridModel.Rows.HeaderCount + 1;
                        if (!isFirstRow)
                        {
                            ri--;
                            ci = this.ColIndex;
                            isFirstRow = !Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref ri, ref ci);
                        }

                        if (isFirstRow)
                        {
                            if (wrapFlags == GridWrapCellBehavior.WrapGrid)
                            {
                                return Move(GridDirectionType.BottomRight, 1, false, false);
                            }
                            else if (wrapFlags == GridWrapCellBehavior.NextControlInForm)
                            {
                                GridWrapCellNextControlInFormEventArgs e = new GridWrapCellNextControlInFormEventArgs(false, false);
                                Grid.RaiseWrapCellNextControlInForm(e);
                                if (!e.Cancel)
                                {
                                    if (!grid.AllowSelectNextControlinProcessDialogKey)
                                    {
                                        return grid.ProcessTabKeyMovingFocus(false);
                                    }

                                    Form f = FindFormHelper.FindForm(Grid);
                                    if (f != null)
                                    {
                                        if (f.SelectNextControl(Grid, false, true, false, true) && f.ActiveControl != null)
                                        {
                                            f.ActiveControl.Focus();
                                        }
                                    }

                                    return true;
                                }
                            }

                            return false;
                        }
                        else
                        {
                            GridCellActivateAction savedCaa = grid.Model.Options.ActivateCurrentCellBehavior;
                            grid.Model.Options.activateCurrentCellBehavior = GridCellActivateAction.None;
                            grid.BeginUpdateModel(BeginUpdateOptions.InvalidateAndScroll, true);
                            try
                            {
                                if (Move(GridDirectionType.Up, 1, false, false))
                                {
                                    Move(GridDirectionType.MostRight, 1, false, false);
                                }
                            }
                            finally
                            {
                                grid.EndUpdateModel(defaultUpdateFlag, true);
                                grid.Model.Options.activateCurrentCellBehavior = savedCaa;
                                if ((savedCaa & GridCellActivateAction.SetCurrent) != 0
                                    && HasCurrentCell)
                                {
                                    BeginEdit();
                                }
                            }

                            return true;
                        }
                    }
                }
                else if (direction == GridDirectionType.Up)
                {
                    isWrapCell = false;
                    if (this.RowIndex == gridModel.Rows.HeaderCount + 1)
                    {
                        isWrapCell = true;
                        if (this.colIndex == gridModel.Cols.HeaderCount + 1)
                        {
                            this.MoveTo(this.grid.Model.RowCount, this.grid.Model.ColCount);
                            this.grid.ScrollCellInView(this.grid.Model.RowCount, this.grid.Model.ColCount, GridScrollCurrentCellReason.MoveTo);
                        }
                        else
                        {
                            this.MoveTo(this.grid.Model.RowCount, this.colIndex - 1);
                            this.grid.ScrollCellInView(this.grid.Model.RowCount, this.colIndex - 1, GridScrollCurrentCellReason.MoveTo);
                        }
                    }
                }
                else if (direction == GridDirectionType.Down)
                {
                    isWrapCell = false;
                    if (this.RowIndex == this.grid.Model.RowCount)
                    {
                        isWrapCell = true;
                        if (this.colIndex == this.grid.Model.ColCount)
                        {
                            this.MoveTo(topRow + this.gridModel.Rows.HeaderCount, firstCol + this.gridModel.Cols.HeaderCount);
                            this.grid.ScrollCellInView(topRow + this.gridModel.Rows.HeaderCount, firstCol + this.gridModel.Cols.HeaderCount, GridScrollCurrentCellReason.MoveTo);
                            }
                            else
                            {
                                this.MoveTo(topRow + this.gridModel.Rows.HeaderCount, this.colIndex + 1);
                                this.grid.ScrollCellInView(topRow + this.gridModel.Rows.HeaderCount, this.colIndex + 1, GridScrollCurrentCellReason.MoveTo);
                            }
                    }
                }
                if (direction == GridDirectionType.PageUp)
                {
                    isWrapCell = false;
                    if (this.RowIndex == gridModel.Rows.HeaderCount + 1)
                    {
                        isWrapCell = true;
                        if (ci == gridModel.Cols.HeaderCount + 1)
                        {
                            this.MoveTo(this.grid.Model.RowCount , this.grid.Model.ColCount);
                            this.grid.ScrollCellInView(this.grid.Model.RowCount, this.grid.Model.ColCount, GridScrollCurrentCellReason.MoveTo);
                        }
                        else
                        {
                            this.MoveTo(this.grid.Model.RowCount, ci - 1);
                            this.grid.ScrollCellInView(this.grid.Model.RowCount, ci - 1, GridScrollCurrentCellReason.MoveTo);
                        }
                    }
                }
                if (direction == GridDirectionType.PageDown)
                {
                    isWrapCell = false;
                    if (this.RowIndex == this.grid.Model.RowCount && this.RowIndex > this.gridModel.Rows.HeaderCount)
                    {
                        isWrapCell = true;
                        if (this.colIndex == this.grid.Model.ColCount)
                        {
                            this.MoveTo(topRow + gridModel.Rows.HeaderCount, firstCol + gridModel.Rows.HeaderCount);
                            this.grid.ScrollCellInView(topRow + gridModel.Rows.HeaderCount, firstCol + gridModel.Rows.HeaderCount, GridScrollCurrentCellReason.MoveTo);
                        }
                        else
                        {
                            this.MoveTo(topRow + gridModel.Rows.HeaderCount, this.colIndex + 1);
                            this.grid.ScrollCellInView(topRow + gridModel.Rows.HeaderCount, this.colIndex + 1, GridScrollCurrentCellReason.MoveTo);
                        }
                    }
                }
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, direction, num, extendSelection);
            }
#else

            ;
#endif
            int savedRowIndex = rowIndex;
            int savedColIndex = colIndex;

            bool beginUpdate = false;
            if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                || (grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended
                && grid.Model.Options.MulitExtendedArrowKeySelect))
            {
                beginUpdate = true;
                grid.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
            }

            try
            {
                if (ExternalMove != null)
                {
                    ExternalMove(direction, num, extendSelection);
                }
                else
                {
                    InternalMove(direction, num, noScrollInView ? GridSetCurrentCellOptions.None : GridSetCurrentCellOptions.ScrollInView);
                }
            }
            finally
            {
                if (beginUpdate)
                {
                    grid.EndUpdate(defaultUpdateFlag);
                }
            }

            return rowIndex != savedRowIndex || colIndex != savedColIndex;
        }

        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// optionally selecting the cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <returns>True if the current cell was moved to a new position; False otherwise (e.g. if current cell is at first row
        /// and you tried to move up).</returns>
        public bool Move(GridDirectionType direction, int num, bool extendSelection)
        {
            return Move(direction, num, extendSelection, gridModel.Options.WrapCell);
        }

        /// <summary>
        /// Moves the current cell in a given direction skipping a specified number of cells and
        /// without selecting the cells. <see cref="GridCurrentCell.Move(Syncfusion.Windows.Forms.Grid.GridDirectionType,int,bool,bool)"/> for a method that
        /// also selects cells.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="num">The number of cells to move.</param>
        /// <param name="options">The <see cref="GridSetCurrentCellOptions"/> that specifies the options for current cell movement.</param>
        /// <returns>True if this operation successfully completes.</returns>
        public bool InternalMove(GridDirectionType direction, int num, GridSetCurrentCellOptions options)
        {
            if (IsLocked)
            {
                return false;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, direction, num, options);
            }
#else

            ;
#endif

            if (grid.ScrollGrid.m_nLeftCol < 0 )
            {
                return false;
            }

            if (grid.ScrollGrid.m_nTopRow < 0)
            {
                return false;
            }

            if (!(gridModel.ColCount == 0 || gridModel.ColCount >= grid.ScrollGrid.m_nLeftCol - 1))
            {
                return false;
            }

            if (!(gridModel.RowCount == 0 || gridModel.RowCount >= grid.ScrollGrid.m_nTopRow - 1))
            {
                return false;
            }

            bool querySuccess = false;
            bool savedDisableScrollWindow = grid.DisableScrollWindow;
            bool hasCurrentCell = HasCurrentCell;
            bool allowSelectRange = (options & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.None;
            bool scrollCellInView = (options & GridSetCurrentCellOptions.ScrollInView) != GridSetCurrentCellOptions.None;
            bool scrollFrozen = gridModel.Options.ScrollFrozen;
            int currentRow = GridUtil.MinMax(this.rowIndex, 0, gridModel.RowCount);
            int currentCol = GridUtil.MinMax(this.colIndex, 0, gridModel.ColCount);
            int frozenRowCount = grid.InternalGetFrozenRows();
            int frozenColCount = grid.InternalGetFrozenCols();
            ////int firstScrollableCol = grid.GetFirstScrollableCol();
            int headerRowCount = grid.InternalGetHeaderRows();
            int headerColCount = grid.InternalGetHeaderCols();
            int targetRowIndex = currentRow;
            int targetColIndex = currentCol;
            int clientRow = grid.GetClientRow(currentRow);
            int count = 0;
            int vscrollup = 0;
            int vscrollleft = 0;
            bool vert = false;
            bool horz = false;

            switch (direction)
            {
                case GridDirectionType.Up:
                case GridDirectionType.Down:
                case GridDirectionType.PageDown:
                case GridDirectionType.PageUp:
                case GridDirectionType.Top:
                case GridDirectionType.Bottom:
                    vert = true;
                    break;

                case GridDirectionType.Left:
                case GridDirectionType.Right:
                case GridDirectionType.MostLeft:
                case GridDirectionType.MostRight:
                    horz = true;
                    break;

                case GridDirectionType.TopLeft:
                case GridDirectionType.BottomRight:
                    break;
            }

            num = Math.Max(num, 1);
            if (horz && keyMoveRowIndex != -1)
            {
                currentRow = GridUtil.MinMax(keyMoveRowIndex, 0, gridModel.RowCount);
            }

            if (vert && keyMoveColIndex != -1)
            {
                currentCol = GridUtil.MinMax(keyMoveColIndex, 0, gridModel.ColCount);
            }

            int newRowIndex = currentRow;
            int newColIndex = currentCol;

            //// Range spanned by covered cell
            GridRangeInfo coveredRange;
            int nRows = 0, nCols = 0;
            if (gridModel.CoveredRanges.Find(currentRow, currentCol, out coveredRange))
            {
                nRows = coveredRange.Bottom - coveredRange.Top;
                nCols = coveredRange.Right - coveredRange.Left;
            }

            switch (direction)
            {
                case GridDirectionType.Up:
                    vert = true;
                    if (!isWrapCell)
                    {
                        while (num > 0)
                        {
                            if (scrollFrozen && (newRowIndex == grid.TopRowIndex || vscrollup > 0))
                            {
                                //// If current cell is at the topmost nonfrozen row, scroll the view.
                                vscrollup++;
                                if (!grid.ScrollGrid.GetPrevRowIndex(ref newRowIndex))
                                {
                                    break;
                                }
                            }
                            else if (newRowIndex <= headerRowCount + 1 || vscrollup > 0)
                            {
                                //// If there are frozen rows, move up one visible cell;
                                //// if current cell is at the top row, scroll the view.
                                vscrollup++;
                                if (currentRow > frozenRowCount)
                                {
                                    newRowIndex = grid.GetRow(grid.GetClientRow(frozenRowCount) + 1) - vscrollup;
                                }
                            }
                            else if (frozenRowCount > headerRowCount && newRowIndex >= grid.TopRowIndex && newRowIndex <= grid.GetRow(grid.GetClientRow(frozenRowCount) + 1))
                            {
                                newRowIndex = grid.GetRow(grid.GetClientRow(newRowIndex) - 1);
                            }
                            else
                            {
                                newRowIndex--;
                            }

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColIndex))
                            {
                                break;
                            }
                            // Scrolling necessary when new row is not visible
                            // because it is between the frozen row and the
                            // top row.
                            if (!(newRowIndex <= frozenRowCount || newRowIndex >= grid.TopRowIndex))
                            {
                                vscrollup += grid.TopRowIndex - newRowIndex;
                            }

                            num--;
                            querySuccess = true;
                            targetRowIndex = Math.Max(newRowIndex, headerRowCount);
                            targetColIndex = newColIndex;
                        }
                    }

                    break;

                case GridDirectionType.Down:
                    vert = true;
                    gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                    count = gridModel.RowCount;
                    if (!isWrapCell)
                    {
                        while (num > 0)
                        {
                            //// If there are frozen rows, move down one visible cell.
                            if (newRowIndex <= frozenRowCount)
                            {
                                newRowIndex += nRows;
                                grid.ScrollGrid.GetNextRowIndex(ref newRowIndex, false);
                                if (newRowIndex > frozenRowCount)
                                {
                                    newRowIndex = grid.TopRowIndex;
                                }
                            }
                            else
                            {
                                newRowIndex += nRows + 1;
                            }

                            if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex))
                            {
                                break;
                            }

                            num--;
                            querySuccess = true;
                            targetRowIndex = Math.Min(newRowIndex, count);
                            targetColIndex = newColIndex;
                        }
                    }

                    break;

                case GridDirectionType.Left:
                    while (num > 0)
                    {
                        if (scrollFrozen && (newColIndex == grid.LeftColIndex || vscrollleft > 0))
                        {
                            //// If current cell is at leftmost nonfrozen column, scroll the view.
                            vscrollleft++;
                            if (!grid.ScrollGrid.GetPrevColIndex(ref newColIndex))
                            {
                                break;
                            }
                        }
                        else if (newColIndex <= headerColCount + 1 || vscrollleft > 0)
                        {
                            // If current cell is at leftmost header column, scroll the view.
                            vscrollleft++;
                            if (newColIndex > frozenColCount)
                            {
                                newColIndex = grid.GetCol(grid.GetClientCol(frozenColCount) + 1) - vscrollleft;
                            }
                        }
                        else if (frozenColCount > headerColCount && newColIndex >= grid.LeftColIndex && newColIndex <= grid.GetCol(grid.GetClientCol(frozenColCount) + 1))
                        {
                            // If there are frozen Cols, move up one visible cell.
                            newColIndex = grid.GetCol(grid.GetClientCol(newColIndex) - 1);
                        }
                        else
                        {
                            newColIndex--;
                        }

                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColIndex))
                        {
                            break;
                        }

                        // Scrolling becomes necessary when new column is not visible
                        // because it is between the frozen columns and the
                        // left column.
                        if (!(newColIndex <= frozenColCount || newColIndex >= grid.LeftColIndex))
                        {
                            vscrollleft += grid.LeftColIndex - newColIndex;
                        }

                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColIndex = Math.Max(newColIndex, headerColCount);
                    }

                    break;

                case GridDirectionType.Right:
                    gridModel.RaiseQueryMaximumRowCol(0, gridModel.ColCount);
                    count = gridModel.ColCount;
                    while (num > 0)
                    {
                        // If there are frozen rows, move one visible cell to the right.
                        if (newColIndex <= frozenColCount)
                        {
                            newColIndex += nCols;
                            grid.ScrollGrid.GetNextColIndex(ref newColIndex, false);
                            if (newColIndex > frozenColCount)
                            {
                                newColIndex = grid.LeftColIndex;
                            }
                        }
                        else
                        {
                            newColIndex += nCols + 1;
                        }

                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColIndex))
                        {
                            break;
                        }

                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColIndex = Math.Min(newColIndex, count);
                    }

                    break;

                case GridDirectionType.PageDown:
                    vert = true;
                    gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                    count = gridModel.RowCount;
                    if (!isWrapCell)
                    {
                        if (grid.ScrollGrid.m_nTopRow < count)
                        {
                            // Get target row.
                            int y = grid.GetRowHeight(newRowIndex);
                            while (y < Grid.ViewLayout.VscrollAreaBounds.Height)
                            {
                                if (!grid.ScrollGrid.GetNextRowIndex(ref newRowIndex, true))
                                {
                                    break;
                                }

                                y += grid.GetRowHeight(newRowIndex);
                            }

                            //// Find the nearest possible cell (first down. then up).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex);
                            if (!querySuccess)
                            {
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColIndex);
                            }

                            if (querySuccess)
                            {
                                targetRowIndex = newRowIndex;
                                targetColIndex = newColIndex;
                            }
                        }
                    }

                    break;

                case GridDirectionType.PageUp:
                    vert = true;
                    if (!isWrapCell)
                    {
                        if (grid.ScrollGrid.m_nTopRow > frozenRowCount + 1)
                        {
                            int firstScrollableRow = grid.GetFirstScrollableRow();
                            Rectangle rect = grid.GridBounds;

                        // Get target row.
                        int y = grid.GetRowHeight(newRowIndex);
                        while (y < Grid.ViewLayout.VscrollAreaBounds.Height)
                            {
                                if (!grid.ScrollGrid.GetPrevRowIndex(ref newRowIndex))
                                {
                                    break;
                                }

                                y += grid.GetRowHeight(newRowIndex);
                            }

                            if (newRowIndex < firstScrollableRow)
                            {
                                newRowIndex = grid.InternalGetHeaderRows();
                                grid.ScrollGrid.GetNextRowIndex(ref newRowIndex, false);
                                vscrollup = 1;
                            }

                            //// Find the nearest possible cell (first up. then down).
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColIndex);

                            if (!querySuccess)
                            {
                                gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                                querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex);
                            }

                            if (querySuccess)
                            {
                                targetRowIndex = newRowIndex;
                                targetColIndex = newColIndex;
                            }
                        }
                        else
                        {
                            gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                            newRowIndex = Math.Min(grid.InternalGetHeaderRows() + 1 + frozenRowCount, gridModel.RowCount);
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex);
                            if (querySuccess)
                            {
                                targetRowIndex = newRowIndex;
                                targetColIndex = newColIndex;
                            }
                            break;
                        }
                    }
                    break;

                case GridDirectionType.MostLeft:
                    gridModel.RaiseQueryMaximumRowCol(0, gridModel.ColCount);
                    vscrollleft = 1;
                    newColIndex = Math.Min(grid.InternalGetHeaderCols() + 1, gridModel.ColCount);
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColIndex);
                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                case GridDirectionType.MostRight:
                    gridModel.RaiseQueryMaximumRowCol(0, GridConstants.MaxRowCol);
                    newColIndex = gridModel.ColCount;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColIndex);
                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                case GridDirectionType.TopLeft:
                    gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, gridModel.ColCount);
                    newColIndex = Math.Min(grid.InternalGetHeaderCols() + 1, gridModel.ColCount);
                    newRowIndex = Math.Min(grid.InternalGetHeaderRows() + 1, gridModel.RowCount);
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColIndex);
                    if (!querySuccess)
                    {
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex);
                    }

                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                case GridDirectionType.BottomRight:
                    gridModel.RaiseQueryMaximumRowCol(GridConstants.MaxRowCol, GridConstants.MaxRowCol);
                    newColIndex = gridModel.ColCount;
                    newRowIndex = gridModel.RowCount;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColIndex);
                    if (!querySuccess)
                    {
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColIndex);
                    }

                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                case GridDirectionType.Top:
                    vert = true;
                    vscrollup = 1;
                    gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                    newRowIndex = Math.Min(grid.InternalGetHeaderRows() + 1, gridModel.RowCount);
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColIndex);
                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                case GridDirectionType.Bottom:
                    vert = true;
                    gridModel.RaiseQueryMaximumRowCol(GridConstants.MaxRowCol, 0);
                    newRowIndex = gridModel.RowCount;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColIndex);
                    if (querySuccess)
                    {
                        targetRowIndex = newRowIndex;
                        targetColIndex = newColIndex;
                    }

                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            grid.DisableScrollWindow = Math.Abs(targetRowIndex - currentRow) > grid.ViewLayout.VisibleRows / 2
                || Math.Abs(targetColIndex - currentCol) > grid.ViewLayout.VisibleCols / 2;

            bool success = false;

            //// position edit cell
            if (querySuccess &&
                ((targetRowIndex != currentRow && targetRowIndex > headerRowCount)
                || (targetColIndex != currentCol && targetColIndex > headerColCount)
                || !hasCurrentCell))
            {
                int n1 = grid.ViewLayout.VisibleCols / 2;
                int n2 = grid.ViewLayout.VisibleRows / 2;

                try
                {
                    // EXCELCURCELL
                    if (gridModel.Options.ExcelLikeCurrentCell
                        && allowSelectRange)
                    {
                        if (gridModel.Selections.Ranges.Count > 0
                            && !gridModel.Selections.Ranges.AnyRangeContains(GridRangeInfo.Cell(targetRowIndex, targetColIndex)))
                        {
                            grid.Selections.Clear(true);
                        }
                    }

                    bool savedActive = this.HasCurrentCell;

                    int _targetRowIndex = targetRowIndex;
                    int _targetColIndex = targetColIndex;
                    this.AdjustRowColIfCoveredCell(ref _targetRowIndex, ref _targetColIndex);

                    if (this.MoveTo(_targetRowIndex, _targetColIndex, options))
                    {
                        if (vscrollleft > 0 && _targetColIndex < grid.LeftColIndex)
                        {
                            grid.LeftColIndex = Math.Max(_targetColIndex, grid.GetFirstScrollableCol());
                        }

                        if (vscrollup > 0 && _targetRowIndex < grid.TopRowIndex)
                        {
                            grid.TopRowIndex = Math.Max(_targetRowIndex, grid.GetFirstScrollableRow());
                        }

                        // Need to be aware here of two special cases:
                        // 1) User could have overriden CurrentCellMoving
                        // and redirected the targetRowIndex and targetColIndex.
                        // 2) End-User could be stepping through a covered range.
                        // Then targetRowIndex and targetColIndex will point to
                        // upper-left corner but next time you hit arrow-key the
                        // current cell should stay in same column or row as it
                        // was before.
                        if (!savedActive || grid.Model.CoveredRanges.FindRange(rowIndex, colIndex).IsEmpty)
                        {
                            keyMoveRowIndex = this.rowIndex;
                            keyMoveColIndex = this.colIndex;
                        }
                        else if (vert)
                        {
                            //// next two handle CoveredCells
                            keyMoveRowIndex = this.rowIndex;
                            keyMoveColIndex = targetColIndex;
                        }
                        else
                        {
                            keyMoveRowIndex = targetRowIndex;
                            keyMoveColIndex = this.colIndex;
                        }

                        success = true;
                    }
                }
                finally
                {
                }
            }

            grid.DisableScrollWindow = savedDisableScrollWindow;
            return success;
        }

        /// <summary>
        /// Adjusts the row index and column index if the cell belongs to a covered range. In that case, the top
        /// left cell coordinates are returned.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public void AdjustRowColIfCoveredCell(ref int rowIndex, ref int colIndex)
        {
            if (rowIndex < 0 || colIndex < 0)
            {
                return;
            }

            GridRangeInfo range;
            if (gridModel.CoveredRanges.Find(rowIndex, colIndex, out range))
            {
                if (range.Top != rowIndex || range.Left != colIndex)
                {
                    rowIndex = range.Top;
                    colIndex = range.Left;
                }
            }
        }

        private ScrollBars GetMovement(int targetRowIndex, int targetColIndex)
        {
            ScrollBars direction = ScrollBars.None;
            if (targetRowIndex < 0 || targetColIndex < 0)
            {
                direction = HasCurrentCell ? ScrollBars.None : ScrollBars.Both;
            }
            else if (targetColIndex <= grid.InternalGetHeaderCols() || targetRowIndex <= grid.InternalGetHeaderRows())
            {
                direction = ScrollBars.None;
            }
            else
            {
                if (!HasCurrentCell)
                {
                    direction = ScrollBars.Both;
                }
                else
                {
                    if (rowIndex != targetRowIndex)
                    {
                        direction |= ScrollBars.Vertical;
                    }

                    if (colIndex != targetColIndex)
                    {
                        direction |= ScrollBars.Horizontal;
                    }
                }
            }

            return direction;
        }

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether RaiseException IfNested Active Or Deactivate. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool RaiseExceptionIfNestedActiveOrDeactivate
        {
            get
            {
                return raiseExceptionIfNestedActiveOrDeactivate;
            }

            set
            {
                raiseExceptionIfNestedActiveOrDeactivate = value;
            }
        }

        /// <summary>
        /// Deactivates the current cell and confirms or rejects changes made to the current cell.
        /// </summary>
        /// <param name="discardChanges">True if changes can be discarded; False otherwise.</param>
        /// <returns>True if current cell can be deactivated; False otherwise.</returns>
        /// <remarks>
        /// <see cref="Deactivate"/> raises a cancelable <see cref="GridControlBase.CurrentCellDeactivating"/> event. If the
        /// event handler set the <see cref="CancelEventArgs.Cancel"/> flag of the <see cref="CancelEventArgs"/>
        /// object, the method will return.
        /// <para/>
        /// Next, it checks if the cell is modified and depending on the value of the discardChanges parameter, it tries to
        /// confirm changes by calling <see cref="ConfirmChanges()"/> or discarding changes by calling <see cref="RejectChanges"/>.
        /// Also, any open drop-down windows will be closed at this time.
        /// <para/>
        /// Above method calls will raise <see cref="GridControlBase.CurrentCellValidating"/> then
        /// <see cref="GridControlBase.CurrentCellValidated"/> and <see cref="GridControlBase.CurrentCellAcceptedChanges"/>
        /// or <see cref="GridControlBase.CurrentCellRejectedChanges"/> events followed by a <see cref="GridControlBase.CurrentCellEditingComplete"/> event
        /// if the cell was in editing mode.
        /// <para/>
        /// After the changes have been saved or canceled, the cell area is invalidated and
        /// a <see cref="GridControlBase.CurrentCellDeactivated"/> event is raised which completes this operation.<para/>
        /// If an exception occurred or if a event handler requested to cancel the operation, <see cref="Deactivate"/> will instead
        /// close the operation with a <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event.
        /// <para/>
        /// This ensures that you will get guaranteed either a <see cref="GridControlBase.CurrentCellDeactivated"/>
        /// or <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event after a <see cref="GridControlBase.CurrentCellDeactivating"/> event.
        /// <para/>
        /// When current cell has been deactivated, the <see cref="HasCurrentCell"/> property will be false indicating the
        /// grid has no active current cell at this time.
        /// </remarks>
        public bool Deactivate(bool discardChanges)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, discardChanges);
            }
#else
            ;
#endif

            if (IsLocked)
            {
                return false;
            }

            if (this.inActivate || this.inDeactivate)
            {
                if (raiseExceptionIfNestedActiveOrDeactivate)
                {
                    throw new InvalidOperationException("Deactivate called while the current cell was in process of activating or deactivating a cell. Calling CurrentCell.Lock() will prevent this exception.");
                }

                Trace.WriteLine("Deactivate called while the current cell was in process of activating or deactivating a cell. Calling CurrentCell.Lock() will prevent this exception.");
                TraceUtil.TraceCurrentMethodInfo(discardChanges, Grid);
                TraceUtil.TraceCalledFrom(20);
                return false;
            }

            GridCellRendererBase savedCellRenderer = this.cellRenderer;
            if (savedCellRenderer == null)
            {
                return true;
            }
            this.ResetError();            
            lastSyncText = Model.GetActiveText(RowIndex, ColIndex);

            bool success = false;

            this.savedModified = isModified;
            inDeactivate = true;
            Trace.Indent();
            try
            {
                Rectangle savedBounds = Rectangle.Empty;
                if (gridModel.Options.RefreshCurrentCellBehavior == GridRefreshCurrentCellBehavior.RefreshRow && Grid.IsVisibleCell(rowIndex, 0))
                {
                    savedBounds = Grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex), GridRangeOptions.MergeAllSpannedCells);
                }
                else if (Grid.IsVisibleCell(rowIndex, colIndex))
                {
                    savedBounds = Grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                }

                if (!Grid.RaiseCurrentCellDeactivating())
                {
                    return false;
                }

                if (cellRenderer == null || !cellRenderer.RaiseDeactivating())
                {
                    return false;
                }

                if (IsModified)
                {
                    if (!discardChanges)
                    {
                        //// Fires NotifyValidate, should CloseDropDown() possibly fires ControlLostFocus event.
                        if (!this.ConfirmChanges())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //// Should CloseDropDown() possibly fires ControlLostFocus event.
                        this.RejectChanges();
                    }
                }

                success = true;

                if (IsEditing)
                {
                    CancelEdit();
                }

                if (cellRenderer != null)
                {
                    cellRenderer.SetHasFocusControl(false);
                }

                this.cellRenderer = null;

                Grid.InvalidateDeactivatedCurrentCell(rowIndex, colIndex, savedBounds);

                //// TODO: Make note in docs that inDeactivate was moved before RaiseDeactivaed.
                inDeactivate = false;

                if (savedCellRenderer != null)
                {
                    savedCellRenderer.RaiseDeactived(rowIndex, colIndex);

                    savedCellRenderer.Model.ResetActiveText(rowIndex, colIndex);
                }

                gridModel.ResetCurrentCellInfo();

                ////                GridRangeInfo r = GridRangeInfo.Cells(rowIndex, colIndex, rowIndex, colIndex+1);
                ////                Grid.Model.FloatingCells.DelayFloatCells(r);
                ////                Grid.Model.FloatingCells.EvaluateFloatingCells(r);

                //// At this time there is no current cell.
                Grid.RaiseCurrentCellDeactivated(rowIndex, colIndex);
                
                return true;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                success = false;
                this.Exception = ex;
                if (!ExceptionManager.RaiseExceptionCatched(grid, ex))
                {
                    throw;
                }

                ErrorMessage = ex.Message;

                return false;
            }
            finally
            {
                inDeactivate = false;
                savedModified = false;

                if (!success)
                {
                    inDeactivateFailed = true;
                    Grid.RaiseCurrentCellDeactivateFailed();
                    inDeactivateFailed = false;
                }

                Trace.Unindent();
            }
        }
        /// <summary>
        /// Get or set the error message in the current cell.
        /// </summary>
        public void SetError(string displayErrorMsg)
        {            
            IsError = true;
            this.ErrorMessage = displayErrorMsg;            
        }

        /// <summary>
        /// Finished up editing mode for the current cell.
        /// </summary>
        /// <remarks>
        /// Close any open drop-down windows. <para/>
        /// If the current cell was modified, the method tries to
        /// confirm changes by calling <see cref="ConfirmChanges()"/>. If the changes can not be confirmed
        /// an exception is shown with <see cref="ErrorMessage"/> as exception text.
        /// <para/>
        /// If changes are confirmed, a <see cref="GridControlBase.CurrentCellEditingComplete"/> event is raised.
        /// </remarks>
        public void EndEdit()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            if (IsLocked || cellRenderer == null)
            {
                return;
            }

            if (isEditing)
            {
                try
                {
                    inEndEdit = true;

                    if (IsDroppedDown)
                    {
                        CloseDropDown(PopupCloseType.Done);
                    }

                    if (IsModified)
                    {
                        if (!this.ConfirmChanges())
                        {
                            if (this.Exception != null)
                            {
                                throw this.Exception;
                            }
                            else if (!GridUtil.IsEmpty(ErrorMessage))
                            {
                                throw new Exception(ErrorMessage);
                            }
                        }
                    }

                    //// Return immediately if saving cell contents failed 
                    //// or if user explicitly called CancelEdit from Validate event handler.
                    if (IsModified || !isEditing)
                    {
                        return;
                    }

                    if (cellRenderer != null)
                    {
                        cellRenderer.RaiseEndEdit();
                        isEditing = false;
                    }

                    if (cellRenderer != null)
                    {
                        cellRenderer.RaiseEditingComplete();
                    }

                    isEditing = false;
                    Grid.RaiseCurrentCellEditingComplete();

                    //// TODO: Grid.FixCurrent
                    notifyChangingCalled = false;
                }
                finally
                {
                    inEndEdit = false;
                }
            }
        }

        /// <overload>
        /// Confirms any pending changes for the current cell.
        /// </overload>
        /// <summary>
        /// Confirms any pending changes for the current cell and closes any open drop-down windows.
        /// </summary>
        /// <returns>True if changes could be saved or if current cell was not modified; False if saving the changes
        /// failed.</returns>
        /// <remarks>
        /// If the current cell was modified, the method raises a <see cref="GridControlBase.CurrentCellValidating"/> event
        /// and if the contents are valid the cell renderers <see cref="GridCellRendererBase.OnSaveChanges"/> are called to
        /// save the changes.<para/>
        /// After changes have been successfully confirmed to the grid, a <see cref="GridControlBase.CurrentCellAcceptedChanges"/>
        /// event is raised.
        /// </remarks>
        public bool ConfirmChanges()
        {
            return ConfirmChanges(true);
        }

        /// <summary>
        /// Confirms any pending changes for the current cell.
        /// </summary>
        /// <param name="closeDropDown">Specifies where any open drop-down windows should be closed.</param>
        /// <returns>True if changes could be saved or if current cell was not modified; False if saving the changes
        /// failed.</returns>
        /// <genoverload/>
        public bool ConfirmChanges(bool closeDropDown)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (IsLocked)
            {
                return false;
            }

            Trace.Indent();
            bool endUpdate = false;
            inConfirmChanges = true;
            try
            {
                if (cellRenderer != null)
                {
                    if (closeDropDown && IsDroppedDown)
                    {
                        CloseDropDown(PopupCloseType.Done);
                    }

                    if (IsModified)
                    {
                        endUpdate = true;
                        Grid.BeginUpdateModel(BeginUpdateOptions.InvalidateAndScroll, true);

                        Validate();
                        if (!IsValid)
                        {
                            Grid.RaiseCurrentCellConfirmChangesFailed();
                            return false;
                        }

                        if (cellRenderer == null || !cellRenderer.RaiseSaveChanges())
                        {
                            return false;
                        }

                        IsModified = false;
                        inAcceptedChanges = true;
                        if (!Grid.RaiseCurrentCellAcceptedChanges())
                        {
                            inAcceptedChanges = false;
                            IsModified = true;
                            Grid.RaiseCurrentCellConfirmChangesFailed();
                            return false;
                        }

                        inAcceptedChanges = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);

                Grid.RaiseCurrentCellConfirmChangesFailed();
                this.Exception = ex;

                if (!ExceptionManager.RaiseExceptionCatched(grid, ex))
                {
                    throw;
                }

                ErrorMessage = ex.Message;

                return false;
            }
            finally
            {
                inConfirmChanges = false;
                if (endUpdate)
                {
                    Grid.EndUpdateModel(false, true);
                }

                Trace.Unindent();
            }

            return true;
        }

        /// <summary>
        /// Cancels editing for the current cell and discards any changes.
        /// </summary>
        /// <remarks>
        /// Close any open drop-down windows. <para/>
        /// If the current cell was modified, the method calls <see cref="RejectChanges"/> which triggers a <see cref="GridControlBase.CurrentCellRejectedChanges"/> event.
        /// <para/>
        /// After the cell renderer has been asked to stop editing, a <see cref="GridControlBase.CurrentCellEditingComplete"/> event is raised.
        /// </remarks>
        public void CancelEdit()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            Trace.Indent();
            try
            {
                if (isEditing)
                {
                    if (IsDroppedDown)
                    {
                        CloseDropDown(PopupCloseType.Canceled);
                    }

                    if (isModified)
                    {
                        this.RejectChanges();
                    }

                    if (cellRenderer != null)
                    {
                        cellRenderer.RaiseEndEdit();
                    }

                    isEditing = false;
                    Grid.RaiseCurrentCellEditingComplete();
                    if (cellRenderer != null)
                    {
                        cellRenderer.RaiseEditingComplete();
                    }
                }

                if (cellRenderer != null)
                {
                    cellRenderer.ResetControlText();

                    if (!cellRenderer.HasFocusControl && cellRenderer.Control != null && cellRenderer.Control.Focused)
                    {
                        if (!Grid.IsInLeaveOrValidate || Grid.validatingFailed)
                        {
                            Grid.Focus();
                        }
                        else
                        {
                            Trace.WriteLineIf(Switches.GridFocus.TraceVerbose, "--- No Grid.Focus() because: " + Grid.IsInLeaveOrValidate.ToString());
                        }
                    }
                }

                notifyChangingCalled = false;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Discards any changes for the current cell.
        /// </summary>
        /// <remarks>
        /// Close any open drop-down windows. <para/>
        /// Calls the cell renderer's <see cref="GridCellRendererBase.OnRejectChanges"/> method and resets
        /// the <see cref="IsModified"/> flag. <para/>
        /// <para/>
        /// After the cell renderer has been asked to discard changes, a <see cref="GridControlBase.CurrentCellRejectedChanges"/>
        /// event is raised.
        /// </remarks>
        public void RejectChanges()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            if (IsDroppedDown)
            {
                CloseDropDown(PopupCloseType.Canceled);
            }

            if (cellRenderer != null)
            {
                cellRenderer.RaiseRejectChanges();
            }

            IsModified = false;
            Grid.RaiseCurrentCellRejectedChanges();
        }
       
        /// <summary>
        /// Gets or sets the custom error message to replace the default error message.
        /// </summary>
        public string ValidationErrorText
        {
            get
            {
                return validationErrorText;
            }

            set
            {
                validationErrorText = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom SetError message for validation of current cell.
        /// </summary>
        /// 
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsError
        {
            get
            {
                return isError;
            }

            set
            {
                isError = value;
            }
        }

        /// <summary>
        /// Gets or sets the error message with reason why deactivation or validation of current cell failed.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        /// <summary>
        /// Shows or hides the Message box with the error message
        /// </summary>
        public bool ShowErrorMessageBox
        {
            get { return showErrorMessageBox; }
            set { showErrorMessageBox = value; }
        }
    
        /// <summary>
        /// Shows or hides the error Icon in the current cell if the validation is failed.
        /// </summary>
        public bool ShowErrorIcon
        {
            get
            {
                return showErrorIcon;
            }

            set
            {
                showErrorIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the exception that causes failure of deactivation or validation of current cell.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return exception;
            }

            set
            {
                exception = value;
            }
        }

        /// <overload>
        /// Activates the current cell at the specified position.
        /// </overload>
        /// <summary>
        /// Activates the current cell at the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if activating the current cell was successful; False otherwise.</returns>
        /// <remarks>
        /// <see cref="Activate(int,int)"/> raises a cancelable <see cref="GridControlBase.CurrentCellActivating"/> event. If the
        /// event handler set the <see cref="CancelEventArgs.Cancel"/> flag of the <see cref="CancelEventArgs"/>
        /// object, the method will return. <see cref="GridControlBase.CurrentCellActivating"/> gives you also the chance
        /// to modify the parameters of the <see cref="Activate(int,int)"/> function call. You can modify the row and column index and the
        /// options.<para/>
        /// <para/>
        /// Next, it calls the cell renderer's <see cref="GridCellRendererBase.OnActivating"/> before it stores internally
        /// the new row and column index and marks the cell as activated.<para/>
        /// Then it invalidates the screen area for the specified cell and calls the cell renderer's <see cref="GridCellRendererBase.OnInitialize"/>
        /// method.<para/>
        /// At last, a <see cref="GridControlBase.CurrentCellActivated"/> event is raised which completes this operation.<para/>
        /// If an exception occurred or if a event handler requested to cancel the operation, <see cref="Activate(int,int)"/> will instead
        /// close up the operation with a  <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
        /// <para/>
        /// This ensures that you will get guaranteed either a <see cref="GridControlBase.CurrentCellActivated"/>
        /// or <see cref="GridControlBase.CurrentCellActivateFailed"/> event after a <see cref="GridControlBase.CurrentCellActivating"/> event.
        /// </remarks>
        public bool Activate(int rowIndex, int colIndex)
        {
            return Activate(rowIndex, colIndex, GridSetCurrentCellOptions.None);
        }

        /// <summary>
        /// Activates the current cell at the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if activating the current cell was successful; False otherwise.</returns>
        /// <genoverload/>
        public bool Activate(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, rowIndex, colIndex, options);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return false;
            }

            if (this.inActivate || this.inDeactivate)
            {
                if (raiseExceptionIfNestedActiveOrDeactivate)
                {
                    throw new InvalidOperationException("Deactivate called while the current cell was in process of activating or deactivating a cell. Calling CurrentCell.Lock() will prevent this exception.");
                }

                Trace.WriteLine("Deactivate called while the current cell was in process of activating or deactivating a cell. Calling CurrentCell.Lock() will prevent this exception.");
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, options, Grid);
                TraceUtil.TraceCalledFrom(20);
                return false;
            }

            if (HasCurrentCell)
            {
                return false; //// better throw InvalidException?
            }

            if (rowIndex < 0 || colIndex < 0
                || rowIndex > gridModel.RowCount || colIndex > gridModel.ColCount)
            {
                return false;
            }

            bool success = false;

            inActivate = true;
            Trace.Indent();
            try
            {
                if (!Grid.RaiseCurrentCellActivating(ref rowIndex, ref colIndex, ref options))
                {
                    return false;
                }

                if (rowIndex < 0 || colIndex < 0
                    || rowIndex > gridModel.RowCount || colIndex > gridModel.ColCount)
                {
                    return false;
                }

                GridCellRendererBase targetRenderer = grid.GetCellRenderer(rowIndex, colIndex);

                string savedT = null;

                if (Grid.Model.Options.ShouldSynchronizeCurrentCell
                    && this.RowIndex == rowIndex && this.ColIndex == colIndex)
                {
                    savedT = lastSyncText;
                }

                if (!targetRenderer.RaiseActivating(rowIndex, colIndex))
                {
                    return false;
                }

                this.rowIndex = rowIndex;
                this.colIndex = colIndex;
                if (!this.inMoveTo)
                {
                    this.keyMoveRowIndex = rowIndex;
                    this.keyMoveColIndex = colIndex;
                }

                this.cellRenderer = targetRenderer;

                bool scrolled = false;
                if ((options & GridSetCurrentCellOptions.ScrollInView) != GridSetCurrentCellOptions.None)
                {
                    scrolled = Grid.ScrollCellInViewInt(rowIndex, colIndex, false, true, GridScrollCurrentCellReason.Activate);
                }

                if (scrolled)
                {
                    Grid.Update();
                    Grid.ScrollCellInViewInt(rowIndex, colIndex, false, false, GridScrollCurrentCellReason.Activate);
                }

                success = true;

                gridModel.CurrentCellInfo = new GridCurrentCellInfo(grid, cellRenderer, rowIndex, colIndex);

                GridRangeInfo rowColRange = GridRangeInfo.Cell(rowIndex, colIndex);

                /*
                    bool bInvert = gridModel.Selections.RangeList.AnyRangeContains(rowColRange);

                    //getRenderer.Refresh();
                    // Check, if refreshing the cell is the preferred way.
                    if (cellRenderer.ForceRefreshOnActivateCell
                        || (options & GridSetCurrentCellOptions.ForceRefresh) != GridSetCurrentCellOptions.None
                        //|| ForceRefreshOnActivateCell
                        || cellRenderer.IsActive()
                        || grid.Model.Options.CurrentCellActivateBehavior == GridCellActivateAction.SetCurrent
                        )
                    {
                        targetRenderer.Refresh();
                    }
                        // If it is not necessary, we can use a smarter
                        // way to outline the cell without refreshing it.
                    else
                    {
                        if (!grid.Updating)
                        {
                            bool scrolled = false;
                            if ((options & GridSetCurrentCellOptions.ScrollInView) != GridSetCurrentCellOptions.None)
                                scrolled = ScrollInView();

                            Rectangle rect = grid.RangeInfoToRectangle(rowColRange, GridRangeOptions.MergeCoveredCells|GridRangeOptions.CalculateNonClientArea);
                            if (!scrolled)
                            {
                                grid.Update();
                                Graphics g = grid.CreateGraphics();
                                g.IntersectClip(grid.GridBounds);

                                if (bInvert && grid.Model.Options.ShowCurrentCellBorderBehavior != GridShowCurrentCellBorder.HideAlways)
                                {
                                    grid.m_bInvertRect = true;
                                    grid.DrawInvertCell(g, rowIndex, colIndex, rect);
                                }

                                if (!rect.IsEmpty)
                                    targetRenderer.OnOutlineCurrentCell(g, grid.Model.SubtractBorders(rect, grid.Model[rowIndex, colIndex]));
                                g.Dispose();
                            }
                            else
                                grid.Invalidate(rect);
                        }
                        else
                        {
                            Trace.WriteLine("InternalEnter");
                        }
                    }
                    */

                ActivateOnGotFocus = false;

                Grid.BeginUpdateModel(BeginUpdateOptions.InvalidateAndScroll, true);

                //// TODO: Provide hook here that allows specifying what should be refreshed,
                //// e.g. None, Cell, RowHeader, ColHeader, Row, Col
                bool invalidated = false;
               if (Grid.Model != null && Grid.Model.ActiveGridView != null && (Grid.Model.Properties.MarkRowHeader || Grid.Model.Properties.MarkColHeader))              
                {
                    Grid.Model.ActiveGridView.BeginUpdate();
                    Grid.Model.ActiveGridView.UpdateStyles();
                    Grid.Model.ActiveGridView.EndUpdate(true);
                }
                if (cellRenderer != null && cellRenderer.ShouldRefreshCurrentCell())
                {
                    if (gridModel.Options.RefreshCurrentCellBehavior == GridRefreshCurrentCellBehavior.RefreshRow)
                    {
                        if (!this.IsInMoveTo || this.MoveFromRowIndex != rowIndex)
                        {
                            if (Grid.Visible && Grid.IsVisibleCell(rowIndex, 0))
                            {
                                invalidated = true;
                                Grid.InvalidateRange(GridRangeInfo.Row(rowIndex), GridRangeOptions.MergeAllSpannedCells);
                            }
                        }
                    }

                    if (!invalidated && Grid.Visible && Grid.IsVisibleCell(rowIndex, colIndex))
                    {
                        Grid.InvalidateRange(rowColRange, GridRangeOptions.MergeAllSpannedCells);
                    }
                }

                if (HasCurrentCell)
                {
                    cellRenderer.Initialize(rowIndex, colIndex);
                }

                Initialize(options);

                if (!GridUtil.IsEmpty(savedT))
                {
                    if (cellRenderer != null)
                    {
                        cellRenderer.ControlText = savedT;
                    }

                    lastSyncText = null;
                }

                if ((options & GridSetCurrentCellOptions.NoSetFocus) == 0
                    && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
                {
                    BeginEdit();
                }

                //// TODO: Make note in docs that inActivate  was moved before RaiseActivated.
                inActivate = false;

                inActivated = true;
                if (cellRenderer != null)
                {
                    this.cellRenderer.RaiseActivated();
                }

                Grid.RaiseCurrentCellActivated();

                inActivated = false;
                Grid.EndUpdateModel(defaultUpdateFlag, true);
                
                return true;
            }
            finally
            {
                inActivate = false;

                if (!success)
                {
                    inActivateFailed = true;
                    Grid.RaiseCurrentCellActivateFailed(rowIndex, colIndex);
                    inActivateFailed = false;
                }

                Trace.Unindent();
            }
        }

        /// <summary>
        /// Starts editing mode for the current cell.
        /// </summary>
        /// <remarks>
        /// You check <see cref="IsEditing"/> to query if the current cell is in editing mode.
        /// <para/>
        /// The method calls the cell renderer's <see cref="GridCellRendererBase.OnBeginEdit"/> method and thereby
        /// requests that current cell sets shows and focus to an associated <see cref="Control"/>. The text box
        /// cell for example, will make its <see cref="TextBox"/> visible and set the focus.<para/>
        /// Also, the current cell will be scrolled into view.
        /// </remarks>
        /// <returns>
        /// True if current cell supports editing; False otherwise.
        /// </returns>
        public bool BeginEdit()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (IsLocked)
            {
                return false;
            }

            inBeginEdit = true;
            try
            {
                if (IsValidCellPosition(rowIndex, colIndex))
                {
                    if (cellRenderer != null && !IsEditing && cellRenderer.SupportsEditing)
                    {
                        notifyChangingCalled = false;

                        if (!Grid.RaiseCurrentCellStartEditing())
                        {
                            return false;
                        }

                        if (cellRenderer == null || !cellRenderer.RaiseStartEditing())
                        {
                            return false;
                        }

                        isEditing = true;
                        cellRenderer.RaiseBeginEdit();
                    }

                    if (grid.HasControlFocus && !grid.IsMousePressed)
                    {
                        //// ScrollInView is a method in GridCurrentCell which then calls grid.ScrollCellInView. 
                        //// But first it checks cellRenderer.OnScrollCellInView which returns false for 
                        //// GridNestedTableControlCellRenderer.
                        ScrollInView(GridScrollCurrentCellReason.BeginEdit);
                    }

                    return true;
                }
            }
            finally
            {
                inBeginEdit = false;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether true when <see cref="BeginEdit()"/> was called; False after method returned.
        /// </summary>
        public bool IsInBeginEdit
        {
            get
            {
                return inBeginEdit;
            }
        }

        /// <overload>
        /// Starts editing mode for the current cell.
        /// </overload>
        /// <summary>
        /// Starts editing mode for the current cell and allows setting the focus to the cell editor.
        /// </summary>
        /// <param name="focusRenderer">Specifies if focus can be set to the cell editor.</param>
        /// <returns>True ifediting can proceed; False if it is aborted.</returns>
        /// <genoverload/>
        public bool BeginEdit(bool focusRenderer)
        {
            bool focusOBE = FocusRendererOnBeginEdit;
            FocusRendererOnBeginEdit = focusRenderer;
            try
            {
                return BeginEdit();
            }
            finally
            {
                FocusRendererOnBeginEdit = focusOBE;
            }
        }

        /// <overload>
        /// Moves the current cell to the specified position.
        /// </overload>
        /// <summary>
        /// Moves the current cell to the specified position and gives instruction about activation of the current cell. Changes may be discarded in the previous current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <param name="discardChanges">True if changes can be discarded; False otherwise.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <remarks>
        /// Moving the current cell is a two step process. In the first step, the grid deactivates the existing current cell, in
        /// the second step the grid activates the new current cell.
        /// <para/>
        /// All parameters that were specified in this method call will be saved. This allows event handler to have better
        /// background knowledge why certain events were raised.<para/>
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// After saving parameter information <see cref="MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> raises a cancelable <see cref="GridControlBase.CurrentCellMoving"/> event.
        /// If the event handler set the <see cref="CancelEventArgs.Cancel"/> flag of the <see cref="GridCurrentCellMovingEventArgs"/>
        /// object, the method will return immediately. <see cref="GridControlBase.CurrentCellMoving"/> gives you the chance
        /// to modify the parameters of the <see cref="MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> function call. You can modify the row and column index and the
        /// options.<para/>
        /// The next step is to deactivate the current cell. See <see cref="Deactivate"/> for a detailed overview what events will
        /// get raised from <see cref="Deactivate"/>. <para/>
        /// After the cell has been deactivated, the current cell will be activated
        /// at the new position. See the <see cref="Activate(int,int)"/> method for information which events are raised. If the
        /// <see cref="GridSetCurrentCellOptions.NoActivate"/> option is specified no current cell will be activated.<para/>
        /// You can sandwich the deactivation and activation process with a BeginUpdate, EndUpdate call pair if you
        /// specify the <see cref="GridSetCurrentCellOptions.BeginEndUpdate"/> option.<para/>
        /// <para/>
        /// At last, a <see cref="GridControlBase.CurrentCellMoved"/> event is raised which completes this operation.<para/>
        /// If an exception occurred or if an event handler requested to cancel the operation, <see cref="MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> will instead
        /// close up the operation with a  <see cref="GridControlBase.CurrentCellMoveFailed"/> event.
        /// <para/>
        /// This ensures that you will be guaranteed either a <see cref="GridControlBase.CurrentCellMoved"/>
        /// or <see cref="GridControlBase.CurrentCellMoveFailed"/> event after a <see cref="GridControlBase.CurrentCellMoving"/> event.
        /// </remarks>
        public bool MoveTo(int rowIndex, int colIndex, GridSetCurrentCellOptions options, bool discardChanges)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, rowIndex, colIndex, options, discardChanges);
            }
#else
            ;
#endif

            if (IsLocked)
            {
                return false;
            }

            ////SS            if (Control.ModifierKeys == Keys.Control)
            /////SS                Debugger.Break();

            GridCellRendererBase savedCellRenderer = this.cellRenderer;
            int savedRowIndex = this.rowIndex;
            int savedColIndex = this.colIndex;
            bool success = false;
            bool focused = grid.HasControlFocus;

            keyMoveRowIndex = -1;
            keyMoveColIndex = -1;

            gridModel.RaiseQueryMaximumRowCol(rowIndex, colIndex);

            if (rowIndex > grid.Model.RowCount || colIndex > grid.Model.ColCount)
            { 
                return false; 
            }

            int temprow = rowIndex == 0 ? grid.Model.Cols.HeaderCount + 1 : rowIndex;
            GridStyleInfo style = grid.GetViewStyleInfo(temprow, colIndex);
            try
            {
                if (!style.Enabled && !Grid.IsDesignMode())
                {
                    return false;
                }

                this.moveFromColIndex = this.colIndex;
                this.moveFromRowIndex = this.rowIndex;
                this.moveFromActiveState = this.HasCurrentCell;
                this.moveToOptions = options;
                this.moveToRowIndex = rowIndex;
                this.moveToColIndex = colIndex;
                this.inMoveTo = true;

                bool endUpdate = false;

                bool noActivate = false;

                Trace.Indent();
                try
                {
                    if (!Grid.RaiseCurrentCellMoving(ref rowIndex, ref colIndex, ref options))
                    {
                        return false;
                    }

                    // Update cached information in case Moving event changes parameters.
                    this.moveToOptions = options;
                    this.moveToRowIndex = rowIndex;
                    this.moveToColIndex = colIndex;

                    endUpdate = (options & GridSetCurrentCellOptions.BeginEndUpdate) != 0;
                    if (endUpdate)
                    {
                        grid.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                    }
                    
                    noActivate = (options & GridSetCurrentCellOptions.NoActivate) != 0;
                    if (!noActivate && rowIndex == savedRowIndex && colIndex == savedColIndex && HasCurrentCell)
                    {
                        if (Renderer.ShouldRefreshCurrentCell())
                        {
                            if (!HasControlFocus && !IsModified)
                            {
                                Refresh();
                                Initialize(options);
                            }
                            else
                            {
                                Renderer.SetHasFocusControl(true);
                                grid.InvalidateRange(GridRangeInfo.Cell(rowIndex, colIndex));
                            }
                        }

                        success = true;
                        return true;
                    }

                    AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                    // Update cached information in case AdjustRowColIfCoveredCell changes parameters.
                    this.moveToRowIndex = rowIndex;
                    this.moveToColIndex = colIndex;
                    ScrollBars movement = GetMovement(rowIndex, colIndex);

                    if (!Deactivate(discardChanges))
                    {
                        return false;
                    }

                    isModified = false;

                    if (!noActivate && !Activate(rowIndex, colIndex, options))
                    {
                        return false;
                    }

                    this.moveToRowIndex = this.rowIndex;
                    this.moveToColIndex = this.colIndex;

                    this.moveToDone = true;

                    success = true;

                    ////TraceUtil.TraceCurrentMethodInfo("Before Focus");
                    if (!noActivate && focused && grid.WantKeys && !HasControlFocus
                        && (options & GridSetCurrentCellOptions.NoSetFocus) == GridSetCurrentCellOptions.None)
                    {
                        grid.Focus();
                    }
#if DEBUG
                    else
                        if (Switches.GridFocus.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("--- No Grid.Focus()", "noActivate", noActivate, "focused", focused, "HasControlFocus", HasControlFocus, "options", options);
                        }
#endif

                    ////TraceUtil.TraceCurrentMethodInfo("After Focus");

                    return true;
                }
                finally
                {
                    this.moveToDone = true;

                    bool hasfocus = HasControlFocus;
                    if (success)
                    {
                        Grid.RaiseCurrentCellMoved(options);
                        hasfocus = false;
                    }
                    else
                    {
                        Grid.RaiseCurrentCellMoveFailed(rowIndex, colIndex, options);
                        if (ErrorMessage.Length > 0)
                        {
                            grid.CancelUpdate();
                            this.DisplayWarningText(this.ErrorMessage);
                        }
                        else
                        {
                            hasfocus = false;
                        }
                    }

                    if (endUpdate)
                    {
                        grid.EndUpdate(defaultUpdateFlag);
                    }

                    this.inMoveTo = false;
                    Trace.Unindent();

                    if (hasfocus && (options & GridSetCurrentCellOptions.NoSetFocus) == 0)
                    {
#if DEBUG
                        if (Switches.GridFocus.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo(Renderer.Control);
                        }
#else
                        ;
#endif

                        if (Renderer != null && Renderer.Control != null)
                        {
                            Renderer.Control.Focus();
                        }
                        else
                        {
                            grid.Focus();
                        }
                    }
                }
            }
            finally
            {
                style.Dispose();
                moveToDone = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this will be set true when MoveTo is finishing and when CurrentCellMoved event is raise. IsInMoveTo is still true
        /// at that time, but you can check MoveToDone whether the operation has ended.
        /// </summary>
        public bool MoveToDone
        {
            get
            {
                return moveToDone;
            }
        }

        // Later:
        //        void CurrentCellOperationFailed()
        //        {
        //
        //        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that holds new row and column index for the current cell; if it is <see cref="GridRangeInfo.Empty"/>
        /// only the current cell will be deactivated.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <genoverload/>
        public bool MoveTo(GridRangeInfo range)
        {
            if (range.IsEmpty)
            {
                return MoveTo(-1, -1, GridSetCurrentCellOptions.None, false);
            }
            else
            {
                return MoveTo(range.Top, range.Left, GridSetCurrentCellOptions.None, false);
            }
        }

        /// <summary>
        /// Moves the current cell to the specified area and gives instruction about activation of the current cell.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that holds new row and column index for the current cell; if it is <see cref="GridRangeInfo.Empty"/>
        /// only the current cell will be deactivated.</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <genoverload/>
        public bool MoveTo(GridRangeInfo range, GridSetCurrentCellOptions options)
        {
            if (range.IsEmpty)
            {
                return MoveTo(-1, -1, options, false);
            }
            else
            {
                return MoveTo(range.Top, range.Left, options, false);
            }
        }

        /// <summary>
        /// Moves the current cell to the specified area and gives instruction about activation of the current cell. Changes may be discarded in the previous current cell.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that holds new row and column index for the current cell; if it is <see cref="GridRangeInfo.Empty"/>
        /// only the current cell will be deactivated.</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <param name="discardChanges">True if changes can be discarded; False otherwise.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <genoverload/>
        public bool MoveTo(GridRangeInfo range, GridSetCurrentCellOptions options, bool discardChanges)
        {
            if (range.IsEmpty)
            {
                return MoveTo(-1, -1, options, discardChanges);
            }
            else
            {
                return MoveTo(range.Top, range.Left, options, discardChanges);
            }
        }

        /// <summary>
        /// Sets internally the new position of a current cell, but does not activate it.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <remarks>
        /// Sets <see cref="GridCurrentCell.RowIndex"/> and <see cref="GridCurrentCell.ColIndex"/> but <see cref="GridCurrentCell.HasCurrentCell"/>
        /// will be False.
        /// </remarks>
        public void SetPositionNoActivate(int rowIndex, int colIndex)
        {
            if (IsLocked)
            {
                return;
            }

            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.keyMoveRowIndex = rowIndex;
            this.keyMoveColIndex = colIndex;

            GridCellRendererBase targetRenderer = grid.GetCellRenderer(rowIndex, colIndex);
            this.cellRenderer = targetRenderer;
            if (cellRenderer != null)
            {
                this.cellRenderer.RowIndex = rowIndex;
                this.cellRenderer.ColIndex = colIndex;
            }
        }

        /// <summary>
        /// Resets internally the current cell, but does not deactivate it.
        /// </summary>
        /// <remarks>
        /// <see cref="GridCurrentCell.HasCurrentCell"/>
        /// will be False.
        /// </remarks>
        public void ResetCurrentCellWithoutDeactivate()
        {
            if (IsLocked)
            {
                return;
            }

            if (this.cellRenderer != null)
            {
                this.cellRenderer.Hide();
            }

            this.cellRenderer = null;
            this.isEditing = false;
            this.isModified = false;
        }

        /// <summary>
        /// Sets internally the new position of a current cell, but does not activate it.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <remarks>
        /// Sets <see cref="GridCurrentCell.RowIndex"/> and <see cref="GridCurrentCell.ColIndex"/>/. <see cref="GridCurrentCell.HasCurrentCell"/>
        /// will be True afterwards.
        /// </remarks>
        public void SetCurrentCellNoActivate(int rowIndex, int colIndex)
        {
            if (IsLocked)
            {
                return;
            }

            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.keyMoveRowIndex = rowIndex;
            this.keyMoveColIndex = colIndex;

            if (rowIndex >= 0 && colIndex >= 0)
            {
                GridCellRendererBase targetRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                this.cellRenderer = targetRenderer;
            }
        }

        /// <summary>
        /// Moves the current cell to the specified position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <genoverload/>
        public bool MoveTo(int rowIndex, int colIndex)
        {
            return MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.None, false);
        }

        /// <summary>
        /// Moves the current cell to the specified area and gives instruction about activation of the current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more.</param>
        /// <returns>True if the current cell could be moved; False otherwise.</returns>
        /// <genoverload/>
        public bool MoveTo(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
            return MoveTo(rowIndex, colIndex, options, false);
        }

        //// read-only access to MoveTo parameters, only valid when IsInMoveTo is true.

        /// <summary>
        /// Gets a value indicating whether there was an activated current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> was called.
        /// </summary>
        public bool MoveFromActiveState
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    ////                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    ////                    TraceUtil.TraceCurrentMethodInfo(moveFromActiveState, Grid);
                    ////                    TraceUtil.TraceCalledFrom(20);
                }

                return moveFromActiveState;
            }
        }

        /// <summary>
        /// Gets the target row index of the current <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> method call.
        /// </summary>
        public int MoveToRowIndex
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    TraceUtil.TraceCurrentMethodInfo(moveToRowIndex, Grid);
                    TraceUtil.TraceCalledFrom(20);
                }

                return moveToRowIndex;
            }
        }

        /// <summary>
        /// Gets the target column index of the current <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> method call.
        /// </summary>
        public int MoveToColIndex
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    TraceUtil.TraceCurrentMethodInfo(moveToColIndex, Grid);
                    TraceUtil.TraceCalledFrom(20);
                }

                return moveToColIndex;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridSetCurrentCellOptions"/> of the current <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> method call.
        /// </summary>
        public GridSetCurrentCellOptions MoveToOptions
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    TraceUtil.TraceCurrentMethodInfo(moveToOptions, Grid);
                    TraceUtil.TraceCalledFrom(20);
                }

                return moveToOptions;
            }
        }

        /// <summary>
        /// Gets the saved row index information about the previous position of the current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> was called.
        /// </summary>
        public int MoveFromRowIndex
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    TraceUtil.TraceCurrentMethodInfo(moveFromRowIndex, Grid);
                    TraceUtil.TraceCalledFrom(20);
                }

                return moveFromRowIndex;
            }
        }

        /// <summary>
        /// Gets the saved column index information about the previous position of the current cell
        /// at the time that <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> was called.
        /// </summary>
        public int MoveFromColIndex
        {
            get
            {
                if (!inMoveTo)
                {
                    if (raiseExceptionIfNestedActiveOrDeactivate)
                    {
                        throw new InvalidOperationException("Invalid state: IsInMoveTo returns false");
                    }

                    Trace.WriteLine("Invalid state: IsInMoveTo returns false");
                    TraceUtil.TraceCurrentMethodInfo(moveFromColIndex, Grid);
                    TraceUtil.TraceCalledFrom(20);
                }

                return moveFromColIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> is in progress. MoveTo saves information
        /// about the current cell state and its target cell so that events can more easily compare
        /// previous and new states of the current cell during <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> calls.
        /// </summary>
        /// <remarks>
        /// If stand-alone Activate and Deactivate calls were made (if these calls where initiated
        /// outside from MoveTo) you can't use any of these properties. But in that case,
        /// the <see cref="GridControlBase.CurrentCellActivating"/> and <see cref="GridControlBase.CurrentCellDeactivating"/>
        /// events supply sufficient information about the intended action and you can query
        /// <see cref="GridCurrentCell.RowIndex"/>, <see cref="GridCurrentCell.ColIndex"/>, and <see cref="GridCurrentCell.HasCurrentCell"/>
        /// for the current state.
        /// </remarks>
        public bool IsInMoveTo
        {
            get
            {
                return inMoveTo;
            }
        }

        //// InActivate, InDeactivae, Failed?

        void Initialize(GridSetCurrentCellOptions options)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, options);
            }
#else
            ;
#endif

            Trace.Indent();

            if (IsLocked)
            {
                return;
            }

            try
            {
                if (HasCurrentCell)
                {
                    if ((options & GridSetCurrentCellOptions.SetFocus) == GridSetCurrentCellOptions.SetFocus)
                    {
                        BeginEdit(Grid.Focused);
                    }

                    if ((options & GridSetCurrentCellOptions.ScrollInView) != GridSetCurrentCellOptions.None)
                    {
                        ScrollInView(GridScrollCurrentCellReason.MoveTo);
                    }

                    gridModel.SetActiveCurrentCell(rowIndex, colIndex);
                }

                if (gridModel.Options.ShouldSynchronizeCurrentCell &&
                    (options & GridSetCurrentCellOptions.NoSyncCurrentCell) == GridSetCurrentCellOptions.None)
                {
                    if (HasCurrentCell)
                    {
                        gridModel.SynchronizeCurrentCell(rowIndex, colIndex);
                    }
                    else
                    {
                        gridModel.SynchronizeCurrentCell(-1, -1);
                    }
                }

                if (!grid.Updating && updateInInitialize)
                {
                    grid.Update();
                }

                IsModified = false;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Refreshes the current cell and forces it to repaint. If the current cell is not modified, this method will deactivate and reactivate current cell.
        /// </summary>
        public void Refresh()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (IsChanging || this.IsLocked)
            {
                return;
            }

            Trace.Indent();
            try
            {
                if (Renderer != null && !(this.inActivate || this.inDeactivate))
                {
                    GridStyleInfo style = Grid.GetViewStyleInfo(rowIndex, colIndex);
                    GridCellRendererBase newRenderer = Grid.CellRenderers[style.CellType];
                    if (newRenderer != cellRenderer)
                    {
                        Reactivate();
                        return;
                    }

                    Renderer.Hide();
                    if (!IsModified)
                    {
                        Renderer.Initialize(rowIndex, colIndex);
                    }
                }

                if (rowIndex >= 0 && colIndex >= 0)
                {
                    Grid.InvalidateRange(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                }
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Deactivates and reactivates the current cell.
        /// </summary>
        public void Reactivate()
        {
            if (IsLocked || this.inActivate || this.inDeactivate)
            {
                return;
            }

            if (!inConfirmChanges && HasCurrentCell)
            {
                bool editing = IsEditing;
                Deactivate(true);
                Activate(RowIndex, ColIndex);
                if (editing && !IsEditing)
                {
                    BeginEdit();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the grid has an active current cell.
        /// </summary>
        public bool HasCurrentCell
        {
            get
            {
                return !InternalHide && this.cellRenderer != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current cell should be activated when the grid gets the focus. The grid
        /// will set this property when it loses focus so that it later knows if a current cell should
        /// be activated when it gets focus again.
        /// </summary>
        public bool ActivateOnGotFocus
        {
            get
            {
                return activateOnGotFocus;
            }

            set
            {
#if DEBUG
                if (Switches.CurrentCell.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, value);
                }
#else
                ;
#endif

                activateOnGotFocus = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the grid has an active current cell.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.cellRenderer != null && !InternalHide;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current cell is in editing mode.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return isEditing && IsActive;
            }
        }

        /// <summary>
        /// Validates the contents of the current cell. If contents are invalid or if an exception is thrown,
        /// a message is stored in <see cref="ErrorMessage"/>.
        /// </summary>
        /// <remarks>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellValidating"/> event and calls the renderer's
        /// <see cref="GridCellRendererBase.OnValidate"/> method. If contents are valid, <see cref="GridControlBase.CurrentCellValidated"/>
        /// will be raised afterwards.
        /// </remarks>
        public void Validate()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            if (cellRenderer == null)
            {
                return;
            }

            Trace.Indent();
            try
            {
                // Possibly shows a dialog box (but user can also set CurrentCell.ErrorMessage).
                IsValid = Grid.RaiseCurrentCellValidating() && (cellRenderer == null || cellRenderer.RaiseValidate());
                if (IsValid)
                {
                    if (cellRenderer != null)
                    {
                        cellRenderer.RaiseValidated();
                    }

                    Grid.RaiseCurrentCellValidated();
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                IsValid = false;
                this.Exception = ex;

                if (!ExceptionManager.RaiseExceptionCatched(grid, ex))
                {
                    throw;
                }

                ErrorMessage = ex.Message;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether success of the latest <see cref="Validate"/> method call.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        /// <summary>
        /// Resets error information for the current cell.
        /// </summary>
        public void ResetError()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            errorMessage = string.Empty;
            IsError = false;
            exception = null;
        }

        /// <summary>
        /// Gets a value indicating whether the drop-down window of a current cell is dropped-down.
        /// </summary>
        public bool IsDroppedDown
        {
            get
            {
                return cellRenderer != null && cellRenderer.GetDroppedDown();
            }
        }

        /// <summary>
        /// Hides or shows the drop-down window.
        /// </summary>
        public void ToggleDropDown()
        {
            if (!IsDroppedDown)
            {
                ShowDropDown();
            }
            else
            {
                CloseDropDown(PopupCloseType.Canceled);
            }
        }

        /// <summary>
        /// Shows the drop-down window.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="BeginEdit()"/> and afterwards the renderer's <see cref="GridCellRendererBase.OnShowDropDown"/> method.
        /// </remarks>
        public void ShowDropDown()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (IsDroppedDown)
            {
                return;
            }

            inShowDropDown = true;
            //// Must be editing.
            BeginEdit();
            if (cellRenderer != null)
            {
                cellRenderer.RaiseShowDropDown();
            }

            inShowDropDown = true;
        }

        internal bool InShowDropDown
        {
            get
            {
                return inShowDropDown;
            }
        }

        /// <summary>
        /// Closes the drop-down window.
        /// </summary>
        /// <param name="reason">The reason lets you specify if the popup is closed because your application
        /// is being deactivated or because the user clicked in the grid and changes in the current cell should be confirmed.</param>
        /// <remarks>
        /// Calls the renderer's <see cref="GridCellRendererBase.OnCloseDropDown"/> method.
        /// </remarks>
        public void CloseDropDown(PopupCloseType reason)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, reason);
            }
#else
            ;
#endif

            if (!IsDroppedDown)
            {
                return;
            }

            if (cellRenderer != null)
            {
                cellRenderer.RaiseCloseDropDown(reason);
            }
        }

        ////        public string DisplayText
        ////        {
        ////            get { return string.Empty; }
        ////            set { }
        ////        }
        ////
        ////        public string SelectedText
        ////        {
        ////            get { return string.Empty; }
        ////            set { }
        ////        }
        ////
        ////        public object CellValue
        ////        {
        ////            get { return null; }
        ////            set { }
        ////        }

        ////        public bool ReadOnly
        ////        {
        ////            get { return false; }
        ////        }

        bool IsValidCellPosition(int rowIndex, int colIndex)
        {
            return rowIndex >= 0 && colIndex >= 0
                && rowIndex <= gridModel.RowCount && colIndex <= gridModel.ColCount;
        }

        /// <summary>
        /// Scrolls the current cell into view.
        /// </summary>
        /// <returns>true if operation could be completed; false if there is no active current cell.</returns>
        public bool ScrollInView()
        {
            return ScrollInView(GridScrollCurrentCellReason.Any);
        }

        /// <summary>
        /// Scrolls the current cell into view.
        /// </summary>
        /// <returns>True if operation could be completed; False if there is no active current cell.</returns>
        public bool ScrollInView(GridScrollCurrentCellReason reason)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (IsValidCellPosition(rowIndex, colIndex) && Renderer != null && Renderer.OnScrollInView(reason))
            {
                return grid.ScrollCellInView(this.rowIndex, this.colIndex, reason);
            }

            return false;
        }

        internal void SetWarningText(string s)
        {
            errorMessage = s;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void DisplayWarningText(string message)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, message);
            }
#else
            ;
#endif

            if (GridUtil.NotEmptyAfterTrim(errorMessage))
            {
                bool hasFocus = grid.HasControlFocus;

                //// Make sure cell is visible.
                int nEditRow, nEditCol;
                if (GetCurrentCell(out nEditRow, out nEditCol))
                {
                    grid.ScrollCellInView(nEditRow, nEditCol, GridScrollCurrentCellReason.Error);
                }

                grid.CancelUpdate();

                // Display box.
                Form form = FindFormHelper.FindForm(grid);
#if DEBUG
                if (Switches.GridFocus.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(form, errorMessage, grid);
                }
#else
                ;
#endif

                GridCurrentCellErrorMessageEventArgs e = new GridCurrentCellErrorMessageEventArgs(form, errorMessage);
                Grid.RaiseCurrentCellErrorMessage(e);
                if (!e.Cancel)
                {
                    if (ShowErrorMessageBox)
                        MessageBoxAdv.Show(form, errorMessage);
                }
                else
                {
                    IsError = false;
                    ValidationErrorText = string.Empty;
                    ShowErrorIcon = false;
                    Grid.ShowRowHeaderErroricon = false;
                }

                if (hasFocus && !grid.ContainsFocus)
                {
                    grid.Focus();
                }
            }

            //this.ResetError();
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Obsolete("This call does not have any effect. Just make sure you have CausesValidation = false.")]
        public void SetIgnoreFocus(bool b)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="Control"/> is displayed in the current cell and if it is focused.
        /// </summary>
        public bool HasControlFocus
        {
            get
            {
                return cellRenderer != null && !InternalHide && cellRenderer.HasFocusControl;
            }

            set
            {
                if (value != HasControlFocus)
                {
                    if (cellRenderer != null && IsEditing)
                    {
                        cellRenderer.SetHasFocusControl(value);
                    }

                    grid.InvalidateRange(RangeInfo);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there are pending changes in the current cell.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return isModified && IsActive;
            }

            set
            {
#if DEBUG
                if (Switches.CurrentCell.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, value);
                }
#else
                ;
#endif
                if (!IsModified && value)
                {
                    isModified = value;  //// this line helps with debugging ...
                }

                isModified = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current cell's <see cref="GridControlBase.CurrentCellChanging"/> event
        /// is being handled.
        /// </summary>
        public bool IsChanging
        {
            get
            {
                return isChanging;
            }

            set
            {
                isChanging = value;
            }
        }

        /// <summary>
        /// Suspends raising events.
        /// </summary>
        public void SuspendEvents()
        {
            suspendEvents++;
        }

        /// <summary>
        /// Resumes raising events.
        /// </summary>
        public void ResumeEvents()
        {
            if (suspendEvents > 0)
            {
                suspendEvents--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether raising events is temporarily disabled.
        /// </summary>
        public bool IsSuspendEvents
        {
            get
            {
                return suspendEvents > 0;
            }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
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
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridCellRendererBase"/> of the current cell.
        /// </summary>
        public GridCellRendererBase Renderer
        {
            get { return cellRenderer; }
        }

        /// <summary>
        /// Gets the <see cref="GridCellModelBase"/> of the current cell.
        /// </summary>
        public GridCellModelBase Model
        {
            get { return cellModel; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.Activate(int, int)"/> is in progress.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridControlBase.CurrentCellActivating"/> event gives information
        /// about the intended action and you can query
        /// <see cref="GridCurrentCell.RowIndex"/> and <see cref="GridCurrentCell.ColIndex"/>
        /// for the current state. <see cref="GridCurrentCell.HasCurrentCell"/> will be false
        /// at the time when <see cref="GridCurrentCell.Activate(int, int)"/> is called.
        /// </remarks>
        public bool IsInActivate
        {
            get
            {
                return inActivate;
            }
        }

        /// <internalonly/>
        /// <summary>Gets a value indicating whether IsInActivated. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsInActivated
        {
            get
            {
                return inActivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.Deactivate"/> is in progress.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridControlBase.CurrentCellDeactivating"/> event gives information
        /// about the intended action and you can query
        /// <see cref="GridCurrentCell.RowIndex"/> and <see cref="GridCurrentCell.ColIndex"/>
        /// for the current state. <see cref="GridCurrentCell.HasCurrentCell"/> will be true
        /// at the time when <see cref="GridCurrentCell.Activate(int, int)"/> is called.
        /// </remarks>
        public bool IsInDeactivate
        {
            get
            {
                return inDeactivate;
            }
        }

        /// <internalonly/>
        /// <summary>Gets a value indicating whether IsInConfirmChanges. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsInConfirmChanges
        {
            get
            {
                return this.inConfirmChanges;
            }
        }

        /// <internalonly/>
        /// <summary>Gets a value indicating whether IsInAcceptedChanges. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsInAcceptedChanges
        {
            get
            {
                return this.inAcceptedChanges;
            }
        }

        /// <internalonly/>
        /// <summary>Gets a value indicating whether IsInEndEdit. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsInEndEdit
        {
            get
            {
                return this.inEndEdit;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridCurrentCell.Deactivate"/> or <see cref="GridCurrentCell.Activate(int, int)"/>
        /// is in progress.
        /// </summary>
        public bool IsInActiveOrDeactivate
        {
            get
            {
                return inDeactivate || inActivate || inActivateFailed || inDeactivateFailed;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifyControlDoubleClick(Control control)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, control);
            }
#else
            ;
#endif

            Grid.RaiseCurrentCellControlDoubleClick(control);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifyControlGotFocus(Control control)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, control);
            }
#else
            ;
#endif

            Grid.RaiseCurrentCellControlGotFocus(control);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifyControlLostFocus(Control control)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, control);
            }
#else
            ;
#endif

            Grid.RaiseCurrentCellControlLostFocus(control);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifyChanged()
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif
            if (!IsInActiveOrDeactivate)
            {
                IsModified = true;
                Grid.RaiseCurrentCellChanged();
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value NotifyChanging</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool NotifyChanging()
        {
            //// QA issue 23 fix
            notifyChangingCalled = true;
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
            ;
#endif

            if (!IsInActiveOrDeactivate)
            {
                notifyChangingCalled = Grid.RaiseCurrentCellChanging();
                return notifyChangingCalled;
            }

            return true;
        }

        /// <summary>
        /// Determines the next enabled cell when current cell wants to move into a given direction. Cells that are not
        /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if an enabled cell was found; False otherwise.</returns>
        /// <remarks>
        /// This method will not raise the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event. Instead
        /// you can call this method from your QueryNextMoveCell event handler to find out about the next
        /// enabled cell and then decide on further criteria if the suggested cell is good.
        /// <para/>
        /// You should call <see cref="GridControlBase.GetNextCurrentCellPosition"/> instead if you want
        /// the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event to be raised.
        /// </remarks>
        public bool QueryNextEnabledCell(GridDirectionType direction, ref int rowIndex, ref int colIndex)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc, direction, rowIndex, colIndex);
            }
#else
            ;
#endif

            int count;
            GridRangeInfo coveredRange;
            int targetRow = rowIndex;
            int targetCol = colIndex;
            bool enabled = false;
            GridStyleInfo style;

            switch (direction)
            {
                case GridDirectionType.Up:
                    while (targetRow >= 1 && !enabled)
                    {
                        //// Skip invisible and covered cells.
                        while (targetRow >= 1
                            && (grid.GetRowHeight(targetRow) == 0
                            || (gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Top != targetRow)))
                        {
                            targetRow--;
                        }

                        if (targetRow >= 1)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetViewStyleInfo(coveredRange.Top, coveredRange.Left);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();
                        }

                        //// Not enabled, continue search.
                        if (!enabled && targetRow >= 1)
                        {
                            targetRow--;
                        }
                    }

                    break;

                case GridDirectionType.Down:
                    count = gridModel.RowCount;
                    while (targetRow <= count && !enabled)
                    {
                        //// Skip invisible and covered cells.
                        while (targetRow <= count
                            && (grid.GetRowHeight(targetRow) == 0
                            || (gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Top != targetRow)))
                        {
                            targetRow++;
                        }

                        if (targetRow <= count)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetViewStyleInfo(coveredRange.Top, coveredRange.Left);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            //// Not enabled, continue search.
                            if (!enabled)
                            {
                                targetRow++;
                            }
                        }
                    }

                    break;

                case GridDirectionType.Left:
                    while (targetCol >= 1 && !enabled)
                    {
                        //// Skip invisible and covered cells.
                        while (targetCol >= 1
                            && (grid.GetColWidth(targetCol) == 0
                            || (gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Left != targetCol)))
                        {
                            targetCol--;
                        }

                        if (targetCol >= 1)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetViewStyleInfo(coveredRange.Top, coveredRange.Left);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            //// Not enabled, continue search.
                            if (!enabled && targetCol >= 1)
                            {
                                targetCol--;
                            }
                        }
                    }

                    break;

                case GridDirectionType.Right:
                    count = gridModel.ColCount;
                    while (targetCol <= count && !enabled)
                    {
                        //// Skip invisible and covered cells.
                        while (targetCol <= count
                            && (grid.GetColWidth(targetCol) == 0
                            || (gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                            && coveredRange.Left != targetCol)))
                        {
                            targetCol++;
                        }

                        if (targetCol <= count)
                        {
                            gridModel.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                            style = grid.GetViewStyleInfo(coveredRange.Top, coveredRange.Left);
                            enabled = style.Enabled || grid.IsDesignMode();
                            style.Dispose();

                            //// Not enabled, continue search.
                            if (!enabled)
                            {
                                targetCol++;
                            }
                        }
                    }

                    break;
            }

            if (enabled)
            {
                rowIndex = targetRow;
                colIndex = targetCol;
            }

            return enabled;
        }

        internal void UpdateRemoveRows(int fromRowIndex, int toRowIndex)
        {
            if (HasCurrentCell)
            {
                return;
            }

            int count = toRowIndex - fromRowIndex + 1;
            if (this.rowIndex >= fromRowIndex && this.rowIndex <= toRowIndex)
            {
            }
            else if (this.rowIndex > toRowIndex)
            {
                this.rowIndex -= count;
                this.Activate(this.rowIndex, this.colIndex);
            }
        }

        internal void UpdateRemoveCols(int fromColIndex, int toColIndex)
        {
            if (HasCurrentCell)
            {
                return;
            }

            int count = toColIndex - fromColIndex + 1;
            if (this.colIndex >= fromColIndex && this.colIndex <= toColIndex)
            {
            }
            else if (this.rowIndex > toColIndex)
            {
                this.colIndex -= count;
                this.Activate(this.rowIndex, this.colIndex);
            }
        }

        internal void UpdateRowsMoved(int fromRowIndex, int toRowIndex, int targetRow)
        {
            if (HasCurrentCell)
            {
                return;
            }

            int count = toRowIndex - fromRowIndex + 1;
            int target = targetRow;
            if (target > fromRowIndex)
            {
                target += count;
            }

            if (this.rowIndex >= fromRowIndex && this.rowIndex <= toRowIndex && this.rowIndex >= target)
            {
                this.rowIndex = target + (this.rowIndex - fromRowIndex);
            }
            else if (this.rowIndex >= fromRowIndex && this.rowIndex <= toRowIndex && this.rowIndex < target)
            {
                this.rowIndex = target - count + (this.rowIndex - fromRowIndex);
            }
            else if (this.rowIndex >= fromRowIndex && this.rowIndex < target)
            {
                this.rowIndex = this.rowIndex - count;
            }
            else if (this.rowIndex <= fromRowIndex && this.rowIndex >= target)
            {
                this.rowIndex = this.rowIndex + count;
            }

            this.Activate(this.rowIndex, this.colIndex);
        }

        internal void UpdateColsMoved(int fromColIndex, int toColIndex, int targetCol)
        {
            if (HasCurrentCell)
            {
                return;
            }

            int count = toColIndex - fromColIndex + 1;
            int target = targetCol;
            if (target > fromColIndex)
            {
                target += count;
            }

            if (this.colIndex >= fromColIndex && this.colIndex <= toColIndex && this.colIndex >= target)
            {
                this.colIndex = target + (this.colIndex - fromColIndex);
            }
            else if (this.colIndex >= fromColIndex && this.colIndex <= toColIndex && this.colIndex < target)
            {
                this.colIndex = target - (toColIndex - fromColIndex + 1) + (this.colIndex - fromColIndex);
            }
            else if (this.colIndex >= fromColIndex && this.colIndex <= target)
            {
                this.colIndex = this.colIndex - (toColIndex - fromColIndex + 1);
            }
            else if (this.colIndex <= fromColIndex && this.colIndex >= target)
            {
                this.colIndex = this.colIndex + (toColIndex - fromColIndex + 1);
            }

            this.Activate(this.rowIndex, this.colIndex);
        }

        internal void UpdateInsertRows(int rowIndex, int count)
        {
            if (!HasCurrentCell && rowIndex <= this.rowIndex)
            {
                this.rowIndex += count;
            }

            this.Activate(this.rowIndex, this.colIndex);
        }

        internal void UpdateInsertCols(int colIndex, int count)
        {
            if (!HasCurrentCell && colIndex <= this.colIndex)
            {
                this.colIndex += count;
            }

            this.Activate(this.rowIndex, this.colIndex);
        }

        /// <overload>
        /// Returns position of the current cell together with a boolean if the current cell is active or not.
        /// </overload>
        /// <summary>
        /// Returns position of the current cell together with a boolean if the current cell is active or not.
        /// </summary>
        /// <param name="rowIndex">The row index of the current cell.</param>
        /// <param name="colIndex">The column index of the current cell.</param>
        /// <returns>True if grid has active current cell; False otherwise.</returns>
        public bool GetCurrentCell(out int rowIndex, out int colIndex)
        {
            return GetCurrentCell(out rowIndex, out colIndex, false);
        }

        /// <summary>
        /// Returns position of the current cell together with a boolean if the current cell is active or not.
        /// </summary>
        /// <param name="rowIndex">The row index of the current cell.</param>
        /// <param name="colIndex">The column index of the current cell.</param>
        /// <param name="bIgnorePrint">True if current cell should also be returned when <see cref="GridControlBase.PrintingMode"/> is True.</param>
        /// <returns>True if grid has active current cell; False otherwise.</returns>
        /// <genoverload/>
        public bool GetCurrentCell(out int rowIndex, out int colIndex, bool bIgnorePrint /*=false*/)
        {
            rowIndex = this.rowIndex;
            colIndex = this.colIndex;

            bool hasCurrentCell = this.HasCurrentCell;

            //// Nomally, it returns NULL when print-modus is active.
            return hasCurrentCell && (bIgnorePrint || !grid.IsPrinting());
        }

        /// <overload>
        /// Checks if the current cell is active at a specified position in the grid.
        /// </overload>
        /// <summary>
        /// Checks if the current cell is active and at the specific row index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>True if current cell was found at row index; False otherwise.</returns>
        /// <genoverload/>
        public bool HasCurrentCellAt(int rowIndex)
        {
            return HasCurrentCellAt(rowIndex, GridConstants.Undefined, false);
        }

        /// <summary>
        /// Checks if the current cell is active and at the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if current cell was found at row and column index; False otherwise.</returns>
        /// <genoverload/>
        public bool HasCurrentCellAt(int rowIndex, int colIndex)
        {
            return HasCurrentCellAt(rowIndex, colIndex, false);
        }

        /// <summary>
        /// Checks if the current cell is active and at the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bIgnorePrint">True if current cell should also be returned when <see cref="GridControlBase.PrintingMode"/> is True.</param>        
        /// <returns>True if current cell was found at row and column index; False otherwise.</returns>
        /// <genoverload/>
        public virtual bool HasCurrentCellAt(int rowIndex /*= GridConstants.Undefined*/, int colIndex /*= GridConstants.Undefined*/, bool bIgnorePrint /*=false*/)
        {
            if (!HasCurrentCell
                || (grid.IsPrinting() && !bIgnorePrint))
            {
                return false;
            }

            if ((rowIndex == GridConstants.Undefined || rowIndex == this.rowIndex)
                && (colIndex == GridConstants.Undefined || colIndex == this.colIndex)
                && this.rowIndex <= grid.Model.RowCount
                && this.colIndex <= grid.Model.ColCount)
            {
                return true;
            }

            return false;
        }
    }
}