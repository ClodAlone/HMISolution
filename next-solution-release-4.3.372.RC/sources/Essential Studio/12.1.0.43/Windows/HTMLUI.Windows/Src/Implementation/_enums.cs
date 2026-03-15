#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Tokens which represent attribute values.
    /// </summary>
    internal enum AttributeToken
    {
        #region Enum Flags
        /// <summary>
        /// Default value.
        /// </summary>
        Empty,

        /// <summary>
        /// Value is an integer type.
        /// </summary>
        Integer,

        /// <summary>
        /// Value is in percentage form.
        /// </summary>
        Percent,

        /// <summary>
        /// Value is a Boolean type.
        /// </summary>
        Bool,

        /// <summary>
        /// Value has a string type.
        /// </summary>
        String,

        /// <summary>
        /// Value has a point form (Integer, Integer).
        /// </summary>
        Point,

        /// <summary>
        /// Value has a float type.
        /// </summary>
        Float,

        /// <summary>
        /// Value has a triangle form (Integer, Integer, Integer).
        /// </summary>
        Triangle,

        /// <summary>
        /// Value has a rectangle form (Integer, Integer, Integer, Integer).
        /// </summary>
        Rectangle,

        /// <summary>
        /// Value has font names.
        /// </summary>
        Font,

        /// <summary>
        /// Value is a color represented by hash value (#XXXXXX).
        /// </summary>
        Color_Hex,

        /// <summary>
        /// Value is a color represented by RGB values (rgb(XX, XX, XX)).
        /// </summary>
        Color_rgb,

        /// <summary>
        /// Value is a color represented by name (black, white, cyan, etc ...).
        /// </summary>
        Color_Word,

        /// <summary>
        /// Value is a URI.
        /// </summary>
        Uri,

        /// <summary>
        /// Value is a border style.
        /// </summary>
        BorderStyle,

        /// <summary>
        /// Value is a font family.
        /// </summary>
        FontFamily,

        /// <summary>
        /// Value is a font style.
        /// </summary>
        FontStyle,

        /// <summary>
        /// Value is a font weight.
        /// </summary>
        FontWeight,

        /// <summary>
        /// Value is a font size.
        /// </summary>
        FontSize,

        /// <summary>
        /// Value is a text decoration.
        /// </summary>
        TextDecoration,

        /// <summary>
        /// Value has a complex form (consists of a few tokens).
        /// </summary>
        Complex
        #endregion
    }

    /// <summary>
    /// Specifies the type of the list item.
    /// </summary>

    public enum ListItemType
    {
        #region Enum Flags
        /// <summary>
        /// Default. Associate numbers with each item in an ordered list.
        /// </summary>
        _1,

        /// <summary>
        /// Associate lowercase letters with each item in an ordered list.
        /// </summary>
        a,

        /// <summary>
        /// Associate uppercase letters with each item in an ordered list.
        /// </summary>
        A,

        /// <summary>
        /// Associate small Roman numerals with each item in an ordered list.
        /// </summary>
        i,

        /// <summary>
        /// Associate large Roman numerals with each item in an ordered list.
        /// </summary>
        I,

        /// <summary>
        /// Associate a solid disc with each item in an unordered list.
        /// </summary>
        disc,

        /// <summary>
        /// Associate a hollow circle with each item in an unordered list.
        /// </summary>
        circle,

        /// <summary>
        /// Associate a solid square with each item in an unordered list.
        /// </summary>
        square
        #endregion
    }

    /// <summary>
    /// Enum type which describes the type of the reaction when some attributes
    /// of an element changes.
    /// </summary>
    internal enum ReactType
    {
        #region Enum Flags
        /// <summary>
        /// No reaction on changing attribute.
        /// </summary>
        None,

        /// <summary>
        /// Element must be repainted.
        /// </summary>
        RePaintElement,

        /// <summary>
        /// All documents must be repainted.
        /// </summary>
        RePaintDocument,

        /// <summary>
        /// Element must recalculate its position.
        /// </summary>
        RePositionElement,

        /// <summary>
        /// Document must recalculate its position.
        /// </summary>
        RePositionDocument,

        /// <summary>
        /// Document must recalculate including size, position and redrawing.
        /// </summary>
        ReCalculatingDocument,

        /// <summary>
        /// Format for the element must be re-merged.
        /// </summary>
        ReFormatMergingElement,

        /// <summary>
        /// Format for the element must be re-merged and the document must be recalculated.
        /// </summary>
        ReFormatMergElmAndReCalcDoc,

        /// <summary>
        /// Format for the element must be re-merged and all of its children and documents must be recalculated.
        /// </summary>
        ReFormatMergElmAndChildrenAndReCalcDoc,

        /// <summary>
        /// Format for the element must be created and the document must be recalculated.
        /// </summary>
        ReFormatCrtElmAndReCalcDoc,

        /// <summary>
        /// Format for the document must be recreated and the document must be recalculated.
        /// </summary>
        ReFormatCrtDocAndReCalcDoc,

        /// <summary>
        /// Inherited item of format is changed. Recalculate format for all child elements.
        /// </summary>
        ReChildrenFormatMerge,

        /// <summary>
        /// Inherited item of format is changed. Recalculate format for all child elements
        /// and recalculate the document.
        /// </summary>
        ReChildrenFormatMergeAndRecalcDoc
        #endregion
    }

    /// <summary>
    /// Specifies the level of the document's reaction after disabling QuietMode.
    /// </summary>
    [Flags]
    internal enum ReactLevel
    {
        #region Enum Flags
        /// <summary>
        /// No reaction.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Repaint document.
        /// </summary>
        RePaintdoc = 0x0001,

        /// <summary>
        /// Recalculate document.
        /// </summary>
        ReCalculateDoc = 0x0002 | RePaintdoc,

        /// <summary>
        /// Merge all formats.
        /// </summary>
        ReMergeFormats = 0x0004,

        /// <summary>
        /// Create all formats.
        /// </summary>
        ReFormatsCreate = 0x0008
        #endregion
    }

    /// <summary>
    /// Specifies the type of the element from which depends the position of element.
    /// </summary>
    [Flags]
    internal enum ElementType
    {
        #region Enum Flags
        /// <summary>
        /// Type is unknown.
        /// </summary>
        Unknown = 0x000000000,

        /// <summary>
        /// Element is inline (font, span, etc.).
        /// </summary>
        InLine = 0x000000001,

        /// <summary>
        /// Element has a hard defined fixed size. It has a block structure.
        /// </summary>
        BlockFixedSize = 0x000000002,

        /// <summary>
        /// Element has a block structure, but its size is not hard defined.
        /// </summary>
        BlockResizable = 0x000000004,

        /// <summary>
        /// Element is always postponed on new line and has fixed size.
        /// </summary>
        BlockNewLineFixedSize = 0x000000008,

        /// <summary>
        /// Element is always postponed on new line and is resizable (hr, div).
        /// </summary>
        BlockNewLineResizable = 0x000000010,

        /// <summary>
        /// Spaces must be inserted before and after element and the element is resizable.
        /// </summary>
        BlockNewLineResizableIndent = 0x000000020,

        /// <summary>
        /// Spaces must be inserted before and after element and the element has fixed size.
        /// </summary>
        BlockNewLineFixedSizeIndent = 0x000000040,

        /// <summary>
        /// Element has a block structure and its size depends on its inner content.
        /// </summary>
        BlockNewLineSimple = 0x000000080,

        /// <summary>
        /// Element has a block structure, not in a new line and its size depends on its inner content.
        /// </summary>
        BlockSimple = 0x000000100,

        /// <summary>
        /// Element must be postponed onto a new line, but it is linear.
        /// </summary>
        InLineNewLine = 0x000000200,

        /// <summary>
        /// Element has a block structure.
        /// </summary>    
        Block = BlockFixedSize | BlockResizable | BlockNewLineFixedSize | BlockNewLineResizable | BlockNewLineResizableIndent | BlockNewLineFixedSizeIndent | BlockNewLineSimple | BlockSimple,

        /// <summary>
        /// Element is always postponed on a new line.
        /// </summary>
        BlockNewLine = BlockNewLineFixedSize | BlockNewLineResizable | BlockNewLineResizableIndent | BlockNewLineFixedSizeIndent | BlockNewLineSimple,

        /// <summary>
        ///  Spaces must be inserted before and after element.
        /// </summary>
        BlockNewLineIndent = BlockNewLineResizableIndent | BlockNewLineFixedSizeIndent,

        /// <summary>
        /// Element has fixed size.
        /// </summary>
        BlockFixed = BlockFixedSize | BlockNewLineFixedSize | BlockNewLineFixedSizeIndent,

        /// <summary>
        /// Element must be postponed onto a new line.
        /// </summary>
        NewLine = InLineNewLine | BlockNewLine
        #endregion
    }

    /// <summary>
    /// Enum indicates which properties of the format are redefined and must be changed.
    /// Used in merging of formats.
    /// </summary>
    [Flags]
    internal enum MergeMask : long
    {
        #region Enum Flags
        /// <summary>
        /// Nothing is changed.
        /// </summary>
        None = 0x000000000,

        /// <summary>
        /// Forecolor property is changed.
        /// </summary>
        ForeColor = 0x000000001,

        /// <summary>
        /// BgColor property is changed.
        /// </summary>
        BgColor = 0x000000002,

        /// <summary>
        /// Vertical Alignment property is changed.
        /// </summary>
        VAlignmnet = 0x000000004,

        /// <summary>
        /// Horizontal Alignment property is changed.
        /// </summary>
        HAlignment = 0x000000008,

        /// <summary>
        /// Font Family property is changed.
        /// </summary>
        FontFamily = 0x000000010,

        /// <summary>
        /// Font Size property is changed.
        /// </summary>
        FontSize = 0x000000020,

        /// <summary>
        /// Font Style property is changed.
        /// </summary>
        FontStyle = 0x000000040,

        /// <summary>
        /// Cursor property is changed.
        /// </summary>
        Cursor = 0x000000080,

        /// <summary>
        /// Padding left property is changed.
        /// </summary>
        PaddingLeft = 0x000000100,

        /// <summary>
        /// Padding top property is changed.
        /// </summary>
        PaddingTop = 0x000000200,

        /// <summary>
        /// Padding right property is changed.
        /// </summary>
        PaddingRight = 0x000000400,

        /// <summary>
        /// Padding bottom property is changed.
        /// </summary>
        PaddingBottom = 0x000000800,

        /// <summary>
        /// Border Style property is changed.
        /// </summary>
        BorderStyle = 0x000001000,

        /// <summary>
        /// Border Color property is changed.
        /// </summary>
        BorderColor = 0x000002000,

        /// <summary>
        /// Border Width property is changed.
        /// </summary>
        BorderWidth = 0x000004000,

        /// <summary>
        /// Left Border property is changed.
        /// </summary>
        BorderLeft = 0x000008000,

        /// <summary>
        /// Top Border property is changed.
        /// </summary>
        BorderTop = 0x000010000,

        /// <summary>
        /// Right Border property is changed.
        /// </summary>
        BorderRight = 0x000020000,

        /// <summary>
        /// Border Bottom property is changed.
        /// </summary>
        BorderBottom = 0x000040000,

        /// <summary>
        /// Bg Image property has been changed.
        /// </summary>
        BgImage = 0x000080000,

        /// <summary>
        /// Bg Repeat Style property is changed.
        /// </summary>
        BgRepeat = 0x000100000,

        /// <summary>
        /// Border Left Color property is changed.
        /// </summary>
        BorderLeftColor = 0x000200000,

        /// <summary>
        /// Width property is changed.
        /// </summary>
        Width = 0x000400000,

        /// <summary>
        /// Height property is changed.
        /// </summary>
        Height = 0x000800000,

        /// <summary>
        /// Font Weight property is changed.
        /// </summary>
        FontWeight = 0x001000000,

        /// <summary>
        /// Text Decorarion property is changed.
        /// </summary>
        TextDecoration = 0x002000000,

        /// <summary>
        /// All Font properties are changed.
        /// </summary>
        FontAll = FontStyle | FontSize | FontFamily | FontWeight | TextDecoration,

        /// <summary>
        /// All Border properties are changed.
        /// </summary>
        BorderAll = BorderLeft | BorderTop | BorderRight | BorderBottom,

        /// <summary>
        /// Vertical and Horizontal Alignment properties are changed.
        /// </summary>
        AlignmentAll = VAlignmnet | HAlignment,

        /// <summary>
        /// All Padding properties are changed.
        /// </summary>
        PaddingAll = PaddingLeft | PaddingTop | PaddingRight | PaddingBottom,

        /// <summary>
        /// All properties are changed.
        /// </summary>
        All = FontAll | BorderAll | AlignmentAll | PaddingAll | Cursor | ForeColor | BgColor | Width | Height | BgImage | BgRepeat,

        /// <summary>
        /// Parameters are inherited by default.
        /// </summary>
        Inherit = FontAll | HAlignment | Cursor | ForeColor | BgColor
        #endregion
    }
}
