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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Enumeration represents patterns (pattern names) that are used in Excel 2007.
  /// This enumeration is used to convert pattern from Excel 2003 into 2007
  /// </summary>
  internal enum Excel2007Pattern
  {
    /// <summary>
    /// The fill style is none (no fill). When foreground and/or background colors
    /// are specified, a pattern of 'none' overrides and means the cell has no fill.
    /// </summary>
    none,
    /// <summary>
    /// The fill style is solid. When solid is specified, the foreground color
    /// (fgColor) is the only color rendered, even when a background color
    /// (bgColor) is also specified.
    /// </summary>
    solid,
    /// <summary>
    /// The fill style is medium gray.
    /// </summary>
    mediumGray,
    /// <summary>
    /// The fill style is 'dark gray'.
    /// </summary>
    darkGray,
    /// <summary>
    /// The fill style is light gray.
    /// </summary>
    lightGray,
    /// <summary>
    /// The fill style is dark horizontal.
    /// </summary>
    darkHorizontal,
    /// <summary>
    /// The fill style is 'dark vertical'.
    /// </summary>
    darkVertical,
    /// <summary>
    /// The fill style is 'dark down'.
    /// </summary>
    darkDown,
    /// <summary>
    /// The fill style is 'dark up'.
    /// </summary>
    darkUp,
    /// <summary>
    /// The fill style is 'dark grid'.
    /// </summary>
    darkGrid,
    /// <summary>
    /// The fill style is 'dark trellis'.
    /// </summary>
    darkTrellis,
    /// <summary>
    /// The fill style is light horizontal.
    /// </summary>
    lightHorizontal,
    /// <summary>
    /// The fill style is light vertical.
    /// </summary>
    lightVertical,
    /// <summary>
    /// The fill style is 'light down'.
    /// </summary>
    lightDown,
    /// <summary>
    /// The fill style is light up.
    /// </summary>
    lightUp,
    /// <summary>
    /// The fill style is 'light grid'.
    /// </summary>
    lightGrid,
    /// <summary>
    /// The fill style is 'light trellis'.
    /// </summary>
    lightTrellis,
    /// <summary>
    /// The fill style is grayscale of 0.125 (1/8) value.
    /// </summary>
    gray125,
    /// <summary>
    /// The fill style is grayscale of 0.0625 (1/16) value.
    /// </summary>
    gray0625,
  }
  /// <summary>
  /// The line style of a border in a cell as it is named Excel2007.
  /// </summary>
  internal enum Excel2007BorderLineStyle
  {
    /// <summary>
    /// The line style of a border is dash-dot.
    /// </summary>
    DashDot = 0x9,
    /// <summary>
    /// The line style of a border is dash-dot-dot.
    /// </summary>
    DashDotDot = 0xB,
    /// <summary>
    /// The line style of a border is dashed.
    /// </summary>
    Dashed = 0x3,
    /// <summary>
    /// The line style of a border is dotted.
    /// </summary>
    Dotted = 0x4,
    /// <summary>
    /// The line style of a border is double line.
    /// </summary>
    Double = 0x6,
    /// <summary>
    /// The line style of a border is hairline.
    /// </summary>
    Hair = 0x7,
    /// <summary>
    /// The line style of a border is medium.
    /// </summary>
    Medium = 0x2,
    /// <summary>
    /// The line style of a border is medium dash-dot.
    /// </summary>
    MediumDashDot = 0xA,
    /// <summary>
    /// The line style of a border is medium dash-dot-dot.
    /// </summary>
    MediumDashDotDot = 0xC,
    /// <summary>
    /// The line style of a border is medium dashed.
    /// </summary>
    MediumDashed = 0x8,
    /// <summary>
    /// The line style of a border is none (no border visible).
    /// </summary>
    None = 0x0,
    /// <summary>
    /// The line style of a border is slant-dash-dot.
    /// </summary>
    SlantDashDot = 0xD,
    /// <summary>
    /// The line style of a border is 'thick'.
    /// </summary>
    Thick = 0x5,
    /// <summary>
    /// The line style of a border is thin.
    /// </summary>
    Thin = 0x1,
  }
  /// <summary>
  /// Represents border index.
  /// </summary>
  internal enum Excel2007BorderIndex
  {
    /// <summary>
    /// Represents left border.
    /// </summary>
    left = ExcelBordersIndex.EdgeLeft,
    /// <summary>
    /// Represents right border.
    /// </summary>
    right = ExcelBordersIndex.EdgeRight,
    /// <summary>
    /// Represents top border.
    /// </summary>
    top = ExcelBordersIndex.EdgeTop,
    /// <summary>
    /// Represents bottom border.
    /// </summary>
    bottom = ExcelBordersIndex.EdgeBottom,
    /// <summary>
    /// Represents diagonal border.
    /// </summary>
    diagonal = ExcelBordersIndex.DiagonalUp,
    /// <summary>
    /// Represents no border.
    /// </summary>
    none = ExcelBordersIndex.DiagonalDown,
    /// <summary>
    /// Represents vertical inner border.
    /// </summary>
    vertical = 0xFE,
    /// <summary>
    /// Represents horizontal innver border.
    /// </summary>
    horizontal = 0xFF,
  }
  /// <summary>
  /// This Enumeration values Indicates the Outer Shadow values
  /// </summary>
  public enum Excel2007ChartPresetsOuter
  {
      /// <summary>
      /// Represents No shadow
      /// </summary>
      NoShadow = 0,
      /// <summary>
      /// Represents Outer shadow at Right
      /// </summary>
      OffsetRight = 1,
      /// <summary>
      /// Represents Outer shadow at DiagonalBottomRight
      /// </summary>
      OffsetDiagonalBottomRight = 2,
      /// <summary>
      /// Represents Outer shadow at Bottom
      /// </summary>
      OffsetBottom = 3,
      /// <summary>
      /// Represents Outer shadow at DiagonalTopLeft
      /// </summary>
      OffsetDiagonalTopLeft = 4,
      /// <summary>
      /// Represents Outer shadow at Center
      /// </summary>
      OffsetCenter = 5,
      /// <summary>
      /// Represents Outer shadow at Top
      /// </summary>
      OffsetTop = 6,
      /// <summary>
      /// Represents Outer shadow at Left
      /// </summary>
      OffsetLeft = 7,
      /// <summary>
      /// Represents Outer shadow at DiagonalTopRight
      /// </summary>
      OffsetDiagonalTopRight = 8,
      /// <summary>
      /// Represents Outer shadow at DiagonalBottomLeft
      /// </summary>
      OffsetDiagonalBottomLeft = 9

  }
  /// <summary>
  /// This enumeration value indicates the Inner Shadow Values
  /// </summary>
  public enum Excel2007ChartPresetsInner
  {
      /// <summary>
      /// Represents No shadow(Default)
      /// </summary>
      NoShadow = 0,
      /// <summary>
      /// Represents Inner Shadow at Diagonal Bottom Left
      /// </summary>
      InsideDiagonalBottomLeft = 1,
      /// <summary>
      /// Represents Inner Shadow at Top
      /// </summary>
      InsideTop = 2,
      /// <summary>
      /// Represents Inner Shadow at Right
      /// </summary>
      InsideRight = 3,
      /// <summary>
      /// Represents Inner Shadow at Left
      /// </summary>
      InsideLeft = 4,
      /// <summary>
      /// Represents Inner Shadow at TopRight
      /// </summary>
      InsideDiagonalTopRight = 5,
      /// <summary>
      /// Represents Inner Shadow at Diagonal Bottom Right
      /// </summary>
      InsideDiagonalBottomRight = 6,
      /// <summary>
      /// Represents Inner Shadow at Center
      /// </summary>
      InsideCenter = 7,
      /// <summary>
      /// Represents Inner Shadow at Bottom
      /// </summary>
      InsideBottom = 8,
      /// <summary>
      /// Represents Inner Shadow at Diagonal Top left
      /// </summary>
      InsideDiagonalTopLeft = 9

  }
  /// <summary>
  /// This Enumeration values gives the Perspective shadow Values
  /// </summary>
  public enum Excel2007ChartPresetsPrespective
  {
      /// <summary>
      /// Represents the No Shadow
      /// </summary>
      NoShadow = 0,
      /// <summary>
      /// Represents Perspective Shadow at Diagonal Upper Right
      /// </summary>
      PrespectiveDiagonalUpperRight = 1,
      /// <summary>
      /// Represents Perspective Shadow at Diagonal Lower Right
      /// </summary>
      PrespectiveDiagonalLowerRight = 2,
      /// <summary>
      /// Represents Perspective Shadow at Diagonal Upper Left
      /// </summary>
      PrespectiveDiagonalUpperLeft = 3,
      /// <summary>
      /// Represents Perspective Shadow at Diagonal Lower Left
      /// </summary>
      PrespectiveDiagonalLowerLeft = 4,
      /// <summary>
      /// Represents Perspective Shadow at Below
      /// </summary>
      Below = 5
  }
  /// <summary>
  /// This enumeration value indicates the bevel properties for both Top nad Bottom Bevel
  /// </summary>
  public enum Excel2007ChartBevelProperties
  {
      /// <summary>
      /// Represents No angle
      /// </summary>
      NoAngle = 0,
      /// <summary>
      /// Represents Angle 
      /// </summary>
      Angle = 1,
      /// <summary>
      /// Represents Art Deco
      /// </summary>
      ArtDeco = 2,
      /// <summary>
      /// Represents Circle
      /// </summary>
      Circle = 3,
      /// <summary>
      /// Represents Convex
      /// </summary>
      Convex = 4,
      /// <summary>
      /// Represents Cool Slant
      /// </summary>
      CoolSlant = 5,
      /// <summary>
      /// Represents Cross
      /// </summary>
      Cross = 6,
      /// <summary>
      /// Represents Divot
      /// </summary>
      Divot = 7,
      /// <summary>
      /// Represents Hard Edge
      /// </summary>
      HardEdge = 8,
      /// <summary>
      /// Represents Relaxed Inset
      /// </summary>
      RelaxedInset = 9,
      /// <summary>
      /// Represents Riblet
      /// </summary>
      Riblet = 10,
      /// <summary>
      /// Represents Slope
      /// </summary>
      Slope = 11,
      /// <summary>
      /// Represents Soft round
      /// </summary>
      SoftRound = 12
  }
  /// <summary>
  /// This Enumeration value indicates the Material property values fro the chart
  /// </summary>
  public enum Excel2007ChartMaterialProperties
  {
      /// <summary>
      /// Represents No Material Effect
      /// </summary>
      NoEffect = 0,
      /// <summary>
      /// Represents Matte Material
      /// </summary>
      Matte = 1,
      /// <summary>
      /// Represents WarmMatte Material
      /// </summary>
      WarmMatte = 2,
      /// <summary>
      /// Represents Plastic Material
      /// </summary>
      Plastic = 3,
      /// <summary>
      /// Represents Metal Material
      /// </summary>
      Metal = 4,
      /// <summary>
      /// Represents Dark Edge Material
      /// </summary>
      DarkEdge = 5,
      /// <summary>
      /// Represents Soft Edge Material
      /// </summary>
      SoftEdge = 6,
      /// <summary>
      /// Represents Flat Material
      /// </summary>
      Flat = 7,
      /// <summary>
      /// Represents Wire Frame Material
      /// </summary>
      WireFrame = 8,
      /// <summary>
      /// Represents Powder Material
      /// </summary>
      Powder = 9,
      /// <summary>
      /// Represents Translucent Powder Material
      /// </summary>
      TranslucentPowder = 10,
      /// <summary>
      /// Represents Clear Material
      /// </summary>
      Clear = 11

  }
  /// <summary>
  /// This enumeration value indicates the Lighting properties for the 
  /// Chart
  /// </summary>
  public enum Excel2007ChartLightingProperties
  {
      /// <summary>
      /// Represents ThreePoint Lighting(Default)
      /// </summary>
      ThreePoint = 0,
      /// <summary>
      /// Represents Balance Lighting
      /// </summary>
      Balance = 1,
      /// <summary>
      /// Represents Bright Room  Lighting
      /// </summary>
      BrightRoom = 2,
      /// <summary>
      /// Represents Chilly Lighting
      /// </summary>
      Chilly = 3,
      /// <summary>
      /// Represents Contrasting Lighting
      /// </summary>
      Contrasting = 4,
      /// <summary>
      /// Represents Flat Lighting
      /// </summary>
      Flat = 5,
      /// <summary>
      /// Represents Flood Lighting
      /// </summary>
      Flood = 6,
      /// <summary>
      /// Represents Freezing Lighting
      /// </summary>
      Freezing = 7,
      /// <summary>
      /// Represents Glow Lighting
      /// </summary>
      Glow = 8,
      /// <summary>
      /// Represents Harsh Lighting
      /// </summary>
      Harsh = 9,
      /// <summary>
      /// Represents Morning Lighting
      /// </summary>
      Morning = 10,
      /// <summary>
      /// Represents Soft Lighting
      /// </summary>
      Soft = 11,
      /// <summary>
      /// Represents Sunrise Lighting
      /// </summary>
      Sunrise = 12,
      /// <summary>
      /// Represents Sun Set Lighting
      /// </summary>
      SunSet = 13,
      /// <summary>
      /// Represents TwoPoint Lighting
      /// </summary>
      TwoPoint = 14

  }
  /// <summary>
  /// The enumeration value indicating the portion of Cell Alignment in
  /// a cell format (XF) that is horizontal alignment.
  /// </summary>
  internal enum Excel2007HAlign
  {
    /// <summary>
    /// The horizontal alignment is centered, meaning the text is centered across the cell.
    /// </summary>
    center = ExcelHAlign.HAlignCenter,
    /// <summary>
    /// The horizontal alignment is centered across multiple cells.
    /// </summary>
    centerContinuous = ExcelHAlign.HAlignCenterAcrossSelection,
    /// <summary>
    /// Indicates that each 'word' in each line of text inside the cell is evenly
    /// distributed across the width of the cell, with flush right and left margins.
    /// When there is also an indent value to apply, both the left and right side
    /// of the cell are padded by the indent value.
    /// </summary>
    distributed = ExcelHAlign.HAlignDistributed,
    /// <summary>
    /// Indicates that the value of the cell should be filled across the entire
    /// width of the cell.
    /// </summary>
    fill = ExcelHAlign.HAlignFill,
    /// <summary>
    /// The horizontal alignment is general-aligned. Text data is left-aligned.
    /// Numbers, dates, and times are rightaligned. Boolean types are centered.
    /// Changing the alignment does not change the type of data.
    /// </summary>
    general = ExcelHAlign.HAlignGeneral,
    /// <summary>
    /// The horizontal alignment is justified (flush left and right). For each
    /// line of text, aligns each line of the wrapped text in a cell to the right
    /// and left (except the last line). If no single line of text wraps in the
    /// cell, then the text is not justified.
    /// </summary>
    justify = ExcelHAlign.HAlignJustify,
    /// <summary>
    /// The horizontal alignment is left-aligned, even in Right-to-Left mode.
    /// Aligns contents at the left edge of the cell. If an indent amount is
    /// specified, the contents of the cell is indented from the left by the
    /// specified number of character spaces. The character spaces are based
    /// on the default font and font size for the workbook.
    /// </summary>
    left = ExcelHAlign.HAlignLeft,
    /// <summary>
    /// The horizontal alignment is right-aligned, meaning that cell contents
    /// are aligned at the right edge of the cell, even in Right-to-Left mode.
    /// </summary>
    right = ExcelHAlign.HAlignRight,
  }
  /// <summary>
  /// This enumeration value indicates the type of vertical alignment for a cell.
  /// </summary>
  internal enum Excel2007VAlign
  {
    /// <summary>
    /// The vertical alignment is aligned-to-bottom.
    /// </summary>
    bottom = ExcelVAlign.VAlignBottom,
    /// <summary>
    /// The vertical alignment is centered across the height of the cell.
    /// </summary>
    center = ExcelVAlign.VAlignCenter,
    /// <summary>
    /// When text direction is horizontal: the vertical alignment of lines of text
    /// is distributed vertically, where each line of text inside the cell is evenly
    /// distributed across the height of the cell, with flush top and bottom margins.
    /// When text direction is vertical: behaves exactly as distributed horizontal
    /// alignment. The first words in a line of text (appearing at the top of the cell)
    /// are flush with the top edge of the cell, and the last words of a line of text
    /// are flush with the bottom edge of the cell, and the line of text is distributed
    /// evenly from top to bottom.
    /// </summary>
    distributed = ExcelVAlign.VAlignDistributed,
    /// <summary>
    /// When text direction is horizontal: the vertical alignment of lines of text
    /// is distributed vertically, where each line of text inside the cell is evenly
    /// distributed across the height of the cell, with flush top and bottom margins.
    /// When text direction is vertical: similar behavior as horizontal justification.
    /// The alignment is justified (flush top and bottom in this case). For each line
    /// of text, each line of the wrapped text in a cell is aligned to the top and
    /// bottom (except the last line). If no single line of text wraps in the cell,
    /// then the text is not justified.
    /// </summary>
    justify = ExcelVAlign.VAlignJustify,
    /// <summary>
    /// The vertical alignment is aligned-to-top.
    /// </summary>
    top = ExcelVAlign.VAlignTop,
  }
  /// <summary>
  /// Enumeration which controls visibility of worksheet in Excel.
  /// </summary>
  internal enum Worksheet2007Visibility
  {
    /// <summary>
    /// Worksheet is visible to the user.
    /// </summary>
    Visible = WorksheetVisibility.Visible,
    /// <summary>
    /// Worksheet is hidden for the user.
    /// </summary>
    Hidden = WorksheetVisibility.Hidden,
    /// <summary>
    /// The strong hidden flag can only be set and cleared with a Visual Basic 
    /// macro. It is not possible to make such a sheet visible via the user interface.
    /// </summary>
    VeryHidden = WorksheetVisibility.StrongHidden,
  }
  /// <summary>
  /// This simple type specifies the possible styles of radar chart.
  /// </summary>
  internal enum Excel2007RadarStyle
  {
    /// <summary>
    /// Specifies that the radar chart shall have lines but no markers and no fill.
    /// </summary>
    standard = ExcelChartType.Radar,
    /// <summary>
    /// Specifies that the radar chart shall have lines and markers but no fill.
    /// </summary>
    marker = ExcelChartType.Radar_Markers,
    /// <summary>
    /// Specifies that the radar chart shall be filled and have lines but no markers.
    /// </summary>
    filled = ExcelChartType.Radar_Filled,
  }
  /// <summary>
  /// This simple type specifies the possible styles of scatter chart.
  /// </summary>
  internal enum Excel2007ScatterStyle
  {
    /// <summary>
    /// Specifies the points on the scatter chart shall not be connected with
    /// lines and markers shall be drawn.
    /// </summary>
    marker = ExcelChartType.Scatter_Markers,
    /// <summary>
    /// Specifies the the points on the scatter chart shall be connected with
    /// smoothed lines and markers shall be drawn.
    /// </summary>
    smoothMarker = ExcelChartType.Scatter_SmoothedLine_Markers,
    /// <summary>
    /// Specifies the the points on the scatter chart shall be connected with
    /// smoothed lines and markers shall not be drawn.
    /// </summary>
    smooth = ExcelChartType.Scatter_SmoothedLine,
    /// <summary>
    /// Specifies the points on the scatter chart shall be connected with
    /// straight lines and markers shall be drawn.
    /// </summary>
    lineMarker = ExcelChartType.Scatter_Line_Markers,
    /// <summary>
    /// Specifies the points on the scatter chart shall be connected with
    /// straight lines but markers shall not be drawn.
    /// </summary>
    line = ExcelChartType.Scatter_Line,
  }
  /// <summary>
  /// Enumeration limits values which can be set by user.
  /// </summary>
  internal enum Excel2007SplitType
  {
    /// <summary>
    /// Represents the Position split type.
    /// </summary>
    pos = ExcelSplitType.Position,
    /// <summary>
    /// Represents the Value split type.
    /// </summary>
    val = ExcelSplitType.Value,
    /// <summary>
    /// Represents the Percent split type.
    /// </summary>
    percent = ExcelSplitType.Percent,
    /// <summary>
    /// Represents the Custom split type.
    /// </summary>
    cust = ExcelSplitType.Custom,
  }
  /// <summary>
  /// 
  /// </summary>
  internal enum Excel2007DataLabelPos
  {
    /// <summary>
    /// Specifies that data labels shall be displayed below the data marker.
    /// </summary>
    b = ExcelDataLabelPosition.Below,
    /// <summary>
    /// Specifies that data labels shall be displayed in the best position.
    /// </summary>
    bestFit = ExcelDataLabelPosition.BestFit,
    /// <summary>
    /// Specifies that data labels shall be displayed centered on the data marker.
    /// </summary>
    ctr = ExcelDataLabelPosition.Center,
    /// <summary>
    /// Specifies that data labels shall be displayed inside the base of the data marker.
    /// </summary>
    inBase = ExcelDataLabelPosition.OutsideBase,
    /// <summary>
    /// Specifies that data labels shall be displayed inside the end of the data marker.
    /// </summary>
    inEnd = ExcelDataLabelPosition.Inside,
    /// <summary>
    /// Specifies that data labels shall be displayed to the left of the data marker.
    /// </summary>
    l = ExcelDataLabelPosition.Left,
    /// <summary>
    /// Specifies that data labels shall be displayed outside the end of the data marker.
    /// </summary>
    outEnd = ExcelDataLabelPosition.Outside,
    /// <summary>
    /// Specifies that data labels shall be displayed to the right of the data marker.
    /// </summary>
    r = ExcelDataLabelPosition.Right,
    /// <summary>
    /// Specifies that data labels shall be displayed above the data marker.}
    /// </summary>
    t = ExcelDataLabelPosition.Above,
  }
  /// <summary>
  /// Enumeration of the marker types for Chart lines in Excel.
  /// </summary>
  internal enum Excel2007ChartMarkerType
  {
    /// <summary>
    ///Represents the None option for the marker type.
    /// </summary>
    none = 0,
    /// <summary>
    ///Represents the square style in the custom marker option for Chart lines.
    /// </summary>
    square = ExcelChartMarkerType.Square,
    /// <summary>
    ///Represents the diamond style in the custom marker option for Chart lines.
    /// </summary>
    diamond = ExcelChartMarkerType.Diamond,
    /// <summary>
    ///Represents the Triangle style in the custom marker option for Chart lines.
    /// </summary>
    triangle = ExcelChartMarkerType.Triangle,
    /// <summary>
    ///Represents the X style in the custom marker option for Chart lines.
    /// </summary>
    x = ExcelChartMarkerType.X,
    /// <summary>
    ///Represents the Star style in the custom marker option for Chart lines.
    /// </summary>
    star = ExcelChartMarkerType.Star,
    /// <summary>
    ///Represents the Dow Jones style in the custom marker option for Chart lines.
    /// </summary>
    dot = ExcelChartMarkerType.DowJones,
    /// <summary>
    ///Represents the Standard Deviation style in the custom marker option for Chart lines.
    /// </summary>
    dash = ExcelChartMarkerType.StandardDeviation,
    /// <summary>
    ///Represents the Circle style in the custom marker option for Chart lines.
    /// </summary>
    circle = ExcelChartMarkerType.Circle,
    /// <summary>
    ///Represents the Plus style in the custom marker option for Chart lines.
    /// </summary>
    plus = ExcelChartMarkerType.PlusSign,
  }

  /// <summary>
  /// Represents trend line values.
  /// </summary>
  internal enum Excel2007TrendlineType
  {
    /// <summary>
    /// Represents Exponential trend line type.
    /// </summary>
    exp = ExcelTrendLineType.Exponential,
    /// <summary>
    /// Represents Linear trend line type.
    /// </summary>
    linear = ExcelTrendLineType.Linear,
    /// <summary>
    /// Represents Logarithmic trend line type.
    /// </summary>
    log = ExcelTrendLineType.Logarithmic,
    /// <summary>
    /// Represents Moving average trend line type.
    /// </summary>
    movingAvg = ExcelTrendLineType.Moving_Average,
    /// <summary>
    /// Represents Polynomial trend line type.
    /// </summary>
    poly = ExcelTrendLineType.Polynomial,
    /// <summary>
    /// Represents Power trend line type.
    /// </summary>
    power = ExcelTrendLineType.Power,
  }
  /// <summary>
  /// Error-bar type.
  /// </summary>
  internal enum Excel2007ErrorBarType
  {
    /// <summary>
    /// Represents the Percentage error-bar source type.
    /// </summary>
    percentage = ExcelErrorBarType.Percentage,
    /// <summary>
    /// Represents the FixedValue error-bar source type.
    /// </summary>
    fixedVal = ExcelErrorBarType.Fixed,
    /// <summary>
    /// Represents the StandardDeviation error-bar source type.
    /// </summary>
    stdDev = ExcelErrorBarType.StandardDeviation,
    /// <summary>
    /// Represents the Custom error-bar source type.
    /// </summary>
    cust = ExcelErrorBarType.Custom,
    /// <summary>
    /// Represents the StandardError error-bar source type.
    /// </summary>
    stdErr = ExcelErrorBarType.StandardError,
  }
  /// <summary>
  /// Enumeration of the legend placement for Charts in Excel.
  /// </summary>
  internal enum Excel2007LegendPosition
  {
    /// <summary>
    ///Represents the bottom option.
    /// </summary>
    b = ExcelLegendPosition.Bottom,
    /// <summary>
    ///Represents the Corner option.
    /// </summary>
    tr = ExcelLegendPosition.Corner,
    /// <summary>
    ///Represents the Top option.
    /// </summary>
    t = ExcelLegendPosition.Top,
    /// <summary>
    ///Represents the Right option.
    /// </summary>
    r = ExcelLegendPosition.Right,
    /// <summary>
    ///Represents the Left option.
    /// </summary>
    l = ExcelLegendPosition.Left,
  }
  /// <summary>
  /// This enumeration specifies the possible ways to display blanks.
  /// </summary>
  internal enum Excel2007ChartPlotEmpty
  {
    /// <summary>
    /// Specifies that blank values shall be left as a gap.
    /// </summary>
    gap = ExcelChartPlotEmpty.NotPlotted,
    /// <summary>
    /// Specifies that blank values shall be treated as zero.
    /// </summary>
    zero = ExcelChartPlotEmpty.Zero,
    /// <summary>
    /// Specifies that blank values shall be spanned with a line.
    /// </summary>
    span = ExcelChartPlotEmpty.Interpolated,
  }
  internal enum Excel2007GradientPattern
  {
    /// <summary>
    /// Represents 5% gradient pattern
    /// </summary>
    pct5 = ExcelGradientPattern.Pat_5_Percent,
    /// <summary>
    /// Represents 10% gradient pattern
    /// </summary>
    pct10 = ExcelGradientPattern.Pat_10_Percent,
    /// <summary>
    /// Represents 20% gradient pattern
    /// </summary>
    pct20 = ExcelGradientPattern.Pat_20_Percent,
    /// <summary>
    /// Represents 25% gradient pattern
    /// </summary>
    pct25 = ExcelGradientPattern.Pat_25_Percent,
    /// <summary>
    /// Represents 30% gradient pattern
    /// </summary>
    pct30 = ExcelGradientPattern.Pat_30_Percent,
    /// <summary>
    /// Represents 40% gradient pattern
    /// </summary>
    pct40 = ExcelGradientPattern.Pat_40_Percent,
    /// <summary>
    /// Represents 50% gradient pattern
    /// </summary>
    pct50 = ExcelGradientPattern.Pat_50_Percent,
    /// <summary>
    /// Represents 60% gradient pattern
    /// </summary>
    pct60 = ExcelGradientPattern.Pat_60_Percent,
    /// <summary>
    /// Represents 70% gradient pattern
    /// </summary>
    pct70 = ExcelGradientPattern.Pat_70_Percent,
    /// <summary>
    /// Represents 75% gradient pattern
    /// </summary>
    pct75 = ExcelGradientPattern.Pat_75_Percent,
    /// <summary>
    /// Represents 80% gradient pattern
    /// </summary>
    pct80 = ExcelGradientPattern.Pat_80_Percent,
    /// <summary>
    /// Represents 90% gradient pattern
    /// </summary>
    pct90 = ExcelGradientPattern.Pat_90_Percent,
    /// <summary>
    /// Represents Dark Downward Diagonal gradient pattern
    /// </summary>
    dkDnDiag = ExcelGradientPattern.Pat_Dark_Downward_Diagonal,
    /// <summary>
    /// Represents Dark Horizontal gradient pattern
    /// </summary>
    dkHorz = ExcelGradientPattern.Pat_Dark_Horizontal,
    /// <summary>
    /// Represents Dark Upward Diagonal gradient pattern
    /// </summary>
    dkUpDiag = ExcelGradientPattern.Pat_Dark_Upward_Diagonal,
    /// <summary>
    /// Represents Dark Vertical gradient pattern
    /// </summary>
    dkVert = ExcelGradientPattern.Pat_Dark_Vertical,
    /// <summary>
    /// Represents Dashed Downward Diagonal gradient pattern
    /// </summary>
    dashDnDiag = ExcelGradientPattern.Pat_Dashed_Downward_Diagonal,
    /// <summary>
    /// Represents Dashed Horizontal gradient pattern
    /// </summary>
    dashHorz = ExcelGradientPattern.Pat_Dashed_Horizontal,
    /// <summary>
    /// Represents Dashed Upward Diagonal gradient pattern
    /// </summary>
    dashUpDiag = ExcelGradientPattern.Pat_Dashed_Upward_Diagonal,
    /// <summary>
    /// Represents Dashed Vertical gradient pattern
    /// </summary>
    dashVert = ExcelGradientPattern.Pat_Dashed_Vertical,
    /// <summary>
    /// Represents Diagonal Brick gradient pattern
    /// </summary>
    diagBrick = ExcelGradientPattern.Pat_Diagonal_Brick,
    /// <summary>
    /// Represents Divot gradient pattern
    /// </summary>
    divot = ExcelGradientPattern.Pat_Divot,
    /// <summary>
    /// Represents Dotted Diamond gradient pattern
    /// </summary>
    dotDmnd = ExcelGradientPattern.Pat_Dotted_Diamond,
    /// <summary>
    /// Represents Dotted Grid gradient pattern
    /// </summary>
    dotGrid = ExcelGradientPattern.Pat_Dotted_Grid,
    /// <summary>
    /// Represents Horizontal Brick gradient pattern
    /// </summary>
    horzBrick = ExcelGradientPattern.Pat_Horizontal_Brick,
    /// <summary>
    /// Represents Large Checker Board gradient pattern
    /// </summary>
    lgCheck = ExcelGradientPattern.Pat_Large_Checker_Board,
    /// <summary>
    /// Represents Large Confetti gradient pattern
    /// </summary>
    lgConfetti = ExcelGradientPattern.Pat_Large_Confetti,
    /// <summary>
    /// Represents Large Grid gradient pattern
    /// </summary>
    lgGrid = ExcelGradientPattern.Pat_Large_Grid,
    /// <summary>
    /// Represents Light Downward Diagonal gradient pattern
    /// </summary>
    ltDnDiag = ExcelGradientPattern.Pat_Light_Downward_Diagonal,
    /// <summary>
    /// Represents Light Horizontal gradient pattern
    /// </summary>
    ltHorz = ExcelGradientPattern.Pat_Light_Horizontal,
    /// <summary>
    /// Represents Light Upward Diagonal gradient pattern
    /// </summary>
    ltUpDiag = ExcelGradientPattern.Pat_Light_Upward_Diagonal,
    /// <summary>
    /// Represents Light Vertical gradient pattern
    /// </summary>
    ltVert = ExcelGradientPattern.Pat_Light_Vertical,
    //Pat_Mixed = ExcelGradientPattern.-2,	
    /// <summary>
    /// Represents Narrow Horizontal gradient pattern
    /// </summary>
    narHorz = ExcelGradientPattern.Pat_Narrow_Horizontal,
    /// <summary>
    /// Represents Narrow Vertical gradient pattern
    /// </summary>
    narVert = ExcelGradientPattern.Pat_Narrow_Vertical,
    /// <summary>
    /// Represents Outlined Diamond gradient pattern
    /// </summary>
    openDmnd = ExcelGradientPattern.Pat_Outlined_Diamond,
    /// <summary>
    /// Represents Plaid gradient pattern
    /// </summary>
    plaid = ExcelGradientPattern.Pat_Plaid,
    /// <summary>
    /// Represents Shingle gradient pattern
    /// </summary>
    shingle = ExcelGradientPattern.Pat_Shingle,
    /// <summary>
    /// Represents Small Checker Board gradient pattern
    /// </summary>
    smCheck = ExcelGradientPattern.Pat_Small_Checker_Board,
    /// <summary>
    /// Represents Small Confetti gradient pattern
    /// </summary>
    smConfetti = ExcelGradientPattern.Pat_Small_Confetti,
    /// <summary>
    /// Represents Small Grid gradient pattern
    /// </summary>
    smGrid = ExcelGradientPattern.Pat_Small_Grid,
    /// <summary>
    /// Represents Solid Diamond gradient pattern
    /// </summary>
    solidDmnd = ExcelGradientPattern.Pat_Solid_Diamond,
    /// <summary>
    /// Represents Sphere gradient pattern
    /// </summary>
    sphere = ExcelGradientPattern.Pat_Sphere,
    /// <summary>
    /// Represents Trellis gradient pattern
    /// </summary>
    trellis = ExcelGradientPattern.Pat_Trellis,
    /// <summary>
    /// Represents Wave gradient pattern
    /// </summary>
    wave = ExcelGradientPattern.Pat_Wave,
    /// <summary>
    /// Represents Weave gradient pattern
    /// </summary>
    weave = ExcelGradientPattern.Pat_Weave,
    /// <summary>
    /// Represents Wide Downward Diagonal gradient pattern
    /// </summary>
    wdDnDiag = ExcelGradientPattern.Pat_Wide_Downward_Diagonal,
    /// <summary>
    /// Represents Wide Upward Diagonal gradient pattern
    /// </summary>
    wdUpDiag = ExcelGradientPattern.Pat_Wide_Upward_Diagonal,
    /// <summary>
    /// Represents Zig Zag gradient pattern
    /// </summary>
    zigZag = ExcelGradientPattern.Pat_Zig_Zag
  }
  /// <summary>
  /// Represents excel 2007 chart uint to display.
  /// </summary>
  internal enum Excel2007ChartDisplayUnit
  {
    /// <summary>
    /// Represents Hundreds display Unit
    /// </summary>
    hundreds = ExcelChartDisplayUnit.Hundreds,
    /// <summary>
    /// Represents Thousands display Unit
    /// </summary>
    thousands = ExcelChartDisplayUnit.Thousands,
    /// <summary>
    /// Represents TenThousands display Unit
    /// </summary>
    tenThousands = ExcelChartDisplayUnit.TenThousands,
    /// <summary>
    /// Represents HundredThousands display Unit
    /// </summary>
    hundredThousands = ExcelChartDisplayUnit.HundredThousands,
    /// <summary>
    /// Represents Millions display Unit
    /// </summary>
    millions = ExcelChartDisplayUnit.Millions,
    /// <summary>
    /// Represents TenMillions display Unit
    /// </summary>
    tenMillions = ExcelChartDisplayUnit.TenMillions,
    /// <summary>
    /// Represents HundredMillions display Unit
    /// </summary>
    hundredMillions = ExcelChartDisplayUnit.HundredMillions,
    /// <summary>
    /// Represents ThousandMillions display Unit
    /// </summary>
    billions = ExcelChartDisplayUnit.ThousandMillions,
    /// <summary>
    /// Represents MillionMillions display Unit
    /// </summary>
    trillions = ExcelChartDisplayUnit.MillionMillions,
  }
  /// <summary>
  /// Possible types for row data storage.
  /// </summary>
  public enum ExcelDataProviderType
  {
    /// <summary>
    /// Uses Win32 API and Marshal class calls to allocate and work with memory blocks.
    /// </summary>
    Native,
    /// <summary>
    /// Uses Win32 API and unsafe blocks to allocate and work with memory blocks.
    /// </summary>
    Unsafe,
    /// <summary>
    /// Uses managed byte array and other managed functions to work with memory blocks.
    /// This method is slowest, but it is the only one suitable for medium trust mode.
    /// </summary>
    ByteArray,
  }
  /// <summary>
  /// Possible icon set types.
  /// </summary>
  public enum ExcelIconSetType
  {
    /// <summary>
    /// 3 arrows icon set.
    /// </summary>
    ThreeArrows = 0x00,
    /// <summary>
    /// 3 gray arrows icon set.
    /// </summary>
    ThreeArrowsGray = 0x01,
    /// <summary>
    /// 3 flags icon set.
    /// </summary>
    ThreeFlags = 0x02,
    /// <summary>
    /// 3 traffic lights icon set (#1).
    /// </summary>
    ThreeTrafficLights1 = 0x03,
    /// <summary>
    /// 3 traffic lights icon set with thick black border.
    /// </summary>
    ThreeTrafficLights2 = 0x04,
    /// <summary>
    /// 3 signs icon set.
    /// </summary>
    ThreeSigns = 0x05,
    /// <summary>
    /// 3 symbols icon set.
    /// </summary>
    ThreeSymbols = 0x06,
    /// <summary>
    /// 3 Symbols icon set.
    /// </summary>
    ThreeSymbols2 = 0x07,
    /// <summary>
    /// 4 arrows icon set.
    /// </summary>
    FourArrows = 0x08,
    /// <summary>
    /// 4 gray arrows icon set.
    /// </summary>
    FourArrowsGray = 0x09,
    /// <summary>
    /// 4 'red to black' icon set.
    /// </summary>
    FourRedToBlack = 0x0A,
    /// <summary>
    /// 4 ratings icon set.
    /// </summary>
    FourRating = 0x0B,
    /// <summary>
    /// 4 traffic lights icon set.
    /// </summary>
    FourTrafficLights=0x0C,
    /// <summary>
    /// 5 arrows icon set.
    /// </summary>
    FiveArrows = 0x0D,
    /// <summary>
    /// 5 gray arrows icon set.
    /// </summary>
    FiveArrowsGray = 0x0E,
    /// <summary>
    /// 5 rating icon set.
    /// </summary>
    FiveRating = 0x0F,
    /// <summary>
    /// 5 quarters icon set.
    /// </summary>
    FiveQuarters = 0x10,
  }
  /// <summary>
  /// If there is vertical text, determines what type of vertical text is going to be used.
  /// </summary>
  public enum Excel2007TextRotation
  {
    /// <summary>
    /// Horizontal text. This should be default.
    /// </summary>
    horz = ExcelTextRotation.LeftToRight,
    /// <summary>
    /// Determines if all of the text is vertical ("one letter on top of another").
    /// </summary>
    wordArtVert = ExcelTextRotation.TopToBottom,
    /// <summary>
    /// Determines if all of the text is vertical orientation (each line is 90
    /// degrees rotated clockwise, so it goes from top to bottom; each next
    /// line is to the left from the previous one).
    /// </summary>
    vert = ExcelTextRotation.Clockwise,
    /// <summary>
    /// Determines if all of the text is vertical orientation (each line is 270
    /// degrees rotated clockwise, so it goes from bottom to top; each next line
    /// is to the right from the previous one).
    /// </summary>
    vert270 = ExcelTextRotation.CounterClockwise,
    /// <summary>
    /// A special version of vertical text, where some fonts are displayed as if rotated
    /// by 90 degrees while some fonts (mostly East Asian) are displayed vertical.
    /// </summary>
    eaVert,
    /// <summary>
    /// A special version of vertical text, where some fonts are displayed as if
    /// rotated by 90 degrees while some fonts (mostly East Asian) are displayed
    /// vertical. The difference between this and the eastAsianVertical is the
    /// text flows top down then LEFT RIGHT, instead of RIGHT LEFT.
    /// </summary>
    mongolianVert,
    /// <summary>
    /// Specifies that vertical WordArt should be shown from right to left rather than left to right.
    /// </summary>
    wordArtVertRtl,
  }
  /// <summary>
  /// Enumeration to align the excel comment Horizontally.
  /// </summary>
  public enum Excel2007CommentHAlign
  {
    /// <summary>
    /// Represents the Left comment align.
    /// </summary>
    l = ExcelCommentHAlign.Left,
    /// <summary>
    /// Represents the Center comment align.
    /// </summary>
    ctr = ExcelCommentHAlign.Center,
    /// <summary>
    /// Represents the Right comment align.
    /// </summary>
    r = ExcelCommentHAlign.Right,
    /// <summary>
    /// Represents the Justified comment align.
    /// </summary>
    just = ExcelCommentHAlign.Justified,
    /// <summary>
    /// Represents the Distributed comment align.
    /// </summary>
    dist = ExcelCommentHAlign.Distributed,
  }
  /// <summary>
  /// This type specifies a list of available anchoring types for text.
  /// </summary>
  public enum Excel2007CommentVAlign
  {
    /// <summary>
    /// Anchor the text at the top of the bounding rectangle.
    /// </summary>
    t = ExcelCommentVAlign.Top,
    /// <summary>
    /// Anchor the text at the middle of the bounding rectangle.
    /// </summary>
    ctr = ExcelCommentVAlign.Center,
    /// <summary>
    /// Anchor the text at the bottom of the bounding rectangle.
    /// </summary>
    b = ExcelCommentVAlign.Bottom,
    /// <summary>
    /// Anchor the text so that it is justified vertically.
    /// </summary>
    just = ExcelCommentVAlign.Justify,
    /// <summary>
    /// Anchor the text so that it is distributed vertically.
    /// </summary>
    dist = ExcelCommentVAlign.Distributed,
  }
  /// <summary>
  /// Represents Excel 2007 Text alignment.
  /// </summary>
  public enum Excel2007TextAlign
  {
    /// <summary>
    /// Align text to the left margin.
    /// </summary>
    l = ExcelCommentHAlign.Left,
    /// <summary>
    /// Align text in the center.
    /// </summary>
    ctr = ExcelCommentHAlign.Center,
    /// <summary>
    /// Align text to the right margin.
    /// </summary>
    r = ExcelCommentHAlign.Right,
    /// <summary>
    /// Align text so that it is justified across the whole line.
    /// </summary>
    just = ExcelCommentHAlign.Justified,
    /// <summary>
    /// Aligns the text with an adjusted kashida length for Arabic text.
    /// </summary>
    justLow,
    /// <summary>
    /// Distributes Thai text specially, because each character is treated as a word.
    /// </summary>
    thaiDist,
    /// <summary>
    /// Distributes the text words across an entire text line.
    /// </summary>
    dist = ExcelCommentHAlign.Distributed,
  }
  /// <summary>
  /// Represents line style.
  /// </summary>
  public enum Excel2007ShapeLineStyle
  {
    /// <summary>
    /// Represents single line style.
    /// </summary>
    sng = ExcelShapeLineStyle.Line_Single,
    /// <summary>
    /// Represents thin thin line style.
    /// </summary>
    dbl = ExcelShapeLineStyle.Line_Thin_Thin,
    /// <summary>
    /// Represents thin thick line style.
    /// </summary>
    thinThick = ExcelShapeLineStyle.Line_Thin_Thick,
    /// <summary>
    /// Represents thick thin line style.
    /// </summary>
    thickThin = ExcelShapeLineStyle.Line_Thick_Thin,
    /// <summary>
    /// Represents thick between thin line style.
    /// </summary>
    tri = ExcelShapeLineStyle.Line_Thick_Between_Thin,
  }
  /// <summary>
  /// Specifies the type of calculation in the Totals row of a list column.
  /// </summary>
  public enum ExcelTotalsCalculation
  {
    /// <summary>
    /// No calculation.
    /// </summary>
    None,
    /// <summary>
    /// Sum of all values in the list column.
    /// </summary>
    Sum = 109,
    /// <summary>
    /// Average.
    /// </summary>
    Average = 101,
    /// <summary>
    /// Count of non-empty cells.
    /// </summary>
    Count = 103,
    /// <summary>
    /// Count of cells with numeric values.
    /// </summary>
    CountNums = 102,
    /// <summary>
    /// Minimum value in the list.
    /// </summary>
    Min = 105,
    /// <summary>
    /// Standard deviation value.
    /// </summary>
    StdDev = 107,
    /// <summary>
    /// Variable.
    /// </summary>
    Var = 110,
    /// <summary>
    /// Maximum value in the list.
    /// </summary>
    Max = 104,
    /// <summary>
    /// Custom formula
    /// </summary>
    Custom = 106,
  }
  /// <summary>
  /// Represents functions added in Excel 2007.
  /// </summary>
  public enum Excel2007Function
  {
    /// <summary>
    /// Returns a key performance indicator (KPI) name, property, and measure, and displays the name and property in the cell. A KPI is a quantifiable measurement, such as monthly gross profit or quarterly employee turnover, used to monitor an organization's performance.
    /// </summary>
    CUBEKPIMEMBER = 0x8000,
    /// <summary>
    /// Returns a member or tuple in a cube hierarchy. Use to validate that the member or tuple exists in the cube.
    /// </summary>
    CUBEMEMBER,
    /// <summary>
    /// Returns the value of a member property in the cube. Use to validate that a member name exists within the cube and to return the specified property for this member.
    /// </summary>
    CUBEMEMBERPROPERTY,
    /// <summary>
    /// Returns the nth, or ranked, member in a set. Use to return one or more elements in a set, such as the top sales performer or top 10 students.
    /// </summary>
    CUBERANKEDMEMBER,
    /// <summary>
    /// Defines a calculated set of members or tuples by sending a set expression to the cube on the server, which creates the set, and then returns that set to Microsoft Office Excel.
    /// </summary>
    CUBESET,
    /// <summary>
    /// Returns the number of items in a set.
    /// </summary>
    CUBESETCOUNT,
    /// <summary>
    /// Returns an aggregated value from a cube.
    /// </summary>
    CUBEVALUE,
    /// <summary>
    /// Returns the modified Bessel function In(x).
    /// </summary>
    BESSELI,
    /// <summary>
    /// Returns the Bessel function Jn(x).
    /// </summary>
    BESSELJ,
    /// <summary>
    /// Returns the modified Bessel function Kn(x).
    /// </summary>
    BESSELK,
    /// <summary>
    /// Returns the Bessel function Yn(x).
    /// </summary>
    BESSELY,
    /// <summary>
    /// Converts a binary number to decimal.
    /// </summary>
    BIN2DEC,
    /// <summary>
    /// Converts a binary number to hexadecimal.
    /// </summary>
    BIN2HEX,
    /// <summary>
    /// Converts a binary number to octal.
    /// </summary>
    BIN2OCT,
    /// <summary>
    /// Converts real and imaginary coefficients into a complex number.
    /// </summary>
    COMPLEX,
    /// <summary>
    /// Converts a number from one measurement system to another.
    /// </summary>
    CONVERT,
    /// <summary>
    /// Converts a decimal number to binary.
    /// </summary>
    DEC2BIN,
    /// <summary>
    /// Converts a decimal number to hexadecimal.
    /// </summary>
    DEC2HEX,
    /// <summary>
    /// Converts a decimal number to octal.
    /// </summary>
    DEC2OCT,
    /// <summary>
    /// Tests whether two values are equal.
    /// </summary>
    DELTA,
    /// <summary>
    /// Returns the error function.
    /// </summary>
    ERF,
    /// <summary>
    /// Returns the complementary error function.
    /// </summary>
    ERFC,
    /// <summary>
    /// Tests whether a number is greater than a threshold value.
    /// </summary>
    GESTEP,
    /// <summary>
    /// Converts a hexadecimal number to binary.
    /// </summary>
    HEX2BIN,
    /// <summary>
    /// Converts a hexadecimal number to decimal.
    /// </summary>
    HEX2DEC,
    /// <summary>
    /// Converts a hexadecimal number to octal.
    /// </summary>
    HEX2OCT,
    /// <summary>
    /// Returns the absolute value (modulus) of a complex number.
    /// </summary>
    IMABS,
    /// <summary>
    /// Returns the imaginary coefficient of a complex number.
    /// </summary>
    IMAGINARY,
    /// <summary>
    /// Returns the argument theta, an angle expressed in radians.
    /// </summary>
    IMARGUMENT,
    /// <summary>
    /// Returns the complex conjugate of a complex number.
    /// </summary>
    IMCONJUGATE,
    /// <summary>
    /// Returns the cosine of a complex number
    /// </summary>
    IMCOS,
    /// <summary>
    /// Returns the quotient of two complex numbers.
    /// </summary>
    IMDIV,
    /// <summary>
    /// Returns the exponential of a complex number.
    /// </summary>
    IMEXP,
    /// <summary>
    /// Returns the natural logarithm of a complex number.
    /// </summary>
    IMLN,
    /// <summary>
    /// Returns the base-10 logarithm of a complex number.
    /// </summary>
    IMLOG10,
    /// <summary>
    /// Returns the base-2 logarithm of a complex number.
    /// </summary>
    IMLOG2,
    /// <summary>
    /// Returns a complex number raised to an integer power.
    /// </summary>
    IMPOWER,
    /// <summary>
    /// Returns the product of from 2 to 29 complex numbers
    /// </summary>
    IMPRODUCT,
    /// <summary>
    /// Returns the real coefficient of a complex number
    /// </summary>
    IMREAL,
    /// <summary>
    /// Returns the sine of a complex number
    /// </summary>
    IMSIN,
    /// <summary>
    /// Returns the square root of a complex number
    /// </summary>
    IMSQRT,
    /// <summary>
    /// Returns the difference between two complex numbers
    /// </summary>
    IMSUB,
    /// <summary>
    /// Returns the sum of complex numbers.
    /// </summary>
    IMSUM,
    /// <summary>
    /// Converts an octal number to binary.
    /// </summary>
    OCT2BIN,
    /// <summary>
    /// Converts an octal number to decimal.
    /// </summary>
    OCT2DEC,
    /// <summary>
    /// Converts an octal number to hexadecimal.
    /// </summary>
    OCT2HEX,
    ///// <summary>
    ///// Converts a number to euros, converts a number from euros to a euro member currency, or converts a number from one euro member currency to another by using the euro as an intermediary (triangulation).
    ///// </summary>
    //EUROCONVERT,
    ///// <summary>
    ///// Connects with an external data source and runs a query from a worksheet, then returns the result as an array without the need for macro programming.
    ///// </summary>
    //[Description( "SQL.REQUEST" )]
    //SQLREQUEST,
    /// <summary>
    /// Adds the cells in a range that meet multiple criteria
    /// </summary>
    SUMIFS,
    /// <summary>
    /// Returns the average (arithmetic mean) of all the cells in a range that meet a given criteria
    /// </summary>
    AVERAGEIF,
    /// <summary>
    /// Returns the average (arithmetic mean) of all cells that meet multiple criteria.
    /// </summary>
    AVERAGEIFS,

  }
  /// <summary>
  /// Represents the OleObject Display Behaviour.
  /// </summary>
  public enum DVAspect
  {
    /// <summary>
    /// Returns the OleObject Display as Content.
    /// </summary>
    DVASPECT_CONTENT,
    /// <summary>
    /// Returns the OleObject Display as Icon.
    /// </summary>
    DVASPECT_ICON
  }
  /// <summary>
  /// Defines types of the ole object field.
  /// </summary>
  public enum OleLinkType
  {
    /// <summary>
    /// Ole object field type is EMBED.
    /// </summary>
    Embed,
    /// <summary>
    /// Ole object field type is LINK.
    /// </summary>
    Link
  }
  /// <summary>
  /// defines the types of OLE object
  /// </summary>
  public enum OleObjectType
  {
      /// <summary>
      /// Type is not defined
      /// </summary>
      Undefined = 0,
      /// <summary>
      /// Adobe Acrobat Document. File has ".pdf" extension.
      /// </summary>
      AdobeAcrobatDocument = 1,
      /// <summary>
      /// Bitmap Image. File has ".png" extension.
      /// </summary>
      BitmapImage = 2,
      /// <summary>
      /// Media Clip
      /// </summary>
      MediaClip = 3,
      /// <summary>
      /// Equation
      /// </summary>
      Equation = 4,
      /// <summary>
      /// Graph Chart
      /// </summary>
      GraphChart = 5,
      /// <summary>
      /// Excel 97-2003 Worksheet. File has ".xls" extension
      /// </summary>
      Excel_97_2003_Worksheet = 6,
      /// <summary>
      /// Excel Binary Worksheet. File has ".xlsb" extension
      /// </summary>
      ExcelBinaryWorksheet = 7,
      /// <summary>
      /// Excel chart. File has ".xls" extension
      /// </summary>
      ExcelChart = 8,
      /// <summary>   
      /// Excel Macro-Enabled Worksheet. File has ".xlsm" extension.
      /// </summary>
      ExcelMacroWorksheet = 9,
      /// <summary>
      /// Excel Worksheet. File has ".xlsx" extension.
      /// </summary>
      ExcelWorksheet = 10,
      /// <summary>
      /// PowerPoint 97-2003 Presentation. File has ".ppt" extension.
      /// </summary>
      PowerPoint_97_2003_Presentation = 11,
      /// <summary>
      /// PowerPoint 97-2003 Slide. File has ".sld" extension.
      /// </summary>
      PowerPoint_97_2003_Slide = 12,
      /// <summary>
      /// PowerPoint Macro-Enabled Presentation. File has ".pptm" extension.
      /// </summary>
      PowerPointMacroPresentation = 13,
      /// <summary>
      /// PowerPoint Macro-Enabled Slide. File has ".sldm" extension.
      /// </summary>
      PowerPointMacroSlide = 14,
      /// <summary>
      /// PowerPoint Presentation. File has ".pptx" extension.
      /// </summary>
      PowerPointPresentation = 15,
      /// <summary>
      /// PowerPoint Slide. File has ".sldx" extension.
      /// </summary>
      PowerPointSlide = 16,
      /// <summary>
      /// Word 97-2003 Document. File has ".doc" extension.
      /// </summary>
      Word_97_2003_Document = 17,
      /// <summary>
      /// Word Document. File has ".docx" extension.
      /// </summary>
      WordDocument = 18,
      /// <summary>
      /// Word Macro-Enabled Document. File has ".docm" extension.
      /// </summary>
      WordMacroDocument = 19,
      /// <summary>
      /// Visio Deawing
      /// </summary>
      VisioDrawing = 20,
      /// <summary>
      /// MIDI Sequence
      /// </summary>
      MIDISequence = 21,
      /// <summary>
      /// OpenDocument Presentation
      /// </summary>
      OpenDocumentPresentation = 22,
      /// <summary>
      /// OpenDocument Spreadsheet
      /// </summary>
      OpenDocumentSpreadsheet = 23,
      /// <summary>
      /// OpenDocument Text
      /// </summary>
      OpenDocumentText = 24,
      /// <summary>
      /// OpenOffice.org 1.1 Spreadsheet
      /// </summary>
      OpenOfficeSpreadsheet1_1 = 25,
      /// <summary>
      /// OpenOffice.org 1.1 Text
      /// </summary>
      OpenOfficeText_1_1 = 26,
      /// <summary>
      /// Package
      /// </summary>
      Package = 27,
      /// <summary>
      /// Video Clip
      /// </summary>
      VideoClip = 28,
      /// <summary>
      /// Wave Sound
      /// </summary>
      WaveSound = 29,
      /// <summary>
      /// WordPad Document
      /// </summary>
      WordPadDocument = 30,
      /// <summary>
      /// OpenOffice spreadsheet
      /// </summary>
      OpenOfficeSpreadsheet = 31,
      /// <summary>
      /// OpenOffice Text
      /// </summary>
      OpenOfficeText = 32
  }
  /// <summary>
  /// Defined types of Sparkline chart types.
  /// </summary>
  public enum SparklineType
  {
      /// <summary>
      /// Sparkline type is WinLoss.
      /// </summary>
      ColumnStacked100,
      /// <summary>
      /// Sparkline type is Column.
      /// </summary>
      Column,
      /// <summary>
      /// Sparkline type is Line.
      /// </summary>
      Line
  }
  /// <summary>
  /// Defines the Display of the Empty Cells within the Sparkline Range.
  /// </summary>
  public enum SparklineEmptyCells
  {
      /// <summary>
      /// Display as Gaps.
      /// </summary>
      Gaps,
      /// <summary>
      /// Diaplay as Zero.
      /// </summary>
      Zero,
      /// <summary>
      /// Display as Continued line.
      /// </summary>
      Line
  }  
  /// <summary>
  /// Defines the Sparkline vertical axis type.
  /// </summary>
  public enum SparklineVerticalAxisOptions
  {
      /// <summary>
      /// Automatic value for the vertical axis.
      /// </summary>
      Automatic,
      /// <summary>
      /// Same value for the vertical axis.
      /// </summary>
      Same,
      /// <summary>
      /// Custom value for the vertical axis.
      /// </summary>
      Custom
  }
  internal enum ChartAxisPos
  {
    l,
    r,
    b,
    t,
  }
  public enum LayoutModes
  {
      auto,
      /// <summary>
      /// Specifies that the width or Height shall be interpreted as the
      /// Right or Bottom of the chart element
      /// </summary>
      edge,
      /// <summary>
      /// Specifies that the Width or Height shall be interpreted as the 
      /// Width or Height of the chart element
      /// </summary>
      factor,
  }
  public enum LayoutTargets
  {
      auto,
      /// <summary>
      /// Specifies that the plot area size shall determine the size of the plot area, 
      /// not including the tick marks and axis labels
      /// </summary>
      inner,
      /// <summary>
      /// Specifies that the plot area size 
      /// shall determine the size of the plot area, the tick marks, and the axis labels
      /// </summary>
      outer,
  }
  /// <summary>
  /// Defines the directions of data bar in conditional formatting
  /// </summary>
  public enum DataBarDirection
  {
      /// <summary>
      /// Context, the default direction
      /// </summary>
      context = -5002,
      /// <summary>
      /// Left to Right (LTR)
      /// </summary>
      leftToRight = -5003,
      /// <summary>
      /// Right to Left (RTL)
      /// </summary>
      rightToLeft = -5004
  }
  /// <summary>
  /// Defines the axis positions of data bar in conditional formatting
  /// </summary>
  public enum DataBarAxisPosition
  {
      /// <summary>
      /// Default value if the conditional formatting rule is created programmatically
      /// </summary>
      none,
      /// <summary>
      /// Default value if the conditional formatting rule is created using the user interface
      /// </summary>
      automatic,
      /// <summary>
      /// Defines the axis position at the mid point
      /// </summary>
      middle
  }
}
