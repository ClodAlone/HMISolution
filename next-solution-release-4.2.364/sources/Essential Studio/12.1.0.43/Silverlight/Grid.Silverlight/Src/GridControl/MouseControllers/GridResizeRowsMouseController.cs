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
#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.Controls.Grid;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.UI.Core;

namespace Syncfusion.WinRT.Controls.Cells
{
#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    class GridResizeRowsMouseController : IMouseController
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4.0;
        double hitTestHiddenRowPrecision = 6.0;
        bool inHiddenRowResize = false;
        protected GridControlBase host;
        IEditableLineSizeHost rowHeights;
        protected VisibleLineInfo dragLine = null;
        bool isResized = false;
        // bool isDoubleClick = false;

        public GridResizeRowsMouseController(GridControlBase viewer)
        {
            this.host = viewer;
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
                        info = lineInfo;
                    }
                }
            }
            return info;
        }

        #region IMouseController Members

        public string Name
        {
            get { return "GridResizeRowsMouseController"; }
        }
#if !WinRT
        public Cursor Cursor
        {
            get
            {
                if (inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
                    return Cursors.SizeNESW;
                else
                    return Cursors.SizeNS;
            }
        }
#else
        public CoreCursor Cursor
        {
            get
            {
                if (inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
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

        public virtual void MouseDown(MouseControllerEventArgs e)
        {
            if (!this.host.Model.TableStyle.AllowRowResize)
            {
                return;
            }
            Point point = e.Location;

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
                dragLine = null;
            }

            isResized = false;
            //isDoubleClick = dragLine != null && e.ClickCount == 2;
            rowHeights = host.RowHeightsProvider as IEditableLineSizeHost;
        }

        protected virtual int GetLineIndex(int rowIndex)
        {
            return rowIndex;
        }


        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;

            if (dragLine != null)
            {
                double delta = point.Y - dragLine.Corner;
                if (isResized || Math.Abs(delta) > 0.5)
                {
                    isResized = true;
                    double height = Math.Max(0, dragLine.Size + delta);

                    int repeatCount;
                    var isHidden = host.Model.RowHeights.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                    var processed = false;

                    // Hidden Row Rezing Block

                    if (isHidden && inHiddenRowResize && this.host.Model.Options.AllowExcelLikeResizing)
                    {
                        var hiddenLineIndex = dragLine.LineIndex + repeatCount;
                        // Set the line size before and after updating hidden state to update the ViewSize else resizing the last hidden row will be unsuccessful
                        host.ScrollRows.SetLineResize(hiddenLineIndex, delta);
                        host.RowHeights.SetHidden(hiddenLineIndex, hiddenLineIndex, false);
                        host.ScrollRows.SetLineResize(hiddenLineIndex, delta);

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
                                host.ScrollRows.SetLineResize(dragLine.LineIndex, height);
                            }
                        }
                    }
                }
            }
        }

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
                return;

            Point point = e.Location;
            double delta = point.Y - dragLine.Corner;
            double height = Math.Max(0, dragLine.Size + delta);
            var args = new GridResizingRowsEventArgs(this.host)
            {
                AllowResize = true,
                Rows = GridRangeInfo.Row(dragLine.LineIndex),
                Height = height,
                Reason = GridResizeCellsReason.MouseUp,
                Point = e.Location
            };
            if (height != 0 && !inHiddenRowResize && host.RaiseResizingRowsEvent(args))
            {
                //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                host.SetRowHeight(dragLine.LineIndex, Math.Max(0, dragLine.Size + delta));
            }
            else if (height != 0 && !inHiddenRowResize && args.Handled)
            {
                host.SetRowHeight(dragLine.LineIndex, Math.Max(0, args.Height));
            }
            //#if SILVERLIGHT
            //            else
            //            {
            //                host.Model.RowHeights.SetRange(dragLine.LineIndex, dragLine.LineIndex, host.Model.RowHeights.DefaultLineSize);
            //            }
            //#endif
            host.ScrollRows.ResetLineResize();
            dragLine = null;
        }

        public void CancelMode()
        {
            host.InvalidateVisual(true);

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
            Point point = mouseEventArgs.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            VisibleLineInfo hit = HitTest(point);
            RowColumnIndex pos = host.PointToCellRowColumnIndex(point, true);
            CoveredCellInfo cc = host.GetCoveredCell(pos);
            /*if (hit != null)
            {
                return 1;
            }*/

            if (hit != null && !this.IsNotNested(hit))
            {
                if (cc == null || cc.Top - 1 == hit.LineIndex || cc.Bottom == hit.LineIndex)
                {
                    VisibleLineInfo column = scrollColumns.GetVisibleLineAtPoint(point.X);
                    if (column != null)
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
