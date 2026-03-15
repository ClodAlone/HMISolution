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

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// Constants used in DLS for serialization into XML format  
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class XDLSConstants
  {
    #region Class constants
    /// <summary>
    /// Document tag names
    /// </summary>
    public const string SectionsTag = "sections";
    public const string StylesTag = "styles";
    public const string ListStylesTag = "liststyles";
    /// <summary>
    /// Section tag names
    /// </summary>
    public const string SectionItemTag = "section";
    public const string PageSetupTag = "page-setup";
    public const string PageSetupFirstPageAttr = "DifferentFirstPage";
    public const string PageSetupDiffOddEvenPagesAttr = "DifferentOddEvenPage";
    public const string ColumnsTag = "columns";
    public const string ColumnTag = "column";
    public const string ParagraphsTag = "paragraphs";
    public const string HeadersFootersTag = "headers-footers";
    public const string TextBodyTag = "body";
    /// <summary>
    /// Section attribute names
    /// </summary>
    public const string SectionBreakCodeAttr = "BreakCode";
    /// <summary>
    /// Style tag names
    /// </summary>
    public const string StyleItemTag = "style";

    public const string CharacterFormatTag = "character-format";
    public const string ParagraphFormatTag = "paragraph-format";
    public const string TableFormatTag = "table-format";
    public const string CellFormatTag = "cell-format";
    public const string ListFormatTag = "list-format";
    public const string StyleBaseTag = "base";
    public const string ShapeFormatTag = "shape-format";

    /// <summary>
    /// List tag and attribute names
    /// </summary>    
    public const string ListLevelItemTag = "level";
    public const string ListLevelsTag = "levels";
    public const string ListLevelInternalDataTag = "internal-data";

    public const string ListLevelPrefPatternAttr = "PrefPattern";
    public const string ListLevelSufPatternAttr = "SufPattern";
    public const string ListLevelBulletPatternAttr = "BulletPattern";
    public const string ListLevelIndentAttr = "Indent";
    public const string ListLevelPrevPatternAttr = "PrevPattern";
    public const string ListLevelPatternTypeAttr = "PatternType";
    public const string ListLevelStartAtAttr = "StartAt";
    public const string ListLevelNumberAlignAttr = "NumberAlign";
    public const string ListLevelFollowCharacterAttr = "FollowCharacter";
    public const string ListLevelIsLegalAttr = "IsLegal";
    public const string ListLevelNoRestartNum = "NoRestart";    
    
    /// <summary>
    /// 
    /// </summary>
    public const string ListFormatTypeAttr = "ListType";
    public const string ListFormatLevelNumAttr = "LevelNumber";
    public const string ListFormatStyleNameAttr = "Name"; 
    /// <summary>
    /// Style attribute names
    /// </summary>
    public const string StyleNameAttr = "Name";
    public const string StyleBaseNameAttr = "BaseStyleName";

    /// <summary>
    /// Pen attribute names
    /// </summary>
    public const string PenColorAttr = "Color";
    public const string PenBrushAttr = "Brush";
    /// <summary>
    /// PageSettings attribute names
    /// </summary>
    public const string PSPageHeightAttr = "PageHeight";
    public const string PSPageWidthAttr = "PageWidth";
    public const string PSAlignmentAttr = "Alignment";
    public const string PSFooterDistanceAttr = "FooterDistance";
    public const string PSHeaderDistanceAttr = "HeaderDistance";
    public const string PSOrientationAttr = "Orientation";
    public const string PSBottomMarginAttr = "BottomMargin";
    public const string PSTopMarginAttr = "TopMargin";
    public const string PSLeftMarginAttr = "LeftMargin";
    public const string PSRightMarginAttr = "RightMargin";
    public const string PSSpacingBeforeColumnsAttr = "SpacingBeforeColumns";
    /// <summary>
    /// HeadersFooters attribute names
    /// </summary>
    public const string EvenHeaderTag = "even-header";
    public const string OddHeaderTag = "odd-header";
    public const string EvenFooterTag = "even-footer";
    public const string OddFooterTag = "odd-footer";
    public const string FirstPageHeaderTag = "first-page-header";
    public const string FirstPageFooterTag = "first-page-footer";
    /// <summary>
    /// Tables tag names
    /// </summary>
    public const string ColumnItemTag = "column";
    public const string ColumnsItemTag = "columns";
    public const string CellItemTag = "cell";
    public const string CellsItemTag = "cells";
    public const string RowItemTag = "row";
    public const string RowsItemTag = "rows";
    /// <summary>
    /// Tables attribute names
    /// </summary>
    public const string TableVrAlignmentAttr = "VAlignment";
    public const string TableVrMergeAttr = "VMerge";
    public const string TableHorizMergeAttr = "HMerge";
    public const string TableWidthAttr = "Width";
    public const string TableCellWidthAttr = "Width";
    public const string TableCellShadingColorAttr = "ShadingColor";
    public const string TableRowHeigthAttr = "RowHeight";
    public const string TableRowHeaderAttr = "IsHeader";
    public const string TableRowHeighTypeAttr = "HeightType";
    public const string TableLeftIndentAttr = "LeftIndent";
    public const string TableCellPaddingsAttr = "Paddings";
    public const string CellSpacing = "CellSpacing";
    public const string LeftOffset = "LeftOffset";
    public const string CellTextWrapAttr = "TextWrap";
    public const string TableCellIsVerticalAttr = "IsVertical";
    public const string TableCellIsBackWardAttr = "IsBackWard";
    public const string TableCellTextureAttr = "Texture";
    public const string TableCellForeColorAttr = "ForeColor";
    /// <summary>
    /// Paragraph tag names
    /// </summary>
    public const string ParagraphItemTag = "paragraph";
    public const string ItemsTag = "items";
    public const string ItemTag = "item";
    /// <summary>
    /// ParagraphFormat attribute names
    /// </summary>
    public const string ParagraphTabsAttr = "Tabs";
    public const string ParagraphHrAlignmentAttr = "HrAlignment";
    public const string ParagraphVrAlignmentAttr = "VrAlignment";
    public const string ParagraphLeftIndentAttr = "LeftIndent";
    public const string ParagraphRightIndentAttr = "RightIndent";
    public const string ParagraphFirstLineIndentAttr = "FirstLineIndent";
    public const string ParagraphKeepAttr = "Keep";
    public const string ParagraphBeforeSpacingAttr = "BeforeSpacing";
    public const string ParagraphAfterSpacingAttr = "AfterSpacing";
    public const string ParagraphKeepFollowAttr = "KeepFollow";
    public const string ParagraphWidowControlAttr = "WidowControl";
    public const string ParagraphPageBreakBeforeAttr = "PageBreakBefore";
    public const string ParagraphPageBreakAfterAttr = "PageBreakAfter";
    public const string ParagraphColumnBreakAfterAttr = "ColumnBreakAfter";
    public const string ParagraphBackColorAttr = "BackColor";
    public const string ParagraphBidiAttr = "Bidi";
    /// <summary>
    /// Tab attribute name
    /// </summary>
    public const string TabPositionAttr = "Position";
    public const string TabJustificationAttr = "Justification";
    public const string TabLeaderAttr = "Leader";
    public const string TabTag = "Tab";
    public const string AutoTabWidthAttr = "AutoTabWidth";
    public const string TabDeleteAttr = "Delete";
    /// <summary>
    /// Shape tag names
    /// </summary>
    public const string ShapeItemTag = "shape";
    /// <summary>
    /// Column attribute names
    /// </summary>
    public const string ColumnWidthAttr = "Width";
    public const string ColumnSpacingAttr = "Spacing";
    /// <summary>
    /// Image tag names
    /// </summary>
    public const string ImageTag = "image";
    
    /// <summary>
    /// Image attribute names
    /// </summary>
    public const string ImageIsMetafileAttr = "IsMetafile";
    /// <summary>
    /// 
    /// </summary>
    public const string WidthScale = "WidthScale";
    /// <summary>
    /// Name of the property.
    /// </summary>
    public const string HeightScale = "HeightScale";
    /// <summary>
    /// 
    /// </summary>
    public const string PictBrightnessAttr = "Brightness";
    public const string PictContrastAttr = "Contrast";
    public const string PictColorAttr = "Color";
    public const string CropFromLeft = "CropLeft";
    public const string CropFromRight = "CropRight";
    public const string CropFromTop = "CropTop";
    public const string CropFromBottom = "CropBottom";
    
    /// <summary>
    /// TextRange tag names
    /// </summary>
    public const string TextTag = "text";
    /// <summary>
    /// Type tag
    /// </summary>
    public const string TypeTag = "type";
    /// <summary>
    /// Paddings tag names
    /// </summary>
    public const string PaddingBottomTag = "Bottom";
    public const string PaddingTopTag = "Top";
    public const string PaddingLeftTag = "Left";
    public const string PaddingRightTag = "Right";
    /// <summary>
    /// Borders tag names
    /// </summary>
    public const string BorderBottomTag = "Bottom";
    public const string BorderTopTag = "Top";
    public const string BorderLeftTag = "Left";
    public const string BorderRightTag = "Right";
    public const string BordersItemTag = "borders";
    /// <summary>
    /// Borders attribute names
    /// </summary>
    public const string BorderColorAttr = "Color";
    public const string BorderWidthAttr = "LineWidth";
    public const string BorderTypeAttr = "BorderType";
    public const string BorderSpaceAttr = "Space";
    public const string BorderShadowAttr = "Shadow";
    /// <summary>
    /// CharacterFormat attribute names
    /// </summary>
    public const string TextColorAttr = "TextColor";
    public const string TextFontNameAttr = "FontName";
    public const string TextFontSizeAttr = "FontSize";
    public const string TextBoldAttr = "Bold";
    public const string TextItalicAttr = "Italic";
    public const string TextStrikeAttr = "Strike";
    public const string TextDoubleStrikeAttr = "DoubleStrike";
    public const string TextUnderlineAttr = "Underline";
    public const string TextSubSuperScriptAttr = "SubSuperScript";
    public const string TextLineSpacingAttr = "LineSpacing";
    public const string TextPositionAttr = "Position";
    public const string TextBackgroundColorAttr = "TextBackgroundColor";
    public const string TextLineBreakAttr = "LineBreak";
    /// <summary>
    /// TextBox tag name
    /// </summary>
    public const string TextBoxFormatTag = "textbox-format";
    /// <summary>
    /// TextBoxFormat attribute names
    /// </summary>
    public const string ShapeHorizOriginAttr = "HorizontalOrigin";
    public const string ShapeVertOriginAttr = "VerticalOrigin";
    public const string ShapeWidthAttr = "Width";
    public const string ShapeHeightAttr = "Height";
    public const string ShapeLineStyleAttr = "LineStyle"; 
    public const string ShapeTextWrappingStyleAttr = "WrappingStyle";
    public const string ShapeFillColorAttr = "FillColor"; 
    public const string ShapeLineColorAttr = "LineColor";
    /// <summary>
    /// TextBoxes tag
    /// </summary>
    public const string HFTextBoxesTag = "hf-textboxes";
    public const string TextBoxesTag = "textboxes";
    /// <summary>
    /// 
    /// </summary>
    public const string BookMarkNameAttr = "BookmarkName";
    public const string Bookmark = "Bookmark";
    /// <summary>
    /// Hyperlink attribute names
    /// </summary>
    public const string FilePath = "FilePath";
    public const string Uri = "Uri";
    public const string HyperlinkType = "HyperlinkType";
    public const string Hyperlink = "hyperlink";
    /// <summary>
    /// 
    /// </summary>
    public const string FieldTypeAttr = "FieldType";
    public const string PictureCharFormatTag = "character-format";
    /// <summary>
    /// Table tags.
    /// </summary>
    public const string BorderVerticalTag = "BorderVertical";
    public const string BorderHorizontalTag = "BorderHorizontal";
    /// <summary>
    /// Break atrribute names.
    /// </summary>
    public const string BreakTypeAttr = "BreakType";
    #endregion
  }
}
