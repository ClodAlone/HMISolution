#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if EXCELGRID
using Syncfusion.Windows.Controls.Spreadsheet;
namespace Syncfusion.Windows.Controls.Grid.Converter
#else
namespace Syncfusion.Windows.Controls.Grid.Converter
#endif
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
    using Syncfusion.XlsIO.Implementation.Charts;
    using Syncfusion.XlsIO.Implementation.Shapes;
    using Syncfusion.XlsIO.Implementation.Collections;
    using System.Text.RegularExpressions;
#if !SILVERLIGHT
    
    using System.Windows.Documents;
    using Syncfusion.Windows.Shared;
#endif
#if SILVERLIGHT
    using System.Windows.Documents; 
    using Syncfusion.Windows.Tools.Controls;
#endif

    #region EventHandler
    
#if EXCELGRID
    /// <summary>
    /// Importing event arguments
    /// </summary>
    public class ImportCellFromExcelEventArgs : EventArgs
    {
        public ImportCellFromExcelEventArgs(IRange range, SpreadsheetGridStyleInfo cell, bool handled)
        {
            Range = range;
            Handled = handled;
            Cell = cell;
        }

        public bool Handled
        {
            get;
            set;
        }

        public IRange Range
        {
            get;
            internal set;
        }

        public SpreadsheetGridStyleInfo Cell
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

    public delegate void ImportGridCellFromExcelHandler(object sender, ImportCellFromExcelEventArgs e);
#else
    /// <summary>
    /// Importing event arguments
    /// </summary>
    public class ImportingCellFromExcelEventArgs : EventArgs
    {
        public ImportingCellFromExcelEventArgs(IRange range, IWorksheet sheet, GridStyleInfo cell, bool handled)
        {
            Range = range;
            Handled = handled;
            Cell = cell;
            Sheet = sheet;
        }

        public bool Handled
        {
            get;
            set;
        }

        public IRange Range
        {
            get;
            internal set;
        }

        public IWorksheet Sheet
        {
            get;
            internal set;
        }

        public GridStyleInfo Cell
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

    public delegate void GridCellImportFromExcelHandler(object sender, ImportingCellFromExcelEventArgs e);
#endif
    #endregion

#if EXCELGRID
    public static class ExcelGridModelImportExtensions
#else
    public static class GridModelImportExtensions
#endif
    {
        #region Fields
        
        private static bool _protected;

        /// <summary>
        /// Sheet family id - This is used to identify a family of grids.
        /// </summary>
        internal static int SheetFamilyId = 1;

#if EXCELGRID
        private static ExcelGridImportTableStyleConverter importTableStyleConverter = new ExcelGridImportTableStyleConverter();
#else
        private static GridImportTableStyleConverter importTableStyleConverter = new GridImportTableStyleConverter();
#endif
        #endregion

        #region GridImportExtensions

        /// <summary>
        /// Imports single sheets from excel to Grid.
        /// </summary>
        /// <param name="grid">The Grid.</param>
        /// <param name="fileStream">The File stream.</param>
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, Stream fileStream)
#else
        public static void ImportFromExcel(this GridModel grid, Stream fileStream)
#endif
        {
            ImportFromExcel(grid, fileStream,null);
        }


        /// <summary>
        /// Imports single sheets from excel to Grid with importing event handler.
        /// </summary>
        /// <param name="grid">The Grid.</param>
        /// <param name="fileStream">The File stream.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, Stream fileStream, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        public static void ImportFromExcel(this GridModel grid, Stream fileStream, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            if (grid == null)
                throw new ArgumentNullException("grid");

            using (ExcelEngine engine = new ExcelEngine())
            {
                IWorkbook book = engine.Excel.Workbooks.Open(fileStream);
#if EXCELGRID
                ImportFromExcel(grid, book, book.Worksheets[0], importFromExcelHandler);
#else
                ImportFromExcel(grid, book.Worksheets[0], importFromExcelHandler);
#endif
                //book.Close(false);
            }
        }

        /// <summary>
        /// Imports single sheets from excel to Grid.
        /// </summary>
        /// <param name="grid">The Grid.</param>
        /// <param name="FileName">Name of the file.</param>
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, string FileName)
#else
        public static void ImportFromExcel(this GridModel grid, string FileName)
#endif
        
        {
            ImportFromExcel(grid, FileName, null);
        }


        /// <summary>
        /// Imports single sheets from excel to Grid with importing event handler.
        /// </summary>
        /// <param name="grid">The Grid.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, string FileName, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
            public static void ImportFromExcel(this GridModel grid, string FileName, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (FileName == null)
                throw new ArgumentNullException("FileName");

            if (FileName.Length == 0)
                throw new ArgumentException("FileName - string can not be empty");

            if (grid == null)
                throw new ArgumentNullException("grid");

            using (ExcelEngine engine = new ExcelEngine())
            {
                IWorkbook book = engine.Excel.Workbooks.Open(FileName);
#if EXCELGRID
                ImportFromExcel(grid, book, book.Worksheets[0], importFromExcelHandler);
#else
                ImportFromExcel(grid, book.Worksheets[0], importFromExcelHandler);
#endif
                //book.Close(false);
            }
        }
        
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, IWorkbook book, IWorksheet sheet)
        {
            ImportFromExcel(grid, book, sheet, null);
        }
#else
        /// <summary>
        /// Imports single sheets from excel to Grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="sheet">The Sheet.</param>
        public static void ImportFromExcel(this GridModel grid, IWorksheet sheet)
        {
            ImportFromExcel(grid, sheet, null);
        }
#endif

        /// <summary>
        /// Imports single sheets from excel to Grid with importing event handler.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="importingHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ImportFromExcel(this SpreadsheetGridModel grid, IWorkbook book, IWorksheet sheet, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        public static void ImportFromExcel(this GridModel grid, IWorksheet sheet, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (grid == null)
                throw new ArgumentNullException("grid");

            int rowCount = 0;
            int columnCount = 0;
            IRange range = sheet.Range;
            if (!sheet.IsRowColumnHeadersVisible)
            {
                grid.RowHeights[0] = 0.0;
                grid.ColumnWidths[0] = 0.0;
            }
            if (range.LastRow > 0 && range.LastColumn > 0)
            {
                grid.RowCount = range.LastRow + 1;
                grid.ColumnCount = range.LastColumn + 1;

                foreach (IListObject table in sheet.ListObjects)
                {
                    if (rowCount < table.Location.LastRow)
                        rowCount = table.Location.LastRow;
                    if (columnCount < table.Location.LastColumn)
                        columnCount = table.Location.LastColumn;
                }

                if (grid.RowCount < rowCount)
                    grid.RowCount = rowCount + 1;
                if (grid.ColumnCount < columnCount)
                    grid.ColumnCount = columnCount + 1;

                CopyRowHeightToGrid(sheet, grid);
                CopyColumnWidthToGrid(sheet, grid);
                CopyFreezePanesToGrid(sheet, grid);
                CopyMergesToGrid(sheet, grid);
                CopyNamesToGrid(sheet, grid);
                CopyComboxToGrid(sheet, grid);
                CopyImageToGrid(sheet, grid);


                for (int iRow = range.Row; iRow <= range.LastRow; iRow++)
                {
                    for (int iCol = range.Column; iCol <= range.LastColumn; iCol++)
                    {
                        IRange rangeToConvert = sheet.Range[iRow, iCol];
                        #if EXCELGRID
                        ConvertExcelRangeToGrid(grid, book, sheet, rangeToConvert, importFromExcelHandler);
#else
                        ConvertExcelRangeToGrid(grid, sheet, rangeToConvert, importFromExcelHandler);
#endif
                    }
                }
            }
        }

#if !SyncfusionFramework3_5
        /// <summary>
        /// Imports the particular sheets from excel to Grid Model array.
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="arrIndexes">The array indexes.</param>
        /// <param name="arrModels">The array models.</param>
#if EXCELGRID 
        public static void ImportFromExcel(string strFileName, int[] arrIndexes, SpreadsheetGridModel[] arrModels)
#else
        public static void ImportFromExcel(string strFileName, int[] arrIndexes, GridModel[] arrModels)
#endif
        {
            ImportFromExcel(strFileName, arrIndexes, arrModels, null);
        }


        /// <summary>
        /// Imports the particular sheets from excel to Grid Model array with importing event handler. 
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="arrIndexes">The array indexes.</param>
        /// <param name="arrModels">The array models.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ImportFromExcel(string strFileName, int[] arrIndexes, SpreadsheetGridModel[] arrModels, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        public static void ImportFromExcel(string strFileName, int[] arrIndexes, GridModel[] arrModels, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (strFileName == null)
                throw new ArgumentNullException("strFileName");

            if (strFileName.Length == 0)
                throw new ArgumentException("strFileName - string can not be empty");

            if (arrIndexes == null)
                throw new ArgumentNullException("arrIndexes");

            if (arrModels == null)
                throw new ArgumentNullException("arrModels");

            if (arrIndexes.Length != arrModels.Length)
                throw new ArgumentException("Indexes and models do not correspond each other");

            using (ExcelEngine engine = new ExcelEngine())
            {
                IWorkbook book = engine.Excel.Workbooks.Open(strFileName);
                int iSheetsCount = book.Worksheets.Count;

                for (int i = 0, len = arrIndexes.Length; i < len; i++)
                {
                    int index = arrIndexes[i];
                    if (index >= iSheetsCount) continue;

                    IWorksheet sheet = book.Worksheets[index];
#if EXCELGRID
                    SpreadsheetGridModel model = arrModels[i];
                    ImportFromExcel(model, book, sheet, importFromExcelHandler);
#else
                    GridModel model = arrModels[i];
                    ImportFromExcel(model, sheet, importFromExcelHandler);
#endif
                }
                //book.Close();
            }
        }

        /// <summary>
        /// Imports the array ranges from excel to Grid Model array.
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="arrRanges">The array ranges.</param>
        /// <param name="arrModels">The array models.</param>
#if EXCELGRID
        internal static void ImportFromExcel(string strFileName, IRange[] arrRanges, SpreadsheetGridModel[] arrModels)
#else
        internal static void ImportFromExcel(string strFileName, IRange[] arrRanges, GridModel[] arrModels)
#endif
        {
            ImportFromExcel(strFileName, arrRanges, arrModels,null);
        }


        /// <summary>
        /// Imports the array ranges from excel to Grid Model array with importing event handler. 
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="arrRanges">The array ranges.</param>
        /// <param name="arrModels">The array models.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
