#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing.Printing;
using Syncfusion.GridHelperClasses;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.ComponentModel;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Class that enables grid to be printed in multiple pages
    /// </summary>
    public class MultiGridPrintDocument : GridPrintDocumentAdv
    {
        private int intOffset = 0;
        private int intRowIndexToStart = 0;
        private int intColIndexToStart = 0;
        private int intPageBreak = 0;
        private int intPageEdge = 0;
        private Rectangle rectToPrintGrid;
        private bool hitBottomOfPrintPage = false;
        private bool needNewRowRange = true;
        private bool changeGrids = false;
        private PrintColumnsToFit printColsToFit;

        private bool includeHeadersFootersOnAllPages = false;
        /// <summary>
        /// Gets or Sets whether to include Header's and footer's on the print page.
        /// </summary>
        public bool ShowHeaderFooterOnAllPages
        {
            get { return includeHeadersFootersOnAllPages; }
            set { includeHeadersFootersOnAllPages = value; }
        }

        private GridPrintOptions multiGridPrintOptions = GridPrintOptions.MultipleGridPrint;
        /// <summary>
        /// Gets or sets the MultigridprintOptions.
        /// </summary>
        public GridPrintOptions GridPrintOption
        {
            get
            {
                return this.multiGridPrintOptions;
            }

            set
            {
                this.multiGridPrintOptions = value;
            }
        }

        /// <summary>
        /// GridPrintOptions is used to customize the printing of Grid.
        /// </summary>
        public enum GridPrintOptions
        {
            /// <summary>
            /// Prints grid in multiple pages
            /// </summary>
            MultipleGridPrint,
            /// <summary>
            /// Enables default printing style
            /// </summary>
            DefaultGridPrint,
            /// <summary>
            /// Prints Grid in new page
            /// </summary>
            PrintGridInNewPage,
            /// <summary>
            /// Scales the columns to fit
            /// </summary>
            ScaleColumnsToFit,
        };

        private List<GridControlBase> lstGridsToPrint = new List<GridControlBase>();
        /// <summary>
        /// Constructor to get the all the controls to MultiGridPrintDocument, and to get the .
        /// </summary>
        /// <param name="ctrl"></param>
        public MultiGridPrintDocument(List<Control> ctrl)
        {
            foreach (Control cd in ctrl)
            {
                if (cd is GridControlBase)
                {
                    lstGridsToPrint.Add((GridControlBase)cd);
                }
            }
        }

        # region Event for removeGrid.
        /// <summary>
        /// RemoveGridEventArgs class has properties which gets or sets the arguments passed to the RemoveGridEventHandler.
        /// </summary>
        public class RemoveGridEventArgs : SyncfusionCancelEventArgs
        {
            GridControlBase gridsToPrint;
            string name;
            GridControlBase gridName=null;
            List<GridControlBase> gridLists = new List<GridControlBase>();
            /// <summary>
            /// Removes the Grid
            /// </summary>
            /// <param name="gridsToPrint">List</param>
            /// <param name="gridName">GridControlBase</param>
            /// <param name="cancel">bool</param>
            public RemoveGridEventArgs(List<GridControlBase> gridsToPrint, GridControlBase gridName, bool cancel)
            {
                this.gridLists = gridsToPrint;
                this.gridsToPrint = gridName;
                this.Cancel = cancel;
            }
            /// <summary>
            /// Removes the Grid
            /// </summary>
            /// <param name="gridsToPrint">GridControlBase</param>
            /// <param name="name">string</param>
            /// <param name="cancel">bool</param>
            public RemoveGridEventArgs(GridControlBase gridsToPrint, string name, bool cancel)
            {
                this.gridsToPrint = gridsToPrint;
                this.name = name;
                this.Cancel = cancel;
            }
            /// <summary>
            /// Grid List
            /// </summary>
            public List<GridControlBase> gridList
            {
                get { return this.gridLists; }
            }
            /// <summary>
            /// property that obtains the grid's name
            /// </summary>
            public GridControlBase GridName
            {
                get { return this.gridName; }
            }
            /// <summary>
            /// Obtains th egrid to be printed
            /// </summary>
            public GridControlBase GridsToPrint
            {
                get { return this.gridsToPrint; }
            }
            /// <summary>
            /// Convets the data to string format
            /// </summary>
            public string Name
            {
                get { return this.name; }
            }
        }
        /// <summary>
        /// RemoveGridEventHandler is used to handle the MultipleGridPrint event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RemoveGridEventHandler(object sender, RemoveGridEventArgs e);

        /// <summary>
        /// MultipleGridPrint event is used to remove grid from the list of the grids passed for printing.
        /// </summary>
        public event RemoveGridEventHandler MultipleGridPrint;
        List<GridControlBase> ctrlbase = new List<GridControlBase>();
        /// <summary>
        /// Raises the MultipleGridPrint event. 
        /// </summary>
        /// <param name="e">A RemoveGridEventArgs contains the event data.</param>
        protected virtual void OnremoveGrid(RemoveGridEventArgs e)
        {
            if (MultipleGridPrint != null)
            {
                MultipleGridPrint(this.lstGridsToPrint, e);
            }
        }

        # endregion

        #region OnBeginPrint,OnEndprint.
        /// <summary>
        /// Overriden OnBeginPrint.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            GridControlBase grd = new GridControlBase();
            RemoveGridEventArgs rem = new RemoveGridEventArgs(lstGridsToPrint, grd, true);
            this.OnremoveGrid(rem);
        }
        /// <summary>
        /// Overriden OnEndPrint.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEndPrint(PrintEventArgs e)
        {
        }
        #endregion

        GridControlBase _grid;
        int intGridCount = 0;
        int headerHeight;
        int footerHeight;
        int pageNo = 0;
        int yaxis = 0;
        bool scalepass = true;
        int scaleBetGrds = 0;

        /// <summary>
        /// Print page override.
        /// </summary>
        /// <param name="ev">The PrintEventArgs</param>
        /// <override/>
        protected override void OnPrintPage(System.Drawing.Printing.PrintPageEventArgs ev)
        {
            try
            {
                if (intGridCount >= lstGridsToPrint.Count)
                    return;

                intOffset = 0;
                hitBottomOfPrintPage = false;

                if (this.intPageEdge == 0)
                {
                    needNewRowRange = true;
                }
                if (scalepass)
                {
                    while (!hitBottomOfPrintPage && intGridCount < lstGridsToPrint.Count)//||newpageprint)
                    {
                        _grid = lstGridsToPrint[intGridCount];

                        #region scaleColumnsToFit
                        if (GridPrintOption == GridPrintOptions.ScaleColumnsToFit)
                        {
                            this.DefaultPageSettings.Margins.Bottom += 50;// this.footerHeight;
                            this.DefaultPageSettings.Margins.Top += 70;//this.headerHeight;
                            //header area
                            Rectangle printArea = new Rectangle(ev.MarginBounds.X, ev.MarginBounds.Y, ev.MarginBounds.Width, ev.MarginBounds.Height);//ev.MarginBounds;

                            //// Draw Header
                            Rectangle headerArea = new Rectangle(printArea.Left, 300, printArea.Width, this.headerHeight);//printArea.Left, printArea.Top - this.headerHeight, printArea.Width, this.headerHeight);

                            int pageCount = this.PrinterSettings.MaximumPage;

                            if (this.headerHeight > 0)
                            {
                                GridPrintHeaderFooterTemplateArgs headerTemplate = new GridPrintHeaderFooterTemplateArgs(headerArea, ev.Graphics, this.pageNo, pageCount, false);

                                //// Raise DrawHeader event
                                this.OnDrawGridPrintHeader(headerTemplate);

                                if (!headerTemplate.Cancel)
                                {
                                    this.DrawHeaderFooterText(headerTemplate, true);
                                }
                            }

                            // ScaleColumnsToFit
                            Rectangle printAreaToFit = new Rectangle(
                       this.DefaultPageSettings.Margins.Left,
                       100 + yaxis,
                      (this.DefaultPageSettings.Bounds.Width - (this.DefaultPageSettings.Margins.Right + this.DefaultPageSettings.Margins.Left)),
                      (this.DefaultPageSettings.Bounds.Height - (this.DefaultPageSettings.Margins.Top + this.DefaultPageSettings.Margins.Bottom)));

                            //if (this.printColumnsToFit == null)
                            {
                                this.printColsToFit = new PrintColumnsToFit(_grid, this.fixedRowIndex, this.PrinterSettings);
                            }

                            int maxPage;
                            yaxis += 100;
                            //// Calculate page breaks
                            this.printColsToFit.ComputePageBreaks(printAreaToFit, out maxPage);
                            this.PrinterSettings.MaximumPage = maxPage;

                            // Prepare an internal Metafile drawing the Grid
                            this.printColsToFit.PrepareGridImage();
                            //// SacleColumnsToFit
                            //// Draws from internal metafile to printer graphics
                            ev.HasMorePages = this.printColsToFit.PrintPage(printAreaToFit, ev.Graphics);
                            yaxis += (int)this.printColsToFit.Printareacalc.Height;
                            printAreaToFit.Y = (int)this.printColsToFit.Printareacalc.Height + 100;
                            scaleBetGrds++;
                            if (scaleBetGrds > 1)
                            {
                                scalepass = false;
                            }
                            ++intGridCount;
                            if (yaxis > 700)
                            {
                                hitBottomOfPrintPage = true;
                                hasMorePage = true;
                                yaxis = 0;
                            }
                            if (intGridCount == lstGridsToPrint.Count)
                            {
                                hitBottomOfPrintPage = true;
                                hasMorePage = false;
                                ev.HasMorePages = false;
                            }
                        }
                        #endregion
                        else
                        {
                            this.DefaultPageSettings.Margins.Left = 75;
                            this.DefaultPageSettings.Margins.Right = 50;
                            if (intGridCount > 0)
                            {
                                hitBottomOfPrintPage = true;
                            }
                            if (needNewRowRange)
                            {
                                needNewRowRange = false;
                                this.ComputePageBreaks(ev, _grid);
                            }

                            this.ComputePageEdge(ev, _grid);

                            //use these to hide the rows/cols that are not on the current page.

                            _grid.Model.ResetVolatileData();
                            _grid.Model.QueryRowHeight += new GridRowColSizeEventHandler(Model_QueryRowHeight);
                            _grid.Model.QueryColWidth += new GridRowColSizeEventHandler(Model_QueryColWidth);
                            if (!ScaleColumnsToFitPage)
                            {
                                this.PrepareImage(ev, _grid);
                            }
                            _grid.Model.QueryRowHeight -= new GridRowColSizeEventHandler(Model_QueryRowHeight);
                            _grid.Model.QueryColWidth -= new GridRowColSizeEventHandler(Model_QueryColWidth);
                            _grid.Model.ResetVolatileData();

                            if (changeGrids)
                            {
                                if (GridPrintOption == GridPrintOptions.PrintGridInNewPage)//printGridInNewPage)
                                {
                                    hitBottomOfPrintPage = true;
                                }
                                ++intGridCount;
                                this.intRowIndexToStart = 0;
                                this.intPageBreak = 0;
                                needNewRowRange = true;
                            }
                        }
                    }
                    if ((hitBottomOfPrintPage || intPageBreak > 0) && intGridCount < lstGridsToPrint.Count)
                    {
                        if ((intRowIndex == _grid.Model.RowCount && grdColumnIndex == _grid.Model.ColCount) && (intGridCount == lstGridsToPrint.Count - 1))
                        {
                            ev.HasMorePages = false;
                        }
                        else
                        {
                            ev.HasMorePages = true;
                        }
                        hitBottomOfPrintPage = false;
                    }
                }
                scalepass = true;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Shows the visible columns in the printpage, by discarding the previously displayed columns.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColSizeEventArgs</param>
        void Model_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            if (e.Index < intColIndexToStart && (e.Index > _grid.Model.Cols.HeaderCount))
            {
                e.Size = 0;
                e.Handled = true;
            }
        }
        /// <summary>
        /// Shows the visible rows in the printpage, by discarding the previously displayed Rows.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">GridRowColSizeEventArgs</param>
        void Model_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            if (e.Index < this.intRowIndexToStart && (e.Index > _grid.Model.Rows.HeaderCount))
            {
                e.Size = 0;
                e.Handled = true;
            }
        }
        bool hasMorePage = false;
        int hitCount = 0;

        /// <summary>
        /// Prepares Grid's image to print
        /// </summary>
        /// <param name="eventArgs">Print event args</param>
        /// <param name="_grid">Grid to print</param>
        private void PrepareImage(PrintPageEventArgs eventArgs, GridControlBase _grid)
        {
            int left = this.DefaultPageSettings.Margins.Left;
            _grid.CurrentCell.MoveTo(_grid.TopRowIndex, _grid.LeftColIndex);
            int intGridHeight = 0;
            int intGridWidth = 0;
            this.hitBottomOfPrintPage = false;
            this.headerHeight = 70;
            this.footerHeight = 50;
            if (ShowHeaderFooterOnAllPages)
            {
                int pageCount = hitCount++;
                //// Draw Header
                Rectangle headerArea = new Rectangle(this.DefaultPageSettings.Margins.Left, eventArgs.PageBounds.Top + 25, eventArgs.PageBounds.Width - (this.DefaultPageSettings.Margins.Left + this.DefaultPageSettings.Margins.Right), this.headerHeight);

                GridPrintHeaderFooterTemplateArgs headerTemplate = new GridPrintHeaderFooterTemplateArgs(headerArea, eventArgs.Graphics, this.pageNo, pageCount, false);

                //// Raise DrawHeader event
                base.OnDrawGridPrintHeader(headerTemplate);
                Rectangle footerArea = new Rectangle(this.DefaultPageSettings.Margins.Left, eventArgs.PageBounds.Bottom - this.footerHeight - 25, eventArgs.PageBounds.Width - (this.DefaultPageSettings.Margins.Left + this.DefaultPageSettings.Margins.Right), this.footerHeight);
                GridPrintHeaderFooterTemplateArgs footerTemplate = new GridPrintHeaderFooterTemplateArgs(
                            footerArea, eventArgs.Graphics, this.pageNo, pageCount, false);

                //// Raise DrawFooter event
                OnDrawGridPrintFooter(footerTemplate);
            }
            if (this.intPageBreak > 0 && this.intPageEdge > 0)
            { hasMorePage = true; }
            else
            { hasMorePage = false; }
            if (this.intPageBreak > 0)//PageBreak represents the number of rows to be prinited for a paricular page.
            {
                intGridHeight = _grid.Model.RowHeights.GetTotal(this.intRowIndexToStart, this.intPageBreak);

            }
            else
                intGridHeight = _grid.Model.RowHeights.GetTotal(this.intRowIndexToStart, _grid.Model.RowCount);
            bool resetOffset = false;

            if (this.intRowIndexToStart > 0)
            {
                intGridHeight += _grid.Model.RowHeights.GetTotal(0, _grid.Model.Rows.HeaderCount);
            }
            int saveRowIndexToStart = this.intRowIndexToStart;

            //Condition check with PageEdge.
            if (this.intPageEdge > 0 || hasMorePage)//PageEdge represents the number of columns to be printed for a paricular page.
            {
                intGridWidth += _grid.Model.ColWidths.GetTotal(this.intColIndexToStart - 1, this.intPageEdge);
                hasMorePage = false;
            }
            else
            {
                intGridWidth = _grid.Model.ColWidths.GetTotal(this.intColIndexToStart, _grid.Model.ColCount);
                intGridWidth += _grid.Model.ColWidths.GetTotal(0, 0);
            }
            if (intColIndexToStart > 1 && grdColumnIndex - 1 != _grid.Model.ColCount)
            {
                intGridWidth += _grid.Model.ColWidths.GetTotal(0, 0);
            }

            resetOffset = false;

            //Offset calculation - used for calculating the space remaining for the next grid to print.
            if (intOffset == 0)
                intOffset = eventArgs.MarginBounds.Top;
            int breakIssue = _grid.Model.RowCount - intPageBreak;
            if (GridPrintOption == GridPrintOptions.ScaleColumnsToFit)//scaleColumnsToFit)
            {
                rectToPrintGrid = new Rectangle(left, intOffset, eventArgs.MarginBounds.Width, intGridHeight);
                _grid.DrawGrid(eventArgs.Graphics, rectToPrintGrid, false, false);
                eventArgs.Graphics.DrawRectangle(Pens.BlueViolet, rectToPrintGrid);
            }
            else if (intGridWidth == 0)
            {//No grid                
            }
            else
            {
                rectToPrintGrid = new Rectangle(left, intOffset, intGridWidth, intGridHeight);
                _grid.DrawGrid(eventArgs.Graphics, rectToPrintGrid, false, false);
                eventArgs.Graphics.DrawRectangle(Pens.BlueViolet, rectToPrintGrid);
                breakIssue = 0;
            }
            //don't increment for empty grids...
            if (intGridWidth > 0)
            {
                intOffset += rectToPrintGrid.Height + 60;
            }
            if (GridPrintOption == GridPrintOptions.DefaultGridPrint)//defaultGridPrint)
            {
                hitBottomOfPrintPage = true;
            }
            else if ((!needNewRowRange && intGridHeight - 60 > (900 - intOffset)) || intOffset >= eventArgs.MarginBounds.Height - 60 || (GridPrintOption == GridPrintOptions.DefaultGridPrint && intGridHeight < 270))
            {
                if (resetOffset)
                    intOffset = 0;
                else
                {
                    intOffset = 0;
                    this.intRowIndexToStart = saveRowIndexToStart;
                }
                hitBottomOfPrintPage = true;
            }
            else
            {
                hitBottomOfPrintPage = false;
            }
        }

        int grdColumnIndex = 0;
        int intPageWidth = 0;
        /// <summary>
        /// Computes PageEdges to stop the page
        /// </summary>
        /// <param name="eventArgs">Print event arguments</param>
        /// <param name="_grid">grid to compute page edges</param>
        private void ComputePageEdge(PrintPageEventArgs eventArgs, GridControlBase _grid)
        {
            grdColumnIndex = 0;
            intPageWidth = 0;
            int intColumnWidth = 0;

            this.intColIndexToStart = this.intPageEdge;

            if (this.intColIndexToStart >= 0)
                this.intColIndexToStart++;

            grdColumnIndex = this.intColIndexToStart - 1;
            this.intPageEdge = 0;
            int hh = 0;
            int width = eventArgs.MarginBounds.Width;

            while (grdColumnIndex <= _grid.Model.ColCount)
            {
                if (_grid.Model.ColCount == intPageEdge)
                {
                    hh = eventArgs.MarginBounds.Width;
                }
                intColumnWidth = _grid.Model.ColWidths[grdColumnIndex];

                int count = 1;
                if (intPageWidth + intColumnWidth < eventArgs.PageBounds.Width - (this.DefaultPageSettings.Margins.Left + this.DefaultPageSettings.Margins.Right))
                {
                    intPageWidth += intColumnWidth;
                    grdColumnIndex++;
                    hh = 0;
                    this.intPageEdge = 0;
                }
                else if (intPageWidth + intColumnWidth == eventArgs.MarginBounds.Width && count == 0)
                {
                    count++;
                    this.intPageEdge = grdColumnIndex - 1;
                    break;
                }
                else
                {
                    this.intPageEdge = grdColumnIndex - 1;
                    break;
                }
            }
            if (grdColumnIndex >= _grid.Model.ColCount && (this.intPageEdge == 0))
            {
                changeGrids = this.intPageBreak == 0;
                if (intPageEdge == _grid.Model.ColCount)
                {
                    changeGrids = true;
                    this.intPageBreak = 0;
                }
                needNewRowRange = true;
            }
            else
            {
                changeGrids = false;
            }
        }

        int intRowIndex = 0;
        /// <summary>
        /// Computes PageBreaks to stop the page
        /// </summary>
        /// <param name="eventArgs">Print event arguments</param>
        /// <param name="_grid">grid to compute pagebraks</param>
        private void ComputePageBreaks(PrintPageEventArgs eventArgs, GridControlBase _grid)
        {
            intRowIndex = 0;
            int intPageHeight = 0;
            int intCurrentRowHeight = 0;
            this.intRowIndexToStart = this.intPageBreak;

            if (this.intRowIndexToStart > 0)
            {
                this.intRowIndexToStart++;
            }

            intRowIndex = this.intRowIndexToStart;
            intPageHeight = this.intOffset;
            int bottom = eventArgs.MarginBounds.Height;
            this.intPageBreak = 0;

            needNewRowRange = false;
            int hh = 0;

            while (intRowIndex < _grid.Model.RowCount && this.intPageBreak == 0)
            {
                if (_grid.Model.RowCount == intPageBreak)
                {
                    hh = eventArgs.MarginBounds.Height;
                }
                intCurrentRowHeight = _grid.Model.RowHeights[intRowIndex];
                if (intOffset > bottom)
                {
                    hitBottomOfPrintPage = true;
                }
                if (intPageHeight + intCurrentRowHeight < bottom && intOffset < bottom)
                {
                    intPageHeight += intCurrentRowHeight;
                    intRowIndex++;
                    this.intPageBreak = 0;
                    hh = 0;
                }
                else
                {
                    this.intPageBreak = intRowIndex;
                    if (GridPrintOption == GridPrintOptions.ScaleColumnsToFit)//scaleColumnsToFit)
                    {
                        needNewRowRange = true;
                        hitBottomOfPrintPage = true;
                    }
                    break;
                }
            }
        }
    }
}
