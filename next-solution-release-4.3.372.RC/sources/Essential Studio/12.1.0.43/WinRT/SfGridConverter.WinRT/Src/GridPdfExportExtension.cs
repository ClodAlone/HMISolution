#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;

#if WinRT
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Globalization;
using Syncfusion.UI.Xaml.Controls.Input;
using System.Reflection;
using System.Drawing;
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
using System.Drawing;
#endif

#if WPF
using System.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    public static class GridPdfExportExtension
    {
        static int startColIndex = 0;
        static PdfPen normalBorder = new PdfPen(PdfBrushes.DarkGray, 0.2f);
        static PdfPen emptyBorder = new PdfPen(PdfBrushes.Transparent, 0f);

        /// <summary>
        /// This method exports the dataGrid to PDF.
        /// </summary>
        /// <param name="sfgrid"> SfDataGrid </param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdf(this SfDataGrid sfgrid)
        {
            return ExportToPdf(sfgrid, sfgrid.View, new PdfExportingOptions());
        }

        /// <summary>
        /// This method exports the dataGrid to PDF.
        /// </summary>
        /// <param name="sfgrid"> SfDataGrid </param>
        /// <param name="pdfExportingOptions"> Class <see cref="PdfExportingOptions"> which is used to set the exporting options </see></param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdf(this SfDataGrid sfgrid, PdfExportingOptions pdfExportingOptions)
        {
            return ExportToPdf(sfgrid, sfgrid.View, pdfExportingOptions);
        }

        /// <summary>
        /// This method exports the dataGrid to PDF.
        /// </summary>
        /// <param name="sfgrid"> SfDataGrid </param>
        /// <param name="gridCollectionView"> ICollectionViewAdv</param>
        /// <param name="pdfExportingOptions"> Class <see cref="PdfExportingOptions"> which is used to set the exporting options </see></param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdf(this SfDataGrid sfgrid, ICollectionViewAdv gridCollectionView, PdfExportingOptions pdfExportingOptions)
        {
            var pdfDocument = new PdfDocument();
            var page = pdfDocument.Pages.Add();
            var pdfGrid = ExportToPdfGrid(sfgrid, gridCollectionView, pdfExportingOptions);
            var format = new PdfGridLayoutFormat()
            {
                Layout = PdfLayoutType.Paginate,
                Break = PdfLayoutBreakType.FitPage
            };

            var headerfooterHandler = pdfExportingOptions.PageHeaderFooterEventHandler;
            var args = new PdfHeaderFooterEventArgs(sfgrid, page, pdfDocument.Template);

            if(headerfooterHandler != null)
                headerfooterHandler(pdfDocument, args);

            pdfGrid.Draw(page, new PointF(), format);

            return pdfDocument;
        }

        /// <summary>
        /// This method exports the dataGrid to PDF.
        /// </summary>
        /// <param name="sfgrid"> SfDataGrid </param>
        /// <param name="gridCollectionView"> ICollectionViewAdv</param>
        /// <param name="pdfExportingOptions"> Class <see cref="PdfExportingOptions"> which is used to set the exporting options </see></param>
        /// <returns>PdfGrid</returns>
        public static PdfGrid ExportToPdfGrid(this SfDataGrid sfgrid, ICollectionViewAdv gridCollectionView, PdfExportingOptions pdfExportingOptions)
        {
            var pdfGrid = new PdfGrid();

            //In Pdfgrid, horizontal overflow is not supported for child grid, so fit all columns in one page
            //when ExportDetailsView is true.
            if (pdfExportingOptions.ExportDetailsView)
                pdfExportingOptions.FitAllColumnsInOnePage = true;

            if (!pdfExportingOptions.FitAllColumnsInOnePage)
                pdfGrid.Style.AllowHorizontalOverflow = true;
            pdfGrid.Style.HorizontalOverflowType = PdfHorizontalOverflowType.NextPage;
            pdfGrid.Style.CellPadding = new PdfPaddings(3, 3, 2, 2);

            var propertyAccessProvider = gridCollectionView.GetPropertyAccessProvider();
            var gridColumns = from column in sfgrid.Columns
                              where !pdfExportingOptions.ExcludeColumns.Contains(column.MappingName) && !column.IsHidden
                              select column;

            if (pdfExportingOptions.RepeatHeaders)
                pdfGrid.RepeatHeader = true;

            //creates the PdfGridCellStyle for all celltypes
            pdfExportingOptions.ExportingEventArgs = new GridPdfExportingEventArgs(pdfGrid, sfgrid, 0);
            InitializeCellStyle(sfgrid, sfgrid.View, pdfExportingOptions);

            if (pdfExportingOptions.ExportGroups && (gridCollectionView.GroupDescriptions.Count > 0) && !(gridCollectionView is GridVirtualizingCollectionView)
                && (!pdfExportingOptions.ExportAllPages || !(gridCollectionView is PagedCollectionView)))
            {
                pdfGrid.Columns.Add(gridColumns.Count() + gridCollectionView.GroupDescriptions.Count);

                int pdfColIndex = startColIndex + gridCollectionView.GroupDescriptions.Count;

                //set the column width for indent cells
                for (int i = startColIndex; i < pdfColIndex; i++)
                {
                    pdfGrid.Columns[i].Width = 12f;
                }

                if (!pdfExportingOptions.AutoColumnWidth && !pdfExportingOptions.FitAllColumnsInOnePage)
                    SetColumnWidth(sfgrid, pdfGrid, gridColumns, pdfColIndex);

                ExportHeadersToPdf(sfgrid, pdfGrid, gridColumns, pdfColIndex, pdfExportingOptions);
                if (pdfExportingOptions.ExportTableSummary)
                {
                    pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Top;
                    ExportSummariesToPdf(sfgrid, pdfGrid, gridCollectionView, null, pdfExportingOptions);
                }
                foreach (Group group in gridCollectionView.TopLevelGroup.Groups)
                {
                    if (group.ItemsCount > 0)
                        ExportGroupToPdf(gridCollectionView, sfgrid, pdfGrid, group, pdfExportingOptions);
                }
            }
            else
            {
                pdfGrid.Columns.Add(gridColumns.Count());

                if (!pdfExportingOptions.AutoColumnWidth && !pdfExportingOptions.FitAllColumnsInOnePage)
                    SetColumnWidth(sfgrid, pdfGrid, gridColumns, startColIndex);

                ExportHeadersToPdf(sfgrid, pdfGrid, gridColumns, startColIndex, pdfExportingOptions);
                if (pdfExportingOptions.ExportTableSummary)
                {
                    pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Top;
                    ExportSummariesToPdf(sfgrid, pdfGrid, gridCollectionView, null, pdfExportingOptions);
                }
                IEnumerable records = gridCollectionView.Records;

                if (pdfExportingOptions.ExportAllPages && gridCollectionView is PagedCollectionView)
                {
                    if (!(gridCollectionView as PagedCollectionView).UseOnDemandPaging)
                    {
                        records = (gridCollectionView as PagedCollectionView).GetInternalList();
                        ExportRecordsToPdf(sfgrid, pdfGrid, records, propertyAccessProvider, gridColumns, null,
                                           pdfExportingOptions);
                    }
                    else
                    {
                        var pageCount = (gridCollectionView as PagedCollectionView).PageCount;
                        for (int i = 0; i < pageCount; i++)
                        {
                            records = (gridCollectionView as PagedCollectionView).GetInternalListForIndex(i);
                            ExportRecordsToPdf(sfgrid, pdfGrid, records, propertyAccessProvider, gridColumns, null,
                                               pdfExportingOptions);
                        }
                    }
                }
                else if (gridCollectionView is GridVirtualizingCollectionView)
                {
                    records = (gridCollectionView as GridVirtualizingCollectionView).GetInternalSource();
                    ExportRecordsToPdf(sfgrid, pdfGrid, records, propertyAccessProvider, gridColumns, null,
                                       pdfExportingOptions);
                }
                else
                    ExportRecordsToPdf(sfgrid, pdfGrid, records, propertyAccessProvider, gridColumns, null,
                                       pdfExportingOptions);

            }
            if (pdfExportingOptions.ExportTableSummary)
            {
                pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Bottom;
                ExportSummariesToPdf(sfgrid, pdfGrid, gridCollectionView, null, pdfExportingOptions);
            }
            return pdfGrid;
        }

        private static void InitializeCellStyle(SfDataGrid sfgrid, ICollectionViewAdv view, PdfExportingOptions pdfExportingOptions)
        {
            var exportingHandler = pdfExportingOptions.ExportingEventHandler;

            #region HeaderCellStyle
            pdfExportingOptions.ExportingEventArgs.HeaderCellStyle = new PdfGridCellStyle();
#if WPF
            var font = new Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            pdfExportingOptions.ExportingEventArgs.HeaderCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
            pdfExportingOptions.ExportingEventArgs.HeaderCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Bold);
#endif
            pdfExportingOptions.ExportingEventArgs.HeaderCellStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            pdfExportingOptions.ExportingEventArgs.HeaderCellStyle.Borders.All = normalBorder;

            pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.HeaderCell;
            pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.HeaderCellStyle;
            if (exportingHandler != null)
                exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);

            #endregion

            #region StackedHeaderCellStyle

            if (pdfExportingOptions.ExportStackedHeaders && sfgrid.StackedHeaderRows.Count > 0)
            {
                pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle = new PdfGridCellStyle();
#if WPF
                font = new Font("Segoe UI", 10f, System.Drawing.FontStyle.Regular);
                pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
                pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Regular);