#if EXCELGRID
        internal static void ImportFromExcel(string strFileName, IRange[] arrRanges, SpreadsheetGridModel[] arrModels, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        internal static void ImportFromExcel(string strFileName, IRange[] arrRanges, GridModel[] arrModels, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (strFileName == null)
                throw new ArgumentNullException("strFileName");

            if (strFileName.Length == 0)
                throw new ArgumentException("strFileName - string can not be empty");

            if (arrRanges == null)
                throw new ArgumentNullException("arrRanges");

            if (arrModels == null)
                throw new ArgumentNullException("arrModels");

            using (ExcelEngine engine = new ExcelEngine())
            {
                IWorkbook book = engine.Excel.Workbooks.Open(strFileName);
                int iSheetsCount = book.Worksheets.Count;
                int iModelIndex = 0;
                int iModelCount = arrModels.Length;

                for (int i = 0, len = arrRanges.Length; i < len; i++)
                {
                    IRange range = arrRanges[i];
                    for (int j = range.Row, last = range.LastRow; j <= last; j++)
                    {
                        if (j >= iSheetsCount) break;

                        IWorksheet sheet = book.Worksheets[j];

                        if (iModelIndex >= iModelCount)
                        {
                            throw new ArgumentOutOfRangeException("Model index was out of range");
                        }
#if EXCELGRID
                        ImportFromExcel(arrModels[iModelIndex], book, sheet, importFromExcelHandler);
#else
                        ImportFromExcel(arrModels[iModelIndex], sheet, importFromExcelHandler);
#endif
                        iModelIndex++;
                    }
                }

                //book.Close();
            }
        }
#endif

        /// <summary>
        /// Imports the entire workbook.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns>Array of GridModels</returns>
#if EXCELGRID
        public static SpreadsheetGridModel[] ImportFromExcel(IWorkbook book)
#else
        public static GridModel[] ImportFromExcel(IWorkbook book)
#endif
        {
            return ImportFromExcel(book, null);
        }


        /// <summary>
        /// Imports the entire workbook.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
        /// <returns>Array of GridModels</returns>
#if EXCELGRID
        public static SpreadsheetGridModel[] ImportFromExcel(IWorkbook book, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
         public static GridModel[] ImportFromExcel(IWorkbook book, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (book == null)
                throw new ArgumentNullException("book");

            int iSheetsCount = book.Worksheets.Count;
#if EXCELGRID
            SpreadsheetGridModel[] arrResult = new SpreadsheetGridModel[iSheetsCount];
#else
            GridModel[] arrResult = new GridModel[iSheetsCount];
#endif
            SheetFamilyId = GridFormulaEngine.CreateSheetFamilyID();
            for (int i = 0; i < iSheetsCount; i++)
            {
#if EXCELGRID
                SpreadsheetGridModel model = new SpreadsheetGridModel();
                GridFormulaEngine.RegisterGridAsSheet(book.Worksheets[i].Name, model, SheetFamilyId);
                ImportFromExcel(model, book, book.Worksheets[i], importFromExcelHandler);
#else
                GridModel model = new GridModel();
                GridFormulaEngine.RegisterGridAsSheet(book.Worksheets[i].Name, model, SheetFamilyId);
                ImportFromExcel(model, book.Worksheets[i], importFromExcelHandler);
#endif
                model.FormulaEngine.SupportBlanksInSheetNames = true;
                model.FormulaEngine.UseNoAmpersandQuotes = true;
                model.Options.AllowExcelLikeResizing = true;
                model.TableStyle.CellType = "FormulaCell";
                arrResult[i] = model;
            }

            //book.Close(false);

            return arrResult;
        }

        /// <summary>
        /// Converts the excel range to grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rangeToConvert">The range to convert.</param>
        /// <param name="importingHandler">The importing handler.</param>
#if EXCELGRID
        public static void ConvertExcelRangeToGrid(SpreadsheetGridModel grid, IWorkbook book, IWorksheet sheet, IRange rangeToConvert, ImportGridCellFromExcelHandler importingHandler)
#else
        public static void ConvertExcelRangeToGrid(GridModel grid, IWorksheet sheet, IRange rangeToConvert, GridCellImportFromExcelHandler importingHandler)
#endif
        {
            if (rangeToConvert == null)
                throw new ArgumentNullException("rangeToConvert");

            if (grid == null)
                throw new ArgumentNullException("grid");

            //if (!rangeToConvert.IsInitialized) return;


#if EXCELGRID
            SpreadsheetGridStyleInfo cell = new SpreadsheetGridStyleInfo();
            ImportCellFromExcelEventArgs args = new ImportCellFromExcelEventArgs(rangeToConvert, cell, false);
#else
            GridStyleInfo cell = new GridStyleInfo();
            ImportingCellFromExcelEventArgs args = new ImportingCellFromExcelEventArgs(rangeToConvert, sheet, cell, false);
#endif
            args.RowIndex = rangeToConvert.Row;
            args.ColumnIndex = rangeToConvert.Column;
            if (importingHandler != null)
            {
                importingHandler(grid, args);
            }

            if (args.Handled)
            {
                grid.Data[rangeToConvert.Row, rangeToConvert.Column] = cell.Store;
                return;
            }
            //cell = grid[rangeToConvert.Row, rangeToConvert.Column];
            _protected = sheet.IsPasswordProtected;
            if (rangeToConvert.HasDateTime)
            {
                cell.CellType = "DateTimeEdit";
                cell.CellValue = rangeToConvert.DateTime;
                cell.DateTimeEdit.DateTimePattern = GetDateTimeFormat(rangeToConvert.NumberFormat);
            }
            else if (rangeToConvert.HasNumber)
            {
                if (rangeToConvert.NumberFormat.Equals("General"))
                {
                    cell.CellValue = rangeToConvert.Number;
                }
                else
                {
                    if (rangeToConvert.NumberFormat.IndexOf("%") != -1)
                    {
                        cell.CellValue = rangeToConvert.DisplayText;
                    }
                    else
                    {
                        cell.CellValue = rangeToConvert.Number;
                    }
                }
                if (rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                    rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;

            }
            else if (rangeToConvert.IsBoolean)
            {
                cell.CellValue = rangeToConvert.Boolean;
            }
            else
            {
                if (rangeToConvert.HasFormula || rangeToConvert.HasFormulaArray)
                {
                    cell.CellType = "FormulaCell";
                    string text = rangeToConvert.Value;
                    cell.CellValue2 = text;
                    cell.Text = text.Replace("'", "");
                    //cell.FormulaTag = new GridFormulaTag(rangeToConvert.Value, rangeToConvert.DisplayText, rangeToConvert.Row, rangeToConvert.Column);
                }
                else
                    cell.Text = rangeToConvert.Value;

                if (rangeToConvert.NumberFormat.Equals(";;;"))
                {
                    cell.Text = "";
                }

                if (rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                {
                    if (!(rangeToConvert.HasFormula || rangeToConvert.HasFormulaArray))
                        rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    else if (rangeToConvert.FormulaNumberValue.ToString() != double.NaN.ToString())
                        rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                }
            }

            if (!rangeToConvert.NumberFormat.Equals("General"))
            {
                cell.Format = GetFormat(rangeToConvert.NumberFormat);
            }

            if (rangeToConvert.ConditionalFormats != null && rangeToConvert.ConditionalFormats.Count > 0)
            {
                CopyConditionalFormat(cell, rangeToConvert);
            }

            if (rangeToConvert.HasDataValidation)
            {
                CopyDataValidation(cell, rangeToConvert, sheet);
            }

            if (rangeToConvert.CellStyleName == "Hyperlink")
            {
#if EXCELGRID
                CopyHyperlinkToCell(book, sheet, rangeToConvert, cell);
#else
                CopyHyperlinkToCell(sheet,rangeToConvert, cell);
#endif
            }

            if (rangeToConvert.Comment != null && rangeToConvert.Comment.Text != "")
            {
                cell.Comment = rangeToConvert.Comment.Text;
            }

            if (rangeToConvert.ColumnWidth != 0 || rangeToConvert.RowHeight != 0)
            {
                SetCellStyle(rangeToConvert, cell, sheet);
            }

            if (_protected && rangeToConvert.HasStyle)
            {
                var cellstyle = rangeToConvert.CellStyle;
                if (cellstyle.Locked == true)
                {
                    cell.ReadOnly = true;
                }
            }

            grid.Data[rangeToConvert.Row, rangeToConvert.Column] = cell.Store;
        }
        #endregion

        #region VirtualGridImportExtensions

        /// <summary>
        /// Imports the single sheet from excel to virtual grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="importFromExcelHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ImportFromExcelToVirtualGrid(this SpreadsheetGridModel grid, IWorkbook book, IWorksheet sheet, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        public static void ImportFromExcelToVirtualGrid(this GridModel grid,IWorkbook book, IWorksheet sheet, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (grid == null)
                throw new ArgumentNullException("grid");

            IRange range = sheet.Range;
            int rowCount = 0;
            int columnCount = 0;
            if (!sheet.IsRowColumnHeadersVisible)
            {
                grid.RowHeights[0] = 0.0;
                grid.ColumnWidths[0] = 0.0;
            }
            if ((range.LastRow > 0 && range.LastColumn > 0) || sheet.Charts.Count > 0 || sheet.Shapes.Count > 0)
            {
                grid.RowCount = range.LastRow + 1;
                grid.ColumnCount = range.LastColumn + 1;

                foreach (IListObject table in sheet.ListObjects)
                {
                    if (rowCount < table.Location.LastRow)
                        rowCount = table.Location.LastRow;
                    if (columnCount < table.Location.LastColumn)
                        columnCount = table.Location.LastColumn;
                }

                if (grid.RowCount < rowCount)
                    grid.RowCount = rowCount + 1;
                if (grid.ColumnCount < columnCount)
                    grid.ColumnCount = columnCount + 1;

                CopyRowHeightToGrid(sheet, grid);
                CopyColumnWidthToGrid(sheet, grid);
                CopyFreezePanesToGrid(sheet, grid);
                CopyMergesToGrid(sheet, grid);
                CopyNamesToGrid(sheet, grid);
                CopyComboxToGrid(sheet, grid);
                CopyHyperlinkToVirtualGrid(sheet,book, grid);
                CopyImageToGrid(sheet, grid);
                CopyTextBoxToGrid(sheet, grid);
                //CopyCheckBoxToGrid(sheet, grid);
#if !SILVERLIGHT
                CopyChartToGrid(sheet, grid);
#endif
                CopySparkLineToGrid(sheet, grid);
            }
        }

        /// <summary>
        /// Imports the entire workbook from excel to virtual grid.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns>Array of GridModel</returns>
#if EXCELGRID
        public static SpreadsheetGridModel[] ImportFromExcelToVirtualGrid(IWorkbook book)
#else
        public static GridModel[] ImportFromExcelToVirtualGrid(IWorkbook book)
#endif
        {
            return ImportFromExcelToVirtualGrid(book, null);
        }

        /// <summary>
        /// Imports the entire workbook from excel to virtual grid.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="importFromExcelHandler">importing event handler.</param>
        /// <returns>Array of GridModel</returns>
#if EXCELGRID
        public static SpreadsheetGridModel[] ImportFromExcelToVirtualGrid(IWorkbook book, ImportGridCellFromExcelHandler importFromExcelHandler)
#else
        public static GridModel[] ImportFromExcelToVirtualGrid(IWorkbook book, GridCellImportFromExcelHandler importFromExcelHandler)
#endif
        {
            if (book == null)
                throw new ArgumentNullException("book");

            int iSheetsCount = book.Worksheets.Count;
            int iChartCount = book.Charts.Count;
#if EXCELGRID
            SpreadsheetGridModel[] arrResult = new SpreadsheetGridModel[book.TabSheets.Count];
#else
            GridModel[] arrResult = new GridModel[iSheetsCount];
#endif

            SheetFamilyId = GridFormulaEngine.CreateSheetFamilyID();
            for (int i = 0; i < iSheetsCount; i++)
            {
#if EXCELGRID
                SpreadsheetGridModel model = new SpreadsheetGridModel();
#else
                GridModel model = new GridModel();
#endif
                GridFormulaEngine.RegisterGridAsSheet(book.Worksheets[i].Name, model, SheetFamilyId);
                ImportFromExcelToVirtualGrid(model, book, book.Worksheets[i], importFromExcelHandler);
#if EXCELGRID
                model.FormulaEngine.ForceParsingOfLibraryFunctionArguments = true;
#endif
                model.FormulaEngine.SupportBlanksInSheetNames = true;
                model.FormulaEngine.UseNoAmpersandQuotes = true;
                model.Options.AllowExcelLikeResizing = true;
                model.TableStyle.CellType = "FormulaCell";
                arrResult[i] = model;
            }

            for (int i = 0; i < iChartCount; i++)
            {
#if EXCELGRID
                SpreadsheetGridModel model = new SpreadsheetGridModel();
#else
               GridModel model = new GridModel();
#endif
                model.RowCount = 0;
                model.ColumnCount = 0;
                GridFormulaEngine.RegisterGridAsSheet(book.TabSheets[i].Name, model, SheetFamilyId);
#if !SILVERLIGHT
                CopyChartToGrid(book.Charts[i], model);
#endif
                model.FormulaEngine.SupportBlanksInSheetNames = true;
                model.FormulaEngine.UseNoAmpersandQuotes = true;
                model.Options.AllowExcelLikeResizing = true;
                model.TableStyle.CellType = "FormulaCell";
                arrResult[iSheetsCount + i] = model;
            }

            return arrResult;
        }

        #if EXCELGRID
        public static void ConvertExcelRangeToVirtualGrid(SpreadsheetGridStyleInfo cell, IWorksheet sheet, IRange rangeToConvert, ImportGridCellFromExcelHandler importingHandler)
#else
        public static void ConvertExcelRangeToVirtualGrid(GridStyleInfo cell, IWorksheet sheet, IRange rangeToConvert, GridCellImportFromExcelHandler importingHandler)
#endif
        {
            ConvertExcelRangeToVirtualGrid(cell, sheet, rangeToConvert, true, importingHandler);
        }

        /// <summary>
        /// Converts the excel range to virtual grid.
        /// </summary>
        /// <param name="cell">The grid cell.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rangeToConvert">The excel cell.</param>
        /// <param name="importingHandler">The importing event handler.</param>
#if EXCELGRID
        public static void ConvertExcelRangeToVirtualGrid(SpreadsheetGridStyleInfo cell, IWorksheet sheet, IRange rangeToConvert, bool ImportConditionalFormat, ImportGridCellFromExcelHandler importingHandler)
#else
        public static void ConvertExcelRangeToVirtualGrid(GridStyleInfo cell, IWorksheet sheet, IRange rangeToConvert, bool ImportConditionalFormat, GridCellImportFromExcelHandler importingHandler)
#endif
        {
            if (rangeToConvert == null)
                throw new ArgumentNullException("rangeToConvert");

            if (cell == null)
                throw new ArgumentNullException("cell");
#if EXCELGRID
            ImportCellFromExcelEventArgs args = new ImportCellFromExcelEventArgs(rangeToConvert, cell, false);
#else
            ImportingCellFromExcelEventArgs args = new ImportingCellFromExcelEventArgs(rangeToConvert, sheet, cell, false);
#endif
            args.RowIndex = rangeToConvert.Row;
            args.ColumnIndex = rangeToConvert.Column;
            if (importingHandler != null)
            {
                importingHandler(cell, args);
            }

            if (args.Handled)
            {
                return;
            }

            _protected = sheet.IsPasswordProtected;
            if (rangeToConvert.HasDateTime)
            {                
                //MT Issue Fix-Referhttp://stackoverflow.com/questions/310700/meaning-of-exception-in-c-sharp-app-not-a-legal-oleaut-date

                if (rangeToConvert.Number < -657435 || rangeToConvert.Number > 2958466)
                {
                    cell.CellValue = "########";                    
                }
                else
                {
                    cell.CellValue = rangeToConvert.DateTime;
                    cell.CellType = "DateTimeEdit";
                    cell.DateTimeEdit.DateTimePattern = DateTimePattern.CustomPattern;
                    cell.DateTimeEdit.CustomPattern = GetFormat(rangeToConvert.NumberFormat);
                }
            }
            else if (rangeToConvert.HasNumber)
            {
                if (rangeToConvert.NumberFormat.Equals("General"))
                {
                    cell.CellValue = rangeToConvert.Number;
                }
                else
                {
                    if (rangeToConvert.NumberFormat.IndexOf("%") != -1)
                    {
                        cell.CellValue = rangeToConvert.DisplayText;
                    }
                    else
                    {
                        cell.CellValue = rangeToConvert.Number;
                    }
                }
                cell.CellValue2 = rangeToConvert.HorizontalAlignment;
                if (rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                    rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;

            }
            else if (rangeToConvert.IsBoolean)
            {
                cell.CellValue = rangeToConvert.Boolean;
            }
            else
            {
                if (rangeToConvert.HasFormula || rangeToConvert.HasFormulaArray)
                {
                    cell.CellType = "FormulaCell";
                    string text = rangeToConvert.Value;
                    cell.CellValue2 = text;
                    cell.Text = text.Replace("'", "");
                }
                else if (rangeToConvert.HasRichText)
                {
                    if (_protected && rangeToConvert.HasStyle)
                    {
                        var cellstyle = rangeToConvert.CellStyle;
                        if (cellstyle.Locked == true)
                        {
                            cell.ReadOnly = true;
                        }
                    }
                    cell.CellValue = ConvertCellValueToParagraph(rangeToConvert.RichText);
                    cell.CellType = "RichText";
                }
                else
                    cell.Text = rangeToConvert.DisplayText.Trim();


                if (rangeToConvert.CellStyle.IsFirstSymbolApostrophe)
                    cell.CellValue2 = ExcelHAlign.HAlignLeft;

                if (rangeToConvert.NumberFormat.Equals(";;;") && rangeToConvert.ColumnWidth != 0.0)
                {
                    cell.Text = "";
                }

                if (rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
                {
                    if (rangeToConvert.FormulaNumberValue.ToString() != double.NaN.ToString())
                        rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                }
            }

            if (!rangeToConvert.NumberFormat.Equals("General"))
            {
                cell.Format = GetFormat(rangeToConvert.NumberFormat);
                if (rangeToConvert.Text != null && rangeToConvert.Text != string.Empty && rangeToConvert.Text[0] == '\'')
                {
                    cell.Format = string.Empty;
                }
            }

            if (ImportConditionalFormat)
            {
                if (rangeToConvert.ConditionalFormats != null && rangeToConvert.ConditionalFormats.Count > 0)
                {
                    CopyConditionalFormat(cell, rangeToConvert);
                }
            }
            if (rangeToConvert.HasDataValidation)
            {
                CopyDataValidation(cell, rangeToConvert, sheet);
            }
            if (rangeToConvert.Comment != null && rangeToConvert.Comment.Text != "")
                cell.Comment = rangeToConvert.Comment.Text;
            else
                cell.Comment = null;
            SetCellStyle(rangeToConvert, cell, sheet);
            if (_protected && rangeToConvert.HasStyle)
            {
                var cellstyle = rangeToConvert.CellStyle;
                cell.ReadOnly = cellstyle.Locked;
            }
        }
        #endregion

        #region ConditionalFormats

        private static bool hastwocondition = false;
        
#if EXCELGRID
        private static void CopyConditionalFormat(SpreadsheetGridStyleInfo cell, IRange rangeToConvert)
#else
        private static void CopyConditionalFormat(GridStyleInfo cell, IRange rangeToConvert)
#endif
        {
            bool hasCondition = false;
            GridConditionalFormat GridConditionalFormat1 = new GridConditionalFormat();
            for (int count = 0; count < rangeToConvert.ConditionalFormats.Count; count++)
            {
                IConditionalFormat format = rangeToConvert.ConditionalFormats[count];
                if (format.FormatType == ExcelCFType.CellValue)
                {
                    hasCondition = true;
                    copyConditionalFormatToCell(GridConditionalFormat1, format);
                    copyConditionFormatStyle(GridConditionalFormat1, format);
                }
                else if (format.FormatType == ExcelCFType.Formula)
                {
                    //cell.CellValue = "=" + format.FirstFormula;
                    hasCondition = true;
                    cell.ApplyConditionalFormatBasedOn = ApplyConditionalBasedOn.FormulaValue;
                    GridConditionalFormat1.FormulaText = "=" + format.FirstFormula;
                    copyConditionFormatStyle(GridConditionalFormat1, format);
                    if (!GridConditionalFormat1.FormulaText.Contains(':'))
                    {
                        RowColumnIndex rowColumn = GetRowColumnIndex(format.FirstFormulaR1C1);
                        if (!rowColumn.IsEmpty)
                            GridConditionalFormat1.Cell = rowColumn;
                    }
                }
                else if (format.FormatType == ExcelCFType.Blank)
                {
                    // When Conditional formula applies to multiple cells, first formula  
                    // is same for all the applied cells. So it causes the conditional format issue. 
                    // Here we replace the formula with the current range address.
                    string formula = format.FirstFormula;
                    int leftParens;
                    int rightParens = formula.IndexOf(')');

                    leftParens = rightParens - 1;
                    while (leftParens > -1 && formula[leftParens] != '(')
                        leftParens--;

                    int len = (rightParens - 1) - leftParens;
                    string refCell = formula.Substring(leftParens + 1, (rightParens - 1) - leftParens);
                    formula = formula.Replace(refCell, rangeToConvert.AddressLocal);

                    hasCondition = true;
                    cell.ApplyConditionalFormatBasedOn = ApplyConditionalBasedOn.FormulaValue;
                    GridConditionalFormat1.FormulaText = "=" + formula;
                    copyConditionFormatStyle(GridConditionalFormat1, format);
                    if (!GridConditionalFormat1.FormulaText.Contains(':'))
                    {
                        RowColumnIndex rowColumn = GetRowColumnIndex(format.FirstFormulaR1C1);
                        if (!rowColumn.IsEmpty)
                            GridConditionalFormat1.Cell = rowColumn;
                    }
                }
            }
            if (hasCondition)
                cell.ConditionalFormat = GridConditionalFormat1;
        }

        /// <summary>
        /// Copies the conditional format to grid cell.
        /// </summary>
        /// <param name="GridConditionalFormating1">The GridConditionalFormating.</param>
        /// <param name="format">The format.</param>
        private static void copyConditionalFormatToCell(GridConditionalFormat GridConditionalFormat1, IConditionalFormat format)
        {
            GridCondition GridCondition1 = new GridCondition();
            GridCondition1.PredicateType = Data.PredicateType.Or;
            GridCondition GridCondition2 = new GridCondition();
            GridCondition2.PredicateType = Data.PredicateType.And;
            string firstFormula = string.Empty;
            string secondFormula = string.Empty;
            if (format.FirstFormula != null)
                firstFormula = format.FirstFormula.Trim('\"');
            if (format.SecondFormula != null)
                secondFormula = format.SecondFormula.Trim('\"');
            hastwocondition = false;
            switch (format.Operator)
            {
                case ExcelComparisonOperator.Between:
                    {
                        GridCondition1.ConditionType = GridConditionType.GreaterThan;
                        GridCondition2.ConditionType = GridConditionType.LessThan;
                        GridCondition1.Value = firstFormula;
                        GridCondition2.Value = secondFormula;
                        GridCondition1.PredicateType = Data.PredicateType.And;
                        GridCondition2.PredicateType = Data.PredicateType.And;
                        hastwocondition = true;
                    }
                    break;
                case ExcelComparisonOperator.Equal:
                    {
                        GridCondition1.ConditionType = GridConditionType.Equals;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                case ExcelComparisonOperator.Greater:
                    {
                        GridCondition1.ConditionType = GridConditionType.GreaterThan;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                case ExcelComparisonOperator.GreaterOrEqual:
                    {
                        GridCondition1.ConditionType = GridConditionType.GreaterThanOrEqual;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                case ExcelComparisonOperator.Less:
                    {
                        GridCondition1.ConditionType = GridConditionType.LessThan;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                case ExcelComparisonOperator.LessOrEqual:
                    {
                        GridCondition1.ConditionType = GridConditionType.LessThanOrEqual;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                case ExcelComparisonOperator.None:
                    return;
                //{
                //    cell.CellType = "FormulaCell";
                //    cell.Text = "=" + format.FirstFormula;
                //    GridCondition1.ConditionType = GridConditionType.Equals;
                //    GridCondition1.Value = true;
                //}
                //break;
                case ExcelComparisonOperator.NotBetween:
                    {
                        GridCondition1.ConditionType = GridConditionType.LessThan;
                        GridCondition2.ConditionType = GridConditionType.GreaterThan;
                        GridCondition1.Value = firstFormula;
                        GridCondition2.Value = secondFormula;
                        GridCondition1.PredicateType = Data.PredicateType.And;
                        GridCondition2.PredicateType = Data.PredicateType.Or;
                        hastwocondition = true;
                    }
                    break;
                case ExcelComparisonOperator.NotEqual:
                    {
                        GridCondition1.ConditionType = GridConditionType.NotEquals;
                        GridCondition1.Value = firstFormula;
                    }
                    break;
                default:
                    break;
            }

            GridConditionalFormat1.Conditions.Add(GridCondition1);
            if (hastwocondition)
                GridConditionalFormat1.Conditions.Add(GridCondition2);
        }

        private static void copyConditionFormatStyle(GridConditionalFormat GridConditionalFormat, IConditionalFormat format)
        {
#if EXCELGRID
            SpreadsheetGridStyleInfo style = new SpreadsheetGridStyleInfo();
#else
            GridStyleInfo style = new GridStyleInfo();
#endif

            if (format.IsBackgroundColorPresent)
            {
                Color bgColor = Color.FromArgb(format.BackColorRGB.A, format.BackColorRGB.R, format.BackColorRGB.G, format.BackColorRGB.B);
                style.Background = new SolidColorBrush(bgColor);
            }
            if (format.IsFontColorPresent)
            {
                Color fgColor = Color.FromArgb(format.FontColorRGB.A, format.FontColorRGB.R, format.FontColorRGB.G, format.FontColorRGB.B);
                style.Foreground = new SolidColorBrush(fgColor);
            }
            if (format.IsBold)
                style.Font.FontWeight = FontWeights.Bold;
            if (format.IsItalic)
                style.Font.FontStyle = FontStyles.Italic;
            if (format.IsBorderFormatPresent)
            {
                if (format.IsBottomBorderModified)
                    style.Borders.Bottom = getBorderPen(format.BottomBorderColorRGB, format.BottomBorderStyle);
                if (format.IsLeftBorderModified)
                    style.Borders.Left = getBorderPen(format.LeftBorderColorRGB, format.BottomBorderStyle);
                if (format.IsTopBorderModified)
                    style.Borders.Top = getBorderPen(format.TopBorderColorRGB, format.TopBorderStyle);
                if (format.IsRightBorderModified)
                    style.Borders.Right = getBorderPen(format.RightBorderColorRGB, format.TopBorderStyle);
            }
            GridConditionalFormat.Style = style;
        }

        private static RowColumnIndex GetRowColumnIndex(string formula)
        {
            //R[23]C[3]
            int RowIndex;
            int ColumnIndex;
            string row = FindNextIndex(formula);
            if (!string.IsNullOrEmpty(row) && int.TryParse(row, out RowIndex))
            {
                int closeBracket = formula.IndexOf(']');
                formula = formula.Substring(closeBracket + 1);
                string col = FindNextIndex(formula);
                if (!string.IsNullOrEmpty(col) && int.TryParse(col, out ColumnIndex))
                {
                    return new RowColumnIndex(RowIndex + 1, ColumnIndex + 1);
                }
            }
            return RowColumnIndex.Empty;
        }

        private static string FindNextIndex(string formula)
        {
            int openBracket = formula.IndexOf('[');
            if (openBracket > 0)
            {
                if (formula[openBracket - 1] == 'R' || formula[openBracket - 1] == 'C')
                {
                    int closeBracket = formula.IndexOf(']');
                    if (closeBracket > openBracket)
                        return formula.Substring(openBracket + 1, (closeBracket - openBracket) - 1);
                }
            }
            return string.Empty;
        }


        #endregion

        #region DataValidation

#if EXCELGRID
        private static void CopyDataValidation(SpreadsheetGridStyleInfo cell, IRange rangeToConvert, IWorksheet sheet)
#else
        private static void CopyDataValidation(GridStyleInfo cell, IRange rangeToConvert, IWorksheet sheet)
#endif
        {
            var dataValidation = rangeToConvert.DataValidation;

            cell.ShowDataValidationTooltip = dataValidation.ShowPromptBox;
            if (!string.IsNullOrEmpty(dataValidation.PromptBoxTitle) && !string.IsNullOrEmpty(dataValidation.PromptBoxText))
                cell.DataValidationTooltip = string.Format("{0} : {1}", dataValidation.PromptBoxTitle, dataValidation.PromptBoxText);
            if (!string.IsNullOrEmpty(dataValidation.ErrorBoxTitle))
                cell.ErrorAlertTitle = dataValidation.ErrorBoxTitle;
            if (!string.IsNullOrEmpty(dataValidation.ErrorBoxText))
                cell.ErrorAlertText = dataValidation.ErrorBoxText;
            //Changing the cell type formula in the particular cell was not calculated.
            if (dataValidation.AllowType == ExcelDataType.Integer)
            {
                //cell.CellType = "IntegerEdit";
                cell.IntegerEdit.IsScrollingOnCircle = false;
                Int64 firstValue = Int64.MinValue;
                Int64 secondValue = Int64.MaxValue;
                if (dataValidation.FirstFormula != string.Empty)
                    if (!dataValidation.FirstFormula.Contains('E'))
                        firstValue = Int64.Parse(dataValidation.FirstFormula);
                    else
                        firstValue = (Int64)double.Parse(dataValidation.FirstFormula);

                if (dataValidation.SecondFormula != null && dataValidation.SecondFormula != string.Empty)
                    if (!dataValidation.SecondFormula.Contains('E'))
                        secondValue = Int64.Parse(dataValidation.SecondFormula);
                    else
                        secondValue = (Int64)double.Parse(dataValidation.SecondFormula);

                cell.IntegerEdit.MinValidation = MinValidation.OnLostFocus;
                cell.IntegerEdit.MaxValidation = MaxValidation.OnLostFocus;
                if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Between)
                {
                    cell.IntegerEdit.MinValue = firstValue;
                    cell.IntegerEdit.MaxValue = secondValue;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotBetween)
                {
                    cell.IntegerEdit.MinValue = firstValue;
                    cell.IntegerEdit.MaxValue = secondValue;
                    cell.IntegerEdit.IsScrollingOnCircle = true;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual)
                {
                    cell.IntegerEdit.MinValue = firstValue;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Greater)
                {
                    cell.IntegerEdit.MinValue = firstValue + 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.LessOrEqual)
                {
                    cell.IntegerEdit.MaxValue = firstValue;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Less)
                {
                    cell.IntegerEdit.MaxValue = firstValue - 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Equal)
                {
                    cell.IntegerEdit.MinValue = firstValue;
                    cell.IntegerEdit.MaxValue = firstValue;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotEqual)
                {
                    cell.IntegerEdit.MinValue = firstValue;
                    cell.IntegerEdit.MaxValue = firstValue;
                    cell.IntegerEdit.IsScrollingOnCircle = true;
                }
            }
            else if (dataValidation.AllowType == ExcelDataType.Decimal)
            {
                //cell.CellType = "DoubleEdit";
                cell.DoubleEdit.IsScrollingOnCircle = false;
                cell.DoubleEdit.MinValidation = MinValidation.OnLostFocus;
                cell.DoubleEdit.MaxValidation = MaxValidation.OnLostFocus;
                if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Between)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula);
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.SecondFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotBetween)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula);
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.SecondFormula);
                    cell.DoubleEdit.IsScrollingOnCircle = true;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Greater)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula) + 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.LessOrEqual)
                {
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Less)
                {
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.FirstFormula) - 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Equal)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula);
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotEqual)
                {
                    cell.DoubleEdit.MinValue = double.Parse(dataValidation.FirstFormula);
                    cell.DoubleEdit.MaxValue = double.Parse(dataValidation.FirstFormula); 
                    cell.DoubleEdit.IsScrollingOnCircle = true;
                }
            }
            else if (dataValidation.AllowType == ExcelDataType.User)
            {
                string list = dataValidation.FirstFormula;
                if (list != null)
                {
                    cell.CellValue2 = list;
                    list = list.Replace('"', ' ');
                    list = list.Trim();
                    List<string> listitem = new List<string>();
                    if (sheet.Workbook.Names.Contains(list))
                    {
                        try
                        {
                            IName name = sheet.Workbook.Names[list];
                            var listCells = name.RefersToRange.Cells.ToList(); // sheet[name.Value].Cells.ToList();
                            foreach (var item in listCells)
                            {
                                if (item.DisplayText != "")
                                    listitem.Add(item.DisplayText);
                            }
                        }
                        catch (Exception)
                        {
                            if (list.Contains('\0'))
                                listitem = list.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            else if (list.Contains(','))
                                listitem = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                    else if (dataValidation.DataRange != null)
                    {
                        IRange[] rangeList = dataValidation.DataRange.Cells;
                        foreach (IRange range in rangeList)
                        {
                            if (!string.IsNullOrEmpty(range.DisplayText))
                                listitem.Add(range.DisplayText);
                        }
                    }
                    else
                    {
                        if (list.Contains('\0'))
                            listitem = list.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        else if (list.Contains(','))
                            listitem = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        else
                        {
                            if (!string.IsNullOrEmpty(list))
                                listitem.Add(list);
                        }
                        for (int index = 0; index < listitem.Count; index++)
                            listitem[index] = listitem[index].TrimStart();
                    }
                    cell.CellType = "ComboBox";
                    cell.ItemsSource = listitem;
                    var cellvalue = rangeToConvert.DisplayText.TrimStart();
                    cell.CellValue = cellvalue.TrimEnd();
                }
            }
            else if (dataValidation.AllowType == ExcelDataType.TextLength)
            {
                //cell.CellType = "MaskEdit";
                cell.MaskEdit.StringValidation = StringValidation.OnLostFocus;

                if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Between)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.SecondFormula);
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotBetween)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.SecondFormula);
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula);
                    cell.MaskEdit.Mask = "NotBetween";
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual)
                {
                    cell.MaskEdit.MaxLength = 200;
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Greater)
                {
                    cell.MaskEdit.MaxLength = 200;
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula) + 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.LessOrEqual)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Less)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.FirstFormula) - 1;
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Equal)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.FirstFormula);
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.NotEqual)
                {
                    cell.MaskEdit.MaxLength = int.Parse(dataValidation.FirstFormula);
                    cell.MaskEdit.MinLength = int.Parse(dataValidation.FirstFormula);
                    cell.MaskEdit.Mask = "NotEqual";
                }
            }
            else if (dataValidation.AllowType == ExcelDataType.Date || dataValidation.AllowType == ExcelDataType.Time)
            {
                cell.CellType = "DateTimeEdit";
                DateTime mindate, maxdate;
                DateTime.TryParse(dataValidation.FirstFormula, out mindate);
                cell.DateTimeEdit.NoneDateText = string.Empty;
                if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Between)
                {
                    cell.DateTimeEdit.MinDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                    DateTime.TryParse(dataValidation.SecondFormula, out maxdate);
                    cell.DateTimeEdit.MaxDateTime = maxdate;// DateTime.Parse(dataValidation.SecondFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual)
                {
                    cell.DateTimeEdit.MinDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Greater)
                {
                    cell.DateTimeEdit.MinDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.LessOrEqual)
                {
                    cell.DateTimeEdit.MaxDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Less)
                {
                    cell.DateTimeEdit.MaxDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                }
                else if (dataValidation.CompareOperator == ExcelDataValidationComparisonOperator.Equal)
                {
                    cell.DateTimeEdit.MinDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                    cell.DateTimeEdit.MaxDateTime = mindate;// DateTime.Parse(dataValidation.FirstFormula);
                }
            }
        }

        #endregion

        #region NumberFormat
        
