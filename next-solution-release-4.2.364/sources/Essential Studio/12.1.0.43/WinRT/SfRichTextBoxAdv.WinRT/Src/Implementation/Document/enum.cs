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
    /// <summary>
    /// Specifies single or double strike through
    /// </summary>
    public enum StrikeThrough
    {
        /// <summary>
        /// No strike will be drawn
        /// </summary>
        None,
        /// <summary>
        /// Draws single strike
        /// </summary>
        SingleStrike,
        /// <summary>
        /// Draws double strike
        /// </summary>
        DoubleStrike
    }
    /// <summary>
    /// 
    /// </summary>
    public enum BaselineAlignment
    {
        /// <summary>
        /// Specifies whether the text to be rendered normally
        /// </summary>
        Normal,

        /// <summary>
        /// Specifies whether text should appear above the baseline of text
        /// </summary>
        Superscript,

        /// <summary>
        /// Specifies whether text should appear below the baseline of text
        /// </summary>
        Subscript
    }
    /// <summary>
    /// Specifies style of the underline.
    /// </summary>
    public enum Underline
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

    public enum HighlightColor
    {
        //No highlight color.
        NoColor,
        //ffffff00
        Yellow,
        //ff00ff00
        BrightGreen,
        //ff00ffff
        Turquoise,
        //ffff00ff
        Pink,
        //ff0000ff
        Blue,
        //ffff0000
        Red,
        //ff000080
        DarkBlue,
        //ff008080
        Teal,
        //ff008000
        Green,
        //ff800080
        Violet,
        //ff800000
        DarkRed,
        //ff808000
        DarkYellow,
        //ff808080
        Gray50,
        //ffc0c0c0
        Gray25,
        //ff000000
        Black
    }
    /// <summary>
    /// Paragraph line spacing type
    /// </summary>
    public enum LineSpacingType
    {
        /// <summary>
        /// The line spacing can be greater than or equal to, but never less than,
        /// the value specified in the LineSpacing property. 
        /// </summary>
        AtLeast = 0,

        /// <summary>
        /// The line spacing never changes from the value specified in the LineSpacing property, 
        /// even if a larger font is used within the paragraph. 
        /// </summary>
        Exactly = 1,

        /// <summary>
        /// The line spacing is specified in the LineSpacing property as the number of lines. 
        /// One line equals 12 points. 
        /// </summary>
        Multiple = 2
    }
    /// <summary>
    /// Specifies a type of list.
    /// ToDo: Handle all other types supported by MS Word while exposing as public. MSDN reference: http://msdn.microsoft.com/en-us/library/ff840653.aspx
    /// </summary>
    internal enum ListType
    {
        None,
        Bullet,
        Numbering,
        OutlineNumbering
    }
    /// <summary>
    /// Specifies type of the list numbering format.
    /// </summary>
    internal enum ListLevelPattern
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
    /// <summary>
    /// Specifies type of the Header/Footer.
    /// </summary>
    internal enum HeaderFooterType
    {
        /// <summary>
        /// Header for even numbered pages.
        /// </summary>
        EvenHeader = 0,
        /// <summary>
        /// Header for odd numbered pages.
        /// </summary>
        OddHeader = 1,
        /// <summary>
        /// Footer for even numbered pages.
        /// </summary>
        EvenFooter = 2,
        /// <summary>
        /// Footer for odd numbered pages.
        /// </summary>
        OddFooter = 3,
        /// <summary>
        /// Header for the first page of the section. 
        /// </summary>
        FirstPageHeader = 4,
        /// <summary>
        /// Footer for the first page of the section. 
        /// </summary>
        FirstPageFooter = 5
    }    
    /// <summary>
    /// Specifies the FormatType
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// Microsoft Word file format.
        /// </summary>
        Doc,
        /// <summary>
        /// Microsoft Word file format.
        /// </summary>
        Docx,
        /// <summary>
        /// Rtf format.
        /// </summary>
        Rtf,
        /// <summary>
        /// Text format.
        /// </summary>
        Txt
    }
    /// <summary>
    /// Specifies the page layout
    /// </summary>
    public enum LayoutType
    {
        /// <summary>
        /// Content will be displayed in multiple pages.
        /// </summary>
        Pages,
        /// <summary>
        /// Content will be displayed continuously in single page.
        /// </summary>
        Continuous,
        /// <summary>
        /// Content will be displayed continuously in single page, whereas Scrolling, Zooming, and Editing will be disabled (similar to RichTextBlock).
        /// </summary>
        Block
    }
    internal enum RowPlacement
    {
        /// <summary>
        /// 
        /// </summary>
        Above,

        /// <summary>
        /// 
        /// </summary>
        Below
    }

    internal enum ColumnPlacement
    {
        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Right
    }
}
