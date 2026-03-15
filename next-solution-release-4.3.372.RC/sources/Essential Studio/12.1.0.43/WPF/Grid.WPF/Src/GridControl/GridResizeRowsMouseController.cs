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

    class GridResizeRowsMouseController : IMouseController
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4.0;
        double hitTestHiddenRowPrecision = 6.0;
        bool inHiddenRowResize = false;
        protected GridControlBase host;
        protected VisibleLineInfo dragLine = null;
        SuspendState suspendState;

        public GridResizeRowsMouseController(GridControlBase grid)
        {
            this.host = grid;
            //this.scrollRows = grid.ScrollRows;
            //this.scrollColumns = grid.ScrollColumns;
            if (inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
                _cursor = CellCursors.ResizeHiddenRowCursor;
            else
                _cursor = CellCursors.ResizeHeightCursor;
        }

        protected VisibleLineInfo HitTest(Point point)
        {
            inHiddenRowResize = false;

            var info = scrollRows.GetLineNearCorner(point.Y, hitTestPrecision);

            // Check to see if any hidden rows exist
            if (info == null && this.host.Model.Options.AllowExcelLikeResizing)
            {
                var lineInfo = scrollRows.GetLineNearCorner(point.Y, hitTestHiddenRowPrecision, CornerSide.Bottom);
                if (lineInfo != null)
                {
                    var lineIndex = lineInfo.LineIndex;
                    int rc;
                    var isHidden = host.Model.RowHeights.GetHidden(lineIndex + 1, out rc);

                    if (isHidden && this.host.Model.IsRowHidden(lineIndex + 1, true))
                    {
                        inHiddenRowResize = true;
                        //set the hidden Row resize cursor
                        this._cursor = CellCursors.ResizeHiddenRowCursor;
                        info = lineInfo;
                    }
                }
            }
            else
            {
                this._cursor = CellCursors.ResizeHeightCursor;
            }
            return info;
        }

        #region IMouseController Members

        public string Name
        {
            get { return "ResizeRowsMouseController"; }
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

        public virtual bool GetInHiddenRowResizeState()
        {
            return this.inHiddenRowResize;
        }

        public virtual double GetHitTestPrecision()
        {
            if (inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
                return this.hitTestHiddenRowPrecision;
            else
                return this.hitTestPrecision;
        }

        private Point mouseDownPoint;
        virtual public void MouseDown(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            mouseDownPoint = e.Location;
            TraceUtil.TraceCurrentMethodInfo(point);

            dragLine = HitTest(point);

            if (dragLine != null && e.ClickCount == 2)
            {
                int l = dragLine.LineIndex;
                int rc, rh;

                double height = host.Model.RowHeights.GetSize(l + 1, out rc);
                bool isHidden = host.Model.RowHeights.GetHidden(l + 1, out rh);

                if (inHiddenRowResize && isHidden && this.host.Model.Options.AllowExcelLikeResizing)
                {
                    l += this.GetLineIndex(rh);

                    host.RowHeights.SetHidden(l, l, false);
                }

                if (host.RaiseResizingRowsEvent(GridRangeInfo.Row(l), ref height, GridResizeCellsReason.DoubleClick, e.Location, true, inHiddenRowResize))
                {
                    if (this.host.Model.Options.AllowExcelLikeResizing)
                        host.Model.RowHeights.SetRange(l, l, host.Model.RowHeights.DefaultLineSize);
                }

                host.InvalidateVisual(true);
                if (host.ScrollOwner != null)
                    host.ScrollOwner.InvalidateScrollInfo();

                dragLine = null;
            }
        }

        protected virtual int GetLineIndex(int rowIndex)
        {
            return rowIndex;
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            //VirtualizingCellsControl cellsControl = host;// as VirtualizingCellsControl; Unused local variable

            if (dragLine != null)
            {
                double delta;
                int repeatCount;
                var isHidden = host.Model.RowHeights.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                if (isHidden && inHiddenRowResize)
                    delta = point.Y - dragLine.Corner;
                else
                    delta = point.Y - mouseDownPoint.Y;
                double height = Math.Max(0, dragLine.Size + delta);
                var processed = false;

                // Hidden Row Rezing Block

                if (isHidden && inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
                {
                    var hiddenLineIndex = dragLine.LineIndex + repeatCount;

                    // Set the line size before and after updating hidden state to update the ViewSize else resizing the last hidden row will be unsuccessful
                    host.SetRowResize(hiddenLineIndex, delta);
                    host.RowHeights.SetHidden(hiddenLineIndex, hiddenLineIndex, false);
                    host.SetRowResize(hiddenLineIndex, delta);

                    // TODO: Need to refresh the scroll bar value, else the HitTest method return the visible line as null 
                    //when the scroll bar position at last row and when we resize the hidden row. 
                    //(Because the scroll bar maximum value was updated based on the Distances.TotalDistance but the value was not updated)
                    if (host.ScrollRows.ScrollBar.Maximum - host.ScrollRows.ScrollBar.LargeChange + 1 <= host.ScrollRows.ScrollBar.Value)
                        host.ScrollRows.ScrollToNextLine();

                    dragLine = HitTest(e.Location); // Due to this call "inHiddenColResize" will get reset. So care must be taken in subsequent calling.
                    processed = true;
                }

                if (host.RaiseResizingRowsEvent(GridRangeInfo.Row(dragLine.LineIndex), ref height, GridResizeCellsReason.MouseMove, e.Location, true, processed))
                {
                    //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                    if (!processed) // Dont use "inHiddenColResize" condition check here since the hidden col resizing is no longer happening.
                    {
                        if (height == 0 && this.host.Model.Options.AllowExcelLikeResizing)
                            host.Model.RowHeights.SetHidden(dragLine.LineIndex, dragLine.LineIndex, true);
                        else
                        {
                            if (height > 0 && host.Model.RowHeights.GetHidden(dragLine.LineIndex, out repeatCount))
                                host.Model.RowHeights.SetHidden(dragLine.LineIndex, dragLine.LineIndex, false);
                            host.SetRowResize(dragLine.LineIndex, height);
                        }
                    }

                    RefreshRowGridLines();
                }
            }
        }

        public void RefreshRowGridLines()
        {
            host.InvalidateVisual(true);
            if (host.ScrollOwner != null)
                host.ScrollOwner.InvalidateScrollInfo();
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
            if (dragLine == null || host.MouseControllerDispatcher.MouseDownLocation==e.Location)
            {
                dragLine = null;
                return;
            }

            Point point = e.Location;
            double delta = point.Y - mouseDownPoint.Y;
            double height = Math.Max(0, dragLine.Size + delta);
            if (!inHiddenRowResize && host.RaiseResizingRowsEvent(GridRangeInfo.Row(dragLine.LineIndex), ref height, GridResizeCellsReason.MouseUp, e.Location))
            {
                //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                host.SetRowHeight(dragLine.LineIndex, Math.Max(0, dragLine.Size + delta));
            }
            host.ResetRowResize();
            dragLine = null;
        }

        public void CancelMode()
        {
            if (dragLine != null)
            {
                suspendState = new SuspendState(this);
                host.ResetRowResize();
            }
            dragLine = null;
            // Lazy way: Don't reset dragLine - then RestoreMode will work just fine.
            //dragLine = null;
        }

        private bool IsNotNested(VisibleLineInfo dragLine)
        {
            var lineSizeCollection = this.host.Model.RowHeights as LineSizeCollection;
            var result = lineSizeCollection.GetNestedLines(dragLine.LineIndex) != null;
            return result;
        }

        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.Button == MouseButton.Right)
                return 0;
            Point point = mouseEventArgs.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            RowColumnIndex pos = host.PointToCellRowColumnIndex(point, true);
            CoveredCellInfo cc = host.GetCoveredCell(pos);
            VisibleLineInfo hit = HitTest(point);
            if (hit != null && !this.IsNotNested(hit))
            {
                if (cc == null || cc.Top - 1 == hit.LineIndex || cc.Bottom == hit.LineIndex)
                {
                    VisibleLineInfo column = scrollColumns.GetVisibleLineAtPoint(point.X);                    
                    if (column != null && column.LineIndex < host.Model.HeaderColumns)
                    {
                        int rc;
                        double height = host.Model.RowHeights.GetSize(hit.LineIndex, out rc);
                        // Event is raised for any cell in grid, but default is that AllowResize is only true
                        // when the cursor is over a divider in header area. A user can override this and 
                        // set AllowResize = true to enable resizing also for any cell within the grid.
                        bool allowResize = column.IsHeader;
                        if (host.RaiseResizingRowsEvent(GridRangeInfo.Row(hit.LineIndex), ref height, GridResizeCellsReason.HitTest, point, allowResize))
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

        /// <summary>
        /// For internal use.
        /// </summary>
        public class SuspendState
        {
            GridResizeRowsMouseController mc;
            double size;
            VisibleLineInfo dragLine;

            /// <summary>
            /// For internal use.
            /// </summary>
            public SuspendState(GridResizeRowsMouseController mc)
            {
                this.mc = mc;
                dragLine = mc.dragLine;
                size = mc.host.ScrollRows.GetLineSize(dragLine.LineIndex);
            }

            /// <summary>
            /// For internal use.
            /// </summary>
            public void Restore()
            {
                mc.dragLine = dragLine;
                mc.host.SetRowResize(dragLine.LineIndex, size);
            }
        }

    }

}