#if SILVERLIGHT
        /// <summary>
        /// Gets the date time format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The DateTimePattern</returns>
        private static Tools.Controls.DateTimePattern GetDateTimeFormat(string format)
        {
            int start = 0;
            int end = 0;
            if (!format.Equals("General"))
            {
                while (format.Contains('['))
                {
                    start = format.IndexOf('[');
                    end = format.IndexOf(']');
                    format = format.Remove(start, (end - start) + 1);
                }
                if (format.Contains('d') && format.Contains('m') && format.Contains('y') && format.Contains('h') && format.Contains('s'))
                {
                    return Tools.Controls.DateTimePattern.FullDateTime;
                }
                else if (format.Contains('d') && format.Contains('m') && format.Contains('y'))
                {
                    return Tools.Controls.DateTimePattern.LongDate;
                }
                else if (format.Contains('h') && format.Contains('m') && format.Contains('s'))
                {
                    return Tools.Controls.DateTimePattern.LongTime;
                }
                else if (format.Contains('d') && format.Contains('m'))
                {
                    return Tools.Controls.DateTimePattern.MonthDay;
                }
                else if (format.Contains('m') && format.Contains('y'))
                {
                    return Tools.Controls.DateTimePattern.YearMonth;
                }
            }
            return Tools.Controls.DateTimePattern.ShortDate;
        }
