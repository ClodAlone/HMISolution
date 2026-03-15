//-------------------------------------------------------------------------------------------------
// <copyright file="GridSelectCellsMouseController.cs" company="syncfusion">
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

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the cell selection behavior of a grid control.
    /// </summary>
    public class GridSelectCellsMouseController : GridMouseController
    {
        internal void SetSelectClickRowCol(int rowIndex, int colIndex)
        {
            this.Mrci.mouseClickColIndex = colIndex;
            this.Mrci.mouseClickRowIndex = rowIndex;
            this.Mrci.mouseDownColIndex = colIndex;
            this.Mrci.mouseDownRowIndex = rowIndex;
            this.Mrci.firstClick = false;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        sealed class MouseRowColInfo
        {
            internal int mouseClickRowIndex = 0;
            internal int mouseClickColIndex = 0;
            internal int mouseDownRowIndex = 0;
            internal int mouseDownColIndex = 0;
            internal int mouseMoveRowIndex = -1;
            internal int mouseMoveColIndex = -1;
            internal bool firstClick = true;

            public override string ToString()
            {
                return String.Format("{0},{1}->{2},{3}", mouseDownRowIndex, mouseDownColIndex, mouseMoveRowIndex, mouseMoveColIndex);
            }
        }

        MouseRowColInfo mrci = null;

        MouseRowColInfo Mrci
        {
            get
            {
                if (mrci == null)
                {
                    if (grid.Model.UserData.Contains("MouseRowColInfo"))
                    {
                        mrci = (MouseRowColInfo)grid.Model.UserData["MouseRowColInfo"];
                    }
                    else
                    {
                        grid.Model.UserData["MouseRowColInfo"] = mrci = new MouseRowColInfo();
                    }
                }

                return mrci;
            }
        }

        private SelectCellsHitTestInfo hitTestInfo = null;
        private SelectCellsHitTestInfo mouseHoverInfo = null;
        private bool ignoreMouseMessages = false;
        private bool inSelectingCells = false;
        private bool addNewSelection;
        private bool extendSelection;
        [ThreadStaticAttribute]
        static bool scrolled = false;
        private bool canceled = false;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GSM(" + (ignoreMouseMessages ? "IGN " : string.Empty) +
                (inSelectingCells ? "INS " : string.Empty)
                + (addNewSelection ? "ADD " : string.Empty)
                + (extendSelection ? "EXT " : string.Empty)
                + (scrolled ? "SCR " : string.Empty)
                + " ) " + Mrci.ToString() + " "
                + ((hitTestInfo != null) ? hitTestInfo.ToString() + " " : string.Empty)
                + grid.Model.SelectedRanges.ToString() + " "
                + grid.ToString();
        }

        GridControlBase grid = null;

        /// <summary>
        /// Initializes a new GridSelectCellsMouseController and attaches it to a grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridSelectCellsMouseController(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            grid.CurrentCellActivated += new EventHandler(GridCurrentCellActivated);
            grid.WindowScrolled += new ScrollWindowEventHandler(GridWindowScrolled);
            grid.ExternalMove = new GridCurrentCellMoveDelegateHandler(GridCurrentCellExternalMove);
            grid.CancelMode += new EventHandler(grid_CancelMode);
        }

        void GridCurrentCellMoveComplete(object sender, GridCurrentCellMovedEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e, this);
            }
#else
            ;
#endif

            if (!(grid.CurrentCell.MoveFromActiveState && grid.CurrentCell.IsInMoveTo
                && grid.CurrentCell.HasCurrentCellAt(grid.CurrentCell.MoveFromRowIndex, grid.CurrentCell.MoveFromColIndex)))
            {
                ProcessSetCurrentCell(grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex, e.Options);
            }
        }

        void GridCurrentCellActivated(object sender, EventArgs e)
        {
            if (Grid.CurrentCell.IsInMoveTo)
            {
                return;
            }

            ProcessSetCurrentCell(grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex, GridSetCurrentCellOptions.None);
        }

        void GridWindowScrolled(object sender, ScrollWindowEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e, this);
            }
#else
            ;
#endif
            scrolled = true;
        }

        /// <override/>
        /// <summary>
        /// Override this method in your <see cref="GridMouseController"/> and return False if it would interfere with your
        /// controller's state when the current cell would be focused and possibly scrolled into view.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> (True by default) that indicates if the grid is allowed to set the focus onto the current cells <see cref="Control"/>.
        /// </returns>
        public override bool GetAllowFixFocus()
        {
            return false;
        }

        bool disposed = false;

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (grid != null)
                {
                    grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
                    grid.CurrentCellActivated -= new EventHandler(GridCurrentCellActivated);
                    grid.WindowScrolled -= new ScrollWindowEventHandler(GridWindowScrolled);
                    grid.CancelMode -= new EventHandler(grid_CancelMode);
                    grid.ExternalMove = null;
                    grid.CancelMode -= new EventHandler(grid_CancelMode);
                    disposed = true;
                }
            }

            base.Dispose(disposing);
        }

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason)
        {
            return RaiseSelectionChanging(ref range, reason, GridRangeInfo.Empty);
        }

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason, GridRangeInfo clickRange)
        {
            GridSelectionChangingEventArgs e = new GridSelectionChangingEventArgs(range, reason, clickRange);
            grid.Model.RaiseSelectionChanging(e);
            range = e.Range;
            return !e.Cancel;
        }

        void RaiseSelectionChanged(GridRangeInfo range, GridSelectionReason reason)
        {
            GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(range, null, reason);
            grid.Model.RaiseSelectionChanged(e);
        }

        void RaiseSelectionChanged(GridRangeInfo range, GridRangeInfoList oldRanges, GridSelectionReason reason)
        {
            GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(range, oldRanges, reason);
            grid.Model.RaiseSelectionChanged(e);
        }

        /// <summary>
        /// Grids the current cell external move.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool GridCurrentCellExternalMove(GridDirectionType direction, int num, bool extendSelection)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(direction, num, extendSelection, this);
            }
