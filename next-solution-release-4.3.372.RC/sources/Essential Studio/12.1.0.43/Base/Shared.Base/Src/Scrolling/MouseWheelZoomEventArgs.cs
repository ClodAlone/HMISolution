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
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Handles the MouseWheelZoom events.
	/// </summary>
	public delegate void MouseWheelZoomEventHandler(object sender, MouseWheelZoomEventArgs e);

	/// <summary>
	/// Provides data for the MouseWheelZoom event.
	/// </summary>
	public class MouseWheelZoomEventArgs : SyncfusionEventArgs
	{
		int delta;


		/// <summary>
		/// Initializes a <see cref="MouseWheelZoomEventArgs"/> with a given delta.
		/// </summary>
		/// <param name="delta">The number of rows or columns to scroll.</param>
		public MouseWheelZoomEventArgs(int delta)
		{
			this.delta = delta;
		}

		/// <summary>
		/// Returns the number of rows or columns to scroll.
		/// </summary>
		[TraceProperty(true)]
		public int Delta
		{
			get
			{
				return delta;
			}
		}
	}

}
