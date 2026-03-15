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

using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
using System.IO;


#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents an Excel chart.
  /// </summary>
  public interface IChart : ITabSheet
  {
    /// <summary>
    /// Type of the chart.
    /// </summary>
    ExcelChartType      ChartType { get; set; }
    /// <summary>
    /// DataRange for the chart series.
    /// </summary>
    IRange              DataRange { get; set; }
    /// <summary>
    /// True if series are in rows in DataRange;
    /// False otherwise.
    /// </summary>
    bool                IsSeriesInRows { get; set; }
    /// <summary>
    /// Title of the chart.
    /// </summary>
    string              ChartTitle { get; set; }
    /// <summary>
    /// Gets title text area. Read-only.
    /// </summary>
    IChartTextArea      ChartTitleArea { get; }
    /// <summary>
    /// Page setup for the chart. Read-only.
    /// </summary>
    IChartPageSetup     PageSetup { get; }

    /// <summary>
    /// X coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    double              XPos   { get; set; }
    /// <summary>
    /// Y coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    double              YPos   { get; set; }
    /// <summary>
    /// Width of the chart in points (1/72 inch).
    /// </summary>
    double              Width  { get; set; }
    /// <summary>
    /// Height of the chart in points (1/72 inch).
    /// </summary>
    double              Height { get; set; }
    /// <summary>
    /// Collection of the all series of this chart. Read-only.
    /// </summary>
    IChartSeries        Series { get; }
    /// <summary>
    /// Primary category axis. Read-only.
    /// </summary>
    IChartCategoryAxis  PrimaryCategoryAxis { get; }
    /// <summary>
    /// Primary value axis. Read-only.
    /// </summary>
    IChartValueAxis     PrimaryValueAxis { get; }
    /// <summary>
    /// Primary series axis. Read-only.
    /// </summary>
    IChartSeriesAxis    PrimarySerieAxis { get; }
    /// <summary>
    /// Secondary category axis. Read-only.
    /// </summary>
    IChartCategoryAxis  SecondaryCategoryAxis { get; }
    /// <summary>
    /// Secondary value axis. Read-only.
    /// </summary>
    IChartValueAxis     SecondaryValueAxis { get; }
    /// <summary>
    /// Returns an object that represents the complete chart area for the chart. Read-only.
    /// </summary>
    IChartFrameFormat ChartArea { get; }
    /// <summary>
    /// Returns plot area frame format. Read-only.
    /// </summary>
    IChartFrameFormat PlotArea { get; }
    /// <summary>
    /// Represents chart walls. Read-only.
    /// </summary>
    IChartWallOrFloor Walls { get; }
    /// <summary>
    /// Represents chart BackWall. Read-only.
    /// </summary>
    IChartWallOrFloor SideWall { get; }
    /// <summary>
    /// Represents chart BackWall. Read-only.
    /// </summary>
    IChartWallOrFloor BackWall { get; }
    /// <summary>
    /// Represents chart floor. Read-only.
    /// </summary>
    IChartWallOrFloor Floor { get; }
    /// <summary>
    /// Represents charts dataTable object.
    /// </summary>
    IChartDataTable DataTable { get; }
    /// <summary>
    /// True if the chart has a data table.
    /// </summary>
    bool HasDataTable { get; set; }
    /// <summary>
    /// Represents chart legend.
    /// </summary>
    IChartLegend Legend { get; }
    /// <summary>
    /// True if the chart has a legend object.
    /// </summary>
    bool HasLegend { get; set; }
    /// <summary>
    /// Returns or sets the rotation of the 3-D chart view
    /// (the rotation of the plot area around the z-axis, in degrees).(0 to 360 degrees).
    /// </summary>
    int Rotation { get; set; }
    /// <summary>
    /// Returns or sets the elevation of the 3-D chart view, in degrees (�90 to +90 degrees).
    /// </summary>
    int  Elevation{ get; set; }
    /// <summary>
    /// Returns or sets the perspective for the 3-D chart view (0 to 100).
    /// </summary>
    int Perspective { get; set; }
    /// <summary>
    ///Returns or sets the height of a 3-D chart as a percentage of the chart width
    /// (between 5 and 500 percent).
    /// </summary>
    int HeightPercent { get; set; }
    /// <summary>
    /// Returns or sets the depth of a 3-D chart as a percentage of the chart width
    /// (between 20 and 2000 percent).
    /// </summary>
    int DepthPercent { get; set; }
    /// <summary>
    /// Returns or sets the distance between the data series in a 3-D chart, as a percentage of the marker width.( 0 - 500 )
    /// </summary>
    int GapDepth { get; set; }
    /// <summary>
    /// True if the chart axes are at right angles, independent of chart rotation or elevation.
    /// </summary>
    bool   RightAngleAxes { get; set; }
    /// <summary>
    /// True if Microsoft Excel scales a 3-D chart so that it's closer in size to the equivalent 2-D chart.
    /// </summary>
    bool   AutoScaling { get; set; }
    /// <summary>
    /// True if gridlines are drawn two-dimensionally on a 3-D chart.
    /// </summary>
    bool   WallsAndGridlines2D { get; set; }
    /// <summary>
    /// Indicates whether chart has plot area.
    /// </summary>
    bool HasPlotArea { get; set; }
    /// <summary>
    /// Represents the way that blank cells are plotted on a chart.
    /// </summary>
    ExcelChartPlotEmpty DisplayBlanksAs { get; set; }
    /// <summary>
    /// True if only visible cells are plotted. False if both visible and hidden cells are plotted.
    /// </summary>
    bool PlotVisibleOnly { get; set; }
    /// <summary>
    /// True if Microsoft Excel resizes the chart to match the size of the chart sheet window.
    /// False if the chart size isn't attached to the window size. Applies only to chart sheets.
    /// </summary>
    bool SizeWithWindow { get; set; }
    /// <summary>
    /// Gets or sets the pivot source.
    /// </summary>
    /// <value>The pivot source.</value>
    IPivotTable PivotSource { get; set; }
    /// <summary>
    /// Gets or sets the type of the pivot chart.
    /// </summary>
    /// <value>The type of the pivot chart.</value>
    ExcelChartType PivotChartType { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [show all field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show all field buttons]; otherwise, <c>false</c>.
    /// </value>
    bool ShowAllFieldButtons { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [show value field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show value field buttons]; otherwise, <c>false</c>.
    /// </value>
    bool ShowValueFieldButtons { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [show axis field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show axis field buttons]; otherwise, <c>false</c>.
    /// </value>
    bool ShowAxisFieldButtons { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [show legend field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show legend field buttons]; otherwise, <c>false</c>.
    /// </value>
    bool ShowLegendFieldButtons { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [show report filter field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show report filter field buttons]; otherwise, <c>false</c>.
    /// </value>
    bool ShowReportFilterFieldButtons { get; set; }
    /// <summary>
    /// Collection of the all categories of this chart. Read-only.
    /// </summary>
    IChartCategories Categories { get; }
    /// <summary>
    /// Represents the series name filter option
    /// </summary>
    ExcelSeriesNameLevel SeriesNameLevel { get; set; }
    /// <summary>
    /// Represents the category name filter option
    /// </summary>
    ExcelCategoriesLabelLevel CategoryLabelLevel { get; set; }
#if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    /// <summary>
    /// Converts the XlsIO chart to Image as stream.
    /// </summary>
    /// <param name="stream">stream in where the image is streamed.</param>
    void SaveAsImage(Stream imageAsStream);
#endif
  }
}
