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
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the drawing mode of chart.
    /// </summary>
    enum DrawingMode
    {
        /// <summary>
        /// Chart will be painted in 2D mode.
        /// </summary>
        Simple2D,

        /// <summary>
        /// Chart will be painted in pseudo 3D mode.
        /// </summary>
        Pseudo3D,

        /// <summary>
        /// Chart will be painted in real 3D mode.
        /// </summary>
        Real3D
    }

    /// <summary>
    /// Specifies type of coordinate system.
    /// </summary>
    enum ChartAreaAxesType
    {
        /// <summary>
        /// Any coortinate sysytem isn't used.
        /// </summary>
        None,

        /// <summary>
        /// The circular coordinate system is used.
        /// </summary>
        Circular,

        /// <summary>
        /// The rectangular coordinate system is used.
        /// </summary>
        Rectangular
    }

    /// <summary>
    /// Lists the different ways in which multiple axes will be rendered on the same side (X or Y)
    /// </summary>
    public enum ChartAxesLayoutMode
    {
        /// <summary>
        /// Multiple axes will be rendered one after the other, side-by-side.
        /// </summary>
        SideBySide,

        /// <summary>
        /// Multiple axes will be rendered in parallel.
        /// </summary>
        Stacking
    }
    public enum Skins
    {
        /// <summary>
        /// Represents No Skins
        /// </summary>
        None,
        /// <summary>
        /// Represents Office2007Black Skin
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Represents Office2007Blue Skin
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Represents Office2007Silver Skin
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Represents Almond Skin
        /// </summary>
        Almond,
        /// <summary>
        /// Represents Blend Skin
        /// </summary>
        Blend,
        /// <summary>
        /// Represents Blueberry Skin
        /// </summary>
        Blueberry,
        /// <summary>
        /// Represents Marble Skin
        /// </summary>
        Marble,
        /// <summary>
        /// Represents MidNight Skin
        /// </summary>
        Midnight,
        /// <summary>
        /// Represents Monochrome Skin
        /// </summary>
        Monochrome,
        /// <summary>
        /// Represents Olive Skin
        /// </summary>
        Olive,
        /// <summary>
        /// Represents Sandune Skin
        /// </summary>
        Sandune,
        /// <summary>
        /// Represents Turquoise Skin
        /// </summary>
        Turquoise,
        /// <summary>
        /// Represents Vista Skin
        /// </summary>
        Vista,
        /// <summary>
        /// Represents VS2010 Skin
        /// </summary>
        VS2010,
        /// <summary>
        /// Represents VS2010 Skin
        /// </summary>
        Metro

    }
    /// <summary>
    /// Specifies flags that control the elements painting.
    /// </summary>
    [Flags]
    public enum ChartPaintFlags
    {
        /// <summary>
        /// Indicates visibility of background.
        /// </summary>
        Background = 0x01,

        /// <summary>
        /// Indicates visibility of border.
        /// </summary>
        Border = 0x02,

        /// <summary>
        /// Indicates visibility of interactive cursors.
        /// </summary>
        InteractiveCursors = 0x08,

        /// <summary>
        /// Indicates visibility of axes.
        /// </summary>
        Axes = 0x16,

        /// <summary>
        /// All elements will be painted.
        /// </summary>
        All = Background | Border | InteractiveCursors | Axes
    }

    /// <summary>
    /// Lists the options available for rendering the labels in the Pyramid, Funnel, Pie or Doughnut Chart.
    /// </summary>
    public enum ChartAccumulationLabelStyle
    {
        /// <summary>
        /// Labels are not shown.
        /// </summary>
        Disabled,

        /// <summary>
        /// Labels are rendered inside the pie.
        /// </summary>
        Inside,

        /// <summary>
        /// Lables are rendered outside the pie.In funnel or pyramid chart, if the label style is set to outside, then the label placement has be left or right.
        /// </summary>
        Outside,

        /// <summary>
        /// Labels are rendered outside the pie and in columns.
        /// </summary>
        OutsideInColumn,


        /// <summary>
        /// Labels are rendered outside the pie and in chart area.
        /// </summary>
        OutsideInArea
    }

    /// <summary>
    /// Lists the options for positioning the lables in an accumulation chart (Pyramid or Funnel)
    /// </summary>
    public enum ChartAccumulationLabelPlacement
    {
        /// <summary>
        /// Renders the label on top of the block, when rendered Inside.
        /// </summary>
        Top,

        /// <summary>
        /// Renders the label at the bottom of the block, when rendered Inside.
        /// </summary>
        Bottom,

        /// <summary>
        /// Renders the label to the left of the block, when rendered Outside.
        /// </summary>
        Left,

        /// <summary>
        /// Renders the label to the right of the block, when rendered Outside.
        /// </summary>
        Right,

        /// <summary>
        /// Renders the label at the center of the block, when rendered Inside.
        /// </summary>
        Center
    }

    /// <summary>
    /// Lists the options in which a pyramid base could be rendered in 3D mode.
    /// </summary>
    public enum ChartFigureBase
    {
        /// <summary>
        /// Base is a square.
        /// </summary>
        Square,

        /// <summary>
        /// Base is a circle.
        /// </summary>
        Circle
    }

    /// <summary>
    /// Lists the funnel mode options.
    /// </summary>
    public enum ChartFunnelMode
    {
        /// <summary>
        /// The specified Y value is used to compute the width of the corresponding block.
        /// </summary>
        YIsWidth,

        /// <summary>
        /// The specified Y value is used to compute the height of the corresponding block.
        /// </summary>
        YIsHeight
    }

    /// <summary>
    /// Specifies the mode in which the Y values should be interpreted in the Pyramid chart.
    /// </summary>
    public enum ChartPyramidMode
    {
        /// <summary>
        /// The Y values are proportional to the length of the sides of the pyramid.
        /// </summary>
        Linear,

        /// <summary>
        /// The Y values are proportional to the surface area of the corresponding blocks.
        /// </summary>
        Surface
    }

    /// <summary>
    /// Specifies how much 3D space will be used by chart.
    /// </summary>
    public enum ChartUsedSpaceType
    {
        /// <summary>
        /// Chart type doesn't use the 3D space.
        /// </summary>
        None,

        /// <summary>
        /// Chart type uses the all 3D space.
        /// </summary>
        All,

        /// <summary>
        /// Chart type uses the single layer of 3D space for each series.
        /// </summary>
        OneForOne,

        /// <summary>
        /// Chart type uses the single layer of 3D space for all series with the same type.
        /// </summary>
        OneForAll
    }

    /// <summary>
    /// Specifies the Docking position of a control.
    /// </summary>
    public enum ChartDock
    {
        /// <summary>
        /// The control will be docked to the Left of its container
        /// </summary>
        Left,

        /// <summary>
        /// The control will be docked to the Right of its container
        /// </summary>
        Right,

        /// <summary>
        /// The control will be docked to the Top of its container
        /// </summary>
        Top,

        /// <summary>
        /// The control will be docked to the Bottom of its container
        /// </summary>
        Bottom,

        /// <summary>
        /// The control will not be docked inside the container
        /// </summary>
        Floating
    }

    /// <summary>
    /// Specifies the alignment of the control.
    /// </summary>
    public enum ChartAlignment
    {
        /// <summary>
        /// The control will be aligned Near
        /// </summary>
        Near,

        /// <summary>
        /// The control will be aligned in the Center
        /// </summary>
        Center,

        /// <summary>
        /// The control will be aligned Far
        /// </summary>
        Far
    }

    /// <summary>
    /// Specifies the placement of element by the parent bounds.
    /// </summary>
    public enum ChartPlacement
    {
        /// <summary>
        /// Elements are located inside the parent.
        /// </summary>
        Inside,

        /// <summary>
        /// Elements are located outside the parent.
        /// </summary>
        Outside
    }

    /// <summary>
    /// Specifies the orientation.
    /// </summary>
    public enum ChartOrientation
    {
        /// <summary>
        /// The chart element is oriented horizontally
        /// </summary>
        Horizontal,

        /// <summary>
        /// The chart element is oriented vertically
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Specifies the mode of drawing the edge labels.
    /// </summary>
    public enum ChartSetMode
    {
        /// <summary>
        /// None of the edge label settings will be applied.
        /// </summary>
        None,

        /// <summary>
        /// The margin labels will be auto set
        /// </summary>
        AutoSet,

        /// <summary>
        /// The margin labels will be user set
        /// </summary>
        UserSet
    }

    /// <summary>
    /// Specifies the vertical alignment.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// The element will be aligned in Top
        /// </summary>
        Top,

        /// <summary>
        /// The element will be aligned in Center
        /// </summary>
        Center,

        /// <summary>
        /// The element will be aligned in Bottom 
        /// </summary>
        Bottom
    }

    /// <summary>
    ///     Specifies the options to control what happens if chart labels intersect each other due to lack of space. It is used in conjunction with
    ///     <see cref="ChartLabelIntersectAction"/>.
    /// </summary>
    public enum ChartLabelIntersectionActionEffect
    {
        /// <summary>
        ///  <see cref="ChartLabelIntersectAction"/> related changes affect all label text.
        /// </summary>
        All,

        /// <summary>
        ///  <see cref="ChartLabelIntersectAction"/> related changes affect only specific labels that may need to be changed.
        /// </summary>
        Specific
    }

    /// <summary>
    ///  Specifies the representation symbol that is to be used inside the legend box for a series.
    /// </summary>
    public enum ChartLegendRepresentationType
    {
        /// <summary>
        /// A visual representation will be none.
        /// </summary>
        None,

        /// <summary>
        /// A visual representation of the series type will be rendered.
        /// </summary>
        SeriesType,

        /// <summary>
        /// The image associated with the series type will be rendered.
        /// </summary>
        SeriesImage,

        /// <summary>
        /// A rectangle will be rendered.
        /// </summary>
        Rectangle,

        /// <summary>
        /// A line will be rendered.
        /// </summary>
        Line,

        /// <summary>
        /// A straight line will be rendered.
        /// </summary>
        StraightLine,

        /// <summary>
        /// A circle will be rendered.
        /// </summary>
        Circle,

        /// <summary>
        /// A diamond will be rendered.
        /// </summary>
        Diamond,

        /// <summary>
        /// A hexagon will be rendered.
        /// </summary>
        Hexagon,

        /// <summary>
        /// A pentagon will be rendered.
        /// </summary>
        Pentagon,

        /// <summary>
        /// A triangle will be rendered.
        /// </summary>
        Triangle,

        /// <summary>
        /// An inverted triangle will be rendered.
        /// </summary>
        InvertedTriangle,

        /// <summary>
        /// A cross will be rendered.
        /// </summary>
        Cross
    }

    /// <summary>
    /// Specifies the options for how to position a custom point on the chart.
    /// </summary>
    public enum ChartCustomPointType
    {
        /// <summary>
        /// Coordinates of the custom point are taken as a percentage of the chart area.
        /// </summary>
        Percent,

        /// <summary>
        /// Coordinates of the custom point are taken to be in pixels.
        /// </summary>
        Pixel,

        /// <summary>
        /// Coordinates of the custom point are taken as a percentage of the chart area.
        /// </summary>
        ChartCoordinates,

        /// <summary>
        /// The custom point will follow the regular point of any series it is assigned to.
        /// </summary>
        PointFollow
    }

    /// <summary>
    ///  Specifies the options for the action that is to be taken when labels intersect each other.
    /// </summary>
    public enum ChartLabelIntersectAction
    {
        /// <summary>
        /// No special action is taken. Labels may intersect.
        /// </summary>
        None,

        /// <summary>
        /// When labels would intersect each other, they are wrapped to avoid intersection.
        /// </summary>
        Wrap,

        /// <summary>
        /// When labels would intersect each other, they are wrapped into multiple rows to avoid intersection.
        /// </summary>
        MultipleRows,

        /// <summary>
        /// When labels would intersect each other, they are rotated to avoid intersection.
        /// </summary>
        Rotate
    }

    /// <summary>
    /// The ChartTitleDrawMode enumerator.
    /// </summary>
    public enum ChartTitleDrawMode
    {
        /// <summary>
        /// No special mode is taken.
        /// </summary>
        None,

        /// <summary>
        /// Wraps title.
        /// </summary>
        Wrap,

        /// <summary>
        /// Removes the end of trimmed lines, and replaces them with an ellipsis.
        /// </summary>
        Ellipsis
    }

    /// <summary>
    ///     Specifies the actual symbol rendered inside the legend box for a series based on hints / specifications given with
    ///     <see cref="ChartLegendRepresentationType"/>.
    /// </summary>
    public enum ChartLegendItemType
    {
        /// <summary>
        /// Visual representation is empty.
        /// </summary>
        None,

        /// <summary>
        ///  Visual representation is a line.
        /// </summary>
        Line,

        /// <summary>
        ///  Visual representation is a rectangle.
        /// </summary>
        Rectangle,

        /// <summary>
        ///  Visual representation is a spline.
        /// </summary>
        Spline,

        /// <summary>
        ///  Visual representation is an area chart.
        /// </summary>
        Area,

        /// <summary>
        ///  Visual representation is a pie slice.
        /// </summary>
        PieSlice,

        /// <summary>
        ///  Visual representation is an image.
        /// </summary>
        Image,

        /// <summary>
        ///  Visual representation is a circle.
        /// </summary>
        Circle,

        /// <summary>
        ///  Visual representation is a diamond.
        /// </summary>
        Diamond,

        /// <summary>
        ///  Visual representation is a hexagon.
        /// </summary>
        Hexagon,

        /// <summary>
        ///  Visual representation is a pentagon.
        /// </summary>
        Pentagon,

        /// <summary>
        ///  Visual representation is a triangle.
        /// </summary>
        Triangle,

        /// <summary>
        ///  Visual representation is an inverted triangle.
        /// </summary>
        InvertedTriangle,

        /// <summary>
        ///  Visual representation is a cross.
        /// </summary>
        Cross,

        /// <summary>
        ///  Visual representation is a spline area.
        /// </summary>
        SplineArea,

        /// <summary>
        ///  Visual representation is a Straight area.
        /// </summary>
        StraightLine
    }

    /// <summary>
    /// Specifies the options for positioning of the Chart's text.
    /// </summary>
    public enum ChartTextPosition
    {
        /// <summary>
        ///  Text is positioned at the top of the chart area.
        /// </summary>
        Top,

        /// <summary>
        ///  Text is positioned at the bottom of the chart area.
        /// </summary>
        Bottom,

        /// <summary>
        ///  Text is positioned to the left of the chart area.
        /// </summary>
        Left,

        /// <summary>
        ///  Text is positioned to the right of the chart area.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the different values that are natively used.
    /// </summary>
    public enum ChartValueType
    {
        /// <summary>
        ///  Double value
        /// </summary>
        Double,

        /// <summary>
        ///  DateTime value
        /// </summary>
        DateTime,

        /// <summary>
        ///  Custom value
        /// </summary>
        Custom,

        /// <summary>
        ///  Logarithmic value
        /// </summary>
        Logarithmic
    }

    /// <summary>
    ///  Specifies the representation classification. Generally used only when you are writing custom renderers.
    /// </summary>
    public enum ChartSeriesBaseType
    {
        /// <summary>
        /// Values are single series rendered.
        /// </summary>
        Single,

        /// <summary>
        ///  Values are plotted side by side.
        /// </summary>
        SideBySide,

        //    /// <summary>
        //    ///  Values are stacked.
        //    /// </summary>
        //    Stacking,
        /// <summary>
        ///  Values are independently rendered.
        /// </summary>
        Independent,

        /// <summary>
        /// Values are plotted by circular (Radar, Polar).
        /// </summary>
        Circular,

        /// <summary>
        ///  Other non-standard rendering.
        /// </summary>
        Other
    }

    /// <summary>
    ///  Specifies the representation classification. Generally used only when you are writing custom renderers.
    /// </summary>
    public enum ChartSeriesBaseStackingType
    {
        /// <summary>
        ///  Values are stacked.
        /// </summary>
        Stacked,

        /// <summary>
        ///  Values are stacked.
        /// </summary>
        FullStacked,

        /// <summary>
        ///  Values are not stacked.
        /// </summary>
        NotStacked
    }

    /// <summary>
    ///  Specifies the different chart types.
    /// </summary>
    public enum ChartSeriesType
    {
        /// <summary>
        ///  Line chart
        /// </summary>
        Line,

        /// <summary>
        ///  Spline chart
        /// </summary>
        Spline,

        /// <summary>
        ///  Rotated spline chart
        /// </summary>
        RotatedSpline,

        /// <summary>
        ///  Scatter chart
        /// </summary>
        Scatter,

        /// <summary>
        ///  Column chart
        /// </summary>
        Column,

        /// <summary>
        ///  Bar chart
        /// </summary>
        Bar,

        /// <summary>
        ///  Gantt chart
        /// </summary>
        Gantt,

        /// <summary>
        ///  Stacking bar chart
        /// </summary>
        StackingBar,

        /// <summary>
        ///  Area chart
        /// </summary>
        Area,

        /// <summary>
        /// Range Area chart
        /// </summary>
        RangeArea,

        /// <summary>
        ///  Area chart with spline connectors
        /// </summary>
        SplineArea,

        /// <summary>
        ///  Stacking area chart
        /// </summary>
        StackingArea,

        /// <summary>
        ///  Stacking column chart
        /// </summary>
        StackingColumn,

        /// <summary>
        ///  Stacking area chart
        /// </summary>
        StackingArea100,

        /// <summary>
        ///  Stacking 100% bar chart
        /// </summary>
        StackingBar100,

        /// <summary>
        ///  Stacking 100% column chart
        /// </summary>
        StackingColumn100,

        /// <summary>
        ///  Pie chart
        /// </summary>
        Pie,

        /// <summary>
        ///  Funnel chart
        /// </summary>
        Funnel,

        /// <summary>
        ///  Pyramid chart
        /// </summary>
        Pyramid,

        /// <summary>
        ///  HiLo chart
        /// </summary>
        HiLo,

        /// <summary>
        ///  HiLoOpenClose chart
        /// </summary>
        HiLoOpenClose,

        /// <summary>
        ///  Candle chart
        /// </summary>
        Candle,

        /// <summary>
        ///  Bubble chart
        /// </summary>
        Bubble,

        /// <summary>
        ///  StepLine chart
        /// </summary>
        StepLine,

        /// <summary>
        ///  StepArea chart
        /// </summary>
        StepArea,

        /// <summary>
        /// Radar chart
        /// </summary>
        Radar,

        /// <summary>
        /// Kagi chart
        /// </summary>
        Kagi,

        /// <summary>
        /// Renko chart
        /// </summary>
        Renko,

        /// <summary>
        /// Polar chart
        /// </summary>
        Polar,

        /// <summary>
        /// ColumnRange chart
        /// </summary>
        ColumnRange,

        /// <summary>
        /// ThreeLineBreak chart
        /// </summary>
        ThreeLineBreak,

        /// <summary>
        /// PointAndFigure chart
        /// </summary>
        PointAndFigure,

        /// <summary>
        ///  BoxAndWhisker chart
        /// </summary>
        BoxAndWhisker,

        /// <summary>
        ///  Histogram chart
        /// </summary>
        Histogram,

        /// <summary>
        ///  This Chart is mainly used in sensitivity analysis.
        ///  It shows how different random factors can influence
        ///  the prognoses income.
        /// </summary>
        Tornado,
		/// <summary>
		/// HeatMap chart
		/// </summary>
		HeatMap,
        /// <summary>
        ///  Custom chart. Rendering is done by user.
        /// </summary>
        Custom
    }

    /// <summary>
    ///     Specifies chart axis ranges configuration options.
    /// </summary>
    public enum ChartAxisRangeType
    {
        /// <summary>
        ///  Bounds are automatically calculated based on values.
        /// </summary>
        Auto,

        /// <summary>
        ///  Bounds and intervals are explicitly set.
        /// </summary>
        Set
    }

    /// <summary>
    ///  Specifies the orientation of text when rendered with a value point.
    /// </summary>
    public enum ChartTextOrientation
    {
        /// <summary>
        /// Text is rendered above and to the left of the point.
        /// </summary>
        UpLeft,

        /// <summary>
        ///  Text is rendered above the point.
        /// </summary>
        Up,

        /// <summary>
        /// Text is rendered above and to the right of the point.
        /// </summary>
        UpRight,

        /// <summary>
        ///  Text is rendered to the left of the point.
        /// </summary>
        Left,

        /// <summary>
        ///  Text is centered on the point.
        /// </summary>
        Center,

        /// <summary>
        ///  Text is rendered to the right of the point.
        /// </summary>
        Right,

        /// <summary>
        /// Text is rendered below and to the left of the point.
        /// </summary>
        DownLeft,

        /// <summary>
        ///  Text is rendered below the point.
        /// </summary>
        Down,

        /// <summary>
        /// Text is rendered below and to the right of the point.
        /// </summary>
        DownRight,

        /// <summary>
        ///  Text is rendered in a manner that is appropriate for the situation.
        /// </summary>
        Smart,

        /// <summary>
        ///  Text is rendered above the region that represents the point (Example: above the bar in a bar chart).
        /// </summary>
        RegionUp,

        /// <summary>
        ///  Text is rendered below the region that represents the point (Example: below the bar in a bar chart).
        /// </summary>
        RegionDown,

        /// <summary>
        ///  Text is centered in the region that represents the point (Example: centered inside the bar in a bar chart).
        /// </summary>
        RegionCenter,

        /// <summary>
        ///  Text is centered to the symbol if one is associated with the point.
        /// </summary>
        SymbolCenter
    }

    /// <summary>
    /// Specifies the style of the radar chart.
    /// </summary>
    public enum ChartRadarAxisStyle
    {
        /// <summary>
        /// Axes are rendered polygonal.
        /// </summary>
        Polygon,

        /// <summary>
        /// Axes are rendered as circles.
        /// </summary>
        Circle
    }

    /// <summary>
    /// Indicates the style of the radar chart.
    /// </summary>
    public enum ChartColumnWidthMode
    {
        /// <summary>
        /// The width is specified in ChartPoint.YValues[1] in units of X-Axis range.
        /// </summary>
        RelativeWidthMode,

        /// <summary>
        /// If width of columns aren't given in point YValues[1], in pixels.
        /// If not specified, the column will be rendered in DefaultWidthMode.
        /// </summary>
        FixedWidthMode,

        /// <summary>
        /// The width of the columns will always be calculated to fill the space between columns.
        /// </summary>
        DefaultWidthMode
    }

    /// <summary>
    /// Specifies the drawing mode of 3D column/bar charts
    /// </summary>
    public enum ChartColumnDrawMode
    {
        /// <summary>
        /// Columns are drawn in depth
        /// </summary>
        InDepthMode,

        /// <summary>
        /// Column are drawn side-by-side
        /// </summary>
        PlaneMode,

        /// <summary>
        /// Columns are drawn in depth with the same size.
        /// </summary>
        ClusteredMode
    }

    /// <summary>
    /// Specifies the mode of drawing the Gantt chart
    /// </summary>
    public enum ChartGanttDrawMode
    {
        /// <summary>
        /// Plots the Gantt chart as overlapped
        /// </summary>
        CustomPointWidthMode,

        /// <summary>
        /// Plots the Gantt chart as side-by-side
        /// </summary>
        AutoSizeMode
    }

    /// <summary>
    /// Specifies the modes that is to be used for drawing tick labels on the axis.
    /// </summary>
    public enum ChartAxisTickLabelDrawingMode
    {
        /// <summary>
        /// The ticks and tick labels aren't drawing;
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The ticks and tick labels are distributed uniformly along the axis 
        /// with specified interval.
        /// </summary>
        AutomaticMode = 0x01,

        /// <summary>
        /// The user can specify the positions of labels and text of labels .
        /// </summary>
        UserMode = 0x02,

        /// <summary>
        /// The user can specify the positions of labels and text of labels.
        /// The Automatic labels are also drawn.
        /// </summary>
        BothUserAndAutomaticMode = AutomaticMode | UserMode
    }

    /// <summary>
    /// Specifies how to print content that contains color or shades of gray. 
    /// </summary>
    public enum ChartPrintColorMode
    {
        /// <summary>
        /// The series Styles will be in monochrome scale during printing.
        /// </summary>
        GrayScale,

        /// <summary>
        /// The series Styles will be in colored scale during printing.
        /// </summary>
        Color,

        /// <summary>
        /// The printer will be checked if it supports colors. If not then the GrayScale mode will be used.
        /// </summary>
        CheckPrinter
    }

    /// <summary>
    /// Specifies type of connection between scatter points.
    /// </summary>
    public enum ScatterConnectType
    {
        /// <summary>
        /// Connect type will be none. (Scatter chart)
        /// </summary>
        None,

        /// <summary>
        /// Connect type will be of line (ScatterLine chart)
        /// </summary>
        Line,

        /// <summary>
        /// Connect type will be spline (ScatterSpline chart)
        /// </summary>
        Spline
    }

    /// <summary>
    /// Specifies that Open and Close lines are displayed. 
    /// </summary>
    public enum ChartOpenCloseDrawMode
    {
        /// <summary>
        /// Draws both open and close lines.
        /// </summary>
        Both,

        /// <summary>
        /// Draws only Close line
        /// </summary>
        Close,

        /// <summary>
        /// Draws only Open line
        /// </summary>
        Open
    }

    /// <summary>
	/// 
	/// </summary>
	public enum ChartHeatMapLayoutStyle
	{
		/// <summary>
		/// 
		/// </summary>
		Rectangular,
		/// <summary>
		/// 
		/// </summary>
		Vertical,
		/// <summary>
		/// 
		/// </summary>
		Horizontal
	}

	/// <summary>
    /// Specifies the Orientation of Interactive Cursor.
    /// </summary>    
    public enum InteractiveCursorOrientation
    {
        /// <summary>
        /// Only Horizontal Cursor gets displayed
        /// </summary>
        Horizontal,

        /// <summary>
        /// Only Vertical Cursor gets displayed
        /// </summary>
        Vertical,

        /// <summary>
        /// Both the Horizontal & vertical Cursor gets displayed
        /// </summary>
        Both
    }

    /// <summary>
    /// Specifies are zooming enabled only for single axis.
    /// </summary>
    /// <internalonly/>
    public enum PartialZoom
    {
        /// <summary>
        /// Only XAxis zooming
        /// </summary>
        OnlyX,

        /// <summary>
        /// Only YAxis zooming
        /// </summary>
        OnlyY,

        /// <summary>
        /// is not partial zooming
        /// </summary>
        None
    }
}
