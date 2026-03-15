#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Diagnostics;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Interop;
using System.Collections;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <exclude/>
    /// <summary>
    /// MouseController for handling selecting object in an ISupportSelectRecords object."/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class GridTreeSelectNodesMouseController<T> : IMouseController, IDisposable where T : ISelectable, IComparable
    {
        #region Fields
        bool inSelectingCells;
        bool addNewSelection;
        bool extendSelection;
        bool canceled;
        MouseRowColInfo mrci = null;
        ISupportsRecordSelection<T> grid;
        RowColumnIndex start = RowColumnIndex.Empty;
        SuspendState<T> suspendState;
        #endregion

        #region Ctor
        public GridTreeSelectNodesMouseController(ISupportsRecordSelection<T> grid)
        {
            this.grid = grid;
            grid.InternalGrid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            grid.InternalGrid.CurrentCellActivated += new GridRoutedEventHandler(GridCurrentCellActivated);
            grid.InternalGrid.ExternalMove = new GridCurrentCellMoveDelegateHandler(GridCurrentCellExternalMove);
#if !SILVERLIGHT
            (grid as GridTreeControlImpl).AutoScroller.AutoScrollerValueChanged += (AutoScroller_AutoScrollerValueChanged);
#endif            
        }
#if !SILVERLIGHT
        void AutoScroller_AutoScrollerValueChanged(object sender, AutoScrollerValueChangedEventArgs args)
        {
            int rowIndex = Mrci.mouseMoveRowIndex;
            int colIndex = Mrci.mouseMoveColIndex;
            bool changed = false;
            if (args.IsLineDown)
            {
                changed = true;
                rowIndex = (grid as GridTreeControlImpl).ScrollRows.LastBodyVisibleLineIndex;
            }

            if (args.IsLineUp)
            {
                changed = true;
                rowIndex = (grid as GridTreeControlImpl).ScrollRows.ScrollLineIndex;
            }

            //if (args.IsLineLeft)
            //{
            //    changed = true;
            //    colIndex = (grid as GridTreeControlImpl).ScrollColumns.ScrollLineIndex;
            //}

            //if (args.IsLineRight)
            //{
            //    changed = true;
            //    colIndex = (grid as GridTreeControlImpl).ScrollColumns.LastBodyVisibleLineIndex;
            //}

            if (!changed)
                return;

            if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex)
            {
                inSelectingCells = true;
                ChangeSelectCells(rowIndex, colIndex);
                if (!canceled)
                {
                    Mrci.mouseMoveRowIndex = rowIndex;
                    Mrci.mouseMoveColIndex = colIndex;
                }
                inSelectingCells = false;
            }
        }
#endif
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
#if !SILVERLIGHT
            (grid as GridTreeControlImpl).AutoScroller.AutoScrollerValueChanged -= (AutoScroller_AutoScrollerValueChanged);
#endif
            grid.InternalGrid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(GridCurrentCellMoveComplete);
            grid.InternalGrid.CurrentCellActivated -= new GridRoutedEventHandler(GridCurrentCellActivated);
            grid.InternalGrid.ExternalMove = null;
        }

        #endregion

        #region Properties
        MouseRowColInfo Mrci
        {
            get
            {
                if (mrci == null)
                {
                    if (grid.InternalGrid.Model.UserData.Contains("MouseNodeInfo"))
                        mrci = (MouseRowColInfo)grid.InternalGrid.Model.UserData["MouseNodeInfo"];
                    else
                        grid.InternalGrid.Model.UserData["MouseNodeInfo"] = mrci = new MouseRowColInfo();
                }
                return mrci;
            }
        }

        GridControlBase Grid
        {
            get { return grid.InternalGrid; }
        }

        GridCurrentCell CurrentCell
        {
            get { return grid.InternalGrid.CurrentCell; }
        }

        GridModel Model
        {
            get { return grid.InternalGrid.Model; }
        }

        GridModelOptions GridOptions
        {
            get { return grid.InternalGrid.Model.Options; }
        }

        MouseControllerDispatcher MouseControllerDispatcher
        {
            get { return grid.InternalGrid.MouseControllerDispatcher; }
        }

        GridModelSelections Selections
        {
            get { return grid.InternalGrid.Model.Selections; }
        }

        GridSelectedObjectsBase<T> SelectedNodes
        {
            get { return grid.SelectedNodes; }
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
                + SelectedNodes.ToString() + " "
                + grid.ToString();
        }
        #endregion

        #region Grid Event Handlers

        void GridCurrentCellMoveComplete(object sender, GridCurrentCellMovedEventArgs e)
        {

            if (MouseControllerDispatcher.InMouseDown)
                return;

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
#if DEBUG &&!SILVERLIGHT
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, flags, this);
            }
