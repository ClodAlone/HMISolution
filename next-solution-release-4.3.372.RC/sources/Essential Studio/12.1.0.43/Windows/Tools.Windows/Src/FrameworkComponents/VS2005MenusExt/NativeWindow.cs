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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Win32API;


namespace Syncfusion.Windows.Forms.Tools
{
	#region INativeMessageFilter
	public interface INativeMessageFilter
	{
		bool ProcessMessage(ref Message m);
	}
	#endregion

	#region INativeMessageFilter2
	public interface INativeMessageFilter2 : INativeMessageFilter
	{
		void PostProcessMessage(ref Message m);
	}
	#endregion

	#region NativeMessageHandler
	public sealed class NativeMessageHandler
	{
		#region *** Handlers
		/// <summary>
		/// 
		/// </summary>
		private class Handlers : Dictionary<IntPtr, ArrayList>
		{
			#region Constructor
			public Handlers()
			{
			}
			#endregion

			#region *** Methods
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hWnd"></param>
			/// <param name="handler"></param>
			public void AddHandler(IntPtr hWnd, NativeMessageHandler handler)
			{
				ArrayList list = null;

				if (this.ContainsKey(hWnd))
				{
					list = this[hWnd];
				}

				if (list == null)
				{
					list = new ArrayList();
					this[hWnd] = list;
				}
				
				list.Add(handler);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hWnd"></param>
			public void RemoveHandlers(IntPtr hWnd)
			{
				if (this.ContainsKey(hWnd))
				{
					this[hWnd].Clear();
					this.Remove(hWnd);
				}
			}
			#endregion
		}
		#endregion

