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

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Utility class. Holds string names of the properties, used for serialization.
    /// </summary>
    public class PropertyNames
    {
        #region Class constants
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string X1 = "x1";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string X2 = "x2";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Y1 = "y1";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Y2 = "y2";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string X3 = "x3";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string X4 = "x4";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Y3 = "y3";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Y4 = "y4";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Type = "type";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string X = "x";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Y = "y";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Width = "width";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Height = "height";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string StartAngle = "startangle";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string SweepAngle = "sweepangle";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string FillMode = "fillmode";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Items = "items";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Item = "item";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Points = "points";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Point = "point";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Style = "style";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Text = "text";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Shapes = "shapes";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string BorderColor = "bordercolor";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Format = "format";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string ShapeFormat = "shape-format";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string NoFill = "nofill";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Color = "color";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string GradientFill = "gradientfill";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string GradientColorStart = "gradientcolorstart";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string GradientColorEnd = "gradientcolorend";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string GradientMode = "gradientmode";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string NoLine = "noline";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string DashStyle = "dashstyle";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Line = "line";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Fill = "fill";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string Transform = "transform";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string M11 = "m11";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string M12 = "m12";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string M21 = "m21";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string M22 = "m22";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string D1 = "d1";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string D2 = "d2";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// This constructor is bever used.
        /// </summary>
        private PropertyNames()
        {
        }
        #endregion
    }
    /// <summary>
    /// Constants used in DLS for serialization to XML format  
    /// </summary>
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
        public const string TOCFieldTag = "toc-field";
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
        ///  List style attributes names
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
        public const string TableHrAlignmentAttr = "HAlignment";
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
        public const string CellSamePaddingsAsTableAttr = "SamePaddingsAsTable";
        public const string TableCellTextureAttr = "Texture";
        public const string TableCellForeColorAttr = "ForeColor";
        public const string TableCellFitTextAttr = "FitText";
        public const string TableCellTextDirAttr = "TextDirection";
        /// <summary>
        /// Paragraph tag names
        /// </summary>
        public const string ParagraphItemTag = "paragraph";
        public const string ItemsTag = "items";
        public const string ItemTag = "item";
        public const string ItemTypeParagraphValue = "Paragraph";
        public const string ItemTypeTableValue = "Table";
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
        /// Width scale attributes.
        /// </summary>
        public const string WidthScale = "WidthScale";
        /// <summary>
        /// Name of the property.
        /// </summary>
        public const string HeightScale = "HeightScale";
        /// <summary>
        /// Image quality attributes names.
        /// </summary>
        public const string PictBrightnessAttr = "Brightness";
        public const string PictContrastAttr = "Contrast";
        public const string PictColorAttr = "Color";
        public const string CropFromLeft = "CropLeft";
        public const string CropFromRight = "CropRight";
        public const string CropFromTop = "CropTop";
        public const string CropFromBottom = "CropBottom";
        /// <summary>
        /// TextRange tag name
        /// </summary>
        public const string TextRangeTag = "text-range";
        /// <summary>
        /// TextRange child tag names
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
        public const string BorderHorizontalTag = "Horizontal";
        public const string BorderVerticalTag = "Vertical";
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
        //public const string ShapeHorizOriginAttr = "HorizontalOrigin";
        //public const string ShapeVertOriginAttr = "VerticalOrigin";
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
        /// Bookmark attributes name
        /// </summary>
        public const string BookMarkNameAttr = "BookmarkName";
        public const string Bookmark = "Bookmark";
        public const string IsCellGroupBkmkAttr = "IsCellGroupBkmk";
        /// <summary>
        /// Hyperlink attribute names
        /// </summary>
        public const string FilePath = "FilePath";
        public const string Uri = "Uri";
        public const string HyperlinkType = "HyperlinkType";
        public const string Hyperlink = "hyperlink";
        /// <summary>
        /// Picture formatting tag.
        /// </summary>
        public const string PictureCharFormatTag = "character-format";
        /// <summary>
        /// Break atrribute names.
        /// </summary>
        public const string BreakTypeAttr = "BreakType";
        #endregion

        #region DocIO
        public const string ProtectionTypeAttr = "ProtectionType";
        public const string ShadeFormDataAttr = "ShadeFormData";
        public const string MacrosTag = "macros";
        public const string EscherContainersTag = "escher-containers";
        public const string EscherDataConatinersTag = "escher-data";
        public const string MacroCommandsTag = "macros-commands";
        /// <summary>
        /// DOP internal data
        /// </summary>
        public const string DOPFormatting1Attr = "DOPFormatting1";
        public const string DOPDxaHotZAttr = "DOPDxaHotZ";
        public const string DOPConsecAttr = "DOPConsec";
        public const string DOPInternalData = "dop-internal";
        /// <summary>
        /// Standard font names
        /// </summary>
        public const string StandardAsciiFont = "StandardAscii";
        public const string StandardFarEastFont = "StandardFarEast";
        public const string StandardNonFarEastFont = "StandardNonFarEast";

        /// <summary>
        /// Merge field attribute names.
        /// </summary>
        public const string MergeFieldNameAttr = "Name";
        public const string FieldConvertedAttr = "ConvertedToText";
        public const string MergeFieldTextBeforeAttr = "BeforeText";
        public const string MergeFieldTextAfterAttr = "AfterText";
        public const string MergeFieldNumberFormatAttr = "NumberFormat";
        public const string MergeFieldDateFormatAttr = "DateFormat";
        public const string MergeFieldPrefixAttr = "Prefix";
        public const string FieldUpperCaseAttr = "UpperCase";
        public const string FieldLowerCaseAttr = "LowerCase";
        public const string FieldTitleCaseAttr = "TitleCase";
        public const string FieldFirstCapitalAttr = "FirstCapitalCase";
        public const string FieldIsLocalAttr = "IsLocal";
        public const string FieldTypeAttr = "FieldType";
        public const string FieldNameAttr = "FieldName";
        public const string FieldValueAttr = "FieldValue";
        public const string FieldMarkTypeAttr = "FieldMarkType";
        public const string FieldFormattingAttr = "FieldFormatting";
        /// <summary>
        /// Attribute name for embedded object storage name containing
        /// </summary>
        public const string EmbedFieldStorageNameAttr = "StoragePicLocation";
        /// <summary>
        /// FormField attribute names.
        /// </summary>
        public const string FormFieldParamsAttr = "Params";
        public const string FormFieldMaxLengthAttr = "MaxLength";
        public const string FormFieldCheckBoxSizeAttr = "CheckBoxSize";
        public const string FormFieldCheckBoxSizeType = "CheckBoxSizeType";
        public const string FormFieldTitleAttr = "Title";
        public const string FormFieldDefTextAttr = "DefaultText";
        public const string FormFieldDefaultCheckBoxValueAttr = "DefaultCheckBoxValue";
        public const string FormFieldDefaultDropDownValueAttr = "DefaultDrowDownValue";
        public const string FormFieldTextFormatAttr = "TextFormat";
        public const string FormFieldStrTextFormatAttr = "StringTextFormat";
        public const string FormFieldHelpAttr = "Help";
        public const string FormFieldTooltipAttr = "Tooltip";
        public const string FormFieldMacroOnStartAttr = "MacroOnStart";
        public const string FormFieldMacroOnEndAttr = "MacroOnEnd";
        public const string FormFieldIsSpecialAttr = "IsSpecial";
        public const string FormFieldTextTypeAttr = "TextType";
        /// <summary>
        ///  FormField attribute names.
        /// </summary>
        public const string FormFieldDropDownItemsTag = "dropdown-items";
        public const string FormFieldDropDownItemTextAttr = "itemText";
        /// <summary>
        /// Document properties tag names.
        /// </summary>
        public const string BuiltinPropertiesTag = "builtin-properties";
        public const string CustomPropertiesTag = "custom-properties";
        public const string PropertyTag = "property";
        public const string CharacterStylesTag = "char-styles";
        public const string TextCharStyleName = "CharStyleName";
        public const string ListOverridesTag = "list-overrides";
        /// <summary>
        /// Styles attributes.
        /// </summary>
        public const string StyleIdAttr = "StyleId";
        /// <summary>
        /// Document properties attribute names.
        /// </summary>
        public const string PropertiesCompanyAttr = "Company";
        public const string PropertiesManagerAttr = "Manager";
        public const string PropertiesCategoryAttr = "Category";
        public const string PropertiesBytesCountAttr = "BytesCount";
        public const string PropertiesLinesCountAttr = "LinesCount";
        public const string PropertiesParagraphCountAttr = "ParagraphCount";
        public const string PropertiesSlideCountAttr = "SlideCount";
        public const string PropertiesNoteCountAttr = "NoteCount";
        public const string PropertiesHiddenCountAttr = "HiddenCount";
        public const string PropertiesAuthorAttr = "Author";
        public const string PropertiesApplicationNameAttr = "ApplicationName";
        public const string PropertiesTitleAttr = "Title";
        public const string PropertiesSubjectAttr = "Subject";
        public const string PropertiesKeywordsAttr = "Keywords";
        public const string PropertiesCommentsAttr = "Comments";
        public const string PropertiesTemplateAttr = "Template";
        public const string PropertiesLastAuthorAttr = "LastAuthor";
        public const string PropertiesRevisionNumberAttr = "RevisionNumber";
        public const string PropertiesEditTimeAttr = "EditTime";
        public const string PropertiesLastPrintedAttr = "LastPrinted";
        public const string PropertiesCreateDateAttr = "CreateDate";
        public const string PropertiesLastSaveDateAttr = "LastSaveDate";
        public const string PropertiesPageCountAttr = "PageCount";
        public const string PropertiesWordCountAttr = "WordCount";
        public const string PropertiesCharCountAttr = "CharCount";
        public const string PropertiesThumbnailAttr = "Thumbnail";
        public const string PropertiesDocSecurityAttr = "DocSecurity";
        /// <summary>
        ///  Property attribute names.
        /// </summary>
        public const string PropertiesTypeAttr = "Type";
        public const string PropertiesNameAttr = "Name";
        public const string PropertiesValueAttr = "Value";
        /// <summary>
        /// Attributes for ListFormatting.
        /// </summary>
        public const string ListFormatListTypeAttr = "ListType";
        public const string ListFormatUseBaseStyleAttr = "UseBaseStyle";
        public const string ListFormatLfoStyleNameAttr = "LfoStyleName";
        public const string ListFormatHybridAttr = "Hybrid";
        public const string ListFormatSimpleAttr = "Simple";

        public const string ListLevelLegacyAttr = "Legacy";
        public const string ListLevelLegacySpaceAttr = "LegacySpace";
        public const string ListLevelLegacyIndentAttr = "LegacyIndent";

        public const string LevelOverrideTag = "level-override";
        public const string LevelOverrideStartAtAttr = "StartAt";
        public const string LevelOverrideFormatAttr = "ChangeFormat";
        public const string LevelOverrideStartAttr = "ChangeStartAt";
        public const string LevelOverrideReserved1Attr = "Reserved1";
        public const string LevelOverrideReserved2Attr = "Reserved2";
        public const string LevelOverrideReserved3Attr = "Reserved3";

        public const string OverrideListLevelsTag = "override-levels";
        public const string OverrideListLevelTag = "override-level";
        public const string OverrideListStyleTag = "OverrideListStyle";
        public const string OverrideListStyleNameAttr = "OverrideStyleName";
        public const string OverrideListRes1Attr = "Res1";
        public const string OverrideListRes2Attr = "Res2";
        public const string OverrideListUnused1Attr = "Unused1";
        public const string OverrideListUnused2Attr = "Unused2";
        /// <summary>
        /// Table`s attributes names.
        /// </summary>
        public const string TableIsAutoResizedAttr = "IsAutoResized";
        public const string TableIsBreakAcrossPagesAttr = "IsBreakAcrossPages";

        public const string TableBidiAttr = "BidiTable";
        /// <summary>
        /// Internal Data.
        /// </summary>
        public const string InternalDataTag = "internal-data";
        /// <summary>
        /// ShapeObject tags.
        /// </summary>
        public const string ShapeContainerDataTag = "ShapeContainer";
        public const string ShapeBlipTag = "ShapeBlip";
        public const string ShapeFbseTag = "ShapeFbse";
        public const string PictureDescriptorTag = "PictureDescriptor";
        /// <summary>
        /// ShapeObjject attribute names
        /// </summary>    
        public const string ShapeObjIsOLEAttr = "IsOLE";
        public const string ShapeObjOLEContId = "OLEContainerId";
        /// <summary>
        /// WCharacterFormat attribute names.
        /// </summary>
        public const string TextShadowAttr = "Shadow";
        public const string TextEmbossAttr = "Emboss";
        public const string TextEngraveAttr = "Engrave";
        public const string TextHiddenAttr = "Hidden";
        public const string TextHiddenComplexAttr = "HiddenComplex";
        public const string TextDStrikeComplexAttr = "DStrikeComplex";
        public const string TextSmallCapsComplexAttr = "SmallCapsComplex";
        public const string TextStrikeComplexAttr = "StrikeComplex";
        public const string TextAllCapsAttr = "AllCaps";
        public const string TextSmallCapsAttr = "SmallCaps";
        public const string TextBidiAttr = "Bidi";
        public const string TextBoldBidiAttr = "BoldBidi";
        public const string TextItalicBidiAttr = "ItalicBidi";
        public const string TextFontSizeBidiAttr = "FontSizeBidi";
        public const string TextFontNameBidiAttr = "FontNameBidi";
        public const string TextFontNameAsciiAttr = "FontNameAscii";
        public const string TextFontNameFarEastAttr = "FontNameFarEast";
        public const string TextFontNameNonFarEastAttr = "FontNameNonFarEast";
        public const string TextHighlightColorKey = "HighlightColor";
        public const string TextItalicComplexKey = "ItalicComplex";
        public const string TextShadowComplexAttr = "ShadowComplex";
        public const string TextBoldComplexKey = "BoldComplex";
        public const string TextRgLid0Attr = "RgLid0";
        public const string TextRgLid1Attr = "RgLid1";
        public const string TextRgLid3Attr = "RgLid3";
        public const string TextRgLid3_2Attr = "RgLid3_2";
        public const string TextLidAttr = "Lid";
        public const string TextLidBiAttr = "LidBi";
        public const string TextNoProofAttr = "NoProof";
        public const string TextIdctHintAttr = "hint";
        public const string TextAllCapsComplexKey = "AllCapsComplex";
        public const string TextTextureAttr = "Texture";
        public const string TextForeColorAttr = "ForeColor";
        public const string TextOutLineAttr = "Outline";
        public const string TextEmbossComplex = "EmbossComplex";
        public const string TextEngraveComplex = "EngraveComplex";
        /// <summary>
        /// WCharacterFormat tag names.
        /// </summary>
        public const string TextBorderTag = "text-border";
        /// <summary>
        /// ParagraphFormat attribute names.
        /// </summary>    
        public const string ParagraphLineSpacingAttr = "LineSpacing";
        public const string ParagraphLineSpacingRuleAttr = "LineSpacingRule";

        /// <summary>
        /// ViewSetup tag names.
        /// </summary>
        public const string ViewSetupTag = "view-setup";
        /// <summary>
        /// ViewSetup attribute names.
        /// </summary>
        public const string ViewSetupZoomPercentAttr = "ZoomPercent";
        public const string ViewSetupZoomTypeAttr = "ZoomType";
        public const string ViewSetupViewTypeAttr = "ViewType";
        /// <summary>
        /// WPageSetup attribute names.
        /// </summary>
        public const string PageSetupLineNumStepAttr = "PageSetupLineNumStep";
        public const string PageSetupLineNumDistanceAttr = "PageSetupLineNumDistance";
        public const string PageSetupLineNumModeAttr = "PageSetupLineNumMode";
        public const string PageSetupLineNumStartValueAttr = "PageSetupLineNumStartValue";
        public const string PageSetupBorderApplyAttr = "PageSetupBorderApply";
        public const string PageSetupBorderOffsetFromAttr = "PageSetupBorderOffsetFrom";
        public const string PageSetupBorderIsInFrontAttr = "PageSetupBorderIsInFront";
        public const string PageSetupColumnEqualAttr = "EqualColWidth";
        /// <summary>
        /// Attributes names for characters.
        /// </summary>
        public const string SymbolCharCodeAttr = "CharCode";
        public const string SymbolCharCodeExtAttr = "CharCodeExt";
        /// <summary>
        /// Textbox tag name
        /// </summary>
        public const string TextBoxTag = "textbox";
        /// <summary>
        /// Shape attribute names
        /// </summary>
        public const string ShapeHorizOriginAttr = "HorizontalOrigin";
        public const string ShapeVertOriginAttr = "VerticalOrigin";
        public const string ShapeHorizPositionAttr = "HorizontalPosition";
        public const string ShapeVertPositionAttr = "VerticalPosition";
        public const string ShapeLineWidthAttr = "LineWidth";
        public const string ShapeLineDashingAttr = "LineDashing";
        public const string ShapeWrappingTypeAttr = "WrappingType";
        public const string ShapeWrappingStyleAttr = "WrappingStyle";
        public const string ShapeWrappingModeAttr = "WrappingMode";
        public const string ShapeIsBelowTextAttr = "IsBelowText";
        public const string ShapeNoLineAttr = "NoLine";
        public const string ShapeNoFillAttr = "NoFill";
        public const string ShapeHorizAlignAttr = "HorizontalAlignment";
        public const string ShapeVertAlignAttr = "VerticalAlignment";
        public const string ShapeFrontOrderAttr = "FrontOrder";
        public const string ShapeIdentAttr = "ShapeID";
        public const string ShapeTextBoxCountAttr = "TxbxCount";
        public const string ShapeIsHeaderAttr = "IsHeader";
        /// <summary>
        /// Comment tags.
        /// </summary>
        public const string CommentTag = "comment";
        public const string CommentFormatTag = "comment-format";
        public const string CommentFormatUserInitialsAttr = "UserInitials";
        public const string CommentFormatUserAttr = "User";
        public const string CommentBookmarkStartAttr = "BookmarkStartPos";
        public const string CommentBookmarkEndAttr = "BookmarkEndPos";
        public const string CommentTagBkmkAttr = "TagBkmk";
        /// <summary>
        /// Footnote tags.
        /// </summary>
        public const string FootnoteTag = "footnote";
        public const string FootnoteMarkerCPTag = "marker-character-format";
        public const string FootnoteIsAutoNumberedAttr = "AutoNumbered";
        public const string FootnoteCustomMarkerAttr = "CustomMarker";
        public const string FootnoteTypeAttr = "IsEndnoteAttr";
        public const string FootnoteSymbolCodeAttr = "SymbolCode";
        public const string FootnoteSymbolFontNameAttr = "SymbolFontName";


        public const string InitialFootnoteNumberAttr = "InitialFootnoteNumber";
        public const string FootnotePositionAttr = "FootnotePosition";
        public const string RestartIndexForFootnotesAttr = "RestartIndexForFootnotes";
        public const string EndnoteNumberFormatAttr = "EndnoteNumberFormat";
        public const string FootnoteNumberFormatAttr = "FootnoteNumberFormat";
        public const string RestartIndexForEndnoteAttr = "RestartIndexForEndnote";
        public const string EndnotePositionAttr = "EndnotePosition";
        public const string InitialEndnoteNumberAttr = "InitialEndnoteNumber";
        /// <summary>
        /// Watermark. 
        /// </summary>
        public const string WatermarkTag = "watermark";
        public const string WatermarkTypeAttr = "WatermarkType";
        public const string WatermarkTextAttr = "Text";
        public const string WatermarkTextFontNameAttr = "TextFontName";
        public const string WatermarkTextFontSizeAttr = "TextFontSize";
        public const string WatermarkTextFontColorAttr = "TextFontColor";
        public const string WatermarkTextLayoutAttr = "TextLayout";
        public const string WatermarkTextSemitransAttr = "Semitransparent";
        public const string WatermarkShapeHeightAttr = "ShapeHeight";
        public const string WatermarkShapeWidthAttr = "ShapeWidth";

        public const string WatermarkPictureWashoutAttr = "PictureWashout";
        public const string WatermarkPictureScaleAttr = "PictureScale";
        public const string WatermarkPictureIsMetaAttr = "PictureIsMetafile";
        public const string WatermarkPicturePibAttr = "PicturePib";
        /// <summary>
        /// Background effects.
        /// </summary>
        public const string BackgroundTag = "background";
        public const string BackgroundGradientTag = "gradient";
        public const string BackgroundTypeAttr = "Type";
        public const string BackImageIsMetaAttr = "IsMetafile";
        public const string BackgroundColorAttr = "FillColor";
        public const string BackgroundBackColorAttr = "FillBackgroundColor";
        public const string BackgroundGradientStyle = "GradientShadingStyle";
        public const string BackgroundGradientVariant = "GradientShadingVariant";
        /// <summary>
        ///  Embedded object storage tags
        /// </summary>    
        public const string EmbedObjectStorageNameAttr = "Name";
        public const string EmbedObjectIsOle2Attr = "Ole2Object";
        public const string ObjectPoolTag = "object-pool";
        /// <summary>
        /// Grammar and spelling data tags
        /// </summary>    
        public const string GrammarDataTag = "grammar-data";
        public const string SpellingDataTag = "spelling-data";
        #endregion
    }
}
