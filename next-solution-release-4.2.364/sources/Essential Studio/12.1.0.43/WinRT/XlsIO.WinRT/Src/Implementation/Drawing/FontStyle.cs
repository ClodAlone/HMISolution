#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.WINRT
{
  [Flags]
  public enum FontStyle
  {
    Bold = 1,
    Italic = 2,
    Regular = 0,
    Strikeout = 8,
    Underline = 4
  }

  public enum GraphicsUnit
  {
    World,
    Display,
    Pixel,
    Point,
    Inch,
    Document,
    Millimeter
  }
}
