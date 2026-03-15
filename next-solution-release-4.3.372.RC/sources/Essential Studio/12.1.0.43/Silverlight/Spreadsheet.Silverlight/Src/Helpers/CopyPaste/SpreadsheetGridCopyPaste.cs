#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid.Converter;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows;
using System.Collections.Generic;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class SpreadsheetGridCopyPaste :GridModelCutPaste, IGridCopyPaste,IPasteOptionChanged
    {
        Popup PasteOptionPopup = new Popup();
        bool pasteOptionChanged;
        public IWorksheet Worksheet
        {
            get;
            set;
        }

        private GridControlBase grid;
        public SpreadsheetGridCopyPaste(GridControlBase gridbase,IWorksheet sheet)
            : base(gridbase.Model)
        {
            Worksheet = sheet;
            grid = gridbase;
            grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(grid_CurrentCellMoved);
            grid.CurrentCellStartEditing += new ComponentModel.GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.Model.ClipboardPasted += new GridCutPasteEventHandler(Model_ClipboardPasted);
        }

        void Model_ClipboardPasted(object sender, GridCutPasteEventArgs e)
        {
            if (PasteValue)
            {
                GridRangeInfo pasterange = e.RangeList.ActiveRange;
                for (int row = pasterange.Top; row <= pasterange.Bottom; row++)
                {
                    for (int col = pasterange.Left; col <= pasterange.Right; col++)
                    {
                        GridStyleInfo style = this.grid.Model[row, col];
                        if (style.Text != string.Empty && style.Text[0] == '=')
                        {
                            style.CellValue = style.FormattedText;
                        }
                    }
                }
            }
            PasteValue = false;
        }

        void grid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
        {
            ClosePopup();
        }

        void grid_CurrentCellStartEditing(object sender, ComponentModel.SyncfusionCancelRoutedEventArgs args)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            if (PasteOptionPopup != null && PasteOptionPopup.IsOpen)
                PasteOptionPopup.IsOpen = false;
        }

        public void Copy(GridCellData gridCellData, GridRangeInfoList rangeList)
        {
            try
            {
                CopyRange(rangeList, false, true);
                SourceRange = (GridRangeInfo)this.GetExpandedRange(rangeList).ActiveRange.Clone();
                SourceRangeText = Clipboard.GetText();
                CutValue = false;
                string excelSourceRange = ConvertGridRangeToExcelRange(SourceRange, Model);
                if (!string.IsNullOrEmpty(excelSourceRange))
                {
                    SourceWorkbookRange = Worksheet.Range[excelSourceRange];
                    if (Worksheet.MergedCells != null)
                    {
                        foreach (IRange cells in Worksheet.MergedCells)
                        {
                            if (SourceWorkbookRange.IntersectWith(cells) != null)
                            {
                                hasMergedCells = true;
                                break;
                            }
                        }
                    }
                }
                else
                    SourceWorkbookRange = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool CutValue;
        private static bool hasMergedCells;
        private static GridControlBase sourceGrid;
        public void Cut(GridCellData gridCellData, GridRangeInfoList rangeList)
        {
            try
            {
                SourceRange = (GridRangeInfo)this.GetExpandedRange(rangeList).ActiveRange.Clone();
                string excelSourceRange = ConvertGridRangeToExcelRange(SourceRange, Model);
                if (!string.IsNullOrEmpty(excelSourceRange))
                {
                    SourceWorkbookRange = Worksheet.Range[excelSourceRange];
                    if (Worksheet.IsPasswordProtected)
                    {
                        if (SourceWorkbookRange.CellStyle.Locked)
                        {
                            MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                            SourceRange = null;
#if !SILVERLIGHT
                            Clipboard.SetText(string.Empty);
#endif
                            return;
                        }
                        else
                        {
                            var lockedCells = SourceWorkbookRange.Cells.FirstOrDefault(range => range.CellStyle.Locked);
                            if (lockedCells != null)
                            {
                                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                                SourceRange = null;
#if !SILVERLIGHT
                                Clipboard.SetText(string.Empty);
#endif
                                return;
                            }
                        }
                    }
                    this.Model.CommandStack.SuspendUndo = true;
                    CopyRange(rangeList, false, true);
                    this.Model.CommandStack.SuspendUndo = false;
                    SourceRangeText = Clipboard.GetText();
                    CutValue = true;

                    sourceGrid = this.grid;
                    if (Worksheet.MergedCells != null)
                    {
                        foreach (IRange cells in Worksheet.MergedCells)
                        {
                            if (SourceWorkbookRange.IntersectWith(cells) != null)
                            {
                                hasMergedCells = true;
                                break;
                            }
                        }
                    }
                }
                else
                    SourceWorkbookRange = null;

            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message);
            }
        }

        bool PasteValue;

        /// <summary>
        /// Indicates whether the paste operation is in progress
        /// </summary>
        internal bool IsInPaste;
#if SILVERLIGHT
        public string Paste(GridRangeInfoList rangeList)
        {
            try
            {
                IsInPaste = true;
                if (((this.Model.Options.CopyPasteOption & CopyPaste.PasteText) != CopyPaste.PasteText) && ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) != CopyPaste.PasteCell))
                    return null;
                string ClipboardText = Clipboard.GetText();
                bool NeedToOpenContextMenu = false;

                if (this.grid.Model.CommandStack.Enabled)
                    this.grid.Model.CommandStack.BeginTrans("Copy Paste");

                GridRangeInfo TargetRange = (GridRangeInfo)this.GetExpandedRange(rangeList).ActiveRange.Clone();
                GridStyleInfo[] cellsInfo = this.Model.GetCellsInfo(TargetRange);
                this.Model.ChangeCells(TargetRange, cellsInfo, Styles.StyleModifyType.Copy);
                if (Worksheet != null && Model != null && SourceRange != null && !SourceRange.IsEmpty && ClipboardText == SourceRangeText && !PasteValue)
                {
                    if (TargetRange.Height != SourceRange.Height || TargetRange.Width != SourceRange.Width)
                    {
                        bool showmsgbox = false;
                        int bottom = TargetRange.Top + (SourceRange.Height - 1);
                        int right = TargetRange.Left + (SourceRange.Width - 1);
                        if (bottom >= Model.RowCount)
                        {
                            bottom = Model.RowCount - 1;
                            showmsgbox = true;
                        }
                        if (right >= Model.ColumnCount)
                        {
                            right = Model.ColumnCount - 1;
                            showmsgbox = true;
                        }
                        if (showmsgbox && MessageBox.Show("There is not enough space, Do you wish to Continue?", "Clipboard CopyPaste", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return null;
                        }
                        TargetRange = GridRangeInfo.Cells(TargetRange.Top, TargetRange.Left, bottom, right);
                    }
                    var list = rangeList.Clone() as GridRangeInfoList;
                    Model.Selections.Clear();
                    Model.Selections.Add(TargetRange);
                    foreach (GridRangeInfo range in rangeList)
                    {
                        this.Model.InvalidateCell(range);
                    }

                    this.Model.CoveredRanges.InvalidateRanges();
                    GridRangeInfo coveredRange = this.Model.CoveredRanges.Ranges.GetOuterRange(TargetRange);
                    if (coveredRange.Bottom != TargetRange.Bottom || coveredRange.Right != TargetRange.Right)
                    {
                        MessageBox.Show("Cannot change a part of merged cell");
                        return null;
                    }

                    string excelTargetRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange TargetWorkbookRange = null;
                    if (SourceWorkbookRange != null && !string.IsNullOrEmpty(excelTargetRange))
                    {
                        TargetWorkbookRange = Worksheet.Range[excelTargetRange];
                        if (TargetWorkbookRange != null)
                        {
                            if (Worksheet.IsPasswordProtected && TargetWorkbookRange.CellStyle.Locked)
                                return null;
                            if (!hasMergedCells && Worksheet.MergedCells != null)
                            {
                                foreach (IRange cells in Worksheet.MergedCells)
                                {
                                    if (TargetWorkbookRange.IntersectWith(cells) != null)
                                    {
                                        hasMergedCells = true;
                                        break;
                                    }
                                }
                            }
                            SourceWorkbookRange.CopyTo(TargetWorkbookRange, ExcelCopyRangeOptions.All | ExcelCopyRangeOptions.UpdateFormulas);
                        }
                    }

                    for (int row = TargetRange.Top; row <= TargetRange.Bottom; row++)
                    {
                        for (int col = TargetRange.Left; col <= TargetRange.Right; col++)
                        {
                            SpreadsheetGridStyleInfo Style = Model[row, col] as SpreadsheetGridStyleInfo;
                            if (!Style.ReadOnly)
                            {
                                IRange rangeToConvert = Worksheet.Range[row, col];
                                Style.Description = "NeedToImport";
                                ImportRange(Model, Worksheet, Style.CellRowColumnIndex, rangeToConvert, Style);
                                if (Style.ReadOnly)
                                    Style.ReadOnly = false;
                                NeedToOpenContextMenu = true;
                            }
                            if (Style.Text != string.Empty && Style.Text[0] == '=')
                                Style.FormulaTag = null;
                        }
                    }

                    // Commit copy paste transaction.
                    if (this.grid.Model.CommandStack.InTransaction)
                        this.grid.Model.CommandStack.CommitTrans();

                    if (hasMergedCells)
                    {
                        if (CutValue)
                        {
                            SourceWorkbookRange.UnMerge();
                            SourceWorkbookRange.HorizontalAlignment = ExcelHAlign.HAlignGeneral;
                            SourceWorkbookRange.VerticalAlignment = ExcelVAlign.VAlignBottom;
                            if (sourceGrid != this.grid && sourceGrid.Model.CoveredCells != null)
                            {
                                var coveredCells = sourceGrid.Model.CoveredCells.SearchCellSpan(SourceRange.ToCellSpan(sourceGrid.Model));
                                foreach (var cell in coveredCells)
                                {
                                    sourceGrid.Model.CoveredCells.Remove(cell);
                                }
                                sourceGrid.InvalidateCells();
                                sourceGrid.InvalidateVisual(true);
                            }
                        }
                        Model.CoveredCells.Clear();
                        ExcelGridModelImportExtensions.CopyMergesToGrid(Worksheet, (this.Model as SpreadsheetGridModel));
                        this.Model.InvalidateVisual(true);
                    }

                    if (CutValue)
                    {
                        // While Source range intersects with target range, the below code only
                        // clear the cells which are not in the target range
                        if (SourceWorkbookRange.IntersectWith(TargetWorkbookRange) != null)
                        {
                            GridRangeInfo SourceGridRange = SourceWorkbookRange.ConvertExcelRangeToGridRange();
                            GridRangeInfo TargetGridRange = TargetWorkbookRange.ConvertExcelRangeToGridRange();
                            for (int row = SourceGridRange.Top; row <= SourceGridRange.Bottom; row++)
                            {
                                for (int col = SourceGridRange.Left; col <= SourceGridRange.Right; col++)
                                {
                                    GridRangeInfo range = GridRangeInfo.Cell(row, col);
                                    if (!TargetGridRange.Contains(range))
                                    {
                                        IRange excelRange = Worksheet.Range[ConvertGridRangeToExcelRange(range, this.Model)];
                                        excelRange.Clear(ExcelClearOptions.ClearAll);
                                    }
                                }
                            }
                        }
                        else
                            SourceWorkbookRange.Clear(ExcelClearOptions.ClearAll);

                        sourceGrid.Model.InvalidateCell(SourceRange);
                        Clipboard.SetText("");
                        SourceRange = null;
                        SourceRangeText = string.Empty;
                    }
                    Model.InvalidateCell(TargetRange);
                    Model.InvalidateVisual();
                    if (NeedToOpenContextMenu && !CutValue)
                    {
                        Rect rangerect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, Model.SelectedCells, true, true);
                        PasteOptionPopup.Child = new PasteDropDownItem(this);
                        PasteOptionPopup.HorizontalOffset = rangerect.Right + this.grid.PointFromRootVisual().X + 1;
                        PasteOptionPopup.VerticalOffset = rangerect.Bottom + this.grid.PointFromRootVisual().Y + 1;
                        PasteOptionPopup.IsOpen = true;
                    }

                    //the below code was added to break the paste function if sourcerange intersects with targetrange.
                    if (SourceWorkbookRange.IntersectWith(TargetWorkbookRange) != null)
                    {
                        Clipboard.SetText("");
                        SourceRange = null;
                        SourceRangeText = string.Empty;
                    }

                    return null;
                }
                if (Worksheet.IsPasswordProtected)
                {
                    string excelTargetRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange TargetWorkbookRange = Worksheet.Range[excelTargetRange];
                    if (TargetWorkbookRange != null && TargetWorkbookRange.CellStyle.Locked)
                        return null;
                }
                if (!string.IsNullOrEmpty(ClipboardText))
                {
                    // Paste from clipboard.
                    string excelTargetedRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange workbookRange = Worksheet.Range[excelTargetedRange];
                    PasteTextFromBuffer(ClipboardText, workbookRange);

                    NeedToOpenContextMenu = true;

                    // Commit copy paste transaction.
                    if (this.grid.Model.CommandStack.InTransaction)
                        this.grid.Model.CommandStack.CommitTrans();
                }
                if (NeedToOpenContextMenu && !pasteOptionChanged)
                {
                    Rect rangerect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, Model.SelectedCells, true, true);
                    PasteOptionPopup.Child = new PasteDropDownItem(this);
                    PasteOptionPopup.HorizontalOffset = rangerect.Right + this.grid.PointFromRootVisual().X + 5;
                    PasteOptionPopup.VerticalOffset = rangerect.Bottom + this.grid.PointFromRootVisual().Y + 5;
                    PasteOptionPopup.IsOpen = true;
                }
                pasteOptionChanged = false;

                if (this.grid.Model.CommandStack.InTransaction)
                    this.grid.Model.CommandStack.CommitTrans();

                return ClipboardText;
            }
            catch (Exception ex)
            {
                if (this.grid.Model.CommandStack.InTransaction)
                    this.grid.Model.CommandStack.Rollback();
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                IsInPaste = false;
            }
        }