#endif
                pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle.Borders.All = normalBorder;

                pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle;
                pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.StackedHeaderCell;
                if (exportingHandler != null)
                    exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);
            }

            #endregion

            #region RecordCellStyle
            pdfExportingOptions.ExportingEventArgs.RecordCellStyle = new PdfGridCellStyle();
#if WPF
            font = new Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular);
            pdfExportingOptions.ExportingEventArgs.RecordCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
            pdfExportingOptions.ExportingEventArgs.RecordCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Regular);
#endif
            pdfExportingOptions.ExportingEventArgs.RecordCellStyle.Borders.All = normalBorder;

            pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.RecordCell;
            pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.RecordCellStyle;
            if (exportingHandler != null)
                exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);

            #endregion

            #region IndentCellStyle
            pdfExportingOptions.ExportingEventArgs.IndentCellStyle = new PdfGridCellStyle
                {
                    Borders = {All = emptyBorder}
                };

            #endregion

            #region CaptionCellStyle
            if (pdfExportingOptions.ExportGroups && view.GroupDescriptions.Count > 0)
            {
                pdfExportingOptions.ExportingEventArgs.CaptionCellStyle = new PdfGridCellStyle();
#if WPF
                font = new Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular);
                pdfExportingOptions.ExportingEventArgs.CaptionCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
                pdfExportingOptions.ExportingEventArgs.CaptionCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9.5f, PdfFontStyle.Regular);
#endif
                pdfExportingOptions.ExportingEventArgs.CaptionCellStyle.Borders.All = normalBorder;

                pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.GroupCaptionCell;
                pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.CaptionCellStyle;
                if (exportingHandler != null)
                    exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);
            }
            #endregion

            #region GroupSummaryCellStyle
            if (pdfExportingOptions.ExportGroups && pdfExportingOptions.ExportGroupSummary && view.GroupDescriptions.Count > 0)
            {
                pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle = new PdfGridCellStyle();
#if WPF
                font = new Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular);
                pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
                pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9.5f, PdfFontStyle.Regular);
#endif
                pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle.Borders.All = normalBorder;

                pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.GroupSummaryCell;
                pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle;
                if (exportingHandler != null)
                    exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);
            }
            #endregion

            #region TableSummaryCellStyle
            if (pdfExportingOptions.ExportTableSummary && sfgrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom) > 0)
            {
                pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle = new PdfGridCellStyle();
#if WPF
                font = new Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular);
                pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
                pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9.5f, PdfFontStyle.Regular);
#endif
                pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle.Borders.All = normalBorder;

                pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.TableSummaryCell;
                pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle;
                if (exportingHandler != null)
                    exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);
            }
            #endregion

            #region TopTableSummaryCellStyle
            if (pdfExportingOptions.ExportTableSummary && sfgrid.GetTableSummaryCount(TableSummaryRowPosition.Top) > 0)
            {
                pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle = new PdfGridCellStyle();
#if WPF
                font = new Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular);
                pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle.Font = new PdfTrueTypeFont(font, true);
#else
                pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9.5f, PdfFontStyle.Regular);
