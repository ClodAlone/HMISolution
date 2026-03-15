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
using System.Xml;
using System.Text;
using System.Collections;
using System.Globalization;

using Syncfusion.XlsIO.Interfaces.XmlSerialization;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser;
using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;

using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif


#endregion

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// Xml serializator. Serializes XlsIO workbook into MsExcel xml file format.
  /// </summary>
  [ XmlSerializator( ExcelXmlSaveType.MSExcel ) ]
	public class WorkbookXmlSerializator : IXmlSerializator
	{
    #region Class constants
    #region Global xml constants
    /// <summary>
    /// Represents xml version.
    /// </summary>
    public const string DEF_VERSION_STRING = @"<?xml version=""1.0""?>";
    /// <summary>
    /// Represents application string;
    /// </summary>
    public const string DEF_APPLICATION_STRING = @"<?mso-application progid=""Excel.Sheet""?>";
    /// <summary>
    /// Represents o namespace.
    /// </summary>
    private const string DEF_O_NAMESPACE = "urn:schemas-microsoft-com:office:office";
    /// <summary>
    /// Represents x namespace.
    /// </summary>
    private const string DEF_X_NAMESPACE = "urn:schemas-microsoft-com:office:excel";
    /// <summary>
    /// Represents ss namespace.
    /// </summary>
    private const string DEF_SS_NAMESPACE = "urn:schemas-microsoft-com:office:spreadsheet";
    /// <summary>
    /// Represents html namespace.
    /// </summary>
    private const string DEF_HTML_NAMESPACE = "http://www.w3.org/TR/REC-html40";
    /// <summary>
    /// Represents ss pref.
    /// </summary>
    private const string DEF_SS_PREF = "ss";
    /// <summary>
    /// Represents html pref.
    /// </summary>
    private const string DEF_HTML_PREF = "html";
    /// <summary>
    /// Represents o pref.
    /// </summary>
    private const string DEF_O_PREF = "o";
    /// <summary>
    /// Represents x pref.
    /// </summary>
    private const string DEF_X_PREF = "x";
    /// <summary>
    /// Represents default pref.
    /// </summary>
    private const string DEF_NAMESPACE_PREF = "xmlns:";
    /// <summary>
    /// Represents xmlns pref.
    /// </summary>
    internal const string DEF_XMLNS_PREF = "xmlns";
    #endregion

    #region Element constans
    /// <summary>
    /// Represents WorkBook pref.
    /// </summary>
    public const string DEF_WORKBOOK_PREF = "Workbook";
    /// <summary>
    /// Represents WorkSheet pref.
    /// </summary>
    public const string DEF_WORKSHEET_PREF = "Worksheet";
    /// <summary>
    /// Represents Name pref.
    /// </summary>
    public const string DEF_NAME_PREF = "Name";
    /// <summary>
    /// Represents Table pref.
    /// </summary>
    public const string DEF_TABLE_PREF = "Table";
    /// <summary>
    /// Represents Row pref.
    /// </summary>
    public const string DEF_ROW_PREF = "Row";
    /// <summary>
    /// Represents Cell pref.
    /// </summary>
    public const string DEF_CELL_PREF = "Cell";
    /// <summary>
    /// Represents Data pref.
    /// </summary>
    public const string DEF_DATA_PREF = "Data";
    /// <summary>
    /// Represents Names pref.
    /// </summary>
    public const string DEF_NAMES_PREF = "Names";
    /// <summary>
    /// Represents NamedRange pref.
    /// </summary>
    public const string DEF_NAMEDRANGE_PREF = "NamedRange";
    /// <summary>
    /// Represents Styles pref.
    /// </summary>
    public const string DEF_STYLES_PREF = "Styles";
    /// <summary>
    /// Represents Style pref.
    /// </summary>
    public const string DEF_STYLE_PREF = "Style";
    /// <summary>
    /// Represents Font pref.
    /// </summary>
    public const string DEF_FONT_PREF = "Font";
    /// <summary>
    /// Represents Protection pref.
    /// </summary>
    public const string DEF_PROTECTION_PREF = "Protection";
    /// <summary>
    /// Represents Alignment pref.
    /// </summary>
    public const string DEF_ALIGNMENT_PREF = "Alignment";
    /// <summary>
    /// Represents NumberFormat pref.
    /// </summary>
    public const string DEF_NUMBERFORMAT_PREF = "NumberFormat";
    /// <summary>
    /// Represents Interior pref.
    /// </summary>
    public const string DEF_INTERIOR_PREF = "Interior";
    /// <summary>
    /// Represents Borders pref.
    /// </summary>
    public const string DEF_BORDERS_PREF = "Borders";
    /// <summary>
    /// Represents Border pref.
    /// </summary>
    public const string DEF_BORDER_PREF = "Border";
    /// <summary>
    /// Represents AutoFilter pref.
    /// </summary>
    private const string DEF_AUTOFILTER_PREF = "AutoFilter";
    /// <summary>
    /// Represents AutoFilterColumn pref.
    /// </summary>
    private const string DEF_AUTOFILTERCOLUMN_PREF = "AutoFilterColumn";
    /// <summary>
    /// Represents AutoFilterAnd pref.
    /// </summary>
    private const string DEF_AUTOFILTERAND_PREF = "AutoFilterAnd";
    /// <summary>
    /// Represents AutoFilterCondition pref.
    /// </summary>
    private const string DEF_AUTOFILTERCONDITION_PREF = "AutoFilterCondition";
    /// <summary>
    /// Represents AutoFilterOr pref.
    /// </summary>
    private const string DEF_AUTOFILTEROR_PREF = "AutoFilterOr";
    /// <summary>
    /// Represents Comment pref.
    /// </summary>
    public const string DEF_COMMENT_PREF = "Comment";
    /// <summary>
    /// Represents B tag.
    /// </summary>
    private const string DEF_B_TAG = "<B>";
    /// <summary>
    /// Represents B end tag.
    /// </summary>
    private const string DEF_B_END_TAG = "</B>";
    /// <summary>
    /// Represents I tag.
    /// </summary>
    private const string DEF_I_TAG = "<I>";
    /// <summary>
    /// Represents I end tag.
    /// </summary>
    private const string DEF_I_END_TAG = "</I>";
    /// <summary>
    /// Represents U tag.
    /// </summary>
    private const string DEF_U_TAG = "<U>";
    /// <summary>
    /// Represents U end tag.
    /// </summary>
    private const string DEF_U_END_TAG = "</U>";
    /// <summary>
    /// Represents S tag.
    /// </summary>
    private const string DEF_S_TAG = "<S>";
    /// <summary>
    /// Represents S end tag.
    /// </summary>
    private const string DEF_S_END_TAG = "</S>";
    /// <summary>
    /// Represents Sub tag.
    /// </summary>
    private const string DEF_SUB_TAG = "<Sub>";
    /// <summary>
    /// Represents Sub end tag.
    /// </summary>
    private const string DEF_SUB_END_TAG = "</Sub>";
    /// <summary>
    /// Represents Sup tag.
    /// </summary>
    private const string DEF_SUP_TAG = "<Sup>";
    /// <summary>
    /// Represents Sup end tag.
    /// </summary>
    private const string DEF_SUP_END_TAG = "</Sup>";
    /// <summary>
    /// Represents Font end tag.
    /// </summary>
    private const string DEF_FONT_END_TAG = "</Font>";
    /// <summary>
    /// Represents Font tag.
    /// </summary>
    private const string DEF_FONT_TAG = "<Font";
    /// <summary>
    /// Represents Span pref.
    /// </summary>
    public const string DEF_SPAN_PREF = "Span";
    /// <summary>
    /// Represents Column pref.
    /// </summary>
    public const string DEF_COLUMN_PREF = "Column";
    /// <summary>
    /// Represents ConditionalFormatting pref.
    /// </summary>
    public const string DEF_CONDITIONAL_FORMATTING_PREF = "ConditionalFormatting";
    /// <summary>
    /// Represents Condition pref.
    /// </summary>
    public const string DEF_CONDITIONAL_PREF = "Condition";
    /// <summary>
    /// Represents Qualifier pref.
    /// </summary>
    public const string DEF_QUALIFIER_PREF = "Qualifier";
    /// <summary>
    /// Represents Value1 pref.
    /// </summary>
    public const string DEF_VALUE1_PREF = "Value1";
    /// <summary>
    /// Represents Value2 pref.
    /// </summary>
    public const string DEF_VALUE2_PREF = "Value2";
    /// <summary>
    /// Represents worksheet options pref.
    /// </summary>
    public const string DEF_WORKSHEET_OPTIONS_PREF = "WorksheetOptions";
    /// <summary>
    /// Represents page setup pref.
    /// </summary>
    public const string DEF_PAGE_SETUP_PREF = "PageSetup";
    /// <summary>
    /// Represents footer pref.
    /// </summary>
    public const string DEF_FOOTER_PREF = "Footer";
    /// <summary>
    /// Represents header pref.
    /// </summary>
    public const string DEF_HEADER_PREF = "Header";
    /// <summary>
    /// Represents layout pref.
    /// </summary>
    public const string DEF_LAYOUT_PREF = "Layout";
    /// <summary>
    /// Represents page margins pref.
    /// </summary>
    public const string DEF_PAGE_MARGINS_PREF = "PageMargins";
    /// <summary>
    /// Represents print pref.
    /// </summary>
    public const string DEF_PRINT_PREF = "Print";
    /// <summary>
    /// Represents print comments layout pref.
    /// </summary>
    public const string DEF_COMMENTS_LAYOUT_PREF = "CommentsLayout";
    /// <summary>
    /// Represents print errors pref.
    /// </summary>
    public const string DEF_PRINT_ERRORS_PREF = "PrintErrors";
    /// <summary>
    /// Represents fit to page pref.
    /// </summary>
    private const string DEF_FIT_TO_PAGE_PREF = "FitToPage";
    /// <summary>
    /// Represents LeftToRight pref.
    /// </summary>
    public const string DEF_LEFT_TO_RIGHT_PREF = "LeftToRight";
    /// <summary>
    /// Represents active pane pref.
    /// </summary>
    public const string DEF_ACTIVE_PANE_PREF = "ActivePane";
    /// <summary>
    /// Represents first visible row pref.
    /// </summary>
    public const string DEF_FIRST_VISIBLE_ROW_PREF = "TopRowVisible";
    /// <summary>
    /// Represents split horizontal pane pref.
    /// </summary>
    public const string DEF_SPLIT_HORIZONTAL_PANE_PREF = "SplitHorizontal";
    /// <summary>
    /// Represents split vertical pane pref.
    /// </summary>
    public const string DEF_SPLIT_VERTICAL_PANE_PREF = "SplitVertical";
    /// <summary>
    /// Represents top row bottom pane pref.
    /// </summary>
    public const string DEF_TOPROW_BOTTOM_PANE_PREF = "TopRowBottomPane";
    /// <summary>
    /// Represents left col right pane pref.
    /// </summary>
    public const string DEF_LEFTCOLUMN_RIGHT_PANE_PREF = "LeftColumnRightPane";
    /// <summary>
    /// Represents freeze panes pref.
    /// </summary>
    public const string DEF_FREEZE_PANES_PREF = "FreezePanes";
    /// <summary>
    /// Represents freeze no split panes pref.
    /// </summary>
    public const string DEF_FROZEN_NOSPLIT_PANES_PREF = "FrozenNoSplit";
    /// <summary>
    /// Represents panes pref.
    /// </summary>
    public const string DEF_PANES_PREF = "Panes";
    /// <summary>
    /// Represents pane pref.
    /// </summary>
    public const string DEF_PANE_PREF = "Pane";
    /// <summary>
    /// Represents number pane pref.
    /// </summary>
    public const string DEF_NUMBER_PANE_PREF = "Number";
    /// <summary>
    /// Represents active column pane pref.
    /// </summary>
    public const string DEF_ACTIVECOL_PANE_PREF = "ActiveCol";
    /// <summary>
    /// Represents active row pane pref.
    /// </summary>
    public const string DEF_ACTIVEROW_PANE_PREF = "ActiveRow";
    /// <summary>
    /// Represents tab color index pref.
    /// </summary>
    public const string DEF_TABCOLOR_INDEX_PREF = "TabColorIndex";
    /// <summary>
    /// Represents zoom pref.
    /// </summary>
    public const string DEF_ZOOM_PREF = "Zoom";
    /// <summary>
    /// Represents Do Not Display Gridlines pref.
    /// </summary>
    public const string DEF_DISPLAY_GRIDLINES_PREF = "DoNotDisplayGridlines";
    /// <summary>
    /// Represents Visible pref.
    /// </summary>
    public const string DEF_VISIBLE_PREF = "Visible";
    /// <summary>
    /// Represents don't display headings pref.
    /// </summary>
    private const string DEF_DISPLAY_HEADINGS_PREF = "DoNotDisplayHeadings";
    /// <summary>
    /// Represents ExcelWorkbook pref.
    /// </summary>
    public const string DEF_EXCELWORKBOOK_PREF = "ExcelWorkbook";
    /// <summary>
    /// Represents Active sheet pref.
    /// </summary>
    public const string DEF_ACTIVE_SHEET_PREF = "ActiveSheet";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    private const string DEF_SELECTED_PREF = "Selected";
    /// <summary>
    /// Represents selected sheets pref.
    /// </summary>
    private const string DEF_SELECTED_SHEETS_PREF = "SelectedSheets";
    /// <summary>
    /// Represents first visible sheet pref.
    /// </summary>
    public const string DEF_FIRST_VISIBLE_SHEET_PREF = "FirstVisibleSheet";
    /// <summary>
    /// Represents DataValidation pref.
    /// </summary>
    public const string DEF_DATAVALIDATION_PREF = "DataValidation";
    #endregion

    #region Attribute constants
    /// <summary>
    /// Represents RightToLeft pref.
    /// </summary>
    public const string DEF_RIGHTTOLEFT_PREF = "RightToLeft";
    /// <summary>
    /// Represents Index pref.
    /// </summary>
    public const string DEF_INDEX_PREF = "Index";
    /// <summary>
    /// Represents Type pref.
    /// </summary>
    public const string DEF_TYPE_PREF = "Type";
    /// <summary>
    /// Represents Ticked pref.
    /// </summary>
    private const string DEF_TICKED_PREF = "Ticked";
    /// <summary>
    /// Represents Formula pref.
    /// </summary>
    public const string DEF_FORMULA_PREF = "Formula";
    /// <summary>
    /// Represents RefersTo pref.
    /// </summary>
    public const string DEF_REFERSTO_PREF = "RefersTo";
    /// <summary>
    /// Represents ID pref.
    /// </summary>
    public const string DEF_ID_PREF = "ID";
    /// <summary>
    /// Represents Parent pref.
    /// </summary>
    public const string DEF_PARENT_PREF = "Parent";
    /// <summary>
    /// Represents Bold pref.
    /// </summary>
    public const string DEF_BOLD_PREF = "Bold";
    /// <summary>
    /// Represents FontName pref.
    /// </summary>
    public const string DEF_FONTNAME_PREF = "FontName";
    /// <summary>
    /// Represents Color pref.
    /// </summary>
    public const string DEF_COLOR_PREF = "Color";
    /// <summary>
    /// Represents Italic pref.
    /// </summary>
    public const string DEF_ITALIC_PREF = "Italic";
    /// <summary>
    /// Represents Outline pref.
    /// </summary>
    public const string DEF_OUTLINE_PREF = "Outline";
    /// <summary>
    /// Represents Shadow pref.
    /// </summary>
    public const string DEF_SHADOW_PREF = "Shadow";
    /// <summary>
    /// Represents Size pref.
    /// </summary>
    public const string DEF_SIZE_PREF = "Size";
    /// <summary>
    /// Represents StrikeThrough pref.
    /// </summary>
    public const string DEF_STRIKETHROUGH_PREF = "StrikeThrough";
    /// <summary>
    /// Represents Underline pref.
    /// </summary>
    public const string DEF_UNDERLINE_PREF = "Underline";
    /// <summary>
    /// Represents Protected pref.
    /// </summary>
    public const string DEF_PROTECTED_PREF = "Protected";
    /// <summary>
    /// Represents HideFormula pref.
    /// </summary>
    public const string DEF_HIDEFORMULA_PREF = "HideFormula";
    /// <summary>
    /// Represents Horizontal pref.
    /// </summary>
    public const string DEF_HORIZONTAL_PREF = "Horizontal";
    /// <summary>
    /// Represents Indent pref.
    /// </summary>
    public const string DEF_INDENT_PREF = "Indent";
    /// <summary>
    /// Represents ReadingOrder pref.
    /// </summary>
    public const string DEF_READINGORDER_PREF = "ReadingOrder";
    /// <summary>
    /// Represents Rotate pref.
    /// </summary>
    public const string DEF_ROTATE_PREF = "Rotate";
    /// <summary>
    /// Represents ShrinkToFit pref.
    /// </summary>
    public const string DEF_SHRINKTOFIT_PREF = "ShrinkToFit";
    /// <summary>
    /// Represents Vertical pref.
    /// </summary>
    public const string DEF_VERTICAL_PREF = "Vertical";
    /// <summary>
    /// Represents VerticalText pref.
    /// </summary>
    public const string DEF_VERTICALTEXT_PREF = "VerticalText";
    /// <summary>
    /// Represents WrapText pref.
    /// </summary>
    public const string DEF_WRAPTEXT_PREF = "WrapText";
    /// <summary>
    /// Represents Format pref.
    /// </summary>
    public const string DEF_FORMAT_PREF = "Format";
    /// <summary>
    /// Represents PatternColor pref.
    /// </summary>
    public const string DEF_PATTERNCOLOR_PREF = "PatternColor";
    /// <summary>
    /// Represents Pattern pref.
    /// </summary>
    public const string DEF_PATTERN_PREF = "Pattern";
    /// <summary>
    /// Represents Position pref.
    /// </summary>
    public const string DEF_POSITION_PREF = "Position";
    /// <summary>
    /// Represents Range pref.
    /// </summary>
    public const string DEF_RANGE_PREF = "Range";
    /// <summary>
    /// Represents Operator pref.
    /// </summary>
    private const string DEF_OPERATOR_PREF = "Operator";
    /// <summary>
    /// Represents Value pref.
    /// </summary>
    private const string DEF_VALUE_PREF = "Value";
    /// <summary>
    /// Represents Author pref.
    /// </summary>
    public const string DEF_AUTHOR_PREF = "Author";
    /// <summary>
    /// Represents ShowAlways pref.
    /// </summary>
    public const string DEF_SHOWALWAYS_PREF = "ShowAlways";
    /// <summary>
    /// Represents DefaultColumnWidth pref.
    /// </summary>
    public const string DEF_DEFAULTCOLUMNWIDTH_PREF = "DefaultColumnWidth";
    /// <summary>
    /// Represents DefaultRowHeight pref.
    /// </summary>
    public const string DEF_DEFAULTROWHEIGHT_PREF = "DefaultRowHeight";
    /// <summary>
    /// Represents Width pref.
    /// </summary>
    public const string DEF_WIDTH_PREF = "Width";
    /// <summary>
    /// Represents Hidden pref.
    /// </summary>
    public const string DEF_HIDDEN_PREF = "Hidden";
    /// <summary>
    /// Represents StyleID pref.
    /// </summary>
    public const string DEF_STYLEID_PREF = "StyleID";
    /// <summary>
    /// Represents AutoFitWidth pref.
    /// </summary>
    public const string DEF_AUTOFIT_WIDTH_PREF = "AutoFitWidth";
    /// <summary>
    /// Represents AutoFitHeight pref.
    /// </summary>
    public const string DEF_AUTOFIT_HEIGHT_PREF = "AutoFitHeight";
    /// <summary>
    /// Represents Height pref.
    /// </summary>
    public const string DEF_HEIGHT_PREF = "Height";
    /// <summary>
    /// Represents Face pref.
    /// </summary>
    public const string DEF_FACE_PREF = "Face";
    /// <summary>
    /// Represents line style pref.
    /// </summary>
    public const string DEF_LINE_STYLE_PREF = "LineStyle";
    /// <summary>
    /// Represents weight pref.
    /// </summary>
    public const string DEF_WEIGHT_PREF = "Weight";
    /// <summary>
    /// Represents vertical align pref.
    /// </summary>
    public const string DEF_VERTICAL_ALIGN_PREF = "VerticalAlign";
    /// <summary>
    /// Represents merge column count pref.
    /// </summary>
    public const string DEF_MERGE_ACROSS_PREF = "MergeAcross";
    /// <summary>
    /// Represents merge row count pref.
    /// </summary>
    public const string DEF_MERGE_DOWN_PREF = "MergeDown";
    /// <summary>
    /// Represents hyper link tip pref.
    /// </summary>
    public const string DEF_HYPRER_TIP_PREF = "HRefScreenTip";
    /// <summary>
    /// Represents hyper link reference pref.
    /// </summary>
    public const string DEF_HREF_PREF = "HRef";
    /// <summary>
    /// Represents margin pref.
    /// </summary>
    public const string DEF_MARGIN_PREF = "Margin";
    /// <summary>
    /// Represents margin top pref.
    /// </summary>
    public const string DEF_MARGIN_TOP_PREF = "Top";
    /// <summary>
    /// Represents margin right pref.
    /// </summary>
    public const string DEF_MARGIN_RIGHT_PREF = "Right";
    /// <summary>
    /// Represents margin left pref.
    /// </summary>
    public const string DEF_MARGIN_LEFT_PREF = "Left";
    /// <summary>
    /// Represents margin bottom pref.
    /// </summary>
    public const string DEF_MARGIN_BOTTOM_PREF = "Bottom";
    /// <summary>
    /// Represents center horizontal pref.
    /// </summary>
    public const string DEF_CENTER_HORIZONTAL_PREF = "CenterHorizontal";
    /// <summary>
    /// Represents center vertical pref.
    /// </summary>
    public const string DEF_CENTER_VERTICAL_PREF = "CenterVertical";
    /// <summary>
    /// Represents orientation pref.
    /// </summary>
    public const string DEF_ORIENTATION_PREF = "Orientation";
    /// <summary>
    /// Represents start page number pref.
    /// </summary>
    public const string DEF_START_PAGE_NUMBER_PREF = "StartPageNumber";
    /// <summary>
    /// Represents number of copies to print.
    /// </summary>
    public const string DEF_NUMBER_OF_COPIES_PREF = "NumberofCopies";
    /// <summary>
    /// Represents default numbers of copies to print.
    /// </summary>
    public const int DEF_NUMBER_OF_COPIES = 1;
    /// <summary>
    /// Represents horizontal resolution pref.
    /// </summary>
    public const string DEF_HORIZONTAL_RESOLUTION_PREF = "HorizontalResolution";
    /// <summary>
    /// Represents paper size index pref.
    /// </summary>
    public const string DEF_PAPER_SIZE_INDEX_PREF = "PaperSizeIndex";
    /// <summary>
    /// Represents Scale pref.
    /// </summary>
    public const string DEF_SCALE_PREF = "Scale";
    /// <summary>
    /// Represents fit width pref.
    /// </summary>
    public const string DEF_FIT_WIDTH_PREF = "FitWidth";
    /// <summary>
    /// Represents fit height pref.
    /// </summary>
    public const string DEF_FIT_HEIGHT_PREF = "FitHeight";
    /// <summary>
    /// Represents gridlines pref.
    /// </summary>
    public const string DEF_GRIDLINES_PREF = "Gridlines";
    /// <summary>
    /// Represents BlackAndWhite pref.
    /// </summary>
    public const string DEF_BLACK_AND_WHITE_PREF = "BlackAndWhite";
    /// <summary>
    /// Represents DraftQuality pref.
    /// </summary>
    public const string DEF_DRAFT_QUALITY_PREF = "DraftQuality";
    /// <summary>
    /// Represents row and column headings pref.
    /// </summary>
    public const string DEF_ROWCOL_HEADINGS_PREF = "RowColHeadings";
    #endregion

    #region Conditional format constants
    /// <summary>
    /// Represents colon.
    /// </summary>
    public const string DEF_COLON = ":";
    /// <summary>
    /// Represents semicolon.
    /// </summary>
    public const string DEF_SEMICOLON = ";";
    /// <summary>
    /// Represents font color.
    /// </summary>
    public const string DEF_FONT_COLOR_CF = "color";
    /// <summary>
    /// Represents font style.
    /// </summary>
    public const string DEF_FONT_STYLE_CF = "font-style";
    /// <summary>
    /// Represents font weight style.
    /// </summary>
    public const string DEF_FONT_WEIGHT_CF = "font-weight";
    /// <summary>
    /// Represents bold font weight.
    /// </summary>
    public const string DEF_FONT_BOLD_CF = "700";
    /// <summary>
    /// Represents regular font weight.
    /// </summary>
    public const string DEF_FONT_REGULAR_CF = "400";
    /// <summary>
    /// Represents italic font const.
    /// </summary>
    private const string DEF_FONT_ITALIC_CF = "font-style:italic;";
    /// <summary>
    /// Represents line through
    /// </summary>
    private const string DEF_FONT_STRIKE_CF = "text-line-through:single;";
    /// <summary>
    /// Represents line through
    /// </summary>
    public const string DEF_FONT_STRIKETHROUGH_CF = "text-line-through";
    /// <summary>
    /// Represents Single line through
    /// </summary>
    public const string DEF_FONT_STRIKETHROUGH_SINGLE_CF = "single";
    /// <summary>
    /// Represents font underline.
    /// </summary>
    public const string DEF_FONT_UNDERLINE_CF = "text-underline-style";
    /// <summary>
    /// Represents pattern back color.
    /// </summary>
    public const string DEF_PATTERN_BACKGROUND_CF = "background";
    /// <summary>
    /// Represents fill pattern.
    /// </summary>
    public const string DEF_PATTERN_FILL_CF = "mso-pattern";
    /// <summary>
    /// Represents border constant.
    /// </summary>
    private const string DEF_BORDER_CF = "border-";
    /// <summary>
    /// Represents border top constant.
    /// </summary>
    public const string DEF_BORDERTOP_CF = "border-top";
    /// <summary>
    /// Represents border bottom constant.
    /// </summary>
    public const string DEF_BORDERBOTTOM_CF = "border-bottom";
    /// <summary>
    /// Represents border left constant.
    /// </summary>
    public const string DEF_BORDERLEFT_CF = "border-left";
    /// <summary>
    /// Represents border right constant.
    /// </summary>
    public const string DEF_BORDERRIGHT_CF = "border-right";
    /// <summary>
    /// Represents pattern string.
    /// </summary>
    public static readonly string[] DEF_PATTERN_STRING_CF =
    {
      "none",
      "solid",
      "gray-50",
      "gray-75",
      "gray-25",
      "horz-stripe",
      "vert-stripe",
      "reverse-diag-stripe",
      "diag-stripe",
      "diag-cross",
      "thick-diag-cross",
      "thin-horz-stripe",
      "thin-vert-stripe",
      "thin-reverse-diag-stripe",
      "thin-diag-stripe",
      "thin-horz-cross",
      "thin-diag-cross",
      "gray-125",
      "gray-0625",
    };
    /// <summary>
    /// Represents border line style.
    /// </summary>
    public static readonly string[] DEF_BORDER_LINE_CF =
    {
      "none",
      ".5pt solid",
      "1.0pt solid",
      ".5pt dashed",
      ".5pt dotted",
      "1.5pt solid",
      "2.0pt double",
      ".5pt hairline",
      "1.0pt dashed",
      ".5pt dot-dash",
      "1.0pt dot-dash",
      ".5pt dot-dot-dash",
      "1.0pt dot-dot-dash",
      "1.0pt dot-dash-slanted",
    };
    /// <summary>
    /// Represent the XML Spreadsheet Comparision operators
    /// </summary>
    public static readonly string[] DEF_COMPARISION_OPERATORS_PREF =
    {
    "None",
    "Between",
    "NotBetween",
    "Equal",
    "NotEqual",
    "Greater",
    "Less",
    "GreaterOrEqual",
    "LessOrEqual",
    };
    #endregion

    #region Style constants
    /// <summary>
    /// Represents Arial font name.
    /// </summary>
    private const string DEF_FONT_NAME = "Arial";
    /// <summary>
    /// Represents none style.
    /// </summary>
    private const string DEF_STYLE_NONE = "None";
    /// <summary>
    /// Represents default font size.
    /// </summary>
    public const int DEF_FONT_SIZE = 8;
    /// <summary>
    /// Represents left diagonal border index.
    /// </summary>
    private const int DEF_LEFT_DIAGONAL_BORDER = 5;
    /// <summary>
    /// Represents right diagonal border index.
    /// </summary>
    private const int DEF_RIGHT_DIAGONAL_BORDER = 6;
    /// <summary>
    /// Represents zero constant.
    /// </summary>
    public const int DEF_STYLE_ZERO = 0;
    /// <summary>
    /// Represents rotation constant.
    /// </summary>
    public const int DEF_STYLE_ROTATION = 90;
    /// <summary>
    /// Represents default style font size.
    /// </summary>
    private const int DEF_STYLE_FONT_SIZE = 10;
    /// <summary>
    /// Represents default rotation text.
    /// </summary>
    public const int DEF_ROTATION_TEXT = 255;
    /// <summary>
    /// Represents default border increment.
    /// </summary>
    private const int DEF_BORDER_INCR = 5;
    /// <summary>
    /// Represents default style name.
    /// </summary>
    public const string DEF_STYLE_NAME = "Default";
    /// <summary>
    /// Represents unique prephix.
    /// </summary>
    private const string DEF_UNIQUE_STRING = "s";
    /// <summary>
    /// Represents style align none.
    /// </summary>
    private const string DEF_STYLE_ALIGN_NONE = "None";
    /// <summary>
    /// Represents style align Subscript.
    /// </summary>
    private const string DEF_STYLE_ALIGN_SUBSCRIPT = "Subscript";
    /// <summary>
    /// Represents style align Superscript.
    /// </summary>
    private const string DEF_STYLE_ALIGN_SUPERSCRIPT = "Superscript";
    /// <summary>
    /// Represents border position string.
    /// </summary>
    public static readonly string[] DEF_BORDER_POSITION_STRING =
    {
      "",
      "",
      "",
      "",
      "",
      "DiagonalLeft",
      "DiagonalRight",
      "Left",
      "Top",
      "Bottom",
      "Right"
    };
    /// <summary>
    /// Represents border line type string.
    /// </summary>
    public static readonly string[] DEF_BORDER_LINE_TYPE_STRING =
    {
      "None",
      "1 Continuous",
      "2 Continuous",
      "1 Dash",
      "1 Dot",
      "3 Continuous",
      "3 Double",
      "Continuous",
      "2 Dash",
      "1 DashDot",
      "2 DashDot",
      "1 DashDotDot",
      "2 DashDotDot",
      "2 SlantDashDot"
    };
    #endregion

    #region Worksheet option constants
    /// <summary>
    /// Represents default margin.
    /// </summary>
    private const double DEF_MARGIN = 0.5;
    /// <summary>
    /// Represents default page scale.
    /// </summary>
    private const int DEF_SCALE = 100;
    /// <summary>
    /// Represents default page fit.
    /// </summary>
    private const int DEF_FIT = 1;
    /// <summary>
    /// Represents default zoom.
    /// </summary>
    private const int DEF_ZOOM = 100;
    /// <summary>
    /// Represents print location string.
    /// </summary>
    public static readonly string[] DEF_PRINT_LOCATION_STRING =
    {
      "InPlace",
      "NoComments",
      "SheetEnd"
    };
    /// <summary>
    /// Represents print error string.
    /// </summary>
    public static readonly string[] DEF_PRINT_ERROR_STRING =
    {
      "none",
      "Blank",
      "Dash",
      "NA"
    };
    /// <summary>
    /// Represents visibility string.
    /// </summary>
    public static readonly string[] DEF_VISIBLE_STRING =
    {
      "error",
      "SheetHidden",
      "SheetVeryHidden"
    };
    #endregion

    /// <summary>
    /// Represents xml true string.
    /// </summary>
    private const string DEF_XML_TRUE = "1";
    /// <summary>
    /// Represents xml false string.
    /// </summary>
    private const string DEF_XML_FALSE = "0";
    /// <summary>
    /// Represents All autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_ALL_TYPE = "All";
    /// <summary>
    /// Represents color string prefix.
    /// </summary>
    public const string DEF_COLOR_STRING = "#";
    /// <summary>
    /// Represents Bottom autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_BOTTOM_TYPE = "Bottom";
    /// <summary>
    /// Represents Top autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_TOP_TYPE = "Top";
    /// <summary>
    /// Represents Percent autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_PERCENT_TYPE = "Percent";
    /// <summary>
    /// Represents blanks autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_BLANKS_TYPE = "Blanks";
    /// <summary>
    /// Represents Custom autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_CUSTOM_TYPE = "Custom";
    /// <summary>
    /// Represents non blanks autofilter type.
    /// </summary>
    private const string DEF_AUTOFILTER_NON_BLANKS_TYPE = "NonBlanks";
    /// <summary>
    /// Represents default column width.
    /// </summary>
    private const double DEF_COLUMN_WIDTH = 48;
    /// <summary>
    /// Represents default row Height.
    /// </summary>
    public const double DEF_ROW_HEIGHT = 12.75;
    /// <summary>
    /// Represents default column div.
    /// </summary>
    public const double DEF_COLUMN_DIV = 256.0;
    /// <summary>
    /// Represents default row div.
    /// </summary>
    public const double DEF_ROW_DIV = 20.0;
    /// <summary>
    /// Represents data time mask.
    /// </summary>
    private const string DEF_DATATIME_MASK = "yyyy-MM-ddTHH:mm:ss";
    /// <summary>
    /// Represents default merged style first index.
    /// </summary>
    private const int DEF_MERGED_STYLE = 5000;
    /// <summary>
    /// Represents cell types.
    /// </summary>
    public enum XmlSerializationCellType
    {
      /// <summary>
      /// Represents number.
      /// </summary>
      Number,
      /// <summary>
      /// Represents dateTime.
      /// </summary>
      DateTime,
      /// <summary>
      /// Represents boolean.
      /// </summary>
      Boolean,
      /// <summary>
      /// Represents string.
      /// </summary>
      String,
      /// <summary>
      /// Represents error.
      /// </summary>
      Error
    }
    /// <summary>
    /// Represents pattern string.
    /// </summary>
    public static readonly string[] DEF_PATTERN_STRING =
    {
      "None",
      "Solid",
      "Gray50",
      "Gray75",
      "Gray25",
      "HorzStripe",
      "VertStripe",
      "ReverseDiagStripe",
      "DiagStripe",
      "DiagCross",
      "ThickDiagCross",
      "ThinHorzStripe",
      "ThinVertStripe",
      "ThinReverseDiagStripe",
      "ThinDiagStripe",
      "ThinHorzCross",
      "ThinDiagCross",
      "Gray125",
      "Gray0625",
    };
    /// <summary>
    /// Represents Autofilter operation string.
    /// </summary>
    private readonly string[] DEF_AUTOFILTER_OPERATION_STRING =
    {
      "",
      "LessThan",
      "Equals",
      "LessThanOrEqual",
      "GreaterThan",
      "DoesNotEqual",
      "GreaterThanOrEqual"
    };
    /// <summary>
    /// Represents the ErrorStyle Options
    /// </summary>
    public static readonly string[] DEF_ERRORSTYLE =
    {
      "Stop",
      "Warn",
      "Info",
    };
    /// <summary>
    /// Represents the ErrorStyle Options
    /// </summary>
    public static readonly string[] DEF_ALLOWTYPE_STRING =
    {
      "Any",
      "Whole",
      "Decimal",
      "List",
      "Date",
      "Time",
      "TextLength",
      "Custom",
    };
    /// <summary>
    /// Represents 13/10 chart.
    /// </summary>
    private const string DEF_10_CHAR = "&#10;";
    /// <summary>
    /// Represents bad reference.
    /// </summary>
    public const string DEF_BAD_REF = "#REF";
    /// <summary>
    /// Represents bad reference in xml.
    /// </summary>
    public const string DEF_BAD_REF_UPDATE = "#REF!";
    /// <summary>
    /// Represents bad formula reference.
    /// </summary>
    public const string DEF_BAD_FORMULA = "=#REF!";
    /// <summary>
    /// Represents default maximum column index.
    /// </summary>
    public const int DEF_MAX_COLUMN = 256;
    /// <summary>
    /// Represents default minimum column index.
    /// </summary>
    public const int DEF_MIN_COLUMN = 0;
    /// <summary>
    /// Represents default cod for generate unique style index.
    /// </summary>
    [CLSCompliant( false )]
    public const long DEF_MERGE_COD = 10000000000;
    #endregion

    #region Class members
    /// <summary>
    /// Represents hash table, that contain merged styles.
    /// </summary>
    private Dictionary<long, int> m_mergeStyles = new Dictionary<long, int>();
    /// <summary>
    /// Builder used to accumulate start of the xml string.
    /// </summary>
    private StringBuilder m_builderStart;
    /// <summary>
    /// Builder used to accumulate end of the xml string.
    /// </summary>
    private StringBuilder m_builderEnd;
    private FormulaUtil m_formulaUtil;
    #endregion

    #region Class static methods
    /// <summary>
    /// Gets unique id for coding sheet index and cell index.
    /// </summary>
    /// <param name="iSheetIndex">Represents sheet index.</param>
    /// <param name="lCellIndex">Represents cell index.</param>
    /// <returns>Returns unique id.</returns>
    public static long GetUniqueID( int iSheetIndex, long lCellIndex )
    {
      long lRow = RangeImpl.GetRowFromCellIndex( lCellIndex );
      long lColumn = RangeImpl.GetColumnFromCellIndex( lCellIndex );

      return ( lRow << 32 ) + ( lColumn << 16 ) + iSheetIndex;
    }
    /// <summary>
    /// Gets sheet index by unique id.
    /// </summary>
    /// <param name="lUniqueId">Represents unique id.</param>
    /// <returns>Returns sheet index.</returns>
    public static int GetSheetIndexByUniqueId( long lUniqueId )
    {
      return ( int )( lUniqueId & 0xFFFF );
    }
    /// <summary>
    /// Gets cell index by unique id.
    /// </summary>
    /// <param name="lUniqueId">Represents unique id.</param>
    /// <returns>Returns cell index.</returns>
    public static long GetCellIndexByUniqueId( long lUniqueId )
    {
      //return ( int )( lUniqueId & 0xffffffff );
      int iRow = ( int )( ( lUniqueId >> 32 ) & 0xFFFFFFFF );
      int iColumn = ( int )( ( lUniqueId >> 16 ) & 0xFFFF );
      return RangeImpl.GetCellIndex( iColumn, iRow );
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the WorkbookXmlSerializator class.
    /// </summary>
    public WorkbookXmlSerializator()
    {
    }
    #endregion

    #region Xml serialization methods

    #region Named range serialization methods
    /// <summary>
    /// Serialize named range collection.
    /// </summary>
    /// <param name="writer">Xml write stream.</param>
    /// <param name="names">Names collection.</param>
    /// <param name="isLocal">If true - serializes only local names.</param>
    private void SerializeNames( XmlWriter writer, INames names, bool isLocal )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( names == null )
        throw new ArgumentNullException( "names" );

      int iLen = names.Count;

      if( iLen == 0 )
        return;

      writer.WriteStartElement( DEF_SS_PREF, DEF_NAMES_PREF, null );

      for( int i = 0; i < iLen; i++ )
      {
        IName name = names[ i ];

        if( name.IsLocal == isLocal )
          SerializeName( writer, name );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize named range.
    /// </summary>
    /// <param name="writer">Xml write stream.</param>
    /// <param name="name">Named range.</param>
    private void SerializeName( XmlWriter writer, IName name )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( name == null )
        throw new ArgumentNullException( "name" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_NAMEDRANGE_PREF, null );

      writer.WriteAttributeString( DEF_SS_PREF, DEF_NAME_PREF, null, name.Name );

      string strName = name.RefersToR1C1;
      string strValue = ( strName != null && strName.Length > 0 ) ? strName : DEF_BAD_FORMULA;

      if( strValue.IndexOf( DEF_BAD_REF ) != -1 )
        strValue = DEF_BAD_FORMULA;

      writer.WriteAttributeString( DEF_SS_PREF, DEF_REFERSTO_PREF, null, strValue );

      if( !name.Visible )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_HIDDEN_PREF, null, DEF_XML_TRUE );

      writer.WriteEndElement();
    }
    #endregion

    #region Style serialization methods
    /// <summary>
    /// Serialize styles.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="extends">Extended format collection.</param>
    /// <param name="listToReparse">List that contain XFFormats to reparse.</param>
    private void SerializeStyles( XmlWriter writer, ExtendedFormatsCollection extends,
      List<ExtendedFormatImpl> listToReparse )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( extends == null )
        throw new ArgumentNullException( "styles" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_STYLES_PREF, null );

      for( int i = 0, iLen = extends.Count; i < iLen; i++ )
      {
        ExtendedFormatImpl format = extends[ i ];
        int iParentIndex = format.ParentIndex;

        if( format.HasParent && iParentIndex > format.XFormatIndex )
        {
          listToReparse.Add( format );
          continue;
        }

        SerializeStyle( writer, format );
      }

      if( listToReparse.Count > 0 )
        ReSerializeStyle( writer, listToReparse );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize style element.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="format">Extended format to serialize.</param>
    private void SerializeStyle( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_STYLE_PREF, null );
      bool bSetParent = format.HasParent && format.ParentIndex != DEF_STYLE_ZERO;

      string strStyleName = ( format.XFormatIndex == DEF_STYLE_ZERO ) ? DEF_STYLE_NAME
        : DEF_UNIQUE_STRING + format.XFormatIndex.ToString();
      
      writer.WriteAttributeString( DEF_SS_PREF, DEF_ID_PREF, null, strStyleName );
      
      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL && !bSetParent )
      {
        StylesCollection styles = ( StylesCollection )format.Workbook.Styles;
        StyleImpl style = styles.GetByXFIndex( format.Index );
        string strName = ( style != null ) ? style.Name : null;

        if( strName != null && strName.Length > 0 )
        {
          writer.WriteAttributeString( DEF_SS_PREF, DEF_NAME_PREF, null, strName );
          bSetParent = false;
        }
      }

      if( bSetParent )
      {
        writer.WriteAttributeString( DEF_SS_PREF, DEF_PARENT_PREF, null, DEF_UNIQUE_STRING
          + format.ParentIndex.ToString() );
      }

      SerializeStyleElements( writer, format );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize style elements.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="format">Extended format to serialize.</param>
    private void SerializeStyleElements( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      bool bHasParent = format.HasParent;

      if( format.IncludeFont )
        SerializeFont( writer, format.Font );

      if( format.IncludeProtection )
        SerializeProtection( writer, format );

      if( format.IncludeAlignment )
        SerializeAlignment( writer, format );

      if( format.IncludeNumberFormat )
        SerializeNumberFormat( writer, format.NumberFormat );

      if( format.IncludePatterns )
        SerializeInterior( writer, format );

      if( format.IncludeBorder )
        SerializeBorders( writer, format.Borders );
    }
    /// <summary>
    /// Serialize fonts.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="font">Font to serialize.</param>
    private void SerializeFont( XmlWriter writer, IFont font )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( font == null )
        throw new ArgumentNullException( "font" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_FONT_PREF, null );

      if( font.Bold )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_BOLD_PREF, null, DEF_XML_TRUE );

      if( font.FontName != DEF_FONT_NAME )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_FONTNAME_PREF, null, font.FontName );

      writer.WriteAttributeString( DEF_SS_PREF, DEF_COLOR_PREF, null, GetColorString( font.RGBColor ) );

      if( font.Italic )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_ITALIC_PREF, null, DEF_XML_TRUE );

      if( font.MacOSOutlineFont )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_OUTLINE_PREF, null, DEF_XML_TRUE );

      if( font.MacOSShadow )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_SHADOW_PREF, null, DEF_XML_TRUE );

      if( font.Size != DEF_STYLE_FONT_SIZE )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_SIZE_PREF, null, XmlConvert.ToString( font.Size ) );

      if( font.Strikethrough )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_STRIKETHROUGH_PREF, null, DEF_XML_TRUE );

      if( font.Underline != ExcelUnderline.None )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_UNDERLINE_PREF, null, font.Underline.ToString() );

      string strFontAlign = GetStyleFontAlign( font );

      if( strFontAlign != DEF_STYLE_NONE )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_VERTICAL_ALIGN_PREF, null, strFontAlign );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize protection object.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="format">Extended format.</param>
    private void SerializeProtection( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_PROTECTION_PREF, null );

      if( !format.Locked )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_PROTECTED_PREF, null, DEF_XML_FALSE );

      if( format.FormulaHidden )
        writer.WriteAttributeString( DEF_X_PREF, DEF_HIDEFORMULA_PREF, null, DEF_XML_TRUE );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize alignment object.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="format">Extended format.</param>
    private void SerializeAlignment( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_ALIGNMENT_PREF, null );

      if( format.HorizontalAlignment != ExcelHAlign.HAlignGeneral )
      {
        string strHAling = GetStyleHAlignString( format.HorizontalAlignment );
        writer.WriteAttributeString( DEF_SS_PREF, DEF_HORIZONTAL_PREF, null, strHAling );
      }

      if( format.IndentLevel != 0 )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_INDENT_PREF, null, format.IndentLevel.ToString() );

      if( format.ReadingOrder != ExcelReadingOrderType.Context )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_READINGORDER_PREF, null, format.ReadingOrder.ToString() );

      int rotation = format.Rotation;

      if( rotation != 0 )
      {
        if( rotation != DEF_ROTATION_TEXT )
        {
          rotation = ( rotation > DEF_STYLE_ROTATION ) ? ( DEF_STYLE_ROTATION - rotation ) : rotation;
          writer.WriteAttributeString( DEF_SS_PREF, DEF_ROTATE_PREF, null, rotation.ToString() );
        }
        else
        {
          writer.WriteAttributeString( DEF_SS_PREF, DEF_VERTICALTEXT_PREF, null, DEF_XML_TRUE );
        }
      }

      if( format.ShrinkToFit )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_SHRINKTOFIT_PREF, null, DEF_XML_TRUE );

      string strVAlign = GetStyleVAlignString( format.VerticalAlignment );
      writer.WriteAttributeString( DEF_SS_PREF, DEF_VERTICAL_PREF, null, strVAlign );

      if( format.WrapText )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_WRAPTEXT_PREF, null, DEF_XML_TRUE );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize number format.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="strNumber">Represents number format.</param>
    private void SerializeNumberFormat( XmlWriter writer, string strNumber )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strNumber == null )
        throw new ArgumentNullException( "strNumber" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_NUMBERFORMAT_PREF, null );

      writer.WriteAttributeString( DEF_SS_PREF, DEF_FORMAT_PREF, null, strNumber );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize interior object.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="format">Extendet format.</param>
    private void SerializeInterior( XmlWriter writer, ExtendedFormatImpl format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_INTERIOR_PREF, null );

      if( !format.IsDefaultColor )
      {
        string strColor = GetColorString( format.Color );
        writer.WriteAttributeString( DEF_SS_PREF, DEF_COLOR_PREF, null, strColor );
      }

      if( !format.IsDefaultPatternColor )
      {
        string strColor = GetColorString( format.PatternColor );
        writer.WriteAttributeString( DEF_SS_PREF, DEF_PATTERNCOLOR_PREF, null, strColor );
      }

      if( format.FillPattern != ExcelPattern.None )
      {
        string strPatern = DEF_PATTERN_STRING[ ( int )format.FillPattern ];
        writer.WriteAttributeString( DEF_SS_PREF, DEF_PATTERN_PREF, null, strPatern );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize Borders collection.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="borders">Borders collection.</param>
    private void SerializeBorders( XmlWriter writer, IBorders borders )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( borders == null )
        throw new ArgumentNullException( "borders" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_BORDERS_PREF, null );

      ExcelBordersIndex[] arrBorders = //( ExcelBordersIndex[] )Enum.GetValues( typeof( ExcelBordersIndex ) );
        new ExcelBordersIndex[]
      {
        ExcelBordersIndex.DiagonalDown,
        ExcelBordersIndex.DiagonalUp,
        ExcelBordersIndex.EdgeBottom,
        ExcelBordersIndex.EdgeLeft,
        ExcelBordersIndex.EdgeRight,
        ExcelBordersIndex.EdgeTop,
      };

      for( int i = 0, iLen = arrBorders.Length; i < iLen; i++ )
      {
        ExcelBordersIndex borderType = arrBorders[ i ];
        IBorder border = borders[ borderType ];

        if( border != null )
        {
          int index = ( int )borderType;
          SerializeBorder( writer, border, index );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize Border object.
    /// </summary>
    /// <param name="writer">Xml stream writer.</param>
    /// <param name="border">Border object.</param>
    /// <param name="iBorderIndex">Represents border index.</param>
    private void SerializeBorder( XmlWriter writer, IBorder border, int iBorderIndex )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( border == null )
        throw new ArgumentNullException( "border" );

      if( ( iBorderIndex == DEF_LEFT_DIAGONAL_BORDER || iBorderIndex == DEF_RIGHT_DIAGONAL_BORDER )
        && !border.ShowDiagonalLine )
      {
        return;
      }

      writer.WriteStartElement( DEF_SS_PREF, DEF_BORDER_PREF, null );

      string strPosition = DEF_BORDER_POSITION_STRING[ iBorderIndex ];
      writer.WriteAttributeString( DEF_SS_PREF, DEF_POSITION_PREF, null, strPosition );

      string strColor = GetColorString( border.ColorRGB );
      writer.WriteAttributeString( DEF_SS_PREF, DEF_COLOR_PREF, null, strColor );

      if( border.LineStyle != ExcelLineStyle.None )
      {
        string strLine = DEF_BORDER_LINE_TYPE_STRING[ ( int )border.LineStyle ];
        int iIndex = strLine.IndexOf( " " );
        string strStyleline = ( iIndex != -1 ) ? strLine.Substring( iIndex + 1 ) : strLine;

        writer.WriteAttributeString( DEF_SS_PREF, DEF_LINE_STYLE_PREF, null, strStyleline );

        if( iIndex != -1 )
          writer.WriteAttributeString( DEF_SS_PREF, DEF_WEIGHT_PREF, null, strLine.Substring( 0, iIndex ) );
      }

      writer.WriteEndElement();
    }
    #endregion

    #region AutoFilter serialize methods
    /// <summary>
    /// Serialize autofilters.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="autofilters">Auto filter collection.</param>
    private void SerializeAutoFilters( XmlWriter writer, IAutoFilters autofilters )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autofilters == null )
        throw new ArgumentNullException( "autofilters" );

      writer.WriteStartElement( DEF_X_PREF, DEF_AUTOFILTER_PREF, null );

      writer.WriteAttributeString( DEF_X_PREF, DEF_RANGE_PREF, null, ( ( AutoFiltersCollection )autofilters ).AddressR1C1 );

      for( int i = 0, iLen = autofilters.Count; i < iLen; i++ )
      {
        IAutoFilter filter = autofilters[ i ];

        if( filter.IsFiltered )
          SerializeAutoFilter( writer, autofilters[ i ] );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize autofilter.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="autofilter">Auto filter object.</param>
    private void SerializeAutoFilter( XmlWriter writer, IAutoFilter autofilter )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( autofilter == null )
        throw new ArgumentNullException( "autofilter" );

      writer.WriteStartElement( DEF_X_PREF, DEF_AUTOFILTERCOLUMN_PREF, null );

      AutoFilterImpl filter = ( AutoFilterImpl )autofilter;

      writer.WriteAttributeString( DEF_X_PREF, DEF_INDEX_PREF, null, filter.Index.ToString() );

      if( filter.IsTop10 )
      {
        string str = filter.IsTop ? DEF_AUTOFILTER_TOP_TYPE : DEF_AUTOFILTER_BOTTOM_TYPE;

        if( filter.IsPercent )
          str += DEF_AUTOFILTER_PERCENT_TYPE;

        writer.WriteAttributeString( DEF_X_PREF, DEF_VALUE_PREF, null, filter.Top10Number.ToString() );
        writer.WriteAttributeString( DEF_X_PREF, DEF_TYPE_PREF, null, str );
      }

      if( filter.IsBlanks )
        writer.WriteAttributeString( DEF_X_PREF, DEF_TYPE_PREF, null, DEF_AUTOFILTER_BLANKS_TYPE );

      if( filter.IsNonBlanks )
        writer.WriteAttributeString( DEF_X_PREF, DEF_TYPE_PREF, null, DEF_AUTOFILTER_NON_BLANKS_TYPE );

      if (filter.IsFirstCondition && filter.FirstCondition.DataType != ExcelFilterDataType.MatchAllNonBlanks )
      {
        writer.WriteAttributeString( DEF_X_PREF, DEF_TYPE_PREF, null, DEF_AUTOFILTER_CUSTOM_TYPE );
        SerializeAFCondition( writer, filter.FirstCondition );
      }

      if (filter.IsSecondCondition && filter.SecondCondition.DataType != ExcelFilterDataType.MatchAllNonBlanks )
      {
        string strAndOr = ( filter.IsAnd ) ? DEF_AUTOFILTERAND_PREF : DEF_AUTOFILTEROR_PREF;
        writer.WriteStartElement( DEF_X_PREF, strAndOr, null );

        SerializeAFCondition( writer, filter.SecondCondition );

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize autofilter condition.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="condition">Condition object.</param>
    private void SerializeAFCondition( XmlWriter writer, IAutoFilterCondition condition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );
      
      if( condition == null )
        throw new ArgumentNullException( "condition" );
      
      writer.WriteStartElement( DEF_X_PREF, DEF_AUTOFILTERCONDITION_PREF, null );
      
      string strOperation = DEF_AUTOFILTER_OPERATION_STRING[ ( int )condition.ConditionOperator ];
      string strValue = GetAutoFilterConditionValue( condition );

      writer.WriteAttributeString( DEF_X_PREF, DEF_OPERATOR_PREF, null, strOperation );
      writer.WriteAttributeString( DEF_X_PREF, DEF_VALUE_PREF, null, strValue );
      
      writer.WriteEndElement();
    }
    #endregion

    #region Worksheet serialization mehtods
    /// <summary>
    /// Serialize single cell.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Current sheet.</param>
    /// <param name="iRowIndex">Row index of current cell.</param>
    private void SerializeCell( XmlWriter writer, WorksheetImpl sheet, int iRowIndex )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      CellRecordCollection cells = sheet.CellRecords;
      HyperLinksCollection hyperLinks = ( HyperLinksCollection )sheet.HyperLinks;
      MergeCellsImpl mergeCells = sheet.MergeCells;
      int iPreviousIndex = 0;

      for( int i = sheet.FirstColumn, iLen = sheet.LastColumn; i <= iLen; i++ )
      {
        long index = RangeImpl.GetCellIndex( i, iRowIndex );

        Rectangle leftTopCell = mergeCells.GetLeftTopCell( new Rectangle( i - 1, iRowIndex - 1, 0, 0) );
        long lLeftTopCellIndex = RangeImpl.GetCellIndex( leftTopCell.X + 1, leftTopCell.Y + 1 );
        
        bool bMerge = lLeftTopCellIndex == index;
        MergeRegion region = mergeCells.FindMergedRegion( new Rectangle( i - 1, iRowIndex - 1, 0, 0 ) );
        bool bData = cells.Contains( index ) && !( region != null && !bMerge );
        bool bComent = sheet.Comments[ iRowIndex, i ] != null;

        if( bData  || bComent || bMerge )
        {
          writer.WriteStartElement( DEF_CELL_PREF, null );

          if( iPreviousIndex + 1 != i )
            writer.WriteAttributeString( DEF_SS_PREF, DEF_INDEX_PREF, null, i.ToString() );

          iPreviousIndex = i;

          bool bFormatted = DisableFormatting( writer );

          SerializeHyperlink( writer, index, hyperLinks );
          SerializeCellStyle( writer, index, bMerge, cells, sheet );
          SerializeMerge( writer, iRowIndex, i, mergeCells, bMerge );

          if( bData  && !( cells.GetCellRecord( index ).TypeCode == TBIFFRecord.Blank ) )
            SerializeData( writer, cells, index );
          
          if( bComent )
            SerializeComment( writer, sheet.Comments[ iRowIndex, i ], sheet.ParentWorkbook.InnerFonts, cells.GetCellFont( index ) );

          writer.WriteEndElement();
          EnableFormatting( writer, bFormatted );
        }
      }
    }
    /// <summary>
    /// Serialize merge region if necessary.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="iRowIndex"></param>
    /// <param name="i"></param>
    /// <param name="mergeCells"></param>
    /// <param name="bMerge"></param>
    private void SerializeMerge( XmlWriter writer, int iRowIndex, int i, MergeCellsImpl mergeCells, bool bMerge )
    {
      if( bMerge )
      {
        Rectangle rect = Rectangle.FromLTRB( i - 1, iRowIndex - 1, i - 1, iRowIndex - 1 );
        SerializeMergedRange( writer, mergeCells[ rect ] );
      }
    }
    /// <summary>
    /// Serializes hyperlink part of the cell.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="index"></param>
    /// <param name="hyperLinks"></param>
    private void SerializeHyperlink( XmlWriter writer, long index, HyperLinksCollection hyperLinks )
    {
      IHyperLink link = hyperLinks.GetHyperlinkByCellIndex( index );

      if( link != null )
      {
        string strAddres = ( link.Type == ExcelHyperLinkType.Workbook ) ? DEF_COLOR_STRING + link.Address : link.Address;
        writer.WriteAttributeString( DEF_SS_PREF, DEF_HREF_PREF, null, strAddres );

        if( link.ScreenTip != null && link.ScreenTip.Length != 0 )
          writer.WriteAttributeString( DEF_X_PREF, DEF_HYPRER_TIP_PREF, null, link.ScreenTip );
      }
    }
    /// <summary>
    /// Serializes cell style block if necessary.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="index"></param>
    /// <param name="bMerge"></param>
    /// <param name="cells"></param>
    /// <param name="sheet"></param>
    private void SerializeCellStyle( XmlWriter writer, long index, bool bMerge, CellRecordCollection cells, WorksheetImpl sheet )
    {
      int iExtFormat = cells.GetExtendedFormatIndex( index );

      if( bMerge )
      {
        long lMergeKey = GetUniqueID( sheet.Index, index );
        iExtFormat = m_mergeStyles[ lMergeKey ];
      }

      WorkbookImpl book = sheet.ParentWorkbook;

      if( iExtFormat != book.DefaultXFIndex && iExtFormat != DEF_STYLE_ZERO && iExtFormat != int.MinValue )
      {
        writer.WriteAttributeString( DEF_SS_PREF, DEF_STYLEID_PREF, null
          , DEF_UNIQUE_STRING + iExtFormat.ToString() );
      }
    }
    /// <summary>
    /// Enables formatting.
    /// </summary>
    /// <param name="writer">XmlWriter to enable formatting.</param>
    /// <param name="bFormatted"></param>
    private void EnableFormatting( XmlWriter writer, bool bFormatted )
    {
      if( bFormatted )
      {
#if  (SILVERLIGHT) || (WINRT) || (WP)
#if !(WINRT )
        writer.Settings.Indent = true;
#endif
#else
        XmlTextWriter writer2 = writer as XmlTextWriter;

        if( writer2 != null )
        {
          writer2.Formatting = Formatting.Indented;
        }
#endif
      }
    }

    private bool DisableFormatting( XmlWriter writer )
    {
      bool result = false;
#if  (SILVERLIGHT || WP)
      result = writer.Settings.Indent;
      writer.Settings.Indent = false;
#elif ( WINRT )
        result=writer.Settings.Indent;
#else 
      XmlTextWriter writer2 = writer as XmlTextWriter;

      if( writer2 != null )
      {
        result = writer2.Formatting != Formatting.None;
        writer2.Formatting = Formatting.None;
      }
#endif

      return result;
    }
    /// <summary>
    /// Serialize Data object.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="cells">Cells collection.</param>
    /// <param name="index">Index of cell.</param>
    private void SerializeData( XmlWriter writer, CellRecordCollection cells, long index )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      NumberFormatInfo numberInfo = CultureInfo.InvariantCulture.NumberFormat;
      string strFormula = cells.GetFormula( index, true, numberInfo );

      XmlSerializationCellType xmlType;
      string value;
      IStyle style = null;
      TextWithFormat rtf = null;
      WorksheetImpl sheet = ( WorksheetImpl )cells.Sheet;

      if( strFormula != null && strFormula.Length > 0 )
      {
        strFormula = UpdateFormulaError( strFormula );
        writer.WriteAttributeString( DEF_SS_PREF, DEF_FORMULA_PREF, null, strFormula );
        xmlType = GetFormulaType( sheet, index, out value );
      }
      else
      {
        value = GetCellTypeValue( cells, index, out xmlType );

        if( xmlType == XmlSerializationCellType.String )
        {
          style = cells.GetCellStyle( index );
          rtf = sheet.GetTextWithFormat( index );
        }
      }

      if( strFormula != DEF_BAD_FORMULA )
        SerializeData( writer, xmlType, value, style, rtf, cells, index );
    }
    /// <summary>
    /// Gets type of the formula value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private XmlSerializationCellType GetFormulaType( IWorksheet sheet, long index, out string value )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( index );
      int iColumn = RangeImpl.GetColumnFromCellIndex( index );

      IRange range = sheet[ iRow, iColumn ];
      XmlSerializationCellType result;

      if( range.HasFormulaBoolValue )
      {
        result = XmlSerializationCellType.Boolean;
        value = XmlConvert.ToString( range.FormulaBoolValue );
      }
      else if( range.HasFormulaDateTime )
      {
        result = XmlSerializationCellType.DateTime;
        value = XmlConvert.ToString( range.FormulaDateTime, DEF_DATATIME_MASK );
      }
      else if( range.HasFormulaErrorValue )
      {
        result = XmlSerializationCellType.Error;
        value = range.FormulaErrorValue;
      }
      else if( ( value = range.FormulaStringValue ) != null )
      {
        result = XmlSerializationCellType.String;
      }
      else
      {
        result = XmlSerializationCellType.Number;

        double numberValue = range.FormulaNumberValue;
        value = ( !Double.IsNaN( numberValue ) ) ?
          XmlConvert.ToString( numberValue ) :
          null;
      }

      return result;
    }
    /// <summary>
    /// Serializes Data tag for the single cell.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="cellType">Type of the cell's data.</param>
    /// <param name="value">Cell's value.</param>
    /// <param name="style">Style of the cell.</param>
    /// <param name="rtf">Rtf string containing cell's value if cell is rtf.</param>
    /// <param name="cells">Cells collection.</param>
    /// <param name="cellIndex">Current cell index.</param>
    private void SerializeData( XmlWriter writer,
      XmlSerializationCellType cellType, string value,
      IStyle style, TextWithFormat rtf, CellRecordCollection cells, long cellIndex )
    {
      if( value == null || cellType == XmlSerializationCellType.String && value.Length == 0 )
        return;

      writer.WriteStartElement( DEF_DATA_PREF, null );

      bool bIsRtf = false;

      writer.WriteAttributeString( DEF_SS_PREF, DEF_TYPE_PREF, null, cellType.ToString() );

      if( cellType == XmlSerializationCellType.String && value.Length != 0 )
      {
        if( style != null && style.IsFirstSymbolApostrophe )
          writer.WriteAttributeString( DEF_X_PREF, DEF_TICKED_PREF, null, DEF_XML_TRUE );

        if( rtf != null && rtf.FormattingRunsCount != 0 )
        {
          WorksheetImpl sheet = ( WorksheetImpl )cells.Sheet;
          writer.WriteAttributeString( DEF_XMLNS_PREF, null, null, DEF_HTML_NAMESPACE );
          IFont defaultFont = cells.GetCellFont( cellIndex );
          FontsCollection fonts = sheet.ParentWorkbook.InnerFonts;
          SerializeRichText( writer, rtf, value, fonts, defaultFont );
          bIsRtf = true;
        }
      }

      if( !bIsRtf )
      {
        if( cellType == XmlSerializationCellType.Boolean )
          value = XmlConvert.ToBoolean( value ) ? "1" : "0";

        writer.WriteString( value );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize worksheets collection.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="worksheets">Worksheets collection.</param>
    private void SerializeWorksheets( XmlWriter writer, IWorksheets worksheets )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( worksheets == null )
        throw new ArgumentNullException( "worksheets" );

      for( int i = 0, len = worksheets.Count; i < len; i++ )
      {
        SerializeWorksheet( writer, ( WorksheetImpl )worksheets[ i ] );
      }
    }
    /// <summary>
    /// Serialize custom worksheet.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Worksheet object.</param>
    private void SerializeWorksheet( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_WORKSHEET_PREF, null );
      writer.WriteAttributeString( DEF_SS_PREF, DEF_NAME_PREF, null, sheet.Name );

      if( sheet.IsRightToLeft )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_RIGHTTOLEFT_PREF, null, DEF_XML_TRUE );

      IAutoFilters autofilters = sheet.AutoFilters;
      
      DataValidationTable dv = sheet.DVTable;
      INames names = sheet.Names;

      if( names.Count > 0 )
        SerializeNames( writer, names, true );

      SerializeTable( writer, sheet );

      if( autofilters.Count > 0 )
        SerializeAutoFilters( writer, autofilters );

      if (dv.Count > 0)
          SerializeDataValidations(writer, dv);

      WorksheetConditionalFormats cf = sheet.ConditionalFormats;
      if( cf.Count > 0 )
        SerializeConditionFormats( writer, cf );

      SerializeWorksheetOption( writer, sheet );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize custom worksheet.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeTable( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_TABLE_PREF, null );

      if( sheet.StandardHeight != DEF_ROW_HEIGHT )
      {
        writer.WriteAttributeString( DEF_SS_PREF, DEF_DEFAULTROWHEIGHT_PREF, null
          , XmlConvert.ToString( sheet.StandardHeight ) );
      }

      float dWidth = ( float )sheet.Application.ConvertUnits( sheet.ColumnWidthToPixels( sheet.StandardWidth )
        , MeasureUnits.Pixel, MeasureUnits.Point );

      if( dWidth != DEF_COLUMN_WIDTH )
      {
        writer.WriteAttributeString( DEF_SS_PREF, DEF_DEFAULTCOLUMNWIDTH_PREF, null, XmlConvert.ToString( dWidth ) );
      }
      
      SerializeColumns( writer, sheet );
      SerializeRows( writer, sheet );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize column object.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="worksheet">Current sheet.</param>
    private void SerializeColumns( XmlWriter writer, WorksheetImpl worksheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( worksheet == null )
        throw new ArgumentNullException( "worksheet" );

      ColumnInfoRecord[] arrColumns = worksheet.ColumnInformation;

      for( int i = 1; i <= arrColumns.Length - 1; i++ )
      {
        ColumnInfoRecord record = arrColumns[ i ];

        if( record == null ) continue;

        int iColumnIndex = record.FirstColumn + 1;

        if( iColumnIndex > DEF_MAX_COLUMN )
          continue;

        writer.WriteStartElement( DEF_SS_PREF, DEF_COLUMN_PREF, null );
        writer.WriteAttributeString( DEF_SS_PREF, DEF_INDEX_PREF, null, iColumnIndex.ToString() );

        float dColumnWidth = worksheet.ColumnWidthToPixels( record.ColumnWidth / DEF_COLUMN_DIV );
        int iSpan = record.LastColumn - record.FirstColumn;

        SerializeRowColumnCommonAttributes( writer, record, record.LastColumn, worksheet.ParentWorkbook );

        if( iSpan > 0  )
          writer.WriteAttributeString( DEF_SS_PREF, DEF_SPAN_PREF, null, iSpan.ToString() );

        writer.WriteAttributeString( DEF_SS_PREF, DEF_AUTOFIT_WIDTH_PREF, null, DEF_XML_FALSE );

        if( dColumnWidth != worksheet.StandardWidth )
        {
          dColumnWidth = ( float )worksheet.Application.ConvertUnits(  dColumnWidth,
            MeasureUnits.Pixel, MeasureUnits.Point );
          writer.WriteAttributeString( DEF_SS_PREF, DEF_WIDTH_PREF, null, XmlConvert.ToString( dColumnWidth ) );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serialize rows collection.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Parent worksheet object.</param>
    private void SerializeRows( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      //IDictionary dic = sheet.RowInformation;
      CellRecordCollection cells = sheet.CellRecords;
      int iPreviousIndex = 0;

      for( int i = sheet.FirstRow, iLen = sheet.LastRow; i <= iLen; i++ )
      {
        if( cells.ContainsRow( i - 1 ) )//|| dic.Contains( i ) )
        {
          writer.WriteStartElement( DEF_ROW_PREF );

          if( iPreviousIndex + 1 != i )
            writer.WriteAttributeString( DEF_SS_PREF, DEF_INDEX_PREF, null, i.ToString() );

          iPreviousIndex = i;
          RowStorage rowStorage = ( RowStorage )cells.Table.Rows[ i - 1 ];

          if( rowStorage != null )
          {
            writer.WriteAttributeString( DEF_SS_PREF, DEF_HEIGHT_PREF, null,
              XmlConvert.ToString( rowStorage.Height / DEF_ROW_DIV ) );

            writer.WriteAttributeString( DEF_SS_PREF, DEF_AUTOFIT_HEIGHT_PREF, null, DEF_XML_FALSE );

            SerializeRowColumnCommonAttributes( writer, rowStorage, rowStorage.LastColumn, sheet.ParentWorkbook );
          }

          SerializeCell( writer, sheet, i );

          writer.WriteEndElement();
        }
      }
    }
    /// <summary>
    /// Serialize row and column common attributes.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="record">Row or ColumnInfo record.</param>
    /// <param name="iLastIndex">Last column or row index.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeRowColumnCommonAttributes( XmlWriter writer, IOutline record
      , int iLastIndex, WorkbookImpl book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      int iXFIndex = record.ExtendedFormatIndex;

      if( iXFIndex != book.DefaultXFIndex && iXFIndex != DEF_STYLE_ZERO )
      {
        writer.WriteAttributeString( DEF_SS_PREF, DEF_STYLEID_PREF, null
          , DEF_UNIQUE_STRING + record.ExtendedFormatIndex.ToString() );
      }

      if( record.IsHidden || record.IsCollapsed )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_HIDDEN_PREF, null, DEF_XML_TRUE );
    }
    /// <summary>
    /// Serializes merged range.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="region">Represents merged cells.</param>
    private void SerializeMergedRange( XmlWriter writer, MergeRegion region )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( region == null )
        throw new ArgumentNullException( "region" );

      int iRowCount = region.RowTo - region.RowFrom;
      int iColumnCount = region.ColumnTo - region.ColumnFrom;

      writer.WriteAttributeString( DEF_SS_PREF, DEF_MERGE_DOWN_PREF, null, iRowCount.ToString() );
      writer.WriteAttributeString( DEF_SS_PREF, DEF_MERGE_ACROSS_PREF, null, iColumnCount.ToString() );
    }
    #endregion

    #region Comment serialization methods
    /// <summary>
    /// Serialize comment object.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="comment">comment to serialize.</param>
    /// <param name="fonts">Inner fonts collection.</param>
    /// <param name="defFont">Represents default font.</param>
    private void SerializeComment( XmlWriter writer, IComment comment, FontsCollection fonts, IFont defFont )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( comment == null )
        throw new ArgumentNullException( "comment" );

      if( fonts == null )
        throw new ArgumentNullException( "fonts" );

      writer.WriteStartElement( DEF_SS_PREF, DEF_COMMENT_PREF, null );
      string strAutor = comment.Author;
      string strText = comment.Text;

      if( strAutor.Length != 0 )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_AUTHOR_PREF, null, strAutor );

      if( comment.IsVisible )
        writer.WriteAttributeString( DEF_SS_PREF, DEF_SHOWALWAYS_PREF, null, DEF_XML_TRUE );

      writer.WriteStartElement( DEF_SS_PREF, DEF_DATA_PREF, null );
      writer.WriteAttributeString( DEF_XMLNS_PREF, null, null, DEF_HTML_NAMESPACE );

      if( strText.Length != 0 )
      {
        TextWithFormat format = ( ( RichTextString )comment.RichText ).TextObject;
        SerializeRichText( writer,  format, strText, fonts, defFont );
      }

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize rich text to xml.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="rtf">Represents collection of formats.</param>
    /// <param name="text">Text to serialize.</param>
    /// <param name="fonts">Inner fonts.</param>
    /// <param name="defFont">Represents default font.</param>
    private void SerializeRichText( XmlWriter writer, TextWithFormat rtf
      , string text, FontsCollection fonts, IFont defFont )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( rtf == null )
        throw new ArgumentNullException( "rtf" );

      if( fonts == null )
        throw new ArgumentNullException( "Fonts" );

      if( text.Length == 0 )
        throw new ArgumentNullException( "text" );

      SortedList<int, int> runs = rtf.FormattingRuns;
      IList<int> lstValues = runs.Values;
      IList<int> lstKeys = runs.Keys;
      bool isFormatted = runs.Count > 0;
      if (!isFormatted)
          return;
      int iStrPos = lstKeys[ 0 ];
      int iTextCount = text.Length;

      if( iStrPos != 0 )
        SerializeRtfFont( writer, defFont, text.Substring( 0, iStrPos ) );

      for( int i = 0, iLen = runs.Count; i < iLen; i++ )
      {
        int indexFont = lstValues[ i ];
        int iStrEndPos = lstKeys[ i ];
        IFont font = fonts[ indexFont ];

        iStrEndPos = ( iLen - i == 1 ) ? iTextCount : lstKeys[ i + 1 ];

        SerializeRtfFont( writer, font, text.Substring( iStrPos, iStrEndPos - iStrPos ) );
        iStrPos = iStrEndPos;
      }

      //writer.WriteRaw( result );
    }
    /// <summary>
    /// Serialize rtf font.
    /// </summary>
    /// <param name="writer">Writer to write value into.</param>
    /// <param name="RTFFont">Font for current string value.</param>
    /// <param name="strValue">String value.</param>
    private void SerializeRtfFont( XmlWriter writer, IFont RTFFont, string strValue )
    {
      if( writer == null )
        throw new ArgumentNullException();

      if( RTFFont == null )
        throw new ArgumentNullException( "rtfString" );

      if( strValue.Length == 0 ) return;

      StringBuilder builderStart = GetStartBuilder();
      StringBuilder builderEnd = GetEndBuilder();
      //int iIndexInString = 0;

      if( RTFFont.Bold )
        AddTagToString( DEF_B_TAG, DEF_B_END_TAG, builderStart, builderEnd );

      if( RTFFont.Italic )
        AddTagToString( DEF_I_TAG, DEF_I_END_TAG, builderStart, builderEnd );

      if( RTFFont.Underline == ExcelUnderline.Single )
        AddTagToString( DEF_U_TAG, DEF_U_END_TAG, builderStart, builderEnd );

      if( RTFFont.Strikethrough )
        AddTagToString( DEF_S_TAG, DEF_S_END_TAG, builderStart, builderEnd );

      if( RTFFont.Subscript )
        AddTagToString( DEF_SUB_TAG, DEF_SUB_END_TAG, builderStart, builderEnd );

      if( RTFFont.Superscript )
        AddTagToString( DEF_SUP_TAG, DEF_SUP_END_TAG, builderStart, builderEnd );

      builderStart.Append( DEF_FONT_TAG );
      builderEnd.Insert( 0, DEF_FONT_END_TAG );

      AddAttributeToString( DEF_X_PREF + DEF_COLON + DEF_COLOR_PREF
        , GetColorString( RTFFont.RGBColor ), builderStart, builderEnd );

      AddAttributeToString( DEF_X_PREF + DEF_COLON + DEF_FACE_PREF
        , RTFFont.FontName, builderStart, builderEnd );

      AddAttributeToString( DEF_X_PREF + DEF_COLON + DEF_SIZE_PREF
        , RTFFont.Size.ToString(), builderStart, builderEnd );

      builderStart.Append( '>' );

      writer.WriteRaw( builderStart.ToString() );

      // NOTE: there could be some other reserved characters that we don't support.
      strValue = strValue.Replace( "&", "&amp;" );
      strValue = strValue.Replace( "\n", DEF_10_CHAR );
      writer.WriteRaw( strValue );
      writer.WriteRaw( builderEnd.ToString() );
    }
    #endregion

    #region DV serialization methods
    /// <summary>
    /// Serialize data validation objects.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="dvTable">DV table.</param>
    private void SerializeDataValidations( XmlWriter writer, DataValidationTable dvTable )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dvTable == null )
        throw new ArgumentNullException( "dvTable" );

      for (int i = 0, iLen = dvTable.Count; i < iLen; i++)
      {

          WorksheetImpl worksheet = dvTable.Worksheet;
          DValRecord dval = (DValRecord)BiffRecordFactory.GetRecord(TBIFFRecord.DVal);
          DataValidationCollection dvCollection = worksheet.DVTable.Add(dval);
          DVRecord dv = (DVRecord)BiffRecordFactory.GetRecord(TBIFFRecord.DV);
          dvCollection.AddDVRecord(dv);
          m_formulaUtil=new FormulaUtil (dvTable.Application,dvTable.Workbook,NumberFormatInfo.InvariantInfo,
              ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR);
          DataValidationCollection datavalidationcollection =dvTable[i];

          for (int j = 0; j < datavalidationcollection.Count; j++)
          {
              IDataValidation dataValidation = datavalidationcollection[j];
              if ((dataValidation as DataValidationImpl).DVRanges.Length > 0)
              {
                  writer.WriteStartElement(DEF_X_PREF, DEF_DATAVALIDATION_PREF, null);
                  SerializeDataValidation(writer, dataValidation);
                  writer.WriteEndElement();
              }
          }
      }

    }
    /// <summary>
    /// Serialize data validation object.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="dv">Data validation object.</param>
    private void SerializeDataValidation(XmlWriter writer, IDataValidation dv)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (dv == null)
            throw new ArgumentNullException("dv");

        DataValidationImpl dataValidation = (DataValidationImpl)dv;

        SerializeRanges(writer, dataValidation);

        SerializeFormulas(writer, dataValidation);

        if (dataValidation.ErrorStyle != ExcelErrorStyle.Stop)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_ERRORSTYLE_PREF, null);
            writer.WriteString(ConvertDataValidationErrorStyle(dataValidation.ErrorStyle));
            writer.WriteEndElement();
        }

        if (dataValidation.PromptBoxTitle != null && dataValidation.PromptBoxTitle.Length > 0)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_INPUTTITLE_PREF, null);
            writer.WriteString(dataValidation.PromptBoxTitle);
            writer.WriteEndElement();
        }

        if (dataValidation.PromptBoxText != null && dataValidation.PromptBoxText.Length > 0)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_INPUTMESSAGE_PREF, null);
            writer.WriteString(dataValidation.PromptBoxText);
            writer.WriteEndElement();
        }

        if (dataValidation.ErrorBoxTitle != null && dataValidation.ErrorBoxTitle.Length > 0)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_ERRORTITLE_PREF, null);
            writer.WriteString(dataValidation.ErrorBoxTitle);
            writer.WriteEndElement();
        }

        if (dataValidation.ErrorBoxTitle != null && dataValidation.ErrorBoxTitle.Length > 0)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_ERRORMESSAGE_PREF, null);
            writer.WriteString(dataValidation.ErrorBoxText);
            writer.WriteEndElement();
        }

        if (dataValidation.AllowType == ExcelDataType.User)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_CELLRANGELIST_PREF, null);
            writer.WriteEndElement();
        }


        // To:Do in feature.
    }
    /// <summary>
    /// Serialize Ranges for the DataValidation
    /// </summary>
    /// <param name="writer">XML Writer</param>
    /// <param name="dataValidation">DataValidationImpl</param>
    private void SerializeRanges(XmlWriter writer, DataValidationImpl dataValidation)
    {
        string[] ranges=dataValidation.DVRanges;

        if (ranges == null)
            throw new ArgumentNullException("strRange");

        int iFirstRow = 0;
        int iFirstColumn = 0;
        int iLastRow =0;
        int iLastColumn = 0;
        string value = null;
        writer.WriteStartElement(DEF_X_PREF, DV.DEF_RANGE_PREF, null);
        foreach (string range in ranges)
        {
            RangeImpl.ParseRangeString(range, dataValidation.Workbook, out iFirstRow, out iFirstColumn, out iLastRow, out iLastColumn);
            value = RangeImpl.GetAddressLocal(iFirstRow, iFirstColumn, iLastRow, iLastColumn, true) + ",";
            if (ranges[ranges.Length - 1] != range)
            {
                writer.WriteString(value.ToString());
            }
            else
            {
                writer.WriteString(value.Substring(0, value.Length - 1));
            }
        }
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize Formulas for the DataValiatin
    /// </summary>
    /// <param name="writer">XML Writer</param>
    /// <param name="dataValidation">DataValidationImpl</param>
    private void SerializeFormulas(XmlWriter writer, DataValidationImpl dataValidation)
    {
        ExcelDataType dataType = dataValidation.AllowType;
        ExcelDataValidationComparisonOperator comparisonOperator = dataValidation.CompareOperator;

        string strFirstFormula = dataValidation.GetR1C1FirstSecondFormula(m_formulaUtil,true);
        string strSecondFormula = dataValidation.GetR1C1FirstSecondFormula(m_formulaUtil,false);

        if (dataType != ExcelDataType.Any)
        {
            writer.WriteStartElement(DEF_X_PREF, DV.DEF_TYPE_PREF, null);
            writer.WriteString(ConvertDataValidationType(dataValidation.AllowType));
            writer.WriteEndElement();

            switch (dataType)
            {
                case ExcelDataType.Formula:
                case ExcelDataType.User:
                    {
                        strFirstFormula=strFirstFormula.Replace( '\0', ',' );
                        writer.WriteStartElement(DEF_X_PREF, DV.DEF_VALUE_PREF, null);
                        writer.WriteString(strFirstFormula);
                        writer.WriteEndElement();
                        break;
                    }
                default:
                    {
                        writer.WriteStartElement(DEF_X_PREF, DV.DEF_QUALIFIER_PREF, null);
                        writer.WriteString(dataValidation.CompareOperator.ToString());
                        writer.WriteEndElement();

                        if (comparisonOperator != ExcelDataValidationComparisonOperator.Between && comparisonOperator != ExcelDataValidationComparisonOperator.NotBetween)
                        {
                            writer.WriteStartElement(DEF_X_PREF, DV.DEF_VALUE_PREF, null);
                            writer.WriteString(strFirstFormula);
                            writer.WriteEndElement();
                        }
                        else
                        {
                            writer.WriteStartElement(DEF_X_PREF, DV.DEF_MIN_PREF, null);
                            writer.WriteString(strFirstFormula);
                            writer.WriteEndElement();

                            writer.WriteStartElement(DEF_X_PREF, DV.DEF_MAX_PREF, null);
                            writer.WriteString(strSecondFormula);
                            writer.WriteEndElement();   

                        }
                        break;
                    }
            }
        }
    }
    #endregion

    #region Condition Format serialize methods
    /// <summary>
    /// Serialize condition formats collection.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="conditions">Condition collection.</param>
    private void SerializeConditionFormats( XmlWriter writer, WorksheetConditionalFormats conditions )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( conditions == null )
        throw new ArgumentNullException( "conditions" );

      for( int i = 0, iLen = conditions.Count; i < iLen; i++ )
      {
        ConditionalFormats formats = conditions[ i ];

        writer.WriteStartElement( DEF_X_PREF, DEF_CONDITIONAL_FORMATTING_PREF, null );
        writer.WriteStartElement( DEF_X_PREF, DEF_RANGE_PREF, null );
        writer.WriteString( formats.AddressR1C1 );
        writer.WriteEndElement();

        IConditionalFormats format = formats;
        SerializeConditionFormat( writer, format );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serialize condition format.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="format">Format to serialize.</param>
    private void SerializeConditionFormat( XmlWriter writer, IConditionalFormats format )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      for( int i = 0, iLen = format.Count; i < iLen; i++ )
      {
        IConditionalFormat cond = format[ i ];

        SerializeCondition( writer, cond );
      }
    }
    /// <summary>
    /// Serialize condition instance.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="condition">condition to serialize.</param>
    private void SerializeCondition( XmlWriter writer, IConditionalFormat condition )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( condition == null )
        throw new ArgumentNullException( "condition" );

      writer.WriteStartElement( DEF_X_PREF, DEF_CONDITIONAL_PREF, null );
      string strFirst = condition.FirstFormulaR1C1;
      string strSecond = condition.SecondFormulaR1C1;

      if( condition.FormatType == ExcelCFType.CellValue )
      {
        writer.WriteStartElement( DEF_X_PREF, DEF_QUALIFIER_PREF, null );
        writer.WriteString( condition.Operator.ToString() );
        writer.WriteEndElement();
      }

      if( strFirst != null && strFirst.Length > 0 )
      {
        writer.WriteStartElement( DEF_X_PREF, DEF_VALUE1_PREF, null );
        writer.WriteString( strFirst );
        writer.WriteEndElement();
      }

      if( strSecond != null && strSecond.Length > 0 )
      {
        writer.WriteStartElement( DEF_X_PREF, DEF_VALUE2_PREF, null );
        writer.WriteString( strSecond );
        writer.WriteEndElement();
      }

      
      writer.WriteStartElement( DEF_X_PREF, DEF_FORMAT_PREF, null );
      writer.WriteAttributeString( DEF_X_PREF, DEF_STYLE_PREF, null, GetConditionaFormatString( condition ) );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    #endregion

    #region Worksheet option serialize methods
    /// <summary>
    /// Serialize worksheet option.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Current worksheet.</param>
    private void SerializeWorksheetOption( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( DEF_X_PREF, DEF_WORKSHEET_OPTIONS_PREF, null );

      if( sheet.Visibility != WorksheetVisibility.Visible )
      {
        int iIndex = ( int )sheet.Visibility;
        WriteElement( writer, DEF_X_PREF, DEF_VISIBLE_PREF, DEF_VISIBLE_STRING[ iIndex ] );
      }

      SerializeWindowTwoProperties( writer, sheet );

      PageSetupImpl setup = ( PageSetupImpl )sheet.PageSetup;

      if( setup.IsFitToPage )
        WriteElement( writer, DEF_X_PREF, DEF_FIT_TO_PAGE_PREF );

      int iColorIndex = ( int )sheet.TabColor;

      if( iColorIndex != ( int )WorksheetImpl.DEF_DEFAULT_TAB_COLOR )
        WriteElement( writer, DEF_X_PREF, DEF_TABCOLOR_INDEX_PREF, iColorIndex.ToString() );

      if( sheet.Zoom != DEF_ZOOM )
        WriteElement( writer, DEF_X_PREF, DEF_ZOOM_PREF, sheet.Zoom.ToString() );

      SerializePageSetup( writer, setup );
      SerializePanes( writer, sheet );

      if( !setup.IsNotValidSettings )
        SerializePrint( writer, setup );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize page setup.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="pageSetup">Current page setup to serialize.</param>
    private void SerializePageSetup( XmlWriter writer, IPageSetup pageSetup )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      PageSetupImpl setup = ( PageSetupImpl )pageSetup;

      writer.WriteStartElement( DEF_X_PREF, DEF_PAGE_SETUP_PREF, null );

      SerializeHeaderFooter( writer, setup, true );
      SerializeHeaderFooter( writer, setup, false );
      SerializeLayout( writer, setup );
      SerializePageMargins( writer, setup );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize header.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="pageSetup">Storage, that contain header properties.</param>
    /// <param name="isFooter">If true - footer to serialize; otherwise - header.</param>
    private void SerializeHeaderFooter( XmlWriter writer, PageSetupImpl pageSetup, bool isFooter )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      double dMargin = isFooter ? pageSetup.FooterMargin : pageSetup.HeaderMargin;
      bool bMargin = dMargin != DEF_MARGIN;
      string strData = isFooter ? pageSetup.FullFooterString : pageSetup.FullHeaderString;
      bool bData = strData != null && strData.Length > 0;

      if( bMargin || bData )
      {
        string strElemName = isFooter ? DEF_FOOTER_PREF : DEF_HEADER_PREF;

        writer.WriteStartElement( DEF_X_PREF, strElemName, null );
        writer.WriteAttributeString( DEF_X_PREF, DEF_MARGIN_PREF, null, XmlConvert.ToString( dMargin ) );

        if( bData )
          writer.WriteAttributeString( DEF_X_PREF, DEF_DATA_PREF, null, strData );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serialize layout
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="pageSetup">Storage, that contain all layout properties.</param>
    private void SerializeLayout( XmlWriter writer, PageSetupImpl pageSetup )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      ExcelPageOrientation orientation = pageSetup.Orientation;

      writer.WriteStartElement( DEF_X_PREF, DEF_LAYOUT_PREF, null );

      if( !pageSetup.AutoFirstPageNumber )
        writer.WriteAttributeString( DEF_X_PREF, DEF_START_PAGE_NUMBER_PREF, null, pageSetup.FirstPageNumber.ToString() );

      if( orientation != ExcelPageOrientation.Portrait )
        writer.WriteAttributeString( DEF_X_PREF, DEF_ORIENTATION_PREF, null, orientation.ToString() );

      if( pageSetup.CenterHorizontally )
        writer.WriteAttributeString( DEF_X_PREF, DEF_CENTER_HORIZONTAL_PREF, null, DEF_XML_TRUE );

      if( pageSetup.CenterVertically )
        writer.WriteAttributeString( DEF_X_PREF, DEF_CENTER_VERTICAL_PREF, null, DEF_XML_TRUE );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize page margins.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="pageSetup">Storage, that contain all page margins properties.</param>
    private void SerializePageMargins( XmlWriter writer, PageSetupImpl pageSetup )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      writer.WriteStartElement( DEF_X_PREF, DEF_PAGE_MARGINS_PREF, null );

      double dRight = pageSetup.RightMargin;
      double dLeft = pageSetup.LeftMargin;
      double dTop = pageSetup.TopMargin;
      double dBottom = pageSetup.BottomMargin;

      if( dRight != PageSetupBaseImpl.DEFAULT_RIGHTMARGIN )
        writer.WriteAttributeString( DEF_X_PREF, DEF_MARGIN_RIGHT_PREF, null, XmlConvert.ToString( dRight ) );

      if( dLeft != PageSetupBaseImpl.DEFAULT_LEFTMARGIN )
        writer.WriteAttributeString( DEF_X_PREF, DEF_MARGIN_LEFT_PREF, null, XmlConvert.ToString( dLeft ) );

      if( dBottom != PageSetupBaseImpl.DEFAULT_BOTTOMMARGIN )
        writer.WriteAttributeString( DEF_X_PREF, DEF_MARGIN_BOTTOM_PREF, null, XmlConvert.ToString( dBottom ) );

      if( dTop != PageSetupBaseImpl.DEFAULT_TOPMARGIN )
        writer.WriteAttributeString( DEF_X_PREF, DEF_MARGIN_TOP_PREF, null, XmlConvert.ToString( dTop ) );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes pains.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Current worksheet</param>
    private void SerializePanes( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      PaneRecord pane = sheet.Pane;
      List<SelectionRecord> selection =  sheet.Selections;

      if( selection != null )
      {
        if( selection.Count > 1 )
          WriteElement( writer, DEF_X_PREF, DEF_ACTIVE_PANE_PREF, pane.ActivePane.ToString() );

        SerializeSelectionPane( writer, selection );
      }

      if( pane == null )
        return;

      int iHorSplit = pane.HorizontalSplit;
      int iVertSplit = pane.VerticalSplit;

      if( sheet.WindowTwo.TopRow != 0 )
        WriteElement( writer, DEF_X_PREF, DEF_FIRST_VISIBLE_ROW_PREF, sheet.WindowTwo.TopRow.ToString() );

      if( iHorSplit != 0 )
      {
        WriteElement( writer, DEF_X_PREF, DEF_SPLIT_HORIZONTAL_PANE_PREF, iHorSplit.ToString() );
        WriteElement( writer, DEF_X_PREF, DEF_TOPROW_BOTTOM_PANE_PREF, pane.FirstRow.ToString() );
      }

      if( iVertSplit != 0 )
      {
        WriteElement( writer, DEF_X_PREF, DEF_SPLIT_VERTICAL_PANE_PREF, iVertSplit.ToString() );
        WriteElement( writer, DEF_X_PREF, DEF_LEFTCOLUMN_RIGHT_PANE_PREF, pane.FirstColumn.ToString() );
      }

      WindowTwoRecord window = sheet.WindowTwo;

      if( window.IsFreezePanes )
        WriteElement( writer, DEF_X_PREF, DEF_FREEZE_PANES_PREF );

      if( window.IsFreezePanesNoSplit )
        WriteElement( writer, DEF_X_PREF, DEF_FROZEN_NOSPLIT_PANES_PREF );
    }
    /// <summary>
    /// Serialize panes block.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="arrSelection">Array list, that contain all selection to serialize.</param>
    private void SerializeSelectionPane( XmlWriter writer, List<SelectionRecord> arrSelection )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( arrSelection == null )
        throw new ArgumentNullException( "arrSelection" );

      int iLen = arrSelection.Count;

      if( iLen > 4 )
        throw new ArgumentOutOfRangeException( "Array cannot contain more than 4 selection records" );

      writer.WriteStartElement( DEF_X_PREF, DEF_PANES_PREF, null );

      for( int i = 0; i < iLen; i++ )
      {
        SelectionRecord record = arrSelection[ i ];

        writer.WriteStartElement( DEF_X_PREF, DEF_PANE_PREF, null );

        WriteElement( writer, DEF_X_PREF, DEF_NUMBER_PANE_PREF, record.Pane.ToString() );

        if( record.ColumnActiveCell != 0 )
          WriteElement( writer, DEF_X_PREF, DEF_ACTIVECOL_PANE_PREF, record.ColumnActiveCell.ToString() );

        if( record.RowActiveCell != 0 )
          WriteElement( writer, DEF_X_PREF, DEF_ACTIVEROW_PANE_PREF, record.RowActiveCell.ToString() );

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes print.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="pageSetup">Storage, that contain all print properties.</param>
    private void SerializePrint( XmlWriter writer, IPageSetup pageSetup )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      int iPaperSize = ( int )pageSetup.PaperSize;

      writer.WriteStartElement( DEF_X_PREF, DEF_PRINT_PREF, null );
      
      if( pageSetup.Copies != DEF_NUMBER_OF_COPIES )
        WriteElement( writer, DEF_X_PREF, DEF_NUMBER_OF_COPIES_PREF, pageSetup.Copies.ToString() );

      if( pageSetup.PrintQuality <= short.MaxValue )
        WriteElement( writer, DEF_X_PREF, DEF_HORIZONTAL_RESOLUTION_PREF, pageSetup.PrintQuality.ToString() );

      if( pageSetup.PaperSize != ExcelPaperSize.PaperLetter )
        WriteElement( writer, DEF_X_PREF, DEF_PAPER_SIZE_INDEX_PREF, iPaperSize.ToString() );

      if( pageSetup.IsFitToPage )
      {
        if( pageSetup.FitToPagesWide != DEF_FIT )
          WriteElement( writer, DEF_X_PREF, DEF_FIT_WIDTH_PREF, pageSetup.FitToPagesWide.ToString() );

        if( pageSetup.FitToPagesTall != DEF_FIT )
          WriteElement( writer, DEF_X_PREF, DEF_FIT_HEIGHT_PREF, pageSetup.FitToPagesTall.ToString() );

      }
      else if( pageSetup.Zoom != DEF_SCALE )
      {
        WriteElement( writer, DEF_X_PREF, DEF_SCALE_PREF, pageSetup.Zoom.ToString() );
      }

      if( pageSetup.PrintGridlines )
        WriteElement( writer, DEF_X_PREF, DEF_GRIDLINES_PREF );

      if( pageSetup.BlackAndWhite )
        WriteElement( writer, DEF_X_PREF, DEF_BLACK_AND_WHITE_PREF );

      if( pageSetup.Draft )
        WriteElement( writer, DEF_X_PREF, DEF_DRAFT_QUALITY_PREF );

      if( pageSetup.PrintHeadings )
        WriteElement( writer, DEF_X_PREF, DEF_ROWCOL_HEADINGS_PREF );

      if( pageSetup.PrintComments != ExcelPrintLocation.PrintNoComments )
      {
        int iIndex = ( int )pageSetup.PrintComments;
        WriteElement( writer, DEF_X_PREF, DEF_COMMENTS_LAYOUT_PREF, DEF_PRINT_LOCATION_STRING[ iIndex ] );
      }

      if( pageSetup.PrintErrors != ExcelPrintErrors.PrintErrorsDisplayed )
      {
        int iIndex = ( int )pageSetup.PrintErrors;
        WriteElement( writer, DEF_X_PREF, DEF_PRINT_ERRORS_PREF, DEF_PRINT_ERROR_STRING[ iIndex ] );
      }

      if( pageSetup.Order == ExcelOrder.OverThenDown )
        WriteElement( writer, DEF_X_PREF, DEF_LEFT_TO_RIGHT_PREF );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes window two record properties.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="sheet">Current sheet.</param>
    private void SerializeWindowTwoProperties( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WindowTwoRecord window = sheet.WindowTwo;

      if( window == null )
        return;

      if( !window.IsDisplayGridlines )
        WriteElement( writer, DEF_X_PREF, DEF_DISPLAY_GRIDLINES_PREF );

      if( !window.IsDisplayRowColHeadings )
        WriteElement( writer, DEF_X_PREF, DEF_DISPLAY_HEADINGS_PREF );

      if( window.IsSelected )
        WriteElement( writer, DEF_X_PREF, DEF_SELECTED_PREF );
    }
    #endregion

    #region ExelWorkbook serialize methods
    /// <summary>
    /// Serialize excelworkbook xml properties.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="book">Current workbook.</param>
    private void SerializeExcelWorkbook( XmlWriter writer, WorkbookImpl book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      writer.WriteStartElement( DEF_X_PREF, DEF_EXCELWORKBOOK_PREF, null );

      int iSheetIndex = book.ActiveSheet.Index;
      int iSheetCount = book.WorksheetGroup.Count;

      if( iSheetIndex > 0 )
      {
        WriteElement( writer, DEF_X_PREF, DEF_ACTIVE_SHEET_PREF, iSheetIndex.ToString() );
        WriteElement( writer, DEF_X_PREF, DEF_FIRST_VISIBLE_SHEET_PREF, book.DisplayedTab.ToString() );
      }

      if( iSheetCount > 1 )
        WriteElement( writer, DEF_X_PREF, DEF_SELECTED_SHEETS_PREF, iSheetCount.ToString() );

      writer.WriteEndElement();
    }
    #endregion

    /// <summary>
    /// Serializes document properties.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="book">Current workbook.</param>
    private void SerializeDocumentProperties( XmlWriter writer, IWorkbook book )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Serialize workbook.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="book">Book to serialize.</param>
    private void SerializeWorkbook( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      WorkbookImpl bokImpl = ( WorkbookImpl )book;

      m_mergeStyles.Clear();
#if ( WINRT )
      writer.WriteStartElement(DEF_WORKBOOK_PREF, DEF_SS_NAMESPACE);
      writer.WriteAttributeString(DEF_XMLNS_PREF, DEF_SS_PREF, null, DEF_SS_NAMESPACE);
      writer.WriteAttributeString(DEF_XMLNS_PREF, DEF_X_PREF, null, DEF_X_NAMESPACE);
      writer.WriteAttributeString(DEF_XMLNS_PREF, DEF_O_PREF, null, DEF_O_NAMESPACE);
      writer.WriteAttributeString(DEF_XMLNS_PREF, DEF_HTML_PREF, null, DEF_HTML_NAMESPACE);
#else
      writer.WriteStartElement( DEF_WORKBOOK_PREF, null );
      writer.WriteAttributeString( DEF_XMLNS_PREF, null, null, DEF_SS_NAMESPACE );
      writer.WriteAttributeString( DEF_XMLNS_PREF, DEF_SS_PREF, null, DEF_SS_NAMESPACE );
      writer.WriteAttributeString( DEF_XMLNS_PREF, DEF_X_PREF, null, DEF_X_NAMESPACE );
      writer.WriteAttributeString( DEF_XMLNS_PREF, DEF_O_PREF, null, DEF_O_NAMESPACE );
      writer.WriteAttributeString( DEF_XMLNS_PREF, DEF_HTML_PREF, null, DEF_HTML_NAMESPACE );
#endif
      List<ExtendedFormatImpl> listMergedStyles = GetMergedList( book.Worksheets );

      if( book.Styles.Count > 0 )
        SerializeStyles( writer, ( ( WorkbookImpl )book ).InnerExtFormats, listMergedStyles );

      SerializeExcelWorkbook( writer, ( WorkbookImpl )book );
      SerializeNames( writer, book.Names, false );
      SerializeWorksheets( writer, book.Worksheets );

      m_mergeStyles.Clear();
      writer.WriteEndElement();
    }
    #endregion

    #region IXmlSerializator methods
    /// <summary>
    /// Serializes workbook to xml.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="book">Workbook to serialize.</param>
    public void Serialize( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );
#if !(WINRT )
      writer.WriteRaw( DEF_VERSION_STRING );
#endif
      writer.WriteRaw( DEF_APPLICATION_STRING );
      SerializeWorkbook( writer, book );
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Initializes builder used to accumulate start of some xml string.
    /// </summary>
    /// <returns>Builder used to accumulate start of some xml string.</returns>
    private StringBuilder GetStartBuilder()
    {
      return InitializeBuilder( ref m_builderStart );
    }
    /// <summary>
    /// Initializes builder used to accumulate end of some xml string.
    /// </summary>
    /// <returns>Builder used to accumulate start of some xml string.</returns>
    private StringBuilder GetEndBuilder()
    {
      return InitializeBuilder( ref m_builderEnd );
    }
    /// <summary>
    /// Initializes builder used to accumulate some xml string.
    /// </summary>
    /// <param name="builder">Builder to initialize.</param>
    /// <returns>Initialized String builder</returns>
    private StringBuilder InitializeBuilder( ref StringBuilder builder)
    {
      if( builder == null )
      {
        builder = new StringBuilder();
      }
      else
      {
        builder.Length = 0;
      }

      return builder;
    }
    /// <summary>
    /// Gets color string  by Color.
    /// </summary>
    /// <param name="col">Current color.</param>
    /// <returns>Returns string by color.</returns>
    private string GetColorString( Color col )
    {
      string strResult = DEF_COLOR_STRING;

      int iColor = col.ToArgb() & 0x00FFFFFF;
      strResult += iColor.ToString( "X6" );

      return strResult;
    }
    /// <summary>
    /// Adds open and close tag.
    /// </summary>
    /// <param name="strOpenTag">Open tag.</param>
    /// <param name="strCloseTag">Closed tag.</param>
    /// <param name="builderStart">String builder to accumulate start of the xml string.</param>
    /// <param name="builderEnd">String builder to accumulate end of the xml string.</param>
    private void AddTagToString( string strOpenTag, string strCloseTag,
      StringBuilder builderStart, StringBuilder builderEnd )
    {
      if( builderStart == null )
        throw new ArgumentNullException( "builderStart" );

      if( builderEnd == null )
        throw new ArgumentNullException( "builderEnd" );

      builderStart.Append( strOpenTag );
      builderEnd.Insert( 0, strCloseTag );
    }
    /// <summary>
    /// Adds attribute to string.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="builderStart">String builder to accumulate start of the xml string.</param>
    /// <param name="builderEnd">String builder to accumulate end of the xml string.</param>
    private void AddAttributeToString( string name, string value,
      StringBuilder builderStart, StringBuilder builderEnd )
    {
      if( builderStart == null )
        throw new ArgumentNullException( "builderStart" );

      if( builderEnd == null )
        throw new ArgumentNullException( "builderEnd" );

      builderStart.Append( " " );
      builderStart.Append( name );
      builderStart.Append( "=\"" );
      builderStart.Append( value );
      builderStart.Append( "\" " );
    }
    /// <summary>
    /// Generates xml string by conditional format.
    /// </summary>
    /// <param name="cond">Conditional format object.</param>
    /// <returns>Returns generated string.</returns>
    private string GetConditionaFormatString( IConditionalFormat cond )
    {
      if( cond == null )
        throw new ArgumentNullException( "cond" );

      string result = "";
      string value = "";

      if( cond.IsFontFormatPresent )
      {
        if( cond.IsFontColorPresent )
        {
          value = GetColorString( cond.FontColorRGB );
          result += DEF_FONT_COLOR_CF + DEF_COLON + value + DEF_SEMICOLON;
        }

        value = cond.IsBold ? DEF_FONT_BOLD_CF : DEF_FONT_REGULAR_CF;
        result += DEF_FONT_WEIGHT_CF + DEF_COLON + value + DEF_SEMICOLON;

        if( cond.IsItalic )
          result += DEF_FONT_ITALIC_CF;
        
        value = cond.Underline.ToString();
        result += DEF_FONT_UNDERLINE_CF + DEF_COLON + value + DEF_SEMICOLON;

        if( cond.IsStrikeThrough )
          result += DEF_FONT_STRIKE_CF;
      }

      if( cond.IsPatternFormatPresent )
      {
        if( cond.IsBackgroundColorPresent )
        {
          value = GetColorString( cond.BackColorRGB );
          result += DEF_PATTERN_BACKGROUND_CF + DEF_COLON + value + DEF_SEMICOLON;
        }

        int index = ( int )cond.FillPattern;
        value = DEF_PATTERN_STRING_CF[ index ] + " " + GetColorString( cond.ColorRGB );
        result += DEF_PATTERN_FILL_CF + DEF_COLON + value + DEF_SEMICOLON;
      }

      if( cond.IsBorderFormatPresent )
      {
        if( cond.IsTopBorderModified )
          result += GetBorderString( "top", cond.TopBorderColorRGB, cond.TopBorderStyle );

        if( cond.IsLeftBorderModified )
          result += GetBorderString( "left", cond.LeftBorderColorRGB, cond.LeftBorderStyle );

        if( cond.IsBottomBorderModified )
          result += GetBorderString( "bottom", cond.BottomBorderColorRGB, cond.BottomBorderStyle );


        if( cond.IsRightBorderModified )
          result += GetBorderString( "right", cond.RightBorderColorRGB, cond.RightBorderStyle );
      }

      return result;
    }
    /// <summary>
    /// Gets border format string for CF serialize.
    /// </summary>
    /// <param name="strBorder">Represents border type.</param>
    /// <param name="borderCol">Represents border color.</param>
    /// <param name="style">Represents border line style.</param>
    /// <returns>Returns border line style.</returns>
    private string GetBorderString( string strBorder, Color borderCol, ExcelLineStyle style )
    {
      string result = DEF_BORDER_CF;

      result += strBorder + DEF_COLON;
      result += " " + DEF_BORDER_LINE_CF[ ( int )style ];
      result += " " + GetColorString( borderCol ) + DEF_SEMICOLON;

      return result;
    }
    /// <summary>
    /// Gets xml string from ExcelHAling type.
    /// </summary>
    /// <param name="hAlign">ExcelHAling type.</param>
    /// <returns>Returns xml string.</returns>
    private string GetStyleHAlignString( ExcelHAlign hAlign )
    {
      if( hAlign == ExcelHAlign.HAlignGeneral )
        return "Automatic";

      string result = hAlign.ToString();
      return result.Remove( 0, 6 );
    }
    /// <summary>
    /// Gets xml string from ExcelVAling type.
    /// </summary>
    /// <param name="vAlign">ExcelVAling type.</param>
    /// <returns>Returns xml string.</returns>
    private string GetStyleVAlignString( ExcelVAlign vAlign )
    {
      string result = vAlign.ToString();
      return result.Remove( 0, 6 );
    }
    /// <summary>
    /// Gets font alignment xml string;
    /// </summary>
    /// <param name="font">Font object.</param>
    /// <returns>Returns xml font alignment string.</returns>
    private string GetStyleFontAlign( IFont font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      string strAllign = DEF_STYLE_ALIGN_NONE;
      if( font.Subscript ) strAllign = DEF_STYLE_ALIGN_SUBSCRIPT;
      if( font.Superscript ) strAllign = DEF_STYLE_ALIGN_SUPERSCRIPT;

      return strAllign;
    }
    /// <summary>
    /// Gets condition string value.
    /// </summary>
    /// <param name="cond">Condition object.</param>
    /// <returns>Returns condition value.</returns>
    private string GetAutoFilterConditionValue( IAutoFilterCondition cond )
    {
      if( cond == null )
        throw new ArgumentNullException( "cond" );

      if( cond.DataType == ExcelFilterDataType.String ) return cond.String;
      if( cond.DataType == ExcelFilterDataType.FloatingPoint ) return cond.Double.ToString();
      if( cond.DataType == ExcelFilterDataType.ErrorCode ) return cond.ErrorCode.ToString();
      if( cond.DataType == ExcelFilterDataType.Boolean ) return ( cond.Boolean ) ? DEF_XML_TRUE : DEF_XML_FALSE;

      throw new ArgumentException( "Unassigned conditonal type" );
    }
    /// <summary>
    /// Gets cell type and value.
    /// </summary>
    /// <param name="cells">Cells collection</param>
    /// <param name="index">Index of cell.</param>
    /// <param name="type">Returns xml type of cell by index.</param>
    /// <returns>Returns xml cell value.</returns>
    private string GetCellTypeValue( CellRecordCollection cells, long index
      , out XmlSerializationCellType type )
    {
      if( cells == null )
        throw new ArgumentNullException( "cell" );

      string strString = cells.GetText( index );

      if( strString != null )
      {
        type = XmlSerializationCellType.String;
        return strString;
      }

      double dVal = cells.GetNumberWithoutFormula( index );
      if( dVal != double.MinValue )
      {
        type = XmlSerializationCellType.Number;
        return XmlConvert.ToString( dVal );
      }

      DateTime time = cells.GetDateTime( index );
      if( time != DateTime.MinValue )
      {
        type = XmlSerializationCellType.DateTime;
        return XmlConvert.ToString( time, DEF_DATATIME_MASK );
      }

      string strError = cells.GetError( index );
      if( strError != null )
      {
        type = XmlSerializationCellType.Error;
        return strError;
      }

      bool bVal;
      if( cells.GetBool( index, out bVal ) )
      {
        type = XmlSerializationCellType.Boolean;
        return bVal ? DEF_XML_TRUE : DEF_XML_FALSE;
      }

      throw new ApplicationException( "Cell dosn't contain value" );
    }
    /// <summary>
    /// Writes element to xml stream.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="strPrefix">Element prefix.</param>
    /// <param name="strName">Element name.</param>
    private void WriteElement( XmlWriter writer, string strPrefix, string strName )
    {
      WriteElement( writer, strPrefix, strName, null );
    }
    /// <summary>
    /// Writes element to xml stream.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="strPrefix">Element prefix.</param>
    /// <param name="strName">Element name.</param>
    /// <param name="strValue">Element value</param>
    private void WriteElement( XmlWriter writer, string strPrefix, string strName, string strValue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strName == null || strName.Length == 0 )
        throw new ArgumentNullException( "strName" );

      if( strPrefix == null || strPrefix.Length == 0 )
        throw new ArgumentNullException( "strPrefix" );

      writer.WriteStartElement( strPrefix, strName, null );

      if( strValue != null )
        writer.WriteString( strValue );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Reserialize merged styles styles.
    /// </summary>
    /// <param name="writer">Xml writer.</param>
    /// <param name="list">List with styles to reserialize.</param>
    private void ReSerializeStyle( XmlWriter writer, List<ExtendedFormatImpl> list )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( list == null )
        throw new ArgumentNullException( "list" );

      for( int i = 0, iLen = list.Count; i < iLen; i++ )
      {
        SerializeStyle( writer, list[ i ] );
      }
    }
    /// <summary>
    /// Gets array of merged styles.
    /// </summary>
    /// <param name="sheets">Worksheets collection</param>
    /// <returns>Returns list with merged styles.</returns>
    private List<ExtendedFormatImpl> GetMergedList( IWorksheets sheets )
    {
      List<ExtendedFormatImpl> result = new List<ExtendedFormatImpl>();

      if( sheets == null )
        throw new ArgumentNullException( "sheets" );

      for( int i = 0, iLen = sheets.Count; i < iLen; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )sheets[ i ];
        MergeCellsImpl merges = sheet.MergeCells;
        IList<ExtendedFormatImpl> list = merges.GetMergedExtendedFormats();

        for( int j = 0, len = list.Count; j < len; j++ )
        {
          ExtendedFormatImpl format = list[ j ];
          Rectangle region = merges[ j ];
          long lFirstCellIndex = RangeImpl.GetCellIndex( region.X + 1, region.Y + 1 );
          long lKey = WorkbookXmlSerializator.GetUniqueID( sheet.Index, lFirstCellIndex );
          int iValue = ( DEF_MERGED_STYLE + m_mergeStyles.Count );
          format.Index = ( ushort )iValue;
          m_mergeStyles.Add( lKey, iValue );
        }

        result.AddRange( list );
      }

      return result;
    }
    /// <summary>
    /// Updates formulas, that contain error.
    /// </summary>
    /// <param name="strFormula">Formula to update.</param>
    /// <returns>Returns updated formula.</returns>
    private string UpdateFormulaError( string strFormula )
    {
      if( strFormula == null || strFormula.Length == 0 )
        throw new ArgumentNullException( "strFormula" );

      return ( strFormula.EndsWith( DEF_BAD_REF ) ) ? DEF_BAD_FORMULA : strFormula;
    }

    /// <summary>
    /// Converts Excel 2007 data validation type to Excel 97-03.
    /// </summary>
    /// <param name="dataValidationType">Excel 2007 data validation type.</param>
    /// <returns>Excel 97-03 data validation type.</returns>
    private string ConvertDataValidationType(ExcelDataType dataValidationType)
    {
        if (dataValidationType == null )
            throw new ArgumentNullException("strErrorStyle");

        int iIndex = (int)dataValidationType;

        return (iIndex > 0) ? WorkbookXmlSerializator.DEF_ALLOWTYPE_STRING[iIndex] : WorkbookXmlSerializator.DEF_ALLOWTYPE_STRING[0];
    }
    /// <summary>
    /// Returns DV error style.
    /// </summary>
    /// <param name="strErrorStyle">DV error style name.</param>
    /// <returns>DV error style.</returns>
    private string ConvertDataValidationErrorStyle(ExcelErrorStyle strErrorStyle)
    {
        if (strErrorStyle == null)
            throw new ArgumentNullException("strErrorStyle");

        int iIndex = (int)strErrorStyle;

        return (iIndex > 0) ? DEF_ERRORSTYLE[iIndex] : DEF_ERRORSTYLE[0];

    }
   
    #endregion
  }
}
