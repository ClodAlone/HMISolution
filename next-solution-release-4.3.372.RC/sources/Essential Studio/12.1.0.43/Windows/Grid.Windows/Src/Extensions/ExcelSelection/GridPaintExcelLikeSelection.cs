//-------------------------------------------------------------------------------------------------
// <copyright file="GridPaintExcelLikeSelection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum GridSelectionMarkerLocation
    {
        /// <summary>
        /// Represents None
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents Default
        /// </summary>
        Default = 1,

        /// <summary>
        /// Represents Top
        /// </summary>
        Top = 2,

        /// <summary>
        /// Represents Left
        /// </summary>
        Left = 3
    }

    /// <summary>
    /// Helper class for drawing of Excel-like selection frame.
    /// </summary>
    public class GridPaintExcelLikeSelection : GridSubComponent, IGridDrawSelectionFrame
    {
        private bool isLockedDrawSelectionFrame;
        private GridRangeInfo m_rgLastSelectionFrame = GridRangeInfo.Empty;
        private GridSelectionMarkerLocation m_nLastSelectionFrameMarker;
        bool inScroll = false;
        GridControlBase grid;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GPE(" + (isLockedDrawSelectionFrame ? "L " : string.Empty)
                + m_rgLastSelectionFrame.ToString() + " " +
                m_nLastSelectionFrameMarker.ToString() + ")" + grid.ToString();
        }

        /// <summary>
        /// Initializes a GridPaintExcelLikeSelection and attaches it to the grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridPaintExcelLikeSelection(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            grid.SelectionFrameChanging += new GraphicsEventHandler(GridSelectionFrameChanging);
            grid.SelectionFrameChanged += new GraphicsEventHandler(GridSelectionFrameChanged);
            grid.LeftColChanging += new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
            grid.TopRowChanging += new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
            grid.LeftColChanged += new GridRowColIndexChangedEventHandler(GridAfterScrolled);
            grid.TopRowChanged += new GridRowColIndexChangedEventHandler(GridAfterScrolled);
            grid.Paint += new PaintEventHandler(GridPaint);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                grid.SelectionFrameChanging -= new GraphicsEventHandler(GridSelectionFrameChanging);
                grid.SelectionFrameChanged -= new GraphicsEventHandler(GridSelectionFrameChanged);
                grid.LeftColChanging -= new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
                grid.TopRowChanging -= new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
                grid.LeftColChanged -= new GridRowColIndexChangedEventHandler(GridAfterScrolled);
                grid.TopRowChanged -= new GridRowColIndexChangedEventHandler(GridAfterScrolled);
                grid.Paint -= new PaintEventHandler(GridPaint);
            }

            base.Dispose(disposing);
        }

        internal static void GridInvertRect(GridControlBase pGrid, Graphics g, Rectangle rect, Rectangle rectClip)
        {
            if (!rectClip.IsEmpty)
            {
                rect = Rectangle.Intersect(rect, rectClip);
            }

            if (!rect.IsEmpty)
            {
                ////TraceUtil.TraceCurrentMethodInfoIf(Switches.SelectRange.TraceVerbose, rect, rectClip);

                pGrid.InvertRect(g, rect);
            }
        }

        /*public*/
        void DrawInvertFrame(GridControlBase pGrid, Graphics g, Rectangle rc, Rectangle rectClip, bool bTopVisible, bool bLeftVisible, GridSelectionMarkerLocation nMarker, bool bOnlyMarker)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rc, rectClip, bTopVisible, bLeftVisible, nMarker, bOnlyMarker, this);
            }
#else
            ;
