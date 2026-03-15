//-------------------------------------------------------------------------------------------------
// <copyright file="PrintColumnsToFit.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Printing;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using Syncfusion.Diagnostics;
    using Syncfusion.Drawing;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Helper class for PrintToFitColumns
    /// </summary>
    public class PrintColumnsToFit
    {
        private GridControlBase grid;
        private int currentPage = 0;
        private Metafile gridImage = null;
        private float offset = 0;
        private float scale = 0;
        private List<int> pageBreaks = new List<int>();
        private List<int> pageEdge = new List<int>();
        private bool printBlackWhiteHeaders = false;
        bool colindexadd = true;
        int fixedRows = 0;
        PrinterSettings setting;
        private int noOfPagesToFitGrid = 0;
        /// <summary>
        /// Gets or sets the no Of pages to fit the grid.
        /// </summary>
        public int NoOfPagesToFitGrid
        {
            get { return noOfPagesToFitGrid; }
            set { noOfPagesToFitGrid = value; }
        }
        private bool scaleColumnsToFit = false;
        /// <summary>
        ///Gets or sets whether to ScaleColumnstofit the pages.
        /// </summary>
        public bool ScaleColumnsToFit
        {
            get { return scaleColumnsToFit; }
            set { scaleColumnsToFit = value; }
        }

        private bool scaleColumnsToFitPage = false;
        /// <summary>
        /// Gets or sets scalecolumstofit page is enabled.
        /// </summary>
        public bool ScaleColumnsToFitPage
        {
            get { return scaleColumnsToFitPage; }
            set { scaleColumnsToFitPage = value; }
        }
        private RectangleF printareacalc;
        /// <summary>
        /// Gets or sets the no Of pages to fit the grid.
        /// </summary>
        internal RectangleF Printareacalc
        {
            get { return printareacalc; }
            set { printareacalc = value; }
        }

        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        static readonly BrushInfo defaultInterior2 = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        /// <summary>
        /// Prints thecolumns to fit into the page
        /// </summary>
        /// <param name="grid">GridControlBase</param>
        /// <param name="fixedRows">int</param>
        /// <param name="setting">PrinterSettings</param>
        public PrintColumnsToFit(GridControlBase grid, int fixedRows, PrinterSettings setting)
        {
            this.grid = grid;
            this.fixedRows = fixedRows;
            this.setting = setting;
            this.grid.PrintInfo.m_awPageFirstRow = new ArrayList();
            this.grid.PrintInfo.m_awPageFirstCol = new ArrayList();
        }


        /// <summary>
        /// Prepare the grid image as a metafile....
        /// </summary>
        public void PrepareGridImage()
        {
            if (noOfPagesToFitGrid == 0 || ScaleColumnsToFit && !ScaleColumnsToFitPage)
            {
                // draw a fullsize metafile of the grid
                int gridHeight = this.grid.Model.RowHeights.GetTotal(0, this.grid.Model.RowCount);
                int gridWidth = this.grid.Model.ColWidths.GetTotal(0, this.grid.Model.ColCount);
                GridRangeInfo range = grid.Selections.Ranges.ActiveRange;
                int width;
                if (range.IsRows)
                    width = (grid.Model.ColCount);
                else
                    width = (range.Width);

                int height;
                if (range.IsCols)
                    height = (grid.Model.RowCount);
                else
                    height = (range.Height);
                if (range.IsRows || range.IsCols)
                    range = GridRangeInfo.Cells(range.Top, range.Left, height, width);
                Graphics gr = Graphics.FromHwndInternal(IntPtr.Zero);
                IntPtr hdc = gr.GetHdc();
                int errorCorrection = 10;
                Metafile mf = new System.Drawing.Imaging.Metafile(hdc, new Rectangle(0, 0, gridWidth + (errorCorrection / 2), gridHeight + errorCorrection), MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                // Make a Graphics object to work with the metafile.
                Graphics g = Graphics.FromImage(mf);

                this.grid.BeginUpdate();
                if (this.setting.PrintRange != PrintRange.Selection)
                {
                    this.grid.GridBounds = new Rectangle(0, 0, gridWidth, gridHeight);
                }
                else
                {
                    int gridHeight1 = grid.Model.RowHeights.GetTotal(range.Top, range.Bottom + 1);
                    int gridWidth1 = grid.Model.ColWidths.GetTotal(range.Left, range.Right + 1);
                    this.grid.GridBounds = new Rectangle(0, 0, gridWidth1, gridHeight1);
                }

                // Handle Draw and PrepareViewStyleInfo events to draw non theme headers
                GridControlBase.UseImageListDrawing = true;
                GridPrintDocumentAdv printAdv = new GridPrintDocumentAdv(this.grid);
                this.printBlackWhiteHeaders = !grid.Model.Properties.ThemedHeader;
                //this.grid.DrawCell += new GridDrawCellEventHandler(this.grid_DrawCell);
                //this.grid.CellDrawn += new GridDrawCellEventHandler(this.grid_CellDrawn);
                if (this.printBlackWhiteHeaders || this.grid.Model.Properties.BlackWhite)
                    this.grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(this.grid_PrepareViewStyleInfo);

                // Make sure grid is scrolled to the top cell
                this.grid.TopRowIndex = 1;
                this.grid.LeftColIndex = 1;

                // Clear selections
                this.grid.Model.Selections.Clear();

                bool t1 = false;
                bool t2 = false;
                t1 = grid.Model.Properties.DisplayHorzLines;
                t2 = grid.Model.Properties.DisplayVertLines;
                grid.Model.Properties.DisplayHorzLines = grid.Model.Properties.PrintHorzLines;
                grid.Model.Properties.DisplayVertLines = grid.Model.Properties.PrintVertLines;

                // Draw Grid
                this.grid.DrawGrid(g);
                this.grid.ResetGridBounds();

                // Reset things
                GridControlBase.UseImageListDrawing = false;
                this.printBlackWhiteHeaders = false;
                //this.grid.DrawCell -= new GridDrawCellEventHandler(this.grid_DrawCell);
                //this.grid.CellDrawn -= new GridDrawCellEventHandler(this.grid_CellDrawn);
                this.grid.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(this.grid_PrepareViewStyleInfo);

                this.grid.EndUpdate();

                g.Dispose();
                gr.ReleaseHdc();

                this.grid.Refresh();

                // Scale the metafile
                this.gridImage = new Metafile(gr.GetHdc(), new Rectangle(0, 0, (int)(gridWidth * this.scale), (int)(gridHeight * this.scale)), MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                g = Graphics.FromImage(this.gridImage);
                g.DrawImage(mf, 0, 0, gridWidth * this.scale, gridHeight * this.scale);

                gr.Dispose();
                g.Dispose();
                mf.Dispose();
                grid.Model.Properties.DisplayHorzLines = t1;
                grid.Model.Properties.DisplayVertLines = t2;
            }
            else
            {
                // draw a fullsize metafile of the grid
                int gridHeight = gridHeights;//this.grid.Model.RowHeights.GetTotal(0, rowPerPage);//rowperpage..
                int gridWidth = gridWidths;//this.grid.Model.ColWidths.GetTotal(0, colPerPage);//colperpage..
                GridRangeInfo range = grid.Selections.Ranges.ActiveRange;
                int width;
                if (range.IsRows)
                    width = (grid.Model.ColCount);
                else
                    width = (range.Width);

                int height;
                if (range.IsCols)
                    height = (grid.Model.RowCount);
                else
                    height = (range.Height);
                if (range.IsRows || range.IsCols)
                    range = GridRangeInfo.Cells(range.Top, range.Left, height, width);
                Graphics gr = Graphics.FromHwndInternal(IntPtr.Zero);
                IntPtr hdc = gr.GetHdc();
                int rowHeaderHeight = this.grid.Model.RowHeights.GetTotal(0, 0);

                RectangleF srcRect = new RectangleF(0, (int)((this.scale * rowHeaderHeight)), areaToPrint.Width, this.scaleRow * this.grid.Model.RowHeights.GetTotal(1, this.grid.Model.RowCount));
                //srcRect.Height = 909;
                Metafile mf = new System.Drawing.Imaging.Metafile(hdc, new RectangleF(0, 0, gridWidth, 900), MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                // Make a Graphics object to work with the metafile.
                Graphics g = Graphics.FromImage(mf);

                this.grid.BeginUpdate();
                if (this.setting.PrintRange != PrintRange.Selection)
                {
                    this.grid.GridBounds = new Rectangle(0, 0, gridWidth, (875 * 2));
                }
                else
                {
                    int gridHeight1 = grid.Model.RowHeights.GetTotal(range.Top, range.Bottom + 1);
                    int gridWidth1 = grid.Model.ColWidths.GetTotal(range.Left, range.Right + 1);
                    this.grid.GridBounds = new Rectangle(0, 0, gridWidth1, gridHeight1);
                }

                // Handle Draw and PrepareViewStyleInfo events to draw non theme headers
                GridControlBase.UseImageListDrawing = true;
                this.printBlackWhiteHeaders = true;
                //this.grid.DrawCell += new GridDrawCellEventHandler(this.grid_DrawCell);
                //this.grid.CellDrawn += new GridDrawCellEventHandler(this.grid_CellDrawn);
                this.grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(this.grid_PrepareViewStyleInfo);

                // Make sure grid is scrolled to the top cell
                this.grid.TopRowIndex = 1;
                if (colindexadd)
                {
                    this.grid.LeftColIndex = 1;
                    colindexadd = false;
                }
                else
                {
                    this.grid.LeftColIndex = colBrk + 1 - colPerPage;
                }

                // Clear selections
                this.grid.Model.Selections.Clear();

                bool t1 = false;
                bool t2 = false;
                t1 = grid.Model.Properties.DisplayHorzLines;
                t2 = grid.Model.Properties.DisplayVertLines;
                grid.Model.Properties.DisplayHorzLines = grid.Model.Properties.PrintHorzLines;
                grid.Model.Properties.DisplayVertLines = grid.Model.Properties.PrintVertLines;

                // Draw Grid
                this.grid.DrawGrid(g);
                this.grid.ResetGridBounds();

                // Reset things
                GridControlBase.UseImageListDrawing = false;
                this.printBlackWhiteHeaders = false;
                //this.grid.DrawCell -= new GridDrawCellEventHandler(this.grid_DrawCell);
                //this.grid.CellDrawn -= new GridDrawCellEventHandler(this.grid_CellDrawn);
                this.grid.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(this.grid_PrepareViewStyleInfo);

                this.grid.EndUpdate();

                g.Dispose();
                gr.ReleaseHdc();

                this.grid.Refresh();

                // Scale the metafile
                this.gridImage = new Metafile(gr.GetHdc(), new Rectangle(0, 0, (int)(gridWidth * this.scale), (int)(900)), MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                g = Graphics.FromImage(this.gridImage);
                g.DrawImage(mf, 0, 0, gridWidth * this.scale, (900));

                gr.Dispose();
                g.Dispose();
                mf.Dispose();
                grid.Model.Properties.DisplayHorzLines = t1;
                grid.Model.Properties.DisplayVertLines = t2;
            }
        }
        private int colPerPage = 0;
        private int rowPerPage = 0;
        private float scaleRow = 0;
        private int gridHeights = 0;
        private int gridWidths = 0;
        private int colBrk = 0;
        Rectangle areaToPrint;

        /// <summary>
        /// Calculates the row breaks for scalecolumnstofit, for Scalecolumnstofitpage it will calculate the column breaks...
        /// </summary>
        /// <param name="printArea"></param>
        /// <param name="maxPages"></param>
        public void ComputePageBreaks(Rectangle printArea, out int maxPages)
        {
            if (NoOfPagesToFitGrid == 0 || ScaleColumnsToFit && !ScaleColumnsToFitPage)
            {
                this.pageBreaks.Clear();

                int gridWidth = this.grid.Model.ColWidths.GetTotal(0, this.grid.Model.ColCount);

                this.scale = (float)printArea.Width / gridWidth;

                int rowHeaderHeight = this.grid.Model.RowHeights.GetTotal(0, this.fixedRows);
                int rowIndex = this.fixedRows + 1;

                RectangleF destRect = new RectangleF(printArea.X, printArea.Y + (rowHeaderHeight * this.scale), printArea.Width, printArea.Height - (rowHeaderHeight * this.scale));

                float pageHeight = 0f;
                float currentRowHeight = 0f;
                if (!this.grid.PrintInfo.m_awPageFirstRow.Contains(defRowCol))
                    this.grid.PrintInfo.m_awPageFirstRow.Add(defRowCol);
                if (!this.grid.PrintInfo.m_awPageFirstCol.Contains(defRowCol))
                    this.grid.PrintInfo.m_awPageFirstCol.Add(defRowCol);
                while (rowIndex < this.grid.Model.RowCount)
                {
                    currentRowHeight = this.grid.Model.RowHeights[rowIndex] * this.scale;

                    if (pageHeight + currentRowHeight < destRect.Height)
                    {
                        pageHeight += currentRowHeight;
                        rowIndex++;
                    }
                    else
                    {
                        this.pageBreaks.Add(rowIndex);
                        this.grid.PrintInfo.m_awPageFirstRow.Add(rowIndex + 1);
                        pageHeight = 0;
                    }
                }

                this.grid.PrintInfo.m_nPrintLeftCol = pageLeftCol;
                this.grid.PrintInfo.m_nPrintTopRow = pageTopRow;
                this.grid.PrintInfo.m_nCurrentPageColIndex = curPageCIndex;
                this.grid.PrintInfo.m_nCurrentPageRowIndex = curPageRIndex;
                maxPages = this.pageBreaks.Count + 1;
            }
            else
            {

                this.pageBreaks.Clear();
                //this.pageEdge.Clear();
                //
                colPerPage = this.grid.Model.ColCount / noOfPagesToFitGrid;
                if (colPerPage * noOfPagesToFitGrid != this.grid.Model.ColCount)
                {
                    extracol = this.grid.Model.ColCount - colPerPage * noOfPagesToFitGrid;
                }
                if (colBrk + colPerPage + colPerPage > this.grid.Model.ColCount)
                {
                    gridWidths = this.grid.Model.ColWidths.GetTotal(0, colPerPage + extracol);
                }
                else
                {
                    gridWidths = this.grid.Model.ColWidths.GetTotal(0, colPerPage);
                }
                gridHeights = this.grid.Model.RowHeights.GetTotal(0, this.grid.Model.RowCount);

                //
                this.scale = (float)printArea.Width / gridWidths;

                int rowHeaderHeight = this.grid.Model.RowHeights.GetTotal(0, this.fixedRows);
                int rowIndex = this.fixedRows + 1;
                float pageHeight = 0f;
                float currentRowHeight = 0f;
                //row
                rowPerPage = this.grid.Model.RowCount / 1;
                pageHeight = printArea.Height / this.grid.Model.RowCount;
                scaleRow = (float)(printArea.Height - printArea.X) / gridHeights;
                RectangleF destRect = new RectangleF(printArea.X, printArea.Y + (rowHeaderHeight * this.scale), printArea.Width, printArea.Height - (rowHeaderHeight * this.scaleRow));


                //pageHeight = this.grid.Model.RowHeights.GetTotal(0, this.grid.Model.RowCount);

                while (rowIndex < this.grid.Model.RowCount)
                {
                    currentRowHeight = this.grid.Model.RowHeights[rowIndex] * this.scale;

                    if (pageHeight + currentRowHeight < destRect.Height)
                    {
                        pageHeight += currentRowHeight;
                        rowIndex++;
                    }
                    else
                    {
                        this.pageBreaks.Add(rowIndex);
                        pageHeight = 0;
                    }
                }
                if (colBrk < this.grid.Model.ColCount)
                {
                    colBrk += colPerPage;
                    this.pageEdge.Add(colBrk);
                }
                gridHeights = (int)pageHeight;
                maxPages = this.pageBreaks.Count + 1;
                maxPages = this.pageEdge.Count + 1;
                areaToPrint = printArea;
            }
        }
        int extracol = 0;
        int curPageRIndex = 0,
            curPageCIndex = 0,
            pageTopRow = 1,
            pageLeftCol = 1,
            defRowCol = 1;
        /// <summary>
        /// Printpage is used to return the number of pages....
        /// </summary>
        /// <param name="printArea"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public bool PrintPage(Rectangle printArea, Graphics g)
        {
            if (noOfPagesToFitGrid == 0 || ScaleColumnsToFit && !ScaleColumnsToFitPage)
            {
                bool hasMorePages = this.currentPage < this.pageBreaks.Count;

                int rowHeaderHeight = this.grid.Model.RowHeights.GetTotal(0, this.fixedRows);
                int rowIndex = this.fixedRows + 1;

                int lastRowIndex = (this.currentPage == 0) ? rowIndex : (int)this.pageBreaks[this.currentPage - 1];
                int currentRowIndex = (this.currentPage < this.pageBreaks.Count) ? (int)this.pageBreaks[this.currentPage] : this.grid.Model.RowCount;

                RectangleF srcRect = new RectangleF(0, this.offset + (this.scale * rowHeaderHeight), this.gridImage.Width, this.scale * this.grid.Model.RowHeights.GetTotal(lastRowIndex, currentRowIndex));
                RectangleF destRect = new RectangleF(printArea.X, printArea.Y + (this.scale * rowHeaderHeight), printArea.Width, Math.Min(srcRect.Height, (printArea.Height - rowHeaderHeight)));
                this.offset += srcRect.Height;

                this.grid.PrintInfo.m_nPrintTopRow = currentRowIndex;
                this.grid.PrintInfo.m_nPrintLeftCol = pageLeftCol;
                this.grid.PrintInfo.m_nCurrentPageColIndex = curPageCIndex;
                this.grid.PrintInfo.m_nCurrentPageRowIndex = curPageRIndex;
                curPageRIndex++;
                // DrawHeader
                RectangleF srcHeaderRect = new RectangleF(0, 0, this.gridImage.Width, this.scale * rowHeaderHeight);
                RectangleF destHeaderRect = new RectangleF(printArea.X, printArea.Y, this.gridImage.Width, this.scale * rowHeaderHeight);
                g.DrawImage(this.gridImage, destHeaderRect, srcHeaderRect, GraphicsUnit.Pixel);

                // Draw Grid page
                g.DrawImage(this.gridImage, destRect, srcRect, GraphicsUnit.Pixel);
                Printareacalc = destRect;
                this.currentPage++;

                return hasMorePages;
            }
            else
            {
                bool hasMorePages = this.currentPage < this.pageBreaks.Count;
                hasMorePages = this.currentPage < this.pageEdge.Count;
                int rowHeaderHeight = this.grid.Model.RowHeights.GetTotal(0, this.fixedRows);
                int rowIndex = this.fixedRows + 1;
                int lastRowIndex = 0;
                int currentRowIndex = this.grid.Model.RowCount;
                RectangleF srcRect;
                RectangleF destRect;
                if (count == 1)
                {
                    count++;
                    srcRect = new RectangleF(0, (int)(this.offset + (this.scale * rowHeaderHeight)), printArea.Width, this.scaleRow * this.grid.Model.RowHeights.GetTotal(lastRowIndex, currentRowIndex));
                    destRect = new RectangleF(printArea.X, ((int)(printArea.Y + (this.scale * rowHeaderHeight))), printArea.Width, Math.Min(srcRect.Height, (printArea.Height - rowHeaderHeight)));
                }
                else
                {
                    count++;
                    srcRect = new RectangleF(0, (int)((this.scale * rowHeaderHeight)), printArea.Width, this.scaleRow * this.grid.Model.RowHeights.GetTotal(lastRowIndex, currentRowIndex));
                    destRect = new RectangleF(printArea.X, ((int)(printArea.Y + (this.scale * rowHeaderHeight))), printArea.Width, Math.Min(srcRect.Height, (printArea.Height - rowHeaderHeight)));
                }
                this.offset += srcRect.Height;

                // DrawHeader
                RectangleF srcHeaderRect = new RectangleF(0, 0, this.gridImage.Width, this.scaleRow * rowHeaderHeight);
                RectangleF destHeaderRect = new RectangleF(printArea.X, printArea.Y, this.gridImage.Width, this.scaleRow * rowHeaderHeight);
                g.DrawImage(this.gridImage, destHeaderRect, srcHeaderRect, GraphicsUnit.Pixel);

                // Draw Grid page
                g.DrawImage(this.gridImage, destRect, srcRect, GraphicsUnit.Pixel);
                if (colBrk + colPerPage > this.grid.Model.ColCount)
                {
                    hasMorePages = false;
                }

                this.currentPage++;

                return hasMorePages;
            }
        }
        int count = 1;
        void grid_CellDrawn(object sender, GridDrawCellEventArgs e)
        {
            this.grid.PrintingMode = false;
        }

        void grid_DrawCell(object sender, GridDrawCellEventArgs e)
        {
            this.grid.PrintingMode = true;
        }
        /// <summary>
        /// To Draw the grid border and the set backcolors for the header.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void grid_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (e.Style.CellType == "Header" && (this.printBlackWhiteHeaders || this.grid.Model.Properties.BlackWhite ))
            {
                int nhr = this.grid.InternalGetHeaderRows();
                int nhc = this.grid.InternalGetHeaderCols();
                this.grid.CurrentCell.MoveTo(-1, -1);
                Syncfusion.Drawing.BrushInfo br = e.Style.Interior;
                if ((br.Equals(defaultInterior1) || br.Equals(defaultInterior2)) && !this.grid.Model.Properties.BlackWhite)
                {
                    e.Style.BackColor = SystemColors.Control;
                }
                else
                {
                    e.Style.BackColor = Color.White;
                    e.Style.TextColor = Color.Black;
                }
                GridBorder border = null;
                if (this.grid.Model.Properties.ThemedHeader)
                    border = new GridBorder(GridBorderStyle.Solid, e.Style.Borders.Bottom.Color);
                else
                    border = new GridBorder(GridBorderStyle.Solid, Color.Black);
                if (e.RowIndex == 0 && e.ColIndex == 0)
                {
                    e.Style.Borders.Top = border;
                    e.Style.Borders.Left = border;
                }
                else if (e.RowIndex == 0)
                {
                    e.Style.Borders.Top = border;
                }
                else if (e.ColIndex == 0)
                {
                    e.Style.Borders.Left = border;
                }
                else if (e.ColIndex <= nhc || e.RowIndex <= nhr)
                {
                    e.Style.Borders.Right = border;
                    e.Style.Borders.Bottom = border;
                    e.Style.Borders.Top = border;
                    e.Style.Borders.Left = border;
                }
                e.Style.Themed = false;
                e.Cancel = true;
            }
            else if (this.grid.Model.Properties.BlackWhite)
            {
                e.Style.TextColor = Color.Black;
            }
        }
    }
}