#else
        /// <summary>
        /// Gets the date time format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The DateTimePattern</returns>
        private static Shared.DateTimePattern GetDateTimeFormat(string format)
        {
            int start = 0;
            int end = 0;
            if (!format.Equals("General"))
            {
                while (format.Contains('['))
                {
                    start = format.IndexOf('[');
                    end = format.IndexOf(']');
                    format = format.Remove(start, (end - start) + 1);
                }
                if (format.Contains('d') && format.Contains('m') && format.Contains('y') && format.Contains('h') && format.Contains('s'))
                {
                    return Shared.DateTimePattern.FullDateTime;
                }
                else if (format.Contains('d') && format.Contains('m') && format.Contains('y'))
                {
                    return Shared.DateTimePattern.LongDate;
                }
                else if (format.Contains('h') && format.Contains('m') && format.Contains('s'))
                {
                    return Shared.DateTimePattern.LongTime;
                }
                else if (format.Contains('d') && format.Contains('m'))
                {
                    return Shared.DateTimePattern.MonthDay;
                }
                else if (format.Contains('m') && format.Contains('y'))
                {
                    return Shared.DateTimePattern.YearMonth;
                }
            }
            return Shared.DateTimePattern.ShortDate;
        }
#endif
        /// <summary>
        /// convert the excel format into grid supports formats.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The format</returns>
        internal static string GetFormat(string format)
        {
            int start = 0;
            int end = 0;
            if (!format.Equals("General"))
            {
                format = format.Replace("\\", "");
                if (Regex.IsMatch(format, @"([mdMy][,][ mdMy])|([mdMy][-/][mdMy])|([mdMy][-/](.*)[-/][mdMy])|([Mdy][\s][Mdy])", RegexOptions.IgnorePatternWhitespace))
                {
                    format = format.Replace('m', 'M');
                    if (format.Contains(':'))
                    {
                        StringBuilder sb = new StringBuilder(format);
                        start = format.IndexOf(':') + 1;
                        while (start < format.Length && format[start] == 'M')
                        {
                            sb[start] = char.ToLower(sb[start]);
                            start++;
                        }
                        format = sb.ToString();
                    }
                }
                //To display the “AM/PM” format in our data time edit we need to set the format as “tt”
                if (format.Contains("AM/PM"))
                    format = format.Replace("AM/PM", "tt");
                format = format.Replace("_(", "");
                format = format.Replace("_)", " ");
                format = format.Replace("*", "");
                format = format.Replace("?", "");
                format = format.Replace("@", "");


                for (int j = 0; j < format.LastIndexOf(';'); j++)
                {
                    if ('_' == format[j] && format[j + 1] != ';')
                        format = format.Replace(format[j + 1].ToString(), " ");
                }
              
                format = format.Replace("_", " ");
                while (format.Contains('['))
                {
                    start = format.IndexOf('[');
                    end = format.IndexOf(']');
                    format = format.Remove(start, (end - start) + 1);
                }
                int i = 3;
                string cellformat = string.Empty;
                var temp = format.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (temp.Count() > i)
                {
                    foreach (var item in temp)
                    {
                        if (i == 0)
                            break;
                        i--;
                        cellformat += (item + ';');
                    }
                    format = cellformat;
                }
                if (temp.Count() == 2 && format.EndsWith(";"))
                {
                    return format+"\"\"";
                }
                if (format != ";;;")
                    format = format.TrimEnd(new char[] { ';' });
                return format;
            }
            return string.Empty;
        }

        #endregion

        #region Images
        
        /// <summary>
        /// Copies the image from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        public static void CopyImageToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyImageToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            IPictures images = sheet.Pictures;
            for (int i = 0; i < images.Count; i++)
            {
                BitmapShapeImpl image = images[i] as BitmapShapeImpl;
                System.Drawing.Image img = image.Picture;
                ShapeImpl shapeImpl = image as ShapeImpl;
                double width = 0, height = 0;
                for (int col = 1; col < shapeImpl.LeftColumn; col++)
                    width += sheet.GetColumnWidthInPixels(col);
                for (int row = 1; row < shapeImpl.TopRow; row++)
                    height += sheet.GetRowHeightInPixels(row);
                GraphicCellSpanInfo cellspan = new GraphicCellSpanInfo(shapeImpl.TopRow, shapeImpl.LeftColumn, shapeImpl.Width, shapeImpl.Height);
                cellspan.OffsetX = shapeImpl.Left - width;
                cellspan.OffsetY = shapeImpl.Top - height;
                cellspan.Name = image.Name;

                if (!grid.GraphicModel.GraphicCells.Contains(cellspan))
                {
                    grid.GraphicModel.GraphicCells.Add(cellspan);
                    int index = cellspan.CellSpanIndex;
                    var style = grid.GraphicModel[index];
                    style.CellType = "ImageCell";
                    style.CellName = cellspan.Name;
#if SILVERLIGHT
                Stream imageStream = new MemoryStream(img.ImageData);
                System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                bi.SetSource(imageStream);
#else
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
#endif
                    style.CellValue = bi;
                    if (shapeImpl.HasLineFormat)
                    {
                        style.BorderThickness = new Thickness(shapeImpl.Line.Weight);
                        Color bordercolor = Color.FromArgb(255,shapeImpl.Line.ForeColor.R, shapeImpl.Line.ForeColor.G, shapeImpl.Line.ForeColor.B);
                        style.BorderBrush = new SolidColorBrush(bordercolor);
                    }
                    else
                        style.BorderThickness = new Thickness(0);
                    if (grid.RowCount < shapeImpl.BottomRow)
                        grid.RowCount = shapeImpl.BottomRow;
                    if (grid.ColumnCount < shapeImpl.RightColumn)
                        grid.ColumnCount = shapeImpl.RightColumn;

                }
            }
        }

        #endregion

        #region Sparkline
        
        /// <summary>
        /// Copies the sparkline groups from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        public static void CopySparkLineToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopySparkLineToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            if (sheet.Workbook.Version != ExcelVersion.Excel2010) 
                return;

            int endRow, endColumn;
            ISparklineGroups spGroups = sheet.SparklineGroups;
            
            for (int i = 0; i < spGroups.Count; i++)
            {
                // get the sparklines count.
                for (int line = 0; line < spGroups[i].Count; line++)
                {
                    //ISparklineGroup spGroup = spGroups[i];
                    ISparklines spLines = spGroups[i][line];
                    for (int j = 0; j < spLines.Count; j++)
                    {
                        ISparkline spLine = spLines[j];
                        IRange dataRange = spLine.DataRange;
                        IRange referenceRange = spLine.ReferenceRange;

                        // Get reference range row index and column index.
                        endRow = referenceRange.Row + 1;
                        endColumn = referenceRange.Column + 1;

                        // Set the row count and column count so that sparkline will displayed 
                        // even if it is in last column.
                        if (grid.RowCount < endRow)
                            grid.RowCount = endRow;
                        if (grid.ColumnCount < endColumn)
                            grid.ColumnCount = endColumn;

                        grid[referenceRange.Row, referenceRange.Column].CellType = "SparkLineCell";
                        grid[referenceRange.Row, referenceRange.Column].CellValue = spGroups[i];

                    }
                }
            }
            
            

        }

        #endregion

        #region TextBox

