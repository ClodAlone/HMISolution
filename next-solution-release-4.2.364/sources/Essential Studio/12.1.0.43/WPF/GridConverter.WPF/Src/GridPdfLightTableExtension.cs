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
using Syncfusion.Pdf.Tables;
using Syncfusion.Pdf;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Graphics;
using System.Drawing;

namespace Syncfusion.Windows.Controls.Grid.Converter
{
    public static partial class GridPdfExportExtension
    {              
        /// <summary>
        /// Exports the Grid to PdfLightTable
        /// </summary>
        /// <param name="grid"><see cref="GridModel"/> </param>
        /// <param name="range">Specifies the range to be exported to pdf. <see cref="GridRangeInfo"/> </param>
        /// <param name="filename">FileName, which used to save the pdfdocument after exporting.</param>
        public static void ExportToPdfLightTableDocument(this GridModel grid, GridRangeInfo range, string filename)
        {            
            ExportToPdfLightTableDocument(grid, range, null, null, filename);            
        }

        /// <summary>
        /// Exports the Grid to PdfLightTable
        /// </summary>
        /// <param name="grid"><see cref="GridModel"/> </param>
        /// <param name="range">Specifies the range to be exported to pdf. <see cref="GridRangeInfo"/> </param>
        /// <param name="headerHandler">Delegate event handler of type DrawPdfHeaderFooterEventHandler, which fires before drawing header.</param>
        /// <param name="footerHandler">Delegate event handler of type DrawPdfHeaderFooterEventHandler, which fires before drawing footer.</param>
        /// <param name="filename">FileName, which used to save the pdfdocument after exporting.</param>
        public static void ExportToPdfLightTableDocument(this GridModel grid, GridRangeInfo range, DrawPdfHeaderFooterEventHandler headerHandler, DrawPdfHeaderFooterEventHandler footerHandler, string filename)
        {
            PdfDocument document = ExportToPdfLightTableDocument(grid, range, headerHandler, footerHandler);
            document.Save(filename);
        }

        /// <summary>
        /// Exports the Grid to PdfLightTable
        /// </summary>
        /// <param name="grid"><see cref="GridModel"/> </param>
        /// <param name="range">Specifies the range to be exported to pdf. <see cref="GridRangeInfo"/> </param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfLightTableDocument(this GridModel grid, GridRangeInfo range)
        {
            return ExportToPdfLightTableDocument(grid, range, null, null);
        }

        /// <summary>
        /// Exports the Grid to PdfLightTable
        /// </summary>
        /// <param name="grid"><see cref="GridModel"/> </param>
        /// <param name="range">Specifies the range to be exported to pdf. <see cref="GridRangeInfo"/> </param>
        /// <param name="headerHandler">Delegate event handler of type DrawPdfHeaderFooterEventHandler, which fires before drawing header.</param>
        /// <param name="footerHandler">Delegate event handler of type DrawPdfHeaderFooterEventHandler, which fires before drawing footer.</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfLightTableDocument(this GridModel grid, GridRangeInfo range, DrawPdfHeaderFooterEventHandler headerHandler, DrawPdfHeaderFooterEventHandler footerHandler)
        {
            
            headerHandler = headerHandler ?? DrawPdfHeader;
            footerHandler = footerHandler ?? DrawPdfFooter;
            var document = new PdfDocument();
            var page = document.Pages.Add();
            var pdfTable = ExportToPdfLightTable(grid, range);
            var format = new PdfGridLayoutFormat() { Layout = PdfLayoutType.Paginate, Break = PdfLayoutBreakType.FitPage };
            OnDrawHeader(grid, ref document, page, headerHandler);
            OnDrawFooter(grid, ref document, page, footerHandler);
            pdfTable.Draw(page, PointF.Empty,format);
            return document;
        }

        static Dictionary<int, int[]> spannedRangeList;        