#endif

            if (SelectedNodes.Count == 0)
            {
                if (rowIndex >= 0 && colIndex >= 0)
                    SetMrciMouseDownClick(rowIndex, colIndex);
            }

            //Below code handling selection while clicking Checkbox and DataTemplate cells
            if (!inSelectingCells
                && rowIndex >= grid.InternalGrid.InternalGetHeaderRows()
                && (flags.SetCurrentCellOptions & GridSetCurrentCellOptions.NoSelectRange) == 0
                && GridOptions.ListBoxSelectionMode != GridSelectionMode.None)
            {
                // SD17159  Selection Issue In Datatemplate Celltypes
                // Move Edit and set selection to new row.
                //if (flags.SetCurrentCellOptions != GridSetCurrentCellOptions.None)
                {
                    T n = grid.GetNodeAtRowIndex(rowIndex);
                    if (n == null)
                        return;
                    if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                    {
                        SelectedNodes.Clear();
                        SelectedNodes.SetSelected(n, true);
                    }
                    else
                    {
                        if (!SelectedNodes.Contains(n))
                            SelectedNodes.SetSelected(n, true);
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

        #region Keyboard Navigation

        protected virtual bool GridCurrentCellExternalMove(GridDirectionType direction, int num, bool extendSelection)
        {
            int rowIndex = 0;
            int colIndex = 0;
            GridSelectedObjectsBase<T> selectedRanges = SelectedNodes;

            extendSelection &= GridOptions.ListBoxSelectionMode != GridSelectionMode.One
                || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended;


            // Grid in list box mode.
            if (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)                
            {
                if (GridOptions.MulitExtendedArrowKeySelect)
                {
                    if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                    {
                        int saveRow = rowIndex;
                        bool horizontal = direction == GridDirectionType.Left || direction == GridDirectionType.Right
                            || direction == GridDirectionType.MostLeft || direction == GridDirectionType.MostRight;

                        if (horizontal)
                        {
                            bool b = CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                            GridRangeInfo r = GridRangeInfo.Row(rowIndex);
                            Grid.InvalidateCell(r);
                            Grid.InvalidateVisual();
                            return b;
                        }
                        bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
                        bool isCtrl = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
                        if (selectedRanges.Count == 0 || !extendSelection)
                        {
                            GridRangeInfo emptyRange = GridRangeInfo.Empty;
                            //SelectedNodes.Clear();//Temp Commented
                          
                            if (rowIndex >= Grid.Model.HeaderRows && rowIndex <= (Grid.Model.RowCount - 1))
                            {
                                if ((rowIndex == (Grid.Model.RowCount - 1) && (direction == GridDirectionType.Up || direction == GridDirectionType.PageUp)))
                                {

                                    Selections.Clear();
                                }
                                else if ((rowIndex == Grid.Model.HeaderRows && (direction == GridDirectionType.Down || direction == GridDirectionType.PageDown)))
                                {
                                    Selections.Clear();
                                }
                                else if (rowIndex > Grid.Model.HeaderRows && rowIndex < (Grid.Model.RowCount - 1))
                                {
                                    Selections.Clear();
                                }
                                else if (isCtrl && (direction == GridDirectionType.BottomRight || direction == GridDirectionType.TopLeft))
                                    Selections.Clear();
                            }
                            

                            this.Mrci.mouseDownRowIndex = rowIndex;
                            this.Mrci.mouseDownColIndex = colIndex;
                        }
                       
                        // Move Edit and set selection to new row.
                        inSelectingCells = true;

                        //rowIndex += ((direction == GridDirectionType.Up) ? -num : num);
                        //InternalMove handle the CurrentCell moving based on direction. 
                        CurrentCell.InternalMove(direction, num, new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView));
                        bool success = CurrentCell.GetCurrentCell(out rowIndex, out colIndex);

                        if (rowIndex < Grid.Model.HeaderRows || rowIndex >= Grid.Model.RowCount)
                            return false;

                        int terminalRow = extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex;
                        int count = Math.Abs(terminalRow - rowIndex) + 1;
                        int inc = terminalRow > rowIndex ? -1 : 1;
                        int row = terminalRow;
                        T n = default(T);

                        SelectedNodes.LockGridRefresh = true;
                        for (int i = 0; i < count; ++i)
                        {
                            n = grid.GetNodeAtRowIndex(row);

                            if (n != null && !n.IsSelected)
                            {
                                SelectedNodes.SetSelected(n, true);
                            }
                            row += inc;
                        }
                        SelectedNodes.LockGridRefresh = false;
                        GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? this.Mrci.mouseDownRowIndex : rowIndex, rowIndex);
                        ChangeSelection(newRange, GridSelectionReason.ArrowKey);
                       
                        this.Mrci.mouseMoveRowIndex = rowIndex;
                        this.Mrci.mouseMoveColIndex = colIndex;

                        // Fire event.
                        //grid.InternalGrid.ScrollCellInView(rowIndex, colIndex, GridScrollCurrentCellReason.MoveTo);

                        if (!extendSelection)
                        {
                            this.Mrci.mouseDownRowIndex = rowIndex;
                            this.Mrci.mouseDownColIndex = colIndex;
                        }
                        inSelectingCells = false;

                        return true;

                    }
                }
                else
                {
                    // TODO: Add code here in GridSelectCellsMouseCountroler to extend selection without moving current cell.

                }
            }

            return false;
        }

        #endregion

        #region Begin/End/Change Selection
        void BeginSelectCells(int rowIndex, int colIndex, ModifierKeys modifierKeys)
        {
            if (Grid.CurrentCell != null && Grid.CurrentCell.IsModified)
            {
#if SILVERLIGHT
                bool isValidationMsgShown = false;
                bool suspendMoveTo = false;
                Grid.CurrentCell.Validate(out isValidationMsgShown, out suspendMoveTo);
#else
                Grid.CurrentCell.Validate();
#endif
                if (!Grid.CurrentCell.IsValid)
                    return;
            }

#if DEBUG&&!SILVERLIGHT
            if (!BrowserInteropHelper.IsBrowserHosted)
            {                                                                       
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, modifierKeys, this);
            }
#endif

            GridSelectedObjectsBase<T> selectedRanges = SelectedNodes;

            int nhRow = grid.InternalGrid.InternalGetHeaderRows();
            int nhCol = grid.InternalGrid.InternalGetHeaderCols();
            bool bCol = (rowIndex <= 0 && colIndex >= nhCol);
            bool bRow = ((colIndex <= 0 || GridOptions.ListBoxSelectionMode != GridSelectionMode.None) && rowIndex >= nhRow);
            bool bCell = !bRow && (rowIndex >= nhRow && colIndex >= nhCol);
            bool bTable = (colIndex == 0 && rowIndex == 0);
            var treeGrid = (this.Grid.FindParentElementOfType<GridTreeControl>()).InternalGrid;

            if (GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
            {
                addNewSelection = (modifierKeys & ModifierKeys.Control) != ModifierKeys.None;
                extendSelection = !Mrci.firstClick && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
            }
            else if (GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiSimple)
                addNewSelection = true;
            else if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One)
                addNewSelection = extendSelection = false;

            if (rowIndex < nhRow)
                return;

            Mrci.firstClick = false;
            GridRangeInfo clickRange = GridRangeInfo.Empty;
            clickRange = GridRangeInfo.Row(rowIndex);
            T tempRange = grid.GetNodeAtRowIndex(rowIndex); ;
            bool resetSelection = false;
            if (selectedRanges.Count == 1 && !selectedRanges[0].Equals(tempRange)
                || selectedRanges.Count > 1)
                resetSelection = !extendSelection && !addNewSelection;

            // First, remove all selections if neither SHIFT or CTRL is pressed
            // or if the clicked range type differs from previous selected ranges.
            if (resetSelection)
            {
                extendSelection = addNewSelection = false;
                SelectedNodes.RemoveRange(SelectedNodes.ToList());
                this.grid.InternalGrid.InvalidateVisual();
            }

            // Ctrl-Key
            if (addNewSelection)
            {
                // If user clicks on a selected row or column header, deselect
                // the row or column.
                if (grid.SelectedNodes.Contains(tempRange))
                {
                    SelectedNodes.Remove(tempRange);
                    if(rowIndex == this.Grid.CurrentCell.RowIndex)
                        this.Grid.CurrentCell.Deactivate();
                    // Selections.Remove(clickRange);
                    if ((modifierKeys & ModifierKeys.Control) == ModifierKeys.None)
                        SetMrciMouseDownClick(clickRange.Top, clickRange.Left);
                    
                    canceled = true;
                    this.RaiseSelectionChanged(this.Model.SelectedRanges.ActiveRange, GridSelectionReason.SelectRange);
                    return;
                }
            }

            // Start selection.
            // If user presses <SHIFT>, extend last range.
            // '>' symbol is changed '==' because multiple Shift+Selection  is breaked
            //Fix for WPF11055
            if (extendSelection && selectedRanges.Count > 0)
            {
                // Select cells starting from current cell.
                int ncRowIndex, ncColIndex;
                if (CurrentCell.HasCurrentCell)
                {
                    CurrentCell.GetCurrentCell(out ncRowIndex, out ncColIndex);
                    //this.Mrci.mouseDownRowIndex = ncRowIndex;
                    //this.Mrci.mouseDownColIndex = ncColIndex;   
                }
            }
            else if (!extendSelection)
            {
                this.Mrci.mouseClickRowIndex = rowIndex;
                this.Mrci.mouseClickColIndex = colIndex;
                this.Mrci.mouseDownRowIndex = rowIndex;
                this.Mrci.mouseDownColIndex = colIndex;
            }

#if DEBUG&&!SILVERLIGHT
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                Trace.WriteLineIf(extendSelection && Switches.SelectRange.TraceVerbose, "ClickRange: " + clickRange.ToString());
            }
