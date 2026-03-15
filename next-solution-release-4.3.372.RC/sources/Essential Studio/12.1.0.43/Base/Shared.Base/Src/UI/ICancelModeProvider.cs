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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// ICancelModeProvider provides an interface for the CancelMode event.
	/// </summary>
	public interface ICancelModeProvider
	{
		/// <summary>
		/// Occurs when the window receives a WM_CANCELMODE message.
		/// </summary>
		/// <remarks>
		/// WM_CANCELMODE is sent to cancel certain modes, such as mouse capture. 
		/// For example, the system sends this message to the active window when a 
		/// dialog box or message box is displayed. Certain functions also send this 
		/// message explicitly to the specified window regardless of whether it is the 
		/// active window. For example, the EnableWindow function sends this message 
		/// when disabling the specified window. 
		/// </remarks>
		event EventHandler CancelMode;
	}
}
