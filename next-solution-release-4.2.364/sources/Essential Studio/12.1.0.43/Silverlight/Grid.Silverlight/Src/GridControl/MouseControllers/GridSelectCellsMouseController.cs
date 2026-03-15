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
#if!WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.ComponentModel;
using System.Windows.Interop;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.Controls.Grid;
using Windows.System;
using Syncfusion.WinRT.Controls.Cells;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Syncfusion.WinRT.ComponentModel;
using Windows.UI.Xaml;
using Windows.Devices.Input;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    class GridSelectCellsMouseController : IMouseController, IDisposable
    {
        #region Fields
        bool inSelectingCells;
        bool addNewSelection;
        bool extendSelection;
        bool canceled;
        bool multiExtendedShouldMoveCurrentCell = true;
        MouseRowColInfo mrci = null;
        GridControlBase grid;
        RowColumnIndex start = RowColumnIndex.Empty;
        SuspendState suspendState;
        #endregion

        #region Ctor
        public GridSelectCellsMouseController(GridControlBase grid)
        {
            this.grid = grid;
            grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            //grid.CurrentCellActivated += new GridRoutedEventHandler(GridCurrentCellActivated);
            grid.ExternalMove = new GridCurrentCellMoveDelegateHandler(GridCurrentCellExternalMove);
#if WinRT
            grid.AutoScroller.AutoScrollerValueChanged += AutoScroller_AutoScrollerValueChanged;
#endif

        }

#if WinRT
        void AutoScroller_AutoScrollerValueChanged(object sender, AutoScrollerValueChangedEventArgs args)
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
                ChangeSelectCells(rowIndex, colIndex);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
            }
        }
#endif
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
#if WinRT
            grid.AutoScroller.AutoScrollerValueChanged -= AutoScroller_AutoScrollerValueChanged;
#endif
            //grid.CurrentCellActivated -= new GridRoutedEventHandler(GridCurrentCellActivated);
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

        MouseControllerDispatcher MouseControllerDispatcher
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

        public bool MultiExtendedShouldMoveCurrentCell
        {
            get { return multiExtendedShouldMoveCurrentCell; }
            set { multiExtendedShouldMoveCurrentCell = value; }
        }
        #endregion

        #region ToString
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
            if (!CurrentCell.IsInMoveTo)
                ProcessSetCurrentCell(CurrentCell.RowIndex, CurrentCell.ColumnIndex, GridActivateCurrentCellOptions.Empty);
        }

        protected virtual void ProcessSetCurrentCell(int rowIndex, int colIndex, GridActivateCurrentCellOptions flags)
        {
            if (inSelectingCells)
                return;

            if ((flags.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == GridSetCurrentCellOptions.NoSelectRange)
                return;
#if DEBUG
            //if (!BrowserInteropHelper.IsBrowserHosted)
            //{
            //    if (Switches.SelectRange.TraceVerbose)
            //        TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, flags, this);
            //}
#endif

            if (GridOptions.ExcelLikeCurrentCell && GridOptions.ListBoxSelectionMode == GridSelectionMode.None)
            {
                if (rowIndex == -1 || colIndex == -1 || !SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    GridRangeInfo newRange;
                    if (rowIndex >= grid.InternalGetHeaderRows() && colIndex >= grid.InternalGetHeaderCols() && GridOptions.AllowSelection != GridSelectionFlags.Row)
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
                            Selections.ChangeSelection(GridRangeInfo.Empty, newRange, GridSelectionReason.SetCurrentCell);
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
                        RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
                    }
                    if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.SetCurrentCell))
                    {
                        // Move Edit and set selection to new row.
                        GridRangeInfo newRange = GridRangeInfo.Row(rowIndex);
                        this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
                        Selections.ChangeSelection(activeRange, newRange, GridSelectionReason.ArrowKey);
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
            extendSelection &= (GridOptions.AllowSelection & GridSelectionFlags.Keyboard) != GridSelectionFlags.None
                && GridOptions.ListBoxSelectionMode != GridSelectionMode.One
                || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended;

            // Grid in list box mode.
            if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One
                || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended
                )
            {
                if (GridOptions.MulitExtendedArrowKeySelect)
                {
                    GridRangeInfo activeRange = selectedRanges.ActiveRange;

                    if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                    {
                        bool horizontal = direction == GridDirectionType.Left || direction == GridDirectionType.Right
                            || direction == GridDirectionType.MostLeft || direction == GridDirectionType.MostRight;

                        if (horizontal)
                        {
                            return CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                        }

                        if (selectedRanges.Count == 0 || !extendSelection)
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

                        if (RaiseSelectionChanging(ref activeRange, GridSelectionReason.ArrowKey))
                        {
                            // Move Edit and set selection to new row.
                            inSelectingCells = true;
                            CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                            bool success = CurrentCell.GetCurrentCell(out rowIndex, out colIndex);
#if !WinRT
                            if (!(CurrentCell.Renderer is GridDataCellNestedGridRenderer))
                            {
                                GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex, rowIndex);
                                Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, GridSelectionReason.ArrowKey);
                                //this.grid.RaiseCurrentCellMoved(new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                                this.Mrci.mouseMoveRowIndex = rowIndex;
                                this.Mrci.mouseMoveColIndex = colIndex;

                                // Fire event.
                                RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.ArrowKey);
                            }
#endif
                            grid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);

                            if (!extendSelection)
                            {
                                this.Mrci.mouseDownRowIndex = rowIndex;
                                this.Mrci.mouseDownColIndex = colIndex;
                            }
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
                        CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange));

                        CurrentCell.GetCurrentCell(out rowIndex, out colIndex);

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

        #endregion

        #region Begin/End/Change Selection
        public enum SelectionType
        {
            IsCells,
            IsRow,
            IsCol,
            IsTable
        }

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

