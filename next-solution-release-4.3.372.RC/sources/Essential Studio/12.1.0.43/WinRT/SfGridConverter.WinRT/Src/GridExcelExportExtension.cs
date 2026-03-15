#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Syncfusion.XlsIO;

#if WinRT
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Globalization;
using Syncfusion.UI.Xaml.Controls.Input;
using System.Reflection;
#else
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Syncfusion.Windows.Shared;
using System.Globalization;
using System.Windows.Documents;
using System.Reflection;
using Syncfusion.Windows.Tools.Controls;
#endif

#if WPF
using System.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    public static class GridExcelExportExtension
    {
        //the below static fields are used to keep the StartRowIndex and StartColumnIndex of Excel, We don't change this anywhere.
        //We will reset this from ExcelExportingOptions before exporting Group,Grid.
        static int excelRowIndex = 1;
        static int excelColumnIndex = 0;

        private static Dictionary<object, int> childEndInfo = new Dictionary<object, int>();
        private static Dictionary<object, int> childStartInfo = new Dictionary<object, int>();
        private static Dictionary<object, bool> childVisibleInfo = new Dictionary<object, bool>();        
        private static Dictionary<Group, int> groupEndIndexInfo = new Dictionary<Group, int>();
        private static Dictionary<Group, int> groupStartIndexInfo = new Dictionary<Group, int>();
        private static Dictionary<Group, bool> groupVisibleInfo = new Dictionary<Group, bool>();


        public static ExcelEngine ExportCollectionToExcel(this SfDataGrid grid, ICollectionViewAdv gridCollectionView)
        {
            return grid.ExportCollectionToExcel(gridCollectionView, ExcelVersion.Excel2007, null, null, false);
        }

        public static ExcelEngine ExportCollectionToExcel(this SfDataGrid grid, ICollectionViewAdv gridCollectionView, ExcelVersion excelVersion)
        {
            return grid.ExportCollectionToExcel(gridCollectionView, excelVersion, null, null, false);
        }

        public static ExcelEngine ExportCollectionToExcel(this SfDataGrid grid, ICollectionViewAdv gridCollectionView, ExcelVersion excelVersion, GridExcelExportingEventhandler exportingHandler, GridCellExcelExportingEventHandler cellsExportingHandler, bool exportAllPages)
        {
            ExcelExportingOptions excelExportingOptions = new ExcelExportingOptions(excelVersion, exportAllPages, false, exportingHandler, cellsExportingHandler);
            return grid.ExportToExcel(gridCollectionView, excelExportingOptions);
        }

        /// <summary>
        /// This method exports the dataGrid to excel by considering Grouping,Sorting,Filtering,Summaries,Paging,NestedGrid,CellTypes.
        /// </summary>
        /// <param name="grid"> SfDataGrid </param>
        /// <param name="gridCollectionView"> ICollectionViewAdv</param>
        /// <param name="excelExportingOptions"> Class <see cref="ExcelExportingOptions"> which is used to set the Exporting Options </see></param>
        /// <returns>ExcelEngine</returns>
        public static ExcelEngine ExportToExcel(this SfDataGrid grid, ICollectionViewAdv gridCollectionView, ExcelExportingOptions excelExportingOptions)
        {
            //Creating New Workbook.
            ExcelEngine engine = new ExcelEngine();
            IWorkbook workbook = engine.Excel.Workbooks.Create();
            IWorksheet sheet;
            if (grid.DetailsViewDefinition.Count > 0)
                excelExportingOptions.AllowOutlining = true;

            workbook.Version = excelExportingOptions.ExcelVersion;

            bool exportAllPages = excelExportingOptions.ExportAllPages;
            excelExportingOptions.ColumnCount = (from column in grid.Columns where !excelExportingOptions.ExcludeColumns.Contains(column.MappingName) select column.MappingName).Count();

            if (exportAllPages && (gridCollectionView is PagedCollectionView) && 
                excelExportingOptions.ExportPageOptions == ExportPageOptions.ExportToDifferentSheets)
            {
                int pageCount = (gridCollectionView as PagedCollectionView).PageCount;
                int pageIndex = (gridCollectionView as PagedCollectionView).PageIndex;

                for (int i = 0; i < pageCount; i++)
                {
                    sheet = workbook.Worksheets[i];
                    if (i != pageIndex)
                    {
                        var records = (gridCollectionView as PagedCollectionView).GetInternalListForIndex(i);
                        var view = grid.CreateCollectionView(records, null, null);
                        ExportToExcelWorksheet(grid, view, excelExportingOptions, sheet);
                        view.Dispose();
                    }
                    else
                        ExportToExcelWorksheet(grid, gridCollectionView, excelExportingOptions, sheet);

                    if (i >= workbook.Worksheets.Count - 1 && i < pageCount - 1)
                        workbook.Worksheets.Create();
                }
            }
            else
            {
                sheet = workbook.Worksheets[0];
                ExportToExcelWorksheet(grid, gridCollectionView, excelExportingOptions, sheet);
            }
            return engine;
        }
        
        # region Internal Methods

        internal static void ExportToExcelWorksheet(this SfDataGrid grid, ICollectionViewAdv gridCollectionView, ExcelExportingOptions excelExportingOptions, IWorksheet sheet)
        {
            GridExcelExportingEventhandler exportingEventHandler = excelExportingOptions.ExportingEventHandler;
            bool allowOutlining = excelExportingOptions.AllowOutlining;
            bool exportAllPages = excelExportingOptions.ExportAllPages;
            sheet.PageSetup.IsSummaryRowBelow = false;

            if (gridCollectionView != null)
            {                
                childStartInfo.Clear();
                childEndInfo.Clear();
                childVisibleInfo.Clear();
                groupStartIndexInfo.Clear();
                groupEndIndexInfo.Clear();
                groupVisibleInfo.Clear();

                IPropertyAccessProvider propertyAccessProvider = gridCollectionView.GetPropertyAccessProvider();
                
                //Resetting the excelRowIndex and excelColumnIndex from ExcelExportingOptions.
                excelRowIndex = excelExportingOptions.StartRowIndex;
                excelColumnIndex = excelExportingOptions.StartColumnIndex;

                //if collection is a PagedCollection and ExportToDifferentSheet option is set as false, then we don't consider about grouping.
                if ((gridCollectionView.GroupDescriptions.Count > 0) &&
                    (!exportAllPages || !(gridCollectionView is PagedCollectionView
                    && excelExportingOptions.ExportPageOptions != ExportPageOptions.ExportToDifferentSheets))
                    && !(gridCollectionView is GridVirtualizingCollectionView))
                {
                    int startColumnIndex = excelExportingOptions.StartColumnIndex;

                    //Below code sets the width for indent cells and merges these cells.
                    //AllowOutlining is True then we don't use indent spacing.
                    if (!allowOutlining)
                    {
                        startColumnIndex = excelColumnIndex + gridCollectionView.GroupDescriptions.Count;
                        sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, startColumnIndex].Merge();
                        SetIndentColumnWidth(sheet, excelColumnIndex, startColumnIndex);
                    }

                    SetColumnWidth(grid, sheet, startColumnIndex, excelExportingOptions.ExcludeColumns);
                    ExportHeadersToExcel(grid, sheet, startColumnIndex, excelExportingOptions);
                    ExportTableSummariesToExcel(grid, sheet, gridCollectionView, excelExportingOptions, TableSummaryRowPosition.Top);
                    foreach (Group group in gridCollectionView.TopLevelGroup.Groups)
                    {
                        if (group.ItemsCount > 0)
                            ExportGroupToExcel(gridCollectionView, grid, sheet, group, excelExportingOptions); ;
                    }
                }
                else
                {
                    SetColumnWidth(grid, sheet, excelColumnIndex, excelExportingOptions.ExcludeColumns);
                    ExportHeadersToExcel(grid, sheet, excelColumnIndex, excelExportingOptions);
                    ExportTableSummariesToExcel(grid, sheet, gridCollectionView, excelExportingOptions, TableSummaryRowPosition.Top);
                    Type propertyType = typeof(string);
                    IEnumerable records = gridCollectionView.Records;

                    if (exportAllPages && gridCollectionView is PagedCollectionView && excelExportingOptions.ExportPageOptions == ExportPageOptions.ExportToSingleSheet)
                    {
                        if (!(gridCollectionView as PagedCollectionView).UseOnDemandPaging)
                        {
                            records = (gridCollectionView as PagedCollectionView).GetInternalList();
                            ExportRecordsToExcel(grid, records, propertyAccessProvider, sheet, excelExportingOptions, null);
                        }
                        else
                        {
                            var pageCount = (gridCollectionView as PagedCollectionView).PageCount;
                            for (int i = 0; i < pageCount; i++)
                            {
                                records = (gridCollectionView as PagedCollectionView).GetInternalListForIndex(i);
                                ExportRecordsToExcel(grid, records, propertyAccessProvider, sheet, excelExportingOptions, null);
                            }
                        }
                    }
                    else if (gridCollectionView is GridVirtualizingCollectionView)
                    {
                        records = (gridCollectionView as GridVirtualizingCollectionView).GetInternalSource();
                        ExportRecordsToExcel(grid, records, propertyAccessProvider, sheet, excelExportingOptions, null);
                    }
                    else
                        ExportRecordsToExcel(grid, records, propertyAccessProvider, sheet, excelExportingOptions, null);
                }
                ExportTableSummariesToExcel(grid, sheet, gridCollectionView, excelExportingOptions, TableSummaryRowPosition.Bottom);

                //Now we don't export the row heights from grid. By default we setting the rowheight as 30;
                sheet.SetRowHeightInPixels(1, excelRowIndex, 30.0);

                //outlining the ChildGrids.
                foreach (KeyValuePair<object, int> pair in childStartInfo)
                {
                    int start = childStartInfo[pair.Key];
                    int end = childEndInfo[pair.Key];
                    if (start != end)
                    {
                        bool IsVisible = childVisibleInfo.Count > 0 ? childVisibleInfo[pair.Key] : false;
                        sheet.Range[start + 1, 1, end, 1].Group(ExcelGroupBy.ByRows, !IsVisible);
                    }
                }

                //Outlining the groups if allowOutlining of parent grid is true.
                if (allowOutlining)
                {
                    foreach (KeyValuePair<Group, int> pair in groupStartIndexInfo)
                    {
                        int start = groupStartIndexInfo[pair.Key];
                        int end = groupEndIndexInfo[pair.Key];
                        if (start != end)
                        {
                            bool IsVisible = groupVisibleInfo[pair.Key];
                            sheet.Range[start + 1, 1, end, 1].Group(ExcelGroupBy.ByRows, !IsVisible);
                        }
                    }
                }
            }
        }

        internal static void ExportRecordsToExcel(SfDataGrid grid, IEnumerable records, IPropertyAccessProvider propertyAccessProvider, IWorksheet sheet, ExcelExportingOptions excelExportingOptions, Group group)
        {
            GridCellExcelExportingEventHandler cellsExportingEventHandler = excelExportingOptions.CellsExportingEventHandler;
            IEnumerable<string> gridColumns = from column in grid.Columns
                                              where !excelExportingOptions.ExcludeColumns.Contains(column.MappingName)
                                              select column.MappingName;
            foreach (object rec in records)
            {
                object record = (rec is RecordEntry) ? (rec as RecordEntry).Data : rec;

                if (record == null)
                    continue;

                //Resetting the excelColumnIndex before exporting each record.
                excelColumnIndex = excelExportingOptions.StartColumnIndex;  

                //Setting the rowindex and columnindex before exporting each record.
                excelRowIndex++;

                int startColumnIndex = 0;
                if (group != null)
                    startColumnIndex = excelExportingOptions.AllowOutlining ? excelColumnIndex : (excelColumnIndex + group.Level);
                else
                    startColumnIndex = excelColumnIndex;

                //Below code merges the indent cells before the record cell.
                if (startColumnIndex > excelColumnIndex + 1)
                    sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, startColumnIndex - 1].Merge();

                foreach (string column in gridColumns)
                {
                    IRange exportRange = sheet.Range[excelRowIndex, startColumnIndex];
                    object cellValue;
                    if (grid.Columns[column] is GridUnBoundColumn)
                        cellValue = grid.GetUnBoundCellValue(grid.Columns[column], record);
                    else
                    {
                        if (excelExportingOptions.ExportMode == ExportMode.Text && !(grid.Columns[column] is GridHyperlinkColumn) && !(grid.Columns[column] is GridImageColumn))
                            cellValue = propertyAccessProvider.GetFormattedValue(record, column);
                        else
                            cellValue = propertyAccessProvider.GetValue(record, column);
                    }

                    GridCellExcelExportingEventArgs cellArgs = new GridCellExcelExportingEventArgs(exportRange, ExportCellType.RecordCell, cellValue, rec, column,
                        excelExportingOptions.GridViewDefinition, excelExportingOptions.ChildLevel, grid, excelExportingOptions.ExportMode, propertyAccessProvider);

                    if (cellsExportingEventHandler != null)
                        cellsExportingEventHandler(grid, cellArgs);

                    if (!cellArgs.Handled)
                    {
                        if (excelExportingOptions.ExportMode != cellArgs.ExportMode
                            && !(grid.Columns[column] is GridHyperlinkColumn)
                            && !(grid.Columns[column] is GridImageColumn)
                            && !(grid.Columns[column] is GridUnBoundColumn))
                        {
                            if (cellArgs.ExportMode == ExportMode.Text)
                                cellValue = propertyAccessProvider.GetFormattedValue(record, column);
                            else
                                cellValue = propertyAccessProvider.GetValue(record, column);
                        }

                        if (cellArgs.ExportMode == ExportMode.Text
                            && !(grid.Columns[column] is GridHyperlinkColumn)
                            && !(grid.Columns[column] is GridImageColumn))
                            exportRange.Text = cellValue != null ? cellValue.ToString() : string.Empty;
                        else
                            ExportCellValueToExcel(exportRange, propertyAccessProvider, record, cellArgs.CellValue, grid.Columns[column]);
                    }
                    startColumnIndex++;
                }

                //Below code added to export the Nested Grid.
                if (grid.DetailsViewDefinition.Count > 0)
                {
                    //Adding start row index of the child to dictionary for outlining.
                    childStartInfo.Add(rec, excelRowIndex);

                    foreach (ViewDefinition definition in grid.DetailsViewDefinition)
                    {
                        ICollectionViewAdv view;
                        SfDataGrid dataGrid = (definition as GridViewDefinition).DataGrid;
                        string relationalColumn = (definition as GridViewDefinition).RelationalColumn;

                        GridChildExportingEventArgs childArgs = new GridChildExportingEventArgs(rec as RecordEntry, relationalColumn, excelExportingOptions.ChildLevel + 1, new List<string>(), grid);

                        if (excelExportingOptions.ChildExportingEventHandler != null)
                            excelExportingOptions.ChildExportingEventHandler(grid, childArgs);

                        if (childArgs.Cancel)
                            continue;

                        //Below code creates a new CollectionViewAdv if ChildView is null.
                        bool isNewlyCreatedView = false;
                        if (rec is RecordEntry)
                        {
                            if ((rec as RecordEntry).ChildViews == null)
                            {
                                view = dataGrid.CreateCollectionView((rec as RecordEntry).Data, relationalColumn, propertyAccessProvider);
                                isNewlyCreatedView = true;
                            }
                            else
                                view = (rec as RecordEntry).ChildViews[relationalColumn].View;
                        }
                        else
                        {
                            view = dataGrid.CreateCollectionView(rec, relationalColumn, propertyAccessProvider);
                            isNewlyCreatedView = true;
                        }

                        if (view != null)
                        {
                            //Creating ExcelExportingOptions that is used to export childGrid.
                            //By default AllowOutlining is True for ChildGrid, because we can't use indent spacing for childGrid.
                            ExcelExportingOptions exportingOptions = new ExcelExportingOptions();
                            exportingOptions.ExcludeColumns = childArgs.ExcludeColumns;
                            exportingOptions.StartColumnIndex = excelExportingOptions.StartColumnIndex;
                            exportingOptions.ExportingEventHandler = excelExportingOptions.ExportingEventHandler;
                            exportingOptions.CellsExportingEventHandler = excelExportingOptions.CellsExportingEventHandler;
                            exportingOptions.ChildExportingEventHandler = excelExportingOptions.ChildExportingEventHandler;
                            exportingOptions.ChildLevel = excelExportingOptions.ChildLevel + 1;
                            exportingOptions.GridViewDefinition = definition;
                            exportingOptions.ExportMode = excelExportingOptions.ExportMode;
                            exportingOptions.ExportPageOptions = excelExportingOptions.ExportPageOptions;
                            ExportChildGridToExcel(dataGrid, sheet, view, exportingOptions);
                        }
                        if (isNewlyCreatedView)
                            view.Dispose();
                    }

                    childEndInfo.Add(rec, excelRowIndex);
                    if (rec is RecordEntry)
                        childVisibleInfo.Add(rec, (rec as RecordEntry).IsExpanded);
                }
            }
        }

        /// <summary>
        /// this method exports the groups and group records to excel.
        /// </summary>
        /// <param name="groupCollection"> ICollectionViewAdv </param>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="sheet">IWorkSheet</param>
        /// <param name="group">Data.Group</param>
        /// <param name="excelExportingOptions"> Class <see cref:"ExcelExportingOptions"> passed as an argument to the exporting method.</param>
        internal static void ExportGroupToExcel(ICollectionViewAdv groupCollection, SfDataGrid grid, IWorksheet sheet, Group group, ExcelExportingOptions excelExportingOptions)
        {
            GridExcelExportingEventhandler exportingEventHandler = excelExportingOptions.ExportingEventHandler;
            GridCellExcelExportingEventHandler cellsExportingEventHandler = excelExportingOptions.CellsExportingEventHandler;
            bool allowOutlining = excelExportingOptions.AllowOutlining;

            //Below code expands the Group if the group not expanded and Set the isExpanded flag as false.
            //if the flag isExpanded is false we collapse the group after exporting.
            bool isExpanded = true;
            if (!group.IsExpanded)
            {
                isExpanded = false;
                group.IsExpanded = true;
            }

            IPropertyAccessProvider propertyAccessProvider = groupCollection.GetPropertyAccessProvider();
            IEnumerable<string> gridColumns = from column in grid.Columns where !excelExportingOptions.ExcludeColumns.Contains(column.MappingName) select column.MappingName;

            //Resetting the excelColumnIndex before exporting Group.
            excelColumnIndex = excelExportingOptions.StartColumnIndex;

            if (group.IsGroups)
            {
                excelRowIndex++;

                //below code adds the group startrow Index to a dictionary for outlining purpose.
                groupStartIndexInfo.Add(group, excelRowIndex);

                ExportGroupCaptionToExcel(grid, groupCollection, group, sheet, excelExportingOptions);

                if (!group.IsBottomLevel)
                {
                    foreach (Group childGroup in group.Groups)
                    {
                        if (childGroup.ItemsCount > 0)
                            ExportGroupToExcel(groupCollection, grid, sheet, childGroup, excelExportingOptions);
                    }
                }
                else
                {
                    ExportRecordsToExcel(grid, group.Records, propertyAccessProvider, sheet, excelExportingOptions, group);
                    ExportGroupSummariesToExcel(grid, sheet, groupCollection, group, excelExportingOptions);
                }
            }
            
            //collapsing the group if the flag isExpanded is false.
            if (!isExpanded)
            {
                group.IsExpanded = false;
            }
            groupEndIndexInfo.Add(group, excelRowIndex);
            if (grid.AutoExpandGroups && groupCollection is PagedCollectionView && groupCollection != grid.View)
                groupVisibleInfo.Add(group, true);
            else
                groupVisibleInfo.Add(group, group.IsExpanded);
        }

        /// <summary>
        /// This method exports Group caption to the excel with/ without indent space.
        /// </summary>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="gridCollection">ICollectionViewAdv</param>
        /// <param name="group">Data.Group</param>
        /// <param name="sheet">IWorkSheet</param>
        /// <param name="excelExportingOptions">Class <see cref:"ExcelExportingOptions"> which contains info about  exporting Options.</see></param>
        internal static void ExportGroupCaptionToExcel(SfDataGrid grid, ICollectionViewAdv gridCollection, Group group, IWorksheet sheet, ExcelExportingOptions excelExportingOptions)
        {
            bool allowOutlining = excelExportingOptions.AllowOutlining;
            GridExcelExportingEventhandler exportingEventHandler = excelExportingOptions.ExportingEventHandler;
            GridExcelExportingEventArgs args = new GridExcelExportingEventArgs(sheet, ExportCellType.GroupCaptionCell, new ExportCellStyle(),excelExportingOptions.ChildLevel, grid);            

            if (exportingEventHandler != null)            
                exportingEventHandler(grid, args);
            
            if (!args.Handled)
            {
                IRange summaryRange;
                string summaryDisplayTextForRow;
                
                //Default Group caption format of SfDataGrid.
                string str = "{ColumnName} : {Key} - {ItemsCount} Items";

                int startColumnIndex = excelExportingOptions.StartColumnIndex;

                if (!allowOutlining)                
                    startColumnIndex = (excelColumnIndex + group.Level) - 1;
                
                //Merging the Indent cells before the GroupCaptionCell.
                if (startColumnIndex > excelColumnIndex + 1)                
                    sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, startColumnIndex - 1].Merge();                

                //Below code merges All the GroupCaptionCells, if ShowSummaryInRow is True.
                if ((grid.CaptionSummaryRow == null) || grid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    if (allowOutlining)                    
                        sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1].Merge();                    
                    else                    
                        sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + gridCollection.GroupDescriptions.Count) - 1].Merge();                    
                }

                if (grid.CaptionSummaryRow == null)
                {
                    string propertyName = (gridCollection.GroupDescriptions[group.Level - 1] as PropertyGroupDescription).PropertyName;
                    var columnHeaderText = grid.Columns.FirstOrDefault(col => col.MappingName == propertyName).HeaderText;
                    string groupCaptiontextFormat = (grid.GroupCaptionTextFormat != null) ? grid.GroupCaptionTextFormat : str;
                    summaryDisplayTextForRow = gridCollection.TopLevelGroup.GetGroupCaptionText(group, groupCaptiontextFormat, columnHeaderText);

                    summaryRange = sheet.Range[excelRowIndex, startColumnIndex];
                    ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow, null, string.Empty, excelExportingOptions);
                }
                else if (grid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    summaryRange = sheet.Range[excelRowIndex, startColumnIndex];
                    summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayTextForRow(group.SummaryDetails, gridCollection);
                    ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow, grid.CaptionSummaryRow, string.Empty, excelExportingOptions);
                }
                else
                {
                    int tempExcelColumn = allowOutlining ? excelColumnIndex : excelColumnIndex + gridCollection.GroupDescriptions.Count;

                    //Below code merges the first column with the part of the indent cells.
                    if (allowOutlining)                    
                        sheet.Range[excelRowIndex, excelColumnIndex + group.Level - 1, excelRowIndex, tempExcelColumn].Merge();                    

                   foreach (GridSummaryColumn summaryColumn in grid.CaptionSummaryRow.SummaryColumns)
                   {
                        var gridColumn = grid.Columns.FirstOrDefault(s => s.MappingName == summaryColumn.MappingName);
                        int columnIndex = grid.ResolveSummaryColumnIndex(summaryColumn.MappingName, excelExportingOptions.ExcludeColumns);

                        if (columnIndex < 0)
                            continue;

                        summaryRange = sheet.Range[excelRowIndex, tempExcelColumn + columnIndex];
                        summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayText(group.SummaryDetails, summaryColumn.MappingName, gridCollection);
                        ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow, grid.CaptionSummaryRow, gridColumn.MappingName, excelExportingOptions);
                    }

                    //Calculation of summaryrange, which is passed to ExportCellStyle.
                    if (allowOutlining)                   
                        summaryRange = sheet.Range[excelRowIndex, tempExcelColumn, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1];                    
                    else                    
                        summaryRange = sheet.Range[excelRowIndex, excelColumnIndex+group.Level-1, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + gridCollection.GroupDescriptions.Count) - 1];                    
                }
                ExportCellStyle(summaryRange, args.CellStyle);
            }
        }
        
        /// <summary>
        /// Exports the GroupSummaries to excel with/without indent space.
        /// </summary>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="sheet">IWorkSheet</param>
        /// <param name="gridCollection">ICollectionViewAdv</param>
        /// <param name="group">Data.Group</param>
        /// <param name="excelExportingOptions">Class <see cref:"ExcelExportingOptions"> which contains the information about Exporting Options </see></param>
        internal static void ExportGroupSummariesToExcel(SfDataGrid grid, IWorksheet sheet, ICollectionViewAdv gridCollection, Group group, ExcelExportingOptions excelExportingOptions)
        {
            bool allowOutlining = excelExportingOptions.AllowOutlining;
            GridExcelExportingEventhandler exportingEventHandler = excelExportingOptions.ExportingEventHandler;
            GridExcelExportingEventArgs args = new GridExcelExportingEventArgs(sheet, ExportCellType.GroupSummaryCell, new ExportCellStyle(),excelExportingOptions.ChildLevel,grid);            

            if (exportingEventHandler != null)            
                exportingEventHandler(grid, args);
            
            if (!args.Handled)
            {
                List<SummaryRecordEntry> groupSummaries = (group.Details as GroupRecordEntry).Summaries;

                foreach (SummaryRecordEntry summaryRecordEntry in groupSummaries)
                {
                    IRange summaryRange;
                    string summaryDisplayTextForRow;
                    excelRowIndex++;

                    int startColumnIndex = excelExportingOptions.StartColumnIndex;

                    if (!allowOutlining)                    
                        startColumnIndex = excelColumnIndex + group.Level;                    

                    //Below code merges the indent cells before the Summary Cell.
                    if (startColumnIndex > excelColumnIndex + 1)                    
                        sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, startColumnIndex-1].Merge();
                    
                    if (!summaryRecordEntry.SummaryRow.ShowSummaryInRow)
                    {
                        foreach (var summaryColumn in summaryRecordEntry.SummaryRow.SummaryColumns)
                        {
                            startColumnIndex = allowOutlining ? excelColumnIndex : excelColumnIndex + group.Level;
                            
                            var visibleColumn = grid.Columns.FirstOrDefault(s => s.MappingName == summaryColumn.MappingName);
                            int columnIndex = grid.ResolveSummaryColumnIndex(summaryColumn.MappingName, excelExportingOptions.ExcludeColumns) + startColumnIndex;
                            
                            if (columnIndex < startColumnIndex)
                                continue;

                            summaryRange = sheet.Range[excelRowIndex, columnIndex];
                            summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, summaryColumn.MappingName, gridCollection);
                            ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.GroupSummaryCell, summaryDisplayTextForRow, summaryRecordEntry, visibleColumn.MappingName, excelExportingOptions);
                        }
                    }
                    else
                    {                        
                        summaryRange = sheet.Range[excelRowIndex, startColumnIndex];
                        summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayTextForRow(summaryRecordEntry, gridCollection);
                        ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.GroupSummaryCell, summaryDisplayTextForRow, summaryRecordEntry, string.Empty, excelExportingOptions);
                        
                        //Below code merges all summary cells if ShowSummaryInRow is True.
                        if (allowOutlining)                        
                            sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1].Merge();                        
                        else                        
                            sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + gridCollection.GroupDescriptions.Count) - 1].Merge();                        
                    }
                    if (allowOutlining)                  
                        summaryRange = sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1];                    
                    else                    
                        summaryRange = sheet.Range[excelRowIndex, startColumnIndex, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + gridCollection.GroupDescriptions.Count) - 1];
                    
                    ExportCellStyle(summaryRange, args.CellStyle);
                }
            }
        }

        /// <summary>
        /// Exports TableSummaries to excel.
        /// </summary>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="sheet">IWorksheet</param>
        /// <param name="gridCollection">ICollectionViewAdv</param>
        /// <param name="excelExportingOptions">Class <see cref:"ExcelExportingOptions"> Which hold the exporting Options. </see></param>
        internal static void ExportTableSummariesToExcel(SfDataGrid grid, IWorksheet sheet, ICollectionViewAdv gridCollection, ExcelExportingOptions excelExportingOptions, TableSummaryRowPosition position)
        {
            bool allowOutlining = excelExportingOptions.AllowOutlining;
            GridExcelExportingEventhandler exportingEventHandler = excelExportingOptions.ExportingEventHandler;
            GridExcelExportingEventArgs args;
            if (position == TableSummaryRowPosition.Top)
                args = new GridExcelExportingEventArgs(sheet, ExportCellType.TopTableSummaryCell, new ExportCellStyle(), excelExportingOptions.ChildLevel, grid);
            else
                args = new GridExcelExportingEventArgs(sheet, ExportCellType.TableSummaryCell, new ExportCellStyle(), excelExportingOptions.ChildLevel, grid);

            if (exportingEventHandler != null)           
                exportingEventHandler(grid, args);
            
            if (!args.Handled)
            {
                IEnumerable<GridSummaryRow> summaryRows;
                if (position == TableSummaryRowPosition.Top)
                    summaryRows = grid.GetTopTableSummaries();
                else
                    summaryRows = grid.GetBottomTableSummaries();
                foreach (GridSummaryRow summaryRow in summaryRows)
                {
                    var summaryRecordEntry = gridCollection.Records.TableSummaries.FirstOrDefault(record => record.SummaryRow == summaryRow);
                    if(summaryRecordEntry == null) continue;
                    IRange summaryRange;
                    string summaryDisplayTextForRow;
                    excelRowIndex++;
                    excelColumnIndex = excelExportingOptions.StartColumnIndex;
                    if (summaryRecordEntry.SummaryRow.ShowSummaryInRow)
                    {
                        //Merges all TableSummaryCells if ShowSummaryInRow is true.
                        if (allowOutlining)                        
                            sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1].Merge();                        
                        else
                            sheet.Range[excelRowIndex, excelColumnIndex, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + grid.View.GroupDescriptions.Count) - 1].Merge();
                        
                        summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayTextForRow(summaryRecordEntry, gridCollection);
                        summaryRange = sheet.Range[excelRowIndex, excelColumnIndex];
                        ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.TableSummaryCell, summaryDisplayTextForRow, summaryRecordEntry, string.Empty, excelExportingOptions);
                    }
                    else
                    {
                        int tempExcelColumn = allowOutlining ? excelColumnIndex : (excelColumnIndex + gridCollection.GroupDescriptions.Count);
                        
                        foreach (GridSummaryColumn summaryColumn in summaryRecordEntry.SummaryRow.SummaryColumns)
                        {
                            var gridColumn = grid.Columns.FirstOrDefault(s => s.MappingName == summaryColumn.MappingName);
                            int columnIndex = grid.ResolveSummaryColumnIndex(summaryColumn.MappingName, excelExportingOptions.ExcludeColumns);

                            if (columnIndex < 0)
                                continue;

                            summaryRange = sheet.Range[excelRowIndex, tempExcelColumn + columnIndex];
                            summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, gridColumn.MappingName, gridCollection);
                            ExportSummaryDisplayTextToExcel(grid, summaryRange, ExportCellType.TableSummaryCell, summaryDisplayTextForRow, summaryRecordEntry, gridColumn.MappingName, excelExportingOptions);
                        }

                        if (allowOutlining)
                            summaryRange = sheet.Range[excelRowIndex, tempExcelColumn, excelRowIndex, (excelColumnIndex + excelExportingOptions.ColumnCount) - 1];                        
                        else
                            summaryRange = sheet.Range[excelRowIndex, tempExcelColumn, excelRowIndex, ((excelColumnIndex + excelExportingOptions.ColumnCount) + grid.View.GroupDescriptions.Count) - 1];                        
                    }

                    ExportCellStyle(summaryRange, args.CellStyle);
                }
            }
        }

        private static void ExportSummaryDisplayTextToExcel(SfDataGrid grid, IRange summaryRange, ExportCellType exportCellType, string summaryDisplayText, object exportNodeEntry, string columnName, ExcelExportingOptions excelExportingOptions)
        {
            GridCellExcelExportingEventHandler cellsExportingEventHandler = excelExportingOptions.CellsExportingEventHandler;

            GridCellExcelExportingEventArgs cellArgs = new GridCellExcelExportingEventArgs(summaryRange, exportCellType, summaryDisplayText, exportNodeEntry, columnName,
                               excelExportingOptions.GridViewDefinition, excelExportingOptions.ChildLevel, grid, excelExportingOptions.ExportMode, null);

            if (cellsExportingEventHandler != null)
                cellsExportingEventHandler(grid, cellArgs);

            if (!cellArgs.Handled)
                summaryRange.Text = cellArgs.CellValue.ToString();
        }

        /// <summary>
        /// This Method exports the header cells to excel.
        /// </summary>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="sheet">IWorkSheet</param>
        /// <param name="columnIndex">ColumnIndex, Specifies the First column index by considering indent space.</param>
        /// <param name="excelExportingOptions">Class <see cref:ExcelExportingOptions> Which holds the information of ExportingOptions.</see></param>
        internal static void ExportHeadersToExcel(SfDataGrid grid, IWorksheet sheet, int columnIndex, ExcelExportingOptions excelExportingOptions)
        {
            IEnumerable<string> gridColumns = from column in grid.Columns
                                              where !excelExportingOptions.ExcludeColumns.Contains(column.MappingName)
                                              select column.HeaderText != null ? column.HeaderText : column.MappingName;

            GridExcelExportingEventhandler exportingHandler = excelExportingOptions.ExportingEventHandler;
            GridCellExcelExportingEventHandler cellsExportingHandler = excelExportingOptions.CellsExportingEventHandler;

            GridExcelExportingEventArgs args = new GridExcelExportingEventArgs(sheet, ExportCellType.HeaderCell, new ExportCellStyle(),excelExportingOptions.ChildLevel,grid);            

            if (exportingHandler != null)            
                exportingHandler(grid, args);            

            if (!args.Handled)
            {
                foreach (string str in gridColumns)
                {
                    IRange exportRange = sheet.Range[excelRowIndex, columnIndex];
                    exportRange = exportRange.IsMerged ? exportRange.MergeArea : exportRange;
                    GridCellExcelExportingEventArgs cellArgs = new GridCellExcelExportingEventArgs(exportRange, ExportCellType.HeaderCell, str, null, str,
                        excelExportingOptions.GridViewDefinition,excelExportingOptions.ChildLevel, grid, excelExportingOptions.ExportMode, null);

                    if (cellsExportingHandler != null)                   
                        cellsExportingHandler(grid, cellArgs);
                    
                    if (!cellArgs.Handled)                    
                        exportRange.Text = cellArgs.CellValue.ToString();
                    
                    ExportCellStyle(exportRange, args.CellStyle);
                    columnIndex++;
                }
            }
            else
            {
                excelRowIndex--;
            }
        }

        /// <summary>
        /// Exports the ChildGrid To Excel with Grouping.
        /// </summary>
        /// <param name="grid">SfDataGrid, Corresponding ChildGrid</param>
        /// <param name="workSheet">IWorkSheet</param>
        /// <param name="gridCollectionView">ICollectionViewAdv</param>
        /// <param name="excelExportingOptions">Class <see cref:"ExcelExportingOptions"> which holds the exporting Options.</see></param>
        internal static void ExportChildGridToExcel(SfDataGrid grid, IWorksheet workSheet, ICollectionViewAdv gridCollectionView, ExcelExportingOptions excelExportingOptions)
        {
            if (gridCollectionView.Records.Count == 0)
                return;

            GridCellExcelExportingEventHandler cellsExportingEventHandler=excelExportingOptions.CellsExportingEventHandler;
#if WPF
            PropertyDescriptorCollection itemProperties = gridCollectionView.GetItemProperties();
#else
            PropertyInfoCollection itemProperties= gridCollectionView.GetItemProperties();
#endif

            if (grid.Columns.Count == 0)
            {
#if SILVERLIGHT
                foreach (var itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (!typeof(IEnumerable).IsAssignableFrom(itemProperty.Value.PropertyType))
                        grid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty.Value as PropertyInfo).Name });
                }
