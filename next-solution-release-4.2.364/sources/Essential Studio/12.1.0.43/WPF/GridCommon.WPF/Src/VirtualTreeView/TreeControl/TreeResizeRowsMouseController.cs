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


namespace Syncfusion.Windows.Controls.VirtualTreeView
{
 
    /// <summary>
    /// Provides support for reszing the height of tree nodes with the mouse.
    /// </summary>
    class TreeResizeRowsMouseController : IMouseController
    {
        ScrollAxisBase scrollRows { get { return host.ScrollRows; } }
        ScrollAxisBase scrollColumns { get { return host.ScrollColumns; } }

        double hitTestPrecision = 4;
        VirtualTreeView host;
        VisibleLineInfo dragLine = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeResizeRowsMouseController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public TreeResizeRowsMouseController(VirtualTreeView grid)
        {
            this.host = grid;
        }

        private VisibleLineInfo HitTest(Point point)
        {
            return scrollRows.GetLineNearCorner(point.Y, hitTestPrecision);
        }

        #region IMouseController Members

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get { return "ResizeRowsMouseController"; }
        }

        /// <summary>
        /// Returns the cursor to be displayed.
        /// </summary>
        /// <value></value>
        public Cursor Cursor
        {
            get { return CellCursors.ResizeHeightCursor; }
        }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the MouseHover is called for the first time.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHoverEnter(MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHoverLeave(MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        public void MouseDown(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            TraceUtil.TraceCurrentMethodInfo(point);

            dragLine = HitTest(point);
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);
            VirtualizingCellsControl cellsControl = host;// as VirtualizingCellsControl;

            if (dragLine != null)
            {
                double delta = point.Y - dragLine.Corner;
                //Console.WriteLine(String.Format("{0} {1}", delta, dragLine));
                host.ScrollRows.SetLineResize(dragLine.LineIndex, Math.Max(0, dragLine.Size + delta));
            }
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseControllerEventArgs e)
        {
            Point point = e.Location;
            double delta = point.Y - dragLine.Corner;
            TreeNode node = host.Model.VisibleNodes[dragLine.LineIndex];
            node.ItemHeight = Math.Max(0, dragLine.Size + delta);
            node.InvalidateCounterBottomUp();
            host.ScrollRows.ResetLineResize();
            dragLine = null;
        }

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public void CancelMode()
        {
            if (dragLine != null)
            {
                host.ScrollRows.ResetLineResize();
            }
            // Lazy way: Don't reset dragLine - then RestoreMode will work just fine.
            //dragLine = null;
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A value not equal to 0 indicates that the controller wants to handle the mouse input; otherwise if equal to 0 the controller is not handling it.
        /// </returns>
        /// <remarks>
        /// The current winner of the vote is specified through the controller parameter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            Point point = mouseEventArgs.Location;
            //TraceUtil.TraceCurrentMethodInfo(point);

            RowColumnIndex pos = host.PointToCellRowColumnIndex(point, true);
            CoveredCellInfo cc = host.GetCoveredCell(pos);
            VisibleLineInfo hit = HitTest(point);
            if (hit != null)
            {
                if (cc == null || cc.Top-1 == hit.LineIndex || cc.Bottom == hit.LineIndex)
                    return 1;
            }
            return 0;
        }

        /// <summary>
        /// Gets a value indicating whether this controller supports the cancel mouse capture feature and the context of the mouse operation can be changed
        /// while the user drags the pressed mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if supports the cancel mouse capture feature; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsCancelMouseCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the controller supports the mouse
        /// tracking feature which allows MouseControllerDispatcher to emulate
        /// a pressed mouse operation similar
        /// to the way a combobox selects the item in a dropped list box while
        /// hovering the mouse over the dropped listbox and simulating
        /// a MouseUp when the user presses the mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the controller supports mouse tracking; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsMouseTracking
        {
            get { return false; }
        }

        /// <summary>
        /// RestoreMode is called when a controller should be reactivated. Prevoius state can be restored if it was backed up earlier when CancelMode was called.
        /// </summary>
        public void RestoreMode()
        {
        }

        #endregion
    }

}
