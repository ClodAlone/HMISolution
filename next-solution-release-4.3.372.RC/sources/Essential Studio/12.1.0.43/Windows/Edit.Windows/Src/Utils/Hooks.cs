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
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Edit.Utils;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	#region WndHook
	/// <summary>
	/// Wrapper over native hook.
	/// </summary>
	abstract class WndHook
		: IDisposable
	{
		#region Constructor
		/// <summary>
		/// Creates new WndHook.
		/// </summary>
		/// <param name="wndProc">Hook procedure.</param>
		public WndHook( Callback.WindowProc wndProc )
		{
			m_wndProc = wndProc;
			m_hookProc = new WinAPI.HookProc( CallWndProc );
		}
		#endregion

		#region Implementation
		[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
		private IntPtr CallWndProc( int nCode, IntPtr wParam, IntPtr lParam )
		{
			CallWndProcInternal( nCode, wParam, lParam );
			return WinAPI.CallNextHookEx( m_hHook, nCode, wParam, lParam );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		protected abstract void CallWndProcInternal( int nCode, IntPtr wParam, IntPtr lParam );
		#endregion

		#region IDisposable Members
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if( m_hHook != IntPtr.Zero )
			{
				WinAPI.UnhookWindowsHookEx( m_hHook );
			}
		}
		#endregion

		#region Fields
		protected Callback.WindowProc m_wndProc;
		protected WinAPI.HookProc m_hookProc;
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
		public CallWndProcHook( Callback.WindowProc wndProc )
			: base( wndProc )
		{
			if( m_wndProc != null )
			{
				m_hHook = WinAPI.SetWindowsHookEx( ( int )WindowsHookCodes.WH_CALLWNDPROC, m_hookProc, IntPtr.Zero, WinAPI.GetCurrentThreadId() );
			}
		}
		#endregion

		#region Overrides
		protected override void CallWndProcInternal( int nCode, IntPtr wParam, IntPtr lParam )
		{
			CWPSTRUCT cwp = ( CWPSTRUCT )Marshal.PtrToStructure( lParam, typeof( CWPSTRUCT ) );
			m_wndProc( cwp.hwnd, cwp.message, cwp.wparam, cwp.lparam );
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
		public GetMessageHook( Callback.WindowProc wndProc )
			: base( wndProc )
		{
			if( m_wndProc != null )
			{
				m_hHook = WinAPI.SetWindowsHookEx( ( int )WindowsHookCodes.WH_GETMESSAGE, m_hookProc, IntPtr.Zero, WinAPI.GetCurrentThreadId() );
			}
		}
		#endregion

		#region Overrides
		protected override void CallWndProcInternal( int nCode, IntPtr wParam, IntPtr lParam )
		{
			MSG msg = ( MSG )Marshal.PtrToStructure( lParam, typeof( MSG ) );
			m_wndProc( msg.hwnd, msg.message, msg.wParam, msg.lParam );
		}
		#endregion
	}
	#endregion
}