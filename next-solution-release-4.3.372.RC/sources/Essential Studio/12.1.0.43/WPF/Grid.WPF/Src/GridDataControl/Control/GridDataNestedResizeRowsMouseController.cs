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
    class GridDataNestedResizeRowsMouseController : GridResizeRowsMouseController
    {
        public GridDataNestedResizeRowsMouseController(GridControlBase grid)
            : base(grid)
        {
        }

        protected override int GetLineIndex(int rowIndex)
        {
            var gridModel = this.host.Model as GridDataTableModel;

            if (gridModel != null && gridModel.Table != null && gridModel.Table.HasNestedTables)
            {
                if (rowIndex == gridModel.ResolveAddNewPositionInGrid())
                {
                    return base.GetLineIndex(rowIndex);
                }

                return rowIndex - 1;
            }

            return base.GetLineIndex(rowIndex);
        }



        public override void MouseDown(MouseControllerEventArgs e)
        {
            GridDataTableModel tableModel = host.Model as GridDataTableModel;
            Point point1 = e.Location;
            dragLine = HitTest(point1);
            int l = dragLine.LineIndex;
            int rc, rh;
            if (tableModel.IsInNestedIndex(l + 1)
#if!SILVERLIGHT
               && !base.GetInHiddenRowResizeState()
#endif
)
            {
                if (dragLine != null && e.ClickCount == 2)
                {
                    double height = host.Model.RowHeights.GetSize(l + 1, out rc);
                    bool isHidden = host.Model.RowHeights.GetHidden(l + 1, out rh);
                    if (host.RaiseResizingRowsEvent(GridRangeInfo.Row(l), ref height, GridResizeCellsReason.DoubleClick, e.Location))
                    {
                        if (height == 0)
                            host.Model.RowHeights.SetRange(l, l + rc - 1, host.Model.RowHeights.DefaultLineSize);
                        else if (isHidden)
                            host.Model.RowHeights.SetHidden(l, l + rh - 1, false);
                        else
                            host.Model.RowHeights.SetRange(l, l, host.Model.RowHeights.DefaultLineSize);
                    }
                    dragLine = null;
                }
                else
                    base.MouseDown(e);
            }

            else
            {
                base.MouseDown(e);
            }
        }
    }
}
