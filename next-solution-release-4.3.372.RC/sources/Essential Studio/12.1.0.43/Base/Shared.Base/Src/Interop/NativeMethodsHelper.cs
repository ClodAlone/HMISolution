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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Documentation;

namespace Syncfusion.Runtime.InteropServices
{
	/// <summary>
	/// The NativeMethodsHelper class is a wrapper over some Interop calls that are exposed using static methods
	/// in this class.
	/// </summary>
	public class NativeMethodsHelper
	{
		/// <summary>
		/// Keeps track of the number of times Suspend and Resume redraw window is called for a particular window.
		/// </summary>
		private static Hashtable redrawWindowCallCountMap = new Hashtable();

		public NativeMethodsHelper()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Sends the WM_SETREDRAW to the handle of the Control to prevent drawing of the control.
		/// </summary>
		/// <param name="ctlHandle">The handle of the control that is to be suspended.</param>
		public static void SuspendRedrawWindow(IntPtr ctlHandle)
		{
			if(redrawWindowCallCountMap.ContainsKey(ctlHandle) == true)
			{
				// The handle already exists in this map - means SuspendRedraw was already called for this window 
				// - just increment count and leave.
				int callCount = Convert.ToInt32(redrawWindowCallCountMap[ctlHandle]);
				callCount++;
				redrawWindowCallCountMap[ctlHandle] = callCount;
#if DEBUG
//				Console.WriteLine("SuspendRedraw Ignored - Count =" + callCount.ToString());
#endif
			}
			else
			{
#if DEBUG
//				Console.WriteLine("SuspendRedraw Called - Count = 1");
#endif

				// The handle does not exist in the map - call SendMessage and add an entry - Set the count to 1
				redrawWindowCallCountMap.Add(ctlHandle, 1);
				Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( ctlHandle,Syncfusion.Runtime.InteropServices.NativeMethods.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero );
			}
		}

		/// <summary>
		/// Resumes redrawing of the window.
		/// </summary>
		public static void ResumeRedrawWindow(IntPtr ctlHandle)
		{
			ResumeRedrawWindow(ctlHandle, false);
		}

		/// <summary>
		/// Resumes redrawing of the window.
		/// </summary>
		/// <param name="ctlHandle"></param>
		/// <param name="bRedraw"></param>
		public static void ResumeRedrawWindow(IntPtr ctlHandle, bool bRedraw)
		{
			if(redrawWindowCallCountMap.ContainsKey(ctlHandle) == true)
			{
				//The handle is present in the map - this is the only case to consider
				//If not, Resume has been called without calling Suspend
				int callCount = Convert.ToInt32(redrawWindowCallCountMap[ctlHandle]);
				callCount--;
				if(callCount == 0)
				{
#if DEBUG
//					Console.WriteLine("ResumeRedrawCalled - Count = 0");
#endif
					Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( ctlHandle,Syncfusion.Runtime.InteropServices.NativeMethods.WM_SETREDRAW, new IntPtr( 1 ), IntPtr.Zero );
					redrawWindowCallCountMap.Remove(ctlHandle);

					if(bRedraw)
						RedrawWindow(ctlHandle);

				}
				else
				{
#if DEBUG
//					Console.WriteLine("ResumeRedraw Ignored - Count =" + callCount.ToString());
#endif
					redrawWindowCallCountMap[ctlHandle] = callCount;
				}
			}
#if DEBUG
//			else
//				Console.WriteLine("ResumeRedraw Ignored - Count = 0 Resume called without Suspend");
#endif
		}

		public static void RedrawWindow(IntPtr ctlHandle)
		{
			int redrawFlags = NativeMethods.RDW_ERASE |NativeMethods.RDW_INVALIDATE |NativeMethods.RDW_ALLCHILDREN |
				NativeMethods.RDW_FRAME ;

			RedrawWindow(ctlHandle, redrawFlags);
		}

		public static void RedrawWindow(IntPtr ctlHandle, int redrawFlags)
		{
			Syncfusion.Runtime.InteropServices.NativeMethods.RedrawWindow( ctlHandle,IntPtr.Zero, IntPtr.Zero, (uint)redrawFlags );
		}

		/// <summary>
		/// Sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn
		/// or to prevent changes in that window from being redrawn.
		/// </summary>
		/// <param name="hWnd">Handle to the native window.</param>
		/// <param name="bRedraw">Indicates the redraw state.
		/// If this parameter is true, the content can be redrawn after a change.
		/// If this parameter is false, the content cannot be redrawn after a change.</param>
		/// <param name="bForceRedraw">Forces redraw of the window using <see cref="Syncfusion.Runtime.InteropServices.NativeMethodsHelper.RedrawWindow(System.IntPtr, int)"/>.
		/// Works only if <c>bRedraw</c> is <c>true</c>.</param>
		public static void SetRedrawWindow( IntPtr hWnd, bool bRedraw, bool bForceRedraw )
		{
			NativeMethods.SendMessage( hWnd, Syncfusion.Runtime.InteropServices.NativeMethods.WM_SETREDRAW,
				new IntPtr( bRedraw ? 1 : 0 ), IntPtr.Zero );

			if( bRedraw && bForceRedraw )
			{
				RedrawWindow( hWnd, NativeMethods.RDW_INVALIDATE|NativeMethods.RDW_INTERNALPAINT|NativeMethods.RDW_ERASE|NativeMethods.RDW_ALLCHILDREN|NativeMethods.RDW_ERASENOW|NativeMethods.RDW_FRAME );
			}
		}

		/// <summary>
		/// Overload of <see cref="NativeMethodsHelper.SetRedrawWindow(System.IntPtr, bool, bool)"/>
		/// Forcefully redraws window if redraw is enabled.
		/// </summary>
		public static void SetRedrawWindow( IntPtr hWnd, bool bRedraw )
		{
			SetRedrawWindow( hWnd, bRedraw, bRedraw );
		}

		public static void LockWindowUpdate( IntPtr hWnd)
		{
#if DEBUG
		//	Console.WriteLine("LockWindowUpdate called");
#endif
			NativeMethods.LockWindowUpdate(hWnd);
		}

        private static Hashtable m_htCallers = new Hashtable();

        public static void ResetRedrawCaller(IntPtr ctrlHandle, object caller)
        {
            if (m_htCallers[ctrlHandle] != null)
            {
                m_htCallers[ctrlHandle] = caller;
            }
        }
        public static bool SetRedrawWindow(IntPtr ctrlHandle, bool bRedraw, object caller )
        {
            bool result = false;

            if (!bRedraw)
            {
                if (m_htCallers[ctrlHandle] == null)
                {
                    result = true;
                    m_htCallers[ctrlHandle] = caller;
                }
            }
            else
            {
                if (m_htCallers[ctrlHandle] != null)
                {
                    object storedCaller = m_htCallers[ctrlHandle];

                    if (storedCaller == caller)
                    {
                        result = true;
                        m_htCallers.Remove(ctrlHandle);                        
                    }
                }
            }

            if (result)
            {
                SetRedrawWindow(ctrlHandle, bRedraw, bRedraw);
            }

            return result;
        }
	}
}
