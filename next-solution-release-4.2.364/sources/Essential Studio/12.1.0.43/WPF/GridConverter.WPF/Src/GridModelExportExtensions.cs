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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.XlsIO;
    using System.IO;
    using System.Windows.Media;
    using Syncfusion.XlsIO.Implementation;
    using System.Windows;
    using System.Collections;
#if !SILVERLIGHT
    using Syncfusion.XlsIO.Implementation.Collections;
#endif
#if SILVERLIGHT
    using System.Windows.Controls;
    using System.IO;
#endif

    public class ExportingToExcelEventArgs : EventArgs
    {
        public ExportingToExcelEventArgs(IRange range, bool handled)
        {
            Range = range;
            Handled = handled;
        }

        public bool Handled
        {
            get;
            set;
        }

        public IRange Range
        {
            get;
            set;
        }

        public int RowIndex
        {
            get;
            internal set;
        }

        public int ColumnIndex
        {
            get;
            internal set;
        }
    }

    public class ExportingGridModelToExcelEventArgs : EventArgs
    {
        public ExportingGridModelToExcelEventArgs(bool handled)
        {
            Handled = handled;
        }


        private bool handled = false;
        public bool Handled
        {
            get
            {
                return handled;
            }
            set
            {
                handled = value;
            }
        }

        private List<String> excluedColumns = null;
        public List<String> ExcluedColumns
        {
            get
            {
                return excluedColumns;
            }
            set
            {
                excluedColumns = value;
            }
        }

        public bool shouldShowExcelExpander = true;
        public bool ShouldShowExcelExpander
        {
            get
            {
                return shouldShowExcelExpander;
            }
            set
            {
                shouldShowExcelExpander = value;
            }
        }

        public bool shouldShowNestedIndent = true;
        public bool ShouldShowNestedIndent
        {
            get
            {
                return shouldShowNestedIndent;
            }
            set
            {
                shouldShowNestedIndent = value;
            }
        }
    }

    public delegate void GridCellExportToExcelHandler(object sender, ExportingToExcelEventArgs e);

    public delegate void GridModelExportToExcelHandler(object sender, ExportingGridModelToExcelEventArgs e);

    public static class GridModelExportExtensions
    {

        public static void ExportToExcel(this GridModel gridModel, GridRangeInfo gridRange, ExcelEngine excelEngine, int sheetNumber, IRange excelRange)
        {
            ExportToExcel(gridModel, gridRange, excelEngine, sheetNumber, excelRange, null);
        }
        
        /// <summary>
        /// Extension method for GridModel used for export the selected grid cell data to excel sheet
        /// </summary>
        /// <param name="model">GridModel Object</param>
        /// <param name="gridRange">Range of the grid cell data</param>
        /// <param name="excelEngine">ExcelEngine Object</param>
        /// <param name="sheetNumber">Work sheet number</param>
        /// <param name="excelRange">Range of the excel sheet</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridModel gridModel, GridRangeInfo gridRange, ExcelEngine excelEngine, int sheetNumber, IRange excelRange, GridCellExportToExcelHandler exportingHandler)
        {
            gridRange = GetExpandedRange(gridRange, gridModel);
            ExportToExcel(gridModel, gridRange, excelEngine.Excel.Worksheets[sheetNumber], excelRange, exportingHandler);
        }
#if !SILVERLIGHT

        public static void ExportToExcel(this GridModel gridModel, string fileName, ExcelVersion excelVersion)
        {
            ExportToExcel(gridModel, fileName, excelVersion, null);
        }

        /// <summary>
        ///  Extension method for GridModel used for export the entire grid Cell data to excel sheet
        /// </summary>
        /// <param name="model">GridModel Object</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridModel gridModel, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;
            ExportToExcel(gridModel, GridRangeInfo.Cells(0, 0, gridModel.RowCount - 1, gridModel.ColumnCount - 1), workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1],exportingHandler);
            workbook.SaveAs(fileName);
            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }
#else


         /// <summary>
        ///  Extension method for GridModel used for export the entire grid Cell data to excel sheet
        /// </summary>
        /// <param name="model">GridModel Object</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridModel gridModel,  ExcelVersion excelVersion)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "xls",
                FilterIndex = 1,
                Filter = "(.xls)|*.xls"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    ExportToExcel(gridModel, GridRangeInfo.Cells(0, 0, gridModel.RowCount - 1, gridModel.ColumnCount - 1), workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1]);
                    workbook.SaveAs(stream);
                }
            }

            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();

        }

