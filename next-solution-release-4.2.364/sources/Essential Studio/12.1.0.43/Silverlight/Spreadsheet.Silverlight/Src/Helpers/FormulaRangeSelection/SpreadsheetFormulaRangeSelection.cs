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
using Syncfusion.Windows.Controls.Grid;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media;
using Syncfusion.Windows.GridCommon;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Input;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Data;
#if SILVERLIGHT
using TabControlExt = Syncfusion.Windows.Tools.Controls.TabControlAdv;
using Syncfusion.Windows.Shared;
#endif

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    /// <summary>
    /// Supports selecting ranges and cells while editing a formulacell to insert
    /// range references and cell references into the editing formula text. You
    /// can use either the mouse or the keyboard arrow keys to select the range
    /// to be inserted.
    /// </summary>
    public class SpreadsheetFormulaRangeSelection : IDisposable
    {
        internal SpreadsheetGrid activeGrid, editingGrid;
        SpreadsheetControl spreadsheet;
        TabControlExt excelTabcontrol;
        TextBox formulaBar;
        TextBox gtb;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpreadsheetFormulaRangeSelection()
        {
                
        } 

        public SpreadsheetFormulaRangeSelection(SpreadsheetControl spreadsheet, TabControlExt excelTabcontrol, TextBox formulaBar)
        {
            this.excelTabcontrol = excelTabcontrol;
            this.formulaBar = formulaBar;
            this.spreadsheet = spreadsheet;

            this.spreadsheet.WorkBookLoaded += new WorkbookLoadedEventHandler(spreadsheet_WorkBookLoaded);
            this.spreadsheet.WorkSheetAdded += new WorkSheetAddedEventHandler(spreadsheet_WorkSheetAdded);
            this.excelTabcontrol.GotFocus += new RoutedEventHandler(excelTabcontrol_GotFocus);
#if !SILVERLIGHT
            this.formulaBar.PreviewKeyDown += new KeyEventHandler(formulaBar_PreviewKeyDown);
            this.formulaBar.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(formulaBar_PreviewMouseLeftButtonUp);
            this.formulaBar.PreviewKeyUp += new KeyEventHandler(formulaBar_PreviewKeyUp);
            this.excelTabcontrol.ContextMenuOpening += new ContextMenuEventHandler(excelTabcontrol_ContextMenuOpening);
            this.excelTabcontrol.SelectedItemChangedEvent += new SelectedItemChangedEventHandler(excelTabcontrol_SelectedItemChangedEvent);
#else
            this.formulaBar.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(formulaBar_PreviewMouseLeftButtonUp), true);
            this.formulaBar.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(formulaBar_PreviewKeyDown), true);
            this.formulaBar.AddHandler(TextBox.KeyUpEvent, new KeyEventHandler(formulaBar_PreviewKeyUp), true);
#endif
        }
        
        internal bool IsInFormulaEditing
        {
            get
            {
                return isInFormulaEditing;
            }
            set
            {
                isInFormulaEditing = value;
                if (value)
                    this.spreadsheet.GridProperties.IsDisableMode = true;
                else if (!this.spreadsheet.GridProperties.IsFocusedOnGraphicCells && !this.spreadsheet.ExcelProperties.IsPasswordProtected)
                    this.spreadsheet.GridProperties.IsDisableMode = false;
            }
        }

        #region worksheet loaded and added event handling code
        
        void spreadsheet_WorkSheetAdded(object sender, WorkSheetAddedEventArgs args)
        {
            if (spreadsheet.EnableFormulaRangeSelection)
            {
                this.activeGrid = (sender as SpreadsheetControl).GridProperties.ActiveSpreadsheetGrid;
                HookEvents(this.activeGrid);
            }
        }

        void spreadsheet_WorkBookLoaded(object sender, WorkbookLoadedEventArgs args)
        {
            if (spreadsheet.EnableFormulaRangeSelection)
            {
                foreach (var grid in args.GridCollection)
                {
                    HookEvents(grid);
                }
            }
        }
        
        #endregion

        #region TabControl handling code

        void excelTabcontrol_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isInFormulaEditing)
            {
#if !SILVERLIGHT
                if (editingGrid == activeGrid && gtb != null)
                    gtb.Focus();
                else
                    formulaBar.Focus();
#else
                if (editingGrid != activeGrid)
                    formulaBar.Focus();
#endif
            }
        }

#if !SILVERLIGHT
        void excelTabcontrol_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (spreadsheet.EnableFormulaRangeSelection && isInFormulaEditing)
                e.Handled = true;
        }

        void excelTabcontrol_SelectedItemChangedEvent(object sender, SelectedItemChangedEventArgs e)
        {
            if (isInFormulaEditing && Keyboard.Modifiers == ModifierKeys.Control)
                e.Cancel = true;
        }
#else
        void contMenu_Opened(object sender, RoutedEventArgs e)
        {
            (sender as ContextMenuAdv).IsOpen = false;
        }
