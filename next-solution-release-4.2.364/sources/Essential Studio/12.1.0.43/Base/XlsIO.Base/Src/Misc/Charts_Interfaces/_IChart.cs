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
  /// Summary description for IChart.
  /// </summary>
  public interface IChart
  {
    IAxis CategoryAxis { get; }
    IAxis ValueAxis { get; }
    IAxis SerieAxis { get; }
    IAxis SecondaryCategoryAxis { get; }
    IAxis SecondaryValueAxis { get; }

    ExcelChartType ChartType { get; set; }
    IRange DataRange { get; set; }
    bool IsSeriesInRows { get; set; }
    IChartSerie[] Series { get; }
    IChartLegend Legend { get; }

    IChartArea ChartArea { get; }
    IChartTitle ChartTitle { get; }
    IChartWalls Walls { get; }
    IChartWalls Floor { get; }
    IChartPlotArea PlotArea { get; }

    IChartPageSetup PageSetup { get; }

    double XPos   { get; set; }
    double YPos   { get; set; }
    double Width  { get; set; }
    double Height { get; set; }
  }
}