#if EXCELGRID
        private static void CopyTextBoxToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyTextBoxToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            ITextBoxes textboxes = sheet.TextBoxes;
            for (int i = 0; i < textboxes.Count; i++)
			{
                TextBoxShapeImpl txtbox = textboxes[i] as TextBoxShapeImpl;
                ShapeImpl shapeImpl = txtbox as ShapeImpl;
                GraphicCellSpanInfo cellspan = new GraphicCellSpanInfo(shapeImpl.TopRow, shapeImpl.LeftColumn, shapeImpl.Width, shapeImpl.Height);
                double xPos = 0, yPos = 0;
                for (int col = 1; col < shapeImpl.LeftColumn; col++)
                    xPos += sheet.GetColumnWidthInPixels(col);
                for (int row = 1; row < shapeImpl.TopRow; row++)
                    yPos += sheet.GetRowHeightInPixels(row);

                cellspan.OffsetX = shapeImpl.Left - xPos;
                cellspan.OffsetY = shapeImpl.Top - yPos;
                cellspan.Name = shapeImpl.Name;
                grid.GraphicModel.GraphicCells.Add(cellspan);
                int index = grid.GraphicModel.GraphicCells.IndexOf(cellspan);
                var style = grid.GraphicModel[cellspan.CellSpanIndex];
                style.CellName = cellspan.Name;
                style.CellType = "RichTextBox";
#if !SILVERLIGHT
                style.CellValue = ConvertCellValueToParagraph(txtbox.RichText);
#else
                style.CellValue = RichTextBoxHelper.ConvertIRangeToParagraph(txtbox.RichText);
#endif
                if (shapeImpl.HasFill)
                {
                    Color fillcolor = Color.FromArgb(255, shapeImpl.Fill.ForeColor.R, shapeImpl.Fill.ForeColor.G, shapeImpl.Fill.ForeColor.B);
                    style.Background = new SolidColorBrush(fillcolor);
                }
                else
                {
                    Color transparent = Color.FromArgb(0, 255, 255, 255);
                    style.Background = new SolidColorBrush(transparent);
                }
                if (shapeImpl.HasLineFormat)
                {
                    style.BorderThickness = new Thickness(shapeImpl.Line.Weight);
                    Color bordercolor = Color.FromArgb(255,shapeImpl.Line.ForeColor.R, shapeImpl.Line.ForeColor.G, shapeImpl.Line.ForeColor.B);
                    style.BorderBrush = new SolidColorBrush(bordercolor);
                }
                else
                    style.BorderThickness = new Thickness(0);

                if (txtbox.HAlignment == ExcelCommentHAlign.Left)
                    style.HorizontalAlignment = HorizontalAlignment.Left;
                else if (txtbox.HAlignment == ExcelCommentHAlign.Right)
                    style.HorizontalAlignment = HorizontalAlignment.Right;
                else if (txtbox.HAlignment == ExcelCommentHAlign.Center)
                    style.HorizontalAlignment = HorizontalAlignment.Center;
                else
                    style.HorizontalAlignment = HorizontalAlignment.Stretch;

                if (txtbox.VAlignment == ExcelCommentVAlign.Bottom)
                    style.VerticalAlignment = VerticalAlignment.Bottom;
                else if (txtbox.VAlignment == ExcelCommentVAlign.Top)
                    style.VerticalAlignment = VerticalAlignment.Top;
                else if (txtbox.VAlignment == ExcelCommentVAlign.Center)
                    style.VerticalAlignment = VerticalAlignment.Center;
                else
                    style.VerticalAlignment = VerticalAlignment.Stretch;

                if (grid.RowCount < shapeImpl.BottomRow)
                    grid.RowCount = shapeImpl.BottomRow;
                if (grid.ColumnCount < shapeImpl.RightColumn)
                    grid.ColumnCount = shapeImpl.RightColumn;
			}
        }

        #endregion

        #region CheckBox
#if EXCELGRID
        private static void CopyCheckBoxToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyCheckBoxToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            ICheckBoxes checkboxes= sheet.CheckBoxes;
            for (int i = 0; i < checkboxes.Count; i++)
            {
                CheckBoxShapeImpl checkbox = checkboxes[i] as CheckBoxShapeImpl;
                ShapeImpl shapeImpl = checkbox as ShapeImpl;
                GraphicCellSpanInfo cellspan = new GraphicCellSpanInfo(shapeImpl.TopRow, shapeImpl.LeftColumn, shapeImpl.Width, shapeImpl.Height);
                double xPos = 0, yPos = 0;
                for (int col = 1; col < shapeImpl.LeftColumn; col++)
                    xPos += sheet.GetColumnWidthInPixels(col);
                for (int row = 1; row < shapeImpl.TopRow; row++)
                    yPos += sheet.GetRowHeightInPixels(row);

                cellspan.OffsetX = shapeImpl.Left - xPos;
                cellspan.OffsetY = shapeImpl.Top - yPos;
                cellspan.Name = checkbox.Name;
                grid.GraphicModel.GraphicCells.Add(cellspan);
                int index = grid.GraphicModel.GraphicCells.IndexOf(cellspan);
                var style = grid.GraphicModel[cellspan.CellSpanIndex];
                style.CellName = cellspan.Name;
                style.CellType = "CheckBox";
                
                switch(checkbox.CheckState)
                {
                    case ExcelCheckState.Checked:
                        style.CellValue = true;
                        break;
                    case ExcelCheckState.Unchecked:
                        style.CellValue = false;
                        break;
                    default:
                        style.CellValue = false;
                        break;
                }
                style.Text = checkbox.Text;
                if (shapeImpl.HasFill)
                {
                    Color fillcolor = Color.FromArgb(255, shapeImpl.Fill.ForeColor.R, shapeImpl.Fill.ForeColor.G, shapeImpl.Fill.ForeColor.B);
                    style.Background = new SolidColorBrush(fillcolor);
                }
                else
                {
                    Color transparent = Color.FromArgb(0, 255, 255, 255);
                    style.Background = new SolidColorBrush(transparent);
                }
                if (shapeImpl.HasLineFormat)
                {
                    style.BorderThickness = new Thickness(shapeImpl.Line.Weight);
                    Color bordercolor = Color.FromArgb(255,shapeImpl.Line.ForeColor.R, shapeImpl.Line.ForeColor.G, shapeImpl.Line.ForeColor.B);
                    style.BorderBrush = new SolidColorBrush(bordercolor);
                }
                else
                    style.BorderThickness = new Thickness(0);
            }
        }

        #endregion

        #region Chart

        private static void CopyChartToGrid(IWorksheet sheet, GridModel grid)
        {
            IChartShapes charts = sheet.Charts;
            for (int i = 0; i < charts.Count; i++)
            {
                IChartShape chartShape = charts[i];
                ShapeImpl shapeImpl = chartShape as ShapeImpl;
                GraphicCellSpanInfo cellspan = new GraphicCellSpanInfo(chartShape.TopRow, chartShape.LeftColumn, shapeImpl.Width, shapeImpl.Height);
                double xPos = 0, yPos = 0;
                for (int col = 1; col < shapeImpl.LeftColumn; col++)
                    xPos += sheet.GetColumnWidthInPixels(col);
                for (int row = 1; row < shapeImpl.TopRow; row++)
                    yPos += sheet.GetRowHeightInPixels(row);

                cellspan.OffsetX = shapeImpl.Left - xPos;
                cellspan.OffsetY = shapeImpl.Top - yPos;

                cellspan.Name = shapeImpl.Name;
                grid.GraphicModel.GraphicCells.Add(cellspan);
                var style = grid.GraphicModel[cellspan.RowIndex, cellspan.ColumnIndex];
                style.CellType = "Chart";
                style.CellValue = chartShape;
                if (grid.RowCount < chartShape.BottomRow)
                    grid.RowCount = chartShape.BottomRow;
                if (grid.ColumnCount < chartShape.RightColumn)
                    grid.ColumnCount = chartShape.RightColumn;
            }
        }

        private static void CopyChartToGrid(IChart chart, GridModel grid)
        {
            RowColumnIndex rowcol;
            if (chart.XPos != 0 && chart.YPos != 0)
                rowcol = grid.ActiveGridView.PointToCellRowColumnIndex(new Point(chart.XPos, chart.YPos));
            else
                rowcol = new RowColumnIndex(4, 2);
            GraphicCellSpanInfo cellspan = new GraphicCellSpanInfo(rowcol.RowIndex, rowcol.ColumnIndex, chart.Width, chart.Height);
            cellspan.Name = chart.Name;
            grid.GraphicModel.GraphicCells.Add(cellspan);
            var style = grid.GraphicModel[cellspan.CellSpanIndex];
            style.CellType = "Chart";
            style.CellValue = chart;
        }
        #endregion

        #region NamedRanges
        
        /// <summary>
        /// Copies the named range from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        private static void CopyNamesToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyNamesToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            foreach (IName item in sheet.Workbook.Names)
            {
                if (item.Value != null)
                    grid.FormulaEngine.AddNamedRange(item.Name, item.Value.Replace("'",""));
            }
        }

        #endregion

        #region Frozen Rows & Columns
        /// <summary>
        /// Copies the freeze panes from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        private static void CopyFreezePanesToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyFreezePanesToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            int frozenRows;
            int frozenColumns;
            frozenRows = sheet.HorizontalSplit + 1;
            frozenColumns = sheet.VerticalSplit + 1;

            if (frozenRows > 0)
                grid.FrozenRows = frozenRows;
            if (frozenColumns > 0)
                grid.FrozenColumns = frozenColumns;
        }
        #endregion

        #region RowHeights & ColumnWidths
        
        /// <summary>
        /// Copies row height information from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">Sheet to copy from.</param>
        /// <param name="grid">Grid to copy into.</param>
