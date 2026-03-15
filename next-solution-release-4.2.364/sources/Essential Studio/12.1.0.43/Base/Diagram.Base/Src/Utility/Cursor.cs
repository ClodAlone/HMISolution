#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing custom cursor implementation.
    /// </summary>
    [Documentation.DocumentationExclude()]
    public class ImageCursor
    {
        #region Class members
        private static IntPtr ptr;
        public struct IconInfo
        {
            public bool bIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        #endregion

        #region Class Public Methods
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr CreateIconIndirect([System.Runtime.InteropServices.In]ref IconInfo icon);

        [System.Runtime.InteropServices.DllImport("gdi32")]
        extern internal static bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Create a custom cursor with the given bitmap
        /// </summary>
        /// <param name="bmp">Bitmap for the cursor.</param>
        /// <param name="xHotSpot">x hot spot to the cursor.</param>
        /// <param name="yHotSpot">y hot spot to the cursor.</param>
        public static System.Windows.Forms.Cursor CreateCursor(System.Drawing.Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr bmpPtr= bmp.GetHicon();
            IconInfo icon = new IconInfo();
            GetIconInfo(bmpPtr, ref icon);
            icon.xHotspot = xHotSpot;
            icon.yHotspot = yHotSpot;
            icon.bIcon = false;
            DestroyIcon(bmpPtr);
            DeleteObject(bmpPtr);
            ptr = CreateIconIndirect(ref icon);
            System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(ptr);            
            //delete the GDI objects and icon
            DeleteObject(icon.hbmColor);
            DeleteObject(icon.hbmMask);
            DestroyIcon(icon.hbmColor);
            DestroyIcon(icon.hbmMask);
            cursor.Tag = "ImageCursor";
            return cursor;
        }

        /// <summary>
        /// Destroy the custom cursor      
        /// </summary>
        public static void Destroy()
        {
            DestroyIcon(ptr);
            DeleteObject(ptr);
        }
        #endregion
    }
}