#endif
            // Gridlinesbehavior
            Rectangle r = rc;

            rectClip.Intersect(Rectangle.Ceiling(g.ClipBounds));
            // Fix Incident 14724 - drawing problem with excel-like selection frame and frozen columns
            if (rectClip.IsEmpty || rectClip.Height == 0 || rectClip.Width == 0)
            {
                return;
            }

            int nyTop = bTopVisible ? 0 : 1;

            // New grid line mode (cell borders at bottom right of cell).
            r.Offset(-1, -1);

            if (!bOnlyMarker)
            {
                if (bTopVisible)
                {
                    // Outside top border.
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 1, r.Top - 1, r.Right + 2, r.Top + 1), rectClip);

                    // Inside top border.
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Top + 2, r.Right - 2, r.Top + 3), rectClip);
                }
                else if (bLeftVisible && r.Bottom > r.Top) 
                {
                    // Last row is completely visible.
                    // Top left pixel (at grid line).
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Top, r.Left + 3, r.Top + 3), rectClip);
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 2, r.Top, r.Right - 1, r.Top + 1), rectClip);
                }

                if (bLeftVisible)
                {
                    // Outside left border.
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 1, r.Top + 1, r.Left + 1, r.Bottom + 2), rectClip);

                    // Inside left border.
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Top + 3, r.Left + 3, r.Bottom - 2), rectClip);
                }
                else if (bTopVisible && r.Right > r.Left) 
                {
                    // Last column is completely visible.
                    // Bottom left pixel (at grid line).
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left, r.Bottom - 2, r.Left + 2, r.Bottom - 1), rectClip);
                    GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left, r.Top + 2, r.Left + 2, r.Top + 3), rectClip);
                }

                // Outside bottom border.
                GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 1, r.Bottom, r.Right + 1, r.Bottom + 2), rectClip);

                // Outside right border.
                GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right, r.Top + 1, r.Right + 2, r.Bottom + 2), rectClip);
                
                // Inside bottom border.
                GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Bottom - 2, r.Right - 2, r.Bottom - 1), rectClip);

                // Inside right border.
                GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 2, r.Top + 2 - nyTop, r.Right - 1, r.Bottom - 1), rectClip);
            }

            if (!inScroll)
            {
                // Check nMarker (in case full rows or cols are selected).
                if (Grid.IsRightToLeft())
                {
                    switch (nMarker)
                    {
                        case GridSelectionMarkerLocation.Left:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 7, r.Bottom - 1, r.Right - 2, r.Bottom + 2), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 7, r.Bottom - 3, r.Right - 3, r.Bottom - 2), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 6, r.Bottom - 2, r.Right - 1, r.Bottom + 3), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 2, r.Bottom + 2, r.Right - 1, r.Bottom + 3), rectClip);
                            break;

                        case GridSelectionMarkerLocation.Top:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 2, r.Top + 2, r.Left + 1, r.Top + 7), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Top + 3, r.Left + 3, r.Top + 7), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 3, r.Top + 1, r.Left + 2, r.Top + 6), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 3, r.Top + 1, r.Left - 2, r.Top + 2), rectClip);
                            break;

                        default:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 2, r.Bottom - 2, r.Left + 1, r.Bottom + 3), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 3, r.Bottom - 1, r.Left + 2, r.Bottom + 3), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left - 3, r.Bottom + 2, r.Left + 2, r.Bottom + 3), rectClip);
                            break;
                    }
                }
                else
                {
                    switch (nMarker)
                    {
                        case GridSelectionMarkerLocation.Left:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 2, r.Bottom - 1, r.Left + 7, r.Bottom + 2), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 3, r.Bottom - 3, r.Left + 7, r.Bottom - 2), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 1, r.Bottom - 2, r.Left + 6, r.Bottom + 3), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Left + 1, r.Bottom + 2, r.Left + 2, r.Bottom + 3), rectClip);
                            break;

                        case GridSelectionMarkerLocation.Top:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 1, r.Top + 2, r.Right + 2, r.Top + 7), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 3, r.Top + 3, r.Right - 2, r.Top + 7), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 2, r.Top + 1, r.Right + 3, r.Top + 6), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right + 2, r.Top + 1, r.Right + 3, r.Top + 2), rectClip);
                            break;

                        default:
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 1, r.Bottom - 2, r.Right + 2, r.Bottom + 3), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right - 2, r.Bottom - 1, r.Right + 3, r.Bottom + 2), rectClip);
                            GridInvertRect(pGrid, g, Rectangle.FromLTRB(r.Right + 2, r.Bottom + 2, r.Right + 3, r.Bottom + 3), rectClip);
                            break;
                    }
                }
            }
        }

        /*public*/
        void DrawSelectionRangeFrame(GridControlBase pGrid, Graphics g, GridRangeInfo rg, GridSelectionMarkerLocation nMarker, bool bOnlyMarker)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rg, nMarker, bOnlyMarker, this);
            }
