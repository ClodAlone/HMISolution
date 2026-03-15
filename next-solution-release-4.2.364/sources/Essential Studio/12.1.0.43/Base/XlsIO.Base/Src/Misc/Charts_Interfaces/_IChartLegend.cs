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

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  /// <summary>
  /// Summary description for IChartLegend.
  /// </summary>
  public interface IChartLegend
  {
    ILineStyle Border { get; set; }
    IArea   Area { get; set; }
    IChartFont   Font { get; set; }
    ExcelLegendPosition Placement { get; set; }
    int XPos { get; set; }
    int YPos { get; set; }
    int Width { get; set; }
    int Height { get; set; }
  }
}
