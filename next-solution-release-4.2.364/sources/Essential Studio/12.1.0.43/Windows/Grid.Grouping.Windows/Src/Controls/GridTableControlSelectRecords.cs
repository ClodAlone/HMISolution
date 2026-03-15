//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableControlSelectRecords.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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

using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Lets you specify the appearance of selected cells
    /// </summary>
    public enum GridListBoxSelectionColorOptions
    {
        /// <summary>
        /// Do not change appearance of selected cells. You can manually color cells by
        /// handling TableControlPrepareViewStyleInfo or TableControlCellDrawn events.
        /// TableControlPrepareViewStyleInfo lets you change the backcolor of cell.
        /// TableControlCellDrawn lets you draw over an already painted cell, e.g. invert or alphablend.
        /// </summary>
        None = 0,

        /// <summary>
        /// Set BackColor and TextColor in PrepareViewStyleInfo.
        /// </summary>
        ApplySelectionColor = 1,

        /// <summary>
        /// Draw alphablend color over selected row.
        /// </summary>
        DrawAlphablend = 2,

        /// <summary>
        /// Invert cells in selected rows.
        /// </summary>
        InvertCells = 3,
    }

    /// <summary>
    /// Lets you specify behavior and appearance of the current cell when ListBoxSelectionMode was set
    /// </summary>
    [Flags]
    public enum GridListBoxSelectionCurrentCellOptions
    {
        /// <summary>
        /// When a current cell is in current row, draw it with same color as used for highlighting the whole record.
        /// </summary>
        None = 0,

        /// <summary>
        /// Don't select a current cell in current row.
        /// </summary>
        HideCurrentCell = 1,

        /// <summary>
        /// When a current cell is in current row, draw it with original cell background color.
        /// </summary>
        WhiteCurrentCell = 2,

        /// <summary>
        /// For SelectionMode.MultiExtended only: move current cell when user extends selection with mouse.
        /// </summary>
        MoveCurrentCellWithMouse = 4
    }

