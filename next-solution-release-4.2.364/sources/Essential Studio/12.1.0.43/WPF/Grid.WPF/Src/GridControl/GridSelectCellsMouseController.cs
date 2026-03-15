#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the mouse controller for selecting grid cells.
    /// </summary>
    public class GridSelectCellsMouseController : IMouseController, IDisposable
    {
        #region Fields
        internal bool inSelectingCells;
        bool addNewSelection;
        bool extendSelection;
        bool canceled;
        bool multiExtendedShouldMoveCurrentCell = true;
        MouseRowColInfo mrci = null;
        GridControlBase grid;
        RowColumnIndex start = RowColumnIndex.Empty;
        SuspendState suspendState;
        bool moveToSuccess = true;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new <see cref="GridSelectCellsMouseController"/>.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridSelectCellsMouseController(GridControlBase grid)
        {
            this.grid = grid;
            grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            grid.CurrentCellActivated += new GridRoutedEventHandler(GridCurrentCellActivated);
            grid.ExternalMove = new GridCurrentCellMoveDelegateHandler(GridCurrentCellExternalMove);
            grid.AutoScroller.AutoScrollerValueChanged += AutoScrollerValueChanged;            
        }

        protected virtual void AutoScrollerValueChanged(object sender, AutoScrollerValueChangedEventArgs args)
        {
            if (grid != null)
            {
                int rowIndex = Mrci.mouseMoveRowIndex;
                int colIndex = Mrci.mouseMoveColIndex;
            

            bool changed = false;
            if (args.IsLineDown)
            {
                changed = true;
                rowIndex = grid.ScrollRows.LastBodyVisibleLineIndex;
            }

            if (args.IsLineUp)
            {
                changed = true;
                rowIndex = grid.ScrollRows.ScrollLineIndex;
            }

            if (args.IsLineLeft)
            {
                changed = true;
                colIndex = grid.ScrollColumns.ScrollLineIndex;
            }

            if (args.IsLineRight)
            {
                changed = true;
                colIndex = grid.ScrollColumns.LastBodyVisibleLineIndex;
            }

            if (!changed)
                return;

            if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex)
            {
                ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseMove);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
            }

            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases all the resources used by this component.
        /// </summary>
        public void Dispose()
        {
            grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            grid.CurrentCellActivated -= new GridRoutedEventHandler(GridCurrentCellActivated);
            grid.AutoScroller.AutoScrollerValueChanged -= AutoScrollerValueChanged;
            grid.ExternalMove = null;
            this.grid = null;
        }

        #endregion

        #region Properties
        MouseRowColInfo Mrci
        {
            get
            {
                if (mrci == null)
                {
                    if (grid.Model.UserData.Contains("MouseRowColInfo"))
                        mrci = (MouseRowColInfo)grid.Model.UserData["MouseRowColInfo"];
                    else
                        grid.Model.UserData["MouseRowColInfo"] = mrci = new MouseRowColInfo();
                }
                return mrci;
            }
        }

        GridControlBase Grid
        {
            get { return grid; }
        }

        protected GridCurrentCell CurrentCell
        {
            get { return grid.CurrentCell; }
        }

        protected GridModel Model
        {
            get { return grid.Model; }
        }

        GridModelOptions GridOptions
        {
            get { return grid.Model.Options; }
        }

        protected MouseControllerDispatcher MouseControllerDispatcher
        {
            get { return grid.MouseControllerDispatcher; }
        }

        GridModelSelections Selections
        {
            get { return grid.Model.Selections; }
        }

        GridRangeInfoList SelectedRanges
        {
            get { return grid.Model.SelectedRanges; }
        }

        /// <summary>
        /// Specifies whether the current cell should be moved when doing MultiExtended selection.
        /// </summary>
        /// <value>
        /// <c>True if the current cell should be moved; false otherwise.</c>
        /// </value>
        public bool MultiExtendedShouldMoveCurrentCell
        {
            get { return multiExtendedShouldMoveCurrentCell; }
            set { multiExtendedShouldMoveCurrentCell = value; }
        }
        #endregion

        #region ToString
        /// <summary>
        /// Returns the equivalent string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString()
        {
            return "GSM(" +
                (inSelectingCells ? "INS " : "")
                + (addNewSelection ? "ADD " : "")
                + (extendSelection ? "EXT " : "")
                + " ) " + Mrci.ToString() + " "
                + SelectedRanges.ToString() + " "
                + grid.ToString();
        }
        #endregion

        #region Grid Event Handlers

        void GridCurrentCellMoveComplete(object sender, GridCurrentCellMovedEventArgs e)
        {

        //if (MouseControllerDispatcher.InMouseDown)
        //    return;
      
            if (!(CurrentCell.MoveFromActiveState && CurrentCell.IsInMoveTo
                && CurrentCell.HasCurrentCellAt(CurrentCell.MoveFromRowIndex, CurrentCell.MoveFromColIndex))
                )
                ProcessSetCurrentCell(CurrentCell.RowIndex, CurrentCell.ColumnIndex, e.Options);
        }

        void GridCurrentCellActivated(object sender, SyncfusionRoutedEventArgs e)
        {
            if ((!CurrentCell.IsInMoveTo) || CurrentCell.ActivateOptions.IsExternalMove)
                ProcessSetCurrentCell(CurrentCell.RowIndex, CurrentCell.ColumnIndex, GridActivateCurrentCellOptions.Empty);
        }

        protected virtual void ProcessSetCurrentCell(int rowIndex, int colIndex, GridActivateCurrentCellOptions flags)
        {
            if (inSelectingCells)
                return;
            //WPF9434-Selection issue in GridDataControl with Shift +Tab Key
            if ((Keyboard.Modifiers == ModifierKeys.Shift &&
                 !(Keyboard.Modifiers == ModifierKeys.Shift && Keyboard.IsKeyDown(Key.Tab))) ||
                (Keyboard.Modifiers == ModifierKeys.Control &&
                 (grid.Model.HeaderRows > 0 && this.Mrci.mouseClickRowIndex < grid.Model.HeaderRows)))
            {
                if (!CurrentCell.ActivateOptions.IsExternalMove)
                    return;
            }
            if (((flags.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.NoSelectRange))
                return;
#if DEBUG
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, flags, this);
            }
#endif

            if (GridOptions.ExcelLikeCurrentCell && GridOptions.ListBoxSelectionMode == GridSelectionMode.None)
            {
                if (rowIndex == -1 || colIndex == -1 || !SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    GridRangeInfo newRange;
                    if (rowIndex >= grid.InternalGetHeaderRows() && colIndex >= grid.InternalGetHeaderCols()&& GridOptions.AllowSelection!=GridSelectionFlags.Row)
                        Model.CoveredRanges.Find(rowIndex, colIndex, out newRange);
                    else if (rowIndex >= grid.InternalGetHeaderRows())
                        newRange = GridRangeInfo.Row(rowIndex);
                    else if (colIndex >= grid.InternalGetHeaderCols())
                        newRange = GridRangeInfo.Col(colIndex);
                    else
                        newRange = GridRangeInfo.Empty;

                    // Check, if selection is allowed.
                    if (
                        newRange.IsTable && (GridOptions.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None
                        || newRange.IsRows && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                        || newRange.IsCols && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                        || newRange.IsCells && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None)
                    {
                        if (RaiseSelectionChanging(ref newRange, GridSelectionReason.SetCurrentCell, newRange))
                        {
                            Selections.Clear(false);
                            Selections.ChangeSelection(GridRangeInfo.Empty, newRange, GridSelectionReason.Clear);
                            if (!(this.CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                                RaiseSelectionChanged(newRange, GridSelectionReason.SetCurrentCell);
                            if (rowIndex >= 0 && colIndex >= 0)
                                SetMrciMouseDownClick(rowIndex, colIndex);
                        }
                    }
                }
            }
            else
            {
                if (SelectedRanges.Count == 0)
                {
                    if (rowIndex >= 0 && colIndex >= 0)
                        SetMrciMouseDownClick(rowIndex, colIndex);
                }

                // Sync list box mode current row selection with current cell row index.
                if (!inSelectingCells
                    && rowIndex >= grid.InternalGetHeaderRows()
                    && (flags.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == 0
                    && GridOptions.ListBoxSelectionMode != GridSelectionMode.None 
                    && !SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    GridRangeInfo activeRange = GridRangeInfo.Empty;
                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.SetCurrentCell))
                    {
                        Selections.Clear(false);
                        if (!(CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
                    }
                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.SetCurrentCell))
                    {
                        // Move Edit and set selection to new row.
                        GridRangeInfo newRange = GridRangeInfo.Row(rowIndex);
                        this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
                        Selections.ChangeSelection(activeRange, newRange, GridSelectionReason.SetCurrentCell);
                        if (!(CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                            RaiseSelectionChanged(newRange, GridSelectionReason.ArrowKey);
                    }
                }
            }

        }

        private void SetMrciMouseDownClick(int rowIndex, int colIndex)
        {
            this.Mrci.mouseDownRowIndex = rowIndex;
            this.Mrci.mouseDownColIndex = colIndex;
            this.Mrci.mouseClickRowIndex = rowIndex;
            this.Mrci.mouseClickColIndex = colIndex;
        }

        #endregion

        #region RaiseSelectionChange(ing)

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason)
        {
            return RaiseSelectionChanging(ref range, reason, GridRangeInfo.Empty);
        }

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason, GridRangeInfo clickRange)
        {
            GridSelectionChangingEventArgs e = new GridSelectionChangingEventArgs(range, reason, clickRange);
            grid.Model.RaiseSelectionChanging(e);
            range = e.Range;
            if(this.Grid.FindParentElementOfType<GridTreeControl>() == null)
                canceled = e.Cancel;
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
        #endregion

        #region Keyboard Navigation

        protected virtual bool GridCurrentCellExternalMove(GridDirectionType direction, int num, bool extendSelection)
         {
            // Move current cell and select cells if user pressed SHIFT- or CONTROL-Key.
            int rowIndex = 0;
            int colIndex = 0;
            GridRangeInfoList selectedRanges = SelectedRanges;
            //WPF9434-Selection issue in GridDataControl with Shift +Tab Key
            extendSelection &= ((GridOptions.AllowSelection & GridSelectionFlags.Keyboard) != GridSelectionFlags.None
                                || (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None)
                                 && GridOptions.ListBoxSelectionMode != GridSelectionMode.One
                                || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended;

            // Grid in list box mode.
            if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
            {
                if (GridOptions.MulitExtendedArrowKeySelect)
                {
                    GridRangeInfo activeRange = selectedRanges.ActiveRange;

                    if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                    {
                        bool horizontal = direction == GridDirectionType.Left || direction == GridDirectionType.Right
                            || direction == GridDirectionType.MostLeft || direction == GridDirectionType.MostRight;

                        GridRangeInfo moveRange = activeRange;
                        int rIndex = rowIndex;
                        int cIndex = colIndex;
                        if (direction == GridDirectionType.Left)
                            cIndex--;
                        else if (direction == GridDirectionType.Right)
                            cIndex++;
                        else if (direction == GridDirectionType.Up)
                            rIndex--;
                        else if (direction == GridDirectionType.Down)
                            rIndex++;
                        if (this.Grid.GetNextCurrentCellPosition(direction, ref rIndex, ref cIndex))
                        {
                            moveRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rIndex, rIndex);
                            if (this.AdjustedRangeFunc != null)
                                moveRange = this.AdjustedRangeFunc(SelectionType.IsRow, this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, moveRange);
                        }

                        if (horizontal)
                        {
                            if (rowIndex != moveRange.Top)
                            {
                                if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey, moveRange))
                                {
                                    inSelectingCells = true;
                                    CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                                    bool success = CurrentCell.GetCurrentCell(out rowIndex, out colIndex);
                                    if (CurrentCell.Renderer != null && !(CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                                    {
                                        GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex, rowIndex);
                                        //if (this.AdjustedRangeFunc != null)
                                        //    newRange = this.AdjustedRangeFunc(SelectionType.IsRow, this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, newRange);
                                        GridRangeInfo oldRange = selectedRanges.ActiveRange;
                                        Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, GridSelectionReason.ArrowKey);
                                        this.Mrci.mouseMoveRowIndex = rowIndex;
                                        this.Mrci.mouseMoveColIndex = colIndex;

                                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);

                                        //Here we invalidate the old selected ranges, in some cases selection not removed from group caption cell while pressing down key.
                                        if (GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
                                            this.Model.InvalidateCell(oldRange);

                                        if (!extendSelection)
                                        {
                                            this.Mrci.mouseDownRowIndex = rowIndex;
                                            this.Mrci.mouseDownColIndex = colIndex;
                                        }
                                    }
                                    if (success)
                                        grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);
                                    inSelectingCells = false;
                                    return true;
                                }
                                else
                                    return false;
                            }
                            else
                                return CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                        }

                        if ((selectedRanges.Count == 0 || !extendSelection) && GridOptions.ListBoxSelectionMode == GridSelectionMode.None)
                        {
                            GridRangeInfo emptyRange = GridRangeInfo.Empty;
                            if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.ArrowKey | GridSelectionReason.Clear))
                            {
                                Selections.Clear(false);
                                RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.ArrowKey | GridSelectionReason.Clear);
                            }
                            else
                                return false;

                            this.Mrci.mouseDownRowIndex = rowIndex;
                            this.Mrci.mouseDownColIndex = colIndex;
                        }

                        if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey, moveRange))
                        {
                            // Move Edit and set selection to new row.
                            inSelectingCells = true;
                            CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                            //To Make the RangeType as Cells here invoking the AdjustRangeFunc for GridDataControl. 
                            //Becasue BeginSelectedCells adding the Range for MultiExtended with RangeType as Cells
                            // Fire event.
                            // Skip the parent record selection when current cell renderer is GridDataCellNestedGridRenderer
                            bool success = CurrentCell.GetCurrentCell(out rowIndex, out colIndex);
                            if (CurrentCell.Renderer != null && !(CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                            {
                                GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex, rowIndex);
                                //if (this.AdjustedRangeFunc != null)
                                //    newRange = this.AdjustedRangeFunc(SelectionType.IsRow, this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, newRange);
                                GridRangeInfo oldRange = selectedRanges.ActiveRange;
                                Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, GridSelectionReason.ArrowKey);
                                this.Mrci.mouseMoveRowIndex = rowIndex;
                                this.Mrci.mouseMoveColIndex = colIndex;
                                
                                RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);

                                //Here we invalidate the old selected ranges, in some cases selection not removed from group caption cell while pressing down key.
                                if(GridOptions.ListBoxSelectionMode==GridSelectionMode.MultiExtended)
                                    this.Model.InvalidateCell(oldRange);

                                if (!extendSelection)
                                {
                                    this.Mrci.mouseDownRowIndex = rowIndex;
                                    this.Mrci.mouseDownColIndex = colIndex;
                                } 
                            }
                            if (success)
                                grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);
                            inSelectingCells = false;
                            return true;
                        }
                        else
                            return false;
                    }
                }
                else
                {
                    // TODO: Add code here in GridSelectCellsMouseCountroler to extend selection without moving current cell.

                }
            }
            // Regular grid.
            else if (extendSelection)
            {
                if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    // Delete previous selection.
                    if (selectedRanges.Count == 0)
                    {
                        GridRangeInfo emptyRange = GridRangeInfo.Empty;
                        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.ArrowKey))
                        {
                            Selections.Clear(false);
                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.ArrowKey);
                        }
                        else
                            return false;

                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                    }

                    GridRangeInfo activeRange = selectedRanges.ActiveRange;
                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey))
                    {
                        inSelectingCells = true;
                        if (Model.Options.ExcelLikeSelectionFrame || Model.Options.ExcelLikeSelection)
                        {
                            ExtendSelection(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange), out rowIndex, out colIndex);
                        }
                        else
                        {
                            CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange));
                            CurrentCell.GetCurrentCell(out rowIndex, out colIndex);
                        }

                        GridRangeInfo savedRange = selectedRanges.ActiveRange;

                        if (savedRange.IsEmpty)
                        {
                            int rowIndex2 = this.Mrci.mouseDownRowIndex;
                            int colIndex2 = this.Mrci.mouseDownColIndex;

                            if (rowIndex2 >= grid.InternalGetHeaderRows() && colIndex2 >= grid.InternalGetHeaderCols())
                                Grid.Model.CoveredRanges.Find(rowIndex2, colIndex2, out savedRange);
                            else if (rowIndex2 >= grid.InternalGetHeaderRows())
                                savedRange = GridRangeInfo.Row(rowIndex2);
                            else if (colIndex2 >= grid.InternalGetHeaderCols())
                                savedRange = GridRangeInfo.Col(colIndex2);
                            else
                                savedRange = GridRangeInfo.Empty;
                        }

                        GridRangeInfo newRange;
                        if (savedRange.IsRows)
                            newRange = GridRangeInfo.Rows(this.Mrci.mouseDownRowIndex, rowIndex);
                        else if (savedRange.IsCols)
                            newRange = GridRangeInfo.Cols(this.Mrci.mouseDownColIndex, colIndex);
                        else
                        {
                            newRange = GridRangeInfo.Cells(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex, rowIndex, colIndex);
                        }

                        if (
                            newRange.IsTable && (GridOptions.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None
                            || newRange.IsRows && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                            || newRange.IsCols && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                            || newRange.IsCells && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None)
                        {
                            if (RaiseSelectionChanging(ref newRange, GridSelectionReason.ArrowKey))
                                Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, GridSelectionReason.ArrowKey);
                        }

                        this.Mrci.mouseMoveRowIndex = rowIndex;
                        this.Mrci.mouseMoveColIndex = colIndex;

                        // Notify derived classes.
                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);
                        grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);
                        inSelectingCells = false;

                        return true;
                    }
                    else
                        return false;
                }
            }
            else // No selection.
            {
                GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView);
                if (!GridOptions.ExcelLikeCurrentCell)
                    options.SetCurrentCellOptions |= GridSetCurrentCellOptions.NoSelectRange;
                CurrentCell.InternalMove(direction, num, options);

                if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    this.Mrci.mouseDownRowIndex = rowIndex;
                    this.Mrci.mouseDownColIndex = colIndex;
                }
                return true;
            }

            return false;
        }

        public void ExtendSelection(GridDirectionType direction, int num, GridActivateCurrentCellOptions options, out int rowindex, out int colindex)
        {
            GridRangeInfo regularCells = grid.NavigateWithArrowKeysCellsRange;
            GridRangeInfo notFrozenCells = grid.ScrollCellsRange;
            bool querySuccess = false;
            // bool allowSelectRange = (options.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.None;
            bool scrollFrozen = this.Model.Options.ScrollFrozen;
            int currentRow = this.CurrentCell.RowIndex;
            int currentCol = this.CurrentCell.ColumnIndex;
           
            int vscrollup = 0;
            int hscrollleft = 0;

            num = Math.Max(num, 1);

            GridRangeInfo activeRange = SelectedRanges.ActiveRange;

            int newRowIndex = activeRange.Bottom == currentRow ? activeRange.Top : activeRange.Bottom;
            int newColumnIndex = activeRange.Right == currentCol ? activeRange.Left : activeRange.Right;
            int targetRowIndex = newRowIndex;
            int targetColumnIndex = newColumnIndex;
            
            // Range spanned by covered cell
            GridRangeInfo coveredRange;
            int nRows = 0, nCols = 0;
            if (Model.CoveredRanges.Find(newRowIndex, newColumnIndex, out coveredRange))
            {
                nRows = coveredRange.Bottom - coveredRange.Top;
                nCols = coveredRange.Right - coveredRange.Left;
            }

            switch (direction)
            {
                #region GridDirectionType.Up
                case GridDirectionType.Up:
                    while (num > 0)
                    {
                        if (scrollFrozen && (newRowIndex == grid.TopRowIndex || vscrollup > 0))
                        {
                            // If current cell is at the topmost nonfrozen row, scroll the view.
                            vscrollup++;
                            if (!grid.ScrollGridGetPrevRowIndex(ref newRowIndex))
                                break;
                        }
                        else if (newRowIndex < regularCells.Top || vscrollup > 0)
                        {
                            // If there are frozen rows, move up one visible cell;
                            // if current cell is at the top row, scroll the view.
                            vscrollup++;
                            if (newRowIndex >= notFrozenCells.Top && notFrozenCells.Top > 0)
                            {
                                newRowIndex = grid.GetRow(grid.GetClientRow(notFrozenCells.Top - 1)) - vscrollup;
                            }
                        }
                        else if (notFrozenCells.Top > regularCells.Top
                            && newRowIndex > grid.TopRowIndex
                            && newRowIndex <= grid.GetRow(grid.GetClientRow(notFrozenCells.Top - 1) + 1))
                        {
                            newRowIndex = grid.GetRow(grid.GetClientRow(newRowIndex) - 1);
                        }
                        else
                            newRowIndex = Math.Max(regularCells.Top, newRowIndex - nRows - 1);

                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex))
                            break;
                        // Scrolling necessary when new row is not visible
                        // because it is between the frozen row and the
                        // top row.
                        if (newRowIndex >= notFrozenCells.Top && newRowIndex < grid.TopRowIndex)
                            vscrollup += grid.TopRowIndex - newRowIndex;

                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }

                    break;
                #endregion

                #region GridDirectionType.Down
                case GridDirectionType.Down:
                    while (num > 0)
                    {
                        // If there are frozen rows, move down one visible cell.
                        if (newRowIndex < notFrozenCells.Top)
                        {
                            newRowIndex += nRows;
                            grid.ScrollGridGetNextRowIndex(ref newRowIndex, false);
                            if (newRowIndex > notFrozenCells.Top)
                                newRowIndex = grid.TopRowIndex;
                        }
                        else
                        {
                            nRows = newRowIndex == coveredRange.Bottom ? 0 : nRows;
                            newRowIndex = Math.Min(regularCells.Bottom, newRowIndex + nRows + 1);
                        }
                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex))
                            break;
                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.Left
                case GridDirectionType.Left:
                    while (num > 0)
                    {
                        if (scrollFrozen && (newColumnIndex == grid.LeftColumnIndex || hscrollleft > 0))
                        {
                            // If current cell is at the topmost nonfrozen Column, scroll the view.
                            hscrollleft++;
                            if (!grid.ScrollGridGetPrevColIndex(ref newColumnIndex))
                                break;
                        }
                        else if (newColumnIndex <= regularCells.Left || hscrollleft > 0)
                        {
                            // If there are frozen Columns, move up one visible cell;
                            // if current cell is at the Left Column, scroll the view.
                            hscrollleft++;
                            if (newColumnIndex >= notFrozenCells.Left && notFrozenCells.Left > 0)
                            {
                                var colIdx = grid.GetClientCol(notFrozenCells.Left - 1);
                                if (colIdx > -1)
                                {
                                    newColumnIndex = grid.GetCol(colIdx) - hscrollleft;
                                }
                                else
                                {
                                    newColumnIndex = colIdx;
                                }
                            }
                        }
                        else if (notFrozenCells.Left > regularCells.Left
                            && newColumnIndex >= grid.LeftColumnIndex
                            && newColumnIndex < grid.GetCol(grid.GetClientCol(notFrozenCells.Left - 1) + 1))
                        {
                            newColumnIndex = grid.GetCol(grid.GetClientCol(newColumnIndex) - 1);
                        }
                        else
                            newColumnIndex = Math.Max(regularCells.Left, newColumnIndex - nCols - 1);

                        if (newColumnIndex < 0 || newRowIndex < 0)
                            break;
                        
                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex))
                            break;

                        // Scrolling becomes necessary when new column is not visible
                        // because it is between the frozen columns and the
                        // left column.
                        if (newColumnIndex >= notFrozenCells.Left && newRowIndex < grid.LeftColumnIndex)
                            hscrollleft += grid.LeftColumnIndex - newColumnIndex;

                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.Right
                case GridDirectionType.Right:
                    while (num > 0)
                    {
                        // If there are frozen rows, move one visible cell to the right.
                        if (newColumnIndex < notFrozenCells.Left)
                        {
                            newColumnIndex += nCols;
                            grid.ScrollGridGetNextColIndex(ref newColumnIndex, false);
                            if (newColumnIndex > notFrozenCells.Left)
                                newColumnIndex = grid.LeftColumnIndex;
                        }
                        else
                        { 
                            nCols = newColumnIndex == coveredRange.Right ? 0 : nCols;
                            newColumnIndex = Math.Min(regularCells.Right, newColumnIndex + nCols + 1);
                           
                        }
                        if (!Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex))
                          break;

                        num--;
                        querySuccess = true;
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.PageDown
                case GridDirectionType.PageDown:
                    //gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);

                    if (grid.TopRowIndex < regularCells.Bottom)
                    {
                        newRowIndex = grid.ScrollRows.GetNextPage(newRowIndex);
                        newRowIndex = Math.Min(regularCells.Bottom, newRowIndex);

                        // Find the nearest possible cell (first down. then up).
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);

                        if (querySuccess)
                        {
                            if (newRowIndex > grid.ScrollCellsRange.Bottom && targetRowIndex <= grid.ScrollCellsRange.Bottom)
                                grid.ScrollToBottom();

                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                    }
                    break;
                #endregion

                #region GridDirectionType.PageUp

                case GridDirectionType.PageUp:
                    if (grid.TopRowIndex >= regularCells.Top)
                    {
                        newRowIndex = grid.ScrollRows.GetPreviousPage(newRowIndex);
                        newRowIndex = Math.Max(regularCells.Top, newRowIndex);

                        // Find the nearest possible cell (first up. then down).
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);

                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);

                        if (querySuccess)
                        {
                            if (newRowIndex < grid.ScrollCellsRange.Top && targetRowIndex >= grid.ScrollCellsRange.Top)
                                grid.ScrollToTop();

                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                    }
                    break;
                #endregion

                #region GridDirectionType.PageRight
                case GridDirectionType.PageRight:
                    //gridModel.RaiseQueryMaximumRowCol(gridModel.RowCount, 0);
                    if (grid.LeftColumnIndex < regularCells.Right)
                    {
                        newColumnIndex = grid.ScrollColumns.GetNextPage(newColumnIndex);
                        newColumnIndex = Math.Min(regularCells.Right, newColumnIndex);

                        // Find the nearest possible cell (first right. then up).
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);

                        if (querySuccess)
                        {
                            if (newColumnIndex > grid.ScrollCellsRange.Right && targetColumnIndex <= grid.ScrollCellsRange.Right)
                                grid.ScrollToRightEnd();

                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                    }
                    break;
                #endregion

                #region GridDirectionType.PageLeft
                case GridDirectionType.PageLeft:
                    if (grid.LeftColumnIndex > regularCells.Left)
                    {
                        newColumnIndex = grid.ScrollColumns.GetPreviousPage(newColumnIndex);
                        newColumnIndex = Math.Max(regularCells.Left, newColumnIndex);

                        // Find the nearest possible cell (first left. then down).
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newColumnIndex, ref newColumnIndex);

                        if (!querySuccess)
                            querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newColumnIndex, ref newColumnIndex);

                        if (querySuccess)
                        {
                            if (newColumnIndex < grid.ScrollCellsRange.Left && targetColumnIndex >= grid.ScrollCellsRange.Left)
                                grid.ScrollToLeftEnd();

                            targetRowIndex = newRowIndex;
                            targetColumnIndex = newColumnIndex;
                        }
                    }
                    break;
                #endregion

                #region GridDirectionType.MostLeft
                case GridDirectionType.MostLeft:
                    hscrollleft = 1;
                    newColumnIndex = regularCells.Left;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToLeftEnd();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.MostRight
                case GridDirectionType.MostRight:
                    newColumnIndex = regularCells.Right;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToRightEnd();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.TopLeft
                case GridDirectionType.TopLeft:
                    newColumnIndex = regularCells.Left;
                    newRowIndex = regularCells.Top;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Right, ref newRowIndex, ref newColumnIndex);
                    if (!querySuccess)
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToTop();
                        grid.ScrollToLeftEnd();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                #region GridDirectionType.BottomRight

                case GridDirectionType.BottomRight:
                    newColumnIndex = regularCells.Right;
                    newRowIndex = regularCells.Bottom;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Left, ref newRowIndex, ref newColumnIndex);
                    if (!querySuccess)
                        querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToBottom();
                        grid.ScrollToRightEnd();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;

                #endregion

                #region GridDirectionType.Top
                case GridDirectionType.Top:
                    vscrollup = 1;
                    newRowIndex = regularCells.Top;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Down, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToTop();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;

                #endregion

                #region GridDirectionType.Bottom
                case GridDirectionType.Bottom:
                    newRowIndex = regularCells.Bottom;
                    querySuccess = Grid.GetNextCurrentCellPosition(GridDirectionType.Up, ref newRowIndex, ref newColumnIndex);
                    if (querySuccess)
                    {
                        grid.ScrollToBottom();
                        targetRowIndex = newRowIndex;
                        targetColumnIndex = newColumnIndex;
                    }
                    break;
                #endregion

                default:
                    throw new InvalidEnumArgumentException("direction");
            }

            rowindex = targetRowIndex;
            colindex = targetColumnIndex;
        }
        #endregion

        #region Begin/End/Change Selection
        /// <summary>
        /// Defines the possible types of selection.
        /// </summary>
        public enum SelectionType
        {
            /// <summary>
            /// Range of cells.
            /// </summary>
            IsCells,

            /// <summary>
            /// One or more rows.
            /// </summary>
            IsRow,

            /// <summary>
            /// One or more columns.
            /// </summary>
            IsCol,

            /// <summary>
            /// Whole table.
            /// </summary>
            IsTable
        }

        /// <summary>
        /// Gets or sets a value that adjusts the selection behavior depends on the given selection type.
        /// </summary>
        public Func<SelectionType, int, int, GridRangeInfo, GridRangeInfo> AdjustedRangeFunc
        {
            get;
            set;
        }

        private void AdjustRange(SelectionType type, int rowIndex, int colIndex, ref GridRangeInfo clickRange)
        {
            if (this.AdjustedRangeFunc != null)
            {
                var adjustedRange = this.AdjustedRangeFunc(type, rowIndex, colIndex, clickRange);
                clickRange = adjustedRange;
            }
        }

        internal void BeginSelectCellsController(int rowIndex, int colIndex, ModifierKeys modifierKeys, GridSelectionReason SelectionReason)
        {
            this.BeginSelectCells(rowIndex, colIndex, modifierKeys, SelectionReason);
        }
        void BeginSelectCells(int rowIndex, int colIndex, ModifierKeys modifierKeys, GridSelectionReason SelectionReason)
        {
            BeginSelectCells(rowIndex, colIndex, modifierKeys, null, SelectionReason);
        }

         void BeginSelectCells(int rowIndex, int colIndex, ModifierKeys modifierKeys, MouseControllerEventArgs e, GridSelectionReason SelectionReason)
        {
#if DEBUG
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, modifierKeys, this);
            }
