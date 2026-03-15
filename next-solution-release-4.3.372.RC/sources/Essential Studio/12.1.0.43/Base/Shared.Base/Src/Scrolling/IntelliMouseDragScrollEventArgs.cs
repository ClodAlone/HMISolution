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
using System.ComponentModel;

namespace Syncfusion.Windows.Forms
{

	/// <summary>
	/// Handles the DragScroll event of an IntelliMouseDragScroll object.
	/// </summary>
	public delegate void IntelliMouseDragScrollEventHandler(object sender, IntelliMouseDragScrollEventArgs e);

	/// <summary>
	/// Provides data for the DragScroll event of an IntelliMouseDragScroll object.
	/// </summary>
	public class IntelliMouseDragScrollEventArgs : CancelEventArgs
	{
		int dx;
		int dy;
		bool scrolled = false;

		/// <summary>
		/// Initializes a new instance of the IntelliMouseDragScrollEventArgs class.
		/// </summary>
		/// <param name="dx">The distance in pixels the mouse pointer has been moved horizontally.</param>
		/// <param name="dy">The distance in pixels the mouse pointer has been moved vertically.</param>
		public IntelliMouseDragScrollEventArgs(int dx, int dy)
		{
			this.dx = dx;
			this.dy = dy;
		}

		/// <summary>
		/// Gets / sets the distance in pixels the mouse pointer has been moved horizontally.
		/// </summary>
		public int DX
		{
			get
			{
				return dx;
			}
			set
			{
				dx = value;
			}
		}

		/// <summary>
		/// Gets / sets the distance in pixels the mouse pointer has been moved vertically.
		/// </summary>
		public int DY
		{
			get
			{
				return dy;
			}
			set
			{
				dy = value;
			}
		}

		/// <summary>
		/// Set this to True if you scrolled and do not want default scrolling behavior.
		/// </summary>
		public bool Scrolled
		{
			get
			{
				return this.scrolled;
			}
			set
			{
				this.scrolled = value;
			}
		}
	}

}
