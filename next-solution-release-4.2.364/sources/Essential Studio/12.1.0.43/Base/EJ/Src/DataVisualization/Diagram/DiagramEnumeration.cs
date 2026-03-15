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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.DiagramEnums
{
    [DataContract]
    [Flags]
    public enum PortVisibility
    {
        Visible = 1 << 0,
        Hidden = 1 << 1,
        Hover = 1 << 2,
        Connection = 1 << 3,
        Default = 1 << 2 | 1 << 3
    }

    [DataContract]
    public enum PortShapes
    {
        [EnumMember(Value = "x")]
        X,
        [EnumMember(Value = "circle")]
        Circle,
        [EnumMember(Value = "square")]
        Square,
        [EnumMember(Value = "path")]
        Path
    }

    [DataContract]
    public enum DecoratorShapes
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "arrow")]
        Arrow,
        [EnumMember(Value = "openArrow")]
        OpenArrow,
        [EnumMember(Value = "circle")]
        Circle,
        [EnumMember(Value = "diamond")]
        Diamond,
        [EnumMember(Value = "path")]
        Path
    }

    [DataContract]
    public enum TextDecorations
    {
        [EnumMember(Value = "underline")]
        Underline,
        [EnumMember(Value = "overline")]
        Overline,
        [EnumMember(Value = "lineThrough")]
        LineThrough,
        [EnumMember(Value = "none")]
        None
    }

    [DataContract]
    public enum TextAlign
    {
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "center")]
        Center
    }

    [DataContract]
    public enum Orientation
    {
        [EnumMember(Value = "landscape")]
        Landscape,
        [EnumMember(Value = "portrait")]
        Portrait
    }

    [DataContract]
    public enum HorizontalAlignment
    {
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "right")]
        Right
    }

    [DataContract]
    public enum VerticalAlignment
    {
        [EnumMember(Value = "top")]
        Top,
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "bottom")]
        Bottom
    }

    [DataContract]
    public enum LabelEditMode
    {
        [EnumMember(Value = "edit")]
        Edit,
        [EnumMember(Value = "view")]
        View 
    }

    [DataContract]
    [Flags]
    public enum NodeConstraints
    {
        None = 1 << 0,   
        Select = 1 << 1,
        Delete = 1 << 2,
        Resize = 1 << 3,
        Drag = 1 << 4,
        Rotate = 1 << 5,
        InConnect = 1 << 6,
        OutConnect = 1 << 7,
        Connect = 1 << 6 | 1 << 7,
        Default = 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 
    }

    [DataContract]
    [Flags]
    public enum ConnectorConstraints
    {
        None =  1 << 0 ,
        Select = 1 << 1,
        Delete = 1 << 2,
        Drag = 1 << 4,
        DragSourceEnd = 1 << 5,
        DragTargetEnd = 1 << 6,
        Default = 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 6 
    }

    [DataContract]
    [Flags]
    public enum PortConstraints
    {
        None = 1 << 0,
        Inherit = 1 << 1,
        InConnect = 1 << 2,
        OutConnect = 1 << 3,
        Connect = 1 << 1 | 1 << 2
    }

    [DataContract]
    [Flags]
    public enum SnapConstraints
    {
        None = 0,
        SnapToHorizontalLines = 1,
        SnapToVerticalLines = 2,
        SnapToLines = 1 | 2,
        ShowHorizontalLines = 4,
        ShowVerticalLines = 8,
        ShowLines = 4 | 8,
        All = 1 | 2 | 4 | 8
    } 
}
