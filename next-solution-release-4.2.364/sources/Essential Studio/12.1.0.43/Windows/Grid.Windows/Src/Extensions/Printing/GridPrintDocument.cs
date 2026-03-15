//-------------------------------------------------------------------------------------------------
// <copyright file="GridPrintDocument.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

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

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    Implements printing support for a grid.
    /// </summary>
    /// <example>
    /// Use the following code for printing:
    /// <code lang="C#">
    ///                     GridPrintDocument pd = new GridPrintDocument(grid); //Assumes the default printer
    ///                     if (PrinterSettings.storedPageSettings != null)
    ///                     {
    ///                         pd.DefaultPageSettings = PrinterSettings.storedPageSettings ;
    ///                     }
    ///                     PrintDialog dlg = new PrintDialog() ;
    ///                     dlg.Document = pd;
    ///                     dlg.AllowSelection = true;
    ///                     dlg.AllowSomePages = true;
    ///                     DialogResult result = dlg.ShowDialog();
    /// <para/> 
    ///                     if (result == DialogResult.OK)
    ///                     {
    ///                         pd.Print();
    ///                     }
    /// </code>
    /// Use the following code for print preview:
    /// <code lang="C#">
    ///                        GridControlBase grid = ActiveGrid;
    ///                     GridPrintDocument pd = new GridPrintDocument(grid, true); //Assumes the default printer
    ///                     if (PrinterSettings.storedPageSettings != null)
    ///                     {
    ///                         pd.DefaultPageSettings = PrinterSettings.storedPageSettings ;
    ///                     }
    ///  <para/>
    ///                     PrintPreviewDialog dlg = new PrintPreviewDialog() ;
    ///                     dlg.Document = pd;
    ///                     dlg.ShowDialog();
    /// </code>
    /// </example>
    [ToolboxItem(false)]
    public class GridPrintDocument : PrintDocument
    {
        GridControlBase m_grid;
        int m_nCurPage = 0;
        internal bool m_printPreview = false;
        int fromPage = 0;
        int toPage = int.MaxValue;
        /// <summary>
        /// empty constructor
        /// </summary>
        public GridPrintDocument()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="GridPrintDocument"/> for a grid.
        /// </summary>
        /// <param name="grid">The parent grid for this object.</param>
        public GridPrintDocument(GridControlBase grid)
        {
            m_grid = grid;
        }

        /// <summary>
        /// Initializes a new <see cref="GridPrintDocument"/> for a grid.
        /// </summary>
        /// <param name="grid">The parent grid for this object.</param>
        /// <param name="printPreview">True if print preview; False if printing.</param>
        public GridPrintDocument(GridControlBase grid, bool printPreview)
        {
            m_grid = grid;
            //// m_printPreview is meant for future use - right now this is not
            //// working and will only break printing code. (only one page will be printed then ...)
            ////m_printPreview = printPreview;
        }

        /// <summary>
        /// Gets a value indicating whether this object is in print preview mode or if it is output to a printer.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPrintPreview
        {
            get
            {
                return m_printPreview;
            }
        }

        Rectangle customBounds = Rectangle.Empty;

        /// <summary>
        /// Gets or sets Grid will take this Bounds while calculating page breaks in the OnBeginPrint override. By default it uses PrintDocument's DefaultPageSettings.Bounds.
        /// </summary>
        protected Rectangle CustomBounds
        {
            get
            {
                return customBounds;
            }

            set
            {
                customBounds = value;
            }
        }

        private bool isPDFExport = false;

        /// <summary>
        /// Enable/Disable ToplevelGroupCaption painting in GridGroupingControl.
        /// </summary>
        protected bool IsPDFExport
        {
            get
            {
                return isPDFExport;
            }
            set
            {
                if (isPDFExport != value)
                    isPDFExport = value;
            }
        }

        static PageSettings helperPageSettings;
        /// <summary>
        /// get / set the page settings for HelperPageSettings
        /// </summary>
        public static PageSettings HelperPageSettings
        {
            get
            {
                return helperPageSettings;
            }
            set
            {
                  if (helperPageSettings != value)
                  {
                      helperPageSettings = value;
                      PageSettingsChanged = true;
                  }
            }
        }
        /// <summary>
        /// set the bool value for PageSettingsChanged
        /// </summary>
        protected static bool PageSettingsChanged = false;
     
        /// <override/>
        protected override void OnBeginPrint(PrintEventArgs ev)
        {
            if (GridPrintDocument.HelperPageSettings != null)
            {
                this.DefaultPageSettings = GridPrintDocument.HelperPageSettings;
               
            }

#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN");
            }
#else
            ;
#endif
            base.OnBeginPrint(ev);
            m_nCurPage = 0;

            GridProperties pProp = m_grid.Model.Properties;

            m_grid.Model.PushPrintFloatingCells();
            m_grid.PrintInfo.m_awPageFirstRow = new ArrayList();
            m_grid.PrintInfo.m_awPageFirstCol = new ArrayList();

            int nCurRow = m_grid.TopRowIndex,
                nCurCol = m_grid.LeftColIndex;

            int nTopRow = m_grid.TopRowIndex;
            int nLeftCol = m_grid.LeftColIndex;

            m_grid.PrintInfo.m_bPrintPaintMsg = false;

            m_grid.PrintingMode = true;

            try
            {
                // PrintBounds gets returned by GridBounds while in PrintingMode.
                Rectangle bounds = this.DefaultPageSettings.Bounds;

                if (!CustomBounds.IsEmpty)
                {
                    bounds = this.CustomBounds;
                }

                bounds.Width -= this.DefaultPageSettings.Margins.Left + this.DefaultPageSettings.Margins.Right;
                bounds.Height -= this.DefaultPageSettings.Margins.Top + this.DefaultPageSettings.Margins.Bottom;
                m_grid.PrintBounds = bounds;
                Rectangle rectGrid = m_grid.GridBounds;

                //// Compute page breaks.

                GridRangeInfoList pSelList = m_grid.Model.SelectedRanges;
                GridRangeInfo selRange = pSelList.ActiveRange;
                bool isSel = selRange.Width > 1 || selRange.Height > 1;

                bool printSel = this.PrinterSettings.PrintRange == PrintRange.Selection; ////  || this.m_printPreview;
                ////|| this.PrintController is PreviewPrintController;

                bool printPages = this.PrinterSettings.PrintRange == PrintRange.SomePages;
                fromPage = printPages ? this.PrinterSettings.FromPage : 1;
                toPage = printPages ? this.PrinterSettings.ToPage : int.MaxValue;

                bool bCurPageOnly = printSel && !isSel;
                bool bCurSelOnly = printSel && isSel;

                if (bCurPageOnly)
                {
                    int bottomRow,
                        rightCol;

                    // Just use top left cell.
                    m_grid.PrintInfo.m_nPrintTopRow = nTopRow;
                    m_grid.PrintInfo.m_nPrintLeftCol = nLeftCol;
                    m_grid.PrintInfo.m_awPageFirstCol.Add(m_grid.PrintInfo.m_nPrintLeftCol);
                    m_grid.PrintInfo.m_awPageFirstRow.Add(m_grid.PrintInfo.m_nPrintTopRow);
                    m_grid.ViewLayout.Reset();

                    // ... and calculate boottom right cell.
                    rightCol = m_grid.ViewLayout.LastVisibleCol;
                    bottomRow = m_grid.ViewLayout.LastVisibleRow;

                    if (!m_grid.ViewLayout.HasPartialVisibleCols)
                    {
                        rightCol++;
                    }

                    if (!m_grid.ViewLayout.HasPartialVisibleRows)
                    {
                        bottomRow++;
                    }

                    m_grid.PrintInfo.m_awPageFirstCol.Add(rightCol);
                    m_grid.PrintInfo.m_awPageFirstRow.Add(bottomRow);
                    this.PrinterSettings.ToPage = 1;
                    this.PrinterSettings.MaximumPage = 1;
                }
                else
                {
                    GridRangeInfo rangePrint;

                    // ExpandRange is necessary if user wants to
                    // print selected rows or columns.
                    if (bCurSelOnly)
                    {
                        rangePrint = pSelList.ActiveRange;
                    }
                    else
                    {
                        rangePrint = GridRangeInfo.Table();
                    }

                    rangePrint = rangePrint.ExpandRange(
                        m_grid.InternalGetFrozenRows() + 1,
                        m_grid.InternalGetFrozenCols() + 1,
                        m_grid.Model.RowCount,
                        m_grid.Model.ColCount);

                    int bottomRow,
                        rightCol;

                    // I must take care of frozen rows and columns
                    m_grid.PrintInfo.m_nPrintTopRow = Math.Max(m_grid.InternalGetFrozenRows() + 1, rangePrint.Top);
                    m_grid.PrintInfo.m_nPrintLeftCol = Math.Max(m_grid.InternalGetFrozenCols() + 1, rangePrint.Left);

                    ////commented out to handle Issue#356
                    ////m_grid.PrintInfo.m_bPrintCurSelOnly = bCurSelOnly;

                    m_grid.PrintInfo.m_awPageFirstCol.Add(m_grid.PrintInfo.m_nPrintLeftCol);
                    m_grid.PrintInfo.m_nCurrentPageColIndex = 0;
                    m_grid.PrintInfo.m_nCurrentPageRowIndex = 0;
                    m_grid.ViewLayout.Reset();
                    
                    // Compute left column for each page.
                    int nPage = 0;
                    bool bEOF = false;
                    while (!bEOF)
                    {
                        ////bool isLargeColumn = false;
                        rightCol = m_grid.ViewLayout.LastVisibleCol;

                        ////if (rightCol <= m_grid.PrintInfo.m_nPrintLeftCol)
                        ////{
                        ////    isLargeColumn = true;
                        ////}

                        bEOF = !m_grid.ViewLayout.HasPartialVisibleCols;
                        if (bEOF || rightCol == m_grid.PrintInfo.m_nPrintLeftCol)
                        {
                            rightCol++;
                        }

                        if (rightCol > rangePrint.Right)
                        {
                            rightCol = rangePrint.Right + 1;
                            bEOF = true;
                        }

                        ++nPage;
                        if (nPage < m_grid.PrintInfo.m_awPageFirstCol.Count)
                        {
                            m_grid.PrintInfo.m_awPageFirstCol[nPage] = rightCol;
                        }
                        else
                        {
                            m_grid.PrintInfo.m_awPageFirstCol.Add(rightCol);
                        }

                        m_grid.PrintInfo.m_nPrintLeftCol = rightCol;
                        //// Commented out - otherwise large columns will get skipped.
                        ////if (isLargeColumn)
                        ////    m_grid.PrintInfo.m_nPrintLeftCol++;
                        m_grid.ViewLayout.Reset();
                    }

                    m_grid.PrintInfo.m_nPrintTopRow = Math.Max(m_grid.InternalGetFrozenRows() + 1, rangePrint.Top);
                    m_grid.PrintInfo.m_nPrintLeftCol = Math.Max(m_grid.InternalGetFrozenCols() + 1, rangePrint.Left);
                    m_grid.PrintInfo.m_awPageFirstRow.Add(m_grid.PrintInfo.m_nPrintTopRow);
                    m_grid.ViewLayout.Reset();

                    // Compute top row for each page.
                    nPage = 0;
                    bEOF = false;
                    while (!bEOF)
                    {
                        ////bool isLargeRow = false;
                        bottomRow = m_grid.ViewLayout.LastVisibleRow;

                        ////if (bottomRow <= m_grid.PrintInfo.m_nPrintTopRow)
                        ////{
                        ////    isLargeRow = true;
                        ////}

                        bEOF = !m_grid.ViewLayout.HasPartialVisibleRows;
                        if (bEOF || bottomRow == m_grid.PrintInfo.m_nPrintTopRow)
                        {
                            bottomRow++;
                        }

                        if (bottomRow > rangePrint.Bottom)
                        {
                            bottomRow = rangePrint.Bottom + 1;
                            bEOF = true;
                        }
                        ////else if (bottomRow == rangePrint.Bottom)
                        ////    bEOF = true;

                        ++nPage;
                        if (nPage < m_grid.PrintInfo.m_awPageFirstRow.Count)
                        {
                            m_grid.PrintInfo.m_awPageFirstRow[nPage] = bottomRow;
                        }
                        else
                        {
                            m_grid.PrintInfo.m_awPageFirstRow.Add(bottomRow);
                        }

                        m_grid.PrintInfo.m_nPrintTopRow = bottomRow;
                        //// Commented out - otherwise large row will get skipped.
                        ////if (isLargeRow)
                        ////    m_grid.PrintInfo.m_nPrintTopRow++;
                        m_grid.ViewLayout.Reset();
                    }

                    // Set no of pages.
                    this.PrinterSettings.ToPage = (m_grid.PrintInfo.m_awPageFirstRow.Count - 1) * (m_grid.PrintInfo.m_awPageFirstCol.Count - 1);
                    this.PrinterSettings.MaximumPage = this.PrinterSettings.ToPage;

                    if (m_printPreview || this.PrintController is PreviewPrintController)
                    {
                        if (pProp.PageOrder == 0)
                        {
                            m_nCurPage = (m_grid.PrintInfo.m_nCurrentPageRowIndex * (m_grid.PrintInfo.m_awPageFirstCol.Count - 1))
                                 + m_grid.PrintInfo.m_nCurrentPageColIndex + 1;
                        }
                        else
                        {
                            m_nCurPage = (m_grid.PrintInfo.m_nCurrentPageColIndex * (m_grid.PrintInfo.m_awPageFirstRow.Count - 1))
                                 + m_grid.PrintInfo.m_nCurrentPageRowIndex + 1;
                        }
                    }
                    else
                    {
                        m_nCurPage = fromPage - 1;
                    }
                }

                // Reset printing mode.
                m_grid.PrintingMode = false;
            }
            finally
            {
                // Reset printing mode.
                m_grid.PrintingMode = false;
            }
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END");
            }
