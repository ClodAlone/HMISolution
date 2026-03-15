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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms
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
	public interface IMouseController
	{
		/// <summary>
		/// Returns the name of this mouse controller.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns the cursor to be displayed.
		/// </summary>
		Cursor Cursor { get; }

		/// <summary>
		/// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
		/// is called before the MouseHover is called for the first time.
		/// </summary>
		void MouseHoverEnter();

		/// <summary>
		/// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
		/// is called after MouseHoverEnter.
		/// </summary>
		void MouseHover(MouseEventArgs e);

		/// <summary>
		/// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
		/// when context changes (e.g. user pressed the mouse button).
		/// </summary>
		void MouseHoverLeave(EventArgs e);

		/// <summary>
		/// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
		/// user pressed the mouse button.
		/// </summary>
		/// <remarks>
		/// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
		/// until the mouse button is released or the mouse operation is cancelled.
		/// </remarks>
		/// <param name="e"></param>
		void MouseDown(MouseEventArgs e);

		/// <summary>
		/// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
		/// </summary>
		/// <param name="e"></param>
		void MouseMove(MouseEventArgs e);

		/// <summary>
		/// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
		/// </summary>
		/// <param name="e"></param>
		void MouseUp(MouseEventArgs e);

		/// <summary>
		/// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
		/// </summary>
		void CancelMode();

		/// <summary>
		/// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
		/// </summary>
		/// <remarks>
		/// The current winner of the vote is specified through the controller parameter. Your implementation of HitTest
		/// can decide if it wants to override the existing vote or leave it.
		/// </remarks>
		/// <param name="mouseEventArgs"></param>
		/// <param name="controller"></param>
		/// <returns></returns>
		int HitTest(MouseEventArgs mouseEventArgs, IMouseController controller);
	}

}