#endif

            if (Grid.CurrentCell != null && Grid is GridTreeControlImpl && Grid.CurrentCell.IsModified)
            {
                Grid.CurrentCell.Validate();
                if (!Grid.CurrentCell.IsValid)
                    return;
            }


            GridRangeInfoList selectedRanges = SelectedRanges;
            selectedRanges.RemoveEmptyRanges();

            if (rowIndex == 0 && colIndex == 0)
            {
                 var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                 if (dataGrid == null || dataGrid.ShowRowHeader)//Following Implementation only for Table Selection. In GridDataControl if RowHeader is not there means, cell(0,0) click doesnt leads to Table selection. Here we restrict this using condition Check
                 {
                     // Toggle table selection.
                     if ((GridOptions.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                     {
                         if (selectedRanges.AnyRangeContains(GridRangeInfo.Cell(0, 0)))
                         {
                             GridRangeInfo emptyRange = GridRangeInfo.Empty;
                             if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.MouseDown, GridRangeInfo.Cell(0, 0)))
                             {
                                 Selections.Clear(false);
                                 RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.MouseDown);
                             }

                             this.Mrci.mouseClickRowIndex = grid.TopRowIndex;
                             this.Mrci.mouseClickColIndex = grid.LeftColIndex;
                             this.Mrci.mouseDownRowIndex = grid.TopRowIndex;
                             this.Mrci.mouseDownColIndex = grid.LeftColIndex;
                             this.Mrci.mouseMoveRowIndex = Mrci.mouseDownRowIndex;
                             this.Mrci.mouseMoveColIndex = Mrci.mouseDownColIndex;
                             // ... Selections.SelectRange will trigger Selections.OnSelectionChanged event

                             return;
                         }
                     }
                 }
            }

            int nhRow = grid.InternalGetHeaderRows();
            int nhCol = grid.InternalGetHeaderCols();
            bool bCol = (rowIndex < nhRow && colIndex >= nhCol);
            bool bRow = ((colIndex < nhCol || GridOptions.ListBoxSelectionMode != GridSelectionMode.None) && rowIndex >= nhRow);
            bool bCell = !bRow && (rowIndex >= nhRow && colIndex >= nhCol);
            bool bTable = (colIndex == 0 && rowIndex == 0);

            if (GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
            {
                addNewSelection = (modifierKeys & ModifierKeys.Control) != ModifierKeys.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
                if (rowIndex < nhRow)
                    return;
            }
            else if (GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiSimple)
            {
                addNewSelection = true;
                if (rowIndex < nhRow)
                    return;
            }
            else if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One)
            {
                addNewSelection = extendSelection = false;
                if (rowIndex < nhRow)
                    return;
            }
            else
            {
                addNewSelection = (modifierKeys & ModifierKeys.Shift) == ModifierKeys.None && (modifierKeys & ModifierKeys.Control) != ModifierKeys.None && (GridOptions.AllowSelection & GridSelectionFlags.Multiple) != GridSelectionFlags.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & ModifierKeys.Control) == ModifierKeys.None && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None && (GridOptions.AllowSelection & GridSelectionFlags.Shift) != GridSelectionFlags.None;
            }

            Mrci.firstClick = false;

			//Here the selection is suspended when AllowSelection is set to None.
            //if (GridOptions.AllowSelection != GridSelectionFlags.None)
            {
                int ncRow, ncCol;

                GridRangeInfo clickRange = GridRangeInfo.Empty;

                if (bCell)
                {
                    Model.CoveredRanges.Find(rowIndex, colIndex, out clickRange);
                    this.AdjustRange(SelectionType.IsCells, rowIndex, colIndex, ref clickRange);
                }
                else if (bRow)
                {
                    clickRange = GridRangeInfo.Row(rowIndex);
                    //this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref clickRange);
                }
                else if (bCol)
                {
                    clickRange = GridRangeInfo.Col(colIndex);
                    this.AdjustRange(SelectionType.IsCol, rowIndex, colIndex, ref clickRange);
                }
                else
                {
                    Point point = e.Location;                   
                    var ClickOutside = grid.PointToCellRowColumnIndexOutsideCells(point, false);
                    if (ClickOutside.RowIndex >= 0 && ClickOutside.ColumnIndex >= 0)
                    {
                        clickRange = GridRangeInfo.Table();
                        this.AdjustRange(SelectionType.IsTable, rowIndex, colIndex, ref clickRange);
                    }
                }

                GridRangeInfo tempRange;
                Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out tempRange);

                bool resetSelection = false;
                if (selectedRanges.Count == 1 && selectedRanges[0] != tempRange || selectedRanges.Count > 1)
                    resetSelection = !extendSelection && !addNewSelection || (GridOptions.AllowSelection & GridSelectionFlags.MixRangeType) == 0 && selectedRanges.FilterRangeType(clickRange.RangeType).Count != selectedRanges.Count;

                // First, remove all selections if neither SHIFT or CTRL is pressed
                // or if the clicked range type differs from previous selected ranges.
                //If ListBoxSelectionMode= GridSelectionMode.MultiSimple means selection should not reset.
                if (resetSelection && GridOptions.ListBoxSelectionMode!= GridSelectionMode.MultiSimple)
                {
                    extendSelection = addNewSelection = false;
                    GridRangeInfo emptyRange = GridRangeInfo.Empty;
                    if (RaiseSelectionChanging(ref emptyRange, SelectionReason, clickRange))
                    {
                        if (!(this.grid.Model.EnableContextMenu && e.Button.Equals(MouseButton.Right)))
                            Selections.Clear(false);
                        RaiseSelectionChanged(GridRangeInfo.Empty, SelectionReason);
                    }
                    else
                        return;
                }

                // Ctrl-Key
                if (addNewSelection)
                {
                    // If user clicks on a selected row or column header, deselect
                    // the row or column.
                    if ((!clickRange.IsCells || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiSimple) && selectedRanges.AnyRangeContains(clickRange))
                    {
                        // In ListBoxSelectionMode selection should besed on the Row. Otherwise RowHeader will remain selected
                        this.CurrentCell.MoveTo(start.RowIndex, start.ColumnIndex);
                        Selections.Remove(GridRangeInfo.Row(rowIndex));                        
                        SetMrciMouseDownClick(clickRange.Top, clickRange.Left);
                        if (e.Button != MouseButton.Right)
                            canceled = true;
                        return;
                    }
                    else if (clickRange.IsCells && selectedRanges.AnyRangeContains(clickRange) && GridOptions.ListBoxSelectionMode == GridSelectionMode.None)
                    {
                        if (Model.Options.ExcelLikeSelectionFrame || Model.Options.ExcelLikeSelection)
                            this.CurrentCell.MoveTo(start.RowIndex, start.ColumnIndex);
                        else
                            Selections.Remove(clickRange); 
                       
                        SetMrciMouseDownClick(clickRange.Top, clickRange.Left);
                        if (e.Button != MouseButton.Right)
                            canceled = true;
                        return;
                    }

                    // If user presses CTRL-Key while no range was selected
                    // but the current cell was visible, select the current
                    // cell.
                    var datagri = this.Grid.FindParentElementOfType<GridDataControl>();
                    if (!resetSelection &&
                        !extendSelection && selectedRanges.Count == 0 && CurrentCell.GetCurrentCell(out ncRow, out ncCol) &&
                        !CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && GridOptions.ListBoxSelectionMode == GridSelectionMode.None 
                        &&(datagri==null || (datagri!=null && datagri.Model.CurrencyManager.IsRecordCell)))
                    {
                        GridRangeInfo range = GridRangeInfo.Cell(ncRow, ncCol);
                        if (ncRow >= nhRow && ncCol >= nhCol)
                        {
                            if (RaiseSelectionChanging(ref range, SelectionReason , clickRange))
                            {
                                Selections.ChangeSelection(GridRangeInfo.Empty, range, SelectionReason);
                                RaiseSelectionChanged(range, SelectionReason);
                            }
                            else
                                // Current cell can't be selected. Continue as if CTRL is not pressed.
                                addNewSelection = false;
                        }
                    }
                }
                //This is added to find which mouse button clicked.If context menu enabled means rightclick should not remove or add selection.                
                bool isLeftButtonPressed = false;
                if (e != null)
                {
                    isLeftButtonPressed = e.Button == MouseButton.Left ? true : false;
                }
               

                // Check, if selection is allowed.
                if (
                    (bTable || clickRange.IsTable) && (GridOptions.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None
                    || (bRow || clickRange.IsRows) && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                    || (bCol || clickRange.IsCols) && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                    || (bCell || clickRange.IsCells) && (!this.Grid.Model.EnableContextMenu || isLeftButtonPressed) && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None
                    || GridOptions.AllowSelection == GridSelectionFlags.None && GridOptions.ListBoxSelectionMode != GridSelectionMode.None) //This allows the selection when the shift key or ctrl pressed
                {

                    // Add a new empty range to selected range list. Will be initialized below.
                    if (addNewSelection)
                        selectedRanges.Add(GridRangeInfo.Empty);

                    // Start selection.

                    // If user presses <SHIFT>, extend last range.
                    if (extendSelection && selectedRanges.Count == 0)
                    {
                        // Select cells starting from current cell.
                        int ncRowIndex, ncColIndex;
                        CurrentCell.GetCurrentCell(out ncRowIndex, out ncColIndex);

                        this.Mrci.mouseDownRowIndex = ncRowIndex;
                        this.Mrci.mouseDownColIndex = ncColIndex;
                    }
                    else if (!extendSelection)
                    {
                        this.Mrci.mouseClickRowIndex = rowIndex;
                        this.Mrci.mouseClickColIndex = colIndex;
                        this.Mrci.mouseDownRowIndex = rowIndex;
                        this.Mrci.mouseDownColIndex = colIndex;
                    }

#if DEBUG
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        Trace.WriteLineIf(extendSelection && Switches.SelectRange.TraceVerbose, "ClickRange: " + clickRange.ToString());
                    }
#endif


                    GridRangeInfo savedRange = selectedRanges.ActiveRange;
                    //This code is added for fixing the issue in selection using Shift in DataBound CellType --SD8226
                    GridRangeInfo newRange;
                    if (extendSelection)
                    {
                        if (Model.Options.ExcelLikeSelectionFrame || Model.Options.ExcelLikeSelection)
                            newRange = clickRange.UnionRange(GridRangeInfo.Cell(CurrentCell.RowIndex, CurrentCell.ColumnIndex));
                        else
                            newRange = clickRange.UnionRange(savedRange);
                    }
                    else
                        newRange = clickRange.UnionRange(GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex));

                   // GridRangeInfo newRange = clickRange.UnionRange(GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex)); --Previous Code--

                    if ((!GridOptions.ExcelLikeSelectionFrame || resetSelection)
                        && !addNewSelection &&
                        newRange.IsCells && newRange.Width == 1 && newRange.Height == 1 && !(this.CurrentCell.IsEditing && GridOptions.ExcelLikeSelection))
                        newRange = GridRangeInfo.Empty;

                    // Allow programmer to change the range.
                    if (RaiseSelectionChanging(ref newRange, SelectionReason, newRange))
                    {
                        Selections.ChangeSelection(savedRange, newRange, false, e, SelectionReason);
                        RaiseSelectionChanged(selectedRanges.ActiveRange, SelectionReason);
                    }
                    else
                    {
                        selectedRanges.RemoveEmptyRanges();
                        return;
                    }

                    if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                        inSelectingCells = true;
                }
            }
        }

        internal bool ChangeSelectCellsController(int rowIndex, int colIndex)
        {
            return this.ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseMove);
        }

        bool ChangeSelectCells(int rowIndex, int colIndex, GridSelectionReason reason)
        {
#if DEBUG
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, this);
            }
