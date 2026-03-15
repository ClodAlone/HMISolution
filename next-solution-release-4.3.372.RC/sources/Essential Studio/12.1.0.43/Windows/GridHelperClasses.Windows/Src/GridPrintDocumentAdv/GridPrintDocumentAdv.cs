//-------------------------------------------------------------------------------------------------
// <copyright file="GridPrintDocumentAdv.cs" company="Syncfusion">
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

    using Syncfusion.Diagnostics;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.ComponentModel;
    using System.Collections.Generic;

    /// <summary>
    /// Implements printing support for the Grid with options to draw header, footer and scale columns to fit
    /// </summary>
    [ToolboxItem(false)]
    public class GridPrintDocumentAdv : GridPrintDocument
    {
        GridControlBase grid;
        int headerHeight;
        int footerHeight;
        int pageNo;
        bool scaleColumnsToFitPage;
        int currentColIndex, currentRowIndex;
        private PrintColumnsToFit printColumnsToFit;
        /// <summary>
        /// Fixed Row Index
        /// </summary>
        protected int fixedRowIndex;

        GridStyleInfo headerPrintStyleInfo;
        GridStyleInfo footerPrintStyleInfo;

        /// <summary>
        /// Gets or sets the Header text and styles. HeaderPrintStyleInfo.Text accepts custom tokens. See the remarks for allowed Tokens.
        /// </summary>
        /// <remarks>
        /// HeaderPrintStyleInfo.Text Format Tokens are: <para/>
        /// <list type="table">
        /// <listheader><term>Token</term><description>Description</description></listheader>
        /// <item><term>{PageNumber}</term><description>Displays the current printing page</description></item>
        /// <item><term>{PageCount}</term><description>Displays the total print pages</description></item>
        /// <item><term>{PrintDate}</term><description>Displays the current date in ShortDate format</description></item>
        /// <item><term>{PrintTime}</term><description>Displays the current time in ShortTime format</description></item>
        /// </list>
        /// </remarks>
        public GridStyleInfo HeaderPrintStyleInfo
        {
            get
            {
                if (this.headerPrintStyleInfo == null)
                {
                    this.headerPrintStyleInfo = new GridStyleInfo();
                }

                return this.headerPrintStyleInfo;
            }

            set
            {
                this.headerPrintStyleInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the Footer text and styles. FooterPrintStyleInfo.Text accepts custom tokens. See the remarks for allowed Tokens.
        /// </summary>
        /// <remarks>
        /// FooterPrintStyleInfo.Text Format Tokens are: <para/>
        /// <list type="table">
        /// <listheader><term>Token</term><description>Description</description></listheader>
        /// <item><term>{PageNumber}</term><description>Displays the current printing page</description></item>
        /// <item><term>{PageCount}</term><description>Displays the total print pages</description></item>
        /// <item><term>{PrintDate}</term><description>Displays the current date in ShortDate format</description></item>
        /// <item><term>{PrintTime}</term><description>Displays the current time in ShortTime format</description></item>
        /// </list>
        /// </remarks>
        public GridStyleInfo FooterPrintStyleInfo
        {
            get
            {
                if (this.footerPrintStyleInfo == null)
                {
                    this.footerPrintStyleInfo = new GridStyleInfo();
                }

                return this.footerPrintStyleInfo;
            }

            set
            {
                this.footerPrintStyleInfo = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of GridPrintDocumentAdv for a grid
        /// </summary>
        
        public GridPrintDocumentAdv()
        {
        }
        /// <summary>
        /// Initializes a new instance of GridPrintDocumentAdv for a grid
        /// </summary>
        /// <param name="grid">The parent grid for this object</param>
        public GridPrintDocumentAdv(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }
        private int noOfPagesToFitGrid = 0;
        /// <summary>
        /// Gets or sets the no of pages to fit the grid, this property will be work only when the PrintColumnToFitPage property is enabled.
        /// </summary>
        public int PagesToFit
        {
            get { return noOfPagesToFitGrid; }
            set { noOfPagesToFitGrid = value; }
        }

        private bool printColumntofitpage = false;
        /// <summary>
        /// Gets or sets whether to fit the grid to the number of pages specified in PagesToFit.
        /// </summary>
        public bool PrintColumnToFitPage
        {
            get { return printColumntofitpage; }
            set { printColumntofitpage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Grid's column should be scaled to fit the print page.
        /// </summary>
        /// <remarks>If the Grid's width exceeds print page, Grid will be scaled to fit the columns in the page while rows will span over to the next page</remarks>
        public bool ScaleColumnsToFitPage
        {
            get
            {
                return this.scaleColumnsToFitPage;
            }

            set
            {
                if (this.scaleColumnsToFitPage != value)
                {
                    this.scaleColumnsToFitPage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the height of the header
        /// </summary>
        public int HeaderHeight
        {
            get
            {
                return this.headerHeight;
            }

            set
            {
                if (this.headerHeight != value)
                {
                    this.headerHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the height of the footer
        /// </summary>
        public int FooterHeight
        {
            get
            {
                return this.footerHeight;
            }

            set
            {
                if (this.footerHeight != value)
                {
                    this.footerHeight = value;
                }
            }
        }

        /// <summary>
        /// Represents the methods that handles <see cref="DrawGridPrintHeader"/> and <see cref="DrawGridPrintFooter"/> events
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">A <see cref="GridPrintHeaderFooterTemplateArgs"/> that contains the event data</param>
        public delegate void DrawGridHeaderFooterEventHandler(object sender, GridPrintHeaderFooterTemplateArgs e);

        /// <summary>
        /// Handle this event to draw Header to the Grid print document
        /// </summary>
        public event DrawGridHeaderFooterEventHandler DrawGridPrintHeader;

        /// <summary>
        /// Handle this event to draw Footer to the Grid print document
        /// </summary>
        public event DrawGridHeaderFooterEventHandler DrawGridPrintFooter;

        /// <summary>
        /// Raises the <see cref="DrawGridPrintHeader"/> event
        /// </summary>
        /// <param name="e">A <see cref="GridPrintHeaderFooterTemplateArgs"/>that contains the event data</param>
        protected virtual void OnDrawGridPrintHeader(GridPrintHeaderFooterTemplateArgs e)
        {
            if (this.DrawGridPrintHeader != null)
            {
                this.DrawGridPrintHeader(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DrawGridPrintFooter"/> event
        /// </summary>
        /// <param name="e">A <see cref="GridPrintHeaderFooterTemplateArgs"/>that contains the event data</param>
        protected virtual void OnDrawGridPrintFooter(GridPrintHeaderFooterTemplateArgs e)
        {
            if (this.DrawGridPrintFooter != null)
            {
                this.DrawGridPrintFooter(this, e);
            }
        }

        // Begin print override
        /// <summary>
        /// Is triggered when the painting is started for cell.
        /// </summary>
        /// <param name="ev">PrintEventArgs</param>
        protected override void OnBeginPrint(PrintEventArgs ev)
        {
            try
            {
                this.DefaultPageSettings.Margins.Bottom += this.footerHeight;
                this.DefaultPageSettings.Margins.Top += this.headerHeight;
                currentRowIndex = this.grid.CurrentCell.RowIndex;
                currentColIndex = this.grid.CurrentCell.ColIndex;
                if (!this.scaleColumnsToFitPage)
                {
                    // Calculates page breaks
                    base.OnBeginPrint(ev);
                }
                else
                {
                    // ScaleColumnsToFit
                    Rectangle printArea = new Rectangle(
               this.DefaultPageSettings.Margins.Left,
               this.DefaultPageSettings.Margins.Top,
              (this.DefaultPageSettings.Bounds.Width - (this.DefaultPageSettings.Margins.Right + this.DefaultPageSettings.Margins.Left)),
              (this.DefaultPageSettings.Bounds.Height - (this.DefaultPageSettings.Margins.Top + this.DefaultPageSettings.Margins.Bottom)));
                    if (!PrintColumnToFitPage)
                    {
                        if (this.printColumnsToFit == null)
                        {
                            this.printColumnsToFit = new PrintColumnsToFit(this.grid, this.fixedRowIndex, this.PrinterSettings);

                        }

                        int maxPage;

                        //// Calculate page breaks
                        this.printColumnsToFit.ComputePageBreaks(printArea, out maxPage);
                        this.PrinterSettings.MaximumPage = maxPage;

                        // Prepare an internal Metafile drawing the Grid
                        this.printColumnsToFit.PrepareGridImage();
                    }
                }
            }
            finally
            {
                this.pageNo = 1;
            }
        }
        /// <summary>
        /// Drws the header and footer and writes text in it
        /// </summary>
        /// <param name="template">GridPrintHeaderFooterTemplateArgs</param>
        /// <param name="header">bool</param>
        public void DrawHeaderFooterText(GridPrintHeaderFooterTemplateArgs template, bool header)
        {
            GridStyleInfo styleInfo;

            if (header)
            {
                styleInfo = this.headerPrintStyleInfo;
            }
            else
            {
                styleInfo = this.footerPrintStyleInfo;
            }

            if (styleInfo == null)
            {
                return;
            }

            string printText = styleInfo.Text;

            string[] tokens = new string[] { "PageNumber", "PageCount", "PrintDate", "PrintTime" };

            //// Equivalent local variables
            object[] internalTokens = new object[] { template.PageNumber, template.PageCount, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() };

            for (int loc = 0; loc < tokens.Length; loc++)
            {
                if (printText.Contains(tokens[loc]))
                {
                    printText = printText.Replace(tokens[loc].ToString(), loc.ToString());
                }
            }

            string drawText;

            try
            {
                drawText = string.Format(printText, internalTokens);
            }
            catch (Exception ex)
            {
                drawText = printText + ": " + ex.Message;
            }

            //// Draw the text
            if (drawText != null)
            {
                GridStaticCellRenderer.DrawText(template.Graphics, drawText, styleInfo.Font.GdipFont, template.DrawRectangle, styleInfo, styleInfo.TextColor, this.grid.IsRightToLeft());
            }
        }

        /// <summary>
        /// Print page override
        /// </summary>
        /// <param name="ev">The PrintEventArgs</param>
        /// <override/>
        protected override void OnPrintPage(PrintPageEventArgs ev)
        {
            try
            {
                Rectangle printArea = ev.MarginBounds;

                //// Draw Header
                Rectangle headerArea = new Rectangle(printArea.Left, printArea.Top - this.headerHeight, printArea.Width, this.headerHeight);

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

                if (!this.scaleColumnsToFitPage && !printColumntofitpage)
                {
                    base.OnPrintPage(ev);
                }
                else if (printColumntofitpage)
                {
                    int maxPage;
                    if (this.printColumnsToFit == null)
                    {
                        this.printColumnsToFit = new PrintColumnsToFit(this.grid, this.fixedRowIndex, this.PrinterSettings);
                        this.printColumnsToFit.NoOfPagesToFitGrid = this.PagesToFit;
                        this.printColumnsToFit.ScaleColumnsToFit = this.ScaleColumnsToFitPage;
                        this.printColumnsToFit.ScaleColumnsToFitPage = this.PrintColumnToFitPage;
                    }
                    //// Calculate page breaks
                    this.printColumnsToFit.ComputePageBreaks(printArea, out maxPage);
                    this.PrinterSettings.MaximumPage = maxPage;

                    // Prepare an internal Metafile drawing the Grid
                    this.printColumnsToFit.PrepareGridImage();
                    ev.HasMorePages = this.printColumnsToFit.PrintPage(printArea, ev.Graphics);
                }
                else
                {
                    //// ScaleColumnsToFit
                    //// Draws from internal metafile to printer graphics
                    ev.HasMorePages = this.printColumnsToFit.PrintPage(printArea, ev.Graphics);
                }

                //// Draw Footer
                Rectangle footerArea = new Rectangle(printArea.Left, printArea.Bottom, printArea.Width, this.footerHeight);

                if (this.footerHeight > 0)
                {
                    GridPrintHeaderFooterTemplateArgs footerTemplate = new GridPrintHeaderFooterTemplateArgs(
                        footerArea, ev.Graphics, this.pageNo, pageCount, false);

                    //// Raise DrawFooter event
                    this.OnDrawGridPrintFooter(footerTemplate);

                    if (!footerTemplate.Cancel)
                    {
                        this.DrawHeaderFooterText(footerTemplate, false);
                    }
                }
            }
            finally
            {
                this.pageNo++;
            }
        }

        /// <summary>
        /// EndEdit override
        /// </summary>
        /// <param name="e">The PrintEventArgs</param> 
        protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);

            this.DefaultPageSettings.Margins.Bottom -= this.footerHeight;
            this.DefaultPageSettings.Margins.Top -= this.headerHeight;
            this.grid.CurrentCell.MoveTo(currentRowIndex, currentColIndex);
            if (this.printColumnsToFit != null)
            {
                this.printColumnsToFit = null;
            }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="GridPrintHeaderFooterTemplateArgs"/> and <see cref="GridPrintHeaderFooterTemplateArgs"/> events
    /// </summary>
    /// <remarks>To draw the Header / Footer for the Grid Print document, handle the <see cref="GridPrintHeaderFooterTemplateArgs"/> / <see cref="GridPrintHeaderFooterTemplateArgs"/> events
    /// </remarks>
    public class GridPrintHeaderFooterTemplateArgs : SyncfusionCancelEventArgs
    {
        Rectangle drawRectangle;
        int pageNumber;
        int pageCount;
        Graphics graphics;

        /// <summary>
        /// Initializes a new <see cref="GridPrintHeaderFooterTemplateArgs"/>
        /// </summary>
        /// <param name="drawRectangle">Rectangle area to draw Header / Footer</param>
        /// <param name="graphics">Printer graphics</param>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageCount">Total page count</param>
        /// <param name="cancel">boolean value to cancel</param>
        public GridPrintHeaderFooterTemplateArgs(Rectangle drawRectangle, Graphics graphics, int pageNumber, int pageCount, bool cancel)
        {
            this.drawRectangle = drawRectangle;
            this.graphics = graphics;
            this.pageNumber = pageNumber;
            this.pageCount = pageCount;
            this.Cancel = cancel;
        }

        /// <summary>
        /// Gets the current page number
        /// </summary>
        public int PageNumber
        {
            get
            {
                return this.pageNumber;
            }
        }

        /// <summary>
        /// Gets the total page count
        /// </summary>
        public int PageCount
        {
            get
            {
                return this.pageCount;
            }
        }

        /// <summary>
        /// Gets the Rectangle area to draw Header / Footer
        /// </summary>
        public Rectangle DrawRectangle
        {
            get
            {
                return this.drawRectangle;
            }
        }

        /// <summary>
        /// Gets the printer graphics
        /// </summary>
        public Graphics Graphics
        {
            get { return this.graphics; }
        }
    }
}