#elif WinRT
                foreach (var itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (itemProperty.Value.PropertyType != typeof(string) && !typeof(IEnumerable).GetTypeInfo().IsAssignableFrom((itemProperty.Value.PropertyType).GetTypeInfo()))
                        grid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty.Value as PropertyInfo).Name });
                }
#else
                foreach (PropertyDescriptor itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (!typeof(IEnumerable).IsAssignableFrom(itemProperty.PropertyType))
                        grid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty as PropertyDescriptor).Name });
                }
#endif
            }

            IEnumerable<string> gridColumns = from column in grid.Columns
                                              where !excelExportingOptions.ExcludeColumns.Contains(column.MappingName)
                                              select column.MappingName;

            excelExportingOptions.ColumnCount = gridColumns.Count();
            IPropertyAccessProvider propertyAccessProvider = gridCollectionView.GetPropertyAccessProvider();
            
            int startColumnIndex = excelExportingOptions.StartColumnIndex;

            excelRowIndex++;
            ExportHeadersToExcel(grid, workSheet, startColumnIndex, excelExportingOptions);
            ExportTableSummariesToExcel(grid, workSheet, gridCollectionView, excelExportingOptions, TableSummaryRowPosition.Top);
            if (gridCollectionView.GroupDescriptions.Count > 0)
            {
                foreach (Group group in gridCollectionView.TopLevelGroup.Groups)
                {
                    ExportGroupToExcel(gridCollectionView, grid, workSheet, group, excelExportingOptions);
                }
            }
            else
            {
                IEnumerable records = gridCollectionView.Records;
                ExportRecordsToExcel(grid, records, propertyAccessProvider, workSheet, excelExportingOptions, null);
            }
            ExportTableSummariesToExcel(grid, workSheet, gridCollectionView, excelExportingOptions, TableSummaryRowPosition.Bottom);
        }

        /// <summary>
        /// Sets the ColumnWidth for the specified cells as 20.
        /// </summary>
        /// <param name="sheet">IWorkSheet</param>
        /// <param name="start">Start: Specifies the start indent column index</param>
        /// <param name="end">end : Specifies the End column index of indent cells</param>
        internal static void SetIndentColumnWidth(IWorksheet sheet, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                sheet.SetColumnWidthInPixels(i, 20);
            }
        }

        /// <summary>
        /// Sets the column width from Grid To Excel for all columns starting from specified column index.        
        /// </summary>
        /// <param name="grid">SfDataGrid</param>
        /// <param name="sheet">IWorksheet</param>
        /// <param name="columnIndex">ColumnIndex: Specifies the First Column index</param>
        internal static void SetColumnWidth(SfDataGrid grid, IWorksheet sheet, int columnIndex, List<string> excludeColumns)
        {
            IEnumerable<string> gridColumns = from column in grid.Columns where !excludeColumns.Contains(column.MappingName) select column.MappingName;

            foreach (string str in gridColumns)
            {
                if (!Double.IsNaN(grid.Columns[str].ActualWidth))
                {
                    int actualWidth = (int)grid.Columns[str].ActualWidth;
                    sheet.SetColumnWidthInPixels(columnIndex, actualWidth);
                }
                columnIndex++;
            }
        }

        /// <summary>
        /// Set the background, foreground, font info from ExcelExportingEventArgs.
        /// </summary>
        /// <param name="excelRange">IRange</param>
        /// <param name="exportCellStyle">ExportCellStyle <see cref:"ExportCellStyle"> Holds the background, foreground, font info</see></param>
        internal static void ExportCellStyle(IRange excelRange, ExportCellStyle exportCellStyle)
        {
            excelRange = excelRange.IsMerged ? excelRange.MergeArea : excelRange;

#if WinRT || SILVERLIGHT

            if (exportCellStyle.BackGroundBrush != null)
                excelRange.CellStyle.Color = (exportCellStyle.BackGroundBrush as SolidColorBrush).Color;

            if (exportCellStyle.ForeGroundBrush != null)
                excelRange.CellStyle.Font.RGBColor = (exportCellStyle.ForeGroundBrush as SolidColorBrush).Color;
#else
            if (exportCellStyle.BackGroundBrush != null)
                excelRange.CellStyle.Color = exportCellStyle.BackGroundBrush.ConvertColor();

            if (exportCellStyle.ForeGroundBrush != null)
                excelRange.CellStyle.Font.RGBColor = exportCellStyle.ForeGroundBrush.ConvertColor();
#endif

            if (exportCellStyle.FontInfo != null)
            {
                excelRange.CellStyle.Font.Bold = exportCellStyle.FontInfo.Bold;
                excelRange.CellStyle.Font.Italic = exportCellStyle.FontInfo.Italic;
                excelRange.CellStyle.Font.Underline = exportCellStyle.FontInfo.Underline;
                excelRange.CellStyle.Font.Size = exportCellStyle.FontInfo.Size;
                excelRange.CellStyle.Font.FontName = exportCellStyle.FontInfo.FontName;
            }
        }

        /// <summary>
        /// Exports the cell value from datagrid to excel.
        /// </summary>
        /// <param name="excelRange">IRange - to which the cellvalue is to be exported.</param>
        /// <param name="propertyAccessProvider">PropertyAccessProvider is to get the value</param>
        /// <param name="record">the record entry</param>
        /// <param name="cellValue">Object which holds the value that is exported to excel</param>
        /// <param name="gridColumn">the GridColumn</param>
        internal static void ExportCellValueToExcel(IRange excelrange, IPropertyAccessProvider propertyAccessProvider, object record, object cellValue, GridColumn gridColumn)
        {
            if (gridColumn is GridDateTimeColumn)
            {
                var column = gridColumn as GridDateTimeColumn;
                DateTime columnValue;
                DateTime.TryParse((cellValue != null ? cellValue.ToString() : string.Empty), out columnValue);
#if WinRT
                DateTime.TryParse(cellValue.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out columnValue);
                excelrange.DateTime = columnValue;
                excelrange.CellStyle.NumberFormat = GridFormatConversionHelper.GetExcelNumberFormat(gridColumn, (gridColumn as GridDateTimeColumn).FormatString);
#else
                excelrange.DateTime = columnValue;
                switch (column.Pattern)
                {
                    case DateTimePattern.ShortDate:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.ShortDatePattern;
                        break;
                    case DateTimePattern.LongDate:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.LongDatePattern;
                        break;
                    case DateTimePattern.LongTime:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.LongTimePattern;
                        break;
                    case DateTimePattern.ShortTime:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.ShortTimePattern;
                        break;
                    case DateTimePattern.FullDateTime:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.FullDateTimePattern;
                        break;
                    case DateTimePattern.RFC1123:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.RFC1123Pattern;
                        break;
                    case DateTimePattern.SortableDateTime:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.SortableDateTimePattern;
                        break;
                    case DateTimePattern.UniversalSortableDateTime:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.UniversalSortableDateTimePattern;
                        break;
                    case DateTimePattern.YearMonth:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.YearMonthPattern;
                        break;
                    case DateTimePattern.MonthDay:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.MonthDayPattern;
                        break;
                    default:
                        excelrange.CellStyle.NumberFormat = column.DateTimeFormat.ShortDatePattern;
                        break;
                }
#endif
            }
            else if (gridColumn is GridNumericColumn)
            {
#if WinRT
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                excelrange.NumberFormat = GridFormatConversionHelper.GetExcelNumberFormat(gridColumn, (gridColumn as GridNumericColumn).FormatString);
#else
                NumberFormatInfo numericNumberFormatInfo = GridFormatConversionHelper.GetNumberFormatInfo(gridColumn);
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                excelrange.CellStyle.NumberFormat = GridFormatConversionHelper.ConvertNumberFormatToExcel(numericNumberFormatInfo, gridColumn);
#endif
            }
#if !WinRT
            else if (gridColumn is GridCurrencyColumn)
            {
                NumberFormatInfo currencyNumberFormatInfo = GridFormatConversionHelper.GetNumberFormatInfo(gridColumn);
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                excelrange.CellStyle.NumberFormat = GridFormatConversionHelper.ConvertNumberFormatToExcel(currencyNumberFormatInfo, gridColumn);
            }
            else if (gridColumn is GridPercentColumn)
            {
                NumberFormatInfo percentFormatInfo = GridFormatConversionHelper.GetNumberFormatInfo(gridColumn);
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                excelrange.CellStyle.NumberFormat = GridFormatConversionHelper.ConvertNumberFormatToExcel(percentFormatInfo, gridColumn);
            }
            else if (gridColumn is GridTimeSpanColumn)
            {
                TimeSpan columnValue;
                TimeSpan.TryParse((cellValue != null ? cellValue.ToString() : string.Empty), out columnValue);
                excelrange.TimeSpan = columnValue;
                excelrange.CellStyle.NumberFormat = (gridColumn as GridTimeSpanColumn).Format;
            }
            else if (gridColumn is GridMaskColumn)
            {
                var column = gridColumn as GridMaskColumn;
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                if (!string.IsNullOrEmpty(column.Mask))
                {
                    string format = column.Mask;
                    format = format.Replace('9', '0');
                    format = format.Replace('#', '0');
                    format = format.Replace(':', '.');
                    excelrange.CellStyle.NumberFormat = format;
                }
            }
#endif
            else if (gridColumn is GridHyperlinkColumn)
            {
                IHyperLink excelLink = excelrange.Worksheet.HyperLinks.Add(excelrange);
                excelLink.Type = ExcelHyperLinkType.Url;
                if (gridColumn.DisplayBinding.Path != gridColumn.ValueBinding.Path)
                {
                    if (gridColumn.DisplayBinding.Converter != null)
                        excelLink.TextToDisplay = gridColumn.DisplayBinding.Converter.Convert(cellValue, null, null, null).ToString();
                    else
                        excelLink.TextToDisplay = cellValue != null ? cellValue.ToString() : string.Empty;

                    object bindedValue = propertyAccessProvider.GetValue((record is RecordEntry) ? (record as RecordEntry).Data : record, gridColumn.ValueBinding.Path.Path);
                    excelLink.Address = bindedValue.ToString();                    
                }
                else
                {
                    excelLink.TextToDisplay = cellValue != null ? cellValue.ToString() : string.Empty;
                    excelLink.Address = cellValue != null ? cellValue.ToString() : string.Empty;
                }
            }
            else if (gridColumn is GridImageColumn)
            {
                ExportImageColumnToExcel(excelrange, cellValue);
            }
#if WinRT
            else if (gridColumn is GridUpDownColumn)
            {
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
                excelrange.NumberFormat = GridFormatConversionHelper.GetExcelNumberFormat(gridColumn, (gridColumn as GridUpDownColumn).FormatString);
            }
#endif
            else
                excelrange.Value = cellValue != null ? cellValue.ToString() : string.Empty;
        }