#endif

			//Here the condition that suspend the selection is removed.
            //if (GridOptions.AllowSelection != GridSelectionFlags.None // selecting cells enabled?
            //    && inSelectingCells)
            if (inSelectingCells)
            {
                GridRangeInfoList selectedRanges = SelectedRanges;

                int nhRow = grid.InternalGetHeaderRows();
                int nhCol = grid.InternalGetHeaderCols();
                int nfRow = grid.InternalGetFrozenRows();
                int nfCol = grid.InternalGetFrozenCols();

                GridRangeInfo savedRange = selectedRanges.ActiveRange;
                GridRangeInfo newRange;

                rowIndex = Math.Max(nhRow, rowIndex);
                if (this.Mrci.mouseDownRowIndex >= nfRow && grid.TopRowIndex >= nfRow)
                    rowIndex = Math.Max(grid.TopRowIndex, rowIndex);
                colIndex = Math.Max(nhCol, colIndex);
                if (this.Mrci.mouseDownColIndex > nfCol && grid.LeftColIndex >nfCol)
                    colIndex = Math.Max(grid.LeftColIndex, colIndex);

                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One)
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                    //this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
                }
                else if (savedRange.IsCols)
                {
                    newRange = GridRangeInfo.Cols(colIndex, this.Mrci.mouseDownColIndex);
                    this.AdjustRange(SelectionType.IsCol, rowIndex, colIndex, ref newRange);
                }
                else if (savedRange.IsRows || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
                {
                    //To Make the RangeType as Cells here invoking the AdjustRangeFunc for GridDataControl. 
                    //Becasue BeginSelectedCells adding the Range for MultiExtended with RangeType as Cells
                    newRange = GridRangeInfo.Rows(rowIndex, this.Mrci.mouseDownRowIndex);                    
                    //if (this.AdjustedRangeFunc != null)
                    //    newRange = this.AdjustedRangeFunc(SelectionType.IsRow, rowIndex, this.Mrci.mouseDownColIndex, newRange);
                }
                else
                {
                    if (rowIndex != this.Mrci.mouseDownRowIndex
                        || colIndex != this.Mrci.mouseDownColIndex
                        || GridOptions.ExcelLikeSelectionFrame)
                    {
                        newRange = GridRangeInfo.Cells(this.Mrci.mouseDownRowIndex,
                            this.Mrci.mouseDownColIndex,
                            rowIndex,
                            colIndex);
                    }
                    else
                        newRange = GridRangeInfo.Empty;
                }

                // Allow programmer to change the range.
                if (RaiseSelectionChanging(ref newRange, reason, newRange))
                {
                    Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, reason);

                    // Trigger Selections.OnSelectionChanged event.
                    RaiseSelectionChanged(selectedRanges.ActiveRange, reason);

                    if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One
                        || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended && multiExtendedShouldMoveCurrentCell)
                    {
                        GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange | /*GridSetCurrentCellOptions.NoSetFocus |*/ GridSetCurrentCellOptions.NoSyncCurrentCell);
                        options.ShouldBeginEdit = false;
                        if (CurrentCell.RowIndex != rowIndex)
                            CurrentCell.MoveTo(rowIndex, CurrentCell.ColumnIndex, options);
                    }

                }

                return true;
            }

            return false;
        }

        internal void EndSelectCellsController()
        {
            this.EndSelectCells(GridSelectionReason.MouseDown);
        }

        void EndSelectCells(GridSelectionReason gridSelectionReason)
        {
#if DEBUG
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(this);
            }