#if !WinRT
        void BeginSelectCells(int rowIndex, int colIndex, ModifierKeys modifierKeys)
#else
        void BeginSelectCells(int rowIndex, int colIndex)
#endif
        {
#if WinRT
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            CoreVirtualKeyStates shift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift);
#endif
            GridRangeInfoList selectedRanges = SelectedRanges;
            selectedRanges.RemoveEmptyRanges();

            if (rowIndex == 0 && colIndex == 0)
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
                        this.Mrci.mouseClickColIndex = grid.LeftColIndex;
                        this.Mrci.mouseDownColIndex = grid.LeftColIndex;
                        this.Mrci.mouseClickRowIndex = grid.TopRowIndex;
                        this.Mrci.mouseDownRowIndex = grid.TopRowIndex;
                        this.Mrci.mouseMoveRowIndex = Mrci.mouseDownRowIndex;
                        this.Mrci.mouseMoveColIndex = Mrci.mouseDownColIndex;
                        // ... Selections.SelectRange will trigger Selections.OnSelectionChanged event

                        return;
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
#if !WinRT
                addNewSelection = (modifierKeys & ModifierKeys.Control) != ModifierKeys.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
#else
                addNewSelection = (ctrl.HasFlag(CoreVirtualKeyStates.Down));
                extendSelection = !Mrci.firstClick && (shift.HasFlag(CoreVirtualKeyStates.Down));
#endif
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
#if !WinRT
                addNewSelection = (modifierKeys & ModifierKeys.Control) != ModifierKeys.None && (GridOptions.AllowSelection & GridSelectionFlags.Multiple) != GridSelectionFlags.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None && (GridOptions.AllowSelection & GridSelectionFlags.Shift) != GridSelectionFlags.None;
#else
                addNewSelection = (ctrl.HasFlag(CoreVirtualKeyStates.Down)) && (GridOptions.AllowSelection & GridSelectionFlags.Multiple) != GridSelectionFlags.None;
                extendSelection = !Mrci.firstClick && (shift.HasFlag(CoreVirtualKeyStates.Down)) && (GridOptions.AllowSelection & GridSelectionFlags.Shift) != GridSelectionFlags.None;
