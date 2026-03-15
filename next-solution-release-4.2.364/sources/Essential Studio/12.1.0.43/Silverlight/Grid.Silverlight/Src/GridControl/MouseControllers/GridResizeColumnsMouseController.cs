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

#if! WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.Devices.Input;
using Windows.Foundation;
namespace Syncfusion.WinRT.Controls.Cells
{
#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    class GridResizeColumnsMouseController : IMouseController
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4.0;
        double hitTestHiddenColPrecision = 6.0;
        bool inHiddenColResize = false;
        GridControlBase host;
        IEditableLineSizeHost columnWidths;
        VisibleLineInfo dragLine = null;
        bool isResized = false;
        //bool isDoubleClick = false;

        public GridResizeColumnsMouseController(GridControlBase viewer)
        {
            this.host = viewer;
        }

        private VisibleLineInfo HitTest(Point point)
        {
            inHiddenColResize = false;

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
                        info = lineInfo;
                    }
                }
            }

            return info;
        }

        #region IMouseController Members

        public string Name
        {
            get { return "GridResizeColumnsMouseController"; }
        }

#if !WinRT
        public Cursor Cursor
        {
            get
            {
                if (inHiddenColResize && this.host.Model.Options.AllowExcelLikeResizing)
                    return Cursors.SizeNWSE;
                else
                    return Cursors.SizeWE;
            }
        }
#else
        public CoreCursor Cursor
        {
            get
            {
                if (inHiddenColResize && this.host.Model.Options.AllowExcelLikeResizing)
                    return new CoreCursor(CoreCursorType.Custom, 1);
                else
                    return new CoreCursor(CoreCursorType.Custom, 1);
            }
        }
#endif
        public void MouseHoverEnter(MouseEventArgs e)
        {
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
        }

        public void MouseDown(MouseControllerEventArgs e)
        {
            Point point = e.Location;

            dragLine = HitTest(point);

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

                double width = host.Model.ColumnWidths.GetSize(l + 1, out rc);
                bool isHidden = host.Model.ColumnWidths.GetHidden(l + 1, out rh);

                if (inHiddenColResize && isHidden)
                {
                    l += rh;
                    host.ColumnWidths.SetHidden(l, l, false);
                }

                if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(l), ref width, GridResizeCellsReason.DoubleClick, e.Location))
                {
                    if (width == 0 && this.host.Model.Options.AllowExcelLikeResizing)
                        host.Model.ColumnWidths.SetRange(l, l + rc - 1, host.Model.ColumnWidths.DefaultLineSize);
                }
                dragLine = null;
            }

            isResized = false;
            //isDoubleClick = dragLine != null && e.ClickCount == 2;
            columnWidths = host.ColumnWidthsProvider as IEditableLineSizeHost;
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;

            if (dragLine != null)
            {
                double delta = point.X - dragLine.Corner;

                if (isResized || Math.Abs(delta) > 0.5)
                {
                    isResized = true;
                    double width = Math.Max(0, dragLine.Size + delta);
                    int repeatCount;
                    var isHidden = host.Model.ColumnWidths.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                    var processed = false;

                    // Hidden Col Resizing Block

                    if (isHidden && inHiddenColResize && this.host.Model.Options.AllowExcelLikeResizing && delta >= 0)
                    {
                        var hiddenLineIndex = dragLine.LineIndex + repeatCount;
                        // Set the line size before and after updating hidden state to update the ViewSize else resizing the last hidden column will be unsuccessful                        
                        host.ScrollColumns.SetLineResize(hiddenLineIndex, delta);
                        host.ColumnWidths.SetHidden(hiddenLineIndex, hiddenLineIndex, false);
                        host.ScrollColumns.SetLineResize(hiddenLineIndex, delta);

                        // TODO: Need to refresh the scroll bar value, else the HitTest method return the visible line as null 
                        //when the scroll bar position at last column and when we resize the hidden column. 
                        //(Because the scroll bar maximum value was updated based on the Distances.TotalDistance but the value was not updated)
                        if (host.ScrollColumns.ScrollBar.Maximum - host.ScrollColumns.ScrollBar.LargeChange + 1 <= host.ScrollColumns.ScrollBar.Value)
                            host.ScrollColumns.ScrollToNextLine();

                        dragLine = HitTest(e.Location); // Due to this call "inHiddenColResize" will get reset. So care must be taken in subsequent calling.
                        processed = true;
                    }

                    if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(dragLine.LineIndex), ref width, GridResizeCellsReason.MouseMove, e.Location, true, processed))
                    {
                        if (!processed) // Dont use "inHiddenColResize" condition check here since the hidden col resizing is no longer happening.
                        {
                            if (width == 0 && this.host.Model.Options.AllowExcelLikeResizing)
                                host.Model.ColumnWidths.SetHidden(dragLine.LineIndex, dragLine.LineIndex, true);
                            else
                            {
                                if (width > 0 && host.Model.ColumnWidths.GetHidden(dragLine.LineIndex, out repeatCount))
                                    host.Model.ColumnWidths.SetHidden(dragLine.LineIndex, dragLine.LineIndex, false);
                                scrollColumns.SetLineResize(dragLine.LineIndex, width);
                            }
                        }
#if SILVERLIGHT
                        InvalidateAllParents(host.Model as IGridDataChildModelInteractivity);
#endif
                    }
                }
            }
        }

