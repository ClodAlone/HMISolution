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

namespace Syncfusion.UI.Xaml.Diagram
{
    [Flags]
    public enum NodeConstraints
    {
        None = 0,
        Selectable = 1 << 0,
        Draggable = 1 << 1,
        Resizable = 1 << 2,
        Rotatable = 1 << 3,
        InConnect = 1 << 4,
        OutConnect = 1 << 5,
        SnapToHorizontalLines=1<<6,
        SnapToVerticalLines=1<<7,
        SnapAngle=1<<8,
        SnapToLines=SnapToHorizontalLines|SnapToVerticalLines,
        Connectable = InConnect | OutConnect,
        AllowPan = 1 << 9,
        InheritSnapping=1<<10,
        InheritSnapToObject = 1 << 11,
        Inherit = InheritSnapToObject | InheritSnapping,
        Default = Selectable | Draggable | Resizable | Rotatable | Connectable | Inherit
    }

    [Flags]
    public enum ConnectorConstraints
    {
        None = 0,
        Selectable = 1 << 0,
        SourceDraggable = 1 << 1,
        TargetDraggable = 1 << 2,
        EndDraggable = SourceDraggable | TargetDraggable,
        EndThumbs = 1 << 3,
        SegmentThumbs = 1 << 4,
        Thumbs = EndThumbs | SegmentThumbs,
        Bridging = 1 << 5,
        Routing = 1 << 6,
        SnapToHorizontalLines=1<<7,
        SnapToVerticalLines=1<<8,
        SnapToLines=SnapToHorizontalLines|SnapToVerticalLines,
        InheritBridging=1<<9,
        InheritRouting=1<<10,
        InheritSnapping=1<<11,
        InheritSnapToObject = 1 << 12,
        InheritSmoothness = 1 << 13,
        Inherit = InheritBridging | InheritRouting | InheritSnapToObject | InheritSnapping | InheritSmoothness,
        
        //SegmentDraggable = 1 << 3,
        Default = Selectable | EndDraggable | Inherit | Thumbs // | SegmentDraggable
    }

    [Flags]
    public enum PortConstraints
    {
        None = 0,
        Inherit = 1 << 0,
        InConnect = 1 << 1,
        OutConnect = 1 << 2,
        Connectable = InConnect | OutConnect,
    }

    [Flags]
    public enum GraphConstraints
    {
        None = 0,
        Zoomable = 1 << 0,
        PannableX = 1 << 1,
        PannableY = 1 << 2,
        Pannable = PannableX | PannableY,
        PanRailsX = 1 << 3,
        PanRailsY = 1 << 4,
        Undoable = 1 << 5,
        Virtualize = 1 << 6,
        Relationship = 1 << 7,
        Events = 1 << 8,
        Bridging = 1 << 9,
        Routing=1<<10,
        PanRails = PanRailsX | PanRailsY,
        Default = Zoomable | Pannable | PanRails | Relationship | Events
    }

    [Flags]
    public enum Tool
    {
        None = 0,
        SingleSelect = 1 << 0,
        MultipleSelect = 1 << 1,
        ZoomPan = 1 << 2,
        DrawOnce = 1 << 3,
        ContinuesDraw = 1 << 4
    }

    public enum DrawingTool
    {
        Connector
    }

    internal enum PageSetup
    {
        PrintandPageProperties,       
        CustomSize
    };

    //public enum SelectionMode
    //{
    //    None,
    //    Single,
    //    Multiple
    //}

    [Flags]
    public enum MultipleSelectionMode
    {
        None = 0,
        RubberBandCompleteIntersect = 1 << 0,
        RubberBandPartialIntersect = 1 << 1,
        JustTap = 1 << 2,
        HoldKeyAndTap = 1 << 3,
        Default = RubberBandCompleteIntersect | HoldKeyAndTap
        //HoldItemAndTap = 1 << 4
    }

