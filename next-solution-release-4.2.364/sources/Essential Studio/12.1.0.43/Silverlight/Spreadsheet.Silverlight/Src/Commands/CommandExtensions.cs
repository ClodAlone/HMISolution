#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Spreadsheet;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Spreadsheet.CommandExtensions
{
    public static class CommandExtensions
    {

        public static void Copy(this SpreadsheetControl excelEditorControl,bool clear,CopyPaste CopyPaste)
        {
            excelEditorControl.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption = CopyPaste;

            if (clear)
            {
                if (excelEditorControl.GridProperties.CurrentCell != null && excelEditorControl.GridProperties.CurrentCell.IsEditing)
                    excelEditorControl.GridProperties.CurrentCell.EndEdit();
                (excelEditorControl.GridProperties.CurrentExcelGridModel.CutPaste as GridModelCutPaste).Cut();
            }
            else
                (excelEditorControl.GridProperties.CurrentExcelGridModel.CutPaste as GridModelCutPaste).Copy();
        }

#if !SILVERLIGHT
        public static void ExecuteCopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SpreadsheetControl AssociatedSpreadsheet = e.Source as SpreadsheetControl;
            if (AssociatedSpreadsheet != null)
            {
                CopyPaste copyPasteOption;
                if (e.Parameter != null)
                    copyPasteOption = (CopyPaste)e.Parameter;
                else
                    copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;
                CommandExtensions.Copy(e.Source as SpreadsheetControl, false, copyPasteOption);
            }
        }

        public static void CanExecuteCopyCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public static void ExecuteCutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SpreadsheetControl AssociatedSpreadsheet = e.Source as SpreadsheetControl;
            if (AssociatedSpreadsheet != null)
            {
                CopyPaste copyPasteOption;
                if (e.Parameter != null)
                    copyPasteOption = (CopyPaste)e.Parameter;
                else
                    copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;
                CommandExtensions.Copy(e.Source as SpreadsheetControl, true, copyPasteOption);
            }
        }

        public static void CanExecuteCutCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public static void ExecutePasteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SpreadsheetControl AssociatedSpreadsheet = e.Source as SpreadsheetControl;
            if (AssociatedSpreadsheet != null)
            {
                CopyPaste copyPasteOption;
                if (e.Parameter != null)
                    copyPasteOption = (CopyPaste)e.Parameter;
                else
                    copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;
                CommandExtensions.Copy(e.Source as SpreadsheetControl, false, copyPasteOption);
            }
        }

        public static void CanExecutePasteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
