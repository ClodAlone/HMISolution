#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;

using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// This class allows user to draw splitter( half-tone brush filled rectangle ).
    /// </summary>
    public sealed class HalftonePainter
    {
        private static IntPtr CreateHalftoneHBRUSH()
        {
            short[] numArray = new short[8];
            for (int i = 0; i < 8; i++)
            {
                numArray[i] = (short)(0x5555 << ((i & 1) & 0x1f));
            }

            IntPtr hBitmap = NativeMethods.CreateBitmap(8, 8, 1, 1, numArray);
            NativeMethods.LOGBRUSH logbrush = new NativeMethods.LOGBRUSH();

            logbrush.lbColor = ColorTranslator.ToWin32(Color.Black);
            logbrush.lbStyle = (int)NativeMethods.LogBrushStyle.BS_PATTERN;
            logbrush.lbHatch = hBitmap;

            IntPtr ptr2 = NativeMethods.CreateBrushIndirect(ref logbrush);
            NativeMethods.DeleteObject(hBitmap);
            return ptr2;
        }

        /// <summary>
        /// Draws splitter in specified rectangle.
        /// </summary>
        /// <param name="rectangle"> rectangle to draw splitter in 
        /// ( screen coordinates ). </param>
        public static void DrawRectangle(Rectangle rectangle)
        {
            const uint style = (int)NativeMethods.DCFlags.DCX_CACHE | (int)NativeMethods.DCFlags.DCX_LOCKWINDOWUPDATE;

            IntPtr hDC = NativeMethods.GetDCEx(IntPtr.Zero, IntPtr.Zero, style);
            IntPtr hBrush = CreateHalftoneHBRUSH();
            IntPtr hCurrentObj = NativeMethods.SelectObject(hDC, hBrush);

            NativeMethods.PatBlt(hDC, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, (int)NativeMethods.TernaryRasterOperations.PATINVERT);
            
            NativeMethods.SelectObject(hDC, hCurrentObj);
            NativeMethods.DeleteObject(hBrush);
            NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
        }
    }
}