#endif

            GridRangeInfoList selectedRanges = SelectedRanges;

            CurrentCell.AdjustRowColIfCoveredCell(ref Mrci.mouseMoveRowIndex, ref Mrci.mouseMoveColIndex);
            int nhRow = grid.InternalGetHeaderRows();
            nhRow = nhRow > 0 ? nhRow - 1 : nhRow;
            int nhCol = grid.InternalGetHeaderCols();
            nhCol = nhCol > 0 ? nhCol - 1 : nhCol;

			//Here the condition that suspend the selection is removed.
            //if (GridOptions.AllowSelection != GridSelectionFlags.None
            //    && inSelectingCells
            //    && (this.Mrci.mouseDownColIndex != 0 || this.Mrci.mouseDownRowIndex != 0))
            if (inSelectingCells
                && (this.Mrci.mouseDownColIndex != 0 || this.Mrci.mouseDownRowIndex != 0))
            {
                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.None
                    && this.Mrci.mouseDownColIndex == this.Mrci.mouseMoveColIndex
                    && this.Mrci.mouseDownRowIndex == this.Mrci.mouseMoveRowIndex
                    && this.Mrci.mouseDownRowIndex > nhRow && this.Mrci.mouseDownColIndex > nhCol)
                {
                    if (addNewSelection)
                    {
                        // Allow programmer to change the range.
                        GridRangeInfo range = GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex);
                        if (selectedRanges.Count > 0 && RaiseSelectionChanging(ref range, gridSelectionReason))
                        {
                            // User presses ctrl-key. Select the current cell.
                            Selections.ChangeSelection(selectedRanges.ActiveRange, range, GridSelectionReason.MouseMove);

                            // Trigger Selections.OnSelectionChanged event.
                            RaiseSelectionChanged(selectedRanges.ActiveRange, gridSelectionReason);
                        }
                    }
                    else
                    {
                        // User did move the current cell but didn't select any cells.
                        if (!GridOptions.ExcelLikeCurrentCell)
                        {
                            Selections.Clear(true);
                        }
                    }
                }
                else
                {
                    // Trigger Selections.OnSelectionChanged event to let user know that selection.
                    // ended.
                    RaiseSelectionChanged(selectedRanges.ActiveRange, gridSelectionReason);
                }
            }
            else
                RaiseSelectionChanged(selectedRanges.ActiveRange, gridSelectionReason);
        }

        #endregion

        #region IMouseController Members

        /// <summary>
        /// Gets the name of the mouse controller.
        /// </summary>
        public string Name
        {
            get { return "SelectCellsMouseController"; }
        }

        ///// <summary>
        ///// Returns the selection cursor.
        ///// </summary>
        public Cursor Cursor
        {
            get
            {
                Cursor Currentcursor = this.Grid.RaiseGridCellCursor();
                if (Currentcursor == Cursors.Arrow)
                {
                    return _cursor;
                }
                else
                {
                    return Currentcursor;
                }
            }
            set
            {
                _cursor = value;
            }
        }

        private Cursor _cursor;       

        /// <summary>
        /// Occurs when the mouse enters a grid cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that holds the event data.</param>
        public void MouseHoverEnter(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverEnter(e);
        }

        /// <summary>
        /// Occurs when the mouse is moved over a grid cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public void MouseHover(MouseControllerEventArgs e)
        {
            //    TraceUtil.TraceCurrentMethodInfo(host.GetType().Name, e.IsMouseOverChildElement);
            this.Grid.RaiseCellMouseHover(e);
        }

        /// <summary>
        /// Occurs when the mouse leaves the cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that holds the event data.</param>
        public void MouseHoverLeave(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverLeave(e);
        }
        
        /// <summary>
        /// Occurs when the mouse is pressed in a grid cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public virtual void MouseDown(MouseControllerEventArgs e)
        {
            this.Grid.RaiseCellMouseDown(e);
            canceled = false;
            Point point = e.Location;
            this.moveToSuccess = true;
            //TraceUtil.TraceCurrentMethodInfo(point);

            Rect r = new Rect();
            ///Previous Code r = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            ///I have change this code as follows. Table Summary row could not get select.
            r = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            var datagrid = this.Grid.FindParentElementOfType<GridDataControl>();
            var ClickOutside = grid.PointToCellRowColumnIndexOutsideCells(point, false);

            /// GridDataCellNestedGridEditor is added in the condition to avoid child model's cell get into edit mode on bouble clicking the empty space in nested cell
            if (!r.IsEmpty && !(r.Contains(point)) && (this.grid is GridDataControlBaseImpl || grid is GridDataCellNestedGridEditor || this.grid is GridTreeControlImpl))
            {
                if (datagrid != null)
                {
                    if (datagrid.Model.TableProperties.TableSummaryRows.Count > 0)
                    {
                        // IsInSummaryPosition(int RowIndex) checks the current row is in Summary row, since summary row should not select.
                        if (datagrid.Model.IsInSummaryPosition(ClickOutside.RowIndex) && !(datagrid.Model.IsInFilterBarPosition(ClickOutside.RowIndex)))
                        {

                            if (ClickOutside.RowIndex == 0 && ClickOutside.ColumnIndex == 0)
                                start = ClickOutside;
                            else
                            {
                                start.RowIndex = this.CurrentCell.RowIndex;
                                start.ColumnIndex = this.CurrentCell.ColumnIndex;
                            }
                        }
                        else
                        {
                            start = ClickOutside;
                        }
                    }
                    else
                        start = ClickOutside;
                }
                if (grid is GridTreeControlImpl)
                {
                    start = ClickOutside;
                }                
            }
            else
            {                
                start = grid.PointToCellRowColumnIndex(point);
            }
            GridRangeInfo coveredRange = null;
            int nRows = 0;
            int nCols = 0;
            int currentRow = start.RowIndex;
            int currentCol = start.ColumnIndex;
            
            if (currentRow >= 0 && currentCol >= 0)
            {
                if (grid.Model.CoveredRanges.Find(currentRow, currentCol, out coveredRange))
                {
                    nRows = coveredRange.Bottom - coveredRange.Top;
                    nCols = coveredRange.Right - coveredRange.Left;
                }
            }

            if (coveredRange != null && (nRows > 0 || nCols > 0))
            {
                this.CurrentCell.AdjustRowColIfCoveredCell(ref currentRow, ref currentCol);
                start = new RowColumnIndex(currentRow, currentCol);
            }

            GridRenderStyleInfo style = grid.GetRenderStyleInfo(start);
            IGridCellRenderer renderer = style.CellRenderer;
            if (!style.Enabled)
            {
                this.canceled = true;
                return;
            }

            //IGridCellRenderer renderer = style.CellRenderer;
            if (e.DirectlyOverRenderer is GridCellHyperlinkCellRenderer || e.DirectlyOverRenderer is GridCellCheckboxRenderer)
            {
                this.MouseControllerDispatcher.CanHandleMouseDown = false;
            }
            else
            {
                this.MouseControllerDispatcher.CanHandleMouseDown = true;
            }
            //IHitTestSelectCells hsc = renderer as IHitTestSelectCells;
            //if (hsc != null)
            //{
            //    hsc.MouseDown(host, e);
            //    if (e.Handled)
            //    {
            //        start = RowColumnPosition.Empty;
            //        return;
            //    }
            //}

            // gridControl.SelectedCells = GridRangeInfo.Empty;
            RowColumnIndex startAdjustedForCoveredCell = grid.AdjustCoveredCellRowColumnIndex(start);

            int rowIndex = startAdjustedForCoveredCell.RowIndex;
            int colIndex = startAdjustedForCoveredCell.ColumnIndex;

            if (rowIndex >= 0 && colIndex >= 0 && rowIndex<grid.Model.RowCount && colIndex<grid.Model.ColumnCount)
                style = grid.GetRenderStyleInfo(rowIndex, this.Grid.NavigateWithArrowKeysCellsRange.Left);

            if (this.Model.Options.AllowSelectionOnMouseUp)
            {
                return;
            }
            GridDataStyleInfo styleinfo = null;
            if (datagrid != null)
                styleinfo = style.ModelStyle as GridDataStyleInfo;
            if (styleinfo != null && styleinfo.CellIdentity != null)
            {
                var cellidentity = styleinfo.CellIdentity;
                if (cellidentity.TableCellType != GridDataTableCellType.NestedTableCell && cellidentity.TableCellType != GridDataTableCellType.DetailsViewCell)
                    BeginSelectCells(rowIndex, colIndex, Keyboard.Modifiers, e, GridSelectionReason.MouseDown);
                else
                    return;
            }
            else
            {
                if (!(style.CellRenderer is GridDataCellNestedGridRenderer))
                    BeginSelectCells(rowIndex, colIndex, Keyboard.Modifiers, e, GridSelectionReason.MouseDown);
                else
                    return;
            }
            
            if (canceled)
            {
                return;
            }

            if (!(extendSelection && Model.Options.ExcelLikeSelectionFrame)
                && (CurrentCell.CellRowColumnIndex != startAdjustedForCoveredCell && this.multiExtendedShouldMoveCurrentCell
                || (this.CurrentCell.ActivateOptions != null && this.CurrentCell.ActivateOptions.Element == null))) /*(Keyboard.Modifiers != ModifierKeys.Control))*/
            {
                GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions();
                options.IsActivateTriggeredByMouseDownIntoUIElement = true;
                if ((grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                {
                    options.ShouldBeginEdit = false;
                }
                else
                {
                    options.ShouldBeginEdit = (grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.SetCurrent | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll)) != 0;
                }                
                bool isMouseLeftButtonClicked=false;
                if (e.Button != null && e.SourceEventArgs is System.Windows.Input.MouseButtonEventArgs)
                    isMouseLeftButtonClicked = ((System.Windows.Input.MouseButtonEventArgs)(e.SourceEventArgs)).ChangedButton == MouseButton.Left ? true : false;
                int nhRow = grid.InternalGetHeaderRows();
                int nhCol = grid.InternalGetHeaderCols();
                bool bCol = (rowIndex < nhRow && colIndex >= nhCol);
                bool bRow = ((colIndex < nhCol || GridOptions.ListBoxSelectionMode != GridSelectionMode.None) && rowIndex >= nhRow);
                if (!SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)) || !this.Grid.Model.EnableContextMenu || isMouseLeftButtonClicked || bRow||bCol)
                                moveToSuccess = CurrentCell.MoveTo(start, options);
                }
            
    
            if (!moveToSuccess)
            {
                return;
            }

            if (datagrid != null)
            {
                // IsInSummaryPosition(int RowIndex) checks the current row is in Summary row, since summary row should not select.
                if (!datagrid.Model.IsInSummaryPosition(ClickOutside.RowIndex) && !CurrentCell.IsEditing)
                {
                    if (CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell) &&
                       ((e.ClickCount == 2 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0) ||
                       (e.ClickCount == 1 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.ClickOnCell) != 0)) ||
                       (e.ClickCount == 1 && renderer is GridDataFilterBarCellRenderer))
                        CurrentCell.BeginEdit(true);
                }
            }
            else
            {
                if (CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell) &&
                   ((e.ClickCount == 2 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0) ||
                   (e.ClickCount == 1 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.ClickOnCell) != 0))||
                    (e.ClickCount == 1 && renderer is GridDataFilterBarCellRenderer))
                    CurrentCell.BeginEdit(true);
            }

            if (!extendSelection)
            {
                Mrci.mouseClickRowIndex = rowIndex;
                Mrci.mouseClickColIndex = colIndex;
                Mrci.mouseMoveRowIndex = rowIndex;
                Mrci.mouseMoveColIndex = colIndex;
            }

            grid.AutoScroller.AutoScrollBounds = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            if (grid.AutoScroller.AutoScrollBounds.Contains(point))
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
        }

        /// <summary>
        /// Occurs when the mouse moves over a grid cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public virtual void MouseMove(MouseControllerEventArgs e)
        {
            //If cell is in EditMode means, further selection process should not take palce on mouse move.
            if (this.Model.Options.ExcelLikeSelection && this.CurrentCell.IsEditing)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                return;
            }
            this.Grid.RaiseCellMouseMove(e);
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            // VirtualizingCellsControl cellsControl = grid;// as VirtualizingCellsControl;
            bool GetCurrentRowColumn = true;
            var datagrid = this.Grid.FindParentElementOfType<GridDataControl>();
            var treeGrid = this.Grid.FindParentElementOfType<GridTreeControl>();
            if (!start.IsEmpty)
            {
                RowColumnIndex end;
                Rect r = new Rect();
                
                var gridControl = this.Grid.FindParentElementOfType<GridControl>();

                if (datagrid != null)
                {
                    r = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
                    GetCurrentRowColumn = datagrid.TableSummaryPosition == Position.Top ? false : true;
                }
                else if (gridControl != null)
                {
                    r = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body);
                }
                else
                {
                    r = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
                }
                if (!r.IsEmpty && (r.Contains(point)) && (grid is GridDataControlBaseImpl) && GetCurrentRowColumn) //&& !(grid is GridTreeControlImpl) && GetCurrentRowColumn && !(grid is GridControl))
                //if (!r.IsEmpty && (r.Contains(point)) && !(grid is GridTreeControlImpl) && GetCurrentRowColumn && !(grid is GridControl))
                {
                    end = new RowColumnIndex(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
                }
                else
                {
                    end = grid.PointToCellRowColumnIndex(point);
                }               

                if (!end.IsEmpty && e.SourceEventArgs.LeftButton == MouseButtonState.Pressed)
                {
                    int rowIndex = end.RowIndex;
                    int colIndex = end.ColumnIndex;
                    int StartRowIndex = 0;
                    int EndRowIndex = 0;
                    if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex)
                    {
                        if (datagrid != null) //  We find the total rows should be able to  be selected. we neglate Addnew row, table summary row, frozen row and footer row here. 
                        {
                            EndRowIndex = this.Grid.Model.RowCount;
                            if (datagrid.Model.TableProperties.TableSummaryRows.Count > 0)   //Table Summary Row
                            {
                                if (datagrid.TableSummaryPosition == Position.Top)
                                {
                                    StartRowIndex = datagrid.Model.TableProperties.TableSummaryRows.Count;
                                }
                                else
                                {
                                    EndRowIndex = this.Grid.Model.RowCount - datagrid.Model.TableProperties.TableSummaryRows.Count;
                                }
                            }
                            if (datagrid.ShowAddNewRow)                                      // Addnew Row
                            {
                                if (datagrid.AddNewRowPosition == Position.Top)
                                {
                                    StartRowIndex = StartRowIndex + 1;
                                }
                                else
                                {
                                    EndRowIndex = EndRowIndex - 1;
                                }
                            }
                            if (datagrid.FrozenRows > 0)                                    // Frozen Row 
                            {
                                if (datagrid.FrozenRows == 1 && !datagrid.ShowAddNewRow)
                                    StartRowIndex = StartRowIndex + datagrid.FrozenRows;
                            }
                            if (datagrid.FooterRows > 0)                                    // footer Row 
                            {
                                EndRowIndex = EndRowIndex - datagrid.FooterRows;
                            }
                            if (((rowIndex < EndRowIndex) && (rowIndex >= StartRowIndex)) || rowIndex == 0)
                            {
                                GridRenderStyleInfo style = null;
                                if (rowIndex >= 0 && colIndex >= 0 && rowIndex < grid.Model.RowCount && colIndex < grid.Model.ColumnCount)
                                    style = grid.GetRenderStyleInfo(rowIndex, this.Grid.NavigateWithArrowKeysCellsRange.Left);
                                if (style != null && !(style.CellRenderer is GridDataCellNestedGridRenderer))
                                    ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseMove);
                            }
                        }
                        else if (treeGrid != null)
                        {
                            ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseMove);
                        }
                        else
                        {
                            ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseMove);
                        }
                        if (!canceled)
                        {
                            Mrci.mouseMoveRowIndex = rowIndex;
                            Mrci.mouseMoveColIndex = colIndex;
                        }
                    }
                }
            }
            grid.AutoScroller.AutoScrollBounds = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            if (grid.AutoScroller.AutoScrollBounds.Contains(point) && !grid.Model.GridContextMenu.IsOpen)
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
            //the below code is added for AutoScrolling is disable when AllowDragColumns is false and click on headerrows then move outside of grid.
            if (datagrid != null && this.CurrentCell.Renderer is GridDataHeaderCellRenderer && !datagrid.AllowDragColumns)
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            
        }

        /// <summary>
        /// Occurs when the mouse button is released in a grid cell.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public virtual void MouseUp(MouseControllerEventArgs e)
        {
            if ((!(CurrentCell.Renderer is GridDataExpandCollapseVisualCellRenderer) && CurrentCell.Renderer is GridDataCellNestedGridRenderer) && (!this.moveToSuccess || !this.Model.Options.AllowSelectionOnMouseUp))
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                return;
            }
             bool isLeftButtonPressed = ((System.Windows.Input.MouseButtonEventArgs)(e.SourceEventArgs)).ChangedButton == MouseButton.Left ? true : false;

            this.Grid.RaiseCellMouseUp(e);
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(start);
            if (canceled)
            {
                if (grid.AutoScroller.AutoScrolling == AutoScrollOrientation.Both)
                {
                    grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                }
                canceled = false;
               
                //In case of Style Disabled, BeginSelectedCells method is not invoked. So we dont have to call EndSelectedCells.
                if (style.Enabled)
                    EndSelectCells(GridSelectionReason.MouseUp);
                return;
            }


            // This is the special case in HitTest where we return a non-zero value
            // when the RoutedEvent is Mouse.PreviewMouseUpEvent even though the
            // click was inside a TextBox.
            var datagrid = this.Grid.FindParentElementOfType<GridDataControl>();
            if (start.IsEmpty)
            {
                if (grid is GridDataControlBaseImpl)
                {
                    if (datagrid.SelectedItems.Count > 0)
                        return;
                }
                GridRangeInfo emptyRange = GridRangeInfo.Empty;
                if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.MouseUp))
                {
                    Selections.Clear(false);
                    RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.MouseUp);
                }
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                return;
            }

            canceled = false;
            Point point = e.Location;
            RowColumnIndex end;

            Rect r = new Rect();
            
            r = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
            if (!r.IsEmpty && (r.Contains(point)) && !(grid is GridTreeControlImpl) && !(grid is GridControl))
            {                
                end = new RowColumnIndex(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
            }
            else
            {
                end = grid.PointToCellRowColumnIndex(point);
            }

            int rowIndex = end.RowIndex;
            int colIndex = end.ColumnIndex;
            var OutsideClick = grid.PointToCellRowColumnIndexOutsideCells(point, false);
            if (!this.CurrentCell.IsEditing && datagrid != null && (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex) && (OutsideClick != null && (OutsideClick.RowIndex >= 0 && OutsideClick.ColumnIndex >= 0)))
            {
                if (!(datagrid.Model.TableProperties.TableSummaryRows.Count > 0) || !(datagrid.Model.TableProperties.TableSummaryPosition == Position.Top ? OutsideClick.RowIndex <= datagrid.Model.TableProperties.TableSummaryRows.Count : OutsideClick.RowIndex >= this.Grid.Model.RowCount - datagrid.Model.TableProperties.TableSummaryRows.Count))
                {
                    ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseUp);
                    if (!canceled)
                    {
                        Mrci.mouseMoveRowIndex = rowIndex;
                        Mrci.mouseMoveColIndex = colIndex;
                    }
                }
            }

            var treeGrid = this.Grid.FindParentElementOfType<GridTreeControl>();
            if (treeGrid != null &&((Keyboard.Modifiers& ModifierKeys.Shift)!= ModifierKeys.None))
            {
                ChangeSelectCells(rowIndex, colIndex, GridSelectionReason.MouseUp);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
            }

            if (this.Model.Options.AllowSelectionOnMouseUp)
            {
                if (rowIndex >= 0 && colIndex >= 0 && rowIndex < grid.Model.RowCount && colIndex < grid.Model.ColumnCount)
                    style = grid.GetRenderStyleInfo(rowIndex, this.Grid.NavigateWithArrowKeysCellsRange.Left);

                GridDataStyleInfo styleinfo = null;
                styleinfo = style.ModelStyle as GridDataStyleInfo;
                if (styleinfo != null && styleinfo.CellIdentity != null)
                {
                    var cellidentity = styleinfo.CellIdentity;
                    if (cellidentity.TableCellType != GridDataTableCellType.NestedTableCell && cellidentity.TableCellType != GridDataTableCellType.DetailsViewCell && (start == end || extendSelection || addNewSelection))
                        BeginSelectCells(rowIndex, colIndex, Keyboard.Modifiers, e, GridSelectionReason.MouseUp);
                }

                if (canceled)
                {
                    return;
                }

                RowColumnIndex startAdjustedForCoveredCell = grid.AdjustCoveredCellRowColumnIndex(start);

                if (!(extendSelection && Model.Options.ExcelLikeSelectionFrame)
                     && (CurrentCell.CellRowColumnIndex != startAdjustedForCoveredCell && this.multiExtendedShouldMoveCurrentCell
                     || (this.CurrentCell.ActivateOptions != null && this.CurrentCell.ActivateOptions.Element == null))) /*(Keyboard.Modifiers != ModifierKeys.Control))*/
                {
                    GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions();
                    options.IsActivateTriggeredByMouseDownIntoUIElement = true;
                    if ((grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                    {
                        options.ShouldBeginEdit = false;
                    }
                    else
                    {
                        options.ShouldBeginEdit = (grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.SetCurrent | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll)) != 0;
                    }
                    bool isMouseLeftButtonClicked = false;
                    if (e.Button != null && e.SourceEventArgs is System.Windows.Input.MouseButtonEventArgs)
                        isMouseLeftButtonClicked = ((System.Windows.Input.MouseButtonEventArgs)(e.SourceEventArgs)).ChangedButton == MouseButton.Left ? true : false;
                    int nhRow = grid.InternalGetHeaderRows();
                    int nhCol = grid.InternalGetHeaderCols();
                    bool bCol = (rowIndex < nhRow && colIndex >= nhCol);
                    bool bRow = ((colIndex < nhCol || GridOptions.ListBoxSelectionMode != GridSelectionMode.None) && rowIndex >= nhRow);
                    if ((!SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)) || !this.Grid.Model.EnableContextMenu || isMouseLeftButtonClicked || bRow || bCol) && (start == end || extendSelection || addNewSelection))
                        moveToSuccess = CurrentCell.MoveTo(end, options);
                }

                if (!moveToSuccess)
                {
                    return;
                }
                var ClickOutside = grid.PointToCellRowColumnIndexOutsideCells(point, false);
                if (datagrid != null)
                {
                    // IsInSummaryPosition(int RowIndex) checks the current row is in Summary row, since summary row should not select.
                    if (!datagrid.Model.IsInSummaryPosition(ClickOutside.RowIndex) && !CurrentCell.IsEditing)
                    {
                        if (CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell) &&
                           ((e.ClickCount == 2 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0) ||
                           (e.ClickCount == 1 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.ClickOnCell) != 0)) ||
                           (e.ClickCount == 1 && style.CellRenderer is GridDataFilterBarCellRenderer))
                            CurrentCell.BeginEdit(true);
                    }
                }
                else
                {
                    if (CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell) &&
                       ((e.ClickCount == 2 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0) ||
                       (e.ClickCount == 1 && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.ClickOnCell) != 0)) ||
                        (e.ClickCount == 1 && style.CellRenderer is GridDataFilterBarCellRenderer))
                        CurrentCell.BeginEdit(true);
                }

                if (!extendSelection && start==end)
                {
                    Mrci.mouseClickRowIndex = rowIndex;
                    Mrci.mouseClickColIndex = colIndex;
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }

                grid.AutoScroller.AutoScrollBounds = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
                if (grid.AutoScroller.InsideScrollBounds.Contains(point))
                    grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
            }

            start = RowColumnIndex.Empty;
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;

            if (!this.Model.Options.AllowSelectionOnMouseUp)
            {
                if (IsHeaderRow(end) && Mrci.mouseClickRowIndex == rowIndex)
                {
                    EndSelectCells(GridSelectionReason.SetCurrentCell);
                }
                else
                    EndSelectCells(GridSelectionReason.MouseUp);

            }

            inSelectingCells = false;

            CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
            if (rowIndex == Mrci.mouseClickRowIndex && colIndex == Mrci.mouseClickColIndex)
            {
                GridRangeInfo newRange = GridRangeInfo.Empty;
                if (rowIndex >= grid.InternalGetHeaderRows() && colIndex >= grid.InternalGetHeaderCols())
                {
                    Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out newRange);
                    //this.AdjustRange(SelectionType.IsCells, rowIndex, colIndex, ref newRange);
                }
                else if (rowIndex >= grid.InternalGetHeaderRows())
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                    //this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
                }
                else if (colIndex >= grid.InternalGetHeaderCols())
                {
                    newRange = GridRangeInfo.Col(colIndex);
                    //this.AdjustRange(SelectionType.IsCol, rowIndex, colIndex, ref newRange);
                }
                else
                    newRange = GridRangeInfo.Empty;

                /// Code added for fix in incident 77868
                if (Keyboard.Modifiers == ModifierKeys.Control && Selections.Ranges.Contains(newRange))
                {
                    Selections.Ranges.Remove(newRange);
                }

                if (!(newRange.IsCols && (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)) &&                 
                  ( newRange.IsRows && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                   || newRange.IsCols && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                   || newRange.IsCells && (!this.Grid.Model.EnableContextMenu || isLeftButtonPressed) && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                {
                    if (GridOptions.ExcelLikeCurrentCell && !Selections.Ranges.AnyRangeContains(newRange))// && !this.CurrentCell.IsEditing)
                    {
                        if (Selections.Ranges.Count > 0 && GridOptions.ListBoxSelectionMode!= GridSelectionMode.MultiSimple)
                        {
                            if (Selections.Ranges[Selections.Ranges.Count - 1].RangeType != newRange.RangeType)
                            {
                                Selections.Clear();
                            }
                        }
                        // Fix for WPF-11179- Issue with Multiple Selection in GDC with Grouping
                        // Previous Code  if (newRange.RangeType == GridRangeInfoType.Rows && !(grid is GridControl) )
                        //I have changed this like following. Because If RangeType is Cell menas New Range doesnt added to the selection.
                        //if (newRange.RangeType != GridRangeInfoType.Cols && !(grid is GridControl) )
                        //{
                        //    var datagri = this.Grid.FindParentElementOfType<GridDataControl>();
                        //    if (datagri != null)
                        //    {
                        //        if (!(datagri.Model.Table.HasGroups))
                        //        {
                        //           Selections.Add(newRange);
                        //        }
                        //        else if (newRange.RangeType == GridRangeInfoType.Cells && this.Grid.Model.Options.ListBoxSelectionMode == GridSelectionMode.None && datagri.Model.CurrencyManager.IsRecordCell)
                        //        {
                        //            Selections.Add(newRange);
                        //        }
                        //    }
                        //}
                        //else
                        {
                            if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                Selections.Add(newRange);
                            }
                            else
                            {
                                if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                                    Selections.Clear();
                                Selections.Add(newRange);
                            }
                        }
                    }

                    else if (Selections.Ranges.Contains(newRange))
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            Selections.Ranges.Remove(newRange);
                        }
                        else
                        {
                            if (!newRange.Equals(this.SelectedRanges.ActiveRange))
                            {
                                if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                                    Selections.Clear();
                                Selections.Add(newRange);
                            }
                        }
                    }                  
                }

                r = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
                var ClickOutside = grid.PointToCellRowColumnIndexOutsideCells(point, false);
               // var datagrid = this.Grid.FindParentElementOfType<GridDataControl>();
                bool flag = true;
                if (datagrid != null)
                {
                    if (datagrid.Model.TableProperties.TableSummaryRows.Count > 0)
                    {
                        if ((datagrid.Model.TableProperties.TableSummaryPosition == Position.Bottom && ClickOutside.RowIndex >= this.Grid.Model.RowCount - datagrid.Model.TableProperties.TableSummaryRows.Count)
                          || (datagrid.Model.TableProperties.TableSummaryPosition == Position.Top && (ClickOutside.RowIndex <= datagrid.Model.TableProperties.TableSummaryRows.Count)))
                        {
                            flag = false;
                        }


                    }
                }

                if (flag)
                {
                    IGridCellRenderer cellRenderer = Grid.GetRenderStyleInfo(rowIndex, colIndex).CellRenderer;
                    cellRenderer.RaiseGridCellClick(rowIndex, colIndex, e);
                }                
               
                //this.Grid.RaiseGridCellClick(rowIndex,colIndex);
                if (e.Handled)
                    return;
            }
