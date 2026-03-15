#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{

    /// <summary>
    /// Identifies range padding type enumeration.
    /// </summary>
    public enum ChartRangePaddingType
    {
        /// <summary>
        /// Range is not getting changed.
        /// </summary>
        None,

        /// <summary>
        /// Range will be rounded to the interval.
        /// </summary>
        Normal,

        /// <summary>
        /// If padding is less the half of an interval, range should be extended.
        /// </summary>
        Additional
    }

    /// <summary>
    /// Identifies range padding type enumeration.
    /// </summary>
    public enum SplitterBarVisibility
    {
        /// <summary>
        /// Enum value for Hide mode
        /// </summary>
        Hide,
        /// <summary>
        /// Enum value for AlwaysVisible mode
        /// </summary>
       AlwaysVisible,  
        /// <summary>
        /// Enum value for VisibleOnMouseMove mode
        /// </summary>
        VisibleOnMouseMove
    }

    /// <summary>
    /// Enum values for ChartAxisLabelsMode values
    /// </summary>
    [Flags]
    public enum ChartAxisLabelsMode
    {
        /// <summary>
        /// Labels values are taken from point's X-coordinate. 
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Content is determined automatically.
        /// </summary>
        Auto = 0x01,

        /// <summary>
        /// External datasource is used for labels content.
        /// </summary>
        DataSource = 0x02,

        /// <summary>
        /// Custom values are used for labels content representation.
        /// </summary>
        Custom = 0x04,

        /// <summary>
        /// Content for labels is either determined automatically, taken from external datasource or being set with custom values.
        /// </summary>
        Default = Auto | DataSource | Custom
    }

    #region enum RangeCalculationMode
    /// <summary>
    ///  Specifies the options for the action that is to be taken when labels intersect each other.
    /// </summary>
    public enum RangeCalculationMode
    {
        /// <summary>
        /// All series will have plus one range added
        /// </summary>
        AdjustAcrossChartTypes,

        /// <summary>
        /// All series segments will start from 0th point
        /// </summary>
        ConsistentAcrossChartTypes
    }
    #endregion

    /// <summary>
    /// Enum values for Host 
    /// </summary>
    public enum Host
    {
        /// <summary>
        /// The Host Control is Silverlight Chart control
        /// </summary>
        Chart,

        /// <summary>
        /// The host control is OLAP chart control
        /// </summary>
        OLAPChart
    }

    #region EmptypointValue
    /// <summary>
    /// Enum values for EmptyPointValues modes
    /// </summary>
    public enum EmptyPointValue
    {
        /// <summary>
        /// Enum value for Zero mode
        /// </summary>
        Zero,
        /// <summary>
        ///Enum value for Average mode 
        /// </summary>
        Average
    }
    #endregion

    #region enum EmptyPointStyle
    /// <summary>
    /// EmptyPointStyle Enum to make difference for Empty Points
    /// </summary>
    public enum EmptyPointStyle
    {
        /// <summary>
        /// Sets a symbol for Empty Point Segment with the Series Interior.
        /// </summary>
        Symbol,

        /// <summary>
        /// Sets a different Brush for Empty Point Segment.
        /// </summary>
        Interior,

        /// <summary>
        /// Sets a symbol and different Brush for Empty Point Segment.
        /// </summary>
        SymbolAndInterior
    }
    #endregion

    /// <summary>
    /// Represents Various Chart type in essential chart
    /// </summary>
    public enum ChartTypes
    {
        /// <summary>
        /// Compares values across categories. The series values are displayed as individual columns, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        /// <remarks>
        /// There are three types of column charts: column, <see cref="ChartTypes.StackingColumn"/>, 
        /// and <see cref="ChartTypes.RangeColumn"/>.
        /// </remarks>
        Column,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual lines, grouped by category..
        /// </summary>
        Line,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual points, grouped by category.
        /// </summary>
        Scatter,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual bar, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        Bar,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual Stacking columns, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        StackingColumn,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual stacking bar, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        StackingBar,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual Area, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        Area,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual Stacking Area, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        StackingArea,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual Stacking Area 100, grouped by category. The height of each series is determined by the series value.
        /// </summary>
        StackingArea100,

        /// <summary>
        /// FastLine chart significantly reduce the drawing time of a series that contains a very large number of data points.
        /// </summary>
        FastLine,

        /// <summary>
        /// Pie chart type.
        /// </summary>
        Pie,

        /// <summary>
        /// A doughnut chart displays value data as percentages of the whole. 
        /// Categories are represented by individual slices. Doughnut charts are typically used to show percentages. 
        /// Doughnut charts are functionally identical to pie charts.
        /// </summary>
        Doughnut,

        /// <summary>
        /// A bubble chart draws bubbles for each point in a series. 
        /// The chart expects three values per bubble: the domain (commonly the X-axis) value and, 
        /// the range (commonly the y-axis) value.
        /// </summary>
        Bubble,

        /// <summary>
        /// HiLo chart is a sophisticated chart type, that finds its place in stock analysis. 
        /// </summary>
        HiLo,

        /// <summary>
        /// HiLo OpenClose Chart is a HiLo chart with four Y values for each point.
        /// </summary>
        HiLoOpenClose,

        /// <summary>
        /// A Gantt chart is a type of bar chart that shows a project schedule. 
        /// Gantt charts show the start and finish dates of the terminal elements and summary elements of a project. 
        /// Terminal elements and summary elements comprise the work breakdown structure of the project. 
        /// Gantt charts can be used to show current schedule status using percent-complete shadings and a 
        /// vertical "TODAY" line (also called "TIME NOW" or "DATA DATE"). It is an instrument or tool of the 
        /// Project management invented by the engineer Henry L. Gannt.
        /// </summary>
        Gantt,

        /// <summary>
        /// A box and whisker chart type is a convenient way of graphically depicting groups of numerical data through their 
        /// five-number summaries (the smallest observation, lower quartile (Q1), median (Q2), upper quartile (Q3), 
        /// and largest observation). A box and whisker chart may also indicate which observations, if any, might be considered outliers.  
        /// </summary>
        /// <remarks>
        /// Box and whisker chart is helpful in Quality Analysis for interpreting the distribution of data since it 
        /// can easily show whether the data is skewed and if there are unusual observations (outliers) in the dataset. 
        /// Box and whisker chart is also very useful when large numbers of observations are involved and when two or more 
        /// datasets are being compared. 
        /// </remarks>
        BoxAndWhisker,

        /// <summary>
        /// Pyramid charts are another type of accumulation chart which has a triangular upper surface that 
        /// converge at one point.
        /// </summary>
        Pyramid,

        /// <summary>
        /// A candlestick chart is a style of bar-chart used primarily to describe price movements of an equity over time.
        /// It is a combination of a line-chart and a bar-chart, in that each bar represents the range of price movement 
        /// over a given time interval. It is most often used in technical analysis of equity and currency price patterns. 
        /// It appears superficially similar to error bars, but are unrelated.
        /// </summary>
        Candle,

        /// <summary>
        /// Columns connecting minimum and maximum series points with the same respective X. 
        /// </summary>
        RangeColumn,

        /// <summary>
        /// Range Area Chart is a variation of Area Chart type that lets you plot bands of data in a chart, like Bollinger bands, weather patterns, etc. 
        /// Each point in the chart is specified by 2 Y values – the lower and higher end of the band. 
        /// </summary>
        RangeArea,

        /// <summary>
        /// Tornado chart is a bar chart, which shows the variability of an outcome, as the result of several factors. 
        /// This variability is displayed using relative lengths of bars across a range. 
        /// </summary>
        Tornado,

        /// <summary>
        /// The Funnel Chart type displays data that equals 100% when totalled. 
        /// This type of chart is a single series chart representing the data as portions of 100%, 
        /// and this chart does not use any axes.
        /// </summary>
        Funnel,

        /// <summary>
        /// Step Line charts are line charts, with values drawn continuously, step by step without any gaps between them. 
        /// </summary>
        StepLine,

        /// <summary>
        /// The StepArea chart consists of pairs of values (plotted as points) on a line for each series in the given 
        /// dataset. Each pair of values consists of one date (value) and one numeric value. 
        /// This chart will have one date\time (value) based axis (the domain axis) and one numeric axis (the range axis).
        /// Each line is drawn at a 90 degree angle from point to point. Each line will fill the from the line to the 
        /// bottom of the plot with its series color.
        /// </summary>
        StepArea,

        /// <summary>
        /// A column chart where multiple series are stacked vertically to fit 100% of the chart area. 
        /// If there is only one series in your chart, all the column bars will fit to 100% of the chart area.
        /// </summary>
        StackingColumn100,

        /// <summary>
        /// A bar chart where multiple series are stacked horizontally to fit 100% of the chart area. 
        /// If there is only one series in chart, all the bars will fit to 100% of area.
        /// </summary>
        StackingBar100,

        /// <summary>
        /// A spline chart is simply a line chart that plots a fitted curve through each data point in a series.
        /// </summary>
        Spline,

        /// <summary>
        /// Spline Chart is an Area chart in which each area is given a color to emphasize the relationships 
        /// between the pieces of charted information.
        /// </summary>
        SplineArea,

        /// <summary>
        /// Rotated Spline chart defines chart series points as interconnected rotated smooth curves.
        /// </summary>
        RotatedSpline,

        /// <summary>
        /// A renko chart is constructed by placing a brick in the next column once the price surpasses the top or 
        /// bottom of the previous brick by a pre-defined amount. Lighter bricks are used when the direction of the 
        /// trend is up, while darker bricks are used when the trend is down. This type of chart is very effective 
        /// for traders to identify key support/resistance levels. 
        /// Transaction signals are generated when the direction of the trend changes and the bricks alternate colors.
        /// </summary>
        Renko,

        /// <summary>
        /// ThreeLineBreak displays a series of vertical boxes that are based on changes in prices. 
        /// It depicts rising and falling lines of varying heights. 
        /// </summary>
        ThreeLineBreak,

        /// <summary>
        /// A Kagi chart is created with a series of vertical lines connected by short horizontal lines. 
        /// The thickness and direction of the lines is based on the price of the underlying stock or asset
        /// </summary>
        Kagi,

        /// <summary>
        /// Point and figure type
        /// </summary>
        PointAndFigure,

        /// <summary>
        /// A radar chart is two-dimensional chart of three or more quantitative variables represented on axes starting from the same point. The relative position and angle of the axes is uninformative.
        /// Radar charts are usually used to compare performance of different entities on a same set of axes.
        /// </summary>
        Radar,

        /// <summary>
        /// Polar chart is used to display chart data points, connected with a line, in a polar co-ordinate system. 
        /// </summary>
        Polar,

        /// <summary>
        /// Represents a graphical display of tabulated frequencies. 
        /// A histogram is the graphical version of a table that shows what proportion of cases fall into each of 
        /// several or many specified categories. The histogram differs from a bar chart in that it is the area 
        /// of the bar that denotes the value, not the height, a crucial distinction when the categories are not 
        /// of uniform width. The categories are usually specified as non-overlapping intervals of some variable. 
        /// The categories (bars) must be adjacent.
        /// </summary>
        Histogram,
        /// <summary>
        /// Enum value for FastScatter chartType
        /// </summary>
        FastScatter,
        /// <summary>
        /// Enum value for FastColumn ChartType
        /// </summary>
        FastColumn,
        /// <summary>
        /// Enum value for Custom chartType
        /// </summary>
        Custom
    }

    /// <summary>
    /// Enum values for ChartPolatDrawType
    /// </summary>
    public enum ChartPolarDrawType
    {
        /// <summary>
        /// Draw the Filled Area in the Polar Chart type
        /// </summary>
        Area,

        /// <summary>
        /// Draw the Lines in the Polar chart type
        /// </summary>
        Line,

        /// <summary>
        /// Draw the Symbol to plot the Polar chart points.
        /// </summary>
        Symbol
    }

    /// <summary>
    /// Enum values for ChartAlignment
    /// </summary>
    public enum ChartAlignment
    {
        /// <summary>
        /// Sets element to the closest left top corner.
        /// </summary>
        Near,

        /// <summary>
        /// Sets element to the center.
        /// </summary>
        Center,

        /// <summary>
        /// Sets element to the closest right bottom corner.
        /// </summary>
        Far
    }

    /// <summary>
    /// Represents legends position in chart area
    /// </summary>
    public enum ChartDock
    {
        /// <summary>
        /// Docks element at the left side of panel.
        /// </summary>
        Left,

        /// <summary>
        /// Docks element at the top side of panel.
        /// </summary>
        Top,

        /// <summary>
        /// Docks element at the right side of panel.
        /// </summary>
        Right,

        /// <summary>
        /// Docks element at the bottom side of panel.
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// Represents Chart axis value type 
    /// of <see cref="ChartAxis"/>.
    /// </summary>
    public enum ChartValueType
    {
        /// <summary>
        ///  Double Axis Label Value Type.
        /// </summary>
        Double,

        /// <summary>
        ///  DateTime Axis Label Value Type.
        /// </summary>
        DateTime,

        /// <summary>
        ///   String Axis Label value Type.
        /// </summary>
        String,

        /// <summary>
        /// Logarithmic axis label value Type.
        /// </summary>
        Logarithmic
    }

    #region enum ChartLabelIntersectAction
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
        /// Labels are rotated to avoid intersection. property doesn't make effect in this mode.
        /// </summary>
        Wrap,

        /// <summary>
        /// Labels are wrapped into multiple rows to avoid intersection.
        /// </summary>
        MultipleRows,

        /// <summary>
        /// Labels are hidden to avoid intersection.
        /// </summary>
        Hide,

        /// <summary>
        /// Labels are rotated to avoid intersection.
        /// </summary>
        Rotate
    }
    #endregion

    /// <summary>
    /// Represents EdgeLabelsDrawingMode enumeration that is used to set behavior of edged labels 
    /// of <see cref="ChartAxis"/>.
    /// </summary>
    public enum EdgeLabelsDrawingMode
    {
        /// <summary>
        /// Value indicating that the edge label should appear at the center of its GridLines.
        /// </summary>
        Center,

        /// <summary>
        /// Value indicating that edge Label should be shifted to either left or right so that it comes with in the Chart Area.
        /// </summary>
        Shift
    }

    #region enum Symbol
    /// <summary>
    /// Symbol Enum to set Predefined and User-defined SymbolTemplates
    /// </summary>
    public enum Symbol
    {
        /// <summary>
        /// Custom option to set User-defined SymbolTemplates
        /// </summary>
        Custom,

        /// <summary>
        /// Renders Ellipse symbol
        /// </summary>
        Ellipse,

        /// <summary>
        /// Renders Cross symbol
        /// </summary>
        Cross,

        /// <summary>
        /// Renders Diamond symbol
        /// </summary>
        Diamond,

        /// <summary>
        /// Renders Hexagon symbol
        /// </summary>
        Hexagon,

        /// <summary>
        /// Renders HorizontalLine symbol
        /// </summary>
        HorizontalLine,

        /// <summary>
        /// Renders InvertedTriangle symbol
        /// </summary>
        InvertedTriangle,

        /// <summary>
        /// Renders Pentagon symbol
        /// </summary>
        Pentagon,

        /// <summary>
        /// Renders Plus symbol
        /// </summary>
        Plus,

        /// <summary>
        /// Renders Square symbol
        /// </summary>
        Square,

        /// <summary>
        /// Renders Traingle symbol
        /// </summary>
        Triangle,

        /// <summary>
        /// Renders VerticalLine symbol
        /// </summary>
        VerticalLine,
    }
    #endregion

    /// <summary>
    /// Enumeration represents series' adorner label content.
    /// </summary>
    public enum LabelContent
    {
        /// <summary>
        /// Identifies that label should contain X value of series' point.
        /// </summary>
        XValue,

        /// <summary>
        /// Identifies that label should contain Y value of series' point.
        /// </summary>
        YValue,

        /// <summary>
        /// Identifies that label should contain percentage value of series' point among other points.
        /// </summary>
        Percentage,

        /// <summary>
        /// Identifies that label should contain value of Y of total values.
        /// </summary>
        YofTot,

        /// <summary>
        /// Identifies that label should contain <see cref="DateTime"/> value.
        /// </summary>
        DateTime,

        /// <summary>
        /// Label's content will be retrieved from the <see cref="ChartAdornmentInfo.LabelContentPath"/> property.
        /// </summary>
        LabelContentPath
    }

    /// <summary>
    /// Represents label's connector alignment.
    /// </summary>
    public enum ConnectorAlignment
    {
        /// <summary>
        /// Connection to the left side of adorner.
        /// </summary>
        Left,

        /// <summary>
        /// Connection to the top side of adorner. 
        /// </summary>
        Top,

        /// <summary>
        /// Connection to the right side of adorner.  
        /// </summary>
        Right,

        /// <summary>
        /// Connection to the top bottom of adorner. 
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Represents AdornmentsPosition enumeration that is used to set behavior 
    /// <see cref="ChartAdornment"/>. 
    /// </summary>
    /// <seealso cref="ChartAdornmentInfo.AdornmentsPosition"/>
    public enum AdornmentsPosition
    {
        /// <summary>
        /// All adornments are positioned at the top of column.
        /// </summary>
        Top,

        /// <summary>
        /// All adornments are positioned at the bottom of column.
        /// </summary>
        Bottom,

        /// <summary>
        /// Adornments will be positioned depending on Y value that is represented by column.
        /// </summary>
        TopAndBottom
    }

    /// <summary>
    /// Represents Annotation Shapes
    /// </summary>
    public enum AnnotationShapes
    {
        /// <summary>
        /// Reperesents the AnnotationShape Ellipse
        /// </summary>
        Ellipse,
        /// <summary>
        /// Reperesents the AnnotationShape Rectangle
        /// </summary>
        Rectangle,
        /// <summary>
        /// Reperesents the AnnotationShape RoundedRectangle
        /// </summary>
        RoundedRectangle,
        /// <summary>
        /// Reperesents the AnnotationShape Arrow
        /// </summary>
        Arrow,
        /// <summary>
        /// Reperesents the AnnotationShape Inverted Arrow
        /// </summary>
        InvertedArrow,
        /// <summary>
        /// Reperesents the AnnotationShape Circle
        /// </summary>
        Circle,
        /// <summary>
        /// Reperesents the AnnotationShape Cross
        /// </summary>
        Cross,
        /// <summary>
        /// Reperesents the AnnotationShape Horizontal Line
        /// </summary>
        HorizontalLine,
        /// <summary>
        /// Reperesents the AnnotationShape Vertical Line
        /// </summary>
        VerticalLine,
        /// <summary>
        /// Reperesents the AnnotationShape Diamond
        /// </summary>
        Diamond,
        /// <summary>
        /// Reperesents the AnnotationShape Square
        /// </summary>
        Square,
        /// <summary>
        /// Reperesents the AnnotationShape Hexagon
        /// </summary>
        Hexagon,
        /// <summary>
        /// Reperesents the AnnotationShape Pentagon
        /// </summary>
        Pentagon,
        /// <summary>
        /// Reperesents the AnnotationShape Star
        /// </summary>
        Star,
        /// <summary>
        /// Reperesents the AnnotationShape None
        /// </summary>
        None
    }
    /// <summary>
    /// Enum value for ChartLegendIcons
    /// </summary>
    public enum ChartLegendIcon
    {
        /// <summary>
        /// Enum value for None type
        /// </summary>
        None,
        /// <summary>
        /// Enum value for SeriesType
        /// </summary>
        SeriesType,
        /// <summary>
        /// enum value for Rectangle type
        /// </summary>
        Rectangle,
        /// <summary>
        /// enum value for Straightline type
        /// </summary>
        StraightLine,
        /// <summary>
        /// Enum value for Circle type
        /// </summary>
        Circle,
        /// <summary>
        /// Enum  value for Diamond type
        /// </summary>
        Diamond,
        /// <summary>
        /// Enum value for Pentagon type
        /// </summary>
        Pentagon,
        /// <summary>
        /// Enum value for Triangular type
        /// </summary>
        Triangle,
        /// <summary>
        /// Enum value for Invertedtriangle type
        /// </summary>
        InvertedTriangle,
        /// <summary>
        /// Enum value for cross type
        /// </summary>
        Cross
    }
    #region enum ChartAreaType
    /// <summary>
    /// Identifies axes types enumeration.
    /// </summary>
    /// <example>
    /// Intended for internal use
    /// </example>
    public enum ChartAxesType
    {
        /// <summary>
        /// Represents Null Template
        /// </summary>
        Null,

        /// <summary>
        /// Represents No axis.
        /// </summary>
        None,

        /// <summary>
        /// Cartesian axis.
        /// </summary>
        CartesianAxes,

        /// <summary>
        /// Radar axis.
        /// </summary>
        RadarAxes,

        /// <summary>
        /// Radar axis.
        /// </summary>
        PolarAxes,
    }
    #endregion

    /// <summary>
    /// Enum values for ContextMenutypes
    /// </summary>
    public enum ContextMenuTypes
    {
        /// <summary>
        /// Enum value for default mode
        /// </summary>
        Default,
        /// <summary>
        /// Enum value for DefaultWithCustom
        /// </summary>
        DefaultWithCustom,
        /// <summary>
        /// Enum value for Custom mode
        /// </summary>
        Custom
    }
    /// <summary>
    /// Enum values for IndicatorTypes
    /// </summary>
    public enum IndicatorTypes
    {
        /// <summary>
        /// Enum value for simpleAverage mode
        /// </summary>
        SimpleAverage,
        /// <summary>
        /// Enum value for BollingBands mode
        /// </summary>

        BollingerBands,        
        /// <summary>
        /// Enum value for Triangular mode
        /// </summary>
        TriangularAverage
    }

    /// <summary>
    /// enum values for Labels in axisPositions
    /// </summary>
    public enum AxisPositions
    {
        /// <summary>
        /// Enum value for Inside position
        /// </summary>
        Inside,
        /// <summary>
        /// Enum value for Outside position
        /// </summary>
        Outside,
        /// <summary>
        /// Enum value for Cross position
        /// </summary>
        Cross
    }

    /// <summary>
    /// enum values for Labels in LabelPositions
    /// </summary>
    public enum LabelPositions
    {
        /// <summary>
        /// Enum value for Inside position
        /// </summary>
        Inside,
        /// <summary>
        /// Enum value for Outside position
        /// </summary>
        Outside,
    }

    /// <summary>
    /// Enum values for AxisLabels position
    /// </summary>
    public enum AxisLabels
    {
        /// <summary>
        /// Enum value for Low position
        /// </summary>
        Low,

        /// <summary>
        /// Enum value for High position
        /// </summary>
        High,

        /// <summary>
        /// Enum value for NextToAxis position
        /// </summary>
        NextToAxis
    }

    #region enum Sorting

    /// <summary>
    /// Enum values for ChartAxis labels Flow Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Enum  value for Ascending order
        /// </summary>
        Ascending,
        /// <summary>
        /// Enum value for Descending order
        /// </summary>
        Descending
    }
    /// <summary>
    /// Enum values for SortingAxis
    /// </summary>
    public enum SortingAxis
    {
        /// <summary>
        /// Enum value for X axis
        /// </summary>
        X,
        /// <summary>
        /// Enum value for Y axis 
        /// </summary>
        Y,
        /// <summary>
        /// Enum  value for XY axis
        /// </summary>
        XY
    }
    #endregion
	
    /// <summary>
    /// Enum values for Adornments mode.
    /// </summary>
    public enum AdornmentMode
    {
        /// <summary>
        /// Enum value for Radial
        /// </summary>
        Radial,
        /// <summary>
        /// Enum value for Horizontal
        /// </summary>
        Horizontal
    }


    /// <summary>
    /// Enum value for AlternatingFillMode
    /// </summary>
    public enum AlternatingFillMode
    {
        /// <summary>
        /// Enum value for Odd mode
        /// </summary>
        Odd,
        /// <summary>
        /// Enum value for Even mode
        /// </summary>
        Even
    }
    /// <summary>
    /// Enum values for WaterMarkTypes 
    /// </summary>
	public enum WatermarkTypes
    {
        /// <summary>
        /// Enum value for Text WaterMarKType
        /// </summary>
        Text,
        /// <summary>
        /// Enum value for Image WaterMarkType
        /// </summary>
        Image
    }
    /// <summary>
    /// Enum values for ScaleBreakModes
    /// </summary>
	public enum ScaleBreaksModes
    {
        /// <summary>
        /// Enum value fopr Auto mode
        /// </summary>
        Auto,
        /// <summary>
        /// Enum value for Manual Mode
        /// </summary>
        Manual,
        /// <summary>
        /// Enum value for None mode
        /// </summary>
        None
    }
    /// <summary>
    /// Enum values for ScaleBreakLineTypes
    /// </summary>
    public enum ScaleBreakLineTypes
    {
        /// <summary>
        /// Enum value for Straightline type
        /// </summary>
        StraightLine,
        /// <summary>
        /// Enum value for Wave type
        /// </summary>
        Wave,
        /// <summary>
        /// Enum value for Randomize type
        /// </summary>
        Randomize
    }


}