#endif
            }

            Mrci.firstClick = false;

            if (GridOptions.AllowSelection != GridSelectionFlags.None)
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
                    clickRange = GridRangeInfo.Table();
                    this.AdjustRange(SelectionType.IsTable, rowIndex, colIndex, ref clickRange);
                }


                GridRangeInfo tempRange;
                Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out tempRange);

                bool resetSelection = false;
                if (selectedRanges.Count == 1 && selectedRanges[0] != tempRange
                    || selectedRanges.Count > 1)
                    resetSelection = !extendSelection && !addNewSelection
                        || (GridOptions.AllowSelection & GridSelectionFlags.MixRangeType) == 0 && selectedRanges.FilterRangeType(clickRange.RangeType).Count != selectedRanges.Count;

                // First, remove all selections if neither SHIFT or CTRL is pressed
                // or if the clicked range type differs from previous selected ranges.
                //Console.WriteLine("reset selection {0}", resetSelection);
                if (resetSelection && !addNewSelection)
                {
                    extendSelection = addNewSelection = false;
                    GridRangeInfo emptyRange = GridRangeInfo.Empty;
                    if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.MouseDown, clickRange))
                    {
                        Selections.Clear(false);
                        RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.MouseDown);
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
                        Selections.Remove(clickRange);
                        SetMrciMouseDownClick(clickRange.Top, clickRange.Left);
                        this.Mrci.mouseMoveRowIndex = clickRange.Top;
                        this.Mrci.mouseMoveColIndex = clickRange.Left;
                        canceled = true;
                        return;
                    }

                    // If user presses CTRL-Key while no range was selected
                    // but the current cell was visible, select the current
                    // cell.
                    if (!resetSelection &&
                        !extendSelection && selectedRanges.Count == 0 && CurrentCell.GetCurrentCell(out ncRow, out ncCol) &&
                        !CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && GridOptions.ListBoxSelectionMode == GridSelectionMode.None)
                    {
                        GridRangeInfo range = GridRangeInfo.Cell(ncRow, ncCol);
                        if (ncRow >= nhRow && ncCol >= nhCol && (GridOptions.AllowSelection != GridSelectionFlags.Cell && !bCell))
                        {
                            if (RaiseSelectionChanging(ref range, GridSelectionReason.MouseDown, clickRange))
                            {
                                Selections.ChangeSelection(GridRangeInfo.Empty, range, GridSelectionReason.MouseDown);
                                RaiseSelectionChanged(range, GridSelectionReason.MouseDown);
                            }
                            else
                                // Current cell can't be selected. Continue as if CTRL is not pressed.
                                addNewSelection = false;
                        }
                    }
                }

                // Check, if selection is allowed.
                if (
                    (bTable || clickRange.IsTable) && (GridOptions.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None
                    || (bRow || clickRange.IsRows) && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                    || (bCol || clickRange.IsCols) && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                    || (bCell || clickRange.IsCells) && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None)
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

                    //#if DEBUG
                    //                    if (!BrowserInteropHelper.IsBrowserHosted)
                    //                    {
                    //                        Trace.WriteLineIf(extendSelection && Switches.SelectRange.TraceVerbose, "ClickRange: " + clickRange.ToString());
                    //                    }
                    //#endif


                    GridRangeInfo savedRange = selectedRanges.ActiveRange;
                    GridRangeInfo newRange = clickRange.UnionRange(GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex));

                    if ((!GridOptions.ExcelLikeSelectionFrame || resetSelection)
                        && !addNewSelection &&
                        newRange.IsCells && newRange.Width == 1 && newRange.Height == 1)
                    {
                        ///This conditon check will enable you to set the SelectedItems when one item selected in GridDataControl.
                        if (GridOptions.ListBoxSelectionMode != GridSelectionMode.One && GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiExtended)
                            newRange = GridRangeInfo.Empty;
                    }

                    // Allow programmer to change the range.
                    if (RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseDown, newRange))
                    {
                        Selections.ChangeSelection(savedRange, newRange, GridSelectionReason.MouseDown);

                        // Trigger Selections.OnSelectionChanged event.
                        RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseDown);
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

        bool ChangeSelectCells(int rowIndex, int colIndex)
        {
            if (GridOptions.AllowSelection != GridSelectionFlags.None // selecting cells enabled?
                && inSelectingCells)
            {
                GridRangeInfoList selectedRanges = SelectedRanges;

                int nhRow = grid.InternalGetHeaderRows();
                int nhCol = grid.InternalGetHeaderCols();
                int nfRow = grid.InternalGetFrozenRows();
                int nfCol = grid.InternalGetFrozenCols();

                GridRangeInfo savedRange = selectedRanges.ActiveRange;
                GridRangeInfo newRange;

                rowIndex = Math.Max(nhRow, rowIndex);
                //if (this.Mrci.mouseDownRowIndex >= nfRow && grid.TopRowIndex >= nfRow)
                //    rowIndex = Math.Max(grid.TopRowIndex, rowIndex);
                colIndex = Math.Max(nhCol, colIndex);
                //if (this.Mrci.mouseDownColIndex > nfCol && grid.LeftColIndex >= nfCol)
                //    colIndex = Math.Max(grid.LeftColIndex, colIndex);

                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One)
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                    this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
                }
                else if (savedRange.IsCols)
                {
                    newRange = GridRangeInfo.Cols(colIndex, this.Mrci.mouseDownColIndex);
                    this.AdjustRange(SelectionType.IsCol, rowIndex, colIndex, ref newRange);
                }
                else if (savedRange.IsRows || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
                {
                    newRange = GridRangeInfo.Rows(rowIndex, this.Mrci.mouseDownRowIndex);
                    this.AdjustRange(SelectionType.IsRow, rowIndex, colIndex, ref newRange);
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
                if (RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseMove, newRange))
                {
                    Selections.ChangeSelection(selectedRanges.ActiveRange, newRange, GridSelectionReason.MouseMove);

                    // Trigger Selections.OnSelectionChanged event.
                    RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseMove);
                    bool isValidationMsgShown = false;
                    if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One
                        || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended && multiExtendedShouldMoveCurrentCell)
                    {
                        GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange | /*GridSetCurrentCellOptions.NoSetFocus |*/ GridSetCurrentCellOptions.NoSyncCurrentCell);
                        options.ShouldBeginEdit = false;
                        CurrentCell.MoveTo(rowIndex, CurrentCell.ColumnIndex, options, out isValidationMsgShown);
                        CurrentCell.Grid.RaiseCurrentCellMoved(options);
                    }
                }

                return true;
            }
            return false;
        }

        void EndSelectCells()
        {
            GridRangeInfoList selectedRanges = SelectedRanges;

            CurrentCell.AdjustRowColIfCoveredCell(ref Mrci.mouseMoveRowIndex, ref Mrci.mouseMoveColIndex);
            int nhRow = grid.InternalGetHeaderRows();
            nhRow = nhRow > 0 ? nhRow - 1 : nhRow;
            int nhCol = grid.InternalGetHeaderCols();
            nhCol = nhCol > 0 ? nhCol - 1 : nhCol;

            if (GridOptions.AllowSelection != GridSelectionFlags.None
                && inSelectingCells
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
                        if (selectedRanges.Count > 0 && RaiseSelectionChanging(ref range, GridSelectionReason.MouseUp))
                        {
                            // User presses ctrl-key. Select the current cell.
                            Selections.ChangeSelection(selectedRanges.ActiveRange, range, GridSelectionReason.MouseUp);

                            // Trigger Selections.OnSelectionChanged event.
                            RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseUp);
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
                    RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseUp);
                }
            }
        }

        #endregion

        #region IMouseController Members

        public string Name
        {
            get { return "SelectCellsMouseController"; }
        }