#endif

        public static void ExportToExcel(this GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange)
        {
            ExportToExcel(gridModel, gridRange, workSheet, excelRange, null);
        }

        /// <summary>
        /// Extension method for GridModel used for export the selected grid cell data to excel sheet
        /// </summary>
        /// <param name="model">GridModel Object</param>
        /// <param name="gridRange">Range of the grid cell data</param>
        /// <param name="mySheet">WorkSheet Object</param>
        /// <param name="excelRange">Range of the excel sheet</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler)
        {
            gridRange = GetExpandedRange(gridRange, gridModel);
            int excelRow = excelRange.Row;
            int excelCol = excelRange.Column;
            CopyRowHeightToSheet(gridModel, workSheet);
            CopyColumnWidthToSheet(gridModel, workSheet);
            CopyFreezePanesToSheet(gridModel, workSheet);
            CopyNamesToSheet(gridModel, workSheet);
            GridExcelConverterControl excelConverter = new GridExcelConverterControl();

            for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
            {
                for (int col = gridRange.Left; col <= gridRange.Right; col++)
                {
                    CoveredCellInfo coveredRange = gridModel.CoveredCells.GetCoveredCell(row, col);
                    if (coveredRange == null)
                        excelConverter.GridCellToExcel(gridModel, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);
                    else if(coveredRange.Top == row && coveredRange.Left == col)
                        excelConverter.GridCellToExcel(gridModel, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);

                    if (coveredRange != null && coveredRange.Left== col && coveredRange.Top == row)
                    {
                        int rowDiff = coveredRange.Top - gridRange.Top;
                        int columnDiff = coveredRange.Left - gridRange.Left;
                        GridRangeInfo gridCRange = GridRangeInfo.Cells(coveredRange.Top, coveredRange.Left, coveredRange.Bottom, coveredRange.Right);
                        IRange exlRange = workSheet.Range[excelRange.Row + rowDiff, excelRange.Column + columnDiff, excelRange.Row + rowDiff + gridCRange.Height - 1, excelRange.Column + columnDiff + gridCRange.Width - 1];

                        if (gridCRange.Height > 1 || gridCRange.Width > 1)
                        {
                            exlRange.Merge();
                            workSheet.SetRowHeightInPixels(exlRange.Row, gridModel.RowHeights[gridCRange.Top]);
                            var borders = gridModel[row, col].Borders;

#if !SILVERLIGHT
                               if (borders.HasLeft)
                                excelConverter.CopyBorder(borders.Left, exlRange.Borders[ExcelBordersIndex.EdgeLeft],false);

                            if (borders.HasTop)
                                excelConverter.CopyBorder(borders.Top, exlRange.Borders[ExcelBordersIndex.EdgeTop],false);

                            if (borders.HasBottom)
                                excelConverter.CopyBorder(borders.Bottom, exlRange.Borders[ExcelBordersIndex.EdgeBottom],false);

                            if (borders.HasRight)
                                excelConverter.CopyBorder(borders.Right, exlRange.Borders[ExcelBordersIndex.EdgeRight],false);
#else
                            if (borders.HasLeft)
                                excelConverter.CopyBorder(borders.Left, exlRange.Borders[ExcelBordersIndex.EdgeLeft]);

                            if (borders.HasTop)
                                excelConverter.CopyBorder(borders.Top, exlRange.Borders[ExcelBordersIndex.EdgeTop]);

                            if (borders.HasBottom)
                                excelConverter.CopyBorder(borders.Bottom, exlRange.Borders[ExcelBordersIndex.EdgeBottom]);

                            if (borders.HasRight)
                                excelConverter.CopyBorder(borders.Right, exlRange.Borders[ExcelBordersIndex.EdgeRight]);
#endif
                        }
                    }
                    else
                    {
                        excelConverter.CopyBorders(gridModel[row, col], workSheet.Range[excelRow, excelCol],false);
                    }

                    excelCol++;
                }

                excelRow++;
                excelCol = excelRange.Column;
            }

        }

        private static void CopyFreezePanesToSheet(GridModel gridModel, IWorksheet workSheet)
        {
            if (gridModel == null)
                throw new ArgumentNullException("grid");

            if (workSheet == null)
                throw new ArgumentNullException("sheet");

            if (gridModel.FrozenRows > 0 && gridModel.FrozenColumns > 0)
                workSheet[gridModel.FrozenRows, gridModel.FrozenColumns].FreezePanes();

        }

        private static void CopyColumnWidthToSheet(GridModel gridModel, IWorksheet workSheet)
        {
            if (gridModel == null)
                throw new ArgumentNullException("grid");

            if (workSheet == null)
                throw new ArgumentNullException("sheet");

            for (int i = 0; i < gridModel.ColumnCount; i++)
            {
                workSheet.SetColumnWidthInPixels(i+1, (int)gridModel.ColumnWidths[i]);
            }
        }

        private static void CopyRowHeightToSheet(GridModel gridModel, IWorksheet workSheet)
        {
            if (gridModel == null)
                throw new ArgumentNullException("grid");

            if (workSheet == null)
                throw new ArgumentNullException("sheet");

            for (int i = 0; i < gridModel.RowCount; i++)
            {
                workSheet.SetRowHeightInPixels(i+1, gridModel.RowHeights[i]);
            }

        }

        private static void CopyNamesToSheet(GridModel gridModel, IWorksheet workSheet)
        {
            if (gridModel == null)
                throw new ArgumentNullException("grid");

            if (workSheet == null)
                throw new ArgumentNullException("sheet");
#if !SILVERLIGHT
            foreach (DictionaryEntry item in gridModel.FormulaEngine.NamedRanges)
            {
                workSheet.Names.Add(item.Key.ToString());
                workSheet.Names[item.Key.ToString()].Value = item.Value.ToString() == string.Empty ? null : item.Value.ToString();
            }
#else
            foreach (var item in gridModel.FormulaEngine.NamedRanges)
            {
                workSheet.Names.Add(item.Key.ToString());
                workSheet.Names[item.Key.ToString()].Value = item.Value.ToString() == string.Empty ? null : item.Value.ToString();
            }
#endif
        }

        /// <summary>
        /// Expand the range when row,column or table is selected
        /// </summary>
        /// <param name="range">GridRangeInfo Object</param>
        /// <param name="model">GridModel object</param>
        /// <returns></returns>
        private static GridRangeInfo GetExpandedRange(GridRangeInfo range, GridModel gridModel)
        {

            if (range.IsCols)
            {
                range = range.ExpandRange(range.Top + 1, range.Left, gridModel.RowCount, range.Left);
            }

            if (range.IsRows)
            {
                range = range.ExpandRange(range.Top, range.Left + 1, range.Top, gridModel.ColumnCount);
            }

            if (range.IsTable)
            {
                range = range.ExpandRange(range.Top + 1, range.Left + 1, gridModel.RowCount, gridModel.ColumnCount);
            }

            return range;
        }

        /// <summary>
        /// Exports the content of GridControl into a CSV file.
        /// </summary>
        /// <param name="gridModel">GridModel Object.</param>
        /// <param name="fileName">The name of the CSV file.</param>
        public static void ExportToCSV(this GridModel gridModel, string fileName)
        {
            ExportToCSV(gridModel, GridRangeInfo.Table(), fileName);
        }

        /// <summary>
        /// Extension method for CSV export used for exporting the given grid range into a CSV file.
        /// </summary>
        /// <param name="gridModel">GridModel Object.</param>
        /// <param name="gridRange">The grid range ro be exported.</param>
        /// <param name="fileName">The name of the CSV file.</param>
        public static void ExportToCSV(this GridModel gridModel, GridRangeInfo gridRange, string fileName)
        {
            string csvText;

            CopyTextToBuffer(gridModel, out csvText, gridRange);
#if SILVERLIGHT
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "(.csv)|*.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                using(var sw = new StreamWriter(sfd.OpenFile()))
				{
					sw.Write(csvText);
				}
            }
#else
            File.WriteAllText(fileName, csvText);
#endif
        }

        private static bool CopyTextToBuffer(this GridModel gridModel, out string buffer, Syncfusion.Windows.Controls.Grid.GridRangeInfo range)
        {
            GridRangeInfo tRange = GetExpandedRange(range, gridModel);

            StringBuilder sb = new StringBuilder();
            string tabDelim = ",";

            for (int row = tRange.Top; row <= tRange.Bottom; row++)
            {
                bool firstCol = true;
                for (int col = tRange.Left; col <= tRange.Right; col++)
                {
                    if (!firstCol)
                    {
                        sb.Append(tabDelim);
                    }

                    GridStyleInfo style = gridModel[row, col];
                    string text = style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.CopyText);
                    text = new StringBuilder(text)
                        .Replace(Environment.NewLine, " ")
                        .Replace("\r", " ")
                        .Replace("\n", " ")
                        .ToString()
                        .Trim();
                    sb.Append("\"" + text + "\"");
                    firstCol = false;
                }

                sb.Append(Environment.NewLine);
            }

            buffer = sb.ToString();
            return true;
        }
    }
}
