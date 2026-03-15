//-------------------------------------------------------------------------------------------------
// <copyright file="GridPaint.cs" company="syncfusion">
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
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridPaint
    {
        GridControlBase m_grid;
        bool isThemedHeader;
        public GridPaint(GridControlBase grid)
        {
            m_grid = grid;
        }

        public void DrawGrid(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            DrawGrid(g, true);
        }

        public void DrawGrid(Graphics g, bool shouldClip)
        {
            Rectangle rectClip = Rectangle.Ceiling(g.ClipBounds);
            DrawGrid(g, shouldClip, rectClip);
        }

        public void DrawGrid(Graphics g, bool shouldClip, Rectangle rectClip)
        {
            try
            {
                //// Get rectangle to be drawn.
                if (rectClip.IsEmpty)
                {
                    rectClip = m_grid.GridBounds;
                }
                else if (!rectClip.IntersectsWith(m_grid.GridBounds))
                {
                    return; //// ... nothing needs drawn
                }

                rectClip = Rectangle.Intersect(m_grid.GridBounds, rectClip);
                Region oldClip = null;
                if (shouldClip)
                {
                    oldClip = g.Clip;
                    g.SetClip(rectClip);
                }

                //// Compute rows and columns.
                int topRow, leftCol, bottomRow, rightCol;
                m_grid.ViewLayout.RectangleToClientRowCol(rectClip, out topRow, out leftCol, out bottomRow, out rightCol, GridCellSizeKind.VisibleSize);
                topRow = Math.Max(0, topRow);
                leftCol = Math.Max(0, leftCol);

                //// ... and draw them.
                m_grid.OnDrawClientRowCol(topRow, leftCol, bottomRow, rightCol, g, rectClip);

                if (shouldClip)
                {
                    g.Clip = oldClip;
                }
            }
            finally
            {
            }
        }
        //// drawing bits (used in OnDrawClientRowCol)

        [Syncfusion.Documentation.DocumentationExclude()]
        internal class DrawCellFlags
        {
            public const int Selected = 0x10;
            public const int Color = 0x20;
            public const int Spanned = 0x40;
            public const int Covered = 0x80;
            public const int SpannedCont = 0x100;
            public const int HorzLine = 0x200;
            public const int VertLine = 0x400;
            public const int FixedVertLine = 0x800;
            public const int FixedHorzLine = 0x1000;
            public const int Merged = 0x2000;
            public const int Bannered = 0x4000;
            public const int BanneredHCont = 0x8000;
            public const int BanneredVCont = 0x10000;
        }

        internal int m_nNestedDraw;

        [Syncfusion.Documentation.DocumentationExclude()]
        internal class DrawStruct ////: IDisposable
        {
            public void Dispose(GridControlBase grid)
            {
                ////apStyles = null;
                apBorders = null;
                apInterior = null;

                if (apStyles != null)
                {
                    for (int i = 0; i < apStyles.Length; i++)
                    {
                        if (apStyles[i] != null && (abBits[i] & GridPaint.DrawCellFlags.SpannedCont) == 0)
                        {
                            grid.DisposePaintStyle(apStyles[i]);
                        }
                    }
                }

                selectionBrush.Dispose();
                pCurrentControl = null;
                standardStyle = null;

                anHeights = null;
                anHeights = null;
                anWidths = null;
                anYOffset = null;
                anXOffset = null;
                anRows = null;
                anCols = null;
                apStyles = null;
                abBits = null;
                GC.SuppressFinalize(this);
            }

            public int nRows, nCols;

            public int gridTopRow,
                gridLeftCol,
                gridFrozenCols,
                gridFrozenRows,
                gridHeaderCols,
                gridHeaderRows;

            public Rectangle rectEdit,
                rectItem;

            public int nxMin,
                nyMin;
            public int nxMax,
                nyMax;

            public int[] anHeights;
            public int[] anWidths;
            public int[] anYOffset;
            public int[] anXOffset;
            public int[] anRows;
            public int[] anCols;
            public GridStyleInfo[] apStyles;
            public BrushInfo[] apInterior;
            public GridBordersInfo[] apBorders;
            public int[] abBits;

            public GridCellRendererBase pCurrentControl;

            public bool bAnyCellCovered;
            public bool bAnyCellSelected;
            public bool bColor;
            public Color rgbStandard;

            public int TopRow,
                LeftCol,
                BottomRow,
                RightCol;

            public Graphics graphics;

            public Rectangle rectClip;
            public Rectangle scrollRect;
            public Rectangle hscrollRect;
            public Rectangle vscrollRect;

            public bool selectionAlphaBlend = false;
            public Brush selectionBrush;

            public GridStyleInfo standardStyle;
        }

        internal DrawStruct m_pDrawStruct = null;
        internal DrawStruct m_pOldDrawStruct = null;
        internal BrushInfo m_tempBrush = null;

        ////        public static int ticks1 = 0;
        ////        public static int  ticks2 = 0;

        bool isRightToLeft()
        {
            return m_grid.IsRightToLeft();
        }

        public void DrawClientRowCol(int topRow, int leftCol, int bottomRow, int rightCol, Graphics g, Rectangle rectClip)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.GridPaint.TraceVerbose, String.Format("DrawClientRowCol({0},{1},{2},{3})", new object[] { m_grid.GetRow(topRow), m_grid.GetCol(leftCol), m_grid.GetRow(bottomRow), m_grid.GetCol(rightCol) }));