#else
            ;
#endif
        }

        ////Override the OnPrintPage to provide the printing logic for the document.

        /// <override/>
        protected override void OnPrintPage(PrintPageEventArgs ev)
        {
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN");
            }
#else
            ;
#endif
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ev.HasMorePages, ev.MarginBounds, ev.PageBounds, ev.PageSettings);
            }
#else
            ;
#endif
            base.OnPrintPage(ev);

            // TODO: how to access page numer of print controller?
            m_nCurPage++;

            GridProperties pProp = m_grid.Model.Properties;

            int bottomRow,
                        rightCol;

            m_grid.PrintInfo.m_bPrintPaintMsg = false;
            m_grid.PrintingMode = true;

            try
            {
                // Check page break array.
                if (m_grid.PrintInfo.m_awPageFirstRow.Count == 0
                    || m_grid.PrintInfo.m_awPageFirstCol.Count == 0)
                {
                    Trace.WriteLineIf(Switches.Printing.TraceWarning, "Warning! You must call OnBeginPrinting and OnEndPrinting from your derived class!\n");
                    Debug.Assert(false);
                }

                ////m_grid.OnAdjustPrintRectangle(g, pInfo);
                ////                Rectangle bounds = this.DefaultPageSettings.Bounds;
                ////                bounds.Width -= this.DefaultPageSettings.Margins.Left + this.DefaultPageSettings.Margins.Right;
                ////                bounds.Height -= this.DefaultPageSettings.Margins.Top + this.DefaultPageSettings.Margins.Bottom;
                ////                m_grid.PrintBounds = bounds;
                m_grid.PrintBounds = ev.MarginBounds;

                // Page order.
                int nPageRowIndex = 0;
                int nPageColIndex = 0;

                // Page order.
                if (pProp.PageOrder == 0)
                {
                    nPageRowIndex = (m_nCurPage - 1) / (m_grid.PrintInfo.m_awPageFirstCol.Count - 1);
                    nPageColIndex = (m_nCurPage - 1) % (m_grid.PrintInfo.m_awPageFirstCol.Count - 1);
                }
                else
                {
                    nPageRowIndex = (m_nCurPage - 1) % (m_grid.PrintInfo.m_awPageFirstRow.Count - 1);
                    nPageColIndex = (m_nCurPage - 1) / (m_grid.PrintInfo.m_awPageFirstRow.Count - 1);
                }

                m_grid.PrintInfo.m_nCurrentPageColIndex = nPageColIndex;
                m_grid.PrintInfo.m_nCurrentPageRowIndex = nPageRowIndex;

                // top left cell
                m_grid.PrintInfo.m_nPrintTopRow = (int)m_grid.PrintInfo.m_awPageFirstRow[nPageRowIndex];
                m_grid.PrintInfo.m_nPrintLeftCol = (int)m_grid.PrintInfo.m_awPageFirstCol[nPageColIndex];
                m_grid.ViewLayout.Reset();
                
                Rectangle rectGrid = m_grid.GridBounds;

                GridRangeInfoList pSelList = m_grid.Model.SelectedRanges;
                GridRangeInfo selRange = pSelList.ActiveRange;
                bool isSel = selRange.Width > 1 || selRange.Height > 1;

                bool printSel = this.PrinterSettings.PrintRange == PrintRange.Selection
                    || this.PrintController is PreviewPrintController || this.m_printPreview;

                bool bCurPageOnly = printSel && !isSel;
                bool bCurSelOnly = printSel && isSel;

                // ... and bottom right cell.
                if (nPageRowIndex + 1 < m_grid.PrintInfo.m_awPageFirstRow.Count)
                {
                    bottomRow = m_grid.GetClientRow(Math.Min(m_grid.Model.RowCount + 1, (int)m_grid.PrintInfo.m_awPageFirstRow[nPageRowIndex + 1])) - 1;
                }
                else
                {
                    bottomRow = m_grid.GetClientRow(m_grid.Model.RowCount);
                }

                if (nPageColIndex + 1 < m_grid.PrintInfo.m_awPageFirstCol.Count)
                {
                    rightCol = m_grid.GetClientCol(Math.Min(m_grid.Model.ColCount + 1, (int)m_grid.PrintInfo.m_awPageFirstCol[nPageColIndex + 1])) - 1;
                }
                else
                {
                    rightCol = m_grid.GetClientCol(m_grid.Model.ColCount);
                }

                // Compute drawing rectangle.
                int x, y;
                Rectangle rectPrint = m_grid.PrintBounds;

                Point corner = m_grid.ViewLayout.ClientRowColToPoint(bottomRow + 1, rightCol + 1, GridCellSizeKind.ActualSize);

                if (m_grid.IsRightToLeft())
                {
                    x = Math.Min(rectPrint.Width, rectPrint.Right - corner.X);
                    y = Math.Min(rectPrint.Height, corner.Y - rectPrint.Top);
                }
                else
                {
                    x = Math.Min(rectPrint.Width, corner.X - rectPrint.Left);
                    y = Math.Min(rectPrint.Height, corner.Y - rectPrint.Top);
                }

                int rl, rt;
                // ... and center it.
                if (pProp.CenterHorizontal)
                {
                    rl = rectPrint.Left + ((rectPrint.Width - x) / 2);
                }
                else
                {
                    rl = rectPrint.Left;
                }

                if (pProp.CenterVertical)
                {
                    rt = rectPrint.Top + ((rectPrint.Height - y) / 2);
                }
                else
                {
                    rt = rectPrint.Top;
                }

                m_grid.PrintBounds = new Rectangle(rl, rt, x + 1, y + 1);
                m_grid.ViewLayout.Reset();

                rectPrint = new Rectangle(rl - 1, rt - 1, x + 1, y + 1);
#if DEBUG

                if (Switches.Printing.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo("Clip", rectPrint);
                }
#else

                ;
#endif
                Region clip = ev.Graphics.Clip;
                ev.Graphics.IntersectClip(rectPrint);

                m_grid.Model.FloatingCells.EvaluateFloatingCells(GridRangeInfo.Cells(m_grid.GetRow(1), m_grid.GetCol(1), m_grid.GetRow(bottomRow), m_grid.GetCol(rightCol)));

                m_grid.Model.MergeCells.EvaluateMergeCells(GridRangeInfo.Cells(m_grid.GetRow(1), m_grid.GetCol(1), m_grid.GetRow(bottomRow), m_grid.GetCol(rightCol)));

                int row = 0;
                if ((!(m_grid is GridControl || m_grid is GridDataBoundGrid)) && this.IsPDFExport)
                {
                    bool incr = false;
                    for (row = 1; row <= bottomRow; row++)
                    {
                        GridStyleInfo style = m_grid.GetPaintStyleInfo(row, 0, true);
                        if (style.CellType == "Header")
                        {
                            using (SolidBrush br = new SolidBrush(style.BackColor))
                            {
                                ev.Graphics.FillRectangle(br, ev.Graphics.ClipBounds);
                                m_grid.OnDrawItem(ev.Graphics, row, 0, ev.MarginBounds, style);
                            }
                            incr = true;
                            break;
                        }
                    }
                    if (incr)
                    {
                        row++;
                    }
                }

                // Draw page.
                m_grid.OnDrawClientRowCol(row, 0, bottomRow, rightCol, ev.Graphics, rectPrint);

                // Check if there are pages to go.
                ev.HasMorePages = !bCurPageOnly &&
                     (nPageRowIndex + 2 < m_grid.PrintInfo.m_awPageFirstRow.Count ||
                    nPageColIndex + 2 < m_grid.PrintInfo.m_awPageFirstCol.Count) &&
                    this.m_nCurPage < this.toPage;

                ev.Graphics.Clip = clip;

                // Draw border around grid.
                if (pProp.PrintFrame)
                {
                    Rectangle rectFrame = new Rectangle(rl - 1, rt - 1, x + 1, y + 1);
                    int penWidth = 2;
                    rectFrame.Inflate(penWidth - 3, penWidth - 3);
                    Pen pen = new Pen(Color.Black, penWidth);
                    ev.Graphics.DrawRectangle(pen, rectFrame);
                    pen.Dispose();
                    ////                    Brush br = new SolidBrush(Color.Black);
                    ////                    ev.Graphics.FillRectangle(br, rl-2, rt-2, x+3, 1);
                    ////                    ev.Graphics.FillRectangle(br, rl-2, rt+y, x+3, 1);
                    ////                    ev.Graphics.FillRectangle(br, rl-2, rt-2, 1, y+3);
                    ////                    ev.Graphics.FillRectangle(br, rl+x, rt-2, 1, y+3);
                }

                // Reset printing mode.
                m_grid.PrintingMode = false;
            }
            finally
            {
                // Reset printing mode.
                m_grid.PrintingMode = false;
            }
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END");
            }
#else

            ;
#endif
        }

        /// <override/>
        protected override void OnEndPrint(PrintEventArgs e)
        {
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN");
            }
#else
            ;
#endif
            m_grid.Model.ResetPrintFloatingCells();
            m_grid.PrintInfo.m_awPageFirstRow = null;
            m_grid.PrintInfo.m_awPageFirstCol = null;
            m_grid.Model.Properties.ThemedHeader = false;
#if DEBUG
            if (Switches.Printing.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END");
            }
#else
            ;
#endif
        }
    }
}