#if EXCELGRID
        private static void CopyRowHeightToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyRowHeightToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            IRange usedRange = sheet.Range;
            int last = usedRange.LastRow;
            for (int i = 1; i <= last; i++)
            {
                //grid.RowHeights[i] = sheet.GetRowHeightInPixels(i);
                var height = sheet.GetRowHeightInPixels(i);
                if (height != 0)
                {
                    grid.RowHeights[i] = sheet.GetRowHeightInPixels(i);
                }
                else
                {
                    grid.RowHeights.SetHidden(i, i, true);
                }

            }
        }

        /// <summary>
        /// Copies column width information from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">Sheet to copy from.</param>
        /// <param name="grid">Grid to copy into.</param>
#if EXCELGRID
        private static void CopyColumnWidthToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyColumnWidthToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            IRange usedRange = sheet.Range;

            for (int i = 1; i <= usedRange.LastColumn; i++)
            {
                //grid.ColumnWidths[i] = sheet.GetColumnWidthInPixels(i);
                var width = sheet.GetColumnWidthInPixels(i);
                if (width != 0)
                {
                    grid.ColumnWidths[i] = sheet.GetColumnWidthInPixels(i);
                }
                else
                {
                    grid.ColumnWidths.SetHidden(i, i, true);
                }
            }
        }
        #endregion

        #region Combox
        
        /// <summary>
        /// Copies the combox from excel worksheet into grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        private static void CopyComboxToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        public static void CopyComboxToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            IComboBoxes comboboxes = sheet.ComboBoxes;
            int rowIndex;
            int colIndex;
            for (int i = 0; i < comboboxes.Count; i++)
            {
                var combo = comboboxes[i];
                ShapeImpl shapeImpl = combo as ShapeImpl;
                rowIndex = shapeImpl.TopRow;
                colIndex = shapeImpl.LeftColumn;
                rowIndex = shapeImpl.TopRow;
                colIndex = shapeImpl.LeftColumn;
                if (combo.ListFillRange != null)
                {
                    var listCells = sheet[combo.ListFillRange.AddressLocal].Cells.ToList();
                    List<object> listitem = new List<object>();
                    foreach (var item in listCells)
                    {
                        listitem.Add(item.DisplayText);
                    }
                    grid[rowIndex, colIndex].CellType = "ComboBox";
                    grid[rowIndex, colIndex].ItemsSource = listitem;
                    grid[rowIndex, colIndex].CellValue = combo.SelectedValue;
                }
            }
        }
        #endregion

        #region CoveredCells
        
        /// <summary>
        /// Copies merged ranges into grid.
        /// </summary>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="grid">Source grid.</param>
#if EXCELGRID
        public static void CopyMergesToGrid(IWorksheet sheet, SpreadsheetGridModel grid)
#else
        private static void CopyMergesToGrid(IWorksheet sheet, GridModel grid)
#endif
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (sheet == null)
                throw new ArgumentNullException("sheet");
            GridCoveredCellInfoCollection coveredRanges = grid.CoveredCells;
            IRange[] arrMerges = sheet.MergedCells;
            if (arrMerges != null)
            {
                for (int i = 0, len = arrMerges.Length; i < len; i++)
                {
                    IRange curRange = arrMerges[i];
                    GridRangeInfo mergedRange;
                    mergedRange = GridRangeInfo.Cells(curRange.Row, curRange.Column, curRange.LastRow, curRange.LastColumn);

                    coveredRanges.Add(new CoveredCellInfo(mergedRange.Top, mergedRange.Left, mergedRange.Bottom, mergedRange.Right));
                }
            }
        }

        /// <summary>
        /// After v6.1 ExcelToGrid Convertion will check for Merged cells when importing borders. 
        /// When there is no merged cells this can be set to true to avoid cehcking for MergeCells on 
        /// importing borders from exccel cells.
        /// <para/>Default value is false.
        /// </summary>
        private static bool SkipMergeCellCheckOnSetBorders = false;

        /// <summary>
        /// Gets the merge cell range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="bottom">The Bottom.</param>
        /// <param name="right">The Right.</param>
        /// <returns></returns>
        private static bool GetMergeCellRange(IRange range, out int bottom, out int right)
        {
            bottom = right = -1;

            IWorksheet sheet = range.Worksheet;

            if (!SkipMergeCellCheckOnSetBorders && sheet.MergedCells != null)
            {
                foreach (IRange cell in sheet.MergedCells)
                {
                    if (cell.Row == range.Row && cell.Column == range.Column)
                    {
                        bottom = cell.LastRow;
                        right = cell.LastColumn;
                        return true;

                    }
                }
            }
            return false;
        }

        #endregion

        #region Copy Cell Style
        
        /// <summary>
        /// Sets grid cell style based on excel style.
        /// </summary>
        /// <param name="excelStyle">Source style.</param>
        /// <param name="cell">Destination cell.</param>
        private static void SetCellStyle(IRange range, GridStyleInfo cell, IWorksheet sheet)
        {
            if (range == null)
                throw new ArgumentNullException("range");

            if (cell == null)
                throw new ArgumentNullException("cell");

            if(sheet==null)
                throw new ArgumentNullException("sheet");

            if (!range.IsInitialized) return;

            bool istablecell = importTableStyleConverter.CheckForTableCell(range, cell, sheet);

            if (!istablecell || !range.HasStyle)
            {
                SetBrush(range.CellStyle, cell);
                SetBorders(range, cell, sheet);
            }
            SetAlignment(range.CellStyle, cell,sheet);
            SetFont(range.CellStyle, cell);
            if (range.CellStyle.Rotation != 0)
                SetOrientation(range.CellStyle, cell);
            if (range.CellStyle.ShrinkToFit)
                cell.TextWrapping = TextWrapping.Wrap;
#if !SILVERLIGHT
            //SetNumberFormat(range.CellStyle, cell);
#endif
        }

        #region Borders

#if SILVERLIGHT
        private static Pen getBorderPen(Color color, ExcelLineStyle excelLineStyle)
        {
            switch (excelLineStyle)
            {
                case ExcelLineStyle.Dash_dot:
                    return new Pen(new SolidColorBrush(color), 0.5, BorderStyle.DashDot);
                case ExcelLineStyle.Dash_dot_dot:
                    return new Pen(new SolidColorBrush(color), 0.5, BorderStyle.DashDotDot);
                case ExcelLineStyle.Dashed:
                    return new Pen(new SolidColorBrush(color), 0.5, BorderStyle.Dashed);
                case ExcelLineStyle.Dotted:
                case ExcelLineStyle.Hair:
                    return new Pen(new SolidColorBrush(color), 0.5, BorderStyle.Dotted);
                case ExcelLineStyle.Medium_dash_dot:
                    return new Pen(new SolidColorBrush(color), 1.0, BorderStyle.DashDot);
                case ExcelLineStyle.Medium_dash_dot_dot:
                case ExcelLineStyle.Slanted_dash_dot:
                    return new Pen(new SolidColorBrush(color), 1.0, BorderStyle.DashDotDot);
                case ExcelLineStyle.Medium_dashed:
                    return new Pen(new SolidColorBrush(color), 1.0, BorderStyle.Dashed);
                case ExcelLineStyle.None:
                    return new Pen(new SolidColorBrush(color), 0.0, BorderStyle.None);
                case ExcelLineStyle.Medium:
                    return new Pen(new SolidColorBrush(color), 1.0, BorderStyle.Standard);
                case ExcelLineStyle.Thin:
                case ExcelLineStyle.Double:
                    return new Pen(new SolidColorBrush(color), 1.5, BorderStyle.Standard);
                case ExcelLineStyle.Thick:
                default:
                    return new Pen(new SolidColorBrush(color), 0.5, BorderStyle.Standard);
            }
        }
#else
        private static Pen getBorderPen(System.Drawing.Color color, ExcelLineStyle excelLineStyle)
        {
            Color borderColor = Color.FromArgb(color.A, color.R, color.G, color.B);
            switch (excelLineStyle)
            {
                case ExcelLineStyle.None:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.00 };
                case ExcelLineStyle.Hair:
                case ExcelLineStyle.Dotted:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Dot };
                case ExcelLineStyle.Dashed:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Dash };
                case ExcelLineStyle.Dash_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.DashDot };
                case ExcelLineStyle.Dash_dot_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.DashDotDot };
                case ExcelLineStyle.Thin:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Solid };
                case ExcelLineStyle.Medium_dashed:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.Dash };
                case ExcelLineStyle.Slanted_dash_dot:
                case ExcelLineStyle.Medium_dash_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.DashDot };
                case ExcelLineStyle.Medium_dash_dot_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.DashDotDot };
                case ExcelLineStyle.Medium:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.Solid };
                case ExcelLineStyle.Thick:
                case ExcelLineStyle.Double:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 2.00, DashStyle = DashStyles.Solid };
                default:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.00 };
            }
        }
