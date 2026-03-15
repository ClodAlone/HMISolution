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
using System.Windows.Forms;

using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides data for a cancelable mouse event.
	/// </summary>
	public class CancelMouseEventArgs : SyncfusionCancelEventArgs
	{
		MouseEventArgs mea;

		/// <summary>
		/// Initializes a new <see cref="CancelMouseEventArgs"/> with data from a <see cref="MouseEventArgs"/>.
		/// </summary>
		/// <param name="e">The <see cref="MouseEventArgs"/> data for this event.</param>
		public CancelMouseEventArgs(MouseEventArgs e)
		{
			mea = e;
		}

		/// <summary>
		/// The <see cref="MouseEventArgs"/> data for this event.
		/// </summary>
		public MouseEventArgs MouseEventArgs
		{
			get
			{
				return mea;
			}
		}

		/// <override/>
		public override string ToString()
		{
			return string.Concat(
				GetType().Name, 
				" { X=", mea.X,
				", Y=", mea.Y,
				", Button=", mea.Button,
				", Clicks=", mea.Clicks,
				" }");
		}
	}

	/// <summary>
	/// Handles a cancelable mouse event.
	/// </summary>
	public delegate void CancelMouseEventHandler(object sender, CancelMouseEventArgs e);

}
