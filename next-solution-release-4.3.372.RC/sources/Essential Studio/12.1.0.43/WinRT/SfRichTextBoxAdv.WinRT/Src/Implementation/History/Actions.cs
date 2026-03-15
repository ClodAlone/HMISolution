#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal enum Actions
    {
        Bold,
        Italic,
        FontSize,
        FontFamily,
        FontColor,
        HighlightColor,
        BaselineAlignment,
        StrikeThrough,
        Underline,
        TextAlignment,
        LineSpacingType,
        LineSpacing,
        AfterSpacing,
        BeforeSpacing,
        LeftIndent,
        RightIndent,
        FirstLineIndent,
        ImageReszing,
        Insert,
        InsertHyperlink,
        InsertInline,
        InsertRowAbove,
        InsertRowBelow,
        InsertColumnLeft,
        InsertColumnRight,
        InsertTable,
        Enter,
        BackSpace,
        Delete,
        Cut,
        Paste,
        ClearCells,
        DeleteCells,
        DeleteRow,
        DeleteColumn,
        DeleteTable,
        MergeCells,
        TableCellBackground,
        ListFormat,
        DragDrop,
        PageMargin,
        PageSize
    }

    internal enum UndoType
    {
        Normal,

        SelectionBased
    }
}
