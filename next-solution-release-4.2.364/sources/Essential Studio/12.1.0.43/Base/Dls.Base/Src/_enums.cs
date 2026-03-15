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
#endregion

namespace Syncfusion.DLS
{
  #region enum DLSFieldType
  /// <summary>
  /// Specifies type of the field.
  /// </summary>
  public enum DLSFieldType
  {
    /// <summary>
    /// Unknown type of the field
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// The number of the current page.
    /// </summary>
    FieldPage = 1,
    /// <summary>
    /// The number of pages in the document. 
    /// </summary>
    FieldNumPages = 2
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
    /// The style is a image style. 
    /// </summary>
    ImageStyle,
    /// <summary>
    /// The style is a shape style. 
    /// </summary>
    ShapeStyle,
    /// <summary>
    /// The style is other kind of style. 
    /// </summary>
    OtherStyle,
    /// <summary>
    /// The style is a character style. 
    /// </summary>
    CharacterStyle
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
    /// ParagraphItem is a canvas.
    /// </summary>
    Canvas,
    /// <summary>
    /// ParagraphItem is a table.
    /// </summary>
    Table,
    /// <summary>
    /// ParagraphItem is a start of bookmark.
    /// </summary>
    BookmarkStart,
    /// <summary>
    /// ParagraphItem is a end of bookmark.
    /// </summary>
    BookmarkEnd,
    /// <summary>
    /// ParagraphItem is a field.
    /// </summary>
    Field,
    /// <summary>
    /// ParagraphItem is a textbox. 
    /// </summary>
    TextBox,
    /// <summary>
    /// PragraphItem is a break.
    /// </summary>
    PageBreak,
    /// <summary>
    /// PragraphItem is a break.
    /// </summary>
    Break
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
    Middle = 1 ,
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
    Middle = 1 ,
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
    /// Specified subrscript format.
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
    UriLink,
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
    Character = 3
  }
  #endregion

  #region enum VerticalOrigin
  /// <summary>
  /// Srecify vertical origin of the object
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
    Line = 3
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
  /// 
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
  /// 
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
  /// 
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
    /// List leve's number or bullet is followed by tab
    /// </summary>
    Tab = 0,
    /// <summary>
    /// List leve's number or bullet is followed by space
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
    ColumnBreak = 1  
  }

  #endregion
}