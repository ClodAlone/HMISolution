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
    using Syncfusion.Windows.Data;
    using System.Drawing;
    
#if SILVERLIGHT
    using System.Windows.Controls;
    using System.IO;
    using Syncfusion.XlsIO;
#endif

    /// <summary>
    /// Extensions class for GridDataControl
    /// </summary>   
    public static class GridDataControlExportExtensions
    {
        static List<string> excludeColumns;

#if !SILVERLIGHT

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion)
        {
            ExportToExcel(gridDataControl, fileName, excelVersion, null, null, false);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, bool useExcelExpanderAndIndentColumn)
        {
            ExportToExcel(gridDataControl, fileName, excelVersion, null, null, useExcelExpanderAndIndentColumn);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="modelExportingHandler">The model exporting handler.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, bool useExcelExpanderAndIndentColumn, GridModelExportToExcelHandler modelExportingHandler)
        {
            ExportToExcel(gridDataControl, fileName, excelVersion, null, modelExportingHandler, useExcelExpanderAndIndentColumn);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="_excludedColumns">The _excluded columns.</param>
        /// <param name="IsExpanderColumnExported">if set to <c>true</c> [is expander column exported].</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, List<string> excludedColumns, bool useExcelExpanderAndIndentColumn)
        {           
            excludeColumns = excludedColumns;
            ExportToExcel(gridDataControl, fileName, excelVersion, useExcelExpanderAndIndentColumn);
            if (excludeColumns != null)      // cleared Excluded colums after exporting, to avoid excluding column while exporting with SelectedRanges
                if (excludeColumns.Count > 0)
                    excludeColumns.Clear();
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [hide expander and indent].</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler, bool useExcelExpanderAndIndentColumn)
        {
            ExportToExcel(gridDataControl, fileName, excelVersion, exportingHandler, null, useExcelExpanderAndIndentColumn);
        }


        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler)
        {
            ExportToExcel(gridDataControl, fileName, excelVersion, exportingHandler, null, false);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        /// <param name="modelExportingHandler">The model exporting handler.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [hide expander and indent].</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler , GridModelExportToExcelHandler modelExportingHandler , bool useExcelExpanderAndIndentColumn)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;         
            GridRangeInfo range = GridRangeInfo.Cells(0, 0, gridDataControl.Model.RowCount - 1, gridDataControl.Model.ColumnCount - 1);
            ExportToExcel(gridDataControl, range, workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1], exportingHandler, modelExportingHandler, useExcelExpanderAndIndentColumn);
            workbook.SaveAs(fileName);
            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, string fileName, ExcelVersion excelVersion, GridCellExportToExcelHandler exportingHandler)
        {            
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
            workbook.Version = excelVersion;
            IWorksheet workSheet = workbook.Worksheets[0];

            GridRangeInfo headerRange;
            GridRangeInfoList rangeList = new GridRangeInfoList();
            if (gridRange.RangeType == GridRangeInfoType.Table)
            {               
                gridRange = GridRangeInfo.Cells(0, 0, gridDataControl.Model.RowCount - 1,
                    gridDataControl.Model.ColumnCount - 1);

                rangeList.Add(gridRange);
            }
            else
            {
                gridRange = GetExpandedRange(gridRange, gridDataControl.Model);
                headerRange = GridRangeInfo.Cells(0, gridRange.Left, gridDataControl.Model.HeaderRows - 1, gridRange.Right);
                int firstRecordRow = gridRange.Top < gridDataControl.Model.HeaderRows
                    ? gridDataControl.Model.HeaderRows
                    : gridRange.Top;
                gridRange = GridRangeInfo.Cells(firstRecordRow, gridRange.Left, gridRange.Bottom, gridRange.Right);

                rangeList.Add(headerRange);
                rangeList.Add(gridRange);
            }
                        
            int x = 1, y = 1;
            foreach (GridRangeInfo range in rangeList)
            {
                ExportToExcel(gridDataControl, range, workSheet, workSheet.Range[x, y], exportingHandler, false);
                x += gridDataControl.Model.HeaderRows;
            }

            workbook.SaveAs(fileName);
            excelEngine.ThrowNotSavedOnDestroy = false;
            excelEngine.Dispose();
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="excelVersion">The excel version.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, string fileName, ExcelVersion excelVersion)
        {
            ExportToExcel(gridDataControl, gridRange, fileName, excelVersion, null);
        }

