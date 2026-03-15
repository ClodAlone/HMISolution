#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.ComponentModel;
using System.Windows.Interop;


namespace Syncfusion.Windows.Controls.Grid
{
    class GridDataNestedSelectCellsMouseController : GridSelectCellsMouseController
    {
        GridControlBase grid;

        public GridDataNestedSelectCellsMouseController(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        public override void MouseDown(MouseControllerEventArgs e)
        {
            /// Getting mouse position
            var start = grid.PointToCellRowColumnIndex(e.Location, false);

#if !SILVERLIGHT
            /// Getting style
            //GridRenderStyleInfo style = grid.GetRenderStyleInfo(start);
            //IGridCellRenderer renderer = style.CellRenderer;

            if (e.DirectlyOverRenderer is GridCellHyperlinkCellRenderer || e.DirectlyOverRenderer is GridCellCheckboxRenderer || e.DirectlyOverRenderer is GridCellDataBoundTemplateRenderer)
            {
                this.MouseControllerDispatcher.CanHandleMouseDown = false;
            }
            else
            {
                this.MouseControllerDispatcher.CanHandleMouseDown = true;
            }
#endif
            if (CurrentCell.RowIndex != -1 && CurrentCell.ColumnIndex != -1)
            {
                if (Model[CurrentCell.RowIndex, CurrentCell.ColumnIndex].CellType == "NestedGrid" && e.ClickCount == 2)
                {
                    return;
                }
            }

            /// To Avoid selection on Details View Cell
            if (start.RowIndex > -1 && start.ColumnIndex > -1 && this.Model[start.RowIndex, start.ColumnIndex].CellType == "GridDataBoundTemplate")
            {
                if ((this.Model[start.RowIndex, start.ColumnIndex].CellIdentity as GridDataTableStyleInfoIdentity).TableCellType == GridDataTableCellType.DetailsViewCell)
                {
                    return;
                }
            }

            base.MouseDown(e);
        }

#if !SILVERLIGHT

        protected override bool IsHeaderRow(Syncfusion.Windows.Controls.Cells.RowColumnIndex end)
        {
            if ((this.Model as GridDataTableModel).TableProperties.StackedHeaderRows.Count >= end.RowIndex)
                return true;
            else
                return false;
        }

#endif

        //public override void MouseMove(MouseControllerEventArgs e)
        //{
        //    base.MouseMove(e);
        //}
    }
}
