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
using Syncfusion.Win32;
using System.Drawing;
using System.Security;
using System.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Drawing
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragRectDrawing
	{
		public static void DrawDragFBRectangle(Rectangle rcdrag)
		{	
			IntPtr hdc = NativeMethods.GetDCEx(IntPtr.Zero, IntPtr.Zero, 0x402);	//DCX_LOCKWINDOWUPDATE|DCX_CACHE
			IntPtr hbrush = CreateHalftoneHbrush();
			IntPtr hgdiobj = NativeMethods.SelectObject(hdc, hbrush);
			NativeMethods.PatBlt(hdc, rcdrag.X, rcdrag.Y, rcdrag.Width, rcdrag.Height, 0x5a0049);	//PATINVERT
			NativeMethods.SelectObject(hdc, hgdiobj);
			NativeMethods.DeleteObject(hbrush);
			NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
		}

		public static IntPtr CreateHalftoneHbrush()
		{
			int n1 = 0;			
			short[] arrclrs = (short[])new System.Int16[8];		
			while(n1 < 8)	
			{
				arrclrs[n1] = ((short) (0x5555 << ((n1 & 1) & 31)));
				n1 = (n1 + 1);
			}			
			IntPtr hbmp = NativeMethods.CreateBitmap(8, 8, 1, 1, arrclrs);
			NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
			lb.lbColor = ColorTranslator.ToWin32(Color.Black);
			lb.lbStyle = 3;
			lb.lbHatch = hbmp;
			IntPtr hbrush  = NativeMethods.CreateBrushIndirect(ref lb);
			NativeMethods.DeleteObject(hbmp);
			return hbrush;
		}
	}
}