        /// <summary>
        /// Exports the Grid to PdfLightTable
        /// </summary>
        /// <param name="grid"><see cref="GridModel"/> </param>
        /// <param name="range">Specifies the range to be exported to pdf. <see cref="GridRangeInfo"/> </param>
        /// <returns>PdfLightTable</returns>   
        public static PdfLightTable ExportToPdfLightTable(this GridModel grid, GridRangeInfo range)
        {
            var pdfrow = 0;
            var pdfTable = new PdfLightTable();
            int exportColumnCount = 0;

            if (range.RangeType == GridRangeInfoType.Table)   
                range = GridRangeInfo.Cells(0, 0, grid.RowCount, grid.ColumnCount);

            pdfTable.DataSourceType = PdfLightTableDataSourceType.TableDirect;
            
            pdfTable.BeginCellLayout += pdfTable_BeginCellLayout;
            pdfTable.BeginRowLayout += pdfTable_BeginRowLayout;

            for (int col = range.Left; col < range.Right; col++)
            {
                if (grid.ColumnWidths[col] > 0)
                {                    
                    var pdfColumn = new PdfColumn(exportColumnCount.ToString()) { Width = (float)grid.ColumnWidths[col] };
                    pdfTable.Columns.Add(pdfColumn);
                    exportColumnCount++;
                }
            }

            spannedRangeList = new Dictionary<int, int[]>();
            for (int row = range.Top; row < range.Bottom; row++)
            {
                // Check the hidden rows
                if (grid.RowHeights[row] == 0) 
                    continue;

                pdfTable.Rows.Add(new object[] { });
                var obj = new object[exportColumnCount];
                var spannedRange = new int[exportColumnCount];
                var pdfcol = 0;
                for (int col = range.Left; col < range.Right; col++)
                {
                    GridRangeInfo tempRange;
                    bool isCoveredCell = grid.CoveredCells.Find(row, col, out tempRange);
                    if (isCoveredCell && tempRange.Width > 1 && tempRange.Top == row && tempRange.Left == col)
                        spannedRange[pdfcol] = tempRange.Width;                        

                    if (grid.ColumnWidths[col]>0 && grid[row,col].CellType!="NestedGrid")
                    {
                        if (grid is GridDataTableModel)
                        {
                            var gridDataTableCellType = (grid[row, col].CellIdentity as GridDataTableStyleInfoIdentity).TableCellType;

                            if (gridDataTableCellType ==GridDataTableCellType.EmptyCell || gridDataTableCellType==GridDataTableCellType.RecordPlusMinusCell 
                                || gridDataTableCellType==GridDataTableCellType.RowHeaderCell || gridDataTableCellType==GridDataTableCellType.FilterBarCell 
                                || gridDataTableCellType==GridDataTableCellType.GroupCaptionPlusMinusCell || gridDataTableCellType==GridDataTableCellType.FilterBarCell
                                || gridDataTableCellType==GridDataTableCellType.AddNewRowHeaderCell || gridDataTableCellType==GridDataTableCellType.AddNewRecordCell)
                                obj[pdfcol] = "Skip";
                            else if (gridDataTableCellType == GridDataTableCellType.ColumnHeaderIndentCell)
                            {
                                if (row >= grid.HeaderRows)
                                    obj[pdfcol] = "Skip";
                            }
                            else if (grid[row, col].CellType != "ImageCell" && grid[row, col].CellType != "RichText")
                                obj[pdfcol] = grid[row, col].Text;
                        }                            
                        pdfcol++;
                    }
                }
                spannedRangeList.Add(row, spannedRange);
                pdfTable.Rows[pdfrow].Values = obj;
                pdfrow++;
            }                       
            return pdfTable;
        }

        static void pdfTable_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
        {
            if(spannedRangeList.ContainsKey(args.RowIndex))
            {
                args.ColumnSpanMap = spannedRangeList[args.RowIndex];                
            }
        }

        static void pdfTable_BeginCellLayout(object sender, BeginCellLayoutEventArgs args)
        {
            if (args.Value == "Skip")
                args.Skip = true;
        }              
    }
}