		#region Constants
		const int GWLP_WNDPROC = (-4);
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		static NativeMessageHandler()
		{
			m_handlers = new Handlers();
		}
		/// <summary>
		/// 
		/// </summary>
		public NativeMessageHandler()
		{
			m_wndProc = new WindowsAPI.WindowProc(WndProc);

			m_hWnd = IntPtr.Zero;
			m_defProc = IntPtr.Zero;

			m_preProcessFilter = null;
			m_postProcessFlter = null;
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		public void Assign(IntPtr hWnd)
		{
			if (!this.IsAssigned)
			{
				if (0 != WindowsAPI.IsWindow(hWnd))
				{
					m_hWnd = hWnd;
					m_defProc = SetWndProc(hWnd, m_wndProc);
					m_handlers.AddHandler(hWnd, this);
				}
			}
			else throw new InvalidOperationException("Handle is already assigned");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		public void Unassign()
		{
			if( this.IsAssigned )
			{
				m_handlers.RemoveHandlers( m_hWnd );
				m_hWnd = IntPtr.Zero;
			}
			else throw new InvalidOperationException( "No handle assigned" );
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public bool IsAssigned
		{
			get
			{
				return m_hWnd != IntPtr.Zero;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public INativeMessageFilter MessageFilter
		{
			get
			{
				return m_preProcessFilter;
			}
			set
			{
				m_preProcessFilter = value;
				m_postProcessFlter = value as INativeMessageFilter2;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="msg"></param>
		/// <param name="wparam"></param>
		/// <param name="lparam"></param>
		/// <returns></returns>
		IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			if (msg == (int)Msg.WM_DESTROY)
			{
				this.MessageFilter = null;
			}
			
			IntPtr rc = CallWndProc(hWnd, msg, wparam, lparam);

			if (msg == (int)Msg.WM_NCDESTROY)
			{
				m_handlers.RemoveHandlers(hWnd);
				m_hWnd = IntPtr.Zero;
			}

			return rc;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="msg"></param>
		/// <param name="wparam"></param>
		/// <param name="lparam"></param>
		/// <returns></returns>
		IntPtr CallWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			Message m = new Message();
			m.HWnd = hWnd;
			m.Msg = msg;
			m.WParam = wparam;
			m.LParam = lparam;

			if (!PreProcessing(ref m))
			{
				DefProcessing(ref m);
				PostProcessing(ref m);
			}
			
			return m.Result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool PreProcessing(ref Message m)
		{
			if (m_preProcessFilter != null)
			{
				return m_preProcessFilter.ProcessMessage(ref m);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		void PostProcessing(ref Message m)
		{
			if (m_postProcessFlter != null)
			{
				m_postProcessFlter.PostProcessMessage(ref m);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		void DefProcessing(ref Message m)
		{
			if (m_defProc != IntPtr.Zero)
			{
				m.Result = (IntPtr)WindowsAPI.CallWindowProc(m_defProc, m.HWnd, m.Msg, m.WParam, m.LParam);
			}
			else
			{
				m.Result = (IntPtr)WindowsAPI.DefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IntPtr SetWndProc(IntPtr hWnd, WindowsAPI.WindowProc wndProc)
		{
			IntPtr oldProc = IntPtr.Zero;

			IntPtr user32 = LoadLibrary("user32.dll");
			if (user32 != IntPtr.Zero)
			{
				IntPtr pProc = GetProcAddress(user32, "SetWindowLongPtrW");
				if (pProc == IntPtr.Zero)
				{
					pProc = GetProcAddress(user32, "SetWindowLongW");
				}
				if (pProc != IntPtr.Zero)
				{
					SetWindowLongPtr func = (SetWindowLongPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(SetWindowLongPtr));
					oldProc = func(hWnd, GWLP_WNDPROC, wndProc);
				}
				FreeLibrary(user32);
			}
			
			return oldProc;
		}
		[DllImport("kernel32.dll", ExactSpelling = false, CharSet = CharSet.Auto)]
		static extern IntPtr LoadLibrary(string sFileName);
		[DllImport("kernel32.dll")]
		static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32.dll")]
		static extern IntPtr GetProcAddress(IntPtr hModule, string sProcName);
		#endregion

		#region Fields
		WindowsAPI.WindowProc m_wndProc;

		IntPtr m_hWnd;
		IntPtr m_defProc;

		INativeMessageFilter m_preProcessFilter;
		INativeMessageFilter2 m_postProcessFlter;

		static Handlers m_handlers;
		#endregion

		#region Delegates
		delegate IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WindowsAPI.WindowProc wndProc);
		#endregion
	}
	#endregion

	#region NativeWindowBase
	public class NativeWindowBase : INativeMessageFilter2, IDisposable
	{
		#region Constructors/Destructors
		
		public NativeWindowBase()
        {
        }
		
		~NativeWindowBase()
        {
            Dispose(false);
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		public void AssignHandle(IntPtr hWnd)
		{
			if (m_handler == null)
			{
				if (WindowsAPI.IsWindow(hWnd)!=0)
				{
					m_handler = new NativeMessageHandler();

					m_handler.Assign(hWnd);
					m_handler.MessageFilter = this;
				}
				else throw new InvalidOperationException("Invalid window handle");
			}
			else throw new InvalidOperationException("Handle is already assigned");
		}
		/// <summary>
		/// 
		/// </summary>
		public void ReleaseHandle()
		{
			if (m_handler != null)
			{
				m_handler.MessageFilter = null;
				m_handler = null;
			}
		}
		#endregion

		#region INativeMessageFilter2 implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public virtual bool ProcessMessage(ref Message m)
		{
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		public virtual void PostProcessMessage(ref Message m)
		{
			;
		}
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bDisposing"></param>
		protected virtual void Dispose(bool bDisposing)
		{
			if (bDisposing)
			{
				ReleaseHandle();
			}
		}
		#endregion

		#region Fields
		NativeMessageHandler m_handler = null;

		#endregion
	}
	#endregion

	#region NativeWindowEx
	class NativeWindowEx : NativeWindow
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		public NativeWindowEx(IntPtr hWnd)
		{
			AssignHandle(hWnd);
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public IMessageFilter MessageFilter
		{
			get { return m_messageFilter; }
			set { m_messageFilter = value; }
		}
		#endregion

		#region Overrides

		protected override void WndProc(ref Message m)
		{
			if (m_messageFilter != null && m_messageFilter.PreFilterMessage(ref m))
			{
				return;
			}
			base.WndProc(ref m);
		}
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		IMessageFilter m_messageFilter = null;
		#endregion
	}
	#endregion
}

#endif