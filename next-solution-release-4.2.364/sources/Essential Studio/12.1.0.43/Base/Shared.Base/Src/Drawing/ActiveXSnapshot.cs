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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Provides support for capturing an ActiveX or Windows Forms control to a bitmap.
	/// </summary>
	public class ActiveXSnapshot
	{
		private static readonly int hiMetricPerInch;
		private static Point logPixels;

		static ActiveXSnapshot()
		{
			hiMetricPerInch = 2540/*0x9ec*/;
		}

		static bool firstTime = true;

		/// <overload>
		/// Captures the contents of an ActiveX control to a bitmap using the IViewObject interface.
		/// </overload>
		/// <summary>
		/// Captures the contents of an ActiveX control to a bitmap using the IViewObject interface.
		/// </summary>
		/// <param name="ocx">The ActiveX control.</param>
		/// <returns>The bitmap with the display contents of the ActiveX control.</returns>
		/// <remarks>
		/// The control must have an implementation of the IViewObject interface.
		/// </remarks>
		public static Bitmap TakeSnapshot(object ocx)
		{
			// If I do GetExtent it returns wrong values the very first time it is called ...
			// IOleObject.GetExtent is alwasy correct.
			bool preferViewObject2 = !firstTime;
			firstTime = false;
			return TakeSnapshot(ocx, preferViewObject2);
		}
		
		/// <summary>
		/// Captures the contents of an ActiveX control to a bitmap using the IViewObject interface.
		/// </summary>
		/// <param name="ocx">The ActiveX control.</param>
		/// <param name="preferViewObject2">Indicates whether IViewObject2 interface should be used if available.</param>
		/// <returns>The bitmap with the display contents of the ActiveX control.</returns>
		/// <genoverload/>
		public static Bitmap TakeSnapshot(object ocx, bool preferViewObject2)
		{
			NativeMethods.IViewObject2 pViewObject2 = null;
			NativeMethods.IViewObject pViewObject = null;
			NativeMethods.IOleObject pOleObject = null;
			IntPtr hDrawDC = IntPtr.Zero;
			IntPtr hBmp = IntPtr.Zero;
			IntPtr hBmpOld = IntPtr.Zero;
			int iWidth = 0;
			int iHeight = 0;
			NativeMethods.tagSIZEL hmSizeL = new NativeMethods.tagSIZEL();
			NativeMethods.COMRECT rectL = new NativeMethods.COMRECT();
			Guid IID_IPICTURE = typeof(NativeMethods.IPicture).GUID;
			NativeMethods.PICTDESC pdPictDesc = new NativeMethods.PICTDESC();

			if (ocx == null)
				throw new ArgumentNullException("ocx");

			try
			{
				// Get the IViewObject or IViewObject2 interface.
				if (preferViewObject2 && ocx is NativeMethods.IViewObject2)
					pViewObject2 = (NativeMethods.IViewObject2) ocx;

				pViewObject = (NativeMethods.IViewObject) ocx;

				// Create a DC compatible with the screen DC.
				//
				hDrawDC = NativeMethods.CreateCompatibleDC(IntPtr.Zero);

				if (pViewObject2 != null)
					pViewObject2.GetExtent(NativeMethods.DVASPECT_CONTENT, -1, null, hmSizeL);
				else
				{
					// Get the IOleObject interface if IViewObject2 is not available.
					//
					pOleObject = (NativeMethods.IOleObject) ocx;

					// Now we have pointers to the control's IViewObject and IOleObject
					// interfaces.
					//

					// We'll have to get the size of the area that
					// our object wants to draw on... apparently, we always get
					// back stuff in HIMETRIC.
					//
					pOleObject.GetExtent(NativeMethods.DVASPECT_OPAQUE, hmSizeL);
				}

				//Convert to pixels, and save information.
				//
				Size pxSize = new Size(HiMetricToPixel(hmSizeL.cx, hmSizeL.cy));

				iWidth = pxSize.Width;
				iHeight = pxSize.Height;
				rectL.top = 0;
				rectL.left = 0;
				rectL.right = iWidth;
				rectL.bottom = iHeight;
    
				// We'll need to create a bitmap of the correct size and select it into our DC.
				//
				int bpp = NativeMethods.GetDeviceCaps(hDrawDC, NativeMethods.BITSPIXEL);
				int planes = NativeMethods.GetDeviceCaps(hDrawDC, NativeMethods.PLANES);

				hBmp = NativeMethods.CreateBitmap(iWidth, iHeight, planes, bpp, null);
				hBmpOld = NativeMethods.SelectObject(hDrawDC, hBmp);

				// Now actually ask the object to draw itself
				//
				pViewObject.Draw(   NativeMethods.DVASPECT_CONTENT, // Aspect to draw - we always want content
					-1,               // Always -1
					IntPtr.Zero,      // We don't use pvAspect
					null,             // We don't need to specify a target device
					IntPtr.Zero,      // We don't need a target HIC
					hDrawDC,          // The DC to draw on
					rectL,            // The bounding box to draw in
					null,             // Only for metafiles
					IntPtr.Zero,      // Not using a callback
					0 );              // Not using a callback
	
				return Image.FromHbitmap(hBmp);
			}
			catch(COMException exception)
			{
				MessageBox.Show("HResult " + exception.ErrorCode.ToString() + ": " + exception.ToString());
				return null;
			}
			finally
			{
				NativeMethods.SelectObject(hDrawDC,hBmpOld);
				NativeMethods.DeleteDC(hDrawDC);
				NativeMethods.DeleteObject(hBmp);

				pOleObject = null;
				pViewObject = null;
				pViewObject2 = null;
			}  
		}

		internal enum PRF
		{
			PRF_CHECKVISIBLE = 0x01,
			PRF_NONCLIENT    = 0x02,
			PRF_CLIENT       = 0x04,
			PRF_ERASEBKGND   = 0x08,
			PRF_CHILDREN     = 0x10,
			PRF_OWNED        = 0x20,
		}

		static bool forceWmPaintInPrintWindow = false;

		/// <summary>
		/// Indicates whether there are issues with WM_PRINT and it is not properly supported by the framework.
		/// The property is available to provide a workaround for issues with GenericControlCell and early Whidbey builds. Default is false.
		/// </summary>
		/// <remarks>
		/// <example>
		/// <code>
		/// if (Environment.Version.Major >= 2)
		///    ActiveXSnapshot.ForceWmPaintInPrintWindow = true;
		/// </code>
		/// </example>
		/// </remarks>
		public static bool ForceWmPaintInPrintWindow
		{
			get { return forceWmPaintInPrintWindow; }
			set { forceWmPaintInPrintWindow = value; }
		}

		/// <summary>
		/// Captures the contents of a Windows Forms control using the WM_PRINT message.
		/// </summary>
		/// <param name="control">The control to be captured.</param>
		/// <returns>The bitmap with the display contents of the Windows Forms control.</returns>
		public static Bitmap PrintWindow(Control control)
		{
			// Workaround for Whidbey - WM_PRINT does not work, let's use WM_PAINT instead.
			//if (Environment.Version.Major >= 2)
			//    return PrintControl(control, NativeMethods.WM_PAINT, 0x3e, control.Size);
			//else
				return PrintControl(control, NativeMethods.WM_PRINT, 0x3e, control.Size);
		}

		/// <summary>
		/// Captures the contents of a Windows Forms control using the WM_PRINTCLIENT message.
		/// </summary>
		/// <param name="control">The control to be captured.</param>
		/// <returns>The bitmap with the display contents of the Windows Forms control.</returns>
		public static Bitmap PrintClient(Control control)
		{
			// Workaround for Whidbey - WM_PRINT does not work, let's use WM_PAINT instead.
			if (ForceWmPaintInPrintWindow)
				return PrintControl(control, NativeMethods.WM_PAINT, 0x3e, control.ClientRectangle.Size);
			else
				return PrintControl(control, NativeMethods.WM_PRINTCLIENT, 0x3e, control.ClientRectangle.Size);
		}

		/// <summary>
		/// Captures the contents of a Windows Forms control using the WM_PRINTCLIENT or WM_PRINT message.
		/// </summary>
		/// <param name="control">The control to be captured.</param>
		/// <param name="msg">WM_PRINT or WM_PRINTCLIENT.</param>
		/// <param name="flags">Flags used for the WM_PRINT message.</param>
		/// <param name="size">The size of the window.</param>
		/// <returns>The bitmap with the display contents of the Windows Forms control.</returns>
		public static Bitmap PrintControl(Control control, int msg, int flags, Size size)
		{
			IntPtr hWnd = control.Handle;
			IntPtr hWndDC = IntPtr.Zero;
			Rectangle WindowRectangle = Rectangle.Empty;
			IntPtr hCompatibleBMP = IntPtr.Zero;
			IntPtr hCompatibleDC = IntPtr.Zero;
			int WindowHeight = 0;
			int WindowWidth = 0;

			try
			{
				// Calculate the width and height of the window.
				WindowWidth = size.Width;
				WindowHeight = size.Height;

				// Get a DC for the window and create a compatible memory DC.
				hWndDC = NativeMethods.GetWindowDC(hWnd);
				hCompatibleDC = NativeMethods.CreateCompatibleDC(hWndDC);
   
				// Create a compatible bitmap and select it into the compatible DC.
				hCompatibleBMP = NativeMethods.CreateCompatibleBitmap(hWndDC, WindowWidth, WindowHeight);
				NativeMethods.SelectObject(hCompatibleDC, hCompatibleBMP);

				// Send WM_PRINT message to window specifying the memory DC as the device context
				// and setting all the PRF_??? flags.
				NativeMethods.SendMessage(hWnd, msg, hCompatibleDC, flags);

				return Image.FromHbitmap(hCompatibleBMP);
			}
			finally
			{
				NativeMethods.ReleaseDC(hWnd, hWndDC);
				NativeMethods.DeleteObject(hCompatibleBMP);
				NativeMethods.DeleteObject(hCompatibleDC);
			}
		}

		private static Point HiMetricToPixel(int x, int y)
		{
			Point point = new Point();
			point.X = (((LogPixels.X * x) + (hiMetricPerInch / 2)) / hiMetricPerInch);
			point.Y = (((LogPixels.Y * y) + (hiMetricPerInch / 2)) / hiMetricPerInch);
			return point;
		} // end of method HiMetricToPixel
            
		private static Point LogPixels 
		{ 
			get
			{
				IntPtr hDC;
				if (logPixels.IsEmpty) 
				{
					logPixels = new Point();
					hDC = NativeMethods.GetDC(IntPtr.Zero);
					logPixels.X = NativeMethods.GetDeviceCaps(hDC, NativeMethods.LOGPIXELSX); // 88/*0x58*/
					logPixels.Y = NativeMethods.GetDeviceCaps(hDC, NativeMethods.LOGPIXELSY); // 90/*0x5a*/
					NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
				}
				return logPixels;
			} // end of method get_LogPixels
		}

		/// <summary>
		/// Temporarily resizes the control without updating the screen. Call <see cref="EndResizeNoPaint"/>
		/// to switch the control back to regular behavior.
		/// </summary>
		/// <param name="c">The target control.</param>
		/// <param name="size">The new size.</param>
		/// <param name="bounds">The saved bounds of the control.</param>
		public static void BeginResizeNoPaint(Control c, Size size, out Rectangle bounds)
		{
			bounds = c.Bounds;
			NativeMethods.SendMessage(c.Handle, 11/*0xb WM_SETREDRAW*/, 0, 0);
			c.Location = new Point(-10000, -10000);
			c.Size = size;
			NativeMethods.SendMessage(c.Handle, 11/*0xb WM_SETREDRAW*/, 1, 0);
		}

		/// <summary>
		/// Ends temporary resizing of a control after you called <see cref="BeginResizeNoPaint"/>.
		/// </summary>
		/// <param name="c">The target control.</param>
		/// <param name="savedBounds">The saved bounds of the control.</param>
		public static void EndResizeNoPaint(Control c, Rectangle savedBounds)
		{
			NativeMethods.SendMessage(c.Handle, 11/*0xb WM_SETREDRAW*/, 0, 0);
			c.Size = savedBounds.Size;
			c.Location = savedBounds.Location;
			NativeMethods.SendMessage(c.Handle, 11/*0xb WM_SETREDRAW*/, 1, 0);
		}

		/// <summary>
		/// Sends a WM_LBUTTONDOWN and WM_LBUTTONUP message to the control at the specified client coordinates.
		/// </summary>
		/// <param name="c">The target control.</param>
		/// <param name="point">The client coordinates where to simulate the click.</param>
		public static void FakeLeftMouseClick(Control c, Point point)
		{
			IntPtr lParam = NativeMethods.Util.MAKELPARAM(point.X, point.Y);
			IntPtr wParam = (IntPtr) NativeMethods.MK_LBUTTON; //(int) e.Button;
			NativeMethods.PostMessage(c.Handle, NativeMethods.WM_LBUTTONDOWN, wParam, lParam);
			NativeMethods.PostMessage(c.Handle, NativeMethods.WM_LBUTTONUP, wParam, lParam);
		}
	}
}
