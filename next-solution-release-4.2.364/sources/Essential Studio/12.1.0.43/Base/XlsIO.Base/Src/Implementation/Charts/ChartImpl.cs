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

#define KEEP_AXES_ORDER
#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.Exceptions;

using Syncfusion.XlsIO.Interfaces;
using TRuns = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAlrunsRecord.TRuns;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif
#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// This class represents the Excel Chart object.
  /// </summary>
  public class ChartImpl
    : WorksheetBaseImpl
    , IChart
    , ISerializableNamedObject
    , IParseable
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// First Series default name.
    /// </summary>
    public const string DEF_FIRST_SERIE_NAME = "Serie1";
    /// <summary>
    /// Default chart type.
    /// </summary>
    public const ExcelChartType DEFAULT_CHART_TYPE = ExcelChartType.Column_Clustered;
    /// <summary>
    /// Prefix for 3D charts.
    /// </summary>
    public const string PREFIX_3D            = "_3D";
    /// <summary>
    /// Prefix for bar charts.
    /// </summary>
    public const string PREFIX_BAR           = "_Bar";
    /// <summary>
    /// Prefix for clustered charts.
    /// </summary>
    public const string PREFIX_CLUSTERED     = "_Clustered";
    /// <summary>
    /// Prefix for contour charts.
    /// </summary>
    public const string PREFIX_CONTOUR       = "_Contour";
    /// <summary>
    /// Prefix for exploded charts.
    /// </summary>
    public const string PREFIX_EXPLODED      = "_Exploded";
    /// <summary>
    /// Prefix for line charts.
    /// </summary>
    public const string PREFIX_LINE          = "_Line";
    /// <summary>
    /// Prefix for charts with markers.
    /// </summary>
    public const string PREFIX_MARKERS       = "_Markers";
    /// <summary>
    /// Prefix for charts with no color.
    /// </summary>
    public const string PREFIX_NOCOLOR       = "_NoColor";
    /// <summary>
    /// Prefix for 100% charts.
    /// </summary>
    public const string PREFIX_SHOW_PERCENT  = "_100";
    /// <summary>
    /// Prefix for charts with smoothed lines.
    /// </summary>
    public const string PREFIX_SMOOTHEDLINE  = "_SmoothedLine";
    /// <summary>
    /// Prefix for stacked charts.
    /// </summary>
    public const string PREFIX_STACKED       = "_Stacked";
    /// <summary>
    /// Start of the chart type for area charts.
    /// </summary>
    public const string START_AREA            = "Area";
    /// <summary>
    /// Start of the chart type for bar charts.
    /// </summary>
    public const string START_BAR             = "Bar";
    /// <summary>
    /// Start of the chart type for bubble charts.
    /// </summary>
    public const string START_BUBBLE          = "Bubble";
    /// <summary>
    /// Start of the chart type for column charts.
    /// </summary>
    public const string START_COLUMN          = "Column";
    /// <summary>
    /// Start of the chart type for cone charts.
    /// </summary>
    public const string START_CONE            = "Cone";
    /// <summary>
    /// Start of the chart type for cylinder charts.
    /// </summary>
    public const string START_CYLINDER        = "Cylinder";
    /// <summary>
    /// Start of the chart type for doughnut charts.
    /// </summary>
    public const string START_DOUGHNUT        = "Doughnut";
    /// <summary>
    /// Start of the chart type for line charts.
    /// </summary>
    public const string START_LINE            = "Line";
    /// <summary>
    /// Start of the chart type for pie charts.
    /// </summary>
    public const string START_PIE             = "Pie";
    /// <summary>
    /// Start of the chart type for pyramid charts.
    /// </summary>
    public const string START_PYRAMID         = "Pyramid";
    /// <summary>
    /// Start of the chart type for radar charts.
    /// </summary>
    public const string START_RADAR           = "Radar";
    /// <summary>
    /// Start of the chart type for scatter charts.
    /// </summary>
    public const string START_SCATTER         = "Scatter";
    /// <summary>
    /// Start of the chart type for surface charts.
    /// </summary>
    public const string START_SURFACE         = "Surface";
    /// <summary>
    /// Index of the primary axes.
    /// </summary>
    private const int DEF_PRIMARY_INDEX = 0;
    /// <summary>
    /// Si value index.
    /// </summary>
    public const int DEF_SI_VALUE = 1;
    /// <summary>
    /// Si category index.
    /// </summary>
    public const int DEF_SI_CATEGORY = 2;
    /// <summary>
    /// Si bubble index.
    /// </summary>
    public const int DEF_SI_BUBBLE = 3;
    /// <summary>
    /// Index of the secondary axes.
    /// </summary>
    private const int DEF_SECONDARY_INDEX = 1;
    /// <summary>
    /// Maximum font count.
    /// </summary>
    private const int MaximumFontCount = 506;
    /// <summary>
    /// Represents the Defualt Plot Area Top Left X.
    /// </summary>
    internal const int DefaultPlotAreaX = 328;
    /// <summary>
    /// Represents the Default Plot Area Top Left Y.
    /// </summary>
    internal const int DefaultPlotAreaY = 243;
    /// <summary>
    /// Represents the Default Plot Area X Length.
    /// </summary>
    internal const int DefaultPlotAreaXLength = 3125;
    /// <summary>
    /// Represents the Default Plot Area Y Length.
    /// </summary>
    internal const int DefaultPlotAreaYLength = 3283;
    /// <summary>
    /// Represents start types of chart that support data point.
    /// </summary>
    public static readonly string[] DEF_LEGEND_NEED_DATA_POINT =
    {
      START_PIE,
      START_DOUGHNUT,
      START_SURFACE
    };
    /// <summary>
    /// Represents types of chart that support series axis .
    /// </summary>
    public static readonly ExcelChartType[] DEF_SUPPORT_SERIES_AXIS =
    {
      ExcelChartType.Surface_3D,
      ExcelChartType.Surface_Contour,
      ExcelChartType.Surface_NoColor_Contour,
      ExcelChartType.Surface_NoColor_3D,
      ExcelChartType.Column_3D,
      ExcelChartType.Line_3D,
      ExcelChartType.Area_3D,
      ExcelChartType.Pyramid_Clustered_3D,
      ExcelChartType.Cone_Clustered_3D,
      ExcelChartType.Cylinder_Clustered_3D,
    };
    /// <summary>
    /// Represents types of chart that do not support the pivot chart.
    /// </summary>
    public static readonly ExcelChartType[] DEF_UNSUPPORT_PIVOT_CHART =
      {
          ExcelChartType.Scatter_Line,
          ExcelChartType.Scatter_Line_Markers,
          ExcelChartType.Scatter_Markers,
          ExcelChartType.Scatter_SmoothedLine,
          ExcelChartType.Scatter_SmoothedLine_Markers,
          ExcelChartType.Stock_HighLowClose,
          ExcelChartType.Stock_OpenHighLowClose,
          ExcelChartType.Stock_VolumeHighLowClose,
          ExcelChartType.Stock_VolumeOpenHighLowClose,
          ExcelChartType.Bubble,
          ExcelChartType.Bubble_3D
      };
    /// <summary>
    /// Represents start types of chart that support data table.
    /// </summary>
    public static readonly string[] DEF_SUPPORT_DATA_TABLE =
    {
      START_COLUMN,
      START_BAR,
      START_LINE,
      START_AREA,
      START_CYLINDER,
      START_CONE,
      START_PYRAMID,
      "Stock"
    };
    /// <summary>
    /// Represents start types of chart that support error bars.
    /// </summary>
    public static readonly string[] DEF_SUPPORT_ERROR_BARS =
    {
      START_COLUMN,
      START_BAR,
      START_LINE,
      START_AREA,
      START_SCATTER,
      START_BUBBLE,
    };
    /// <summary>
    /// Represents start types of chart that support trendlines.
    /// </summary>
    public static readonly ExcelChartType[] DEF_SUPPORT_TREND_LINES =
    {
      ExcelChartType.Column_Clustered,
      ExcelChartType.Bar_Clustered,
      ExcelChartType.Line,
      ExcelChartType.Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Stock_HighLowClose,
      ExcelChartType.Stock_OpenHighLowClose,
      ExcelChartType.Stock_VolumeHighLowClose,
      ExcelChartType.Stock_VolumeOpenHighLowClose,
      ExcelChartType.Area,
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D,
    };
    /// <summary>
    /// Represents types of charts that contain walls or floor objects.
    /// </summary>
    public static readonly ExcelChartType[] DEF_WALLS_OR_FLOOR_TYPES =
    {
      ExcelChartType.Column_3D,
      ExcelChartType.Column_Clustered_3D,
      ExcelChartType.Column_Stacked_100_3D,
      ExcelChartType.Column_Stacked_3D,
      ExcelChartType.Bar_Clustered_3D,
      ExcelChartType.Bar_Stacked_3D,
      ExcelChartType.Bar_Stacked_100_3D,
      ExcelChartType.Line_3D,
      ExcelChartType.Area_3D,
      ExcelChartType.Area_Stacked_3D,
      ExcelChartType.Area_Stacked_100_3D,
      ExcelChartType.Cylinder_Clustered,
      ExcelChartType.Cylinder_Stacked,
      ExcelChartType.Cylinder_Stacked_100,
      ExcelChartType.Cylinder_Bar_Clustered,
      ExcelChartType.Cylinder_Bar_Stacked,
      ExcelChartType.Cylinder_Bar_Stacked_100,
      ExcelChartType.Cylinder_Clustered_3D,
      ExcelChartType.Cone_Clustered,
      ExcelChartType.Cone_Stacked,
      ExcelChartType.Cone_Stacked_100,
      ExcelChartType.Cone_Bar_Clustered,
      ExcelChartType.Cone_Bar_Stacked,
      ExcelChartType.Cone_Bar_Stacked_100,
      ExcelChartType.Cone_Clustered_3D,
      ExcelChartType.Pyramid_Clustered,
      ExcelChartType.Pyramid_Stacked,
      ExcelChartType.Pyramid_Stacked_100,
      ExcelChartType.Pyramid_Bar_Clustered,
      ExcelChartType.Pyramid_Bar_Stacked,
      ExcelChartType.Pyramid_Bar_Stacked_100,
      ExcelChartType.Pyramid_Clustered_3D,
      ExcelChartType.Surface_3D,
      ExcelChartType.Surface_NoColor_3D
    };
    /// <summary>
    /// Specifies default secondary axis types.
    /// </summary>
    private static readonly ExcelAxisType[] DEF_SECONDARY_AXES_TYPES = new ExcelAxisType[]
    {
      ExcelAxisType.Category, ExcelAxisType.Value
    };
    /// <summary>
    /// Represents types of charts that can't be 3d.
    /// </summary>
    public static readonly ExcelChartType[] DEF_NOT_3D =
    {
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Radar,
      ExcelChartType.Radar_Markers,
      ExcelChartType.Radar_Filled,
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D,
      ExcelChartType.Stock_HighLowClose,
      ExcelChartType.Stock_OpenHighLowClose,
      ExcelChartType.Stock_VolumeHighLowClose,
      ExcelChartType.Stock_VolumeOpenHighLowClose,
      ExcelChartType.Combination_Chart
    };
    /// <summary>
    /// Represents types of charts that are not 3d.
    /// </summary>
    public static readonly ExcelChartType[] DEF_CHANGE_SERIE =
    {
      ExcelChartType.Column_Clustered,
      ExcelChartType.Column_Stacked,
      ExcelChartType.Column_Stacked_100,
      ExcelChartType.Bar_Clustered,
      ExcelChartType.Bar_Stacked,
      ExcelChartType.Bar_Stacked_100,
      ExcelChartType.Line,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Line_Markers,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Pie,
      ExcelChartType.PieOfPie,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Area,
      ExcelChartType.Area_Stacked,
      ExcelChartType.Area_Stacked_100,
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Radar,
      ExcelChartType.Radar_Markers,
      ExcelChartType.Radar_Filled,
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D
    };
    /// <summary>
    /// Represents series type that supports gridlines.
    /// </summary>
    public static readonly ExcelChartType[] DEF_NOT_SUPPORT_GRIDLINES =
    {
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.PieOfPie,
      ExcelChartType.Pie,
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Exploded_3D
    };
      /// <summary>
      /// Represents series type that must be in secondary primary axis.
      /// </summary>
      public static readonly ExcelChartType[] DEF_NEED_SECONDARY_AXIS =
    {
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.PieOfPie,
      ExcelChartType.Pie,
      ExcelChartType.Radar,
      //ExcelChartType.Radar_Markers,
      ExcelChartType.Radar_Filled,
      ExcelChartType.Bar_Clustered,
      ExcelChartType.Bar_Stacked,
      ExcelChartType.Bar_Stacked_100
    };
    /// <summary>
    /// Represents 
    /// </summary>
    public static readonly ExcelChartType[] DEF_COMBINATION_CHART =
    {
      ExcelChartType.Stock_HighLowClose,
      ExcelChartType.Stock_OpenHighLowClose,
      ExcelChartType.Stock_VolumeHighLowClose,
      ExcelChartType.Stock_VolumeOpenHighLowClose,
      ExcelChartType.Combination_Chart
    };
    /// <summary>
    /// Represents array that contain start Series types sorted by drawing order.
    /// </summary>
    public static readonly string[] DEF_PRIORITY_START_TYPES =
    {
      "Pie",
      "Doughnut",
      "Radar",
      "Area",
      "Column",
      "Bar",
      "Line",
      "Scatter"
    };
    /// <summary>
    /// Represents chart types that can change as intimate types.
    /// </summary>
    public static readonly ExcelChartType[] DEF_CHANGE_INTIMATE =
    {
      ExcelChartType.Radar,
      ExcelChartType.Radar_Markers,
      ExcelChartType.Radar_Filled,
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Line,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Line_Markers,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D
    };
    /// <summary>
    /// Represents chart start types that doesn't need plot or walls.
    /// </summary>
    public static readonly ExcelChartType[] DEF_DONT_NEED_PLOT =
    {
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.PieOfPie,
      ExcelChartType.Pie,
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Exploded_3D,
      ExcelChartType.Radar,
      ExcelChartType.Radar_Markers,
      ExcelChartType.Radar_Filled,
      ExcelChartType.Surface_Contour,
      ExcelChartType.Surface_NoColor_Contour
    };
    /// <summary>
    /// Represents chart types for pivot chart which need view tag to be rendered.
    /// </summary>
    public static readonly ExcelChartType[] DEF_NEED_VIEW_3D =
      {
          ExcelChartType.Surface_3D,
          ExcelChartType.Surface_Contour,
          ExcelChartType.Surface_NoColor_3D,
          ExcelChartType.Surface_NoColor_Contour
      };
    #region 100% charts
    /// <summary>
    /// 100% charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_100 = new ExcelChartType[]
    {
      ExcelChartType.Column_Stacked_100,
      ExcelChartType.Column_Stacked_100_3D,
      ExcelChartType.Bar_Stacked_100,
      ExcelChartType.Bar_Stacked_100_3D,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Area_Stacked_100,
      ExcelChartType.Area_Stacked_100_3D,
      ExcelChartType.Cylinder_Stacked_100,
      ExcelChartType.Cylinder_Bar_Stacked_100,
      ExcelChartType.Cone_Stacked_100,
      ExcelChartType.Cone_Bar_Stacked_100,
      ExcelChartType.Pyramid_Stacked_100,
      ExcelChartType.Pyramid_Bar_Stacked_100,
    };
    #endregion
    
    #region Stacked charts
    /// <summary>
    /// Stacked charts:
    /// </summary>
    public static readonly ExcelChartType[] STACKEDCHARTS = new ExcelChartType[]
    {
      ExcelChartType.Column_Stacked,
      ExcelChartType.Column_Stacked_3D,
      ExcelChartType.Bar_Stacked,
      ExcelChartType.Bar_Stacked_3D,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Area_Stacked,
      ExcelChartType.Area_Stacked_3D,
      ExcelChartType.Cylinder_Stacked,
      ExcelChartType.Cylinder_Bar_Stacked,
      ExcelChartType.Cone_Stacked,
      ExcelChartType.Cone_Bar_Stacked,
      ExcelChartType.Pyramid_Stacked,
      ExcelChartType.Pyramid_Bar_Stacked,
      ExcelChartType.Column_Stacked_100,
      ExcelChartType.Column_Stacked_100_3D,
      ExcelChartType.Bar_Stacked_100,
      ExcelChartType.Bar_Stacked_100_3D,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Area_Stacked_100,
      ExcelChartType.Area_Stacked_100_3D,
      ExcelChartType.Cylinder_Stacked_100,
      ExcelChartType.Cylinder_Bar_Stacked_100,
      ExcelChartType.Cone_Stacked_100,
      ExcelChartType.Cone_Bar_Stacked_100,
      ExcelChartType.Pyramid_Stacked_100,
      ExcelChartType.Pyramid_Bar_Stacked_100,
    };
    #endregion

    #region 3D charts
    /// <summary>
    /// 3D charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS3D = new ExcelChartType[]
    {
      ExcelChartType.Column_Clustered_3D,
      ExcelChartType.Column_Stacked_3D,
      ExcelChartType.Column_Stacked_100_3D,
      ExcelChartType.Column_3D,

      ExcelChartType.Bar_Clustered_3D,
      ExcelChartType.Bar_Stacked_3D,
      ExcelChartType.Bar_Stacked_100_3D,
      
      ExcelChartType.Line_3D,
      
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Exploded_3D,

      ExcelChartType.Area_3D,
      ExcelChartType.Area_Stacked_3D,
      ExcelChartType.Area_Stacked_100_3D,

      ExcelChartType.Surface_3D,
      ExcelChartType.Surface_NoColor_3D,
      ExcelChartType.Surface_Contour,
      ExcelChartType.Surface_NoColor_Contour,

      ExcelChartType.Cylinder_Clustered,
      ExcelChartType.Cylinder_Stacked,
      ExcelChartType.Cylinder_Stacked_100,
      ExcelChartType.Cylinder_Bar_Clustered,
      ExcelChartType.Cylinder_Bar_Stacked,
      ExcelChartType.Cylinder_Bar_Stacked_100,
      ExcelChartType.Cylinder_Clustered_3D,

      ExcelChartType.Cone_Clustered,
      ExcelChartType.Cone_Stacked,
      ExcelChartType.Cone_Stacked_100,
      ExcelChartType.Cone_Bar_Clustered,
      ExcelChartType.Cone_Bar_Stacked,
      ExcelChartType.Cone_Bar_Stacked_100,
      ExcelChartType.Cone_Clustered_3D,

      ExcelChartType.Pyramid_Clustered,
      ExcelChartType.Pyramid_Stacked,
      ExcelChartType.Pyramid_Stacked_100,
      ExcelChartType.Pyramid_Bar_Clustered,
      ExcelChartType.Pyramid_Bar_Stacked,
      ExcelChartType.Pyramid_Bar_Stacked_100,
      ExcelChartType.Pyramid_Clustered_3D,
    };
    #endregion

    #region Line charts
    /// <summary>
    /// Line charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_LINE = new ExcelChartType[]
    {
      ExcelChartType.Line,
      ExcelChartType.Line_3D,
      ExcelChartType.Line_Markers,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Line_Stacked_100,
    };
    #endregion

    #region Bubble charts
    /// <summary>
    /// Bubble charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_BUBBLE = new ExcelChartType[]
    {
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D,
    };
    #endregion

    #region No category Axis
    /// <summary>
    /// Charts that can be without category axis:
    /// </summary>
    public static readonly ExcelChartType[] NO_CATEGORY_AXIS = new ExcelChartType[]
    {
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie,
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.Pie_Exploded_3D,
      ExcelChartType.PieOfPie,
    };
    #endregion

    #region Chart Vary color
    /// <summary>
    /// Charts that need another color for each value:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_VARYCOLOR = new ExcelChartType[]
    {
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie,
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.Pie_Exploded_3D,
      ExcelChartType.PieOfPie,
    };
    #endregion

    #region Exploded charts
    /// <summary>
    /// Exploded charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_EXPLODED = new ExcelChartType[]
    {
      ExcelChartType.Doughnut_Exploded,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.Pie_Exploded_3D,
    };
    #endregion

    #region Series Lines
    /// <summary>
    /// Charts that need series lines:
    /// </summary>
    private static readonly ExcelChartType[] CHART_SERIES_LINES = new ExcelChartType[]
    {
      ExcelChartType.PieOfPie,
      ExcelChartType.Pie_Bar,
    };
    #endregion

    #region Scatter
    /// <summary>
    /// Scatter charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_SCATTER = new ExcelChartType[]
    {
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine,
    };
    #endregion

    #region Smoothed Line
    /// <summary>
    /// Charts with smoothed lines:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_SMOOTHED_LINE = new ExcelChartType[]
    {
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine
    };
    #endregion

    #region Stock
    /// <summary>
    /// Stock charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_STOCK = new ExcelChartType[]
    {
      ExcelChartType.Stock_HighLowClose,
      ExcelChartType.Stock_OpenHighLowClose,
      ExcelChartType.Stock_VolumeHighLowClose,
      ExcelChartType.Stock_VolumeOpenHighLowClose,
    };
    #endregion

    #region Perspective
    /// <summary>
    /// Charts with perspective:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_PERSPECTIVE = new ExcelChartType[]
    {
      ExcelChartType.Area_3D,
      ExcelChartType.Column_3D,
      ExcelChartType.Cone_Clustered_3D,
      ExcelChartType.Cylinder_Clustered_3D,
      ExcelChartType.Line_3D,
      ExcelChartType.Pyramid_Clustered_3D,
      ExcelChartType.Surface_3D,
      ExcelChartType.Surface_NoColor_3D,
      ExcelChartType.Surface_Contour,
      ExcelChartType.Surface_NoColor_Contour,
    };
    #endregion

    #region Clustered
    /// <summary>
    /// Clustered charts:
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_CLUSTERED = new ExcelChartType[]
    {
      ExcelChartType.Bar_Clustered,
      ExcelChartType.Bar_Clustered_3D,
      ExcelChartType.Column_Clustered,
      ExcelChartType.Column_Clustered_3D,
      ExcelChartType.Cone_Clustered,
      ExcelChartType.Cone_Bar_Clustered,
      ExcelChartType.Cylinder_Clustered,
      ExcelChartType.Cylinder_Bar_Clustered,
      ExcelChartType.Pyramid_Clustered,
      ExcelChartType.Pyramid_Bar_Clustered,
    };
    #endregion

    #region Charts with plot area
    /// <summary>
    /// Charts with plot area (by default):
    /// </summary>
    public static readonly ExcelChartType[] CHARTS_WITH_PLOT_AREA = new ExcelChartType[]
    {
      ExcelChartType.Column_Clustered,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Stock_VolumeOpenHighLowClose,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Stock_VolumeHighLowClose,
      ExcelChartType.Column_Stacked_100,
      ExcelChartType.Stock_OpenHighLowClose,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Area_Stacked_100,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Line_Markers,
      ExcelChartType.Stock_HighLowClose,
      ExcelChartType.Column_Stacked,
      ExcelChartType.Bar_Clustered,
      ExcelChartType.Bar_Stacked_100,
      ExcelChartType.Area_Stacked,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Bar_Stacked,
      ExcelChartType.Bubble_3D,
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Bubble,
      ExcelChartType.Area,
      ExcelChartType.Line,
    };
    #endregion

    #region Vertical legend
    /// <summary>
    /// Legend types that are displayed vertically:
    /// </summary>
    public static readonly ExcelLegendPosition[] LEGEND_VERTICAL = new ExcelLegendPosition[]
    {
      ExcelLegendPosition.Right,
      ExcelLegendPosition.Corner,
      ExcelLegendPosition.Left,
    };
    #endregion

    #region Unknown byte sequences
    /// <summary>
    /// Unknown bytes. Needed for data labels serialization.
    /// </summary>
    private static readonly byte[][] DEF_UNKNOWN_SERIE_LABEL = new byte[][]
    {
      new byte[]
      {
        0x50, 0x08, 0x00, 0x00, 0x0A, 0x0A, 0x03, 0x00, 0x50, 0x08, 0x5A,
        0x08, 0x61, 0x08, 0x61, 0x08, 0x6A, 0x08, 0x6B, 0x08
      },
      new byte[]
      {
        0x52, 0x08, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x52, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x52, 0x08, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x6A, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x54, 0x08, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x24, 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x25, 0x10, 0x20, 0x00,
        0x02, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
        0xA9, 0xFE, 0xFF, 0xFF, 0xBB, 0xFE, 0xFF, 0xFF,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0xB1, 0x00, 0x4D, 0x00, 0x50, 0x28, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x33, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x4F, 0x10, 0x14, 0x00,
        0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x51, 0x10, 0x08, 0x00,
        0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x27, 0x10, 0x06, 0x00,
        0x04, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x51, 0x08, 0x00, 0x00, 0x34, 0x10, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x55, 0x08, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
      },
    };
    #endregion

    #region Special Data Labels
    /// <summary>
    /// Charts that have different ways of data labels storage:
    /// </summary>
    public static readonly ExcelChartType[] DEF_SPECIAL_DATA_LABELS = new ExcelChartType[]
    {
      ExcelChartType.Radar_Filled,
      ExcelChartType.Area,
      ExcelChartType.Area_3D,
      ExcelChartType.Area_Stacked,
      ExcelChartType.Area_Stacked_100,
      ExcelChartType.Area_Stacked_100_3D,
      ExcelChartType.Area_Stacked_3D,
    };
    #endregion

    #region Charts that allow percentage labels
    /// <summary>
    /// Charts that can have percentage data labels:
    /// </summary>
    public static readonly ExcelChartType[] DEF_CHART_PERCENTAGE = new ExcelChartType[]
    {
      ExcelChartType.Pie,
      ExcelChartType.Pie_3D,
      ExcelChartType.Pie_Bar,
      ExcelChartType.Pie_Exploded,
      ExcelChartType.Pie_Exploded_3D,
      ExcelChartType.PieOfPie,
      ExcelChartType.Doughnut,
      ExcelChartType.Doughnut_Exploded,
    };
    #endregion

    #endregion

    #region Class members
      /// <summary>
    /// Represents to parse sheet on demand
    /// </summary>
    private bool m_bParseDataOnDemand;
      /// <summary>
    /// Specifies the contents of this attribute contain an integer between -100 and 100.
      /// </summary>
    private int m_iOverlap;
    /// <summary>
    /// Specifies the contents
    /// </summary>
    private int m_gapWidth;
    /// <summary>
    /// Represents whether to serialize gapwidth 
    /// </summary>
    private bool m_bShowGapWidth;
    /// <summary>
    /// If true than chart contain secondary axis.
    /// </summary>
    private bool m_bIsSecondaryAxis;
    /// <summary>
    /// Represents if chart in worksheet.
    /// </summary>
    private bool m_bInWorksheet = false;
    /// <summary>
    /// Chart type.
    /// </summary>
    private ExcelChartType m_chartType = DEFAULT_CHART_TYPE;
    /// <summary>
    /// Pivot Chart type.
    /// </summary>
    private ExcelChartType m_pivotChartType = DEFAULT_CHART_TYPE;
    /// <summary>
    /// DataRange for the chart series.
    /// </summary>
    private IRange m_dataRange;
    /// <summary>
    /// True if series are in rows in DataRange;
    /// otherwise False.
    /// </summary>
    private bool m_bSeriesInRows = true;
    /// <summary>
    /// True if has data table;
    /// otherwise False.
    /// </summary>
    private bool m_bHasDataTable;
    /// <summary>
    /// Page setup for the chart.
    /// </summary>
    private ChartPageSetupImpl m_pageSetup;
    /// <summary>
    /// X coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    private double m_dXPos;
    /// <summary>
    /// Y coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    private double m_dYPos;
    /// <summary>
    /// Width of the chart in points (1/72 inch).
    /// </summary>
    private double m_dWidth;
    /// <summary>
    /// Height of the chart in points (1/72 inch).
    /// </summary>
    private double m_dHeight;
    /// <summary>
    /// Array of the fonts used in the chart.
    /// </summary>
    private List<ChartFbiRecord> m_arrFonts;
    /// <summary>
    /// Collection of all the series of this chart.
    /// </summary>
    private ChartSeriesCollection m_series;
    /// <summary>
    /// Collection of all the categories of this chart.
    /// </summary>
    private ChartCategoryCollection m_categories;
    /// <summary>
    /// Chart's data table.
    /// </summary>
    private ChartDataTableImpl m_dataTable;
    /// <summary>
    /// Chart sheet properties.
    /// </summary>
    private ChartShtpropsRecord m_chartProperties;
    /// <summary>
    /// This record stores scale factors for font scaling.
    /// </summary>
    private ChartPlotGrowthRecord m_plotGrowth;
    /// <summary>
    /// Position of the plot area bounding box. The plot-area bounding box
    /// includes the plot area, tick marks, and a small border around
    /// the tick marks.
    /// </summary>
    private ChartPosRecord m_plotAreaBoundingBox;
    /// <summary>
    /// Chart frame format.
    /// </summary>
    private ChartFrameFormatImpl m_chartArea;
    /// <summary>
    /// Frame for plot area.
    /// </summary>
    private ChartFrameFormatImpl m_plotAreaFrame;
    /// <summary>
    /// Dictionary that contains information about default text.
    /// Key - Object identifier for the text,
    /// Value - List with text records.
    /// </summary>
    private TypedSortedListEx<int, List<BiffRecordRaw>> m_lstDefaultText = new TypedSortedListEx<int, List<BiffRecordRaw>>();
    /// <summary>
    /// Title area.
    /// </summary>
    private ChartTextAreaImpl m_title;
    /// <summary>
    /// Represents primary parent axis record and subrecords.
    /// </summary>
    private ChartParentAxisImpl m_primaryParentAxis;
    /// <summary>
    /// Represents secondary parent axis record and subrecords.
    /// </summary>
    private ChartParentAxisImpl m_secondaryParentAxis;
    /// <summary>
    /// Represents legend in chart.
    /// </summary>
    private ChartLegendImpl m_legend;
    /// <summary>
    /// True if has legend;
    /// otherwise False.
    /// </summary>
    private bool m_bHasLegend;
    /// <summary>
    /// Represents chart walls/back_walls.
    /// </summary>
    private ChartWallOrFloorImpl m_walls;
    /// <summary>
    /// Represents chart side_walls.
    /// </summary>
    private ChartWallOrFloorImpl m_sidewall;
    /// <summary>
    /// Represents chart floor.
    /// </summary>
    private ChartWallOrFloorImpl m_floor;
    /// <summary>
    /// Represents chart plot area.
    /// </summary>
    private ChartPlotAreaImpl m_plotArea;
    /// <summary>
    /// Indicates if we change chart or Series type.
    /// </summary>
    private bool m_bTypeChanging;
    /// <summary>
    /// Indicates if we change chart or Series type.
    /// </summary>
    private ExcelChartType m_destinationType;
    /// <summary>
    /// Represents helper array for parse trends. Use only for parsing.
    /// </summary>
    private List<BiffRecordRaw> m_trendList = new List<BiffRecordRaw>();
    /// <summary>
    /// Represents list with pivot records;
    /// </summary>
    private List<BiffRecordRaw> m_pivotList;
    /// <summary>
    /// WindowZoomRecord that was met inside ChartChartRecord block.
    /// </summary>
    private WindowZoomRecord m_chartChartZoom;
    /// <summary>
    /// Relations collection.
    /// </summary>
    private RelationCollection m_relations;
    /// <summary>
    /// Style index for Excel 2007 chart.
    /// </summary>
    private int m_iStyle2007;
    /// <summary>
    /// Stream with extracted pivot formats data.
    /// </summary>
    private Stream m_pivotFormatsStream;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bZoomToFit;
    /// <summary>
    /// Dictionary with error bars which requires future reparsing.
    /// </summary>
    private Dictionary<int, List<BiffRecordRaw>> m_dictReparseErrorBars;
    /// <summary>
    /// Preserved band formats.
    /// </summary>
    private Stream m_bandFormats;
    /// <summary>
    /// Pivot source string.
    /// </summary>
    private IPivotTable m_pivotSource;
    /// <summary>
    /// Since we don't parse pivot tables we can only preserve pivot source using string variable, this should be remove after we start pivot tables parsing.
    /// </summary>
    private string m_preservedPivotSource;
      /// <summary>
      /// represents the format id of the chart
      /// </summary>
    private int m_formatId;
    /// <summary>
    /// Indicates the all button fields in a pivot chart.
    /// </summary>
    private bool m_showAllFieldButtons = true;
    /// <summary>
    /// Indicates the Axis button in a pivot chart.
    /// </summary>
    private bool m_showAxisFieldButtons = true;
    /// <summary>
    /// Indicates the value button in a pivot chart.
    /// </summary>
    private bool m_showValueFieldButtons = true;
    /// <summary>
    /// Indicates the legend button in a pivot chart.
    /// </summary>
    private bool m_showLegendFieldButtons = true;
    /// <summary>
    /// Indicates the filter button in a pivot chart.
    /// </summary>
    private bool m_showReportFilterFieldButtons = true;
    private Stream m_alternateContent;
    /// <summary>
    /// Indicates wheather the chart contains title 
    /// </summary>
    private bool m_bHasChartTitle;
    /// <summary>
    /// Preserves the Chart's Default Text Property.
    /// TODO: Need to support Chart default text proprety, should be remove after we start to parse.
    /// </summary>
    private Stream m_defaultTextProperty;
    /// <summary>
    /// Font used for chart drawing.
    /// </summary>
    private FontWrapper m_font;    
    private bool? m_hasAutoTitle;
    private List<int> m_axisIds;    
    /// <summary>
    /// Plot area layout
    /// </summary>
    private ChartPlotAreaLayoutRecord m_plotAreaLayout;
    /// <summary>
    /// Collect the filter from category
    /// </summary>
    private bool[] category;
    /// <summary>
    /// Collect the filter from series
    /// </summary>
    private bool[] series;
    /// <summary>
    /// Represents the Excel2013 series name filter
    /// </summary>
    private ExcelSeriesNameLevel m_seriesNameLevel = ExcelSeriesNameLevel.SeriesNameLevelAll;
    /// <summary>
    /// Represents the Excel2013 category Filter
    /// </summary>
    private ExcelCategoriesLabelLevel m_categoriesLabelLevel = ExcelCategoriesLabelLevel.CategoriesLabelLevelAll;
    /// <summary>
    /// Indicates whether the PlotVisOnly attribute exists or not
    /// </summary>
    private bool m_showPlotVisible = false;
    /// <summary>
    /// Indicates whether the PlotVisOnly attribute exists or not
    /// </summary>
    private string m_radarStyle;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates chart and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    public ChartImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      
      m_bInWorksheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl != null;

      if( !m_book.Loading )
        CreateChartTitle();

      // NOTE: this code makes chart to create correct fill formats
      //OnChartTypeChanged( ChartType, false );
      //PrimaryFormats.AddFormat( new ChartFormatImpl( Application, PrimaryFormats ) );
      ChartFormatImpl format = new ChartFormatImpl( Application, PrimaryFormats );
      PrimaryFormats.Add( format, true );

      format.ChangeChartType( ExcelChartType.Column_Clustered, false );

      if( !m_book.Loading )
      {
        HasLegend = true;
        PrimaryValueAxis.HasMajorGridLines = true;

        m_chartProperties = ( ChartShtpropsRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartShtprops );

        m_walls = new ChartWallOrFloorImpl( application, this, true );
        m_floor = new ChartWallOrFloorImpl( application, this, false );
        m_sidewall = new ChartWallOrFloorImpl( application, this, true );        
        m_plotArea = new ChartPlotAreaImpl( application, this, ChartType );
      }
      else if( m_book.Version != ExcelVersion.Excel97to2003 )
      {
        m_chartProperties = ( ChartShtpropsRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartShtprops );
      }

      InitializeFrames();
    }
    /// <summary>
    /// Extracts chart from biff reader and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    /// <param name="reader">BiffReader to extract data from.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    /// <param name="hashXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public ChartImpl( IApplication application, object parent, BiffReader reader,
      ExcelParseOptions options, bool bSkipParsing, Dictionary<int, int> hashXFormatIndexes, IDecryptor decryptor )
      : base( application, parent, reader, options, bSkipParsing, hashXFormatIndexes, decryptor )
    {
    }
    /// <summary>
    /// Creates chart from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    /// <param name="data">Array of BiffRecords with chart's data.</param>
    /// <param name="iPos">Position of the first chart's record.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public ChartImpl( IApplication application, object parent, IList data,
      ref int iPos, ExcelParseOptions options )
      : this( application, parent )
    {
      m_arrRecords.Clear();
      bool bSkipParsing = ( ( options & ExcelParseOptions.DoNotParseCharts ) != 0 );
      m_parseOptions = options;

      //      if( !bSkipParsing )
      //      {
      //        Parse( data, ref iPos, options );
      //      }
      //      else
      //      {
      GetChartRecords( data, ref iPos, m_arrRecords, options );
      //      }
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    /// <summary>
    /// Creates chart title object.
    /// </summary>
    private void CreateChartTitle()
    {
      m_title = new ChartTextAreaImpl( Application, this, ExcelObjectTextLink.Chart );

      if( m_book.Version != ExcelVersion.Excel97to2003 || m_book.InnerFonts.Count < MaximumFontCount )
        m_title.Bold = true;

      m_title.FrameFormat.Interior.UseAutomaticFormat = true;
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses internal records.
    /// </summary>
    public override void Parse()
    {
      KeepRecord = true;
      base.Parse ();
    }

    /// <summary>
    /// Extracts chart from array of BiffRecords.
    /// </summary>
    /// <param name="data">Array of BiffRecords containing chart's data.</param>
    /// <param name="iPos">Position of the first chart's record.</param>
    /// <param name="options">Parse options.</param>
    private void    Parse( IList data, ref int iPos, ExcelParseOptions options )
    {
    }
    /// <summary>
    /// Parses object's records.
    /// </summary>
    protected internal override void ParseData( Dictionary<int, int> dictUpdatedSSTIndexes )
    {
      SetDefaultValues();

      if( ( m_parseOptions & ExcelParseOptions.DoNotParseCharts ) != 0 )
        return;

      if( m_dataHolder == null )
      {
        m_pivotList = new List<BiffRecordRaw>();

        int iPos = 0;
        bool bExit = false;
        BiffRecordRaw record = ( BiffRecordRaw )m_arrRecords[ iPos ];
        int iLevel = 0;
        Dictionary<int, int> newSeriesIndex = new Dictionary<int, int>();

        while( iPos < m_arrRecords.Count && !bExit )
        {
          record = ( BiffRecordRaw )m_arrRecords[ iPos ];

          if( record.TypeCode == TBIFFRecord.BOF )
          {
            iLevel++;
            iPos++;
          }
          else if( record.TypeCode == TBIFFRecord.EOF )
          {
            iLevel--;
            iPos++;

            if( iLevel == 0 )
              bExit = true;
          }
          else if( iLevel == 1 )
          {
            ParseOrdinaryRecord( record, ref iPos, newSeriesIndex );
          }
          else
          {
            iPos++;
          }
        }

        PrepareProtection();
        ReparseErrorBars( newSeriesIndex );
      }
      else
      {
        m_dataHolder.ParseChartsheetData( this );
      }

      IsParsed = true;
    }
    /// <summary>
    /// Reparses error bars.
    /// </summary>
    private void ReparseErrorBars( Dictionary<int, int> newSeriesIndex )
    {
      if( m_dictReparseErrorBars != null && m_dictReparseErrorBars.Count > 0 )
      {
        foreach( KeyValuePair<int, List<BiffRecordRaw>> errorData in m_dictReparseErrorBars )
        {
          int iSerieIndex = errorData.Key;

          if( newSeriesIndex.ContainsKey( iSerieIndex ) )
            iSerieIndex = newSeriesIndex[ iSerieIndex ];

          List<BiffRecordRaw> holder = errorData.Value;
          ChartSerieImpl serie = ( ChartSerieImpl )m_series[ iSerieIndex ];
          serie.ParseErrorBars( holder );
        }
      }

      m_dictReparseErrorBars = null;
    }
    /// <summary>
    /// Parses ordinary chart record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="iPos">Record position - will be updated during this operation.</param>
    private void ParseOrdinaryRecord( BiffRecordRaw record, ref int iPos, Dictionary<int, int> newSeriesIndex )
    {
      switch( record.TypeCode )
      {
        case TBIFFRecord.Header:
          m_pageSetup = new ChartPageSetupImpl( Application, this, m_arrRecords, ref iPos );
          break;

        case TBIFFRecord.ChartFbi:
          ParseFonts( m_arrRecords, ref iPos );
          break;

        // parses chart pivot table...
        case ( TBIFFRecord )2128:
        case ( TBIFFRecord )2136:
        case ( TBIFFRecord )2137:
          m_pivotList.Add( record );
          iPos++;
          break;

        case TBIFFRecord.ChartChart:
          ParseChart( m_arrRecords, ref iPos, newSeriesIndex );
          break;

        case TBIFFRecord.Dimensions:
          ParseDimensions( ( DimensionsRecord )m_arrRecords[ iPos++ ] );
          break;

        case TBIFFRecord.ChartSiIndex:
          ParseSiIndex( m_arrRecords, ref iPos );
          break;

        case TBIFFRecord.WindowTwo:
          ParseWindowTwo( ( WindowTwoRecord )m_arrRecords[ iPos++ ] );
          break;

        case TBIFFRecord.WindowZoom:
          ParseWindowZoom( ( WindowZoomRecord )m_arrRecords[ iPos++ ] );
          break;

        case TBIFFRecord.CodeName:
          m_strCodeName = ( ( CodeNameRecord )m_arrRecords[ iPos ] ).CodeName;
          iPos++;
          break;

        case TBIFFRecord.Protect:
          ParseProtect( ( ProtectRecord )record );
          iPos++;
          break;

        case TBIFFRecord.Password:
          ParsePassword( ( PasswordRecord )record );
          iPos++;
          break;

        case TBIFFRecord.ObjectProtect:
          ParseObjectProtect( ( ObjectProtectRecord )record );
          iPos++;
          break;

        case TBIFFRecord.ScenProtect:
          ParseScenProtect( ( ScenProtectRecord )record );
          iPos++;
          break;

        default:
          iPos++;
          break;
      }
    }
    /// <summary>
    /// Saves all chart records in the internal storage (without first BOF record).
    /// </summary>
    /// <param name="data">Array with chart records.</param>
    /// <param name="iPos">Position to the first chart record.</param>
    /// <param name="records">Array that will get all chart records.</param>
    /// <param name="options">Parse options.</param>
    private void    GetChartRecords( IList data, ref int iPos, List<BiffRecordRaw> records,
      ExcelParseOptions options )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos >= data.Count )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Count - 1" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode != TBIFFRecord.BOF )
        throw new ArgumentOutOfRangeException( "BOF record was expected." );

      records.Add( record );
      iPos++;
      record = ( BiffRecordRaw )data[ iPos ];

      bool bIgnoreStyles = false;//( ( options & ExcelParseOptions.SkipStyles ) != 0 );
      FontRecord font = null;

      if( bIgnoreStyles )
      {
        WorkbookImpl book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

        if( book == null )
          throw new ArgumentNullException( "Can't find parent workbook" );

        FontImpl fontImpl = ( FontImpl )book.InnerFonts[ 0 ];
        font = fontImpl.Record;
      }

      while( record.TypeCode != TBIFFRecord.EOF )
      {
        if( !bIgnoreStyles || bIgnoreStyles && record.TypeCode != TBIFFRecord.ChartFbi )
        {
          records.Add( record );
        }

        if( record.TypeCode == TBIFFRecord.MSODrawing && m_iMsoStartIndex < 0 )
        {
          m_iMsoStartIndex = records.Count - 1;
        }

        iPos++;
        record = ( BiffRecordRaw )data[ iPos ];
      }

      records.Add( ( BiffRecordRaw )data[ iPos ] );
      iPos++;
    }
    /// <summary>
    /// Extracts fonts from the BiffRecords array.
    /// </summary>
    /// <param name="data">BiffRecords array containing fonts.</param>
    /// <param name="iPos">Position of the first font record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If specified by parameter record is not ChartFbi record.
    /// </exception>
    private void    ParseFonts( IList data, ref int iPos )
    {
      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode != TBIFFRecord.ChartFbi )
        throw new ArgumentOutOfRangeException( "ChartFbi record was expected." );

      while( record.TypeCode == TBIFFRecord.ChartFbi )
      {
        m_arrFonts.Add( ( ChartFbiRecord )record );
        iPos++;
        record = ( BiffRecordRaw )data[ iPos ];
      }
    }
    /// <summary>
    /// Parses ChartChartReacod and all its subrecords.
    /// </summary>
    /// <param name="data">Array of BiffRecords containing chart's data.</param>
    /// <param name="iPos">Position of the ChartChart record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartChart record
    /// or the next record is not Begin record.
    /// </exception>
    private void ParseChart( IList<BiffRecordRaw> data, ref int iPos, Dictionary<int, int> newSeriesIndex )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartChart );
      
      FillDataFromChartRecord( ( ChartChartRecord )record );
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );
      iPos++;

      int iLevel = 0;
      int iCount = data.Count;
      m_series.TrendIndex = 0;

      record = ( BiffRecordRaw )data[ iPos ];
      List<ChartTextAreaImpl> unassignedTextAreaList = null;

      while( record.TypeCode != TBIFFRecord.End || iLevel != 0 )
      {
        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartPlotGrowth:
            ParsePlotGrowth( ( ChartPlotGrowthRecord )data[ iPos++ ] );
            break;

          case TBIFFRecord.ChartSeries:
            ParseSeriesOrErrorBars( data, ref iPos, newSeriesIndex );
            break;
            
          case TBIFFRecord.ChartShtprops:
            ParseSheetProperties( data, ref iPos );
            break;

          case TBIFFRecord.ChartDefaultText:
            ParseDefaultText( data, ref iPos );
            break;

          case TBIFFRecord.ChartText:
            ChartTextAreaImpl textArea = ParseText( data, ref iPos );
            List<ChartTextAreaImpl> textAreas = AssignTextArea( textArea, newSeriesIndex );

            if( unassignedTextAreaList == null )
            {
              unassignedTextAreaList = textAreas;
            }
            else
            {
              unassignedTextAreaList.AddRange( textAreas );
            }
            break;

          case TBIFFRecord.ChartAxesUsed:
            ParseAxesUsed( data, ref iPos );
            break;

          case TBIFFRecord.ChartAxisParent:
            ParseAxisParent( data, ref iPos );
            break;

          case TBIFFRecord.ChartDat:
            ParseDataTable( data, ref iPos );
            break;

          case TBIFFRecord.ChartFrame:
            InnerChartArea.Parse( data, ref iPos );
            break;

          case TBIFFRecord.ChartWrapper:
            ChartWrapperRecord wrapper = ( ChartWrapperRecord )record;
            if( wrapper.Record.TypeCode == TBIFFRecord.ChartText )
            {
              ChartWrappedTextAreaImpl wrappedTextArea = new ChartWrappedTextAreaImpl( Application, this,
                data, ref iPos );
              AssignTextArea( wrappedTextArea, newSeriesIndex );
            }
            else
            {
              iPos++;
            }
            break;

          case TBIFFRecord.Begin:
            iLevel++;
            iPos++;
            break;

          case TBIFFRecord.End:
            iLevel--;
            iPos++;
            break;

          case TBIFFRecord.WindowZoom:
            m_chartChartZoom = ( WindowZoomRecord )record;
            iPos++;
            break;

          case TBIFFRecord.PlotAreaLayout:
            m_plotAreaLayout = ( ChartPlotAreaLayoutRecord ) record;
            iPos++;
            break;

          default:
            iPos++;
            break;
        }

        if( iPos == iCount ) break;

        record = ( BiffRecordRaw )data[ iPos ];
      }

      iPos++;

      if( m_chartProperties == null )
        m_chartProperties = ( ChartShtpropsRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartShtprops );

      UpdateChartTitle();
      DetectIsInRowOnParsing();

      ChangePrimaryAxis( true );
      DetectChartType();
      ReparseTrendLegends();

      iCount = ( unassignedTextAreaList != null ) ?
        unassignedTextAreaList.Count :
        0;

      if (this.DataRange != null)
      {
          IRange values = null;

          this.IsSeriesInRows = DetectIsInRow(this.Series[0].Values);
          GetSerieOrAxisRange(this.DataRange, this.IsSeriesInRows, out values);
          GetSerieOrAxisRange(values, !this.IsSeriesInRows, out values);
          int count = values.Count / this.Series[0].Values.Count;
          for (int i = 0; i < this.Series[0].Values.Count; i++)
          {
              IRange cate_range = ChartImpl.GetCategoryRange(values, out values, count, this.IsSeriesInRows);
              if (this.Categories.Count > 0)
              {
                  (this.Categories[i] as ChartCategory).CategoryLabel = this.Series[0].CategoryLabels;
                  (this.Categories[i] as ChartCategory).Values = cate_range;
                  if (this.Categories[0].CategoryLabel != null)
                      (this.Categories[i] as ChartCategory).Name = this.Categories[0].CategoryLabel.Cells[i].Text;
                  else
                  {
                      (this.Categories[i] as ChartCategory).Name = (i + 1).ToString();
                      if (this.Legend != null &&
                          this.Legend.LegendEntries != null &&
                          this.Legend.LegendEntries[i].TextArea != null)
                      {
                          this.Legend.LegendEntries[i].TextArea.Text = (this.Categories[i] as ChartCategory).Name;
                      }
                  }
              }
              else
              {
                  if (this.Legend != null &&
                      this.Legend.LegendEntries != null &&
                      this.Legend.LegendEntries[i].TextArea != null)
                  {
                      this.Legend.LegendEntries[i].TextArea.Text = (i + 1).ToString();
                  }
              }
          }

      }
      else
      {
          if (this.Series != null && this.Series.Count > 0 && this.Series[0].Values != null)
          {
              for (int i = 0; i < this.Series[0].Values.Count; i++)
              {
                  if (i < this.Categories.Count)
                  {
                      (this.Categories[i] as ChartCategory).CategoryLabel = this.Series[0].CategoryLabels;
                      (this.Categories[i] as ChartCategory).Values = this.Series[0].Values;
                      if (this.Categories[0].CategoryLabel != null &&
                          (this.Categories[0].CategoryLabel.GetType() != typeof(ExternalRange)) &&
                          (this.Categories[0].CategoryLabel.Worksheet != null) &&
                          (i < this.Categories[0].CategoryLabel.Cells.Length))
                      {
                          (this.Categories[i] as ChartCategory).Name = this.Categories[0].CategoryLabel.Cells[i].Text;
                      }
                  }
              }
          }
      }

      for( int i = 0; i < iCount; i++ )
      {
        ChartTextAreaImpl textArea = unassignedTextAreaList[ i ];
        m_series.AssignTrendDataLabel( textArea );
      }
    }
    /// <summary>
    /// Extracts data from ChartChart record.
    /// </summary>
    /// <param name="chart">Record with data.</param>
    private void FillDataFromChartRecord( ChartChartRecord chart )
    {
      XPos = FixedPointToDouble( chart.X );
      YPos = FixedPointToDouble( chart.Y );
      EMUWidth = FixedPointToDouble( chart.Width );
      EMUHeight = FixedPointToDouble( chart.Height );
    }
    /// <summary>
    /// Parses ChartPlotGrowth record.
    /// </summary>
    /// <param name="plotGrowth">Record to parse.</param>
    private void    ParsePlotGrowth( ChartPlotGrowthRecord plotGrowth )
    {
      if( plotGrowth == null )
        throw new ArgumentNullException( "plotGrowth" );

      m_plotGrowth = plotGrowth;
    }
    /// <summary>
    /// Parses ChartSiIndex records.
    /// </summary>
    /// <param name="data">Array of records containing ChartSiIndex records.</param>
    /// <param name="iPos">Position of the first ChartSiIndex record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartSiIndex record.
    /// </exception>
    private void    ParseSiIndex( IList<BiffRecordRaw> data, ref int iPos )
    {
      m_series.ParseSiIndex( data, ref iPos );
    }
    /// <summary>
    /// Parses ChartSeries record and its subrecords.
    /// Creates new series in the Series collection.
    /// </summary>
    /// <param name="data">Array of records containing ChartSeries data.</param>
    /// <param name="iPos">Position of the ChartSeries record.</param>
    private void ParseSeriesOrErrorBars( IList<BiffRecordRaw> data, ref int iPos, Dictionary<int, int> newSeriesIndexes )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      List<BiffRecordRaw> holder = new List<BiffRecordRaw>();
      int iSerieIndex = 0;
      bool bIsError = false;

      if( AddRecords( data, holder, ref iPos, ref iSerieIndex, ref bIsError ) )
      {
        int position = 0;

        ChartSerieImpl newSerie = new ChartSerieImpl( Application, m_series
          , holder, ref position );

        m_series.Add( newSerie );
        newSeriesIndexes[ iSerieIndex ] = m_series.Count - 1;
      }
      else
      {
        if( bIsError )
        {
          if( newSeriesIndexes.ContainsKey( iSerieIndex ) )
            iSerieIndex = newSeriesIndexes[ iSerieIndex ];

          ChartSerieImpl series = ( ChartSerieImpl )m_series[ iSerieIndex ];//[ iSerieIndex ];

          if( series != null )
          {
            series.ParseErrorBars( holder );
          }
          else
          {
            if( m_dictReparseErrorBars == null )
              m_dictReparseErrorBars = new Dictionary<int, List<BiffRecordRaw>>();

            m_dictReparseErrorBars.Add( iSerieIndex, holder );
          }
        }
        else
        {
          //m_trendList.Add( iSerieIndex );

          for( int i = 0, len = holder.Count; i < len; i++ )
          {
            m_trendList.Add( holder[ i ] );
          }
        }
      }
    }
    /// <summary>
    /// Parses sheet properties.
    /// </summary>
    /// <param name="data">Array of records containing sheet properties data.</param>
    /// <param name="iPos">Position of the ChartShtprops record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartShtprops record.
    /// </exception>
    private void ParseSheetProperties( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = data[ iPos ];

      record.CheckTypeCode( TBIFFRecord.ChartShtprops );

      m_chartProperties = ( ChartShtpropsRecord )record.Clone();
      iPos++;
    }
    /// <summary>
    /// Parses ChartDefaultText record.
    /// </summary>
    /// <param name="data">Array of records containing default text data.</param>
    /// <param name="iPos">Position of the record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartDefaultText record.
    /// </exception>
    private void    ParseDefaultText( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartDefaultText );

      ChartDefaultTextRecord defaultText = ( ChartDefaultTextRecord )record.Clone();
      int index = ( int )defaultText.TextCharacteristics;
      iPos++;

      List<BiffRecordRaw> arrData = GetTextData( data, ref iPos );
      m_lstDefaultText[ index ] = arrData;
    }
    /// <summary>
    /// Gets record of the text data block.
    /// </summary>
    /// <param name="data">Array of records containing text data.</param>
    /// <param name="iPos">Position of the first text record.</param>
    /// <returns>List with all text records.</returns>
    private List<BiffRecordRaw> GetTextData( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos > data.Count )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Length" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartText );

      List<BiffRecordRaw> result = new List<BiffRecordRaw>();
      result.Add( record );
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );
      result.Add( record );

      do
      {
        iPos++;
        record = ( BiffRecordRaw )data[ iPos ];
        result.Add( record );
      }
      while( record.TypeCode != TBIFFRecord.End );

      iPos++;
      return result;
    }
    /// <summary>
    /// Parses ChartText record and all its subrecords.
    /// </summary>
    /// <param name="data">Array of records containing text data.</param>
    /// <param name="iPos">Position of the record.</param>
    /// <returns>ChartTextAreaImpl that represents parsed text area.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartText record
    /// or when next record is not Begin record.
    /// </exception>
    private ChartTextAreaImpl ParseText( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartText );

      return new ChartTextAreaImpl( Application, this, data, ref iPos );
    }
    /// <summary>
    /// Parses ChartAxesUsed record.
    /// </summary>
    /// <param name="data">Array of records containing record.</param>
    /// <param name="iPos">Position of the record to parse.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartAxesUsed record.
    /// </exception>
    private void ParseAxesUsed( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartAxesUsed );
      //ChartAxesUsedRecord axesUsed = (ChartAxesUsedRecord) data[ iPos ];
      m_primaryParentAxis.Formats.PrimaryFormats.Clear();
      m_primaryParentAxis.Formats.SecondaryFormats.Clear();
      iPos++;
      // TODO: implement
    }
    /// <summary>
    /// Parses ChartAxisParent record.
    /// </summary>
    /// <param name="data">Array of records containing record.</param>
    /// <param name="iPos">Position of the record to parse.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartAxisParent record.
    /// </exception>
    private void    ParseAxisParent( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartAxisParent );

      ChartAxisParentRecord axisParent = ( ChartAxisParentRecord )record.Clone();

      switch( axisParent.AxesIndex )
      {
        case 0:
          //m_primaryParentAxis = new ChartParentAxisImpl( Application, this, true );
          m_primaryParentAxis.Parse( data, ref iPos );
          break;

        case 1:
          //m_secondaryParentAxis = new ChartParentAxisImpl( Application, this, false );
          m_secondaryParentAxis.Parse( data, ref iPos );
          break;

        default:
          throw new ArgumentOutOfRangeException( "Axes index must be 0 or 1." );
      }
    }
    /// <summary>
    /// Parses ChartData table records.
    /// </summary>
    /// <param name="data">Array of records containing data table records.</param>
    /// <param name="iPos">Position of the ChartData record in the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartData record.
    /// </exception>
    private void ParseDataTable( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartDat );
      
      m_bHasDataTable = true;
      m_dataTable = new ChartDataTableImpl( Application, this, data, ref iPos );
    }
    /// <summary>
    /// Assigns text area to its owner.
    /// </summary>
    /// <param name="textArea">Text area to assign.</param>
    /// <returns>List with unassigned text areas.</returns>
    private List<ChartTextAreaImpl>    AssignTextArea( ChartTextAreaImpl textArea, 
      Dictionary<int, int> newSeriesIndexes )
    {
      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      ChartObjectLinkRecord objectLink = textArea.ObjectLink;

      if( objectLink == null )
        throw new ArgumentNullException( "objectLink" );

      List<ChartTextAreaImpl> result = null;

      switch( objectLink.LinkObject )
      {
        case ExcelObjectTextLink.Chart:
          m_title = textArea;
          break;

        case ExcelObjectTextLink.XAxis:
          CategoryAxisTitle = textArea.Text;
          break;
          
        case ExcelObjectTextLink.YAxis:
          ValueAxisTitle = textArea.Text;
          break;

        case ExcelObjectTextLink.ZAxis:
          SeriesAxisTitle = textArea.Text;
          break;

        case ExcelObjectTextLink.DataLabel:
          int iSeriesIndex = objectLink.SeriesNumber;
          int iPointNumber = objectLink.DataPointNumber;

          if( newSeriesIndexes.ContainsKey( iSeriesIndex ) )
            iSeriesIndex = newSeriesIndexes[ iSeriesIndex ];

          //if( textArea.ContainDataLabels )
          {
            if( iSeriesIndex >= m_series.Count )
            {
              if( result == null )
                result = new List<ChartTextAreaImpl>();

              result.Add( textArea );
            }
            else
            {
              ChartSerieImpl series = ( ChartSerieImpl )m_series[ iSeriesIndex ];
              ChartDataPointImpl dataPoint = ( ChartDataPointImpl )series.DataPoints[ iPointNumber ];
              dataPoint.SetDataLabels( textArea );
            }
          }
          break;
      }

      return result;
    }
    /// <summary>
    /// Parses chart's plot area and all its sub records.
    /// </summary>
    /// <param name="data">Array of records containing plot area data.</param>
    /// <param name="iPos">
    /// Position of the ChartPlotArea record in the array.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartPlotArea record.
    /// </exception>
    private void ParsePlotArea( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartPlotArea );
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartFrame );
      InnerPlotArea.Parse( data, ref iPos );
    }
    /// <summary>
    /// Parses chart frame records.
    /// </summary>
    /// <param name="data">Array of records containing axes names data.</param>
    /// <param name="iPos">Position of the ChartFrame record in the array.</param>
    private void ParseChartFrame( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartFrame );
      iPos++;

      int iBeginCounter = 0;

      record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode == TBIFFRecord.Begin )
      {
        iBeginCounter++;
        iPos++;

        while( iBeginCounter != 0 )
        {
          record = ( BiffRecordRaw )data[ iPos ];
          switch( record.TypeCode )
          {
            case TBIFFRecord.Begin:
              iBeginCounter++;
              break;

            case TBIFFRecord.End:
              iBeginCounter--;
              break;
          }

          iPos++;
        }
      }
    }
    /// <summary>
    /// Detects type of the chart.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When it is not possible to detect chart type.
    /// </exception>
    public void    DetectChartType()
    {
      if( Series.Count == 0 )
      {
        m_chartType = DEFAULT_CHART_TYPE;
        return;
      }

      m_chartType = m_primaryParentAxis.Formats.DetectChartType( m_series );

    }
    /// <summary>
    /// Sets some important variables into initial state.
    /// </summary>
    private void    SetDefaultValues()
    {
      //m_dataLabels = null;
      m_plotAreaFrame = null;
      m_chartArea = null;
    }
    /// <summary>
    /// Parses ChartFontxRecord.
    /// </summary>
    /// <param name="fontx">Record to parse.</param>
    /// <returns>IFont corresponding to the fontx record.</returns>
    private IFont   ParseFontx( ChartFontxRecord fontx )
    {
      if( fontx == null )
        throw new ArgumentNullException( "fontx" );

      int index = fontx.FontIndex;

      FontImpl fontToWrap = m_book.InnerFonts[ index ] as FontImpl;

      //return new FontWrapper( Application, this, fontToWrap );
      return new FontWrapper( fontToWrap );
    }
    /// <summary>
    /// Parses legend.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    public void ParseLegend( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentException( "data" );

      HasLegend = true;
      m_legend.Parse( data, ref iPos );
    }
    /// <summary>
    /// Adds series records to subholder from global holder.
    /// </summary>
    /// <param name="list">Represents global record holder.</param>
    /// <param name="holder">Represents record subholder.</param>
    /// <param name="iPos">Represents position in global holder</param>
    /// <param name="serieIndex">Gets series index for error bar.</param>
    /// <param name="bIsErrorBars">Indicates parsing error bars.</param>
    /// <returns>If true - parse Series; otherwise - error bars or trend lines.</returns>
    private bool AddRecords( IList<BiffRecordRaw> list, IList<BiffRecordRaw> holder, ref int iPos, ref int serieIndex
      , ref bool bIsErrorBars )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      bool bIsSerie = true;
      bIsErrorBars = false;

      BiffRecordRaw record = ( BiffRecordRaw )list[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartSeries );
      iPos++;
      holder.Add( record );

      record = ( BiffRecordRaw )list[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );
      iPos++;
      holder.Add( record );

      int iCount = 1;

      while( iCount > 0 )
      {
        record = ( BiffRecordRaw )list[ iPos ];
        holder.Add( record );

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          case TBIFFRecord.ChartSerAuxErrBar:
            bIsSerie = false;
            bIsErrorBars = true;
            break;

          case TBIFFRecord.ChartSerAuxTrend:
            bIsSerie = false;
            break;

          case TBIFFRecord.ChartSerParent:
            serieIndex = ( ( ChartSerParentRecord )record ).Series - 1;
            break;

          case TBIFFRecord.ChartDataFormat:
            ChartDataFormatRecord dataFormat = ( ChartDataFormatRecord )record;
            serieIndex = dataFormat.SeriesIndex;
            break;
        }

        iPos++;
      }

      return bIsSerie;
    }
    /// <summary>
    /// Reparse trend line legend entries.
    /// </summary>
    private void ReparseTrendLegends()
    {
      ChartLegendEntriesColl legendColl = HasLegend 
        ? ( ChartLegendEntriesColl )m_legend.LegendEntries
        : null;

      for( int i = 0, iLen = m_trendList.Count; i < iLen; i++ )
      {
        int iSerIndex = FindSeriesIndex( m_trendList, i );// = ( int )m_trendList[ i ];
        ChartSerieImpl serie = ( ChartSerieImpl )m_series[ iSerIndex ];
        ChartTrendLineCollection trendLines = ( ChartTrendLineCollection )serie.TrendLines;
        ChartLegendEntryImpl entry;
        //i++;

        ChartTrendLineImpl trend = new ChartTrendLineImpl( Application, trendLines, m_trendList
          , ref i, out entry );

        trendLines.Add( trend );

        if( legendColl != null && entry != null )
        {
          int entryIndex = m_series.GetLegendEntryOffset( iSerIndex );
          legendColl.UpdateEntries( entryIndex, 1 );
          legendColl.Add( entryIndex, entry );
        }
      }
    }

    private int FindSeriesIndex( List<BiffRecordRaw> m_trendList, int i )
    {
      int iResult = -1;

      for( int len = m_trendList.Count; i < len; i++ )
      {
        BiffRecordRaw record = m_trendList[ i ];

        if( record.TypeCode == TBIFFRecord.ChartSerParent )
        {
          ChartSerParentRecord parent = ( ChartSerParentRecord )record;
          iResult = parent.Series - 1;
          break;
        }

      }

      return iResult;
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Saves chart into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all chart's records.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [CLSCompliant( false )]
    public override void Serialize( OffsetArrayList records )
    {
#if DEBUG
      WorkbookImpl book = (WorkbookImpl) FindParent( typeof( WorkbookImpl ) );
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, book.FullFileName, "FileName:" );
#endif

      if( records == null )
        throw new ArgumentNullException( "records" );

      //m_iDrawingOrder = 0;

      // TODO: this serialization not for chart that is placed on the worksheet
      int iCount = m_arrRecords.Count;

      m_bof.Type = BOFRecord.TType.TYPE_CHART;
      m_bof.IsNested = ( FindParent( typeof( WorksheetImpl ) ) != null );
      //IsParsed = true;

      if( m_arrRecords.Count > 0 )
      {
        records.AddList( m_arrRecords );
        return;
      }

      records.Add( m_bof );
      SerializeHeaderFooterPictures( records );
      m_pageSetup.Serialize( records );
      
      SerializeFonts( records );

      if( m_bInWorksheet )
      {
        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Protect ) );
      }
      else
      {
        SerializeProtection( records, true );
      }

      SerializeMsoDrawings( records );

      if( m_pivotList != null && m_pivotList.Count > 0 )
        records.AddRange( m_pivotList );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartUnits ) );

      SerializeChart( records );

      DimensionsRecord dimensions = (DimensionsRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.Dimensions );
      dimensions.LastColumn = ( ushort )( m_series.TrendErrorBarIndex + 1 );

      IRange values = ( m_series.Count > 0 ) ? m_series[ 0 ].Values : null;
      dimensions.LastRow = ( values != null ) ? m_series[ 0 ].Values.Count : 0;

      records.Add( dimensions );

      SerializeChartSiIndexes( records );

      if( !m_bInWorksheet )
      {
        SerializeWindowTwo( records );
        SerializeWindowZoom( records );
        SerializeSheetLayout( records );
      }

      SerializeMacrosSupport( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.EOF ) );
    }
    /// <summary>
    /// Saves all chart's fonts into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all fonts records.
    /// </param>
    private void SerializeFonts( OffsetArrayList records )
    {
      records.AddList( m_arrFonts );
    }
    /// <summary>
    /// Saves ChartChart record and all its subrecords.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeChart( OffsetArrayList records )
    {
      ChartChartRecord chart = ( ChartChartRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartChart );
      chart.X = DoubleToFixedPoint( XPos );
      chart.Y = DoubleToFixedPoint( YPos );
      chart.Width = DoubleToFixedPoint( EMUWidth );
      chart.Height = DoubleToFixedPoint( EMUHeight );

      if( chart.Width == 0 )
      {
        chart.Width = 48027384;//47681352;
        chart.Height = 29506896;//29664144;
      }

     // chart.Height = 10027008;
      
      records.Add( chart );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      //SerializeWindowZoom( records )
      if( m_chartChartZoom != null )
      {
        records.Add( m_chartChartZoom );
      }
      else
      {
        WindowZoomRecord zoom = ( WindowZoomRecord )BiffRecordFactory.GetRecord( TBIFFRecord.WindowZoom );
        zoom.NumMagnification = 1;
        zoom.DenumMagnification = 1;
        records.Add( zoom );
      }

      records.Add( ( BiffRecordRaw )PlotGrowth.Clone() );

      if( m_chartArea != null )
        m_chartArea.Serialize( records );

      m_series.Serialize( records );
      SerializeSheetProperties( records );
      SerializeDefaultText( records );
      SerializeAxes( records );

      if (m_plotAreaLayout != null)
          records.Add((ChartPlotAreaLayoutRecord)m_plotAreaLayout.Clone());

      //serialize trend lines labels.
      records.AddRange( m_series.TrendLabels );

      SerializeDataTable( records );

      //if( m_title.Pos.X2 == 0 )
      //{
      //  ChartTextRecord chartText = m_title.TextRecord;
      //  chartText.XPos = 1695;
      //  chartText.YPos = 79;
      //  chartText.XSize = 610;
      //  chartText.YSize = 685;
      //  chartText.ColorIndex = ( ExcelKnownColors )77;
      //  chartText.Options2 = 3504;
      //  chartText.IsAutoColor = false;



      //  ChartPosRecord chartPos = m_title.Pos;
      //  chartPos.X2 = 161;
      //  chartPos.Y2 = 104;
      //}

      if( HasTitle )
        m_title.Serialize( records );

      SerializeDataLabels( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    /// <summary>
    /// Saves DefaultText.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeDefaultText( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      //foreach( DictionaryEntry entry in m_hashDefaultText )
      for( int i = 0, len = m_lstDefaultText.Count; i < len; i++ )
      {
        int index = m_lstDefaultText.GetKey( i );
        List<BiffRecordRaw> arrRecords = m_lstDefaultText.GetByIndex( i );

        ChartDefaultTextRecord defaultText = ( ChartDefaultTextRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartDefaultText );

        defaultText.TextCharacteristics = ( ChartDefaultTextRecord.TextDefaults )index;
        records.Add( defaultText );
        records.AddList( arrRecords );
      }
    }
    /// <summary>
    /// Serializes chart's axes.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeAxes( OffsetArrayList records )
    {
      ChartAxesUsedRecord axesUsed = ( ChartAxesUsedRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxesUsed );

      int iSecondaryFormatsCount = SecondaryFormats.Count;

      axesUsed.NumberOfAxes = ( ushort )( ( iSecondaryFormatsCount > 0 ) ? 2 : 1 );

      records.Add( axesUsed );

      m_primaryParentAxis.Serialize( records );
      //SerializeDefaultPrimaryAxes( records, 0 );

      if( iSecondaryFormatsCount > 0 )
      {
        m_secondaryParentAxis.Serialize( records );
      }
    }
    /// <summary>
    /// Serializes sheet properties.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeSheetProperties( OffsetArrayList records )
    {
      records.Add( ( BiffRecordRaw )m_chartProperties.Clone() );
    }
    /// <summary>
    /// Serializes ChartSiIndex records.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeChartSiIndexes( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartSiIndexRecord siIndex = (ChartSiIndexRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSiIndex );
      siIndex.NumIndex = DEF_SI_CATEGORY;
      records.Add( siIndex );
      SerializeChartSiMembers( records, DEF_SI_CATEGORY );

      InsertSeriesLabels( records );

      siIndex = (ChartSiIndexRecord) siIndex.Clone();
      siIndex.NumIndex = DEF_SI_VALUE;
      records.Add( siIndex );
      SerializeChartSiMembers( records, DEF_SI_VALUE );

      siIndex = (ChartSiIndexRecord) siIndex.Clone();
      siIndex.NumIndex = DEF_SI_BUBBLE;
      records.Add( siIndex );
      SerializeChartSiMembers( records, DEF_SI_BUBBLE );
    }
    /// <summary>
    /// Serializes category labels.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void InsertSeriesLabels( OffsetArrayList records )
    {
    }
    /// <summary>
    /// Serializes series values.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void InsertSeriesValues( OffsetArrayList records )
    {
      NumberRecord number = (NumberRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.Number );

      for( int i = 0; i < Series.Count; i++ )
      {
        ChartSerieImpl serie = (ChartSerieImpl) Series[ i ];
        int j = 0;

        foreach( IRange range in serie.Values.Cells )
        {
          number = (NumberRecord) number.Clone();
          number.Row = (ushort) j;
          number.Column = (ushort) i;
          number.Value = range.Number;
          number.ExtendedFormatIndex = 0;
          records.Add( number );
          j++;
        }
      }
    }
    /// <summary>
    /// Serializes data table if it is visible.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeDataTable( OffsetArrayList records )
    {
      if( HasDataTable )
      {
        m_dataTable.Serialize( records );
      }
    }
    /// <summary>
    /// Serializes data labels.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeDataLabels( OffsetArrayList records )
    {
//      if( HasDataLabels )
//      {
        m_series.SerializeDataLabels( records );

//        if( NeedUnknownSection )
//        {
//          SerializeUnknownSection( records );
////          return;
//        }
//
////        records.Add( m_dataLabels );
//        if( NeedDefTextInDataLabels ) SerializeDefTextInDataLabels( records );
//
//        if( NeedAttachedLabel )
//        {
//          ChartDataFormatRecord dataFormat = ( ChartDataFormatRecord )
//            BiffRecordFactory.GetRecord( TBIFFRecord.ChartDataFormat );
//          dataFormat.SeriesNumber = 65533;
//
//          records.Add( dataFormat );
//          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
//        
//          SerializeAttachedLabel( records );
//
//          //        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Chart3DDataFormat ) );
//          //        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartLineFormat ) );
//          //        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartAreaFormat ) );
//          //        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat ) );
//          //        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartMarkerFormat ) );
//          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
//        }
//      }
    }
    /// <summary>
    /// Serializes Series list.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    private void SerializeSeriesList( OffsetArrayList records )
    {
      if( IsChartVolume )
      {
        ChartSeriesListRecord seriesList = (ChartSeriesListRecord)
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesList );
        
        if( ChartType == ExcelChartType.Stock_VolumeHighLowClose )
        {
          seriesList.Series = new ushort[] { 1, 2, 3, 4 };
        }
        else if( ChartType == ExcelChartType.Stock_VolumeOpenHighLowClose )
        {
          seriesList.Series = new ushort[] { 1, 2, 3, 4, 5 };
        }
        records.Add( seriesList );
      }
    }
    /// <summary>
    /// Serializes ChartSi members.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    /// <param name="siIndex">SiIndex record index.</param>
    private void SerializeChartSiMembers( OffsetArrayList records, int siIndex )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( siIndex > 3 || siIndex < 1 )
        throw new ArgumentOutOfRangeException( "siIndex" );

      List<BiffRecordRaw> list = m_series.GetEnteredRecords( siIndex );

      if( list != null && list.Count > 0 )
      {
        records.AddList( list );
      }
    }
    /// <summary>
    /// Serialize legend.
    /// </summary>
    /// <param name="records">Represents record list to serialize into.</param>
    [CLSCompliant( false )]
    public void SerializeLegend( OffsetArrayList records )
    {
      if( m_legend != null )
        m_legend.Serialize( records );
    }
    /// <summary>
    /// Serialize walls.
    /// </summary>
    /// <param name="records">Records collection.</param>
    [ CLSCompliant( false ) ]
    public void SerializeWalls( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_walls != null )
        m_walls.Serialize( records );
    }
    /// <summary>
    /// Serialize floor.
    /// </summary>
    /// <param name="records">Records collection.</param>
    [ CLSCompliant( false ) ]
    public void SerializeFloor( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_floor != null )
        m_floor.Serialize( records );
    }
    /// <summary>
    /// Serialize plot area to stg stream.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    [ CLSCompliant( false ) ]
    public void SerializePlotArea( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_plotArea != null )
        m_plotArea.Serialize( records );
    }
    #endregion 

    #region IChart Members
      ///<summary>
      ///Specifies the contents of this attribute contain an integer between -100 and 100.
      /// </summary>
    internal int OverLap
    {
        get
        {
            return m_iOverlap;
        }
        set
        {
            m_iOverlap = value;
        }
    }
    internal int GapWidth
    {
        get
        {
            return m_gapWidth;
        }
        set
        {
            m_gapWidth = value;
        }
    }
    /// <summary>
    /// Represents whether to serialize gapwidth 
    /// </summary>
    internal bool ShowGapWidth
    {
        get
        {
            return m_bShowGapWidth;
        }
        set
        {
            m_bShowGapWidth = value;
        }
    }
      /// <summary>
    /// Returns or sets the rotation of the 3-D chart view
    /// (the rotation of the plot area around the z-axis, in degrees).(0 to 360 degrees).
    /// </summary>
    public int Rotation
    {
      get
      {
        return ChartFormat.Rotation;
      }
      set
      {
        ChartFormat.Rotation = value;
      }
    }
    /// <summary>
    /// Returns or sets the elevation of the 3-D chart view, in degrees (�90 to +90 degrees).
    /// </summary>
    public int  Elevation
    {
      get
      {
        return ChartFormat.Elevation;
      }
      set
      {
        ChartFormat.Elevation = value;
      }
    }
    /// <summary>
    /// Returns or sets the perspective for the 3-D chart view.( 0 - 100 )
    /// </summary>
    public int Perspective
    {
      get
      {
        return ChartFormat.Perspective;
      }
      set
      {
        ChartFormat.Perspective = value;
      }
    }
    /// <summary>
    /// Gets or sets the type of the pivot chart.
    /// </summary>
    /// <value>The type of the pivot chart.</value>
    public ExcelChartType PivotChartType
    {
      get
      {
        return m_pivotChartType;
      }
      set
      {
        if( this.Series.Count != 0 )
          this.ChartType = value;

        m_pivotChartType = value;

        bool hasSource = HasPivotSource;

        if( !hasSource )
          m_preservedPivotSource = string.Empty;

        CreateNecessaryAxes( true );

        if( !hasSource )
          m_preservedPivotSource = null;
      }
    }
    /// <summary>
    /// Gets or sets the pivot source.
    /// </summary>
    /// <value>The pivot source.</value>
    public IPivotTable PivotSource
    {
      get
      {
        return m_pivotSource;
      }
      set
      {
        if( Array.IndexOf( DEF_UNSUPPORT_PIVOT_CHART, this.PivotChartType ) != -1 )
          throw new NotSupportedException( "PivotChartType" );

        m_preservedPivotSource = null;
        m_pivotSource = value;
      }
    }
    public string PreservedPivotSource
    {
      get
      {
        return m_preservedPivotSource;
      }
      set
      {
        m_preservedPivotSource = value;
      }
    }
      /// <summary>
      /// It specifies the Format id of the chart
      /// </summary>
    public int FormatId
    {
        get
        {
            return m_formatId;
        }
        set
        {
            m_formatId = value;
        }
    }
    public bool HasPivotSource
    {
      get
      {
        return PivotSource != null || PreservedPivotSource != null;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show all field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show all field buttons]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowAllFieldButtons
    {
        get
        {
            return m_showAllFieldButtons;
        }
        set
        {
            m_showAllFieldButtons = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show value field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show value field buttons]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowValueFieldButtons
    {
        get
        {
            return m_showValueFieldButtons;
        }
        set
        {

            m_showValueFieldButtons = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show axis field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show axis field buttons]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowAxisFieldButtons
    {
        get
        {
            return m_showAxisFieldButtons;
        }
        set
        {
            m_showAxisFieldButtons = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show legend field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show legend field buttons]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowLegendFieldButtons
    {
        get
        {
            return m_showLegendFieldButtons;
        }
        set
        {
            m_showLegendFieldButtons = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show report filter field buttons].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [show report filter field buttons]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowReportFilterFieldButtons
    {
        get
        {
            return m_showReportFilterFieldButtons;
        }
        set
        {
            m_showReportFilterFieldButtons = value;
        }
    }
    /// <summary>
    /// Returns or sets the height of a 3-D chart as a percentage of the chart width
    /// (between 5 and 500 percent).
    /// </summary>
    public int HeightPercent
    {
      get
      {
        return ChartFormat.HeightPercent;
      }
      set
      {
        ChartFormat.HeightPercent = value;
      }
    }
    /// <summary>
    /// Returns or sets the depth of a 3-D chart as a percentage of the chart width
    /// (between 20 and 2000 percent).
    /// </summary>
    public int DepthPercent
    {
      get
      {
        return ChartFormat.DepthPercent;
      }
      set
      {
        ChartFormat.DepthPercent = value;
      }
    }
    /// <summary>
    /// Returns or sets the distance between the data series in a 3-D chart, as a percentage of the marker width.( 0 - 500 )
    /// </summary>
    public int GapDepth
    {
      get
      {
        return ChartFormat.GapDepth;
      }
      set
      {
        ChartFormat.GapDepth = value;
      }
    }
    /// <summary>
    /// True if the chart axes are at right angles, independent of chart rotation or elevation.
    /// </summary>
    public bool   RightAngleAxes
    {
      get
      {
        return ChartFormat.RightAngleAxes;
      }
      set
      {
        ChartFormat.RightAngleAxes = value;
      }
    }
    /// <summary>
    /// True if Microsoft Excel scales a 3-D chart so that it's closer in size to the equivalent 2-D chart..
    /// </summary>
    public bool   AutoScaling
    {
      get
      {
        return ChartFormat.AutoScaling;
      }
      set
      {
        ChartFormat.AutoScaling = value;
      }
    }
    /// <summary>
    /// True if gridlines are drawn two-dimensionally on a 3-D chart.
    /// </summary>
    public bool   WallsAndGridlines2D
    {
      get
      {
        return ChartFormat.WallsAndGridlines2D;
      }
      set
      {
        ChartFormat.WallsAndGridlines2D = value;
      }
    }
    /// <summary>
    /// Type of the chart.
    /// </summary>
    public ExcelChartType ChartType
    {
      get
      {
        DetectChartType();
        return m_chartType;
      }
      set
      {
        if(!m_book.Loading)
        this.m_radarStyle = null;
        ChangeChartType( value, false );
      }
    }
    /// <summary>
    /// Represents the Series NameLevel.
    /// </summary>
    public ExcelSeriesNameLevel SeriesNameLevel
    {
        get
        {
            return m_seriesNameLevel;
        }
        set
        {
            m_seriesNameLevel = value;
        }
    }
    /// <summary>
    /// Represents the Cateories NameLevel.
    /// </summary>
    public ExcelCategoriesLabelLevel CategoryLabelLevel
    {
        get
        {
            return m_categoriesLabelLevel;
        }
        set
        {
            m_categoriesLabelLevel = value;
        }
    }
    /// <summary>
    /// DataRange for the chart series.
    /// </summary>
    public IRange DataRange
    {
      get
      {
        if( m_dataRange == null )
          m_dataRange = DetectDataRange();

        return m_dataRange;
      }
      set
      {
        if( m_dataRange != value )
        {
          ExcelChartType type = ChartType;

          //if( m_series.Count != 0 )
          //  m_bSeriesInRows = DetectIsInRow( value );

          m_dataRange = value;
          OnDataRangeChanged( type );

            ChartSerieImpl serie = ( ChartSerieImpl )m_series[ 0 ];
            UpdateSeries(m_series);
            Categories.Clear();
            UpdateCategory(m_series,true);
            
            if (serie.NumRefFormula != null || serie.StrRefFormula != null)
            {
                serie.NumRefFormula = null;
                serie.StrRefFormula = null;
            }
        }
      }
    }
    /// <summary>
    /// Find and Update Category
    /// </summary>
    /// <param name="series"></param>
    /// <param name="fromDataRange"></param>
    public void UpdateCategory(ChartSeriesCollection series,bool fromDataRange)
    {
        IRange values = null;
        int count = series[0].Values.Count;
        ChartImpl chart = (series[0] as ChartSerieImpl).ParentChart;       
        IRange category = chart.Series[0].CategoryLabels;
        chart.DetectIsInRowOnParsing();
        if (chart.DataRange != null)
        {
            chart.IsSeriesInRows = DetectIsInRow(chart.Series[0].Values);
            GetSerieOrAxisRange(chart.DataRange, chart.IsSeriesInRows, out values);
            GetSerieOrAxisRange(values, !chart.IsSeriesInRows, out values);
            int count1 = values.Count / count;
            for (int i = 0; i < count; i++)
            {
                IRange cate_range = GetCategoryRange(values, out values, count1, chart.IsSeriesInRows);
                if (fromDataRange)
                    (chart.Categories as ChartCategoryCollection).Add();
                (chart.Categories[i] as ChartCategory).CategoryLabel = category;
                (chart.Categories[i] as ChartCategory).Values = cate_range;
                if (chart.Categories[0].CategoryLabel != null)
                    (chart.Categories[i] as ChartCategory).Name = chart.Categories[0].CategoryLabel.Cells[i].Text;
            }
        }
        
    }
    /// <summary>
    /// Update seriesFilter
    /// </summary>
    /// <param name="series"></param>
    public void UpdateSeries(ChartSeriesCollection series)
    {
        for (int i = 0; i < series.Count; i++)
        {
            series[i].IsFiltered = false;
        }
    }
    /// <summary>
    /// Find the Category Range
    /// </summary>
    /// <param name="Chartvalues"></param>
    /// <param name="values"></param>
    /// <param name="count"></param>
    /// <param name="bIsInRow"></param>
    /// <returns></returns>
    public static IRange GetCategoryRange(IRange Chartvalues, out IRange values, int count, bool bIsInRow)
    {
        int firstRow = Chartvalues.Row;
        int lastRow = Chartvalues.LastRow;
        int firstColumn = Chartvalues.Column;
        int lastColumn = Chartvalues.LastColumn;
        IRange result = null;
        if (Chartvalues.Count == count)
        {
            values = Chartvalues;
            return Chartvalues;
        }
        result = (bIsInRow)
          ? Chartvalues[firstRow, firstColumn, lastRow, firstColumn]
          : Chartvalues[firstRow, firstColumn, firstRow, lastColumn];
        if (firstRow == lastRow && firstColumn == lastColumn)
        {
            values = result;
        }
        else
        {
            values = (bIsInRow)
              ? Chartvalues[firstRow, firstColumn + 1, lastRow, lastColumn]
              : Chartvalues[firstRow + 1, firstColumn, lastRow, lastColumn];
        }
        return result;
    }
    /// <summary>
    /// True if series are in rows in DataRange;
    /// otherwise False.
    /// </summary>
    public bool   IsSeriesInRows
    {
      get
      {
        return m_bSeriesInRows;
      }
      set
      {
        int iCount = m_series.Count;

        if( DataRange == null && iCount != 0 )
          throw new NotSupportedException( "This property supported only in chart where can detect data range." );

        if( m_bSeriesInRows != value )
        {
          m_bSeriesInRows = value;

          if (iCount != 0)
          {
              GetFilter();
              OnSeriesInRowsChanged();
              m_categories.Clear();
              Setfilter();
              UpdateCategory(m_series,false);             
              
          }
        }
      }
    }
    /// <summary>
    /// Getting the filter from series and categories
    /// </summary>
    private void GetFilter()
    {
        category = new bool[m_series.Count];
        series = new bool[(m_categories as IChartCategories).Count];
        for (int i = 0; i < m_series.Count; i++)
        {
            category[i] = (m_series as IChartSeries)[i].IsFiltered;
        }
        for (int i = 0; i <(m_categories as IChartCategories).Count;i++)
        {
            series[i] = (m_categories as IChartCategories)[i].IsFiltered;
        }
        
        ChartImpl chart = (m_series[0] as ChartSerieImpl).ParentChart;
        if (chart != null)
        {
            ExcelSeriesNameLevel seriesname = chart.m_seriesNameLevel;
            ExcelCategoriesLabelLevel categoryname = chart.CategoryLabelLevel;
            switch (seriesname)
            {
                case ExcelSeriesNameLevel.SeriesNameLevelAll:
                    chart.CategoryLabelLevel = ExcelCategoriesLabelLevel.CategoriesLabelLevelAll;
                    break;
                default:
                    chart.CategoryLabelLevel = ExcelCategoriesLabelLevel.CategoriesLabelLevelNone;
                    break;
            }
            switch (categoryname)
            {
                case ExcelCategoriesLabelLevel.CategoriesLabelLevelAll:
                    chart.SeriesNameLevel = ExcelSeriesNameLevel.SeriesNameLevelAll;
                    break;
                default:
                    chart.SeriesNameLevel = ExcelSeriesNameLevel.SeriesNameLevelNone;
                    break;
            }
        }

    }
    /// <summary>
    /// Filter is assigned to the categories/series.
    /// </summary>
    private void Setfilter()
    {
        for (int i = 0; i < m_series.Count; i++)
        {
            (m_series as IChartSeries)[i].IsFiltered = series[i];
        }
        for (int i = 0; i < category.Length; i++)
        {
            {
                m_categories.Add();
                (m_categories as IChartCategories)[i].IsFiltered = category[i];
            }
        }
    }
    /// <summary>
    /// Title of the chart.
    /// </summary>
    public string ChartTitle
    {
      get
      {
        return ChartTitleArea.Text;
      }
      set
      {
        ChartTitleArea.Text = value;
      }
    }

    /// <summary>
    /// Returns object that describes chart title area. Read-only.
    /// </summary>
    public IChartTextArea ChartTitleArea
    {
      get
      {

        if( m_title == null )
          CreateChartTitle();

        return m_title;
      }
    }
    /// <summary>
    /// Gets font used for title displaying. Read-only.
    /// </summary>
    public IFont ChartTitleFont
    {
      get
      {
        return ( IFont )ChartTitleArea;
      }
    }
    /// <summary>
    /// Title of the category axis.
    /// </summary>
    public string CategoryAxisTitle
    {
      get
      {
        return PrimaryCategoryAxis.Title;
      }
      set
      {
        PrimaryCategoryAxis.Title = value;
      }
    }

    /// <summary>
    /// Title of the value axis.
    /// </summary>
    public string ValueAxisTitle
    {
      get
      {
        return PrimaryValueAxis.Title;
      }
      set
      {
        PrimaryValueAxis.Title = value;
      }
    }
    /// <summary>
    /// Title of the secondary category axis.
    /// </summary>
    public string SecondaryCategoryAxisTitle
    {
      get
      {
        return SecondaryCategoryAxis.Title;
      }
      set
      {
        SecondaryCategoryAxis.Title = value;
      }
    }
    /// <summary>
    /// Title of the secondary value axis.
    /// </summary>
    public string SecondaryValueAxisTitle
    {
      get
      {
        return SecondaryValueAxis.Title;
      }
      set
      {
        SecondaryValueAxis.Title = value;
      }
    }
    /// <summary>
    /// Title of the series axis.
    /// </summary>
    public string SeriesAxisTitle
    {
      get
      {
        return PrimarySerieAxis.Title;
      }
      set
      {
        PrimarySerieAxis.Title = value;
      }
    }
    /// <summary>
    /// Returns primary category axis. Read-only.
    /// </summary>
    public IChartCategoryAxis PrimaryCategoryAxis
    {
      get
      {
        //return ( IChartCategoryAxis )GetAxis( ExcelAxisType.Category, 0 );
        return ( IChartCategoryAxis )m_primaryParentAxis.CategoryAxis;
      }
    }
    /// <summary>
    /// Returns primary value axis. Read-only.
    /// </summary>
    public IChartValueAxis PrimaryValueAxis
    {
      get
      {
        //return ( IChartValueAxis )GetAxis( ExcelAxisType.Value, 0 );
        return ( IChartValueAxis )m_primaryParentAxis.ValueAxis;
      }
    }
    /// <summary>
    /// Returns primary series axis. Read-only.
    /// </summary>
    public IChartSeriesAxis PrimarySerieAxis
    {
      get
      {
        if( !IsSeriesAxisAvail && !Loading )//Array.IndexOf( DEF_SUPPORT_SERIES_AXIS, ChartType ) == -1 )
          throw new NotSupportedException( "Series axis doesnot exist in current chart type." );

        return ( IChartSeriesAxis )m_primaryParentAxis.SeriesAxis;
      }
    }
    /// <summary>
    /// Returns secondary category axis. Read-only.
    /// </summary>
    public IChartCategoryAxis SecondaryCategoryAxis
    {
      get
      {
        //return ( IChartCategoryAxis )GetAxis( ExcelAxisType.Category, 1 );
        return ( IChartCategoryAxis )m_secondaryParentAxis.CategoryAxis;
      }
    }
    /// <summary>
    /// Returns secondary value axis. Read-only.
    /// </summary>
    public IChartValueAxis SecondaryValueAxis
    {
      get
      {
        //return ( IChartValueAxis )GetAxis( ExcelAxisType.Value, 1 );
        return ( IChartValueAxis )m_secondaryParentAxis.ValueAxis;
      }
    }
    /// <summary>
    /// Page setup for the chart. Read-only.
    /// </summary>
    public IChartPageSetup PageSetup
    {
      get
      {
        return m_pageSetup;
      }
    }

    /// <summary>
    /// X coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    public double XPos
    {
      get
      {
        return m_dXPos;
      }
      set
      {
        m_dXPos = value;
      }
    }

    /// <summary>
    /// Y coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    public double YPos
    {
      get
      {
        return m_dYPos;
      }
      set
      {
        m_dYPos = value;
      }
    }

    /// <summary>
    /// Width of the chart in points (1/72 inch).
    /// </summary>
    public double Width
    {
        get
        {
            return (int)Math.Round(ApplicationImpl.ConvertToPixels(EMUWidth, MeasureUnits.EMU));
        }
        set
        {
            m_dWidth = Math.Round(ApplicationImpl.ConvertFromPixel(value, MeasureUnits.EMU)); ;
        }
    }

    /// <summary>
    /// Height of the chart in points (1/72 inch).
    /// </summary>
    public double Height
    {
      get
      {
          return (int)Math.Round(ApplicationImpl.ConvertToPixels(EMUHeight, MeasureUnits.EMU)); 
      }
      set
      {
          m_dHeight = Math.Round(ApplicationImpl.ConvertFromPixel(value, MeasureUnits.EMU)); ;
      }
    }
    internal double EMUHeight
    {
        get
        {
            return m_dHeight;
        }
        set
        {
            m_dHeight = value;
        }
    }
    internal double EMUWidth
    {
        get
        {
            return m_dWidth;
        }
        set
        {
            m_dWidth = value;
        }
    }
    /// <summary>
    /// Collection of the all series of this chart. Read-only.
    /// </summary>
    public IChartSeries Series
    {
      get
      {
        return m_series;
      }
    }
    /// <summary>
    /// Collection of all categories of this chart.Read-only
    /// </summary>
    public IChartCategories Categories
    {
        get
        {
            return m_categories;
        }
    }
    /// <summary>
    /// Returns chart format collection in primary axis.
    /// </summary>
    public ChartFormatCollection PrimaryFormats
    {
      get
      {
        return m_primaryParentAxis.ChartFormats;
      }
    }
    /// <summary>
    /// Returns chart format collection in secondary axis.
    /// </summary>
    public ChartFormatCollection SecondaryFormats
    {
      get
      {
        return m_secondaryParentAxis.ChartFormats;
      }
    }
    /// <summary>
    /// Returns an object that represents the complete chart area for the chart. Read-only.
    /// </summary>
    public IChartFrameFormat ChartArea
    {
      get
      {
        if( m_chartArea == null )
        {
          m_chartArea = new ChartFrameFormatImpl( Application, this );
          m_chartArea.Interior.ForegroundColorIndex = ExcelKnownColors.WhiteCustom;
        }

        return m_chartArea;
      }
    }
    /// <summary>
    /// Indicates whether chart has chart area.
    /// </summary>
    public bool HasChartArea
    {
      get
      {
        return m_chartArea != null;
      }
      set
      {
        if( value != HasChartArea )
        {
          m_chartArea = value
            ? new ChartFrameFormatImpl( Application, this )
            : null;
        }
      }
    }
    /// <summary>
    /// Indicates whether chart has plot area.
    /// </summary>
    public bool HasPlotArea
    {
      get
      {
        return m_plotArea != null;
      }
      set
      {
        if( value != HasPlotArea )
        {
          m_plotArea = value
            ? new ChartPlotAreaImpl( Application, this, ChartType )
            : null;
        }
      }
    }
    /// <summary>
    /// Returns plot area frame format. Read-only.
    /// </summary>
    public IChartFrameFormat PlotArea
    {
      get
      {
        return m_plotArea;
      }
      set
      {
        m_plotArea = ( ChartPlotAreaImpl )value;
      }
    }
    /// <summary>
    /// Returns primary parent axis.
    /// </summary>
    public ChartParentAxisImpl PrimaryParentAxis
    {
      get
      {
        return m_primaryParentAxis;
      }
    }
    /// <summary>
    /// Returns secondary parent axis.
    /// </summary>
    public ChartParentAxisImpl SecondaryParentAxis
    {
      get
      {
        return m_secondaryParentAxis;
      }
    }
    /// <summary>
    /// Represents chart walls.
    /// </summary>
    public IChartWallOrFloor Walls
    {
      get
      {
        if( !m_book.Loading && Array.IndexOf( DEF_WALLS_OR_FLOOR_TYPES, ChartType ) == -1 )
          throw new ApplicationException( "Walls are not supported in this chart type" );

        if( m_walls == null )
          m_walls = new ChartWallOrFloorImpl( Application, this, true );
        
        return m_walls;
      }
      set
      {
        m_walls = ( ChartWallOrFloorImpl )value;
      }
    }
    /// <summary>
    /// Represents the SideWall 
    /// </summary>
    public IChartWallOrFloor SideWall
    {
        get
        {
            if (!m_book.Loading && Array.IndexOf(DEF_WALLS_OR_FLOOR_TYPES, ChartType) == -1)
                throw new ApplicationException("Walls are not supported in this chart type");

            if (m_sidewall == null)
                m_sidewall = new ChartWallOrFloorImpl(Application, this,true);

            return m_sidewall;
        }
        set
        {
            m_sidewall = (ChartWallOrFloorImpl)value;
        }
    }
    /// <summary>
    /// Represents the BackWall 
    /// </summary>
    public IChartWallOrFloor BackWall
    {
        get
        {                       
            return Walls;
        }
        set
        {
            Walls = (ChartWallOrFloorImpl)value;            
        }
    }
    /// <summary>
    /// Represents chart floor.
    /// </summary>
    public IChartWallOrFloor Floor
    {
      get
      {
        if( !m_book.Loading && !SupportWallsAndFloor )
          throw new ApplicationException( "Floor is not supported by this chart type." );

        if( m_floor == null )
          m_floor = new ChartWallOrFloorImpl( Application, this, false );
        
        return m_floor;
      }
      set
      {
        m_floor = ( ChartWallOrFloorImpl )value;
      }
    }
    /// <summary>
    /// Represents charts dataTable object.
    /// </summary>
    public IChartDataTable DataTable
    {
      get
      {
        return m_dataTable;
      }
    }
    /// <summary>
    /// True if the chart has a data table.
    /// </summary>
    public bool HasDataTable
    {
      get
      {
        return m_bHasDataTable;
      }
      set
      {
        if( m_bHasDataTable != value )
        {
          if( value )
          {
            CheckSupportDataTable();
          }

          m_bHasDataTable = value;
          m_dataTable = ( value ) ? new ChartDataTableImpl( Application, this ) : null;
        }
      }
    }
    /// <summary>
    /// Represents chart legend.
    /// </summary>
    public IChartLegend Legend
    {
      get
      {
        return m_legend;
      }
    }
    /// <summary>
    /// True if the chart has a legend object.
    /// </summary>
    public bool HasLegend
    {
      get
      {
        return m_bHasLegend;
      }
      set
      {
        if( m_bHasLegend != value )
        {
          m_bHasLegend = value;
          m_legend = ( value ) ? new ChartLegendImpl( Application, this ) : null;
        }
      }
    }
    /// <summary>
    /// Represents the way that blank cells are plotted on a chart.
    /// </summary>
    public ExcelChartPlotEmpty DisplayBlanksAs
    {
      get
      {
        return m_chartProperties.PlotBlank;
      }
      set
      {
        m_chartProperties.PlotBlank = value;
      }
    }
    /// <summary>
    /// True if only visible cells are plotted. False if both visible and hidden cells are plotted.
    /// </summary>
    public bool PlotVisibleOnly
    {
      get
      {
        return m_chartProperties.IsPlotVisOnly;
      }
      set
      {
        m_chartProperties.IsPlotVisOnly = value;
      }
    }
    /// <summary>
    /// Indicates whether to show the PlotVisOnly attribute or not
    /// </summary>
    public bool ShowPlotVisible
    {
        get 
        { 
            return m_showPlotVisible; 
        }
        set
        {
            m_showPlotVisible = value;
        }
    }
    /// <summary>
    /// True if Microsoft Excel resizes the chart to match the size of the chart sheet window.
    /// False if the chart size isn't attached to the window size. Applies only to chart sheets.
    /// </summary>
    public bool SizeWithWindow
    {
      get
      {
        return ( m_bInWorksheet )
          ? true
          : !m_chartProperties.IsNotSizeWith;
      }
      set
      {
        if( !m_bInWorksheet )
          m_chartProperties.IsNotSizeWith = !value;
      }
    }
    /// <summary>
    /// Indicates whether this chart supports walls and floor. Read-only.
    /// </summary>
    public bool SupportWallsAndFloor
    {
      get
      {
        return Array.IndexOf( DEF_WALLS_OR_FLOOR_TYPES, ChartType ) >= 0;
      }
    }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    public override bool ProtectDrawingObjects
    {
      get
      {
        return ( InnerProtection & ExcelSheetProtection.Objects ) != 0;
      }
    }
    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    public override bool ProtectScenarios
    {
      get
      {
        return ( InnerProtection & ExcelSheetProtection.Scenarios ) != 0;
      }
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    public override ExcelSheetProtection Protection
    {
      get
      {
        return base.Protection & ~ExcelSheetProtection.Scenarios;
      }
    }
    /// <summary>
    /// Return attached label layout record. Read-only
    /// </summary>
    public ChartPlotAreaLayoutRecord PlotAreaLayout
    {
        get
        {
            if (m_plotAreaLayout == null)
            {
                m_plotAreaLayout = (ChartPlotAreaLayoutRecord)
                    BiffRecordFactory.GetRecord(TBIFFRecord.PlotAreaLayout);
            }
            return m_plotAreaLayout;
        }
    }
    #endregion

    #region Implementation properties
    /// <summary>
    /// Tab color.
    /// </summary>
    public override ExcelKnownColors TabColor
    {
      get
      {
        return base.TabColor;
      }
      set
      {
        if( m_bInWorksheet )
          throw new NotSupportedException();

        base.TabColor = value;
      }
    }
    /// <summary>
    /// Returns True if chart has a category axis. Read-only.
    /// </summary>
    public bool IsCategoryAxisAvail
    {
      get
      {
        return ( Array.IndexOf( NO_CATEGORY_AXIS, ChartType ) == -1 );
      }
    }
    /// <summary>
    /// Returns True if chart has a value axis. Read-only.
    /// </summary>
    public bool IsValueAxisAvail
    {
      get
      {
        return ( Array.IndexOf( NO_CATEGORY_AXIS, ChartType ) == -1 );
      }
    }
    /// <summary>
    /// Returns True if chart has a series axis. Read-only.
    /// </summary>
    public bool IsSeriesAxisAvail
    {
      get
      {
        ExcelChartType chartType = HasPivotSource ? PivotChartType : ChartType;
        return ( Array.IndexOf( DEF_SUPPORT_SERIES_AXIS, chartType ) != -1 );
        //return IsChart3D;//false;
      }
    }
    /// <summary>
    /// Returns True if chart is stacked. Read-only.
    /// </summary>
    public bool IsStacked
    {
      get
      {
        return GetIsStacked( ChartType );
      }
    }
    /// <summary>
    /// Returns True if chart is 100%. Read-only.
    /// </summary>
    public bool IsChart_100
    {
      get
      {
        return GetIs100( ChartType );
      }
    }
    /// <summary>
    /// Returns True if chart is 3D. Read-only.
    /// </summary>
    public bool IsChart3D
    {
      get
      {
        return ( Array.IndexOf( CHARTS3D, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Gets a value indicating whether this instance is pivot chart3 D.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is pivot chart3 D; otherwise, <c>false</c>.
    /// </value>
    public bool IsPivotChart3D
    {
        get
        {
            return (Array.IndexOf(DEF_NEED_VIEW_3D, PivotChartType) != -1);
        }
    }
    /// <summary>
    /// Returns True if chart is line. Read-only.
    /// </summary>
    public bool IsChartLine
    {
      get
      {
        return ( Array.IndexOf( CHARTS_LINE, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart needs data format to be saved. Read-only.
    /// </summary>
    public bool NeedDataFormat
    {
      get
      {
        return ( IsChart3D || IsChartLine || IsChartExploded || IsChartScatter
          || IsChartStock
          || ( ChartType == ExcelChartType.Bubble_3D )
          || ( ChartType == ExcelChartType.Radar ) )
          && ( ChartType != ExcelChartType.Surface_NoColor_Contour );
      }
    }
    /// <summary>
    /// Returns True if chart needs marker format to be saved. Read-only.
    /// </summary>
    public bool NeedMarkerFormat
    {
      get
      {
        return IsChartPyramid || IsChartCone || IsChartCylinder;
      }
    }
    /// <summary>
    /// Returns True if chart is a bar chart. Read-only.
    /// </summary>
    public bool IsChartBar
    {
      get
      {
        return ( ChartType.ToString().IndexOf( START_BAR ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a pyramid shape. Read-only.
    /// </summary>
    public bool IsChartPyramid
    {
      get
      {
        return ( ChartType.ToString().IndexOf( "Pyramid" ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a conical shape. Read-only.
    /// </summary>
    public bool IsChartCone
    {
      get
      {
        return ( ChartType.ToString().IndexOf( "Cone" ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a cylinder shape. Read-only.
    /// </summary>
    public bool IsChartCylinder
    {
      get
      {
        return ( ChartType.ToString().IndexOf( "Cylinder" ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a bubble chart. Read-only.
    /// </summary>
    public bool IsChartBubble
    {
      get
      {
        return ( Array.IndexOf( CHARTS_BUBBLE, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a doughnut chart. Read-only.
    /// </summary>
    public bool IsChartDoughnut
    {
      get
      {
        return ( ChartType.ToString().IndexOf( "Doughnut" ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart should have a different color for each series value. Read-only.
    /// </summary>
    public bool IsChartVaryColor
    {
      get
      {
        return ( Array.IndexOf( CHARTS_VARYCOLOR, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is exploded. Read-only.
    /// </summary>
    public bool IsChartExploded
    {
      get
      {
        return ( Array.IndexOf( CHARTS_EXPLODED, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart has series lines. Read-only.
    /// </summary>
    public bool IsSeriesLines
    {
      get
      {
        return CanChartHaveSeriesLines;
      }
    }
    /// <summary>
    /// Returns True if chart can have series lines. Read-only.
    /// </summary>
    public bool CanChartHaveSeriesLines
    {
      get
      {
        return ( Array.IndexOf( CHART_SERIES_LINES, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a scatter chart. Read-only.
    /// </summary>
    public bool IsChartScatter
    {
      get
      {
        return ( Array.IndexOf( CHARTS_SCATTER, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns default line pattern for the chart. Read-only.
    /// </summary>
    public ExcelChartLinePattern DefaultLinePattern
    {
      get
      {
        if( ChartType == ExcelChartType.Scatter_Markers || IsChartStock)
          return ExcelChartLinePattern.None;

        return ExcelChartLinePattern.Solid;
      }
    }
    /// <summary>
    /// Returns True if chart has smoothed lines. Read-only.
    /// </summary>
    public bool IsChartSmoothedLine
    {
      get
      {
        return ( Array.IndexOf( CHARTS_SMOOTHED_LINE, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if this is a stock chart. Read-only.
    /// </summary>
    public bool IsChartStock
    {
      get
      {
        return ( Array.IndexOf( CHARTS_STOCK, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart needs drop bars to be saved. Read-only.
    /// </summary>
    public bool NeedDropBar
    {
      get
      {
        return ( ChartType == ExcelChartType.Stock_OpenHighLowClose )
          || ( ChartType == ExcelChartType.Stock_VolumeOpenHighLowClose );
      }
    }
    /// <summary>
    /// Returns True if chart is a stock chart with volume. Read-only.
    /// </summary>
    public bool IsChartVolume
    {
      get
      {
        return ( ChartType == ExcelChartType.Stock_VolumeHighLowClose 
              || ChartType == ExcelChartType.Stock_VolumeOpenHighLowClose );
      }
    }
    /// <summary>
    /// Returns True if chart has perspective. Read-only.
    /// </summary>
    public bool IsPerspective
    {
      get
      {
        return ( Array.IndexOf( CHARTS_PERSPECTIVE, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart is a clustered chart. Read-only.
    /// </summary>
    public bool IsClustered
    {
      get
      {
        return GetIsClustered( ChartType );
      }
    }
    /// <summary>
    /// Returns True if chart has no plot area. Read-only.
    /// </summary>
    public bool NoPlotArea
    {
      get
      {
        return IsChartRadar || IsChartPie || IsChartDoughnut //|| IsChartVolume
          || ( ChartType == ExcelChartType.Surface_NoColor_Contour );
      }
    }
    /// <summary>
    /// Returns True if chart is a radar chart. Read-only.
    /// </summary>
    public bool IsChartRadar
    {
      get
      {
        return ChartType.ToString().StartsWith( "Radar" );
      }
    }
    /// <summary>
    /// Returns True if chart is a pie chart. Read-only.
    /// </summary>
    public bool IsChartPie
    {
      get
      {
        return GetIsChartPie( ChartType );
      }
    }
    /// <summary>
    /// Returns True if chart has walls. Read-only.
    /// </summary>
    public bool IsChartWalls
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Returns True if chart has floor. Read-only.
    /// </summary>
    public bool IsChartFloor
    {
      get
      {
        return (ExcelChartType.Surface_NoColor_Contour == ChartType )
          || (ExcelChartType.Surface_Contour == ChartType );
      }
    }
    /// <summary>
    /// Gets the serialized axis ids.
    /// </summary>
    /// <value>The serialized axis ids.</value>
    internal List<int> SerializedAxisIds
    {
        get
        {
            if (m_axisIds == null)
                m_axisIds = new List<int>();

            return m_axisIds;
        }

    }
    /// <summary>
    /// Returns True if secondary category axis present. Read-only.
    /// </summary>
    public bool IsSecondaryCategoryAxisAvail
    {
      get
      {
        //return false;
        //return IsChartVolume;
        return SecondaryCategoryAxis != null;
      }
    }
    /// <summary>
    /// Returns True if secondary value axis present. Read-only.
    /// </summary>
    public bool IsSecondaryValueAxisAvail
    {
      get
      {
        //return IsChartVolume;
        return SecondaryValueAxis != null;
      }
    }
    /// <summary>
    /// Returns True if at least one of the secondary axes is present. Read-only.
    /// </summary>
    public bool IsSecondaryAxes
    {
      get
      {
        return IsSecondaryValueAxisAvail || IsSecondaryCategoryAxisAvail
          || m_bIsSecondaryAxis;
      }
      set
      {
        m_bIsSecondaryAxis = value;
      }    
    }
    /// <summary>
    /// Returns True if chart needs special data labels serialization.
    /// Read-only.
    /// </summary>
    public bool IsSpecialDataLabels
    {
      get
      {
        return ( Array.IndexOf( DEF_SPECIAL_DATA_LABELS, ChartType ) != -1 );
        }
    }
    /// <summary>
    /// Returns True if chart can have percentage data labels. Read-only.
    /// </summary>
    public bool CanChartPercentageLabel
    {
      get
      {
        return ( Array.IndexOf( DEF_CHART_PERCENTAGE, ChartType ) != -1 );
      }
    }
    /// <summary>
    /// Returns True if chart can have bubble data labels. Read-only.
    /// </summary>
    public bool CanChartBubbleLabel
    {
      get
      {
        return IsChartBubble;
      }
    }
    /// <summary>
    /// Indicates whether chart was manually formatted.
    /// </summary>
    public bool IsManuallyFormatted
    {
      get
      {
        return m_chartProperties.IsManSerAlloc;
      }
      set
      {
        m_chartProperties.IsManSerAlloc = value;
      }
    }
    /// <summary>
    /// This record stores scale factors for font scaling.
    /// </summary>
    private ChartPlotGrowthRecord PlotGrowth
    {
      get
      {
        if( m_plotGrowth == null )
        {
          m_plotGrowth = ( ChartPlotGrowthRecord )BiffRecordFactory.GetRecord(
            TBIFFRecord.ChartPlotGrowth );
        }

        return m_plotGrowth;
      }
    }
    /// <summary>
    /// Plot are bounding box. Read-only.
    /// </summary>
    private ChartPosRecord PlotAreaBoundingBox
    {
      get
      {
        if( m_plotAreaBoundingBox == null )
        {
          m_plotAreaBoundingBox = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
          m_plotAreaBoundingBox.BottomRight = 2;
          m_plotAreaBoundingBox.TopLeft = 2;
        }

        return m_plotAreaBoundingBox;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl InnerWorkbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Returns inner frame format. Read-only.
    /// </summary>
    public ChartFrameFormatImpl InnerChartArea
    {
      get
      {
        return ChartArea as ChartFrameFormatImpl;
      }
    }
    /// <summary>
    /// Returns inner plot area frame format. Read-only.
    /// </summary>
    public ChartFrameFormatImpl InnerPlotArea
    {
      get
      {
        if( m_plotAreaFrame == null )
          m_plotAreaFrame = new ChartFrameFormatImpl( Application, this );

        return m_plotAreaFrame;
      }
    }
    /// <summary>
    /// Returns start type of chart type. Read-only.
    /// </summary>
    public string ChartStartType
    {
      get
      {
        return ChartFormatImpl.GetStartSerieType( ChartType );
      }
    }
    /// <summary>
    /// Page setup for the chart. Read-only.
    /// </summary>
    public override PageSetupBaseImpl PageSetupBase
    {
      get
      {
        return m_pageSetup;
      }
    }
    /// <summary>
    /// Gets chart options. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartShtpropsRecord ChartProperties
    {
      get
      {
        return m_chartProperties;
      }
    }
    /// <summary>
    /// Represents if chart is reading from biff stream. Read-only.
    /// </summary>
    public bool Loading
    {
      get
      {
        return m_book.Loading;
      }
    }
    /// <summary>
    /// Gets chart format for 3-d charts. Read-only.
    /// </summary>
    internal ChartFormatImpl ChartFormat
    {
      get
      {
        if( m_series.Count == 0 )
          throw new ApplicationException( "cannot get format." );

        ChartSerieImpl serie = ( ChartSerieImpl )m_series[ 0 ];
        return serie.GetCommonSerieFormat();
      }
    }
    /// <summary>
    /// Indicates if we change chart or Series type.
    /// </summary>
    public bool TypeChanging
    {
      get
      {
        return m_bTypeChanging;
      }
      set
      {
        m_bTypeChanging = value;
      }
    }
    /// <summary>
    /// Resulting chart type after type change operation.
    /// </summary>
    public ExcelChartType DestinationType
    {
      get
      {
        return m_destinationType;
      }
      set
      {
        m_destinationType = value;
      }
    }
    /// <summary>
    /// Returns collection of chart relations. Read-only.
    /// </summary>
    public RelationCollection Relations
    {
      get
      {
        if( m_relations == null )
          m_relations = new RelationCollection();

        return m_relations;
      }
    }
    /// <summary>
    /// Style index for Excel 2007 chart.
    /// </summary>
    public int Style
    {
      get
      {
        return m_iStyle2007;
      }
      set
      {
        m_iStyle2007 = value;
      }
    }
    /// <summary>
    /// Gets value indicating whether floor object was created.
    /// </summary>
    public bool HasFloor
    {
      get
      {
        return m_floor != null;
      }
    }
    /// <summary>
    /// Gets value indicating whether floor object was created.
    /// </summary>
    public bool HasWalls
    {
      get
      {
        return m_walls != null;
      }
    }
    /// <summary>
    /// Gets or sets pivot formats stream.
    /// </summary>
    public Stream PivotFormatsStream
    {
      get
      {
        return m_pivotFormatsStream;
      }
      set
      {
        m_pivotFormatsStream = value;
      }
    }
    /// <summary>
    /// Gets or sets zoomToFit value.
    /// </summary>
    public bool ZoomToFit
    {
      get
      {
        return SizeWithWindow;
      }
      set
      {
        SizeWithWindow = value;
      }
    }
    /// <summary>
    /// Gets default protection options for the worksheet.
    /// </summary>
    protected override ExcelSheetProtection DefaultProtectionOptions
    {
      get
      {
        return ExcelSheetProtection.Objects | ExcelSheetProtection.Scenarios | ExcelSheetProtection.Content;
      }
    }
    /// <summary>
    /// Gets value indicating whether chart is embeded into worksheet.
    /// </summary>
    public bool IsEmbeded
    {
      get
      {
        return m_bInWorksheet;
      }
    }
    /// <summary>
    /// Gets font index from the default font records.
    /// </summary>
    public int DefaultTextIndex
    {
      get
      {
        int result = 0;

        if( m_lstDefaultText != null && m_lstDefaultText.Count > 0 )
        {
          List<BiffRecordRaw> records = m_lstDefaultText.GetByIndex( 0 );

          if( records != null )
          {
            foreach( BiffRecordRaw record in records )
            {
              if( record.TypeCode == TBIFFRecord.ChartFontx )
              {
                result = ( ( ChartFontxRecord )record ).FontIndex;
                break;
              }
            }
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Gets or sets preserved band formats for surface chart.
    /// </summary>
    public Stream PreservedBandFormats
    {
      get
      {
        return m_bandFormats;
      }
      set
      {
        m_bandFormats = value;
      }
    }
    /// <summary>
    /// Indicates whether chart has title.
    /// </summary>
    public bool HasTitle
    {
      get
      {
        bool result = false;

        if( m_title != null /*&& m_title.Text != null*/)
        {
          if( m_title.Text != null || m_title.FontIndex == 0 ||
            /*m_title.Text == null &&*/!m_title.TextRecord.IsAutoColor ||
            //!m_title.TextRecord.IsAutoText ||
            //!m_title.TextRecord.IsGenerated ||
            !m_title.TextRecord.IsAutoMode )
          {
            result = true;
          }
        }

        return result;
      }
    }
    internal Stream AlternateContent
    {
      get
      {
        return m_alternateContent;
      }
      set
      {
        m_alternateContent = value;
      }
    }
    /// <summary>
    /// Indicates wheather the chart has title
    /// </summary>
    public bool HasChartTitle
    {
        get
        {
            return m_title != null;
        }
    }
    /// <summary>
    /// Preserves the Chart's Default Text Property.
    /// TODO: Need to support Chart default text proprety, should be remove after we start to parse.
    /// </summary>
    internal Stream DefaultTextProperty
    {
        get
        {
            return m_defaultTextProperty;
        }
        set
        {
            m_defaultTextProperty = value;
        }
    }
    /// <summary>
    /// Returns font used for axis text displaying. Read-only.
    /// </summary>
    public IFont Font
    {
        get
        {
            if (m_font == null)
            {
                FontImpl font = (FontImpl)ParentWorkbook.InnerFonts[0];
                m_font = new FontWrapper(font);
            }
            return m_font;
        }
    }   
    /// <summary>
    /// Gets or sets to specify the title shall not be shown for this chart.
    /// </summary>
    internal bool? HasAutoTitle
    {
        get
        {
            return m_hasAutoTitle;
        }
        set
        {
            m_hasAutoTitle = value;
        }

    }
    /// <summary>
    /// Gets or sets the boolean value to load worksheets on demand
    /// </summary>
    internal override bool ParseDataOnDemand
    {
        get
        {
            return m_bParseDataOnDemand;
        }
        set
        {
            m_bParseDataOnDemand = value;
        }
    }
    /// <summary>
    /// Gets or sets to specify the title shall not be shown for this chart.
    /// </summary>
    internal string RadarStyle
    {
        get
        {
            return m_radarStyle;
        }
        set
        {
            m_radarStyle = value;
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns True if chart is a clustered chart. Read-only.
    /// </summary>
    public static bool GetIsClustered( ExcelChartType chartType )
    {
      return ( Array.IndexOf<ExcelChartType>( CHARTS_CLUSTERED, chartType ) >= 0 );
    }
    /// <summary>
    /// Returns True if chart is 100%. Read-only.
    /// </summary>
    public static bool GetIs100( ExcelChartType chartType )
    {
      return ( Array.IndexOf<ExcelChartType>( CHARTS_100, chartType ) >= 0 );
    }
    /// <summary>
    /// Returns True if chart is a stacked chart. Read-only.
    /// </summary>
    public static bool GetIsStacked( ExcelChartType chartType )
    {
      return ( Array.IndexOf<ExcelChartType>( STACKEDCHARTS, chartType ) >= 0 );
    }
    /// <summary>
    /// Indicates whether specified charttype is pie chart.
    /// </summary>
    /// <param name="chartType">Represents chart type to check.</param>
    /// <returns>Value indicating whether specified chart type is pie chart.</returns>
    public static bool GetIsChartPie( ExcelChartType chartType )
    {
      return chartType.ToString().StartsWith( "Pie" );
    }
    /// <summary>
    /// Creates necessary primary axes.
    /// </summary>
    /// <param name="bPrimary">Value indicating whether axis is primary.</param>
    public void CreateNecessaryAxes( bool bPrimary )
    {
      if( bPrimary )
      {
        if( IsCategoryAxisAvail && m_primaryParentAxis.CategoryAxis == null )
        {
          m_primaryParentAxis.CategoryAxis = new ChartCategoryAxisImpl( Application, m_primaryParentAxis, ExcelAxisType.Category );
        }

        if( IsValueAxisAvail && m_primaryParentAxis.ValueAxis == null )
        {
          m_primaryParentAxis.ValueAxis = new ChartValueAxisImpl( Application, m_primaryParentAxis, ExcelAxisType.Value );
        }

        if( IsSeriesAxisAvail && m_primaryParentAxis.SeriesAxis == null )
        {
          m_primaryParentAxis.SeriesAxis = new ChartSeriesAxisImpl( Application, m_primaryParentAxis, ExcelAxisType.Serie );
        }
      }
      else if( m_secondaryParentAxis.CategoryAxis == null )
      {
        m_secondaryParentAxis.CategoryAxis = new ChartCategoryAxisImpl( Application, m_secondaryParentAxis, ExcelAxisType.Category, false );
        m_secondaryParentAxis.ValueAxis = new ChartValueAxisImpl( Application, m_secondaryParentAxis, ExcelAxisType.Value, false );
      }
    }
    /// <summary>
    /// Initializes all internal collections.
    /// </summary>
    protected override void InitializeCollections()
    {
      base.InitializeCollections();

      m_arrFonts = new List<ChartFbiRecord>();
      m_series = new ChartSeriesCollection( Application, this );
      m_pageSetup = new ChartPageSetupImpl( Application, this );
      //m_dataTable = new ChartDataTableImpl( Application, this );
      m_categories = new ChartCategoryCollection(Application, this);
      m_primaryParentAxis = new ChartParentAxisImpl( Application, this );
      m_secondaryParentAxis = new ChartParentAxisImpl( Application, this, false );
      m_primaryParentAxis.CreatePrimaryFormats();
      m_secondaryParentAxis.UpdateSecondaryAxis( false );

//      ChartFormatImpl format = new ChartFormatImpl( Application, m_primaryParentAxis.ChartFormats );
//      m_primaryParentAxis.ChartFormats.Add( format );

      InitializeDefaultText();

      //      ChartFbiRecord chartFbi = (ChartFbiRecord)
      //        BiffRecordFactory.GetRecord( TBIFFRecord.ChartFbi );
      //      chartFbi.BasisWidth         = 14505;
      //      chartFbi.BasisHeight        = 8850;
      //      chartFbi.AppliedFontHeight  = 200;
      //      chartFbi.ScaleBasis         = 0;
      //      chartFbi.FontIndex          = 0;
      //
      //      m_arrFonts.Add( chartFbi );
    }
    /// <summary>
    /// Checks whether chart support data table.
    /// </summary>
    private void CheckSupportDataTable()
    {
      if( ChartType != ExcelChartType.Combination_Chart )
      {
        string strStartType = ChartFormatImpl.GetStartSerieType( ChartType );
        CheckDataTablePossibility( strStartType, true );
      }
      else
      {
        for( int i = 0, len = Series.Count; i < len; i++ )
        {
          ChartSerieImpl series = Series[ i ] as ChartSerieImpl;
          string serieStartType = ChartFormatImpl.GetStartSerieType( series.SerieType );
          CheckDataTablePossibility( serieStartType, true );
        }
      }
    }
    /// <summary>
    /// Checks whether data table is compatible with specified start type.
    /// </summary>
    /// <param name="startType"></param>
    public static bool CheckDataTablePossibility( string startType, bool bThrowException )
    {
      bool bResult = ( Array.IndexOf( DEF_SUPPORT_DATA_TABLE, startType ) != -1 );

      if( !bResult && bThrowException )
        throw new NotSupportedException( "Data table does not suported in this chart type" );

      return bResult;
    }
    /// <summary>
    /// Initializes collection of default text objects.
    /// </summary>
    private void InitializeDefaultText()
    {
      #region First default text

      List<BiffRecordRaw> records = new List<BiffRecordRaw>();

      ChartTextRecord chartText = (ChartTextRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartText );
      chartText.IsAutoText = true;
      chartText.IsGenerated = true;
      chartText.HorzAlign = ExcelChartHorzAlignment.Center;
      chartText.VertAlign = ExcelChartVertAlignment.Center;
      //      chartText.XPos = 4294966830;
      //      chartText.YPos = 4294966846;
      //      chartText.ColorIndex = 77;

      records.Add( chartText );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
      
      ChartPosRecord chartPos = (ChartPosRecord) BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
      chartPos.TopLeft = 2;
      chartPos.BottomRight = 2;
      records.Add( chartPos );

      ChartFontxRecord fontX = (ChartFontxRecord) BiffRecordFactory.GetRecord( TBIFFRecord.ChartFontx );
      //      fontX.FontIndex = 6;
      records.Add( fontX );
      
      ChartAIRecord chartAi = ( ChartAIRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
      chartAi.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;

      records.Add( chartAi );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );

      m_lstDefaultText.Add( ( int )ChartDefaultTextRecord.TextDefaults.All, records );
      #endregion

      #region Second default text
      records = new List<BiffRecordRaw>();//.Clear();
      records.Add( ( BiffRecordRaw )chartText.Clone() );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      records.Add( ( BiffRecordRaw )chartPos.Clone() );
      
      fontX = ( ChartFontxRecord )fontX.Clone();
      //      fontX.FontIndex = 7;
      records.Add( fontX );
      records.Add( ( BiffRecordRaw )chartAi.Clone() );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
      m_lstDefaultText.Add( 3, records );
      #endregion
    }
    /// <summary>
    /// This method is called if DataRange was changed.
    /// </summary>
    /// <param name="type">Represents chart type.</param>
    private void OnDataRangeChanged( ExcelChartType type )
    {
      if( m_dataRange == null )
      {
        m_series.Clear();
        return;
      }

      IRange serieValue;
      IRange serieNameRange = GetSerieOrAxisRange( m_dataRange, m_bSeriesInRows, out serieValue );
      IRange axisRange = GetSerieOrAxisRange( serieValue, !m_bSeriesInRows, out serieValue );

      if( !ValidateSerieRangeForChartType( serieValue, type ) )
        throw new ApplicationException( "Cann't set data range." );

      PrimaryCategoryAxis.CategoryLabels = axisRange;
      int iIndex = 0;

      if( serieNameRange != null && axisRange != null )
      {
        iIndex = ( m_bSeriesInRows )
          ? axisRange.LastRow - axisRange.Row + 1
          : axisRange.LastColumn - axisRange.Column + 1;
      }

      UpdateSeriesByDataRange( serieValue, serieNameRange, ChartFormatImpl.GetStartSerieType( type ), iIndex );
//      if( m_bSeriesInRows )
//      {
//        if( IsChartBubble ) AddBubbleRowSerie();
//        else if( IsChartScatter ) AddScatterRowSerie();
//        else if( ChartType == ExcelChartType.Stock_HighLowClose ) AddStockHLCRowSerie( 3 );
//        else if( ChartType == ExcelChartType.Stock_OpenHighLowClose ) AddStockHLCRowSerie( 4 );
//        else if( ChartType == ExcelChartType.Stock_VolumeHighLowClose ) AddStockVolumeRowSerie( 4 );
//        else if( ChartType == ExcelChartType.Stock_VolumeOpenHighLowClose ) AddStockVolumeRowSerie( 5 );
//        else AddDefaultRowSerie();
//      }
//      else
//      {
//        if( IsChartBubble )AddBubbleColumnSerie();
//        else if( IsChartScatter ) AddScatterColumnSerie();
//        else if (ChartType == ExcelChartType.Stock_HighLowClose ) AddStockHLCColumnSerie( 3 );
//        else if (ChartType == ExcelChartType.Stock_OpenHighLowClose ) AddStockHLCColumnSerie( 4 );
//        else if (ChartType == ExcelChartType.Stock_VolumeHighLowClose ) AddStockVolumeColumnSerie( 4 );
//        else if (ChartType == ExcelChartType.Stock_VolumeOpenHighLowClose ) AddStockVolumeColumnSerie( 5 );
//        else AddDefaultColumnSerie();
//      }
    }
    /// <summary>
    /// Adds default row series to the series collection.
    /// </summary>
    private void AddDefaultRowSerie()
    {
      for( int i = m_dataRange.Row, iLastRow = m_dataRange.LastRow; i <= iLastRow; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
        serie.Values = m_dataRange[ i, m_dataRange.Column, i,
          m_dataRange.LastColumn ];
        serie.ValueRangeChanged += new ValueChangedEventHandler(serie_ValueRangeChanged);
      }
    }
    /// <summary>
    /// Adds default column series to the series collection.
    /// </summary>
    private void AddDefaultColumnSerie()
    {
      int iFirstRow = m_dataRange.Row;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      for( int i = m_dataRange.Column; i <= iLastColumn; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
        serie.Values = m_dataRange[ iFirstRow, i, iLastRow, i ];
        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }
    }
    /// <summary>
    /// Adds bubble row series to the series collection.
    /// </summary>
    private void AddBubbleRowSerie()
    {
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;
      int iFirstColumn = m_dataRange.Column;

      for( int i = m_dataRange.Row; i <= iLastRow; i+=2 )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
        serie.Bubbles = m_dataRange[ i, iFirstColumn, i, iLastColumn ];

        serie.Values = m_dataRange[ i + 1, iFirstColumn, i + 1, iLastColumn ];

        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }
    }
    /// <summary>
    /// Adds bubble column series to the series collection.
    /// </summary>
    private void AddBubbleColumnSerie()
    {
      int iFirstRow = m_dataRange.Row;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      for( int i = m_dataRange.Column; i <= iLastColumn; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
            
        serie.Bubbles = m_dataRange[ iFirstRow, i, iLastRow, i ];
        serie.Values = m_dataRange[ iFirstRow, i + 1, iLastRow, i + 1 ];
        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }
    }
    /// <summary>
    /// Adds scatter row series to the series collection.
    /// </summary>
    private void AddScatterRowSerie()
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      for( int i = iFirstRow + 1; i <= iLastRow; i+=2 )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
        serie.CategoryLabels = m_dataRange[ iFirstRow, iFirstColumn,
          iFirstRow, iLastColumn ];

        serie.Values = m_dataRange[ i, iFirstColumn, i, iLastColumn ];

        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }
    }
    /// <summary>
    /// Adds scatter column series to the series collection.
    /// </summary>
    private void AddScatterColumnSerie()
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      for( int i = iFirstColumn + 1; i <= iLastColumn; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )Series.Add();
        serie.CategoryLabels = m_dataRange[ iFirstRow, iFirstColumn,
          iLastRow, iFirstColumn ];
            
        serie.Values = m_dataRange[ iFirstRow, i, iLastRow, i ];
        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }
    }
    /// <summary>
    /// Adds stock row series without volume to the series collection.
    /// </summary>
    /// <param name="count">Number of series to add.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data range rows count does not correspond to the specified count.
    /// </exception>
    private void AddStockHLCRowSerie( int count )
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      if( iLastRow - iFirstRow != count - 1 )
      {
        throw new ArgumentOutOfRangeException
          ( "There should be " + count.ToString() + " rows for this chart type." );
      }

      ChartSerieImpl serie;

      for( int i = 0; i < count; i++ )
      {
        serie = ( ChartSerieImpl )Series.Add();
        serie.Values = m_dataRange[ iFirstRow + i, iFirstColumn, iLastRow + i,
          iFirstColumn ];

        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }

      serie = ( ChartSerieImpl )Series[ count - 1 ];
      SetStockSerieFormat( serie );
    }
    /// <summary>
    /// Adds stock column series without volume to the series collection.
    /// </summary>
    /// <param name="count">Number of series to add.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data range columns count does not correspond to the specified count.
    /// </exception>
    private void AddStockHLCColumnSerie( int count )
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      if( iLastColumn - iFirstColumn != count - 1 )
      {
        throw new ArgumentOutOfRangeException
          ( "There should be " + (count) + " columns for this chart type." );
      }

      ChartSerieImpl serie;

      for( int i = 0; i < count; i++ )
      {
        serie = ( ChartSerieImpl )Series.Add();
        serie.Values = m_dataRange[ iFirstRow, iFirstColumn + i,
          iLastRow, iFirstColumn + i ];

        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }

      serie = ( ChartSerieImpl )Series[ count - 1 ];
      SetStockSerieFormat( serie );
    }
    /// <summary>
    /// Adds stock row series with volume to the series collection.
    /// </summary>
    /// <param name="count">Number of series to add.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data range rows count does not correspond to the specified count.
    /// </exception>
    private void AddStockVolumeRowSerie( int count )
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      if( iLastRow - iFirstRow != count - 1 )
      {
        throw new ArgumentOutOfRangeException
          ( "There should be " + count.ToString() + " rows for this chart type." );
      }

      ChartSerieImpl serie = ( ChartSerieImpl )m_series.Add();
            
      serie.Values = m_dataRange[ iFirstRow, iFirstColumn, iLastRow, iFirstColumn ];
      serie.Number = count - 1;
      serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );

      for( int i = 1; i < count; i++ )
      {
        serie = ( ChartSerieImpl )m_series.Add();
            
        serie.Values = m_dataRange[ iFirstRow + i, iFirstColumn,
          iLastRow + i, iFirstColumn ];

        serie.Number = i - 1;
        serie.ChartGroup = 1;
        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }

      if( count == 4 ) SetStockSerieFormat( ( ChartSerieImpl )m_series[ 3 ] );
      SetVolumeSecondaryAxisFormat();
    }
    /// <summary>
    /// Adds stock column series with volume to the series collection.
    /// </summary>
    /// <param name="count">Number of series to add.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data range columns count does not correspond to the specified count.
    /// </exception>
    private void AddStockVolumeColumnSerie( int count )
    {
      int iFirstRow = m_dataRange.Row;
      int iFirstColumn = m_dataRange.Column;
      int iLastRow = m_dataRange.LastRow;
      int iLastColumn = m_dataRange.LastColumn;

      if( iLastColumn - iFirstColumn != count - 1 )
      {
        throw new ArgumentOutOfRangeException
          ( "There should be " + (count) + " columns for this chart type." );
      }

      ChartSerieImpl serie = ( ChartSerieImpl )m_series.Add();
            
      serie.Values = m_dataRange[ iFirstRow, iFirstColumn, iLastRow, iFirstColumn ];
      serie.Number = count - 1;
      serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );

      for( int i = 1; i < count; i++ )
      {
        serie = ( ChartSerieImpl )m_series.Add();
            
        serie.Values = m_dataRange[ iFirstRow, iFirstColumn + i, iLastRow,
          iFirstColumn + i ];
        serie.Number = i - 1;
        serie.ChartGroup = 1;
        serie.ValueRangeChanged += new ValueChangedEventHandler( serie_ValueRangeChanged );
      }

      if( count == 4 ) SetStockSerieFormat( ( ChartSerieImpl )m_series[ 3 ] );
      SetVolumeSecondaryAxisFormat();
    }
    /// <summary>
    /// Sets format of secondary axes for volume chart type.
    /// </summary>
    private void SetVolumeSecondaryAxisFormat()
    {
      SecondaryCategoryAxis.IsMaxCross = true;
      ChartCategoryAxisImpl categoryAxis = ( ChartCategoryAxisImpl )SecondaryCategoryAxis;

      m_chartProperties.IsManSerAlloc = true;
    }
    /// <summary>
    /// Sets default format for the first stock Series.
    /// </summary>
    /// <param name="serie">Series to set format.</param>
    private void SetStockSerieFormat( ChartSerieImpl serie )
    {
      if( serie == null )
        throw new ArgumentNullException( "serie" );

      ChartDataPointImpl dataPoint = ( ChartDataPointImpl )serie.DataPoints.DefaultDataPoint;
      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )dataPoint.DataFormat;

      IChartBorder lineFormat = dataFormat.LineProperties;

      lineFormat.LinePattern = ExcelChartLinePattern.None;
      lineFormat.LineWeight = ExcelChartLineWeight.Hairline;
      
      dataFormat.PieFormat.Percent = 0;

      ChartMarkerFormatRecord markerFormat = dataFormat.MarkerFormat;
      markerFormat.MarkerType = ExcelChartMarkerType.DowJones;
      markerFormat.LineSize = 60;
    }
    /// <summary>
    /// This method is called when IsSeriesInRows is changed.
    /// </summary>
    private void OnSeriesInRowsChanged()
    {
      if( m_dataRange != null )
      {
        OnDataRangeChanged( ChartType );
      }
    }
    /// <summary>
    /// Updates series in bubble chart.
    /// </summary>
    private void UpdateSeriesInBubleChart()
    {
      ChartSeriesCollection seriesColl = m_series;
      m_series = new ChartSeriesCollection( Application, m_series.Parent );

      for( int i = 0, iLen = seriesColl.Count; i < iLen; i++ )
      {
        ChartSerieImpl serieToUpdate = ( ChartSerieImpl )m_series.Add();
        ChartSerieImpl serie = ( ChartSerieImpl )seriesColl[ i ];

        serieToUpdate.Values = serie.Values;
        IRange bubbles = serie.Bubbles;

        if( bubbles != null )
        {
          ChartSerieImpl serieToAdd = new ChartSerieImpl( Application, m_series );
          serieToAdd.Values = bubbles;
          m_series.Add( serieToAdd );
        }
      }
    }
    /// <summary>
    /// This method is called when chart type is changed.
    /// </summary>
    /// <param name="type">Represents chart type.</param>
    /// <param name="isSeriesCreation">Value indicating whether series needs to be created.</param>
    private void OnChartTypeChanged( ExcelChartType type, bool isSeriesCreation )
    {
      if( type == ExcelChartType.Combination_Chart )
          throw new ArgumentException( "Cannot change chart type." );

      HasDataTable = false;
      m_series.ClearErrorBarsAndTrends();

      if( ChartStartType == START_BUBBLE )
        UpdateSeriesInBubleChart();

      m_primaryParentAxis.Formats.Clear();
      m_series.ClearSeriesForChangeChartType();

      if( Array.IndexOf( CHARTS_STOCK, type ) != -1 )
      {
        ChangeChartStockType( type );

        return;
      }

      ChartFormatImpl format = new ChartFormatImpl( Application, PrimaryFormats );
      PrimaryFormats.Add( format, false );

      format.ChangeChartType( type, isSeriesCreation );
      UpdateChartMembersOnTypeChanging( type );
    }
    /// <summary>
    /// Updates tick record in surface chart type.
    /// </summary>
    private void UpdateSurfaceTickRecord()
    {
      ( ( ChartAxisImpl )PrimaryCategoryAxis ).UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_Low );
      ( ( ChartAxisImpl )PrimaryValueAxis ).UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_None );
      ( ( ChartAxisImpl )PrimarySerieAxis ).UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_Low );
    }
    /// <summary>
    /// Updates tick record in radar chart type.
    /// </summary>
    private void UpdateRadarTickRecord()
    {
      ( ( ChartAxisImpl )PrimaryCategoryAxis ).UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_NextToAxis );
      ( ( ChartAxisImpl )PrimaryValueAxis ).UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_NextToAxis );
    }
    /// <summary>
    /// Updates chart members on type changing.
    /// </summary>
    /// <param name="type">Represents new chart type.</param>
    private void UpdateChartMembersOnTypeChanging( ExcelChartType type )
    {
      if( !m_book.Loading )
      {
          if (!m_bHasLegend)
          {
              m_bHasLegend = true;
              m_legend = new ChartLegendImpl(Application, this);
          }
        m_primaryParentAxis.ClearGridLines();

        SetToDefaultGridlines( type );
        m_sidewall = new ChartWallOrFloorImpl( Application, this, true );        
        m_walls = new ChartWallOrFloorImpl( Application, this, true );
        m_floor = new ChartWallOrFloorImpl( Application, this, false );

        m_plotArea = new ChartPlotAreaImpl( Application, this, type );

        if( Array.IndexOf( CHARTS3D, ChartType ) != -1 )
        {
          ChartAxisImpl parentAxis = ( ( ChartAxisImpl )PrimaryCategoryAxis );
          parentAxis.UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_Low );
        }

        bool bFlag = Array.IndexOf( ChartImpl.DEF_SUPPORT_SERIES_AXIS, type ) != -1;

        if( bFlag )
        {
          ChartAxisImpl seriesAxis = m_primaryParentAxis.SeriesAxis;

          if( seriesAxis == null )
          {
            seriesAxis = m_primaryParentAxis.SeriesAxis = new ChartSeriesAxisImpl( Application
              , m_primaryParentAxis, ExcelAxisType.Serie );
          }

          seriesAxis.UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_Low );
        }
        else
        {
          m_primaryParentAxis.SeriesAxis = null;
        }

        if( type == ExcelChartType.Surface_Contour || type == ExcelChartType.Surface_NoColor_Contour )
        {
          UpdateSurfaceTickRecord();

          if( type == ExcelChartType.Surface_NoColor_Contour )
          {
            m_sidewall.Interior.Pattern = ExcelPattern.None;            
            m_walls.Interior.Pattern = ExcelPattern.None;
            m_floor.Interior.Pattern = ExcelPattern.None;
          }
        }
        else if( type == ExcelChartType.Radar || type == ExcelChartType.Radar_Filled || type == ExcelChartType.Radar_Markers )
        {
          UpdateRadarTickRecord();
        }
        else
        {
          ChartAxisImpl parentAxis = ( ( ChartAxisImpl )PrimaryValueAxis );
          parentAxis.UpdateTickRecord( ExcelTickLabelPosition.TickLabelPosition_Low );
        }
      }
    }
    /// <summary>
    /// Changes Chart type for one of stock types.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeChartStockType( ExcelChartType type )
    {
      int iSerieCount = m_series.Count;
      ChartFormatImpl format;

      switch( type )
      {
        case ExcelChartType.Stock_HighLowClose:
          if( iSerieCount != 3 )
            throw new ArgumentException( "Cannot change serie type." );

          format = new ChartFormatImpl( Application, PrimaryFormats );
          PrimaryFormats.Add( format );

          format.ChangeChartStockHigh_Low_CloseType();
          break;

        case ExcelChartType.Stock_OpenHighLowClose:
          if( iSerieCount != 4 )
            throw new ArgumentException( "Cannot change serie type." );

          format = new ChartFormatImpl( Application, PrimaryFormats );
          PrimaryFormats.Add( format );

          format.ChangeChartStockOpen_High_Low_CloseType();
          break;

        case ExcelChartType.Stock_VolumeHighLowClose:
          if( iSerieCount != 4 )
            throw new ArgumentException( "Cannot change serie type." );

          format = new ChartFormatImpl( Application, PrimaryFormats );
          PrimaryFormats.Add( format );
          format.ChangeChartStockVolume_High_Low_CloseTypeFirst();

          format = new ChartFormatImpl( Application, SecondaryFormats );
          format.DrawingZOrder = 1;
          SecondaryFormats.Add( format );
          format.ChangeChartStockVolume_High_Low_CloseTypeSecond();
          break;

        case ExcelChartType.Stock_VolumeOpenHighLowClose:
          if( iSerieCount != 5 )
            throw new ArgumentException( "Cannot change serie type." );

          IsManuallyFormatted = true;

          m_secondaryParentAxis.UpdateSecondaryAxis( true );
          format = new ChartFormatImpl( Application, PrimaryFormats );
          PrimaryFormats.Add( format );
          format.ChangeChartStockVolume_High_Low_CloseTypeFirst();

          format = new ChartFormatImpl( Application, SecondaryFormats );
          format.DrawingZOrder = 1;
          SecondaryFormats.Add( format );
          format.ChangeChartStockVolume_Open_High_Low_CloseType();

          //m_series[ 0 ].UsePrimaryAxis = false;
          SecondaryCategoryAxis.IsMaxCross = true;
          break;

        default:
          throw new ArgumentException( "type" );
      }
    }
    /// <summary>
    /// Returns Chart3D record for this chart.
    /// </summary>
    /// <returns>Chart3D record for this chart.</returns>
    private Chart3DRecord GetChart3D()
    {
      Chart3DRecord result = (Chart3DRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );

      result.IsPerspective = IsPerspective;
      result.IsClustered = IsClustered;
      switch( ChartType )
      {
        case ExcelChartType.Surface_Contour:
        case ExcelChartType.Surface_NoColor_Contour:
          result.RotationAngle = 0;
          result.ElevationAngle = 90;
          result.DistanceFromEye = 0;
          break;

        case ExcelChartType.Pie_3D:
        case ExcelChartType.Pie_Exploded_3D:
          result.RotationAngle = 0;
          result.IsAutoScaled = false;
          result.Is2DWalls = false;
          // result.Reserved = false;
          break;
      }

      return result;
    }
    /// <summary>
    /// This is event handler for ValueRangeChanged of each series of the chart.
    /// </summary>
    private void serie_ValueRangeChanged( object sender, ValueChangedEventArgs e)
    {
      m_dataRange = null;
    }
    /// <summary>
    /// Initializes chart frames.
    /// </summary>
    private void InitializeFrames()
    {
      m_chartArea = new ChartFrameFormatImpl( Application, this );
      m_chartArea.Interior.ForegroundColorIndex = ExcelKnownColors.WhiteCustom;
      m_plotAreaFrame = new ChartFrameFormatImpl( Application, this, true, false, true );
    }
    /// <summary>
    /// Removes chart format.
    /// </summary>
    /// <param name="formatToRemove">Format to remove.</param>
    public void RemoveFormat( IChartFormat formatToRemove )
    {
      if( formatToRemove == null )
        throw new ArgumentNullException( "formatToRemove" );

      m_primaryParentAxis.Formats.Remove( ( ChartFormatImpl )formatToRemove );
    }
    /// <summary>
    /// Updates chart title on parsing and creating chart.
    /// </summary>
    public void UpdateChartTitle()
    {
      if( m_title == null )
        return;

      ChartTextAreaImpl titleArea = ChartTitleArea as ChartTextAreaImpl;

      if( ChartTitle == null && titleArea.TextRecord.IsAutoMode )
      {
        int iSerCount = Series.Count;

        if( iSerCount > 0 )
        {
          ChartSerieImpl serie = ( ChartSerieImpl )Series[ 0 ];

          if( !serie.IsDefaultName )
          {
            string strType = ( iSerCount != 1 ) ?
              GetChartTypeStart() : //ChartFormatImpl.GetStartSerieType( ChartType ) :
              null;

            if( iSerCount == 1 || strType == START_PIE || strType == START_DOUGHNUT )
            {
              string strSerieName = serie.ParseSerieNotDefaultText;
              ChartTitle = strSerieName;
            }
          }
        }
      }
    }
    /// <summary>
    /// Detects start type of the chart.
    /// </summary>
    /// <returns>Returns null if cann't detect.</returns>
    private string GetChartTypeStart()
    {
      if( m_series.Count == 0 )
        return null;

      string result = ( m_series[ 0 ] as ChartSerieImpl ).DetectSerieTypeStart();

      for( int i = 1, len = m_series.Count; i < len; i++ )
      {
        ChartSerieImpl serie = m_series[ i ] as ChartSerieImpl;

        if( serie.DetectSerieTypeStart() != result )
        {
          result = null;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Detects data range by series and category axis.
    /// </summary>
    /// <returns>Returns detected data range or null if cann't detect.</returns>
    internal IRange DetectDataRange()
    {
      if( m_series.Count == 0 )
        return null;

      ChartSerieImpl serie = ( ChartSerieImpl )Series[ 0 ];
      IRange firstRange = serie.Values;
      IRange nameRange = serie.GetSerieNameRange();
      IRange bubles = serie.Bubbles;
      
      

      if( firstRange == null || firstRange.Worksheet==null)
        return null;
      IWorksheet sheet = firstRange.Worksheet;
      string strSheetName = sheet.Name;      
      IRange valueRange = GetSeriesValuesRange( firstRange, bubles, sheet, strSheetName );
      IRange serieNameRange;
      IRange categoryRange = PrimaryCategoryAxis.CategoryLabels;


      if (categoryRange != null && categoryRange.Worksheet!=null && strSheetName != categoryRange.Worksheet.Name)
        return null;

      if( valueRange == null )
        return null;

      if( !GetSerieNameValuesRange( nameRange, bubles, sheet, strSheetName, out serieNameRange ) )
        return null;

      Rectangle result = RangeImpl.GetRectangeOfRange( valueRange, true );

      if( serieNameRange != null )
      {
        if( !GetDataRangeRec( serieNameRange, ref result, !m_bSeriesInRows ) )
          return null;
      }

      if( categoryRange != null )
      {
        if( !GetDataRangeRec( categoryRange, ref result, m_bSeriesInRows ) )
          return null;
      }

      return sheet[ result.Top, result.Left, result.Bottom, result.Right ];
    }
    /// <summary>
    /// Detects if series in rows or in column. 
    /// </summary>
    /// <param name="range">Represents data range.</param>
    /// <returns>Returns true if in rows; otherwise in column.</returns>
    private bool DetectIsInRow( IRange range )
    {
      if( range == null )
        return true;

      int iRowCount = range.LastRow - range.Row;
      int iColCount = range.LastColumn - range.Column;

      return iRowCount <= iColCount;
    }
    /// <summary>
    /// Gets range, that represents series name or category axis by data range.
    /// </summary>
    /// <param name="range">Represents data range.</param>
    /// <param name="bIsInRow">Represents if series in row.</param>
    /// <param name="serieRange">Represents range, that contain series value range.</param>
    /// <returns>Returns series name or category axis range, if can; otherwise null.</returns>
    public IRange GetSerieOrAxisRange( IRange range, bool bIsInRow, out IRange serieRange )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      int iFirstLen = bIsInRow ? range.Row : range.Column;
      int iRowColumn = bIsInRow ? range.LastRow : range.LastColumn;

      int iFirsCount = bIsInRow ? range.Column : range.Row;
      int iLastCount = bIsInRow ? range.LastColumn : range.LastRow;

      int iIndex = -1;

      bool bIsName = false;

      for( int i = iFirsCount; i < iLastCount && !bIsName; i++ )
      {
        IRange curRange = bIsInRow ? range[ iRowColumn, i ] : range[ i, iRowColumn ];

        bIsName = curRange.HasNumber || curRange.IsBlank || curRange.HasFormula;

        if( !bIsName )
          iIndex = i;
      }

      if( iIndex == -1 )
      {
        serieRange = range;
        return null;
      }

      IRange result = ( bIsInRow )
        ? range[ iFirstLen, iFirsCount, iRowColumn, iIndex ]
        : range[ iFirsCount, iFirstLen, iIndex, iRowColumn ];

      serieRange = ( bIsInRow )
        ? range[ range.Row, result.LastColumn + 1, range.LastRow, range.LastColumn ]
        : range[ result.LastRow + 1, range.Column, range.LastRow, range.LastColumn ];

      return result;
    }
    /// <summary>
    /// Validates Series range for min Series count of custom chart type.
    /// </summary>
    /// <param name="serieValue">Represents range, that contain Series values.</param>
    /// <param name="type">Represents chart type.</param>
    /// <returns>Returns true if can set data range, otherwise false.</returns>
    private bool ValidateSerieRangeForChartType( IRange serieValue, ExcelChartType type )
    {
      if( serieValue == null )
        throw new ArgumentNullException( "serieValue" );

      string strType = ChartFormatImpl.GetStartSerieType( type );

      int iSeriesInRangeCount = ( m_bSeriesInRows )
        ? serieValue.LastRow - serieValue.Row + 1
        : serieValue.LastColumn - serieValue.Column + 1;

      if( iSeriesInRangeCount < 2 && ( strType == START_BUBBLE || strType == START_SURFACE ) )
        return false;

      if( type == ExcelChartType.Stock_HighLowClose && iSeriesInRangeCount != 3 )
        return false;

      if( type == ExcelChartType.Stock_OpenHighLowClose || type == ExcelChartType.Stock_VolumeHighLowClose
        && iSeriesInRangeCount != 4 )
      {
        return false;
      }

      if( type == ExcelChartType.Stock_VolumeOpenHighLowClose && iSeriesInRangeCount != 5 )
        return false;

      if( strType == START_BUBBLE )
        iSeriesInRangeCount = iSeriesInRangeCount / 2 + iSeriesInRangeCount % 2;

      int iSeriesCount = m_series.Count;
      bool bRemove = iSeriesCount > iSeriesInRangeCount;
      int iStart = bRemove ? iSeriesInRangeCount : iSeriesCount;
      int iLen = bRemove ? iSeriesCount : iSeriesInRangeCount;

      for( int i = iStart; i < iLen; i++ )
      {
        if( bRemove )
        {
          m_series.RemoveAt( iLen - i + iStart - 1 );
        }
        else
        {
          m_series.Add();
        }
      }

      return true;
    }
    /// <summary>
    /// Updates series value by data range.
    /// </summary>
    /// <param name="serieValue">Represents range, that contain series value.</param>
    /// <param name="serieNameRange">Represents range, that contain series name.</param>
    /// <param name="strType">Represents chart type.</param>
    /// <param name="iIndex">Represents index.</param>
    private void UpdateSeriesByDataRange( IRange serieValue, IRange serieNameRange, string strType, int iIndex )
    {
      for( int i = 0, iLen = m_series.Count; i < iLen; i++ )
      {
        IRange value = ( m_bSeriesInRows )
          ? serieValue[ serieValue.Row + i, serieValue.Column, serieValue.Row + i, serieValue.LastColumn ]
          : serieValue[ serieValue.Row, serieValue.Column + i, serieValue.LastRow, serieValue.Column + i ];

        if( strType == START_BUBBLE && i % 2 == 1 )
        {
          IChartSerie bubbleSerie = Series[ i - 1 ];
          bubbleSerie.Bubbles = value;

          continue;
        }

        ChartSerieImpl serie = ( ChartSerieImpl )Series[ i ];
        serie.Bubbles = null;
        serie.SetDefaultName( m_series.GetDefSerieName( i ) );

        int iAddIndex = iIndex;
        serie.Values = value;

        if( serieNameRange != null )
        {
          iAddIndex += ( m_bSeriesInRows ) ? serieNameRange.Row : serieNameRange.Column;

          string formula = ( m_bSeriesInRows )
            ? serieValue[ iAddIndex + i, serieNameRange.Column, iAddIndex + i, serieNameRange.LastColumn ].AddressGlobal
            : serieValue[ serieNameRange.Row, iAddIndex + i, serieNameRange.LastRow, iAddIndex + i ].AddressGlobal;

          serie.Name = "=" + formula;
        }
      }
    }
    /// <summary>
    /// Indicates if Series is in data range.
    /// </summary>
    /// <param name="rec">Represents rec of first Series values.</param>
    /// <param name="range">Represents range of current Series values.</param>
    /// <param name="i">index of this Series in collection.</param>
    /// <param name="strSheetName">Represents sheet name.</param>
    /// <returns>Returns true if series values is equal.</returns>
    private bool CompareSeriesValues( Rectangle rec, IRange range, int i, string strSheetName )
    {
      if( strSheetName == null || strSheetName.Length == 0 )
        throw new ArgumentNullException( "strSheetName" );

      if( range == null )
        return false;

      if (range.Worksheet != null && range.Worksheet.Name != strSheetName)
        return false;

      bool flag = ( m_bSeriesInRows )
        ? range.Row == rec.Top + i && range.LastRow == rec.Bottom + i && range.Column == rec.Left && range.LastColumn == rec.Right
        : range.Row == rec.Top && range.LastRow == rec.Bottom && range.Column == rec.Left + i && range.LastColumn == rec.Right + i;

      return flag;
    }
    /// <summary>
    /// Gets range, that represents series value range.
    /// </summary>
    /// <param name="lastRange">Represents last range.</param>
    /// <param name="buble">Represents bubble range.</param>
    /// <param name="sheet">Represents parent sheet.</param>
    /// <param name="strSheetName">Represents name of worksheet in data range.</param>
    /// <returns>Returns detected series range.</returns>
    internal IRange GetSeriesValuesRange( IRange lastRange, IRange buble, IWorksheet sheet, string strSheetName )
    {
      if( lastRange == null )
        throw new ArgumentNullException( "lastRange" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      Rectangle rec = RangeImpl.GetRectangeOfRange( lastRange, true );
      bool bBubles = ChartStartType == START_BUBBLE;
      int iLen = Series.Count;
      int iIndex = 0;

      if( bBubles && buble != null )
      {
        if( !CompareSeriesValues( rec, buble, 1, strSheetName ) )
          return null;

        if( iLen == 1 )
          iIndex++;
      }

      for( int i = 1; i < iLen; i++ )
      {
        ChartSerieImpl nextSerie = ( ChartSerieImpl )Series[ i ];
        lastRange = nextSerie.Values;

        if( !CompareSeriesValues( rec, lastRange, ( bBubles ) ? i * 2 : i, strSheetName ) )
          return null;

        if( bBubles )
        {
          buble = nextSerie.Bubbles;

          if( iLen - i > 1 && !CompareSeriesValues( rec, lastRange, i * 2 + i, strSheetName ) )
            return null;

          if( iLen == i && buble != null )
          {
            if( !CompareSeriesValues( rec, lastRange, i * 2 + i, strSheetName ) )
              return null;

            iIndex++;
          }
        }
      }

      return ( m_bSeriesInRows )
        ? sheet[ rec.Top, rec.Left, lastRange.LastRow + iIndex, lastRange.LastColumn ]
        : sheet[ rec.Top, rec.Left, lastRange.LastRow, lastRange.LastColumn + iIndex ];
    }
    /// <summary>
    /// Gets range, that represents series name values.
    /// </summary>
    /// <param name="lastRange">Represents first Series name range.</param>
    /// <param name="bubles">Represents bubble range.</param>
    /// <param name="sheet">Represents parent sheet.</param>
    /// <param name="strSheetName">Represents sheet name.</param>
    /// <param name="result">Sets out parameter to Series name range.</param>
    /// <returns>Value indicating range representing serie name values.</returns>
    private bool GetSerieNameValuesRange( IRange lastRange, IRange bubles, IWorksheet sheet, string strSheetName, out IRange result )
    {
      bool bIsNull = lastRange == null;
      Rectangle rec = new Rectangle( 0, 0, 0, 0 );
      bool bBubles = ChartStartType == START_BUBBLE;
      result = null;
      int iLen = Series.Count;
      int iBubleIndex = 0;

      if( bBubles && bubles != null && iLen == 1 )
        iBubleIndex++;

      if( !bIsNull )
        rec = RangeImpl.GetRectangeOfRange( lastRange, true );

      for( int i = 1; i < iLen; i++ )
      {
        ChartSerieImpl nextSerie = ( ChartSerieImpl )Series[ i ];
        lastRange = nextSerie.GetSerieNameRange();

        if( bIsNull != ( lastRange == null ) )
          return false;

        if( bBubles && iLen - i > 1 && nextSerie.Bubbles != null )
          iBubleIndex++;

        if( lastRange != null )
        {
          if( !CompareSeriesValues( rec, lastRange, bBubles ? i * 2 : i, strSheetName ) )
            return false;
        }
        else
        {
          if( !nextSerie.IsDefaultName )
            return false;
        }
      }

      if( bIsNull )
        return true;

      result = ( m_bSeriesInRows )
        ? sheet[ rec.Top, rec.Left, lastRange.LastRow + iBubleIndex, lastRange.LastColumn ]
        : sheet[ rec.Top, rec.Left, lastRange.LastRow, lastRange.LastColumn + iBubleIndex ];

      return true;
    }
    /// <summary>
    /// Updates data range rectangle.
    /// </summary>
    /// <param name="range">Represents range to update.</param>
    /// <param name="rec">Represents rec, that represents current data range. Ref parameter.</param>
    /// <param name="inRow">Indicates if values is in rows.</param>
    /// <returns>Returns true if can update data range; otherwise false.</returns>
    private bool GetDataRangeRec( IRange range, ref Rectangle rec, bool inRow )
    {
      if( range == null )
        throw new ArgumentNullException( "values" );

      if( range != null )
      {
        bool bFlag = ( inRow )
          ? ( rec.Right == range.LastColumn && rec.Top == range.LastRow + 1 )
          : ( rec.Bottom == range.LastRow && rec.Left == range.Column + 1 );

        if( !bFlag )
          return false;

        if( inRow )
        {
          rec.Y = range.Row;
          rec.Height += 1;
        }
        else
        {
          rec.X = range.Column;
          rec.Width += 1;
        }
      }

      return true;
    }
    /// <summary>
    /// Detects if series in rows.
    /// </summary>
    /// <returns>Returns true if series values in row; otherwise - false.</returns>
    public void DetectIsInRowOnParsing()
    {
      int iCount = m_series.Count;

      if( iCount == 0 )
      {
        m_bSeriesInRows = false;
      }
      else
      {
        IRange serieRange = m_series[ 0 ].Values;

        if( serieRange == null )
        {
          m_bSeriesInRows = false;
        }
        else
        {
          m_bSeriesInRows = DetectIsInRow( serieRange );
        }
      }
    }
    /// <summary>
    /// Changes chart type.
    /// </summary>
    /// <param name="newChartType">Represents new chart type.</param>
    /// <param name="isSeriesCreation">Value indicating whether to create series.</param>
    internal void ChangeChartType( ExcelChartType newChartType, bool isSeriesCreation )
    {
      if( ChartType != newChartType )
      {
        TypeChanging = true;
        DestinationType = newChartType;

        OnChartTypeChanged( newChartType, isSeriesCreation );
        m_chartType = newChartType;

        TypeChanging = false;
      }
    }
    /// <summary>
    /// Prepares protection options before setting protection.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected override ExcelSheetProtection PrepareProtectionOptions( ExcelSheetProtection options )
    {
      return options |= ExcelSheetProtection.Scenarios;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns clone of current object.</returns>
    public override object Clone( object parent )
    {
      ChartImpl result = Clone( null, parent, null );

      if( !m_bInWorksheet )
      {
        result.ParentWorkbook.InnerCharts.AddInternal( result );
      }

      return result;
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="hashNewNames">Hash table with new Worksheet names.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>Returns clone of current object.</returns>
    public ChartImpl Clone( Dictionary<string, string> hashNewNames
      , object parent, Dictionary<int, int> dicFontIndexes )
    {
      ChartImpl result = ( ChartImpl )base.Clone( parent );
      result.SetParent( parent );
      result.FindParents();

      result.m_arrFonts = CloneUtils.CloneCloneable( m_arrFonts );

      if( m_arrFonts != null)
      {
        result.UpdateChartFbiIndexes( dicFontIndexes );
      }

      if( m_arrRecords != null)
      {
        result.m_arrRecords = CloneUtils.CloneCloneable( m_arrRecords );
      }

      if( m_chartProperties != null)
      {
        result.m_chartProperties = ( ChartShtpropsRecord )m_chartProperties.Clone();
      }

      if( m_plotArea != null )
        result.m_plotArea = ( ChartPlotAreaImpl )m_plotArea.Clone( result );

      if( m_dataTable != null)
      {
        result.m_dataTable = m_dataTable.Clone( result );
      }

      if( m_chartArea != null)
      {
        result.m_chartArea = m_chartArea.Clone( result );
      }

      if( m_lstDefaultText != null)
      {
        result.m_lstDefaultText = m_lstDefaultText.CloneAll();
        result.UpdateChartFontXIndexes( dicFontIndexes );
      }

      if( m_pageSetup != null )
      {
        result.m_pageSetup = m_pageSetup.Clone( result );
      }

      m_plotAreaBoundingBox = ( ChartPosRecord )CloneUtils.CloneCloneable( m_plotAreaBoundingBox );

      if( m_plotAreaFrame != null)
      {
        result.m_plotAreaFrame = m_plotAreaFrame.Clone( result );
      }

      m_plotGrowth = ( ChartPlotGrowthRecord )CloneUtils.CloneCloneable( m_plotGrowth );

      if( m_series != null)
      {
        result.m_series = m_series.Clone( result, hashNewNames, dicFontIndexes );
      }

      if( m_title != null)
      {
        result.m_title = ( ChartTextAreaImpl )m_title.Clone( result, dicFontIndexes, hashNewNames );
      }

      if( m_primaryParentAxis != null )
        result.m_primaryParentAxis = m_primaryParentAxis.Clone( result, dicFontIndexes, hashNewNames );

      if( m_secondaryParentAxis != null )
        result.m_secondaryParentAxis = m_secondaryParentAxis.Clone( result, dicFontIndexes, hashNewNames );

      if( m_legend != null )
        result.m_legend = m_legend.Clone( result, dicFontIndexes, hashNewNames );

      return result;
    }
    /// <summary>
    /// Changes primary axis from primary to secondary.
    /// </summary>
    /// <param name="isParsing">If parsing - true; otherwise false.</param>
    public void ChangePrimaryAxis( bool isParsing )
    {
      if( isParsing && ( SecondaryFormats.Count == 0 || PrimaryFormats.Count > 1
        || !PrimaryFormats.NeedSecondaryAxis ) )
      {
        return;
      }

      PrimaryParentAxis.Formats.ChangeCollections();
    }
    /// <summary>
    /// Updates Fbi Font indexes.
    /// </summary>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    public void UpdateChartFbiIndexes( IDictionary dicFontIndexes )
    {
      if( dicFontIndexes == null || m_arrFonts == null )
        return;

      for( int i = 0, iCount = m_arrFonts.Count; i < iCount; i++ )
      {
        ChartFbiRecord record = m_arrFonts[ i ];
        int index = record.FontIndex;
        int iNewIndex = ( int )dicFontIndexes[ index ];

        record.FontIndex = ( ushort ) iNewIndex;
      }
    }
    /// <summary>
    /// Updates Fontx indexes.
    /// </summary>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    public void UpdateChartFontXIndexes( IDictionary dicFontIndexes )
    {
      if( dicFontIndexes == null || m_lstDefaultText == null )
        return;

      for( int i = 0, iCount = m_lstDefaultText.Count; i < iCount; i++ )
      {
        List<BiffRecordRaw> array = m_lstDefaultText.GetByIndex( i );

        if( array == null )
          continue;

        for( int j = 0, iLen = array.Count; j < iLen; j++ )
        {
          ChartFontxRecord record = array[ j ] as ChartFontxRecord;

          if( record != null )
          {
            int index = record.FontIndex;

            if( dicFontIndexes.Contains( index ) )
            {
              int iNewIndex = ( int )dicFontIndexes[ index ];
              record.FontIndex = ( ushort )iNewIndex;
            }
          }
        }
      }
    }
    /// <summary>
    /// Chacks for existing gridlines in chart.
    /// </summary>
    /// <returns>Returns true if can exist; otherwise false.</returns>
    public bool CheckForSupportGridLine()
    {
      ExcelChartType type = ChartType;
      ChartSeriesCollection series = m_series;

      if( type == ExcelChartType.Combination_Chart )
      {
        for( int i = 0, iLen = series.Count; i < iLen; i++ )
        {
          IChartSerie serie = series[ i ];

          if( Array.IndexOf( ChartImpl.DEF_NOT_SUPPORT_GRIDLINES, serie.SerieType ) != -1 )
            return false;
        }

        return true;
      }

      return Array.IndexOf( DEF_NOT_SUPPORT_GRIDLINES, type ) == -1;
    }
    /// <summary>
    /// Sets to default chart grid lines on chart type or Series type changing.
    /// </summary>
    /// <param name="type">Represents type to change.</param>
    public void SetToDefaultGridlines( ExcelChartType type )
    {
      IChartAxis axis = m_primaryParentAxis.SeriesAxis;

      if( axis != null )
      {
        axis.HasMinorGridLines = false;
        axis.HasMajorGridLines = false;
      }

      axis = m_primaryParentAxis.CategoryAxis;

      if( axis != null )
      {
        axis.HasMinorGridLines = false;
        axis.HasMajorGridLines = false;
      }

      axis = m_primaryParentAxis.ValueAxis;

      if( axis != null )
      {
        axis.HasMinorGridLines = false;
        axis.HasMajorGridLines = false;
      }

      if( Array.IndexOf( DEF_NOT_SUPPORT_GRIDLINES, type ) == -1 )
        axis.HasMajorGridLines = true;
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public override void UpdateFormula( int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      m_series.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public override void MarkUsedReferences( bool[] usedItems )
    {
      if( m_series != null )
        m_series.MarkUsedReferences( usedItems );

      if( m_primaryParentAxis != null )
        m_primaryParentAxis.MarkUsedReferences( usedItems );

      if( m_secondaryParentAxis != null )
        m_secondaryParentAxis.MarkUsedReferences( usedItems );

      if( m_title != null )
        m_title.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public override void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      if( m_series != null )
        m_series.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_primaryParentAxis != null )
        m_primaryParentAxis.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_secondaryParentAxis != null )
        m_secondaryParentAxis.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_title != null )
        m_title.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    internal void RemoveSecondaryAxes()
    {
        if (IsSecondaryAxes)
        {
            if (IsCategoryAxisAvail)
                m_secondaryParentAxis.RemoveAxis(true);

            if (IsValueAxisAvail)
                m_secondaryParentAxis.RemoveAxis(false);
        }
    }
#if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    /// <summary>
    /// Method saves the chart sheet as image in the stream.
    /// </summary>
    /// <param name="imageAsstream">stream in where the image is streamed.</param>
    public void SaveAsImage(Stream imageAsstream)
    {
        IChartToImageConverter chartConverter = Application.ChartToImageConverter;
        if (chartConverter == null)
            throw new ArgumentException("IApplication.ChartToImageConverter must be instantiated");
        chartConverter.SaveAsImage(this,imageAsstream);        
    }
#endif
    #endregion

    #region Public static methods
    /// <summary>
    /// Converts double value into 32-bits fixed-point value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted 32-bits fixed-point value.</returns>
    public static int DoubleToFixedPoint( double value )
    {
      ushort number = (ushort) value;
      double fraction = (value - number) * 100000;
      
      if( fraction > ushort.MaxValue ) fraction /= 10;

      byte[] bytes1 = BitConverter.GetBytes( number );
      byte[] bytes2 = BitConverter.GetBytes( (ushort) fraction );
      byte[] buffer = new byte[4];
      
      bytes1.CopyTo( buffer, 2 );
      bytes2.CopyTo( buffer, 0 );

      return BitConverter.ToInt32( buffer, 0 );
    }
    /// <summary>
    /// Converts 32-bits fixed-point value into double.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted double value.</returns>
    public static double FixedPointToDouble( int value )
    {
      byte[] bytes = BitConverter.GetBytes( value );
      
      // Fraction part of the number.
      int fraction = BitConverter.ToUInt16( bytes, 0 );

      // Integer part of the double value.
      int number = BitConverter.ToUInt16( bytes, 2 );

      // Number of digits in the fraction part.
      int count = ( fraction != 0 ) ? ( int ) Math.Log10( fraction ) + 1 : 0;

      double result = Math.Abs( number ) + fraction / Math.Pow( 10, count );

      result *= Math.Sign( number );
      return result;
    }
    /// <summary>
    /// Creates primary series axis.
    /// </summary>
    /// <returns>Created axis.</returns>
    internal ChartSeriesAxisImpl CreatePrimarySeriesAxis()
    {
      ChartSeriesAxisImpl result;
      m_primaryParentAxis.SeriesAxis = result = new ChartSeriesAxisImpl( Application,
        m_primaryParentAxis, ExcelAxisType.Serie );

      return result;
    }
    #endregion
  }
}