#if SILVERLIGHT
        private void InvalidateAllParents(IGridDataChildModelInteractivity model)
        {
            if (model == null)
            {
                return;
            }

            var parentModel = model.GetParentModel();
            if (parentModel != null)
            {
                parentModel.InvalidateVisual(true);
            }

            if (parentModel != null && parentModel is IGridDataChildModelInteractivity)
            {
                this.InvalidateAllParents(parentModel as IGridDataChildModelInteractivity);
            }
        }
#endif

        public void MouseUp(MouseControllerEventArgs e)
        {
            //// Sample: enable mouse tracking on click-release.
            //if (!host.MouseControllerDispatcher.IsMouseTracking 
            //    && viewerUtil.GetSurroundingRect(host.MouseControllerDispatcher.MouseDownLocation, new Size(2, 2)).Contains(e.Location))
            //{
            //    host.MouseControllerDispatcher.StartTrackMouse();
            //    return;
            //}
            if (dragLine == null)
            {
                return;
            }

            Point point = e.Location;
            double delta = point.X - dragLine.Corner;
            double width = Math.Max(0, dragLine.Size + delta);
            var args = new GridResizingColumnsEventArgs(this.host)
            {
                AllowResize = true,
                Columns = GridRangeInfo.Col(dragLine.LineIndex),
                Width = width,
                Reason = GridResizeCellsReason.MouseUp,
                Point = e.Location
            };
            //if (host.RaiseResizingColumnsEvent(GridRangeInfo.Col(dragLine.LineIndex), ref width, GridResizeCellsReason.MouseUp, e.Location))
            if (!inHiddenColResize && host.RaiseResizingColumnsEvent(args))
            {
                host.SetColumnWidth(dragLine.LineIndex, Math.Max(0, width));
            }
            else if (!inHiddenColResize && args.Handled)
            {
                host.SetColumnWidth(dragLine.LineIndex, Math.Max(0, args.Width));
            }
            //#if SILVERLIGHT
            //            else
            //            {
            //                host.Model.ColumnWidths.SetRange(dragLine.LineIndex, dragLine.LineIndex, host.Model.ColumnWidths.DefaultLineSize);
            //            }
            //#endif
            host.ScrollColumns.ResetLineResize();
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

        public void CancelMode()
        {
            host.InvalidateVisual();
            // Lazy way: Don't reset dragLine - then RestoreMode will work just fine.
            //dragLine = null;
        }

        VisibleLineInfo hitLine;
        double edgeWidth = 5;
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            Point point = mouseEventArgs.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            /*VisibleLineInfo hit = HitTest(point);
            if (hit != null)
            {
                return 1;
            }*/

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
                    if (row != null)
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
            host.InvalidateVisual();
            host.UpdateLayout();
        }

        #endregion

#if WinRT
        public void MouseHoverEnter(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }

        public void MouseHoverLeave(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }
#endif
    }
}