#else
        public DataObject Paste(GridRangeInfoList rangeList)
        {
            try
            {
                IsInPaste = true;
                if (((this.Model.Options.CopyPasteOption & CopyPaste.PasteText) != CopyPaste.PasteText) && ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) != CopyPaste.PasteCell))
                    return null;
                string ClipboardText = Clipboard.GetText();
                if (SourceRangeText != ClipboardText)
                {
                    int lastIndex = ClipboardText.LastIndexOf("\r\n");
                    if (lastIndex > 0 && lastIndex == ClipboardText.Length - 2)
                        ClipboardText = ClipboardText.Remove(ClipboardText.Length - 2);
                }
                bool NeedToOpenContextMenu = false;

                if (this.grid.Model.CommandStack.Enabled)
                    this.grid.Model.CommandStack.BeginTrans("Copy Paste");
                GridRangeInfo TargetRange = (GridRangeInfo)this.GetExpandedRange(rangeList).ActiveRange.Clone();

                if (Worksheet != null && Model != null && SourceRange != null && !SourceRange.IsEmpty && ClipboardText == SourceRangeText && !PasteValue)
                {
                    if (TargetRange.Height != SourceRange.Height || TargetRange.Width != SourceRange.Width)
                    {
                        bool showmsgbox = false;
                        int bottom = TargetRange.Top + (SourceRange.Height - 1);
                        int right = TargetRange.Left + (SourceRange.Width - 1);
                        if (bottom >= Model.RowCount)
                        {
                            bottom = Model.RowCount - 1;
                            showmsgbox = true;
                        }
                        if (right >= Model.ColumnCount)
                        {
                            right = Model.ColumnCount - 1;
                            showmsgbox = true;
                        }
                        if (showmsgbox && MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_NotEnoughSpaceToPaste, SpreadsheetResourceWrapper.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return null;
                        }
                        TargetRange = GridRangeInfo.Cells(TargetRange.Top, TargetRange.Left, bottom, right);                                                
                    }
                    var list = rangeList.Clone() as GridRangeInfoList;
                    Model.Selections.Clear();
                    Model.Selections.Add(TargetRange);
                    foreach (GridRangeInfo range in rangeList)
                    {
                        this.Model.InvalidateCell(range);
                    }

                    this.Model.CoveredRanges.InvalidateRanges();
                    GridRangeInfo coveredRange = this.Model.CoveredRanges.Ranges.GetOuterRange(TargetRange);
                    if (coveredRange.Bottom != TargetRange.Bottom || coveredRange.Right != TargetRange.Right)
                    {
                        MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_PasteOnMergedCells);
                        return null;
                    }
                    
                    string excelTargetRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange TargetWorkbookRange=null;
                    if (SourceWorkbookRange != null && !string.IsNullOrEmpty(excelTargetRange))
                    {
                        TargetWorkbookRange = Worksheet.Range[excelTargetRange];
                        if (TargetWorkbookRange != null)
                        {
                            if (Worksheet.IsPasswordProtected)
                            {
                                if (TargetWorkbookRange.CellStyle.Locked)
                                {
                                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                                    return null;
                                }
                                else
                                {
                                    var lockedCells =
                                        TargetWorkbookRange.Cells.FirstOrDefault(range => range.CellStyle.Locked);
                                    if (lockedCells != null)
                                    {
                                        MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                                        return null;
                                    }
                                }
                            }
                            if (!hasMergedCells && Worksheet.MergedCells != null)
                            {
                                foreach (IRange cells in Worksheet.MergedCells)
                                {
                                    if (TargetWorkbookRange.IntersectWith(cells) != null)
                                    {
                                        hasMergedCells = true;
                                        break;
                                    }
                                }
                            }
                            //Here we push the GridStyleinfo of target range to the UndoStack.
                            GridStyleInfo[] cellsInfo = this.Model.GetCellsInfo(TargetRange);
                            this.Model.ChangeCells(TargetRange, cellsInfo, Styles.StyleModifyType.Copy);
                            SourceWorkbookRange.CopyTo(TargetWorkbookRange, ExcelCopyRangeOptions.All | ExcelCopyRangeOptions.UpdateFormulas);
                        }
                    }

                    //Here we suspending the undo. because we are pushing the GridStyleInfo to the Undo Stack above.
                    this.Model.CommandStack.SuspendUndo = true;

                    for (int row = TargetRange.Top; row <= TargetRange.Bottom; row++)
                    {
                        for (int col = TargetRange.Left; col <= TargetRange.Right; col++)
                        {
                            SpreadsheetGridStyleInfo Style = Model[row, col] as SpreadsheetGridStyleInfo;
                            if (!Style.ReadOnly)
                            {
                                IRange rangeToConvert = Worksheet.Range[row, col];
                                Style.Description = "NeedToImport";
                                ImportRange(Model, Worksheet, Style.CellRowColumnIndex, rangeToConvert, Style);
                                if (Style.ReadOnly)
                                    Style.ReadOnly = false;
                                NeedToOpenContextMenu = true;
                            }
                            if (Style.Text != string.Empty && Style.Text[0] == '=')
                                Style.FormulaTag = null;
                        }
                    }

                    this.Model.CommandStack.SuspendUndo = false;
                    
                    if (hasMergedCells)
                    {
                        if(this.Model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            if (CutValue)
                            {
                                this.Model.CommandStack.Push(new SpreadsheetMergeCommand(this.Model as SpreadsheetGridModel, SourceRange, this.Model.GetCellsInfo(SourceRange), Styles.StyleModifyType.Copy, true));
                                this.Model.CommandStack.Push(new SpreadsheetMergeCommand(this.Model as SpreadsheetGridModel, TargetRange, new GridStyleInfo[1], Styles.StyleModifyType.Copy, false));
                            }
                            else
                                this.Model.CommandStack.Push(new SpreadsheetMergeCommand(this.Model as SpreadsheetGridModel, TargetRange, new GridStyleInfo[1], Styles.StyleModifyType.Copy, false));
                        }
   
                        if (CutValue)
                        {
                            SourceWorkbookRange.UnMerge();
                            SourceWorkbookRange.HorizontalAlignment = ExcelHAlign.HAlignGeneral;
                            SourceWorkbookRange.VerticalAlignment = ExcelVAlign.VAlignBottom;
                            if (sourceGrid != this.grid && sourceGrid.Model.CoveredCells != null)
                            {
                                var coveredCells = sourceGrid.Model.CoveredCells.SearchCellSpan(SourceRange.ToCellSpan(sourceGrid.Model));
                                foreach (var cell in coveredCells)
                                {
                                    sourceGrid.Model.CoveredCells.Remove(cell);
                                }
                                sourceGrid.InvalidateCells();
                                sourceGrid.InvalidateVisual(true);
                            }
                        }
                        Model.CoveredCells.Clear();
                        //Here we are suspending undo because the merged cells pasted the all values after that it is beeing merged. 
                        this.Model.CommandStack.SuspendUndo = true;
                        ExcelGridModelImportExtensions.CopyMergesToGrid(Worksheet, (this.Model as SpreadsheetGridModel));
                        this.Model.CommandStack.SuspendUndo = false;
                        
                        this.Model.InvalidateVisual(true);
                    }

                    if (CutValue)
                    {
                        // While Source range intersects with target range, the below code only
                        // clear the cells which are not in the target range
                        if (SourceWorkbookRange.IntersectWith(TargetWorkbookRange) != null)
                        {
                            GridRangeInfo SourceGridRange = SourceWorkbookRange.ConvertExcelRangeToGridRange();
                            GridRangeInfo TargetGridRange = TargetWorkbookRange.ConvertExcelRangeToGridRange();
                            for (int row = SourceGridRange.Top; row <= SourceGridRange.Bottom; row++)
                            {
                                for (int col = SourceGridRange.Left; col <= SourceGridRange.Right; col++)
                                {
                                    GridRangeInfo range = GridRangeInfo.Cell(row, col);
                                    if (!TargetGridRange.Contains(range))
                                    {
                                        if (this.Model.CommandStack.Enabled)
                                        {
                                            GridStyleInfo[] cellsInfo = this.Model.GetCellsInfo(range);
                                            this.Model.ChangeCells(range, cellsInfo, Styles.StyleModifyType.Copy);
                                        }
                                        IRange excelRange = Worksheet.Range[ConvertGridRangeToExcelRange(range, this.Model)];
                                        excelRange.Clear(ExcelClearOptions.ClearAll);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Model.CommandStack.Enabled)
                            {
                                GridStyleInfo[] cellsInfo = this.Model.GetCellsInfo(SourceRange);
                                this.Model.ChangeCells(SourceRange, cellsInfo, Styles.StyleModifyType.Copy);
                            }
                            SourceWorkbookRange.Clear(ExcelClearOptions.ClearAll);
                        }
                        sourceGrid.Model.InvalidateCell(SourceRange);
                        Clipboard.Clear();
                        SourceRange = null;
                        SourceRangeText = string.Empty;
                    }
                    Model.InvalidateCell(TargetRange);
                    Model.InvalidateVisual();
                    if (NeedToOpenContextMenu && !CutValue)
                    {
                        Rect rangerect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, Model.SelectedCells, true, true);
                        PasteOptionPopup.Child = new PasteDropDownItem(this);
                        PasteOptionPopup.PlacementTarget = this.grid;
                        PasteOptionPopup.Placement = PlacementMode.Bottom;
                        PasteOptionPopup.VerticalOffset = 2;
                        PasteOptionPopup.HorizontalOffset = rangerect.Width + 1;
                        PasteOptionPopup.PlacementRectangle = rangerect;
                        PasteOptionPopup.MaxWidth = 40;
                        PasteOptionPopup.IsOpen = true;
                    }
                    
                    //the below code was added to break the paste function if sourcerange intersects with targetrange.
                    if (SourceWorkbookRange.IntersectWith(TargetWorkbookRange)!=null)
                    {
                        Clipboard.Clear();
                        SourceRange = null;
                        SourceRangeText = string.Empty;
                    }

                    (this.grid as SpreadsheetGrid).ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();

                    // Commit copy paste transaction.
                    if (this.grid.Model.CommandStack.InTransaction)
                        this.grid.Model.CommandStack.CommitTrans();

                    return null;
                }
                if (Worksheet.IsPasswordProtected)
                {
                    string excelTargetRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange TargetWorkbookRange = Worksheet.Range[excelTargetRange];
                    if (TargetWorkbookRange != null)
                    {
                        if (TargetWorkbookRange.CellStyle.Locked)
                        {
                            MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                            return null;
                        }
                        else
                        {
                            var lockedCells = TargetWorkbookRange.Cells.FirstOrDefault(range => range.CellStyle.Locked);
                            if (lockedCells != null)
                            {
                                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ReadOnlyCells);
                                return null;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ClipboardText))
                {
                    string excelTargetedRange = ConvertGridRangeToExcelRange(TargetRange, Model);
                    IRange workbookRange = Worksheet.Range[excelTargetedRange];
                    PasteTextFromBuffer(ClipboardText, workbookRange);
                    
                    NeedToOpenContextMenu = true;
                    // Commit copy paste transaction.
                    if (this.grid.Model.CommandStack.InTransaction)
                        this.grid.Model.CommandStack.CommitTrans();
                }
                if (NeedToOpenContextMenu && !pasteOptionChanged)
                {
                    Rect rangerect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, Model.SelectedCells, true, true);
                    PasteOptionPopup.Child = new PasteDropDownItem(this);
                    PasteOptionPopup.PlacementTarget = this.grid;
                    PasteOptionPopup.Placement = PlacementMode.Bottom;
                    PasteOptionPopup.VerticalOffset = 2;
                    PasteOptionPopup.HorizontalOffset = rangerect.Width + 1;
                    PasteOptionPopup.PlacementRectangle = rangerect;
                    PasteOptionPopup.MaxWidth = 40;
                    PasteOptionPopup.IsOpen = true;
                }
                pasteOptionChanged = false;

                //GridStyleInfo[] cellsInfo = null;
                //cellsInfo = this.Model.GetCellsInfo(TargetRange);
                //this.Model.ChangeCells(TargetRange, cellsInfo, Styles.StyleModifyType.Copy);

                if (this.grid.Model.CommandStack.InTransaction)
                    this.grid.Model.CommandStack.CommitTrans();

                (this.grid as SpreadsheetGrid).ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();
                return Clipboard.GetDataObject() as DataObject;
            }
            catch (Exception ex)
            {
                if (this.grid.Model.CommandStack.InTransaction)
                    this.grid.Model.CommandStack.Rollback();
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                IsInPaste = false;
            }
        }
