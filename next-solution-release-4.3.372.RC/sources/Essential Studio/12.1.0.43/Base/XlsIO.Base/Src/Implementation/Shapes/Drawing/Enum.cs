#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    internal enum PlacementType
    {
        FreeFloating,
        Move,
        MoveAndSize
    }
    internal enum ShapeDrawingType
    {
        Arc = 4,
        Button = 7,
        CellsDrawing = 30,
        Chart = 5,
        CheckBox = 11,
        ComboBox = 20,
        Comment = 0x19,
        DialogBox = 15,
        Group = 0,
        GroupBox = 0x13,
        Label = 14,
        Line = 1,
        ListBox = 0x12,
        OleObject = 0x18,
        Oval = 3,
        Picture = 8,
        Polygon = 9,
        RadioButton = 12,
        Rectangle = 2,
        ScrollBar = 0x11,
        Spinner = 0x10,
        TextBox = 6,
        Unknown = 0x1d
    }
    internal enum ExcelAutoShapeType
    {
        sp,
        grpSp,
        graphicFrame,
        cxnSp,
        pic,
        contentPart,
    }
    internal enum AnchorType
    {
        Absolute,
        RelSize,
        OneCell,
        TwoCell,
    }
    public enum TextDirection
    {
        Horizontal,
        RotateAllText90,
        RotateAllText270,
        StackedLeftToRight,
        StackedRightToLeft,
    }
    public enum ExcelHorizontalAlignment
    {
        Left,
        Center,
        Right,
        LeftMiddle,
        CenterMiddle,
        RightMiddle,
    }
    public enum ExcelVerticalAlignment
    {
        Top,
        Middle,
        Bottom,
        TopCentered,
        MiddleCentered,
        BottomCentered,
    }

    public enum TextVertOverflowType
    {
        OverFlow,
        Ellipsis,
        Clip,
    }
    public enum TextHorzOverflowType
    {
        OverFlow,
        Clip,
    }
    public struct TextFrameColumns
    {
        private int number;
        private int spacingPt;

        public int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }
        public int SpacingPt
        {
            get { return this.spacingPt; }
            set { this.spacingPt = value; }
        }
    }
}
