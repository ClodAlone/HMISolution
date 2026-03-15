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
	/// Summary description for IChartLegend.
	/// </summary>
	public interface IChartLegend
	{
    #region Interface properties
    /// <summary>
    /// Represents chart frame format.
    /// </summary>
    IChartFrameFormat FrameFormat { get; }
    /// <summary>
    /// Return text area of legend.
    /// </summary>
    IChartTextArea TextArea { get; }
    /// <summary>
    /// X-position of upper-left corner. 1/4000 of chart plot.
    /// </summary>
    int X { get; set; }
    /// <summary>
    /// Y-position of upper-left corner. 1/4000 of chart plot.
    /// </summary>
    int Y { get; set; }
    /// <summary>
    /// Type:
    /// 0 = bottom
    /// 1 = corner
    /// 2 = top
    /// 3 = right
    /// 4 = left
    /// 7 = not docked or inside the plot area
    /// </summary>
    ExcelLegendPosition Position { get; set; }
    /// <summary>
    /// True if vertical legend (a single column of entries);
    /// False if horizontal legend (multiple columns of entries).
    /// Manual-sized legends always have this bit set to False.
    /// </summary>
    bool IsVerticalLegend { get; set; }
    /// <summary>
    /// Represents legend entries collection. Read-only.
    /// </summary>
    IChartLegendEntries LegendEntries { get; }
    /// <summary>
    /// Show legend without overlapping. Default is True.
    /// </summary>
    bool IncludeInLayout { get; set; }
    /// <summary>
    /// Represents the Layout settings of TextArea
    /// </summary>
    IChartLayout Layout { get; set; }
    #endregion

    #region Class methods
    /// <summary>
    /// Clears chart legend.
    /// </summary>
    void Clear();
    /// <summary>
    /// Deletes chart legend.
    /// </summary>
    void Delete();
    #endregion
	}
}
