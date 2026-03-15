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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region WndHook
	abstract class WndHook : IDisposable
	{
		#region Constructor
		public WndHook(WindowsAPI.WindowProc wndProc)
		{
			m_wndProc = wndProc;
			m_hookProc = new WindowsAPI.HookProc(CallWndProc);
		}
		#endregion

		#region Implementation
		private IntPtr CallWndProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			CallWndProcInternal(nCode, wParam, lParam);
			return WindowsAPI.CallNextHookEx(m_hHook, nCode, wParam, lParam);
		}
		#endregion

		#region Overrides
		protected abstract void CallWndProcInternal(int nCode, IntPtr wParam, IntPtr lParam);
		#endregion

		#region IDisposable Members
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (m_hHook != IntPtr.Zero)
			{
				WindowsAPI.UnhookWindowsHookEx(m_hHook);
			}
		}
		#endregion

		#region Fields
		protected WindowsAPI.WindowProc m_wndProc;
		protected WindowsAPI.HookProc m_hookProc;
		protected IntPtr m_hHook;
		#endregion
	}
	#endregion

	#region CallWndProcHook
	class CallWndProcHook : WndHook
	{
		#region Constructor
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="wndProc"></param>
		public CallWndProcHook(WindowsAPI.WindowProc wndProc) : base(wndProc)
		{
			if (m_wndProc != null)
			{
				m_hHook = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_CALLWNDPROC, m_hookProc, IntPtr.Zero, WindowsAPI.GetCurrentThreadId());
			}
		}
		#endregion

		#region Overrides
		protected override void CallWndProcInternal(int nCode, IntPtr wParam, IntPtr lParam)
		{
			CWPSTRUCT cwp = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
			m_wndProc(cwp.hwnd, cwp.message, cwp.wparam, cwp.lparam);
		}
		#endregion
	}
	#endregion

	#region GetMessageHook
	class GetMessageHook : WndHook
	{
		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wndProc"></param>
		public GetMessageHook(WindowsAPI.WindowProc wndProc) : base(wndProc)
		{
			if (m_wndProc != null)
			{
				m_hHook = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_GETMESSAGE, m_hookProc, IntPtr.Zero, WindowsAPI.GetCurrentThreadId());
			}
		}
		#endregion

		#region Overrides
		protected override void CallWndProcInternal(int nCode, IntPtr wParam, IntPtr lParam)
		{
			MSG msg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));
			m_wndProc(msg.hwnd, msg.message, msg.wParam, msg.lParam);
		}
		#endregion
	}
	#endregion
}
#endif