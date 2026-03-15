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
  /// Summary description for IChartSerie.
  /// </summary>
  public interface IChartSerie
  {
    ILineStyle Border { get; set; }
    IArea   Area { get; set; }
    ISerieDataLabels DataLabels { get; set; }
    IChartErrorBars YErrorBars { get; set; }
    IChartErrorBars XErrorBars { get; set; }
    bool IsPlotOnPrimary { get; set; }
    int Overlap { get; set; }
    int GapWidth { get; set; }
    bool IsVaryColors { get; set; }
    int SplitSeriesBy { get; set; }
    int InSecondPlot { get; set; }
    int SecondPlotSize { get; set; }
    bool IsSeriesLines { get; set; }
    bool IsSizeIsArea { get; set; }
    int BubbleSizeScale { get; set; }
    bool ShowNegativeBubbles { get; set; }
  }
}
