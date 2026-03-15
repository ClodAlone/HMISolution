#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid
{

    public class GridResizeColumnsMouseController : IMouseController, IDisposable
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4.0;
        double hitTestHiddenColPrecision = 6.0;
        bool inHiddenColResize = false;
        GridControlBase host;
        VisibleLineInfo dragLine = null;
        SuspendState suspendState;
        public GridResizeColumnsMouseController(GridControlBase grid)
        {
            this.host = grid;
            //this.scrollRows = grid.ScrollRows;
            //this.scrollColumns = grid.ScrollColumns;

            if (inHiddenColResize && this.host.Model.Options.AllowExcelLikeResizing)
                this._cursor = CellCursors.ResizeHiddenColumnCursor;
            else
                this._cursor = CellCursors.ResizeWidthCursor;
            
        }

        private VisibleLineInfo HitTest(Point point)
        {
            inHiddenColResize = false;

            if (point.X == 0.0)
            {
                return scrollColumns.GetVisibleLineAtPoint(point.X);
            }
            else
            {
                var info = scrollColumns.GetLineNearCorner(point.X, hitTestPrecision);

                // Check to see if any hidden columns exist
                if (info == null && this.host.Model.Options.AllowExcelLikeResizing)
                {
                    var lineInfo = scrollColumns.GetLineNearCorner(point.X, hitTestHiddenColPrecision, CornerSide.Right);

                    if (lineInfo != null)
                    {
                        var lineIndex = lineInfo.LineIndex;
                        int rc;
                        var isHidden = host.Model.ColumnWidths.GetSize(lineIndex + 1, out rc) == 0;

                        if (isHidden)
                        {
                            inHiddenColResize = true;
                            //set the hidden column resize cursor
                            this._cursor = CellCursors.ResizeHiddenColumnCursor;
                            info = lineInfo;
                        }
                    }
                    if (point.X < edgeWidth && point.X >=1)
                    {
                        RowColumnIndex rowColumnIndex = host.PointToCellRowColumnIndex(point, true);
                        if (rowColumnIndex.ColumnIndex > 0)
                        {
                            inHiddenColResize = true;
                            this._cursor = CellCursors.ResizeHiddenColumnCursor;
                            info = lineInfo;
                            var line = new VisibleLineInfo(0, rowColumnIndex.ColumnIndex - 1, 0, point.X, 0, true, false);
                            info = line;
                        }
                    }
                }
                else
                {
                    this._cursor = CellCursors.ResizeWidthCursor;
                }

                return info;
            }
        }

        #region IMouseController Members

        public string Name
        {
            get { return "ResizeColumnsMouseController"; }
        }      

        private Cursor _cursor;


        public Cursor Cursor
        {
            get
            {
                return _cursor;
            }
            set
            {
                _cursor = value;
            }
        }

        public void MouseHoverEnter(MouseEventArgs e)
        {
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
        }

        private Point mouseDownPoint;
        private bool IsResizingLastHiddenColumn = false;
        private int hiddenCount = 1;
        public void MouseDown(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            mouseDownPoint = e.Location;
            TraceUtil.TraceCurrentMethodInfo(point);

            dragLine = HitTest(point);
            hiddenCount = 1;
            
            var info = scrollColumns.GetLineNearCorner(point.X, hitTestPrecision);
            //info will be null when the resizing hidden columns
            if (info == null)
            {
                var rc = 0;
                var IsLastColumnHidden = host.Model.ColumnWidths.GetHidden(host.Model.ColumnCount - 1,out rc);
                rc = 0;
                while (host.Model.ColumnWidths.GetHidden(host.Model.ColumnCount - hiddenCount, out rc))
                {
                    hiddenCount++;
                }
                if (dragLine.LineIndex == host.Model.ColumnCount - hiddenCount && IsLastColumnHidden)
                    IsResizingLastHiddenColumn = true;
            }

            if (point.X == 0.0 && e.ClickCount == 2 && point.X < edgeWidth)
            {
                dragLine = new VisibleLineInfo(0, 0, 0, point.X, 0, true, false);
            }

            if (dragLine == null && e.ClickCount == 2 && point.X < edgeWidth && point.X > 0)
            {
                dragLine = new VisibleLineInfo(0, 0, 0, point.X, 0, true, false);
            }

            if (dragLine != null && e.ClickCount == 2)
            {
                int l = dragLine.LineIndex;
                int rc, rh;

                double width = host.Model.ColumnWidths.GetSize(l , out rc);
                bool isHidden = host.Model.ColumnWidths.GetHidden(l , out rh);

                if (inHiddenColResize && isHidden && this.host.Model.Options.AllowExcelLikeResizing)
                {
                    l += rh;
                    host.ColumnWidths.SetHidden(l, l, false);
                }

                if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(l), ref width, GridResizeCellsReason.DoubleClick, e.Location, true, inHiddenColResize))
                {
                    if (width == 0 && this.host.Model.Options.AllowExcelLikeResizing)
                        host.Model.ColumnWidths.SetRange(l, l, host.Model.ColumnWidths.DefaultLineSize);                                        
                }
                dragLine = null;
            }
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            // VirtualizingCellsControl cellsControl = host;// as VirtualizingCellsControl; Unused local variable

            if (dragLine != null)
            {
                double delta;
                int repeatCount;
                var isHidden = host.Model.ColumnWidths.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                if (isHidden && inHiddenColResize)
                    delta = point.X - dragLine.Corner;
                else
                    delta = point.X - mouseDownPoint.X;
                double width = Math.Max(0, dragLine.Size + delta);

                var processed = false;
                int index = dragLine.LineIndex;
                RowColumnIndex rowColumnIndex = host.PointToCellRowColumnIndex(point, true);
                if (isHidden && inHiddenColResize)
                {
                    if (point.X <= 5 && point.X >= 1 && rowColumnIndex.ColumnIndex > 0)
                    {
                        host.ColumnWidths.SetHidden(index, index, false);
                        processed = true;
                    }
                }
                // Hidden Col Rezing Block

                if (isHidden && (inHiddenColResize || IsResizingLastHiddenColumn) && this.host.Model.Options.AllowExcelLikeResizing)
                {
                    var hiddenDelta = delta;
                    if (IsResizingLastHiddenColumn)
                        hiddenDelta = Math.Max(-1, point.X - dragLine.Corner);
                    if (hiddenDelta < 0)
                        return;
                    var hiddenLineIndex = IsResizingLastHiddenColumn ? this.host.Model.ColumnCount - (hiddenCount - 1) : dragLine.LineIndex + repeatCount;

                    // Set the line size before and after updating hidden state to update the ViewSize else resizing the last hidden column will be unsuccessful
                    host.ScrollColumns.SetLineResize(hiddenLineIndex, hiddenDelta);
                    host.ColumnWidths.SetHidden(hiddenLineIndex, hiddenLineIndex, false);
                    host.ScrollColumns.SetLineResize(hiddenLineIndex, hiddenDelta);
                    // TODO: Need to refresh the scroll bar value, else the HitTest method return the visible line as null 
                    //when the scroll bar position at last column and when we resize the hidden column. 
                    //(Because the scroll bar maximum value was updated based on the Distances.TotalDistance but the value was not updated)
                    if (host.ScrollColumns.ScrollBar.Maximum - host.ScrollColumns.ScrollBar.LargeChange + 1 <= host.ScrollColumns.ScrollBar.Value)
                        host.ScrollColumns.ScrollToNextLine();

                    dragLine = HitTest(e.Location); // Due to this call "inHiddenColResize" will get reset. So care must be taken in subsequent calling.
                    if (dragLine == null)
                        return;
                    processed = true;
                }
                var lineIndex = IsResizingLastHiddenColumn ? this.host.Model.ColumnCount - (hiddenCount - 1) : dragLine.LineIndex;
                if (IsResizingLastHiddenColumn)
                    width = Math.Max(-1, point.X - dragLine.Corner);
                if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(lineIndex), ref width, GridResizeCellsReason.MouseMove, e.Location, true, processed))
                {
                    //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                    if (!processed) // Dont use "inHiddenColResize" condition check here since the hidden col resizing is no longer happening.
                    {
                        if (width >= 0)
                        {
                            if (width == 0 && this.host.Model.Options.AllowExcelLikeResizing)
                                host.Model.ColumnWidths.SetHidden(lineIndex, lineIndex, true);
                            else
                            {
                                if (width > 0 && host.Model.ColumnWidths.GetHidden(lineIndex, out repeatCount))
                                    host.Model.ColumnWidths.SetHidden(lineIndex, lineIndex, false);
                                host.ScrollColumns.SetLineResize(lineIndex, width);
                            }
                        }
                    }
                    else
                    {
                        //Resetting the LineCollection while UnHiding the Column
                        host.ScrollColumns.ResetLineResize();
                    }
                }
            }
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            //// Sample: enable mouse tracking on click-release.
            //if (!host.MouseControllerDispatcher.IsMouseTracking 
            //    && GridUtil.GetSurroundingRect(host.MouseControllerDispatcher.MouseDownLocation, new Size(2, 2)).Contains(e.Location))
            //{
            //    host.MouseControllerDispatcher.StartTrackMouse();
            //    return;
            //}
            if (dragLine == null || host.MouseControllerDispatcher.MouseDownLocation == e.Location)
            {
                dragLine = null;
                return;
            }

            Point point = e.Location;
            double delta = point.X - mouseDownPoint.X;
            double width = Math.Max(0, dragLine.Size + delta);
            if (IsResizingLastHiddenColumn)
                width = Math.Max(-1, point.X - dragLine.Corner);
            var lineIndex = IsResizingLastHiddenColumn ? this.host.Model.ColumnCount - (hiddenCount - 1) : dragLine.LineIndex;
            if (!inHiddenColResize && host.RaiseResizingColumnsEvent(GridRangeInfo.Col(lineIndex), ref width, GridResizeCellsReason.MouseUp, e.Location))
            {
                //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                host.SetColumnWidth(lineIndex, Math.Max(0, width));
            }
            host.ScrollColumns.ResetLineResize();
            dragLine = null;
            if (IsResizingLastHiddenColumn)
                IsResizingLastHiddenColumn = false;
        }

        public void CancelMode()
        {
            if (dragLine != null)
            {
                suspendState = new SuspendState(this);
                host.ScrollColumns.ResetLineResize();
            }
            dragLine = null;
        }

        public virtual bool GetInHiddenRowResizeState()
        {
            return this.inHiddenColResize;
        }

        public virtual double GetHitTestPrecision()
        {
            if (inHiddenColResize && this.host.Model.Options.AllowExcelLikeResizing)
                return this.hitTestHiddenColPrecision;
            else
                return this.hitTestPrecision;
        }

        VisibleLineInfo hitLine;
        double edgeWidth = 5;
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.Button == MouseButton.Right)
                return 0;
            Point point = mouseEventArgs.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            RowColumnIndex pos = host.PointToCellRowColumnIndex(point, true);
            CoveredCellInfo cc = host.GetCoveredCell(pos);
            VisibleLineInfo hit = HitTest(point);
            hitLine = hit;
            bool edgeHit = point.X < edgeWidth && point.X > 0;
            if (hitLine == null && edgeHit)
            {
                hitLine = new VisibleLineInfo(0, 0, 0, point.X, 0, true, false);
            }
            if (hit != null || edgeHit)
            {
                // sometimes hit is null when edgehit has value in nested grid
                if (hit == null)
                {
                    return 0;
                }

                if (cc == null || cc.Left - 1 == hit.LineIndex || cc.Right == hit.LineIndex || edgeHit)
                {
                    VisibleLineInfo row = scrollRows.GetVisibleLineAtPoint(point.Y);
                    if (row != null && row.LineIndex < host.Model.HeaderRows)
                    {
                        int rc;
                        int index = hit == null ? 0 : hit.LineIndex;
                        double width = host.Model.ColumnWidths.GetSize(index, out rc);
                        // Event is raised for any cell in grid, but default is that AllowResize is only true
                        // when the cursor is over a divider in header area. A user can override this and 
                        // set AllowResize = true to enable resizing also for any cell within the grid.
                        bool allowResize = row.IsHeader;
                        if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(index), ref width, GridResizeCellsReason.HitTest, point, allowResize, inHiddenColResize))
                        {
                            return 1;
                        }
                    }

                    return 0;
                }
            }
            return 0;
        }

        public bool SupportsCancelMouseCapture
        {
            get { return false; }
        }

        public bool SupportsMouseTracking
        {
            get { return false; }
        }

        public void RestoreMode()
        {
            if (suspendState != null)
                suspendState.Restore();
            suspendState = null;
            host.InvalidateVisual();
            host.UpdateLayout();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.host != null)
            {
                this.host = null;
            }
        }

        #endregion

        /// <summary>
        /// For internal use.
        /// </summary>
        public class SuspendState
        {
            GridResizeColumnsMouseController mc;
            double size;
            VisibleLineInfo dragLine;

            /// <summary>
            /// For internal use.
            /// </summary>
            public SuspendState(GridResizeColumnsMouseController mc)
            {
                this.mc = mc;
                dragLine = mc.dragLine;
                size = mc.host.ScrollColumns.GetLineSize(dragLine.LineIndex);
            }

            /// <summary>
            /// For internal use.
            /// </summary>
            public void Restore()
            {
                mc.dragLine = dragLine;
                mc.host.ScrollColumns.SetLineResize(dragLine.LineIndex, size);
            }
        }
    }

}
