#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal enum HorizontalAlignment
    {
        Default,
        General,
        Left,
        Center,
        Right
    }
    internal enum VerticalAlignment
    {
        Default,
        Top,
        Middle,
        Bottom
    }

    internal enum FontStyle
    {
        Default,
        Bold,
        Italic
    }

    internal enum FontWeight
    {
        Default,
        Thin,
        ExtraLight,
        Light,
        Normal,
        Medium,
        SemiBold,
        Bold,
        ExtraBold,
        Heavy
    }

    internal enum FontEffects
    {
        Default,
        None,
        Underline,
        Overline,
        StrikeThrough
    }

    internal enum Sizing
    {
        AutoSize,
        Fit,
        FitProportional,
        Clip
    }

    internal enum LineStyle
    {
        Solid,
        Dashed,
        Dotted
    }

    internal enum ChartType
    {
        Area,
        Bubble,
        Bar,
        Column,
        Doughnut,
        FastLine,
        Funnel,
        Line,
        Pie,
        Polar,
        Pyramid,
        Radar,
        RangeArea,
        StackingArea,
        StackingColumn100,
        StepLine,
        StackingBar,
        Scatter
    }

    internal enum ChartObject
    {
        ChartTitle,
        PrimaryAxis,
        ReversePrimaryAxis,
        SecondaryAxis,
        ReverseSecondaryAxis,
        PrimaryAxisTitle,
        SecondaryAxisTile,
        ChartLegend,
        LegendTitle,
        ChartSeries,
        ChartPlotArea,
        SecondaryXAxisTitle,
        SecondaryYAxisTitle
    }

    internal enum AdornmentType
    {
        None,
        Cross,
        Diamond,
        Ellipse,
        Hexagon,
        InvertedTriangle,
        Pentagon,
        Plus,
        Square,
        Triangle
    }
    internal enum FillStyle
    {
        Solid,
        Gradient
    }

    internal enum GradientStyle
    {
        None,
        LeftRight,
        TopBottom
    }

    internal enum Position
    {
        Top,
        Right,
        Bottom,
        Left
    }

    internal enum Layout
    {
        Row,
        Column
    }

    internal enum TickStyle
    {
        Solid,
        Dashed,
        Dotted,
        DashDot,
        DashDotDot
    }

    internal enum TitleAlignment
    {
        Center,
        Far,
        Near
    }

    internal enum Type
    {
        Circular1,
        Circular2,
        Circular3,
        Circular4
    }

    internal enum PageBreak
    {
        None,
        Start,
        End,
        StartAndEnd
    }

    internal enum Style
    {
        Solid,
        Dashed,
        Dotted
    }

    internal enum Placement
    {
        Cross,
        Inside,
        Outside
    }

    internal enum PointerType
    {
        Needle,
        Marker
    }

    internal enum NeedleType
    {
        Triangle,
        Rectangle,
        Arrow
    }

    internal enum MajorTickShape
    {
        Rectangle,
        RoundedRectangle,
        Ellipse,
        Triangle
    }

    internal enum BackgroundHatchTypes
    {
        Default,
        None,
        BackwardDiagonal,
        Cross,
        DarkDownwardDiagonal,
        DarkHorizontal,
        DarkUpwardDiagonal,
        DarkVertical,
        DashedDownwardDiagonal,
        DashedHorizontal,
        DashedUpwardDiagonal,
        DashedVertical,
        DiagonalBrick,
        DiagonalCross,
        Divot,
        DottedDiamond,
        DottedGrid,
        ForwardDiagonal,
        Horizontal,
        HorizontalBrick,
        LargeCheckerBoard,
        LargeConfetti,
        LargeGrid,
        LightDownwardDiagonal,
        LightHorizontal,
        LightUpwardDiagonal,
        LightVertical,
        NarrowHorizontal,
        NarrowVertical,
        OutlinedDiamond,
        Percent05,
        Percent10,
        Percent20,
        Percent25,
        Percent30,
        Percent40,
        Percent50,
        Percent60,
        Percent70,
        Percent75,
        Percent80,
        Percent90,
        Plaid,
        Shingle,
        SmallCheckerBoard,
        SmallConfetti,
        SmallGrid,
        SolidDiamond,
        Sphere,
        Trellis,
        Vertical,
        Wave,
        Weave,
        WideDownwardDiagonal,
        WideUpwardDiagonal,
        ZigZag
    }

    internal enum SideBorderStyles
    {
        Default,
        None,
        Dotted,
        Dashed,
        Solid,
        Double
    }

    internal enum Source
    {
        External,
        Embedded,
        Database
    }

    internal enum BackgroundRepeat
    {
        Default,
        Repeat,
        RepeatX,
        RepeatY,
        Fit,
        Clip
    }



    internal enum BreakLocation
    {
        Start,
        End,
        StartAndEnd,
        Between
    }

    internal enum DataElementStyle
    {
        Auto,
        Attribute,
        Element
    }

    internal enum ListStyle
    {
        None,
        Numbered,
        Bulleted
    }
    internal enum MarkupType
    {
        None,
        HTML
    }

    internal enum KeepWithGroup
    {
        None,
        Before,
        After
    }


    internal enum VisualizationType
    {
        Column,
        Bar,
        Line,
        Shape,
        Scatter,
        Area,
        Range,
        Polar
    }
    internal enum VisualizationSubType
    {
        Plain,
        Stacked,
        PercentStacked,
        Smooth,
        Stepped,
        Pie,
        ExplodedPie,
        Doughnut,
        Funnel,
        Pyramid,
        Bubble,
        Candlestick,
        Stock,
        Bar,
        Column,
        BoxPlot,
        ErrorBar,
        ExplodedDoughnut,
        Radar
    }

    internal enum AllowOutSidePlotArea
    {
        Partial,
        True,
        False
    }



    internal enum BorderStyle
    {
        Solid,
        Dotted,
        Dashed,
        Double,
        DashDot,
        DashDotDot
    }
    internal enum CalloutLineAnchor
    {
        None,
        Arrow,
        Diamond,
        Square,
        Round
    }

    internal enum CalloutStyle
    {
        Underline,
        Box,
        None
    }
    internal enum DerivedSeriesFormula
    {
        BollingerBands,
        MovingAverage,
        ExponentialMovingAverage,
        TriangularMovingAverage,
        WeightedMovingAverage,
        MACD,
        DetrendedPriceOscillator,
        Envelopes,
        Performance,
        RateOfChange,
        RelativeStrengthIndex,
        StandardDeviation,
        TRIX,
        Mean,
        Median
    }

    internal enum IntervalType
    {
        Auto,
        Number,
        Years,
        Months,
        Weeks,
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds
    }

    internal enum Location
    {
        Default,
        Opposite
    }

    internal enum Arrows
    {
        None,
        Triangle,
        SharpTriangle,
        Lines
    }

    internal enum AllowLabelRotation
    {
        Rotate90 = 90,
        Rotate30 = 30,
        Rotate45 = 45,
        Rotate360 = 360,
        None = 0
    }
    internal enum ChartAxisTitlePosition
    {
        Center,
        Near,
        Far
    }

    internal enum TextOrientation
    {
        Auto,
        Horizantal,
        Rotated90,
        Rotated270,
        Stacked
    }

    internal enum BreakLineType
    {
        Ragged,
        Straight,
        Wave,
        None
    }

    internal enum ChartTickMarksType
    {
        Outside,
        Inside,
        Cross,
        None
    }

    internal enum AlignOrientation
    {
        None,
        Vertical,
        Horizontal,
        All
    }

    internal enum ProjectionMode
    {
        Oblique,
        Perspective
    }

    internal enum Shading
    {
        Real,
        Simple,
        None
    }
    internal enum Hidden
    {
        True,
        False
    }

    internal enum Positions
    {
        RightTop,
        TopLeft,
        TopCenter,
        TopRight,
        LeftTop,
        LeftCenter,
        LeftBottom,
        RightCenter,
        RightBottom,
        BottomRight,
        BottomCenter,
        BottomLeft
    }



    internal enum TitleSeparator
    {
        None,
        Line,
        thickLine,
        DoubleLine,
        DashLine,
        DotLine,
        GradientLine,
        ThickGradientColor
    }

    internal enum Separator
    {
        None,
        Line,
        ThickLine,
        DoubleLine,
        DashLine,
        DotLine,
        GradientLine,
        ThickGradientLine
    }

    internal enum Palette
    {
        Default,
        EarthTones,
        Excel,
        GrayScale,
        Light,
        Pastel,
        SemiTransparent,
        Berry,
        Chocolate,
        Fire,
        SeaGreen,
        BrightPastel,
        Custom
    }

    internal enum PaletteHatchBehavior
    {
        Default,
        None,
        Always
    }

    internal enum BorderSkinType
    {
        None,
        Emboss,
        Raised,
        Sunken,
        FrameThin1,
        FrameThin2,
        FrameThin3,
        FrameThin4,
        FrameThin5,
        FrameThin6,
        FrameTitle1,
        FrameTitle2,
        FrameTitle3,
        FrameTitle4,
        FrameTitle5,
        FrameTitle6,
        FrameTitle7,
        FrameTitle8
    }

    internal enum DistributionType
    {
        Optimal,
        EualInterval,
        EqualDistribution,
        Custom
    }

    internal enum MapColorPalette
    {
        Random,
        Light,
        SemiTransparent,
        BrightPastel
    }

    internal enum MapMarkerStyle
    {
        None,
        Rectangle,
        Circle,
        Diamond,
        Triangle,
        Trapezoid,
        Star,
        Wedge,
        Pentagon,
        PushPin,
        Image
    }

    internal enum ResizeMode
    {
        AutoFit,
        None
    }

    internal enum Unit
    {
        Percentage,
        Inch,
        Point,
        Centimeter,
        Millimeter,
        Pica
    }

    internal enum VisibilityMode
    {
        Visible,
        Hidden,
        ZoomBased
    }

    internal enum TileStyle
    {
        Road,
        Aerial,
        Hybrid
    }

    internal enum LabelPlacement
    {
        MiddleCenter,
        MiddleLeft,
        MiddleRight,
        TopCenter,
        TopLeft,
        TopRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
        Bottom,
        Top,
        Left,
        Right,
        Center,
        Above,
        Below,
        Alternate
    }

    internal enum LabelBehaviour
    {
        Auto,
        ShowMiddleValue,
        ShowBorderValue
    }
    internal enum MapCoordinateSystem
    {
        Planar,
        Geographic
    }

    internal enum MapProjection
    {
        Equirectangular,
        Mercator,
        Robinson,
        Fahey,
        Eckert1,
        Eckert3,
        HammerAitoff,
        Wagner3,
        Bonne
    }

    internal enum LabelPosition
    {
        Near,
        OneQuarter,
        Center,
        ThreeQuarters,
        Far
    }

    internal enum AntiAliasing
    {
        All,
        None,
        Text,
        Graphics
    }

    internal enum TextAntiAliasingQuality
    {
        High,
        Normal,
        SystemDefault
    }

    #region Gauge Enums

    /// <summary>
    /// Specifies the type of background gradient in Gauge.
    /// </summary>
    internal enum BackgroundGradientType
    {
        StartToEnd,
        LeftRight,
        TopBottom,
        Center,
        DiagonalLeft,
        DiagonalRight,
        HorizontalCenter,
        VerticalCenter,
        None
    }


    /// <summary>
    /// Specifies the Shape of the Tick Mark in Gauge.
    /// </summary>
    internal enum Shape
    {
        Rectangle,
        Triangle,
        Circle,
        Diamond,
        Trapezoid,
        Star,
        Wedge,
        Pentagon,
        None
    }

    /// <summary>
    /// Specifies the orientation of Gauge.
    /// </summary>
    internal enum Orientation
    {
        /// <summary>
        /// Default value. The Orientation of the gauge is Automatic.
        /// </summary>
        Auto,

        /// <summary>
        /// The Orientation of the gauge is Horizontal.
        /// </summary>
        Horizontal,

        /// <summary>
        /// The Orientation of the gauge is Vertical.
        /// </summary>
        Vertical
    }


    /// <summary>
    /// Specifies the type of calculation to perform on the values, if more than one is present.
    /// </summary>
    internal enum Formula
    {
        /// <summary>
        /// Default value. Indicates the last value is used.
        /// </summary>
        None,

        /// <summary>
        /// The type of calculation to perform on the values is Average.
        /// </summary>
        Average,

        /// <summary>
        /// The type of calculation to perform on the values is Linear.
        /// </summary>
        Linear,

        /// <summary>
        /// The type of calculation to perform on the values is Max.
        /// </summary>
        Max,

        /// <summary>
        /// The type of calculation to perform on the values is Min.
        /// </summary>
        Min,

        /// <summary>
        /// The type of calculation to perform on the values is Median.
        /// </summary>
        Median,

        /// <summary>
        /// The type of calculation to perform on the values is OpenClose.
        /// </summary>
        OpenClose,

        /// <summary>
        /// The type of calculation to perform on the values is Percentile.
        /// </summary>
        Percentile,

        /// <summary>
        /// The type of calculation to perform on the values is Variance.
        /// </summary>
        Variance,

        /// <summary>
        /// The type of calculation to perform on the values is RateOfChange.
        /// </summary>
        RateOfChange,

        /// <summary>
        /// The type of calculation to perform on the values is Integral.
        /// </summary>
        Integral
    }

    /// <summary>
    /// Indicates whether the item should appear in a data rendering.
    /// </summary>
    internal enum DataElementOutputGauge
    {
        /// <summary>
        /// Default. Indicates the item should appear in the output.
        /// </summary>
        Output,

        /// <summary>
        /// Indicates the item should not appear in the output.
        /// </summary>       
        NoOutput
    }

    /// <summary>
    /// Specifies the type of pointer.
    /// </summary>
    internal enum RadialPointerType
    {
        /// <summary>
        /// The pointer type is Needle.
        /// </summary>
        Needle,

        /// <summary>
        /// The pointer type is Marker.
        /// </summary>
        Marker,

        /// <summary>
        /// The pointer type is Bar.
        /// </summary>
        Bar
    }

    /// <summary>
    /// Specifies the style of the needle.
    /// </summary>
    internal enum NeedleStyleGauge
    {
        /// <summary>
        /// The style of the needle is Triangular.
        /// </summary>
        Triangular,

        /// <summary>
        /// The style of the needle is Rectangular.
        /// </summary>      
        Rectangular,

        /// <summary>
        /// The style of the needle is TaperedWithTail.
        /// </summary>
        TaperedWithTail,

        /// <summary>
        /// The style of the needle is Tapered.
        /// </summary>
        Tapered,

        /// <summary>
        /// The style of the needle is ArrowWithTail.
        /// </summary>
        ArrowWithTail,

        /// <summary>
        /// The style of the needle is Arrow.
        /// </summary>
        Arrow,

        /// <summary>
        /// The style of the needle is StealthArrowWithTail.
        /// </summary>
        StealthArrowWithTail,

        /// <summary>
        /// The style of the needle is StealthArrow.
        /// </summary>
        StealthArrow,

        /// <summary>
        /// The style of the needle is TaperedWithStealthArrow.
        /// </summary>
        TaperedWithStealthArrow,

        /// <summary>
        /// The style of the needle is StealthArrowWithWideTail.
        /// </summary>
        StealthArrowWithWideTail,

        /// <summary>
        /// The style of the needle is TaperedWithRoundedPoint.
        /// </summary>
        TaperedWithRoundedPoint
    }

    /// <summary>
    /// Specifies the type of pointer.
    /// </summary>
    internal enum LinearPointerType
    {
        /// <summary>
        /// Default value. The type of pointer is Marker.
        /// </summary>
        Marker,

        /// <summary>
        /// The type of pointer is Bar.
        /// </summary>
        Bar,

        /// <summary>
        /// The type of pointer is Thermometer.
        /// </summary>
        Thermometer
    }

    /// <summary>
    /// Indicates where the pointer will start if it is of type Bar.
    /// </summary>
    internal enum BarStart
    {
        /// <summary>
        /// Default value. The pointer starts where the scale starts.
        /// </summary>
        ScaleStart,

        /// <summary>
        /// The pointer starts at zero.
        /// </summary>
        Zero
    }

    /// <summary>
    /// Specifies the type of marker.
    /// </summary>
    internal enum MarkerStyle
    {
        /// <summary>
        /// Default value. The marker type is triangle.
        /// </summary>
        Triangle,

        /// <summary>
        /// The marker type is rectangle.
        /// </summary>
        Rectangle,

        /// <summary>
        /// The marker type is Circle.
        /// </summary>
        Circle,

        /// <summary>
        /// The marker type is diamond.
        /// </summary>
        Diamond,

        /// <summary>
        /// The marker type is trapezoid.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// The marker type is star.
        /// </summary>
        Star,

        /// <summary>
        /// The marker type is wedge.
        /// </summary>
        Wedge,

        /// <summary>
        /// The marker type is pentagon.
        /// </summary>
        Pentagon,

        /// <summary>
        /// No marker type.
        /// </summary>
        None
    }

    /// <summary>
    /// Determines where the pointer should be placed relative to the scale.
    /// </summary>


    /// <summary>
    /// Specifies the style of the frame.
    /// </summary>
    internal enum FrameStyle
    {
        /// <summary>
        /// Default value. No frame style.
        /// </summary>
        None,

        /// <summary>
        /// The frame style is simple.
        /// </summary>
        Simple,

        /// <summary>
        /// The frame style is edged.
        /// </summary>
        Edged
    }

    /// <summary>
    /// Specifies the shape of the frame.
    /// </summary>
    internal enum FrameShape
    {
        /// <summary>
        /// Default, Treated as Circular for radial gauges, Rectangular for linear gauges and gauge panels.
        /// </summary>
        Default,

        /// <summary>
        /// The shape of the frame is circular.
        /// </summary>
        Circular,

        /// <summary>
        /// The shape of the frame is rectangular.
        /// </summary>
        Rectangular,

        /// <summary>
        /// The shape of the frame is rounded rectangular.
        /// </summary>
        RoundedRectangular,

        /// <summary>
        /// The shape of the frame is auto.
        /// </summary>
        AutoShape,

        /// <summary>
        /// The shape of the frame is CustomCircular1.
        /// </summary>
        CustomCircular1,

        /// <summary>
        /// The shape of the frame is CustomCircular2.
        /// </summary>
        CustomCircular2,

        /// <summary>
        /// The shape of the frame is CustomCircular3.
        /// </summary>
        CustomCircular3,

        /// <summary>
        /// The shape of the frame is CustomCircular4.
        /// </summary>
        CustomCircular4,

        /// <summary>
        /// The shape of the frame is CustomCircular5.
        /// </summary>
        CustomCircular5,

        /// <summary>
        /// The shape of the frame is CustomCircular6.
        /// </summary>
        CustomCircular6,

        /// <summary>
        /// The shape of the frame is CustomCircular7.
        /// </summary>
        CustomCircular7,

        /// <summary>
        /// The shape of the frame is CustomCircular8.
        /// </summary>
        CustomCircular8,

        /// <summary>
        /// The shape of the frame is CustomCircular9.
        /// </summary>
        CustomCircular9,

        /// <summary>
        /// The shape of the frame is CustomCircular10.
        /// </summary>
        CustomCircular10,

        /// <summary>
        /// The shape of the frame is CustomCircular11.
        /// </summary>
        CustomCircular11,

        /// <summary>
        /// The shape of the frame is CustomCircular12.
        /// </summary>
        CustomCircular12,

        /// <summary>
        /// The shape of the frame is CustomCircular13.
        /// </summary>
        CustomCircular13,

        /// <summary>
        /// The shape of the frame is CustomCircular14.
        /// </summary>
        CustomCircular14,

        /// <summary>
        /// The shape of the frame is CustomCircular15.
        /// </summary>
        CustomCircular15,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularN1.
        /// </summary>
        CustomSemiCircularN1,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularN2.
        /// </summary>
        CustomSemiCircularN2,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularN3.
        /// </summary>
        CustomSemiCircularN3,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularN4.
        /// </summary>
        CustomSemiCircularN4,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularS1.
        /// </summary>
        CustomSemiCircularS1,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularS2.
        /// </summary>
        CustomSemiCircularS2,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularS3.
        /// </summary>
        CustomSemiCircularS3,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularS4.
        /// </summary>
        CustomSemiCircularS4,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularE1.
        /// </summary>
        CustomSemiCircularE1,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularE2.
        /// </summary>
        CustomSemiCircularE2,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularE3.
        /// </summary>
        CustomSemiCircularE3,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularE4.
        /// </summary>
        CustomSemiCircularE4,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularW1.
        /// </summary>
        CustomSemiCircularW1,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularW2.
        /// </summary>
        CustomSemiCircularW2,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularW3.
        /// </summary>
        CustomSemiCircularW3,

        /// <summary>
        /// The shape of the frame is CustomSemiCircularW4.
        /// </summary>
        CustomSemiCircularW4,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNE1.
        /// </summary>
        CustomQuarterCircularNE1,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNE2.
        /// </summary>
        CustomQuarterCircularNE2,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNE3.
        /// </summary>
        CustomQuarterCircularNE3,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNE4.
        /// </summary>
        CustomQuarterCircularNE4,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNW1.
        /// </summary>
        CustomQuarterCircularNW1,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNW2.
        /// </summary>
        CustomQuarterCircularNW2,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNW3.
        /// </summary>
        CustomQuarterCircularNW3,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularNW4.
        /// </summary>
        CustomQuarterCircularNW4,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSE1.
        /// </summary>
        CustomQuarterCircularSE1,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSE2.
        /// </summary>
        CustomQuarterCircularSE2,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSE3.
        /// </summary>
        CustomQuarterCircularSE3,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSE4.
        /// </summary>
        CustomQuarterCircularSE4,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSW1.
        /// </summary>
        CustomQuarterCircularSW1,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSW2.
        /// </summary>
        CustomQuarterCircularSW2,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSW3.
        /// </summary>
        CustomQuarterCircularSW3,

        /// <summary>
        /// The shape of the frame is CustomQuarterCircularSW4.
        /// </summary>
        CustomQuarterCircularSW4
    }

    internal enum GlassEffect
    {
        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        None,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        Simple
    }


    internal enum CapStyle
    {
        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedDark,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        Rounded,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedLight,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedWithAdditionalTop,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedWithWideIndentation,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        FlattenedWithIndentation,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        FlattenedWithWideIndentation,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedGlossyWithIndentation,

        /// <summary>
        /// Rectangle Report Item
        /// </summary>
        RoundedWithIndentation
    }

    internal enum ThermometerStyle
    {
        /// <summary>
        /// Default
        /// </summary>
        Standard,

        /// <summary>
        /// Represents flask type.
        /// </summary>
        Flask
    }

    #endregion
}