#endif

        #endregion

        #region formula bar handling code

        void formulaBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isInFormulaEditing)
            {
                selectionStart = gtb.SelectionStart = formulaBar.SelectionStart;
                gtb.SelectionLength = formulaBar.SelectionLength;
                isCurrentCellClick = true;
                ClearSelectionAndSetCursor(false);
            }
            else if (this.spreadsheet.EnableFormulaRangeSelection && activeSheetName == activeGrid.SheetName 
                && !this.spreadsheet.GridProperties.IsFocusedOnGraphicCells)
            {
                if (!activeGrid.CurrentCell.IsEditing)
                {
                    activeGrid.CurrentCell.BeginEdit(true);
                    formulaBar.Focus();
                }
                if (activeGrid.CurrentCell.Renderer != null)
                    gtb = activeGrid.CurrentCell.Renderer.CurrentCellUIElement as TextBox;

                if (gtb != null)
                {
                    selectionStart = gtb.SelectionStart = formulaBar.SelectionStart;
                    gtb.SelectionLength = formulaBar.SelectionLength;
                }

            }
        }

        void formulaBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.spreadsheet.EnableFormulaRangeSelection && !this.spreadsheet.GridProperties.IsFocusedOnGraphicCells)
            {
                KeyDownHandler(e, true);
                if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Escape)
                    formulaBar.Focus();
            }
        }

        void formulaBar_PreviewKeyUp(object sender, KeyEventArgs e)
        {
#if !SILVERLIGHT
            if (isInFormulaEditing && e.Key != Key.RightCtrl && e.Key != Key.LeftCtrl && e.Key != Key.RightShift && e.Key != Key.LeftShift)
#else
            if (isInFormulaEditing && e.Key != Key.Ctrl && e.Key != Key.Shift)
#endif
            {
                gtb.Text = formulaBar.Text;
                editingGrid.CurrentCell.Renderer.ControlText = formulaBar.Text;
                if (gtb.Text == "")
                {
                    excelTabcontrol.SelectedIndex = editingTabIndex;
                    editingGrid.ScrollInView(editingGrid.CurrentCell.CellRowColumnIndex);
                }
                if (isCurrentCellClick || (e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Up && e.Key != Key.Down))
                {
                    selectionStart = gtb.SelectionStart = formulaBar.SelectionStart;
                    gtb.SelectionLength = formulaBar.SelectionLength;
                }
            }
            else if (this.spreadsheet.EnableFormulaRangeSelection && !IsInFormulaEditing && !this.spreadsheet.GridProperties.IsFocusedOnGraphicCells)
            {
                if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Escape)
                {
                    if (activeGrid.CurrentCell.Renderer != null)
                        gtb = activeGrid.CurrentCell.Renderer.CurrentCellUIElement as TextBox;
                    if (gtb != null)
                    {
                        gtb.Text = formulaBar.Text;
                        selectionStart = gtb.SelectionStart = formulaBar.SelectionStart;
                        if (CanPlaceText())
                        {
                            StartFormulaEditing();
                            editingTabIndex = excelTabcontrol.SelectedIndex;
                        }
                    }
                    formulaBar.Focus();
                }
            }
        }

        #endregion

        #region mouse handling code

        private GridStyleInfo editingCellStyle;
        private int editingTabIndex = 0;
        private bool isInFormulaEditing = false;
        private bool isFormulaWithSheetName, isSheetChanged = false;
        private bool isCurrentCellClick = false;
        private bool isHeaderRowCellClick, isHeaderColCellClick = false;
        private string activeSheetName, editingSheetName;
        private bool inRangeSelection = false;
        private bool usingMouse = false;
        private GridRangeInfo tempRange = GridRangeInfo.Empty;
        private GridRangeInfo selectedRange = GridRangeInfo.Empty;
        private GridRangeInfoList selectedRangesList = GridRangeInfoList.Empty;
        private int selectionStart = 0;
        private string validPrecedingChars = " (+-*/^&<>=,:!";

#if !SILVERLIGHT
        void FormulaRangeSelection_PreviewMouseUp(object sender, MouseButtonEventArgs e)
#else
        void grid_CellMouseUp(object sender, GridCellMouseControllerEventArgs args)
#endif
        {
            if (IsInFormulaEditing)
            {
                if (inRangeSelection)
                {
                    inRangeSelection = false;
                    isHeaderRowCellClick = isHeaderColCellClick = false;
                    usingMouse = false;
                    isSheetChanged = false;
                    editingGrid.Focus();
#if !SILVERLIGHT
                    e.Handled = true;
#else
                    args.MouseControllerEventArgs.Handled = true;
#endif
                }
                if (isCurrentCellClick && gtb != null)
                    selectionStart = gtb.SelectionStart;
            }
        }

        void FormulaRangeSelection_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            currentCell = activeGrid.PointToCellRowColumnIndex(e);
            if (isInFormulaEditing && isCurrentCellClick && currentCell != this.editingGrid.CurrentCell.CellRowColumnIndex)
            {
#if !SILVERLIGHT
                e.Handled = true;
#endif
                return;
            }
            if (inRangeSelection && usingMouse)
            {
                if (isHeaderRowCellClick && isHeaderColCellClick)
                {
#if !SILVERLIGHT
                e.Handled = true;
#endif
                    return;
                }
                int top = 0, bottom = 0, left = 0, right = 0;
                activeGrid.ScrollRows.GetVisibleSection(ScrollAxisRegion.Body, out top, out bottom);
                activeGrid.ScrollColumns.GetVisibleSection(ScrollAxisRegion.Body, out left, out right);

                if (!isHeaderColCellClick)
                {
                    if (currentCell.RowIndex >= activeGrid.ScrollRows.LastBodyVisibleLineIndex)
                    {
                        activeGrid.ScrollRows.ScrollToNextLine();
                    }
                    else if (currentCell.RowIndex <= top)
                    {
                        activeGrid.ScrollRows.ScrollToPreviousLine();
                    }
                }
                if (!isHeaderRowCellClick)
                {
                    if (currentCell.ColumnIndex >= activeGrid.ScrollColumns.LastBodyVisibleLineIndex)
                    {
                        activeGrid.ScrollColumns.ScrollToNextLine();
                    }
                    else if (currentCell.ColumnIndex <= left)
                    {
                        activeGrid.ScrollColumns.ScrollToPreviousLine();
                    }
                }
                if (currentCell.RowIndex >= 0 && currentCell.ColumnIndex >= 0 &&
                    (currentCell.ColumnIndex != lastCol || currentCell.RowIndex != lastRow))
                {
                    if ((currentCell.RowIndex != 0 || isHeaderColCellClick) && (currentCell.ColumnIndex != 0 || isHeaderRowCellClick))
                    {
                        if ((!isHeaderRowCellClick || currentCell.ColumnIndex == 0) &&
                            (!isHeaderColCellClick || currentCell.RowIndex == 0))
                        {
                            lastCol = currentCell.ColumnIndex;
                            lastRow = currentCell.RowIndex;
                            GridRangeInfo range = selectedRange;
                            selectedRange = GridRangeInfo.Cell(startRow, startCol).UnionRange(GridRangeInfo.Cell(lastRow, lastCol));

                            selectedRange = ComputeCoveredRange(selectedRange, this.activeGrid);
                            PlaceTextInCell(false, selectedRange, true);
                            activeGrid.InvalidateCell(range.UnionRange(selectedRange));
                            activeGrid.InvalidateVisual(true);
#if !SILVERLIGHT
                e.Handled = true;
#endif
                        }
                    }
                }
            }
        }

#if !SILVERLIGHT
        void FormulaRangeSelection_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void grid_CellMouseDown(object sender, GridCellMouseControllerEventArgs args)
