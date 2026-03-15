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
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IMouseHookProcClient
	{
		bool MouseHookProc(int wParam, Point point, int dwExtraInfo); 
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IMouseHookHLProcClient
	{
		bool MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo); 
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IKeyboardProcHookClient
	{
		bool KeyboardHookProc(int wParam, int lParam); 
	}


}