#else
            ;
#endif
            int nfc = pGrid.InternalGetFrozenCols();
            int nfr = pGrid.InternalGetFrozenRows();
            int nLeftCol = pGrid.LeftColIndex;
            int nTopRow = pGrid.TopRowIndex;

            int nlRow = nTopRow - 1;
            while (nlRow > nfr && pGrid.GetRowHeight(nlRow) == 0)
            {
                nlRow--;
            }

            int nlCol = nLeftCol - 1;
            while (nlCol > nfc && pGrid.GetColWidth(nlCol) == 0)
            {
                nlCol--;
            }

            bool bAnyColVisible = rg.Left <= nfc || (rg.Right >= nlCol && rg.Left <= pGrid.ViewLayout.LastVisibleCol);
            bool bAnyRowVisible = rg.Top <= nfr || (rg.Bottom >= nlRow && rg.Top <= pGrid.ViewLayout.LastVisibleRow);

            if (!bAnyColVisible || !bAnyRowVisible)
            {
                return;
            }

            ////            if (pGrid.CurrentCell.HasCurrentCell && rg.Contains(pGrid.CurrentCell.RangeInfo))
            ////                pGrid.DrawInvertCurrentCell(g, pGrid.CurrentCell.RowIndex, pGrid.CurrentCell.ColIndex);

            // Last row completely visible (with outside border).
            bool bLastRowFullyVisible = rg.Bottom >= nTopRow;
            bool bLastColFullyVisible = rg.Right >= nLeftCol;

            // Only the bottom border of the last row is visible.
            bool bLastRowOnlyBorderVisible = (!bLastRowFullyVisible && rg.Bottom >= nlRow && rg.Bottom < nTopRow)
                || (rg.Bottom == nfr && rg.Bottom + 1 == nTopRow);
            bool bLastColOnlyBorderVisible = (!bLastColFullyVisible && rg.Right >= nlCol && rg.Right < nLeftCol)
                || (rg.Right == nfc && rg.Right + 1 == nLeftCol);

            // First row is completely visible (with outside border).
            bool bFirstRowVisible = rg.Top <= nfr || rg.Top > nTopRow || (rg.Top == nTopRow && nfr + 1 == nTopRow);
            bool bFirstColVisible = rg.Left <= nfc || rg.Left > nLeftCol || (rg.Left == nLeftCol && nfc + 1 == nLeftCol);

            bool bFirstRowClipBorder = !bFirstRowVisible && rg.Top == nTopRow;
            bool bFirstColClipBorder = !bFirstColVisible && rg.Left == nLeftCol;

            //// I have to take care on frozen rows and columns:
            ////    +--|-----------+
            ////    |r0|r2         |
            ////   ======================= top row
            ////    |r1|r3         |
            ////    |  |           |r
            ////    +--|-----------+
            ////       |
            ////      left column

            Rectangle rectGrid = pGrid.GridBounds;
            int nyMax = rectGrid.Height;
            int nxMax = rectGrid.Width;

            ////r[i].Offset(rectGrid.Location);

            // nxLeft used for clipping, so use visible size instead of actual size
            int nxLeft = pGrid.ViewLayout.GetColRangeWidth(0, nfc, nxMax, GridCellSizeKind.VisibleSize);
            int nyTop = pGrid.ViewLayout.GetRowRangeHeight(0, nfr, nyMax, GridCellSizeKind.VisibleSize);

            Rectangle[] r = new Rectangle[4];
            Rectangle[] rcClip = new Rectangle[4];
            bool[] bTopRow = new bool[4];
            bool[] bLeftCol = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                rcClip[i] = Rectangle.Empty;
                r[i] = Rectangle.Empty;
                bTopRow[i] = bLeftCol[i] = false;
            }

            if (rg.Left <= nfc && rg.Top < nTopRow)
            {
                // TODO: Review if I should use actual or visible size ...
                int top = rg.Top > 0 ? pGrid.ViewLayout.GetRowRangeHeight(0, rg.Top - 1, nyMax, GridCellSizeKind.ActualSize) : 0;
                int left = rg.Left > 0 ? pGrid.ViewLayout.GetColRangeWidth(0, rg.Left - 1, nxMax, GridCellSizeKind.ActualSize) : 0;
                int bottom = top + pGrid.ViewLayout.GetRowRangeHeight(rg.Top, Math.Min(nfr, rg.Bottom), nyMax, GridCellSizeKind.VisibleSize);
                int right = left + pGrid.ViewLayout.GetColRangeWidth(rg.Left, Math.Min(nfc, rg.Right), nxMax, GridCellSizeKind.VisibleSize);
                r[0] = Rectangle.FromLTRB(left, top, right, bottom);
                rcClip[0] = Rectangle.FromLTRB(0, 0, nxLeft, nyTop);
                bTopRow[0] = bLeftCol[0] = true;
            }

            if (rg.Left <= nfc && rg.Bottom >= nTopRow)
            {
                int top;
                if (bFirstRowVisible || bFirstRowClipBorder)
                {
                    top = rectGrid.Y + pGrid.ViewLayout.GetClientRowRangeHeight(0, pGrid.GetClientRow(rg.Top) - 1, nyMax, GridCellSizeKind.ActualSize);
                }
                else
                {
                    top = nyTop;
                }

                int left = rg.Left > 0 ? pGrid.ViewLayout.GetColRangeWidth(0, rg.Left - 1, nxMax, GridCellSizeKind.ActualSize) : 0;
                int bottom = nyTop + pGrid.ViewLayout.GetRowRangeHeight(nTopRow, rg.Bottom, nyMax, GridCellSizeKind.VisibleSize);
                int right = left + pGrid.ViewLayout.GetColRangeWidth(rg.Left, rg.Right, nxMax, GridCellSizeKind.VisibleSize);
                r[1] = Rectangle.FromLTRB(left, top, right, bottom);
                rcClip[1] = Rectangle.FromLTRB(0, nyTop, nxLeft, rectGrid.Height);
                bLeftCol[1] = true;
                bTopRow[1] = rg.Top >= nTopRow;
            }

            if (rg.Top <= nfr && rg.Right >= nLeftCol)
            {
                int top = rg.Top > 0 ? pGrid.ViewLayout.GetRowRangeHeight(0, rg.Top - 1, nyMax, GridCellSizeKind.ActualSize) : 0;
                int left;
                if (bFirstColVisible || bFirstColClipBorder)
                {
                    left = rectGrid.X + pGrid.ViewLayout.GetClientColRangeWidth(0, pGrid.GetClientCol(rg.Left) - 1, GridCellSizeKind.ActualSize);
                }
                else
                {
                    left = nxLeft;
                }

                int bottom = top + pGrid.ViewLayout.GetRowRangeHeight(rg.Top, rg.Bottom, nyMax, GridCellSizeKind.VisibleSize);
                int right = nxLeft + pGrid.ViewLayout.GetColRangeWidth(nLeftCol, rg.Right, nxMax, GridCellSizeKind.VisibleSize);
                r[2] = Rectangle.FromLTRB(left, top, right, bottom);
                rcClip[2] = Rectangle.FromLTRB(nxLeft, 0, rectGrid.Width, nyTop);
                bLeftCol[2] = rg.Left >= nLeftCol;
                bTopRow[2] = true;
            }

            if ((rg.Bottom >= nTopRow || bLastRowOnlyBorderVisible)
                && (rg.Right >= nLeftCol || bLastColOnlyBorderVisible))
            {
                int top, left;
                if (bFirstRowVisible || bFirstRowClipBorder)
                {
                    top = pGrid.ViewLayout.GetClientRowRangeHeight(0, pGrid.GetClientRow(rg.Top) - 1, GridCellSizeKind.ActualSize);
                }
                else
                {
                    top = nyTop;
                }

                if (bFirstColVisible || bFirstColClipBorder)
                {
                    left = pGrid.ViewLayout.GetClientColRangeWidth(0, pGrid.GetClientCol(rg.Left) - 1, GridCellSizeKind.ActualSize);
                }
                else
                {
                    left = nxLeft;
                }

                int bottom = nyTop + pGrid.ViewLayout.GetRowRangeHeight(nTopRow, rg.Bottom, nyMax, GridCellSizeKind.VisibleSize);
                int right = nxLeft + pGrid.ViewLayout.GetColRangeWidth(nLeftCol, rg.Right, nxMax, GridCellSizeKind.VisibleSize);
                r[3] = Rectangle.FromLTRB(left, top, right, bottom);
                rcClip[3] = Rectangle.FromLTRB(nxLeft, nyTop, rectGrid.Width, rectGrid.Height);
                bTopRow[3] = rg.Top >= nTopRow;
                bLeftCol[3] = rg.Left >= nLeftCol;
            }

            // Now draw the frames (and clip them as necessary).
            for (int i = 0; i < 4; i++)
            {
                if (!rcClip[i].IsEmpty)
                {
                    if (Grid.IsRightToLeft())
                    {
                        r[i] = new Rectangle(pGrid.GridBounds.Right - r[i].Right, r[i].Top, r[i].Width, r[i].Height);
                        rcClip[i] = new Rectangle(pGrid.GridBounds.Right - rcClip[i].Right, rcClip[i].Top, rcClip[i].Width, rcClip[i].Height);

                        r[i].Offset(0, pGrid.GridBounds.Location.Y);
                        rcClip[i].Offset(0, pGrid.GridBounds.Location.Y);
                    }
                    else
                    {
                        r[i].Offset(pGrid.GridBounds.Location);
                        rcClip[i].Offset(pGrid.GridBounds.Location);
                    }

                    DrawInvertFrame(pGrid, g, r[i], rcClip[i], bTopRow[i], bLeftCol[i], nMarker, bOnlyMarker);
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void DrawSelectionFrame(GridControlBase pGrid, Graphics g, bool bDrawOld, GridRangeInfo pNewRange)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(isLockedDrawSelectionFrame, bDrawOld, pNewRange, this);
            }
#else
            ;
#endif
            if (!pGrid.Model.Options.ExcelLikeSelectionFrame || isLockedDrawSelectionFrame)
            {
                return;
            }

            Rectangle rectGrid = pGrid.GridBounds;

            if (bDrawOld && !m_rgLastSelectionFrame.IsEmpty && g != null)
            {
                DrawSelectionRangeFrame(pGrid, g, m_rgLastSelectionFrame, m_nLastSelectionFrameMarker, false);
            }

            m_rgLastSelectionFrame = GridRangeInfo.Empty;

            if (pNewRange != null && !pNewRange.IsEmpty)
            {
                GridRangeInfo rg = pNewRange;

                if (rg.Top == 0 && rg.Left == 0)
                {
                    rg = GridRangeInfo.Table();
                }
                else if (rg.Left == 0)
                {
                    rg = GridRangeInfo.Rows(rg.Top, rg.Bottom);
                }
                else if (rg.Top == 0)
                {
                    rg = GridRangeInfo.Cols(rg.Left, rg.Right);
                }
                else
                {
                    rg = GridRangeInfo.Cells(rg.Top, rg.Left, rg.Bottom, rg.Right);
                }

                if (
                    (rg.IsTable && (grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None)
                    || (rg.IsRows && (grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None)
                    || (rg.IsCols && (grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None)
                    || (rg.IsCells && (grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None))
                {
                    // When rows are selected, draw marker at bottom-left.
                    // When columns are selected, draw marker at top-right.
                    GridSelectionMarkerLocation nMarker;
                    if (rg.IsCols)
                    {
                        nMarker = GridSelectionMarkerLocation.Top;
                    }
                    else if (rg.IsRows)
                    {
                        nMarker = GridSelectionMarkerLocation.Left;
                    }
                    else
                    {
                        nMarker = GridSelectionMarkerLocation.Default;
                    }

                    m_rgLastSelectionFrame = rg.ExpandRange(pGrid.InternalGetHeaderRows() + 1, pGrid.InternalGetHeaderCols() + 1, pGrid.Model.RowCount, pGrid.Model.ColCount);
                    m_nLastSelectionFrameMarker = nMarker;

                    if (g != null)
                    {
                        DrawSelectionRangeFrame(pGrid, g, m_rgLastSelectionFrame, nMarker, false);
                    }
                }
            }
        }

        /*public*/
        void ToggleSelectionFrameMarker(GridControlBase pGrid)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            // Hide marker before scrolling and show it afterwards.
            if (m_nLastSelectionFrameMarker != GridSelectionMarkerLocation.None && !m_rgLastSelectionFrame.IsEmpty)
            {
                Graphics g = pGrid.CreateGridGraphics();
                DrawSelectionRangeFrame(pGrid, g, m_rgLastSelectionFrame, m_nLastSelectionFrameMarker, true);
                g.Dispose();
            }
        }

        void GridSelectionFrameChanging(object sender, GraphicsEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            ResetSelectionFrame(e.Graphics);
        }

        void GridSelectionFrameChanged(object sender, GraphicsEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            DrawSelectionFrame(e.Graphics);
        }

        void GridAfterScrolled(object sender, GridRowColIndexChangedEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e, this);
            }
#else
            ;
#endif
            inScroll = false;
            ToggleSelectionFrameMarker(grid);
        }

        void GridBeforeScrolling(object sender, GridRowColIndexChangingEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e, this);
            }
