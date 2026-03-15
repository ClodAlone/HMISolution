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

namespace Syncfusion.JavaScript
{
    [DataContract]
    public enum TextAlign
    {
        [EnumMember(Value="left")]
        Left,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "justify")]
        Justify
    }

     [DataContract]
    public enum SortOrder
    {
        [EnumMember(Value = "ascending")]
        Ascending,
        [EnumMember(Value = "descending")]
        Descending
    }

     [DataContract]
     public enum HeightStyle
     {
         [EnumMember(Value = "content")]
         Content,
         [EnumMember(Value = "auto")]
         Auto,
         [EnumMember(Value = "fill")]
         Fill
     }
     [DataContract]
     public enum FilterOperatorType
     {
         [EnumMember(Value = "startswith")]
         StartsWith,
         [EnumMember(Value = "contains")]
         Contains,
         [EnumMember(Value = "endswith")]
         EndsWith,
         [EnumMember(Value = "lessthan")]
         LessThan,
         [EnumMember(Value = "greaterthan")]
         GreaterThan,
         [EnumMember(Value = "lessthanorequal")]
         LessThanOrEqual,
         [EnumMember(Value = "greaterthanorequal")]
         GreaterThanOrEqual,
         [EnumMember(Value = "equal")]
         Equal,
         [EnumMember(Value = "notequal")]
         NotEqual
     }
     [DataContract]
     public enum ButtonSize
     {
         [EnumMember(Value = "normal")]
         Normal,
         [EnumMember(Value = "mini")]
         Mini,
         [EnumMember(Value = "small")]
         Small,
         [EnumMember(Value = "medium")]
         Medium,
         [EnumMember(Value = "large")]
         Large
     }
     
     [DataContract]
     public enum Contents
     {
         [EnumMember(Value = "textonly")]
         TextOnly,
         [EnumMember(Value = "imageonly")]
         ImageOnly,
         [EnumMember(Value = "imageboth")]
         ImageBoth,
         [EnumMember(Value = "textandimage")]
         TextAndImage,
         [EnumMember(Value = "imagetextimage")]
         ImageTextImage
     }

     [DataContract]
     public enum ImagePositions
     {
         [EnumMember(Value = "imageright")]
         ImageRight,
         [EnumMember(Value = "imageleft")]
         ImageLeft
     }

    [DataContract]
     public enum Size
     {
         [EnumMember(Value = "small")]
         Small,
         [EnumMember(Value = "medium")]
         Medium

     }

    [DataContract]
     public enum Header
     {
         [EnumMember(Value = "ShowHeaderNone")]
         ShowHeaderNone,
         [EnumMember(Value = "ShowHeaderShort")]
         ShowHeaderShort,
         [EnumMember(Value = "ShowHeaderMin")]
         ShowHeaderMin,
         [EnumMember(Value = "ShowHeaderLong")]
         ShowHeaderLong
     }
    
     [DataContract]
     public enum Period
     {
         [EnumMember(Value = "Month")]
         Month,
         [EnumMember(Value = "Year")]
         Year,
         [EnumMember(Value = "Decade")]
         Decade,
         [EnumMember(Value = "Century")]
         Century,
         [EnumMember(Value = "")]
         None
     }
      [DataContract]
     public enum InputMode
     {
         [EnumMember(Value = "password")]
         Password,
         [EnumMember(Value = "text")]
         Text
     }
     [DataContract]
     public enum MenuType
     {
         [EnumMember(Value = "NormalMenu")]
         NormalMenu,
         [EnumMember(Value = "ContextMenu")]
         ContextMenu
     }
     [DataContract]
     public enum Animation
     {
         [EnumMember(Value = "None")]
         None,
         [EnumMember(Value = "Default")]
         Default
     }
     [DataContract]
     public enum Orientation
     {
         [EnumMember(Value = "horizontal")]
         Horizontal,
         [EnumMember(Value = "vertical")]
         Vertical
     }
     [DataContract]
     public enum RadioButtonSize
     {
         [EnumMember(Value = "small")]
         Small,
         [EnumMember(Value = "medium")]
         Medium,
     }
     [DataContract]
     public enum Direction
     {
         [EnumMember(Value = "Left")]
         Left,
         [EnumMember(Value = "Right")]
         Right
     }
     [DataContract]
     public enum Precisions
     {
         [EnumMember(Value = "full")]
         Full,
         [EnumMember(Value = "half")]
         Half,
         [EnumMember(Value = "exact")]
         Exact
     }
     [DataContract]
     public enum SlideType
     {
         [EnumMember(Value = "default")]
         Default,
         [EnumMember(Value = "range")]
         Range,
         [EnumMember(Value = "minrange")]
         MinRange
     }
    [DataContract]
     public enum HeaderPosition
     {
         [EnumMember(Value = "top")]
         Top,
         [EnumMember(Value = "bottom")]
         Bottom
     }

     [DataContract]
     public enum Formats
     {
         [EnumMember(Value = "cloud")]
         Cloud,
         [EnumMember(Value = "list")]
         List
     }
     [DataContract]
     public enum PagerPosition
     {
         [EnumMember(Value = "topleft")]
         TopLeft,
         [EnumMember(Value = "topright")]
         TopRight,
         [EnumMember(Value = "bottomleft")]
         BottomLeft,
         [EnumMember(Value = "bottomright")]
         BottomRight,
         [EnumMember(Value = "outside")]
         Outside,
         [EnumMember(Value = "topCenter")]
         TopCenter
     }
     [DataContract]
     public enum CharacterType
     {
         [EnumMember(Value="SevenSegment")]
         SevenSegment,
         [EnumMember(Value="FourteenSegment")]
         FourteenSegment,
         [EnumMember(Value="SixteenSegment")]
         SixteenSegment,
         [EnumMember(Value="EightCrossEightDotMatrix")]
         EightCrossEightDotMatrix,
         [EnumMember(Value="EightCrossEightSquareMatrix")]
         EightCrossEightSquareMatrix
     }
     [DataContract]
     public enum FontStyle
     {
         [EnumMember(Value = "Normal")]
         Normal,
         [EnumMember(Value = "Bold")]
         Bold,
         [EnumMember(Value = "Italic")]
         Italic,
         [EnumMember(Value = "Underline")]
         UnderLine,
         [EnumMember(Value = "Strikeout")]
         Strikeout
     }
     [DataContract]
     public enum Themes
     {
         [EnumMember(Value = "flatlight")]
         FlatLight,
         [EnumMember(Value = "flatdark")]
         FlatDark
     }
   
     public enum OlapChartType
     {
         [EnumMember(Value = "column")]
         Column
     }

     [DataContract]
     public enum OlapGridLayout
     {
         [EnumMember(Value = "Normal")]
         Normal,
         [EnumMember(Value = "NormalTopSummary")]
         NormalTopSummary,
         [EnumMember(Value = "NoSummaries")]
         NoSummaries,
         [EnumMember(Value = "ExcelLikeLayout")]
         ExcelLikeLayout
     }

     [DataContract]
     public enum OlapClientDisplayMode
     {
         [EnumMember(Value = "chartOnly")]
         ChartOnly,
         [EnumMember(Value = "gridOnly")]
         GridOnly,
         [EnumMember(Value = "chartAndGrid")]
         ChartAndGrid
     }

     [DataContract]
     public enum OlapClientControlPlacement
     {
         [EnumMember(Value = "tab")]
         Tab,
         [EnumMember(Value = "tile")]
         Tile
     }

     [DataContract]
     public enum OlapClientDefaultView
     {
         [EnumMember(Value = "chart")]
         Chart,
         [EnumMember(Value = "grid")]
         Grid
     }

     [DataContract]
     public enum ProgressMode
     {
         [EnumMember(Value = "infinite")]
         Infinite,
         [EnumMember(Value = "normal")]
         Normal,
         [EnumMember(Value = "progress")]
         Progress
     }

   [DataContract]
    public enum LabelPlacements
    {
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Center")]
        Center,
    }
    [DataContract]
    public enum MarkerStyles
    {
        [EnumMember(Value = "Triangle")]
        Triangle,
        [EnumMember(Value = "Rectangle")]
        Rectangle,
        [EnumMember(Value = "Ellipse")]
        Ellipse,
        [EnumMember(Value = "Diamond")]
        Diamond,
        [EnumMember(Value = "Pentgon")]
        Pentagon,
        [EnumMember(Value = "Circle")]
        Circle,
        [EnumMember(Value = "Star")]
        Star,
        [EnumMember(Value = "Slider")]
        Slider,
        [EnumMember(Value = "Pointer")]
        Pointer,
        [EnumMember(Value = "Wedge")]
        Wedge,
        [EnumMember(Value = "Trapezoid")]
        Trapezoid,
        [EnumMember(Value = "RoundedRectangle")]
        TRoundedRectangle
    }
    [DataContract]
    public enum MarkerPointerPlacements
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum TickPlacements
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum RangePositions
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum IndicatorStyles
    {
        [EnumMember(Value = "Rectangle")]
        Rectangle,
        [EnumMember(Value = "Circle")]
        Circle,
        [EnumMember(Value = "RoundedRectangle")]
        RoundedRectangle,
        [EnumMember(Value = "text")]
        text
    }
    [DataContract]
    public enum GaugeStyles
    {
        [EnumMember(Value = "Major")]
        Major,
        [EnumMember(Value = "Minor")]
        Minor,
    }
    [DataContract]
    public enum Tickstyles
    {
        [EnumMember(Value = "MajorInterval")]
        MajorInterval,
        [EnumMember(Value = "MinorInterval")]
        MinorInterval,
    }
    [DataContract]
    public enum CircularTickstyles
    {
        [EnumMember(Value = "Major")]
        Major,
        [EnumMember(Value = "Minor")]
        Minor,
    }
    [DataContract]
    public enum ScaleStyles
    {
        [EnumMember(Value = "Thermometer")]
        Thermometer,
        [EnumMember(Value = "Rectangle")]
        Rectangle,
        [EnumMember(Value = "RoundedRectangle")]
        RoundedRectangle,
    }
    [DataContract]
    public enum ScaleDirections
    {
        [EnumMember(Value = "Clockwise")]
        Clockwise,
        [EnumMember(Value = "CounterClockwise")]
        CounterClockwise,
    }
    [DataContract]
    public enum LabelPositions
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum PointerPositions
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum TickPositions
    {
        [EnumMember(Value = "Far")]
        Far,
        [EnumMember(Value = "Near")]
        Near,
        [EnumMember(Value = "Center")]
        Center
    }
    [DataContract]
    public enum FrameTypes
    {
        [EnumMember(Value = "fullCircle")]
        fullCircle,
        [EnumMember(Value = "halfCircle")]
        halfCircle
    }
    [DataContract]
    public enum PointerTypes
    {
        [EnumMember(Value = "Needle")]
        Needle,
        [EnumMember(Value = "Marker")]
        Marker
    }
    [DataContract]
    public enum NeedleStyles
    {
        [EnumMember(Value = "Triangle")]
        Triangle,
        [EnumMember(Value = "Rectangle")]
        Rectangle,
        [EnumMember(Value = "Trapezoid")]
        Trapezoid,
        [EnumMember(Value = "Arrow")]
        Arrow
    }
    [DataContract]
    public enum MultiSelectModeTypes
    {
        [EnumMember(Value = "Delimiter")]
        Delimiter,
        [EnumMember(Value = "VisualMode")]
        VisualMode,
        [EnumMember(Value = "None")]
        None,
    }
}