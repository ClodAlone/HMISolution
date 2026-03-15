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

namespace Syncfusion.RDL.DOM
{
    static class Extensions
    {
        public static List<Field> Clone(this Fields list)
        {
            return list.Select(field => field.Clone() as Field).ToList();
        }

        public static List<Filter> Clone(this Filters list)
        {
            return list.Select(filter => filter.Clone() as Filter).ToList();
        }

        public static List<FilterValue> Clone(this FilterValues list)
        {
            return list.Select(filterValue => filterValue.Clone() as FilterValue).ToList();
        }

        public static List<ParameterValue> Clone(this ParameterValues list)
        {
            return list.Select(parameter => parameter.Clone() as ParameterValue).ToList();
        }

        public static List<QueryParameter> Clone(this QueryParameters list)
        {
            return list.Select(queryparameter => queryparameter.Clone() as QueryParameter).ToList();
        }

        public static List<string> Clone(this Values list)
        {
            return list.Select(value => value as string).ToList();
        }

        public static List<TablixCornerRow> Clone(this TablixCornerRows list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixcornerrow => tablixcornerrow.Clone() as TablixCornerRow).ToList();
        }

        public static List<TablixCornerCell> Clone(this TablixCornerCells list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixcornercell => tablixcornercell.Clone() as TablixCornerCell).ToList();
        }

        public static List<TablixColumn> Clone(this TablixColumns list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixcolumn => tablixcolumn.Clone() as TablixColumn).ToList();
        }

        public static List<TablixCell> Clone(this TablixCells list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixcell => tablixcell.Clone() as TablixCell).ToList();
        }

        public static List<TablixRow> Clone(this TablixRows list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixrow => tablixrow.Clone() as TablixRow).ToList();
        }

        public static List<SortExpression> Clone(this SortExpressions list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(sortexpression => sortexpression.Clone() as SortExpression).ToList();
        }

        public static List<TablixMember> Clone(this TablixMembers list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(tablixmember => tablixmember.Clone() as TablixMember).ToList();
        }

        public static List<CustomProperty> Clone(this CustomProperties list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(customproperty => customproperty.Clone() as CustomProperty).ToList();
        }

        public static List<GroupExpression> Clone(this GroupExpressions list)
        {
            if (list == null)
            {
                return null;
            }

            return list.Select(groupexpression => groupexpression.Clone() as GroupExpression).ToList();
        }
    }

    public enum RDLType
    {
        RDL2008,
        RDL2010,
        None
    }

    public enum ReportUnitType
    {
        In,
        Cm
    }

    public enum MeasurementUnits
    {
        None,
        Cm,
        Mm,
        In,
        Pt,
        Pc,
        Px
    }

    public enum DataElementStyles
    {
        Attribute,
        Element
    }

    public enum DataTypes
    {
        String,
        Boolean,
        DateTime,
        Integer,
        Float,
        Decimal
    }

    public enum BooleanOptions
    {
        Auto,
        True,
        False
    }

    public enum CommandType
    {
        Text,
        StoredProcedure,
        TableDirect
    }

    public enum FilterOperators
    {
        Equal,
        Like,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        TopN,
        BottomN,
        TopPercent,
        BottomPercent,
        In,
        Between
    }

    public enum DataElementOutputs
    {
        Auto,
        Output,
        NoOutput,
        ContentsOnly
    }

    public enum BackgroundGradientTypes
    {
        Default,
        None,
        LeftRight,
        TopBottom,
        Center,
        DiagonalLeft,
        DiagonalRight,
        HorizontalCenter,
        VerticalCenter,
        StartToEnd
    }

    public enum FontStyle
    {
        Default,
        Normal,
        Italic
    }

    public enum FontWeight
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

    public enum TextDecoration
    {
        Default,
        None,
        Underline,
        Overline,
        LineThrough
    }

    public enum TextAlign
    {
        Default,
        General,
        Left,
        Center,
        Right
    }

    public enum VerticalAlign
    {
        Default,
        Top,
        Middle,
        Bottom
    }

    public enum Direction
    {
        Default,
        LTR,
        RTL
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum WritingMode
    {
        Default,
        Horizontal,
        Vertical
    }

    public enum Calendar
    {
        Default,
        Gregorian,
        GregorianArabic,
        GregorianMiddleEastFrench,
        GregorianTransliteratedEnglish,
        GregorianTransliteratedFrench,
        GregorianUSEnglish,
        Hebrew,
        Hijri,
        Japanese,
        Korean,
        Taiwan,
        ThaiBuddhist
    }

    public enum TextEffects
    {
        Default,
        None,
        Shadow,
        Emboss,
        Embed,
        Frame
    }

    public enum BackgroundHatchTypes
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

    public enum BorderStyles
    {
        Default,
        None,
        Dotted,
        Dashed,
        Solid,
        Double,
        DashDot,
        DashDotDot
    }

    public enum SideBorderStyles
    {
        Default,
        None,
        Dotted,
        Dashed,
        Solid,
        Double
    }

    public enum Source
    {
        External,
        Embedded,
        Database
    }

    public enum BackgroundRepeat
    {
        Default,
        Repeat,
        RepeatX,
        RepeatY,
        Fit,
        Clip
    }

    public enum Position
    {
        Default,
        Top,
        TopLeft,
        TopRight,
        Left,
        Center,
        Right,
        RightTop,
        BottomRight,
        Bottom,
        BottomLeft,
        Outside
    }

    public enum BreakLocation
    {
        None,
        Start,
        End,
        StartAndEnd,
        Between
    }

    public enum DataElementStyle
    {
        Auto,
        Attribute,
        Element
    }

    public enum ListStyle
    {
        None,
        Numbered,
        Bulleted
    }
    public enum MarkupType
    {
        None,
        HTML
    }
    public enum Sizing
    {
        AutoSize,
        Fit,
        FitProportional,
        Clip
    }
    public enum KeepWithGroup
    {
        None,
        Before,
        After
    }

    public enum Type
    {
        None,
        Square,
        Circle,
        Diamond,
        Triangle,
        Cross,
        Star4,
        Star5,
        Star6,
        Star10,
        Auto
    }
    public enum VisualizationType
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
    public enum VisualizationSubType
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

    public enum AllowOutSidePlotArea
    {
        Partial,
        True,
        False
    }

    public enum LineStyle
    {
        Solid,
        None,
        Dotted,
        Dashed,
        Double,
        DashDot,
        DashDotDot
    }

    public enum CalloutLineAnchor
    {
        None,
        Arrow,
        Diamond,
        Square,
        Round
    }

    public enum CalloutStyle
    {
        Underline,
        Box,
        None
    }
    public enum DerivedSeriesFormula
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

    public enum IntervalType
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

    public enum Location
    {
        Default,
        Opposite
    }

    public enum Arrows
    {
        None,
        Triangle,
        SharpTriangle,
        Lines
    }

    public enum AllowLabelRotation
    {
        Rotate90 = 90,
        Rotate30 = 30,
        Rotate45 = 45,
        Rotate360 = 360,
        None = 0
    }
    public enum ChartAxisTitlePosition
    {
        Center,
        Near,
        Far
    }

    public enum TextOrientation
    {
        Auto,
        Horizantal,
        Rotated90,
        Rotated270,
        Stacked
    }

    public enum BreakLineType
    {
        Ragged,
        Straight,
        Wave,
        None
    }

    public enum ChartTickMarksType
    {
        Outside,
        Inside,
        Cross,
        None
    }

    public enum AlignOrientation
    {
        None,
        Vertical,
        Horizontal,
        All
    }

    public enum ProjectionMode
    {
        Oblique,
        Perspective
    }

    public enum Shading
    {
        Real,
        Simple,
        None
    }

    public enum Positions
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

    public enum Layout
    {
        AutoTable,
        Column,
        Row,
        WideTable,
        TallTable
    }

    public enum Separator
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

    public enum Palette
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

    public enum PaletteHatchBehavior
    {
        Default,
        None,
        Always
    }

    public enum BorderSkinType
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

    public enum DistributionType
    {
        Optimal,
        EqualInterval,
        EqualDistribution,
        Custom
    }
    
    public enum MapColorPalette
    {
        Random,
        Light,
        SemiTransparent,
        BrightPastel
    }

    public enum MapMarkerStyle
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

    public enum ResizeMode
    {
        AutoFit,
        None
    }

    public enum Unit
    {
        Percentage,
        Inch,
        Point,
        Centimeter,
        Millimeter,
        Pica
    }

    public enum VisibilityMode
    {
        Visible,
        Hidden,
        ZoomBased
    }

    public enum TileStyle
    {
        Road,
        Aerial,
        Hybrid
    }

    public enum LabelPlacement
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

    public enum LabelBehaviour
    {
        Auto,
        ShowMiddleValue,
        ShowBorderValue
    }
    public enum MapCoordinateSystem
    {
        Planar,
        Geographic
    }

    public enum MapProjection
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

    public enum LabelPosition
    {
        Near,
        OneQuarter,
        Center,
        ThreeQuarters,
        Far
    }

    public enum AntiAliasing
    {
        All,
        None,
        Text,
        Graphics
    }

    public enum TextAntiAliasingQuality
    {
        High,
        Normal,
        SystemDefault
    }

    #region Gauge Enums

    /// <summary>
    /// Specifies the type of background gradient in Gauge.
    /// </summary>
    public enum BackgroundGradientType
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
    public enum Shape
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
    public enum Orientation
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
    public enum Formula
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
    public enum DataElementOutputGauge
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
    public enum RadialPointerType
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
    public enum NeedleStyleGauge
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
    public enum LinearPointerType
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
    public enum BarStart
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
    public enum MarkerStyle
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
    public enum Placement
    {
        /// <summary>
        /// The pointer is placed inside.
        /// </summary>
        Inside,

        /// <summary>
        /// Default value for linear gauge. The pointer is placed outside.
        /// </summary>
        Outside,

        /// <summary>
        /// Default value for radial gauge. The pointer is placed in cross.
        /// </summary>
        Cross
    }

    /// <summary>
    /// Specifies the style of the frame.
    /// </summary>
    public enum FrameStyle
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
    public enum FrameShape
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

    public enum GlassEffect
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
      

    public enum CapStyle
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

    public enum ThermometerStyle
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

    public enum GaugeStateIndicatorStyles
    {
        Circle,
        Flag,
        ArrowDown,
        ArrowDownIncline,
        ArrowSide,
        ArrowUp,
        ArrowUpIncline,
        BoxesAllFilled,
        BoxesNoneFilled,
        BoxesOneFilled,
        BoxesTwoFilled,
        BoxesThreeFilled,
        QuartersAllFilled,
        QuartersNoneFilled,
        QuartersOneFilled,
        QuartersTwoFilled,
        QuartersThreeFilled,
        SignalMeterFourFilled,
        SignalMeterNoneFilled,
        SignalMeterOneFilled,
        SignalMeterThreeFilled,
        SignalMeterTwoFilled,
        StarQuartersAllFilled,
        StarQuartersNoneFilled,
        StarQuartersOneFilled,
        StarQuartersTwoFilled,
        StarQuartersThreeFilled,
        ThreeSignsCircle,
        ThreeSignsDiamond,
        ThreeSignsTriangle,
        ThreeSymbolCheck,
        ThreeSymbolCross,
        ThreeSymbolExclamation,
        ThreeSymbolUnCircledCheck,
        ThreeSymbolUnCircledCross,
        ThreeSymbolUnCircledExclamation,
        TrafficLight,
        TrafficLightUnrimmed,
        TriangleDash,
        TriangleDown,
        TriangleUp,
        ButtonStop,
        ButtonPlay,
        ButtonPause,
        FaceSmile,
        FaceNeutral,
        FaceFrown,
        Image,
        None
    }

    public enum StateIndicatorIconsSet
    {
        Custom,
        ThreeColoredArrows,
        ThreeGrayArrows,
        ThreeFlags,
        ThreeUnrimmedTrafficLights,
        ThreeRimmedTrafficLights,
        ThreeSigns,
        ThreeCircledSymbols,
        ThreeUncircledSymbols,
        FourColoredArrows,
        FourGrayArrows,
        RedToBlack,
        FourRatings,
        FourTrafficLights,
        FiveColoredArrows,
        FiveGrayArrows,
        FiveRatings,
        FiveQuarters,
        FiveBlocks,
        FiveStars,
        ThreeStars,
        ThreeUpDownTriangles
    }

    public enum ResizeModes
    {
        AutoFit,
        None
    }

    public enum TransformationType
    {
        None,
        Percentage
    }


    #endregion
}

