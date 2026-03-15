#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Syncfusion.UI.Xaml.Charts
{
    static public partial class WriteableBitmapExtensions
    {


        //public static int[] GetPixelsArray(DependencyObject obj)
        //{
        //    return (int[])obj.GetValue(PixelsArrayProperty);
        //}

        //public static void SetPixelsArray(DependencyObject obj, int[] value)
        //{
        //    obj.SetValue(PixelsArrayProperty, value);
        //}

        // Using a DependencyProperty as the backing store for GetPixels.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PixelsArrayProperty =
        //    DependencyProperty.RegisterAttached("PixelsArray", typeof(int[]), typeof(WriteableBitmapExtensions), new PropertyMetadata(null));

        public static void BeginWrite(this WriteableBitmap bmp)
        {
            //int[] pixels = new int[bmp.PixelWidth * bmp.PixelHeight];
            bmp.Lock();
        }

        //public static int[] GetPixels(this WriteableBitmap bmp)
        //{
        //    return PixelsArray(bmp);
        //}

        public unsafe static int* GetPixels(this WriteableBitmap bmp)
        {
            return (int*)bmp.BackBuffer;
        }

        public static void EndWrite(this WriteableBitmap bmp)
        {
            bmp.AddDirtyRect(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            bmp.Unlock();
        }

        public static int GetLength(this WriteableBitmap bmp)
        {
            double pixelWidth = bmp.BackBufferStride / 4;
            double pixelHeight = bmp.PixelHeight;
            return (int)(pixelWidth * pixelHeight);
        }

        /// <summary>
        /// Clears the BitmapContext, filling the underlying bitmap with zeros
        /// </summary>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static void Clear(this WriteableBitmap bmp)
        {
            NativeMethods.SetUnmanagedMemory(bmp.BackBuffer, 0, bmp.BackBufferStride * bmp.PixelHeight);
        }
    }

    internal static class NativeMethods
    {
        [TargetedPatchingOptOut("Internal method only, inlined across NGen boundaries for performance reasons")]
        internal static void SetUnmanagedMemory(IntPtr dst, int filler, int count)
        {
            memset(dst, filler, count);
        }

        // Win32 memory set function
        //[DllImport("ntdll.dll")]
        //[DllImport("coredll.dll", EntryPoint = "memset", SetLastError = false)]
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void memset(
            IntPtr dst,
            int filler,
            int count);
    }
}