#else
            ;
#endif
            ToggleSelectionFrameMarker(grid);
            inScroll = true;
        }

        void GridPaint(object sender, PaintEventArgs e)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            DrawSelectionFrame(e.Graphics);
        }
        
        /*public*/
        bool LockSelectionFrame(GridControlBase pGrid, bool bLock)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(bLock, this);
            }
#else
            ;
#endif
            if (!pGrid.Model.Options.ExcelLikeSelectionFrame)
            {
                return false;
            }

            bool bOldLock = isLockedDrawSelectionFrame;

            if (bLock != bOldLock)
            {
                Graphics g = null;

                if (pGrid.ShouldPrepareUpdate() && pGrid.Visible)
                {
                    g = pGrid.CreateGridGraphics();
                }

                isLockedDrawSelectionFrame = false;

                if (bLock)
                {
                    ResetSelectionFrame(g);
                }
                else
                {
                    DrawSelectionFrame(g);
                }

                isLockedDrawSelectionFrame = bLock;

                if (g != null)
                {
                    g.Dispose();
                }
            }

            return bOldLock;
        }

        /*public*/
        void DrawSelectionFrame(Graphics g)
        {
            if (grid.Model.Selections.Ranges.ActiveRange.IsEmpty)
            {
                return;
            }
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            if (g == null)
            {
                g = grid.CreateGraphics();
                g.SetClip(grid.GridBounds);
                DrawSelectionFrame(grid, g, false, grid.Model.Selections.Ranges.ActiveRange);
                g.Dispose();
            }
            else
            {
                DrawSelectionFrame(grid, g, false, grid.Model.Selections.Ranges.ActiveRange);
            }
        }

        void ResetSelectionFrame(Graphics g)
        {
#if DEBUG
            if (Switches.SelectRange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            if (g == null)
            {
                g = grid.CreateGraphics();
                g.SetClip(grid.GridBounds);
                DrawSelectionFrame(grid, g, true, GridRangeInfo.Empty);
                g.Dispose();
            }
            else
            {
                DrawSelectionFrame(grid, g, true, GridRangeInfo.Empty);
            }
        }
    }
}