#else
            ;
#endif
            // Move current cell and select cells if user pressed SHIFT- or CONTROL-Key.
            int rowIndex = 0;
            int colIndex = 0;
            GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

            extendSelection &= ((grid.Model.Options.AllowSelection & GridSelectionFlags.Keyboard) != GridSelectionFlags.None
                && grid.Model.Options.ListBoxSelectionMode != SelectionMode.One)
                || grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended;

            // Grid in list box mode.
            if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                || (grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended
                && grid.Model.Options.MulitExtendedArrowKeySelect))
            {
                GridRangeInfo activeRange = selectedRanges.ActiveRange;

                if (grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    bool horizontal = direction == GridDirectionType.Left || direction == GridDirectionType.Right
                        || direction == GridDirectionType.MostLeft || direction == GridDirectionType.MostRight;

                    if (horizontal)
                    {
                        return grid.CurrentCell.InternalMove(direction, num, GridSetCurrentCellOptions.ScrollInView);
                    }

                    if (selectedRanges.Count == 0 || !extendSelection)
                    {
                        GridRangeInfo emptyRange = GridRangeInfo.Empty;
                        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.ArrowKey))
                        {
                            grid.Selections.Clear();
                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.ArrowKey);
                        }
                        else
                        {
                            return false;
                        }

                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                    }

                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey))
                    {
                        // Move Edit and set selection to new row.
                        grid.CurrentCell.InternalMove(direction, num, GridSetCurrentCellOptions.None);
                        grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex);
                        GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex, rowIndex);
                        grid.Selections.ChangeSelection(selectedRanges.ActiveRange, newRange);
                        this.Mrci.mouseMoveRowIndex = rowIndex;
                        this.Mrci.mouseMoveColIndex = colIndex;

                        // Fire event.
                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);
                        grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);

                        if (!extendSelection)
                        {
                            this.Mrci.mouseDownRowIndex = rowIndex;
                            this.Mrci.mouseDownColIndex = colIndex;
                        }

                         return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }            
            else if (extendSelection)
            {
                // Regular grid.
                if (grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    // Delete previous selection.
                    if (selectedRanges.Count == 0)
                    {
                        GridRangeInfo emptyRange = GridRangeInfo.Empty;
                        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.ArrowKey))
                        {
                            grid.Selections.Clear();
                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.ArrowKey);
                        }
                        else
                        {
                            return false;
                        }

                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                    }

                    ////grid.SelectionFrameChanging();
                    grid.BeginUpdate(BeginUpdateOptions.Invalidate | BeginUpdateOptions.ScrollWindow | BeginUpdateOptions.SynchronizeScrollBars);
                    bool endupdate = false;
                    try
                    {
                        GridRangeInfo activeRange = selectedRanges.ActiveRange;
                        if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey))
                        {
                            grid.CurrentCell.InternalMove(direction, num, GridSetCurrentCellOptions.NoSelectRange);
                            grid.EndUpdate();
                            endupdate = true;

                            grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex);

                            GridRangeInfo savedRange = selectedRanges.ActiveRange;

                            if (savedRange.IsEmpty)
                            {
                                int rowIndex2 = this.Mrci.mouseDownRowIndex;
                                int colIndex2 = this.Mrci.mouseDownColIndex;

                                if (rowIndex2 > grid.InternalGetHeaderRows() && colIndex2 > grid.InternalGetHeaderCols())
                                {
                                    Grid.Model.CoveredRanges.Find(rowIndex2, colIndex2, out savedRange);
                                }
                                else if (rowIndex2 > grid.InternalGetHeaderRows())
                                {
                                    savedRange = GridRangeInfo.Row(rowIndex2);
                                }
                                else if (colIndex2 > grid.InternalGetHeaderCols())
                                {
                                    savedRange = GridRangeInfo.Col(colIndex2);
                                }
                                else
                                {
                                    savedRange = GridRangeInfo.Empty;
                                }
                            }

                            GridRangeInfo newRange;
                            if (savedRange.IsRows)
                            {
                                newRange = GridRangeInfo.Rows(this.Mrci.mouseDownRowIndex, rowIndex);
                            }
                            else if (savedRange.IsCols)
                            {
                                newRange = GridRangeInfo.Cols(this.Mrci.mouseDownColIndex, colIndex);
                            }
                            else
                            {
                                newRange = GridRangeInfo.Cells(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, rowIndex, colIndex);
                            }

                            if ((newRange.IsTable && (grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                                || (newRange.IsRows && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                                || (newRange.IsCols && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                                || (newRange.IsCells && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                            {
                                if (RaiseSelectionChanging(ref newRange, GridSelectionReason.ArrowKey))
                                {
                                    grid.Selections.ChangeSelection(selectedRanges.ActiveRange, newRange);
                                }
                            }

                            this.Mrci.mouseMoveRowIndex = rowIndex;
                            this.Mrci.mouseMoveColIndex = colIndex;

                            // Notify derived classes.
                            RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);
                            grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        if (!endupdate)
                        {
                            grid.EndUpdate();
                        }
                    }
                }
            }
            else 
            {
                // No selection.
                GridSetCurrentCellOptions options = GridSetCurrentCellOptions.ScrollInView;
                if (!grid.Model.Options.ExcelLikeCurrentCell)
                {
                    options |= GridSetCurrentCellOptions.NoSelectRange;
                }

                grid.CurrentCell.InternalMove(direction, num, options);

                if (grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    this.Mrci.mouseDownRowIndex = rowIndex;
                    this.Mrci.mouseDownColIndex = colIndex;
                }

                return true;
            }

            return false;
        }

        void BeginSelectCells(int rowIndex, int colIndex, Keys modifierKeys)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, modifierKeys, this);
            }
