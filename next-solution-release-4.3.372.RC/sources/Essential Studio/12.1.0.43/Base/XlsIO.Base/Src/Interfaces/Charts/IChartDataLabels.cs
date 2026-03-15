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
	/// Represents the chart data labels properties.
	/// </summary>
	public interface IChartDataLabels : IChartTextArea
	{
    /// <summary>
    /// Indicates whether series name is in data labels.
    /// </summary>
    bool IsSeriesName { get; set; }
    /// <summary>
    /// Indicates whether category name is in data labels.
    /// </summary>
    bool IsCategoryName { get; set; }
    /// <summary>
    /// Indicates whether value is in data labels.
    /// </summary>
    bool IsValue { get; set; }
    /// <summary>
    /// Indicates whether percentage is in data labels.
    /// </summary>
    bool IsPercentage { get; set; }
    /// <summary>
    /// Indicates whether bubble size is in data labels.
    /// </summary>
    bool IsBubbleSize { get; set; }
    /// <summary>
    /// Delimiter.
    /// </summary>
    string Delimiter{ get; set; }
    /// <summary>
    /// Indicates whether legend key is in data labels.
    /// </summary>
    bool IsLegendKey { get; set; }
    /// <summary>
    /// Indicates whether Leader Lines is in data labels.
    /// </summary>
    bool ShowLeaderLines { get; set; }
    /// <summary>
    /// Represents data labels position.
    /// </summary>
    ExcelDataLabelPosition Position { get; set; }
  }
}
