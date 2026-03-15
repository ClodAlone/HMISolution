#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{

    /// <summary>
    /// VirtualizingCellsControl instantiates this MouseControllerDispatcher class which adds
    /// support for changing context of a mouse operation when pressing mouse inside a textbox
    /// or other UIElement and then switching to a cell selection mode when moving mouse outside textbox.
    /// </summary>
    public class CellMouseControllerDispatcher : MouseControllerDispatcher
    {
        VirtualizingCellsControl cellsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellMouseControllerDispatcher"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public CellMouseControllerDispatcher(VirtualizingCellsControl owner)
            : base(owner)
        {
            this.cellsControl = owner;
        }

        /// <summary>
        /// Creates the capture info (<see cref="CellMouseCaptureInfo"/>) when the mouse pressed
        /// inside a child UIElement.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected override ICaptureContext CreateCaptureInfo(Point point)
        {
            return new CellMouseCaptureInfo(cellsControl, Mouse.PrimaryDevice.Captured as UIElement, point);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mouse captured by child element.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse captured by child element; otherwise, <c>false</c>.
        /// </value>
        public override bool IsMouseCapturedByChildElement
        {
            get
            {
                if (ActiveController != null)
                    return false;

                Visual captured = Mouse.Captured as Visual;
                return captured != null && VirtualizingCellsControl.GetCellsControl(captured) == Owner;
            }
        }
        //public new CellMouseCaptureInfo CancelCaptureInfo
        //{
        //    get
        //    {
        //        return (CellMouseCaptureInfo)base.CancelCaptureInfo;
        //    }
        //}

        /// <summary>
        /// Determines whether the specified cell is the same cell used for creating the
        /// <see cref="MouseControllerDispatcher.CancelCaptureInfo"/>.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cells row column.</param>
        /// <returns>
        /// 	<c>true</c> if the specified cell is the same cell used for creating the
        /// <see cref="MouseControllerDispatcher.CancelCaptureInfo"/>; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsMouseOperationOrigin(RowColumnIndex cellRowColumnIndex)
        {
            CellMouseCaptureInfo ci = CancelCaptureInfo as CellMouseCaptureInfo;
            return ci != null && cellRowColumnIndex == ci.CellRowColumnIndex;
        }

    }

    /// <summary>
    /// Holds information about the cells row and column index, the parent cells control,
    /// the renderer and the offset to the top-left of the cell with the UIElement
    /// that the mouse was pressed in.
    /// </summary>
    class CellMouseCaptureInfo : ICaptureContext
    {
        VirtualizingCellsControl cellsControl;
        UIElement element = null;
        RowColumnIndex cellRowColumnIndex;
        ICellRenderer renderer = null;
        Vector offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellMouseCaptureInfo"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="element">The element.</param>
        /// <param name="pt">The point where the mouse was pressed.</param>
        public CellMouseCaptureInfo(VirtualizingCellsControl owner, UIElement element, Point pt)
        {
            this.cellsControl = owner;
            if (owner == element)
            {
                var grid = owner as VirtualizingCellsControl;
                this.cellRowColumnIndex = owner.PointToCellRowColumnIndexOutsideCells(Mouse.GetPosition(grid), true);
                var cellInfo = grid.GetRenderCellInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
                this.renderer = grid.GetCellRenderer(cellInfo);
            }
            else if (VirtualizingCellsControl.GetCellsControl(element) == owner)
            {
                this.cellRowColumnIndex = VirtualizingCellsControl.GetCellRowColumnIndex(element);
                this.renderer = VirtualizingCellsControl.GetCellRenderer(element);
            }
            else
            {
                this.cellRowColumnIndex = RowColumnIndex.Empty;
                this.renderer = null;
            }
            this.element = element;
            Rect bounds = VisualContainer.GetRenderBounds(element);
            if (bounds.IsEmpty)
                this.offset = new Vector(pt.X, pt.Y);
            else
                this.offset = new Vector(pt.X - bounds.X, pt.Y - bounds.Y);
        }

        /// <summary>
        /// Gets the index of the cell row column.
        /// </summary>
        /// <value>The index of the cell row column.</value>
        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        #region ICaptureContext Members

        /// <summary>
        /// Determines if the points belongs to the same cell that mouse is over.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns></returns>
        public bool PointInContext(Point mousePosition)
        {
            RowColumnIndex mouseOverCell = cellsControl.PointToCellRowColumnIndex(mousePosition, true);
            //Trace.WriteLine(String.Format("{0} == {1}", cellRowColumnIndex, mouseOverCellPosition));
            if (cellRowColumnIndex == mouseOverCell)
                return true;
            return false;
        }

        /// <summary>
        /// Recaptures the mouse if the capture was taken away earlier because the user
        /// moved the mouse away from the UIElement and now has moved the mouse back into
        /// the UIElement.
        /// </summary>
        /// <returns></returns>
        public Point RecaptureMouse()
        {
            cellsControl.MouseControllerDispatcher.SuspendMouse();
            try
            {
                renderer.RecaptureMouse(element);
                Rect bounds = VisualContainer.GetRenderBounds(element);
                return new Point(bounds.X + offset.X, bounds.Y + offset.Y);
            }
            finally
            {
                cellsControl.MouseControllerDispatcher.ResumeMouse();
            }
        }

        /// <summary>
        /// Cancels the mouse capture.
        /// </summary>
        /// <returns></returns>
        public bool CancelMouseCapture()
        {
            if (cellsControl != null && cellsControl.MouseControllerDispatcher != null)
            {
                cellsControl.MouseControllerDispatcher.SuspendMouse();
            }

            try
            {
                if (renderer != null)
                    renderer.CancelMouseCapture(element);
                if (Mouse.Captured != null)
                    return false;

                return true;
            }
            finally
            {
                if (cellsControl != null && cellsControl.MouseControllerDispatcher != null)
                {
                    cellsControl.MouseControllerDispatcher.ResumeMouse();
                }
            }
        }
        #endregion
    }


}
