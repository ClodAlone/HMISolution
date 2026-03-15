#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion
#if !WinRT
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Scroll
#else
using System;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// IMouseController defines the interface for mouse controllers to be used with MouseControllerDispatcher.
    /// </summary>
    /// <remarks>
    /// Any mouse controller needs to implement the IMouseController interface.<para/>
    /// In its implementation of MouseController.HitTest, the mouse controller should determine whether your
    /// controller wants to handle the mouse events based current context.<para/>
    /// See MouseControllerDispatcher for further discussion.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IMouseController
    {
        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        string Name { get; }

#if !WinRT
		/// <summary>
		/// Returns the cursor to be displayed.
		/// </summary>
		Cursor Cursor { get; }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the MouseHover is called for the first time.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>\
        void MouseHoverEnter(MouseEventArgs e);
#else
        void MouseHoverEnter(PointerRoutedEventArgs e);

        CoreCursor Cursor { get; }
#endif

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void MouseHover(MouseControllerEventArgs e);

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
#if !WinRT
        void MouseHoverLeave(MouseEventArgs e);
#else
        void MouseHoverLeave(PointerRoutedEventArgs e);
#endif
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        void MouseDown(MouseControllerEventArgs e);

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void MouseMove(MouseControllerEventArgs e);

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void MouseUp(MouseControllerEventArgs e);

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        void CancelMode();

        /// <summary>
        /// RestoreMode is called when a controller should be reactivated. Prevoius state can be restored if it was backed up earlier when CancelMode was called.
        /// </summary>
        void RestoreMode();

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>A value not equal to 0 indicates that the controller wants to handle the mouse input; otherwise if equal to 0 the controller is not handling it.</returns>
        /// <remarks>
        /// The current winner of the vote is specified through the controller parameter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller);

        /// <summary>
        /// Gets a value indicating whether this controller supports the cancel mouse capture feature and the context of the mouse operation can be changed
        /// while the user drags the pressed mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if supports the cancel mouse capture feature; otherwise, <c>false</c>.
        /// </value>
        bool SupportsCancelMouseCapture
        {
            get;
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
        bool SupportsMouseTracking
        {
            get;
        }
    }

}