#endif

        /// <summary>
        /// Sets the horizontal alignment.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        public static void SetHorizontalAlignment(this SpreadsheetControl excelEditorControl,HorizontalAlignment horizontalAlignment)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        if (horizontalAlignment == HorizontalAlignment.Center)
                            excelStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        else if (horizontalAlignment == HorizontalAlignment.Right)
                            excelStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                        else
                            excelStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;

                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
                    if (horizontalAlignment == HorizontalAlignment.Center)
                        excelStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    else if (horizontalAlignment == HorizontalAlignment.Right)
                        excelStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                    else
                    {
                        excelStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        excelEditorControl.GridProperties.CurrentCellStyle.CellValue2 = ExcelHAlign.HAlignLeft;
                    }
                    GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);

                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedRange, cellsInfo, StyleModifyType.Copy);
                }
                excelEditorControl.GridProperties.CurrentCellStyle.HorizontalAlignment = horizontalAlignment;
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }




       

        /// <summary>
        /// Sets WrapText method called when calling WrapTextCommand to set WrapText on selected cells.
        /// </summary>
        /// <param name="excelEditorControl"></param>
        public static void SetWrapText(this SpreadsheetControl excelEditorControl)
        {
             IWorksheet worksheet =
                 excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
             GridStyleInfo[] cellsInfo = null;
             excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
             if (excelEditorControl.GridProperties.HasSelectedRange)
             {
                 foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                 {
                     string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                     var excelStyle = worksheet[cell].CellStyle;

                     var r = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                     var c = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                     string currentCell = GridRangeInfo.GetAlphaLabel(c) + r;
                     var excelCurrentCellStyle = worksheet[currentCell].CellStyle;
                     if (excelCurrentCellStyle != null)
                     {
                         if (excelCurrentCellStyle.WrapText)
                         {
                             excelStyle.WrapText = false;
                             for (int row = item.Top; row <= item.Bottom; row++)
                             {
                                 for (int col = item.Left; col <= item.Right; col++)
                                 {
                                     var style = excelEditorControl.GridProperties.CurrentExcelGridModel[row, col];
                                     style.TextWrapping = TextWrapping.NoWrap;
                                     style.TextTrimming = TextTrimming.WordEllipsis;
#if !SILVERLIGHT
                                     style.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
#else
                                     style.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
#endif
                                 }
                             }
                             excelEditorControl.GridProperties.CurrentExcelGridModel.SetWrapText(excelStyle.WrapText);
                         }
                         else
                         {
                             excelStyle.WrapText = true;
                             excelEditorControl.GridProperties.CurrentExcelGridModel.SetWrapText(excelStyle.WrapText);
                         }
                     }
                     excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                     if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                     {
                         cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                         SpreadsheetWraptextCommand wrapCommand =new SpreadsheetWraptextCommand(excelEditorControl.GridProperties.CurrentExcelGridModel,item, cellsInfo, StyleModifyType.Copy, !excelStyle.WrapText);
                         excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(wrapCommand);
                     }
                 }
             }
             else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
             {
                 var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                 var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                 string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                 var excelStyle = worksheet[cell].CellStyle;
                 GridRangeInfo changedInfo=GridRangeInfo.Empty;
                 if (excelStyle != null)
                 {
                     if (excelStyle.WrapText)
                     {
                         excelStyle.WrapText = false;
                         var style = excelEditorControl.GridProperties.CurrentExcelGridModel[row, col];
                         excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("UnWrap");
                         style.TextWrapping = TextWrapping.NoWrap;
                         style.TextTrimming = TextTrimming.WordEllipsis;
#if !SILVERLIGHT
                         style.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
#else
                         style.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
#endif
                         excelEditorControl.GridProperties.CurrentExcelGridModel.SetWrapText(excelStyle.WrapText);
                         excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                     }
                     else
                     {
                         excelStyle.WrapText = true;
                         changedInfo = GridRangeInfo.Auto(row, col);                         
                         excelEditorControl.GridProperties.CurrentExcelGridModel.SetWrapText(excelStyle.WrapText);
                     }
                     if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                     {
                         cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Auto(row, col));
                         SpreadsheetWraptextCommand wrapCommand = new SpreadsheetWraptextCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, GridRangeInfo.Auto(row, col), cellsInfo, StyleModifyType.Copy, !excelStyle.WrapText);
                         excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(wrapCommand);
                     }
                 }
             }
        }
        
        /// <summary>
        /// Sets the vertical alignment.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        public static void SetVerticalAlignment(this SpreadsheetControl excelEditorControl, VerticalAlignment verticalAlignment)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        if (verticalAlignment == VerticalAlignment.Center)
                            excelStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                        else if (verticalAlignment == VerticalAlignment.Bottom)
                            excelStyle.VerticalAlignment = ExcelVAlign.VAlignBottom;
                        else
                            excelStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
                    if (verticalAlignment == VerticalAlignment.Center)
                        excelStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    else if (verticalAlignment == VerticalAlignment.Bottom)
                        excelStyle.VerticalAlignment = ExcelVAlign.VAlignBottom;
                    else
                        excelStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

                    GridRangeInfo changedInfo = GridRangeInfo.Auto(row, col);
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedInfo);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedInfo, cellsInfo, StyleModifyType.Copy);

                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Changes the color of the font.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="fontColor">Color of the font.</param>
        public static void ChangeFontColor(this SpreadsheetControl excelEditorControl, Color fontColor)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
#if SILVERLIGHT
                    excelStyle.Font.RGBColor = fontColor;
                    if (fontColor == Colors.Black)
                    {
                        excelStyle.Font.Color = ExcelKnownColors.Black;
                    }
#else
                    excelStyle.Font.RGBColor  = ConvertColor(fontColor);
#endif
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
#if SILVERLIGHT
                    excelStyle.Font.RGBColor = fontColor;
                    if (fontColor == Colors.Black)
                    {
                        excelStyle.Font.Color = ExcelKnownColors.Black;
                    }
#else
                    excelStyle.Font.RGBColor = ConvertColor(fontColor);