#endif
        internal static GridRangeInfo SourceRange;
        static string SourceRangeText;
        static IRange SourceWorkbookRange;

        private GridRangeInfoList GetExpandedRange(GridRangeInfoList rangeList)
        {
            if (rangeList.Count > 0)
            {
                if (rangeList[0].IsCols)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, rangeList[0].Left, Model.RowCount, rangeList[0].Left);
                }

                if (rangeList[0].IsRows)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, rangeList[0].Left, rangeList[0].Top, Model.ColumnCount);
                }

                if (rangeList[0].IsTable)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, rangeList[0].Left, Model.RowCount, Model.ColumnCount);
                }
                return rangeList;
            }

            return null;
        }

        public string ConvertGridRangeToExcelRange(GridRangeInfo gridRangeInfo, GridModel gridModel)
        {
            if (!gridRangeInfo.IsEmpty)
            {
                string firstcell = string.Empty;
                string lastcell = string.Empty;
                string range = string.Empty;
                if (gridRangeInfo.IsCells)
                {

                    firstcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Left == 0 ? gridRangeInfo.Left + 1 : gridRangeInfo.Left) + (gridRangeInfo.Top == 0 ? gridRangeInfo.Top + 1 : gridRangeInfo.Top);
                    lastcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Right) + gridRangeInfo.Bottom;
                }
                else if (gridRangeInfo.IsCols)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Left) + 1;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Right) + gridModel.RowCount;
                }
                else if (gridRangeInfo.IsRows)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(1) + gridRangeInfo.Top;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridModel.ColumnCount) + gridRangeInfo.Bottom;
                }
                else if (gridRangeInfo.IsTable)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(1) + 1;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridModel.ColumnCount) + gridModel.RowCount;
                }
                if (!string.IsNullOrEmpty(firstcell) && !string.IsNullOrEmpty(lastcell))
                    range = firstcell + ":" + lastcell;
                return range;
            }
            return string.Empty;
        }

        private void ImportRange(GridModel gridModel, IWorksheet sheet, RowColumnIndex Cell, IRange rangeToConvert, SpreadsheetGridStyleInfo Style)
        {
            if (Style.Description == "NeedToImport")
            {
                if (Style.Description != "IsImported" && Style.CellType != "Hyperlink" && Style.CellType != "ImageCell")
                {
                    if (Cell.RowIndex <= sheet.Range.LastRow && Cell.ColumnIndex <= sheet.Range.LastColumn)
                    {
                        if (rangeToConvert.Hyperlinks.Count > 0)
                            ExcelGridModelImportExtensions.CopyHyperlinkToCell(sheet.Workbook, sheet, rangeToConvert, Style);
                        else
                            ExcelGridModelImportExtensions.ConvertExcelRangeToVirtualGrid(Style, sheet, rangeToConvert, true, null);
                        gridModel.Data[Cell.RowIndex, Cell.ColumnIndex] = Style.Store;
                        Style.Tag = true;
                        Style.Description = "IsImported";
                    }
                }
            }
        }

        private void PasteTextFromBuffer(string buffer, IRange range)
        {
            var baseString = buffer;
            bool canceled = true;
            string s = string.Empty;
            int rowIndex, colIndex, lastcol;
            string tabDelim = "\t";
            rowIndex = range.Row;
            colIndex = range.Column;
            GridStyleInfo[] cellsInfo = null;
            int size = buffer.Length;
            lastcol = colIndex;
            var colCollect = buffer.Split(new string[] { tabDelim }, StringSplitOptions.None);
            foreach (var item in colCollect)
            {
                if (item.EndsWith("\r\n\""))
                    s += item.Remove(item.Length - 3) + "\"\t";
                else
                    s += item + "\t";
            }
            s = s.Remove(s.Length - 1);
            buffer = s;
            var rowCollect = buffer.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (var item in rowCollect)
            {
                buffer = item;
                size = buffer.Length;
                for (int index = 0, last = 0; index <= size; index++)
                {
                    //// Check for a delimiter.
                    if (size == 0)
                    {
                        this.PasteTextRowCol(rowIndex, colIndex, string.Empty, range);
                    }
                    else
                    {
                        bool isDelimiter = (("\r\n".IndexOf(buffer[index]) != -1 || tabDelim[0] == buffer[index]) && (buffer[index] != '\n'));
                        if (index == size - 1 || isDelimiter)
                        {
                            //// End of a string found, copy value to cell.
                            if ("\r\n".IndexOf(buffer[index]) != -1)
                            {
                                if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                {
                                    index++;
                                    colIndex = range.Column;
                                    last = index + 1;
                                    continue;
                                }
                            }

                            if (rowIndex <= this.Model.RowCount && colIndex <= this.Model.ColumnCount)
                            {
                                s = isDelimiter
                                        ? (index != last ? buffer.Substring(last, index - last) : String.Empty)
                                        : buffer.Substring(last);

                                //Remove double quotes start and end when new line is used in cell
                                if (s.StartsWith("\"") && s.EndsWith("\"") && s.Length > 1)
                                    s = s.Substring(1, s.Length - 2);
                                else if (s.StartsWith("\""))
                                {
                                    if (s.Length > 1)
                                        s = s.Substring(1, s.Length - 1);
                                    else
                                    {
                                        rowIndex--;
                                        break;
                                    }
                                }
                                s = s.Replace("\"\"", "\"");

                                cellsInfo = this.Model.GetCellsInfo(GridRangeInfo.Cell(rowIndex, colIndex));
                                this.Model.ChangeCells(GridRangeInfo.Cell(rowIndex, colIndex), cellsInfo, Styles.StyleModifyType.Copy);

                                //// Give the control the chance to validate
                                //// and change the pasted text.
                                canceled = !PasteTextRowCol(rowIndex, colIndex, s, range);
                            }

                            if (canceled || index == size - 1)
                            {
                                break;
                            }
                            else if ("\r\n".IndexOf(buffer[index]) != -1)
                            {
                                rowIndex++;
                                colIndex = range.Column;
                                if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                {
                                    index++;
                                }

                                //// Abort parsing the string if next char
                                //// is an end-of-string.
                                if (index == size - 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                colIndex++;
                            }

                            last = index + 1;
                            lastcol = Math.Max(colIndex, lastcol);
                        }
                    }
                }
                rowIndex++;
                colIndex = range.Column;
            }
            Model.Selections.Clear();
            Model.Selections.Add(GridRangeInfo.Cells(range.Row, range.Column, rowIndex - 1, lastcol));
            Model.InvalidateCell(GridRangeInfo.Cells(range.Row,range.Column,rowIndex - 1, lastcol));
        }

        private bool PasteTextRowCol(int row, int col, string text, IRange range)
        {
            range[row, col].Value = text;
            return true;
        }

        public void PasteOptionChanged(string Option)
        {
            pasteOptionChanged = true;
            if (Option == "Values")
            {
                PasteValue = true;
                Paste();
                //PasteOptionPopup.IsOpen = false;
            }
            else if (Option == "Formulas")
            {
                PasteValue = false;
                Paste();
                //PasteOptionPopup.IsOpen = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (grid != null)
                {
                    grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(grid_CurrentCellMoved);
                    grid.Model.ClipboardPasted -= new GridCutPasteEventHandler(Model_ClipboardPasted);
                    grid.CurrentCellStartEditing -= new GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
                }
                grid = null;
                Worksheet = null;
                PasteOptionPopup = null;
            }
        }
        
    }
}