#else
            ;
#endif

            GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

            if (rowIndex == 0 && colIndex == 0)
            {
                // Toggle table selection.
                if ((grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                {
                    if (selectedRanges.AnyRangeContains(GridRangeInfo.Cell(0, 0)))
                    {
                        GridRangeInfo emptyRange = GridRangeInfo.Empty;
                        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.MouseDown, GridRangeInfo.Cell(0, 0)))
                        {
                            grid.Selections.Clear();
                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.MouseDown);
                        }

                        this.Mrci.mouseClickRowIndex = grid.TopRowIndex;
                        this.Mrci.mouseClickColIndex = grid.LeftColIndex;
                        this.Mrci.mouseDownRowIndex = grid.TopRowIndex;
                        this.Mrci.mouseDownColIndex = grid.LeftColIndex;
                        this.Mrci.mouseMoveRowIndex = Mrci.mouseDownRowIndex;
                        this.Mrci.mouseMoveColIndex = Mrci.mouseDownColIndex;
                        //// ... Selections.SelectRange will trigger Selections.OnSelectionChanged event

                        return;
                    }
                }
            }

            int nhRow = grid.InternalGetHeaderRows();
            int nhCol = grid.InternalGetHeaderCols();
            bool bCol = rowIndex <= 0 && colIndex > nhCol;
            bool bRow = colIndex <= 0 && rowIndex > nhRow;
            bool bCell = rowIndex > nhRow && colIndex > nhCol;
            bool bTable = colIndex == 0 && rowIndex == 0;

            if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.MultiExtended)
            {
                addNewSelection = (modifierKeys & Keys.Control) != Keys.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & Keys.Shift) != Keys.None;
                if (rowIndex <= nhRow)
                {
                    return;
                }
            }
            else if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.MultiSimple)
            {
                addNewSelection = true;
                if (rowIndex <= nhRow)
                {
                    return;
                }
            }
            else if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.One)
            {
                addNewSelection = extendSelection = false;
                if (rowIndex <= nhRow)
                {
                    return;
                }
            }
            else
            {
                addNewSelection = (modifierKeys & Keys.Control) != Keys.None && (grid.Model.Options.AllowSelection & GridSelectionFlags.Multiple) != GridSelectionFlags.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & Keys.Shift) != Keys.None && (grid.Model.Options.AllowSelection & GridSelectionFlags.Shift) != GridSelectionFlags.None;
            }

            Mrci.firstClick = false;

            if (grid.Model.Options.AllowSelection != GridSelectionFlags.None)
            {
                int ncRow, ncCol;

                GridRangeInfo clickRange = GridRangeInfo.Empty;
                if (rowIndex == 0 && colIndex == 0)
                {
                    clickRange = GridRangeInfo.Table();
                }
                else if (colIndex <= 0 || grid.Model.Options.ListBoxSelectionMode != SelectionMode.None)
                { 
                    // REVIEW: need check for GridSelectionFlags.Rows?
                    clickRange = GridRangeInfo.Row(rowIndex);
                }
                else if (rowIndex <= 0)
                {
                    clickRange = GridRangeInfo.Col(colIndex);
                }
                else
                {
                    clickRange = GridRangeInfo.Cell(rowIndex, colIndex);
                }

                GridRangeInfo tempRange;
                Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out tempRange);

                bool resetSelection = false;
                if ((selectedRanges.Count == 1 && selectedRanges[0] != tempRange)
                    || selectedRanges.Count > 1)
                {
                    resetSelection = (!extendSelection && !addNewSelection)
                         || ((grid.Model.Options.AllowSelection & GridSelectionFlags.MixRangeType) == 0 && selectedRanges.FilterRangeType(clickRange.RangeType).Count != selectedRanges.Count);
                }

                // First, remove all selections if neither SHIFT or CTRL is pressed
                // or if the clicked range type differs from previous selected ranges.
                if (resetSelection)
                {
                    extendSelection = addNewSelection = false;
                    GridRangeInfo emptyRange = GridRangeInfo.Empty;
                    if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.MouseDown, clickRange))
                    {
                        grid.Selections.Clear();
                        RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.MouseDown);
                    }
                    else
                    {
                        return;
                    }
                }

                // Ctrl-Key
                if (addNewSelection)
                {
                    // If user clicks on a selected row or column header, deselect
                    // the row or column.
                    if (!clickRange.IsCells && selectedRanges.AnyRangeContains(clickRange))
                    {
                        grid.Selections.Remove(clickRange);
#if DEBUG
                        Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, selectedRanges.ToString());
#endif

                        this.Mrci.mouseClickRowIndex = clickRange.Top;
                        this.Mrci.mouseClickColIndex = clickRange.Left;
                        this.Mrci.mouseDownRowIndex = clickRange.Top;
                        this.Mrci.mouseDownColIndex = clickRange.Left;
                        this.Mrci.mouseMoveRowIndex = clickRange.Top;
                        this.Mrci.mouseMoveColIndex = clickRange.Left;
                        return;
                    }

                    // If user presses CTRL-Key while no range was selected
                    // but the current cell was visible, select the current
                    // cell.
                    if (!resetSelection &&
                        !extendSelection && selectedRanges.Count == 0 && grid.CurrentCell.GetCurrentCell(out ncRow, out ncCol) &&
                        !grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && grid.Model.Options.ListBoxSelectionMode == SelectionMode.None)
                    {
                        GridRangeInfo range = GridRangeInfo.Cell(ncRow, ncCol);
                        if (ncRow > nhRow && ncCol > nhCol)
                        {
                            if (RaiseSelectionChanging(ref range, GridSelectionReason.MouseDown, clickRange))
                            {
                                grid.Selections.ChangeSelection(GridRangeInfo.Empty, range);
                                RaiseSelectionChanged(range, GridSelectionReason.MouseDown);
                            }
                            else
                            { 
                                // Current cell can't be selected. Continue as if CTRL is not pressed.
                                addNewSelection = false;
                            }
                        }
                    }
                }

                // Check, if selection is allowed.
                if (((bTable || clickRange.IsTable) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                    || ((bRow || clickRange.IsRows) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                    || ((bCol || clickRange.IsCols) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                    || ((bCell || clickRange.IsCells) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                {
                    // Add a new empty range to selected range list. Will be initialized below.
                    if (addNewSelection)
                    {
                        selectedRanges.Add(GridRangeInfo.Empty);
                    }
                    // SH 1/4/03: selectedRanges.Add(GridRangeInfo.Cell(int.MaxValue, int.MaxValue));

                    // Start selection.

                    // If user presses <SHIFT>, extend last range.
                    if (extendSelection && selectedRanges.Count == 0)
                    {
                        // Select cells starting from current cell.
                        //                        grid.CurrentCell.GetCurrentCell(out ncRowIndex, out ncColIndex);
                        //
                        //                        this.Mrci.mouseDownRowIndex = ncRowIndex;
                        //                        this.Mrci.mouseDownColIndex = ncColIndex;
                    }
                    else if (!extendSelection)
                    {
                        this.Mrci.mouseClickRowIndex = rowIndex;
                        this.Mrci.mouseClickColIndex = colIndex;
                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                    }

#if DEBUG
                    Trace.WriteLineIf(extendSelection && Switches.SelectRange.TraceVerbose, "ClickRange: " + clickRange.ToString());
#endif
                    GridRangeInfo savedRange = selectedRanges.ActiveRange;
                    GridRangeInfo newRange = clickRange.UnionRange(GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex));

                    if ((!grid.Model.Options.ExcelLikeSelectionFrame || resetSelection)
                        && !addNewSelection &&
                        newRange.IsCells && newRange.Width == 1 && newRange.Height == 1)
                    {
                        newRange = GridRangeInfo.Empty;
                    }

                    // Allow programmer to change the range.
                    if (RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseDown, newRange))
                    {
                        grid.Selections.ChangeSelection(savedRange, newRange);

                        // Trigger Selections.OnSelectionChanged event.
                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseDown);
                    }
                    else
                    {
                        selectedRanges.RemoveEmptyRanges();
                        return;
                    }

                    if (grid.Model.Options.ListBoxSelectionMode != System.Windows.Forms.SelectionMode.MultiSimple)
                    {
                        inSelectingCells = true;
                    }
                }
            }
            if (this.Grid != null && this.Grid.Model != null && this.Grid.Model.activeGridView != null && (grid.Model.Properties.MarkRowHeader || grid.Model.Properties.MarkColHeader))
            {
                this.Grid.Model.activeGridView.BeginUpdate();
                this.Grid.Model.activeGridView.UpdateStyles();
                this.Grid.Model.activeGridView.EndUpdate(true);
            }
        }

        bool multiExtendedShouldMoveCurrentCell = true;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether MultiExtended Should Move CurrentCell. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool MultiExtendedShouldMoveCurrentCell
        {
            get
            {
                return multiExtendedShouldMoveCurrentCell;
            }

            set
            {
                multiExtendedShouldMoveCurrentCell = value;
            }
        }

        bool ChangeSelectCells(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {  
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, this); 
            }