#endif
                }
                GridRangeInfo changedInfo = GridRangeInfo.Auto(row, col);
                cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedInfo);
                excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedInfo, cellsInfo, StyleModifyType.Copy);
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Merges the cells.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="canMerge">if set to <c>true</c> [can merge].</param>
        public static void MergeCells(this SpreadsheetControl excelEditorControl, bool canMerge)
        {
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
                GridStyleInfo[] cellsInfo = null;
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string range = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        SpreadsheetMergeCommand mergeCommand = new SpreadsheetMergeCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, item, cellsInfo, StyleModifyType.Copy, !canMerge);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(mergeCommand);
                    }
                    if (canMerge)
                    {
                        worksheet.Range[range].Merge();
                        worksheet.Range[range].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[range].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    }
                    else
                    {
                        worksheet.Range[range].UnMerge();
                        worksheet.Range[range].HorizontalAlignment = ExcelHAlign.HAlignGeneral;
                        worksheet.Range[range].VerticalAlignment = ExcelVAlign.VAlignBottom;
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.CoveredCells.Clear();
                excelEditorControl.GridProperties.ActiveSpreadsheetGrid.Model.CommandStack.SuspendUndo = true;
                ExcelGridModelImportExtensions.CopyMergesToGrid(worksheet, excelEditorControl.GridProperties.CurrentExcelGridModel);
                excelEditorControl.GridProperties.ActiveSpreadsheetGrid.Model.CommandStack.SuspendUndo = false;
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// Fills the color.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="fillColor">Color of the fill.</param>
        public static void ChangeFillColor(this SpreadsheetControl excelEditorControl, Color fillColor)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
#if SILVERLIGHT
                        excelStyle.Color = fillColor;
#else
                        excelStyle.Color = ConvertColor(fillColor);
#endif
                        excelStyle.Interior.FillPattern = ExcelPattern.Solid;

                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
#if SILVERLIGHT
                    excelStyle.Color = fillColor;
#else
                    excelStyle.Color= ConvertColor(fillColor);
#endif
                    excelStyle.Interior.FillPattern = ExcelPattern.Solid;

                    GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedRange, cellsInfo, StyleModifyType.Copy);
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

#if !SILVERLIGHT
        public static System.Drawing.Color ConvertColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
