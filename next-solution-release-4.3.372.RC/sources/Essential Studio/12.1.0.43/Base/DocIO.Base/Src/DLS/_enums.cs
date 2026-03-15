#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.DocIO.DLS
{
    #region enum EntityType
    /// <summary>
    /// Specifies the type of a DLS entity.
    /// </summary>
    public enum EntityType
    {
        WordDocument,
        /// <summary>
        /// The text bodies
        /// </summary>
        Section,
        TextBody,
        HeaderFooter,
        /// <summary>
        /// The text body items
        /// </summary>
        Paragraph,
        AlternateChunk,
        StructureDocumentTag,
        StructureDocumentTagInline,
        StructureDocumentTagRow,
        StructureDocumentTagCell,
        SDTBlockContent,
        SDTInlineContent,
        SDTRowContent,
        SDTCellContent,
        Table,
        TableRow,
        TableCell,
        /// <summary>
        /// The paragraph items
        /// </summary>
        TextRange,
        Picture,
        Field,
        FieldMark,
        MergeField,
        SeqField,
        EmbededField,
        ControlField,
        TextFormField,
        DropDownFormField,
        CheckBox,
        BookmarkStart,
        BookmarkEnd,
        Shape,
        Comment,
        Footnote,
        TextBox,
        //Tab,
        Break,
        Symbol,
        TOC,
        XmlParaItem,
        Undefined,
        CommentMark,
        CommentEnd,
        OleObject,
        AbsoluteTab,
        AutoShape
    }
    #endregion

    #region enum HeaderFooterType
    /// <summary>
    /// Specifies type of the Header/Footer.
    /// </summary>
    public enum HeaderFooterType
    {
        /// <summary>
        /// Header for even numbered pages.
        /// </summary>
        EvenHeader,
        /// <summary>
        /// Header for odd numbered pages.
        /// </summary>
        OddHeader,
        /// <summary>
        /// Footer for even numbered pages.
        /// </summary>
        EvenFooter,
        /// <summary>
        /// Footer for odd numbered pages.
        /// </summary>
        OddFooter,
        /// <summary>
        /// Header for the first page of the section. 
        /// </summary>
        FirstPageHeader,
        /// <summary>
        /// Footer for the first page of the section. 
        /// </summary>
        FirstPageFooter
    }
    #endregion

    #region enum ShapeType
    /// <summary>
    /// Specifies type of the Shape.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// Specifies text shape.
        /// </summary>
        Text,
        /// <summary>
        /// Specifies rectangle shape.
        /// </summary>
        Rectangle,
        /// <summary>
        /// Specifies image shape.
        /// </summary>
        Image,
        /// <summary>
        /// Specifies arc shape.
        /// </summary>
        Arc,
        /// <summary>
        /// Specifies ellipse shape.
        /// </summary>
        Ellipse,
        /// <summary>
        /// Specifies line shape.
        /// </summary>
        Line,
        /// <summary>
        /// Specifies path shape.
        /// </summary>
        Path,
        /// <summary>
        /// Specifies bezier shape.
        /// </summary>
        Bezier,
        /// <summary>
        /// Specifies pie shape.
        /// </summary>
        Pie,
        /// <summary>
        /// Specifies polygon shape.
        /// </summary>
        Polygon
    }
    #endregion

    #region enum StyleType
    /// <summary>
    /// Specifies type of the Style.
    /// </summary>
    public enum StyleType
    {
        /// <summary>
        /// The style is a paragraph style. 
        /// </summary>
        ParagraphStyle,
        /// <summary>
        /// The style is a character style. 
        /// </summary>
        CharacterStyle,
        /// <summary>
        /// The style is a table style.
        /// </summary>
        TableStyle,
        /// <summary>
        /// The style is a numbering style.
        /// </summary>
        NumberingStyle,
        /// <summary>
        /// The style is other kind of style. 
        /// </summary>
        OtherStyle,
    }
    #endregion

    #region enum ParagraphItemType
    /// <summary>
    /// Specifies type of the ParagraphItem.
    /// </summary>
    public enum ParagraphItemType
    {
        /// <summary>
        /// ParagraphItem is a text.
        /// </summary>
        TextRange,
        /// <summary>
        /// ParagraphItem is a picture.
        /// </summary>
        Picture,
        /// <summary>
        /// ParagraphItem is a field.
        /// </summary>
        Field,
        /// <summary>
        /// Paragraph item is field mark.
        /// </summary>
        FieldMark,
        /// <summary>
        /// 
        /// </summary>
        MergeField,
        /// <summary>
        /// 
        /// </summary>
        FormField,
        /// <summary>
        /// 
        /// </summary>
        CheckBox,
        /// <summary>
        /// 
        /// </summary>
        TextFormField,
        /// <summary>
        /// 
        /// </summary>
        DropDownFormField,
        /// <summary>
        /// 
        /// </summary>
        SeqField,
        /// <summary>
        /// 
        /// </summary>
        EmbedField,
        /// <summary>
        /// Paragraph item is form control field.
        /// </summary>
        ControlField,
        /// <summary>
        /// ParagraphItem is a start of bookmark.
        /// </summary>
        BookmarkStart,
        /// <summary>
        /// ParagraphItem is a end of bookmark.
        /// </summary>
        BookmarkEnd,
        /// <summary>
        /// 
        /// </summary>
        ShapeObject,
        /// <summary>
        /// 
        /// </summary>
        InlineShapeObject,
        /// <summary>
        /// ParagraphItem is a comment.
        /// </summary>
        Comment,
        /// <summary>
        /// ParagraphItem is a footnote.
        /// </summary>
        Footnote,
        /// <summary>
        /// ParagraphItem is a textbox. 
        /// </summary>
        TextBox,
        /// <summary>
        /// 
        /// </summary>
        //Tab,
        /// <summary>
        /// PragraphItem is a break.
        /// </summary>
        Break,
        /// <summary>
        /// 
        /// </summary>    
        Symbol,
        /// <summary>
        /// 
        /// </summary>
        TOC,
        /// <summary>
        /// 
        /// </summary>
        OleObject
    }
    #endregion

    #region enum PageOrientation
    /// <summary>
    /// Specifies orientation of the page.
    /// </summary>
    public enum PageOrientation
    {
        /// <summary>
        /// Portrait page orientation.
        /// </summary>
        Portrait = 0,
        /// <summary>
        /// Landscape page orientation.
        /// </summary>
        Landscape = 2,
    }
    #endregion

    #region enum PageAlignment
    /// <summary>
    /// Specifies alignment of the text on a page.
    /// </summary>
    public enum PageAlignment
    {
        /// <summary>
        /// Text is aligned at the top of the page.
        /// </summary>
        Top = 0,
        /// <summary>
        /// Text is aligned at the middle of the page.
        /// </summary>
        Middle = 1,
        /// <summary>
        /// Text is spanned to fill the page. 
        /// </summary>
        Justified = 2,
        /// <summary>
        /// Text is aligned at the bottom of the page.
        /// </summary>
        Bottom = 3
    }
    #endregion

    #region enum VerticalAlignment
    /// <summary>
    /// Specifies type of the vertical alignment.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Specifies top alignment.
        /// </summary>
        Top = 0,
        /// <summary>
        /// Specifies middle alignment.
        /// </summary>
        Middle = 1,
        /// <summary>
        /// Specifies bottom alignment.
        /// </summary>
        Bottom = 2
    }
    #endregion

    #region enum HorizonatalAlignment
    /// <summary>
    /// Specifies type of the horizontal alignment.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// Specifies alignment to the left. 
        /// </summary>
        Left,
        /// <summary>
        /// Specifies alignment to the center. 
        /// </summary>
        Center,
        /// <summary>
        /// Specifies alignment to the right. 
        /// </summary>
        Right,
        /// <summary>
        /// Specifies alignment to both left and right. 
        /// </summary>
        Justify
    }
    /// <summary>
    /// Specifies type of the horizontal alignment.
    /// </summary>
    public enum RowAlignment
    {
        /// <summary>
        /// Specifies alignment to the left. 
        /// </summary>
        Left,
        /// <summary>
        /// Specifies alignment to the center. 
        /// </summary>
        Center,
        /// <summary>
        /// Specifies alignment to the right. 
        /// </summary>
        Right
    }
    #endregion

    #region enum UnderlineStyle
    /// <summary>
    /// Specifies style of the underline.
    /// </summary>
    public enum UnderlineStyle
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Single = 1,
        /// <summary>
        /// 
        /// </summary>
        Words = 2,
        /// <summary>
        /// 
        /// </summary>
        Double = 3,
        /// <summary>
        /// 
        /// </summary>
        Dotted = 4,
        /// <summary>
        /// 
        /// </summary>
        [ObsoleteAttribute("This enumeration option has been deprecated. On using this enumeration, None style will be set instead of DotDot.")]
        DotDot = 5,
        /// <summary>
        /// 
        /// </summary>
        Thick = 6,
        /// <summary>
        /// 
        /// </summary>
        Dash = 7,
        /// <summary>
        /// 
        /// </summary>
        DashLong = 39,
        /// <summary>
        /// 
        /// </summary>
        DotDash = 9,
        /// <summary>
        /// 
        /// </summary>
        DotDotDash = 10,
        /// <summary>
        /// 
        /// </summary>
        Wavy = 11,
        /// <summary>
        /// 
        /// </summary>
        DottedHeavy = 20,
        /// <summary>
        /// 
        /// </summary>
        DashHeavy = 23,
        /// <summary>
        /// 
        /// </summary>
        DashLongHeavy = 55,
        /// <summary>
        /// 
        /// </summary>
        DotDashHeavy = 25,
        /// <summary>
        /// 
        /// </summary>
        DotDotDashHeavy = 26,
        /// <summary>
        /// 
        /// </summary>
        WavyHeavy = 27,
        /// <summary>
        /// 
        /// </summary>
        WavyDouble = 43
    }
    #endregion

    #region enum SubSuperScript
    /// <summary>
    /// Specifies type of the SubSuperScript.
    /// </summary>
    public enum SubSuperScript
    {
        /// <summary>
        /// No sub- or superscript.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specified superscript format.
        /// </summary>
        SuperScript = 1,
        /// <summary>
        /// Specified subscript format.
        /// </summary>
        SubScript = 2
    }
    #endregion

    #region enum BorderStyle
    /// <summary>
    /// Specifies style of the border line.
    /// </summary>
    public enum BorderStyle
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Single = 1,
        /// <summary>
        /// 
        /// </summary>
        Thick = 2,
        /// <summary>
        /// 
        /// </summary>
        Double = 3,
        /// <summary>
        /// 
        /// </summary>
        Hairline = 5,
        /// <summary>
        /// 
        /// </summary> 
        Dot = 6,
        /// <summary>
        /// 
        /// </summary>
        DashLargeGap = 7,
        /// <summary>
        /// 
        /// </summary>
        DotDash = 8,
        /// <summary>
        /// 
        /// </summary>
        DotDotDash = 9,
        /// <summary>
        /// 
        /// </summary>
        Triple = 10,
        /// <summary>
        /// 
        /// </summary>
        ThinThickSmallGap = 11,
        /// <summary>
        /// 
        /// </summary>
        ThinThinSmallGap = 12,
        /// <summary>
        /// 
        /// </summary>
        ThinThickThinSmallGap = 13,
        /// <summary>
        /// 
        /// </summary>
        ThinThickMediumGap = 14,
        /// <summary>
        /// 
        /// </summary>
        ThickThinMediumGap = 15,
        /// <summary>
        /// 
        /// </summary>
        ThickThickThinMediumGap = 16,
        /// <summary>
        /// 
        /// </summary>
        ThinThickLargeGap = 17,
        /// <summary>
        /// 
        /// </summary>
        ThickThinLargeGap = 18,
        /// <summary>
        /// 
        /// </summary>
        ThinThickThinLargeGap = 19,
        /// <summary>
        /// 
        /// </summary>
        Wave = 20,
        /// <summary>
        /// 
        /// </summary>
        DoubleWave = 21,
        /// <summary>
        /// 
        /// </summary>
        DashSmallGap = 22,
        /// <summary>
        /// 
        /// </summary>
        DashDotStroker = 23,
        /// <summary>
        /// 
        /// </summary>
        Emboss3D = 24,
        /// <summary>
        /// 
        /// </summary>
        Engrave3D = 25,
        /// <summary>
        /// 
        /// </summary>
        Outset = 26,
        /// <summary>
        /// 
        /// </summary>
        Inset = 27,
        /// <summary>
        /// 
        /// </summary>
        TwistedLines1 = 214,
        /// <summary>
        /// 
        /// </summary>
        Cleared = 255
    }
    #endregion

    #region enum SectionBreakCode
    /// <summary>
    /// Specifies type of the section break code.
    /// </summary>
    public enum SectionBreakCode
    {
        /// <summary>
        /// Specifies no break code.
        /// </summary>
        NoBreak = 0,
        /// <summary>
        /// The section starts from a new column.
        /// </summary>
        NewColumn = 1,
        /// <summary>
        /// The section starts from a new page. 
        /// </summary>
        NewPage = 2,
        /// <summary>
        /// The section starts on a new even page. 
        /// </summary>
        EvenPage = 3,
        /// <summary>
        /// The section starts on a new odd page. 
        /// </summary>
        Oddpage = 4
    }
    #endregion

    #region enum HyperlinkType
    /// <summary>
    /// Specifies type of the link. 
    /// </summary>
    public enum HyperlinkType
    {
        /// <summary>
        /// No links. 
        /// </summary>
        None,
        /// <summary>
        /// Links to another file. 
        /// </summary>
        FileLink,
        /// <summary>
        /// Links to a web page. 
        /// </summary>
        WebLink,
        /// <summary>
        /// Link to e-mail.
        /// </summary>
        EMailLink,
        /// <summary>
        /// Bookmark link.
        /// </summary>
        Bookmark
    }
    #endregion

    #region enum ListType
    /// <summary>
    /// Specifies type of the list format.
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// Specifies numbered list. 
        /// </summary>
        Numbered = 0,
        /// <summary>
        /// Specifies bulleted list.
        /// </summary>
        Bulleted = 1,
        /// <summary>
        /// No numbering.
        /// </summary>
        NoList = 2
    }
    #endregion

    #region enum ListPatternType
    /// <summary>
    /// Specifies type of the list numbering format.
    /// </summary>
    public enum ListPatternType
    {
        /// <summary>
        /// Specifies default numbering format.
        /// </summary>
        Arabic = 0,
        /// <summary>
        /// Specifies UppRoman numbering format.
        /// </summary>
        UpRoman = 1,
        /// <summary>
        /// Specifies LowRoman numbering format.
        /// </summary>
        LowRoman = 2,
        /// <summary>
        /// Specifies UpLetter numbering format.
        /// </summary>
        UpLetter = 3,
        /// <summary>
        /// Specifies LowLetter numbering format.
        /// </summary>
        LowLetter = 4,
        /// <summary>
        /// Specifies Ordinal numbering format.
        /// </summary>
        Ordinal = 5,
        /// <summary>
        /// Specifies Number numbering format.
        /// </summary>
        Number = 6,
        /// <summary>
        /// Specifies OrdinalText numbering format.
        /// </summary>
        OrdinalText = 7,
        /// <summary>
        /// Specifies LeadingZero numbering format.
        /// </summary>
        LeadingZero = 0x16,
        /// <summary>
        /// Specifies Bullet numbering format.
        /// </summary>
        Bullet = 0x17,
        /// <summary>
        /// 
        /// </summary>
        FarEast = 20,
        /// <summary>
        /// Special numbering format.
        /// </summary>
        Special = 0x3a,
        /// <summary>
        /// Specifies None numbering format.
        /// </summary>
        None = 0xff
    }
    #endregion

    #region enum CellMerge
    /// <summary>
    /// Specifies the way of cell merging.
    /// </summary>
    public enum CellMerge
    {
        /// <summary>
        /// No merge.
        /// </summary>
        None,
        /// <summary>
        /// Merge starts from this cell.
        /// </summary>
        Start,
        /// <summary>
        /// Merge is continued.
        /// </summary>
        Continue
    }
    #endregion
    #region enum shape position
    /// <summary>
    /// Specifies shape position 
    /// </summary>
    internal enum ShapePosition
    {
        /// <summary>
        /// 
        /// </summary>
        Static = 0,
        /// <summary>
        /// 
        /// </summary>
        Absolute = 1,
        /// <summary>
        /// 
        /// </summary>
        Relative = 2,
    }
    #endregion
    #region enum HorizontalOrigin
    /// <summary>
    /// Specify object's horizontal origin 
    /// </summary>
    public enum HorizontalOrigin
    {
        /// <summary>
        /// 
        /// </summary>
        Margin = 0,
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        Column = 2,
        /// <summary>
        /// 
        /// </summary>
        Character = 3,
        /// <summary>
        /// 
        /// </summary>
        LeftMargin = 4,
        /// <summary>
        /// 
        /// </summary>
        RightMargin = 5,
        /// <summary>
        /// 
        /// </summary>
        InsideMargin = 6,
        /// <summary>
        /// 
        /// </summary>
        OutsideMargin = 7
    }
    #endregion

    #region enum VerticalOrigin
    /// <summary>
    /// Specify vertical origin of the object
    /// </summary>
    public enum VerticalOrigin
    {
        /// <summary>
        /// 
        /// </summary>
        Margin = 0,
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        Paragraph = 2,
        /// <summary>
        /// 
        /// </summary>
        Line = 3,
        TopMargin=4,
        BottomMargin=5,
        InsideMargin = 6,
        OutsideMargin = 7
    }
    /// <summary>
    /// Specify Vertical Origin of the object height
    /// </summary>
    public enum HeightOrigin
    {
        /// <summary>
        /// 
        /// </summary>
        Margin = 0,
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        TopMargin = 2,
        /// <summary>
        /// 
        /// </summary>
        BottomMargin = 3,
        /// <summary>
        /// 
        /// </summary>
        InsideMargin=4,
        /// <summary>
        /// 
        /// </summary>
        OutsideMargin=5
    }
    /// <summary>
    /// Specify Horizontal origin of the object width
    /// </summary>
    public enum WidthOrigin
    {
        /// <summary>
        /// 
        /// </summary>
        Margin = 0,
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        LeftMargin = 2,
        /// <summary>
        /// 
        /// </summary>
        RightMargin = 3,
        /// <summary>
        /// 
        /// </summary>
        InsideMargin = 4,
        /// <summary>
        /// 
        /// </summary>
        OutsideMargin = 5
    }
    #endregion

    #region enum TextBoxLineStyle
    /// <summary>
    /// Specify object's line style
    /// </summary>
    public enum TextBoxLineStyle
    {
        /// <summary>
        /// Single line (of width lineWidth)
        /// </summary>
        Simple = 0,
        /// <summary>
        /// Double lines of equal width
        /// </summary>
        Double = 1,
        /// <summary>
        /// Double lines, one thick, one thin
        /// </summary>
        ThickThin = 2,
        /// <summary>
        /// Double lines, reverse order
        /// </summary>
        ThinThick = 3,
        /// <summary>
        /// Three lines, thin, thick, thin
        /// </summary>
        Triple = 4
    };

    #endregion

    #region enum TextWrappingStyle
    /// <summary>
    /// Specify text wrapping style for object.
    /// </summary>
    public enum TextWrappingStyle
    {
        /// <summary>
        /// Inline text wrapping style
        /// </summary>
        Inline = 0,
        /// <summary>
        /// TopAndBottom text wrapping style
        /// </summary>
        TopAndBottom = 1,
        /// <summary>
        /// Square text wrapping style
        /// </summary>
        Square = 2,
        /// <summary>
        /// No text wrapping style
        /// </summary>
        InFrontOfText = 3,
        /// <summary>
        /// Tight text wrapping style
        /// </summary>
        Tight = 4,
        /// <summary>
        /// Through text wrapping style
        /// </summary>
        Through = 5,
        /// <summary>
        /// Behind text wrapping style
        /// </summary>
        Behind = 6

    }

    #endregion

    #region enum TextWrappingType
    /// <summary>
    /// Specify text wrapping type for textbox.
    /// </summary>
    public enum TextWrappingType
    {
        /// <summary>
        /// Wrap text both sides
        /// </summary>
        Both = 0,
        /// <summary>
        /// Wrap text left side
        /// </summary>
        Left = 1,
        /// <summary>
        /// Wrap text right side
        /// </summary>
        Right = 2,
        /// <summary>
        /// Wrap text largest
        /// </summary>
        Largest = 3
    }
    #endregion

    #region enum TabJustification
    /// <summary>
    /// Specifies the tab justification.
    /// </summary>
    public enum TabJustification
    {
        /// <summary>
        /// Left tab.
        /// </summary>
        Left = 0,
        /// <summary>
        /// Centered tab.
        /// </summary>
        Centered = 1,
        /// <summary>
        /// Right tab.
        /// </summary>
        Right = 2,
        /// <summary>
        /// Decimal tab.
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// Bar.
        /// </summary>
        Bar = 4,
        /// <summary>
        /// 
        /// </summary>
        List = 6
    }
    #endregion

    #region enum TabLeader
    /// <summary>
    /// Specifies Tab leader.
    /// </summary>
    public enum TabLeader
    {
        /// <summary>
        /// No leader.
        /// </summary>
        NoLeader = 0,
        /// <summary>
        /// Dotted leader.
        /// </summary>
        Dotted = 1,
        /// <summary>
        /// Hyphenated leader.
        /// </summary>
        Hyphenated = 2,
        /// <summary>
        /// Single line leader. 
        /// </summary>
        Single = 3,
        /// <summary>
        /// Heavy line leader.
        /// </summary>
        Heavy = 4
    }
    #endregion

    #region enum TableRowHeightType
    /// <summary>
    /// Specifies the table row height type.
    /// </summary>
    public enum TableRowHeightType
    {
        /// <summary>
        /// "At least" table row height type
        /// </summary>
        AtLeast = 0,
        /// <summary>
        /// " Exactly" table row height type
        /// </summary>
        Exactly = 1
    }

    #endregion

    #region enum ListNumberAlignment
    /// <summary>
    /// Number alignments
    /// </summary>
    public enum ListNumberAlignment
    {
        /// <summary>
        /// Number aligned left
        /// </summary>
        Left = 0,
        /// <summary>
        /// Number is centered 
        /// </summary>
        Center = 1,
        /// <summary>
        /// Number aligned right
        /// </summary>
        Right = 2
    }
    #endregion

    #region enum FollowCharacterType
    /// <summary>
    /// The type of character following the number text for the paragraph
    /// </summary>
    public enum FollowCharacterType
    {
        /// <summary>
        /// List levels number or bullet is followed by tab
        /// </summary>
        Tab = 0,
        /// <summary>
        /// List levels number or bullet is followed by space
        /// </summary>
        Space = 1,
        /// <summary>
        /// Follow character isn't used  
        /// </summary>
        Nothing = 2
    }

    #endregion

    #region enum PictureColor
    /// <summary>
    /// Picture color types.
    /// </summary>
    public enum PictureColor
    {
        /// <summary>
        /// Picture automatic color.
        /// </summary>
        Automatic = 0,
        /// <summary>
        /// Picture grayscale color.
        /// </summary>
        Grayscale = 1,
        /// <summary>
        /// Picture black and white color.
        /// </summary>
        BlackAndWhite = 2,
        /// <summary>
        /// Picture washout color.
        /// </summary>
        Washout = 3
    }

    #endregion

    #region enum Chapter Page Seprator
    /// <summary>
    /// Chapter Page Separator Types
    /// </summary>
    public enum ChapterPageSeparatorType
    {        
        Hyphen = 0,     
        Period = 1,      
        Colon = 2,
        EmDash=3,
        EnDash=4
    }
    #endregion

    #region enum Heading Level for chapter
    /// <summary>
    /// Heading Levels for chapter
    /// </summary>
    public enum HeadingLevel
    {
        None = 0,
        Heading1 = 1,
        Heading2 = 2,
        Heading3 = 3,
        Heading4 = 4,
        Heading5 = 5,
        Heading6 = 6,
        Heading7 = 7,
        Heading8 = 8,
        Heading9 = 9
    }
    #endregion

    #region enum BreakType
    /// <summary>
    /// Document's break type.
    /// </summary>
    public enum BreakType
    {
        /// <summary>
        /// Page break type.
        /// </summary>
        PageBreak = 0,
        /// <summary>
        /// Column break type.
        /// </summary>
        ColumnBreak = 1,
        /// <summary>
        /// Line break type.
        /// </summary>
        LineBreak = 2
    }

    #endregion

    #region enum WatermarkType
    /// <summary>
    /// Specifies the watermark type.
    /// </summary>
    public enum WatermarkType
    {
        /// <summary>
        /// No watermark.
        /// </summary>
        NoWatermark = 0,
        /// <summary>
        /// Picture watermark.
        /// </summary>
        PictureWatermark = 1,
        /// <summary>
        /// Text watermark.
        /// </summary>
        TextWatermark = 2
    }
    #endregion

    #region enum WatermarkLayout
    /// <summary>
    /// Specifies WatermarkLayout.
    /// </summary>
    public enum WatermarkLayout
    {
        /// <summary>
        /// Diagonal watermark layout.
        /// </summary>
        Diagonal = 0,
        /// <summary>
        /// Horizontal watermark layout.
        /// </summary>
        Horizontal = 1
    }
    #endregion

    #region enum BackgroundType
    /// <summary>
    /// Specifies BackgroundType
    /// </summary>
    public enum BackgroundType
    {
        /// <summary>
        /// No background fill effect.
        /// </summary>
        NoBackground = 0,
        /// <summary>
        /// Gradient fill effect.
        /// </summary>
        Gradient = 1,
        /// <summary>
        /// Picture fill effect.
        /// </summary>
        Picture = 2,
        /// <summary>
        /// Texture fill effect.
        /// </summary>
        Texture = 3,
        /// <summary>
        /// Color fill effect.
        /// </summary>
        Color = 4
    }
    #endregion

    #region enum GradientShadingStyle
    /// <summary>
    ///Shading styles for Gradient background effect. 
    /// </summary>
    public enum GradientShadingStyle
    {
        /// <summary>
        /// Horizontal shading style.
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Vertical shading style.
        /// </summary>
        Vertical = 1,
        /// <summary>
        /// Diagonal Up shading style.
        /// </summary>
        DiagonalUp = 2,
        /// <summary>
        /// Diagonal Down shading style.
        /// </summary>
        DiagonalDown = 3,
        /// <summary>
        /// FromCorner shading style.
        /// </summary>
        FromCorner = 4,
        /// <summary>
        /// From Center shading style.
        /// </summary>
        FromCenter = 5
    }
    #endregion

    #region enum GradientShadingVariant
    /// <summary>
    /// Shading variants for background gradient.
    /// </summary>
    public enum GradientShadingVariant
    {
        /// <summary>
        /// Shading in the upper part.
        /// </summary>
        ShadingUp = 0,
        /// <summary>
        /// Shading in the lower part.
        /// </summary>
        ShadingDown = 1,
        /// <summary>
        /// Shading in upper and lower parts.
        /// </summary>
        ShadingOut = 2,
        /// <summary>
        /// Shading in the middle.
        /// </summary>
        ShadingMiddle = 3
    }
    #endregion

    #region enum FormFieldType
    /// <summary>
    /// Specifies the type of a form field.
    /// </summary>
    public enum FormFieldType
    {
        /// <summary>
        /// Text form field.
        /// </summary>
        TextInput = 0,
        /// <summary>
        /// Check box form field.
        /// </summary>
        CheckBox = 1,
        /// <summary>
        /// Drop-down form field.
        /// </summary>
        DropDown = 2
    }
    #endregion

    #region enum CheckBoxSizeType
    /// <summary>
    /// Defines checkBox size type.
    /// </summary>
    public enum CheckBoxSizeType
    {
        /// <summary>
        /// Auto check box size.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Exact check box size.
        /// </summary>
        Exactly = 1
    }

    #endregion

    #region enum TextFormat
    /// <summary>
    /// Defines TextFormat
    /// </summary>
    public enum TextFormat
    {
        /// <summary>
        /// No text formatting
        /// </summary>
        None = 0,
        /// <summary>
        /// Uppercase text formatting.
        /// </summary>
        Uppercase = 1,
        /// <summary>
        /// Lowercase text formatting.
        /// </summary>
        Lowercase = 2,
        /// <summary>
        /// First capital text formatting.
        /// </summary>
        FirstCapital = 3,
        /// <summary>
        /// Title case text formatting.
        /// </summary>
        Titlecase = 4
    }
    #endregion

    #region enum NumberFormat
    /// <summary>
    /// Defines Number format.
    /// </summary>
    public enum NumberFormat
    {
        /// <summary>
        /// No formatting 
        /// </summary>
        None = 0,
        /// <summary>
        /// Format with while number.
        /// </summary>
        WholeNumber = 1,
        /// <summary>
        /// Format with floating point number.
        /// </summary>
        FloatingPoint = 2,
        /// <summary>
        /// Whole number in percents.
        /// </summary>
        WholeNumberPercent = 3,
        /// <summary>
        /// Floating point number in percents. 
        /// </summary>
        FloatingPointPercent = 4,
        /// <summary>
        /// Format which suits to "#�##0" Word format.
        /// </summary>
        WholeNumberWithSpace = 5,
        /// <summary>
        /// Format which suites to "#�##0,00" Word format.
        /// </summary>
        FloatingPointWithSpace = 6,
        /// <summary>
        /// Format which suites to "#�##0,00 $;(#�##0,00 $)" Word format.
        /// </summary>
        CurrencyFormat = 7
    }
    #endregion

    #region enum TextDirection
    /// <summary>
    /// Defines the direction of text.
    /// </summary>
    public enum TextDirection
    {
        /// <summary>
        /// Horizontal text direction.
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Vertical text direction, text direction from bottom to top .
        /// </summary>
        VerticalBottomToTop = 2,
        /// <summary>
        /// Vertical text direction, text direction from to to bottom.
        /// </summary>
        VerticalTopToBottom = 3
    }
    /// <summary>
    /// Specifies the Text Direction.
    /// </summary>
    public enum DocTextDirection
    {
        /// <summary>
        /// Specifies text direction in left to right order.
        /// </summary>
        LeftToRight = 0, //lr-tb
        /// <summary>
        /// Specifies text direction in Top to bottom order.
        /// </summary>
        TopToBottom = 1, //tb-rl
        /// <summary>
        /// Specifies text direction in Top to bottom order and rotated.
        /// </summary>
        TopToBottomRotated = 2, //Binary value (Doc format) = 2, No info on TextFlow, handled as like value 4
        /// <summary>
        /// Specifies text direction in left to right order and rotated.
        /// </summary>
        LeftToRightRotated = 3, //bt-lr
        /// <summary>
        /// Specifies text direction in right to left order.
        /// </summary>
        RightToLeft = 4, //lr-tb-v
        /// <summary>
        /// Specifies text direction in right to left order and rotated.
        /// </summary>
        RightToLeftRotated = 5 //tb-rl-v
    }
    #endregion

    #region enum FildMarkType
    /// <summary>
    /// Defines types of field marks.
    /// </summary>
    public enum FieldMarkType
    {
        /// <summary>
        /// Field separator type
        /// </summary>
        FieldSeparator = 0,
        /// <summary>
        /// Field end type.
        /// </summary>
        FieldEnd
    }
    #endregion

    #region enum BuiltinStyles
    /// <summary>
    /// Defines built-in styles.
    /// </summary>
    public enum BuiltinStyle
    {
        Normal,
        Heading1,
        Heading2,
        Heading3,
        Heading4,
        Heading5,
        Heading6,
        Heading7,
        Heading8,
        Heading9,
        Index1,
        Index2,
        Index3,
        Index4,
        Index5,
        Index6,
        Index7,
        Index8,
        Index9,
        Toc1,
        Toc2,
        Toc3,
        Toc4,
        Toc5,
        Toc6,
        Toc7,
        Toc8,
        Toc9,
        NormalIndent,
        FootnoteText,
        CommentText,
        Header,
        Footer,
        IndexHeading,
        Caption,
        TableOfFigures,
        FootnoteReference,
        CommentReference,
        LineNumber,
        PageNumber,
        EndnoteReference,
        EndnoteText,
        TableOfAuthorities,
        MacroText,
        ToaHeading,
        List,
        ListBullet,
        ListNumber,
        List2,
        List3,
        List4,
        List5,
        ListBullet2,
        ListBullet3,
        ListBullet4,
        ListBullet5,
        ListNumber2,
        ListNumber3,
        ListNumber4,
        ListNumber5,
        Title,
        Closing,
        Signature,
        DefaultParagraphFont,
        BodyText,
        BodyTextInd,
        ListContinue,
        ListContinue2,
        ListContinue3,
        ListContinue4,
        ListContinue5,
        MessageHeader,
        Subtitle,
        Salutation,
        Date,
        BodyText1I,
        BodyText1I2,
        NoteHeading,
        BodyText2,
        BodyText3,
        BodyTextInd2,
        BodyTextInd3,
        BlockText,
        Hyperlink,
        FollowedHyperlink,
        Strong,
        Emphasis,
        DocumentMap,
        PlainText,
        EmailSignature,
        NormalWeb,
        HtmlAcronym,
        HtmlAddress,
        HtmlCite,
        HtmlCode,
        HtmlDefinition,
        HtmlKeyboard,
        HtmlPreformatted,
        HtmlSample,
        HtmlTypewriter,
        HtmlVariable,
        CommentSubject,
        NoList,
        BalloonText,
        User,
        NoStyle
    }
    /// <summary>
    /// Defines built-in list styles
    /// </summary>
    internal enum BuiltinListStyle
    {
        ListBullet = 0,
        ListNumber = 1,
        ListBullet2 = 2,
        ListBullet3 = 3,
        ListBullet4 = 4,
        ListBullet5 = 5,
        ListNumber2 = 6,
        ListNumber3 = 7,
        ListNumber4 = 8,
        ListNumber5 = 9
    }
    /// <summary>
    /// Defines built-in table styles
    /// </summary>
    public enum BuiltinTableStyle
    {
        TableNormal,
        TableGrid,
        LightShading,
        LightShadingAccent1,
        LightShadingAccent2,
        LightShadingAccent3,
        LightShadingAccent4,
        LightShadingAccent5,
        LightShadingAccent6,
        LightList,
        LightListAccent1,
        LightListAccent2,
        LightListAccent3,
        LightListAccent4,
        LightListAccent5,
        LightListAccent6,
        LightGrid,
        LightGridAccent1,
        LightGridAccent2,
        LightGridAccent3,
        LightGridAccent4,
        LightGridAccent5,
        LightGridAccent6,
        MediumShading1,
        MediumShading1Accent1,
        MediumShading1Accent2,
        MediumShading1Accent3,
        MediumShading1Accent4,
        MediumShading1Accent5,
        MediumShading1Accent6,
        MediumShading2,
        MediumShading2Accent1,
        MediumShading2Accent2,
        MediumShading2Accent3,
        MediumShading2Accent4,
        MediumShading2Accent5,
        MediumShading2Accent6,
        MediumList1,
        MediumList1Accent1,
        MediumList1Accent2,
        MediumList1Accent3,
        MediumList1Accent4,
        MediumList1Accent5,
        MediumList1Accent6,
        MediumList2,
        MediumList2Accent1,
        MediumList2Accent2,
        MediumList2Accent3,
        MediumList2Accent4,
        MediumList2Accent5,
        MediumList2Accent6,
        MediumGrid1,
        MediumGrid1Accent1,
        MediumGrid1Accent2,
        MediumGrid1Accent3,
        MediumGrid1Accent4,
        MediumGrid1Accent5,
        MediumGrid1Accent6,
        MediumGrid2,
        MediumGrid2Accent1,
        MediumGrid2Accent2,
        MediumGrid2Accent3,
        MediumGrid2Accent4,
        MediumGrid2Accent5,
        MediumGrid2Accent6,
        MediumGrid3,
        MediumGrid3Accent1,
        MediumGrid3Accent2,
        MediumGrid3Accent3,
        MediumGrid3Accent4,
        MediumGrid3Accent5,
        MediumGrid3Accent6,
        DarkList,
        DarkListAccent1,
        DarkListAccent2,
        DarkListAccent3,
        DarkListAccent4,
        DarkListAccent5,
        DarkListAccent6,
        ColorfulShading,
        ColorfulShadingAccent1,
        ColorfulShadingAccent2,
        ColorfulShadingAccent3,
        ColorfulShadingAccent4,
        ColorfulShadingAccent5,
        ColorfulShadingAccent6,
        ColorfulList,
        ColorfulListAccent1,
        ColorfulListAccent2,
        ColorfulListAccent3,
        ColorfulListAccent4,
        ColorfulListAccent5,
        ColorfulListAccent6,
        ColorfulGrid,
        ColorfulGridAccent1,
        ColorfulGridAccent2,
        ColorfulGridAccent3,
        ColorfulGridAccent4,
        ColorfulGridAccent5,
        ColorfulGridAccent6,
        Table3Deffects1,
        Table3Deffects2,
        Table3Deffects3,
        TableClassic1,
        TableClassic2,
        TableClassic3,
        TableClassic4,
        TableColorful1,
        TableColorful2,
        TableColorful3,
        TableColumns1,
        TableColumns2,
        TableColumns3,
        TableColumns4,
        TableColumns5,
        TableContemporary,
        TableElegant,
        TableGrid1,
        TableGrid2,
        TableGrid3,
        TableGrid4,
        TableGrid5,
        TableGrid6,
        TableGrid7,
        TableGrid8,
        TableList1,
        TableList2,
        TableList3,
        TableList4,
        TableList5,
        TableList6,
        TableList7,
        TableList8,
        TableProfessional,
        TableSimple1,
        TableSimple2,
        TableSimple3,
        TableSubtle1,
        TableSubtle2,
        TableTheme,
        TableWeb1,
        TableWeb2,
        TableWeb3
    }
    /// <summary>
    /// Defines conditional formatting styles type.
    /// </summary>
    internal enum ConditionalFormattingCode
    {
        FirstRow = 0,
        LastRow = 1,
        OddRowBanding = 2,
        EvenRowBanding = 3,
        FirstColumn = 4,
        LastColumn = 5,
        OddColumnBanding = 6,
        EvenColumnBanding = 7,
        FirstRowLastCell = 8,
        FirstRowFirstCell = 9,
        LastRowLastCell = 10,
        LastRowFirstCell = 11
    }
    #endregion

    #region enum BorderSide
    /// <summary>
    /// 
    /// </summary>
    internal enum BorderSide
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3,
        Between = 4,
        Bar = 5
    }
    #endregion

    #region enum NumberStyle
    /// <summary>
    /// Specifies the Number Style for a page.
    /// </summary>
    public enum PageNumberStyle
    {
        Arabic = 0,
        RomanUpper = 1,
        RomanLower = 2,
        LetterUpper = 3,
        LetterLower = 4
    }
    #endregion

    #region enum PageNumberAlignment
    /// <summary>
    /// Specifies PageNumber alignment.
    /// </summary>
    public enum PageNumberAlignment
    {
        Left = 0,
        Center = -4,
        Right = -8,
        Inside = -12,
        Outside = -16
    }
    #endregion

    #region enum FrameAnchor
    /// <summary>
    /// Specifies the horizontal frame anchor.
    /// </summary>
    internal enum FrameHorzAnchor
    {
        Text = 0,
        Margin = 1,
        Page = 2,
        None = 3
    }
    /// <summary>
    /// Specifies the vertical frame anchor.
    /// </summary>
    internal enum FrameVertAnchor
    {
        Margin = 0,
        Page = 1,
        Text = 2,
        None = 3
    }
    /// <summary>
    /// Specifies the vertical frame position.
    /// </summary>
    internal enum FrameVerticalPosition
    {
        Inline = 0,
        Top = -4,
        Center = -8,
        Bottom = -12,
        Inside = -16,
        Outside = -20
    }
    #endregion

    #region enum XHTMLValidationType
    /// <summary>
    /// Specifies the XHTMLValidation type.
    /// </summary>
    public enum XHTMLValidationType
    {
        /// <summary>
        /// XHTML 1.0 validation.
        /// </summary>
        Strict,
        /// <summary>
        /// XHTML 1.1 validation.
        /// </summary>
        Transitional,
        /// <summary>
        /// No validation.
        /// </summary>
        None
    }
    #endregion

    #region enum GridPitchType
    /// <summary>
    /// Defines how tall a grid unit is up/down
    /// </summary>
    public enum GridPitchType
    {
        NoGrid = 0,
        CharsAndLine = 1,
        LinesOnly = 2,
        SnapToChars = 3
    }
    #endregion

    #region enum DocumentVersion
    /// <summary>
    /// Defines document version
    /// </summary>
    public enum DocumentVersion
    {
        /// <summary>
        /// Document created using Word 97
        /// </summary>
        Word97 = 0,
        /// <summary>
        /// Document created using Word 2000
        /// </summary>
        Word2000 = 1,
        /// <summary>
        /// Document created using Word 2002
        /// </summary>
        Word2002 = 2,
        /// <summary>
        /// Document created using Word 2003
        /// </summary>
        Word2003 = 3,
        /// <summary>
        /// Document created using Word 2007
        /// </summary>
        Word2007 = 4
    }
    #endregion

    #region enum CommentMarkType
    /// <summary>
    /// Defines types of comment mark.
    /// </summary>
    public enum CommentMarkType
    {
        /// <summary>
        /// Comment start mark type.
        /// </summary>
        CommentStart = 0,
        /// <summary>
        /// Comment end mark type
        /// </summary>
        CommentEnd = 1
    }
    #endregion

    #region enum OleObjectTyle
    /// <summary>
    /// Defines types of the ole object field
    /// </summary>
    public enum OleLinkType
    {
        /// <summary>
        /// Ole object field type is EMBED
        /// </summary>
        Embed,
        /// <summary>
        /// Ole object field type is LINK
        /// </summary>
        Link
    }
    /// <summary>
    /// defines the types of OLE object
    /// </summary>
    public enum OleObjectType
    {
        /// <summary>
        /// Type is not defined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Adobe Acrobat Document. File has ".pdf" extension.
        /// </summary>
        AdobeAcrobatDocument = 1,
        /// <summary>
        /// Bitmap Image. File has ".png" extension.
        /// </summary>
        BitmapImage = 2,
        /// <summary>
        /// Media Clip
        /// </summary>
        MediaClip = 3,
        /// <summary>
        /// Equation
        /// </summary>
        Equation = 4,
        /// <summary>
        /// Graph Chart
        /// </summary>
        GraphChart = 5,
        /// <summary>
        /// Excel 97-2003 Worksheet. File has ".xls" extension
        /// </summary>
        Excel_97_2003_Worksheet = 6,
        /// <summary>
        /// Excel Binary Worksheet. File has ".xlsb" extension
        /// </summary>
        ExcelBinaryWorksheet = 7,
        /// <summary>
        /// Excel chart. File has ".xls" extension
        /// </summary>
        ExcelChart = 8,
        /// <summary>   
        /// Excel Macro-Enabled Worksheet. File has ".xlsm" extension.
        /// </summary>
        ExcelMacroWorksheet = 9,
        /// <summary>
        /// Excel Worksheet. File has ".xlsx" extension.
        /// </summary>
        ExcelWorksheet = 10,
        /// <summary>
        /// PowerPoint 97-2003 Presentation. File has ".ppt" extension.
        /// </summary>
        PowerPoint_97_2003_Presentation = 11,
        /// <summary>
        /// PowerPoint 97-2003 Slide. File has ".sld" extension.
        /// </summary>
        PowerPoint_97_2003_Slide = 12,
        /// <summary>
        /// PowerPoint Macro-Enabled Presentation. File has ".pptm" extension.
        /// </summary>
        PowerPointMacroPresentation = 13,
        /// <summary>
        /// PowerPoint Macro-Enabled Slide. File has ".sldm" extension.
        /// </summary>
        PowerPointMacroSlide = 14,
        /// <summary>
        /// PowerPoint Presentation. File has ".pptx" extension.
        /// </summary>
        PowerPointPresentation = 15,
        /// <summary>
        /// PowerPoint Slide. File has ".sldx" extension.
        /// </summary>
        PowerPointSlide = 16,
        /// <summary>
        /// Word 97-2003 Document. File has ".doc" extension.
        /// </summary>
        Word_97_2003_Document = 17,
        /// <summary>
        /// Word Document. File has ".docx" extension.
        /// </summary>
        WordDocument = 18,
        /// <summary>
        /// Word Macro-Enabled Document. File has ".docm" extension.
        /// </summary>
        WordMacroDocument = 19,
        /// <summary>
        /// Visio Deawing
        /// </summary>
        VisioDrawing = 20,
        /// <summary>
        /// MIDI Sequence
        /// </summary>
        MIDISequence = 21,
        /// <summary>
        /// OpenDocument Presentation
        /// </summary>
        OpenDocumentPresentation = 22,
        /// <summary>
        /// OpenDocument Spreadsheet
        /// </summary>
        OpenDocumentSpreadsheet = 23,
        /// <summary>
        /// OpenDocument Text
        /// </summary>
        OpenDocumentText = 24,
        /// <summary>
        /// OpenOffice.org 1.1 Spreadsheet
        /// </summary>
        OpenOfficeSpreadsheet1_1 = 25,
        /// <summary>
        /// OpenOffice.org 1.1 Text
        /// </summary>
        OpenOfficeText_1_1 = 26,
        /// <summary>
        /// Package
        /// </summary>
        Package = 27,
        /// <summary>
        /// Video Clip
        /// </summary>
        VideoClip = 28,
        /// <summary>
        /// Wave Sound
        /// </summary>
        WaveSound = 29,
        /// <summary>
        /// WordPad Document
        /// </summary>
        WordPadDocument = 30,
        /// <summary>
        /// OpenOffice spreadsheet
        /// </summary>
        OpenOfficeSpreadsheet = 31,
        /// <summary>
        /// OpenOffice Text
        /// </summary>
        OpenOfficeText = 32
    }
    #endregion

    #region Enum ImageType
    /// <summary>
    /// Specifies the image type.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// 
        /// </summary>
        Metafile,
        /// <summary>
        /// 
        /// </summary>
        Bitmap
    }
    #endregion

    #region Enum RtfTableTyle
    public enum RtfTableType
    {
        /// <summary>
        /// Specifies font table collection
        /// </summary>
        FontTable,
        /// <summary>
        /// Specifies list table collection
        /// </summary>
        ListTable,
        /// <summary>
        /// Specifies color table collection
        /// </summary>
        ColorTable,
        /// <summary>
        /// Specifies list overrid collection
        /// </summary>
        ListOverrideTable,
        /// <summary>
        /// Specifies style sheet collection
        /// </summary>
        StyleSheet,
        /// <summary>
        /// Specifies no table collection
        /// </summary>
        None
    }
   
    #endregion

    #region Enum RtfTokenType
    public enum RtfTokenType
    {
        GroupStart,
        GroupEnd,
        ControlWord,
        Text,
        TableEntry,
        Unknown

    }
    #endregion

    #region Enum FieldCharType
    public enum FieldCharType
    {
        Begin,
        Seperate,
        End,
        Unknown,
        SimpleField
    }
    #endregion

    #region Enum FontHintType
    internal enum FontHintType
    {
        //0x00 - default
        Default,
        //0x01 - eastAsia
        EastAsia,
        //0x02 - cs
        CS
    }
    #endregion
    #region Word-2010 Specific enumeration types
    /// <summary>
    /// Specifies the ligature type.
    /// </summary>
    public enum LigatureType
    {
        None,
        Standard,
        Contextual,
        StandardContextual,
        Historical,
        StandardHistorical,
        ContextualHistorical,
        StandardContextualHistorical,
        Discretional,
        StandardDiscretional,
        ContextualDiscretional,
        StandardContextualDiscretional,
        HistoricalDiscretional,
        StandardHistoricalDiscretional,
        ContextualHistoricalDiscretional,
        All
    }
    /// <summary>
    /// Specifies the number form type.
    /// </summary>
    public enum NumberFormType
    {
        Default,
        Lining,
        OldStyle
    }
    /// <summary>
    /// Specifies the number spacing type.
    /// </summary>
    public enum NumberSpacingType
    {
        Default,
        Proportional,
        Tabular
    }
    /// <summary>
    /// Specifies the stylistic set type.
    /// </summary>
    public enum StylisticSetType
    {
        StylisticSetDefault,
        StylisticSet01,
        StylisticSet02,
        StylisticSet03,
        StylisticSet04,
        StylisticSet05,
        StylisticSet06,
        StylisticSet07,
        StylisticSet08,
        StylisticSet09,
        StylisticSet10,
        StylisticSet11,
        StylisticSet12,
        StylisticSet13,
        StylisticSet14,
        StylisticSet15,
        StylisticSet16,
        StylisticSet17,
        StylisticSet18,
        StylisticSet19,
        StylisticSet20
    }
    #endregion

    #region Enum Compatibility options
    /// <summary>
    /// Specifies the compatibility types.
    /// </summary>
    internal enum CompatibilityOption
    {
        /// <summary>
        ///   Don't add automatic tab stop for hanging indent.
        /// </summary>
        NoTabForInd=1,
        /// <summary>
        /// Don't add extra space for raised/lowered characters.
        /// </summary>
        NoSpaceRaiseLower=2,
        /// <summary>
        ///  Print colors as black on noncolor printers.
        /// </summary>
        MapPrintTextColor=3,
        /// <summary>
        ///  Wrap trailing spaces to next line.
        /// </summary>
        WrapTrailSpaces = 4,
        /// <summary>
        /// Don't balance columns for continuous section starts.
        /// </summary>
        NoColumnBalance = 5,
        /// <summary>
        ///  Treat \" as "" in mail merge data sources.
        /// </summary>
        ConvMailMergeEsc=6,
        /// <summary>
        ///  Suppress Space Before after a hard page or column break.
        /// </summary>
        SuppressSpBfAfterPgBrk=7,
        /// <summary>
        ///  Suppress extra line spacing at top of page.
        /// </summary>
        SuppressTopSpacing=8,
        /// <summary>
        ///  Combine table borders like Word 5.x for the Macintosh.
        /// </summary>
        OrigWordTableRules=9,
        /// <summary>
        ///  Truncate font height.
        /// </summary>
        TruncDxaExpand=10,
        /// <summary>
        ///  Show hard page or column breaks in frames.
        /// </summary>
        ShowBreaksInFrames=11,
        /// <summary>
        ///  Swap left and right borders on odd facing pages.
        /// </summary>
        SwapBordersFacingPgs=12,
        /// <summary>
        /// Convert backslash characters into yen signs.
        /// </summary>
        LeaveBackslashAlone=13,
        /// <summary>
        ///  Don't expand character spaces on the line ending Shift+Return.
        /// </summary>
        ExpShRtn=14,
        /// <summary>
        ///   Draw underline on trailing spaces.
        /// </summary>
        DntULTrlSpc=15,
        /// <summary>
        ///  Balance SBCS characters and DBCS characters.
        /// </summary>
        DntBlnSbDbWid=16,
        /// <summary>
        /// Suppress extra line spacing at top of page like Word 5.x for the Macintosh.
        /// </summary>
        SuppressTopSpacingMac5=17,
        /// <summary>
        /// Specifies whether line spacing emulates WordPerfect 5.x line spacing
        /// </summary>
        F2ptExtLeadingOnly=18,
        /// <summary>
        /// Print body text before header/footer.
        /// </summary>
        PrintBodyBeforeHdr=19,
        /// <summary>
        /// Don't add leading (extra space) between rows of text.
        /// </summary>
        NoExtLeading=20,
        /// <summary>
        /// Add space for underline.
        /// </summary>
        DontMakeSpaceForUL=21,
        /// <summary>
        /// Use larger small caps like Word 5.x for the Macintosh.
        /// </summary>
        MWSmallCaps=22,
        /// <summary>
        /// Suppress extra line spacing like WordPerfect 5.x.
        /// </summary>
        ExtraAfter=23,
        /// <summary>
        /// Truncate font height.
        /// </summary>
        TruncFontHeight=24,
        /// <summary>
        /// Substitute fonts based on font size.
        /// </summary>
        SubOnSize=25,
        /// <summary>
        ///  Use printer metrics to lay out document.
        /// </summary>
        PrintMet=26,
        /// <summary>
        /// Use Word 6.x/95 border rules.
        /// </summary>
        WW6BorderRules=27,
        /// <summary>
        /// Don't center "exact line height" lines.
        /// </summary>
        ExactOnTop=28,
        /// <summary>
        /// Set the width of a space like WordPerfect 5.x.
        /// </summary>
        WPSpace=30,
        /// <summary>
        /// Do full justification like WordPerfect 6.x for Windows.
        /// </summary>
        WPJust=31,
        /// <summary>
        /// Line wrap like Word 6.0.
        /// </summary>
        LineWrapLikeWord6=32,
        /// <summary>
        /// Lay out autoshapes like Word 97.
        /// </summary>
        SpLayoutLikeWW8=33,
        /// <summary>
        /// Lay out footnotes like Word 6.x/95/97.
        /// </summary>
        FtnLayoutLikeWW8=34,
        /// <summary>
        ///  Don't use HTML paragraph auto spacing.
        /// </summary>
        DontUseHTMLParagraphAutoSpacing=35,
        /// <summary>
        ///   Adjust line height to grid height in the table.
        /// </summary>
        DontAdjustLineHeightInTable=36,
        /// <summary>
        /// Forget last tab alignment.
        /// </summary>
        ForgetLastTabAlign=37,
        /// <summary>
        /// Specifies whether to emulate Word for Windows 95 full-width character spacing
        /// </summary>
        UseAutospaceForFullWidthAlpha=38,
        /// <summary>
        /// Slign table rows independently.
        /// </summary>
        AlignTablesRowByRow=39,
        /// <summary>
        /// Lay out tables with raw width.
        /// </summary>
        LayoutRawTableWidth=40,
        /// <summary>
        /// Allow table rows to lay out apart.
        /// </summary>
        LayoutTableRowsApart=41,
        /// <summary>
        /// Use Word 97 line breaking rules for Asian text.
        /// </summary>
        UseWord97LineBreakingRules=42,
        /// <summary>
        ///  Don't break wrapped tables across pages.
        /// </summary>
        DontBreakWrappedTables=43,
        /// <summary>
        /// Don't snap text to grid inside table with inline objects.
        /// </summary>
        DontSnapToGridInCell=44,
        /// <summary>
        /// Select entire field with first or last character.
        /// </summary>
        DontAllowFieldEndSelect=45,
        /// <summary>
        /// Use line-breaking rules.
        /// </summary>
        ApplyBreakingRules=46,
        /// <summary>
        ///  Don't allow hanging punctuation with character grid.
        /// </summary>
        DontWrapTextWithPunct = 47,
        /// <summary>
        ///  Don't use Asian rules for line breaks with character grid.
        /// </summary>
        DontUseAsianBreakRules=48,
        /// <summary>
        /// Use Word 2002 table style rules.
        /// </summary>
        UseWord2002TableStyleRules=49,
        /// <summary>
        /// Allow tables to extend into margins.
        /// </summary>
        GrowAutoFit=50,
        /// <summary>
        ///  Use the Normal style instead of the List Paragraph style for bulleted or
        //   numbered lists.
        /// </summary>
        UseNormalStyleForList=51,
        /// <summary>
        ///  Do not use hanging indent as tab stop for bullets and numbering.
        /// </summary>
        DontUseIndentAsNumberingTabStop=52,
        /// <summary>
        /// Use Word 2003 hanging-punctuation rules in Asian langauges.
        /// </summary>
        FELineBreak11=53,
        /// <summary>
        /// Allow space between paragraphs of the same style in a table.
        /// </summary>
        AllowSpaceOfSameStyleInTable=54,
        /// <summary>
        /// Use Word 2003 indent rules for text next to wrapped objects.
        /// </summary>
        WW11IndentRules=55,
        /// <summary>
        /// Do not autofit tables next to wrapped objects.
        /// </summary>
        DontAutofitConstrainedTables=56,
        /// <summary>
        /// Use Microsoft Office Word 2003 table autofit rules.
        /// </summary>
        AutofitLikeWW11=57,
        /// <summary>
        /// Underline the tab character between the number and the text in numbered lists.
        /// </summary>
        UnderlineTabInNumList=58,
        /// <summary>
        /// Do not use proportional width for Korean characters.
        /// </summary>
        HangulWidthLikeWW11=59,
        /// <summary>
        /// Split apart page break and paragraph mark.
        /// </summary>
        SplitPgBreakAndParaMark=60,
        /// <summary>
        /// Specifies whether to not vertically align cells containing floating objects
        /// </summary>
        DontVertAlignCellWithSp=61,
        /// <summary>
        /// Specifies whether to not break table rows around floating tables
        /// </summary>
        DontBreakConstrainedForcedTables=62,
        /// <summary>
        /// Specifies whether to ignore vertical alignment in text boxes
        /// </summary>
        DontVertAlignInTxbx=63,
        /// <summary>
        /// Specifies whether to use ANSI kerning pairs from fonts instead of the Unicode kerning pair info
        /// </summary>
        Word11KerningPairs=64,
        /// <summary>
        /// Specifies whether to use cached paragraph information for column balancing
        /// </summary>
        CachedColBalance=65,
        /// <summary>
        /// specifies whether to apply the additional preceding rules when determining the font size and justification of text within tables.
        /// </summary>
        overrideTableStyleFontSizeAndJustification=66,
        /// <summary>
        /// specifies whether the preceding features are to be used when displaying the font.
        /// </summary>
        enableOpenTypeFeatures=67,
        /// <summary>
        /// specifies whether to swap indentation values when displaying paragraphs.
        /// </summary>
        doNotFlipMirrorIndents=68,
        /// <summary>
        /// conditional formatting of table row headers does apply separately to multi-row table headers 
        /// </summary>
        differentiateMultirowTableHeader=69
    }
    internal enum CompatibilityMode
    {
        Word2003,
        Word2007,
        Word2010,
        Word2013
    }
    #endregion

    #region Enum Import options
    /// <summary>
    /// Specifies the Import options.
    /// </summary>
    public enum ImportOptions
    {
        KeepSourceFormatting = 0,
        MergeFormatting = 1,
        KeepTextOnly = 2,
        UseDestinationStyles = 3
    }
    #endregion

    internal enum GradientShadeType
    {
        Circle,
        Rectangle,
        Shape
    }
    #region enum AbsoluteTab Relation
    /// <summary>
    /// Document's AbsoluteTab Relation.
    /// </summary>
    internal enum AbsoluteTabRelation
    {
        /// <summary>
        /// Relative to margin.
        /// </summary>
        Margin = 0,
        /// <summary>
        /// Relative to indent.
        /// </summary>
        Indent = 1,
    }

    #endregion

    #region enum AbsoluteTab Alignment
    /// <summary>
    /// Document's AbsoluteTab Relation.
    /// </summary>
    internal enum AbsoluteTabAlignment
    {
        /// <summary>
        /// left alignment.
        /// </summary>
        Left = 0,
        /// <summary>
        /// center alignment.
        /// </summary>
        Center = 1,
        /// <summary>
        /// right alignment.
        /// </summary>
        Right = 2,
    }

    #endregion
}