#endif
            ////SSTrace.WriteLine(String.Format("\nDrawClientRowCol({0},{1},{2},{3})", new object[]
            ////SS        {
            ////SS            m_grid.GetRow(topRow), m_grid.GetCol(leftCol), m_grid.GetRow(bottomRow), m_grid.GetCol(rightCol)
            ////SS        }) + m_grid.ToString());

            Rectangle oldClip = Rectangle.Ceiling(g.ClipBounds);
            if (m_grid.HasGridBounds)
            {
                g.IntersectClip(m_grid.GridBounds);
            }

            ////            int startTick = Environment.TickCount;

            try
            {
                int rowIndex,
                    colIndex;

                GridRangeInfoList pSelList = m_grid.Model.Selections.Ranges;

                m_nNestedDraw++;
                m_pOldDrawStruct = m_pDrawStruct;
                DrawStruct ds = m_pDrawStruct = new DrawStruct();

                //// Initialize data and fill cells.

                if (!m_grid.CurrentCell.IsInMoveTo)
                {
                    m_grid.ViewLayout.Reset();
                }

                int ll = m_grid.ViewLayout.VisibleRows;

                ds.TopRow = Math.Max(0, Math.Min(topRow, m_grid.ViewLayout.VisibleRows - 1));
                ds.LeftCol = Math.Max(0, Math.Min(leftCol, m_grid.ViewLayout.VisibleCols - 1));
                ds.BottomRow = Math.Max(0, Math.Min(bottomRow, m_grid.ViewLayout.VisibleRows - 1));
                ds.RightCol = Math.Max(0, Math.Min(rightCol, m_grid.ViewLayout.VisibleCols - 1));
                ds.graphics = g;
                if (rectClip.IsEmpty)
                {
                    ds.rectClip = Rectangle.Ceiling(g.ClipBounds);
                }
                else
                {
                    ds.rectClip = rectClip;
                }

                //// Fixed this for 3.0.0.23. In GridInGridCell example the floated
                //// cells were not drawn correctly because ds.rectClip was empty
                //// in DrawSpannedCell.
                if (!m_grid.PrintingMode && m_grid.IsWindowless)
                {
                    ds.rectClip.Intersect(m_grid.GetWindow().ClientRectangle);
                }

                if (topRow < m_grid.ViewLayout.VisibleRows && leftCol < m_grid.ViewLayout.VisibleCols)
                {
                    ds.nRows = ds.BottomRow - ds.TopRow + 1;
                    ds.nCols = ds.RightCol - ds.LeftCol + 1;
                }
                else
                {
                    ds.nRows = ds.nCols = 0;
                }

                ds.gridTopRow = m_grid.TopRowIndex;
                ds.gridLeftCol = m_grid.LeftColIndex;
                ds.gridFrozenCols = m_grid.InternalGetFrozenCols();
                ds.gridFrozenRows = m_grid.InternalGetFrozenRows();
                ds.gridHeaderCols = m_grid.InternalGetHeaderCols();
                ds.gridHeaderRows = m_grid.InternalGetHeaderRows();

                //// RTL: ptLocation.X is on right side!
                Point ptLocation = m_grid.ViewLayout.ClientRowColToPoint(ds.TopRow, ds.LeftCol, GridCellSizeKind.VisibleSize);
                ds.nxMin = ptLocation.X;
                ds.nyMin = ptLocation.Y;
                ds.nxMax = ds.nxMin;
                ds.nyMax = ds.nyMin;

                ds.anHeights = new int[ds.nRows];
                ds.anWidths = new int[ds.nCols];
                ds.anYOffset = new int[ds.nRows];
                ds.anXOffset = new int[ds.nCols];
                ds.anRows = new int[ds.nRows + 1];
                ds.anCols = new int[ds.nCols + 1];
                int cellCount = ds.nRows * ds.nCols;
                ds.apStyles = new GridStyleInfo[cellCount];
                ds.apInterior = new BrushInfo[cellCount];
                ds.apBorders = new GridBordersInfo[cellCount];
                ds.abBits = new int[cellCount];
                ds.pCurrentControl = null;
                ds.bAnyCellCovered = false;
                ds.bAnyCellSelected = /*!ds.pDC->IsPrinting() && */ pSelList.Count > 0;

                ds.standardStyle = m_grid.Model.BaseStylesMap["Standard"].StyleInfo;

                ds.selectionAlphaBlend = (m_grid.Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) != 0;
                ds.selectionBrush = new SolidBrush(m_grid.Model.Options.AlphaBlendSelectionColor);

                ds.bColor = !m_grid.PrintingMode || !m_grid.Model.Properties.BlackWhite;

                ds.rgbStandard = ds.bColor
                    ? ds.standardStyle.Interior.BackColor
                    : Color.White; // white

                int clipPhaseCount = 1;
                int clipPhaseRight = -1;
                int clipPhaseBottom = -1;
                for (rowIndex = 0; rowIndex < ds.nRows; rowIndex++)
                {
                    ds.anRows[rowIndex] = m_grid.GetRow(ds.TopRow + rowIndex);
                    if (ds.anRows[rowIndex] == ds.gridTopRow)
                    {
                        ds.vscrollRect = Rectangle.FromLTRB(0, ds.nyMax, m_grid.GridBounds.Width, m_grid.GridBounds.Height);
                        if (m_grid.vScrollPixelDelta != 0)
                        {
                            ds.nyMax -= m_grid.vScrollPixelDelta;
                            clipPhaseBottom = rowIndex - 1;
                        }

                        clipPhaseCount = 4;
                    }

                    ds.anHeights[rowIndex] = m_grid.GetRowHeight(ds.anRows[rowIndex]);
                    ds.anYOffset[rowIndex] = ds.nyMax;
                    ds.nyMax += ds.anHeights[rowIndex];
                }

                ds.anRows[rowIndex] = m_grid.GetRow(ds.TopRow + rowIndex);
                if (ds.anRows[rowIndex] >= m_grid.Model.RowCount)
                {
                    ds.anRows[rowIndex] = 0;
                }

                if (isRightToLeft())
                {
                    for (colIndex = 0; colIndex < ds.nCols; colIndex++)
                    {
                        ds.anCols[colIndex] = m_grid.GetCol(ds.LeftCol + colIndex);
                        if (ds.anCols[colIndex] == ds.gridLeftCol)
                        {
                            ds.hscrollRect = Rectangle.FromLTRB(0, 0, ds.nxMax, m_grid.GridBounds.Height);
                            if (m_grid.hScrollPixelDelta != 0)
                            {
                                ds.nxMax += m_grid.hScrollPixelDelta;
                                clipPhaseRight = colIndex - 1;
                            }

                            clipPhaseCount = 4;
                        }

                        ds.anWidths[colIndex] = m_grid.GetColWidth(ds.anCols[colIndex]);
                        ds.anXOffset[colIndex] = ds.nxMax;
                        ds.nxMax -= ds.anWidths[colIndex];
                    }

                    ds.anCols[colIndex] = m_grid.GetCol(ds.LeftCol + colIndex);
                    if (ds.anCols[colIndex] >= m_grid.Model.ColCount)
                    {
                        ds.anCols[colIndex] = 0;
                    }
                }
                else
                {
                    for (colIndex = 0; colIndex < ds.nCols; colIndex++)
                    {
                        ds.anCols[colIndex] = m_grid.GetCol(ds.LeftCol + colIndex);
                        if (ds.anCols[colIndex] == ds.gridLeftCol)
                        {
                            ds.hscrollRect = Rectangle.FromLTRB(ds.nxMax, 0, m_grid.GridBounds.Width, m_grid.GridBounds.Height);
                            if (m_grid.hScrollPixelDelta != 0)
                            {
                                ds.nxMax -= m_grid.hScrollPixelDelta;
                                clipPhaseRight = colIndex - 1;
                            }

                            clipPhaseCount = 4;
                        }

                        ds.anWidths[colIndex] = m_grid.GetColWidth(ds.anCols[colIndex]);
                        ds.anXOffset[colIndex] = ds.nxMax;
                        ds.nxMax += ds.anWidths[colIndex];
                    }

                    ds.anCols[colIndex] = m_grid.GetCol(ds.LeftCol + colIndex);
                    if (ds.anCols[colIndex] >= m_grid.Model.ColCount)
                    {
                        ds.anCols[colIndex] = 0;
                    }
                }

                ds.scrollRect = Rectangle.Intersect(ds.hscrollRect, ds.vscrollRect);
                isThemedHeader = m_grid.Model.Properties.ThemedHeader;
                int ncRowIndex = -1, ncColIndex = -1;
                if (m_grid.CurrentCell.HasCurrentCell)
                {
                    m_grid.CurrentCell.GetCurrentCell(out ncRowIndex, out ncColIndex);
                }

                if (m_grid.Model.Options.DrawOrder == GridDrawOrder.Columns)
                {
                    for (colIndex = 0; colIndex < ds.nCols; colIndex++)
                    {
                        for (rowIndex = 0; rowIndex < ds.nRows; rowIndex++)
                        {
                            int i3 = (rowIndex * ds.nCols) + colIndex;
                            if ((rowIndex <= m_grid.Model.Cols.HeaderCount || colIndex <= m_grid.Model.Rows.HeaderCount) && isThemedHeader)
                                ds.apStyles[i3] = this.GetGridPrintStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], true);
                            else
                                ds.apStyles[i3] = m_grid.GetPaintStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], true);
                            ds.apInterior[i3] = ds.apStyles[i3].Interior;
                            ds.apBorders[i3] = ds.apStyles[i3].ReadOnlyBorders;
                        }
                    }
                }

                for (rowIndex = 0; rowIndex < ds.nRows; rowIndex++)
                {
                    for (colIndex = 0; colIndex < ds.nCols; colIndex++)
                    {
                        try
                        {
                            if (ds.anRows[rowIndex] == ncRowIndex && ds.anCols[colIndex] == ncColIndex)
                            {
                                // temporarily set ds.pCurrentControl (will be used
                                // in InternalLoadCell)
                                ds.pCurrentControl = m_grid.GetCellRenderer(ncRowIndex, ncColIndex);
                            }

                            InternalLoadCell(ds, rowIndex, colIndex);
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                            ////                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            ////                                throw ex;
                        }
                    }
                }

                ////                ticks2 += Environment.TickCount-startTick;

                //// Reset pCurrentControl again
                ds.pCurrentControl = null;

                bool bCCellInvert = false;

                //// Mark all cells with "background not yet drawn".
                for (rowIndex = 0; rowIndex < ds.nRows; rowIndex++)
                {
                    for (colIndex = 0; colIndex < ds.nCols; colIndex++)
                    {
                        ds.abBits[(rowIndex * ds.nCols) + colIndex] |= DrawCellFlags.Color;
                    }
                }

                //// Drawing happens in 4 phases:
                ////    +--|-----------+
                ////    |r0|r2         |
                ////   ======================= top row
                ////    |r1|r3         |
                ////    |  |           |
                ////    +--|-----------+
                ////       |
                ////      left column
                //// clipPhase 0: Draw cell left of LeftColIndex and above top row.
                //// clipPhase 1: Draw cell left of LeftColIndex and below top row. Clipping will happen if grid.vScrollPixelDelta != 0 (top cells only partially visible).
                //// clipPhase 2: Draw cell at or after LeftColIndex and above top row. Clipping will happen if grid.hScrollPixelDelta != 0 (left cells only partially visible).
                //// clipPhase 3: Draw cell at or after LeftColIndex and below top row. Clipping will happen if grid.hScrollPixelDelta != 0 or grid.vScrollPixelDelta != 0 (left cells only partially visible).

                int[] rtlPhases = new int[] { 2, 3, 0, 1 };
                for (int clipPhase1 = 0; clipPhase1 < clipPhaseCount; clipPhase1++)
                {
                    int clipPhase = clipPhase1; ////this.isRightToLeft() ? rtlPhases[clipPhase1] : clipPhase1;
                    int left = 0;
                    int right = ds.nCols - 1;
                    int top = 0;
                    int bottom = ds.nRows - 1;

                    Region clipRegion = null;
                    switch (clipPhase)
                    {
                        case 0:
                            if (clipPhaseRight != -1)
                            {
                                right = clipPhaseRight;
                            }

                            if (clipPhaseBottom != -1)
                            {
                                bottom = clipPhaseBottom;
                            }

                            break;

                        case 1:
                            if (clipPhaseBottom == -1)
                            {
                                continue;
                            }

                            top = clipPhaseBottom + 1;
                            if (clipPhaseRight != -1)
                            {
                                right = clipPhaseRight;
                            }

                            clipRegion = ds.graphics.Clip;
                            ds.graphics.IntersectClip(ds.vscrollRect);
                            break;

                        case 2:
                            if (clipPhaseRight == -1)
                            {
                                continue;
                            }

                            left = clipPhaseRight + 1;
                            if (clipPhaseBottom != -1)
                            {
                                bottom = clipPhaseBottom;
                            }

                            clipRegion = ds.graphics.Clip;
                            ds.graphics.IntersectClip(ds.hscrollRect);
                            break;

                        case 3:
                            if (clipPhaseBottom == -1)
                            {
                                continue;
                            }

                            if (clipPhaseRight == -1)
                            {
                                continue;
                            }

                            left = clipPhaseRight + 1;
                            top = clipPhaseBottom + 1;
                            clipRegion = ds.graphics.Clip;
                            ds.graphics.IntersectClip(ds.scrollRect);
                            break;
                    }

                    for (rowIndex = top; rowIndex <= bottom; rowIndex++)
                    {
                        //// Erase the line.
                        InternalDrawBackground(ds, rowIndex, top, left, bottom, right);

                        //// draw all columns in row

                        //// Spanned cells.
                        for (colIndex = left; colIndex <= right; colIndex++)
                        {
                            ds.rectItem = GetCellRectangle(
                                ds.anXOffset[colIndex],
                                ds.anYOffset[rowIndex],
                                ds.anWidths[colIndex],
                                ds.anHeights[rowIndex]);

                            InternalDrawCell(ds, rowIndex, colIndex, true);
                        }

                        //// Regular cells.
                        for (colIndex = left; colIndex <= right; colIndex++)
                        {
                            ds.rectItem = GetCellRectangle(
                                ds.anXOffset[colIndex],
                                ds.anYOffset[rowIndex],
                                ds.anWidths[colIndex],
                                ds.anHeights[rowIndex]);

                            InternalDrawCell(ds, rowIndex, colIndex, false);
                        }

                        //// Should take care on clipping and seting the brush origin here.
                        //// Draw top and bottom borders (or horizontal grid lines).
                        InternalDrawHorzBorders(ds, rowIndex, left, right);

                        //// Current cell.
                        if (ds.pCurrentControl != null && !ds.rectEdit.IsEmpty && !bCCellInvert)
                        {
                            if (ds.pCurrentControl.Initalized)
                            {
                                ds.pCurrentControl.RaiseOutlineCurrentCell(ds.graphics, ds.rectEdit);
                            }

                            bCCellInvert = true;
                        }
                    }

                    //// Draw Vertical Borders (and grid lines).
                    for (colIndex = left; colIndex <= right; colIndex++)
                    {
                        InternalDrawVertBorders(ds, colIndex, top, bottom);
                    }

                    if (!m_grid.PrintingMode)
                    {
                        //// Invert cells.
                        if (ds.bAnyCellSelected && !m_grid.IsPrinting())
                        {
                            InternalInvertCells(ds, left, right, top, bottom);
                        }
                    }

                    if (clipRegion != null)
                    {
                        ds.graphics.Clip = clipRegion;
                    }
                }

                ////if (clipPhase == clipPhaseCount-1)
                {
                    Color rgb = Color.White;

                    if (!m_grid.PrintingMode && !m_grid.Model.Options.TransparentBackground)
                    {
                        rgb = m_grid.GetBackgroundColor();
                        rgb = m_grid.GetBackColor(rgb);
                        ////                }
                        ////                else
                        ////                    rgb = Color.White;
                        ////
                        if (m_grid.ViewLayout.VisibleCols > 0 && m_grid.ViewLayout.VisibleRows > 0)
                        {
                            //// Erase unused Background to the right ...
                            int colCount = m_grid.Model.ColCount;
                            if (!m_grid.ViewLayout.HasPartialVisibleCols && m_grid.ScrollGrid.ColIndexToScrollPosition(m_grid.GetCol(ds.RightCol)) == m_grid.ScrollGrid.ColIndexToScrollPosition(colCount))
                            {
                                Rectangle rect;
                                if (this.isRightToLeft())
                                {
                                    Point p = m_grid.ViewLayout.ClientRowColToPoint(0, ds.RightCol + 1, GridCellSizeKind.VisibleSize);
                                    rect = Rectangle.FromLTRB(m_grid.GridBounds.X, m_grid.GridBounds.Y, p.X, m_grid.GridBounds.Bottom);
                                }
                                else
                                {
                                    rect = new Rectangle(m_grid.ViewLayout.ClientRowColToPoint(0, ds.RightCol + 1, GridCellSizeKind.VisibleSize), m_grid.GridBounds.Size);
                                }

                                rect.Intersect(rectClip);

                                if (rect.Width > 0)
                                {
                                    GridFillRectangleHookEventArgs e = new GridFillRectangleHookEventArgs(g, rect, new BrushInfo(rgb));
                                    m_grid.RaiseFillRectangleHook(e);
                                    if (!e.Cancel)
                                    {
                                        BrushPaint.FillRectangle(g, rect, rgb);
                                    }
                                }

                                if (rect.Left > rectClip.Left)
                                {
                                    rectClip.Width = rect.Left - rectClip.Left;
                                }
                            }

                            //// ... and to the bottom.
                            int rowCount = m_grid.Model.RowCount;
                            if (!m_grid.ViewLayout.HasPartialVisibleRows && m_grid.ScrollGrid.RowIndexToScrollPosition(m_grid.GetRow(ds.BottomRow)) == m_grid.ScrollGrid.RowIndexToScrollPosition(rowCount))
                            {
                                Point p = m_grid.ViewLayout.ClientRowColToPoint(ds.BottomRow + 1, 0, GridCellSizeKind.VisibleSize);
                                Rectangle rect = new Rectangle(new Point(m_grid.GridBounds.X, p.Y), m_grid.GridBounds.Size);
                                rect.Intersect(rectClip);
                                if (rect.Height > 0)
                                {
                                    GridFillRectangleHookEventArgs e = new GridFillRectangleHookEventArgs(g, rect, new BrushInfo(rgb));
                                    m_grid.RaiseFillRectangleHook(e);
                                    if (!e.Cancel)
                                    {
                                        BrushPaint.FillRectangle(g, rect, rgb);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Rectangle rect = m_grid.GridBounds;
                            rect.Intersect(rectClip);
                            if (rect.Height > 0)
                            {
                                GridFillRectangleHookEventArgs e = new GridFillRectangleHookEventArgs(g, rect, new BrushInfo(rgb));
                                m_grid.RaiseFillRectangleHook(e);
                                if (!e.Cancel)
                                {
                                    BrushPaint.FillRectangle(g, rect, rgb);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                throw;
            }
            finally
            {
                //// Decrease counter for nested drawing.
                m_nNestedDraw--;
                m_pDrawStruct.Dispose(m_grid);
                m_pDrawStruct = m_pOldDrawStruct;

                ////                ticks1 += Environment.TickCount-startTick;

                if (m_grid.HasGridBounds)
                {
                    g.SetClip(oldClip);
                }

                ////SS            Trace.WriteLine("\nEnd DrawClientRowCol " + m_grid.ToString());
            }
        }

        Rectangle GetCellRectangle(int xOffset, int yOffset, int width, int height)
        {
            if (this.isRightToLeft())
            {
                return new Rectangle(
                      xOffset - width,
                      yOffset,
                      width,
                      height);
            }
            else
            {
                return new Rectangle(
                      xOffset,
                      yOffset,
                      width,
                      height);
            }
        }

        Rectangle GetCellRectangle(Rectangle r)
        {
            return GetCellRectangle(r.Left, r.Top, r.Width, r.Height);
        }

        void InternalLoadCell(DrawStruct ds, int rowIndex, int colIndex)
        {
            GridRangeInfoList pSelList = m_grid.Model.Selections.Ranges;
            int i = (rowIndex * ds.nCols) + colIndex;

            // Grid line settings.
            bool bNewMode = true;
            int nHorzFlag, nVertFlag;

            if (bNewMode)
            {
                nVertFlag = 0x02;
                nHorzFlag = 0x08;
            }
            else
            {
                nVertFlag = 0x01;
                nHorzFlag = 0x04;
            }

            //// If cell is hidden by a big spanned cell, there is no need
            //// to load this cell again.

            if ((ds.abBits[i] & DrawCellFlags.Spanned) == 0)
            {
                bool bSpanned = false;

                bSpanned = LoadSpannedCell(m_grid, ds, rowIndex, colIndex);

                this.LoadBannerCell(m_grid, ds, rowIndex, colIndex);

                if (!bSpanned)
                {
                    // This is a regular cell.
                    if (ds.apStyles[i] == null)
                    {
                        if ((rowIndex <= m_grid.Model.Cols.HeaderCount || colIndex <= m_grid.Model.Rows.HeaderCount) && isThemedHeader)
                            ds.apStyles[i] = this.GetGridPrintStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex],true);
                        else
                            ds.apStyles[i] = m_grid.GetPaintStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], true);
                    }

                    if (ds.apInterior[i] == null)
                    {
                        ds.apInterior[i] = ds.apStyles[i].Interior;
                    }

                    ds.apBorders[i] = ds.apStyles[i].ReadOnlyBorders;
                    if (ds.anHeights[rowIndex] > 0 && ds.anWidths[colIndex] > 0)
                    {
                        if ((ds.abBits[i] & DrawCellFlags.Bannered) != 0 || ds.standardStyle.Interior != ds.apInterior[i])
                        {
                            ds.abBits[i] |= DrawCellFlags.Color;
                        }

                        GridBordersInfo borders = ds.apBorders[i];
                        // Left
                        if (borders.Left.Style != GridBorderStyle.Standard)
                        {
                            ds.abBits[i] |= 0x01;
                        }

                        // Right
                        if (borders.Right.Style != GridBorderStyle.Standard)
                        {
                            ds.abBits[i] |= 0x02;
                        }

                        //// Top
                        if (borders.Top.Style != GridBorderStyle.Standard)
                        {
                            ds.abBits[i] |= 0x04;
                        }

                        //// Bottom
                        if (borders.Bottom.Style != GridBorderStyle.Standard)
                        {
                            ds.abBits[i] |= 0x08;
                        }

                        //// Specifies if grid line shall be drawn for that cell.

                        if ((ds.abBits[i] & nVertFlag) == 0)
                        {
                            ds.abBits[i] |= DrawCellFlags.VertLine;

                            //// Fixed column.
                            if (ds.anCols[colIndex] > ds.gridHeaderCols && ds.anCols[colIndex] == ds.gridFrozenCols)
                            {
                                ds.abBits[i] |= DrawCellFlags.FixedVertLine;
                            }
                        }

                        if ((ds.abBits[i] & nHorzFlag) == 0)
                        {
                            ds.abBits[i] |= DrawCellFlags.HorzLine;

                            // Fixed row.
                            if (ds.anRows[rowIndex] > ds.gridHeaderRows && ds.anRows[rowIndex] == ds.gridFrozenRows)
                            {
                                ds.abBits[i] |= DrawCellFlags.FixedHorzLine;
                            }
                        }
                    }
                }
            }

            // Invert cell?
            if (ds.bAnyCellSelected
                && pSelList.AnyRangeContains(GridRangeInfo.Cell(ds.anRows[rowIndex], ds.anCols[colIndex])))
            {
                ds.abBits[i] |= DrawCellFlags.Selected;
            }
        }
        internal GridStyleInfo GetGridPrintStyleInfo(int row, int col,bool forceQueryCellInfo)
        {
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;
            GridVisualStyles visualStyles = m_grid.Model.Options.GridVisualStyles;
            GridStyleInfo printStyle = m_grid.GetViewStyleInfo(row, col, forceQueryCellInfo);
            bool isGroupCaption = printStyle.CellIdentity.Info.StartsWith("GroupCaptionCell");
            if (m_grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((printStyle.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && m_grid.ThemesEnabled)
                      || ((printStyle.Themed && m_grid.ThemesEnabled && ((m_grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))))))
            {
                printStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.ExtraThin);
                printStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, clrRight, GridBorderWeight.ExtraThin);
            }
            if (m_grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((printStyle.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && m_grid.ThemesEnabled)
                      || ((printStyle.Themed && m_grid.ThemesEnabled && ((m_grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
                      && (isGroupCaption))
            {
                printStyle.Borders.Top = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.ExtraThin);
            } 
            switch (visualStyles)
            {
                case GridVisualStyles.Metro:
                    printStyle.TextColor = Color.FromArgb(91, 91, 91);
                    printStyle.Interior = new BrushInfo(Color.White);
                    break;
                case GridVisualStyles.Office2007Blue:
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(249, 252, 255), Color.FromArgb(197, 222, 255));
                    break;
                case GridVisualStyles.Office2007Black:
                    printStyle.TextColor = SystemColors.InactiveCaptionText;
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(248, 248, 248), Color.FromArgb(223, 223, 223));
                    break;
                case GridVisualStyles.Office2007Silver:
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(241, 243, 243), Color.FromArgb(200, 201, 202));
                    break;
                case GridVisualStyles.Office2010Blue:
                    printStyle.Interior= new BrushInfo(GradientStyle.Vertical, Color.FromArgb(241, 245, 249), Color.FromArgb(218, 231, 245));                    
                    break;
                case GridVisualStyles.Office2010Black:
                    printStyle.TextColor = Color.White;
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(106, 106, 106), Color.FromArgb(89, 89, 89));
                    break;
                case GridVisualStyles.Office2010Silver:
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(223, 227, 232), Color.FromArgb(183, 188, 193));
                    break;
                default:
                    printStyle.TextColor = Color.Black;
                    printStyle.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(249, 252, 255), Color.FromArgb(197, 222, 255));
                    break;
            }
            return printStyle;
        }
        void DrawBanneredCells(DrawStruct ds, int rowIndex, int colIndex, int i, BrushInfo brInterior, ref int nxWidth, ref int nyHeight)
        {
            int toColIndex = colIndex + 1;
            int i2 = (rowIndex * ds.nCols) + toColIndex;

            // Width (No. of cells in current row with same banner)
            while (toColIndex < ds.nCols
                && (ds.abBits[i2] & DrawCellFlags.BanneredHCont) != 0)
            {
                ////&& (ds.abBits[i2] & DrawCellFlags.Color) != 0     

                ds.abBits[i2] &= ~DrawCellFlags.Color;  //// mark cell with "background is drawn"
                nxWidth += ds.anWidths[toColIndex];
                i2++;
                toColIndex++;
            }

            toColIndex--;

            //// Check also rows.
            int toRowIndex = rowIndex + 1;
            bool equals = true;

            while (equals && nxWidth > 0 && toRowIndex < ds.nRows)
            {
                i2 = (toRowIndex * ds.nCols) + colIndex;
                for (int colIndex2 = colIndex; equals && colIndex2 <= toColIndex; colIndex2++)
                {
                    ////&& (ds.abBits[i2] & DrawCellFlags.Color) > 0
                    equals &= (ds.abBits[i2] & DrawCellFlags.BanneredVCont) != 0;
                    i2++;
                }

                if (equals)
                {
                    //// Mark cell with "background is drawn".
                    i2 = (toRowIndex * ds.nCols) + colIndex;
                    for (int colIndex2 = colIndex; equals && colIndex2 <= toColIndex; colIndex2++)
                    {
                        ds.abBits[i2] &= ~DrawCellFlags.Color;
                        i2++;
                    }

                    nyHeight += ds.anHeights[toRowIndex];
                    i2++;
                    toRowIndex++;
                }
            }

            toRowIndex--;

            if (nxWidth > 0 && nyHeight > 0)
            {
                ds.abBits[i] &= ~DrawCellFlags.Color;

                Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, nxWidth, nyHeight);
#if DEBUG

                if (Switches.GridPaint.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, ds.anRows[rowIndex], ds.anCols[colIndex], ds.anRows[toRowIndex], ds.anCols[toColIndex], r);
                }