#endif
                pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle.Borders.All = normalBorder;

                pdfExportingOptions.ExportingEventArgs.CellType = ExportCellType.TopTableSummaryCell;
                pdfExportingOptions.ExportingEventArgs.CellStyle = pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle;
                if (exportingHandler != null)
                    exportingHandler(sfgrid, pdfExportingOptions.ExportingEventArgs);
            }
            #endregion
        }

        internal static void ExportGroupToPdf(ICollectionViewAdv groupCollection, SfDataGrid sfgrid, PdfGrid pdfGrid, Group group, PdfExportingOptions pdfExportingOptions)
        {
            //Below code expands the Group if the group not expanded and Set the isExpanded flag as false.
            //if the flag isExpanded is false we collapse the group after exporting.
            var isExpanded = group.IsExpanded;
            if (!group.IsExpanded)
            {
                group.IsExpanded = true;
            }

            var propertyAccessProvider = groupCollection.GetPropertyAccessProvider();
            var gridColumns = from column in sfgrid.Columns
                              where !pdfExportingOptions.ExcludeColumns.Contains(column.MappingName) && !column.IsHidden
                              select column;

            if (group.IsGroups)
            {
                ExportGroupCaptionToPdf(sfgrid, pdfGrid, groupCollection, group, pdfExportingOptions);
                if (!group.IsBottomLevel)
                {
                    foreach (Group childGroup in group.Groups)
                    {
                        if (childGroup.ItemsCount > 0)
                            ExportGroupToPdf(groupCollection, sfgrid, pdfGrid, childGroup, pdfExportingOptions);
                    }
                }
                else
                {
                    ExportRecordsToPdf(sfgrid, pdfGrid, group.Records, propertyAccessProvider, gridColumns, group, pdfExportingOptions);
                    if (pdfExportingOptions.ExportGroupSummary)
                        ExportSummariesToPdf(sfgrid, pdfGrid, groupCollection, group, pdfExportingOptions);
                }
            }

            //collapsing the group if the flag isExpanded is false.
            if (!isExpanded)
            {
                group.IsExpanded = false;
            }
        }

        internal static void ExportGroupCaptionToPdf(SfDataGrid sfgrid, PdfGrid pdfGrid, ICollectionViewAdv groupCollection, Group group, PdfExportingOptions pdfExportingOptions)
        {
            //Add a new row for caption summary
            var row = pdfGrid.Rows.Add();

            if (!pdfExportingOptions.AutoRowHeight)
                row.Height = (float)(sfgrid.RowHeight / 1.4);

            var pdfColIndex = startColIndex + group.Level - 1;

            //set the border and column span for indent cells
            if (pdfColIndex != startColIndex)
            {
                row.Cells[startColIndex].Style = pdfExportingOptions.ExportingEventArgs.IndentCellStyle;
                row.Cells[startColIndex].ColumnSpan = pdfColIndex;
            }

            //Default Group caption format of SfDataGrid.
            var str = "{ColumnName} : {Key} - {ItemsCount} Items";

            string summaryDisplayTextForRow;
            PdfGridCell pdfGridCell = null;

            if (sfgrid.CaptionSummaryRow == null)
            {
                var propertyName = (groupCollection.GroupDescriptions[group.Level - 1] as PropertyGroupDescription).PropertyName;
                var columnHeaderText = sfgrid.Columns.FirstOrDefault(col => col.MappingName == propertyName).HeaderText;
                var groupCaptiontextFormat = sfgrid.GroupCaptionTextFormat ?? str;
                summaryDisplayTextForRow = groupCollection.TopLevelGroup.GetGroupCaptionText(group, groupCaptiontextFormat, columnHeaderText);

                pdfGridCell = row.Cells[pdfColIndex];
                pdfGridCell.Style = pdfExportingOptions.ExportingEventArgs.CaptionCellStyle;
                pdfGridCell.ColumnSpan = pdfGrid.Columns.Count - pdfColIndex;
                
                ExportSummaryDisplayTextToPdf(sfgrid, pdfGridCell, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow,
                                                  sfgrid.CaptionSummaryRow, string.Empty, pdfExportingOptions);
            }
            else if (sfgrid.CaptionSummaryRow.ShowSummaryInRow)
            {
                summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayTextForRow(group.SummaryDetails, groupCollection);
                pdfGridCell = row.Cells[pdfColIndex];
                pdfGridCell.Style = pdfExportingOptions.ExportingEventArgs.CaptionCellStyle;
                pdfGridCell.ColumnSpan = pdfGrid.Columns.Count - pdfColIndex;

                ExportSummaryDisplayTextToPdf(sfgrid, pdfGridCell, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow,
                                                  sfgrid.CaptionSummaryRow, string.Empty, pdfExportingOptions);
            }
            else
            {
                //set border style for caption summary row
                row.ApplyStyle(pdfExportingOptions.ExportingEventArgs.CaptionCellStyle);

                foreach (GridSummaryColumn summaryColumn in sfgrid.CaptionSummaryRow.SummaryColumns)
                {
                    var columnIndex = sfgrid.ResolveSummaryColumnIndex(summaryColumn.MappingName, pdfExportingOptions.ExcludeColumns);

                    if (columnIndex < 0)
                        continue;

                    summaryDisplayTextForRow = SummaryCreator.GetSummaryDisplayText(group.SummaryDetails, summaryColumn.MappingName, groupCollection);
                    pdfGridCell = row.Cells[columnIndex + groupCollection.GroupDescriptions.Count];

                    ExportSummaryDisplayTextToPdf(sfgrid, pdfGridCell, ExportCellType.GroupCaptionCell, summaryDisplayTextForRow,
                                                  sfgrid.CaptionSummaryRow, summaryColumn.MappingName, pdfExportingOptions);
                }
            }
        }

        internal static void ExportSummariesToPdf(SfDataGrid sfGrid, PdfGrid pdfGrid, ICollectionViewAdv gridCollection, Group group, PdfExportingOptions pdfExportingOptions)
        {
            string summaryDisplayText;
            PdfGridCell pdfGridCell = null;
            ExportCellType cellType;
            PdfGridCellStyle cellStyle;
            IList<SummaryRecordEntry> summaries = new List<SummaryRecordEntry>();
            if (group != null)
            {
                summaries = (group.Details as GroupRecordEntry).Summaries;
                cellType = ExportCellType.GroupSummaryCell;
                cellStyle = pdfExportingOptions.ExportingEventArgs.GroupsummaryCellStyle;
            }
            else
            {
                IEnumerable<GridSummaryRow> summaryRows;
                if (pdfExportingOptions.TableSummaryPosition == TableSummaryRowPosition.Top)
                {
                    summaryRows = sfGrid.GetTopTableSummaries();
                    cellType = ExportCellType.TopTableSummaryCell;
                    cellStyle = pdfExportingOptions.ExportingEventArgs.TopTablesummaryCellStyle;
                }
                else
                {
                    summaryRows = sfGrid.GetBottomTableSummaries();
                    cellType = ExportCellType.TableSummaryCell;
                    cellStyle = pdfExportingOptions.ExportingEventArgs.TablesummaryCellStyle;
                }
                summaryRows.ForEach(summaryRow =>
                {
                    summaries.Add(gridCollection.Records.TableSummaries.FirstOrDefault(record => record.SummaryRow == summaryRow));
                });
            }

            foreach (SummaryRecordEntry summaryRecordEntry in summaries)
            {
                //Add a new row for summary
                var row = pdfGrid.Rows.Add();

                if (!pdfExportingOptions.AutoRowHeight)
                    row.Height = (float)(sfGrid.RowHeight / 1.4);

                int pdfColIndex = startColIndex;
                if (pdfExportingOptions.ExportGroups && group != null)
                    pdfColIndex = startColIndex + gridCollection.GroupDescriptions.Count;

                if (pdfColIndex != startColIndex)
                {
                    row.Cells[startColIndex].Style = pdfExportingOptions.ExportingEventArgs.IndentCellStyle;
                    row.Cells[startColIndex].ColumnSpan = pdfColIndex;
                }

                if (!summaryRecordEntry.SummaryRow.ShowSummaryInRow)
                {
                    row.ApplyStyle(cellStyle);
                    if (group != null)
                        row.Cells[startColIndex].Style = pdfExportingOptions.ExportingEventArgs.IndentCellStyle;

                    foreach (var summaryColumn in summaryRecordEntry.SummaryRow.SummaryColumns)
                    {
                        var visibleColumn = sfGrid.Columns.FirstOrDefault(s => s.MappingName == summaryColumn.MappingName);
                        int columnIndex = sfGrid.ResolveSummaryColumnIndex(summaryColumn.MappingName, pdfExportingOptions.ExcludeColumns);

                        if (columnIndex < 0)
                            continue;

                        summaryDisplayText = SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, summaryColumn.MappingName, gridCollection);
                        if (pdfExportingOptions.ExportGroups && group == null && gridCollection.GroupDescriptions.Count > 0)
                        {
                            pdfGridCell = row.Cells[columnIndex + pdfColIndex + gridCollection.GroupDescriptions.Count];
                            row.Cells[startColIndex].ColumnSpan = pdfColIndex + gridCollection.GroupDescriptions.Count;
                        }
                        else
                            pdfGridCell = row.Cells[columnIndex + pdfColIndex];

                        ExportSummaryDisplayTextToPdf(sfGrid, pdfGridCell, cellType, summaryDisplayText,
                                                      summaryRecordEntry, summaryColumn.MappingName, pdfExportingOptions);
                    }
                }
                else
                {
                    summaryDisplayText = SummaryCreator.GetSummaryDisplayTextForRow(summaryRecordEntry, gridCollection);
                    pdfGridCell = row.Cells[pdfColIndex];
                    row.ApplyStyle(cellStyle);
                    if (group != null)
                        row.Cells[startColIndex].Style = pdfExportingOptions.ExportingEventArgs.IndentCellStyle;
                    pdfGridCell.ColumnSpan = pdfGrid.Columns.Count - pdfColIndex;

                    ExportSummaryDisplayTextToPdf(sfGrid, pdfGridCell, cellType, summaryDisplayText,
                                                      summaryRecordEntry, string.Empty, pdfExportingOptions);
                }
            }
        }

        internal static void ExportSummaryDisplayTextToPdf(SfDataGrid sfGrid, PdfGridCell pdfGridCell, ExportCellType exportCellType, string summaryDisplayText, object exportNodeEntry, string columnName, PdfExportingOptions pdfExportingOptions)
        {
            var cellExportingHandler = pdfExportingOptions.CellsExportingEventHandler;
            var cellArgs = new GridCellPdfExportingEventArgs(sfGrid, pdfGridCell, exportCellType, summaryDisplayText, exportNodeEntry, 
                                                            pdfExportingOptions.GridViewDefinition, pdfExportingOptions.ChildLevel, columnName, null);
            if (cellExportingHandler != null)
                cellExportingHandler(sfGrid, cellArgs);

            if (cellArgs.Handled)
                return;

            pdfGridCell.Value = cellArgs.CellValue;
        }

        internal static void ExportRecordsToPdf(SfDataGrid sfgrid, PdfGrid pdfGrid, IEnumerable records, IPropertyAccessProvider propertyAccessProvider, IEnumerable<GridColumn> gridColumns, Group group, PdfExportingOptions pdfExportingOptions)
        {
            int pdfColIndex;
            foreach (var rec in records)
            {
                var record = (rec is RecordEntry) ? (rec as RecordEntry).Data : rec;
                if (record == null)
                    continue;

                var row = pdfGrid.Rows.Add();

                if (!pdfExportingOptions.AutoRowHeight)
                    row.Height = (float)(sfgrid.RowHeight / 1.4);

                if (group != null)
                {
                    pdfColIndex = startColIndex + group.Level;
                    row.Cells[startColIndex].Style.Borders.All = emptyBorder;
                    row.Cells[startColIndex].ColumnSpan = pdfColIndex;
                }
                else
                    pdfColIndex = startColIndex;

                foreach (var column in gridColumns)
                {
                    var pdfGridCell = row.Cells[pdfColIndex];
                    pdfGridCell.Style = pdfExportingOptions.ExportingEventArgs.RecordCellStyle;
                    ExportCellValueToPdf(sfgrid, pdfGridCell, propertyAccessProvider, record, column, pdfExportingOptions);
                    pdfColIndex++;
                }

                #region DetailsView Exporting
                //below code is to export the details view
                if (sfgrid.DetailsViewDefinition.Count > 0 && pdfExportingOptions.ExportDetailsView)
                {
                    foreach (var definition in sfgrid.DetailsViewDefinition)
                    {
                        ICollectionViewAdv view = null;
                        var dataGrid = (definition as GridViewDefinition).DataGrid;
                        string relationalColumn = (definition as GridViewDefinition).RelationalColumn;

                        //creating new PdfExportingOptions for exporting child grid.
                        var childpdfExportingOptions = new PdfExportingOptions
                            {
                                ExportingEventHandler = pdfExportingOptions.ExportingEventHandler,
                                CellsExportingEventHandler = pdfExportingOptions.CellsExportingEventHandler,
                                ChildGridExportingEventHandler = pdfExportingOptions.ChildGridExportingEventHandler,
                                ExportAllDetails = pdfExportingOptions.ExportAllDetails,
                                ExportDetailsView = pdfExportingOptions.ExportDetailsView,
                                ExportAllPages = pdfExportingOptions.ExportAllPages,
                                ExportFormat = pdfExportingOptions.ExportFormat,
                                ExportGroups = pdfExportingOptions.ExportGroups,
                                ExportGroupSummary = pdfExportingOptions.ExportGroupSummary,
                                AutoRowHeight = pdfExportingOptions.AutoRowHeight,
                                ExportStackedHeaders = pdfExportingOptions.ExportStackedHeaders,
                                ExportTableSummary = pdfExportingOptions.ExportTableSummary,
                                FitAllColumnsInOnePage = pdfExportingOptions.FitAllColumnsInOnePage,
                                ExcludeColumns = pdfExportingOptions.ExcludeColumns,
                                RepeatHeaders = pdfExportingOptions.RepeatHeaders
                            };

                        var childArgs = new ChildGridPdfExportingEventArgs(rec as RecordEntry, relationalColumn,
                            pdfExportingOptions.ChildLevel + 1, sfgrid, childpdfExportingOptions);

                        if (pdfExportingOptions.ChildGridExportingEventHandler != null)
                            pdfExportingOptions.ChildGridExportingEventHandler(sfgrid, childArgs);

                        if (childArgs.Cancel)
                            continue;

                        //Below code creates a new CollectionViewAdv if ChildView is null.
                        bool isNewlyCreatedView = false;
                        if (rec is RecordEntry)
                        {
                            if ((rec as RecordEntry).ChildViews == null)
                            {
                                if (pdfExportingOptions.ExportAllDetails)
                                {
                                    view = dataGrid.CreateCollectionView((rec as RecordEntry).Data, relationalColumn, propertyAccessProvider);
                                    isNewlyCreatedView = true;
                                }
                                else
                                    continue;
                            }
                            else
                                view = (rec as RecordEntry).ChildViews[relationalColumn].View;
                        }
                        else if (pdfExportingOptions.ExportAllDetails)
                        {
                            view = dataGrid.CreateCollectionView(rec, relationalColumn, propertyAccessProvider);
                            isNewlyCreatedView = true;
                        }

                        if (view != null && view.Records.Count > 0)
                        {

                            childArgs.PdfExportingOptions.ChildLevel = pdfExportingOptions.ChildLevel + 1;
                            childArgs.PdfExportingOptions.GridViewDefinition = definition;
                            childArgs.PdfExportingOptions.FitAllColumnsInOnePage = true; 
                            ExportChildGridToPdf(dataGrid, pdfGrid, view, childArgs.PdfExportingOptions, group);
                        }
                        if (isNewlyCreatedView)
                            view.Dispose();
                    }
                }
                #endregion
            }
        }

        internal static void ExportChildGridToPdf(SfDataGrid sfgrid, PdfGrid pdfGrid, ICollectionViewAdv view, PdfExportingOptions pdfExportingOptions, Group group)
        {
            int columnIndex = startColIndex;
#if WPF
            PropertyDescriptorCollection itemProperties = view.GetItemProperties();
#else
            PropertyInfoCollection itemProperties = view.GetItemProperties();
#endif

            if (sfgrid.Columns.Count == 0)
            {
#if SILVERLIGHT
                foreach (var itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (!typeof(IEnumerable).IsAssignableFrom(itemProperty.Value.PropertyType))
                        sfgrid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty.Value as PropertyInfo).Name });
                }
