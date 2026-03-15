// <copyright file="ChartEnums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    #region enum ChartAlignment
    /// <summary>
    /// Represents the chart alignment options.
    ///
    /// Enumeration represents value that can be set on <see cref="CameraProjection"/> property 
    /// when ChartArea is in 3D mode
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
    #endregion

    #region enum ChartGridCalcCellType
    /// <summary>
    /// Represents ChartGridCalcCellType Enumeration
    /// </summary>
    public enum ChartGridCalcCellType
    {
        /// <summary>
        /// The none value
        /// </summary>
        None,

        /// <summary>
        /// The auto set value
        /// </summary>
        Auto,

        /// <summary>
        /// The children
        /// </summary>
        Children
    }

    #endregion
    
    #region enum ChartValueType
    /// <summary>
    /// Specifies the different values that are natively used.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    public enum ChartValueType
    {
        /// <summary>
        ///  <see cref="Double"/> value
        /// </summary>
        Double,

        /// <summary>
        ///  <see cref="DateTime"/> value
        /// </summary>
        DateTime,

        /// <summary>
        ///   <see cref="String"/> value
        /// </summary>
        String,

        /// <summary>
        ///   <see cref="TimeSpan"/> value
        /// </summary>
        TimeSpan,

        /// <summary>
        ///   Logarithmic value
        /// </summary>
        Logarithmic
    }
    #endregion
    
    #region enum ChartColorPalette

    /// <summary>
    /// Pre-defined palettes for use with the ChartControl. Palettes are simply a group of colors that
    /// can be used to provide a better visual appearance when displaying multiple chart series.
    /// </summary>    
    public enum ChartColorPalette
    {

        /// <summary>
        /// Default palette.
        /// </summary> 
        Default,

        /// <summary>
        /// Default dark palette.
        /// </summary> 
        DefaultDark,

        /// <summary>
        /// Default palette with alpha blending.
        /// </summary>
        DefaultAlpha,

        /// <summary>
        /// Palette containing earth tone colors.
        /// </summary>
        EarthTone,

        /// <summary>
        /// Palette containing analog colors.
        /// </summary>
        Analog,

        /// <summary>
        /// Colorful palette.
        /// </summary>
        Colorful,

        /// <summary>
        /// Palette containing the colors of nature.
        /// </summary>
        Nature,

        /// <summary>
        /// Palette containing pastel colors.
        /// </summary>
        Pastel,

        /// <summary>
        /// Palette containing triad colors.
        /// </summary>
        Triad,

        /// <summary>
        /// Palette that contains mixed warm and cold colors.
        /// </summary>
        WarmCold,

        /// <summary>
        /// GrayScale color palette which can be used for monochrome printing.
        /// </summary>
        Grayscale,

        /// <summary>
        /// Office 2007 Blue palette.
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Office 2007 Black palette
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Office 2007 Silver palette
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Gradient palette
        /// </summary>
        Gradient,

        /// <summary>
        /// MixedGray palette
        /// </summary>
        MixedGray,

        /// <summary>
        /// BlueScale palette
        /// </summary>
        BlueScale,

        /// <summary>
        /// MaroonRed palette
        /// </summary>
        MaroonRed,

        /// <summary>
        /// GreenScale palette
        /// </summary>
        GreenScale,

        /// <summary>
        /// MixedViolet palette
        /// </summary>
        MixedViolet,

        /// <summary>
        /// CoolBlueScale palette
        /// </summary>
        CoolBlueScale,

        /// <summary>
        /// Chocolateorange palette
        /// </summary>
        ChocolateOrange,

        /// <summary>
        /// MixedFantasy palette
        /// </summary>
        MixedFantasy,

        /// <summary>
        /// Metro palette
        /// </summary>
        Metro,

        /// <summary>
        /// Custom user assigned color palette.
        /// </summary>
        Custom,



    }

    #endregion

    #region enum ChartStyles

    /// <summary>
    /// Enum values for ChartStyles
    /// </summary>
    public enum ChartStyles
    {

        /// <summary>
        /// GrayScale ChartStyles
        /// </summary>
        GrayScale = 0,

        /// <summary>
        /// MixedFantasy ChartStyle
        /// </summary>
        MixedFantacy = 1,

        /// <summary>
        /// blueScale ChartStyle
        /// </summary>
        BlueScale = 2,

        /// <summary>
        /// MaroonRed ChartStyle
        /// </summary>
        MaroonRed = 3,

        /// <summary>
        /// GreenScale ChartStyle
        /// </summary>
        GreenScale = 4,

        /// <summary>
        /// MixedViolet ChartStyle
        /// </summary>
        MixedViolet = 5,

        /// <summary>
        /// CoolBlueScale ChartStyle
        /// </summary>
        CoolBlueScale = 6,

        /// <summary>
        /// ChocolateOrange ChartStyle
        /// </summary>
        ChocolateOrange = 7,

        /// <summary>
        /// GrayWithBorder ChartStyle
        /// </summary>
        GrayWithBorder = 8,

        /// <summary>
        /// MixedWithBorder ChartStyle
        /// </summary>
        MixedWithBorder = 9,

        /// <summary>
        /// BluWithBorder ChartStyle
        /// </summary>
        BlueWithBorder = 10,

        /// <summary>
        /// RedWithBoder ChartStyle
        /// </summary>
        RedWithBorder = 11,

        /// <summary>
        /// GreenWithBorder ChartStyle
        /// </summary>
        GreenWithBorder = 12,

        /// <summary>
        /// VioletWithBorder ChartStyle
        /// </summary>
        VioletWithBorder = 13,

        /// <summary>
        /// CoolBlueWithBorder ChartStyle
        /// </summary>
        CoolBlueWithBorder = 14,

        /// <summary>
        /// ChocolateWithBorder ChartStyle
        /// </summary>
        ChocolateWithBorder = 15,

        /// <summary>
        /// AlphaGray ChartStyle
        /// </summary>
        AlphaGray = 16,

        /// <summary>
        /// AlphaFantacy ChartStyle
        /// </summary>
        AlphaFantacy = 17,

        /// <summary>
        /// AlphaBlue ChartStyle
        /// </summary>
        AlphaBlue = 18,

        /// <summary>
        /// AlphaRed ChartStyle
        /// </summary>
        AlphaRed = 19,

        /// <summary>
        /// AlphaGreen ChartStyle
        /// </summary>
        AlphaGreen = 20,

        /// <summary>
        /// AlphaViolet ChartStyle
        /// </summary>
        AlphaViolet = 21,

        /// <summary>
        /// AlphaCoolBlue ChartStyle
        /// </summary>
        AlphaCoolBlue = 22,

        /// <summary>
        /// AlphaOrange ChartStyle
        /// </summary>
        AlphaOrange = 23,

        /// <summary>
        /// EnabledGray ChartStyle
        /// </summary>
        EnabledGray = 24,

        /// <summary>
        /// EnabledMixed ChartStyle
        /// </summary>
        EnabledMixed = 25,

        /// <summary>
        /// EnabledBlue ChartStyle
        /// </summary>
        EnabledBlue = 26,

        /// <summary>
        /// Enabledred ChartStyle
        /// </summary>
        EnabledRed = 27,

        /// <summary>
        /// EnabledGreen ChartStyle
        /// </summary>
        EnabledGreen = 28,

        /// <summary>
        /// EnabledGreen ChartStyle.
        /// </summary>
        EnabledViolet = 29,

        /// <summary>
        /// EnabledCoolBlue ChartStyle
        /// </summary>
        EnabledCoolBlue = 30,

        /// <summary>
        /// EnabledChocolate ChartStyle
        /// </summary>
        EnabledChocolate = 31,

        /// <summary>
        /// GrayScreen ChartStyle
        /// </summary>
        GrayScreen = 32,

        /// <summary>
        /// MixedScreen ChartStyle
        /// </summary>
        MixedScreen = 33,

        /// <summary>
        /// BlueScreen ChartStyle
        /// </summary>
        BlueScreen = 34,

        /// <summary>
        /// RedScreen ChartStyle
        /// </summary>
        RedScreen = 35,

        /// <summary>
        /// GreenScreen ChartStyle
        /// </summary>
        GreenScreen = 36,

        /// <summary>
        /// VioletScreen ChartStyle
        /// </summary>
        VioletScreen = 37,

        /// <summary>
        /// CoolBlueScreen ChartStyle
        /// </summary>
        CoolBlueScreen = 38,

        /// <summary>
        /// ChocolateScreen ChartStyle
        /// </summary>
        ChocolateScreen = 39,

        /// <summary>
        /// BlendGray ChartStyle
        /// </summary>
        BlendGray = 40,

        /// <summary>
        /// MixedBlend ChartStyle
        /// </summary>
        MixedBlend = 41,

        /// <summary>
        /// BlueBlend ChartStyle
        /// </summary>
        BlueBlend = 42,

        /// <summary>
        /// RedBlend ChartStyle
        /// </summary>
        RedBlend = 43,

        /// <summary>
        /// GreenBlend ChartStyle
        /// </summary>
        GreenBlend = 44,

        /// <summary>
        /// VioletBlend ChartStyle
        /// </summary>
        VioletBlend = 45,

        /// <summary>
        /// CoolBlueBlend ChartStyle
        /// </summary>
        CoolBlueBlend = 46,

        /// <summary>
        /// ChocolateBlend ChartStyle
        /// </summary>
        ChocolateBlend = 47,

        /// <summary>
        /// Default ChartStyle
        /// </summary>
        Default = 48,

        /// <summary>
        /// AeroNormalColor ChartStyle
        /// </summary>
        AeroNormalColor = 49,

        /// <summary>
        /// LunaMetallic ChartStyle
        /// </summary>
        LunaMetallic = 50,

        /// <summary>
        /// LunaNormalColor ChartStyle
        /// </summary>
        LunaNormalColor = 51,

        /// <summary>
        /// LunaHomestead ChartStyle
        /// </summary>
        LunaHomestead = 52,

        /// <summary>
        /// RoyaleNormalColor ChartStyle
        /// </summary>
        RoyaleNormalColor = 53,

        /// <summary>
        /// ZuneNormalColor ChartStyle
        /// </summary>
        ZuneNormalColor = 54,

        /// <summary>
        /// CoolBlue ChartStyle
        /// </summary>
        CoolBlue = 55,

        /// <summary>
        /// BlueWave ChartStyle
        /// </summary>
        BlueWave = 56,

        /// <summary>
        /// ChocolateYellow ChartStyle
        /// </summary>
        ChocolateYellow = 57,

        /// <summary>
        /// SpringGreen ChartStyle
        /// </summary>
        SpringGreen = 58,

        /// <summary>
        /// BrightGray ChartStyle
        /// </summary>
        BrightGray = 59,

        /// <summary>
        /// Blend ChartStyle
        /// </summary>
        Blend = 60,

        /// <summary>
        /// ForestGreen ChartStyle
        /// </summary>
        ForestGreen = 61,

        /// <summary>
        /// LawnGreen ChartStyle
        /// </summary>
        LawnGreen = 62,

        /// <summary>
        /// MixedGreen ChartStyle
        /// </summary>
        MixedGreen = 63,

        /// <summary>
        /// OrangeRed ChartStyle
        /// </summary>
        OrangeRed = 64,

        /// <summary>
        /// Office2007Blue ChartStyle
        /// </summary>
        Office2007Blue = 65,

        /// <summary>
        /// Office2007Black ChartStyle
        /// </summary>
        Office2007Black = 66,

        /// <summary>
        /// Office2007Silver ChartStyle
        /// </summary>
        Office2007Silver = 67,

        /// <summary>
        /// Office2003 ChartStyle
        /// </summary>
        Office2003 = 68,

        /// <summary>
        /// VS2010 ChartStyle
        /// </summary>
        Vs2010 = 69,

        /// <summary>
        /// Metro  ChartStyle
        /// </summary>
        Metro = 70,

        /// <summary>
        /// None ChartStyle
        /// </summary>
        None

    }

    #endregion
    
    #region enum RangeCalculationMode
    /// <summary>
    ///  Specifies the options for the action that is to be taken when labels intersect each other.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
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

    #region enum ChartLabelIntersectAction
    /// <summary>
    ///  Specifies the options for the action that is to be taken when labels intersect each other.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    public enum ChartLabelIntersectAction
    {
        /// <summary>
        /// No special action is taken. Labels may intersect.
        /// </summary>
        None,

        /// <summary>
        /// Labels are rotated to avoid intersection. <see cref="ChartAxis.LabelRotateAngle"/> property doesn't make effect in this mode.
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
        /// Represents No axis.
        /// </summary>
        None,

        /// <summary>
        /// Cartesian axis.
        /// </summary>
        CartesianAxes,

        /// <summary>
        /// Polar axis.
        /// </summary>
        PolarAxes
    }
    #endregion

    #region enum Symbol
    /// <summary>
    /// Symbol Enum to set Predefined and User-defined SymbolTemplates
    /// </summary>
    /// <seealso cref="ChartAdornmentInfo"/>
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

    #region enum Mode
    /// <summary>
    /// Enum values for Symbol Mode
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Enum value for Fixed mode
        /// </summary>
        Fixed,
        /// <summary>
        /// Enum values for relative Mode
        /// </summary>
        Relative
    }

    #endregion

    #region enum EmptyPointStyle
    /// <summary>
    /// EmptyPointStyle Enum to make difference for Empty Points
    /// </summary>
    /// <seealso cref="ChartSeries"/>
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

    #region enum ChartAxisCap
    /// <summary>
    /// Represents chart axis end line caps.
    /// </summary>
    /// <seealso cref="ChartCartesianAxisElement"/>
    [Flags]
    public enum ChartAxisCap
    {
        /// <summary>
        /// Represents No cap.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Start arrow is drawn at the end of axis.
        /// </summary>
        StartArrow = 0x01,

        /// <summary>
        /// End arrow is drawn at the end of axis.
        /// </summary>
        EndArrow = 0x02,

        /// <summary>
        /// Start and end arrows are drawn at the ends of axis.
        /// </summary>
        Arrows = StartArrow | EndArrow
    }
    #endregion

    #region enum EmptyPointValue
    /// <summary>
    /// Enum values for Emplty point visual State
    /// </summary>
    public enum EmptyPointValue
    {
        /// <summary>
        /// Enum value for Zero State
        /// </summary>
        Zero,

        /// <summary>
        /// Enum value for Average State
        /// </summary>
        Average
    }
    #endregion

    #region enum ChartTypes
    /// <summary>
    /// Enumeration represents all built-in chart types. Different sets of data are
    /// particularly suited to a certain chart type.
    /// </summary>
    /// <seealso cref="Chart"/>
    public enum ChartTypes
    {
        /// <summary>
        /// A type of presentation graphic that emphasizes a change in values by filling in
        /// the portion of the graph beneath the line connecting various data points.
        /// </summary>
        Area,

        /// <summary>
        /// A bar chart, also known as a bar graph, is a chart with rectangular bars of lengths 
        /// proportional to that value that they represent. 
        /// Bar charts are used for comparing two or more values.
        /// </summary>    
        Bar,

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
        /// A bubble chart draws bubbles for each point in a series. 
        /// The chart expects three values per bubble: the domain (commonly the X-axis) value and, 
        /// the range (commonly the y-axis) value.
        /// </summary>
        Bubble,

        /// <summary>
        /// A candlestick chart is a style of bar-chart used primarily to describe price movements of an equity over time.
        /// It is a combination of a line-chart and a bar-chart, in that each bar represents the range of price movement 
        /// over a given time interval. It is most often used in technical analysis of equity and currency price patterns. 
        /// It appears superficially similar to error bars, but are unrelated.
        /// </summary>
        Candle,

        /// <summary>
        /// Compares values across categories. The series values are displayed as individual columns, grouped by category. The height of each column is determined by the series value.
        /// </summary>
        /// <remarks>
        /// There are three types of column charts: column, <see cref="ChartTypes.StackingColumn"/>, 
        /// and <see cref="ChartTypes.RangeColumn"/>.
        /// </remarks>
        Column,

        /// <summary>
        /// A doughnut chart displays value data as percentages of the whole. 
        /// Categories are represented by individual slices. Doughnut charts are typically used to show percentages. 
        /// Doughnut charts are functionally identical to pie charts.
        /// </summary>
        Doughnut,

        /// <summary>
        /// When performance is critical, the FastLine chart type is a good alternative to the Line chart. 
        /// FastLine chart significantly reduce the drawing time of a series that contains a very large number of data points.
        /// </summary>
        FastLine,

        /// <summary>
        /// When performance is critical, the FastBar chart type is a good alternative to the Bar chart. 
        /// FastBar chart significantly reduce the drawing time of a series that contains a very large number of data points.
        /// </summary>
        FastBar,

        /// <summary>
        /// When performance is critical, the FastSpline chart type is a good alternative to the Spline chart. 
        /// FastSpline chart significantly reduce the drawing time of a series that contains a very large number of data points.
        /// </summary>
        FastSpline,

        /// <summary>
        /// The Funnel Chart type displays data that equals 100% when totalled. 
        /// This type of chart is a single series chart representing the data as portions of 100%, 
        /// and this chart does not use any axes.
        /// </summary>
        Funnel,

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
        /// HiLo chart is a sophisticated chart type, that finds its place in stock analysis. 
        /// </summary>
        /// <remarks>
        /// Key Features:
        /// <para/>* Display of Error Bars.
        /// <para/>* Trading range of stock and its closing value, for each period.
        /// <para/>* Displays Y - value for each X - value.
        /// <para/>* In special cases, displays a range of Y - values for a given X value.
        /// </remarks>
        HiLo,

        /// <summary>
        /// Represents the HiLoArea chart type
        /// </summary>
        HiLoArea,

        /// <summary>
        /// HiLo OpenClose Chart is a HiLo chart with four Y values for each point.
        /// </summary>
        /// <remarks>
        /// The functionality of each Y value is given below:
        /// <para/>* One representing the high value of the plotted stock, for the time period for which the chart is being rendered.
        /// <para/>* One representing the low values for the same period.
        /// <para/>* One representing the opening value for that period.
        /// <para/>* One representing the closing value for that period.
        /// </remarks>
        HiLoOpenClose,

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
        /// A Kagi chart is created with a series of vertical lines connected by short horizontal lines. 
        /// The thickness and direction of the lines is based on the price of the underlying stock or asset
        /// </summary>
        Kagi,

        /// <summary>
        /// A style of chart that is created by connecting a series of data points together with a line. 
        /// This is the most basic type of chart used in finance and it is generally created by connecting a 
        /// series of past prices together with a line.
        /// </summary>
        Line,

        /// <summary>
        /// A pie chart (or a circle graph) is a circular chart divided into sectors, illustrating relative magnitudes 
        /// or frequencies or percents. In a pie chart, the arc length of each sector (and consequently its central 
        /// angle and area), is proportional to the quantity it represents. Together, the sectors create a full disk. 
        /// It is named for its resemblance to a pie which has been sliced.
        /// </summary>
        Pie,

        /// <summary>
        /// A point and figure chart is used for technical analysis of securities. Unlike most other investment charts, 
        /// point and figure charts do not present a linear representation of time. Instead, they show trends in price.
        /// <para/>The aim of point and figure charting is to filter out the "noise" (unimportant price movement) and 
        /// focus on the main direction of the price trend. 
        /// <para/>Point and figure charts are usually used for longer term price movements, but may be used to day 
        /// trade by trying to identify the key points of "supply and demand." Point and figure charts are close 
        /// relatives to <see cref="ChartTypes.ThreeLineBreak"/> , <see cref="ChartTypes.Renko"/> 
        /// and <see cref="ChartTypes.Kagi"/> charts which all do not have a fixed time frame.
        /// </summary>
        PointAndFigure,

        /// <summary>
        /// Polar chart is used to display chart data points, connected with a line, in a polar co-ordinate system. 
        /// </summary>
        Polar,

        /// <summary>
        /// Pyramid charts are another type of accumulation chart which has a triangular upper surface that 
        /// converge at one point. Similar to a <see cref="ChartTypes.Funnel"/> chart, 
        /// the height of a segment is proportional to the Y value of the corresponding point.
        /// </summary>
        Pyramid,

        /// <summary>
        /// A radar chart is two-dimensional chart of three or more quantitative variables represented on axes starting from the same point. The relative position and angle of the axes is uninformative.
        /// Radar charts are usually used to compare performance of different entities on a same set of axes.
        /// </summary>
        Radar,

        /// <summary>
        /// Range Area Chart is a variation of Area Chart type that lets you plot bands of data in a chart, like Bollinger bands, weather patterns, etc. 
        /// Each point in the chart is specified by 2 Y values � the lower and higher end of the band. 
        /// </summary>
        RangeArea,

        /// <summary>
        /// Columns connecting minimum and maximum series points with the same respective X. 
        /// </summary>
        RangeColumn,

        /// <summary>
        /// A renko chart is constructed by placing a brick in the next column once the price surpasses the top or 
        /// bottom of the previous brick by a pre-defined amount. Lighter bricks are used when the direction of the 
        /// trend is up, while darker bricks are used when the trend is down. This type of chart is very effective 
        /// for traders to identify key support/resistance levels. 
        /// Transaction signals are generated when the direction of the trend changes and the bricks alternate colors.
        /// </summary>
        Renko,

        /// <summary>
        /// Rotated Spline chart defines chart series points as interconnected rotated smooth curves.
        /// </summary>
        RotatedSpline,

        /// <summary>
        /// A scatter type contains one or more scatter plots each of which use Cartesian coordinates 
        /// to display values for two series values for a set of data. The data is displayed as a collection 
        /// of points, each having the value of one variable determining the position on the horizontal axis 
        /// and the value of the other variable determining the position on the vertical axis.
        /// </summary>
        Scatter,

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
        /// StackingArea is an area chart with Y values stacked over one another, in series order. 
        /// </summary>
        StackingArea,

        /// <summary>
        /// StackingArea100 is an area chart with Y values stacked over one another for 100%, in series order. 
        /// </summary>
        StackingArea100,

        /// <summary>
        /// A stacked bar chart displays the relationship of individual items to the whole, 
        /// comparing the contributions of each value to a total across categories. 
        /// The series values are stacked in a single column for each category. 
        /// The height of each column is determined by the total of all series values for the category.
        /// </summary>
        StackingBar,

        /// <summary>
        /// A bar chart where multiple series are stacked horizontally to fit 100% of the chart area. 
        /// If there is only one series in chart, all the bars will fit to 100% of area.
        /// </summary>
        StackingBar100,

        /// <summary>
        /// A stacked column chart displays the relationship of individual items to the whole, 
        /// comparing the contributions of each value to a total across categories. 
        /// The series values are stacked in a single column for each category. 
        /// The height of each column is determined by the total of all series values for the category.
        /// </summary>
        StackingColumn,

        /// <summary>
        /// A column chart where multiple series are stacked vertically to fit 100% of the chart area. 
        /// If there is only one series in your chart, all the column bars will fit to 100% of the chart area.
        /// </summary>
        StackingColumn100,

        /// <summary>
        /// StackingLine is a line chart with Y values stacked over one another, in series order. 
        /// </summary>
        StackingLine,

        /// <summary>
        /// StackingLine100 is a line chart with Y values stacked over one another for 100%, in series order. 
        /// </summary>
        StackingLine100,

        /// <summary>
        /// StackingSpline is a spline chart with Y values stacked over one another, in series order.
        /// </summary>
        StackingSpline,

        /// <summary>
        /// StackingSpline100 is a spline chart with Y values stacked over one another for 100%, in series order. 
        /// </summary>
        StackingSpline100,

        /// <summary>
        /// StackingSplineArea is a splinearea chart with Y values stacked over one another, in series order. 
        /// </summary>
        StackingSplineArea,

        /// <summary>
        /// StackingSplineArea100 is a splinearea chart with Y values stacked over one another for 100%, in series order. 
        /// </summary>
        StackingSplineArea100,

        /// <summary>
        /// The StepArea chart consists of pairs of values (plotted as points) on a line for each series in the given 
        /// dataset. Each pair of values consists of one date (value) and one numeric value. 
        /// This chart will have one date\time (value) based axis (the domain axis) and one numeric axis (the range axis).
        /// Each line is drawn at a 90 degree angle from point to point. Each line will fill the from the line to the 
        /// bottom of the plot with its series color.
        /// </summary>
        StepArea,

        /// <summary>
        /// Step Line charts are line charts, with values drawn continuously, step by step without any gaps between them. 
        /// </summary>
        StepLine,

        /// <summary>
        /// 3D Surface Chart Type
        /// </summary>
        Surface3D,

        /// <summary>
        /// ThreeLineBreak displays a series of vertical boxes that are based on changes in prices. 
        /// It depicts rising and falling lines of varying heights. 
        /// </summary>
        ThreeLineBreak,

        /// <summary>
        /// Tornado chart is a bar chart, which shows the variability of an outcome, as the result of several factors. 
        /// This variability is displayed using relative lengths of bars across a range. 
        /// </summary>
        Tornado,

        /// <summary>
        /// FastColumn Chart is a Column Chart, Which Comes under FastChartType
        /// </summary>
        FastColumn,

        /// <summary>
        /// FastStackingColumn Chart is a Column Chart, Which Comes under FastChartType
        /// </summary>
        FastStackingColumn,

        /// <summary>
        ///FastScatter Chart is a Scatter Chart, Which Comes under FastChartType 
        /// </summary>
        FastScatter,

        /// <summary>
        /// FastHiLoOpenClose Chart is a Fast Chart Type.
        /// </summary>
        FastHiLoOpenClose,

        /// <summary>
        /// Custom Chart type
        /// </summary>
        Custom
    }
    #endregion

    #region enum ChartPolarDrawType
    /// <summary>
    /// Enum values for polarArea Charttypes
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
    #endregion

    #region enum ChartRadarDrawType
    /// <summary>
    /// Enum values for Radar area Charttypes
    /// </summary>
    public enum ChartRadarDrawType
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
    #endregion

    #region enum Direction
    /// <summary>
    /// Enum values for Visual Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Enum values for Ascending Order
        /// </summary>
        Ascending,
        /// <summary>
        /// Enum values for Descending Order
        /// </summary>
        Descending
    }
    #endregion

    #region enum SortingAxis
    /// <summary>
    /// Enum values for SortingAxis types.
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
        /// Enum value for XY axis
        /// </summary>
        XY
    }
    #endregion

    #region enum LocalizeEnumChartTypes
    /// <summary>
    /// The class creates an extension method on the actual enum "ChartColorPalette" which then allows
    /// you to call a ToFriendlyString() on the instance of all enums of that type
    /// Which could be used for localization
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class LocalizeEnumChartTypes
    {
        /// <summary>
        /// Returns a friendly enum name, used for localization
        /// </summary>
        /// <param name="chartTypeEnum">The chart type enum.</param>
        /// <returns>Returns string from ResourceDictionary</returns>
        public static string ToFriendlyString(this ChartTypes chartTypeEnum)
        {
            //ResourceDictionary resourceDictionaryItems = new ResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute)
            //};

            ChartResourceWrapper resourcesWrapper = new ChartResourceWrapper();
            //string str = resourceDictionaryItems[chartTypeEnum.ToString()] as string;
            object str = ChartDataUtils.GetPropertyDescriptor(resourcesWrapper, chartTypeEnum.ToString());

            if (str != null)
            {
                return str.ToString();
            }
            else
            {
                return chartTypeEnum.ToString();
            }
        }
    }
    #endregion

    #region enum LabelContent

    /// <summary>
    /// Enumeration represents series' adorner label content.
    /// </summary>
    /// <seealso>
    ///     <cref>AdornmentInfo</cref>
    /// </seealso>
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
    #endregion

    #region enum ConnectorAlignment

    /// <summary>
    /// Represents label's connector alignment.
    /// </summary>
    /// <seealso>
    ///     <cref>AdornmentPresenter</cref>
    /// </seealso>
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
    #endregion

    #region enum AlternatingFillMode
    /// <summary>
    /// Enumeration represents value that can be set on <see cref="ChartArea.AlternatingGridBackground"/> property 
    /// when alternating grid background lines are drawn.
    /// </summary>
    /// <seealso cref="ChartArea"/>
    public enum AlternatingFillMode
    {
        /// <summary>
        /// Alternating gridlines are drawn for odd axis intervals only.
        /// </summary>
        Odd,

        /// <summary>
        /// Alternating gridlines are drawn for even axis intervals only.
        /// </summary>
        Even
    }
    #endregion

    #region enum CameraProjection
    /// <summary>
    /// Enumeration represents value that can be set on <see cref="CameraProjection"/> property 
    /// when ChartArea is in 3D mode
    /// </summary>  
    /// <seealso cref="Chart3D"/>
    public enum CameraProjection
    {
        /// <summary>
        /// Represents Perspective CameraProjection
        /// </summary>
        Perspective,

        /// <summary>
        /// Represents Orthographic CameraProjection
        /// </summary>
        Orthographic
    }
    #endregion

    #region enum EdgeLabelsDrawingMode
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
        Shift,

        /// <summary>
        /// Value indicating that edge Label should be fit with in the Chart Area.
        /// </summary>
        Fit
    }
    #endregion

    #region enum AdornmentsPosition
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
    #endregion

    #region enum GridSide
    /// <summary>
    /// Represents Grid side
    /// </summary>
    /// <seealso cref="Chart3DGrid"/>
    internal enum GridSide
    {
        /// <summary>
        /// The Left side
        /// </summary>
        Left,

        /// <summary>
        /// The Bottom side
        /// </summary>
        Bottom,

        /// <summary>
        /// The Back side
        /// </summary>
        Back,

        /// <summary>
        /// The Right side
        /// </summary>
        Right,

        /// <summary>
        /// The top side
        /// </summary>
        Top
    }
    #endregion

    #region enum AutoDiscardType
    /// <summary>
    /// Represents the Range behavior of the  Axis.
    /// </summary>
    /// <seealso cref="ChartSeries"/>
    public enum AutoDiscardType
    {
        /// <summary>
        /// Default behaviour
        /// </summary>
        None,

        /// <summary>
        /// When the data reaches the max range it externed the range .
        /// </summary>
        ExtendRange,

        /// <summary>
        /// When the data reaches the max range it reset the range to a new range. 
        /// </summary>
        ResetRange
    }
    #endregion

    #region enum ChartLegendIcon
    /// <summary>
    /// Represents the Icon for the Chartlegend
    /// </summary>  
    /// <seealso cref="ChartSeries"/>
    public enum ChartLegendIcon
    {
        /// <summary>
        /// Default behaviour
        /// </summary>
        None,

        /// <summary>
        /// Represents the Icon of Series type
        /// </summary>
        SeriesType,

        /// <summary>
        /// Represents the Rectangular Icon
        /// </summary>
        Rectangle,

        /// <summary>
        ///Represents the Straight Line
        /// </summary>       
        StraightLine,

        /// <summary>
        /// Represents the Circle
        /// </summary>       
        Circle,

        /// <summary>
        /// Represents the Diamond
        /// </summary>
        Diamond,

        /// <summary>
        /// Represents the Pentagon
        /// </summary>      
        Pentagon,

        /// <summary>
        /// Represents the Triangle
        /// </summary>
        Triangle,

        /// <summary>
        /// Represents the Inverted Triangle
        /// </summary>   
        InvertedTriangle,

        /// <summary>
        /// Represents the Ellipse
        /// </summary>
        Ellipse,

        /// <summary>
        /// Represents the Cross
        /// </summary>       
        Cross,

        /// <summary>
        ///Represents the Horizontal Line
        /// </summary>       
        HorizontalLine
    }
    #endregion

    #region enum AnnotationShapes

    /// <summary>
    /// Represents the Shape for the Annotation
    /// </summary>
    /// <seealso>
    ///     <cref>ChartLabelAnnotation</cref>
    /// </seealso>
    public enum AnnotationShapes
    {

        /// <summary>
        /// Represents the Ellipse
        /// </summary>
        Ellipse,


        /// <summary>
        /// Represents the Rectangle
        /// </summary>     
        Rectangle,

        /// <summary>
        /// Represents the RoundedRectangle
        /// </summary>
        /// 
        RoundedRectangle,

        /// <summary>
        /// Represents the Arrow
        /// </summary>
        Arrow,

        /// <summary>
        /// Represents the Circle
        /// </summary>
        Circle,

        /// <summary>
        /// Represents the Cross
        /// </summary>
        Cross,

        /// <summary>
        /// Represents the Inverted Arrow
        /// </summary>
        InverteredArrow,

        /// <summary>
        /// Represents the Horizontal Line
        /// </summary>
        HorizontalLine,

        /// <summary>
        /// Represents the Vertical Line
        /// </summary>
        VerticalLine,

        /// <summary>
        /// Represents the Diamond
        /// </summary>
        Diamond,

        /// <summary>
        /// Represents the Square
        /// </summary>
        Square,

        /// <summary>
        /// Represents the Hexagon
        /// </summary>
        Hexagon,

        /// <summary>
        /// Represents the Pentagon
        /// </summary>
        Pentagon,

        /// <summary>
        /// Represents the Star
        /// </summary>
        Star,

        /// <summary>
        /// Default shape
        /// </summary>
        None


    }
    #endregion

    #region enum SpliterVisibility
    /// <summary>
    /// Represents the Splitter Viisbility
    /// </summary>  
    /// <seealso cref="ChartArea"/>
    public enum SpliterVisibility
    {
        /// <summary>
        /// Hides the Splitter
        /// </summary>  
        Hide,

        /// <summary>
        /// Splitter is in Visibility mode always
        /// </summary>
        ShowAlways,

        /// <summary>
        /// Splitter is visible when the Cursor is over the Splitter
        /// </summary>
        ShowOnMouseHover
    }
    #endregion

    #region enum AnimationOptions
    /// <summary>
    /// Display the different Animation options to animate Chart series Segment
    /// </summary>
    /// <seealso cref="ChartSeries"/>
    public enum AnimationOptions
    {
        /// <summary>
        /// Move the segments from the Below to Top
        /// </summary>
        Top,

        /// <summary>
        /// Move the Segments from Left to Right
        /// </summary>
        Left,

        /// <summary>
        /// Move the Segments from the Right to Left
        /// </summary>
        Right,

        /// <summary>
        /// Move the segments from the Top to Bottom
        /// </summary>
        Bottom,

        /// <summary>
        /// Rotate the segments
        /// </summary>
        Rotate,

        /// <summary>
        /// change the Opacity of the segments
        /// </summary>
        Fade,

        /// <summary>
        /// Perform the scalling of segments.
        /// </summary>
        Scaling
    }
    #endregion

    #region enum ChartAxisLabelsMode
    /// <summary>
    /// Represents chart axis labels modes.
    /// </summary>
    /// <remarks>
    /// Apart from the default Labels displayed, you can also add Custom Labels to be
    /// displayed in the Chart. 
    /// </remarks>
    /// <example>
    /// Below given code example illustrates adding labels from custom source
    /// <code lang="XAML">
    /// &lt;syncfusion:Chart Name="Chart1" &gt;
    ///           &lt;syncfusion:ChartArea Name="area" &gt;
    ///                     &lt;syncfusion:ChartArea.PrimaryAxis&gt;
    ///                        &lt;syncfusion:ChartAxis LabelsMode="Custom"
    /// RangeCalculationMode="AdjustAcrossChartTypes"&gt;
    ///                             &lt;syncfusion:ChartAxis.CustomLabels&gt;
    ///                                 &lt;syncfusion:ChartAxisLabel Content="III place"
    /// Position="0" /&gt;
    ///                                &lt;syncfusion:ChartAxisLabel Content="I place"
    /// Position="1" /&gt;
    ///                                 &lt;syncfusion:ChartAxisLabel Content="II place"
    /// Position="4" /&gt;
    ///                            &lt;/syncfusion:ChartAxis.CustomLabels&gt;
    ///                        &lt;/syncfusion:ChartAxis&gt;
    ///                    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;                 
    ///           &lt;/syncfusion:ChartArea&gt;
    /// &lt;/syncfusion:Chart&gt;    
    /// </code>
    /// <code lang="C#">
    /// // Indicates that the axis labels need to be taken from a custom source.
    /// area.PrimaryAxis.LabelsMode = ChartAxisLabelsMode.Custom;
    /// ChartAxisLabel customLabel1 = new ChartAxisLabel();
    /// customLabel1.Content = "III place";
    /// customLabel1.Position = 0;
    /// ChartAxisLabel customLabel2 = new ChartAxisLabel();
    /// customLabel2.Content = "I place";
    /// customLabel2.Position = 1;
    /// ChartAxisLabel customLabel3 = new ChartAxisLabel();
    /// customLabel3.Content = "II place";
    /// customLabel3.Position = 4;
    /// // Adding custom label to labels collection.
    /// area.PrimaryAxis.CustomLabels.Add(customLabel1);
    /// area.PrimaryAxis.CustomLabels.Add(customLabel2);
    /// area.PrimaryAxis.CustomLabels.Add(customLabel3); 
    /// </code>   
    /// </example>
    /// <seealso cref="ChartAxisLabel">ChartAxisLabel</seealso>
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
    #endregion

    #region enum ChartRangePaddingType
    /// <summary>
    /// Identifies range padding type enumeration.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
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
    #endregion

    #region enum ChartFunnelMode
    /// <summary>
    /// Lists the funnel mode options.
    /// </summary>
    /// <seealso cref="ChartFunnelType"/>
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
    #endregion

    #region enum ChartPyramidMode
    /// <summary>
    /// Specifies the mode in which the Y values should be interpreted in the Pyramid chart.
    /// </summary>
    /// <seealso cref="ChartPyramidType"/>
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
    #endregion

    #region enum ChartPointAndFigure
    /// <summary>
    /// Represents point and figure enumeration.
    /// </summary>
    /// <seealso cref="ChartPointAndFigureType"/>
    public enum ChartPointAndFigure
    {
        /// <summary>
        /// Determines that point should be drawn.
        /// </summary>
        Point,

        /// <summary>
        /// Determines that figure should be drawn.
        /// </summary>
        Figure
    }
    #endregion

    #region enum HeatMapLayoutMode

    /// <summary>
    /// Specifies the different layout modes that can be used to layout the items in a <see cref="HeatMapControl"/>.
    /// </summary>
    /// <seealso>
    ///     <cref>HeapMapControl</cref>
    /// </seealso>
    public enum HeatMapLayoutMode
    {
        /// <summary>
        /// Items will be laid out one after another vertically if there is more vertical space available or horizontally
        /// if there is more horizontal space available. The <see cref="HeatMapsPanel"/> type is used to host the items.
        /// </summary>
        SliceAndDiceAuto,

        /// <summary>
        /// Items will be laid out horizontally one after another. The <see cref="HorizontalSlicesPanel"/> type is used 
        /// to host the items.
        /// </summary>
        SliceAndDiceHorizontal,

        /// <summary>
        /// Items will be laid out vertically one after another. The <see cref="VerticalSlicesPanel"/> type is used 
        /// to host the items.
        /// </summary>
        SliceAndDiceVertical,

        /// <summary>
        /// Lays out child items within the available space in rectangles with aspect ratio that is closer to 1.
        /// </summary>
        Squarified
    }
    #endregion

    #region enum ChartDock
    /// <summary>
    /// Represents chart dock panel docking options.
    /// </summary>
    /// <seealso cref="ChartDockPanel"/>
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

        /// <summary>
        /// Sets element to the floating mode on panel.
        /// </summary>
        Floating
    }
    #endregion

    #region enum ContextMenuTypes
    /// <summary>
    /// Enum values for ContextMenuTypes
    /// </summary>
    public enum ContextMenuTypes
    {
        /// <summary>
        /// Enum value for Default Mode
        /// </summary>
        Default,

        /// <summary>
        ///Enum value for  DefaultWithCustom mode
        /// </summary>
        DefaultWithCustom,

        /// <summary>
        /// Enum value for Custom mode
        /// </summary>
        Custom
    }
    #endregion

    #region enum LegendPanelTypes
    /// <summary>
    /// Enum values for LegendPanelTypes
    /// </summary>
    public enum LegendPanelTypes
    {
        /// <summary>
        /// Enum value for Grid panel
        /// </summary>
        Grid,

        /// <summary>
        /// Enum value for WrapPanel
        /// </summary>
        WrapPanel,

        /// <summary>
        /// Enum value for Custom panel
        /// </summary>
        Custom

    }
    #endregion

    #region enum IndicatorTypes
    /// <summary>
    /// Enum values for Chart IndicatorTypes
    /// </summary>
    public enum IndicatorTypes
    {
        /// <summary>
        /// Enum value for MACD type
        /// </summary>
        MACD,

        /// <summary>
        /// Enum value for BollingerBands,
        /// </summary>
        BollingerBands,

        /// <summary>
        /// Enum value for ExponentialAverage type
        /// </summary>
        ExponentialAverage,

        /// <summary>
        /// Enum value for SimpleAverage type
        /// </summary>
        SimpleAverage,

        /// <summary>
        /// Enum value for TriangularAverage type.
        /// </summary>
        TriangularAverage,

        /// <summary>
        /// Enum value for Stochastics type.
        /// </summary>
        Stochastics,

        /// <summary>
        /// Enum value for Accumulationdistribution type.
        /// </summary>
        AccumulationDistribution,

        /// <summary>
        /// Enum value for RelativeStrengthIndex type.
        /// </summary>
        RelativeStrengthIndex,

        /// <summary>
        /// Enum value for momentum type.
        /// </summary>
        Momentum,

        /// <summary>
        /// Enum value for AverageTrueRange type.
        /// </summary>
        AverageTrueRange
    }
    #endregion

    #region enum AxisLabels
    /// <summary>
    /// enum for labels positions in axis
    /// </summary>
    public enum AxisLabels
    {
        /// <summary>
        /// Enum value for low position
        /// </summary>
        Low,

        /// <summary>
        /// enum value for High position
        /// </summary>
        High,

        /// <summary>
        /// Enum value for NextToAxis
        /// </summary>
        NextToAxis
    }
    #endregion

    #region enum AxisPositions
    /// <summary>
    /// Represents axis panel elements' alignments
    /// </summary>
    public enum AxisPositions
    {
        /// <summary>
        /// Enum value for arrange label within the axis
        /// </summary>
        Inside,
        /// <summary>
        /// Enum value for arrange label outside the axis
        /// </summary>
        Outside,
        /// <summary>
        /// Enum value for arrange label across the axis
        /// </summary>
        Cross
    }
    /// <summary>
    /// arrange the Segments based on the ticks
    /// </summary>
    public enum SegmentPositions
    { 
    /// <summary>
    /// Enum value for set the segment within the Ticks
    /// </summary>
    BetweenTicks,
        /// <summary>
    /// Enum value for set the segment on the Ticks
        /// </summary>
        OnTicks
    }
    #endregion

    #region enum AnnotationIntersectActions
    /// <summary>
    /// Represents Annotation's Intersect Action.
    /// </summary>
    public enum AnnotationIntersectActions
    {
        /// <summary>
        /// Enum value for overlap the Annotations when get intersect
        /// </summary>
        None,
        /// <summary>
        /// Enum value for hide the annotations when get intersect
        /// </summary>
        Hide,
    }
    #endregion

    #region enum TextIntersectActions
    /// <summary>
    /// Enum value for TextIntersectAction modes
    /// </summary>
    public enum TextIntersectActions
    {
        /// <summary>
        /// Enum value for Shrink
        /// </summary>
        Shrink,
        /// <summary>
        /// Enum value for Wrap
        /// </summary>
        Wrap
    }
    #endregion

    #region enum AdornmentIntersectActions
    /// <summary>
    /// Enum for AdornmentIntersectActions when Adornments get overlapped
    /// </summary>
    public enum AdornmentIntersectActions
    {
        /// <summary>
        /// enum value for no other operations
        /// </summary>
        None,
        /// <summary>
        /// Enum value for Hide the labels
        /// </summary>
        Hide,
        /// <summary>
        /// Enum value SmartAxis labels
        /// </summary>
        AdjustAroundPoints
    }
    #endregion

    #region enum AdornmentSegmentModes
    /// <summary>
    /// To arrange pie and doughnut adornments
    /// </summary>
    public enum AdornmentSegmentModes
    {
        /// <summary>
        /// Enum value for Radial mode
        /// </summary>
        Radial,
        /// <summary>
        /// Enum value for Horizontal Mode
        /// </summary>
        Horizontal
    }
    #endregion

    #region enum ChartPrintMode
    /// <summary>
    /// To select chart print mode.
    /// </summary>
    public enum ChartPrintMode
    {
        /// <summary>
        /// Enum value for Portrait
        /// </summary>
        Portrait,
        /// <summary>
        /// Enum value for Landscape
        /// </summary>
        Landscape
    }
    #endregion

     #region enum DoubleUnit
    /// <summary>
    /// Specifies the different values that are natively used.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    public enum DoubleUnits
    {
        /// <summary>
        ///  <see cref="AutoDetect"/> value
        /// </summary>
        AutoDetect,
        /// <summary>
        ///  <see cref="None"/> value
        /// </summary>
        None,

        /// <summary>
        ///  <see cref="Hundreds"/> value
        /// </summary>
        Hundreds,

        /// <summary>
        ///   <see cref="Thousands"/> value
        /// </summary>
        Thousands,

        /// <summary>
        ///   <see cref="TenThousands"/> value
        /// </summary>
        TenThousands,

        /// <summary>
        ///   <see cref="HundredThousands"/> value
        /// </summary>
        HundredThousands,

        /// <summary>
        ///   <see cref="Millions"/> value
        /// </summary>
        Millions,

        /// <summary>
        ///   <see cref="TenMillions"/> value
        /// </summary>
        TenMillions,

        /// <summary>
        ///   <see cref="HundredMillions"/> value
        /// </summary>
        HundredMillions,

        /// <summary>
        ///   <see cref="Billions"/> value
        /// </summary>
        Billions,

        /// <summary>
        ///   <see cref="Trillions"/> value
        /// </summary>
        Trillions,

        /// <summary>
        ///   <see cref="Quadrillion"/> value
        /// </summary>
        Quadrillion,

    }
    #endregion

    #region enum LogarithmicType
    /// <summary>
    /// Specifies the different values that are natively used.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    public enum LogarithmicType
    {
        /// <summary>
        ///  <see cref="Value"/> value
        /// </summary>
        Value,
        /// <summary>
        ///  <see cref="Exponential"/> value
        /// </summary>
        Exponential

    }
    #endregion

    #region enum LabelPositions
    /// <summary>
    /// Represents axis Label Positions
    /// </summary>
    public enum LabelPositions
    {
        /// <summary>
        /// Enum value for Inside mode
        /// </summary>
        Inside,
        /// <summary>
        /// Enum value for Outside mode
        /// </summary>
        Outside        
    }
    #endregion

    #region enum HeaderPositions
    /// <summary>
    /// Represents axis Header Positions
    /// </summary>
    public enum HeaderPositions
    {
        /// <summary>
        /// Enum value for Inside mode
        /// </summary>
        Inside,
        /// <summary>
        /// Enum value for Outside mode
        /// </summary>
        Outside
    }
    #endregion


   

}
