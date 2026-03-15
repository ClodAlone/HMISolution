#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <exclude/>
    class GridTreeResizeRowsMouseController : IMouseController
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4;
        GridControlBase host;
        VisibleLineInfo dragLine = null;
        internal static bool inSizing = false;

        public GridTreeResizeRowsMouseController(GridControlBase grid)
        {
            this.host = grid;

#if !SILVERLIGHT
            _cursor = CellCursors.ResizeHeightCursor;
#else
                _cursor=Cursors.SizeNS;
#endif
        }

        private VisibleLineInfo HitTest(Point point)
        {
            return scrollRows.GetLineNearCorner(point.Y, hitTestPrecision);
        }

        #region IMouseController Members

        public string Name
        {
            get { return "ResizeInsideGridRowsMouseController"; }
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

        public void MouseDown(MouseControllerEventArgs e)
        {
            Point point = e.Location;

            dragLine = HitTest(point);

            if (dragLine != null && e.ClickCount == 2)
            {
                int l = dragLine.LineIndex + 1;
                int rc;
                if (host.Model.RowHeights.GetSize(l, out rc) == 0)
                    host.Model.RowHeights.SetRange(l, l + rc - 1, host.Model.RowHeights.DefaultLineSize);
                else if (host.Model.RowHeights.GetHidden(l, out rc))
                    host.Model.RowHeights.SetHidden(l, l + rc - 1, false);
                else
                    host.Model.RowHeights.SetRange(dragLine.LineIndex, dragLine.LineIndex, host.Model.RowHeights.DefaultLineSize);
                dragLine = null;
                inSizing = false;
                host.InvalidateCells();
            }
            else if(dragLine != null)
            {
                inSizing = true;
                host.InvalidateCells();
            }

        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;

            if (dragLine != null)
            {
                double delta = point.Y - dragLine.Corner;
                 //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                host.ScrollRows.SetLineResize(dragLine.LineIndex, Math.Max(0, dragLine.Size + delta));
            }
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            inSizing = false;

            if (dragLine == null) return;

            Point point = e.Location;
            double delta = point.Y - dragLine.Corner;
            host.SetRowHeight(dragLine.LineIndex, Math.Max(0, dragLine.Size + delta));
            host.ScrollRows.ResetLineResize();
            dragLine = null;
        }

        public void CancelMode()
        {
            inSizing = false;
            if (dragLine != null)
            {
                host.ScrollRows.ResetLineResize();
            }
        }

        DateTime lastTime = DateTime.MinValue;
        DateTime nowTime;
        double requiredTimeSpanMSecs = 50;
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            nowTime = DateTime.Now;
            Point point = mouseEventArgs.Location;
            VisibleLineInfo hit = HitTest(point);
            if (hit != null)
            {
                VisibleLineInfo column = scrollColumns.GetVisibleLineAtPoint(point.X);
                if (column != null && column.LineIndex < host.Model.HeaderRows  && point.X < this.host.ColumnWidths.TotalExtent
                    && nowTime.Subtract(lastTime).TotalMilliseconds > requiredTimeSpanMSecs)
                {
                    return 1;
                }
            }
            lastTime = nowTime;
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
        }

        #endregion
    }
}