#if!WinRT
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
        }

        private Cursor _cursor = null; 
#else
        public CoreCursor Cursor
        {
            get
            {
                return new CoreCursor(CoreCursorType.Arrow,1);
            }
        }
#endif

        public void MouseHoverEnter(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverEnter(e);
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
            this.Grid.RaiseCellMouseHover(e);
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverLeave(e);
        }

        /// <summary>
        /// isValidationMsgShown is True when the validation message shown. In Silverlight we don't have the support for checking the button state
        /// so that this flag was used to remove the selection while mouse over the grid.
        /// </summary>
        bool isValidationMsgShown = false;
        bool moveToSuccess = true;
        virtual public void MouseDown(MouseControllerEventArgs e)
        {
            this.Grid.RaiseCellMouseDown(e);
            if (e.Handled)
                return;
            canceled = false;
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
#if!WinRT
            Rect r = new Rect();
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

            if (grid is GridDataCellNestedGridEditor)
            {
                //In nested grid, after selecting parent row if we select child means we should move the corresponding nested table row.
                var parentGrid = (grid as GridDataCellNestedGridEditor).ParentGrid;
                if (parentGrid.Model.SelectedRanges.Count > 0)
                {
                    var gridLocation = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual);
                    var poi = DependencyObjectExtensions.PointFromRootVisual(parentGrid);
                    gridLocation.X -= poi.X;
                    gridLocation.Y -= poi.Y;
                    var rowColIndex = parentGrid.PointToCellRowColumnIndex(gridLocation);
                    parentGrid.CurrentCell.MoveTo(rowColIndex.RowIndex, rowColIndex.ColumnIndex);
                    
                }
            }