#else
        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="excelVersion">The excel version.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, ExcelVersion excelVersion)
        {
            ExportToExcel(gridDataControl, gridRange, excelVersion, false);
        }

        /// <summary>
        /// Extension method for GridDataControl
        /// </summary>
        /// <param name="gridDataControl">GridDataControl Object</param>
        /// <param name="gridRange">Selected Range in GridDataControl</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, ExcelVersion excelVersion, bool useExcelExpanderAndIndentColumn)
        {

            string xlVersion = GridDataControlExportExtensions.GetVersion(excelVersion);
            SaveFileDialog sfdone = new SaveFileDialog()
            {
                DefaultExt = xlVersion,
                FilterIndex = 1,
                Filter = "(." + xlVersion + ")|*." + xlVersion
            };
            if (sfdone.ShowDialog() == true)
            {
                using (Stream stream = sfdone.OpenFile())
                {
                    gridRange = gridRange.ExpandRange(gridRange.Top, gridRange.Left, gridRange.Top, gridDataControl.Model.ColumnCount - 1);
                    ExcelEngine excelEngine = new ExcelEngine();
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
                    workbook.Version = excelVersion;
                    GridRangeInfoList rangeList = new GridRangeInfoList();
                    int top = 0, left = 0, right = 0, bottom = 0;
                    left = gridDataControl.ShowRowHeader ? 1 : 0;
                    bottom = (gridDataControl.StackedHeaderRows.Count > 0) ? gridDataControl.StackedHeaderRows.Count : 0;
                    right = gridDataControl.Model.ColumnCount - 1;
                    rangeList.Add(GridRangeInfo.Cells(top, gridRange.Left, bottom, gridRange.Right - 1));
                    rangeList.Add(GridRangeInfo.Cells(gridRange.Top, gridRange.Left, gridRange.Bottom, gridRange.Right - 1));
                    int x = 1, y = 1;
                    if (rangeList.Count > 0)
                    {
                        foreach (GridRangeInfo range in rangeList)
                        {
                            ExportToExcel(gridDataControl, range, workbook.Worksheets[0], workbook.Worksheets[0].Range[x, y], null, null, useExcelExpanderAndIndentColumn);
                            x = bottom + 2;
                        }
                    }

                    workbook.SaveAs(stream);
                    excelEngine.ThrowNotSavedOnDestroy = false;
                    excelEngine.Dispose();
                }
            }
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="excelVersion">The excel version.</param>
        /// <param name="excludedColumns">The excluded columns.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [use excel expander and indent column].</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, ExcelVersion excelVersion, List<string> excludedColumns, bool useExcelExpanderAndIndentColumn)
        {
            excludeColumns = excludedColumns;
            ExportToExcel(gridDataControl, gridRange, excelVersion, useExcelExpanderAndIndentColumn);
            if (excludeColumns != null)
                if (excludeColumns.Count > 0)
                    excludeColumns.Clear();
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="excelVersion">The excel version.</param>
        /// <returns></returns>
        private static string GetVersion(ExcelVersion excelVersion)
        {
            string ret = string.Empty;
            switch (excelVersion)
            {
                case XlsIO.ExcelVersion.Excel2010:
                case XlsIO.ExcelVersion.Excel2007:
                    ret = "xlsx";
                    break;
                default:
                    ret = "xls";
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Silverlight - Extension method for GridDataControl  used for export the  GridDataControl cell data to excel sheet
        /// </summary>
        /// <param name="gridDataControl">GridDataControl Object</param>
        /// <param name="fileName">Name of the file for Save the information</param>
        /// <param name="excelVersion">Version of the Excel Sheet</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, ExcelVersion excelVersion,GridCellExportToExcelHandler exportingHandler,GridModelExportToExcelHandler modelExportingHandler,bool useExcelExpanderAndIndentColumn)
        {
            string xlVersion = GridDataControlExportExtensions.GetVersion(excelVersion);
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = xlVersion,
                FilterIndex = 1,
                Filter = "(." + xlVersion + ")|*." + xlVersion
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    ExcelEngine excelEngine = new ExcelEngine();
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Add();
                    workbook.Version = excelVersion;
                    GridRangeInfoList rangeList = new GridRangeInfoList();
                    int top = 0, left = 0, right = 0, bottom = 0;
                    bottom = (gridDataControl.StackedHeaderRows.Count > 0) ? gridDataControl.StackedHeaderRows.Count : 0;
                    right = gridDataControl.Model.ColumnCount - 1;
                    bottom = gridDataControl.Model.RowCount - 1;
                    GridRangeInfo range = GridRangeInfo.Cells(top, left, bottom, right);
                    ExportToExcel(gridDataControl, range, workbook.Worksheets[0], workbook.Worksheets[0].Range[1, 1], exportingHandler, modelExportingHandler, useExcelExpanderAndIndentColumn);
                    workbook.SaveAs(stream);
                    excelEngine.ThrowNotSavedOnDestroy = false;
                    excelEngine.Dispose();
                }
            }

        }


        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="excelVersion">The excel version.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, ExcelVersion excelVersion)
        {
            ExportToExcel(gridDataControl, excelVersion, null, null, false);
        }
#endif
        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="excelRange">The excel range.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange)
        {
            ExportToExcel(gridDataControl, gridRange, workSheet, excelRange, null);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="excelRange">The excel range.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        public static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler)
        {
            GridRangeInfo headerRange;
            GridRangeInfoList rangeList = new GridRangeInfoList();

            if (gridRange.RangeType == GridRangeInfoType.Table)
            {               
                gridRange = GridRangeInfo.Cells(0, 0, gridDataControl.Model.RowCount - 1,
                    gridDataControl.Model.ColumnCount - 1);

                rangeList.Add(gridRange);
            }
            else
            {
                headerRange = GridRangeInfo.Cells(0, gridRange.Left, gridDataControl.Model.HeaderRows - 1,gridRange.Right);
                int firstRecordRow = gridRange.Top < gridDataControl.Model.HeaderRows
                    ? gridDataControl.Model.HeaderRows
                    : gridRange.Top;
                gridRange = GridRangeInfo.Cells(firstRecordRow, gridRange.Left, gridRange.Bottom, gridRange.Right);

                rangeList.Add(headerRange);
                rangeList.Add(gridRange);
            }
                        
            int x = 1, y = 1;
            foreach (GridRangeInfo range in rangeList)
            {
                ExportToExcel(gridDataControl, range, workSheet, workSheet.Range[x, y], exportingHandler, false);
                x += gridDataControl.Model.HeaderRows;
            }                                    
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="excelRange">The excel range.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [hide expander and indent].</param>
        private static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler ,bool useExcelExpanderAndIndentColumn)
        {
            ExportToExcel(gridDataControl, gridRange, workSheet, excelRange, exportingHandler, null, useExcelExpanderAndIndentColumn);
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="gridDataControl">The grid data control.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="excelRange">The excel range.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        /// <param name="modelExportingHandler">The model exporting handler.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [hide expander and indent].</param>
        private static void ExportToExcel(this GridDataControl gridDataControl, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler ,GridModelExportToExcelHandler modelExportingHandler,bool useExcelExpanderAndIndentColumn)
        {
            int childCount = 0; //this holds the number for child count which includes the grouping child also.
            int childModelCount = -1;// this holds the number of child models
            bool isFirstGroupRow = false; //this flag is find the 1st grouped row.
            Dictionary<int, int> rowDict = new Dictionary<int, int>();//this dict which holds the corresponding row index and indent level.
            Dictionary<int, ExportingGridModelToExcelEventArgs> modelEventDict = new Dictionary<int, ExportingGridModelToExcelEventArgs>();//this holds the event args for every model.
            List<object> lstModel = new List<object>();//this lstmodel holds the model for the purpose of rasing the Model event.
            
            //Calling the recursive method.
            ExportToExcel(gridDataControl.Model, gridRange, workSheet, excelRange.Row, excelRange.Column, exportingHandler, modelExportingHandler, useExcelExpanderAndIndentColumn, rowDict, modelEventDict, lstModel,ref childCount,ref childModelCount,ref isFirstGroupRow);

            #region Indent for Child and Group

            if (useExcelExpanderAndIndentColumn && childCount > 0)
            {//this phase is for adding new columns for the indenting 

                for (int i = 1; i <= childCount; i++)
                {
                    workSheet.InsertColumn(i);
                    workSheet.Columns[i - 1].ColumnWidth = 1;
                }
                childCount++;
                foreach (var item in rowDict)
                {//this phase is to arrange the indent as per the corresponding levels.
                    if (!workSheet.Range[item.Key, childCount + item.Value].IsMerged)
                    {//this phase is to merge the inserted columns with already merged columns.
                        var style = workSheet.Range[item.Key, childCount].CellStyle;
                        var cellValue = workSheet.Range[item.Key, childCount].Value;
                        var cellHeight = workSheet.Range[item.Key, childCount].RowHeight;
                        workSheet.Range[item.Key, item.Value, item.Key, childCount].Merge();
                        workSheet.Range[item.Key, item.Value, item.Key, childCount].CellStyle = style;
                        workSheet.Range[item.Key, item.Value, item.Key, childCount].Value = cellValue;
                        workSheet.Range[item.Key, item.Value, item.Key, childCount].RowHeight = cellHeight;
                    }
                    else
                    {
                        var style = workSheet.Range[item.Key, childCount + item.Value].CellStyle;
                        var cellValue = workSheet.Range[item.Key, childCount + item.Value].MergeArea.DisplayText;
                        var cellHeight = workSheet.Range[item.Key, childCount + item.Value].MergeArea.RowHeight;
                        int mergedLastColumn = workSheet.Range[item.Key, childCount + item.Value].MergeArea.LastColumn;

                        //Below code was added to merge the GroupCaptionCells.
                        //if the Column count and GroupDescriptions.Count is same then we merged an additional column in ExportToExcel Method. 
                        //So here we unmerging the additionaly merged column by checking the condition.
                        if (mergedLastColumn > gridDataControl.Model.ColumnCount)
                        {
                            workSheet.Range[item.Key, mergedLastColumn].UnMerge();
                            mergedLastColumn--;
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].Merge();
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.CellStyle = style;
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.Value = cellValue;
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.RowHeight = cellHeight;
                            workSheet.Range[item.Key, mergedLastColumn + 1].Clear(ExcelClearOptions.ClearAll);
                        }
                        else
                        {
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].Merge();
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.CellStyle = style;
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.Value = cellValue;
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].MergeArea.RowHeight = cellHeight;
                        }
                        var tableCellType = (gridDataControl.Model[item.Value - 1, mergedLastColumn] as GridDataStyleInfo).CellIdentity.TableCellType;
                        if (excludeColumns != null && excludeColumns.Count > 0 && tableCellType != GridDataTableCellType.StackedColumnHeaderCell)
                        {
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn].UnMerge();
                            workSheet.Range[item.Key, item.Value, item.Key, mergedLastColumn - excludeColumns.Count].Merge();
                            workSheet.Range[item.Key, (mergedLastColumn - (excludeColumns.Count - 1)), item.Key, mergedLastColumn].Clear(ExcelClearOptions.ClearAll);
                        }
                    }
                }
            }
            #endregion          
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="excelRow">The excel row.</param>
        /// <param name="excelCol">The excel col.</param>
        /// <param name="exportingHandler">The exporting handler.</param>
        /// <param name="modelExportingHandler">The model exporting handler.</param>
        /// <param name="useExcelExpanderAndIndentColumn">if set to <c>true</c> [hide expander and indent].</param>
        /// <returns></returns>
        private static int ExportToExcel(GridDataTableModel model, GridRangeInfo gridRange, IWorksheet workSheet, int excelRow, int excelCol, 
            GridCellExportToExcelHandler exportingHandler, GridModelExportToExcelHandler modelExportingHandler, bool useExcelExpanderAndIndentColumn,
            Dictionary<int, int> rowDict, Dictionary<int, ExportingGridModelToExcelEventArgs> modelEventDict, List<object> lstModel,ref int childCount,ref int childModelCount,ref bool isFirstGroupRow)
        {
            GridExcelConverterControl excelConverter = new GridExcelConverterControl();
            int startingcolumnindex = excelCol;//this holds the startign index of every model grid.
            int groupLevelIndex = 0;//this holds the level of the grouped row.
            int hiddenColumnCount = 0;//this holds the hiddencolumn count.
            bool hasGroup = false; //this flag that shows that the particular row has group.
            Dictionary<int, int> groupRowDict = new Dictionary<int, int>(); //this groupRowDict which holds the grouped row index and corresponding level

            childModelCount++;

            if (!useExcelExpanderAndIndentColumn)
            {
                startingcolumnindex = childModelCount + 1;
                excelCol = startingcolumnindex;
            }
            
            ExportingGridModelToExcelEventArgs args = new ExportingGridModelToExcelEventArgs(false);
            if (excludeColumns != null && excludeColumns.Count > 0)
                args.ExcluedColumns = excludeColumns;
            if (modelExportingHandler != null)
            {//this phase is to triggers the model export event.
                
                if (model is GridDataChildTableModel && !lstModel.Contains((model as GridDataChildTableModel).ParentRecord.Data.GetType()))
                {
                    lstModel.Add((model as GridDataChildTableModel).ParentRecord.Data.GetType());
                    modelExportingHandler(model, args);
                    if (!modelEventDict.ContainsKey(childModelCount))
                        modelEventDict.Add(childModelCount, args);
                }

                else if (!(model is GridDataChildTableModel))
                {
                    modelExportingHandler(model, args);
                }

                else if (lstModel.Contains((model as GridDataChildTableModel).ParentRecord.Data.GetType()) && modelEventDict != null && modelEventDict.Count > 0 && modelEventDict.ContainsKey(childModelCount))
                    args = modelEventDict[childModelCount];
            }
            
            if (!args.Handled)
            {

                if (childCount < childModelCount)
                    childCount = childModelCount;
                #region hiddenColumnCount
                for (int i = 0; i <= model.TableProperties.VisibleColumns.Count + model.TableProperties.GroupedColumns.Count; i++)
                {
                    int visibleColumnIndex = model.ResolvePositionToVisibleColumnIndex(i);
                    if (visibleColumnIndex >= 0 && visibleColumnIndex < model.TableProperties.VisibleColumns.Count && model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                    {
                        hiddenColumnCount++;
                    }
                }
               
                #endregion
                for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
                {                    
                    int skippedCol = 0, previousExcluded = 0;//this holds the count of skipped columns in the particular row.
                    bool hasExtraRow = false;//this flag used to show this hasExtra Row
                    hasGroup = false;
                    bool isSummaryRowwithGroupHeader = false;
                    if (model.RowHeights[row] == 0)//this phase will remove the hidden rows.
                        continue;

                    for (int col = gridRange.Left; col <= gridRange.Right; col++)
                    {
                        bool isStackedHeaderColumn = false;
                        var tableCellType = (model[row, col] as GridDataStyleInfo).CellIdentity.TableCellType;
                        if (tableCellType == GridDataTableCellType.EmptyCell || tableCellType == GridDataTableCellType.RowHeaderCell || tableCellType == GridDataTableCellType.TopLeftHeaderCell
                            || tableCellType == GridDataTableCellType.RecordPlusMinusCell || tableCellType == GridDataTableCellType.ColumnHeaderIndentCell 
                            || tableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell)
                        {
                            skippedCol++;
                            continue;
                        }

                        if (tableCellType == GridDataTableCellType.AddNewRecordCell || tableCellType == GridDataTableCellType.AddNewRowHeaderCell ||
                            tableCellType == GridDataTableCellType.FilterBarCell)
                        {
                            hasExtraRow = true;
                            break;
                        }

                        bool nestedTableRow = false;
                        for (int b = col; b <= gridRange.Right; b++)
                        {
                            if (model[row, b].CellType == "NestedGrid")
                            {
                                nestedTableRow = true;
                                break;
                            }
                        }

                        CoveredCellInfo coveredRange = model.CoveredCells.GetCoveredCell(row, col);

                        if (coveredRange == null && args.ExcluedColumns != null && args.ExcluedColumns.Count > 0 && (col - skippedCol) < model.TableProperties.VisibleColumns.Count && args.ExcluedColumns.Contains(model.TableProperties.VisibleColumns[col - skippedCol].MappingName))
                            continue;

                        #region Grouped row export
                        
                        if (coveredRange != null && model.Table.HasGroups && (tableCellType == GridDataTableCellType.GroupCaptionCell || tableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell || tableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell || tableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell 
                            || tableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell || tableCellType == GridDataTableCellType.SummaryCoveredCell || tableCellType == GridDataTableCellType.SummaryTitleCell))
                        {
                            hasGroup = true;
                            int uptoRightColumn = 0;
                            if (!isFirstGroupRow)
                            {
                                childModelCount += model.Table.GroupModel.GroupDescriptions.Count;
                                childCount += model.Table.GroupModel.GroupDescriptions.Count;
                                isFirstGroupRow = true;
                            }
                            if (col >= coveredRange.Left && col <= coveredRange.Right)
                            {
                                if (model[row, col].CellValue != null)
                                {
                                    int additionalColumn = 0;
                                    groupLevelIndex = col - excelCol;

                                    if (!model.TableProperties.ShowRowHeader)
                                        groupLevelIndex++;
                                    if (useExcelExpanderAndIndentColumn)
                                        excelCol = groupLevelIndex;
                                    if (tableCellType == GridDataTableCellType.SummaryCoveredCell && (row -1) < rowDict.Count)//dict.Count)
                                        groupLevelIndex = rowDict.ElementAt(row - 1).Value;
                                    if (model.TableProperties.ShowGroupCaptionPlusMinus)
                                        additionalColumn++;
                                    excelConverter.GridCellToExcel(model, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);
                                    var rowHeight = workSheet.Range[excelRow, excelCol].RowHeight;
                                    var rowStyle = workSheet.Range[excelRow, excelCol].CellStyle;
                                    if (coveredRange.Left != coveredRange.Right && coveredRange.Right >= model.TableProperties.VisibleColumns.Count)
                                        if (!useExcelExpanderAndIndentColumn)
                                        {
                                            uptoRightColumn = model.TableProperties.VisibleColumns.Count - (args.ExcluedColumns != null ? args.ExcluedColumns.Count : 0);
                                            uptoRightColumn = uptoRightColumn - (additionalColumn + hiddenColumnCount);
                                        }
                                        //else if (args.ExcluedColumns != null && args.ExcluedColumns.Count > 0)
                                        //    uptoRightColumn = model.TableProperties.VisibleColumns.Count - args.ExcluedColumns.Count;
                                        else
                                        {
                                            uptoRightColumn = model.TableProperties.VisibleColumns.Count - hiddenColumnCount;
                                            uptoRightColumn = model.TableProperties.VisibleColumns.Count;
                                            if (excelCol == uptoRightColumn)
                                                uptoRightColumn++;
                                        }
                                    else
                                    {
                                        isSummaryRowwithGroupHeader = true;
                                        uptoRightColumn = coveredRange.Right;
                                        //below code is to adjust the grid column index to excel column index
                                        uptoRightColumn += (excelCol - col);
                                    }
                                    
                                    workSheet.Range[excelRow, excelCol, excelRow, uptoRightColumn].Merge();                                         
                                    if (workSheet.Range[excelRow, excelCol, excelRow, uptoRightColumn].MergeArea != null)
                                    {
                                        workSheet.Range[excelRow, excelCol, excelRow, uptoRightColumn].MergeArea.RowHeight = rowHeight;
                                        workSheet.Range[excelRow, excelCol, excelRow, uptoRightColumn].MergeArea.CellStyle = rowStyle;
                                    }
                                    
                                    if (!groupRowDict.ContainsKey(excelRow))
                                        groupRowDict.Add(excelRow, groupLevelIndex);
                                    if (isSummaryRowwithGroupHeader)
                                    {
                                        int exCol = coveredRange.Right + (excelCol - col);
                                        int additionalColumns = 0;
                                        var adjustGridToExcelColumn = model.TableProperties.ShowRowHeader ? 0 : 1;
                                        if (model.TableProperties.ShowGroupCaptionPlusMinus)
                                        {
                                            additionalColumns = model.Table.GroupModel.GroupDescriptions.Count;
                                            exCol -= (model.Table.GroupModel.GroupDescriptions.Count - groupLevelIndex);
                                        }
                                        int excludeColumnCount = excludeColumns != null ? excludeColumns.Count : 0;
                                        int columnCount = model.TableProperties.VisibleColumns.Count - excludeColumnCount + additionalColumns - adjustGridToExcelColumn;
                                        for (int i = coveredRange.Right; i <= columnCount; i++)
                                        {
                                            int visibleColumnIndex = model.ResolvePositionToVisibleColumnIndex(i);
                                            if (visibleColumnIndex >= 0 && visibleColumnIndex < model.TableProperties.VisibleColumns.Count && model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                                                continue;
                                            var value = model[row, i].CellValue;
                                            excelConverter.GridCellToExcel(model, row, i, workSheet.Range[excelRow, exCol < 1 ? 1 : exCol], exportingHandler, false);
                                            exCol++;
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                        #endregion

                        if (!hasGroup && tableCellType == GridDataTableCellType.GroupCaptionCell)
                        {
                            skippedCol++;
                            continue;
                        }

                        #region Stacked header export

                        if (tableCellType == GridDataTableCellType.StackedColumnHeaderCell)
                        {
                            var adjustGridToExcelColumn = model.TableProperties.ShowRowHeader ? 0 : 1;
                            int start = 0, end = 0, excluded = 0;
                            isStackedHeaderColumn = true;
                            excelConverter.GridCellToExcel(model, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);
                            var style = workSheet.Range[excelRow, excelCol].CellStyle;
                            var rowHeight = workSheet.Range[excelRow, excelCol].RowHeight;
                            var cellValue = workSheet.Range[excelRow, excelCol].Value;

                            if (args.ExcluedColumns != null)
                            {
                                for (int i = coveredRange.Left-1; i < coveredRange.Right; i++)
                                {                                
                                    foreach (var column in args.ExcluedColumns)
                                    {
                                        int visibleColumnIndex = model.ResolvePositionToVisibleColumnIndex(i);
                                        if (visibleColumnIndex >= 0 && visibleColumnIndex < model.TableProperties.VisibleColumns.Count && column == model.TableProperties.VisibleColumns[visibleColumnIndex].MappingName)
                                        {
                                            excluded++;
                                            break;
                                        }
                                    }
                                }
                            }
                            for (int i = coveredRange.Left; i <= coveredRange.Right ; i++)
                            {
                                int visibleColumnIndex = model.ResolvePositionToVisibleColumnIndex(i);
                                if (visibleColumnIndex >= 0 && visibleColumnIndex < model.TableProperties.VisibleColumns.Count && model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                                {
                                    excluded++;
                                }
                            }
                            if (model.Table.HasGroups)
                            {
                                if (model.TableProperties.ShowGroupCaptionPlusMinus)
                                {
                                    start = (coveredRange.Left -model.View.GroupDescriptions.Count- previousExcluded) + adjustGridToExcelColumn;
                                    end = (coveredRange.Right -model.View.GroupDescriptions.Count - (excluded + previousExcluded)) + adjustGridToExcelColumn;
                                }
                                else
                                {
                                    start = (coveredRange.Left - previousExcluded) + adjustGridToExcelColumn;
                                    end = (coveredRange.Right - (excluded + previousExcluded)) + adjustGridToExcelColumn;
                                }
                            }
                            else
                            {                                
                                int distanceBwLeftRanges = coveredRange.Left < gridRange.Left
                                    ? gridRange.Left - coveredRange.Left
                                    : 0;

                                int distanceBwRightRanges = coveredRange.Right > gridRange.Right
                                    ? coveredRange.Right - gridRange.Right
                                    : 0;

                                start = (coveredRange.Left - previousExcluded)+adjustGridToExcelColumn;
                                end = (coveredRange.Right - (excluded + previousExcluded)) +adjustGridToExcelColumn-distanceBwLeftRanges-distanceBwRightRanges;
                            }
                            workSheet.Range[excelRow, start, excelRow, end].Merge();
                            workSheet.Range[excelRow, start, excelRow, end].CellStyle = style;
                            workSheet.Range[excelRow, start, excelRow, end].RowHeight = rowHeight;
                            workSheet.Range[excelRow, start, excelRow, end].Value = cellValue;
                            col = coveredRange.Right;
                            excelCol = end+1;
                            previousExcluded += excluded;
                        }

                        #endregion

                        if (!nestedTableRow && !hasGroup && !isStackedHeaderColumn)
                        {//this phase exports the normal record cells to excel.
                            int visibleColumnIndex = model.ResolvePositionToVisibleColumnIndex(col);
                            if (visibleColumnIndex >= 0 && visibleColumnIndex < model.TableProperties.VisibleColumns.Count)
                            {
                                if (!model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                                {
                                    excelConverter.GridCellToExcel(model, row, col, workSheet.Range[excelRow, excelCol], exportingHandler, false);
                                    excelCol++;
                                }
                            }
                            else
                                excelCol++;
                        }
                        else
                        {
                            if (coveredRange != null)
                            {
                                GridDataChildTableModel childModel = model[row, col].CellValue as GridDataChildTableModel;

                                #region Child model Export
                                
                                if (childModel != null)
                                {
                                    GridRangeInfo gCRange = GridRangeInfo.Cells(0, 0, childModel.RowCount - 1, childModel.ColumnCount - 1);
                                    if (useExcelExpanderAndIndentColumn)
                                    {//New row with minimum height is added at the top of the model
                                        workSheet.InsertRow(excelRow);
                                        if (!rowDict.ContainsKey(excelRow))
                                            rowDict.Add(excelRow, childModelCount + 1);
                                        workSheet.Range[excelRow, excelCol].RowHeight = 5;
                                        excelRow++;
                                    }
                                    int lastrow = ExportToExcel(childModel, gCRange, workSheet, excelRow, excelCol, exportingHandler, modelExportingHandler, useExcelExpanderAndIndentColumn, rowDict, modelEventDict, lstModel, ref childCount, ref childModelCount, ref isFirstGroupRow);
                                    if (useExcelExpanderAndIndentColumn && workSheet.Range[lastrow -1, excelCol].RowHeight > 5)
                                    {//New row with minimum height is added at the bottom of the model.
                                        if (!rowDict.ContainsKey(lastrow))
                                            rowDict.Add(lastrow, childModelCount + 1);
                                        workSheet.InsertRow(lastrow);
                                        workSheet.Range[lastrow, excelCol].RowHeight = 5;
                                        lastrow++;
                                    }
                                    if (useExcelExpanderAndIndentColumn && args.ShouldShowExcelExpander)
                                    {//Expander for Child
                                        int index = lastrow - 1;
                                        if (workSheet.Range[index, excelCol].RowHeight <= 5)
                                            index++;
                                        workSheet.Range[excelRow - 1, excelCol, index -1, excelCol].Group(ExcelGroupBy.ByRows, false);
                                    }

                                    excelRow = lastrow;
                                    excelRow--;
                                    childModelCount--;
                                    break;
                                }

                                #endregion
                            }
                        }
                    }
                    excelCol = startingcolumnindex;
                    if (!hasExtraRow)
                    {//this phase keeps the row index and the coresponding level for indent.
                        if (hasGroup && !rowDict.ContainsKey(excelRow))
                            rowDict.Add(excelRow, groupLevelIndex);
                        else if (!rowDict.ContainsKey(excelRow))
                        {
                            if (!args.ShouldShowNestedIndent)
                                rowDict.Add(excelRow, childModelCount);
                            else
                                rowDict.Add(excelRow, childModelCount + 1);
                        }
                        excelRow++;
                    }
                }
            }

            #region Expander for Group

            if (useExcelExpanderAndIndentColumn && args.ShouldShowExcelExpander && groupRowDict.Count > 0)
            {
                int groupStartingRowIndex = groupRowDict.ElementAt(0).Key;
                for (int i = 1; i <= groupRowDict.Count; i++)
                {
                    if (i != groupRowDict.Count)
                    {
                        if (groupRowDict.ElementAt(i).Value == 1)
                        {
                            if ((groupStartingRowIndex + 1) >= (groupRowDict.ElementAt(i).Key - 1))
                            {
                                groupStartingRowIndex = groupRowDict.ElementAt(i).Key;
                                continue;
                            }
                            if ((i + 1) <= groupRowDict.Count)
                            {
                                int index = groupRowDict.ElementAt(i).Key;
                                if (workSheet.Range[index, excelCol].RowHeight <= 5)
                                    index++;
                                workSheet.Range[groupStartingRowIndex + 1, 1, index - 1, 1].Group(ExcelGroupBy.ByRows, false); ;
                            }
                            groupStartingRowIndex = groupRowDict.ElementAt(i).Key;
                        }
                        else if ((i + 1) == groupRowDict.Count)
                        {
                            int index = excelRow;
                            if (workSheet.Range[index, excelCol].RowHeight <= 5)
                                index++;
                            workSheet.Range[groupStartingRowIndex + 1, 1, index - 1, 1].Group(ExcelGroupBy.ByRows, false);
                        }
                    }
                    else if (groupStartingRowIndex <= model.View.TopLevelGroup.DisplayElements.Count)//for last group expander; if last group in not expanded, then no expander needed. So checked condition
                    {
                        int index = model.View.TopLevelGroup.DisplayElements.Count + 1; // next row to set as last row for expander.
                        if (workSheet.Range[index, excelCol].RowHeight <= 5)
                            index++;
                        workSheet.Range[groupStartingRowIndex + 1, 1, index, 1].Group(ExcelGroupBy.ByRows, false);
                    }
                }
            }

            #endregion

            return excelRow;
        }

        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void ExportToCSV(this GridDataTableModel gridModel, string fileName)
        {
            if (!gridModel.Table.HasGroups && !gridModel.Table.HasNestedTables)
            {
                ExportToCSV(gridModel, GridRangeInfo.Table(), fileName);
            }
        }

        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="delimiter">The delimiter.</param>
        public static void ExportToCSV(this GridDataTableModel gridModel, string fileName, string delimiter)
        {
            if (!gridModel.Table.HasGroups && !gridModel.Table.HasNestedTables)
            {
                ExportToCSV(gridModel, GridRangeInfo.Table(), fileName , delimiter);
            }
        }
        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void 
            ExportToCSV(this GridDataTableModel gridModel, GridRangeInfo gridRange, string fileName)
        {
            string csvText;
            CopyTextToBuffer(gridModel, out csvText,",", gridRange);

#if SILVERLIGHT

            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "(.csv)|*.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(sfd.OpenFile()))
                {
                    sw.Write(csvText);
                }
            }
#else
            File.WriteAllText(fileName, csvText);

#endif
        }

        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="gridRange">The grid range.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="delimiter">The delimiter.</param>
        public static void
           ExportToCSV(this GridDataTableModel gridModel, GridRangeInfo gridRange, string fileName, string delimiter)
        {
            string csvText;
            CopyTextToBuffer(gridModel, out csvText, delimiter, gridRange);

#if SILVERLIGHT

            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "(.csv)|*.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(sfd.OpenFile()))
                {
                    sw.Write(csvText);
                }
            }
#else
            File.WriteAllText(fileName, csvText);

#endif
        }

        /// <summary>
        /// Copies the text to buffer.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        private static bool CopyTextToBuffer(this GridDataTableModel gridModel, out string buffer, string delimiter, Syncfusion.Windows.Controls.Grid.GridRangeInfo range)
        {
            GridRangeInfo tRange = GetExpandedRange(range, gridModel);

            StringBuilder sb = new StringBuilder();
            string tabDelim = delimiter;

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
                    sb.Append(text);
                    firstCol = false;
                }

                sb.Append(Environment.NewLine);
            }

            buffer = sb.ToString();
            return true;
        }

        /// <summary>
        /// Gets the expanded range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="gridModel">The grid model.</param>
        /// <returns></returns>
        private static GridRangeInfo GetExpandedRange(GridRangeInfo range, GridDataTableModel gridModel)
        {
            var firstrow = gridModel.HeaderRows;
            var firstcol = gridModel.HeaderColumns;

            if (range.IsCols)
            {
                range = range.ExpandRange(range.Top + firstrow, range.Left, gridModel.RowCount, range.Left);
            }

            if (range.IsRows)
            {
                range = range.ExpandRange(range.Top, range.Left + firstcol, range.Top, gridModel.ColumnCount);
            }

            if (range.IsTable)
            {
                range = range.ExpandRange(range.Top + firstrow, range.Left + firstcol, gridModel.RowCount, gridModel.ColumnCount);
            }

            return range;
        }
    }
}