#elif WinRT
                foreach (var itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (itemProperty.Value.PropertyType != typeof(string) && !typeof(IEnumerable).GetTypeInfo().IsAssignableFrom((itemProperty.Value.PropertyType).GetTypeInfo()))
                        sfgrid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty.Value as PropertyInfo).Name });
                }
#else
                foreach (PropertyDescriptor itemProperty in itemProperties)
                {
                    //below code skips the property, if property type is collection.                    
                    if (!typeof(IEnumerable).IsAssignableFrom(itemProperty.PropertyType))
                        sfgrid.Columns.Add(new GridTextColumn() { MappingName = (itemProperty as PropertyDescriptor).Name });
                }
#endif
            }

            var gridColumns = from column in sfgrid.Columns
                                                  where !pdfExportingOptions.ExcludeColumns.Contains(column.MappingName) && !column.IsHidden
                                                  select column;
            var propertyAccessProvider = view.GetPropertyAccessProvider();

            var childGrid = new PdfGrid();
            pdfExportingOptions.ExportingEventArgs = new GridPdfExportingEventArgs(childGrid, sfgrid, pdfExportingOptions.ChildLevel);
            InitializeCellStyle(sfgrid, view, pdfExportingOptions);

            childGrid.RepeatHeader = pdfExportingOptions.RepeatHeaders;

            if (pdfExportingOptions.ExportGroups && (view.GroupDescriptions.Count > 0))
            {
                childGrid.Columns.Add(gridColumns.Count() + view.GroupDescriptions.Count);

                int pdfColIndex = startColIndex + view.GroupDescriptions.Count;

                //set the column width for indent cells
                for (int i = startColIndex; i < pdfColIndex; i++)
                {
                    childGrid.Columns[i].Width = 12f;
                }

                if (!pdfExportingOptions.AutoColumnWidth && !pdfExportingOptions.FitAllColumnsInOnePage)
                    SetColumnWidth(sfgrid, pdfGrid, gridColumns, pdfColIndex);

                ExportHeadersToPdf(sfgrid, childGrid, gridColumns, pdfColIndex, pdfExportingOptions);
                if (pdfExportingOptions.ExportTableSummary)
                {
                    pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Top;
                    ExportSummariesToPdf(sfgrid, pdfGrid, view, null, pdfExportingOptions);
                }
                foreach (Group childGroup in view.TopLevelGroup.Groups)
                {
                    if (childGroup.ItemsCount > 0)
                        ExportGroupToPdf(view, sfgrid, childGrid, childGroup, pdfExportingOptions); ;
                }
            }
            else
            {
                childGrid.Columns.Add(gridColumns.Count());
                ExportHeadersToPdf(sfgrid, childGrid, gridColumns, startColIndex, pdfExportingOptions);
                if (pdfExportingOptions.ExportTableSummary)
                {
                    pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Top;
                    ExportSummariesToPdf(sfgrid, pdfGrid, view, null, pdfExportingOptions);
                }
                IEnumerable records = view.Records;
                ExportRecordsToPdf(sfgrid, childGrid, records, propertyAccessProvider, gridColumns, null, pdfExportingOptions);
            }

            if (pdfExportingOptions.ExportTableSummary)
            {
                pdfExportingOptions.TableSummaryPosition = TableSummaryRowPosition.Bottom;
                ExportSummariesToPdf(sfgrid, pdfGrid, view, null, pdfExportingOptions);
            }

            var row = pdfGrid.Rows.Add();
            //set the border and column span for indent cell
            if (group != null)
            {
                columnIndex = startColIndex + group.Level;
                row.Cells[startColIndex].Style = pdfExportingOptions.ExportingEventArgs.IndentCellStyle;
                row.Cells[startColIndex].ColumnSpan = columnIndex;
            }
            row.Cells[columnIndex].Value = childGrid;
            row.Cells[columnIndex].ColumnSpan = pdfGrid.Columns.Count - columnIndex;
            row.Cells[columnIndex].Style.Borders.All = normalBorder;
        }

        internal static void ExportHeadersToPdf(SfDataGrid sfgrid, PdfGrid pdfGrid, IEnumerable<GridColumn> gridColumns, int pdfColumnIndex, PdfExportingOptions pdfExportingOptions)
        {
            int headerCount = 1;
            int columnIndex = pdfColumnIndex;

            if (pdfExportingOptions.ExportStackedHeaders && sfgrid.StackedHeaderRows.Count > 0)
                headerCount = sfgrid.StackedHeaderRows.Count + 1;

            pdfGrid.Headers.Add(headerCount);
            
            for (int i = 0; i < headerCount; i++)
            {
                if (!pdfExportingOptions.AutoRowHeight)
                    pdfGrid.Headers[i].Height = (float)(sfgrid.HeaderRowHeight / 1.5);

                if (pdfExportingOptions.ExportStackedHeaders && i < sfgrid.StackedHeaderRows.Count)
                {
                    ExportStackedHeadersToPdf(sfgrid, pdfGrid, sfgrid.StackedHeaderRows[i], i, pdfExportingOptions, pdfColumnIndex);
                }
                else
                {
                    if (startColIndex != columnIndex)
                        pdfGrid.Headers[i].Cells[startColIndex].ColumnSpan = columnIndex + 1;

                    foreach (var column in gridColumns)
                    {
                        int index = columnIndex == pdfColumnIndex ? startColIndex : columnIndex;
                        var pdfHeaderCell = pdfGrid.Headers[i].Cells[index];
                        
                        //To set the row span for stacked headers
                        if (pdfExportingOptions.ExportStackedHeaders && sfgrid.StackedHeaderRows.Count > 0)
                        {
                            int gridColIndex = sfgrid.Columns.IndexOf(column) + sfgrid.ResolveToScrollColumnIndex(0);
                            int rowspan = sfgrid.GetHeightIncremeantationLimit(new CoveredCellInfo(gridColIndex, gridColIndex), i - 1);
                            if (rowspan > 0)
                            {
                                pdfHeaderCell = pdfGrid.Headers[i - rowspan].Cells[index];
                                pdfHeaderCell.RowSpan = rowspan + 1;

                                if (columnIndex == pdfColumnIndex)
                                    pdfHeaderCell.ColumnSpan = columnIndex + 1;
                            }
                        }

                        pdfHeaderCell.Style = pdfExportingOptions.ExportingEventArgs.HeaderCellStyle;
                        var value = column.HeaderText ?? column.MappingName;

                        var cellExportingHandler = pdfExportingOptions.CellsExportingEventHandler;
                        var cellArgs = new GridCellPdfExportingEventArgs(sfgrid, pdfHeaderCell,
                                                                         ExportCellType.HeaderCell,
                                                                         value, null,
                                                                         pdfExportingOptions.GridViewDefinition,
                                                                         pdfExportingOptions.ChildLevel,
                                                                         column.MappingName, null);
                        if (cellExportingHandler != null)
                            cellExportingHandler(sfgrid, cellArgs);

                        if (cellArgs.Handled)
                            continue;

                        pdfHeaderCell.Value = cellArgs.CellValue;
                        columnIndex++;
                    }
                }
            }
        }

        private static void ExportStackedHeadersToPdf(SfDataGrid sfgrid, PdfGrid pdfGrid, StackedHeaderRow stackedHeaderRow, int rowIndex, PdfExportingOptions pdfExportingOptions, int pdfColumnIndex)
        {
            var pdfheader = pdfGrid.Headers[rowIndex];
            pdfheader.ApplyStyle(pdfExportingOptions.ExportingEventArgs.StackedHeaderCellStyle);

            foreach (var column in stackedHeaderRow.StackedColumns)
            {
                int colIndex = stackedHeaderRow.StackedColumns.IndexOf(column);
                List<int> childSequence = sfgrid.GetChildSequence(column, rowIndex);
                childSequence.Sort();

                childSequence = childSequence.Except(sfgrid.IntersectedChildColumn(childSequence, stackedHeaderRow, column)).ToList();
                if (rowIndex - 1 >= 0)
                {
                    var newSequence = sfgrid.CheckChildSequence(childSequence, sfgrid.StackedHeaderRows[rowIndex - 1], column);
                    childSequence = newSequence.Intersect(childSequence).ToList();
                }

                var sequence = childSequence.GroupBy(num => childSequence.Where(candidate => candidate >= num)
                                      .OrderBy(candidate => candidate)
                                      .TakeWhile((candidate, index) => candidate == num + index)
                                      .Last())
                 .Select(seq => seq.OrderBy(num => num));
                foreach (var item in sequence)
                {
                    var columnList = item.ToList();
                    int right = columnList.Max() + sfgrid.ResolveToScrollColumnIndex(0);
                    int left = columnList.Min() + sfgrid.ResolveToScrollColumnIndex(0);

                    int firstcolumnIndex = left == pdfColumnIndex ? startColIndex : left;
                    int lastcolumnIndex = right;
                    if (!pdfExportingOptions.ExportGroups && sfgrid.View.GroupDescriptions.Count > 0)
                        lastcolumnIndex = right - sfgrid.ResolveToScrollColumnIndex(0);

                    //if (sfgrid.ShowRowHeader)
                    //{
                    //    firstcolumnIndex = (firstcolumnIndex - 1 == pdfColumnIndex) ? startColIndex : firstcolumnIndex--;
                    //    lastcolumnIndex--;
                    //}
                    int columnSpan = lastcolumnIndex - firstcolumnIndex + 1;

                    //To calculate the column index and column span if the columns are excluded.
                    if (pdfExportingOptions.ExcludeColumns.Count > 0)
                    {
                        int excludecolumnsCount = 0;
                        var excludeColumnsIndex = pdfExportingOptions.ExcludeColumns.Select(col => sfgrid.Columns.IndexOf(sfgrid.Columns[col]) + sfgrid.ResolveToScrollColumnIndex(0));
                        
                        //To adjust Pdfgrid columnIndex based on excluded columns and indent cells
                        if (firstcolumnIndex > 0)
                        {
                            excludecolumnsCount = excludeColumnsIndex.Count(index => Enumerable.Range(0, left).Contains(index));
                            firstcolumnIndex = firstcolumnIndex - excludecolumnsCount <= 0 ? 0 : firstcolumnIndex - excludecolumnsCount;
                        }
                        if (firstcolumnIndex != 0 && firstcolumnIndex - sfgrid.ResolveToScrollColumnIndex(0) <= 0)
                        {
                            firstcolumnIndex = 0;
                            columnSpan += sfgrid.ResolveToScrollColumnIndex(0);
                        }

                        //To adjust columnSpan based on excluded columns and indent cells
                        excludecolumnsCount = excludeColumnsIndex.Count(index => Enumerable.Range(left, right - left).Contains(index));
                        columnSpan = columnSpan - excludecolumnsCount;
                        if (left == pdfColumnIndex && columnSpan - left <= 0)
                            columnSpan = 0;
                    }

                    if (columnSpan > 0)
                    {
                        var pdfHeaderCell = pdfheader.Cells[firstcolumnIndex];
                        int rowSpan = sfgrid.GetHeightIncremeantationLimit(new CoveredCellInfo(left, right), rowIndex - 1);
                        if (rowSpan > 0)
                        {
                            pdfHeaderCell = pdfGrid.Headers[rowIndex - rowSpan].Cells[firstcolumnIndex];
                            pdfHeaderCell.RowSpan = rowSpan + 1;
                        }

                        pdfHeaderCell.ColumnSpan = columnSpan;

                        var value = column.HeaderText;
                        var cellExportingHandler = pdfExportingOptions.CellsExportingEventHandler;
                        var cellArgs = new GridCellPdfExportingEventArgs(sfgrid, pdfHeaderCell, ExportCellType.StackedHeaderCell,
                                                                     value, null, pdfExportingOptions.GridViewDefinition, pdfExportingOptions.ChildLevel, string.Empty, null);
                        if (cellExportingHandler != null)
                            cellExportingHandler(sfgrid, cellArgs);

                        if (cellArgs.Handled)
                            continue;

                        pdfHeaderCell.Value = cellArgs.CellValue;
                    }
                }
            }
        }

        internal static void SetColumnWidth(SfDataGrid sfgrid, PdfGrid pdfGrid, IEnumerable<GridColumn> gridColumns, int columnIndex)
        {
            foreach (var column in gridColumns)
            {
                var actualWidth = (float)(column.ActualWidth / 1.5);
                pdfGrid.Columns[columnIndex].Width = actualWidth;
                columnIndex++;
            }
        }

        internal static void ExportCellValueToPdf(SfDataGrid sfgrid, PdfGridCell pdfGridCell, IPropertyAccessProvider propertyAccessProvider, object record, GridColumn column, PdfExportingOptions pdfExportingOptions)
        {
            object value = null ;
            if (column is GridImageColumn)
            {
                ExportImageToPdf(sfgrid, pdfGridCell, propertyAccessProvider, record, column, pdfExportingOptions);
            }
            else
            {
                if (column is GridUnBoundColumn)
                {
                    value = sfgrid.GetUnBoundCellValue(column, record);
                }
                else if (column is GridHyperlinkColumn)
                {
                    value = propertyAccessProvider.GetValue(record, column.MappingName);
                    var style = pdfExportingOptions.ExportingEventArgs.RecordCellStyle.Clone() as PdfGridCellStyle;
                    style.TextBrush = PdfBrushes.Blue;
#if WPF
                    Font font = new Font(style.Font.Name, style.Font.Size, (System.Drawing.FontStyle.Regular | System.Drawing.FontStyle.Underline));
                    style.Font = new PdfTrueTypeFont(font);
#else
                    style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold);
#endif
                    pdfGridCell.Style = style;
                }
                else
                {
                    if (pdfExportingOptions.ExportFormat)
                        value = propertyAccessProvider.GetFormattedValue(record, column.MappingName);
                    else
                        value = propertyAccessProvider.GetValue(record, column.MappingName);
                }

                var cellExportingHandler = pdfExportingOptions.CellsExportingEventHandler;
                var cellArgs = new GridCellPdfExportingEventArgs(sfgrid, pdfGridCell, ExportCellType.RecordCell,
                                                             value, record, pdfExportingOptions.GridViewDefinition, pdfExportingOptions.ChildLevel, column.MappingName, propertyAccessProvider);
                if (cellExportingHandler != null)
                    cellExportingHandler(sfgrid, cellArgs);

                if (cellArgs.Handled)
                    return;

                pdfGridCell.Value = cellArgs.CellValue != null ? cellArgs.CellValue.ToString() : string.Empty;
            }
        }