#else
               
            ;
#endif
            if (grid.Model.Options.AllowSelection != GridSelectionFlags.None // selecting cells enabled?
                && inSelectingCells)
            {
                GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

                int nhRow = grid.InternalGetHeaderRows();
                int nhCol = grid.InternalGetHeaderCols();
                int nfRow = grid.InternalGetFrozenRows();
                int nfCol = grid.InternalGetFrozenCols();

                GridRangeInfo savedRange = selectedRanges.ActiveRange;
                GridRangeInfo newRange;

                rowIndex = Math.Max(nhRow + 1, rowIndex);
                if (this.Mrci.mouseDownRowIndex > nfRow && grid.TopRowIndex - 1 > nfRow)
                {
                    rowIndex = Math.Max(grid.TopRowIndex - 1, rowIndex);
                }

                colIndex = Math.Max(nhCol + 1, colIndex);
                if (this.Mrci.mouseDownColIndex > nfCol && grid.LeftColIndex - 1 > nfCol)
                {
                    colIndex = Math.Max(grid.LeftColIndex - 1, colIndex);
                }

                if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.One)
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                }
                else if (savedRange.IsCols)
                {
                    newRange = GridRangeInfo.Cols(colIndex, this.Mrci.mouseDownColIndex);
                }
                else if (savedRange.IsRows)
                {
                    newRange = GridRangeInfo.Rows(rowIndex, this.Mrci.mouseDownRowIndex);
                }
                else
                {
                    if (rowIndex != this.Mrci.mouseDownRowIndex
                        || colIndex != this.Mrci.mouseDownColIndex
                        || grid.Model.Options.ExcelLikeSelectionFrame)
                    {
                        newRange = GridRangeInfo.Cells(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, rowIndex, colIndex);
                    }
                    else
                    {
                        newRange = GridRangeInfo.Empty;
                    }
                }

                bool endUpdate = false;

                if ((grid.Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) != 0)
                {
                    grid.BeginUpdate(BeginUpdateOptions.Invalidate);
                    endUpdate = true;
                }

                try
                {
                    // Allow programmer to change the range.
                    if (RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseMove, newRange))
                    {
                        grid.Selections.ChangeSelection(selectedRanges.ActiveRange, newRange);

                        if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                            || (grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended && multiExtendedShouldMoveCurrentCell))
                        {
                            grid.CurrentCell.MoveTo(rowIndex, grid.CurrentCell.ColIndex, GridSetCurrentCellOptions.NoSelectRange | GridSetCurrentCellOptions.NoSetFocus | GridSetCurrentCellOptions.NoSyncCurrentCell);
                        }

                        //// Trigger Selections.OnSelectionChanged event.
                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseMove);
                    }
                }
                finally
                {
                    if (endUpdate)
                    {
                        grid.EndUpdate(true);
                    }
                }

                return true;
            }

            return false;
        }

        void EndSelectCells()
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif

            GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

            grid.CurrentCell.AdjustRowColIfCoveredCell(ref Mrci.mouseMoveRowIndex, ref Mrci.mouseMoveColIndex);

            if (grid.Model.Options.AllowSelection != GridSelectionFlags.None
                && inSelectingCells
                && (this.Mrci.mouseDownColIndex != 0 || this.Mrci.mouseDownRowIndex != 0))
            {
                if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.None
                    && this.Mrci.mouseDownColIndex == this.Mrci.mouseMoveColIndex
                    && this.Mrci.mouseDownRowIndex == this.Mrci.mouseMoveRowIndex
                    && this.Mrci.mouseDownRowIndex > 0 && this.Mrci.mouseDownColIndex > 0)
                {
                    if (addNewSelection)
                    {
                        // Allow programmer to change the range.
                        GridRangeInfo range = GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex);
                        if (selectedRanges.Count > 0 && RaiseSelectionChanging(ref range, GridSelectionReason.MouseUp))
                        {
                            // User presses ctrl-key. Select the current cell.
                            grid.Selections.ChangeSelection(selectedRanges.ActiveRange, range);

                            // Trigger Selections.OnSelectionChanged event.
                            RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseUp);
                        }
                    }
                    else
                    {
                        // User did move the current cell but didn't select any cells.
                        if (!grid.Model.Options.ExcelLikeCurrentCell)
                        {
                            grid.Selections.Clear();
                        }
                    }
                }
                else
                {
                    // Trigger Selections.OnSelectionChanged event to let user know that selection.
                    // ended.
                    RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseUp);
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void ProcessSetCurrentCell(int rowIndex, int colIndex, GridSetCurrentCellOptions flags)
        {
            if ((flags & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.NoSelectRange)
            {
                return;
            }
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, flags, this);
            }
