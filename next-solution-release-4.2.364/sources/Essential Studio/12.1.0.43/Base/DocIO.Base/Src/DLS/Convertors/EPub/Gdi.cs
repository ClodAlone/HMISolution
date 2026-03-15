#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WinAPi functions.
    /// </summary>
    internal sealed class GdiApi
    {
        #region Constructors
        /// <summary>
        /// To prevent construction of a class, we make a private constructor.
        /// </summary>
        private GdiApi()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Selects an object into the specified device context (DC).
        /// The new object replaces the previous object of the same type.
        /// </summary>
        /// <param name="hdc">Handle to the DC. </param>
        /// <param name="hgdiobj">Handle to the object to be selected.</param>
        /// <returns>If the selected object is not a region and the function succeeds,
        /// the return value is a handle to the object being replaced.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        /// <summary>
        /// Deletes a logical pen, brush, font, bitmap, region, or palette,
        /// freeing all system resources associated with the object.
        /// After the object is deleted, the specified handle is no longer valid.
        /// </summary>
        /// <param name="hdc">Handle to a logical pen, brush, font,
        /// bitmap, region, or palette.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int DeleteObject(IntPtr hdc);
        /// <summary>
        /// Retrieves font metric data for a TrueType font.
        /// </summary>
        /// <param name="hdc">Handle to the device context. </param>
        /// <param name="dwTable">Specifies the name of a font metric table 
        /// from which the font data is to be retrieved</param>
        /// <param name="dwOffset">Specifies the offset from the beginning of the font metric table
        /// to the location where the function should begin retrieving information.</param>   
        /// <param name="lpvBuffer">Pointer to a buffer that receives the font information.</param>
        /// <param name="cbData">Specifies the length in bytes of the information to be retrieved</param>
        /// <returns>If the function succeeds, the return value is the number of bytes returned.</returns>
        [DllImport("gdi32.dll")]
        internal static extern uint GetFontData(IntPtr hdc, uint dwTable,
            uint dwOffset, [In, Out] byte[] lpvBuffer, uint cbData);
        /// <summary>
        /// The CreateDC function creates a device context (DC) for a device using the specified name.
        /// </summary>
        /// <param name="lpszDriver">Driver name.</param>
        /// <param name="lpszDevice">Device name.</param>
        /// <param name="lpszOutput">Not used; should be NULL.</param>
        /// <param name="lpInitData">Optional printer data.</param>
        /// <returns>If the function succeeds, the return value is the handle to a DC for the specified device.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateDC(string lpszDriver, string lpszDevice,
            string lpszOutput, IntPtr lpInitData);

        /// <summary>
        /// The DeleteDC function deletes the specified device context (DC). 
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(IntPtr hdc);
    }
}
# endif