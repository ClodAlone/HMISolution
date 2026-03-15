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
  /// Summary description for ILineStyle.
  /// </summary>
  public interface ILineStyle
  {
    ExcelAutoType LineType { get; set; }
    // TODO: maybe ExcelChartLinePattern
    ExcelLineStyle Style { get; set; }
    Color Color { get; set; }
    ExcelChartLineWeight Weight { get; set; }
  }
}
