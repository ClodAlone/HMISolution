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
using System.IO;
using System.Collections;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    ///<exclude/>
	[Documentation.DocumentationExclude()]
	public interface IGetMsgProcListener
	{
		void GetMsgProc(int ncode, IntPtr wparam, IntPtr lparam);
	}

	[Documentation.DocumentationExclude()]
	public interface ICallWndProcListener
	{
		void CallWndProc(int ncode, IntPtr wparam, IntPtr lparam);
	}

	[Documentation.DocumentationExclude()]
	public class DesignerHooks
	{
		private static Hashtable threadHookMap = new Hashtable();

		public static bool GetMsgProcListContains(IGetMsgProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			return GetMsgProcListContains( listener, callingthread );
			
		}
		public static bool GetMsgProcListContains(IGetMsgProcListener listener, int threadId)
		{
			if(DesignerHooks.threadHookMap.ContainsKey(threadId) == true)
			{
				ThreadHooks hkthread = DesignerHooks.threadHookMap[threadId] as ThreadHooks;
				return hkthread.GetMsgProcListContains(listener);
			}
			return false;
		}

		public static bool CallWndProcListContains(ICallWndProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			return CallWndProcListContains( listener, callingthread);
		}

		public static bool CallWndProcListContains(ICallWndProcListener listener, int threadId)
		{
			if(DesignerHooks.threadHookMap.ContainsKey(threadId) == true)
			{
				ThreadHooks hkthread = DesignerHooks.threadHookMap[threadId] as ThreadHooks;
				return hkthread.CallWndProcListContains(listener);
			}

			return false;
		}

		public static int AddGetMsgProcListener(IGetMsgProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			ThreadHooks hkthread;
			if(DesignerHooks.threadHookMap.ContainsKey(callingthread) == true)
			{
				hkthread = DesignerHooks.threadHookMap[callingthread] as ThreadHooks;
			}
			else
			{
				hkthread = new ThreadHooks();
				DesignerHooks.threadHookMap.Add(callingthread, hkthread);
			}
			hkthread.AddGetMsgProcListener(listener);

			return callingthread;
		}

		public static void RemoveGetMsgProcListener(IGetMsgProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			RemoveGetMsgProcListener( listener, callingthread );
		}
		public static void RemoveGetMsgProcListener(IGetMsgProcListener listener, int threadId)
		{
			if(DesignerHooks.threadHookMap.ContainsKey(threadId) == true)
			{
				ThreadHooks hkthread = DesignerHooks.threadHookMap[threadId] as ThreadHooks;
				hkthread.RemoveGetMsgProcListener(listener);
				if((hkthread.getMsgProcCount == 0) && (hkthread.callWndProcCount == 0))
					DesignerHooks.threadHookMap.Remove(threadId);
			}
		}


		public static int AddCallWndProcListener(ICallWndProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			ThreadHooks hkthread;
			if(DesignerHooks.threadHookMap.ContainsKey(callingthread) == true)
			{
				hkthread = DesignerHooks.threadHookMap[callingthread] as ThreadHooks;
			}
			else
			{
				hkthread = new ThreadHooks();
				DesignerHooks.threadHookMap.Add(callingthread, hkthread);
			}
			hkthread.AddCallWndProcListener(listener);

			return callingthread;
		}

		public static void RemoveCallWndProcListener(ICallWndProcListener listener)
		{
			int callingthread = Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId();
			RemoveCallWndProcListener( listener, callingthread );
		}

		public static void RemoveCallWndProcListener(ICallWndProcListener listener, int threadId)
		{
			if(DesignerHooks.threadHookMap.ContainsKey(threadId) == true)
			{
				ThreadHooks hkthread = DesignerHooks.threadHookMap[threadId] as ThreadHooks;
				hkthread.RemoveCallWndProcListener(listener);
				if((hkthread.callWndProcCount == 0) && (hkthread.getMsgProcCount == 0))
					DesignerHooks.threadHookMap.Remove(threadId);
			}
		}
	}


	[Documentation.DocumentationExclude()]
	public class ThreadHooks
	{
		private IGetMsgProcListener[] getMsgProcArray = new IGetMsgProcListener[16];
		public int getMsgProcCount;
		private ICallWndProcListener[] callWndProcArray = new ICallWndProcListener[16];
		public int callWndProcCount;

		private IntPtr hkGetMsgProc = IntPtr.Zero;
		private IntPtr hkCallWndProc = IntPtr.Zero;
		private Syncfusion.Runtime.InteropServices.NativeMethods.HookProc getMsgProc;
		private Syncfusion.Runtime.InteropServices.NativeMethods.HookProc callWndProc;

		public ThreadHooks()
		{
		}

		public bool GetMsgProcListContains(IGetMsgProcListener listener)
		{
			for(int i=0; i<this.getMsgProcCount; i++)
			{
				if(listener == this.getMsgProcArray[i])
					return true;
			}
			return false;
		}

		public bool CallWndProcListContains(ICallWndProcListener listener)
		{
			for(int i=0; i<this.callWndProcCount; i++)
			{
				if(listener == this.callWndProcArray[i])
					return true;
			}
			return false;
		}

		public void AddGetMsgProcListener(IGetMsgProcListener listener)
		{
			if(this.GetMsgProcListContains(listener) == false)
			{
				// If the array size is insufficient then allocate a new array and assign this as the GetMsgProcArray
				if(this.getMsgProcArray.Length <= this.getMsgProcCount)
				{
					IGetMsgProcListener[] newarray = new IGetMsgProcListener[(this.getMsgProcCount == 0) ? 16 : this.getMsgProcCount*2];
					Array.Copy(this.getMsgProcArray, 0, newarray, 0, this.getMsgProcCount);
					Array.Clear(this.getMsgProcArray, 0, this.getMsgProcCount);
					this.getMsgProcArray = newarray;
				}
				this.getMsgProcArray[this.getMsgProcCount++] = listener;
			}

			if((this.getMsgProcCount > 0) && (this.hkGetMsgProc == IntPtr.Zero))
			{
				this.getMsgProc = new Syncfusion.Runtime.InteropServices.NativeMethods.HookProc(this.GetMsgProcedure);
				this.hkGetMsgProc = Syncfusion.Runtime.InteropServices.NativeMethods.SetWindowsHookEx(3/*WH_GETMESSAGE*/,
					this.getMsgProc, IntPtr.Zero, Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId());
			}
		}

		public void RemoveGetMsgProcListener(IGetMsgProcListener listener)
		{
			for(int nindex=0; nindex < this.getMsgProcCount; nindex++)
			{
				if(listener == this.getMsgProcArray[nindex])
				{
					this.getMsgProcCount--;
					if(nindex < this.getMsgProcCount)
						Array.Copy(this.getMsgProcArray, nindex+1, this.getMsgProcArray, nindex, this.getMsgProcCount-nindex);
					this.getMsgProcArray[this.getMsgProcCount] = null;
					break;
				}
			}

			if((this.getMsgProcCount == 0) && (this.hkGetMsgProc != IntPtr.Zero))
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.UnhookWindowsHookEx(this.hkGetMsgProc);
				this.getMsgProc = null;
				this.hkGetMsgProc = IntPtr.Zero;
			}
		}

		public void AddCallWndProcListener(ICallWndProcListener listener)
		{
			if(this.CallWndProcListContains(listener) == false)
			{
				// If the array size is insufficient then allocate a new array and assign this as the GetMsgProcArray
				if(this.callWndProcArray.Length <= this.callWndProcCount)
				{
					ICallWndProcListener[] newarray = new ICallWndProcListener[(this.callWndProcCount == 0) ? 16 : this.callWndProcCount*2];
					Array.Copy(this.callWndProcArray, 0, newarray, 0, this.callWndProcCount);
					Array.Clear(this.callWndProcArray, 0, this.callWndProcCount);
					this.callWndProcArray = newarray;
				}
				this.callWndProcArray[this.callWndProcCount++] = listener;
			}

			if((this.callWndProcCount > 0) && (this.hkCallWndProc == IntPtr.Zero))
			{
				this.callWndProc = new Syncfusion.Runtime.InteropServices.NativeMethods.HookProc(this.CallWndProcedure);
				this.hkCallWndProc = Syncfusion.Runtime.InteropServices.NativeMethods.SetWindowsHookEx(4/*WH_CALLWNDPROC*/,
					this.callWndProc, IntPtr.Zero, Syncfusion.Runtime.InteropServices.NativeMethods.GetCurrentThreadId());
			}
		}

		public void RemoveCallWndProcListener(ICallWndProcListener listener)
		{
			for(int nindex=0; nindex < this.callWndProcCount; nindex++)
			{
				if(listener == this.callWndProcArray[nindex])
				{
					this.callWndProcCount--;
					if(nindex < this.callWndProcCount)
						Array.Copy(this.callWndProcArray, nindex+1, this.callWndProcArray, nindex, this.callWndProcCount-nindex);
					this.callWndProcArray[this.callWndProcCount] = null;
					break;
				}
			}

			if((this.callWndProcCount == 0) && (this.hkCallWndProc != IntPtr.Zero))
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.UnhookWindowsHookEx(this.hkCallWndProc);
				this.callWndProc = null;
				this.hkCallWndProc = IntPtr.Zero;
			}
		}

        /// <summary>
        /// Workaround for incident 35141. When calling Timer.Stop in GridGroupDropArea
        /// this ends up calling MdiSysMenuProvider.MsgHook 
        /// </summary>
        public static bool IgnoreWndProcNcodeZero = false;

        public IntPtr GetMsgProcedure(int ncode, IntPtr wparam, IntPtr lparam)
		{
            if (IgnoreWndProcNcodeZero && ncode == 0)
            {
            }
            else if (ncode >= 0)
			{
				// Copy contents of getMsgProcArray into a temporary array and invoke the GetMsgProc method
				// on the contents of the temp array. This indirection allows nested addition/removal of subscribers
				// from within the message proc.
				IGetMsgProcListener[] msgprocarray = new IGetMsgProcListener[this.getMsgProcCount];
				Array.Copy(this.getMsgProcArray, msgprocarray, this.getMsgProcCount);
				foreach(IGetMsgProcListener listener in msgprocarray)
				{
					listener.GetMsgProc(ncode, wparam, lparam);
				}
			}
			return Syncfusion.Runtime.InteropServices.NativeMethods.CallNextHookEx(this.hkGetMsgProc, ncode, wparam, lparam);
		}

		public IntPtr CallWndProcedure(int ncode, IntPtr wparam, IntPtr lparam)
		{
            if (ncode >= 0)
            {
				// Copy contents of callWndProcArray into a temporary array and invoke the CallWndProc method
				// on the contents of the temp array. This indirection allows nested addition/removal of subscribers
				// from within the wnd proc.
				ICallWndProcListener[] wndprocarray = new ICallWndProcListener[this.callWndProcCount];
				Array.Copy(this.callWndProcArray, wndprocarray, this.callWndProcCount);
				foreach(ICallWndProcListener listener in wndprocarray)
				{
					listener.CallWndProc(ncode, wparam, lparam);
				}
			}
			return Syncfusion.Runtime.InteropServices.NativeMethods.CallNextHookEx(this.hkCallWndProc, ncode, wparam, lparam);
		}
	}
}
