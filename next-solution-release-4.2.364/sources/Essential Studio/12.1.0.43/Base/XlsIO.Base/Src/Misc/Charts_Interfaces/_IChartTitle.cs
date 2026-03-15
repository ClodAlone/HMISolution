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
using System.Drawing;

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  /// <summary>
  /// Interface for chart and axis title
  /// </summary>
  public interface IChartTitle
  {
    int XPos { get; set; }
    int YPos { get; set; }
    string Text { get; set; }
    IArea Area { get; set; }
    ILineStyle Border { get; set; }
    IChartFont Font { get; set; }
    ExcelChartHorzAlignment HAlign { get; set; }
    ExcelChartVertAlignment VAlign { get; set; }
    int TextDirection { get; set; }
    int TextOrientation { get; set; }
  }
}