#else
            ;
#endif

            if (grid.Model.Options.ExcelLikeCurrentCell)
            {
                if (rowIndex == -1 || colIndex == -1 || !grid.Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    GridRangeInfo newRange;
                    if (rowIndex > grid.InternalGetHeaderRows() && colIndex > grid.InternalGetHeaderCols())
                    {
                        Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out newRange);
                    }
                    else if (rowIndex > grid.InternalGetHeaderRows())
                    {
                        newRange = GridRangeInfo.Row(rowIndex);
                    }
                    else if (colIndex > grid.InternalGetHeaderCols())
                    {
                        newRange = GridRangeInfo.Col(colIndex);
                    }
                    else
                    {
                        newRange = GridRangeInfo.Empty;
                    }

                    // Check, if selection is allowed.
                    if ((newRange.IsTable && (grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                        || (newRange.IsRows && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                        || (newRange.IsCols && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                        || (newRange.IsCells && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                    {
                        if (RaiseSelectionChanging(ref newRange, GridSelectionReason.SetCurrentCell, newRange))
                        {
                            grid.Selections.Clear();
                            grid.Selections.ChangeSelection(GridRangeInfo.Empty, newRange);
                            RaiseSelectionChanged(newRange, GridSelectionReason.SetCurrentCell);
                            if (rowIndex >= 0 && colIndex >= 0)
                            {
                                this.Mrci.mouseDownRowIndex = rowIndex;
                                this.Mrci.mouseDownColIndex = colIndex;
                                this.Mrci.mouseClickRowIndex = rowIndex;
                                this.Mrci.mouseClickColIndex = colIndex;
                            }
                        }
                    }
                }
            }
            else
            {
                if (grid.Model.SelectedRanges.Count == 0)
                {
                    if (rowIndex >= 0 && colIndex >= 0)
                    {
                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                        this.Mrci.mouseClickRowIndex = rowIndex;
                        this.Mrci.mouseClickColIndex = colIndex;
                    }
                }

                // Sync list box mode current row selection with current cell row index.
                if (!inSelectingCells
                    && rowIndex >= grid.InternalGetHeaderRows()
                    && (flags & GridSetCurrentCellOptions.NoSelectRange) == 0
                    && grid.Model.Options.ListBoxSelectionMode != System.Windows.Forms.SelectionMode.None
                    && !grid.Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    GridRangeInfo activeRange = GridRangeInfo.Empty;
                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.SetCurrentCell))
                    {
                        grid.Selections.Clear();
                        RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
                    }

                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.SetCurrentCell))
                    {
                        // Move Edit and set selection to new row.
                        GridRangeInfo newRange = GridRangeInfo.Row(rowIndex);
                        grid.Selections.ChangeSelection(activeRange, newRange);

                        // Fire event.
                        RaiseSelectionChanged(newRange, GridSelectionReason.ArrowKey);
                    }
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class SelectCellsHitTestInfo
        {
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;
            internal Rectangle cellBounds = Rectangle.Empty;
            internal int clientCol;
            internal int clientRow;
            internal int rowIndex;
            internal int colIndex;
            internal GridCellRendererBase cellRenderer = null;
            internal bool selectCells;

            public override string ToString()
            {
                return String.Format("{0}:{1},{2}", hitTestResult, rowIndex, colIndex);
            }

            internal SelectCellsHitTestInfo(GridControlBase grid, MouseEventArgs e, IMouseController controller)
            {
                this.point = new Point(e.X, e.Y);
                clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                if (clientCol >= 0 && clientRow >= 0)
                {
                    rowIndex = grid.GetRow(clientRow);
                    colIndex = grid.GetCol(clientCol);

                    if (clientCol < grid.ViewLayout.VisibleCols && clientRow < grid.ViewLayout.VisibleRows
                        && rowIndex <= grid.Model.RowCount && colIndex <= grid.Model.ColCount)
                    {
                        GridRangeInfo spannedRange;
                        if (grid.Model.CoveredRanges.Find(rowIndex, colIndex, out spannedRange))
                        {
                            rowIndex = spannedRange.Top;
                            colIndex = spannedRange.Left;
                            cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                        }
                        else
                        {
                            cellBounds = new Rectangle(grid.ViewLayout.ClientRowColToPoint(clientRow, clientCol, GridCellSizeKind.VisibleSize), new Size(grid.GetColWidth(colIndex), grid.GetRowHeight(rowIndex)));
                        }

                        cellRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                        hitTestResult = cellRenderer.RaiseHitTest(rowIndex, colIndex, e, controller);
                        selectCells = false;

                        if (hitTestResult == 0 && grid.Model.Options.AllowSelection != GridSelectionFlags.None)
                        {
                            selectCells = true;
                            int nhRow = grid.InternalGetHeaderRows();
                            int nhCol = grid.InternalGetHeaderCols();
                            bool bCol = rowIndex <= nhCol && colIndex > nhCol;
                            bool bRow = colIndex <= nhRow && rowIndex > nhRow;
                            bool bCell = rowIndex > nhRow && colIndex > nhCol;

                            GridRangeInfo clickRange = GridRangeInfo.Empty;
                            if (colIndex <= nhCol || grid.Model.Options.ListBoxSelectionMode != SelectionMode.None)
                            {
                                // REVIEW: need check for GridSelectionFlags.Rows?
                                clickRange = GridRangeInfo.Row(rowIndex);
                            }
                            else if (rowIndex <= nhRow && grid.Model.Options.ListBoxSelectionMode != SelectionMode.None)
                            {
                                clickRange = GridRangeInfo.Col(colIndex);
                            }
                            else
                            {
                                clickRange = GridRangeInfo.Cell(rowIndex, colIndex);
                            }

                            if (((bRow || clickRange.IsRows) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                                || ((bCol || clickRange.IsCols) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                                || ((bCell || clickRange.IsCells) && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                            {
                                if (clientCol > grid.InternalGetHeaderCols() && clientRow > grid.InternalGetHeaderRows())
                                {
                                    hitTestResult = GridHitTestContext.Cell;
                                }
                                else
                                {
                                    hitTestResult = GridHitTestContext.Header;
                                }
                            }
                        }

                        if (hitTestResult == GridHitTestContext.None)
                        {
                            hitTestResult = GridHitTestContext.Cell;
                        }
                    }
                }
            }
        }

        /// <override/>
        /// <summary>
        /// The name of this mouse controller.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SelectCells";
            }
        }

        /// <override/>
        /// <summary>
        /// The cursor to be displayed.
        /// </summary>
        public override Cursor Cursor
        {
            get
            {
                if (hitTestInfo != null)
                {
                    if (hitTestInfo.selectCells)
                    {
                        if (hitTestInfo.hitTestResult == GridHitTestContext.Header
                            && hitTestInfo.rowIndex == 0 && hitTestInfo.colIndex > 0)
                        {
                            if ((grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                            {
                                return GridCursors.SelectColumnCursor;
                            }
                        }
                        else if (hitTestInfo.hitTestResult == GridHitTestContext.Header
                            && hitTestInfo.rowIndex > 0 && hitTestInfo.colIndex == 0)
                        {
                            if ((grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                            {
                                if (grid.IsRightToLeft())
                                {
                                    return GridCursors.SelectRowRTLCursor;
                                }
                                else
                                {
                                    return GridCursors.SelectRowCursor;
                                }
                            }
                        }
                    }

                    if (hitTestInfo.cellRenderer != null)
                    {
                        return hitTestInfo.cellRenderer.RaiseGetCursor(hitTestInfo.rowIndex, hitTestInfo.colIndex);
                    }
                }

                return null;
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        public override void MouseHoverEnter()
        {
            if (hitTestInfo.cellRenderer != null)
            {
                hitTestInfo.cellRenderer.RaiseMouseHoverEnter(hitTestInfo.rowIndex, hitTestInfo.colIndex);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseHover(MouseEventArgs e)
        {
#if DEBUG
            if (Switches.MouseController.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Button, e.X, e.Y, hitTestInfo.rowIndex, hitTestInfo.colIndex, hitTestInfo.cellRenderer);
            }
#else
            ;
#endif
            if (mouseHoverInfo != null)
            {
                if (mouseHoverInfo.rowIndex != hitTestInfo.rowIndex
                    || mouseHoverInfo.colIndex != hitTestInfo.colIndex
                    || mouseHoverInfo.cellRenderer != hitTestInfo.cellRenderer)
                {
                    if (mouseHoverInfo.cellRenderer != null)
                    {
                        mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(mouseHoverInfo.rowIndex, mouseHoverInfo.colIndex, e);
                    }

                    if (hitTestInfo != null && hitTestInfo.cellRenderer != null)
                    {
                        hitTestInfo.cellRenderer.RaiseMouseHoverEnter(hitTestInfo.rowIndex, hitTestInfo.colIndex);
                    }

                    mouseHoverInfo = this.hitTestInfo;
                }
            }
            else if (hitTestInfo != null && hitTestInfo.cellRenderer != null)
            {
                mouseHoverInfo = this.hitTestInfo;
                //WF-11951 - CellMouseHoverEnter event fired twice. Hence, removed.
               // hitTestInfo.cellRenderer.RaiseMouseHoverEnter(hitTestInfo.rowIndex, hitTestInfo.colIndex);
            }

            if (hitTestInfo != null && hitTestInfo.cellRenderer != null)
            {
                hitTestInfo.cellRenderer.RaiseMouseHover(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> holding event data.</param>
        public override void MouseHoverLeave(EventArgs e)
        {
            if (mouseHoverInfo != null)
            {
                if (mouseHoverInfo.cellRenderer != null)
                {
                    mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(mouseHoverInfo.rowIndex, mouseHoverInfo.colIndex, e);
                }

                mouseHoverInfo = null;
            }

            if (hitTestInfo != null)
            {
                hitTestInfo.cellRenderer = null;
            }
        }

        /// <override/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Button, this);
            }
#else
            ;
#endif
            HitTest(e, null);
            if (hitTestInfo != null && hitTestInfo.cellRenderer != null && !this.hitTestInfo.selectCells)
            {
                try
                {
                    ////SS grid.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                    if (!(hitTestInfo.cellRenderer is IGridWindowlessObject)
                        && !grid.RaiseMouseActivating())
                    {
                        return;
                    }

                    hitTestInfo.cellRenderer.RaiseMouseDown(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                    ////SS                    if (hitTestInfo.cellRenderer is GridOriginalTextBoxCellRenderer && Control.ModifierKeys == Keys.Shift)
                    ////SS                        HitTest(e, null);
                    return;
                }
                finally
                {
                    ////SS grid.EndUpdate(true);
                }
            }

            // if e.Button equals MouseButtons.None, the MouseControllerDispatcher checks if I want to handle MouseTracking
            // if e.Button equals MouseButtons.Left, the MouseControllerDispatcher checks if I want to handle Left click
            if (hitTestInfo != null &&
                (CheckMouseButtons(e) || e.Button == MouseButtons.None))
            {
                ignoreMouseMessages = false;
                Point point = hitTestInfo.point;
                int rowIndex = hitTestInfo.rowIndex;
                int colIndex = hitTestInfo.colIndex;
                grid.CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                this.canceled = false;

                if (e.Clicks == 2)
                {
                    GridCellRendererBase cellRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                    cellRenderer.RaiseDoubleClick(rowIndex, colIndex, e);
                    canceled = true;
                    return;
                }

                ////WL
                canceled = !grid.RaiseMouseActivating();

                if (!canceled)
                {
                    if (grid.Model.Options.ExcelLikeCurrentCell && !this.grid.Model[rowIndex, colIndex].Enabled)
                    {
                        if (grid.Model.Options.ClickedOnDisabledCellBehavior == GridClickedOnDisabledCellBehavior.LeaveCurrentCell
                            || grid.Model.Options.ClickedOnDisabledCellBehavior == GridClickedOnDisabledCellBehavior.Default)
                        {
                            return;
                        }

                        if (!this.grid.CurrentCell.Deactivate(false))
                        {
                            return;
                        }
                    }

                    BeginSelectCells(rowIndex, colIndex, Control.ModifierKeys);

                    GridSetCurrentCellOptions options;
                    if (grid.Model.Options.ExcelLikeCurrentCell && grid.Model.SelectedRanges.Count == 0)
                    {
                        options = GridSetCurrentCellOptions.None;
                    }
                    else
                    {
                        options = GridSetCurrentCellOptions.NoSelectRange;
                    }

                    grid.IsMousePressed = true;
                    if (grid.CurrentCell.MoveTo(rowIndex, colIndex, options | GridSetCurrentCellOptions.NoSetFocus)
                        && (Control.ModifierKeys & Keys.Shift) == 0)
                    {
                        Mrci.mouseDownRowIndex = rowIndex;
                        Mrci.mouseDownColIndex = colIndex;
                    }

                    this.Mrci.mouseClickRowIndex = rowIndex;
                    this.Mrci.mouseClickColIndex = colIndex;
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;

                    if (ignoreMouseMessages)
                    {
                        return;
                    }

                    int dy = 0;
                    int dx = 0;
                    Rectangle r = grid.GridBounds;

                    ScrollBars ab = ScrollBars.None;
                    if (e.Button != MouseButtons.None)
                    {
                        if (rowIndex > 0 && !grid.InternalIsFrozenRow(rowIndex))
                        {
                            dy = grid.ViewLayout.GetRowRangeHeight(0, grid.InternalGetFrozenRows(), GridCellSizeKind.VisibleSize);
                            ab |= ScrollBars.Vertical;
                        }

                        if (colIndex > 0 && !grid.InternalIsFrozenCol(colIndex))
                        {
                            dx = grid.ViewLayout.GetColRangeWidth(0, grid.InternalGetFrozenCols(), GridCellSizeKind.VisibleSize);
                            ab |= ScrollBars.Horizontal;
                        }
                    }

                    grid.AutoScrolling = ab;
                    grid.AutoScrollBounds = Rectangle.FromLTRB(dx, dy, r.Right, r.Bottom);
#if DEBUG
                    Trace.WriteLineIf(Switches.GridScrolling.TraceVerbose, "AutoScrollBounds: " + grid.AutoScrollBounds.ToString());
#endif
                }
            }

            scrolled = false;
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            if (canceled || scrolled && e.Button == MouseButtons.None)
            {
                if (scrolled)
                {
                    Rectangle r = Grid.ClientRectangle;
                    r.X = r.Right - SystemInformation.VerticalScrollBarWidth;
                    if (e.X > r.X)
                        scrolled = false;
                }
                return;
            }
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Button, this);
            }
#else
            ;
#endif
            if (grid.CurrentCell.StaticDrawing)
            {
                return;
            }

            if (hitTestInfo != null && hitTestInfo.cellRenderer != null && !this.hitTestInfo.selectCells)
            {
                hitTestInfo.cellRenderer.RaiseMouseMove(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                return;
            }

            GridRangeInfo rgCell = grid.PointToRangeInfo(new Point(e.X, e.Y), 0);
            int rowIndex = rgCell.Top;
            int colIndex = rgCell.Left;
            if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex || scrolled)
            {
                ChangeSelectCells(rowIndex, colIndex);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
            }

            scrolled = false;
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            if (hitTestInfo != null && hitTestInfo.cellRenderer != null && !this.hitTestInfo.selectCells)
            {
                hitTestInfo.cellRenderer.RaiseMouseUp(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                return;
            }

            if (canceled || this.disposed)
            {
                return;
            }
#if DEBUG

            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Button, this);
            }
#else

            ;
#endif
            EndSelectCells();
            grid.AutoScrolling = ScrollBars.None;
            inSelectingCells = false;

            GridRangeInfo rgCell = grid.PointToRangeInfo(new Point(e.X, e.Y), 0);
            int rowIndex = rgCell.Top;
            int colIndex = rgCell.Left;

            grid.CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
            if (rowIndex == Mrci.mouseClickRowIndex && colIndex == Mrci.mouseClickColIndex)
            {
                GridRangeInfo newRange = GridRangeInfo.Empty;
                if (rowIndex > grid.InternalGetHeaderRows() && colIndex > grid.InternalGetHeaderCols())
                {
                    Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out newRange);
                }
                else if (rowIndex > grid.InternalGetHeaderRows())
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                }
                else if (colIndex > grid.InternalGetHeaderCols())
                {
                    newRange = GridRangeInfo.Col(colIndex);
                }
                else
                {
                    newRange = GridRangeInfo.Empty;
                }

                if ((newRange.IsTable && (grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                    || (newRange.IsRows && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                    || (newRange.IsCols && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                    || (newRange.IsCells && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                {
                    if (grid.Model.Options.ExcelLikeCurrentCell && !grid.Model.Selections.Ranges.AnyRangeContains(newRange))
                    {
                        grid.Model.Selections.Add(newRange);
                    }
                }

                GridCellRendererBase cellRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                cellRenderer.RaiseClick(rowIndex, colIndex, e);
            }
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            if (hitTestInfo != null && hitTestInfo.cellRenderer != null && !this.hitTestInfo.selectCells)
            {
                hitTestInfo.cellRenderer.RaiseCancelMode(hitTestInfo.rowIndex, hitTestInfo.colIndex);
                return;
            }

            inSelectingCells = false;
            canceled = true;
        }

        bool CheckMouseButtons(MouseEventArgs e)
        {
            return (e.Button == MouseButtons.None && grid.Model.Options.SelectCellsMouseButtonsMask != MouseButtons.None)
                || (e.Button & grid.Model.Options.SelectCellsMouseButtonsMask) != MouseButtons.None;
        }

        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data..</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            if (this.grid.IsDesignMode() && e.Button != MouseButtons.Left)
            {
                return 0;
            }

            // This HitTest code has higher priority than "".
            hitTestInfo = null;
            Point pt = new Point(e.X, e.Y);
            // if e.Button equals MouseButtons.None, the MouseControllerDispatcher checks if I want to handle MouseTracking
            // if e.Button equals MouseButtons.Left, the MouseControllerDispatcher checks if I want to handle Left click
            if (CheckMouseButtons(e) &&
                (controller == null || GridUtil.IsEmpty(controller.Name)))
            {
                hitTestInfo = new SelectCellsHitTestInfo(grid, e, controller);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                {
                    hitTestInfo = null;
                }
            }

            return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
        }

        private void grid_CancelMode(object sender, EventArgs e)
        {
            this.mouseHoverInfo = null;
            this.hitTestInfo = null;
        }
    }
}