    [Flags]
    public enum SnapToObject
    {
        None = 0,
        LeftLeft = 1,
        TopTop = 1 << 1,
        RightRight = 1 << 2,
        BottomBottom = 1 << 3,
        LeftRight = 1 << 4,
        RightLeft = 1 << 5,
        TopBottom = 1 << 6,
        BottomTop = 1 << 7,
        HorizontalCenter = 1 << 8,
        VerticalCenter = 1 << 9,
        HorizontalSpacing = 1 << 10,
        VerticalSpacing = 1 << 11,
        Width = 1 << 12,
        Height = 1 << 13,
        Left = LeftLeft | LeftRight,
        Right = RightRight | RightLeft,
        Top = TopTop | TopBottom,
        Bottom = BottomBottom | BottomTop,
        Size = Width | Height,
        All = Left | Right | Top | Bottom | HorizontalCenter | VerticalCenter | HorizontalSpacing | VerticalSpacing | Size
    }

    [Flags]
    public enum SnapConstraints
    {
        None = 0,
        HorizontalLines = 1,
        VerticalLines = 1 << 1,
        ShowLines = HorizontalLines | VerticalLines,
        SnapToHorizontalLines = 1 << 2,
        SnapToVerticalLines = 1 << 3,
        Rotation=1<<4,
        SnapToLines = SnapToHorizontalLines | SnapToVerticalLines,
        All = ShowLines | SnapToLines|Rotation
    }

    /// <summary>
    /// Describes curve editing
    /// </summary>
    [Flags]
    public enum BezierSmoothness
    {
        None=0,
        SymmetricAngle=1,
        SymmetricDistance=1<<1,
        Symmetric=SymmetricAngle|SymmetricDistance
    }

    /// <summary>
    /// Segment Constraints
    /// </summary>
    [Flags]
    public enum SegmentConstraints
    {
        None=0,
        Inherit=1
    }

    /// <summary>
    /// The side of the node that is going to be snapped with Gridline
    /// </summary>
    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>
    /// Reason or Target of the snap
    /// </summary>
    [Flags]
    public enum SnapReason
    {
        GridLine=1,
        Sides=1<<1,
        Segment=1<<2,
        EqualSpace=1<<3,
        Size=1<<4,
        Angle=1<<5
    }
   

    /// <summary>
    /// The property that is going to be changed
    /// </summary>
    [Flags]
    public enum SnapChanges
    {
        None=0,
        X=1,
        Y=1<<1,
        Width=1<<2,
        Height=1<<3,
        Angle=1<<4
    }

    


    //internal enum GraphState
    //{
    //    Idle,
    //    UIInteraction,
    //    CommandExecuting,
    //    Changing,
    //    Undoing,
    //    Redoing,
    //    Layouting,
    //}

    public enum ExportMode
    {
        PageSettings,
        Content
    };

    /// <summary>
    /// Describes the behavior of Reset Command
    /// </summary>
    [Flags]
    public enum Reset
    {
        None=0,
        Zoom=1,
        Pan=1<<1,
        ZoomPan=Zoom|Pan
    }

    /// <summary>
    /// Describes the behavior of Flip Command
    /// </summary>
    [Flags]
    public enum Flip
    {
        None=0,
        HorizontalFlip=1,
        VerticalFlip=1<<1,
        Flip=HorizontalFlip|VerticalFlip
    }

    /// <summary>
    /// Describes the behavior of FitToPage Command
    /// </summary>
    [Flags]
    public enum FitToPage
    {
        None,
        FitToWidth=1,
        FitToHeight=1<<1,
        FitToPage = FitToHeight | FitToWidth
    }

    /// <summary>
    /// Describes the Zoom command
    /// </summary>
    public enum ZoomCommand
    {
        ZoomIn,
        ZoomOut
    }

    public enum ExpandMode
    {
      One,
      OneOrMore,
      ZeroOrOne,
      ZeroOrMore
    }

    public enum StencilConstraints
    {
        None = 0,
        ShowPreview = 1 << 0,
        Filters = 1 << 1,
        Default = ShowPreview | Filters
    }

    public enum SymbolDropMode
    {
        Drop,
        ContinueDragging,
        Cancel
    }
}