#else

                ;
#endif
                GridRangeInfo range = m_grid.Model.BanneredRanges.FindRange(ds.anRows[rowIndex], ds.anCols[colIndex]);
                GridStyleInfo style = m_grid.GetPaintStyleInfo(range.Top, range.Left, true);

                //// Calculate actual bounds of cell (not the only visible portion ...).
                Point pt1 = m_grid.ViewLayout.RowColToPoint(range.Top, range.Left, false, GridCellSizeKind.ActualSize);
                int width = m_grid.ViewLayout.GetColRangeWidth(range.Left, range.Right, GridCellSizeKind.ActualSize);
                int height = m_grid.ViewLayout.GetRowRangeHeight(range.Top, range.Bottom, GridCellSizeKind.ActualSize);
                Rectangle fullRect = GetCellRectangle(pt1.X, pt1.Y, width, height);

                bool needsClip = fullRect != r;
                Region clipRegion = null;

                if (needsClip)
                {
                    clipRegion = ds.graphics.Clip;
                    ds.graphics.IntersectClip(r);
                }

                GridDrawCellBackgroundEventArgs e = new GridDrawCellBackgroundEventArgs(ds.graphics, range, true, ds.bColor, style, fullRect, r, needsClip);
                m_grid.RaiseDrawCellBackground(e);
                if (!e.Cancel)
                {
                    GridCellRendererBase renderer = m_grid.CellRenderers[style.CellType];
                    renderer.RaiseDrawCellBackground(e);
                }

                if (needsClip)
                {
                    ds.graphics.Clip = clipRegion;
                }

                m_grid.DisposePaintStyle(style);
            }
        }

        void InternalDrawBackground(DrawStruct ds, int rowIndex, int top, int left, int bottom, int right)
        {
            Color rgbWhite = Color.White;

            int colIndex;

            if (!m_grid.Model.Options.TransparentBackground && m_grid.OptimizeDrawBackground)
            {
                Rectangle rectClip = ds.rectClip;

                //// Draw Rectangles.
                ds.rectItem.Y = ds.anYOffset[rowIndex];
                ds.rectItem.Height = ds.anHeights[rowIndex];

                for (colIndex = left; colIndex <= right; colIndex++)
                {
                    int i;
                    ds.rectItem.X = ds.anXOffset[colIndex]; ////nxMin
                    ds.rectItem.Width = ds.anWidths[colIndex];

                    //// Skip spanned cells and cells where I already
                    //// drew the background.

                    //// if spanned cell is bannered we draw background here ...
                    if (((ds.abBits[i = (rowIndex * ds.nCols) + colIndex] & DrawCellFlags.Spanned) == 0
                        || (ds.abBits[i] & DrawCellFlags.Bannered) != 0)
                        && (ds.abBits[i] & DrawCellFlags.Color) > 0
                        && (ds.abBits[i] & DrawCellFlags.BanneredHCont) == 0)
                    {
                        if (ds.apStyles[i] == null)
                        {
                            ds.rectItem.X = ds.anXOffset[colIndex]; ////ds.rectItem.Right
                            ds.rectItem.Width = 0;
                            continue;
                        }

                        int nyHeight = ds.anHeights[rowIndex],
                            nxWidth = ds.anWidths[colIndex];

                        BrushInfo brInterior = ds.apInterior[i];

                        //// Has bannered flags, but not BanneredCont flag ...
                        if ((ds.abBits[i] & DrawCellFlags.Bannered) != 0)
                        {
                            DrawBanneredCells(ds, rowIndex, colIndex, i, brInterior, ref nxWidth, ref nyHeight);
                        }
                        else if (brInterior.Style != BrushStyle.Gradient
                            && brInterior.Style != BrushStyle.None
                            && ds.apStyles[i].BackgroundImage == null)
                        {
                            int toColIndex = colIndex + 1;
                            int i2 = (rowIndex * ds.nCols) + toColIndex;

                            // Width (No. of cells in current row with same color).
                            while (toColIndex <= right
                                && (ds.abBits[i2] & DrawCellFlags.Spanned) == 0
                                && (ds.abBits[i2] & DrawCellFlags.Bannered) == 0
                                && (ds.abBits[i2] & DrawCellFlags.Color) != 0
                                && ds.apInterior[i2].Equals(brInterior))
                            {
                                ds.abBits[i2] &= ~DrawCellFlags.Color;  // mark cell with "background is drawn"
                                nxWidth += ds.anWidths[toColIndex];
                                i2++;
                                toColIndex++;
                            }

                            toColIndex--;

                            // Check also rows.
                            int toRowIndex = rowIndex + 1;
                            bool equals = true;

                            while (equals && nxWidth > 0 && toRowIndex <= bottom)
                            {
                                i2 = (toRowIndex * ds.nCols) + colIndex;
                                for (int colIndex2 = colIndex; equals && colIndex2 <= toColIndex; colIndex2++)
                                {
                                    equals &= (
                                        ds.abBits[i2] & DrawCellFlags.Spanned) == 0
                                        && (ds.abBits[i2] & DrawCellFlags.Bannered) == 0
                                        && (ds.abBits[i2] & DrawCellFlags.Color) > 0
                                        && ds.apInterior[i2] != null && ds.apInterior[i2].Equals(brInterior);
                                    i2++;
                                }

                                if (equals)
                                {
                                    // Mark cell with "background is drawn".
                                    i2 = (toRowIndex * ds.nCols) + colIndex;
                                    for (int colIndex2 = colIndex; equals && colIndex2 <= toColIndex; colIndex2++)
                                    {
                                        ds.abBits[i2] &= ~DrawCellFlags.Color;
                                        i2++;
                                    }

                                    nyHeight += ds.anHeights[toRowIndex];
                                    i2++;
                                    toRowIndex++;
                                }
                            }

                            toRowIndex--;

                            //// Draw cell background in advance only if
                            //// we can draw more than one cell in advance;
                            //// otherwise the drawing of the background
                            //// until the GridCellRendererBase itself calls DrawBackground
                            //// in its Draw method --- this gives less flickering.

                            if (nxWidth > 0 && nyHeight > 0 && (toColIndex > colIndex || toRowIndex > rowIndex))
                            {
                                ds.abBits[i] &= ~DrawCellFlags.Color;

                                Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, nxWidth, nyHeight);
#if DEBUG

                                if (Switches.GridPaint.TraceVerbose)
                                {
                                    TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, ds.anRows[rowIndex], ds.anCols[colIndex], ds.anRows[toRowIndex], ds.anCols[toColIndex], r);
                                }
#else

                                ;
#endif
                                GridFillRectangleHookEventArgs e = new GridFillRectangleHookEventArgs(ds.graphics, r, brInterior);
                                m_grid.RaiseFillRectangleHook(e);
                                if (!e.Cancel)
                                {
                                    if (ds.bColor)
                                    {
                                        ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridPaint.TraceVerbose, m_grid.PaneDesc, r, m_grid.GetInterior(brInterior));
                                        BrushPaint.FillRectangle(ds.graphics, r, m_grid.GetInterior(brInterior));
                                    }
                                    else
                                    {
                                        BrushPaint.FillRectangle(ds.graphics, r, brInterior.MakeBlackAndWhite());
                                    }
                                }
                            }
                        }
                    }

                    if (this.isRightToLeft())
                    {
                        ds.rectItem.X = ds.rectItem.X - ds.rectItem.Width;
                    }
                    else
                    {
                        ds.rectItem.X = ds.rectItem.Right;
                    }

                    ds.rectItem.Width = 0;
                }
            }
        }

        void InternalDrawCell(DrawStruct ds, int rowIndex, int colIndex, bool spannedOnly)
        {
            int i = (rowIndex * ds.nCols) + colIndex;

            //// If this is a cell which is hidden by a merge or floating cell
            //// and if it is the current cell, we have to outline the frame
            //// around the cell.

            int flags = GridPaint.DrawCellFlags.SpannedCont | GridPaint.DrawCellFlags.Spanned;
            //// Special drawing for spanned cell.
            bool isSpanned = ds.apStyles[i] != null && (ds.abBits[i] & flags) != 0;

            if (isSpanned == spannedOnly)
            {
                ////Trace.WriteLine(String.Format("DrawCell({0}, {1}) ", ds.anRows[rowIndex], ds.anCols[colIndex]));

                bool bSpanned = DrawSpannedCell(m_grid, ds, rowIndex, colIndex);

                //// Easier drawing for regular cell.
                if (!bSpanned && ds.apStyles[i] != null)
                {
                    Rectangle cellRect = ds.rectItem; ////GetCellRectangle(ds.rectItem.X, ds.rectItem.Y, ds.rectItem.Width, ds.rectItem.Height);

                    if (ds.rectItem.Width > 0 && ds.rectItem.Height > 0)
                    {
                        if ((ds.abBits[i] & DrawCellFlags.Color) > 0)
                        {
                            //// Force erasing the background when cell is not marked
                            //// with "background is drawn" - see InternalDrawBackground before.
                            m_grid.m_bForceDrawBackground = true;
                        }

                        if ((ds.abBits[i] & DrawCellFlags.Bannered) > 0)
                        {
                            m_grid.m_bDrawBannerCell = true;
                        }

                        m_grid.OnDrawItem(ds.graphics, ds.anRows[rowIndex], ds.anCols[colIndex], cellRect, ds.apStyles[i]);

                        m_grid.m_bForceDrawBackground = false;
                        m_grid.m_bDrawBannerCell = false;
                    }

                    //// Borders will be drawn later.

                    if (m_grid.CurrentCell.HasCurrentCellAt(ds.anRows[rowIndex], ds.anCols[colIndex]))
                    {
                        ds.pCurrentControl = m_grid.CellRenderers[ds.apStyles[i].CellType];
                        ds.rectEdit = m_grid.Model.SubtractBorders(cellRect, ds.apStyles[i], this.isRightToLeft());
                    }
                }
                ////                else if (ds.pCurrentControl == null && m_grid.CurrentCell.HasCurrentCellAt(ds.anRows[rowIndex], ds.anCols[colIndex]))
                ////                {
                ////                    ds.pCurrentControl = m_grid.CurrentCell.Renderer;
                ////                    ds.rectEdit = m_grid.Model.SubtractBorders(ds.rectItem, m_grid.GetViewStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], false));
                ////                }
            }
        }

        GridBorder m_tempPen = new GridBorder();

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        void InternalDrawVertBorders(DrawStruct ds, int colIndex, int top, int bottom)
        {
#if DEBUG
            if (Switches.GridBorderPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, colIndex, m_grid.PrintingMode);
            }