#endif

            GridRangeInfo newRange = clickRange.UnionRange(GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex));

            if (addNewSelection)
            {
                if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                {
                    Selections.ChangeSelection(GridRangeInfo.Empty, newRange, GridSelectionReason.MouseDown);
                    this.RaiseSelectionChanged(this.Model.SelectedRanges.ActiveRange, GridSelectionReason.MouseDown);
                    if (GridOptions.ListBoxSelectionMode== GridSelectionMode.MultiExtended)
                    	CurrentCell.MoveTo(rowIndex, colIndex);
                }
                else
                {
                    GridTreeNode node = treeGrid.GetNodeAtRowIndex(rowIndex);
                    if (!treeGrid.SelectedNodes.Contains(node))
                    {
                        Selections.ChangeSelection(this.Model.SelectedRanges.ActiveRange, newRange, GridSelectionReason.MouseDown);
                        this.RaiseSelectionChanged(newRange, GridSelectionReason.SelectRange);
                    }
                }
            }
            else if (!extendSelection)
                ChangeSelection(newRange, GridSelectionReason.MouseDown);

            if (extendSelection)
            {
               this.SelectedNodes.Clear();
                GridSelectedTreeNodes selectedNodes = new GridSelectedTreeNodes(treeGrid);
                for (int i = newRange.Top; i <= newRange.Bottom; i++)
                {
                  selectedNodes.Add(treeGrid.GetNodeAtRowIndex(i));
                }
                treeGrid.SelectedNodes.AddRange(selectedNodes, treeGrid.GetNodeAtRowIndex(clickRange.Top));
                //this.RaiseSelectionChanged(newRange, GridSelectionReason.MouseDown);
            }

            if (GridOptions.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                inSelectingCells = true;


        }

        void RaiseSelectionChanged(GridRangeInfo range, GridSelectionReason reason)
        {
            GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(range, null, reason);
            this.Grid.Model.RaiseSelectionChanged(e);
        }

        GridRangeInfoList lastRanges = new GridRangeInfoList();

        void ChangeSelection(GridRangeInfo newRange, GridSelectionReason reason)
        {
            GridRangeInfoList oldRanges = new GridRangeInfoList();
            //here the node will be select by two places SelectedNodes.SetSelected(...) and Selections.ChangeSelection(...). So i comment the followings.
            //for (int row = newRange.Top; row <= newRange.Bottom; ++row)
            //{
            //    SelectedNodes.SetSelected(grid.GetNodeAtRowIndex(row), true);
            //}
            grid.InternalGrid.InvalidateCell(newRange);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0 && (inMouseMove || (Keyboard.Modifiers & ModifierKeys.Shift) == 0))
            {
                foreach (GridRangeInfo r in lastRanges)
                {
                    grid.InternalGrid.InvalidateCell(r);
                }
                oldRanges = lastRanges;
                lastRanges.Clear();
            }
           lastRanges.Add(newRange);
           if (oldRanges.Count > 0)
           {
               Selections.ChangeSelection(oldRanges[oldRanges.Count - 1], newRange, true, reason);
           }
           else if(!extendSelection)
           {
               Selections.ChangeSelection(GridRangeInfo.Empty, newRange, true, reason);
           }
           grid.InternalGrid.InvalidateVisual();
        }

        bool ChangeSelectCells(int rowIndex, int colIndex)
        {
#if DEBUG&&!SILVERLIGHT
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, this);
            }
