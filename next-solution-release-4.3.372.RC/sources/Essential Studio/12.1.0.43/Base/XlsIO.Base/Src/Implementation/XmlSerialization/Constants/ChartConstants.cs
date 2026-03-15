#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// This class contains constants required for charts parsing and serialization in Excel 2007 format.
  /// </summary>
  public sealed class ChartConstants
  {
    #region Constants
    /// <summary>
    /// Main charts namespace.
    /// </summary>
    public const string CNamespace = "http://schemas.openxmlformats.org/drawingml/2006/chart";
    /// <summary>
    /// Namespace of the slicer
    /// </summary>
    public const string SlicerNamespace = "http://schemas.microsoft.com/office/drawing/2010/slicer";
    /// <summary>
    /// Main charts namespace for Excel 2010
    /// </summary>
    public const string CNamespace2007 = "http://schemas.microsoft.com/office/drawing/2007/8/2/chart";
    /// <summary>
    /// Chart color style namespace for Excel 2013
    /// </summary>
    public const string CColorNamespace2013 = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";
    /// <summary>
    /// Chart style namespace for Excel 2013
    /// </summary>
    public const string CStyleNamespace2013 = "http://schemas.microsoft.com/office/2011/relationships/chartStyle"; 
    /// <summary>
    /// Uri for the pivot chart.
    /// </summary>
    public const string Pivoturi = "{781A3756-C4B2-4CAC-9D66-4F8BD8637D16}";
    /// <summary>
    /// prefix for the Excel 2010 chart tag.
    /// </summary>
    public const string ChartPrefix2010 = "c14";
    /// <summary>
    /// This element specifies a category axis.
    /// </summary>
    public const string CategoryAxisTag = "catAx";
    /// <summary>
    /// This element specifies a value axis.
    /// </summary>
    public const string ValueAxisTag = "valAx";
    /// <summary>
    /// This element specifies a series axis.
    /// </summary>
    public const string SeriesAxisTag = "serAx";
    /// <summary>
    /// This element specifies a date axis.
    /// </summary>
    public const string DateAxisTag = "dateAx";
    /// <summary>
    /// Value attribute.
    /// </summary>
    public const string ValueAttribute = "val";
    /// <summary>
    /// Some value axis id.
    /// </summary>
    public const int DefaultValueAxisId = 57253888;
    /// <summary>
    /// Some category axis id.
    /// </summary>
    public const int DefaultCategoryAxisId = 59983360;
    /// <summary>
    /// Some series axis id.
    /// </summary>
    public const int DefaultSeriesAxisId = 63149376;
    /// <summary>
    /// Some secondary category axis id.
    /// </summary>
    public const int DefaultSecondaryCategoryAxisId = 62908672;
    /// <summary>
    /// Some secondary value axis id.
    /// </summary>
    public const int DefaultSecondaryValueAxisId = 61870848;
    /// <summary>
    /// This element contains additional axis settings.
    /// </summary>
    public const string ScalingTag = "scaling";
    /// <summary>
    /// This element specifies the logarithmic base for a logarithmic axis.
    /// </summary>
    public const string LogarithmicBaseTag = "logBase";
    /// <summary>
    /// This element specifies the maximum value of the axis.
    /// </summary>
    public const string MaximumTag = "max";
    /// <summary>
    /// This element specifies the minimum value of the axis.
    /// </summary>
    public const string MinimumTag = "min";
    /// <summary>
    /// This element specifies the stretching and stacking of the picture on the
    /// data point, series, wall, or floor.
    /// </summary>
    public const string AxisOrientationTag = "orientation";
    /// <summary>
    /// When specified as a child element of valAx, dateAx, catAx, or serAx,
    /// this element specifies the identifier for the axis. When specified as
    /// a child element of a chart, this element specifies the identifier of
    /// an axis that defines the coordinate space of the chart.
    /// </summary>
    public const string AxisIdTag = "axId";
    /// <summary>
    /// Specifies that the values on the axis shall be reversed so they go from maximum to minimum.
    /// </summary>
    public const string MaxMinOrientation = "maxMin";
    /// <summary>
    /// Specifies that the axis values shall be in the usual order, minimum to maximum.
    /// </summary>
    public const string MinMaxOrientation = "minMax";
    /// <summary>
    /// This element specifies the position of the axis on the chart.
    /// </summary>
    public const string AxisPositionTag = "axPos";
    /// <summary>
    /// Specifies that the axis shall be displayed at the left of the plot area.
    /// </summary>
    public const string AxisPosLeft = "l";
    /// <summary>
    /// Specifies that the axis shall be displayed at the right of the plot area.
    /// </summary>
    public const string AxisPosRight = "r";
    /// <summary>
    /// Specifies that the axis shall be displayed at the top of the plot area.
    /// </summary>
    public const string AxisPosTop = "t";
    /// <summary>
    /// Specifies that the axis shall be displayed at the bottom of the plot area.
    /// </summary>
    public const string AxisPosBottom = "b";
    /// <summary>
    /// This element specifies major gridlines.
    /// </summary>
    public const string MajorGridlinesTag = "majorGridlines";
    /// <summary>
    /// This element specifies minor gridlines.
    /// </summary>
    public const string MinorGridlinesTag = "minorGridlines";
    /// <summary>
    /// This element specifies number formatting for the parent element.
    /// </summary>
    public const string NumberFormatTag = "numFmt";
    /// <summary>
    /// This element specifies a string representing the format code to apply.
    /// </summary>
    public const string FormatCodeAttribute = "formatCode";
    /// <summary>
    /// Indicates whether number format is applied to the axis.
    /// </summary>
    public const string SourceLinkedAttribute = "sourceLinked";
    /// <summary>
    /// This element specifies the position of the tick labels on the axis.
    /// </summary>
    public const string TickLabelPositionTag = "tickLblPos";
    /// <summary>
    /// Specifies the axis labels shall be at the high end of the perpendicular axis.
    /// </summary>
    public const string TickLabelHigh = "high";
    /// <summary>
    /// Specifies the axis labels shall be at the low end of the perpendicular axis.
    /// </summary>
    public const string TickLabelLow = "low";
    /// <summary>
    /// Specifies the axis labels shall be next to the axis.
    /// </summary>
    public const string TickLabelNextTo = "nextTo";
    /// <summary>
    /// Specifies the axis labels are not drawn.
    /// </summary>
    public const string TickLabelNone = "none";
    /// <summary>
    /// This element specifies the major tick marks.
    /// </summary>
    public const string MajorTickMarkTag = "majorTickMark";
    /// <summary>
    /// This element specifies the minor tick marks.
    /// </summary>
    public const string MinorTickMarkTag = "minorTickMark";
    /// <summary>
    /// Specifies there shall be no tick marks.
    /// </summary>
    public const string TickMarkNone = "none";
    /// <summary>
    /// Specifies the tick marks shall be inside the plot area.
    /// </summary>
    public const string TickMarkInside = "in";
    /// <summary>
    /// Specifies the tick marks shall be outside the plot area.
    /// </summary>
    public const string TickMarkOutside = "out";
    /// <summary>
    /// Specifies the tick marks shall cross the axis.
    /// </summary>
    public const string TickMarkCross = "cross";
    /// <summary>
    /// This element specifies the ID of axis that this axis crosses.
    /// </summary>
    public const string CrossAxisTag = "crossAx";
    /// <summary>
    /// This element specifies whether the value axis crosses the category axis between categories.
    /// </summary>
    public const string CrossBetweenTag = "crossBetween";
    /// <summary>
    /// Specifies the value axis shall cross the category axis between data markers.
    /// </summary>
    public const string BetweenValue = "between";
    /// <summary>
    /// Specifies the value axis shall cross the category axis at the midpoint of a category.
    /// </summary>
    public const string CategoryMidpoint = "midCat";
    /// <summary>
    /// This element specifies the distance between major ticks.
    /// </summary>
    public const string MajorUnitTag = "majorUnit";
    /// <summary>
    /// This element specifies the distance between minor tick marks.
    /// </summary>
    public const string MinorUnitTag = "minorUnit";
    /// <summary>
    /// This element specifies the distance of labels from the axis.
    /// </summary>
    public const string LabelOffsetTag = "lblOffset";
    /// <summary>
    /// This element specifies how many tick labels to skip between label that is drawn.
    /// </summary>
    public const string TickLabelSkip = "tickLblSkip";
    /// <summary>
    /// This element specifies how many tick marks shall be skipped before the next one shall be drawn.
    /// </summary>
    public const string TickMarkSkip = "tickMarkSkip";
    /// <summary>
    /// This element specifies whether multi level labels exists or not
    /// </summary>
    public const string NoMultiLvlLblTag = "noMultiLvlLbl";
    /// <summary>
    /// This element specifies the smallest time unit that is represented on the date axis.
    /// </summary>
    public const string BaseTimeUnitTag = "baseTimeUnit";
    /// <summary>
    /// This element specifies the time unit for major tick marks.
    /// </summary>
    public const string MajorTimeUnit = "majorTimeUnit";
    /// <summary>
    /// This element specifies the time unit for the minor tick marks.
    /// </summary>
    public const string MinorTimeUnit = "minorTimeUnit";
    /// <summary>
    /// This element specifies the chart.
    /// </summary>
    public const string ChartTag = "chart";
    /// <summary>
    /// This element specifies the plot area of the chart.
    /// </summary>
    public const string PlotAreaTag = "plotArea";
    /// <summary>
    /// This element specifies whether the series form a bar (horizontal) chart or a column (vertical) chart.
    /// </summary>
    public const string BarDirectionTag = "barDir";
    /// <summary>
    /// Specifies that the chart is a bar chart - the data markers are horizontal rectangles.
    /// </summary>
    public const string BarDirectionBar = "bar";
    /// <summary>
    /// Specifies that the chart is a column chart - the data markers are vertical rectangles.
    /// </summary>
    public const string BarDirectionColumn = "col";
    /// <summary>
    /// This element specifies the type of grouping for a bar chart.
    /// </summary>
    public const string BarGroupingTag = "grouping";
    /// <summary>
    /// Specifies that the chart series are drawn next to each other along the category axis.
    /// </summary>
    public const string Clustered = "clustered";
    /// <summary>
    /// Specifies that the chart series are drawn next to each other along the
    /// value axis and scaled to total 100%.
    /// </summary>
    public const string PercentStacked = "percentStacked";
    /// <summary>
    /// Specifies that the chart series are drawn next to each other on the value axis.
    /// </summary>
    public const string Stacked = "stacked";
    /// <summary>
    /// Specifies that the chart series are drawn next to each other on the depth axis.
    /// </summary>
    public const string Standard = "standard";
    /// <summary>
    /// This element specifies that each data marker in the series shall have a different color.
    /// </summary>
    public const string VaryColorsTag = "varyColors";
    /// <summary>
    /// This element specifies a series on a chart.
    /// </summary>
    public const string SeriesTag = "ser";
    /// <summary>
    /// This element specifies the index of the containing element. This index
    /// shall determine which of the parent's children collection this element applies to.
    /// </summary>
    public const string IndexTag = "idx";
    /// <summary>
    /// This element specifies the order of the series in the collection. It is 0 based.
    /// </summary>
    public const string SeriesOrderTag = "order";
    /// <summary>
    /// This element specifies the data values which shall be used to define
    /// the location of data markers on a chart.
    /// </summary>
    public const string SeriesValuesTag = "val";
    /// <summary>
    /// This element specifies the data used for the category axis.
    /// </summary>
    public const string CategoryValuesTag = "cat";
    /// <summary>
    /// This element specifies a reference to numeric data with a cache of the last values used.
    /// </summary>
    public const string NumberReferenceTag = "numRef";
    /// <summary>
    /// This element specifies a reference to string data with a cache of the last values used.
    /// </summary>
    public const string StringReferenceTag = "strRef";
    /// <summary>
    /// This element specifies a reference to data for the category axis (or for the x-values in a bubble or scatter chart)), 
    /// along with a cache of the last values used.
    /// </summary>
    public const string MultiLevelStringReferenceTag = "multiLvlStrRef";
    /// <summary>
    /// This element specifies the last numeric data used for a chart.
    /// </summary>
    public const string NumberCacheTag = "numCache";
    /// <summary>
    /// This element specifies the last string data used for a chart.
    /// </summary>
    public const string StringCacheTag = "strCache";
    /// <summary>
    /// This element specifies a cache of the labels on the category axis, or the x-values in a bubble or scatter chart.
    /// </summary>
    public const string MultiLevelStringCacheTag = "multiLvlStrCache";
    /// <summary>
    /// This element specifies a reference to source of the data contained in this chart.
    /// </summary>
    public const string Formula = "f";
    /// <summary>
    /// This element specifies overall settings for a single chart, and is the root node for the chart part.
    /// </summary>
    public const string ChartSpaceTag = "chartSpace";
    /// <summary>
    /// This element specifies the 3-D area series on this chart.
    /// </summary>
    public const string Area3DChartTag = "area3DChart";
    /// <summary>
    /// This element specifies the 2-D area series on this chart.
    /// </summary>
    public const string AreaChartTag = "areaChart";
    /// <summary>
    /// This element contains the 2-D bar or column series on this chart.
    /// </summary>
    public const string BarChartTag = "barChart";
    /// <summary>
    /// This element contains the 3-D bar or column series on this chart.
    /// </summary>
    public const string Bar3DChartTag = "bar3DChart";
    /// <summary>
    /// This element contains the 3-D line chart series.
    /// </summary>
    public const string Line3DChartTag = "line3DChart";
    /// <summary>
    /// This element contains the 2-D line chart series.
    /// </summary>
    public const string LineChartTag = "lineChart";
    /// <summary>
    /// This element specifies the space between bar or column clusters,
    /// as a percentage of the bar or column width.
    /// </summary>
    public const string GapWidthTag = "gapWidth";
    /// <summary>
    /// This element specifies the space between bar or column clusters,
    /// as a percentage of the bar or column width.
    /// </summary>
    public const string GapDepthTag = "gapDepth";
    /// <summary>
    /// This element specifies how much bars and columns shall overlap on 2-D charts.
    /// </summary>
    public const string OverlapTag = "overlap";
    /// <summary>
    /// Specifies the chart shall be drawn as a cone, with the base of the cone
    /// on the floor and the point of the cone at the top of the data marker.
    /// </summary>
    public const string BarShapeCone = "cone";
    /// <summary>
    /// Specifies the chart shall be drawn with truncated cones such that the
    /// point of the cone would be the maximum data value.
    /// </summary>
    public const string BarShapeConeToMax = "coneToMax";
    /// <summary>
    /// Specifies the chart shall be drawn as a rectangular pyramid, with the
    /// base of the pyramid on the floor and the point of the pyramid at the
    /// top of the data marker.
    /// </summary>
    public const string BarShapePyramid = "pyramid";
    /// <summary>
    /// Specifies the chart shall be drawn with truncated cones such that the
    /// point of the cone would be the maximum data value.
    /// </summary>
    public const string BarShapePyramidToMax = "pyramidToMax";
    /// <summary>
    /// Specifies the chart shall be drawn as a cylinder.
    /// </summary>
    public const string BarShapeCylinder = "cylinder";
    /// <summary>
    /// Specifies the chart shall be drawn with a box shape.
    /// </summary>
    public const string BarShapeBox = "box";
    /// <summary>
    /// This element specifies the shape of a series or a 3-D bar chart.
    /// </summary>
    public const string BarShapeTag = "shape";
    /// <summary>
    /// This element contains the bubble series on this chart.
    /// </summary>
    public const string BubbleChartTag = "bubbleChart";
    /// <summary>
    /// This element specifies that the bubbles have a 3-D effect applied to them.
    /// </summary>
    public const string Bubble3DTag = "bubble3D";
    /// <summary>
    /// Default bubble scale value.
    /// </summary>
    public const int BubbleScaleDefault = 100;
    /// <summary>
    /// This element specifies the scale factor for the bubble chart. This element can be
    /// an integer value from 0 to 300, corresponding to a percentage of the default size.
    /// </summary>
    public const string BubbleScaleTag = "bubbleScale";
    /// <summary>
    /// This element specifies negative sized bubbles shall be shown on a bubble chart.
    /// </summary>
    public const string ShowNegativeBubbles = "showNegBubbles";
    /// <summary>
    /// Specifies the area of the bubbles shall be proportional to the bubble size value.
    /// </summary>
    public const string BubbleSizeArea = "area";
    /// <summary>
    /// Specifies the radius of the bubbles shall be proportional to the bubble size value.
    /// </summary>
    public const string BubbleSizeWidth = "w";
    /// <summary>
    /// This element specifies how the bubble size values are represented on the chart.
    /// </summary>
    public const string BubbleSizeRepresents = "sizeRepresents";
    /// <summary>
    /// This element contains the set of 2-D contour charts.
    /// </summary>
    public const string SurfaceChartTag = "surfaceChart";
    /// <summary>
    /// This element contains the set of 3-D surface series.
    /// </summary>
    public const string Surface3DChartTag = "surface3DChart";
    /// <summary>
    /// This element specifies the surface chart is drawn as a wireframe.
    /// </summary>
    public const string WireframeTag = "wireframe";
    public const string BandFormats = "bandFmts";
    /// <summary>
    /// This element contains the radar chart series on this chart.
    /// </summary>
    public const string RadarChartTag = "radarChart";
    /// <summary>
    /// This element specifies what type of radar chart shall be drawn.
    /// </summary>
    public const string RadarStyleTag = "radarStyle";
    /// <summary>
    /// This element contains the scatter chart series for this chart.
    /// </summary>
    public const string ScatterChartTag = "scatterChart";
    /// <summary>
    /// This element specifies the type of lines for the scatter chart.
    /// </summary>
    public const string ScatterStyleTag = "scatterStyle";
    /// <summary>
    /// This element contains the 2-D pie series for this chart.
    /// </summary>
    public const string PieChartTag = "pieChart";
    /// <summary>
    /// This element contains the 3-D pie series for this chart.
    /// </summary>
    public const string Pie3DChartTag = "pie3DChart";
    /// <summary>
    /// This element specifies the angle of the first pie or doughnut chart slice,
    /// in degrees (clockwise from up).
    /// </summary>
    public const string FirstSliceAngleTag = "firstSliceAng";
    /// <summary>
    /// This element contains the doughnut chart series.
    /// </summary>
    public const string DoughnutChartTag = "doughnutChart";
    /// <summary>
    /// This element specifies the size of the hole in a doughnut chart group.
    /// </summary>
    public const string DoughnutHoleSizeTag = "holeSize";
    /// <summary>
    /// This element contains the pie of pie or bar of pie series on this chart.
    /// </summary>
    public const string OfPieChartTag = "ofPieChart";
    /// <summary>
    /// Specifies that the chart is pie of pie chart, not a bar of pie chart.
    /// </summary>
    public const string OfPieTypePie = "pie";
    /// <summary>
    /// Specifies that the chart is a bar of pie chart, not a pie of pie chart.
    /// </summary>
    public const string OfPieTypeBar = "bar";
    /// <summary>
    /// This element specifies whether this chart is pie of pie or bar of pie.
    /// </summary>
    public const string OfPieTypeTag = "ofPieType";
    /// <summary>
    /// This element specifies a value that shall be used to determine which data
    /// points are in the second pie or bar on a pie of pie or bar of pie chart.
    /// </summary>
    public const string SplitPosTag = "splitPos";
    /// <summary>
    /// This element specifies the size of the second pie or bar of a pie of pie
    /// chart or a bar of pie chart, as a percentage of the size of the first pie.
    /// </summary>
    public const string SecondPieSizeTag = "secondPieSize";
    /// <summary>
    /// This element specifies how to determine which data points are in the
    /// second pie or bar on a pie of pie or bar of pie chart.
    /// </summary>
    public const string SplitTypeTag = "splitType";
    /// <summary>
    /// This element contains the collection of stock chart series.
    /// </summary>
    public const string StockChartTag = "stockChart";
    /// <summary>
    /// This element serves as a root element that specifies the settings for the
    /// data labels for an entire series or the entire chart. It contains child
    /// elements that specify the specific formatting and positioning settings.
    /// </summary>
    public const string DataLabelsTag = "dLbls";
    /// <summary>
    /// This element specifies the position of the data label.
    /// </summary>
    public const string DataLabelPosTag = "dLblPos";
    /// <summary>
    /// This element specifies that the value shall be shown in a data label.
    /// </summary>
    public const string ShowValueTag = "showVal";
    /// <summary>
    /// This element specifies that the category name shall be shown in the data label.
    /// </summary>
    public const string ShowCategoryTag = "showCatName";
    /// <summary>
    /// This element specifies that the percentage shall be shown in a data label.
    /// </summary>
    public const string ShowPercentageTag = "showPercent";
    /// <summary>
    /// This element specifies the bubble size shall be shown in a data label.
    /// </summary>
    public const string ShowBubbleSizeTag = "showBubbleSize";
    /// <summary>
    /// This element specifies that the series name shall be shown in a data label.
    /// </summary>
    public const string ShowSeriesNameTag = "showSerName";
    /// <summary>
    /// This element specifies legend keys shall be shown in data labels.
    /// </summary>
    public const string ShowLegendKeyTag = "showLegendKey";
    /// <summary>
    /// This element specifies Leader Lines shall be shown in data labels.
    /// </summary>
    public const string ShowLeaderLineTag = "showLeaderLines";
    /// <summary>
    /// This element specifies text that shall be used to separate the parts of a data label.
    /// The default is a comma, except for pie charts showing only category name and percentage,
    /// when a line break shall be used instead.
    /// </summary>
    public const string DataLabelsSeparatorTag = "separator";
    /// <summary>
    /// This element specifies a data marker.
    /// </summary>
    public const string MarkerTag = "marker";
    /// <summary>
    /// This element specifies the marker that shall be used for the data points.
    /// </summary>
    public const string MarkerStyleTag = "symbol";
    /// <summary>
    /// This element specifies the size of the marker in points.
    /// </summary>
    public const string MarkerSizeTag = "size";
    /// <summary>
    /// This element specifies the floor of a 3D chart.
    /// </summary>
    public const string FloorTag = "floor";
    /// <summary>
    /// This element specifies the back wall of the chart.
    /// </summary>
    public const string BackWallTag = "backWall";
    /// <summary>
    /// This element specifies the side wall.
    /// </summary>
    public const string SideWallTag = "sideWall";
    /// <summary>
    /// This element specifies a title.
    /// </summary>
    public const string TitleTag = "title";
    /// <summary>
    /// This element specifies a trend line.
    /// </summary>
    public const string TrendlineTag = "trendline";
    /// <summary>
    /// This element specifies the name of the trend line.
    /// </summary>
    public const string TrendlineNameTag = "name";
    /// <summary>
    /// This element specifies the type of the trend line.
    /// </summary>
    public const string TrendlineTypeTag = "trendlineType";
    /// <summary>
    /// This element specifies the order of the polynomial trend line.
    /// It is ignored for other trend line types.
    /// </summary>
    public const string TrendlineOrderTag = "order";
    /// <summary>
    /// This element specifies the period of the trend line for a moving average
    /// trend line. It is ignored for other trend line types.
    /// </summary>
    public const string TrendlinePeriodTag = "period";
    /// <summary>
    /// This element specifies the number of categories (or units on a scatter
    /// chart) that the trend line extends after the data for the series that is
    /// being trended. On non-scatter charts, the value must be a multiple of 0.5.
    /// </summary>
    public const string TrendlineForwardTag = "forward";
    /// <summary>
    /// This element specifies the number of categories (or units on a scatter chart)
    /// that the trend line extends before the data for the series that is being trended.
    /// On non-scatter charts, the value shall be 0 or 0.5.
    /// </summary>
    public const string TrendlineBackwardTag = "backward";
    /// <summary>
    /// This element specifies the value where the trend line shall cross the y
    /// axis. This property shall be supported only when the trend line type is
    /// exp, linear, or poly.
    /// </summary>
    public const string TrendlineIntercept = "intercept";
    /// <summary>
    /// This element specifies that the R-squared value of the trend line is
    /// displayed on the chart (in the same label as the equation).
    /// </summary>
    public const string DisplayRSquared = "dispRSqr";
    /// <summary>
    /// This element specifies that the equation for the trend line is displayed
    /// on the chart (in the same label as the R-squared value).
    /// </summary>
    public const string DisplayEquation = "dispEq";
    /// <summary>
    /// This element specifies error bars.
    /// </summary>
    public const string ErrorBarsTag = "errBars";
    /// <summary>
    /// This element specifies the type of the error bars - positive, negative, or both.
    /// </summary>
    public const string ErrorBarTypeTag = "errBarType";
    /// <summary>
    /// This element specifies the type of values used to determine the length of the error bars.
    /// </summary>
    public const string ErrorBarValueType = "errValType";
    /// <summary>
    /// This element specifies an end cap is not drawn on the error bars.
    /// </summary>
    public const string ErrorBarsNoCap = "noEndCap";
    /// <summary>
    /// This element specifies a value which is used with the Error Bar Type
    /// to determine the length of the error bars.
    /// </summary>
    public const string ErrorBarValueTag = "val";
    /// <summary>
    /// This element specifies the error bar value in the positive direction.
    /// It shall be used only when the errValType is cust.
    /// </summary>
    public const string ErrorBarPlusTag = "plus";
    /// <summary>
    /// This element specifies the error bar value in the negative direction.
    /// It shall be used only when the errValType is cust.
    /// </summary>
    public const string ErrorBarMinusTag = "minus";
    /// <summary>
    /// Specifies that error bars shall be shown in the x direction.
    /// </summary>
    public const string ErrorBarX = "x";
    /// <summary>
    /// Specifies that error bars shall be shown in the y direction.
    /// </summary>
    public const string ErrorBarY = "y";
    /// <summary>
    /// This element specifies the direction of the error bars.
    /// </summary>
    public const string ErrorBarDirection = "errDir";
    /// <summary>
    /// This element specifies the 3-D view of the chart.
    /// </summary>
    public const string View3DTag = "view3D";
    /// <summary>
    /// This element specifies the amount a 3-D chart shall be rotated in the X direction.
    /// </summary>
    public const string RotationXTag = "rotX";
    /// <summary>
    /// This element specifies the height of a 3-D chart as a percentage of the
    /// chart width (between 5 and 500).
    /// </summary>
    public const string HeightPercentTag = "hPercent";
    /// <summary>
    /// This element specifies the amount a 3-D chart shall be rotated in the Y direction.
    /// </summary>
    public const string RotationYTag = "rotY";
    /// <summary>
    /// This element specifies that the chart axes are at right angles,
    /// rather than drawn in perspective. Applies only to 3-D charts.
    /// </summary>
    public const string RightAngleAxesTag = "rAngAx";
    /// <summary>
    /// This element specifies the field of view angle for the 3-D chart.
    /// This element is ignored if Right Angle Axes is true.
    /// </summary>
    public const string PerspectiveTag = "perspective";
    /// <summary>
    /// This element specifies the depth of a 3-D chart as a percentage of the
    ///   chart width (between 20 and 2000 percent).
    /// </summary>
    public const string DepthPercentTag = "depthPercent";
    /// <summary>
    /// This element specifies the legend.
    /// </summary>
    public const string LegendTag = "legend";
    /// <summary>
    /// This element specifies the position of the legend.
    /// </summary>
    public const string LegendPositionTag = "legendPos";
    /// <summary>
    /// This element specifies that other chart elements shall be allowed to overlap this chart element.
    /// </summary>
    public const string OverlayTag = "overlay";
    /// <summary>
    /// This element specifies a legend entry.
    /// </summary>
    public const string LegendEntryTag = "legendEntry";
    /// <summary>
    /// This element specifies that the chart element specified by its containing
    /// element shall be deleted from the chart.
    /// </summary>
    public const string DeleteTag = "delete";
    /// <summary>
    /// This element specifies how blank cells shall be plotted on a chart.
    /// </summary>
    public const string DisplayBlanksAsTag = "dispBlanksAs";
    /// <summary>
    /// This element specifies that only visible cells should be plotted on the chart.
    /// </summary>
    public const string PlotVisibleOnlyTag = "plotVisOnly";
    /// <summary>
    /// This element specifies the page margins for a chart.
    /// </summary>
    public const string PageMarginsTag = "pageMargins";
    /// <summary>
    /// Specifies the contents of this attribute will contain the left page margin in inches.
    /// </summary>
    public const string LeftMargin = "l";
    /// <summary>
    /// Specifies the contents of this attribute will contain the right page margin in inches.
    /// </summary>
    public const string RightMargin = "r";
    /// <summary>
    /// Specifies the contents of this attribute will contain the top page margin in inches.
    /// </summary>
    public const string TopMargin = "t";
    /// <summary>
    /// Specifies the contents of this attribute will contain the bottom page margin in inches.
    /// </summary>
    public const string BottomMargin = "b";
    /// <summary>
    /// Specifies the contents of this attribute will contain the header margin in inches.
    /// </summary>
    public const string HeaderMargin = "header";
    /// <summary>
    /// Specifies the contents of this attribute will contain the footer margin in inches.
    /// </summary>
    public const string FooterMargin = "footer";
    /// <summary>
    /// This element specifies the up and down bars.
    /// </summary>
    public const string UpDownBarsTag = "upDownBars";
    /// <summary>
    /// This element specifies the up bars on the chart.
    /// </summary>
    public const string UpBarsTag = "upBars";
    /// <summary>
    /// This element specifies the down bars on the chart.
    /// </summary>
    public const string DownBarsTag = "downBars";
    /// <summary>
    /// This element specifies text to use on a chart, including rich text formatting.
    /// </summary>
    public const string ChartTextTag = "tx";
    /// <summary>
    /// This element contains a string with rich text formatting.
    /// </summary>
    public const string RichTextTag = "rich";
    /// <summary>
    /// This is the root element of Sheet Parts that are of type 'chartsheet'.
    /// </summary>
    public const string ChartsheetTag = "chartsheet";
    /// <summary>
    /// Prefix for chart namespace.
    /// </summary>
    public const string CPrefix = "c";
    /// <summary>
    /// Underline the text with a single line of normal thickness.
    /// </summary>
    public const string UnderlineSingle = "sng";
    /// <summary>
    /// Underline the text with two lines of normal thickness.
    /// </summary>
    public const string UnderlineDouble = "dbl";
    /// <summary>
    /// A single strikethrough is applied to the text.
    /// </summary>
    public const string StrikeThroughSingle = "sngStrike";
    /// <summary>
    /// Text is not strike through.
    /// </summary>
    public const string StrikeThroughNone = "noStrike";
    /// <summary>
    /// This element specifies how this axis crosses the perpendicular axis.
    /// </summary>
    public const string CrossesTag = "crosses";
    /// <summary>
    /// This element specifies where on the axis the perpendicular axis crosses.
    /// The units are dependent on the type of axis.
    /// </summary>
    public const string CrossesAtTag = "crossesAt";
    /// <summary>
    /// The category axis crosses at the zero point of the value axis (if possible),
    /// or the minimum value (if the minimum is greater than zero) or the maximum
    /// (if the maximum is less than zero).
    /// </summary>
    public const string CrossesAutoZero = "autoZero";
    /// <summary>
    /// The axis crosses at the maximum value
    /// </summary>
    public const string CrossesMaximum = "max";
    /// <summary>
    /// This element specifies the x values which shall be used to define the
    /// location of data markers on a chart.
    /// </summary>
    public const string XValues = "xVal";
    /// <summary>
    /// This element specifies the y values which shall be used to define the
    /// location of data markers on a chart.
    /// </summary>
    public const string YValues = "yVal";
    /// <summary>
    /// This element specifies the data for the sizes of the bubbles on the bubble chart.
    /// </summary>
    public const string BubbleSize = "bubbleSize";
    /// <summary>
    /// This element specifies the amount the data point shall be moved from the center of the pie.
    /// </summary>
    public const string PieExplosionTag = "explosion";
    /// <summary>
    /// This element specifies series lines for the chart.
    /// </summary>
    public const string SeriesLinesTag = "serLines";
    /// <summary>
    /// This element specifies the line connecting the points on the chart shall
    /// be smoothed using Catmull-Rom splines.
    /// </summary>
    public const string SmoothTag = "smooth";
    /// <summary>
    /// This element specifies the label for the trend line.
    /// </summary>
    public const string TrendlineLabelTag = "trendlineLbl";
    /// <summary>
    /// This element specifies text for a series name, without rich text formatting.
    /// </summary>
    public const string SeriesTextTag = "tx";
    /// <summary>
    /// This element specifies a text value for a category axis label or a series name.
    /// </summary>
    public const string TextValueTag = "v";
    /// <summary>
    /// This element specifies the high-low lines for the series.
    /// </summary>
    public const string HiLowLinesTag = "hiLowLines";
    /// <summary>
    /// This element specifies how the chart element is placed on the chart.
    /// </summary>
    public const string LayoutTag = "layout";
    /// <summary>
    /// This element specifies how the chart element is placed on the chart manually
    /// </summary>
    public const string ManualLayoutTag = "manualLayout";
    /// <summary>
    /// This element specifies the chart layout target
    /// </summary>
    public const string LayoutTargetTag = "layoutTarget";
    /// <summary>
    /// Represents left element for manual layout
    /// </summary>
    public const string LeftModeTag = "xMode";
    /// <summary>
    /// Represents top element for manual layout
    /// </summary>
    public const string TopModeTag = "yMode";
    /// <summary>
    /// Represents the x location (left) of the chart element as a 
    /// fraction of width of the chart
    /// </summary>
    public const string LeftTag = "x";
    /// <summary>
    /// Represents the y location (top) of the chart element as a 
    /// fraction of width of the chart
    /// </summary>
    public const string TopTag = "y";
    /// <summary>
    /// Represents the width or horizontal offset
    /// </summary>
    public const string dXTag = "dX";
    /// <summary>
    /// Represents the height or a vertical offset
    /// </summary>
    public const string dYTag = "dY";
    /// <summary>
    /// Represents the layout mode for the width of the element
    /// </summary>
    public const string WidthModeTag = "wMode";
    /// <summary>
    /// Represents the layout mode for the height of the element
    /// </summary>
    public const string HeightModeTag = "hMode";
    /// <summary>
    /// Represents the width of the element
    /// </summary>
    public const string WidthTag = "w";
    /// <summary>
    /// Represents the height of the element
    /// </summary>
    public const string HeightTag = "h";
    /// <summary>
    /// This element specifies a single data point.
    /// </summary>
    public const string DataPointTag = "dPt";
    /// <summary>
    /// This element specifies a data label.
    /// </summary>
    public const string DataLabelTag = "dLbl";
    /// <summary>
    /// This element specifies the logarithmic base for a logarithmic axis.
    /// </summary>
    public const string LogBaseTag = "logBase";
    /// <summary>
    /// Default value of the logBase tag.
    /// </summary>
    public const int LogBaseDefault = 10;
    /// <summary>
    /// This element specifies the scaling value of the display units for the value axis.
    /// </summary>
    public const string DisplayUnitsTag = "dispUnits";
    /// <summary>
    /// This element specifies the display unit is one of the built in values.
    /// </summary>
    public const string BuiltInUnitTag = "builtInUnit";
    /// <summary>
    /// This element specifies a custom value for the display unit.
    /// </summary>
    public const string CustomUnitTag = "custUnit";
    /// <summary>
    /// This element specifies a data table.
    /// </summary>
    public const string DataTableTag = "dTable";
    /// <summary>
    /// This element specifies the horizontal borders shall be shown in a data table.
    /// </summary>
    public const string ShowHorizontalBorder = "showHorzBorder";
    /// <summary>
    /// This element specifies the vertical border shall be shown in a data table.
    /// </summary>
    public const string ShowVerticalBorder = "showVertBorder";
    /// <summary>
    /// This element specifies the outline shall be shown on a data table.
    /// </summary>
    public const string ShowOutline = "showOutline";
    /// <summary>
    /// This element specifies the legend keys shall be shown in a data table.
    /// </summary>
    public const string ShowSeriesKeys = "showKeys";
    /// <summary>
    /// This element specifies drop lines.
    /// </summary>
    public const string DropLinesTag = "dropLines";
    /// <summary>
    /// This element contains text properties.
    /// </summary>
    public const string TextPropertiesTag = "txPr";
    /// <summary>
    /// This element specifies that this axis is a date or text axis based on
    /// the data that is used for the axis labels, not a specific choice.
    /// </summary>
    public const string AutoCategoryAxis = "auto";
    /// <summary>
    /// This element specifies the rounded corner tag.
    /// </summary>
    public const string RoundedCornersTag = "roundedCorners";
    /// <summary>
    /// This element specifies the style tag.
    /// </summary>
    public const string ChartStyleTag = "style";
    /// <summary>
    /// This element specifies the shapes drawn on top of the chart.
    /// </summary>
    public const string UserShapesTag = "userShapes";
    /// <summary>
    /// This element specifies that the shape described here to reside within a
    /// chart should be sized based on relative anchor points.
    /// </summary>
    public const string RelativeSizeAnchorTag = "relSizeAnchor";
    /// <summary>
    /// This element specifies the relative x coordinate that is used to define
    /// the percentage-based horizontal position for a shape within a chart drawing object.
    /// </summary>
    public const string XTagName = "x";
    /// <summary>
    /// This element specifies the relative y coordinate that is used to define
    /// the percentage-based vertical position for a shape within a chart drawing object.
    /// </summary>
    public const string YTagName = "y";
    /// <summary>
    /// Multiplier for coordinates transform.
    /// </summary>
    public const double CoordinatesMultiplyer = 1000;
    /// <summary>
    /// This element specifies a set of numbers used for the parent element.
    /// </summary>
    public const string NumberLiteral = "numLit";
    /// <summary>
    /// This element specifies a set of strings used for the parent element.
    /// </summary>
    public const string StringLiteral = "strLit";
    /// <summary>
    /// This element contains the number of values in the cache.
    /// </summary>
    public const string PointCount = "ptCount";
    /// <summary>
    /// This element specifies data for a particular data point.
    /// </summary>
    public const string NumericPoint = "pt";
    /// <summary>
    /// This element specifies the series
    /// to invert its colors if the value is negative.
    /// </summary>
    public const string InvertIfNegative = "invertIfNegative";
    /// <summary>
    /// Value attribute.
    /// </summary>
    public const string NumbericValue = "v";
    public const string PivotSourceTag = "pivotSource";
    public const string PivotFormats = "pivotFmts";
    public const string DisplayUnitsLabel = "dispUnitsLbl";
    public const string ZoomToFit = "zoomToFit";
    public const string LabelAlignment = "lblAlgn";
    public const string PrintSettings = "printSettings";
    public const int TransparencyValue = 1000;
    public const int SizeValue = 1000;
    public const int BlurValue = 12700;
    public const int Anglevalue = 60000;
    public const int DistanceValue = 12700;
    public const string PivotOptionsTag = "pivotOptions";
    public const string ShowZoneFilterTag = "dropZoneFilter";
    public const string ShowZoneCategoryTag = "dropZoneCategories";
    public const string ShowZoneDataTag = "dropZoneData";
    public const string ShowZoneSeriesTag = "dropZoneSeries";
    public const string ShowZoneVisibleTag = "dropZonesVisible";
    public const string PivotSourceNameTag = "name";
    public const string ChartFormatId = "fmtId";
    public const string AlternateContentTag = "AlternateContent";
    public const string AutoTitleDeletedTag="autoTitleDeleted";
      /// <summary>
      /// Represents the standard Number format attribute.
      /// </summary>
    public const string StandardFormatAttribute = "Standard";    
    public const string formulareference = "formulaRef";
    public const string SqureReference = "sqref";
    public const string fullReference = "fullRef";
    public const string c15tag = "c15";
    public const string xml15web = "http://schemas.microsoft.com/office/drawing/2012/chart";
    public const string Filterseriesuri = "{02D57815-91ED-43cb-92C2-25804820EDAC}";
    public const string FilteredAreaSeries = "filteredAreaSeries";
    public const string Filterbarseries = "filteredBarSeries";
    public const string FilteredLineSeries = "filteredLineSeries";
    public const string FilteredPieSeries = "filteredPieSeries";
    public const string FilteredRadarSeries = "filteredRadarSeries";
    public const string FilteredScatterSeries = "filteredScatterSeries";
    public const string FilteredSurfaceSeries = "filteredSurfaceSeries";
    public const string FilteredBubbleSeries = "filteredBubbleSeries";
    public const string FilteredSeriesTitle = "filteredSeriesTitle";
    public const string FilteredCategoryTitle = "filteredCategoryTitle";
    #endregion

    #region Constructors
    /// <summary>
    /// Prevents a default instance of the ChartConstants class from being created.
    /// </summary>
    private ChartConstants()
    {
    }
    #endregion
  }
}