#endif
        {
#if !SILVERLIGHT
            this.activeGrid = (sender as ScrollViewer).FindElementOfType<SpreadsheetGrid>();
#else
            this.activeGrid = sender as SpreadsheetGrid;
#endif
            isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            activeSheetName = this.activeGrid.SheetName;
#if !SILVERLIGHT
            if (activeGrid.IsMouseOver && this.spreadsheet.EnableFormulaRangeSelection)
            {
                currentCell = activeGrid.PointToCellRowColumnIndex(new MouseEventArgs(e.MouseDevice, e.Timestamp));
#else
            if (this.spreadsheet.EnableFormulaRangeSelection)
            {
                currentCell = activeGrid.PointToCellRowColumnIndex(args.MouseControllerEventArgs.Location);
#endif
                if (currentCell.RowIndex >= 0 && currentCell.ColumnIndex >= 0)
                {
                    if (activeGrid.CurrentCell.CellRowColumnIndex != currentCell || isInFormulaEditing)
                    {
                        if (currentCell.RowIndex == 0)
                            isHeaderColCellClick = true;
                        if (currentCell.ColumnIndex == 0)
                            isHeaderRowCellClick = true;

                        if (activeGrid.CurrentCell.IsEditing || isInFormulaEditing)
                        {
                            if (isInFormulaEditing)
                            {
                                GridRangeInfo coveredRanges = GridRangeInfo.Cell(currentCell.RowIndex, currentCell.ColumnIndex);
                                if (activeGrid == editingGrid)
                                {
                                    coveredRanges = ComputeCoveredRange(coveredRanges, this.editingGrid);
                                    currentCell = new RowColumnIndex(coveredRanges.Top, coveredRanges.Left);
                                    if (currentCell == editingGrid.CurrentCell.CellRowColumnIndex)
                                    {
                                        isCurrentCellClick = true;
                                        ClearSelectionAndSetCursor(false);
                                        return;
                                    }
                                }
                            }

                            if (CanPlaceText())
                            {
                                if (isCurrentCellClick)
                                    gtb.SelectedText = string.Empty;

                                if (!isInFormulaEditing)
                                {
                                    StartFormulaEditing();
                                    editingTabIndex = excelTabcontrol.SelectedIndex;
                                }

                                inRangeSelection = true;
                                usingMouse = true;
                                isCurrentCellClick = false;
                                GridRangeInfo range = selectedRange;
                                if (isShiftKey && !isControlKey)
                                    ExpandSelection();
                                else
                                {
                                    startRow = currentCell.RowIndex;
                                    startCol = currentCell.ColumnIndex;
                                    GridRangeInfo coveredRanges;
                                    if (this.activeGrid.Model.CoveredRanges.Find(startRow, startCol, out coveredRanges))
                                    {
                                        if (coveredRanges != GridRangeInfo.Empty)
                                        {
                                            startRow = coveredRanges.Top;
                                            startCol = coveredRanges.Left;
                                        }
                                    }
                                    selectedRange = GridRangeInfo.Cell(startRow, startCol);
                                }

                                PlaceTextInCell(false, selectedRange, false);
                                activeGrid.InvalidateCell(selectedRange.UnionRange(range));
                                activeGrid.InvalidateVisual(true);
#if !SILVERLIGHT
                                e.Handled = true;
#else
                                args.MouseControllerEventArgs.Handled = true;
#endif
                                return;
                            }
                            else
                            {
                                try
                                {
                                    if (activeGrid.CurrentCell.Renderer.CellModel is GridCellFormulaModel)
                                    {
                                        //check for missing right parenthesis
                                        string s = activeGrid.CurrentCell.Renderer.ControlText;
                                        int loc = s.LastIndexOf('(');
                                        if (loc > -1)
                                        {
                                            int loc1 = s.LastIndexOf(')');
                                            if (loc1 < loc || loc1 == -1)
                                            {
                                                activeGrid.CurrentCell.Renderer.ControlText = s + ")";
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }
                }
                if (isInFormulaEditing)
                {
                    excelTabcontrol.SelectedIndex = editingTabIndex;
                    editingGrid.CurrentCell.EndEdit();
                    editingGrid.CurrentCell.MoveTo(currentCell);
                    editingGrid.InvalidateCells();
                    editingGrid.InvalidateVisual(true);
                }
            }

        }

        #endregion

        #region keyboard handling code
        
        private bool isControlKey = false;
        private bool isShiftKey = false;
        
#if !SILVERLIGHT
        void FormulaRangeSelection_PreviewKeyDown(object sender, KeyEventArgs e)
         {
             if (spreadsheet.EnableFormulaRangeSelection)
                KeyDownHandler(e, false);
        }
#else
        void grid_CurrentCellKeyDown(object sender, GridCellKeyEventArgs args)
        {
            if (this.spreadsheet.EnableFormulaRangeSelection)
                KeyDownHandler(args.KeyEventArgs, false);
            if (args.KeyEventArgs.Handled)
                args.Handled = true;
        }
#endif
        void FormulaRangeSelection_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Escape)
            {
#if !SILVERLIGHT
                if (isInFormulaEditing && e.Key != Key.RightCtrl && e.Key != Key.LeftCtrl && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    selectionStart = formulaBar.SelectionStart = gtb.SelectionStart;
#else
                if (isInFormulaEditing && e.Key != Key.Ctrl && e.Key != Key.Shift)
                    selectionStart = formulaBar.SelectionStart = gtb.SelectionStart;
                if (gtb != null && e.Key != Key.Ctrl && e.Key != Key.Shift)
                    formulaBar.Text = gtb.Text;
#endif
            }
        }
        
        void KeyDownHandler(KeyEventArgs e, bool isFormulaBar)
        {
            isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;

            if (currentCell == RowColumnIndex.Empty)
            {
                currentCell = activeGrid.CurrentCell.CellRowColumnIndex;
            }

            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    if (isInFormulaEditing)
                        HandleEnterAndTab(e);
                    break;
                case Key.Escape:
                    if (isInFormulaEditing)
                    {
                        excelTabcontrol.SelectedIndex = editingTabIndex;
                        (editingGrid.CurrentCell.Renderer.CurrentCellUIElement as TextBox).Text = editingCellStyle.CellValue.ToString();
                        editingGrid.ScrollInView(editingGrid.CurrentCell.CellRowColumnIndex);
                        activeGrid.InvalidateCells();
                        activeGrid.InvalidateVisual(true);
                    }
                    break;
                case Key.Back:
                    ClearSelectionAndSetCursor(false);
                    break;
                case Key.Delete:
                    ClearSelectionAndSetCursor(false);
                    break;
                case Key.F2:
                    ClearSelectionAndSetCursor();
                    isCurrentCellClick = true;
                    break;
                case Key.Down:
                case Key.Up:
                case Key.Left:
                case Key.Right:
                    if (isInFormulaEditing && isCurrentCellClick)
                    {
                        if (e.Key == Key.Up || e.Key == Key.Down)
                            e.Handled = true;
                        if (gtb.SelectionStart == 0 && e.Key == Key.Left)
                            e.Handled = true;
                        if (gtb.SelectionStart == gtb.Text.Length && e.Key == Key.Right)
                            e.Handled = true;
                        return;
                    }
                    else if (this.activeGrid.CurrentCell.IsEditing && CanPlaceText())
                    {
                        ExpandSelection(e.Key, isControlKey, isShiftKey);
                        usingMouse = false;
                        e.Handled = true;
                        activeGrid.Focus();
                    }
                    break;
                default:
                    if (this.activeGrid.CurrentCell.IsEditing && !isInFormulaEditing)
                    {
                        if (this.activeGrid.CurrentCell.Renderer != null)
                            gtb = this.activeGrid.CurrentCell.Renderer.CurrentCellUIElement as TextBox;

                        if (gtb != null && gtb.Text.StartsWith("="))
                        {
                            StartFormulaEditing();
                            isCurrentCellClick = true;
                            editingTabIndex = excelTabcontrol.SelectedIndex;
                        }
                    }

#if !SILVERLIGHT
                    if (isInFormulaEditing && e.Key != Key.RightShift && e.Key != Key.LeftShift
                        && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
#else
                    if (isInFormulaEditing && e.Key != Key.Shift && e.Key != Key.Ctrl)
                    #endif
                    {
                        ClearSelectionAndSetCursor();
                        if (isInFormulaEditing && gtb != null)
                            gtb.Focus();

                        if (isControlKey || isShiftKey)
                            isCurrentCellClick = true;
                    }
                    break;
            }
        }

        private void HandleEnterAndTab(KeyEventArgs e)
        {
            excelTabcontrol.SelectedIndex = editingTabIndex;
            if (placeTextDone)
            {
                //check for missing right parenthesis
                string s = editingGrid.CurrentCell.Renderer.ControlText;
                int loc = s.LastIndexOf('(');
                if (loc > -1)
                {
                    int loc1 = s.LastIndexOf(')');
                    if (loc1 < loc || loc1 == -1)
                    {
                        editingGrid.CurrentCell.Renderer.ControlText = s + ")";
                    }
                }
            }
            ConfirmChangeAndResetFlags();
            e.Handled = true;
            if (e.Key == Key.Enter)
            {
                editingGrid.CurrentCell.MoveDown();
            }
            else
            {
                bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;

                if (isShiftKey)
                {
                    editingGrid.CurrentCell.MoveLeft();
                }
                else
                {
                    editingGrid.CurrentCell.MoveRight();
                }
            }

            editingGrid.InvalidateCells();
            editingGrid.InvalidateVisual(true);
        }

        private void ClearSelectionAndSetCursor()
        {
            ClearSelectionAndSetCursor(true);
        }

        private void ClearSelectionAndSetCursor(bool setTextCursor)
        {
            if (gtb != null)
            {
                if (isInFormulaEditing && !isCurrentCellClick && !string.IsNullOrEmpty(gtb.SelectedText) && setTextCursor)
                {
                    gtb.SelectionStart += gtb.SelectionLength;
                }
                else if(formulaBar.SelectionStart != 0)
                {
                    selectionStart = formulaBar.SelectionStart;
                }
                string header = this.activeGrid.SheetName;
                if (activeSheetName == header)
                {
                    activeGrid.InvalidateCell(selectedRange);
                    foreach (GridRangeInfo range in selectedRangesList)
                    {
                        activeGrid.InvalidateCell(range);
                    }
                    activeGrid.InvalidateVisual(true);
                    isSheetChanged = false;
                    startRow = startCol = 0;
                    selectedRange = tempRange = GridRangeInfo.Empty;
                    selectedRangesList.Clear();
                }
            }
        }

        private void ConfirmChangeAndResetFlags()
        {
            this.spreadsheet.GridProperties.CurrentExcelGridModel.IsInFormulabarEditing = false;
#if !SILVERLIGHT
            editingGrid.CurrentCell.ConfirmChanges();
#else
            bool isvalidationMsgShown, suspendMoveTo;
            editingGrid.CurrentCell.ConfirmChanges(out isvalidationMsgShown, out suspendMoveTo);
#endif
        }

        #endregion

        #region utility methods to manage the range selection work

        RowColumnIndex currentCell = RowColumnIndex.Empty;
        private bool placeTextDone = false;

        int lastRow = 0;
        int lastCol = 0;
        int startRow = 0;
        int startCol = 0;

        private void ExpandSelection(Key key, bool isControlKey, bool isShiftKey)
        {
            GridRangeInfo range = selectedRange;
            int row = range.IsEmpty ? activeGrid.CurrentCell.CellRowColumnIndex.RowIndex : range.Top;
            int col = range.IsEmpty ? activeGrid.CurrentCell.CellRowColumnIndex.ColumnIndex : range.Left;
            selectedRange = GridRangeInfo.Cell(startRow, startCol);
           
            GridRangeInfo coveredRanges;
            if (this.activeGrid.Model.CoveredRanges.Find(lastRow, lastCol, out coveredRanges))
            {
                if (coveredRanges != GridRangeInfo.Empty)
                {
                    switch(key)
                    {
                        case Key.Down:
                            lastRow = coveredRanges.Bottom;
                            break;
                        case Key.Up:
                            lastRow = coveredRanges.Top;
                            break;
                        case Key.Left:
                            lastCol = coveredRanges.Left;
                            break;
                        case Key.Right:
                            lastCol = coveredRanges.Right;
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (range.Height <= 1 && range.Width <= 1)
            {
                startRow = row;
                startCol = col;
                lastRow = row;
                lastCol = col;
            }

            if (!isControlKey)
            {
                switch (key)
                {
                    case Key.Down:
                        lastRow++;
                        break;
                    case Key.Up:
                        lastRow--;
                        break;
                    case Key.Left:
                        lastCol--;
                        break;
                    case Key.Right:
                        lastCol++;
                        break;
                    default:
                        break;
                }
            }
            else
            { //// control key goes to end of row/col
                switch (key)
                {
                    case Key.Down:
                        lastRow = activeGrid.Model.RowCount - 1;
                        break;
                    case Key.Up:
                        lastRow = 1;
                        break;
                    case Key.Left:
                        lastCol = 1;
                        break;
                    case Key.Right:
                        lastCol = activeGrid.Model.ColumnCount - 1;
                        break;
                    default:
                        break;
                }
            }
            lastRow = Math.Min(Math.Max(1, lastRow), activeGrid.Model.RowCount - 1);
            lastCol = Math.Min(Math.Max(1, lastCol), activeGrid.Model.ColumnCount - 1);
            if (!isShiftKey)
            {
                startRow = lastRow;
                startCol = lastCol;
            }

            inRangeSelection = true;
            if (!isInFormulaEditing)
            {
                StartFormulaEditing();
            }
            activeGrid.ScrollInView(new RowColumnIndex(lastRow, lastCol));
            selectedRange = GridRangeInfo.Cell(startRow, startCol).UnionRange(GridRangeInfo.Cell(lastRow, lastCol));
            selectedRange = ComputeCoveredRange(selectedRange, this.activeGrid);
            PlaceTextInCell(false, selectedRange, false);
            activeGrid.InvalidateCell(selectedRange.UnionRange(range));
            activeGrid.InvalidateVisual(true);

        }

        private void ExpandSelection()
        {
            GridRangeInfo range = selectedRange;
            lastRow = currentCell.RowIndex;
            lastCol = currentCell.ColumnIndex;
            if (startRow == 0)
                startRow = lastRow;
            if (startCol == 0)
                startCol = lastCol;

            selectedRange = GridRangeInfo.Cell(startRow, startCol).UnionRange(GridRangeInfo.Cell(lastRow, lastCol));
            selectedRange = ComputeCoveredRange(selectedRange, this.activeGrid);                       
        }

        private bool CanPlaceText()
        {
            bool b = false;
            if (!isInFormulaEditing && activeGrid.CurrentCell.Renderer != null)
            {
                gtb = activeGrid.CurrentCell.Renderer.CurrentCellUIElement as TextBox;
            }

            if (gtb != null)
            {
                string s = gtb.Text;
                b = s.StartsWith("=");
                if (b && isInFormulaEditing && gtb.SelectionStart == 0 && gtb.Text.Length > 1)
                {
                    if (formulaBar.SelectionStart != 0)
                        gtb.SelectionStart = formulaBar.SelectionStart;
                    else if (gtb.SelectedText == "")
                        gtb.SelectionStart = gtb.Text.Length;
                    else
                        gtb.SelectionStart += 1;

                    b = validPrecedingChars.IndexOf(s[gtb.SelectionStart - 1]) > -1;
                }
                if (b && gtb.SelectionStart > 0)
                {
                    b = validPrecedingChars.IndexOf(s[gtb.SelectionStart - 1]) > -1;
                }
            }
            return b;
        }

        private void PlaceTextInCell(bool resetSelection, GridRangeInfo activeRange, bool isInMouseMove)
        {
            if (activeRange.IsEmpty)
            {
                return;
            }
            string range = ComputeRangeValue(this.activeGrid, activeRange, true);

            if (isFormulaWithSheetName && !isSheetChanged)
            {
                range = activeSheetName + '!' + range;
                if (gtb.SelectionStart != 0 && gtb.Text[gtb.SelectionStart - 1] == '!')
                {
                    string lastchars = "(+-*/^&<>=,";
                    int i = gtb.Text.LastIndexOfAny(lastchars.ToCharArray());
                    if (i > -1)
                        selectionStart = i+1;
                }      
            }

            if (gtb != null)
            {

                int start = gtb.SelectionStart;
                if (start == 0)
                    start = gtb.SelectionStart = gtb.Text.Length;

                if (isControlKey && !isShiftKey && !isInMouseMove && usingMouse)
                {
                    gtb.SelectionStart += gtb.SelectionLength;
                    gtb.SelectionLength = 0;
                    var lastchar = gtb.Text[gtb.SelectionStart - 1];
                    if (lastchar == ')')
                    {
                        gtb.Text = gtb.Text.TrimEnd(')');
                        lastchar = gtb.Text[gtb.Text.Length - 1];
                        gtb.SelectionStart = start = gtb.Text.Length;
                    }

                    if (!validPrecedingChars.Contains(lastchar))
                    {
                        start = gtb.SelectionStart + 1;
                        gtb.SelectedText = "," + range;
                    }
                    else
                    {
                        start = gtb.SelectionStart;
                        gtb.SelectedText = range;
                    }
                    if (tempRange != GridRangeInfo.Empty && !selectedRangesList.Contains(tempRange))
                    {
                        selectedRangesList.Add(tempRange);
                    }
                    selectedRangesList.Add(selectedRange);
                    tempRange = selectedRange;
                }
                else
                {
                    if (string.IsNullOrEmpty(gtb.SelectedText))
                    {
                        selectedRangesList.Add(selectedRange);
                    }
                    else
                    {
                        if (!isControlKey && !isShiftKey)
                        {
                            string oldRange = ComputeRangeValue(this.activeGrid ,tempRange, false);

                            string oldText = string.Empty;
                            string tempText = gtb.Text.Remove(0, selectionStart);
                            if (selectionStart < gtb.Text.Length)
                                oldText = gtb.Text.Remove(selectionStart);
                            else
                                oldText = gtb.Text;

                            if (isFormulaWithSheetName && !isSheetChanged)
                                oldRange = activeSheetName + '!' + oldRange;

                            if (tempText.Contains(oldRange + ','))
                                tempText = tempText.Replace(oldRange + ',', "");
                            else if (tempText.Contains(oldRange))
                                tempText = tempText.Replace(oldRange, "");

                            foreach (GridRangeInfo r in selectedRangesList)
                            {
                                oldRange = ComputeRangeValue(this.activeGrid, r, false);
                                
                                if (isFormulaWithSheetName && !isSheetChanged)
                                    oldRange = activeSheetName + '!' + oldRange;
                                
                                if (tempText.Contains(oldRange + ','))
                                    tempText = tempText.Replace(oldRange + ',', "");
                                else if (tempText.Contains(oldRange))
                                    tempText = tempText.Replace(oldRange, "");
                                
                                activeGrid.InvalidateCell(r);
                            }
                            gtb.Text = oldText + tempText;
                            string validchars = " (+-*/^&<>=,:!";
                            var index = gtb.Text.LastIndexOfAny(validchars.ToCharArray());
                            if (selectionStart != 0)
                            {
                                start = gtb.SelectionStart = selectionStart;
                                gtb.SelectionLength = 0;
                            }
                            else if (index > -1)
                            {
                                start = gtb.SelectionStart = index + 1;
                                gtb.SelectionLength = 0;
                            }
                            selectedRangesList.Clear();
                        }
                        if (tempRange != GridRangeInfo.Empty && selectedRangesList.Contains(tempRange))
                        {
                            selectedRangesList.Remove(tempRange);
                        }
                        selectedRangesList.Add(selectedRange);
                    }
                    gtb.SelectedText = range;
                    tempRange = selectedRange;

                }
                if (resetSelection)
                {
                    gtb.SelectionStart = start + range.ToString().Length;
                    gtb.SelectionLength = 0;
                }
                else
                {
                    gtb.SelectionStart = start;
                    gtb.SelectionLength = range.ToString().Length;
                }
                editingGrid.CurrentCell.Renderer.ControlText = gtb.Text;
                formulaBar.Text = gtb.Text;
                if (formulaBar.SelectionStart == 0)
                    formulaBar.SelectionStart = formulaBar.Text.Length;
                placeTextDone = true;
            }
        }

        private string ComputeRangeValue(GridControl grid, GridRangeInfo activeRange, bool isNewRange)
        {
            
            string range = GridRangeInfo.GetAlphaLabel(activeRange.Left) + GridRangeInfo.GetNumericLabel(activeRange.Top);

            if ((activeRange.Top == 0 && activeRange.Left == 0) ||
                (activeRange.Top == 1 && activeRange.Bottom == grid.Model.RowCount - 1) &&
                (activeRange.Left == 1 && activeRange.Right == grid.Model.ColumnCount - 1))
            {
                range = "1" + ':' + (grid.Model.RowCount - 1).ToString();

                if (isNewRange && grid == activeGrid)
                    selectedRange = GridRangeInfo.Cells(1, 1, grid.Model.RowCount - 1, grid.Model.ColumnCount - 1);
                else if (isNewRange)
                    tempRange = GridRangeInfo.Cells(1, 1, this.activeGrid.Model.RowCount - 1, this.activeGrid.Model.ColumnCount - 1);
            }

            else if (activeRange.Top == 0 || (activeRange.Top == 1 && activeRange.Bottom == grid.Model.RowCount - 1))
            {
                if (isNewRange && grid == activeGrid)
                {
                    selectedRange = GridRangeInfo.Cells(activeRange.Top + 1, activeRange.Left, grid.Model.RowCount - 1, activeRange.Right);
                    activeRange = selectedRange = ComputeCoveredRange(selectedRange, grid);
                }
                else if (isNewRange)
                    tempRange = GridRangeInfo.Cells(activeRange.Top, activeRange.Left, this.activeGrid.Model.RowCount - 1, activeRange.Right);
                
                range = GridRangeInfo.GetAlphaLabel(activeRange.Left) + ':' + GridRangeInfo.GetAlphaLabel(activeRange.Right);

            }

            else if (activeRange.Left == 0 || (activeRange.Left == 1 && activeRange.Right == grid.Model.ColumnCount - 1))
            {
                if (isNewRange && grid == activeGrid)
                {
                    selectedRange = GridRangeInfo.Cells(activeRange.Top, activeRange.Left + 1, activeRange.Bottom, grid.Model.ColumnCount - 1);
                    activeRange = selectedRange = ComputeCoveredRange(selectedRange, grid);
                }
                else if(isNewRange)
                    tempRange = GridRangeInfo.Cells(activeRange.Top, activeRange.Left, activeRange.Bottom, activeGrid.Model.ColumnCount - 1);
                
                range = GridRangeInfo.GetNumericLabel(activeRange.Top) + ':' + GridRangeInfo.GetNumericLabel(activeRange.Bottom);
            }

            else if (activeRange.Top != activeRange.Bottom || activeRange.Left != activeRange.Right)
            {
                range += ':' + GridRangeInfo.GetAlphaLabel(activeRange.Right) + GridRangeInfo.GetNumericLabel(activeRange.Bottom);
            }

            return range;
        }

        private GridRangeInfo ComputeCoveredRange(GridRangeInfo range, GridControl oldGrid)
        {
            oldGrid.Model.CoveredRanges.InvalidateRanges();
            range = oldGrid.Model.CoveredRanges.Ranges.GetOuterRange(range);

            this.activeGrid.Model.CoveredRanges.InvalidateRanges();
            range = this.activeGrid.Model.CoveredRanges.Ranges.GetOuterRange(range);

            GridRangeInfo coveredRanges;
            if (this.activeGrid.Model.CoveredRanges.Find(range.Top, range.Left, out coveredRanges))
            {
                if (coveredRanges != GridRangeInfo.Empty && range.Bottom == coveredRanges.Bottom && range.Right == coveredRanges.Right)
                {
                    range = GridRangeInfo.Cell(coveredRanges.Top, coveredRanges.Left);
                }
            }
            return range;
        }

        private void StartFormulaEditing()
        {
#if !SILVERLIGHT
            Binding formulaBinding = new Binding("Text");
            formulaBinding.Mode = BindingMode.TwoWay;
            formulaBinding.Source = gtb;
            formulaBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(formulaBar, TextBox.TextProperty, formulaBinding);
#else
            formulaBar.ClearValue(TextBox.TextProperty);
            formulaBar.Text = gtb.Text;
            formulaBar.SelectionStart = gtb.SelectionStart;
            formulaBar.SelectionLength = gtb.SelectionLength;
#endif
            formulaBar.IsReadOnly = false;
            IsInFormulaEditing = true;
            activeSheetName = editingSheetName = this.activeGrid.SheetName;
            editingTabIndex = excelTabcontrol.SelectedIndex;
            editingGrid = this.activeGrid;
            editingCellStyle = this.activeGrid.Model[activeGrid.CurrentCell.RowIndex, activeGrid.CurrentCell.ColumnIndex];
#if SILVERLIGHT
            if (this.spreadsheet.TabStyleManager.ShowTabItemContextMenu)
            {
                foreach (var tabitem in excelTabcontrol.Items)
                {
                    ContextMenuAdv contMenu = ContextMenuAdvService.GetContextMenuAdv(tabitem as DependencyObject);
                    contMenu.Opened += new RoutedEventHandler(contMenu_Opened);
                }
            }
#endif
        }
        
        private void EndFormulaEditing()
        {
            inRangeSelection = false;
            IsInFormulaEditing = false;

            Binding formulaBinding = new Binding("FormattedText");
            formulaBinding.Mode = BindingMode.TwoWay;
            formulaBinding.Source = spreadsheet.GridProperties;
#if !SILVERLIGHT
            formulaBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
#else
            formulaBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
#endif
            BindingOperations.SetBinding(formulaBar, TextBox.TextProperty, formulaBinding);

            isFormulaWithSheetName = false;
            isCurrentCellClick = false;
            selectedRange = tempRange = GridRangeInfo.Empty;
            selectedRangesList.Clear();
            placeTextDone = false;
            editingGrid.ScrollInView(editingGrid.CurrentCell.CellRowColumnIndex);
            gtb = null;
            startRow = startCol = 0;
            selectionStart = 0;
            currentCell = RowColumnIndex.Empty;
#if SILVERLIGHT
            if (this.spreadsheet.TabStyleManager.ShowTabItemContextMenu)
            {
                foreach (var tabitem in excelTabcontrol.Items)
                {
                    ContextMenuAdv contMenu = ContextMenuAdvService.GetContextMenuAdv(tabitem as DependencyObject);
                    contMenu.Opened -= new RoutedEventHandler(contMenu_Opened);
                }
            }
#endif
        }

        #endregion

        #region grid handling code

        void grid_CurrentCellStartEditing(object sender, ComponentModel.SyncfusionCancelRoutedEventArgs args)
        {
            this.activeGrid = sender as SpreadsheetGrid;
            if (this.spreadsheet.EnableFormulaRangeSelection && !isInFormulaEditing && CanPlaceText())
            {
                currentCell = (sender as SpreadsheetGrid).CurrentCell.CellRowColumnIndex;
                StartFormulaEditing();
            }
        }

        void grid_CurrentCellChanged(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            if (this.spreadsheet.EnableFormulaRangeSelection && !isInFormulaEditing && CanPlaceText())
            {
                currentCell = (sender as SpreadsheetGrid).CurrentCell.CellRowColumnIndex;
                StartFormulaEditing();
            }
        }

        void grid_CurrentCellEditingComplete(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            if (isInFormulaEditing)
            {
                excelTabcontrol.SelectedIndex = editingTabIndex;
                EndFormulaEditing();
                editingGrid.ScrollInView(editingGrid.CurrentCell.CellRowColumnIndex);
                editingGrid.InvalidateCells();
                editingGrid.InvalidateVisual(true);
            }
            else
                gtb = null;
        }

        void grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.spreadsheet.EnableFormulaRangeSelection)
            {
                GridControl oldGrid = this.activeGrid;
                this.activeGrid = sender as SpreadsheetGrid;
                string oldSheetName = activeSheetName;
                activeSheetName = activeGrid.SheetName;
                if (isInFormulaEditing)
                {
                    if (selectionStart == 0 && gtb.SelectedText != "")
                    {
                        gtb.Text = "=";
                        selectionStart = gtb.SelectionStart = 1;
                    }
                    string oldRange, newRange;
                    string validchars = "(+-*/^&<>=,";
                    if (tempRange == GridRangeInfo.Empty)
                    {
                        oldRange = string.Empty;
                        newRange = string.Empty;
                    }
                    else
                    {
                        editingGrid.InvalidateCell(tempRange);
                        oldRange = ComputeRangeValue(oldGrid, tempRange, true);
                        tempRange = selectedRange = ComputeCoveredRange(tempRange, oldGrid);
                        newRange = ComputeRangeValue(activeGrid, tempRange, false);
                    }

                    newRange = activeSheetName + '!' + newRange;
                        string oldText = string.Empty;
                        string tempText = gtb.Text.Remove(0, selectionStart);
                        if (selectionStart < gtb.Text.Length)
                            oldText = gtb.Text.Remove(selectionStart);
                        else
                            oldText = gtb.Text;
                    
                    if (isFormulaWithSheetName && oldRange != string.Empty)
                    {
                        if (gtb.SelectionStart > 0 && gtb.Text[gtb.SelectionStart - 1] == '!')
                        {
                            int i = gtb.Text.LastIndexOfAny(validchars.ToCharArray());
                            if (i > -1)
                                selectionStart = i + 1;
                            tempText = gtb.Text.Remove(0, selectionStart);
                            if (selectionStart < gtb.Text.Length)
                                oldText = gtb.Text.Remove(selectionStart);
                            else
                                oldText = gtb.Text;
                        }    

                        oldRange = oldSheetName + '!' + oldRange;
                        if (tempText.Contains(oldRange + ','))
                            tempText = tempText.Replace(oldRange + ',', "");
                        if (tempText.Contains(oldRange))
                            tempText = tempText.Replace(oldRange, newRange);
                    }
                    else if (oldRange != string.Empty)
                    {
                        if (tempText.Contains(oldRange + ','))
                            tempText = tempText.Replace(oldRange + ',', "");
                        if (tempText.Contains(oldRange))
                            tempText = tempText.Replace(oldRange, activeSheetName + '!');
                        editingGrid.InvalidateCell(tempRange);
                        tempRange = selectedRange = GridRangeInfo.Empty;
                        isSheetChanged = true;
                    }
                    else
                    {
                        if (gtb.Text == "")
                            return;
                        int i = selectionStart;
                        if (gtb.Text.Length > 0 && gtb.Text[gtb.Text.Length - 1] == '!')
                        {
                            string lastchars = "(+-*/^&<>=,";
                            i = gtb.Text.LastIndexOfAny(lastchars.ToCharArray());
                            if (i > -1)
                                gtb.Text = gtb.Text.Remove(i + 1);
                        }
                        gtb.Text += activeSheetName + '!';
                        isSheetChanged = true;
                        tempText = gtb.Text.Remove(0, i + 1);
                        oldText = gtb.Text.Remove(i + 1);
                    }

                    foreach (GridRangeInfo r in selectedRangesList)
                    {
                        oldRange = ComputeRangeValue(oldGrid, r, false);
                        if (isFormulaWithSheetName)
                            oldRange = oldSheetName + '!' + oldRange;
                        if (tempText.Contains(oldRange))
                            tempText = tempText.Replace(oldRange + ',', "");
                        editingGrid.InvalidateCell(r);
                    }
                    selectedRangesList.Clear();
                    gtb.Text = oldText + tempText;
                    var index = gtb.Text.LastIndexOfAny(validchars.ToCharArray());
                    if (index > -1)
                    {
                        selectionStart = gtb.SelectionStart = index + 1;
                        gtb.SelectionLength = (gtb.Text.Length) - (index + 1);
                    }
                    if (tempRange == GridRangeInfo.Empty)
                        selectionStart = gtb.SelectionStart = gtb.Text.Length;
                    isFormulaWithSheetName = true;
                    formulaBar.Text = gtb.Text;
                    if (activeGrid != editingGrid)
                    {
                        formulaBar.Focus();
                        formulaBar.SelectionStart = formulaBar.Text.Length;
                        spreadsheet.GridProperties.CurrentCellStyle = editingCellStyle;
                        activeGrid.InvalidateCells();
                        activeGrid.Model.Selections.Clear();
                    }
                    else
                    {
                        editingGrid.InvalidateCell(selectedRange);
                        gtb.Focus();
                    }
                }
                else if(oldGrid != null)
                {
                    this.activeGrid.InvalidateCells();
                    this.activeGrid.Focus();
                }
                this.activeGrid.InvalidateVisual(true);
            }
        }

        #endregion

        #region draw border for formula range selections

        void grid_PrepareRenderCell(object sender, GridPrepareRenderCellEventArgs e)
        {
            if (this.spreadsheet.EnableFormulaRangeSelection)
            {
                if (e.Cell.RowIndex != 0 && e.Cell.ColumnIndex != 0 && selectedRange.Contains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                {
                    GridRangeInfo comprange = ComputeBorderRange(e.Cell.RowIndex, e.Cell.ColumnIndex, selectedRange);
#if !SILVERLIGHT

                    if (e.Cell.RowIndex == comprange.Top)
                        e.Style.Borders.Top = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                    if (e.Cell.RowIndex == comprange.Bottom)
                        e.Style.Borders.Bottom = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                    if (e.Cell.ColumnIndex == comprange.Right)
                        e.Style.Borders.Right = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                    if (e.Cell.ColumnIndex == comprange.Left)
                        e.Style.Borders.Left = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
#else
                    if (e.Cell.RowIndex == comprange.Top)
                        e.Style.Borders.Top = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                    if (e.Cell.RowIndex == comprange.Bottom)
                        e.Style.Borders.Bottom = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                    if (e.Cell.ColumnIndex == comprange.Right)
                        e.Style.Borders.Right = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                    if (e.Cell.ColumnIndex == comprange.Left)
                        e.Style.Borders.Left = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };

#endif
                }
                foreach (GridRangeInfo range in selectedRangesList)
                {
                    if (e.Cell.RowIndex != 0 && e.Cell.ColumnIndex != 0 && range.Contains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                    {
                        GridRangeInfo comprange = ComputeBorderRange(e.Cell.RowIndex, e.Cell.ColumnIndex, range);
#if !SILVERLIGHT
                        if (e.Cell.RowIndex == comprange.Top)
                            e.Style.Borders.Top = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                        if (e.Cell.RowIndex == comprange.Bottom)
                            e.Style.Borders.Bottom = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                        if (e.Cell.ColumnIndex == comprange.Right)
                            e.Style.Borders.Right = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
                        if (e.Cell.ColumnIndex == comprange.Left)
                            e.Style.Borders.Left = new Pen() { Brush = new SolidColorBrush(Colors.Black), DashStyle = DashStyles.Dash, Thickness = 2.0 };
#else
                        if (e.Cell.RowIndex == comprange.Top)
                            e.Style.Borders.Top = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                        if (e.Cell.RowIndex == comprange.Bottom)
                            e.Style.Borders.Bottom = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                        if (e.Cell.ColumnIndex == comprange.Right)
                            e.Style.Borders.Right = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };
                        if (e.Cell.ColumnIndex == comprange.Left)
                            e.Style.Borders.Left = new Pen() { Brush = new SolidColorBrush(Colors.Black), Style = BorderStyle.Dotted, Thickness = 1.5 };

#endif
                    }
                }
            }
        }

        private GridRangeInfo ComputeBorderRange(int rowindex, int colindex, GridRangeInfo range)
        {
            GridRangeInfo coveredRanges;
            if (this.activeGrid.Model.CoveredRanges.Find(rowindex, colindex, out coveredRanges))
            {
                int bottom = range.Bottom, right = range.Right;
                if (coveredRanges != GridRangeInfo.Empty)
                {
                    if (range.Bottom == coveredRanges.Bottom)
                        bottom = coveredRanges.Top;
                    if (range.Right == coveredRanges.Right)
                        right = coveredRanges.Left;
                    range = GridRangeInfo.Cells(range.Top, range.Left, bottom, right);
                }
            }
            return range;
        }

        #endregion

        #region Hook and Unhook events of grid
        
        internal void HookEvents(GridControl grid)
        {
            if (grid == null)
                return;

            grid.Loaded += new RoutedEventHandler(grid_Loaded);
            grid.CurrentCellStartEditing += new ComponentModel.GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellEditingComplete += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellChanged += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellChanged);
            grid.PrepareRenderCell += new GridPrepareRenderCellEventHandler(grid_PrepareRenderCell);

            //used to catch keys and mouse before the grid can process them to check whether the RangeSelectionHelper should process them instead of the grid.
#if !SILVERLIGHT
            ((Control)grid.Parent).PreviewKeyDown += new KeyEventHandler(FormulaRangeSelection_PreviewKeyDown);
            ((Control)grid.Parent).PreviewKeyUp += new KeyEventHandler(FormulaRangeSelection_PreviewKeyUp);
            ((Control)grid.Parent).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(FormulaRangeSelection_PreviewMouseLeftButtonDown);
            ((Control)grid.Parent).PreviewMouseMove += new MouseEventHandler(FormulaRangeSelection_PreviewMouseMove);
            ((Control)grid.Parent).PreviewMouseUp += new MouseButtonEventHandler(FormulaRangeSelection_PreviewMouseUp);
#else
            ((Control)grid.Parent).KeyUp += new KeyEventHandler(FormulaRangeSelection_PreviewKeyUp);
            ((Control)grid.Parent).MouseMove += new MouseEventHandler(FormulaRangeSelection_PreviewMouseMove);
            grid.CellMouseUp += new GridCellMouseControllerEventHandler(grid_CellMouseUp);
            grid.CellMouseDown += new GridCellMouseControllerEventHandler(grid_CellMouseDown);
            grid.CurrentCellKeyDown += new GridCellKeyEventHandler(grid_CurrentCellKeyDown);
#endif
        }

        internal void UnHookEvents(GridControl grid)
        {
            if (grid == null)
                return;

            grid.Loaded -= new RoutedEventHandler(grid_Loaded);
            grid.CurrentCellStartEditing -= new ComponentModel.GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellEditingComplete -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellChanged -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellChanged);
            grid.PrepareRenderCell -= new GridPrepareRenderCellEventHandler(grid_PrepareRenderCell);

#if !SILVERLIGHT
            ((Control)grid.Parent).PreviewKeyDown -= new KeyEventHandler(FormulaRangeSelection_PreviewKeyDown);
            ((Control)grid.Parent).PreviewKeyUp -= new KeyEventHandler(FormulaRangeSelection_PreviewKeyUp);
            ((Control)grid.Parent).PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(FormulaRangeSelection_PreviewMouseLeftButtonDown);
            ((Control)grid.Parent).PreviewMouseMove -= new MouseEventHandler(FormulaRangeSelection_PreviewMouseMove);
            ((Control)grid.Parent).PreviewMouseUp -= new MouseButtonEventHandler(FormulaRangeSelection_PreviewMouseUp);
#else
            ((Control)grid.Parent).KeyUp -= new KeyEventHandler(FormulaRangeSelection_PreviewKeyUp);
            ((Control)grid.Parent).MouseMove -= new MouseEventHandler(FormulaRangeSelection_PreviewMouseMove);
            grid.CellMouseUp -= new GridCellMouseControllerEventHandler(grid_CellMouseUp);
            grid.CellMouseDown -= new GridCellMouseControllerEventHandler(grid_CellMouseDown);
            grid.CurrentCellKeyDown -= new GridCellKeyEventHandler(grid_CurrentCellKeyDown);
#endif
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.spreadsheet.WorkBookLoaded -= new WorkbookLoadedEventHandler(spreadsheet_WorkBookLoaded);
            this.spreadsheet.WorkSheetAdded -= new WorkSheetAddedEventHandler(spreadsheet_WorkSheetAdded);
            this.excelTabcontrol.GotFocus -= new RoutedEventHandler(excelTabcontrol_GotFocus);
#if !SILVERLIGHT

            this.formulaBar.PreviewKeyDown -= new KeyEventHandler(formulaBar_PreviewKeyDown);
            this.formulaBar.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(formulaBar_PreviewMouseLeftButtonUp);
            this.formulaBar.PreviewKeyUp -= new KeyEventHandler(formulaBar_PreviewKeyUp);
            this.excelTabcontrol.ContextMenuOpening -= new ContextMenuEventHandler(excelTabcontrol_ContextMenuOpening);
            this.excelTabcontrol.SelectedItemChangedEvent -= new SelectedItemChangedEventHandler(excelTabcontrol_SelectedItemChangedEvent);
#else
            this.formulaBar.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(formulaBar_PreviewMouseLeftButtonUp));
            this.formulaBar.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(formulaBar_PreviewKeyDown));
            this.formulaBar.RemoveHandler(TextBox.KeyUpEvent, new KeyEventHandler(formulaBar_PreviewKeyUp));
#endif
        }

        #endregion
    }

}
