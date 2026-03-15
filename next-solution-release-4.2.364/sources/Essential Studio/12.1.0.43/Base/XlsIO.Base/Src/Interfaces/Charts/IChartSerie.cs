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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the chart Series.
  /// </summary>
  public interface IChartSerie
    : IParentApplication
  {
    #region Class properties
    /// <summary>
    /// Values range for the series.
    /// </summary>
    IRange        Values { get; set; }
    /// <summary>
    /// Category labels for the series.
    /// </summary>
    IRange        CategoryLabels { get; set; }
    /// <summary>
    /// Bubble sizes for the series.
    /// </summary>
    IRange        Bubbles { get; set; }
    /// <summary>
    /// Name of the series.
    /// </summary>
    string        Name { get; set; }
    /// <summary>
    /// Series Name range for the series.
    /// </summary>    
    IRange        NameRange { get; }
    /// <summary>
    /// Indicates whether to use primary axis for series drawing.
    /// </summary>
    bool UsePrimaryAxis { get; set; }
    /// <summary>
    /// Returns collection of data points. Read-only.
    /// </summary>
    IChartDataPoints DataPoints { get; }
    /// <summary>
    /// Returns format of current series.
    /// </summary>
    IChartSerieDataFormat SerieFormat { get; }
    /// <summary>
    /// Represents series type.
    /// </summary>
    ExcelChartType SerieType { get; set; }
    /// <summary>
    /// Represents value as entered directly.
    /// </summary>
    object[] EnteredDirectlyValues { get; set; }
    /// <summary>
    /// Represents category values as entered directly.
    /// </summary>
    object[] EnteredDirectlyCategoryLabels { get; set; }
    /// <summary>
    /// Represents bubble values as entered directly.
    /// </summary>
    object[] EnteredDirectlyBubbles{ get; set; }
    /// <summary>
    /// Represents Y error bars. Read-only.
    /// </summary>
    IChartErrorBars ErrorBarsY { get; }
    /// <summary>
    /// Indicates if series contains Y error bars.
    /// </summary>
    bool HasErrorBarsY { get; set; }
    /// <summary>
    /// Represents X error bars. Read-only.
    /// </summary>
    IChartErrorBars ErrorBarsX{ get; }
    /// <summary>
    /// Indicates if series contains X error bars.
    /// </summary>
    bool HasErrorBarsX{ get; set; }
    /// <summary>
    /// Represents series trend lines collection. Read-only.
    /// </summary>
    IChartTrendLines TrendLines { get; }
    /// <summary>
    /// Represent the series Filter.
    /// </summary>
    bool IsFiltered { get; set; }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <returns>Return error bar object.</returns>
    IChartErrorBars ErrorBar( bool bIsY );
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <returns>Return error bar object.</returns>
    IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include );
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <param name="type">Represents error bar type.</param>
    /// <returns>Return error bar object.</returns>
    IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include
      , ExcelErrorBarType type );
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <param name="type">Represents error bar type.</param>
    /// <param name="numberValue">Represents number value.</param>
    /// <returns>Return error bar object.</returns>
    IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include
      , ExcelErrorBarType type, double numberValue );
    /// <summary>
    /// Sets custom error bar type.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="plusRange">Represents plus range.</param>
    /// <param name="minusRange">Represents minus range.</param>
    /// <returns>Returns error bar object.</returns>
    IChartErrorBars ErrorBar( bool bIsY, IRange plusRange, IRange minusRange );
    #endregion
  }
}