#endif

            if (inSelectingCells)
            {

                int nhRow = grid.InternalGrid.InternalGetHeaderRows();
                int nhCol = grid.InternalGrid.InternalGetHeaderCols();
                int nfRow = grid.InternalGrid.InternalGetFrozenRows();
                int nfCol = grid.InternalGrid.InternalGetFrozenCols();

                GridRangeInfo newRange;
                int index = ((grid as GridTreeControlImpl).ShowRowHeader ? grid.InternalGrid.Model.FrozenColumns - grid.InternalGrid.Model.HeaderRows : grid.InternalGrid.FrozenColumns);
                rowIndex = Math.Max(nhRow, rowIndex);
                if (this.Mrci.mouseDownRowIndex >= nfRow && grid.InternalGrid.TopRowIndex >= nfRow)
                    rowIndex = Math.Max(grid.InternalGrid.TopRowIndex, rowIndex);
                colIndex = Math.Max(nhCol, colIndex);
                if ((this.Mrci.mouseDownColIndex >= (nfCol - grid.InternalGrid.Model.HeaderRows)) && grid.InternalGrid.LeftColIndex >= nfCol)
                    colIndex = Math.Max(grid.InternalGrid.LeftColIndex -index,colIndex) ;

                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One)
                {
                    this.SelectedNodes.Clear();
                    newRange = GridRangeInfo.Row(rowIndex);
                }
                else
                    newRange = GridRangeInfo.Rows(rowIndex, this.Mrci.mouseDownRowIndex);

                ChangeSelection(newRange, GridSelectionReason.MouseMove);
                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.One
                     || GridOptions.ListBoxSelectionMode == GridSelectionMode.MultiExtended && !this.Grid.Model.Options.ExcelLikeSelection)
                {
                    var options = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange | /*GridSetCurrentCellOptions.NoSetFocus |*/ GridSetCurrentCellOptions.NoSyncCurrentCell);
                    options.ShouldBeginEdit = false;
