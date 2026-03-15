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
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;

namespace Syncfusion.Windows.Grid.Olap
{
    /// <summary>
    /// GridCell Mouse cursor controller
    /// </summary>
    internal class GridCellMouseCursorController : IMouseController
    {
        public const string GridCellMouseCursorControllerName = "GridCellMouseCursorController";

        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellMouseCursorController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridCellMouseCursorController(OlapGridBase grid)
        {
            this.Grid = grid;
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public OlapGridBase Grid
        {
            get;
            set;
        }

        #region IMouseController Members

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        string IMouseController.Name
        {
            get
            {
                return GridCellMouseCursorController.GridCellMouseCursorControllerName;
            }
        }

        /// <summary>
        /// Returns the cursor to be displayed.
        /// </summary>
        /// <value></value>
        System.Windows.Input.Cursor IMouseController.Cursor
        {
            get
            {
                return this.Grid.RaiseGridCellCursor();
            }
        }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the MouseHover is called for the first time.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHoverEnter(System.Windows.Input.MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHover(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHoverLeave(System.Windows.Input.MouseEventArgs e)
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
        void IMouseController.MouseDown(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseMove(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseUp(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        void IMouseController.CancelMode()
        {
        }

        /// <summary>
        /// RestoreMode is called when a controller should be reactivated. Prevoius state can be restored if it was backed up earlier when CancelMode was called.
        /// </summary>
        void IMouseController.RestoreMode()
        {
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
        int IMouseController.HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (this.Grid.IsProcessing)
            {
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
        bool IMouseController.SupportsCancelMouseCapture
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
        bool IMouseController.SupportsMouseTracking
        {
            get { return false; }
        }

        #endregion
    }
}
