#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// WinAPi functions.
    /// </summary>
    internal sealed class GdiApi
    {
        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="GdiApi"/> class.
        /// </summary>
        private GdiApi()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Class GDI methods
        /// <summary>
        /// Adds the font resource from the specified file to the system font table.
        /// The font can subsequently be used for text output by any application.
        /// </summary>
        /// <param name="lpszFilename">String that contains a valid font file name.</param>
        /// <returns>If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int AddFontResource(string lpszFilename);
        /// <summary>
        /// Removes the fonts in the specified file from the system font table.
        /// </summary>
        /// <param name="lpFileName">String that names a font resource file.</param>
        /// <returns>If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool RemoveFontResource(string lpFileName);
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
        /// Retrieves the widths, in logical coordinates,
        /// of consecutive characters in a specified range from the current font.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="iFirstChar">Specifies the first character in
        /// the group of consecutive characters. </param>
        /// <param name="iLastChar">Specifies the last character in
        /// the group of consecutive characters,
        /// which must not precede the specified first character. </param>
        /// <param name="lpBuffer">Pointer to a buffer that receives
        /// the character widths, in logical coordinates.</param>   
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetCharWidth32")]
        internal static extern bool GetCharWidth(IntPtr hdc, int iFirstChar,
            int iLastChar, int[] lpBuffer);
        /// <summary>
        /// Computes the width and height of the specified string of text.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="lpString">Pointer to a buffer that specifies the text string.</param>
        /// <param name="cbString">Specifies the length of the lpString buffer.</param>
        /// <param name="lpSize">Pointer to a size structure that receives the dimensions of the string
        /// in logical units.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32")]
        internal static extern bool GetTextExtentPoint(IntPtr hdc, string lpString,
            int cbString, ref Size lpSize);
        /// <summary>
        /// Computes the width and height of the specified string of text.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="uFirstChar">Specifies the first character in the group of
        /// consecutive characters from the current font.</param>  
        /// <param name="uLastChar">Specifies the last character in the group of
        /// consecutive characters from the current font. </param>
        /// <param name="lpabc">Pointer to an array of ABC structures that receives
        /// the character widths, in logical units.</param>
        /// <returns>The function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool GetCharABCWidths(IntPtr hdc, int uFirstChar,
            int uLastChar, ref ABC lpabc);
        /// <summary>
        /// The SetTextColor function sets the text color for the specified
        /// device context to the specified color.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="crColor">Specifies the color of the text.</param>
        /// <returns>If the function succeeds, the return value is
        /// a color reference for the previous text color as a COLORREF value.</returns>   
        [DllImport("gdi32.dll")]
        internal static extern int SetTextColor(IntPtr hdc, int crColor);
        /// <summary>
        /// The SetTextAlign function sets the text-alignment flags for
        /// the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="fMode">Specifies the text alignment by using a mask of values.</param>
        /// <returns>If the function succeeds, the return value is the previous
        /// text-alignment setting.</returns>       
        [DllImport("gdi32.dll")]
        internal static extern int SetTextAlign(IntPtr hdc, int fMode);
        /// <summary>
        /// The SetBkColor function sets the current background color to
        /// the specified color value or to the nearest physical color
        /// if the device cannot represent the specified color value.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="crColor">Specifies the new background color.</param>
        /// <returns>If the function succeeds, the return value specifies
        /// the previous background color as a COLORREF value. </returns>   
        [DllImport("gdi32.dll")]
        internal static extern int SetBkColor(IntPtr hdc, int crColor);
        /// <summary>
        /// The SaveDC function saves the current state of the specified
        /// device context (DC)
        /// </summary>
        /// <param name="hdc">Handle to the DC whose state is to be saved.</param>
        /// <returns>If the function succeeds, the return value identifies
        /// the saved state. </returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SaveDC(IntPtr hdc);
        /// <summary>
        /// The GetTextColor function retrieves the current text color
        /// for the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context. </param>
        /// <returns>If the function succeeds, the return value is
        /// the current text color as a COLORREF value.</returns>   
        [DllImport("gdi32.dll")]
        internal static extern int GetTextColor(IntPtr hdc);
        /// <summary>   
        /// The GetBkColor function returns the current background color
        /// for the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context whose
        /// background color is to be returned. </param>
        /// <returns>If the function succeeds, the return value is
        /// a COLORREF value for the current background color.</returns>   
        [DllImport("gdi32.dll")]
        internal static extern int GetBkColor(IntPtr hdc);
        /// <summary>
        /// The GetTextAlign function retrieves the text-alignment
        /// setting for the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <returns>The function succeeds, the return value is the status of
        /// the text-alignment flags.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetTextAlign(IntPtr hdc);
        /// <summary>
        /// The RestoreDC function restores a device context (DC) to the specified state
        /// </summary>
        /// <param name="hdc">Handle to the DC.</param>
        /// <param name="nSavedDC">Specifies the saved state to be restored.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr RestoreDC(IntPtr hdc, int nSavedDC);
        /// <summary>
        /// The LPtoDP function converts logical coordinates into device coordinates.
        /// </summary>
        /// <param name="hdc">Handle to device context.</param>
        /// <param name="lpPoints">Pointer to an array of POINT structures.</param>
        /// <param name="nCount">Specifies the number of points in the array.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr LPtoDP(IntPtr hdc, [In, Out] POINT[] lpPoints, int nCount);
        /// <summary>
        /// The LPtoDP function converts logical coordinates into logical coordinates.
        /// </summary>
        /// <param name="hdc">Handle to device context.</param>
        /// <param name="lpPoints">Pointer to an array of POINT structures.</param>
        /// <param name="nCount">Specifies the number of points in the array.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr DPtoLP(IntPtr hdc, [In, Out] POINT[] lpPoints, int nCount);
        /// <summary>
        /// The CreateIC function creates an information context for the specified device.
        /// </summary>
        /// <param name="lpszDriver">Driver name.</param>
        /// <param name="lpszDevice">Device name.</param>
        /// <param name="lpszOutput">Port or file name.</param>
        /// <param name="lpdvmInit">Optional initialization data.</param>
        /// <returns>If the function succeeds, the return value is
        /// the handle to an information context.</returns>   
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateIC(string lpszDriver, string lpszDevice,
            string lpszOutput, IntPtr lpdvmInit);
        /// <summary>
        /// The SetWindowExtEx function sets the horizontal and vertical extents of
        /// the window for a device context by using the specified values.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="nXExtent">Specifies the window's horizontal extent in
        /// logical units.</param>
        /// <param name="nYExtent">Specifies the window's vertical extent in
        /// logical units.</param>
        /// <param name="lpSize">Pointer to a size structure that receives
        /// the previous window extents, in logical units.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern bool SetWindowExtEx(IntPtr hdc, int nXExtent, int nYExtent, ref SIZE lpSize);

        /// <summary>
        /// The SetWindowOrgEx function specifies which window point maps
        /// to the viewport origin (0,0).
        /// </summary>
        /// <param name="hdc">Handle to the device context. </param>
        /// <param name="X">Specifies the X coordinate in logical units
        /// of the new window origin.</param>   
        /// <param name="Y">Specifies the Y coordinate in logical units
        /// of the new window origin.</param>   
        /// <param name="lpPoint">Pointer to a point structure that receives
        /// the previous origin of the window, in logical units.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool SetWindowOrgEx(IntPtr hdc, int X, int Y,
            ref POINT lpPoint);
        /// <summary>
        /// The SetViewportExtEx function sets the horizontal and vertical extents
        /// of the viewport for a device context by using the specified values.
        /// </summary>
        /// <param name="hdc">Handle to the device context. </param>
        /// <param name="nXExtent">Specifies the horizontal extent
        /// in device units of the viewport.</param>
        /// <param name="nYExtent">Specifies the vertical extent
        /// in device units of the viewport.</param>   
        /// <param name="lpSize">Pointer to a size structure that
        /// receives the previous viewport extents in device units.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool SetViewportExtEx(IntPtr hdc, int nXExtent, int nYExtent,
            ref SIZE lpSize);
        /// <summary>
        /// The SetViewportOrgEx function specifies which device point maps
        /// to the window origin (0,0).
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="X">Specifies the x-coordinate, in device units, 
        /// of the new viewport origin.</param>
        /// <param name="Y">Specifies the Y coordinate in device units
        /// of the new viewport origin.</param>   
        /// <param name="lpPoint">Pointer to a point structure that receives
        /// the previous viewport origin in device coordinates.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, ref POINT lpPoint);
        /// <summary>
        /// The ScaleWindowExtEx function modifies the window for a device context using
        /// the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="Xnum">Specifies the amount by which to multiply
        /// the current horizontal extent.</param>
        /// <param name="Xdenom">Specifies the amount by which to divide
        /// the current horizontal extent.</param>
        /// <param name="Ynum">Specifies the amount by which to multiply
        /// the current vertical extent</param>
        /// <param name="Ydenom">Specifies the amount by which to divide
        /// the current vertical extent</param>
        /// <param name="lpSize">Pointer to a size structure that receives
        /// the previous window extents in logical units.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool ScaleWindowExtEx(IntPtr hdc, int Xnum, int Xdenom,
            int Ynum, int Ydenom, ref SIZE lpSize);
        /// <summary>
        /// The ScaleViewportExtEx function modifies the viewport for a device context
        /// using the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="hdc">Handle to the device context. </param>
        /// <param name="Xnum">Specifies the amount by which to multiply
        /// the current horizontal extent.</param>
        /// <param name="Xdenom">Specifies the amount by which to divide
        /// the current horizontal extent.</param>
        /// <param name="Ynum">Specifies the amount by which to multiply
        /// the current vertical extent.</param>
        /// <param name="Ydenom">Specifies the amount by which to divide
        /// the current vertical extent.</param>
        /// <param name="lpSize">Pointer to a size structure that receives
        /// the previous viewport extents in device units.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool ScaleViewportExtEx(IntPtr hdc, int Xnum, int Xdenom,
            int Ynum, int Ydenom, ref SIZE lpSize);
        /// <summary>
        /// The SetMapMode function sets the mapping mode of the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to device context.</param>
        /// <param name="fnMapMode">New mapping mode.</param>
        /// <returns>If the function succeeds, the return value identifies
        /// the previous mapping mode.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int SetMapMode(IntPtr hdc, int fnMapMode);
        /// <summary>
        /// The GetGraphicsMode function retrieves the current graphics mode
        /// for the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to device context.</param>
        /// <returns>If the function succeeds, the return value is the current graphics mode.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetGraphicsMode(IntPtr hdc);
        /// <summary>
        /// The GetDeviceCaps function retrieves device-specific information
        /// for the specified device.
        /// </summary>
        /// <param name="hdc">Handle to the DC.</param>
        /// <param name="nIndex">Specifies the item to return. </param>
        /// <returns>The return value specifies the value of the desired item. </returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        /// <summary>
        /// The GetDC function retrieves a handle to a display device context (DC)
        /// for the client area of a specified window or for the entire screen.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose DC is to be retrieved</param>
        /// <returns>If the function succeeds, the return value is a handle to the DC
        /// for the specified window's client area.</returns>   
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);
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
        /// <summary>
        /// The ModifyWorldTransform function changes the world transformation for a device context using the specified mode.
        /// </summary>
        /// <param name="hdc">handle to device context.</param>
        /// <param name="lpXform">transformation data.</param>
        /// <param name="iMode">The modification mode.</param>
        /// <returns>modification mode.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool ModifyWorldTransform(IntPtr hdc, ref XFORM lpXform, int iMode);
        /// <summary>
        /// The SetWorldTransform function sets the world transformation for a device context using the specified mode.
        /// </summary>
        /// <param name="hdc">handle to device context.</param>
        /// <param name="lpXform">transformation data.</param>
        /// <returns>modification mode.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool SetWorldTransform(IntPtr hdc, ref XFORM lpXform);
        /// <summary>
        /// The GetMapMode function retrieves the current mapping mode.
        /// </summary>
        /// <param name="hdc">handle to device context.</param>
        /// <returns>If the function succeeds, the return value specifies the mapping mode.</returns>
        [DllImport("gdi32.dll")]
        internal extern static int GetMapMode(IntPtr hdc);
        /// <summary>
        /// The SetMiterLimit function sets the limit for the length of miter joins for the specified device context.
        /// </summary>
        /// <param name="hdc">handle to DC</param>
        /// <param name="eNewLimit">new miter limit</param>
        /// <param name="peOldLimit">previous miter limit</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal extern static bool SetMiterLimit(IntPtr hdc, float eNewLimit, out float peOldLimit);
        /// <summary>
        /// The GetMiterLimit function retrieves the miter limit for the specified device context.
        /// </summary>
        /// <param name="hdc">handle to DC</param>
        /// <param name="peLimit">miter limit</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool GetMiterLimit(IntPtr hdc, out float peLimit);
        /// <summary>
        /// The SetPolyFillMode function sets the polygon fill mode for functions that fill polygons.
        /// </summary>
        /// <param name="hdc">handle to DC</param>
        /// <param name="iPolyFillMode">polygon fill mode</param>
        /// <returns>The return value specifies the previous filling mode. If an error occurs, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int SetPolyFillMode(IntPtr hdc, int iPolyFillMode);
        /// <summary>
        /// The GetPolyFillMode function retrieves the current polygon fill mode.
        /// </summary>
        /// <param name="hdc">handle to DC</param>
        /// <returns>The return value specifies the filling mode. If an error occurs, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetPolyFillMode(IntPtr hdc);
        /// <summary>
        /// The SetGraphicsMode function sets the graphics mode for the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="iMode">Specifies the graphics mode.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern int SetGraphicsMode(IntPtr hdc, int iMode);

        /// <summary>
        /// The BeginPath function opens a path bracket in the specified device context.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool BeginPath(IntPtr hdc);
        /// <summary>
        /// The MoveToEx function updates the current position to the specified point and optionally returns the previous position.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="X">Specifies the x-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="Y">Specifies the y-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="lpPoint">Pointer to a POINT structure that receives the previous current position.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool MoveToEx(IntPtr hdc, int X, int Y, ref POINT lpPoint);
        /// <summary>
        /// The LineTo function draws a line from the current position up to, but not including, the specified point.
        /// </summary>
        /// <param name="hdc"> Handle to a device context. </param>
        /// <param name="nXEnd">Specifies the x-coordinate, in logical units, of the line's ending point.</param>
        /// <param name="nYEnd">Specifies the y-coordinate, in logical units, of the line's ending point.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool EndPath(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool AbortPath(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool SelectClipPath(IntPtr hdc, int iMode);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetICMMode(IntPtr hDC, int iEnableICM);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int GetMetaFileBitsEx(IntPtr hmf, int nSize, byte[] lpvData);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int GetEnhMetaFileBits(IntPtr hmf, int nSize, byte[] lpvData);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SetMetaFileBitsEx(uint nSize, byte[] lpData);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern uint GetWinMetaFileBits(IntPtr hemf, uint cbBuffer, byte[] lpbBuffer, int fnMapMode, IntPtr hdcRef);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CopyMetaFile(IntPtr hmfSrc, string lpszFile);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SetWinMetaFileBits(int cbBuffer, byte[] lpbBuffer, IntPtr hdcRef, ref METAFILEPICT lpmfp);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteEnhMetaFile(IntPtr hemf);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetArcDirection(IntPtr hdc, int ArcDirection);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int GetArcDirection(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetBkMode(IntPtr hdc, int iBkMode);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool AngleArc(IntPtr hdc, int X, int Y, int dwRadius, float eStartAngle, float eSweepAngle);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool Chord(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nXRadial1, int nYRadial1, int nXRadial2, int nYRadial2);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool ArcTo(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nXRadial1, int nYRadial1, int nXRadial2, int nYRadial2);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool Arc(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nXRadial1, int nYRadial1, int nXRadial2, int nYRadial2);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool CloseFigure(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool FillPath(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool StrokeAndFillPath(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool StrokePath(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest,
            int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth,
            int nSrcHeight, byte[] lpBits, [In] ref BITMAPINFO lpBitsInfo, int iUsage,
         uint dwRop);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth,
            int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int GetDCBrushColor(IntPtr hdc);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int GetDCPenColor(IntPtr hdc);
        /// <summary>
        /// Retrieves text metrics for TrueType fonts.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="cbData">Specifies the size in bytes of the array
        /// that receives the text metrics.</param>
        /// <param name="lpOTM">Pointer to an array of OUTLINETEXTMETRIC structures.</param>
        /// <returns>If the function succeeds, the return value is nonzero
        /// or the size of the required buffer.</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetOutlineTextMetrics")]
        internal static extern int GetOutlineTextMetricsEx(IntPtr hdc, int cbData,
            IntPtr lpOTM);
        /// <summary>
        /// Retrieves text metrics for TrueType fonts.
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="cbData">Specifies the size in bytes of the array
        /// that receives the text metrics.</param>
        /// <param name="lpOTM">Pointer to an array of OUTLINETEXTMETRIC structures.</param>
        /// <returns>If the function succeeds, the return value is nonzero
        /// or the size of the required buffer.</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetOutlineTextMetrics")]
        internal static extern int GetOutlineTextMetrics(IntPtr hdc, int cbData,
            ref OUTLINETEXTMETRIC lpOTM);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString,
            int cbString, out SIZE lpSize);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool PolyBezierTo(IntPtr hdc, POINT[] lppt, uint cCount);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool PolylineTo(IntPtr hdc, POINT[] lppt, uint cCount);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateFontIndirect(ref LOGFONT lplf);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetStretchBltMode(IntPtr hdc, int iStretchMode);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateBitmapIndirect(ref BITMAP lpbm);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateDIBitmap(IntPtr hdc, ref BITMAPINFOHEADER
            lpbmih, uint fdwInit, byte[] lpbInit, ref BITMAPINFO lpbmi,
            uint fuUsage);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateDIBitmap(IntPtr hdc, IntPtr lpbmih,
            uint fdwInit, byte[] lpbInit, IntPtr lpbmi,
            uint fuUsage);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool ExtTextOut(IntPtr hdc, int X, int Y, int fuOptions, ref RECT lprc, string lpString, int cbCount, IntPtr lpDx);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
            int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
            int yMask, uint dwRop);

        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetLayout(IntPtr hdc, int dwLayout);
        /// <summary>
        /// Exported function from Windows GDI. For more details see Windows GDI reference.
        /// </summary>
        [DllImport("gdi32.dll")]
        internal static extern int SetMetaRgn(IntPtr hdc);
        #endregion
    }
}
