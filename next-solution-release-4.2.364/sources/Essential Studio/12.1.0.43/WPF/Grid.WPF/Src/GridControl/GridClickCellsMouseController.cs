#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Grid
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using System.Windows.Shapes;

    public class GridClickCellsMouseController : IMouseController
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class ClickCellsHitTestInfo
        {
            internal int hitTestResult = 0;
            internal Point point = new Point();
            internal Rect cellBounds = Rect.Empty;
            internal RowColumnIndex rowColumnIndex = RowColumnIndex.Empty;
            internal IGridCellRenderer cellRenderer = null;

            internal ClickCellsHitTestInfo(GridControlBase grid, Point point, IMouseController controller)
            {
                cellRenderer = null;
                hitTestResult = 0;
                this.rowColumnIndex = grid.PointToCellRowColumnIndex(point);
                if (this.rowColumnIndex != RowColumnIndex.Empty)
                {
                    var ci = grid.GetCoveredCell(this.rowColumnIndex);
                    if (ci != null)
                    {
                        this.rowColumnIndex.RowIndex = ci.Top;
                        this.rowColumnIndex.ColumnIndex = ci.Left;
                    }

                    var style = grid.GetRenderStyleInfo(this.rowColumnIndex);
                    this.cellRenderer = style.CellRenderer;
                    this.hitTestResult = 1;
                }
            }
        }

        public GridClickCellsMouseController(GridControlBase grid)
        {
            this.Grid = grid;
        }

        public GridControlBase Grid
        {
            get;
            private set;
        }

        #region IMouseController Members

        public string Name
        {
            get { return "ClickCellsMouseController"; }
        }

        public virtual Cursor Cursor
        {
            get
            {
                return this.Grid.RaiseGridCellCursor();
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
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
        }

        public void CancelMode()
        {
        }

        public void RestoreMode()
        {
        }

        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            return 0;
        }

        public bool SupportsCancelMouseCapture
        {
            get { return true; }
        }

        public bool SupportsMouseTracking
        {
            get { return true; }
        }

        #endregion

    }

}