#if !SILVERLIGHT
            if (!grid.IsKeyboardFocusWithin &&(!grid.Model.EnableContextMenu ||isLeftButtonPressed))
                grid.Focus();
#else
            if (!grid.IsKeyboardFocusWithin )
                grid.Focus();
#endif         
            //this.Grid.InvalidateCell(GridRangeInfo.Col(0));
        }

        /// <summary>
        /// Check if the row is header row in GridDataControl
        /// </summary>
        protected virtual bool IsHeaderRow(RowColumnIndex end)
        {
            return false;
        }

        /// <summary>
        /// Cancels the grid selection.
        /// </summary>
        public void CancelMode()
        {
            this.Grid.RaiseCellCancelMode();
            TraceUtil.TraceCurrentMethodInfo();
            suspendState = new SuspendState(this);
            Selections.Clear();
            start = RowColumnIndex.Empty;

            if (this.Mrci != null && Mrci.mouseDownRowIndex < this.Model.RowCount && Mrci.mouseDownColIndex < this.Model.ColumnCount)
            {
                var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                if (dataGrid != null && this.Mrci.mouseDownRowIndex == this.Mrci.mouseMoveRowIndex &&
                    this.Mrci.mouseDownColIndex == this.Mrci.mouseMoveColIndex)
                {
                    if (Model.Options.ListBoxSelectionMode == GridSelectionMode.None &&
                        (Model.Options.AllowSelection == GridSelectionFlags.Any ||
                         Model.Options.AllowSelection == GridSelectionFlags.Cell))
                    {
                        var newRange = GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex,
                                                                    this.Mrci.mouseDownColIndex);
                        Selections.ChangeSelection(SelectedRanges.ActiveRange, newRange, GridSelectionReason.MouseMove);
                        RaiseSelectionChanged(SelectedRanges.ActiveRange, GridSelectionReason.MouseMove);
                    }
                }
            }
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            inSelectingCells = false;
            canceled = true;
        }

        /// <summary>
        /// Restores the grid selection.
        /// </summary>
        public void RestoreMode()
        {
            if (suspendState != null)
                suspendState.Restore();
            suspendState = null;
            this.Grid.RaiseCellRestoreMode();
        }

        /// <summary>
        /// Checks if the mouse is over a UIElement inside a cell and if yes, it will let that UI element
        /// handle the mouse events when the user hovers mouse.
        /// </summary>
        /// <param name="mouseEventArgs">A <see cref="MouseControllerEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.</returns>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            //TraceUtil.TraceCurrentMethodInfo(mouseEventArgs.Location);
            if (controller != null)
                return 0;

            Point point = mouseEventArgs.Location;
            RowColumnIndex end = grid.PointToCellRowColumnIndexOutsideCells(point, false);

            if (end.IsEmpty)
            {
                end = new RowColumnIndex(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
            }


            // If mouse if over a TextBox (or any other child UIElement)
            // inside a cell let that TextBox handle the mouse events when 
            // the user hovers mouse. IsMouseOverChildElement will be true
            // in this case and DirectlyOverRenderer will not be null.
            //
            // If mouse was pressed down in that TextBox and the user 
            // moved the mouse cursor outside the textbox over another cell then let 
            // HitTest proceed. IsMouseOverChildElement will be false in this case.
            // Once HitTest proceeds and returns non-zero value the grid will try 
            // to cancel the MouseCapture for the TextBox and start selecting cells instead.
            //
            // In the case that shift or ctrl key is pressed then grab the mouse action
            // no matter if mouse is pressed inside a textbox or not.
            //
            // TODO: With ListBoxSelectionMode I right now also use brute force to
            // grab the mouse action meaning that clicking on checkboxes or pushbutton
            // will not trigger click events for these renderers. Some finetuning
            // is needed here.
            if (mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.DirectlyOverRenderer != null
                && !(mouseEventArgs.DirectlyOverRenderer is GridCellDataTemplateRenderer) 
                //&& (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0)
                //&& GridOptions.ListBoxSelectionMode == GridSelectionMode.None
                )
            {
                // Trying to implement fine-tuned implementation for ListBoxSelectionMode here.
                bool listModeCriteria = true;
                if (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)
                {
                    listModeCriteria = GridOptions.ListBoxModeAllowUIElementClick;
                }

                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0)
                {
                    IInputElement ie = mouseEventArgs.SourceEventArgs.Source as IInputElement;
                    if ((ie.IsKeyboardFocusWithin || ie.IsMouseDirectlyOver)&& !(mouseEventArgs.DirectlyOverRenderer is GridCellHyperlinkCellRenderer))
                        listModeCriteria = false;

                    // Another option here would be to check if UIElement belongs to current cell
                    // and if it is focused.
                    // Or we could also add an attached property that we could check.
                    // SHIFT and CTRL-Selection will stop working for these cells. Further
                    // fine-tuning might be needed.
                }

                if (listModeCriteria && !end.IsEmpty)
                {
                    if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent
                        || mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseUpEvent)
                    {
                        // Clearing selection and setting current range in case of ExcelLikeCurrentCell option.
                        // I do this here in HitTest because I do only want to quickly hook into the
                        // flow when the user clicked inside a TextBox. The SelectCellsMouseController 
                        // does not want to handle the MouseDown, MouseMove at that time.  The SelectCellsMouseController
                        // waits until the user moves the mouse outside the textbox to start processing
                        // mouse events. But I need to reset the selection at the time the user clicks
                        // down the mouse (and also possibly select a range containing the current cell in  
                        // the case that ExcelLikeCurrentCell is en
                        //if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
                        //{
                        //    if (!grid.CurrentCell.HasCurrentCellAt(end.RowIndex, end.ColumnIndex))
                        //    {
                        //        GridRangeInfo emptyRange = GridRangeInfo.Empty;
                        //        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.SetCurrentCell))
                        //        {
                        //           RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
                        //        }
                        //    }

                        //    if (GridOptions.ExcelLikeCurrentCell)
                        //    {
                        //        // Be aware that when you cancel the current cell activation in CurrentCellActivating
                        //        // event that the selection already occured with ProcessSetCurrentCell. If you do not
                        //        // want this you should also handle SelectionChanging event.
                        //        ProcessSetCurrentCell(end.RowIndex, end.ColumnIndex, GridActivateCurrentCellOptions.Empty);
                        //    }
                        //}
                        Mrci.mouseDownRowIndex = end.RowIndex;
                        Mrci.mouseDownColIndex = end.ColumnIndex;
                        Mrci.mouseClickRowIndex = end.RowIndex;
                        Mrci.mouseClickColIndex = end.ColumnIndex;
                        Mrci.mouseMoveRowIndex = end.RowIndex;
                        Mrci.mouseMoveColIndex = end.ColumnIndex;
                        Mrci.firstClick = false;

                        if (mouseEventArgs.DirectlyOverRenderer is GridCellHyperlinkCellRenderer || mouseEventArgs.DirectlyOverRenderer is GridCellCheckboxRenderer)
                            return 2;
                    }

                    if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.MouseDownEvent)
                    {
                        return 2;
                    }
                }
                else if (!listModeCriteria
                    && !end.IsEmpty
                    && mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.MouseDownEvent)
                {
                    return 2;
                }

                return 0;
            }

            // In the case that user clicked on a rendered cell and there is no UIElement
            // under the mouse, do not handle the event during PreviewMouseDown. Instead
            // wait for the following MouseDown. Waiting for MouseDown gives programmers 
            // the chance to handle the PreviewMouseDown in an attached event handler 
            // and possible prevent the grid from handling it.
            if (!mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
                return 0;

            if (!end.IsEmpty)
                return 2;
            return 0;
        }

        /// <summary>
        /// Allows to cancel the mouse capture.
        /// </summary>
        public bool SupportsCancelMouseCapture
        {
            get { return true; }
        }

        /// <summary>
        /// Allows to track the mouse.
        /// </summary>
        public bool SupportsMouseTracking
        {
            get { return true; }
        }

        #endregion

        #region SuspendState
        /// <summary>
        /// A helper class that tends to suspend a grid selection. It takes a back up of the selection
        /// when the selection is suspended and restores the grid with the selections when the
        /// selection is resumed.
        /// </summary>
        public class SuspendState
        {
            GridSelectCellsMouseController mc;
            RowColumnIndex start;
            GridRangeInfoList selectedCells;
            AutoScrollOrientation autoScroll;

            /// <summary>
            /// Suspends the grid selection.
            /// </summary>
            /// <param name="mc">The selection mouse controller.</param>
            public SuspendState(GridSelectCellsMouseController mc)
            {
                this.mc = mc;
                start = mc.start;
                autoScroll = mc.grid.AutoScroller.AutoScrolling;
                selectedCells = mc.SelectedRanges.Clone();
            }

            /// <summary>
            /// Restores the grid selection.
            /// </summary>
            public void Restore()
            {
                mc.start = start;
                foreach (GridRangeInfo range in selectedCells)
                    mc.SelectedRanges.Add(range);
                mc.grid.AutoScroller.AutoScrolling = autoScroll;
                mc.grid.RenderCurrentCellBorder();
                mc.inSelectingCells = true;
                mc.canceled = false;
            }
        }
        #endregion

        #region MouseRowColInfo

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
                return String.Format("{0},{1}->{2},{3}",
                    mouseDownRowIndex,
                    mouseDownColIndex,
                    mouseMoveRowIndex,
                    mouseMoveColIndex);
            }
        }

        #endregion
    }

}