#if WinRT
       internal async static void ExportImageColumnToExcel(IRange excelRange,object cellValue)
#else
        internal static void ExportImageColumnToExcel(IRange excelRange,object cellValue)
#endif
        {
            if (cellValue != null)
                {
                    BitmapImage image = (cellValue as BitmapImage);
                    Uri uri = image.UriSource;
                    int schemaLength = uri.Scheme.Length;
                    string uriString = uri.OriginalString;
                    uriString = uriString.Insert(schemaLength + 1, "//");
                    uri = new Uri(uriString);
                    Stream stream;
#if WinRT
                    var srcfile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    stream = await srcfile.OpenStreamForReadAsync();
#else
                    StreamResourceInfo streamInfo = Application.GetResourceStream(uri);
                    stream = streamInfo.Stream;
#endif
                    excelRange.Worksheet.Pictures.AddPicture(excelRange.Row, excelRange.Column, stream);
                }
        }


        #endregion
    }

    /// <summary>
    /// Class which is used to set the ExportingOptions. 
    /// user can pass this class as an argument to the ExportToExcelMethod.
    /// </summary>
    public class ExcelExportingOptions
    {
        private bool allowOutlining=true;
        private GridCellExcelExportingEventHandler cellsExportingEventHandler;
        private Syncfusion.XlsIO.ExcelVersion excelVersion=ExcelVersion.Excel2007;
        private bool exportAllPages=false;
        private GridExcelExportingEventhandler exportingEventHandler;
        private GridChildExportingEventHandler childExportingEventHandler;
        private int startColumnIndex=1;
        private int startRowIndex=1;
        private int childLevel = 0;
        private int columnCount = 0;
        private ViewDefinition viewDefinition = null;
        public List<string> ExcludeColumns = new List<string>();
        private ExportPageOptions exportPageOptions = ExportPageOptions.ExportToSingleSheet;
        private ExportMode exportMode = ExportMode.Value;

        public ExcelExportingOptions()
        {
            
        }

        public ExcelExportingOptions(Syncfusion.XlsIO.ExcelVersion ExcelVersion, bool ExportAllPages, bool AllowOutlining, GridExcelExportingEventhandler ExportingEventHandler, GridCellExcelExportingEventHandler CellsExportingEventHandler)
        {            
            this.excelVersion = ExcelVersion;
            this.exportAllPages = ExportAllPages;
            this.allowOutlining = AllowOutlining;
            this.exportingEventHandler = ExportingEventHandler;
            this.cellsExportingEventHandler = CellsExportingEventHandler;
        }

        /// <summary>
        /// Specifies whether the groups should export with expand/collapse options or not.
        /// There is no indent space maintained if AllowOutlining is set to true.
        /// If the grid contains the Details View definition, the AllowOutlining is set to true internally.
        /// </summary>
        public bool AllowOutlining
        {
            get{ return this.allowOutlining; }
            set{ this.allowOutlining = value; }
        }

        /// <summary>
        /// Delegate handler which is used to handle or customize the exporting of a particular cell in Excel.
        /// </summary>
        public GridCellExcelExportingEventHandler CellsExportingEventHandler
        {
            get{ return this.cellsExportingEventHandler; }
            set{ this.cellsExportingEventHandler = value; }
        }
        
        /// <summary>
        /// Delegate handler which is used to customize the exporting of Details View.
        /// </summary>
        public GridChildExportingEventHandler ChildExportingEventHandler
        {
            get{return this.childExportingEventHandler;}
            set{this.childExportingEventHandler = value;}
        }

        /// <summary>
        /// Exports data to Excel in the specificed workbook version
        /// </summary>
        public Syncfusion.XlsIO.ExcelVersion ExcelVersion
        {
            get{return this.excelVersion;}
            set{this.excelVersion = value;}
        }

        /// <summary>
        /// Specifies whether the method should export all pages for PagedCollection. By default, it exports the first page only.
        /// This option is not supported with OnDemandPaging.
        /// </summary>
        public bool ExportAllPages
        {
            get{return this.exportAllPages;}
            set{this.exportAllPages = value;}
        }

        /// <summary>
        /// Delegate handler which is used to customize the styles for Header, Table Summary, Group Summary and Caption Summary rows.
        /// </summary>
        public GridExcelExportingEventhandler ExportingEventHandler
        {
            get{return this.exportingEventHandler;}
            set{this.exportingEventHandler = value;}
        }

        /// <summary>
        /// Exports data to the specified column index in Excel.
        /// </summary>
        public int StartColumnIndex
        {
            get{return this.startColumnIndex;}
            set{this.startColumnIndex = value;}
        }

        /// <summary>
        /// Exports data to to the specified row index in Excel
        /// </summary>
        public int StartRowIndex
        {
            get{return this.startRowIndex;}
            set{this.startRowIndex = value;}
        }

        internal int ChildLevel
        {
            get{return this.childLevel;}
            set{this.childLevel = value;}
        }

        internal int ColumnCount
        {
            get { return this.columnCount; }
            set { this.columnCount = value; }
        }

        internal ViewDefinition GridViewDefinition
        {
            get { return viewDefinition; }
            set { viewDefinition = value; }
        }

        /// <summary>
        /// Specifies how the paged collection to be exported into excel
        /// </summary>
        public ExportPageOptions ExportPageOptions
        {
            get { return exportPageOptions; }
            set { exportPageOptions = value; }
        }

        /// <summary>
        /// Specifies how the data to be exported into excel
        /// </summary>
        public ExportMode ExportMode
        {
            get { return exportMode; }
            set { exportMode = value; }
        }
    }

    public enum ExportMode
    {
        /// <summary>
        /// Exports the FormattedText to Excel
        /// </summary>
        Text = 0,

        /// <summary>
        /// Exports the Format into Excel by setting Excel number format.
        /// </summary>
        Value = 1
    }

    public enum ExportPageOptions
    {
        /// <summary>
        /// Export paged collection to Single sheet
        /// </summary>
        ExportToSingleSheet = 0,

        /// <summary>
        /// Export paged collection to different sheets based on page by page (For this case, Grouping can be considered).
        /// </summary>
        ExportToDifferentSheets = 1
    }
}
