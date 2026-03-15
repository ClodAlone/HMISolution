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

#region file using directives
using System;
using System.Drawing;
#endregion

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  /// <summary>
  /// Summary description for IChartFont.
  /// </summary>
  public interface IChartFont
  {
    string Name { get; set; }
    int Style { get; set; } // Italic ...
    int Size { get; set; }
    int Underline { get; set; }
    Color Color { get; set; }
    bool IsAutoColor { get; set; }
    Color BackColor { get; set; }
    bool IsAutoBackColor { get; set; }
    bool IsStrikethrough { get; set; }
    bool IsSuperscript { get; set; }
    bool IsSubscript { get; set; }
    bool IsAutoScale { get; set; }
  }
}