#endif
        /// <summary>
        /// Changes the size of the font.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="size">The size.</param>
        public static void ChangeFontSize(this SpreadsheetControl excelEditorControl, double size)
        {
            if ((excelEditorControl.GridProperties.CurrentCell != null && excelEditorControl.GridProperties.CurrentCell.IsInMoveTo) ||
                ((excelEditorControl.GridProperties.CurrentExcelGridModel.GridCopyPaste as SpreadsheetGridCopyPaste).IsInPaste))
                return;
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];

            GridStyleInfo[] cellsInfo;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    GridRangeInfo newitem = item.ExpandRange(0, 0, excelEditorControl.GridProperties.CurrentExcelGridModel.RowCount,
                                                             excelEditorControl.GridProperties.CurrentExcelGridModel.ColumnCount);
                    string cell = newitem.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                        excelStyle.Font.Size = size / 1.2;

                    if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        SpreadsheetFontSizeChangedCommand fontCommand = new SpreadsheetFontSizeChangedCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, item, cellsInfo, StyleModifyType.Copy);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(fontCommand);
                    }

                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ResizeRowsToFit(item, GridResizeToFitOptions.None);
                    
                    //When we reduce the font size then the row height also reduced below the default value.
                    for (int row = item.Top; row <= item.Bottom; row++)
                    {
                        if (excelEditorControl.GridProperties.CurrentExcelGridModel.RowHeights[row] < 24)
                            excelEditorControl.GridProperties.CurrentExcelGridModel.RowHeights[row] = 24;
                    }

                    excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                    excelStyle.Font.Size = size / 1.2;

                if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                {
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cell(row, col));
                    SpreadsheetFontSizeChangedCommand fontCommand = new SpreadsheetFontSizeChangedCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, GridRangeInfo.Cell(row, col), cellsInfo, StyleModifyType.Copy);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(fontCommand);
                }

                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                bool shouldresize = excelEditorControl.GridProperties.CurrentExcelGridModel.checkWrapRow(row, col);
                if (shouldresize && !excelEditorControl.GridProperties.CurrentCell.IsInMoveTo)
                {
                    excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ResizeRowsToFit(GridRangeInfo.Cell(row, col), GridResizeToFitOptions.None);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                    //When we reduce the font size then the row height also reduced below the default value.
                    if (excelEditorControl.GridProperties.CurrentExcelGridModel.RowHeights[row] < 24)
                        excelEditorControl.GridProperties.CurrentExcelGridModel.RowHeights[row] = 24;
                }
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Changes the font family.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        /// <param name="fontFamily">The font family.</param>
        public static void ChangeFontFamily(this SpreadsheetControl excelEditorControl, string fontFamily)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        excelStyle.Font.FontName = fontFamily;
                        if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                        {
                            cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                            excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetGridCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, item, cellsInfo, StyleModifyType.Copy, GridStyleInfoStore.FontProperty));
                        }
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
                    excelStyle.Font.FontName = fontFamily;
                    if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetGridCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, changedRange, cellsInfo, StyleModifyType.Copy, GridStyleInfoStore.FontProperty));
                    }
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Underlines the specified excel editor control.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        public static void Underline(this SpreadsheetControl excelEditorControl)
        {
            IWorksheet worksheet =
                   excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            var row = excelEditorControl.GridProperties.CurrentCell.RowIndex;
            var col = excelEditorControl.GridProperties.CurrentCell.ColumnIndex;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                IRange CurrentExcelRange = worksheet.Range[row, col];
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    GridStyleInfo[] cellsInfo = null;
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        if (CurrentExcelRange.CellStyle.Font.Underline == ExcelUnderline.Double ||
                            CurrentExcelRange.CellStyle.Font.Underline == ExcelUnderline.Single)
                            excelStyle.Font.Underline = ExcelUnderline.None;
                        else
                            excelStyle.Font.Underline = ExcelUnderline.Single;
                        if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                        {
                            cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                            excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetGridCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, item, cellsInfo, StyleModifyType.Copy, GridStyleInfoStore.FontProperty));
                        }
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                GridStyleInfo[] cellsInfo = null;
                if (excelStyle != null)
                {
                    if (excelStyle.Font.Underline == ExcelUnderline.Double ||
                        excelStyle.Font.Underline == ExcelUnderline.Single)
                        excelStyle.Font.Underline = ExcelUnderline.None;
                    else
                        excelStyle.Font.Underline = ExcelUnderline.Single;
                    if (excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetGridCommand(excelEditorControl.GridProperties.CurrentExcelGridModel, changedRange, cellsInfo, StyleModifyType.Copy, GridStyleInfoStore.FontProperty));
                    }
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Italics the specified excel editor control.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        public static void Italic(this SpreadsheetControl excelEditorControl)
        {
            IWorksheet worksheet =
                    excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            var row = excelEditorControl.GridProperties.CurrentCell.RowIndex;
            var col = excelEditorControl.GridProperties.CurrentCell.ColumnIndex;
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                IRange CurrentExcelRange = worksheet.Range[row, col];
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                        excelStyle.Font.Italic = !CurrentExcelRange.CellStyle.Font.Italic;
                    }
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
                    GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedRange, cellsInfo, StyleModifyType.Copy);
                    excelStyle.Font.Italic = !excelStyle.Font.Italic;
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        /// <summary>
        /// Bolds the specified excel editor control.
        /// </summary>
        /// <param name="excelEditorControl">The excel editor control.</param>
        public static void Bold(this SpreadsheetControl excelEditorControl)
        {
            IWorksheet worksheet =
                    excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            var row = excelEditorControl.GridProperties.CurrentCell.RowIndex;
            var col = excelEditorControl.GridProperties.CurrentCell.ColumnIndex;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                IRange CurrentExcelRange = worksheet.Range[row, col];
                GridStyleInfo[] cellsInfo = null;
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cell = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    var excelStyle = worksheet[cell].CellStyle;
                    if (excelStyle != null)
                    {
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, StyleModifyType.Copy);
                        excelStyle.Font.Bold = !CurrentExcelRange.CellStyle.Font.Bold;
                    }
                    
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                var excelStyle = worksheet[cell].CellStyle;
                if (excelStyle != null)
                {
                    GridRangeInfo changedRange = GridRangeInfo.Auto(row, col);
                    GridStyleInfo[] cellInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(changedRange);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(changedRange, cellInfo, StyleModifyType.Copy);
                    excelStyle.Font.Bold = !excelStyle.Font.Bold;
                }
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        public static void ChangeNumberFormat(this SpreadsheetControl excelEditorControl, string numberFormat)
        {
            var worksheet =
                excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string range = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    if (worksheet.Range[range].NumberFormat != numberFormat)
                    {
                        worksheet.Range[range].NumberFormat = numberFormat;
                        cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);
                        excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    }
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string range = GridRangeInfo.GetAlphaLabel(col) + row;
                if (worksheet.Range[range].NumberFormat != numberFormat)
                {
                    worksheet.Range[range].NumberFormat = numberFormat;
                    GridRangeInfo rangeInfo = excelEditorControl.GridProperties.CurrentCell.RangeInfo;
                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo, Styles.StyleModifyType.Copy);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                }
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        public static void ChangeBorder(this SpreadsheetControl excelEditorControl, ExcelBorderStyle excelBorderStyle)
        {
            var worksheet =
                excelEditorControl.ExcelProperties.WorkBook.Worksheets[excelEditorControl.GridProperties.CurrentSheetName];
            GridStyleInfo[] cellsInfo = null;
            if (excelEditorControl.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo item in excelEditorControl.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string range = item.ConvertGridRangeToExcelRange(excelEditorControl.GridProperties.CurrentExcelGridModel);
                    IRange excelRange = worksheet.Range[range];

                    cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);

                    ChangeCellBorder(excelRange, excelBorderStyle);
                    excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                }
            }
            else if (excelEditorControl.GridProperties.CurrentCellStyle != null)
            {
                var row = excelEditorControl.GridProperties.CurrentCellStyle.RowIndex;
                var col = excelEditorControl.GridProperties.CurrentCellStyle.ColumnIndex;
                string range = GridRangeInfo.GetAlphaLabel(col) + row;
                IRange excelRange = worksheet.Range[range];
                ChangeCellBorder(excelRange, excelBorderStyle);
                GridRangeInfo rangeInfo = excelEditorControl.GridProperties.CurrentCell.RangeInfo;
                cellsInfo = excelEditorControl.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                excelEditorControl.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo, Styles.StyleModifyType.Copy);
                excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
            }
            excelEditorControl.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
        }

        public static void ChangeCellBorder(IRange range,ExcelBorderStyle excelBorderStyle)
        {
            try
            {
                if (excelBorderStyle == ExcelBorderStyle.AllBorder)
                {
                    range.BorderAround(ExcelLineStyle.Thin, ExcelKnownColors.Black);
                    if (range.Count > 1)
                        range.BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);
                }
                else if (excelBorderStyle == ExcelBorderStyle.OutSideBorder)
                    range.BorderAround(ExcelLineStyle.Thin, ExcelKnownColors.Black);
                else if (excelBorderStyle == ExcelBorderStyle.ThickBoxBorder)
                    range.BorderAround(ExcelLineStyle.Thick, ExcelKnownColors.Black);

                else if (excelBorderStyle == ExcelBorderStyle.BottomBorder)
                    range.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                else if (excelBorderStyle == ExcelBorderStyle.LeftBorder)
                    range.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                else if (excelBorderStyle == ExcelBorderStyle.RightBorder)
                    range.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                else if (excelBorderStyle == ExcelBorderStyle.TopBorder)
                    range.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;

                else if (excelBorderStyle == ExcelBorderStyle.ThickBottomBorder)
                    range.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                else if (excelBorderStyle == ExcelBorderStyle.ThickTopBorder)
                    range.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                else if (excelBorderStyle == ExcelBorderStyle.ThickLeftBorder)
                    range.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                else if (excelBorderStyle == ExcelBorderStyle.ThickRightBorder)
                    range.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;

                else if (excelBorderStyle == ExcelBorderStyle.TopAndBottomBorder)
                {
                    range.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    range.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                }
                else if (excelBorderStyle == ExcelBorderStyle.TopAndThickBottomBorder)
                {
                    range.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                    range.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                }
                else //if (excelBorderStyle == ExcelBorderStyle.NoBorder)
                    range.BorderNone();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
            }
        }
    }

    public enum ExcelBorderStyle
    {
        BottomBorder,
        TopBorder,
        LeftBorder,
        RightBorder,

        NoBorder,
        AllBorder,
        OutSideBorder,
        ThickBoxBorder,

        ThickTopBorder,
        ThickLeftBorder,
        ThickRightBorder,
        ThickBottomBorder,
        TopAndBottomBorder,
        TopAndThickBottomBorder
    }
}