#if WinRT
        internal async static void ExportImageToPdf(SfDataGrid sfgrid, PdfGridCell pdfGridCell, IPropertyAccessProvider propertyAccessProvider, object record, GridColumn column, PdfExportingOptions pdfExportingOptions)
#else
        internal static void ExportImageToPdf(SfDataGrid sfgrid, PdfGridCell pdfGridCell, IPropertyAccessProvider propertyAccessProvider, object record, GridColumn column, PdfExportingOptions pdfExportingOptions)
#endif
        {
            var cellvalue = propertyAccessProvider.GetValue(record, column.MappingName);
            var cellExportingHandler = pdfExportingOptions.CellsExportingEventHandler;
            var cellArgs = new GridCellPdfExportingEventArgs(sfgrid, pdfGridCell, ExportCellType.RecordCell,
                                                         cellvalue, record, pdfExportingOptions.GridViewDefinition, pdfExportingOptions.ChildLevel, column.MappingName, propertyAccessProvider);
            if (cellExportingHandler != null)
                cellExportingHandler(sfgrid, cellArgs);

            if (cellArgs.Handled)
                return;

            if (cellArgs.CellValue != null)
            {
                Uri uri = null;
                try
                {
                    if (cellArgs.CellValue is string)
                        uri = new Uri(@cellvalue.ToString(), UriKind.RelativeOrAbsolute);
                    else
                    {
                        var image = cellArgs.CellValue as BitmapImage;
                        uri = image.UriSource;
                    }
                    Stream stream;
#if WinRT
                    var srcfile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    stream = await srcfile.OpenStreamForReadAsync();
#else
                    StreamResourceInfo streamInfo = Application.GetResourceStream(uri);
                    stream = streamInfo.Stream;
#endif
                    var style = pdfExportingOptions.ExportingEventArgs.RecordCellStyle.Clone() as PdfGridCellStyle;
                    style.BackgroundImage = PdfImage.FromStream(stream);
                    style.Borders.All = normalBorder;
                    pdfGridCell.Style = style;
                }
                catch (Exception) { }
            }
        }

    }

    public class PdfExportingOptions
    {
        List<string> excludeColumns = new List<string>();
        bool repeatHeaders = true;
        bool autoColumnWidth = true;
        bool autoRowHeight = true;
        bool exportDetailsView = false;
        bool exportAllDetails = false;
        bool exportGroups = true;
        bool exportFormat = true;
        bool exportGroupSummary = true;
        bool exportTableSummary = true;
        bool exportAllPages = false;
        bool fitAllColumnsInOnePage = false;
        bool exportStackedHeaders = false;
        int childLevel;
        ViewDefinition viewDefinition;
        internal GridPdfExportingEventArgs ExportingEventArgs;
        internal TableSummaryRowPosition TableSummaryPosition; 
        public PdfExportingOptions()
        {

        }

        /// <summary>
        /// Gets or sets whether the headers can be repeated in each page or not
        /// </summary>
        public bool RepeatHeaders
        {
            get { return repeatHeaders; }
            set { repeatHeaders = value; }
        }

        /// <summary>
        /// Gets or sets the columns that needs to be excluded while exporting
        /// </summary>
        public List<string> ExcludeColumns
        {
            get { return excludeColumns; }
            set { excludeColumns = value; }
        }

        /// <summary>
        /// Gets or sets whether the column widths are automatically assigned or not.
        /// </summary>
        public bool AutoColumnWidth
        {
            get { return autoColumnWidth; }
            set { autoColumnWidth = value; }
        }

        /// <summary>
        /// Gets or sets whether the row heights can be exported or not.
        /// </summary>
        public bool AutoRowHeight
        {
            get { return autoRowHeight; }
            set { autoRowHeight = value; }
        }

        /// <summary>
        /// Gets or sets whether the values can be exported with format or not.
        /// </summary>
        public bool ExportFormat
        {
            get { return exportFormat; }
            set { exportFormat = value; }
        }

        /// <summary>
        /// Gets or sets whether the details view can be exported or not
        /// </summary>
        public bool ExportDetailsView
        {
            get { return exportDetailsView; }
            set { exportDetailsView = value; }
        }

        /// <summary>
        /// Gets or sets whether export all details view even the details view is not expanded.
        /// </summary>
        public bool ExportAllDetails
        {
            get { return exportAllDetails; }
            set { exportAllDetails = value; }
        }

        /// <summary>
        /// Gets or sets whether groups can be exported or not
        /// </summary>
        public bool ExportGroups
        {
            get { return exportGroups; }
            set { exportGroups = value; }
        }

        /// <summary>
        /// Gets or sets whether group summary can be exported or not
        /// </summary>
        public bool ExportGroupSummary
        {
            get { return exportGroupSummary; }
            set { exportGroupSummary = value; }
        }

        /// <summary>
        /// Gets or sets whether table summary can be exported or not
        /// </summary>
        public bool ExportTableSummary
        {
            get { return exportTableSummary; }
            set { exportTableSummary = value; }
        }

        /// <summary>
        /// Gets or sets whether all pages to be exported or not on PagedCollection
        /// </summary>
        public bool ExportAllPages
        {
            get { return exportAllPages; }
            set { exportAllPages = value; }
        }

        /// <summary>
        /// Gets or sets whether the all columns should be fit on a page or not
        /// <value>true</value>
        /// </summary>
        public bool FitAllColumnsInOnePage
        {
            get { return fitAllColumnsInOnePage; }
            set { fitAllColumnsInOnePage = value; }
        }

        /// <summary>
        /// Gets or sets whether stacked headers can be exported or not
        /// </summary>
        public bool ExportStackedHeaders
        {
            get { return exportStackedHeaders; }
            set { exportStackedHeaders = value; }
        }

        /// <summary>
        /// Gets or sets the level of Childgrid
        /// </summary>
        internal int ChildLevel
        {
            get { return this.childLevel; }
            set { this.childLevel = value; }
        }

        /// <summary>
        /// Gets or sets the ViewDefinition of ChildGrid
        /// </summary>
        internal ViewDefinition GridViewDefinition
        {
            get { return viewDefinition; }
            set { viewDefinition = value; }
        }

        /// <summary>
        /// Delegate handler which triggers while exporting grid and is used to customize the Headers, 
        /// Table Summaries, Group Summaries and Group captions
        /// </summary>
        public GridPdfExportingEventhandler ExportingEventHandler { get; set; }

        /// <summary>
        /// Delegate handler which triggers for each cell and is used to handle or customize the cell.
        /// </summary>
        public GridCellPdfExportingEventhandler CellsExportingEventHandler { get; set; }

        /// <summary>
        /// Delegate handler which triggers while exporting Details view and is used to handle the exporting details view.
        /// </summary>
        public ChildGridPdfExportingEventhandler ChildGridExportingEventHandler { get; set; }

        /// <summary>
        /// Delegate handler which triggers for allowing the user to add header and footer of the page.
        /// </summary>
        public PdfHeaderFooterEventHandler PageHeaderFooterEventHandler { get; set; }
    }
}