#endif
        /// <summary>
        /// Sets grid cell borders based on excel style.
        /// </summary>
        /// <param name="excelStyle">Source style.</param>
        /// <param name="cell">Destination cell.</param>
        private static void SetBorders(IRange range, GridStyleInfo cell, IWorksheet sheet)
        {
            if (range == null)
                throw new ArgumentNullException("range");

            if (cell == null)
                throw new ArgumentNullException("cell");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            int rowIndex = range.Row;
            int colIndex = range.Column;
            IRange checkRange = null;
            IBorder border = null;
            CellBordersInfo borders = cell.Borders;
            IBorders rangeBorders = range.CellStyle.Borders;
            IBorder BottomRangeBorderStyle = range.CellStyle.Borders[ExcelBordersIndex.EdgeBottom];
            IBorder TopRangeBorderStyle = range.CellStyle.Borders[ExcelBordersIndex.EdgeTop];
            IBorder LeftRangeBorderStyle = range.CellStyle.Borders[ExcelBordersIndex.EdgeLeft];
            IBorder RightRangeBorderStyle = range.CellStyle.Borders[ExcelBordersIndex.EdgeRight];
            if (rowIndex + 1 <= sheet.Range.LastRow)
            {
                checkRange = sheet[rowIndex + 1, colIndex];
            }
            if (checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0)
            {
                //Fix for the issue outside border having different color than excel.
                if (BottomRangeBorderStyle.LineStyle == checkRange.Borders[ExcelBordersIndex.EdgeTop].LineStyle && checkRange.Borders[ExcelBordersIndex.EdgeTop].ColorRGB != BottomRangeBorderStyle.ColorRGB)
                {
                    if (BottomRangeBorderStyle.Color == ExcelKnownColors.Black)
                        border = checkRange.Borders[ExcelBordersIndex.EdgeTop];
                    else
                        border = BottomRangeBorderStyle;
                    borders.Bottom = ConvertExcelBorder(border);
                }
                else
                {
                    borders.Bottom = ConvertExcelBorder(BottomRangeBorderStyle);
                }
            }
            //Fix for the issue - The border lines are visible even when the rows/columns are hidden.
            else if (range.RowHeight != 0 && range.ColumnWidth != 0 && BottomRangeBorderStyle.LineStyle != ExcelLineStyle.None)
            {
                borders.Bottom = ConvertExcelBorder(BottomRangeBorderStyle);
            }
            else
            {
                //Fix for the Gridline issue – to copy the last hidden row border
                var temprowindex = 2;
                while (rowIndex + temprowindex <= sheet.Range.LastRow)
                {
                    checkRange = sheet[rowIndex + temprowindex, colIndex];
                    if ((checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0))
                    {
                        borders.Bottom = ConvertExcelBorder(TopRangeBorderStyle);
                        break;
                    }
                    temprowindex++;
                }
            }

            if (colIndex + 1 <= sheet.Range.LastColumn)
            {
                checkRange = sheet[rowIndex, colIndex + 1];
            }
            if ((checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0))
            {
                //Fix for the issue outside border having different color than excel.
                if (RightRangeBorderStyle.LineStyle == checkRange.Borders[ExcelBordersIndex.EdgeLeft].LineStyle && checkRange.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB != RightRangeBorderStyle.ColorRGB)
                {
                    if (RightRangeBorderStyle.Color == ExcelKnownColors.Black)
                        border = checkRange.Borders[ExcelBordersIndex.EdgeLeft];
                    else
                        border = RightRangeBorderStyle;
                    borders.Right = ConvertExcelBorder(RightRangeBorderStyle);
                }
                else
                {
                    borders.Right = ConvertExcelBorder(RightRangeBorderStyle);
                }
            }
            else if (range.RowHeight != 0 && range.ColumnWidth != 0 && BottomRangeBorderStyle.LineStyle != ExcelLineStyle.None && RightRangeBorderStyle.LineStyle != ExcelLineStyle.None)
            {
                borders.Right = ConvertExcelBorder(RightRangeBorderStyle);
            }
            else if (range.ColumnWidth != 0 && (BottomRangeBorderStyle.LineStyle != ExcelLineStyle.None || TopRangeBorderStyle.LineStyle != ExcelLineStyle.None))
            {
                //Fix for the Gridline issue – to copy the last hidden column border
                var tempcolindex = 2;
                while (colIndex + tempcolindex <= sheet.Range.LastColumn)
                {
                    checkRange = sheet[rowIndex, colIndex + tempcolindex];
                    if ((checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0))
                    {
                        border = checkRange.Borders[ExcelBordersIndex.EdgeLeft];
                        borders.Right = ConvertExcelBorder(border);
                        break;
                    }
                    tempcolindex++;
                }
            }
            else
            {
#if SILVERLIGHT
                borders.Right = new Pen(new SolidColorBrush(range.CellStyle.Color), 0.50, BorderStyle.None);
#else
                Color borderColor = Color.FromArgb(range.CellStyle.Color.A, range.CellStyle.Color.R, range.CellStyle.Color.G, range.CellStyle.Color.B);
                borders.Top = new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.00 };
#endif
            }

            if (rowIndex - 1 > 0)
            {
                checkRange = sheet[rowIndex - 1, colIndex];
            }
            if (checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0)
            {
                if (TopRangeBorderStyle.LineStyle == checkRange.Borders[ExcelBordersIndex.EdgeBottom].LineStyle && checkRange.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB != TopRangeBorderStyle.ColorRGB)
                {
                    if (TopRangeBorderStyle.Color == ExcelKnownColors.Black)
                        border = checkRange.Borders[ExcelBordersIndex.EdgeBottom];
                    else
                        border = TopRangeBorderStyle;
                    borders.Top = ConvertExcelBorder(border);
                }
                else
                {
                    borders.Top = ConvertExcelBorder(TopRangeBorderStyle);
                }
            }
            //Fix for the issue - The border lines are visible even when the rows/columns are hidden.
            else if (range.ColumnWidth != 0 && range.RowHeight != 0 && RightRangeBorderStyle.LineStyle != ExcelLineStyle.None)
            {
                borders.Top = ConvertExcelBorder(TopRangeBorderStyle);
            }

            if (colIndex - 1 > 0)
            {
                checkRange = sheet[rowIndex, colIndex - 1];
            }
            if (checkRange != null && checkRange.ColumnWidth != 0 && checkRange.RowHeight != 0)
            {
                //Fix for the issue outside border having different color than excel.
                if (LeftRangeBorderStyle.LineStyle == checkRange.Borders[ExcelBordersIndex.EdgeRight].LineStyle && checkRange.Borders[ExcelBordersIndex.EdgeRight].ColorRGB != LeftRangeBorderStyle.ColorRGB)
                {
                    if (rangeBorders[ExcelBordersIndex.EdgeLeft].Color == ExcelKnownColors.Black)
                        border = checkRange.Borders[ExcelBordersIndex.EdgeRight];
                    else
                        border = LeftRangeBorderStyle;
                    borders.Left = ConvertExcelBorder(border);
                }
                else
                {
                    borders.Left = ConvertExcelBorder(LeftRangeBorderStyle);
                }
            }
            else if (range.ColumnWidth != 0 && range.RowHeight != 0 && BottomRangeBorderStyle.LineStyle != ExcelLineStyle.None)
            {
                borders.Left = ConvertExcelBorder(LeftRangeBorderStyle);
            }

            if (rowIndex == sheet.Range.LastRow)
            {
                borders.Bottom = ConvertExcelBorder(BottomRangeBorderStyle);
            }
            if (colIndex == sheet.Range.LastColumn)
            {
                borders.Right = ConvertExcelBorder(RightRangeBorderStyle);
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Converts excel border into grid border.
        /// </summary>
        /// <param name="border">Border to convert.</param>
        private static Pen ConvertExcelBorder(IBorder border)
        {
            if (border == null)
                throw new ArgumentNullException("border");

            Color borderColor = Color.FromArgb(border.ColorRGB.A, border.ColorRGB.R, border.ColorRGB.G, border.ColorRGB.B);
            switch (border.LineStyle)
            {
                case ExcelLineStyle.None:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.None);
                case ExcelLineStyle.Hair:
                case ExcelLineStyle.Dotted:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.Dotted);
                case ExcelLineStyle.Dashed:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.Dashed);
                case ExcelLineStyle.Dash_dot:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.DashDot);
                case ExcelLineStyle.Dash_dot_dot:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.DashDotDot);
                case ExcelLineStyle.Thin:
                    return new Pen(new SolidColorBrush(borderColor), 0.50, BorderStyle.Standard);
                case ExcelLineStyle.Medium_dashed:
                    return new Pen(new SolidColorBrush(borderColor), 1.00, BorderStyle.Dashed);
                case ExcelLineStyle.Slanted_dash_dot:
                case ExcelLineStyle.Medium_dash_dot:
                    return new Pen(new SolidColorBrush(borderColor), 1.00, BorderStyle.DashDot);
                case ExcelLineStyle.Medium_dash_dot_dot:
                    return new Pen(new SolidColorBrush(borderColor), 1.00, BorderStyle.DashDotDot);
                case ExcelLineStyle.Medium:
                    return new Pen(new SolidColorBrush(borderColor), 1.00, BorderStyle.Standard);
                case ExcelLineStyle.Thick:
                case ExcelLineStyle.Double:
                    return new Pen(new SolidColorBrush(borderColor), 2.00, BorderStyle.Standard);
                default:
                    return new Pen(new SolidColorBrush(borderColor), 0.00, BorderStyle.None);
            }
        }
#else
        /// <summary>
        /// Converts excel border into grid border.
        /// </summary>
        /// <param name="border">Border to convert.</param>
        private static Pen ConvertExcelBorder(IBorder border)
        {
            if (border == null)
                throw new ArgumentNullException("border");

            Color borderColor = Color.FromArgb(255, border.ColorRGB.R, border.ColorRGB.G, border.ColorRGB.B);

            switch (border.LineStyle)
            {
                case ExcelLineStyle.None:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.00 };
                case ExcelLineStyle.Hair:
                case ExcelLineStyle.Dotted:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Dot };
                case ExcelLineStyle.Dashed:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Dash };
                case ExcelLineStyle.Dash_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.DashDot };
                case ExcelLineStyle.Dash_dot_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.DashDotDot };
                case ExcelLineStyle.Thin:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.50, DashStyle = DashStyles.Solid };
                case ExcelLineStyle.Medium_dashed:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.Dash };
                case ExcelLineStyle.Slanted_dash_dot:
                case ExcelLineStyle.Medium_dash_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.DashDot };
                case ExcelLineStyle.Medium_dash_dot_dot:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.DashDotDot };
                case ExcelLineStyle.Medium:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 1.00, DashStyle = DashStyles.Solid };
                case ExcelLineStyle.Thick:
                case ExcelLineStyle.Double:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 2.00, DashStyle = DashStyles.Solid };
                default:
                    return new Pen() { Brush = new SolidColorBrush(borderColor), Thickness = 0.00 };
            }
        }
#endif

        #endregion

        /// <summary>
        /// Sets cell's brush.
        /// </summary>
        /// <param name="excelStyle">Excel style containing information about brush.</param>
        /// <param name="cell">Style info to set brush in.</param>
        private static void SetBrush(IStyle excelStyle, GridStyleInfo cell)
        {
            if (excelStyle == null)
                throw new ArgumentNullException("excelStyle");

            if (cell == null)
                throw new ArgumentNullException("cell");

            //if (excelStyle.FillPattern == ExcelPattern.None) return;

            Color bcolor = Color.FromArgb(255, excelStyle.Color.R, excelStyle.Color.G, excelStyle.Color.B);
            cell.Background = new SolidColorBrush(bcolor);
            IFont excelFont = excelStyle.Font;
            Color fcolor = Color.FromArgb(255, excelFont.RGBColor.R, excelFont.RGBColor.G, excelFont.RGBColor.B);
            cell.Foreground = new SolidColorBrush(fcolor);
        }

        /// <summary>
        /// Sets alignment of the GridStyleInfo.
        /// </summary>
        /// <param name="excelStyle">Excel style to get alignment block from.</param>
        /// <param name="cell">GridStyleInfo to set alignment block in.</param>
        private static void SetAlignment(IStyle excelStyle, GridStyleInfo cell,IWorksheet sheet)
        {
            if (excelStyle == null)
                throw new ArgumentNullException("excelStyle");

            if (cell == null)
                throw new ArgumentNullException("cell");

            ExcelHAlign hAlign = excelStyle.HorizontalAlignment;
            switch (hAlign)
            {
                case ExcelHAlign.HAlignLeft:
                    //Added the below code to set the Left Alignment. Internal Issue: Left Alignment not set for Formula Cells and DateTime cells.
                    cell.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    break;
                case ExcelHAlign.HAlignGeneral:
                    //cell.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    break;
                case ExcelHAlign.HAlignRight:
                    cell.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    break;
                default:
                    cell.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    break;
            }

            ExcelVAlign vAlign = excelStyle.VerticalAlignment;
            switch (vAlign)
            {
                case ExcelVAlign.VAlignBottom:
                    cell.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    break;
                case ExcelVAlign.VAlignTop:
                    cell.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    break;

                default:
                    cell.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    break;
            }

            //cell.WrapText = excelStyle.WrapText;
            if (excelStyle.WrapText)
            {
                cell.TextWrapping = TextWrapping.Wrap;
                cell.TextTrimming = TextTrimming.None;
            }

            int left = excelStyle.IndentLevel;
            if (left > 0)
                cell.TextMargins = new CellMarginsInfo(10*left, 0, 0, 0);
        }

        /// <summary>
        /// Copies orientation settings from excel style into grid cell.
        /// </summary>
        /// <param name="excelStyle">Source cell style.</param>
        /// <param name="cell">Destination grid cell.</param>
        private static void SetOrientation(IStyle excelStyle, GridStyleInfo cell)
        {
            if (excelStyle == null)
                throw new ArgumentNullException("excelStyle");

            if (cell == null)
                throw new ArgumentNullException("cell");

            int iAngle = excelStyle.Rotation;

            if (iAngle != 0)
            {
                if (iAngle > 90 && iAngle <= 180)
                {
                    iAngle -= 90;
                }
                else if (iAngle > 0 && iAngle <= 90)
                {
                    iAngle = 360 - iAngle;
                }
                else
                {
                    iAngle = 0;
                }

                cell.Font.Orientation = iAngle;
            }
        }

        /// <summary>
        /// Sets font for the cell based on Excel style.
        /// </summary>
        /// <param name="excelStyle">Excel style to get font setting from.</param>
        /// <param name="cell">Cell style to set font.</param>
        private static void SetFont(IStyle excelStyle, GridStyleInfo cell)
        {
            if (excelStyle == null)
                throw new ArgumentNullException("excelStyle");

            if (cell == null)
                throw new ArgumentNullException("cell");

            IFont excelFont = excelStyle.Font;
            GridFontInfo gridFont = cell.Font;
            if (excelFont.Italic)
                gridFont.FontStyle = FontStyles.Italic;
            if (excelFont.Bold)
                gridFont.FontWeight = FontWeights.Bold;

            if (excelFont.Underline == ExcelUnderline.Single)
                gridFont.TextDecorations = TextDecorations.Underline;
#if !SILVERLIGHT
            else if (excelFont.Underline == ExcelUnderline.Double)
            {
                TextDecoration myUnderline = new TextDecoration();
                Pen myPen = new Pen();
                myPen.Brush = new SolidColorBrush(Colors.Black);
                myPen.Thickness = 1;
                myUnderline.Pen = myPen;
                myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                myUnderline.Location = TextDecorationLocation.Baseline;
                TextDecoration myUnderline1 = new TextDecoration();
                Pen myPen1 = new Pen();
                myPen1.Brush = new SolidColorBrush(Colors.Black);
                myPen1.Thickness = 1;
                myUnderline1.Pen = myPen1;
                myUnderline1.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                myUnderline1.Location = TextDecorationLocation.Underline;
                myUnderline1.PenOffset = 1;
                if (gridFont.TextDecorations != null)
                {
                    gridFont.TextDecorations.Add(myUnderline);
                    gridFont.TextDecorations.Add(myUnderline1);
                }
                else
                {
                    gridFont.TextDecorations = new TextDecorationCollection() { myUnderline, myUnderline1 };
                }
            }
            if (excelFont.Strikethrough)
            {
                if (gridFont.TextDecorations != null)
                    gridFont.TextDecorations.Add(TextDecorations.Strikethrough);
                else
                    gridFont.TextDecorations = TextDecorations.Strikethrough;
            }
#endif
            gridFont.FontFamily = new System.Windows.Media.FontFamily(excelFont.FontName.ToString());
            gridFont.FontSize = Math.Round(excelFont.Size * 1.2);
            gridFont.Orientation = excelStyle.Rotation;
        }
        #endregion

        #region Hyperlinks

        /// <summary>
        /// Copies the hyperlink to virtual grid.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="book">The WorkBook</param>
        /// <param name="grid">The grid.</param>
