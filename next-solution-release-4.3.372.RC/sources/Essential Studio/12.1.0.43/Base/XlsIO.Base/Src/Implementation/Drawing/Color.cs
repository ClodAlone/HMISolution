#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Media;

namespace Syncfusion.XlsIO.Implementation
{
  public static class ColorExtension
  {
#if SILVERLIGHT
    public static int ToArgb(this Color color)
    {
      return ( color.A << 24 ) + ( color.R << 16 ) + ( color.G << 8 ) + color.B;
    }
#endif

    public static Color Black = Color.FromArgb(255, 0, 0, 0);
    public static Color White = Color.FromArgb(255, 255, 255, 255);
    public static Color Empty = Color.FromArgb(0, 0, 0, 0);
    public static Color Red = Color.FromArgb(255, 255, 0, 0);
    public static Color Blue = Color.FromArgb(255, 0, 0, 255);
    public static Color DarkGray = Color.FromArgb( 255, 0x80, 0x80, 0x80 );
    public static Color Yellow = Color.FromArgb( 255, 255, 255, 0 );
    public static Color Cyan = Color.FromArgb( 255, 0, 255, 255 );
    public static Color Magenta = Color.FromArgb( 255, 255, 0, 255 );
    public static Color Gray = Color.FromArgb( 255, 192, 192, 192 );
    public static Color FromArgb(int value)
    {
      byte b = (byte)value;
      value = value >> 8;

      byte g = (byte)value;
      value = value >> 8;

      byte r = (byte)value;
      value = value >> 8;

      byte a = (byte)value;

      return Color.FromArgb(a, r, g, b);
    }
  }
}
