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
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.DataVisualization
{
    // Chart Series type
    [DataContract]
    public enum SeriesType
    {
        [EnumMember(Value = "area")]
        Area,
        [EnumMember(Value = "line")]
        Line,
        [EnumMember(Value = "spline")]
        Spline,
        [EnumMember(Value = "column")]
        Column,
        [EnumMember(Value = "scatter")]
        Scatter,
        [EnumMember(Value = "bubble")]
        Bubble,
        [EnumMember(Value = "splinearea")]
        SplineArea,
        [EnumMember(Value = "StepArea")]
        StepArea,
        [EnumMember(Value = "StepLine")]
        StepLine,
        [EnumMember(Value = "pie")]
        Pie,
        [EnumMember(Value = "hilo")]
        Hilo,
        [EnumMember(Value = "hiloopenclose")]
        HiloOpenClose,
        [EnumMember(Value = "candle")]
        Candle,
        [EnumMember(Value = "bar")]
        Bar,
        [EnumMember(Value = "stackingarea")]
        StackingArea,
        [EnumMember(Value = "rangecolumn")]
        RangeColumn,
        [EnumMember(Value = "stackingcolumn")]
        StackingColumn,
        [EnumMember(Value = "stackingbar")]
        StackingBar,
        [EnumMember(Value = "pyramid")]
        Pyramid,
        [EnumMember(Value = "doughnut")]
        Doughnut
    }
    //Shape types for marker and dataLabel shape
    [DataContract]
    public enum ChartShape
    {
        [EnumMember(Value = "None")]
        None,
        [EnumMember(Value = "LeftArrow")]
        LeftArrow,
        [EnumMember(Value = "RightArrow")]
        RightArrow,
        [EnumMember(Value = "Circle")]
        Circle,
        [EnumMember(Value = "Cross")]
        Cross,
        [EnumMember(Value = "HorizLine")]
        HorizLine,
        [EnumMember(Value = "VertLine")]
        VertLine,
        [EnumMember(Value = "Diamond")]
        Diamond,
        [EnumMember(Value = "Rectangle")]
        Rectangle,
        [EnumMember(Value = "Triangle")]
        Triangle,
        [EnumMember(Value = "InvertedTriangle")]
        InvertedTriangle,
        [EnumMember(Value = "Hexagon")]
        Hexagon,
        [EnumMember(Value = "Pentagon")]
        Pentagon,
        [EnumMember(Value = "Star")]
        Star,
        [EnumMember(Value = "Ellipse")]
        Ellipse,
        [EnumMember(Value = "Wedge")]
        Wedge,
        [EnumMember(Value = "Trapezoid")]
        Trapezoid,
        [EnumMember(Value = "UpArrow")]
        UpArrow,
        [EnumMember(Value = "DownArrow")]
        DownArrow,
        [EnumMember(Value = "Image")]
        Image
    }
    //Text position
    [DataContract]
    public enum TextPosition
    {
        [EnumMember(Value = "top")]
        Top,
        [EnumMember(Value = "middle")]
        Middle,
        [EnumMember(Value = "bottom")]
        Bottom
    }
    //HiloOpenClose DrawMode
    [DataContract]
    public enum SeriesDrawMode
    {
        [EnumMember(Value = "both")]
        Both,
        [EnumMember(Value = "open")]
        Open,
        [EnumMember(Value = "close")]
        Close
    }
    //connector type for accumulate series
    [DataContract]
    public enum ConnectorType
    {
        [EnumMember(Value = "line")]
        Line,
        [EnumMember(Value = "bezier")]
        Bezier,
        
    }
    //Text alignment
    [DataContract]
    public enum TextAlignment
    {
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "near")]
        Near,
        [EnumMember(Value = "far")]
        Far,
    }
     //Text alignment
    [DataContract]
    public enum CrosshairType
    {
        [EnumMember(Value = "crosshair")]
        Crosshair,
        [EnumMember(Value = "trackball")]
        Trackball,
        
    }
    //StripLineText alignment
    [DataContract]
    public enum StriplineTextAlignment
    {
        [EnumMember(Value = "middletop")]
        MiddleTop,
        [EnumMember(Value = "middlecenter")]
        MiddleCenter,
        [EnumMember(Value = "middlebottom")]
        MiddleBottom,
    }

    //Stripline zorder
    [DataContract]
    public enum ChartZOrder
    {
        [EnumMember(Value = "over")]
        Over,
        [EnumMember(Value = "behind")]
        Behind
    }
    //labelIntersectAction enum
    [DataContract]
    public enum LabelIntersectAction
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "rotate90")]
        Rotate90,
        [EnumMember(Value = "rotate45")]
        Rotate45,
        [EnumMember(Value = "trim")]
        Trim,
        [EnumMember(Value = "hide")]
        Hide,
        [EnumMember(Value = "multipleRows")]
        MultipleRows,
        [EnumMember(Value = "wrap")]
        Wrap
    }
    //Linecap
    [DataContract]
    public enum ChartLineCap
    {
        [EnumMember(Value = "butt")]
        Butt,
        [EnumMember(Value = "round")]
        Round,
        [EnumMember(Value = "square")]
        Square,
    }
    //Linejoin
    [DataContract]
    public enum ChartLineJoin
    {
        [EnumMember(Value = "round")]
        Round,
        [EnumMember(Value = "bevel")]
        Bevel,
        [EnumMember(Value = "miter")]
        Miter
    }
    
    //Legend position
    [DataContract]
    public enum LegendPosition
    {
        [EnumMember(Value = "top")]
        Top,
        [EnumMember(Value = "bottom")]
        Bottom,
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "custom")]
        Custom
    }
    //Legend alignment
    [DataContract]
    public enum Alignment
    {
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "near")]
        Near,
        [EnumMember(Value = "far")]
        Far,
    }
    //Label position
    [DataContract]
    public enum ChartLabelPosition
    {
        [EnumMember(Value = "outside")]
        Outside,
        [EnumMember(Value = "inside")]
        Inside,
        [EnumMember(Value = "outsideExtended")]
        OutsideExtended,
    }
    //xAxis valueType
    [DataContract]
    public enum AxisValueType
    {
        [EnumMember(Value = "double")]
        Double,
        [EnumMember(Value = "datetime")]
        Datetime,
        [EnumMember(Value = "category")]
        Category,
        [EnumMember(Value = "logarithmic")]
        Logarithmic
    }
    //Chart AreaType
    [DataContract]
    public enum ChartAreaType
    {
        [EnumMember(Value = "cartesianAxes")]
        CartesianAxes,
        [EnumMember(Value = "None")]
        None
    }
    //Interval type
    [DataContract]
    public enum ChartIntervalType
    {
        [EnumMember(Value = "days")]
        Days,
        [EnumMember(Value = "hours")]
        Hours,
        [EnumMember(Value = "seconds")]
        Seconds,
        [EnumMember(Value = "milliseconds")]
        Milliseconds,
        [EnumMember(Value = "minutes")]
        Minutes,
        [EnumMember(Value = "months")]
        Months,
        [EnumMember(Value = "years")]
        Years
    }
    //Font style
    [DataContract]
    public enum ChartFontStyle
    {
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "bold")]
        Bold,
        [EnumMember(Value = "italic")]
        Italic
    }
	//Font weight
	[DataContract]
    public enum ChartFontWeight
    {
        [EnumMember(Value = "regular")]
        Regular,
        [EnumMember(Value = "lighter")]
        Lighter
    }
    //Range padding
    [DataContract]
    public enum ChartRangePadding
    {
        [EnumMember(Value = "additional")]
        Additional,
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "round")]
        Round
    }
    //Themes
	[DataContract]
	public enum ChartTheme
    {
        [EnumMember(Value = "azure")]
        Azure,
        [EnumMember(Value = "flatlight")]
        FlatLight,
        [EnumMember(Value = "azuredark")]
        Azuredark,
		[EnumMember(Value = "lime")]
        Lime,
        [EnumMember(Value = "limedark")]
        LimeDark,
		[EnumMember(Value = "saffron")]
        Saffron,
        [EnumMember(Value = "saffrondark")]
        SaffronDark,
		[EnumMember(Value = "gradientlight")]
        GradientLight,
        [EnumMember(Value = "gradientdark")]
        GradientDark
    }

    // PyramidMode
    [DataContract]
    public enum PyramidMode
    {
        [EnumMember(Value = "linear")]
        Linear,
        [EnumMember(Value = "surface")]
        Surface,
    }
}