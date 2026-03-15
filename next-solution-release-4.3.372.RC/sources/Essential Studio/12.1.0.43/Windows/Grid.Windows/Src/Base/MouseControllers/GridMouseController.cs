//-------------------------------------------------------------------------------------------------
// <copyright file="GridMouseController.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A base class for MouseControllers in the grid that implements the IMouseController interface
    /// to be used with MouseControllerDispatcher.
    /// </summary>
    /// <remarks>
    /// Any Mouse Controller needs to implement the IMouseController interface.<para/>
    /// In its implementation of MouseController.HitTest, the mouse controller should determine whether your
    /// controller wants to handle the mouse events based current context.<para/>
    /// See MouseControllerDispatcher for further discussion.
    /// </remarks>
    public abstract class GridMouseController : GridSubComponent, IMouseController, IGridFocusHelper
    {
        /// <summary>
        /// Initializes a new <see cref="GridMouseController"/> and attaches it to a grid.
        /// </summary>
        /// <param name="grid">The grid control</param>
        protected GridMouseController(GridControlBase grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Gets the name of this mouse controller.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the cursor to be displayed.
        /// </summary>
        public abstract Cursor Cursor { get; }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        public abstract void MouseHoverEnter();

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public abstract void MouseHover(MouseEventArgs e);

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> holding event data.</param>
        public abstract void MouseHoverLeave(EventArgs e);

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public abstract void MouseDown(MouseEventArgs e);

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public abstract void MouseMove(MouseEventArgs e);

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public abstract void MouseUp(MouseEventArgs e);

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public abstract void CancelMode();

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> holding event data..</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public abstract int HitTest(MouseEventArgs mouseEventArgs, IMouseController controller);

        /// <summary>
        /// Override this method in your <see cref="GridMouseController"/> and return False if it would interfere with your 
        /// controller's state when the current cell would be focused and possibly scrolled into view.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> (True by default) that indicates if the grid is allowed to set the focus onto the current cells <see cref="Control"/>.
        /// </returns>
        public virtual bool GetAllowFixFocus()
        {
            return true;
        }
    }
}
