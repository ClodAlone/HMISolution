// <copyright file="_Enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Specifies the Mode for Connections.
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// Connect
        /// </summary>
        Connect,

        /// <summary>
        /// Move
        /// </summary>
        Move
    }
    /// <summary>
    /// Specifies the decorator shapes.
    /// </summary>
    public enum DecoratorShape
    {
        /// <summary>
        /// None shape
        /// </summary>
        None,

        /// <summary>
        /// Arrow shape
        /// </summary>
        Arrow,

        /// <summary>
        /// Diamond shape
        /// </summary>
        Diamond,

        /// <summary>
        /// Circle shape
        /// </summary>
        Circle,

        /// <summary>
        /// Custom Shape
        /// </summary>
        Custom
    }

    [Flags]
    public enum NodeSnapMode
    {
        None=0,
        Node=1,
        Port=2
    }

    [Flags]
    public enum ImageStretch
    {
        /// <summary>
        /// Image is not stretched. 
        /// </summary>
        None = 0,
        /// <summary>
        /// Image can be expanded by scaling. 
        /// </summary>
        Expand = 1,
        /// <summary>
        /// Image can be shrunk by scaling.
        /// </summary>
        Shrink = 1 << 1,
        /// <summary>
        /// Image can be shrunk / expanded by scaling.   
        /// </summary>
        BestFit = Expand | Shrink
    }

    //Routing Mode
    public enum RoutingMode
    {
        DragEnd,
        Immediate
    }

    public enum NodeMovementX
    {

        None,

        Z,

        X,

        Alt

    }
    public enum NodeMovementY
    {
        None,

        Y,

        Space,

        AltZ,

        AltY

    }

    public enum TranslateRailsMode
    {
        None,

        TranslateRailsX,

        TranslateRailsY

    }
    /// <summary>
    /// Specifies the LayoutHorizontalAlignment Type.
    /// </summary>
    public enum LayoutHorizontalAlignment
    {
        /// <summary>
        /// Left
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right
        /// </summary>
        Right = 2,

        /// <summary>
        ///  Center
        /// </summary>
        Center = 3
    }

    /// <summary>
    /// Specifies the LayoutVerticalAlignment Type.
    /// </summary>
    public enum LayoutVerticalAlignment
    {
        /// <summary>
        /// Top
        /// </summary>
        Top = 1,

        /// <summary>
        /// Bottom
        /// </summary>
        Bottom = 2,

        /// <summary>
        ///  Center
        /// </summary>
        Center = 3
    }

    public enum LabelOrientation
    {
        /// <summary>
        /// Auto
        /// </summary>
        Auto,

        /// <summary>
        /// Horizonatl Orientation
        /// </summary>
        Horizontal,


        /// <summary>
        /// Vertical Orientation.
        /// </summary>
        Vertical
    }


    /// <summary>
    /// Specifies the Port shape.
    /// </summary>
    public enum PortShapes
    {
        /// <summary>
        /// None shape
        /// </summary>
        None,

        /// <summary>
        /// Arrow shape
        /// </summary>
        Arrow,

        /// <summary>
        /// Diamond shape
        /// </summary>
        Diamond,

        /// <summary>
        /// Circle shape
        /// </summary>
        Circle,
        /// <summary>
        /// Custom Shape
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies the layout types.
    /// </summary>
    public enum LayoutType
    {
        /// <summary>
        /// None type.
        /// </summary>
        None,

        /// <summary>
        /// Directed tree layout.
        /// </summary>
        DirectedTreeLayout,

        /// <summary>
        /// Hierarchical tree layout.
        /// </summary>
        HierarchicalTreeLayout,

        /// <summary>
        /// Table layout.
        /// </summary>
        TableLayout,

		/// <summary>
        /// Radial tree layout.
        /// </summary>
        RadialTreeLayout,

        /// <summary>
        /// Bowtie Layout
        /// </summary>
        BowtieLayout
    }

    /// <summary>
    /// Specifies the tree orientation .
    /// </summary>
    public enum TreeOrientation
    {
        /// <summary>
        /// Left to right
        /// </summary>
        LeftRight,

        /// <summary>
        /// Right to left
        /// </summary>
        RightLeft,

        /// <summary>
        /// Top to Bottom
        /// </summary>
        TopBottom,

        /// <summary>
        /// Bottom to top
        /// </summary>
        BottomTop,
    }

    /// <summary>
    /// Specifies the Expand Mode.
    /// </summary>
    public enum ExpandMode
    {
        /// <summary>
        /// Horizontal expansion.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical expansion.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Specifies the shapes.
    /// </summary>
    public enum Shapes
    {
        /// <summary>
        /// Rectangle shape
        /// </summary>
        Rectangle,

        /// <summary>
        /// Star shape
        /// </summary>
        Star,

        /// <summary>
        /// Hexagon shape
        /// </summary>
        Hexagon,

        /// <summary>
        /// Octagon shape
        /// </summary>
        Octagon,

        /// <summary>
        /// Pentagon shape
        /// </summary>
        Pentagon,

        /// <summary>
        /// Heptagon shape
        /// </summary>
        Heptagon,

        /// <summary>
        /// Triangle shape
        /// </summary>
        Triangle,

        /// <summary>
        /// Ellipse shape
        /// </summary>
        Ellipse,

        /// <summary>
        /// Plus shape
        /// </summary>
        Plus,

        /// <summary>
        /// Rounded Rectangle
        /// </summary>
        RoundedRectangle,

        /// <summary>
        /// Rounded Square
        /// </summary>
        RoundedSquare,

        /// <summary>
        /// Right Triangle
        /// </summary>
        RightTriangle,

        /// <summary>
        /// ThreeDBox shape
        /// </summary>
        ThreeDBox,

        /// <summary>
        /// FlowChart Process shape
        /// </summary>
        FlowChart_Process,

        /// <summary>
        /// FlowChart Start shape
        /// </summary>
        FlowChart_Start,

        /// <summary>
        /// FlowChart Decision shape
        /// </summary>
        FlowChart_Decision,

        /// <summary>
        /// FlowChart_Predefined shape
        /// </summary>
        FlowChart_Predefined,

        /// <summary>
        /// FlowChart_StoredData shape
        /// </summary>
        FlowChart_StoredData,

        /// <summary>
        /// FlowChart_Document shape
        /// </summary>
        FlowChart_Document,

        /// <summary>
        /// FlowChart_Data shape
        /// </summary>
        FlowChart_Data,

        /// <summary>
        /// FlowChart_InternalStorage shape
        /// </summary>
        FlowChart_InternalStorage,

        /// <summary>
        /// FlowChart_PaperTape shape
        /// </summary>
        FlowChart_PaperTape,

        /// <summary>
        /// FlowChart_SequentialData shape
        /// </summary>
        FlowChart_SequentialData,

        /// <summary>
        /// FlowChart_DirectData shape
        /// </summary>
        FlowChart_DirectData,

        /// <summary>
        /// FlowChart_ManualInput shape
        /// </summary>
        FlowChart_ManualInput,

        /// <summary>
        /// FlowChart_Card shape
        /// </summary>
        FlowChart_Card,

        /// <summary>
        /// FlowChart_Delay shape
        /// </summary>
        FlowChart_Delay,

        /// <summary>
        /// FlowChart_Terminator shape
        /// </summary>
        FlowChart_Terminator,

        /// <summary>
        /// FlowChart_Display shape
        /// </summary>
        FlowChart_Display,

        /// <summary>
        /// FlowChart_LoopLimit shape
        /// </summary>
        FlowChart_LoopLimit,

        /// <summary>
        /// FlowChart_Preparation shape
        /// </summary>
        FlowChart_Preparation,

        /// <summary>
        /// FlowChart_ManualOperation shape
        /// </summary>
        FlowChart_ManualOperation,

        /// <summary>
        /// FlowChart_OffPageReference shape
        /// </summary>
        FlowChart_OffPageReference,

        /// <summary>
        /// FlowChart_Star shape
        /// </summary>
        FlowChart_Star,

        /// <summary>
        /// Default shape
        /// </summary>
        Default,

        /// <summary>
        /// CustomPath shape
        /// </summary>
        CustomPath
    }

    /// <summary>
    /// Specifies the units.
    /// </summary>
    public enum MeasureUnits
    {
        /// <summary>
        /// Pixel unit
        /// </summary>
        Pixel = 0,

        /// <summary>
        /// Points unit
        /// </summary>
        Point,

        /// <summary>
        /// Document unit
        /// </summary>
        Document,

        /// <summary>
        /// Display unit
        /// </summary>
        Display,

        /// ENGLISH
        /// <summary>
        /// Sixteenth Inches
        /// </summary>
        SixteenthInch,

        /// <summary>
        /// Eighth Inches
        /// </summary>
        EighthInch,

        /// <summary>
        /// Quarter Inches
        /// </summary>
        QuarterInch,

        /// <summary>
        /// Half Inches
        /// </summary>
        HalfInch,

        /// <summary>
        /// Inches unit
        /// </summary>
        Inch,

        /// <summary>
        /// Feet measurement unit
        /// </summary>
        Foot,

        /// <summary>
        /// Yards measurement unit
        /// </summary>
        Yard,

        /// <summary>
        /// Miles measurement unit
        /// </summary>
        Mile,

        /// METRIC
        /// <summary>
        /// Millimeters unit
        /// </summary>
        Millimeter,

        /// <summary>
        /// Centimeters unit
        /// </summary>
        Centimeter,

        /// <summary>
        /// Meters measurement unit
        /// </summary>
        Meter,

        /// <summary>
        /// Kilometers measurement unit
        /// </summary>
        Kilometer,
    }

    /// <summary>
    /// Specifies the connector types.
    /// </summary>
    public enum ConnectorType
    {
        /// <summary>
        /// Orthogonal line
        /// </summary>
        Orthogonal,

        /// <summary>
        /// Bezier line
        /// </summary>
        Bezier,

        /// <summary>
        /// Straight line
        /// </summary>
        Straight,

        /// <summary>
        /// Arc line
        /// </summary>
        Arc
    }

    /// <summary>
    /// Specifies the intersection mode.
    /// </summary>
    public enum IntersectionMode
    {
        /// <summary>
        /// Connects to the content.
        /// </summary>
        OnContent,

        /// <summary>
        /// Connects to outer boudaries
        /// </summary>
        OnBorder
    }

    /// <summary>
    /// Specifies Theme styles.
    /// </summary>
    [Obsolete("This feature is no more supported")]
    public enum Themes
    {
        /// <summary>
        /// Default Theme
        /// </summary>
        Default,

        /// <summary>
        /// Custom Theme
        /// </summary>
        Custom,

        /// <summary>
        /// OutLine Themes
        /// </summary>
        OutLineBlack, 
        OutLineBlue,
        OutLineRed,
        OutLineOliveGreen,
        OutLinePurple,
        OutLineAqua, 
        OutLineOrange,

        /// <summary>
        /// Fill Themes
        /// </summary>
        FillBlack, 
        FillBlue,
        FillRed, 
        FillOliveGreen,
        FillPurple, 
        FillAqua,
        FillOrange,

        /// <summary>
        /// OutLine Fill Themes
        /// </summary>
        OutLineFillBlack,
        OutLineFillBlue, 
        OutLineFillRed, 
        OutLineFillOliveGreen, 
        OutLineFillPurple, 
        OutLineFillAqua, 
        OutLineFillOrange,

        /// <summary>
        /// SubtleEffect Themes
        /// </summary>
        SubtleEffectBlack, 
        SubtleEffectBlue,
        SubtleEffectRed,
        SubtleEffectOliveGreen,
        SubtleEffectPurple,
        SubtleEffectAqua,
        SubtleEffectOrange,

        /// <summary>
        /// ModerateEffect Themes
        /// </summary>
        ModerateEffectBlack,
        ModerateEffectBlue,
        ModerateEffectRed,
        ModerateEffectOliveGreen,
        ModerateEffectPurple,
        ModerateEffectAqua,
        ModerateEffectOrange,
    }

    /// <summary>
    /// Specifies Command's Parameters.
    /// </summary>
    public enum SelectionFilter
    {
        /// <summary>
        /// Both Nodes and Line Connectors
        /// </summary>
        None,

        /// <summary>
        /// Only Nodes
        /// </summary>
        FilterLines,

        /// <summary>
        /// Only Line Connectors
        /// </summary>
        FilterNodes
    }

    public enum DrawingTools
    {
        Ellipse,

        Rectangle,

        RoundedRectangle,

        Polygon,

        StraightLine,

        BezierLine,

        PolyLine,

        OrthogonalLine,

        Arc,
        TextBox
    }


    /// <summary>
    /// Specifies Selection Option for Node.
    /// </summary>
    public enum ItemSelectionMode
    {
        /// <summary>
        /// Only one Item selected
        /// </summary>
        Single,

        /// <summary>
        /// Many Items selected
        /// </summary>
        Multiple
    }

    /// <summary>
    /// Specifies the Ordering Mode for DiagramControl Items.
    /// </summary>
    public enum ZOrderModes
    {
        Visual,
        Index
    }

    public enum BowtieSubTreePlacement
    {
        /// <summary>
        /// Left
        /// </summary>
        /// <remarks></remarks>
        Left,

        /// <summary>
        /// Right
        /// </summary>
        /// <remarks></remarks>
        Right
    }

    public enum CustomLabelPositions
    {
        /// <summary>
        /// Default position
        /// </summary>        
        Auto,
        /// <summary>
        /// Customized Position
        /// </summary>        
        Custom,
        /// <summary>
        /// Dragging Position
        /// </summary>        
        Drag
    }


    internal enum LineContext
    {
        Segment,
        Line
    }

    public enum LineUnit
    {
        /// <summary>
        /// Absolute values in fraction
        /// </summary>
        /// <remarks></remarks>
        AbsoluteFraction,

        /// <summary>
        /// Relative value in fraction
        /// </summary>
        /// <remarks></remarks>
        RelativeFraction,

        /// <summary>
        /// Absolute value in pixel
        /// </summary>
        /// <remarks></remarks>
        AbsoluteValue,

        /// <summary>
        /// Relative value in pixel
        /// </summary>
        /// <remarks></remarks>
        RelativeValue
    }

    /// <summary>
    /// Specifies the Orientation of the FirstSegment.
    /// </summary>
    public enum SegmentOrientation
    {

        /// <summary>
        /// Default Value
        /// </summary>     
        Auto,

        /// <summary>
        /// First Segment Horizontal to HeadNode
        /// </summary>     
        Horizontal,

        /// <summary>
        /// First Segment Vertical to HeadNode
        /// </summary>   
        Vertical
    }

    /// <summary>
    /// Customize the DateTime format string for the Rulers.
    /// </summary>
    /// <remarks></remarks>
    internal enum DateTimeFormatSetting
    {
        /// <summary>
        /// DateTime format sting will be update automatically based on DateTimeSettings
        /// </summary>
        /// <remarks></remarks>
        Auto,

        /// <summary>
        /// Allows user to specify a custom format string for DateTime.
        /// </summary>
        /// <remarks></remarks>
        Custom
    }

    /// <summary>
    /// Options for port Visibility
    /// </summary>
    public enum PortVisibility
    {
        /// <summary>
        /// Port will be visible always
        /// </summary>
        AlwaysVisible,

        /// <summary>
        /// Port will not be visible
        /// </summary>
        AlwaysHidden,

        /// <summary>
        /// Visible when mouse is over the Node.
        /// </summary>
        MouseOverNode
    }

    /// <summary>
    /// Options for Drawing
    /// </summary>
    public enum DrawingMode
    {
        /// <summary>
        /// Port will be visible always
        /// </summary>
        Default,

        /// <summary>
        /// Port will not be visible
        /// </summary>
        Continous
    }
    ///<summary>
    ///Options for Node Deleting
    ///</summary>
    public enum DeletingMode
    {
        ///<summary>
        ///Node and its children are all will be deleted
        ///</summary>
        DeleteDependentEdges,

        ///<summary>
        ///Delete Node only
        ///</summary>
        None
    }

    ///<summary>
    ///Options for Node Deleting
    ///</summary>
    public enum ItemGenerateMode
    {
        ///<summary>
        ///Node and its children are all will be deleted
        ///</summary>
        ItemsSource,

        ///<summary>
        ///Delete Node only
        ///</summary>
        Manual
    }

}