#else
            start = grid.PointToCellRowColumnIndex(point);
#endif
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(start);
            IGridCellRenderer renderer = style.CellRenderer;
            IHitTestSelectCells hsc = renderer as IHitTestSelectCells;
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
#if!WinRT
            style = grid.GetRenderStyleInfo(rowIndex, this.Grid.NavigateWithArrowKeysCellsRange.Left);
            if (!(style.CellRenderer is GridDataCellNestedGridRenderer))
                BeginSelectCells(rowIndex, colIndex, Keyboard.Modifiers);
#else
            BeginSelectCells(rowIndex, colIndex);
#endif

            if (canceled)
            {
                return;
            }

            moveToSuccess = true;
            if (CurrentCell.CellRowColumnIndex != startAdjustedForCoveredCell
                && this.multiExtendedShouldMoveCurrentCell) /*(Keyboard.Modifiers != ModifierKeys.Control))*/
            {
                GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions();
                if ((this.Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                {
                    options.ShouldBeginEdit = false;
                }
                else
                {
                    options.ShouldBeginEdit = (this.Grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.SetCurrent | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll)) != 0;
                }

                moveToSuccess = CurrentCell.MoveTo(start, options, out isValidationMsgShown);
                CurrentCell.Grid.RaiseCurrentCellMoved(options);
            }

            if (e.ClickCount == 2
                && CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell)
                && (grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0
                || (e.ClickCount == 1
#if!WinRT
                && renderer is GridDataFilterBarCellRenderer))
#else
) && !(Model is GridModel))
#endif
                CurrentCell.BeginEdit(true);

            if (!extendSelection)
            {
                Mrci.mouseClickRowIndex = rowIndex;
                Mrci.mouseClickColIndex = colIndex;
                Mrci.mouseMoveRowIndex = rowIndex;
                Mrci.mouseMoveColIndex = colIndex;
            }

            /*
            grid.AutoScroller.AutoScrollBounds = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            if (grid.AutoScroller.InsideScrollBounds.Contains(point))
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;*/
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            this.Grid.RaiseCellMouseMove(e);
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            VirtualizingCellsControl cellsControl = grid;// as VirtualizingCellsControl;

            if (!start.IsEmpty && !isValidationMsgShown)
            {
                RowColumnIndex end = grid.PointToCellRowColumnIndex(point);

                if (!end.IsEmpty)
                {
                    int rowIndex = end.RowIndex;
                    int colIndex = end.ColumnIndex;
                    if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex)
                    {
                        ChangeSelectCells(rowIndex, colIndex);
                        if (!canceled)
                        {
                            Mrci.mouseMoveRowIndex = rowIndex;
                            Mrci.mouseMoveColIndex = colIndex;
                        }
                    }
                }
            }
            grid.AutoScroller.AutoScrollBounds = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            if (grid.AutoScroller.InsideScrollBounds.Contains(point))
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            if (!moveToSuccess)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                return;
            }
            this.Grid.RaiseCellMouseUp(e);
            if (canceled || e.Handled)
            {
                canceled = false;
                return;
            }

#if WinRT
            bool isCtrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
            bool isShift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