#if ASPNET
#else
    /// <internalonly/>
    /// <summary>
    /// Implements the cell selection behavior of a grid control.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridTableControlSelectRecords
    {
        GridTableControl grid = null;

        private bool addNewSelection;
        private bool extendSelection;
        bool inSelectingCells = false;
        bool canceled = false;
        bool scrolled = false;
        WeakReference lastSelectedRecord;

        /// <internalonly/>
        /// <summary>Contructor for GridTableControlSelectRecords.</summary>
        /// <param name="grid">The grid table control.</param>
        public GridTableControlSelectRecords(GridTableControl grid)
        {
            this.grid = grid;
        }

        bool SelectedRecordsChanging()
        {
            return true;
        }

        void SelectedRecordsChanged()
        {
        }

        bool inChangeSelectionHelper = false;

        void ChangeSelectionHelper(GridRangeInfo newRange, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Table.NestedDisplayElements.Count)
            {
                return;
            }

            Element el = grid.Table.NestedDisplayElements[rowIndex];
            if (el is RecordRow || el is Record)
            {
                Record r = Record.GetParentRecord(el);
                if (r != null)
                {
                    inChangeSelectionHelper = true;
                    try
                    {
                        bool select = rowIndex >= newRange.Top && rowIndex <= newRange.Bottom;
                        if (r.IsSelected() != select || r.IsCurrent != select)
                        {
                            if (!r.IsDisposed)
                            {
                                if (this.grid.TableDescriptor.TableOptions.ListBoxSelectionRecursive
                                    && this.grid.TableDescriptor.TableOptions.ListBoxSelectionMode != SelectionMode.One
                                    && r.HasNestedTables)
                                {
                                    grid.InvalidateElement(r);
                                    r.SetSelectedRecursive(select);
                                }
                                else
                                {
                                    grid.InvalidateElement(r.GetRecordDisplayElement());
                                    r.SetSelected(select);
                                }

                                if (select)
                                {
                                    lastSelectedRecord = new WeakReference(r);
                                }
                            }
                        }

                        if (!grid.TableDescriptor.TableOptions.ListBoxSelectionOutlineBorder.IsEmpty)
                        {
                            grid.InvalidateElement(r);
                        }
                    }
                    finally
                    {
                        inChangeSelectionHelper = false;
                    }
                }
            }
         }

        void ChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange)
        {
            if (grid.Table.TableOptions.AllowSelection != GridSelectionFlags.None)
            {
                return;
            }

            if (oldRange.IsEmpty)
            {
                oldRange = newRange;
            }

            if (newRange.IsEmpty)
            {
                newRange = oldRange;
            }

            int first = Math.Min(oldRange.Top, newRange.Top);
            int first2 = Math.Max(oldRange.Top, newRange.Top);
            int last = Math.Min(oldRange.Bottom, newRange.Bottom);
            int last2 = Math.Max(oldRange.Bottom, newRange.Bottom);

            for (int rowIndex = first; rowIndex <= first2; rowIndex++)
            {
                ChangeSelectionHelper(newRange, rowIndex);
            }

            for (int rowIndex = last; rowIndex <= last2; rowIndex++)
            {
                ChangeSelectionHelper(newRange, rowIndex);
            }

            grid.Table.activeRange = newRange;
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void Clear()
        {
            bool first = true;
            //// foreach GridTable table in groupingControl ...
            foreach (GridTable t in grid.Table.Engine.EnumerateTables())
            {
                foreach (SelectedRecord sr in t.SelectedRecords)
                {
                    if (sr.Record.IsDisposed)
                    {
                        continue;
                    }

                    Element el = sr.Record.GetRecordDisplayElement();
                    if (first)
                    {
                        t.CurrentElement = null;
                        if (grid.IsElementVisible(el))
                        {
                            //// grid.Update();
                            first = false;
                        }
                    }
                    //// int rowIndex1 = grid.Table.NestedDisplayElements.IndexOf(sr.Record);
                    //// int rowIndex2 = rowIndex1 + sr.Record.RecordRows.Count - 1;
                    grid.InvalidateElement(el);
                    //// if (grid.ViewLayout.IsRowVisible(rowIndex1) || grid.ViewLayout.IsRowVisible(rowIndex2))
                    //// grid.InvalidateRange(GridRangeInfo.Rows(rowIndex1, rowIndex2));
                }

                t.SelectedRecords.Clear();
                t.activeRange = GridRangeInfo.Empty;
            }
        }

        bool BeginSelectCells(int rowIndex, int colIndex, Keys modifierKeys)
        {
            //// TraceUtil.TraceCurrentMethodInfoIf(Switches.SelectRange.TraceVerbose, rowIndex, colIndex, modifierKeys, this);
            GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

            if (rowIndex == 0 && colIndex == 0)
            {
                //// toggle table selection
                return false;
            }

            int nhRow = grid.InternalGetHeaderRows();
            int nhCol = grid.InternalGetHeaderCols();
            bool bCol = (rowIndex <= 0) && (colIndex > nhCol);
            bool bRow = (colIndex <= 0) && (rowIndex > nhRow);
            bool bCell = (rowIndex > nhRow) && (colIndex > nhCol);
            bool bTable = (colIndex == 0) && (rowIndex == 0);

            if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.MultiExtended)
            {
                addNewSelection = (modifierKeys & Keys.Control) != Keys.None;
                extendSelection = (modifierKeys & Keys.Shift) != Keys.None;
                if (rowIndex <= nhRow)
                {
                    return false;
                }
            }
            else if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.MultiSimple)
            {
                addNewSelection = true;
                if (rowIndex <= nhRow)
                {
                    return false;
                }
            }
            else if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.One)
            {
                addNewSelection = extendSelection = false;
                if (rowIndex <= nhRow)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            GridTable table = grid.Table;

            /* see TableControl.OnMouseActivating
//            Element oldCurrentRecord = table.CurrentElement;
//            while (oldCurrentRecord is NestedTable)
//                oldCurrentRecord = ((NestedTable) oldCurrentRecord).ChildTable.ParentTable.CurrentElement;
//
//            if (oldCurrentRecord != null)
//            {
//                int rowIndex0 = grid.Table.NestedDisplayElements.IndexOf(oldCurrentRecord);
//                if (grid.ViewLayout.IsRowVisible(rowIndex0))
//                    grid.InvalidateRange(GridRangeInfo.Row(rowIndex0));
//            }*/

            Element el;
            GridTable recordTable;
            GridColumnDescriptor column;
            GetRecordColumnAndTable(table.Engine, rowIndex, colIndex, out recordTable, out el, out column);

            if (Table.IsColumnHeader(el))
            {
                return true;
            }

            GridRecord record = Record.GetParentRecord(el) as GridRecord;

            GridRangeInfo clickRange = GridRangeInfo.Empty;
            if (rowIndex == 0 && colIndex == 0)
            {
                clickRange = GridRangeInfo.Table();
            }
            else
            {
                clickRange = GridRangeInfo.Row(rowIndex);
            }

            GridRangeInfo tempRange = GridRangeInfo.Cell(rowIndex, colIndex);
            ////Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out tempRange);

            bool resetSelection = !extendSelection && !addNewSelection && !(record != null && record.IsCurrent);

            recordTable.ignoreEnterRecordComplete = resetSelection;
            recordTable.CurrentRecordManager.NavigateTo(el, false, false);
            recordTable.ignoreEnterRecordComplete = false;

            //// First, remove all selections if neither SHIFT or CTRL is pressed
            //// or if the clicked range type differs from previous selected ranges
            if (resetSelection)
            {
                extendSelection = addNewSelection = false;
                //// raise events, allow cancel, clear Selections
                if (SelectedRecordsChanging())
                {
                    Clear();
                    this.grid.Update();
                    this.SelectedRecordsChanged();
                }
            }

            //// Ctrl-Key
            if (addNewSelection && record != null)
            {
                //// If user clicks on a selected row or column header, deselect
                //// the row or column.

                //// if record
                if (record.IsSelected())
                {
                    //// allow programmer to change the range 
                    ////RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseDown, newRange))
                    if (this.SelectedRecordsChanging())
                    {
                        ChangeSelectionHelper(GridRangeInfo.Empty, rowIndex);

                        //// trigger Selections.OnSelectionChanged event
                        this.SelectedRecordsChanged(); ////RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseDown);
                    }

                    ////Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, selectedRanges.ToString());
                    return false;
//// table.mouseClickRowIndex = clickRange.Top;
//// table.mouseDownRowIndex = clickRange.Top;
//// table.mouseMoveRowIndex = clickRange.Top;
                }
            }

            //// Check, if selection is allowed
            if (true)
            {
                //// start selection

                //// if user pressed <SHIFT>, extend last range
                if (extendSelection && selectedRanges.Count == 0)
                {
                    //// select cells starting from current cell
                    //// grid.CurrentCell.GetCurrentCell(out ncRowIndex, out ncColIndex);
                    //// this.Mrci.mouseDownRowIndex = ncRowIndex;
                    //// this.Mrci.mouseDownColIndex = ncColIndex;
                }
                else if (!extendSelection)
                {
                    table.mouseClickColIndex = colIndex;
                    table.mouseClickRowIndex = rowIndex;
                    table.mouseDownRowIndex = rowIndex;
                    table.mouseDownColIndex = rowIndex;
                }

                ////Trace.WriteLineIf(extendSelection && Switches.SelectRange.TraceVerbose, "ClickRange: " + clickRange.ToString());

                GridRangeInfo savedRange = this.addNewSelection ? GridRangeInfo.Empty : table.activeRange;
                GridRangeInfo newRange = clickRange.UnionRange(GridRangeInfo.Row(table.mouseDownRowIndex));

                //// allow programmer to change the range
                if (this.SelectedRecordsChanging()) 
                {
                    ////RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseDown, newRange))
                    ChangeSelection(savedRange, newRange);

                    //// trigger Selections.OnSelectionChanged event
                    this.SelectedRecordsChanged(); ////RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseDown);
                }

                if (grid.Model.Options.ListBoxSelectionMode != System.Windows.Forms.SelectionMode.MultiSimple)
                {
                    inSelectingCells = true;
                }
            }

            return true;
        }

        bool MultiExtendedShouldMoveCurrentCell
        {
            get
            {
                return (grid.Table.TableOptions.ListBoxSelectionCurrentCellOptions & GridListBoxSelectionCurrentCellOptions.MoveCurrentCellWithMouse) != 0;
            }
        }

        bool ChangeSelectCells(int rowIndex, int colIndex)
        {
            ////TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, this);
            if (inSelectingCells)
            {
                GridRangeInfoList selectedRanges = grid.Model.SelectedRanges;

                int nhRow = grid.InternalGetHeaderRows();
                int nfRow = grid.InternalGetFrozenRows();

                GridRangeInfo savedRange = selectedRanges.ActiveRange;
                GridRangeInfo newRange;
                GridTable table = grid.Table;

                rowIndex = Math.Max(nhRow+1, rowIndex);
                if (table.mouseDownRowIndex > nfRow && grid.TopRowIndex - 1 > nfRow)
                {
                    rowIndex = Math.Max(grid.TopRowIndex, rowIndex);
                }

                GridTable recordTable;
                Element el;
                GridColumnDescriptor column;
                GetRecordColumnAndTable(table.Engine, rowIndex, colIndex, out recordTable, out el, out column);

                if (el == null)
                {
                    return false;
                }

                if (grid.Model.Options.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.One)
                {
                    newRange = GridRangeInfo.Row(rowIndex);
                }
                else
                {
                    newRange = GridRangeInfo.Rows(rowIndex, table.mouseClickRowIndex);
                }

                try
                {
                    //// allow programmer to change the range
                    //// RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseMove, newRange))
                    if (this.SelectedRecordsChanging()) 
                    {
                        ChangeSelection(table.activeRange, newRange);

                        if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                            || (grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended && MultiExtendedShouldMoveCurrentCell))
                        {
                            Record r = el as Record;
                            if (r != null)
                            {
                                r.SetCurrent(column != null ? column.MappingName : string.Empty);
                            }
                            else
                            {
                                recordTable.CurrentRecordManager.NavigateTo(el);
                            }
                        }

                        //// trigger Selections.OnSelectionChanged event
                        this.SelectedRecordsChanged(); ////RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseMove);
                    }
                }
                finally
                {
            ////        grid.EndUpdate(true);
                }

                return true;
            }

            return false;
        }

        void EndSelectCells()
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.SelectRange.TraceVerbose, this);
            GridTable table = grid.Table;

            if (inSelectingCells && (table.mouseDownRowIndex != 0))
            {
                    //// trigger Selections.OnSelectionChanged event to let user know that selection
                    //// ended.
                    ////RaiseSelectionChanged(selectedRanges.ActiveRange, GridSelectionReason.MouseUp);
            }
        }

        void GetRecordColumnAndTable(GridEngine engine, int rowIndex, int colIndex, out GridTable recordTable, out Element record, out GridColumnDescriptor column)
        {
            Element el = null;
            record = null;
            if (rowIndex >= 0 && rowIndex < engine.Table.NestedDisplayElements.Count)
            {
                el = engine.Table.NestedDisplayElements[rowIndex];
                record = Record.GetParentRecord(el);
                if (record == null)
                {
                    record = el;
                }
            }

            if (record != null)
            {
                recordTable = record.ParentTable as GridTable;
                column = recordTable.GetColumnDescriptorAt(el, colIndex);
            }
            else
            {
                recordTable = null;
                column = null;
            }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void MouseDown(GridTableClickCellsEventArgs te)
        {
            //// if e.Button equals MouseButtons.None, the MouseControllerDispatcher checks if I want to handle MouseTracking
            //// if e.Button equals MouseButtons.Left, the MouseControllerDispatcher checks if I want to handle Left click
            MouseEventArgs e = te.MouseEventArgs;
            if (CheckMouseButtons(e) || e.Button == MouseButtons.None)
            {
                int rowIndex = te.RowIndex;
                int colIndex = te.ColIndex;
                this.canceled = false;

                if (e.Clicks == 2)
                {
                    GridCellRendererBase cellRenderer = te.CellRenderer;
                    cellRenderer.RaiseDoubleClick(rowIndex, colIndex, e);
                    canceled = true;
                    return;
                }

                GridTable gridTable = grid.Table;
                GridTable recordTable;
                Element el;
                GridColumnDescriptor column;
                GetRecordColumnAndTable(gridTable.Engine, rowIndex, colIndex, out recordTable, out el, out column);

                ////canceled = !grid.RaiseMouseActivating();

                canceled = !BeginSelectCells(rowIndex, colIndex, Control.ModifierKeys);

                if (!canceled)
                {
                    //// allow programmer to change the range
                    //// RaiseSelectionChanging(ref newRange, GridSelectionReason.MouseMove, newRange))
                    if (this.SelectedRecordsChanging()) 
                    {
                        Record r = el as Record;
                        if (r != null)
                        {
                            ////grid.Update();
                            r.SetCurrent(column != null ? column.MappingName : string.Empty);
                            if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                                || grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended)
                            {
                                ////grid.SynchronizeCurrentCellWithRecord(r, false);

                                if (r.IsCurrent && (Control.ModifierKeys & Keys.Shift) == 0)
                                {
                                    gridTable.mouseDownRowIndex = rowIndex;
                                    gridTable.mouseDownColIndex = colIndex;
                                }
                            }
                        }
                        else
                        {
                            if (gridTable.CurrentRecordManager.NavigateTo(el) != null)
                            {
                                gridTable.mouseDownRowIndex = rowIndex;
                                gridTable.mouseDownColIndex = colIndex;
                            }
                        }

                        gridTable.mouseClickColIndex = colIndex;
                        gridTable.mouseClickRowIndex = rowIndex;
                        gridTable.mouseMoveRowIndex = rowIndex;
                        gridTable.mouseMoveColIndex = rowIndex;
                    }

                    int dy = 0;
                    int dx = 0;
                    Rectangle rc = grid.GridBounds;

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
                    grid.AutoScrollBounds = Rectangle.FromLTRB(dx, dy, rc.Right, rc.Bottom);
                    ////Trace.WriteLineIf(Switches.GridScrolling.TraceVerbose, "AutoScrollBounds: " + grid.AutoScrollBounds.ToString());
                }
            }

            scrolled = false;
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void MouseMove(GridTableClickCellsEventArgs te)
        {
            if (canceled)
            {
                te.Cancel = true;
                return;
            }

            GridTable table = grid.Table;
            Point pt = new Point(te.MouseEventArgs.X, te.MouseEventArgs.Y);
            int rowIndex = te.RowIndex;
            int colIndex = te.ColIndex;
            if (rowIndex != table.mouseMoveRowIndex || colIndex != table.mouseMoveColIndex || scrolled)
            {
                ////TraceUtil.TraceCurrentMethodInfo(this, rowIndex, colIndex);
                ChangeSelectCells(rowIndex, colIndex);
                if (!canceled)
                {
                    table.mouseMoveRowIndex = rowIndex;
                    table.mouseMoveColIndex = colIndex;
                }
            }

            scrolled = false;
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void MouseUp(GridTableClickCellsEventArgs te)
        {
            if (canceled)
            {
                te.Cancel = true;
                return;
            }

//// TraceUtil.TraceCurrentMethodInfo(this);

            EndSelectCells();
            grid.AutoScrolling = ScrollBars.None;
            inSelectingCells = false;

            GridTable table = grid.Table;
            int rowIndex = te.RowIndex;
            int colIndex = te.ColIndex;

            if (te.RowIndex != table.mouseClickRowIndex || te.ColIndex != table.mouseClickColIndex)
            {
                te.Cancel = true;
            }
            ////grid.CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
//// if (te.RowIndex == table.mouseClickRowIndex && te.ColIndex == table.mouseClickColIndex)
//// {
//// Element el = table.NestedDisplayElements[te.RowIndex];
//// ChildTable childTable = (ChildTable) el.ParentChildTable;
//// rowIndex = childTable.DisplayElements.IndexOf(el);
//// te.CellRenderer.RaiseClick(rowIndex, te.ColIndex, te.MouseEventArgs);
//// }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void CancelMode(GridTableClickCellsEventArgs e)
        {
//// TraceUtil.TraceCurrentMethodInfo(this);
            inSelectingCells = false;
            canceled = true;
        }

        bool CheckMouseButtons(MouseEventArgs e)
        {
            return (e.Button == MouseButtons.None && grid.Model.Options.SelectCellsMouseButtonsMask != MouseButtons.None)
                || (e.Button & grid.Model.Options.SelectCellsMouseButtonsMask) != MouseButtons.None;
        }

        bool RaiseQueryAllowArrowKeyNavigateTo(Element el)
        {
            bool allowNavigateTo = el is RecordRow || el is Record;
            if (CaptionSection.IsCaption(el))
            {
                allowNavigateTo = !el.ParentGroup.IsTopLevelGroup;
            }

            GridQueryAllowArrowKeyNavigateToEventArgs e = new GridQueryAllowArrowKeyNavigateToEventArgs(this.grid, el, allowNavigateTo);
            grid.RaiseQueryAllowArrowKeyNavigateTo(e);
            return e.AllowNavigateTo;
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void MoveCurrentCellDirection(GridTableControlMoveCurrentCellDirectionEventArgs e)
        {
            GridTable innerTable = e.TableControl.Table;

            if (grid.Table.TableOptions.AllowSelection != GridSelectionFlags.None)
            {
                return;
            }

            if (!e.Inner.Handled && innerTable != null && innerTable.Engine != null && innerTable.CurrentElement != null)
            {
                e.Inner.Handled = true;

                GridGroupingControl groupingControl = innerTable.Engine.ParentControl;
                GridTableControl mainTableControl = groupingControl.TableControl;
                if (mainTableControl == null)
                {
                    return;
                }

                GridTable engineTable = (GridTable) innerTable.EngineTable;

                SelectionMode listBoxMode = engineTable.TableOptions.ListBoxSelectionMode;
                bool extendSelection = listBoxMode == SelectionMode.MultiExtended && (Control.ModifierKeys & Keys.Shift) != 0;

                int rowIndex1, rowIndex2;
                Record r = innerTable.CurrentRecord;
                int numCells = 1; //// e.Inner.NumCells;

                int newRowIndex;
                switch (e.Inner.Direction)
                {
                    case GridDirectionType.TopLeft:
                    case GridDirectionType.Top:
                    {
                        int pos1 = mainTableControl.VScrollBar.Minimum;
                        Element scrollElement;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            scrollElement = engineTable.NestedDisplayElements.GetItemAtYAmount(pos1); //// get element at new y position
                        }
                        else
                        {
                            scrollElement = engineTable.NestedDisplayElements[pos1]; //// get element at new y position
                        }

                        numCells = engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement)
                            - engineTable.NestedDisplayElements.IndexOf(scrollElement); //// get number of rows to be used in GridDirectionType.Up
                        mainTableControl.VScrollBar.Value = mainTableControl.VScrollBar.Minimum;
                        goto case GridDirectionType.Up;
                    }

                    case GridDirectionType.BottomRight:
                    case GridDirectionType.Bottom:
                    {
                        int pos1 = mainTableControl.VScrollBar.Maximum;
                        Element scrollElement;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            scrollElement = engineTable.NestedDisplayElements.GetItemAtYAmount(pos1); //// get element at new y position
                        }
                        else
                        {
                            scrollElement = engineTable.NestedDisplayElements[pos1]; //// get element at new y position
                        }

                        numCells = engineTable.NestedDisplayElements.IndexOf(scrollElement)
                            - engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement); //// get number of rows to be used in GridDirectionType.Down
                        mainTableControl.VScrollBar.Value = mainTableControl.VScrollBar.Maximum;
                        goto case GridDirectionType.Down;
                    }

                    case GridDirectionType.PageUp:
                    {
                        //// Determine the element that is one page up (using Yamount metrics)
                        int y = mainTableControl.VScrollBar.LargeChange; //// no. of pixels for page up/down
                        int pos1;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            pos1 = (int)engineTable.NestedDisplayElements.GetYAmountPositionOf(innerTable.CurrentElement); //// current y position
                        }
                        else
                        {
                            pos1 = (int)engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement); //// current y position
                        }

                        pos1 = Math.Max(mainTableControl.VScrollBar.Minimum, pos1 - y); //// subtract page
                        Element scrollElement;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            scrollElement = engineTable.NestedDisplayElements.GetItemAtYAmount(pos1); //// get element at new y position
                        }
                        else
                        {
                            scrollElement = engineTable.NestedDisplayElements[pos1]; //// get element at new y position
                        }

                        numCells = engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement)
                            - engineTable.NestedDisplayElements.IndexOf(scrollElement); //// get number of rows to be used in GridDirectionType.Up
                        goto case GridDirectionType.Up;
                    }

                    case GridDirectionType.PageDown:
                    {
                        //// Determine the element that is one page down (using Yamount metrics)
                        int y = mainTableControl.VScrollBar.LargeChange; //// no. of pixels for page up/down
                        int pos1;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            pos1 = (int)engineTable.NestedDisplayElements.GetYAmountPositionOf(innerTable.CurrentElement); //// current y position
                        }
                        else
                        {
                            pos1 = (int)engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement); //// current y position
                        }

                        pos1 = Math.Min(mainTableControl.VScrollBar.Maximum, pos1+y); //// add page
                        Element scrollElement;
                        if (innerTable.Engine.SupportsYAmount && innerTable.TableOptions.VerticalPixelScroll)
                        {
                            scrollElement = engineTable.NestedDisplayElements.GetItemAtYAmount(pos1); //// get element at new y position
                        }
                        else
                        {
                            scrollElement = engineTable.NestedDisplayElements[pos1]; //// get element at new y position
                        }

                        numCells = engineTable.NestedDisplayElements.IndexOf(scrollElement)
                            - engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement); //// get number of rows to be used in GridDirectionType.Down
                        goto case GridDirectionType.Down;
                    }

                    case GridDirectionType.Down:
                    {
                        rowIndex1 = engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement);
                        if (r != null)
                        {
                            rowIndex2 = rowIndex1 + r.GetRecordDisplayElement().GetVisibleCount() - 1;
                        }
                        else
                        {
                            rowIndex2 = rowIndex1 + innerTable.CurrentElement.GetVisibleCount() - 1;
                        }

                        newRowIndex = Math.Min(engineTable.NestedDisplayElements.Count-1, rowIndex2 + numCells);
                        if (newRowIndex == 0)
                            newRowIndex = engineTable.NestedDisplayElements.Count - 1;
                        int targetRowIndex = newRowIndex;

                        Element targetElement = null;
                        for (targetRowIndex = newRowIndex; targetRowIndex < engineTable.NestedDisplayElements.Count; targetRowIndex++)
                        {
                            Element el = engineTable.NestedDisplayElements[targetRowIndex];
                            if (RaiseQueryAllowArrowKeyNavigateTo(el))
                            {
                                targetElement = el;
                            }

                            if (targetElement != null)
                            {
                                break;
                            }
                        }

                        if (targetElement == null)
                        {
                            for (targetRowIndex = newRowIndex-1; targetRowIndex > rowIndex2; targetRowIndex--)
                            {
                                Element el = engineTable.NestedDisplayElements[targetRowIndex];
                                if (RaiseQueryAllowArrowKeyNavigateTo(el))
                                {
                                    targetElement = el;
                                }

                                if (targetElement != null)
                                {
                                    break;
                                }
                            }
                        }

                        if (targetElement is RecordRow || targetElement is CaptionRow || targetElement is Record || targetElement is CaptionSection)
                        {
                            if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                                || grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended)
                            {
                                GridRangeInfo activeRange = engineTable.activeRange;

                                if (activeRange.IsEmpty || !extendSelection)
                                {
                                    engineTable.SelectedRecords.Clear();
                                    activeRange = GridRangeInfo.Empty;
                                    engineTable.mouseDownRowIndex = targetRowIndex;
                                }

                                GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection ? engineTable.mouseDownRowIndex : targetRowIndex, targetRowIndex);

                                ChangeSelection(engineTable.activeRange, newRange);
                                engineTable.mouseMoveRowIndex = targetRowIndex;
                            }

                            if (targetElement.GetVisibleCount() == 1 && engineTable == targetElement.ParentTable && targetRowIndex == newRowIndex)
                            {
                                if (grid.Table.TableOptions.AllowSelection == GridSelectionFlags.None)
                                {
                                    e.Inner.Result = mainTableControl.CurrentCell.InternalMove(e.Inner.Direction, 1, GridSetCurrentCellOptions.ScrollInView);
                                    return;
                                }

                                //// use old current cell navigation code
                                e.Inner.Handled = false;
                                return;
                            }

                            //// Make sure old record is also repainted
                            if (innerTable.Engine.UpdateDisplayFrequency == 100)
                                mainTableControl.InvalidateElement(innerTable.CurrentElement);

                            if (targetElement is RecordRow || targetElement is Record)
                            {
                                Record targetRecord = Record.GetParentRecord(targetElement);
                                if (targetRecord != null)
                                {
                                    targetRecord.SetCurrent();
                                    e.Inner.Result = targetRecord.IsCurrent;
                                }
                            }
                            else
                            {
                                e.Inner.Result = targetElement.ParentTable.CurrentRecordManager.NavigateTo(targetElement) != null;
                            }

                            e.Inner.Handled = true;
                            return;
                        }
                        else
                        {
                            //// Scroll into view if this is the bottom summary row
                            mainTableControl.ScrollInView(engineTable.NestedDisplayElements[engineTable.NestedDisplayElements.Count - 1]);
                        }

                        break;
                    }

                    case GridDirectionType.Up:
                    {
                        rowIndex1 = engineTable.NestedDisplayElements.IndexOf(innerTable.CurrentElement);
                        newRowIndex = Math.Max(0, rowIndex1 - numCells);

                        rowIndex2 = rowIndex1 + innerTable.CurrentElement.GetVisibleCount() - 1;

                        int targetRowIndex = newRowIndex;

                        Element targetElement = null;
                        for (targetRowIndex = newRowIndex; targetRowIndex >= 0; targetRowIndex--)
                        {
                            Element el = engineTable.NestedDisplayElements[targetRowIndex];
                            if (RaiseQueryAllowArrowKeyNavigateTo(el))
                            {
                                targetElement = el;
                            }

                            if (targetElement != null)
                            {
                                break;
                            }
                        }
                        if ((targetElement is CaptionRow) && targetRowIndex <= this.grid.TopRowIndex)
                        {
                            if (this.grid.Table.RelatedTables != null && this.grid.TableDescriptor.GroupedColumns != null)
                            {
                                if (this.grid.Table.RelatedTables.Count == 0 && this.grid.TableDescriptor.GroupedColumns.Count == 0)
                                {
                                    targetElement = engineTable.NestedDisplayElements[targetRowIndex + 1];
                                    targetRowIndex++;
                                }
                            }
                            else
                            {
                                targetElement = engineTable.NestedDisplayElements[targetRowIndex + 1];
                                targetRowIndex++;
                            }
                        }
                        if (targetElement == null)
                        {
                            for (targetRowIndex = rowIndex2; targetRowIndex < newRowIndex; targetRowIndex++)
                            {
                                Element el = engineTable.NestedDisplayElements[targetRowIndex];
                                if (RaiseQueryAllowArrowKeyNavigateTo(el))
                                {
                                    targetElement = el;
                                }

                                if (targetElement != null)
                                {
                                    break;
                                }
                            }
                        }

                        if (targetElement is RecordRow || targetElement is CaptionRow || targetElement is Record || targetElement is CaptionSection)
                        {
                            if (grid.Model.Options.ListBoxSelectionMode == SelectionMode.One
                                || grid.Model.Options.ListBoxSelectionMode == SelectionMode.MultiExtended)
                            {
                                GridRangeInfo activeRange = engineTable.activeRange;

                                if (activeRange.IsEmpty || !extendSelection)
                                {
                                    engineTable.SelectedRecords.Clear();
                                    activeRange = GridRangeInfo.Empty;
                                    engineTable.mouseDownRowIndex = targetRowIndex;
                                }

                                GridRangeInfo newRange = GridRangeInfo.Rows(extendSelection?engineTable.mouseDownRowIndex:targetRowIndex, targetRowIndex);

                                ChangeSelection(engineTable.activeRange, newRange);
                                engineTable.mouseMoveRowIndex = targetRowIndex;
                            }

                            if (mainTableControl.InternalGetFrozenRows() >= targetRowIndex)
                            {
                                mainTableControl.VScrollBar.Value = mainTableControl.VScrollBar.Minimum;
                            }

                            if (targetElement.GetVisibleCount() == 1 && engineTable == targetElement.ParentTable && (targetRowIndex == e.Inner.RowIndex - numCells || e.Inner.ColIndex ==1 && innerTable.CurrentElement is GridCaptionSection || innerTable.CurrentElement is GridCaptionRow))
                            {
                                if (grid.Table.TableOptions.AllowSelection == GridSelectionFlags.None)
                                {
                                    if (this.grid.Table.RelatedTables != null)
                                    {
                                        if (this.grid.Table.RelatedTables.Count == 0 && this.grid.Table.TableOptions.ListBoxSelectionMode == SelectionMode.MultiExtended || this.grid.Table.TableOptions.ListBoxSelectionMode == SelectionMode.MultiSimple)
                                        {
                                            e.Inner.Result = mainTableControl.CurrentCell.MoveTo(GridRangeInfo.Row(targetRowIndex));
                                            this.grid.ScrollCellInView(GridRangeInfo.Row(targetRowIndex));
                                        }
                                        else
                                        {
                                            if (e.Inner.ColIndex == 0 && targetElement is CaptionRow)
                                                e.Inner.Result = targetElement.ParentTable.CurrentRecordManager.NavigateTo(targetElement) != null;
                                            else
                                                e.Inner.Result = mainTableControl.CurrentCell.InternalMove(e.Inner.Direction, 1, GridSetCurrentCellOptions.ScrollInView);
                                        }
                                    }
                                    else
                                    {
                                        if (this.grid.Table.TableOptions.ListBoxSelectionMode == SelectionMode.MultiExtended || this.grid.Table.TableOptions.ListBoxSelectionMode == SelectionMode.MultiSimple)
                                        {
                                            e.Inner.Result = mainTableControl.CurrentCell.MoveTo(GridRangeInfo.Row(targetRowIndex));
                                            this.grid.ScrollCellInView(GridRangeInfo.Row(targetRowIndex));
                                        }
                                        else
                                        {
                                            if (e.Inner.ColIndex == 0 && targetElement is CaptionRow)
                                                e.Inner.Result = targetElement.ParentTable.CurrentRecordManager.NavigateTo(targetElement) != null;
                                            else
                                                e.Inner.Result = mainTableControl.CurrentCell.InternalMove(e.Inner.Direction, 1, GridSetCurrentCellOptions.ScrollInView);
                                        }
                                    }
                                    return;
                                }

                                //// use old current cell navigation code
                                e.Inner.Handled = false;
                                return;
                            }

                            //// Make sure old record is also repainted
                            mainTableControl.InvalidateElement(innerTable.CurrentElement);

                            if (targetElement is RecordRow || targetElement is Record)
                            {
                                Record targetRecord = Record.GetParentRecord(targetElement);
                                if (targetRecord != null)
                                {
                                    targetRecord.SetCurrent();
                                    e.Inner.Result = mainTableControl.CurrentCell.MoveTo(GridRangeInfo.Row(targetRowIndex));
                                }
                            }
                            else
                            {
                                e.Inner.Result = targetElement.ParentTable.CurrentRecordManager.NavigateTo(targetElement) != null;
                            }

                            e.Inner.Handled = true;
                            this.grid.ScrollCellInView(GridRangeInfo.Row(targetRowIndex));
                            return;
                        }

                        break;
                    }

                    case GridDirectionType.Right:
                    {
                        if (r != null)
                        {
                            bool done = false;
                            GridColumnDescriptor newColumn;
                            FieldDescriptor field = innerTable.CurrentRecordManager.CurrentField;
                            string fieldName = (field != null) ? field.Name : string.Empty;
                            if (field != null)
                            {
                                GridColumnDescriptor cd = innerTable.TableDescriptor.Columns.FindByField(field);
                                int visNum = innerTable.TableDescriptor.VisibleColumns.IndexOf(cd.Name);
                                do
                                {
                                    newColumn = null;
                                    if (visNum == -1)
                                    {
                                        //// Fallback for multiple row / column sets - jump from column to next column ...
                                        visNum = innerTable.TableDescriptor.Columns.IndexOf(cd.Name);
                                        if (visNum >= 0 && visNum < innerTable.TableDescriptor.Columns.Count - 1)
                                        {
                                            visNum++;
                                            newColumn = innerTable.TableDescriptor.Columns[visNum];
                                        }
                                    }
                                    else
                                    {
                                        if (visNum >= 0 && visNum < innerTable.TableDescriptor.VisibleColumns.Count - 1)
                                        {
                                            if (e.TableControl.CurrentCell.ColIndex != 0)
                                                visNum++;
                                            string newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum].Name;
                                            newColumn = innerTable.TableDescriptor.Columns[newColumnName];
                                        }
                                    }

                                    if (newColumn != null)
                                    {
                                        fieldName = newColumn.MappingName;

                                        if (newColumn.Width > 0)
                                        {
                                            GridStyleInfo style = ((GridTable)r.ParentTable).GetTableCellStyle(r, newColumn);
                                            if (style.Enabled)
                                            {
                                                done = true;
                                                r.SetCurrent(fieldName);
                                                e.TableControl.CurrentCell.ScrollInView(GridScrollCurrentCellReason.MoveTo);
                                                if (e.TableControl.CurrentCell.ColIndex == 0)
                                                {
                                                    visNum++;
                                                    string newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum].Name;
                                                    newColumn = innerTable.TableDescriptor.Columns[newColumnName];
                                                    fieldName = newColumn.MappingName;
                                                    r.SetCurrent(fieldName);
                                                    if (e.TableControl.CurrentCell.ColIndex + 1 <= innerTable.TableDescriptor.Columns.Count)
                                                        e.TableControl.CurrentCell.Activate(e.TableControl.CurrentCell.RowIndex, e.TableControl.LeftColIndex + 2);
                                                    newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum-1].Name;
                                                    fieldName = innerTable.TableDescriptor.Columns[newColumnName].MappingName;
                                                    r.SetCurrent(fieldName);
                                                }
                                            }
                                        }
                                    }
                                } 
                                while (newColumn != null && !done);
                            }
                        }
                        else
                        {
                            e.Inner.Result = e.TableControl.CurrentCell.InternalMove(e.Inner.Direction, numCells, GridSetCurrentCellOptions.ScrollInView);
                        }

                        break;
                    }

                    case GridDirectionType.Left:
                    {
                        if (r != null)
                        {
                            bool done = false;
                            GridColumnDescriptor newColumn;
                            FieldDescriptor field = innerTable.CurrentRecordManager.CurrentField;
                            if (field != null)
                            {
                                string fieldName = field.Name;
                                GridColumnDescriptor cd = innerTable.TableDescriptor.Columns.FindByField(field);
                                int visNum = innerTable.TableDescriptor.VisibleColumns.IndexOf(cd.Name);
                                do
                                {
                                    newColumn = null;
                                    if (visNum == -1)
                                    {
                                        //// Fallback for multiple row / column sets - jump from column to next column ...
                                        visNum = innerTable.TableDescriptor.Columns.IndexOf(cd.Name);
                                        if (visNum >= 1 && visNum < innerTable.TableDescriptor.Columns.Count)
                                        {
                                            visNum--;
                                            newColumn = innerTable.TableDescriptor.Columns[visNum];
                                        }
                                    }
                                    else
                                    {
                                        if (visNum >= 1 && visNum < innerTable.TableDescriptor.VisibleColumns.Count)
                                        {
                                            visNum--;
                                            string newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum].Name;
                                            newColumn = innerTable.TableDescriptor.Columns[newColumnName];
                                            if (visNum == 0)
                                            {
                                                mainTableControl.HScrollBar.Value = mainTableControl.HScrollBar.Minimum;
                                            }
                                        }
                                    }

                                    if (newColumn != null)
                                    {
                                        fieldName = newColumn.MappingName;

                                        if (newColumn.Width > 0)
                                        {
                                            GridStyleInfo style = ((GridTable)r.ParentTable).GetTableCellStyle(r, newColumn);
                                            if (style.Enabled)
                                            {
                                                done = true;
                                                r.SetCurrent(fieldName);
                                                e.TableControl.CurrentCell.ScrollInView(GridScrollCurrentCellReason.MoveTo);
                                            }
                                        }
                                    }
                                } 
                                while (newColumn != null && !done);
                            }
                        }
                        else
                        {
                            e.Inner.Result = e.TableControl.CurrentCell.InternalMove(e.Inner.Direction, numCells, GridSetCurrentCellOptions.ScrollInView);
                        }
                        
                        break;
                    }

                    case GridDirectionType.MostLeft:
                    {
                        if (r != null)
                        {
                            FieldDescriptor field = innerTable.CurrentRecordManager.CurrentField;
                            string fieldName = (field != null) ? field.Name : string.Empty;
                            GridColumnDescriptor cd = innerTable.TableDescriptor.Columns.FindByField(field);
                            GridColumnDescriptor newColumn = null;
                            int visNum = innerTable.TableDescriptor.VisibleColumns.IndexOf(cd.Name);
                            if (visNum == -1)
                            {
                                // Fallback for multiple row / column sets - jump from column to next column ...
                                visNum = innerTable.TableDescriptor.Columns.IndexOf(cd.Name);
                                if (visNum >= 1 && visNum < innerTable.TableDescriptor.Columns.Count)
                                {
                                    visNum = 0;
                                    newColumn = innerTable.TableDescriptor.Columns[visNum];
                                }
                            }
                            else
                            {
                                if (visNum >= 1 && visNum < innerTable.TableDescriptor.VisibleColumns.Count)
                                {
                                    visNum = 0;
                                    string newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum].Name;
                                    newColumn = innerTable.TableDescriptor.Columns[newColumnName];
                                    mainTableControl.HScrollBar.Value = mainTableControl.HScrollBar.Minimum;
                                }
                            }

                            if (newColumn != null)
                            {
                                r.SetCurrent(newColumn.MappingName);

                                bool done = false;
                                if (newColumn.Width > 0)
                                {
                                    GridStyleInfo style = ((GridTable)r.ParentTable).GetTableCellStyle(r, newColumn);
                                    if (style.Enabled)
                                    {
                                        done = true;
                                        e.TableControl.CurrentCell.ScrollInView(GridScrollCurrentCellReason.MoveTo);
                                    }
                                }

                                if (!done)
                                {
                                    goto case GridDirectionType.Right;
                                }
                            }
                        }
                        else
                        {
                            e.Inner.Result = e.TableControl.CurrentCell.InternalMove(e.Inner.Direction, numCells, GridSetCurrentCellOptions.ScrollInView);
                        }

                        break;
                    }

                    case GridDirectionType.MostRight:
                    {
                        if (r != null)
                        {
                            FieldDescriptor field = innerTable.CurrentRecordManager.CurrentField;
                            string fieldName = (field != null) ? field.Name : string.Empty;
                            GridColumnDescriptor cd = innerTable.TableDescriptor.Columns.FindByField(field);
                            GridColumnDescriptor newColumn = null;
                            int visNum = innerTable.TableDescriptor.VisibleColumns.IndexOf(cd.Name);
                            if (visNum == -1)
                            {
                                // Fallback for multiple row / column sets - jump from column to next column ...
                                visNum = innerTable.TableDescriptor.Columns.IndexOf(cd.Name);
                                if (visNum >= 0 && visNum < innerTable.TableDescriptor.Columns.Count)
                                {
                                    visNum = innerTable.TableDescriptor.Columns.Count - 1;
                                    newColumn = innerTable.TableDescriptor.Columns[visNum];
                                }
                            }
                            else
                            {
                                if (visNum >= 0 && visNum < innerTable.TableDescriptor.VisibleColumns.Count)
                                {
                                    visNum = innerTable.TableDescriptor.VisibleColumns.Count - 1;
                                    string newColumnName = innerTable.TableDescriptor.VisibleColumns[visNum].Name;
                                    newColumn = innerTable.TableDescriptor.Columns[newColumnName];
                                }
                            }

                            if (newColumn != null)
                            {
                                r.SetCurrent(newColumn.MappingName);

                                bool done = false;
                                if (newColumn.Width > 0)
                                {
                                    GridStyleInfo style = ((GridTable)r.ParentTable).GetTableCellStyle(r, newColumn);
                                    if (style.Enabled)
                                    {
                                        done = true;
                                        e.TableControl.CurrentCell.ScrollInView(GridScrollCurrentCellReason.MoveTo);
                                    }
                                }

                                if (!done)
                                {
                                    goto case GridDirectionType.Left;
                                }
                            }
                        }
                        else
                        {
                            e.Inner.Result = e.TableControl.CurrentCell.InternalMove(e.Inner.Direction, numCells, GridSetCurrentCellOptions.ScrollInView);
                        }

                        break;
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void SelectedRecordCellDrawn(GridTableControl tableControl, GridDrawCellEventArgs e)
        {
            GridTableCellStyleInfo style = (GridTableCellStyleInfo) e.Style;

            if (e.Cancel || style.TableCellIdentity == null)
            {
                return;
            }

            if (style.TableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AlternateRecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AddNewRecordFieldCell)
            {
                GridListBoxSelectionColorOptions colorOptions = this.grid.Table.TableOptions.ListBoxSelectionColorOptions;
                switch (colorOptions)
                {
                    case GridListBoxSelectionColorOptions.DrawAlphablend:
                    {
                        GridListBoxSelectionCurrentCellOptions currentCellOptions = this.grid.Table.TableOptions.ListBoxSelectionCurrentCellOptions;
                        if (!((currentCellOptions & GridListBoxSelectionCurrentCellOptions.WhiteCurrentCell) != 0
                            && tableControl.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex)))
                        {
                            Color clr = grid.Table.TableOptions.SelectionBackColor;
                            Brush br;
                            if (clr.A != 255)
                            {
                                br = new SolidBrush(clr);
                            }
                            else
                            {
                                br = new SolidBrush(Color.FromArgb(64, grid.Table.TableOptions.SelectionBackColor));
                            }

                            e.Graphics.FillRectangle(br, e.Bounds);
                            br.Dispose();
                        }

                        break;
                    }

                    case GridListBoxSelectionColorOptions.InvertCells:
                    {
                        GridListBoxSelectionCurrentCellOptions currentCellOptions = this.grid.Table.TableOptions.ListBoxSelectionCurrentCellOptions;
                        if (!((currentCellOptions & GridListBoxSelectionCurrentCellOptions.WhiteCurrentCell) != 0
                            && tableControl.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex)))
                        {
                            Rectangle bounds = e.Bounds;
                            bounds.Intersect(grid.ViewLayout.ScrollAreaBounds);
                            grid.InvertRect(e.Graphics, bounds);
                        }

                        break;
                    }
                }
            }

            GridBorder listBoxSelectionOutlineBorder = grid.TableDescriptor.TableOptions.ListBoxSelectionOutlineBorder;
            if (!listBoxSelectionOutlineBorder.IsEmpty)
            {
                GridTableCellStyleInfoIdentity id = style.TableCellIdentity;
                Element el = id.DisplayElement;

                if (id.TableCellType == GridTableCellType.RecordFieldCell
                    || id.TableCellType == GridTableCellType.AlternateRecordFieldCell)
                {
                    if (el is Record || el is RecordRow)
                    {
                        Record r = Record.GetRecord(el);

                        bool isFirstRow = false;
                        bool isLastRow = false;
                        bool isFirstColumn = false;
                        bool isLastColumn = false;
                        Rectangle rect = e.Bounds;

                        if (r != null && r.IsSelected())
                        {
                            //// check previous row
                            int pos = grid.Table.NestedDisplayElements.IndexOf(el);
                            if (pos > 0)
                            {
                                Element el2 = grid.Table.NestedDisplayElements[pos - 1];
                                if (el2 is Record || el2 is RecordRow)
                                {
                                    Record r2 = Record.GetRecord(el2);
                                    if (r2 == null || !r2.IsSelected() || r2.ParentTable != r.ParentTable)
                                    {
                                        isFirstRow = true;
                                    }
                                }
                                else
                                {
                                    isFirstRow = true;
                                }
                            }

                            //// check next row
                            if (pos < grid.Table.NestedDisplayElements.Count - 1)
                            {
                                Element el2 = grid.Table.NestedDisplayElements[pos + 1];
                                if (el2 is Record || el2 is RecordRow)
                                {
                                    Record r2 = Record.GetRecord(el2);
                                    if (r2 == null || !r2.IsSelected() || r2.ParentTable != r.ParentTable)
                                    {
                                        isLastRow = true;
                                    }
                                }
                                else
                                {
                                    isLastRow = true;
                                }
                            }

                            GridTableDescriptor td = (GridTableDescriptor) r.ParentTable.TableDescriptor;

                            //// check column
                            int col1 = e.ColIndex;

                            if (col1 == td.GetColumnIndentCount())
                            {
                                isFirstColumn = true;
                            }

                            //// covered range?
                            int col2 = e.ColIndex;
                            GridRangeInfo rg = tableControl.Model.CoveredRanges.FindRange(e.RowIndex, e.ColIndex);
                            if (rg != null && !rg.IsEmpty)
                            {
                                col2 = e.ColIndex + rg.Width - 1;
                            }

                            if (col1 == td.GetColCount() - 1)
                            {
                                isLastColumn = true;
                            }

                            GridBorderSide side = (GridBorderSide) 0;
                            if (isFirstColumn)
                            {
                                side |= GridBorderSide.Left;
                            }

                            if (isLastColumn)
                            {
                                side |= GridBorderSide.Right;
                                rect.Width -= listBoxSelectionOutlineBorder.Width;
                            }

                            if (isLastRow)
                            {
                                side |= GridBorderSide.Bottom;
                                rect.Height -= listBoxSelectionOutlineBorder.Width;
                            }

                            if (isFirstRow)
                            {
                                side |= GridBorderSide.Top;
                            }

                            if ((int) side != 0)
                            {
                                GridBorderPaint.DrawRectangle(e.Graphics, listBoxSelectionOutlineBorder, rect, style.BackColor, side);
                            }
                        }
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void SelectedRecordPrepareViewStyleInfo(GridTableControl tableControl, GridPrepareViewStyleInfoEventArgs e)
        {
            GridTableCellStyleInfo style = (GridTableCellStyleInfo) e.Style;

            if (e.Cancel || style.TableCellIdentity == null)
            {
                return;
            }

            if (style.TableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AlternateRecordFieldCell
                || style.TableCellIdentity.TableCellType == GridTableCellType.AddNewRecordFieldCell)
            {
                GridListBoxSelectionColorOptions colorOptions = this.grid.Table.TableOptions.ListBoxSelectionColorOptions;
                switch (colorOptions)
                {
                    case GridListBoxSelectionColorOptions.ApplySelectionColor:
                    {
                        GridListBoxSelectionCurrentCellOptions currentCellOptions = this.grid.Table.TableOptions.ListBoxSelectionCurrentCellOptions;
                        if (!((currentCellOptions & GridListBoxSelectionCurrentCellOptions.WhiteCurrentCell) != 0
                            && tableControl.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex)))
                        {
                            e.Style.BackColor = grid.Table.TableOptions.SelectionBackColor;
                            e.Style.TextColor = grid.Table.TableOptions.SelectionTextColor;
                        }

                        break;
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void ProcessSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (this.inChangeSelectionHelper || !this.grid.Visible)
            {
                return;
            }

            switch (e.Action)
            {
                case SelectedRecordsChangedType.Reset:
                {
                    foreach (SelectedRecord sr in grid.Table.SelectedRecords)
                    {
/*                        int rowIndex1 = grid.Table.NestedDisplayElements.IndexOf(sr.Record);
//                        int rowIndex2 = rowIndex1 + sr.Record.RecordRows.Count;
//                        if (grid.ViewLayout.IsRowVisible(rowIndex1) || grid.ViewLayout.IsRowVisible(rowIndex2))
                            grid.InvalidateRange(GridRangeInfo.Rows(rowIndex1, rowIndex2));*/
                        if (!sr.Record.IsDisposed)
                        {
                            grid.InvalidateElement(sr.Record.GetRecordDisplayElement());
                        }
                    }

                    break;
                }
            }
        }

        internal void Init()
        {
            if (grid.Table != null && grid.Table.SelectedRecords.Count > 0)
            {
                SelectedRecord sr = grid.Table.SelectedRecords[grid.Table.SelectedRecords.Count-1];
                if (!sr.Record.IsDisposed)
                {
                    grid.Table.FilteredChildTable = sr.Record.ParentChildTable;
                    GridRangeInfo rg = grid.Table.GetRecordRangeInfo(sr.Record);
                    //// if (grid.ViewLayout.IsRowVisible(rg.Top) || grid.ViewLayout.IsRowVisible(rg.Bottom))
                    //// grid.InvalidateRange(rg);
                    if (sr.Record.ParentTable.RecordsAsDisplayElements)
                    {
                        grid.InvalidateElement(sr.Record);
                    }
                    else
                    {
                        grid.InvalidateElement(sr.Record.RecordParts[0]);
                    }

                    grid.Table.activeRange = rg;
                    grid.Table.mouseDownRowIndex = rg.Top;
                }
            }
        }

        /// <summary>
        /// Synchronizes the grid with active range of selection.
        /// </summary>
        public void SyncActiveRange()
        {
            if (grid.Table.SelectedRecords.Count > 0)
            {
                Record r = null;
                if (lastSelectedRecord != null)
                {
                    r = lastSelectedRecord.Target as Record;
                }

                if (r == null || r.IsDisposed)
                {
                SelectedRecord sr = grid.Table.SelectedRecords[grid.Table.SelectedRecords.Count - 1];
                    r = sr.Record;
                }

                if (!r.IsDisposed && r.ParentTable == grid.Table)
                {
                    grid.Table.FilteredChildTable = r.ParentChildTable;
                    GridRangeInfo rg = grid.Table.GetRecordRangeInfo(r);
                    if (grid.Table.activeRange == null || !grid.Table.activeRange.Contains(rg))
                    {
                        grid.Table.activeRange = rg;
                        grid.Table.mouseDownRowIndex = rg.Top;
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>Internaly only.</summary>
        public void ProcessSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (this.inChangeSelectionHelper || !this.grid.Visible)
            {
                return;
            }

            switch (e.Action)
            {
                case SelectedRecordsChangedType.Added:
                {
                    GridRangeInfo rg = grid.Table.GetRecordRangeInfo(e.SelectedRecord.Record);
//// if (grid.ViewLayout.IsRowVisible(rg.Top) || grid.ViewLayout.IsRowVisible(rg.Bottom))
//// grid.InvalidateRange(rg);
                    grid.InvalidateElement(e.SelectedRecord.Record.GetRecordDisplayElement());
                    grid.Table.activeRange = rg;
                    grid.Table.mouseDownRowIndex = rg.Top;
                    break;
                }

                case SelectedRecordsChangedType.Removed:
                {
                    GridRangeInfo rg = grid.Table.GetRecordRangeInfo(e.SelectedRecord.Record);
                    if (grid.ViewLayout.IsRowVisible(rg.Top) || grid.ViewLayout.IsRowVisible(rg.Bottom))
                    {
                        grid.InvalidateRange(rg);
                    }

                    break;
                }
            }
        }
    }
#endif
}