#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Converter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.XlsIO.Implementation;
    using Syncfusion.XlsIO;
    using System.IO;
#if SILVERLIGHT
    using System.Windows.Controls;
    using System.IO;
    using Syncfusion.XlsIO;
#endif

    /// <summary>
    /// Extensions class for GridDataControl
    /// </summary>   
    public static class GridTreeControlExportExtensions
    {

#if !SILVERLIGHT

        public static void ExportToExcel(this GridTreeControl gridTreeControl, GridRangeInfo gridRange, string fileName, ExcelVersion excelVersion)
        {
            ExportToExcel(gridTreeControl, gridRange, fileName, excelVersion, null);
        }

        public static void ExportToExcel(this GridTreeControl gridTreeControl, GridRangeInfo gridRange, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler)
        {
            //if (!gridRange.IsRows)
            //{
            //    return;
            //}

            gridRange = gridRange.ExpandRange(gridRange.Top, gridRange.Left, gridRange.Top, gridTreeControl.Model.ColumnCount);
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;
            ExportToExcel(gridTreeControl, gridRange, workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1], exportingHandler);
            workbook.SaveAs(fileName);
            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }
#else
        /// <summary>
        /// Extension method for GridDataControl
        /// </summary>
        /// <param name="gridDataControl">GridDataControl Object</param>
        /// <param name="gridRange">Selected Range in GridDataControl</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridTreeControl gridTreeControl, GridRangeInfo gridRange, ExcelVersion excelVersion)
        {
            gridRange = gridRange.ExpandRange(gridRange.Top, gridRange.Left, gridRange.Top, gridTreeControl.Model.ColumnCount - 1);
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;

            ExportToExcel(gridTreeControl, gridRange, workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1]);

            SaveFileDialog sfdone = new SaveFileDialog()
            {
                DefaultExt = "xls",
                FilterIndex = 1,
                Filter = "(.xls)|*.xls"
            };
            if (sfdone.ShowDialog() == true)
            {
                using (Stream stream = sfdone.OpenFile())
                {
                    workbook.SaveAs(stream);
                }
            }

            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }

#endif

#if !SILVERLIGHT

        public static void ExportToExcel(this GridTreeControl gridTreeControl, string fileName, ExcelVersion excelVersion)
        {
            ExportToExcel(gridTreeControl, fileName, excelVersion, null);
        }

        /// <summary>
        ///
        /// Extension method for GridDataControl used for export the  GridDataControl cell data to excel sheet
        /// </summary>
        /// <param name="gridDataControl">GridDataControl Object</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridTreeControl gridTreeControl, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;
            GridRangeInfoList rangeList = new GridRangeInfoList();

            ExportToExcel(gridTreeControl, GridRangeInfo.Cells(0, 1, gridTreeControl.Model.RowCount - 1, gridTreeControl.Model.ColumnCount - 1), workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1], exportingHandler);


            //workbook.Worksheets.AddCopyBefore(workbook.Worksheets[0]);
            workbook.SaveAs(fileName);
            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }
#else
        /// <summary>
        /// Silverlight - Extension method for GridDataControl  used for export the  GridDataControl cell data to excel sheet
        /// </summary>
        /// <param name="gridDataControl">GridDataControl Object</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridTreeControl gridTreeControl, ExcelVersion excelVersion)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "xls",
                FilterIndex = 1,
                Filter = "(.xls)|*.xls"
            };

            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;

            if (sfd.ShowDialog() == true)
            {
                ExportToExcel(gridTreeControl, GridRangeInfo.Cells(0, 1, gridTreeControl.Model.RowCount - 1, gridTreeControl.Model.ColumnCount - 1), workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1]);
                using (Stream stream = sfd.OpenFile())
                {
                    workbook.SaveAs(stream);
                }
            }

            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }
#endif
        public static void ExportToExcel(this GridTreeControl gridTeeControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange)
        {
            ExportToExcel(gridTeeControl, gridRange, workSheet, excelRange, null);
        }

        /// <summary>
        /// Extension method for GridDataControl used for export the  GridDataControl cell data to excel sheet
        /// </summary>
        /// <param name="gridData">GridTreeControl Object</param>
        /// <param name="gridRange">Range of the grid cell data</param>
        /// <param name="mySheet">WorkSheet Object</param>
        /// <param name="excelRange">Range of the excel sheet</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridTreeControl gridTeeControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler)
        {
            GridModel model = gridTeeControl.Model;
            int excelRow = excelRange.Row;
            int excelCol = excelRange.Column;

            GridExcelConverterControl excelConverter = new GridExcelConverterControl();
            GridRangeInfoList groupList = new GridRangeInfoList();

            bool nestedChildInner = false;

            for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
            {
                if (nestedChildInner)
                {
                    excelRow--;
                    nestedChildInner = false;
                }

                for (int col = gridRange.Left; col <= gridRange.Right; col++)
                {
                    excelConverter.GridCellToExcel(model, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);
                    CoveredCellInfo coveredRange = model.CoveredCells.GetCoveredCell(row, col);

                    if (coveredRange != null)
                    {
                        int rowDiff = coveredRange.Top - gridRange.Top;
                        int columnDiff = coveredRange.Left - gridRange.Left;
                        GridRangeInfo gridCRange = GridRangeInfo.Cells(coveredRange.Top, coveredRange.Left, coveredRange.Bottom, coveredRange.Right);
                        IRange xlRange = workSheet.Range[excelRange.Row + rowDiff, excelRange.Column + columnDiff, excelRange.Row + rowDiff + gridCRange.Height - 1, excelRange.Column + columnDiff + gridCRange.Width - 1];
                        if (gridCRange.Height > 1 || gridCRange.Width > 1)
                        {
                            if (!xlRange.IsMerged)
                            {
                                int x1 = gridRange.Right - gridRange.Left;
                                if (excelRange.Column + x1 < xlRange.LastColumn)
                                {
                                    xlRange = workSheet.Range[xlRange.Row, xlRange.Column, xlRange.LastRow, excelRange.Column + x1];
                                }

                                xlRange.Merge();
                            }
                        }
                    }

                    excelCol++;
                }

                excelRow++;
                excelCol = excelRange.Column;
            }

            excelRow--;
            GridTreeModel tableModel1 = model as GridTreeModel;
            //excelRow = 1;
            excelRow = (excelRange != null) ? excelRange.Row : 1;
            for (int row = gridRange.Top; row <= gridRange.Bottom; row++, excelRow++)
            {
                excelCol = excelRange.Column;
                GridTreeNode node = gridTeeControl.InternalGrid.GetNodeAtRowIndex(row);
                for (int col = gridRange.Left; col <= gridRange.Right; col++)
                {

                    if (model[row, col].CellType == "ExpanderCell" && node.Expanded)
                    {
                        int count = GetInnerChildCount(node);
                        if (count > 0)
                            workSheet.Range[excelRow + 1, node.Level + 1, count + excelRow, node.Level + 1].Group(ExcelGroupBy.ByRows, false);
                    }

                    excelCol++;
                }
            }

            excelCol = excelRange.Column;
        }

        public static int GetInnerChildCount(GridTreeNode node)
        {
            int count = 0;
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.ChildNodes.Count != 0)
                {
                    count += childNode.Expanded ? GetInnerChildCount(childNode) + 1 : 1;
                }
                else
                {
                    count += 1;
                }
            }

            return count;
        }
    }
}