#else
            ;
#endif
            //// Grid line settings.
            bool bNewMode = true;
            GridBorderSide tVertLine;
            bool bDispVertLine = m_grid.PrintingMode ? m_grid.Model.Properties.PrintVertLines : m_grid.Model.Properties.DisplayVertLines;
            bool bFixedCol = false;
            GridBorderSide borderSide;

            if (bDispVertLine)
            {
                m_tempPen = m_grid.Model.GetGridLineBorder();

                bFixedCol = m_grid.Model.Options.HighlightFrozenLine && !m_grid.PrintingMode && ds.anCols[colIndex] > ds.gridHeaderCols && ds.anCols[colIndex] == ds.gridFrozenCols;
            }
            else
            {
                m_tempPen = GridBorder.Empty;
            }
            //// && !m_grid.IsRightToLeft())

            if (bNewMode)
            {
                tVertLine = GridBorderSide.Right;
            }
            else
            {
                tVertLine = GridBorderSide.Left;
            }

            // Draw left and right borders (or vertical grid lines).
            for (int nFlag = 1; nFlag <= 2; nFlag *= 2)
            {
                GridBorderSide tBorder;
                if (nFlag == 1)
                {
                    tBorder = GridBorderSide.Left;
                    borderSide = GridBorderSide.Left;
                }
                else
                {
                    tBorder = GridBorderSide.Right;
                    borderSide = GridBorderSide.Right;
                }

                for (int rowIndex = top; rowIndex <= bottom; rowIndex++)
                {
                    int i = (rowIndex * ds.nCols) + colIndex;

                    bool bBorder = ds.apStyles[i] != null && (ds.abBits[i] & nFlag) > 0;
                    bool bGridLine = !bBorder
                        && bDispVertLine
                        && ((bFixedCol && tBorder == GridBorderSide.Right && (ds.abBits[i] & DrawCellFlags.FixedVertLine) > 0)
                        || (tBorder == tVertLine && (ds.abBits[i] & DrawCellFlags.VertLine) > 0));

                    if (bBorder || bGridLine)
                    {
                        ds.rectItem = new Rectangle(
                            ds.anXOffset[colIndex],
                            ds.anYOffset[rowIndex],
                            ds.anWidths[colIndex],
                            ds.anHeights[rowIndex]);

                        Rectangle cellRect = GetCellRectangle(ds.rectItem.X, ds.rectItem.Y, ds.rectItem.Width, ds.rectItem.Height);

                        int nyHeight = ds.anHeights[rowIndex];

                        int toRowIndex = rowIndex + 1,
                            i2;

                        GridBorder bl = (tBorder == GridBorderSide.Right) ? ds.apBorders[i].Right : ds.apBorders[i].Left;
                        if (bBorder && bl.Style == GridBorderStyle.None)
                        {
                            continue;
                        }

                        //
                        GridBorder pPen;
                        GridBorder pPen2;

                        if (bBorder && ds.bColor)
                        {
                            pPen = bl;
                        }
                        else if (bBorder)
                        {
                            pPen = bl.MakeBlackAndWhite();
                        }
                        else
                        {
                            pPen = m_tempPen;
                        }

                        Color backColor = Color.White;
                        if (ds.bColor)
                        {
                            backColor = ds.apInterior[i].BackColor;
                        }

                        bool bNoSolid = pPen.Style != GridBorderStyle.Solid;

                        if (bNoSolid)
                        {
                            // Height (No. of cells in current column with same pen).
                            while (toRowIndex <= bottom)
                            {
                                i2 = (toRowIndex * ds.nCols) + colIndex;
                                bool bBorder2 = ds.apStyles[i2] != null && (ds.abBits[i2] & nFlag) > 0;
                                bool bGridLine2 = !bBorder
                                    && bDispVertLine
                                    && ((bFixedCol && tBorder == GridBorderSide.Right && (ds.abBits[i2] & DrawCellFlags.FixedVertLine) > 0)
                                    || (tBorder == tVertLine && (ds.abBits[i2] & DrawCellFlags.VertLine) > 0));

                                if (!bBorder2 && !bGridLine2)
                                {
                                    break;
                                }

                                if (bBorder2)
                                {
                                    pPen2 = (tBorder == GridBorderSide.Right) ? ds.apBorders[i2].Right : ds.apBorders[i2].Left;
                                }
                                else
                                {
                                    pPen2 = m_tempPen;
                                }

                                Color backColor2 = Color.White;
                                if (ds.bColor)
                                {
                                    backColor2 = ds.apInterior[i2].BackColor;
                                }

                                if (!pPen2.Equals(pPen) || backColor2 != backColor)
                                {
                                    break;
                                }

                                // same pen, same brush
                                if (bBorder2)
                                {
                                    ds.abBits[i2] &= ~nFlag;
                                }

                                if (!bBorder2 || nFlag == (int)tVertLine)
                                {
                                    ds.abBits[i2] &= ~(DrawCellFlags.VertLine | DrawCellFlags.FixedVertLine);
                                }

                                nyHeight += ds.anHeights[toRowIndex];
                                toRowIndex++;
                            }
                        }
                        else
                        {
                            // Height (No. of cells in current column with same pen).
                            while (toRowIndex <= bottom)
                            {
                                i2 = (toRowIndex * ds.nCols) + colIndex;
                                bool bBorder2 = ds.apStyles[i2] != null && (ds.abBits[i2] & nFlag) > 0;
                                bool bGridLine2 = !bBorder
                                    && bDispVertLine
                                    && ((bFixedCol && tBorder == GridBorderSide.Right && (ds.abBits[i2] & DrawCellFlags.FixedVertLine) > 0)
                                    || (tBorder == tVertLine && (ds.abBits[i2] & DrawCellFlags.VertLine) > 0));

                                if (!bBorder2 && !bGridLine2)
                                {
                                    break;
                                }

                                if (bBorder2)
                                {
                                    pPen2 = (tBorder == GridBorderSide.Right) ? ds.apBorders[i2].Right : ds.apBorders[i2].Left;
                                }
                                else
                                {
                                    pPen2 = m_tempPen;
                                }

                                if (!pPen2.Equals(pPen))
                                {
                                    break;
                                }

                                // Same pen, (solid - no need to take care on brush).
                                if (bBorder2)
                                {
                                    ds.abBits[i2] &= ~nFlag;
                                }

                                if (!bBorder2 || nFlag == (int)tVertLine)
                                {
                                    ds.abBits[i2] &= ~(DrawCellFlags.VertLine | DrawCellFlags.FixedVertLine);
                                }

                                nyHeight += ds.anHeights[toRowIndex];
                                toRowIndex++;
                            }
                        }

                        // Border must always be drawn with DrawBorder.
                        // Grid lines will be drawn with DrawBorder when not solid.
                        Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, ds.rectItem.Width, nyHeight);

                        if (this.isRightToLeft())
                        {
                            if (tBorder == GridBorderSide.Left)
                            {
                                borderSide = GridBorderSide.Right;
                            }
                            else
                            {
                                borderSide = GridBorderSide.Left;
                            }
                        }

                        GridBorderPaint.DrawRectangle(ds.graphics, pPen, r, backColor, borderSide, m_grid.PrintingMode);
                    }
                }

                // Draw right border with fixed line color if this is frozen column.
                if (bFixedCol && bDispVertLine)
                {
                    m_tempPen = m_grid.Model.GetFixedLineBorder();
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        void InternalDrawHorzBorders(DrawStruct ds, int rowIndex, int leftCol, int rightCol)
        {
#if DEBUG
            if (Switches.GridBorderPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, rowIndex, m_grid.PrintingMode);
            }
#else
            ;
#endif
            //// Grid line settings.
            bool bNewMode = true; ////Param.GetNewGridLineMode();
            GridBorderSide tHorzLine;
            bool bDispHorzLine = m_grid.PrintingMode ? m_grid.Model.Properties.PrintHorzLines : m_grid.Model.Properties.DisplayHorzLines;
            GridBorderSide borderSide;

            bool bFixedRow = false;

            if (bDispHorzLine)
            {
                m_tempPen = m_grid.Model.GetGridLineBorder();

                bFixedRow = m_grid.Model.Options.HighlightFrozenLine && !m_grid.PrintingMode && ds.anRows[rowIndex] > ds.gridHeaderRows && ds.anRows[rowIndex] == ds.gridFrozenRows;
            }
            else
            {
                m_tempPen = GridBorder.Empty;
            }

            if (bNewMode)
            {
                tHorzLine = GridBorderSide.Bottom;
            }
            else
            {
                tHorzLine = GridBorderSide.Top;
            }

            // Draw top and bottom borders (or horizontal grid lines).
            for (int nFlag = 4; nFlag <= 8; nFlag *= 2)
            {
                GridBorderSide tBorder;
                if (nFlag == 4)
                {
                    tBorder = GridBorderSide.Top;
                    borderSide = GridBorderSide.Top;
                }
                else
                {
                    tBorder = GridBorderSide.Bottom;
                    borderSide = GridBorderSide.Bottom;
                }

                for (int colIndex = leftCol; colIndex <= rightCol; colIndex++)
                {
                    ds.rectItem = new Rectangle(
                        ds.anXOffset[colIndex],
                        ds.anYOffset[rowIndex],
                        ds.anWidths[colIndex],
                        ds.anHeights[rowIndex]);

                    int i = (rowIndex * ds.nCols) + colIndex;

                    bool bBorder = ds.apStyles[i] != null && (ds.abBits[i] & nFlag) > 0;
                    bool bGridLine = !bBorder
                        && bDispHorzLine
                        && ((bFixedRow && tBorder == GridBorderSide.Bottom && (ds.abBits[i] & DrawCellFlags.FixedHorzLine) > 0)
                        || (tBorder == tHorzLine && (ds.abBits[i] & DrawCellFlags.HorzLine) > 0));

                    if (bBorder || bGridLine)
                    {
                        int nxWidth = ds.anWidths[colIndex];

                        int toColIndex = colIndex + 1,
                            i2;

                        GridBorder bl = (tBorder == GridBorderSide.Bottom) ? ds.apBorders[i].Bottom : ds.apBorders[i].Top;
                        if (bBorder && bl.Style == GridBorderStyle.None)
                        {
                            continue;
                        }

                        GridBorder pPen;
                        GridBorder pPen2;

                        if (bBorder && ds.bColor)
                        {
                            pPen = bl;
                        }
                        else if (bBorder)
                        {
                            pPen = bl.MakeBlackAndWhite();
                        }
                        else
                        {
                            pPen = m_tempPen;
                        }

                        Color backColor = Color.White;
                        if (ds.bColor)
                        {
                            backColor = ds.apInterior[i].BackColor;
                        }

                        bool bNoSolid = pPen.Style != GridBorderStyle.Solid;

                        if (bNoSolid)
                        {
                            // Width (No. of cells in current row with same pen).
                            while (toColIndex <= rightCol)
                            {
                                i2 = (rowIndex * ds.nCols) + toColIndex;
                                bool bBorder2 = ds.apStyles[i2] != null && (ds.abBits[i2] & nFlag) > 0;
                                bool bGridLine2 = !bBorder
                                    && bDispHorzLine
                                    && ((bFixedRow && tBorder == GridBorderSide.Bottom && (ds.abBits[i2] & DrawCellFlags.FixedHorzLine) > 0)
                                    || (tBorder == tHorzLine && (ds.abBits[i2] & DrawCellFlags.HorzLine) > 0));

                                if (!bBorder2 && !bGridLine2)
                                {
                                    break;
                                }

                                if (bBorder2)
                                {
                                    pPen2 = (tBorder == GridBorderSide.Bottom) ? ds.apBorders[i2].Bottom : ds.apBorders[i2].Top;
                                }
                                else
                                {
                                    pPen2 = m_tempPen;
                                }

                                Color backColor2 = Color.White;
                                if (ds.bColor)
                                {
                                    backColor2 = ds.apInterior[i2].BackColor;
                                }

                                if (!pPen2.Equals(pPen) || backColor2 != backColor)
                                {
                                    break;
                                }

                                // Same pen, same brush.
                                if (bBorder2)
                                {
                                    ds.abBits[i2] &= ~nFlag;
                                }

                                if (!bBorder2 || nFlag == (int)tHorzLine)
                                {
                                    ds.abBits[i2] &= ~(DrawCellFlags.HorzLine | DrawCellFlags.FixedHorzLine);
                                }

                                nxWidth += ds.anWidths[toColIndex];
                                toColIndex++;
                            }
                        }
                        else
                        {
                            // Width (No. of cells in current row with same pen).
                            while (toColIndex <= rightCol)
                            {
                                i2 = (rowIndex * ds.nCols) + toColIndex;
                                bool bBorder2 = ds.apStyles[i2] != null && (ds.abBits[i2] & nFlag) > 0;
                                bool bGridLine2 = !bBorder
                                    && bDispHorzLine
                                    && ((bFixedRow && tBorder == GridBorderSide.Bottom && (ds.abBits[i2] & DrawCellFlags.FixedHorzLine) > 0)
                                    || (tBorder == tHorzLine && (ds.abBits[i2] & DrawCellFlags.HorzLine) > 0));

                                if (!bBorder2 && !bGridLine2)
                                {
                                    break;
                                }

                                if (bBorder2)
                                {
                                    pPen2 = (tBorder == GridBorderSide.Bottom) ? ds.apBorders[i2].Bottom : ds.apBorders[i2].Top;
                                }
                                else
                                {
                                    pPen2 = m_tempPen;
                                }

                                if (!pPen2.Equals(pPen))
                                {
                                    break;
                                }

                                // Same pen, (solid - no need to take care on brush).
                                if (bBorder2)
                                {
                                    ds.abBits[i2] &= ~nFlag;
                                }

                                if (!bBorder2 || nFlag == (int)tHorzLine)
                                {
                                    ds.abBits[i2] &= ~(DrawCellFlags.HorzLine | DrawCellFlags.FixedHorzLine);
                                }

                                nxWidth += ds.anWidths[toColIndex];
                                toColIndex++;
                            }
                        }

                        //// Border must always be drawn with DrawBorder.
                        //// Grid lines will be drawn with DrawBorder when not solid.
                        Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, nxWidth, ds.rectItem.Height);
                        ////Rectangle r = Rectangle.FromLTRB(ds.rectItem.Left, ds.rectItem.Top, ds.rectItem.Left+nxWidth, ds.rectItem.Bottom);
                        ////Trace.WriteLine(String.Format("{0}: Row = {1} {4}, Cols {2}-{3}", r, rowIndex, colIndex, toColIndex, borderSide));
                        GridBorderPaint.DrawRectangle(ds.graphics, pPen, r, backColor, borderSide, m_grid.PrintingMode);
                        
                    }
                }

                //// Draw bottom border with fixed line color if this is frozen row.
                if (bFixedRow && bDispHorzLine)
                {
                    m_tempPen = m_grid.Model.GetFixedLineBorder();
                }
            }
        }

        void InternalInvertCells(DrawStruct ds, int leftCol, int rightCol, int topRow, int bottomRow)
        {
            int rowIndex;

            for (ds.rectItem.Y = ds.nyMin, rowIndex = topRow; rowIndex <= bottomRow; rowIndex++)
            {
                ds.rectItem.Y = ds.anYOffset[rowIndex];
                ds.rectItem.Height = ds.anHeights[rowIndex];

                for (int colIndex = leftCol; colIndex <= rightCol; colIndex++)
                {
                    ds.rectItem.X = ds.anXOffset[colIndex];
                    ds.rectItem.Width = ds.anWidths[colIndex]; ////m_grid.ViewLayout.GetColWidth(ds.anCols[colIndex], GridCellSizeKind.VisibleSize);
                    Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, ds.rectItem.Width, ds.rectItem.Height);

                    if ((ds.abBits[(rowIndex * ds.nCols) + colIndex] & DrawCellFlags.Selected) > 0)
                    {
                        m_grid.IntDrawInvertCell(ds.graphics, ds.anRows[rowIndex], ds.anCols[colIndex], r, true);
                    }
                }
            }
        }

        void InternalInvertCells(DrawStruct ds, int rowIndex)
        {
            ds.rectItem.Y = ds.anYOffset[rowIndex];
            ds.rectItem.Height = ds.anHeights[rowIndex];

            for (int colIndex = 0; colIndex < ds.nCols; colIndex++)
            {
                ds.rectItem.X = ds.anXOffset[colIndex];
                ds.rectItem.Width = ds.anWidths[colIndex];

                if ((ds.abBits[(rowIndex * ds.nCols) + colIndex] & DrawCellFlags.Selected) > 0)
                {
                    int toColIndex = colIndex + 1;
                    while (toColIndex < ds.nCols && (ds.abBits[(rowIndex * ds.nCols) + toColIndex] & DrawCellFlags.Selected) > 0)
                    {
                        ds.rectItem.Width += ds.anWidths[toColIndex];
                        toColIndex++;
                    }

                    Rectangle r = GetCellRectangle(ds.rectItem.Left, ds.rectItem.Top, ds.rectItem.Width, ds.rectItem.Height);
                    m_grid.IntDrawInvertCell(ds.graphics, ds.anRows[rowIndex], ds.anCols[colIndex], r, true);
                    colIndex = toColIndex - 1;
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public void OnDrawBorders(Graphics g, Rectangle rectItem, GridStyleInfo style)
        {
            // Draw Borders.
            Color backColor = m_grid.GetBackColor(style.Interior.BackColor);
            GridBordersInfo borders = style.ReadOnlyBorders;
#if DEBUG
            if (Switches.GridBorderPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, rectItem, borders);
            }
#else
            ;
#endif

            GridBorderPaint.DrawRectangle(g, borders.Top, rectItem, backColor, GridBorderSide.Top, m_grid.PrintingMode);
            GridBorderPaint.DrawRectangle(g, borders.Bottom, rectItem, backColor, GridBorderSide.Bottom, m_grid.PrintingMode);
            if (this.isRightToLeft())
            {
                GridBorderPaint.DrawRectangle(g, borders.Right, rectItem, backColor, GridBorderSide.Left, m_grid.PrintingMode);
                GridBorderPaint.DrawRectangle(g, borders.Left, rectItem, backColor, GridBorderSide.Right, m_grid.PrintingMode);
            }
            else
            {
                GridBorderPaint.DrawRectangle(g, borders.Left, rectItem, backColor, GridBorderSide.Left, m_grid.PrintingMode);
                GridBorderPaint.DrawRectangle(g, borders.Right, rectItem, backColor, GridBorderSide.Right, m_grid.PrintingMode);
            }
        }

        ////        [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
        ////        public void DrawBorder(GridBorder pen, Rectangle rc, GridBorderSide bt, Graphics g, BrushInfo interior)
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.GridBorderPaint.TraceVerbose, m_grid.PaneDesc, pen, rc, bt, interior);
        ////            GridBorderSide borderSide = GridBorderSide.Top;
        ////            switch (bt)
        ////            {
        ////            case GridBorderSide.Top: borderSide = GridBorderSide.Top; break;
        ////            case GridBorderSide.Bottom: borderSide = GridBorderSide.Bottom; break;
        ////            case GridBorderSide.Left: borderSide = GridBorderSide.Left; break;
        ////            case GridBorderSide.Right: borderSide = GridBorderSide.Right; break;
        //// //// TODO: Not sure
        //// //// case GridBorderSide.Left: borderSide = isRightToLeft() ? GridBorderSide.Right : GridBorderSide.Left; break;
        //// ////                case GridBorderSide.Right: borderSide = isRightToLeft() ? GridBorderSide.Left : GridBorderSide.Right; break;
        ////            }
        ////
        ////            Color backColor = m_grid.GetBackColor(interior.BackColor);
        ////            GridBorderPaint.DrawRectangle(g, pen, rc, backColor, borderSide, m_grid.PrintingMode);
        ////        }

        public void DrawBrushRect(Graphics g, Rectangle rect, BrushInfo brush)
        {
            GridFillRectangleHookEventArgs e = new GridFillRectangleHookEventArgs(g, rect, brush);
            m_grid.RaiseFillRectangleHook(e);
            if (!e.Cancel)
            {
                BrushPaint.FillRectangle(g, rect, m_grid.GetInterior(brush));
            }
        }

        public void DrawInvertCell(Graphics g, int rowIndex, int colIndex, Rectangle rectItem, bool inPaint)
        {
            if (!inPaint && m_grid.Updating)
            {
                m_grid.Invalidate(rectItem);
                return;
            }

            //// Invert cell (selected cells).

            //// Don't invert current cell when m_bInvertRect is false. SetCurrentCell
            //// will set m_bInvertRect = false

            if ((m_grid.Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) == 0)
            {
                m_grid.CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                bool bCCell = m_grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

                if (!bCCell || m_grid.m_bInvertRect || m_grid.Model.Options.ShowCurrentCellBorderBehavior == GridShowCurrentCellBorder.HideAlways)
                {
                    InvertRect(g, rectItem);
                }
            }
            else
            {
                if (!inPaint)
                {
                    m_grid.Invalidate(rectItem);
                }
                else
                {
                    DrawStruct ds = m_pDrawStruct;
                    if (ds != null)
                    {
                        g.FillRectangle(ds.selectionBrush, rectItem);
                    }
                }
            }

            m_grid.m_bInvertRect = false;
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public void InvertRect(Graphics g, Rectangle r)
        {
            IntPtr dc = g.GetHdc();
            r.Intersect(this.m_grid.GridBounds);
            NativeMethods.RECT rect = new NativeMethods.RECT(r);
            NativeMethods.InvertRect(dc, ref rect);
            g.ReleaseHdc(dc);
        }

        public static void Draw3dFrame(Graphics g, int x0, int y0, int x1, int y1, int w, Color rgbTopLeft, Color rgbBottomRight)
        {
            Rectangle rc;

            for (int i = 0; i < w; i++)
            {
                // Top
                Brush brTL = new SolidBrush(rgbTopLeft);
                rc = Rectangle.FromLTRB(x0, y0, x1, y0 + 1);
                g.FillRectangle(brTL, rc);

                // Left
                rc = Rectangle.FromLTRB(x0, y0, x0 + 1, y1);
                g.FillRectangle(brTL, rc);
                brTL.Dispose();

                Brush brBR = new SolidBrush(rgbBottomRight);

                // Bottom
                rc = Rectangle.FromLTRB(x0, y1, x1 + 1, y1 + 1);
                g.FillRectangle(brBR, rc);

                // Right
                rc = Rectangle.FromLTRB(x1, y0, x1 + 1, y1);
                g.FillRectangle(brBR, rc);
                brBR.Dispose();

                if (i < w - 1)
                {
                    x0++;
                    y0++;
                    x1--;
                    y1--;
                }
            }
        }

        public bool LoadBannerCell(GridControlBase pGrid, object pds, int rowIndex, int colIndex)
        {
            GridPaint.DrawStruct ds = (GridPaint.DrawStruct)pds;

            int i = (rowIndex * ds.nCols) + colIndex;

            if ((ds.abBits[i] & DrawCellFlags.Bannered) != 0)
            {
                return false;
            }

            GridRangeInfo rgBannered;

            //// Bannered cells.
            bool isBannereded = pGrid.Model.BanneredRanges.Find(ds.anRows[rowIndex], ds.anCols[colIndex], out rgBannered);
            bool isBannered = isBannereded;

            //// If we find a spanned cell, initialize settings
            //// for all those cells which are hidden by this spanned cell.

            //// - For each hidden cell, the style pointer from the spanned
            ////   cell will be copied - each hidden cell will have the same
            ////   style pointer.

            //// - The first cell will be marked with GridPaint.DrawCellFlags.Banner. All
            ////   subsequent hidden cells will be marked with GridPaint.DrawCellFlags.BannerCont.
            ////   GridPaint.DrawCellFlags.BannerCont will be checked in DrawCell - if it is set
            ////   the cell must not be drawn.
            ////

            if (isBannered)
            {
                if (ds.apInterior[i] == null)
                {
                    ////ds.apStyles[i] = m_grid.GetViewStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], true);
                    GridStyleInfo bannerStyle = m_grid.GetPaintStyleInfo(rgBannered.Top, rgBannered.Left, true);
                    if (ds.bColor)
                    {
                        ds.apInterior[i] = bannerStyle.Interior;
                        ////ds.apBorders[i] = ds.apStyles[i].ReadOnlyBorders;
                    }
                    else
                    {
                        ds.apInterior[i] = bannerStyle.Interior.MakeBlackAndWhite();
                        ////ds.apBorders[i] = ds.apStyles[i].ReadOnlyBorders;
                    }

                    m_grid.DisposePaintStyle(bannerStyle);
                }

                //// Block: mark all cells of covered range.
                try
                {
                    int row1, col1, row2, col2;

                    int rgRight = rgBannered.Right;
                    int rgBottom = rgBannered.Bottom;
                    if (rgBottom < ds.gridTopRow && rgBottom > ds.gridFrozenRows)
                    {
                        rgBottom = ds.gridFrozenRows;
                    }

                    if (rgRight < ds.gridLeftCol && rgRight > ds.gridFrozenCols)
                    {
                        rgRight = ds.gridFrozenCols;
                    }

                    row2 = Math.Min(pGrid.GetClientRow(rgBottom) - ds.TopRow, ds.nRows - 1);
                    col2 = Math.Min(pGrid.GetClientCol(rgRight) - ds.LeftCol, ds.nCols - 1);

                    ds.abBits[(rowIndex * ds.nCols) + colIndex] |= GridPaint.DrawCellFlags.Bannered;
                    int n1 = 0;
                    int n2 = 0;
                    for (row1 = rowIndex; row1 <= row2; row1++)
                    {
                        n1 = 0;
                        for (col1 = colIndex; col1 <= col2; col1++)
                        {
                            int i2 = (row1 * ds.nCols) + col1;
                            ds.apInterior[i2] = ds.apInterior[i];
                            if (isBannereded)
                            {
                                ds.abBits[i2] |= (int)(GridPaint.DrawCellFlags.Bannered | n1 * GridPaint.DrawCellFlags.BanneredHCont | n2 * GridPaint.DrawCellFlags.BanneredVCont);
                            }

                            n1 = 1;
                        }

                        n2 = 1;
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    ////                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    ////                        throw ex;
                }

                if (ds.bColor)
                {
                    ds.abBits[i] |= GridPaint.DrawCellFlags.Color;
                }

                return true;
            }

            return false;
        }

        public bool LoadSpannedCell(GridControlBase pGrid, object pds, int rowIndex, int colIndex)
        {
            GridPaint.DrawStruct ds = (GridPaint.DrawStruct)pds;

            int i = (rowIndex * ds.nCols) + colIndex;
            GridRangeInfo rgCovered;

            //// Grid line settings.
            bool bNewMode = true; ////pGrid.Param.GetNewGridLineMode();
            int nHorzFlag, nVertFlag;
            GridBorderSide tVertLine, tHorzLine;

            if (bNewMode)
            {
                nVertFlag = 0x02;
                nHorzFlag = 0x08;
                tVertLine = GridBorderSide.Right;
                tHorzLine = GridBorderSide.Bottom;
            }
            else
            {
                nVertFlag = 0x01;
                nHorzFlag = 0x04;
                tVertLine = GridBorderSide.Left;
                tHorzLine = GridBorderSide.Top;
            }

            // Covered cells.
            bool bCovered = pGrid.Model.CoveredRanges.Find(ds.anRows[rowIndex], ds.anCols[colIndex], out rgCovered);
            bool bSpanned = bCovered;
            bool bMerged = false;

            //
            // Check if cell is a floating or merge cell and make sure it
            // is not an active cell in the floating or merge cell.
            //
            // We have to check for active current cell because with
            // floating or merge cells, the user should still be able
            // to edit the hidden cells.
            //
            if (!bCovered)
            {
                // ds.pCurrentControl has been set in OnDrawClientRowCol
                GridCurrentCell cc = pGrid.CurrentCell;
                bool focused = ds.pCurrentControl != null && cc.HasCurrentCellAt(ds.anRows[rowIndex], ds.anCols[colIndex]) && cc.Renderer.HasFocusControl;

                // Floating cells.
                bSpanned |= pGrid.Model.FloatingCells.Find(ds.anRows[rowIndex], ds.anCols[colIndex], out rgCovered)
                    && (ds.anCols[colIndex] == rgCovered.Left
                    || !focused);

                // Merge Cells.
                if (!bSpanned)
                {
                    bMerged = pGrid.Model.MergeCells.Find(GridMergeCellDirection.Both, ds.anRows[rowIndex], ds.anCols[colIndex], out rgCovered)
                        && ((ds.anCols[colIndex] == rgCovered.Left && ds.anRows[rowIndex] == rgCovered.Top)
                        || !focused);
                    bSpanned |= bMerged;
                }
            }

            //// If we find a spanned cell, initialize settings
            //// for all those cells which are hidden by this spanned cell.

            //// - For each hidden cell, the style pointer from the spanned
            ////   cell will be copied - each hidden cell will have the same
            ////   style pointer.

            //// - The first cell will be marked with GridPaint.DrawCellFlags.Spanned. All
            ////   subsequent hidden cells will be marked with GridPaint.DrawCellFlags.SpannedCont.
            ////   GridPaint.DrawCellFlags.SpannedCont will be checked in DrawCell - if it is set
            ////   the cell must not be drawn.
            ////
            ////   An exception for Merge and Floating cells: If an active current cell
            ////   is in the range of a hidden cell, the cell will be ignored (and
            ////   not marked with GridPaint.DrawCellFlags.SpannedCont flag).
            ////
            //// - The border setting will be initialized for all hidden cells.
            ////   This helps the DrawBorders methods to find out if a grid line
            ////   should be drawn above or below the cell or if a normal
            ////   border shall be drawn.

            if (bSpanned)
            {
                if (ds.apStyles[i] == null)
                {
                    ds.apStyles[i] = m_grid.GetPaintStyleInfo(rgCovered.Top, rgCovered.Left, true);
                    if (ds.bColor)
                    {
                        if (ds.apInterior[i] == null)
                        {
                            ds.apInterior[i] = ds.apStyles[i].Interior;
                        }

                        ds.apBorders[i] = ds.apStyles[i].ReadOnlyBorders;
                    }
                    else
                    {
                        if (ds.apInterior[i] == null)
                        {
                            ds.apInterior[i] = ds.apStyles[i].Interior.MakeBlackAndWhite();
                        }

                        ds.apBorders[i] = ds.apStyles[i].ReadOnlyBorders;
                    }
                }

                // Block: mark all cells of covered range.
                try
                {
                    int row1, col1, row2, col2;

                    int rgRight = rgCovered.Right;
                    int rgBottom = rgCovered.Bottom;
                    if (rgBottom < ds.gridTopRow && rgBottom > ds.gridFrozenRows)
                    {
                        rgBottom = ds.gridFrozenRows;
                    }

                    if (rgRight < ds.gridLeftCol && rgRight > ds.gridFrozenCols)
                    {
                        rgRight = ds.gridFrozenCols;
                    }

                    row2 = Math.Min(pGrid.GetClientRow(rgBottom) - ds.TopRow, ds.nRows - 1);
                    if (row2 < 0)
                    {
                        // TODO: Hack! There is some bug in QueryCoveredRange or in DisplayElements Collections regarding nested tables.
                        pGrid.Model.CoveredRanges.Find(ds.anRows[rowIndex], ds.anCols[colIndex], out rgCovered);
                        rgRight = rgCovered.Right;
                        rgBottom = rgCovered.Bottom;
                        if (rgBottom < ds.gridTopRow && rgBottom > ds.gridFrozenRows)
                        {
                            rgBottom = ds.gridFrozenRows;
                        }

                        if (rgRight < ds.gridLeftCol && rgRight > ds.gridFrozenCols)
                        {
                            rgRight = ds.gridFrozenCols;
                        }

                        row2 = Math.Min(pGrid.GetClientRow(rgBottom) - ds.TopRow, ds.nRows - 1);
                    }

                    col2 = Math.Min(pGrid.GetClientCol(rgRight) - ds.LeftCol, ds.nCols - 1);

                    if (row2 >= rowIndex && col2 >= colIndex)
                    {
                        ds.abBits[(rowIndex * ds.nCols) + colIndex] |= GridPaint.DrawCellFlags.Spanned;
                        int n1 = 0;
                        for (row1 = rowIndex; row1 <= row2; row1++)
                        {
                            for (col1 = colIndex; col1 <= col2; col1++)
                            {
                                // Covered or hidden cell (active cells will
                                // be ignored for floating and merge cell).
                                int i2 = (row1 * ds.nCols) + col1;
                                ds.apStyles[i2] = ds.apStyles[i];
                                if (ds.apInterior[i2] == null)
                                {
                                    ds.apInterior[i2] = ds.apInterior[i];
                                }

                                ds.apBorders[i2] = ds.apBorders[i];
                                if (bCovered)
                                {
                                    ds.abBits[i2] |= (int)(GridPaint.DrawCellFlags.Spanned | GridPaint.DrawCellFlags.Covered | n1 * GridPaint.DrawCellFlags.SpannedCont);
                                }
                                else if (!(pGrid.CurrentCell.HasCurrentCellAt(ds.anRows[row1], ds.anCols[col1])
                                    && pGrid.CurrentCell.Renderer.HasFocusControl))
                                {
                                    ds.abBits[i2] |= (int)(GridPaint.DrawCellFlags.Spanned | n1 * GridPaint.DrawCellFlags.SpannedCont);
                                }

                                if (bMerged)
                                {
                                    ds.abBits[i2] |= (int)GridPaint.DrawCellFlags.Merged;
                                }

                                n1 = 1;
                            }
                        }

                        //// Determine for which cells the grid line
                        //// shall be drawn.

                        if (bNewMode)
                        {
                            if (ds.anCols[col2] == rgCovered.Right)
                            {
                                for (row1 = rowIndex; row1 <= row2; row1++)
                                {
                                    int i2 = (row1 * ds.nCols) + col2;

                                    if (ds.apStyles[i2] != null)
                                    {
                                        // Left (or right)
                                        if (ds.apBorders[i2][tVertLine].Style != GridBorderStyle.Standard)
                                        {
                                            ds.abBits[i2] |= nVertFlag;
                                        }

                                        if ((ds.abBits[i2] & nVertFlag) == 0)
                                        {
                                            ds.abBits[i2] |= GridPaint.DrawCellFlags.VertLine;
                                        }
                                    }
                                }
                            }

                            if (ds.anRows[row2] == rgCovered.Bottom)
                            {
                                for (col1 = colIndex; col1 <= col2; col1++)
                                {
                                    int i2 = (row2 * ds.nCols) + col1;

                                    if (ds.apStyles[i2] != null)
                                    {
                                        // Top
                                        if (ds.apBorders[i2][tHorzLine].Style != GridBorderStyle.Standard)
                                        {
                                            ds.abBits[i2] |= nHorzFlag;
                                        }

                                        if ((ds.abBits[i2] & nHorzFlag) == 0)
                                        {
                                            ds.abBits[i2] |= GridPaint.DrawCellFlags.HorzLine;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ds.anCols[colIndex] == rgCovered.Left)
                            {
                                for (row1 = rowIndex; row1 <= row2; row1++)
                                {
                                    int i2 = (row1 * ds.nCols) + colIndex;

                                    if (ds.apStyles[i2] != null)
                                    {
                                        // Left (or right)
                                        if (ds.apBorders[i2][tVertLine].Style != GridBorderStyle.Standard)
                                        {
                                            ds.abBits[i2] |= nVertFlag;
                                        }

                                        if ((ds.abBits[i2] & nVertFlag) == 0)
                                        {
                                            ds.abBits[i2] |= GridPaint.DrawCellFlags.VertLine;
                                        }
                                    }
                                }
                            }

                            if (ds.anRows[rowIndex] == rgCovered.Top)
                            {
                                for (col1 = colIndex; col1 <= col2; col1++)
                                {
                                    int i2 = (rowIndex * ds.nCols) + col1;

                                    if (ds.apStyles[i2] != null)
                                    {
                                        // Top
                                        if (ds.apBorders[i2][tHorzLine].Style != GridBorderStyle.Standard)
                                        {
                                            ds.abBits[i2] |= nHorzFlag;
                                        }

                                        if ((ds.abBits[i2] & nHorzFlag) == 0)
                                        {
                                            ds.abBits[i2] |= GridPaint.DrawCellFlags.HorzLine;
                                        }
                                    }
                                }
                            }
                        }

                        // Fixed column.
                        if (ds.anCols[col2] > ds.gridHeaderCols && ds.anCols[col2] == ds.gridFrozenCols
                            && ds.anCols[col2] == rgCovered.Right)
                        {
                            for (row1 = rowIndex; row1 <= row2; row1++)
                            {
                                int i2 = (row1 * ds.nCols) + col2;

                                if (ds.apStyles[i2] != null && ds.apBorders[i2].Right.Style != GridBorderStyle.Standard)
                                {
                                    ds.abBits[i2] |= GridPaint.DrawCellFlags.FixedVertLine;
                                }
                            }
                        }

                        //// Fixed row.
                        if (ds.anRows[row2] > ds.gridHeaderRows && ds.anRows[row2] == ds.gridFrozenRows
                            && ds.anRows[row2] == rgCovered.Bottom)
                        {
                            for (col1 = colIndex; col1 <= col2; col1++)
                            {
                                int i2 = (row2 * ds.nCols) + col1;

                                if (ds.apStyles[i2] != null && ds.apBorders[i2].Bottom.Style != GridBorderStyle.Standard)
                                {
                                    ds.abBits[i2] |= GridPaint.DrawCellFlags.FixedHorzLine;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    ////                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    ////                        throw ex;
                }

                ds.bAnyCellCovered = true;
                if (ds.bColor)
                {
                    ds.abBits[i] |= GridPaint.DrawCellFlags.Color;
                }

                return true;
            }

            return false;
        }

        public bool DrawSpannedCell(GridControlBase pGrid, object pds, int rowIndex, int colIndex)
        {
            GridDataBoundGrid gdbg = pGrid as GridDataBoundGrid;
            GridPaint.DrawStruct ds = (GridPaint.DrawStruct)pds;
            GridRangeInfo rgCovered;

            int i;

            //// If this is a cell which is hidden by a merge or floating cell
            //// and if it is the current cell, we have to outline the frame
            //// around the cell.

            if (ds.apStyles[i = (rowIndex * ds.nCols) + colIndex] != null && (ds.abBits[i] & GridPaint.DrawCellFlags.SpannedCont) != 0)
            {
#if DEBUG
                if (Switches.GridPaint.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(m_grid.PaneDesc, "SpannedCont", rowIndex, colIndex);
                }
#else
                ;
#endif
                //// Get control only for inverting its borders when a cell
                //// is hidden by floating or merged cell.

                if ((ds.abBits[i] & GridPaint.DrawCellFlags.Covered) == 0 &&
                    ds.rectItem.Width > 0 && ds.rectItem.Height > 0)
                {
                    GridCellRendererBase pControl = null;

                    if (pGrid.CurrentCell.HasCurrentCellAt(ds.anRows[rowIndex], ds.anCols[colIndex]))
                    {
                        pControl = pGrid.CurrentCell.Renderer;

                        //// Borders will be drawn later.
                        // && !pControl.HasFocusControl)
                        if (pControl != null)
                        {
                            if (pControl.InitalizedAt(ds.anRows[rowIndex], ds.anCols[colIndex]))
                            {
                                ds.pCurrentControl = pControl;
                            }

                            ds.rectEdit = GetCellRectangle(ds.rectItem);
                            // can't use ds.apStyles[i] here - this would reference covered cell ...
                            GridStyleInfo style = pGrid.GetPaintStyleInfo(ds.anRows[rowIndex], ds.anCols[colIndex], false);
                            ds.rectEdit = pGrid.Model.SubtractBorders(ds.rectEdit, style, m_grid.IsRightToLeft());
                            m_grid.DisposePaintStyle(style);
                        }
                    }
                }

                return true;
            }
            else if (ds.apStyles[i] != null)
            {
                //// This is either a spanned cell or normal cell.

                if ((ds.abBits[i] & GridPaint.DrawCellFlags.Spanned) != 0)
                {
                    pGrid.Model.GetSpannedRangeInfo(ds.anRows[rowIndex], ds.anCols[colIndex], out rgCovered);
                    GridRangeInfo pr = rgCovered;
                    Debug.Assert(!pr.IsEmpty);

                    GridRangeInfo[] r = new GridRangeInfo[4];

                    //// I have to deal with frozen rows and columns:
                    ////    +--|-----------+
                    ////    |r0|r2         |
                    ////   ======================= top row
                    ////    |r1|r3         |
                    ////    |  |           |r
                    ////    +--|-----------+
                    ////       |
                    ////      left column

                    int nLastFixedRow = pGrid.GetRow(pGrid.InternalGetFrozenRows());
                    int nLastFixedCol = pGrid.GetCol(pGrid.InternalGetFrozenCols());

                    r[0] = GridRangeInfo.InternalCells(
                        pr.Top,
                        pr.Left,
                        Math.Min(nLastFixedRow, pr.Bottom),
                        Math.Min(nLastFixedCol, pr.Right));

                    r[1] = GridRangeInfo.InternalCells(
                        Math.Max(pGrid.TopRowIndex, pr.Top),
                        pr.Left,
                        pr.Bottom,
                        Math.Min(nLastFixedCol, pr.Right));

                    r[2] = GridRangeInfo.InternalCells(
                        pr.Top,
                        Math.Max(pGrid.LeftColIndex, pr.Left),
                        Math.Min(nLastFixedRow, pr.Bottom),
                        pr.Right);

                    r[3] = GridRangeInfo.InternalCells(
                        Math.Max(pGrid.TopRowIndex, pr.Top),
                        Math.Max(pGrid.LeftColIndex, pr.Left),
                        pr.Bottom,
                        pr.Right);

                    Rectangle rectVisible, rect;
                    GridCellRendererBase pControl = null;

                    ////GridStyleInfo style = pGrid.Model[pr.Top, pr.Left];
                    GridStyleInfo style = pGrid.GetPaintStyleInfo(pr.Top, pr.Left, true);

                    if (pGrid.CurrentCell.HasCurrentCellAt(ds.anRows[rowIndex], ds.anCols[colIndex])
                        || ((ds.abBits[i] & GridPaint.DrawCellFlags.Covered) != 0
                        && pGrid.CurrentCell.HasCurrentCellAt(pr.Top, pr.Left)))
                    {
                        pControl = pGrid.CellRenderers[style.CellType];
                    }

                    //// Draw each subrectangle, take care on clipping.
                    bool bInvert = false;
                    ////int[] rtlPhases = new int[] { 2, 3, 0, 1 };
                    for (int j1 = 0; j1 <= 3; j1++)
                    {
                        int j = j1;
                        if (r[j].IsEmpty
                            || r[j].Bottom < ds.anRows[rowIndex]
                            || r[j].Right < ds.anCols[colIndex])
                        {
                            continue;
                        }

                        bool bClip = false; ////m_grid.PrintingMode;
                        
                        if (gdbg != null && gdbg.DrawIndividualSpannedCellBorders)
                            rect = GetCellRectangle(ds.rectItem);
                        else
                            rect = pGrid.ViewLayout.RangeInfoToRectangle(r[j], false, GridCellSizeKind.ActualSize);

                        rectVisible = pGrid.ViewLayout.RangeInfoToRectangle(r[j], false, GridCellSizeKind.VisibleSize);
                        rectVisible.Intersect(pGrid.GridBounds);
                        // phase 2 and 3: clip at left side
                        if (j >= 2)
                        {
                            if (this.isRightToLeft())
                            {
                                if (rectVisible.Right < ds.scrollRect.Right)
                                {
                                    bClip = true;
                                    GridUtil.SetRight(ref rectVisible, ds.scrollRect.Right);
                                }
                            }
                            else
                            {
                                if (rectVisible.Left < ds.scrollRect.Left)
                                {
                                    bClip = true;
                                    GridUtil.SetLeft(ref rectVisible, ds.scrollRect.Left);
                                }
                            }
                        }

                        // phase 1 and 3: clip at top side
                        if (j == 1 || j == 3)
                        {
                            if (rectVisible.Top < ds.scrollRect.Top)
                            {
                                bClip = true;
                                GridUtil.SetTop(ref rectVisible, ds.scrollRect.Top);
                            }
                        }

                        rectVisible.Intersect(Rectangle.Ceiling(ds.graphics.ClipBounds));

                        //// clipping
                        RectangleF clipBounds = RectangleF.Empty;

                        try
                        {
                            //// Full rectangle (can contain negative top and left value).

                            bool coveredCellFullRect = style.CellModel.coveredCellFullRect;
                            if (coveredCellFullRect)
                            {
                                if (pr.Top < r[j].Top)
                                {
                                    bClip = true;
                                    GridUtil.OffsetTop(ref rect, -pGrid.ViewLayout.GetRowRangeHeight(pr.Top, r[j].Top - 1, GridCellSizeKind.ActualSize));
                                }

                                if (pr.Bottom > r[j].Bottom)
                                {
                                    bClip = true;
                                    rect.Height += pGrid.ViewLayout.GetRowRangeHeight(r[j].Bottom + 1, pr.Bottom, GridCellSizeKind.ActualSize);
                                }
                            }
                            else
                            {
                                rect.Y = Math.Max(ds.nyMin, rect.Top);
                                rect.Height = Math.Min(rect.Height, ds.nyMax - rect.Top + 1);
                            }

                            if (pr.Left < r[j].Left)
                            {
                                bClip = true;
                                if (this.isRightToLeft())
                                {
                                    rect.Width += pGrid.ViewLayout.GetColRangeWidth(pr.Left, r[j].Left - 1, GridCellSizeKind.ActualSize);
                                }
                                else
                                {
                                    GridUtil.OffsetLeft(ref rect, -pGrid.ViewLayout.GetColRangeWidth(pr.Left, r[j].Left - 1, GridCellSizeKind.ActualSize));
                                }
                            }

                            if (pr.Right > r[j].Right)
                            {
                                bClip = true;
                                if (this.isRightToLeft())
                                {
                                    GridUtil.OffsetLeft(ref rect, -pGrid.ViewLayout.GetColRangeWidth(r[j].Right + 1, pr.Right, GridCellSizeKind.ActualSize));
                                }
                                else
                                {
                                    rect.Width += pGrid.ViewLayout.GetColRangeWidth(r[j].Right + 1, pr.Right, GridCellSizeKind.ActualSize);
                                }
                            }
                            ////TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, rectVisible, rect);

                            bClip |= !rectVisible.Contains(rect);

                            // Force background drawing, but not when it is part of a banner cell.
                            pGrid.m_bDrawCoveredCell = (ds.abBits[i] & DrawCellFlags.Bannered) == 0;
                            if (rect.Width > 0 && rect.Height > 0)
                            {
                                if (bClip)
                                {
                                    // Clip area if cell is only partly visible.
                                    Rectangle rectIntersect;
                                    clipBounds = ds.graphics.ClipBounds;
                                    rectIntersect = Rectangle.Intersect(rectVisible, ds.rectClip);
                                    if (rectIntersect.IsEmpty)
                                    {
                                        clipBounds = RectangleF.Empty;
                                    }
                                    else
                                    {
                                        ds.graphics.IntersectClip(rectIntersect);

                                        // Draw it with full rectangle.
                                        pGrid.OnDrawItem(ds.graphics, coveredCellFullRect ? pr.Top : ds.anRows[rowIndex], pr.Left, rect, style);
                                    }
                                }
                                else
                                {
                                    // Draw it with full rectangle.
                                    pGrid.OnDrawItem(ds.graphics, coveredCellFullRect ? pr.Top : ds.anRows[rowIndex], pr.Left, rect, style);
                                }
                            }

                            pGrid.m_bDrawCoveredCell = false;

                            // Draw Borders.
                            if (rect.Height > 0 && rect.Width > 0)
                            {
                                pGrid.OnDrawBorders(ds.graphics, rect, style);
                            }

                            if (pControl != null)
                            {
                                if (!pControl.HasFocusControl && !bInvert)
                                {
                                    if ((ds.abBits[i] & GridPaint.DrawCellFlags.Covered) == 0)
                                    {
                                        rect = ds.rectItem;
                                    }

                                    rect = pGrid.Model.SubtractBorders(rect, style, m_grid.IsRightToLeft());

                                    if (!rect.IsEmpty)
                                    {
                                        pControl.RaiseOutlineCurrentCell(ds.graphics, rect);
                                    }

                                    if ((ds.abBits[i] & GridPaint.DrawCellFlags.Covered) == 0)
                                    {
                                        bInvert = true;
                                    }
                                }
                                else
                                {
                                    ds.pCurrentControl = pControl;
                                    if ((ds.abBits[i] & GridPaint.DrawCellFlags.Merged) != 0)
                                    {
                                        ds.rectEdit = pGrid.Model.SubtractBorders(GetCellRectangle(ds.rectItem), ds.apStyles[i], m_grid.IsRightToLeft());
                                    }
                                    else
                                    {
                                        ds.rectEdit = pGrid.Model.SubtractBorders(rect, ds.apStyles[i], m_grid.IsRightToLeft());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                            throw;
                        }
                        finally
                        {
                            if (clipBounds != RectangleF.Empty)
                            {
                                ds.graphics.SetClip(clipBounds);
                            }

                            if (gdbg != null && gdbg.DrawIndividualSpannedCellBorders)
                                pGrid.Invalidate(pGrid.ViewLayout.RangeInfoToRectangle(r[j], GridCellSizeKind.VisibleSize),true);
                        }
                    }

                    m_grid.DisposePaintStyle(style);

                    return true;
                }
            }

            return false;
        }
    }
}
