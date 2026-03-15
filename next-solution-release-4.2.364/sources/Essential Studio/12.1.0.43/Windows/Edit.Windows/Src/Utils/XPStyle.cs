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
using System.Runtime.InteropServices;
using System.Drawing;


namespace Syncfusion.Windows.Forms.Edit
{
	/// <summary>
	/// Provides static methods for drawing XP style visual themes.
	/// </summary>
	internal class XPStyle
	{
    #region Structs & Enums
    /// <summary>
    /// Summary description for RECT.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)] 
    public struct RECT
    {
      [FieldOffset(0)] public int Left;

      [FieldOffset(4)] public int Top;

      [FieldOffset(8)] public int Right;

      [FieldOffset(12)] public int Bottom;

      public RECT(int left, int top, int right, int bottom) 
      {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
      }

      public RECT(Rectangle rect) 
      {
        Left = rect.Left; 
        Top = rect.Top;
        Right = rect.Right;
        Bottom = rect.Bottom;
      }

      public Rectangle ToRectangle() 
      {
        return new Rectangle(Left, Top, Right, Bottom - 1);
      }
    }
    #endregion

    #region UxThemes.dll Imports
    /// <summary>
    /// Tests if a visual style for the current application is active.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern bool IsThemeActive();
		
    /// <summary>
    /// Opens the theme data for a window and its associated class.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern IntPtr OpenThemeData(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string classList);
		
    /// <summary>Closes the theme data handle.</summary>
    /// <remarks>The CloseThemeData function should be called when a window that has a visual style applied is destroyed.</remarks> 
    [DllImport("uxtheme.dll")]
    private static extern void CloseThemeData(IntPtr hTheme);
		
    /// <summary>
    /// Draws the background image defined by the visual style for the specified control part.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void DrawThemeBackground(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref RECT rect, ref RECT clipRect);
		
    /// <summary>
    /// Draws one or more edges defined by the visual style of a rectangle.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void DrawThemeEdge(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref RECT destRect, uint edge, uint flags, ref RECT contentRect);
		
    /// <summary>
    /// Draws an image from an image list with the icon effect defined by the visual style.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void DrawThemeIcon(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref RECT rect, IntPtr hIml, int imageIndex);
		
    /// <summary>
    /// Draws text using the color and font defined by the visual style.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void DrawThemeText(IntPtr hTheme, IntPtr hDC, int partId, int stateId, [MarshalAs(UnmanagedType.LPTStr)] string text, int charCount, uint textFlags, uint textFlags2, ref RECT rect);
		
    /// <summary>
    /// Draws the part of a parent control that is covered by a partially-transparent or alpha-blended child control.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void DrawThemeParentBackground(IntPtr hWnd, IntPtr hDC, ref RECT rect);
		
    /// <summary>
    /// Causes a window to use a different set of visual style information than its class normally uses.
    /// </summary>
    [DllImport("uxtheme.dll")]
    private static extern void SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);
    #endregion

    #region Static Methods
    /// <summary>
    /// Draws XP styled background.
    /// </summary>
    /// <param name="handle">Window handle to draw.</param>
    /// <param name="hDC">Device context to draw on.</param>
    /// <param name="rect">Area to draw background.</param>
    /// <param name="strTheme">Name of the theme.</param>
    /// <param name="partId">Id of theme part.</param>
    /// <param name="stateId">Id of theme part state.</param>
    public static void Draw( IntPtr handle, IntPtr hDC, Rectangle rect, string strTheme, int partId, int stateId )
    {
      if( !XPThemesEnabled() ) return;

      if( IntPtr.Zero == handle )
        throw new ArgumentNullException( "handle" );

      if( IntPtr.Zero == hDC )
        throw new ArgumentOutOfRangeException( "hDC" );

      if( null == strTheme )
        throw new ArgumentNullException( "strTheme" );

      if( string.Empty == strTheme )
        throw new ArgumentOutOfRangeException( "strTheme" );

      if( partId <= 0 )
        throw new ArgumentOutOfRangeException( "partId" );

      if( stateId <= 0 )
        throw new ArgumentOutOfRangeException( "stateId" );

      IntPtr theme = XPStyle.OpenThemeData( handle, strTheme );

      if( IntPtr.Zero == theme )
        throw new ArgumentOutOfRangeException( "strTheme" );

      RECT wndRect = new RECT( rect );

      DrawThemeBackground( theme, hDC, partId, stateId, ref wndRect, ref wndRect );

      CloseThemeData( theme );
    }
    /// <summary>
    /// Draws XP styled background.
    /// </summary>
    /// <param name="handle">Window handle to draw.</param>
    /// <param name="g">Graphics object to draw.</param>
    /// <param name="rect">Area to draw background.</param>
    /// <param name="strTheme">Name of the theme.</param>
    /// <param name="partId">Id of theme part.</param>
    /// <param name="stateId">Id of theme part state.</param>
    public static void Draw( IntPtr handle, Graphics g, Rectangle rect, string strTheme, int partId, int stateId )
    {
      IntPtr hDC = g.GetHdc();
      Draw( handle, hDC, rect, strTheme, partId, stateId );
      g.ReleaseHdc( hDC );
    }
    /// <summary>
    /// Checks whether OS is ready to draw XP style themes.
    /// </summary>
    /// <returns>bool indicating whether OS is ready to draw XP style themes.</returns>
    public static bool XPThemesEnabled()
    {
      PlatformID platformId = Environment.OSVersion.Platform;
      Version version = Environment.OSVersion.Version;
      Version targetVersion = new Version( "5.1.2600.0" );

      return version >= targetVersion && platformId == PlatformID.Win32NT && IsThemeActive();
    }
   #endregion
  }
}