#if EXCELGRID
        private static void CopyHyperlinkToVirtualGrid(IWorksheet sheet, IWorkbook book, SpreadsheetGridModel grid)
#else
        private static void CopyHyperlinkToVirtualGrid(IWorksheet sheet,IWorkbook book, GridModel grid)
#endif
        {
            IHyperLinks hyperlinks = sheet.HyperLinks;
            for (int i = 0; i < hyperlinks.Count; i++)
            {
                IHyperLink hylink = hyperlinks[i];
                string url = string.Empty;
                switch (hylink.Type)
                {
                    case ExcelHyperLinkType.Url:
                        url = hylink.Address;
                        break;
                    case ExcelHyperLinkType.Workbook:
                        {
                            url = hylink.Address;
                            if (url.Contains("!"))
                            {
                                string sheetname = url.Substring(0, url.IndexOf('!'));
                                sheetname = sheetname.Replace("'", "");
                                string strcell = url.Substring(url.IndexOf('!') + 1);
                                try
                                {
                                    url = book.Worksheets[sheetname].Range[strcell].AddressGlobal;
                                }
                                catch (Exception e)
                                {
                                    url = sheetname;
                                }

                            }
                            else if (sheet.Names.Contains(url))
                            {
                                IName name = sheet.Names[url];
                                var listCells = sheet[name.Value].Cells.ToList();
                                if (listCells.Count > 0)
                                    url = listCells[0].Address;
                            }
                            else if (sheet.Workbook.Names.Contains(url))
                            {
                                IName name = sheet.Workbook.Names[url]; ;
                                var listCells = sheet[name.Value].Cells.ToList();
                                if (listCells.Count > 0)
                                    url = listCells[0].Address;
                            }
                            url = url.Replace("'", "");
                            url = url.Replace("$", "");
                        }
                        break;
                    default:
                        break;
                }
                IRange range = hylink.Range;
                GridStyleInfo cell = grid[range.Row, range.Column];

                cell.CellType = "Hyperlink";
                cell.CellValue = hylink.TextToDisplay;
                cell.CellValue2 = url;
                cell.ReadOnly = sheet.IsPasswordProtected && range.CellStyle.Locked;
                SetCellStyle(range, cell, sheet);
            }
        }

        /// <summary>
        /// Copies the hyperlink to normal grid.
        /// </summary>
        /// <param name="excelrange">The excelrange.</param>
        /// <param name="cell">The cell.</param>
#if EXCELGRID
        public static void CopyHyperlinkToCell(IWorkbook book, IWorksheet sheet, IRange excelrange, GridStyleInfo cell)
#else
        internal static void CopyHyperlinkToCell(IWorksheet sheet, IRange excelrange, GridStyleInfo cell)
#endif
        {
            if (excelrange.Hyperlinks.Count > 0)
            {
                IHyperLink hylink = excelrange.Hyperlinks[excelrange.Hyperlinks.Count - 1];
                string url = string.Empty;
                switch (hylink.Type)
                {
                    case ExcelHyperLinkType.Url:
                        url = hylink.Address;
                        break;
                    case ExcelHyperLinkType.Workbook:
                        {
                            url = hylink.Address;
#if EXCELGRID
                            if (url.Contains("!"))
                            {
                                string sheetname = url.Substring(0, url.IndexOf('!'));
                                sheetname = sheetname.Replace("'", "");
                                string strcell = url.Substring(url.IndexOf('!') + 1);
                                url = book.Worksheets[sheetname].Range[strcell].AddressGlobal;
                            }
                            else if (sheet.Names.Contains(url))
                            {
                                IName name = sheet.Names[url];
                                var listCells = sheet[name.Value].Cells.ToList();
                                if (listCells.Count > 0)
                                    url = listCells[0].Address;
                            }
                            else if (sheet.Workbook.Names.Contains(url))
                            {
                                IName name = sheet.Workbook.Names[url]; ;
                                var listCells = sheet[name.Value].Cells.ToList();
                                if (listCells.Count > 0)
                                    url = listCells[0].Address;
                            }
#endif
                            url = url.Replace("'", "");
                            url = url.Replace("$", "");
                        }
                        break;
                    default:
                        break;
                }
                cell.CellType = "Hyperlink";
                cell.CellValue = hylink.TextToDisplay;
                cell.ReadOnly = sheet.IsPasswordProtected && excelrange.CellStyle.Locked;
                cell.CellValue2 = url;
            }
        }
        #endregion

        #region RichText
#if !SILVERLIGHT

        public static bool IsFormatSame(IFont char1, IFont char2)
        {
            bool canappend = true;
            if (char1.Bold != char2.Bold)
            {
                canappend = false;
            }
            if (char1.Color != char2.Color)
            {
                canappend = false;
            }          
            if (char1.Italic != char2.Italic)
            {
                canappend = false;
            }           
            if (char1.RGBColor != char2.RGBColor)
            {
                canappend = false;
            }
            if (char1.Size != char2.Size)
            {
                canappend = false;
            }
            if (char1.Strikethrough != char2.Strikethrough)
            {
                canappend = false;
            }
            if (char1.Subscript != char2.Superscript)
            {
                canappend = false;
            }
            if (char1.Underline != char2.Underline)
                {
                canappend = false;
                }
            if (char1.Underline != char2.Underline)
            {
                canappend = false;
            }
            if (char1.VerticalAlignment != char2.VerticalAlignment)
            {
                canappend = false;
            }
            return canappend;

        }

        public static Run AppendFormat(IFont charfont, Run run)
        {
            if (charfont.Bold)
            {
                run.FontWeight = FontWeights.Bold;
            }
            if (charfont.Italic)
            {
                run.FontStyle = FontStyles.Italic;
            }
            if (charfont.Strikethrough)
            {
                run.TextDecorations = TextDecorations.Strikethrough;
            }
            if (charfont.Underline != ExcelUnderline.None)
                {
                run.TextDecorations = TextDecorations.Underline;
                }
            if (charfont.Underline == ExcelUnderline.Single || charfont.Underline == ExcelUnderline.Double)
            {
                run.TextDecorations = TextDecorations.Underline;
            }
            if (charfont.Strikethrough)
            {
                run.TextDecorations = TextDecorations.Strikethrough;
            }
            if (charfont.Subscript)
            {
                run.BaselineAlignment = BaselineAlignment.Subscript;
            }
            if (charfont.Superscript)
            {
                run.BaselineAlignment = BaselineAlignment.Superscript;
            }
            run.FontFamily = new FontFamily(charfont.FontName);
            run.Foreground = new SolidColorBrush(Color.FromArgb(255, charfont.RGBColor.R, charfont.RGBColor.G, charfont.RGBColor.B));
            run.FontSize = charfont.Size * 1.2;
            return run;

        }

        public static FlowDocument ConvertCellValueToParagraph(IRichTextString RichText)
        {
            FlowDocument Flowdocument = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            int TextLength = RichText.Text.Length;
            bool IsLastChar = false;
           // int NoofPara = range.RichText.Text.Split('\n').Length;            
            StringBuilder sb = new StringBuilder();
            bool startchar = true;
            for (int i = 0; i < TextLength-1; i++)
            {
                if (startchar)
                {
                    sb.Append(RichText.Text[i]);                 
                }
                if (i == TextLength - 2)
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                    IsLastChar = true;
                }
                if ((IsFormatSame(RichText.GetFont(i), RichText.GetFont(i + 1)) && !IsLastChar && RichText.Text[i + 1].ToString() != "\n") || (RichText.Text[i + 1].ToString() == " " && !IsLastChar))              
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                }
                else
                {
                    Run run = new Run(sb.ToString());
                    paragraph.Inlines.Add(AppendFormat(RichText.GetFont(RichText.Text.IndexOf(run.Text)), run));
                    startchar = true;
#if SyncfusionFramework3_5 && !SyncfusionFramework4_0
                    sb.Remove(0, sb.Length);
#else
                    sb.Clear();
#endif

                    if (RichText.Text[i + 1].ToString() == "\n" || IsLastChar)
                    {
                        Flowdocument.Blocks.Add(paragraph);
                        paragraph = new Paragraph();
                        startchar = true;
#if SyncfusionFramework3_5 && !SyncfusionFramework4_0
                    sb.Remove(0, sb.Length);
#else
                        sb.Clear();
#endif
                        i++;
                        IsLastChar = false;
                    }
                }
            }
            return Flowdocument;
        }

#else
        /// <summary>
        /// IsFormatSame() method compares <param name="char1"/> and <param name="char2"/> font style of the characters. 
        /// </summary>
        public static bool IsFormatSame(IFont char1, IFont char2)
        {
            bool canappend = true;
            if (char1.Bold != char2.Bold)
            {
                canappend = false;
            }
            if (char1.Color != char2.Color)
            {
                canappend = false;
            }
            if (char1.Italic != char2.Italic)
            {
                canappend = false;
            }
            if (char1.RGBColor != char2.RGBColor)
            {
                canappend = false;
            }
            if (char1.Size != char2.Size)
            {
                canappend = false;
            }
            if (char1.Underline != char2.Underline)
            {
                canappend = false;
            }
            if (char1.VerticalAlignment != char2.VerticalAlignment)
            {
                canappend = false;
            }
            return canappend;

        }
        /// <summary>
        ///  AppendFormat() method Returns <param name="run"/> append the next character when both have same font format. 
        /// </summary>
        public static Run AppendFormat(IFont charfont, Run run, bool isnewline)
        {
            if (charfont.Bold)
            {
                run.FontWeight = FontWeights.Bold;
            }
            if (charfont.Italic)
            {
                run.FontStyle = FontStyles.Italic;
            }
            if (charfont.Underline == ExcelUnderline.Single || charfont.Underline == ExcelUnderline.Double)
            {
                run.TextDecorations = TextDecorations.Underline;
            }
            run.FontFamily = new FontFamily(charfont.FontName);
            run.Foreground = new SolidColorBrush(Color.FromArgb(255, charfont.RGBColor.R, charfont.RGBColor.G, charfont.RGBColor.B));
            run.FontSize = charfont.Size * 1.2;
            if (isnewline)
                run.Text = run.Text + "\n";
            return run;

        }
        /// <summary>
        ///  ConvertCellValueToParagraph() returns <param name="Paragraph"/> for the cellvalue. 
        /// </summary>
        public static Paragraph ConvertCellValueToParagraph(IRichTextString RichText)
        {
            Paragraph paragraph = new Paragraph();
            int TextLength = RichText.Text.Length;
            bool IsLastChar = false;
            bool IsNewLine = false;
            StringBuilder sb = new StringBuilder();
            bool startchar = true;
            for (int i = 0; i < TextLength - 1; i++)
            {
                if (startchar)
                {
                    sb.Append(RichText.Text[i]);
                }
                if (i == TextLength - 2)
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                    IsLastChar = true;
                }
                if ((IsFormatSame(RichText.GetFont(i), RichText.GetFont(i + 1)) && !IsLastChar && RichText.Text[i + 1].ToString() != "\n"))
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                }
                else
                {
                    if (RichText.Text[i + 1].ToString() == "\n")
                    {
                        IsNewLine = true;
                    }
                    else
                        IsNewLine = false;

                    Run run = new Run();
                    run.Text = sb.ToString();
                    paragraph.Inlines.Add(AppendFormat(RichText.GetFont(RichText.Text.IndexOf(run.Text)), run, IsNewLine));
                    startchar = true;
                    sb.Clear();
                    if (RichText.Text[i + 1].ToString() == "\n" || IsLastChar)
                    {
                        startchar = true;
                        sb.Clear();
                        i++;
                        IsLastChar = false;
                    }
                }
            }
            return paragraph;
        }
#endif

        #endregion

        #region Center Across Selection
#if EXCELGRID
        internal static void CopyCenterAcrossSelection(GridModel gridModel, IWorksheet sheet, GridStyleInfo Style)
        {
            if (gridModel == null)
                return;
            int row = Style.RowIndex;
            int col = Style.ColumnIndex;
            var checkforCoveredCell = gridModel.CoveredCells.GetCoveredCell(row, col);
            if (checkforCoveredCell == null)
            {
                int column = col + 1;
                while (true)
                {
                    if (sheet[row, column].HorizontalAlignment == ExcelHAlign.HAlignCenterAcrossSelection
                        && (string.IsNullOrEmpty(sheet[row, column].DisplayText)))
                    {
                        //Cells having center across selection
                    }
                    else
                    {
                        if (col != column - 1)
                        {
                            CoveredCellInfo coveredcell = new CoveredCellInfo(row, col, row, column - 1);
                            gridModel.CoveredCells.Add(coveredcell);
                            col = column - 1;
                            break;
                        }
                        else
                            break;
                    }
                    column++;
                }
                if (gridModel.Views != null)
                {
                    foreach (var item in gridModel.Views)
                    {
                        VirtualizingCellsControl.ReArrangeCoveredCells(item);
                    }
                }
                Style.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
#endif
        #endregion
    }
}
