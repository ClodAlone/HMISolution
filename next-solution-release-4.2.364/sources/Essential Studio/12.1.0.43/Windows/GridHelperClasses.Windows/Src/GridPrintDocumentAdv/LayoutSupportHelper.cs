//-------------------------------------------------------------------------------------------------
// <copyright file="LayoutSupportHelper.cs" company="Syncfusion">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Drawing.Printing;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// A printing utility that allows the user to view the watermark page layout on the grid according to the pages that divide the grid for printing. 
    /// </summary>
    public class LayoutSupportHelper : GridPrintDocument, IDisposable
    {
        private Color textColor = Color.FromArgb(128, Color.Blue);
        List<int> pageBreakRows;
        private Color lineColor = Color.FromArgb(128, Color.Blue);
        private int lineWidth = 3;
        bool showLayoutLines = false;
        string pageTextFormat = "page {0}";
        PageFlowDirection pageFlow = PageFlowDirection.Horizontal; ////PageFlowDirection.Vertical;
        List<int> pageBreakCols;
        private GridControlBase grid;

        /// <summary>
        /// An event to draw page number in grid.
        /// </summary>
        public event DrawPageNumberEventHandler DrawPageNumber;

        /// <summary>
        /// Gets or sets the color of teh overlaid text.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return this.textColor;
            }

            set
            {
                this.textColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the overlaid lines.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return this.lineColor;
            }

            set
            {
                this.lineColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the overlaid lines.
        /// </summary>
        public int LineWidth
        {
            get
            {
                return this.lineWidth;
            }

            set
            {
                this.lineWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether print page layout borders are displayed.
        /// </summary>
        public bool ShowLayoutLines
        {
            get
            {
                return this.showLayoutLines;
            }

            set
            {
                if (this.showLayoutLines != value)
                {
                    this.showLayoutLines = value;
                    this.grid.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the content of the displayed text string.
        /// </summary>
        /// <remarks> This string is used as a format in string.Format(PageTextFormat, pageNumber)
        /// to provide the string that is overlaid on each page. The default value is "page {0}".
        /// </remarks>
        public string PageTextFormat
        {
            get
            {
                return this.pageTextFormat;
            }

            set
            {
                this.pageTextFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the page numbers are counted horizontally or vertically.
        /// </summary>
        public PageFlowDirection PageFlow
        {
            get
            {
                return this.pageFlow;
            }

            set
            {
                this.pageFlow = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of page break column numbers.
        /// </summary>
        public List<int> PageBreakCols
        {
            get
            {
                if (this.pageBreakCols == null)
                {
                    this.pageBreakCols = new List<int>();
                }

                return this.pageBreakCols;
            }

            set
            {
                this.pageBreakCols = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of page break row numbers.
        /// </summary>
        public List<int> PageBreakRows
        {
            get
            {
                if (this.pageBreakRows == null)
                {
                    this.pageBreakRows = new List<int>();
                }

                return this.pageBreakRows;
            }

            set
            {
                this.pageBreakRows = value;
            }
        }

        /// <summary>
        /// Constuctor for LayoutSupportHelper.
        /// </summary>
        /// <param name="grid">The GridControlBase whose display is being overlaid.</param>
        public LayoutSupportHelper(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            this.grid.DisableScrollWindow = true;
            this.grid.Paint += new PaintEventHandler(this.grid_Paint);
            this.grid.TopRowChanged += new GridRowColIndexChangedEventHandler(this.grid_TopRowChanged);
            this.grid.LeftColChanged += new GridRowColIndexChangedEventHandler(this.grid_LeftColChanged);
            this.grid.ResizingRows += new GridResizingRowsEventHandler(this.grid_ResizingRows);
            this.grid.ResizingColumns += new GridResizingColumnsEventHandler(this.grid_ResizingColumns);

            this.SetBreaks();
        }

        private void SetBreaks()
        {
            ////DateTime dt = DateTime.Now;
            ////while (DateTime.Now.Subtract(dt).TotalMilliseconds < SystemInformation.DoubleClickTime)
            ////{
            ////    wait..
            ////}
            this.PageBreakCols.Clear();
            this.PageBreakRows.Clear();

            PrintEventArgs ev = new PrintEventArgs();
            OnBeginPrint(ev);
            foreach (int i in this.grid.PrintInfo.m_awPageFirstCol)
            {
                this.PageBreakCols.Add(i - 1);
            }

            this.PageBreakCols.RemoveAt(0);
            foreach (int i in this.grid.PrintInfo.m_awPageFirstRow)
            {
                this.PageBreakRows.Add(i - 1);
            }

            this.PageBreakRows.RemoveAt(0);
            OnEndPrint(ev);
            if (this.grid.Updating)
            {
                this.grid.EndUpdate();
            }

            this.grid.Refresh();
        }

        void grid_ResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
            if ((e.Reason == GridResizeCellsReason.MouseUp) || (e.Reason == GridResizeCellsReason.ResetDefault) || (e.Reason == GridResizeCellsReason.ResetHide))
            {
                this.grid.BeginUpdate();
                ////use BeginInvoke to reset the breaks after the event handling completes...
                this.grid.BeginInvoke(new MethodInvoker(this.SetBreaks));
            }
        }

        void grid_ResizingRows(object sender, GridResizingRowsEventArgs e)
        {
            if ((e.Reason == GridResizeCellsReason.MouseUp) || (e.Reason == GridResizeCellsReason.ResetDefault) || (e.Reason == GridResizeCellsReason.ResetHide))
            {
                this.grid.BeginUpdate();
                ////use BeginInvoke to reset the breaks after the event handling completes...
                this.grid.BeginInvoke(new MethodInvoker(this.SetBreaks));
            }
        }

        void grid_Paint(object sender, PaintEventArgs e)
        {
            if (this.ShowLayoutLines)
            {
                if (GridPrintDocument.PageSettingsChanged)
                {
                    SetBreaks();
                    GridPrintDocument.PageSettingsChanged = false;
                }
                this.DrawColumnLines(e);
                this.DrawRowLines(e);
                this.DrawPageNumbers(e);
            }
        }

        void grid_TopRowChanged(object sender, GridRowColIndexChangedEventArgs e)
        {
            if (this.ShowLayoutLines)
            {
                this.grid.Invalidate();
            }
        }

        void grid_LeftColChanged(object sender, GridRowColIndexChangedEventArgs e)
        {
            if (this.ShowLayoutLines)
            {
                this.grid.Invalidate();
            }
        }

        private void DrawPageNumbers(PaintEventArgs pe)
        {
            if (this.PageFlow == PageFlowDirection.Horizontal)
            {
                this.DrawHorizontalPageNumbers(pe);
            }
            else
            {
                this.DrawVerticalPageNumbers(pe);
            }
        }

        private void DrawVerticalPageNumbers(PaintEventArgs pe)
        {
            int pageNo = 1;
            int leftCol = 1;
            int topRow = 1;

            if (this.PageBreakCols.Count > 0)
            {
                foreach (int col in this.PageBreakCols)
                {
                    topRow = 1;
                    Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, this.grid.ViewLayout.LastVisibleRow, col));

                    if (this.PageBreakRows.Count > 0)
                    {
                        foreach (int row in this.PageBreakRows)
                        {
                            rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, row, col));
                            this.DrawPageNumberinRect(rect, pageNo, pe);
                            pageNo++;
                            topRow = row;
                        }
                    }
                    else
                    {
                        this.DrawPageNumberinRect(rect, pageNo, pe);
                        pageNo++;
                    }

                    leftCol = col;
                }
            }
            else
            {
                Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, this.grid.ViewLayout.LastVisibleRow, this.grid.ViewLayout.LastVisibleCol));

                if (this.PageBreakRows.Count > 0)
                {
                    foreach (int row in this.PageBreakRows)
                    {
                        rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, row, this.grid.ViewLayout.LastVisibleCol));
                        this.DrawPageNumberinRect(rect, pageNo, pe);
                        pageNo++;
                        topRow = row;
                    }
                }
                else
                {
                    this.DrawPageNumberinRect(rect, pageNo, pe);
                    pageNo++;
                }
            }
        }

       /// <summary>
       /// To draw the page number in grid.
       /// </summary>
       /// <param name="e">An event argument.</param>
        protected virtual void OnDrawPageNumber(DrawPageNumberEventArgs e)
        {
            if (this.DrawPageNumber != null)
            {
                this.DrawPageNumber(this.grid, e);
            }
        }

        private void DrawPageNumberinRect(Rectangle rect, int pageNo, PaintEventArgs pe)
        {
            if ((rect.Width > 100) && (rect.Height > 100))
            {
                string s = string.Format(pageTextFormat, pageNo);
                using (Font f = new Font(this.grid.Font.FontFamily, 40))
                {
                    Size sz = TextRenderer.MeasureText(s, f);
                    Rectangle r = GridUtil.CenterInRect(rect, sz);

                    DrawPageNumberEventArgs e = new DrawPageNumberEventArgs(pe, r, s);
                    this.OnDrawPageNumber(e);
                    if (!e.Cancel)
                    {
                        using (Brush b = new SolidBrush(this.TextColor))
                        {
                            Region region = pe.Graphics.Clip;
                            pe.Graphics.Clip = new Region(this.grid.RangeInfoToRectangle(this.grid.ViewLayout.VisibleCellsRange));
                            pe.Graphics.DrawString(s, f, b, r);
                            pe.Graphics.Clip = region;
                        }
                    }
                }
            }
        }

        private void DrawHorizontalPageNumbers(PaintEventArgs pe)
        {
            int pageNo = 1;
            int leftCol = 1;
            int topRow = 1;

            if (this.PageBreakRows.Count > 0)
            {
                foreach (int row in this.PageBreakRows)
                {
                    leftCol = 1;

                    Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, row, this.grid.ViewLayout.LastVisibleCol));

                    if (this.PageBreakCols.Count > 0)
                    {
                        foreach (int col in this.PageBreakCols)
                        {
                            rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, row, col));
                            this.DrawPageNumberinRect(rect, pageNo, pe);
                            pageNo++;
                            leftCol = col;
                        }
                    }
                    else
                    {
                        this.DrawPageNumberinRect(rect, pageNo, pe);
                        pageNo++;
                    }

                    topRow = row;
                }
            }
            else
            {
                Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, this.grid.ViewLayout.LastVisibleRow, this.grid.ViewLayout.LastVisibleCol));

                if (this.PageBreakCols.Count > 0)
                {
                    foreach (int col in this.PageBreakCols)
                    {
                        rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Cells(topRow, leftCol, this.grid.ViewLayout.LastVisibleRow, col));
                        this.DrawPageNumberinRect(rect, pageNo, pe);
                        pageNo++;
                        leftCol = col;
                    }
                }
                else
                {
                    this.DrawPageNumberinRect(rect, pageNo, pe);
                    pageNo++;
                }
            }
        }

        private void DrawRowLines(PaintEventArgs pe)
        {
            if (this.PageBreakRows.Count > 0)
            {
                foreach (int row in this.PageBreakRows)
                {
                    if ((row > this.grid.ViewLayout.LastVisibleRow) || (row == this.grid.ViewLayout.LastVisibleRow && this.grid.ViewLayout.HasPartialVisibleRows))
                    {
                        break;
                    }

                    if (row < this.grid.TopRowIndex)
                    {
                        continue;
                    }

                    Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Row(row));

                    using (Pen p = new Pen(this.LineColor, this.LineWidth))
                    {
                        Region region = pe.Graphics.Clip;
                        pe.Graphics.Clip = new Region(this.grid.RangeInfoToRectangle(this.grid.ViewLayout.VisibleCellsRange));
                        pe.Graphics.DrawLine(p, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                        pe.Graphics.Clip = region;
                    }
                }
            }
        }

        private void DrawColumnLines(PaintEventArgs pe)
        {
            if (this.PageBreakCols.Count > 0)
            {
                foreach (int col in this.PageBreakCols)
                {
                    if ((col > this.grid.ViewLayout.LastVisibleCol) || (col == this.grid.ViewLayout.LastVisibleCol && this.grid.ViewLayout.HasPartialVisibleCols))
                    {
                        break;
                    }

                    if (col < this.grid.LeftColIndex)
                    {
                        continue;
                    }

                    Rectangle rect = this.grid.RangeInfoToRectangle(GridRangeInfo.Col(col));

                    using (Pen p = new Pen(this.LineColor, this.LineWidth))
                    {
                        Region region = pe.Graphics.Clip;
                        pe.Graphics.Clip = new Region(this.grid.RangeInfoToRectangle(this.grid.ViewLayout.VisibleCellsRange));
                        pe.Graphics.DrawLine(p, rect.Right, rect.Top, rect.Right, rect.Bottom);
                        pe.Graphics.Clip = region;
                    }
                }
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.grid != null && !this.grid.IsDisposed)
            {
                this.grid.Paint -= new PaintEventHandler(this.grid_Paint);
                this.grid.TopRowChanged -= new GridRowColIndexChangedEventHandler(this.grid_TopRowChanged);
                this.grid.LeftColChanged -= new GridRowColIndexChangedEventHandler(this.grid_LeftColChanged);
                this.grid.ResizingRows -= new GridResizingRowsEventHandler(this.grid_ResizingRows);
                this.grid.ResizingColumns -= new GridResizingColumnsEventHandler(this.grid_ResizingColumns);
            }
        }

        #endregion
    }

    /// <summary>
    /// Event that is raised just prior to the page number being drawn.
    /// </summary>
    /// <param name="grid">The GridControlBase</param>
    /// <param name="e">The event arguments.</param>
    public delegate void DrawPageNumberEventHandler(GridControlBase grid, DrawPageNumberEventArgs e);

    /// <summary>
    /// Event arguments for an event that is raised just prior to the page number being drawn. You can cancel this event
    /// to avoid the default drawing and instead do your own drawing to provide a customized look.
    /// </summary>
    public class DrawPageNumberEventArgs : CancelEventArgs
    {
        string text;
        PaintEventArgs pe;
        Rectangle rect;

        /// <summary>
        /// Initializes event arguments for page number drawing.
        /// </summary>
        /// <param name="pe">To draw the text.</param>
        /// <param name="rect">To set the region to be drawn with text.</param>
        /// <param name="text">Text to be drawn.</param>
        public DrawPageNumberEventArgs(PaintEventArgs pe, Rectangle rect, string text)
        {
            this.pe = pe;
            this.rect = rect;
            this.text = text;
        }

        /// <summary>
        /// Gets the PaintEventArgs that is drawing the text.
        /// </summary>
        public PaintEventArgs Pe
        {
            get
            {
                return this.pe;
            }
        }

        /// <summary>
        /// Gets or sets the Rectangle where the text is drawn.
        /// </summary>
        public Rectangle Rect
        {
            get
            {
                return this.rect;
            }

            set
            {
                this.rect = value;
            }
        }

        /// <summary>
        /// Gets or sets the text to be drawn.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
            }
        }
    }
    /// <summary>
    /// Specifies to what direction the page flow is.
    /// </summary>
    public enum PageFlowDirection
    {
        /// <summary>
        /// Represents Vertical flow direction
        /// </summary>
        Vertical,

        /// <summary>
        /// Represents Horizontal flow direction
        /// </summary>
        Horizontal
    }
}