#endif
            // This is the special case in HitTest where we return a non-zero value
            // when the RoutedEvent is Mouse.PreviewMouseUpEvent even though the
            // click was inside a TextBox.
            if (start.IsEmpty)
            {
                GridRangeInfo emptyRange = GridRangeInfo.Empty;
                if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.ArrowKey))
                {
                    Selections.Clear(false);
                    RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.ArrowKey);
                }
                return;
            }

            canceled = false;
            Point point = e.Location;
            RowColumnIndex end = grid.PointToCellRowColumnIndex(point);

            int rowIndex = end.RowIndex;
            int colIndex = end.ColumnIndex;
            if ((rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex) && !isValidationMsgShown)
            {
                ChangeSelectCells(rowIndex, colIndex);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
            }

            start = RowColumnIndex.Empty;
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;

            EndSelectCells();
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
#if !WinRT
                if (Keyboard.Modifiers == ModifierKeys.Control && Selections.Ranges.Contains(newRange))
#else
                if ((isCtrl) && Selections.Ranges.Contains(newRange))
#endif
                {
                    Selections.Ranges.Remove(newRange);
                }

                if (!(newRange.IsCols && (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)) &&
                  (newRange.IsRows && (GridOptions.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                   || newRange.IsCols && (GridOptions.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                   || newRange.IsCells && (!isValidationMsgShown) && (GridOptions.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                {
                    if (GridOptions.ExcelLikeCurrentCell && !Selections.Ranges.AnyRangeContains(newRange))
                    {
                        if (Selections.Ranges.Count > 0 && GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                        {
                            if (Selections.Ranges[Selections.Ranges.Count - 1].RangeType != newRange.RangeType)
                            {
                                Selections.Clear();
                            }
                        }
                        // Previous Code  if (newRange.RangeType == GridRangeInfoType.Rows && !(grid is GridControl) )
                        //I have changed this like following. Because If RangeType is Cell menas New Range doesnt added to the selection.
#if !WinRT
                        if (newRange.RangeType != GridRangeInfoType.Cols && (grid is GridDataControl))
                        {
                            var datagri = this.Grid.FindParentElementOfType<GridDataControl>();
                            if (datagri != null)
                            {
                                if (!(datagri.Model.Table.HasGroups))
                                {
                                    Selections.Add(newRange);
                                }
                                else if (newRange.RangeType == GridRangeInfoType.Cells && this.Grid.Model.Options.ListBoxSelectionMode == GridSelectionMode.None && datagri.Model.CurrencyManager.IsRecordCell)
                                {
                                    Selections.Add(newRange);
                                }
                            }
                        }
                        else
#endif
                        {
#if !WinRT
                            if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
#else
                            if (isCtrl || isShift)
#endif
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
#if !WinRT
                        if (Keyboard.Modifiers == ModifierKeys.Control)
#else
                        if (isCtrl)
#endif
                        {
                            Selections.Ranges.Remove(newRange);
                        }
                        else
                        {
                            if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                                Selections.Clear();
                            Selections.Add(newRange);
                        }
                    }
                }

                IGridCellRenderer cellRenderer = Grid.GetRenderStyleInfo(rowIndex, colIndex).CellRenderer;
                cellRenderer.RaiseGridCellClick(rowIndex, colIndex, e);
                //this.Grid.RaiseGridCellClick(rowIndex,colIndex);
                if (e.Handled)
                    return;

            }
#if WPF
            if (!grid.IsKeyboardFocusWithin)
                grid.Focus();
#endif
        }

        public void CancelMode()
        {
            this.Grid.RaiseCellCancelMode();
            suspendState = new SuspendState(this);
            Selections.Clear();
            start = RowColumnIndex.Empty;
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            inSelectingCells = false;
            canceled = true;
        }

        public void RestoreMode()
        {
            if (suspendState != null)
                suspendState.Restore();
            suspendState = null;
            this.Grid.RaiseCellRestoreMode();
        }

        //public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        //{
        //    //TraceUtil.TraceCurrentMethodInfo(mouseEventArgs.Location);
        //    if (controller != null)
        //        return 0;

        //    Point point = mouseEventArgs.Location;
        //    RowColumnIndex end = grid.PointToCellRowColumnIndexOutsideCells(point, false);

        //    // If mouse if over a TextBox (or any other child UIElement)
        //    // inside a cell let that TextBox handle the mouse events when 
        //    // the user hovers mouse. IsMouseOverChildElement will be true
        //    // in this case and DirectlyOverRenderer will not be null.
        //    //
        //    // If mouse was pressed down in that TextBox and the user 
        //    // moved the mouse cursor outside the textbox over another cell then let 
        //    // HitTest proceed. IsMouseOverChildElement will be false in this case.
        //    // Once HitTest proceeds and returns non-zero value the grid will try 
        //    // to cancel the MouseCapture for the TextBox and start selecting cells instead.
        //    //
        //    // In the case that shift or ctrl key is pressed then grab the mouse action
        //    // no matter if mouse is pressed inside a textbox or not.
        //    //
        //    // TODO: With ListBoxSelectionMode I right now also use brute force to
        //    // grab the mouse action meaning that clicking on checkboxes or pushbutton
        //    // will not trigger click events for these renderers. Some finetuning
        //    // is needed here.
        //    if (mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.DirectlyOverRenderer != null
        //        //&& (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0)
        //        //&& GridOptions.ListBoxSelectionMode == GridSelectionMode.None
        //        )
        //    {
        //        // Trying to implement fine-tuned implementation for ListBoxSelectionMode here.
        //        bool listModeCriteria = true;
        //        if (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)
        //        {
        //            listModeCriteria = GridOptions.ListBoxModeAllowUIElementClick;
        //        }

        //        if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0)
        //        {
        //           // IInputElement ie = mouseEventArgs.SourceEventArgs.Source as IInputElement;
        //            if (ie.IsKeyboardFocusWithin)
        //                listModeCriteria = false;

        //            // Another option here would be to check if UIElement belongs to current cell
        //            // and if it is focused.
        //            // Or we could also add an attached property that we could check.
        //            // SHIFT and CTRL-Selection will stop working for these cells. Further
        //            // fine-tuning might be needed.
        //        }

        //        if (listModeCriteria && !end.IsEmpty)
        //        {
        //            if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent
        //                || mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseUpEvent
        //                )
        //            {
        //                // Clearing selection and setting current range in case of ExcelLikeCurrentCell option.
        //                // I do this here in HitTest because I do only want to quickly hook into the
        //                // flow when the user clicked inside a TextBox. The SelectCellsMouseController 
        //                // does not want to handle the MouseDown, MouseMove at that time.  The SelectCellsMouseController
        //                // waits until the user moves the mouse outside the textbox to start processing
        //                // mouse events. But I need to reset the selection at the time the user clicks
        //                // down the mouse (and also possibly select a range containing the current cell in  
        //                // the case that ExcelLikeCurrentCell is en
        //                if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
        //                {
        //                    if (!grid.CurrentCell.HasCurrentCellAt(end.RowIndex, end.ColumnIndex))
        //                    {
        //                        GridRangeInfo emptyRange = GridRangeInfo.Empty;
        //                        if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.SetCurrentCell))
        //                        {
        //                            Selections.Clear(false);
        //                            RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
        //                        }
        //                    }

        //                    if (GridOptions.ExcelLikeCurrentCell)
        //                    {
        //                        // Be aware that when you cancel the current cell activation in CurrentCellActivating
        //                        // event that the selection already occured with ProcessSetCurrentCell. If you do not
        //                        // want this you should also handle SelectionChanging event.
        //                        ProcessSetCurrentCell(end.RowIndex, end.ColumnIndex, GridActivateCurrentCellOptions.Empty);
        //                    }
        //                }
        //                Mrci.mouseDownRowIndex = end.RowIndex;
        //                Mrci.mouseDownColIndex = end.ColumnIndex;
        //                Mrci.mouseClickRowIndex = end.RowIndex;
        //                Mrci.mouseClickColIndex = end.ColumnIndex;
        //                Mrci.firstClick = false;
        //            }
        //        }

        //        return 0;
        //    }

        //    // In the case that user clicked on a rendered cell and there is no UIElement
        //    // under the mouse, do not handle the event during PreviewMouseDown. Instead
        //    // wait for the following MouseDown. Waiting for MouseDown gives programmers 
        //    // the chance to handle the PreviewMouseDown in an attached event handler 
        //    // and possible prevent the grid from handling it.
        //    if (!mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
        //        return 0;

        //    if (!end.IsEmpty)
        //        return 2;
        //    return 0;
        //}

        public bool SupportsCancelMouseCapture
        {
            get { return true; }
        }

        public bool SupportsMouseTracking
        {
            get { return true; }
        }

        #endregion

        #region SuspendState
        public class SuspendState
        {
            GridSelectCellsMouseController mc;
            RowColumnIndex start;
            GridRangeInfoList selectedCells;
            AutoScrollOrientation autoScroll;

            public SuspendState(GridSelectCellsMouseController mc)
            {
                this.mc = mc;
                start = mc.start;
                autoScroll = mc.grid.AutoScroller.AutoScrolling;
                selectedCells = mc.SelectedRanges.Clone();
            }

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

        #region IMouseController Members


        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            //TraceUtil.TraceCurrentMethodInfo(mouseEventArgs.Location);
            bool isCtrl = false;
            bool isShift = false;
            if (controller != null)
                return 0;

            Point point = mouseEventArgs.Location;
            RowColumnIndex end = grid.PointToCellRowColumnIndexOutsideCells(point, false);
#if WinRT
            isCtrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down;
            isShift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift) == CoreVirtualKeyStates.Down;
#endif

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
            if (mouseEventArgs.IsMouseOverChildElement//&& mouseEventArgs.DirectlyOverRenderer != null
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
                if (isCtrl || isShift)
                {
                    //IInputElement ie = mouseEventArgs.SourceEventArgs.Source as IInputElement;
                    //if (ie.IsKeyboardFocusWithin)
                    //    listModeCriteria = false;

                    // Another option here would be to check if UIElement belongs to current cell
                    // and if it is focused.
                    // Or we could also add an attached property that we could check.
                    // SHIFT and CTRL-Selection will stop working for these cells. Further
                    // fine-tuning might be needed.
                }
                if (listModeCriteria && !end.IsEmpty)
                {
                    //if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent
                    //    || mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseUpEvent
                    //    )
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
                        if (!grid.CurrentCell.HasCurrentCellAt(end.RowIndex, end.ColumnIndex))
                        {
                            GridRangeInfo emptyRange = GridRangeInfo.Empty;
                            if (RaiseSelectionChanging(ref emptyRange, GridSelectionReason.SetCurrentCell))
                            {
                                Selections.Clear(false);
                                RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.SetCurrentCell);
                            }
                        }
                        if (GridOptions.ExcelLikeCurrentCell)
                        {
                            // Be aware that when you cancel the current cell activation in CurrentCellActivating
                            // event that the selection already occured with ProcessSetCurrentCell. If you do not
                            // want this you should also handle SelectionChanging event.
                            ProcessSetCurrentCell(end.RowIndex, end.ColumnIndex, GridActivateCurrentCellOptions.Empty);
                        }
                        //}
                        Mrci.mouseDownRowIndex = end.RowIndex;
                        Mrci.mouseDownColIndex = end.ColumnIndex;
                        Mrci.mouseClickRowIndex = end.RowIndex;
                        Mrci.mouseClickColIndex = end.ColumnIndex;
                        Mrci.firstClick = false;
                    }
                }

                return 0;
            }

            // In the case that user clicked on a rendered cell and there is no UIElement
            // under the mouse, do not handle the event during PreviewMouseDown. Instead
            // wait for the following MouseDown. Waiting for MouseDown gives programmers 
            // the chance to handle the PreviewMouseDown in an attached event handler 
            // and possible prevent the grid from handling it.
            //if (!mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
            //    return 0;

            if (!end.IsEmpty)
                return 2;
            return 0;
        }

        #endregion
        
#if WinRT
        public void MouseHoverEnter(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        public void MouseHoverLeave(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }
#endif
    }

}

