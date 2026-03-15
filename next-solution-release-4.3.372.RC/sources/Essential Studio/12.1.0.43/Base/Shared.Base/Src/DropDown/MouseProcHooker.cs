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

namespace Syncfusion.Windows.Forms
{
	using System;
	using System.Diagnostics;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Text;
	using Syncfusion.Diagnostics;
	using Syncfusion.Runtime.InteropServices;
	
	[Syncfusion.Documentation.DocumentationExclude()]
	internal class NativeHookMethods
	{

		[
			StructLayout(LayoutKind.Sequential) 
		]
		internal class MSG 
		{
			public IntPtr hwnd = IntPtr.Zero; 
			public int message = 0; 
			public int wParam = 0; 
			public int lParam = 0; 
			public int time = 0; 
			public int pt_x = 0; 
			public int pt_y = 0; 
		}

		[
			StructLayout(LayoutKind.Sequential) 
		]
		internal class MOUSEHOOKSTRUCT 
		{
			public int pt_x = 0; 
			public int pt_y = 0; 
			public IntPtr hwnd = IntPtr.Zero; 
			public int wHitTestCode = 0; 
			public int dwExtraInfo = 0; 
		}

		[
			StructLayout(LayoutKind.Sequential) 
		]
		internal class MOUSEHOOKSTRUCT_LL
		{
			public int pt_x = 0; 
			public int pt_y = 0; 
			public ulong mouseData = 0;
			public ulong flags = 0;
			public ulong time = 0;
			public int dwExtraInfo = 0; 
		}

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=true)]
		extern public static IntPtr CallNextHookEx(IntPtr hhook, int code, int wparam, int lparam)  ;

		[DllImport("user32", CharSet=CharSet.Auto)]
		extern public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)  ;

		public delegate IntPtr HookProc(int nCode, int wParam, int lParam);

		[DllImport("user32", CharSet=CharSet.Auto)]
		extern public static IntPtr SetWindowsHookEx(int hookid, HookProc pfnhook, IntPtr hinst, int threadid)  ;

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=true)]
		extern public static int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		extern public static IntPtr GetModuleHandle(string modName)  ;
		
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		extern public static bool UnhookWindowsHookEx(IntPtr hhook)  ;
	}
       

	[Syncfusion.Documentation.DocumentationExclude()]
	internal enum WH
	{
		JOURNALRECORD    = 0,
		JOURNALPLAYBACK  = 1,
		KEYBOARD         = 2,
		GETMESSAGE       = 3,
		CALLWNDPROC      = 4,
		CBT              = 5,
		SYSMSGFILTER     = 6,
		MOUSE            = 7,
		HARDWARE         = 8,
		DEBUG            = 9,
		SHELL            = 10,
		FOREGROUNDIDLE   = 11,
		CALLWNDPROCRET   = 12,
		KEYBOARD_LL      = 13,
		MOUSE_LL         = 14,
	};

	/*
		 * Hook Codes
#define HC_ACTION           0
#define HC_GETNEXT          1
#define HC_SKIP             2
#define HC_NOREMOVE         3
#define HC_NOREM            HC_NOREMOVE
#define HC_SYSMODALON       4
#define HC_SYSMODALOFF      5
		*/

	[Syncfusion.Documentation.DocumentationExclude()]
	internal enum HCBT
	{
		MOVESIZE       = 0,
		MINMAX         = 1,
		QS             = 2,
		CREATEWND      = 3,
		DESTROYWND     = 4,
		ACTIVATE       = 5,
		CLICKSKIPPED   = 6,
		KEYSKIPPED     = 7,
		SYSCOMMAND     = 8,
		SETFOCUS       = 9
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class KeyboardProcHooker : IDisposable
	{
		// MouseProcHookerUtil Fields
		internal int thisProcessID = 0;
		private IKeyboardProcHookClient client;
		private IntPtr handle;
		private bool hookDisable = false;
		private IntPtr MessageHookHandle;
		private GCHandle MessageHookRoot;

		// KeyboardProcHooker Constructors
		public KeyboardProcHooker(IntPtr handle, IKeyboardProcHookClient client)  
		{
			this.handle = handle;
			this.client = client;
		}

		~KeyboardProcHooker()
		{
			Dispose();
		}

		// Methods
		public void Dispose()  
		{
			this.UnHookMessage();
			GC.SuppressFinalize(this);
		}

		private void HookMessage()  
		{
			GC.KeepAlive(this);
			NativeHookMethods.HookProc hookProc;
			KeyboardProcHooker keyboardProcHooker = (KeyboardProcHooker)this;
			lock (this)
			{
				if (this.MessageHookHandle == IntPtr.Zero)
				{
					if (this.thisProcessID == 0)
						NativeHookMethods.GetWindowThreadProcessId(this.handle,ref (this.thisProcessID));
					MessageHookObject MessageHookobject = new KeyboardProcHooker.MessageHookObject(this);
					hookProc = new NativeHookMethods.HookProc(MessageHookobject.Callback);
					this.MessageHookRoot = GCHandle.Alloc(hookProc);
					this.MessageHookHandle = 
						// Thread specific hook
						NativeHookMethods.SetWindowsHookEx((int) WH.KEYBOARD, 
							hookProc, 
							IntPtr.Zero, //NativeHookMethods.GetModuleHandle(null), 
                            NativeMethods.GetCurrentThreadId()
							);
				}
			} 
		}

		/// <summary>
		///     HookProc used for catch mouse messages.
		/// </summary>
		/// <internalonly/>
		private IntPtr MessageHookProc(int nCode, int wParam, int lParam)  
		{
			GC.KeepAlive(this);
			if (nCode >= 0)
			{
				IKeyboardProcHookClient ikhc = client as IKeyboardProcHookClient;
				if (ikhc != null)
				{
					if(ikhc.KeyboardHookProc(wParam, lParam))
						return (IntPtr)1;
				}
			}

			return NativeHookMethods.CallNextHookEx(this.MessageHookHandle,nCode,wParam,lParam);
		}


		private void UnHookMessage()  
		{
			GC.KeepAlive(this);
			lock(this)
			{
				if (this.MessageHookHandle != IntPtr.Zero)
				{
					NativeHookMethods.UnhookWindowsHookEx(this.MessageHookHandle);
					(this.MessageHookRoot).Free();
					this.MessageHookHandle = IntPtr.Zero;
				}
			} 
		}

		// MouseProcHookerUtil Properties
		public bool DisableMessageHook
		{
			get
			{
				return this.hookDisable;
			}
			set 
			{
				this.hookDisable = value;
//				if (value) 
//					this.UnHookMessage();
			}
		}

		public bool HookMessages
		{
			get 
			{
				GC.KeepAlive(this);
				return (this.MessageHookHandle != IntPtr.Zero);
			}
			set 
			{
				if (value && !this.hookDisable) 
					this.HookMessage();
				else
					this.UnHookMessage();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			internal class MessageHookObject 
		{
			// MessageHookObject Constructors
			public MessageHookObject(KeyboardProcHooker parent)  
			{
				this.reference = new WeakReference(parent, false);
			} 

			private const int HC_ACTION = 0;

			// MessageHookObject Methods
			public IntPtr Callback(int nCode, int wParam, int lParam)  
			{
				IntPtr retval = IntPtr.Zero;
				try
				{
					if( nCode == HC_ACTION )
					{
						KeyboardProcHooker messageHooker = (KeyboardProcHooker)this.reference.Target;
						if (messageHooker != null) 
							retval = messageHooker.MessageHookProc(nCode,wParam,lParam);
					}
				}  
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;

				}
				return retval;
			} 

			// Fields
			internal WeakReference reference;
		} // end of class KeyboardProcHooker.MessageHookObject

	} // end of class KeyboardProcHooker

	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class GetMsgProcHooker : IDisposable
	{
		// MouseProcHookerUtil Fields
		internal int thisProcessID = 0;
		private IMessageFilter client;
		private IntPtr handle;
		private bool hookDisable = false;
		private IntPtr MessageHookHandle;
		private GCHandle MessageHookRoot;

		// GetMsgProcHooker Constructors
		public GetMsgProcHooker(IntPtr handle, IMessageFilter client)  
		{
			this.handle = handle;
			this.client = client;
		}

		~GetMsgProcHooker()
		{
			Dispose();
		}

		// Methods
		public void Dispose()  
		{
			this.UnHookMessage();
			GC.SuppressFinalize(this);
		}

		private void HookMessage()  
		{
			GC.KeepAlive(this);
			NativeHookMethods.HookProc hookProc;
			GetMsgProcHooker getMsgProcHooker = (GetMsgProcHooker)this;
			lock (this)
			{
				if (this.MessageHookHandle == IntPtr.Zero)
				{
					if (this.thisProcessID == 0)
						NativeHookMethods.GetWindowThreadProcessId(this.handle,ref (this.thisProcessID));
					MessageHookObject mho = new GetMsgProcHooker.MessageHookObject(this);
					hookProc = new NativeHookMethods.HookProc(mho.Callback);
					this.MessageHookRoot = GCHandle.Alloc(hookProc);
					this.MessageHookHandle = NativeHookMethods.SetWindowsHookEx((int) WH.GETMESSAGE, hookProc, 
						IntPtr.Zero, Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId());
				}
			} 
		}

		/// <summary>
		///     HookProc used for catch mouse messages.
		/// </summary>
		/// <internalonly/>
		private IntPtr MessageHookProc(int nCode, int wParam, int lParam, ref bool processMessage)  
		{
			GC.KeepAlive(this);
			if (nCode >= 0 && !this.DisableMessageHook)
			{
				IMessageFilter imf = client as IMessageFilter;
				if (imf != null)
				{
					Message msg = (Message)(Marshal.PtrToStructure((IntPtr)lParam, typeof(Message)));

					if(imf.PreFilterMessage(ref msg))
					{
						processMessage = false;
						return (IntPtr)1;
					}
				}
			}

			return NativeHookMethods.CallNextHookEx(this.MessageHookHandle,nCode,wParam,lParam);
		}


		private void UnHookMessage()  
		{
			GC.KeepAlive(this);
			lock(this)
			{
				if (this.MessageHookHandle != IntPtr.Zero)
				{
					NativeHookMethods.UnhookWindowsHookEx(this.MessageHookHandle);
					(this.MessageHookRoot).Free();
					this.MessageHookHandle = IntPtr.Zero;
				}
			} 
		}

		public bool DisableMessageHook
		{
			get
			{
				return this.hookDisable;
			}
			set 
			{
				this.hookDisable = value;
//				if (value) 
//					this.UnHookMessage();
			}
		}

		public bool HookMessages
		{
			get 
			{
				GC.KeepAlive(this);
				return (this.MessageHookHandle != IntPtr.Zero);
			}
			set 
			{
				if (value && !this.hookDisable) 
					this.HookMessage();
				else
					this.UnHookMessage();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			internal class MessageHookObject 
		{
			// MessageHookObject Constructors
			public MessageHookObject(GetMsgProcHooker parent)  
			{
				this.reference = new WeakReference(parent, false);
			} 

			private bool inrecursion = false;
			// MessageHookObject Methods
			public IntPtr Callback(int nCode, int wParam, int lParam)  
			{
				IntPtr retval = IntPtr.Zero;
				if(inrecursion)
					return retval;

				try
				{
					GetMsgProcHooker messageHooker = (GetMsgProcHooker)this.reference.Target;
					if (messageHooker != null) 
					{
						bool processMessage = true;
						retval = messageHooker.MessageHookProc(nCode,wParam,lParam,ref processMessage);
						// If this doesn't work we can go back to using the Keyboard and mouse hooks.
						// Remove message from pump
						if(!processMessage)
						{
							inrecursion = true;
							try
							{
								GetMsgProcHooker hooker = this.reference.Target as GetMsgProcHooker;
								// Temporarily disable this hook while removing messages from the pump
								hooker.DisableMessageHook = true;

								Message msg = (Message)(Marshal.PtrToStructure((IntPtr)lParam, typeof(Message)));

								Syncfusion.Runtime.InteropServices.NativeMethods.MSG outmsg = new Syncfusion.Runtime.InteropServices.NativeMethods.MSG();
								bool removed = Syncfusion.Runtime.InteropServices.NativeMethods.PeekMessage
									(ref outmsg, msg.HWnd, msg.Msg, msg.Msg, 1/*PM_REMOVE*/);
								
								// If you didn't call other hooks, return 0.
								retval = IntPtr.Zero;

								// Reenable hooking
								hooker.DisableMessageHook = false;
							}
							finally
							{
								inrecursion = false;
							}
						}
					}
				}  
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;

				}
				return retval;
			} 

			// Fields
			internal WeakReference reference;
		} // end of class GetMsgProcHooker.MessageHookObject

	} // end of class GetMsgProcHooker


	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class WndProcHooker : IDisposable
	{
		// MouseProcHookerUtil Fields
		internal int thisProcessID = 0;
		private IMessageFilter client;
		private IntPtr handle;
		private bool hookDisable = false;
		private IntPtr MessageHookHandle;
		private GCHandle MessageHookRoot;

		// KeyboardProcHooker Constructors
		public WndProcHooker(IntPtr handle, IMessageFilter client)  
		{
			this.handle = handle;
			this.client = client;
		}

		~WndProcHooker()
		{
			Dispose();
		}

		// Methods
		public void Dispose()  
		{
			this.UnHookMessage();
			GC.SuppressFinalize(this);
		}

		private void HookMessage()  
		{
			GC.KeepAlive(this);
			NativeHookMethods.HookProc hookProc;
			WndProcHooker wndProcHooker = (WndProcHooker)this;
			lock (this)
			{
				if (this.MessageHookHandle == IntPtr.Zero)
				{
					if (this.thisProcessID == 0)
						NativeHookMethods.GetWindowThreadProcessId(this.handle,ref (this.thisProcessID));
					MessageHookObject mho = new WndProcHooker.MessageHookObject(this);
					hookProc = new NativeHookMethods.HookProc(mho.Callback);
					this.MessageHookRoot = GCHandle.Alloc(hookProc);
					this.MessageHookHandle = NativeHookMethods.SetWindowsHookEx((int) WH.CALLWNDPROC, hookProc, 
						IntPtr.Zero, Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId());
				}
			} 
		}

		/// <summary>
		///     HookProc used for catch mouse messages.
		/// </summary>
		/// <internalonly/>
		private IntPtr MessageHookProc(int nCode, int wParam, int lParam, ref bool processMessage)  
		{
			GC.KeepAlive(this);
			if (nCode >= 0 && !this.DisableMessageHook)
			{
				IMessageFilter imf = client as IMessageFilter;
				if (imf != null)
				{
					Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp = 
						(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure((IntPtr)lParam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));

					Message msg = Message.Create(cwp.hwnd, cwp.message, cwp.wParam, cwp.lParam);

					if(imf.PreFilterMessage(ref msg))
					{
						processMessage = false;
						return (IntPtr)1;
					}
				}
			}

			return NativeHookMethods.CallNextHookEx(this.MessageHookHandle,nCode,wParam,lParam);
		}


		private void UnHookMessage()  
		{
			GC.KeepAlive(this);
			lock(this)
			{
				if (this.MessageHookHandle != IntPtr.Zero)
				{
					NativeHookMethods.UnhookWindowsHookEx(this.MessageHookHandle);
					(this.MessageHookRoot).Free();
					this.MessageHookHandle = IntPtr.Zero;
				}
			} 
		}

		public bool DisableMessageHook
		{
			get
			{
				return this.hookDisable;
			}
			set 
			{
				this.hookDisable = value;
//				if (value) 
//					this.UnHookMessage();
			}
		}

		public bool HookMessages
		{
			get 
			{
				GC.KeepAlive(this);
				return (this.MessageHookHandle != IntPtr.Zero);
			}
			set 
			{
				if (value && !this.hookDisable) 
					this.HookMessage();
				else
					this.UnHookMessage();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			internal class MessageHookObject 
		{
			// MessageHookObject Constructors
			public MessageHookObject(WndProcHooker parent)  
			{
				this.reference = new WeakReference(parent, false);
			} 

			private bool inrecursion = false;
			// MessageHookObject Methods
			public IntPtr Callback(int nCode, int wParam, int lParam)  
			{
				IntPtr retval = IntPtr.Zero;
				if(inrecursion)
					return retval;

				try
				{
					WndProcHooker messageHooker = (WndProcHooker)this.reference.Target;
					if (messageHooker != null) 
					{
						bool processMessage = true;
						retval = messageHooker.MessageHookProc(nCode,wParam,lParam, ref processMessage);
						// If this doesn't work we can go back to using the Keyboard and mouse hooks.
						// Remove message from pump
						if(!processMessage)
						{
							inrecursion = true;
							try
							{
								WndProcHooker hooker = this.reference.Target as WndProcHooker;
								// Temporarily disable this hook while removing messages from the pump
								hooker.DisableMessageHook = true;

								Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp = 
									(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure((IntPtr)lParam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));

								Syncfusion.Runtime.InteropServices.NativeMethods.MSG outmsg = new Syncfusion.Runtime.InteropServices.NativeMethods.MSG();
								bool removed = Syncfusion.Runtime.InteropServices.NativeMethods.PeekMessage
									(ref outmsg, cwp.hwnd, cwp.message, cwp.message, 1/*PM_REMOVE*/);
								
								// If you didn't call other hooks, return 0.
								retval = IntPtr.Zero;

								// Reenable hooking
								hooker.DisableMessageHook = false;
							}
							finally
							{
								inrecursion = false;
							}
						}
					}
				}  
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;

				}
				return retval;
			} 

			// Fields
			internal WeakReference reference;
		} // end of class WndProcHooker.MessageHookObject

	} // end of class WndProcHooker

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MouseProcHookerUtil : IDisposable
	{
		// MouseProcHookerUtil Fields
		internal int thisProcessID = 0;
		private IMouseHookProcClient client;
		private IMouseHookHLProcClient clientHL;
		private IntPtr handle;
		private bool hookDisable = false;
		private IntPtr MessageHookHandle;
		private GCHandle MessageHookRoot;

		// MouseProcHookerUtil Constructors
		public MouseProcHookerUtil(IntPtr handle, IMouseHookProcClient client)  
		{
			this.handle = handle;
			this.client = client;
		}
		public MouseProcHookerUtil(IntPtr handle, IMouseHookHLProcClient clientHL)  
		{
			this.handle = handle;
			this.clientHL = clientHL;
		}

		~MouseProcHookerUtil()
		{
			Dispose();
		}

		// Methods
		public void Dispose()  
		{
			this.UnHookMessage();
			GC.SuppressFinalize(this);
		}

		private void HookMessage()  
		{
			GC.KeepAlive(this);
			NativeHookMethods.HookProc hookProc;
			MouseProcHookerUtil mouseProcHooker = (MouseProcHookerUtil)this;
			lock (this)
			{
				if (this.MessageHookHandle == IntPtr.Zero)
				{
					if (this.thisProcessID == 0)
						NativeHookMethods.GetWindowThreadProcessId(this.handle,ref (this.thisProcessID));
					MessageHookObject MessageHookobject = new MouseProcHookerUtil.MessageHookObject(this);
					hookProc = new NativeHookMethods.HookProc(MessageHookobject.Callback);
					this.MessageHookRoot = GCHandle.Alloc(hookProc);
					int hookType = 0;
					if(this.client != null)
						hookType = (int)WH.MOUSE_LL;
					else
						hookType = (int)WH.MOUSE;
//					// Thread specific hook.
					this.MessageHookHandle = NativeHookMethods.
						SetWindowsHookEx(hookType, 
							hookProc, 
						IntPtr.Zero,//NativeHookMethods.GetModuleHandle(null), 
						NativeMethods.GetCurrentThreadId() );
//					this.MessageHookHandle = NativeHookMethods.SetWindowsHookEx(hookType, 
//						hookProc, NativeHookMethods.GetModuleHandle(null), 0);
				}
			} 
		}

		/// <summary>
		///     HookProc used for catch mouse messages.
		/// </summary>
		/// <internalonly/>
		private IntPtr MessageHookProc(int nCode, int wParam, int lParam)  
		{
			GC.KeepAlive(this);
			if (nCode >= 0)
			{
				if(this.client != null)
				{
					NativeHookMethods.MOUSEHOOKSTRUCT_LL mhStruct;
					mhStruct = (NativeHookMethods.MOUSEHOOKSTRUCT_LL) Marshal.PtrToStructure((IntPtr) lParam,
						typeof(NativeHookMethods.MOUSEHOOKSTRUCT_LL));
					if (mhStruct != null) 
					{
						IMouseHookProcClient imhc = client;
						//						IntPtr wndDest = Syncfusion.Win32.NativeWin32.WindowFromPoint(mhStruct.pt_x, mhStruct.pt_y);
						if(imhc.MouseHookProc(wParam, new Point(mhStruct.pt_x, mhStruct.pt_y),
							/*mhStruct.wHitTestCode,*/ mhStruct.dwExtraInfo))
							// Debug.WriteLine("MessageHookProc(int nCode = " + nCode + "HitTest " + mhStruct.wHitTestCode + "Extra " + mhStruct.dwExtraInfo + ")");
							return (IntPtr)1;
					}
				}
				else if(this.clientHL != null)
				{
					NativeHookMethods.MOUSEHOOKSTRUCT mhStruct;
					mhStruct = (NativeHookMethods.MOUSEHOOKSTRUCT) Marshal.PtrToStructure((IntPtr) lParam,
						typeof(NativeHookMethods.MOUSEHOOKSTRUCT));
					if(mhStruct != null)
					{
						IMouseHookHLProcClient imhc = clientHL;
						if(imhc.MouseHookProc(wParam, new Point(mhStruct.pt_x, mhStruct.pt_y), mhStruct.hwnd, mhStruct.wHitTestCode, mhStruct.dwExtraInfo))
							return (IntPtr)1;
					}
				}
			}

			return NativeHookMethods.CallNextHookEx(this.MessageHookHandle,nCode,wParam,lParam);
		}


		private void UnHookMessage()  
		{
			GC.KeepAlive(this);
			lock(this)
			{
				if (this.MessageHookHandle != IntPtr.Zero)
				{
					NativeHookMethods.UnhookWindowsHookEx(this.MessageHookHandle);
					(this.MessageHookRoot).Free();
					this.MessageHookHandle = IntPtr.Zero;
				}
			} 
		}

		// MouseProcHookerUtil Properties
		public bool DisableMessageHook
		{
			get
			{
				return this.hookDisable;
			}
			set 
			{
				this.hookDisable = value;
//				if (value) 
//					this.UnHookMessage();
			}
		}

		public bool HookMessages
		{
			get 
			{
				GC.KeepAlive(this);
				return (this.MessageHookHandle != IntPtr.Zero);
			}
			set 
			{
				if (value && !this.hookDisable) 
					this.HookMessage();
				else
					this.UnHookMessage();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			internal class MessageHookObject 
		{
			// MessageHookObject Constructors
			public MessageHookObject(MouseProcHookerUtil parent)  
			{
//				this.reference = new WeakReference(parent, false);
				this.reference = parent;
			} 

			// MessageHookObject Methods
			public IntPtr Callback(int nCode, int wParam, int lParam)  
			{
				IntPtr retval = IntPtr.Zero;
				try
				{
//					MouseProcHookerUtil messageHooker = (MouseProcHookerUtil)this.reference.Target;
					MouseProcHookerUtil messageHooker = (MouseProcHookerUtil)this.reference;
					if (messageHooker != null) 
						retval = messageHooker.MessageHookProc(nCode,wParam,lParam);
				}  
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
				}
				return retval;
			} 

			// Fields
//			internal WeakReference reference;
			internal MouseProcHookerUtil reference;
		} // end of class MouseProcHookerUtil.MessageHookObject

	} // end of class MouseProcHookerUtil
}