#if SILVERLIGHT
                    bool isValidationMsgShown = false;
                    CurrentCell.MoveTo(rowIndex, CurrentCell.ColumnIndex, options, out isValidationMsgShown);
#else
                    CurrentCell.MoveTo(rowIndex, CurrentCell.ColumnIndex, options);
#endif
                }
                return true;
            }
            return false;
        }

        void EndSelectCells()
        {
#if DEBUG&&!SILVERLIGHT
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (Switches.SelectRange.TraceVerbose)
                    TraceUtil.TraceCurrentMethodInfo(this);
            }
#endif

            GridSelectedObjectsBase<T> selectedRanges = SelectedNodes;

            CurrentCell.AdjustRowColIfCoveredCell(ref Mrci.mouseMoveRowIndex, ref Mrci.mouseMoveColIndex);

            if (inSelectingCells
                && (this.Mrci.mouseDownColIndex != 0 || this.Mrci.mouseDownRowIndex != 0))
            {
                if (GridOptions.ListBoxSelectionMode == GridSelectionMode.None
                    && this.Mrci.mouseDownColIndex == this.Mrci.mouseMoveColIndex
                    && this.Mrci.mouseDownRowIndex == this.Mrci.mouseMoveRowIndex
                    && this.Mrci.mouseDownRowIndex > 0 && this.Mrci.mouseDownColIndex > 0)
                {
                    if (addNewSelection)
                    {
                        // Allow programmer to change the range.
                        GridRangeInfo range = GridRangeInfo.Cell(this.Mrci.mouseDownRowIndex, this.Mrci.mouseDownColIndex);
                        if (selectedRanges.Count > 0)
                        {
                            ChangeSelection(range, GridSelectionReason.MouseMove);
                        }
                    }
                    else
                    {
                        // User did move the current cell but didn't select any cells.
                        SelectedNodes.Clear();
                    }
                }

            }
        }

        #endregion

        #region IMouseController Members

        public string Name
        {
            get { return "SelectNodesMouseController"; }
        }

       

        private Cursor _cursor;
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

        public void MouseHoverEnter(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverEnter(e);
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
            //    TraceUtil.TraceCurrentMethodInfo(host.GetType().Name, e.IsMouseOverChildElement);
            this.Grid.RaiseCellMouseHover(e);
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
            this.Grid.RaiseCellMouseHoverLeave(e);
        }

        public void MouseDown(MouseControllerEventArgs e)
        {
            this.Grid.RaiseCellMouseDown(e);
            if (e.Handled)
                return;

            canceled = false;
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            //Rect r = new Rect();
            //r = grid.InternalGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            //if (!r.IsEmpty && !(r.Contains(point)))
            //{
            //    start.RowIndex = this.CurrentCell.RowIndex;
            //    start.ColumnIndex = this.CurrentCell.ColumnIndex;
            //}
            //else
            //{
         
                start = grid.InternalGrid.PointToCellRowColumnIndexOutsideCells(point,false);
            //}

            //start = grid.InternalGrid.PointToCellRowColumnIndex(point);

            //ignore rightmouse button if it is in a selection
#if !SILVERLIGHT
            if (e.Button != null && e.Button.Value == MouseButton.Right)
            {
                T n = grid.GetNodeAtRowIndex(start.RowIndex);
                if (n != null && n.IsSelected)
                    return;
            }
#endif


            GridRenderStyleInfo style = grid.InternalGrid.GetRenderStyleInfo(start);
            IGridCellRenderer renderer = style.CellRenderer;
            IHitTestSelectCells hsc = renderer as IHitTestSelectCells;
            
#if !SILVERLIGHT
            if (renderer is GridCellCheckboxRenderer)
            {
                this.MouseControllerDispatcher.CanHandleMouseDown = false;
            }
#endif

            //bool isCoveredCellFound = false; The variable is assigned but it is never used
            RowColumnIndex startAdjustedForCoveredCell = grid.InternalGrid.AdjustCoveredCellRowColumnIndex(start);

            int rowIndex = startAdjustedForCoveredCell.RowIndex;
            int colIndex = startAdjustedForCoveredCell.ColumnIndex;

            //Update the Mrci while the SelectedNodes set from code behind.
            if (Mrci.firstClick && this.SelectedNodes.Count > 0)
            {
                this.Mrci.firstClick = false;
                int _rowindex = grid.GetRowIndexFromItem((this.SelectedNodes[0] as GridTreeNode).Item);
                Mrci.mouseClickRowIndex = _rowindex;
                mrci.mouseDownRowIndex = _rowindex;
                Mrci.mouseMoveRowIndex = _rowindex;
                Mrci.mouseClickColIndex = 1;
                mrci.mouseDownColIndex = 1;
                Mrci.mouseMoveColIndex = 1;
            }

            BeginSelectCells(rowIndex, colIndex, Keyboard.Modifiers);

            if (canceled)
            {
                return;
            }



            GridTreeExpanderCellRendererExt rendererext = this.Grid.CellRenderers["ExpanderCell"] as GridTreeExpanderCellRendererExt;
#if !SILVERLIGHT

            var SkipBeginEdit = rendererext != null ? rendererext.ClickOutSideCell : false;//this boolean variable useed to find Mouse point click left side of the Expander.
#endif
            bool moveToSuccess = true;
            if (CurrentCell.CellRowColumnIndex != startAdjustedForCoveredCell
                && (Keyboard.Modifiers != ModifierKeys.Control))
            {
                GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions();
#if !SILVERLIGHT
                options.IsActivateTriggeredByMouseDownIntoUIElement = true;
#endif
                if ((grid.InternalGrid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                {
                    options.ShouldBeginEdit = false;
                }
                else
                {
                    options.ShouldBeginEdit = (grid.InternalGrid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.SetCurrent | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll)) != 0;
                }
#if !SILVERLIGHT
                if (SkipBeginEdit)//Mouse Click is in Leftside of the Expander means cell should not enter into edit mode.
                {
                    options.ShouldBeginEdit = false;
                }
#endif
                if (this.SelectedNodes.Count > 0 && rowIndex > this.grid.InternalGrid.Model.HeaderColumns)
                {
                    if (!grid.GetNodeAtRowIndex(start.RowIndex).IsSelected)
                    {
                        int _rowindex = -1;
                        if (start.RowIndex != (_rowindex = grid.GetRowIndexFromItem((this.SelectedNodes[0] as GridTreeNode).Item)))
                            start.RowIndex = _rowindex;
                    }
                }
#if SILVERLIGHT
                bool isValidationMsgShown = false;
                moveToSuccess = CurrentCell.MoveTo(start, options, out isValidationMsgShown);
#else
                moveToSuccess = CurrentCell.MoveTo(start, options);
#endif
            }
            if (
#if !SILVERLIGHT
!SkipBeginEdit &&
#endif
e.ClickCount == 2
                && CurrentCell.HasCurrentCellAt(startAdjustedForCoveredCell)
                && (grid.InternalGrid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                CurrentCell.BeginEdit(true);
            if (!extendSelection)
            {
                Mrci.mouseClickRowIndex = rowIndex;
                Mrci.mouseClickColIndex = colIndex;
                Mrci.mouseMoveRowIndex = rowIndex;
                Mrci.mouseMoveColIndex = colIndex;
            }
#if !SILVERLIGHT

            if(rendererext!=null)
                rendererext.ClickOutSideCell = false;
#endif
            grid.InternalGrid.AutoScroller.AutoScrollBounds = grid.InternalGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            if (grid.InternalGrid.AutoScroller.InsideScrollBounds.Contains(point))
                grid.InternalGrid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
        }

        bool inMouseMove = false;
        public void MouseMove(MouseControllerEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;          
            if (this.Grid.Model.Options.ExcelLikeCurrentCell && this.CurrentCell.IsEditing)//If cell is in EditMode means, further selection process should not take palce on mouse move.This is the ExcelBehavior.
            {
                return;
            }
            this.Grid.RaiseCellMouseMove(e);
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            VirtualizingCellsControl cellsControl = grid.InternalGrid;// as VirtualizingCellsControl;

            if (!start.IsEmpty)
            {
                RowColumnIndex end;
                    end = grid.InternalGrid.PointToCellRowColumnIndex(point);

                if (!end.IsEmpty)
                {
                    int rowIndex = end.RowIndex;
                    int colIndex = end.ColumnIndex;
                    if (rowIndex != Mrci.mouseMoveRowIndex || colIndex != Mrci.mouseMoveColIndex)
                    {
                        if (inMouseMove)
                        {
                            //if (!((rowIndex > Mrci.mouseMoveRowIndex && rowIndex > start.RowIndex)
                            //    || (rowIndex < Mrci.mouseMoveRowIndex && rowIndex < start.RowIndex)))
                            List<int> removeSelections = null;

                            if (rowIndex > Mrci.mouseMoveRowIndex && rowIndex <= start.RowIndex)
                            {
                                removeSelections = new List<int>();
                                for (int i = Math.Min(Mrci.mouseMoveRowIndex, start.RowIndex); i < rowIndex; ++i)
                                {
                                    removeSelections.Add(i);
                                }
                            }
                            else if (rowIndex < Mrci.mouseMoveRowIndex && rowIndex >= start.RowIndex)
                            {
                                removeSelections = new List<int>();
                                for (int i = Math.Max(Mrci.mouseMoveRowIndex, start.RowIndex); i > rowIndex; --i)
                                {
                                    removeSelections.Add(i);
                                }
                            }
                            if (shift)
                            {
                                if (rowIndex < Mrci.mouseMoveRowIndex && rowIndex <= start.RowIndex)
                                {
                                    removeSelections = new List<int>();
                                    for (int i = Math.Max(Mrci.mouseMoveRowIndex, start.RowIndex); i > rowIndex; --i)
                                    {
                                        removeSelections.Add(i);
                                    }
                                }
                                else if (rowIndex > Mrci.mouseMoveRowIndex && rowIndex >= start.RowIndex)
                                {
                                    removeSelections = new List<int>();
                                    for (int i = Math.Min(Mrci.mouseMoveRowIndex, start.RowIndex); i < rowIndex; ++i)
                                    {
                                        removeSelections.Add(i);
                                    }
                                }

                            }
                            if (removeSelections != null)
                            {
                                foreach (int i in removeSelections)
                                {
                                    T n = grid.GetNodeAtRowIndex(i);
                                    if (n != null && SelectedNodes.Contains(n))
                                    {
                                        SelectedNodes.SetSelected(n, false);
                                    }
                                }
                            }
                        }

                        ChangeSelectCells(rowIndex, colIndex);
                        if (!canceled)
                        {
                            inMouseMove = true;
                            Mrci.mouseMoveRowIndex = rowIndex;
                            Mrci.mouseMoveColIndex = colIndex;
                        }
                    }
                }
            }
            grid.InternalGrid.AutoScroller.AutoScrollBounds = grid.InternalGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            if (grid.InternalGrid.AutoScroller.InsideScrollBounds.Contains(point) || Mrci.mouseMoveColIndex<= grid.InternalGrid.FrozenColumns && (grid.InternalGrid.Model.Options.ListBoxSelectionMode!= GridSelectionMode.MultiSimple))
                grid.InternalGrid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            inMouseMove = false;
            this.Grid.RaiseCellMouseUp(e);
            if (canceled ||(this.Grid.Model.Options.ExcelLikeCurrentCell && this.CurrentCell.IsEditing))//If cell is in EditMode means, further selection process should not take palce on mouse move.This is the ExcelBehavior.
            {
                canceled = false;
                return;
            }

            Point point = e.Location;
            bool outSideGrid = point.X >= this.Grid.ColumnWidths.TotalExtent ||
                               point.X < 0 ||
                               point.Y < 0 ||
                               point.Y > this.Grid.RowHeights.TotalExtent;
            if (outSideGrid)
            {
                return;
            }

            // This is the special case in HitTest where we return a non-zero value
            // when the RoutedEvent is Mouse.PreviewMouseUpEvent even though the
            // click was inside a TextBox.
            if (start.IsEmpty)
            {
                GridRangeInfo emptyRange = GridRangeInfo.Empty;
                SelectedNodes.Clear();
                return;
            }

            canceled = false;
            //RowColumnIndex end = grid.InternalGrid.PointToCellRowColumnIndex(point);
            RowColumnIndex end;

            //Rect r = new Rect();
            //r = grid.InternalGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            //if (!r.IsEmpty && !(r.Contains(point)))
            //{
            //    if (Grid.CurrentCell != null && Grid is GridTreeControlImpl && Grid.CurrentCell.IsModified)
            //    {
            //        Grid.CurrentCell.Validate();
            //        if (!Grid.CurrentCell.IsValid)
            //            return;
            //    }
            //    end = new RowColumnIndex(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
            //}
            //else
            //{
                end = grid.InternalGrid.PointToCellRowColumnIndex(point);
            //}

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

            start = RowColumnIndex.Empty;
            grid.InternalGrid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;

            EndSelectCells();
            inSelectingCells = false;

            CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
#if SILVERLIGHT
            if (rowIndex == Mrci.mouseClickRowIndex && colIndex == Mrci.mouseClickColIndex)
#else
            if (rowIndex == Mrci.mouseClickRowIndex && colIndex == Mrci.mouseClickColIndex || (grid.InternalGrid.Model.EnableContextMenu))
#endif
            {
                GridRangeInfo newRange = GridRangeInfo.Empty;
                if (rowIndex >= grid.InternalGrid.InternalGetHeaderRows() && colIndex >= grid.InternalGrid.InternalGetHeaderCols())
                    Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out newRange);
                else if (rowIndex >= grid.InternalGrid.InternalGetHeaderRows())
                    newRange = GridRangeInfo.Row(rowIndex);
                else if (colIndex >= grid.InternalGrid.InternalGetHeaderCols())
                    newRange = GridRangeInfo.Col(colIndex);
                else
                    newRange = GridRangeInfo.Empty;



                IGridCellRenderer cellRenderer = Grid.GetRenderStyleInfo(rowIndex, colIndex).CellRenderer;
                cellRenderer.RaiseGridCellClick(rowIndex, colIndex, e);
            }
#if !SILVERLIGHT
             bool isLeftButtonPressed = ((System.Windows.Input.MouseButtonEventArgs)(e.SourceEventArgs)).ChangedButton == MouseButton.Left ? true : false;

            if(rowIndex==0)
            {
                this.Mrci.mouseDownRowIndex =  CurrentCell.RowIndex;
                this.Mrci.mouseDownColIndex =  CurrentCell.ColumnIndex;
            }

            if (!grid.InternalGrid.IsKeyboardFocusWithin && (!Grid.Model.EnableContextMenu || isLeftButtonPressed))
                grid.InternalGrid.Focus();
#endif
        }

        public void CancelMode()
        {
            if ((this.Grid is GridTreeControlImpl) && !this.Grid.Model.Options.ExcelLikeSelection)
            {
                this.Grid.RaiseCellCancelMode();
#if !SILVERLIGHT
                TraceUtil.TraceCurrentMethodInfo();
#endif
                suspendState = new SuspendState<T>(this);
                SelectedNodes.Clear();
                start = RowColumnIndex.Empty;
                grid.InternalGrid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                inSelectingCells = false;
                inMouseMove = false;
                canceled = true;
            }
        }

        public void RestoreMode()
        {
            if (suspendState != null)
                suspendState.Restore();
            suspendState = null;
            this.Grid.RaiseCellRestoreMode();
        }

        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            //TraceUtil.TraceCurrentMethodInfo(mouseEventArgs.Location);
            if (controller != null)
                return 0;

            Point point = mouseEventArgs.Location;
            RowColumnIndex end = grid.InternalGrid.PointToCellRowColumnIndex(point);

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
            if (mouseEventArgs.IsMouseOverChildElement &&
#if !SILVERLIGHT
                mouseEventArgs.DirectlyOverRenderer != null && 
#endif
 (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == 0
                //&& GridOptions.ListBoxSelectionMode == GridSelectionMode.None
                )
            {
                // Trying to implement fine-tuned implementation for ListBoxSelectionMode here.
                bool listModeCriteria = true;
                if (GridOptions.ListBoxSelectionMode != GridSelectionMode.None)
                {
                    listModeCriteria = GridOptions.ListBoxModeAllowUIElementClick;
                }

                if (listModeCriteria && !end.IsEmpty)
                {
#if !SILVERLIGHT
                    if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent
                        || mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseUpEvent
                        )
                    {
                        // Clearing selection and setting current range in case of ExcelLikeCurrentCell option.
                        // I do this here in HitTest because I do only want to quickly hook into the
                        // flow when the user clicked inside a TextBox. The SelectCellsMouseController 
                        // does not want to handle the MouseDown, MouseMove at that time.  The SelectCellsMouseController
                        // waits until the user moves the mouse outside the textbox to start processing
                        // mouse events. But I need to reset the selection at the time the user clicks
                        // down the mouse (and also possibly select a range containing the current cell in  
                        // the case that ExcelLikeCurrentCell is en
                        if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
                        {
                            if (!grid.InternalGrid.CurrentCell.HasCurrentCellAt(end.RowIndex, end.ColumnIndex))
                            {
                                GridRangeInfo emptyRange = GridRangeInfo.Empty;
                                SelectedNodes.Clear();

                            }


                        }
                        Mrci.mouseDownRowIndex = end.RowIndex;
                        Mrci.mouseDownColIndex = end.ColumnIndex;
                        Mrci.mouseClickRowIndex = end.RowIndex;
                        Mrci.mouseClickColIndex = end.ColumnIndex;
                        Mrci.firstClick = false;
                    }
                    if (mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.MouseDownEvent)
                    {
                        return 2;
                    }
                    if( mouseEventArgs.DirectlyOverRenderer is GridCellCheckboxRenderer)
                    {
                        return 2;
                    }
#else
#endif
                }

                return 0;
            }

            // In the case that user clicked on a rendered cell and there is no UIElement
            // under the mouse, do not handle the event during PreviewMouseDown. Instead
            // wait for the following MouseDown. Waiting for MouseDown gives programmers 
            // the chance to handle the PreviewMouseDown in an attached event handler 
            // and possible prevent the grid from handling it.
#if !SILVERLIGHT
            if (!mouseEventArgs.IsMouseOverChildElement && mouseEventArgs.SourceEventArgs.RoutedEvent == Mouse.PreviewMouseDownEvent)
                return 0;
#else
#endif

            if (!end.IsEmpty)
                return 2;
            return 0;
        }

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
        public class SuspendState<V> where V : ISelectable, IComparable
        {
            GridTreeSelectNodesMouseController<V> mc;
            RowColumnIndex start;
            GridSelectedObjectsBase<V> selectedCells;
            AutoScrollOrientation autoScroll;

            public SuspendState(GridTreeSelectNodesMouseController<V> mc)
            {
                this.mc = mc;
                start = mc.start;
                autoScroll = mc.grid.InternalGrid.AutoScroller.AutoScrolling;
                selectedCells = mc.SelectedNodes.Clone() as GridSelectedObjectsBase<V>;
            }

            public void Restore()
            {
                mc.start = start;
                foreach (V range in selectedCells)
                    mc.SelectedNodes.Add(range);
                mc.grid.InternalGrid.AutoScroller.AutoScrolling = autoScroll;
                mc.grid.InternalGrid.RenderCurrentCellBorder();
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
