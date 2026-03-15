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

using System;
using System.Xml;
using System.Globalization;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces.XmlSerialization;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
	/// <summary>
	/// Xml serializator. Serializes XlsIO workbook into dls xml file format.
	/// </summary>
	[ XmlSerializator( ExcelXmlSaveType.DLS ) ]
	public class DLSXmlSerializator : IXmlSerializator
	{
    #region Class constants
    /// <summary>
    /// Root tag for dls format.
    /// </summary>
    private const string DEF_DLS_START = "DLS";
    /// <summary>
    /// Protection attribute name.
    /// </summary>
    private const string DEF_PROTECTION_ATTRIBUTE = "ProtectionType";
    /// <summary>
    /// Default protection value.
    /// </summary>
    private const string DEF_PROTECTION_VALUE = "NoProtection";
    /// <summary>
    /// Styles tag in dls format.
    /// </summary>
    private const string DEF_STYLES_START = "styles";
    /// <summary>
    /// Style tag in dls format.
    /// </summary>
    private const string DEF_STYLE_START = "style";
    /// <summary>
    /// Id attribute for style tag.
    /// </summary>
    private const string DEF_ID_ATTRIBUTE = "id";
    /// <summary>
    /// Name attribute for style tag.
    /// </summary>
    private const string DEF_NAME_ATTRIBUTE = "Name";
    /// <summary>
    /// Type attribute for style tag.
    /// </summary>
    private const string DEF_TYPE_ATTRIBUTE = "type";
    /// <summary>
    /// Sections tag in dls format.
    /// </summary>
    private const string DEF_SECTIONS_START = "sections";
    /// <summary>
    /// Section tag in dls format.
    /// </summary>
    private const string DEF_SECTION_START = "section";
    /// <summary>
    /// Break code attribute in section tag.
    /// </summary>
    private const string DEF_BREAK_CODE_ATTRIBUTE = "BreakCode";
    /// <summary>
    /// Paragraphs tag in dls format.
    /// </summary>
    private const string DEF_PARAGRAPHS_START = "paragraphs";
    /// <summary>
    /// Paragraph tag in dls format.
    /// </summary>
    private const string DEF_PARAGRAPH_START = "paragraph";
    /// <summary>
    /// Items tag in dls format.
    /// </summary>
    private const string DEF_ITEMS_START = "items";
    /// <summary>
    /// Item tag in dls format.
    /// </summary>
    private const string DEF_ITEM_START = "item";
    /// <summary>
    /// Rows tag in dls format.
    /// </summary>
    private const string DEF_ROWS_START = "rows";
    /// <summary>
    /// Row tag in dls format.
    /// </summary>
    private const string DEF_ROW_START = "row";
    /// <summary>
    /// Cells tag in dls format.
    /// </summary>
    private const string DEF_CELLS_START = "cells";
    /// <summary>
    /// Cell tag in dls format.
    /// </summary>
    private const string DEF_CELL_START = "cell";
    /// <summary>
    /// Width attribute.
    /// </summary>
    private const string DEF_WIDTH_ATTRIBUTE = "Width";
    /// <summary>
    /// Text tag in dls format.
    /// </summary>
    private const string DEF_TEXT_RANGE_ATTRIBUTE = "TextRange";
    /// <summary>
    /// Text tag in dls format.
    /// </summary>
    private const string DEF_TEXT_START = "text";
    /// <summary>
    /// Columns count attribute in dls format.
    /// </summary>
    private const string DEF_COLUMNS_COUNT_ATTRIBUTE = "ColumnsCount";
    /// <summary>
    /// Format tag in dls format.
    /// </summary>
    private const string DEF_FORMAT_START = "format";
    /// <summary>
    /// Font name attribute in dls format.
    /// </summary>
    private const string DEF_FONT_NAME_ATTRIBUTE = "FontName";
    /// <summary>
    /// Font size attribute in dls format.
    /// </summary>
    private const string DEF_FONT_SIZE_ATTRIBUTE = "FontSize";
    /// <summary>
    /// Bold attribute in dls format.
    /// </summary>
    private const string DEF_BOLD_ATTRIBUTE = "Bold";
    /// <summary>
    /// Italic attribute in dls format.
    /// </summary>
    private const string DEF_ITALIC_ATTRIBUTE = "Italic";
    /// <summary>
    /// Underline attribute in dls format.
    /// </summary>
    private const string DEF_UNDERLINE_ATTRIBUTE = "Underline";
    /// <summary>
    /// Text color attribute in dls format.
    /// </summary>
    private const string DEF_TEXT_COLOR_ATTRIBUTE = "TextColor";
    /// <summary>
    /// Color prefix.
    /// </summary>
    private const string DEF_COLOR_PREFIX = "#";
    /// <summary>
    /// No underline in dls.
    /// </summary>
    private const string DEF_UNDERLINE_NONE = "None";
    /// <summary>
    /// Single underline in dls.
    /// </summary>
    private const string DEF_UNDERLINE_SINGLE = "Single";
    /// <summary>
    /// Double underline in dls.
    /// </summary>
    private const string DEF_UNDERLINE_DOUBLE = "Double";
    /// <summary>
    /// Subscript string in dls.
    /// </summary>
    private const string DEF_SUBCRIPT = "SubScript";
    /// <summary>
    /// Super script in dls.
    /// </summary>
    private const string DEF_SUPSCRIPT = "SuperScript";
    /// <summary>
    /// No sub/super script in dls.
    /// </summary>
    private const string DEF_NO_SUBSUPERSCIRPT = "None";
    /// <summary>
    /// SubSuperScript attribute in dls.
    /// </summary>
    private const string DEF_SUBSUPERSCRIPT_ATTRIBUTE = "SubSuperScript";
    /// <summary>
    /// Strikeout attribute in dls.
    /// </summary>
    private const string DEF_STRIKEOUT_ATTRIBUTE = "Strike";//"Strikeout";
    /// <summary>
    /// True string in dls.
    /// </summary>
    private static readonly string DEF_TRUE_STRING = bool.TrueString.ToLower();
    /// <summary>
    /// Table format tag in dls.
    /// </summary>
    private const string DEF_TABLE_FORMAT_START = "cell-format";
    /// <summary>
    /// Start of the character formatting tag.
    /// </summary>
    private const string DEF_CHARACTER_FORMAT_START = "character-format";
    /// <summary>
    /// Borders tag in dls.
    /// </summary>
    private const string DEF_BORDERS_START = "borders";
    /// <summary>
    /// Border tag in dls.
    /// </summary>
    private const string DEF_BORDER_START = "border";
    /// <summary>
    /// Color attribute string.
    /// </summary>
    private const string DEF_COLOR_ATTRIBUTE = "Color";
    /// <summary>
    /// Line width attribute string.
    /// </summary>
    private const string DEF_LINE_WIDTH_ATTRIBUTE = "LineWidth";
    /// <summary>
    /// Border type attribute string.
    /// </summary>
    private const string DEF_BORDER_TYPE_ATTRIBUTE = "BorderType";
    /// <summary>
    /// Border width for None border style.
    /// </summary>
    private const string DEF_BORDER_WIDTH_NONE = "0";
    /// <summary>
    /// Single border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_SIGNLE = "Single";
    /// <summary>
    /// Double border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_DOUBLE = "Double";
    /// <summary>
    /// Dot border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_DOT = "Dot";
    /// <summary>
    /// Dash with small gaps border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_DASH_SMALL = "DashSmallGap";
    /// <summary>
    /// Dash dot border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_DOT_DASH = "DotDash";
    /// <summary>
    /// Dash-dot-dot border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_DOT_DOT_DASH = "DotDotDash";
    /// <summary>
    /// Thick border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_THICK = "Thick";
    /// <summary>
    /// None border type.
    /// </summary>
    private const string DEF_BORDER_TYPE_NONE = "None";
    /// <summary>
    /// Tag name for page settings block.
    /// </summary>
    private const string DEF_PAGE_SETTINGS_START = "page-setup";
    /// <summary>
    /// Page height attribute.
    /// </summary>
    private const string DEF_PAGE_HEIGHT_ATTRIBUTE = "PageHeight";
    /// <summary>
    /// Page width attribute.
    /// </summary>
    private const string DEF_PAGE_WIDTH_ATTRIBUTE = "PageWidth";
    /// <summary>
    /// Footer distance attribute.
    /// </summary>
    private const string DEF_FOOTER_DISTANCE_ATTRIBUTE = "FooterDistance";
    /// <summary>
    /// Header distance attribute.
    /// </summary>
    private const string DEF_HEADER_DISTANCE_ATTRIBUTE = "HeaderDistance";
    /// <summary>
    /// Top margin attribute.
    /// </summary>
    private const string DEF_TOP_MARGIN_ATTRIBUTE = "TopMargin";
    /// <summary>
    /// Bottom margin attribute.
    /// </summary>
    private const string DEF_BOTTOM_MARGIN_ATTRIBUTE = "BottomMargin";
    /// <summary>
    /// Left margin attribute.
    /// </summary>
    private const string DEF_LEFT_MARGIN_ATTRIBUTE = "LeftMargin";
    /// <summary>
    /// Right margin attribute.
    /// </summary>
    private const string DEF_RIGHT_MARGIN_ATTRIBUTE = "RightMargin";
    /// <summary>
    /// Page break after attribute.
    /// </summary>
    private const string DEF_PAGE_BREAK_AFTER_ATTRIBUTE = "PageBreakAfter";
    /// <summary>
    /// Orientation attribute.
    /// </summary>
    private const string DEF_ORIENTATION_ATTRIBUTE = "Orientation";
    /// <summary>
    /// Paragraph format tag in dls format.
    /// </summary>
    private const string DEF_PARAGRAPH_FORMAT_START = "paragraph-format";
    /// <summary>
    /// Headers-footers section start tag.
    /// </summary>
    private const string DEF_HEADERS_FOOTERS_START = "headers-footers";
    /// <summary>
    /// Value of item type for table item.
    /// </summary>
    private const string DEF_ITEM_TYPE_TABLE = "Table";
    /// <summary>
    /// Start of even footer.
    /// </summary>
    private const string DEF_EVEN_FOOTER_START = "even-footer";
    /// <summary>
    /// Start of even footer.
    /// </summary>
    private const string DEF_ODD_FOOTER_START = "odd-footer";
    /// <summary>
    /// Start of even header.
    /// </summary>
    private const string DEF_EVEN_HEADER_START = "even-header";
    /// <summary>
    /// Start of odd header.
    /// </summary>
    private const string DEF_ODD_HEADER_START = "odd-header";
    /// <summary>
    /// Row height attribute.
    /// </summary>
    private const string DEF_ROW_HEIGHT_ATTRIBUTE = "RowHeight";
    /// <summary>
    /// Shadow color attribute.
    /// </summary>
    private const string DEF_TABLE_SHADOW_COLOR_ATTRIBUTE = "ShadingColor";
    /// <summary>
    /// Constant that is added to the column width to preserve place for borders.
    /// </summary>
    private const int DEF_BORDER_WIDTH = 1;
    /// <summary>
    /// Horizontal alignment attribute.
    /// </summary>
    private const string DEF_HALIGNMENT_ATTRIBUTE = "HrAlignment";
    /// <summary>
    /// Vertical alignment attribute.
    /// </summary>
    private const string DEF_VALIGNMENT_ATTRIBUTE = "VAlignment";
    /// <summary>
    /// Alignment attribute value for center alignment.
    /// </summary>
    private const string DEF_ALIGN_CENTER = "Center";
    /// <summary>
    /// Alignment attribute value for top alignment.
    /// </summary>
    private const string DEF_ALIGN_TOP = "Top";
    /// <summary>
    /// Alignment attribute value for bottom alignment.
    /// </summary>
    private const string DEF_ALIGN_BOTTOM = "Bottom";
    /// <summary>
    /// Alignment attribute value for middle (center) alignment.
    /// </summary>
    private const string DEF_ALIGN_MIDDLE = "Middle";
    /// <summary>
    /// Alignment attribute value for left alignment.
    /// </summary>
    private const string DEF_ALIGN_LEFT = "Left";
    /// <summary>
    /// Alignment attribute value for right alignment.
    /// </summary>
    private const string DEF_ALIGN_RIGHT = "Right";
    /// <summary>
    /// Alignment attribute value for justify alignment.
    /// </summary>
    private const string DEF_ALIGN_JUSTIFY = "Justify";
    #endregion

    #region Static readonly members
    /// <summary>
    /// Excel borders that are supported by DLS.
    /// </summary>
    private static readonly ExcelBordersIndex[] DEF_DLS_BORDERS = new ExcelBordersIndex[]
    {
      ExcelBordersIndex.EdgeBottom,
      ExcelBordersIndex.EdgeLeft,
      ExcelBordersIndex.EdgeRight,
      ExcelBordersIndex.EdgeTop,
    };
    /// <summary>
    /// Corresponding dls border names.
    /// </summary>
    private static readonly string[] DEF_DLS_BORDER_NAMES = new string[]
    {
      "Bottom",
      "Left",
      "Right",
      "Top",
    };
    /// <summary>
    /// Represents dls culture.
    /// </summary>
    private static readonly CultureInfo DLSCulture = CultureInfo.InvariantCulture;
    /// <summary>
    /// Border width for Hair border style.
    /// </summary>
    private static readonly string DEF_BORDER_WIDTH_HAIR = ( 0.25 ).ToString( DLSCulture );
    /// <summary>
    /// Border width for Thin border style.
    /// </summary>
    private static readonly string DEF_BORDER_WIDTH_THIN = ( 0.5 ).ToString( DLSCulture );
    /// <summary>
    /// Border width for Medium border style.
    /// </summary>
    private static readonly string DEF_BORDER_WIDTH_MEDIUM = ( 1 ).ToString( DLSCulture );
    /// <summary>
    /// Border width for Thick border style.
    /// </summary>
    private static readonly string DEF_BORDER_WIDTH_THICK = ( 2.25 ).ToString( DLSCulture );
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the DLSXmlSerializator class.
    /// </summary>
    public DLSXmlSerializator()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Saves workbook into writer.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="book">Workbook to save.</param>
    public void Serialize( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      writer.WriteStartElement( DEF_DLS_START );
      writer.WriteAttributeString( DEF_PROTECTION_ATTRIBUTE, DEF_PROTECTION_VALUE );
      SerializeStyles( writer, book );
      SerializeDocumentProperties( writer, book );
      SerializeSections( writer, book );
      //Serial
      writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes all required styles.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="book">Workbook to save.</param>
    private void SerializeStyles( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      writer.WriteStartElement( DEF_STYLES_START );
      
      writer.WriteStartElement( DEF_STYLE_START );

      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );
      writer.WriteAttributeString( DEF_NAME_ATTRIBUTE, "Normal" );
      writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, "ParagraphStyle" );

      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes all required document properties.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="book">Workbook to save.</param>
    private void SerializeDocumentProperties( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );
    }
    /// <summary>
    /// Serializes all required sections.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="book">Workbook to save.</param>
    private void SerializeSections( XmlWriter writer, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      writer.WriteStartElement( DEF_SECTIONS_START );

      IWorksheets sheets = book.Worksheets;

      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        SerializeWorksheet( writer, book.Worksheets[ i ] );
        // TODO: remove break and learn how to handle situations with bad page setup block.
        //break;
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes worksheet into xml writer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    private void SerializeWorksheet( XmlWriter writer, IWorksheet sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( ( ( WorksheetImpl )sheet ).IsEmpty ) return;

      writer.WriteStartElement( DEF_SECTION_START );
      writer.WriteAttributeString( DEF_BREAK_CODE_ATTRIBUTE, "NewPage" );

      // TODO: remove ( PageSetupImpl ) we have to use interface instead.
      SerializePageSettings( writer, ( PageSetupImpl )sheet.PageSetup );
      double dPageWidth = SerializeParagraphs( writer, sheet );
      SerializeHeaderFooter( writer, sheet, dPageWidth );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes page settings.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="pageSetup">Page setup.</param>
    private void SerializePageSettings( XmlWriter writer, PageSetupImpl pageSetup )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      writer.WriteStartElement( DEF_PAGE_SETTINGS_START );
      WriteAttribute( writer, DEF_PAGE_HEIGHT_ATTRIBUTE, pageSetup.PageHeight );
      WriteAttribute( writer, DEF_PAGE_WIDTH_ATTRIBUTE, pageSetup.PageWidth );
      WriteAttribute( writer, DEF_FOOTER_DISTANCE_ATTRIBUTE, pageSetup.FooterMargin, MeasureUnits.Inch );
      WriteAttribute( writer, DEF_HEADER_DISTANCE_ATTRIBUTE, pageSetup.FooterMargin, MeasureUnits.Inch );
      WriteAttribute( writer, DEF_TOP_MARGIN_ATTRIBUTE, pageSetup.FooterMargin, MeasureUnits.Inch );
      WriteAttribute( writer, DEF_BOTTOM_MARGIN_ATTRIBUTE, pageSetup.BottomMargin, MeasureUnits.Inch );
      WriteAttribute( writer, DEF_LEFT_MARGIN_ATTRIBUTE, pageSetup.LeftMargin, MeasureUnits.Inch );
      WriteAttribute( writer, DEF_RIGHT_MARGIN_ATTRIBUTE, pageSetup.RightMargin, MeasureUnits.Inch );
      writer.WriteAttributeString( DEF_ORIENTATION_ATTRIBUTE, pageSetup.Orientation.ToString() );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes paragraphs.
    /// </summary>
    /// <param name="writer">Writer to save worksheet into.</param>
    /// <param name="sheet">Worksheet to save.</param>
    /// <returns>Page width.</returns>
    private double SerializeParagraphs( XmlWriter writer, IWorksheet sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( DEF_PARAGRAPHS_START );

      INames names = sheet.Names;

      IRange printArea = ( names.Contains( PageSetupImpl.DEF_AREA_XlS ) )
      ? names[PageSetupImpl.DEF_AREA_XlS].RefersToRange
      : sheet.UsedRange;

      SizeF pageSize;
      SizeF headingsSize;

      MeasurePageArea( printArea, out pageSize, out headingsSize );

      ExcelOrder pageOrder = sheet.PageSetup.Order;

      // NOTE: now we are not supporting nested ranges collection.
      // If it is necessary we will add recursive algorithm for this.

      if( printArea is RangesCollection )
      {
        RangesCollection ranges = ( RangesCollection )printArea;
        for( int i = 0, len = ranges.Count; i < len; i++ )
        {
          IRange range = ranges[ i ];
          SerializeRange( writer, range, pageSize, headingsSize, pageOrder );
        }
      }
      else
      {
        SerializeRange( writer, printArea, pageSize, headingsSize, pageOrder );
      }

      writer.WriteEndElement();
      return pageSize.Width;
    }
    /// <summary>
    /// Measure page area.
    /// </summary>
    /// <param name="printArea">Print area.</param>
    /// <param name="pageSize">Output available page area size.</param>
    /// <param name="headingsSize">Headings size.</param>
    private void  MeasurePageArea( IRange printArea, out SizeF pageSize,
      out SizeF headingsSize )
    {
      pageSize = new SizeF( 0, 0 );
      headingsSize = new SizeF( 0, 0 );

      if( printArea == null )
        throw new ArgumentNullException( "printArea" );

      IWorksheet sheet = printArea.Worksheet;
      PageSetupImpl pageSetup = ( PageSetupImpl )sheet.PageSetup;
      double value = pageSetup.LeftMargin + pageSetup.RightMargin;
      value = ApplicationImpl.ConvertUnitsStatic( value, MeasureUnits.Inch, MeasureUnits.Point );
      pageSize.Width = ( float )( pageSetup.PageWidth - value );

      value = pageSetup.TopMargin + pageSetup.BottomMargin;
      value = ApplicationImpl.ConvertUnitsStatic( value, MeasureUnits.Inch, MeasureUnits.Point );
      pageSize.Height = ( float )( pageSetup.PageHeight - value );
      ExcelOrder pageOrder = pageSetup.Order;

      if( pageSetup.PrintHeadings )
      {
        IWorkbook book = sheet.Workbook;
        IStyle style = book.Styles[ "Normal" ];
        FontWrapper fontWrapper = ( FontWrapper )style.Font;
        FontImpl font = fontWrapper.Wrapped;

        int iMaxRow = printArea.LastRow;
        string strMaxRow = iMaxRow.ToString();
        headingsSize.Width = font.MeasureString( strMaxRow ).Width;
        headingsSize.Width = ( float )ApplicationImpl.ConvertUnitsStatic( headingsSize.Width,
          MeasureUnits.Pixel, MeasureUnits.Point );

        headingsSize.Height = ( float )sheet.StandardHeight;
          
        pageSize.Width -= headingsSize.Width;
        pageSize.Height -= headingsSize.Height;
      }
    }

    /// <summary>
    /// Serializes specified range.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="range">Range to serialize.</param>
    /// <param name="pageSize">Page size available for export.</param>
    /// <param name="headingsSize">Headings size.</param>
    /// <param name="pageOrder">Page order.</param>
    private void SerializeRange( XmlWriter writer, IRange range, SizeF pageSize, SizeF headingsSize,
      ExcelOrder pageOrder )
    {
//      int iFirstRow = range.Row;
//      int iFirstCol = range.Column;
//      int iLastRow = range.LastRow;
//      int iLastCol = range.LastColumn;
      int iFirstRow = -1;
      int iFirstCol = -1;
      int iLastRow = -1;
      int iLastCol = -1;
      int iRowId = -1;

      IWorksheet sheet = range.Worksheet;
      IPageSetup pageSetup = sheet.PageSetup;
      int iId = 0;
      
      while( FillNextPage( range, ref iFirstRow, ref iFirstCol, ref iLastRow, ref iLastCol,
        pageSize, pageOrder ) )
      {
        int iColCount = iLastCol - iFirstCol + 1;

        if( headingsSize.Width > 0 ) iColCount++;

        writer.WriteStartElement( DEF_PARAGRAPH_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, iId.ToString() );

        writer.WriteStartElement( DEF_PARAGRAPH_FORMAT_START );
        SerializeBoolAttribute( writer, DEF_PAGE_BREAK_AFTER_ATTRIBUTE, true );
        writer.WriteEndElement();

        writer.WriteStartElement( DEF_ITEMS_START );

        writer.WriteStartElement( DEF_ITEM_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );
        writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, DEF_ITEM_TYPE_TABLE );
        writer.WriteAttributeString( DEF_COLUMNS_COUNT_ATTRIBUTE, iColCount.ToString() );

        // Remove border from whole table. We need just border on the cell if they are present.
        WriteEmptyBorders( writer );

        writer.WriteStartElement( DEF_ROWS_START );

        if( headingsSize.Height > 0 )
        {
          iRowId = SerializeHeadingsRow( writer, sheet, headingsSize, iFirstCol,
            iLastCol, iRowId );
        }

        for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
        {
          iRowId = SerializeRow( writer, sheet, iRow, iFirstCol, iLastCol, iRowId, headingsSize );
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        iId++;
      }
    }
    /// <summary>
    /// Writes table format block with empty borders.
    /// </summary>
    /// <param name="writer">Writer to write into.</param>
    private void WriteEmptyBorders( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( DEF_TABLE_FORMAT_START );
      writer.WriteStartElement( DEF_BORDERS_START );
      
      for( int i = 0, len = DEF_DLS_BORDER_NAMES.Length; i < len; i++ )
      {
        string strBorderName = DEF_DLS_BORDER_NAMES[ i ];
        writer.WriteStartElement( strBorderName );
        writer.WriteAttributeString( DEF_BORDER_TYPE_ATTRIBUTE, DEF_BORDER_TYPE_NONE );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Writes borders for heading cells.
    /// </summary>
    /// <param name="writer">Writer to write borders info into.</param>
    private void WriteHeadingsBorders( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( DEF_TABLE_FORMAT_START );
      writer.WriteStartElement( DEF_BORDERS_START );
      
      for( int i = 0, len = DEF_DLS_BORDER_NAMES.Length; i < len; i++ )
      {
        string strBorderName = DEF_DLS_BORDER_NAMES[ i ];
        writer.WriteStartElement( strBorderName );
        writer.WriteAttributeString( DEF_BORDER_TYPE_ATTRIBUTE, DEF_BORDER_TYPE_SIGNLE );
        writer.WriteAttributeString( DEF_WIDTH_ATTRIBUTE, "1" );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Fills size of the next page to serialize.
    /// </summary>
    /// <param name="range">Range to serialize.</param>
    /// <param name="iFirstRow">First row of the previous page; -1 for the first page.</param>
    /// <param name="iFirstCol">First column of the previous page.</param>
    /// <param name="iLastRow">Last row of the previous page.</param>
    /// <param name="iLastCol">Last column of the previous page.</param>
    /// <param name="pageSize">Available page size.</param>
    /// <param name="pageOrder">Page order.</param>
    /// <returns>True if there is next page to serialize; false otherwise.</returns>
    private bool FillNextPage( IRange range, ref int iFirstRow, ref int iFirstCol,
      ref int iLastRow, ref int iLastCol, SizeF pageSize, ExcelOrder pageOrder )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      if( iLastRow == range.LastRow && iLastCol == range.LastColumn )
        return false;

      if( iFirstRow == -1 )
      {
        // Here we have to return first page to print.
        // TODO: optimize a little bit.
        iFirstRow = range.Row;
        iFirstCol = range.Column;
      }
      else if( pageOrder == ExcelOrder.DownThenOver )
      {
        if( iLastRow == range.LastRow )
        {
          iFirstRow = range.Row;
          iFirstCol = iLastCol + 1;
        }
        else
        {
          iFirstRow = iLastRow + 1;
        }
      }
      else
      {
        if( iLastCol == range.LastColumn )
        {
          iFirstCol = range.Column;
          iFirstRow = iLastRow + 1;
        }
        else
        {
          iFirstCol = iLastCol + 1;
        }
      }

      iLastRow = Math.Min( GetMaxRow( range.Worksheet, iFirstRow, pageSize.Height ), range.LastRow );
      iLastCol = Math.Min( GetMaxColumn( range.Worksheet, iFirstCol, pageSize.Width ), range.LastColumn );
      return true;
    }

    /// <summary>
    /// Returns maximum possible column that can fit into page.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="iFirstColumn">One-based index of the first column.</param>
    /// <param name="dPageWidth">Page width.</param>
    /// <returns>Last column that fits into page.</returns>
    private int GetMaxColumn( IWorksheet sheet, int iFirstColumn, double dPageWidth )
    {
      if( dPageWidth <= 0 )
        throw new ArgumentOutOfRangeException( "dPageWidth" );

      if( iFirstColumn <= 0 )
        throw new ArgumentOutOfRangeException( "iFirstColumn" );

      double dCurWidth = 0;
      int iLastColumn = iFirstColumn - 1;
      int iLastApproved = iFirstColumn;
      IVPageBreaks arrBreaks = sheet.VPageBreaks;

      while( dCurWidth <= dPageWidth )
      {
        iLastApproved = iLastColumn;
        iLastColumn++;

        if( iLastColumn > sheet.Workbook.MaxColumnCount )
          break;

        double dPoints = GetColumnWidth( sheet, iLastColumn );
        dCurWidth += dPoints;

        if( iLastColumn > iFirstColumn && arrBreaks.GetPageBreak( iLastColumn ) != null ) break;
      }

      //return iLastColumn - 2;
      return iLastApproved;
    }
    /// <summary>
    /// Returns maximum possible row23 that can fit into page.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="iFirstRow">One-based index of the first row.</param>
    /// <param name="dPageHeight">Page height.</param>
    /// <returns>Last row that fits into page.</returns>
    private int GetMaxRow( IWorksheet sheet, int iFirstRow, double dPageHeight )
    {
      if( dPageHeight <= 0 )
        throw new ArgumentOutOfRangeException( "dPageHeight" );

      if( iFirstRow <= 0 )
        throw new ArgumentOutOfRangeException( "iFirstRow" );

      double dCurHeight = 0;
      int iLastRow = iFirstRow - 1;
      int iLastApproved = iFirstRow;
      IHPageBreaks arrBreaks = sheet.HPageBreaks;
      int iMaxRowCount = sheet.Workbook.MaxRowCount;

      while( dCurHeight <= dPageHeight )
      {
        iLastApproved = iLastRow;
        iLastRow++;

        if( iLastRow > iMaxRowCount )
          break;

        int iPixels = sheet.GetRowHeightInPixels( iLastRow );
        double dPoints = ApplicationImpl.ConvertUnitsStatic( iPixels,
          MeasureUnits.Pixel, MeasureUnits.Point );

        dCurHeight += dPoints;

        if( iLastRow > iFirstRow && arrBreaks.GetPageBreak( iLastRow ) != null ) break;
      }

      return iLastApproved;
    }
    /// <summary>
    /// Serializes single row into writer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="sheet">Worksheet that contains required row.</param>
    /// <param name="iRow">Row index to serialize.</param>
    /// <param name="iFirstCol">First column to serialize.</param>
    /// <param name="iLastCol">Last column to serialize.</param>
    /// <param name="iRowId">Represents Row id.</param>
    /// <param name="headingsSize">Headings size.</param>
    /// <returns>Row id after serialization.</returns>
    private int SerializeRow( XmlWriter writer, IWorksheet sheet, int iRow,
      int iFirstCol, int iLastCol, int iRowId, SizeF headingsSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      double dRowHeight = sheet.GetRowHeightInPixels( iRow );

      if( dRowHeight > 0 )
      {
        iRowId++;

        dRowHeight = ApplicationImpl.ConvertUnitsStatic( dRowHeight,
          MeasureUnits.Pixel, MeasureUnits.Point );

        writer.WriteStartElement( DEF_ROW_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, iRowId.ToString() );

        WriteAttribute( writer, DEF_ROW_HEIGHT_ATTRIBUTE, dRowHeight );
        writer.WriteStartElement( DEF_CELLS_START );

        int iCellId = 0;
        float fWidth = headingsSize.Width;

        if( fWidth > 0 )
        {
          // Here we have to add one cell with row index.
          SerializeCell( writer, fWidth, iRow.ToString(), iCellId );
          iCellId++;
        }

        for( int iCol = iFirstCol; iCol <= iLastCol; iCol++, iCellId++ )
        {
          SerializeCell( writer, sheet, iRow, iCol, iCellId );
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
      }

      return iRowId;
    }
    /// <summary>
    /// Serializes headings row into writer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="sheet">Worksheet that contains required row.</param>
    /// <param name="headingsSize">Headings size</param>
    /// <param name="iFirstColumn">First column to serialize.</param>
    /// <param name="iLastColumn">Last column to serialize.</param>
    /// <param name="iRowId">Represents Row id.</param>
    /// <returns>Row id after serialization.</returns>
    private int SerializeHeadingsRow( XmlWriter writer, IWorksheet sheet, SizeF headingsSize,
      int iFirstColumn, int iLastColumn, int iRowId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      float fRowHeight = headingsSize.Height;
      float fRowWidth = headingsSize.Width;

      if( fRowHeight > 0 )
      {
        iRowId++;

        fRowHeight = ( float )ApplicationImpl.ConvertUnitsStatic( fRowHeight,
          MeasureUnits.Pixel, MeasureUnits.Point );

        writer.WriteStartElement( DEF_ROW_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, iRowId.ToString() );

        WriteAttribute( writer, DEF_ROW_HEIGHT_ATTRIBUTE, fRowHeight );
        writer.WriteStartElement( DEF_CELLS_START );
        int iCellId = 0;

        if( fRowWidth > 0 )
        {
          SerializeCell( writer, fRowWidth, string.Empty, iCellId );
          iCellId++;
        }

        for( int iColumn = iFirstColumn; iColumn <= iLastColumn;
          iColumn++, iCellId++ )
        {
          string strCellName = RangeImpl.GetColumnName( iColumn );
          double dWidth = GetColumnWidth( sheet, iColumn );
          SerializeCell( writer, dWidth, strCellName, iCellId );
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
      }

      return iRowId;
    }
    /// <summary>
    /// Serializes single cell into xml.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="sheet">Worksheet that contains required cell.</param>
    /// <param name="iRow">Row index of the cell to serialize.</param>
    /// <param name="iColumn">Column index of the cell to serialize.</param>
    /// <param name="iCellId">Represents Cell id.</param>
    private void SerializeCell( XmlWriter writer, IWorksheet sheet, int iRow,
      int iColumn, int iCellId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WorksheetImpl worksheet = ( WorksheetImpl )sheet;
      int iXFIndex = worksheet.GetXFIndex( iRow, iColumn );
      ExtendedFormatImpl xFormat = worksheet.ParentWorkbook.InnerExtFormats[ iXFIndex ];

      double fWidth = GetColumnWidth( sheet, iColumn );

      writer.WriteStartElement( DEF_CELL_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, iCellId.ToString() );
      WriteAttribute( writer, DEF_WIDTH_ATTRIBUTE, fWidth );

      // NOTE: usage of WorksheetImpl must be removed later, when API that
      // allows to work with IWorksheet cells without IRange objects will
      // be finished and enabled in IWorksheet interface.

      if( sheet.Contains( iRow, iColumn ) )
      {
        long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
        RichTextString rtfString = worksheet.CellRecords.GetRTFString( lCellIndex, false );
        SerializeRichTextString( writer, rtfString, xFormat );
      }

      // Here we have to serialize empty cell (only style data, borders and other stuff).
      SerializeTableFormat( writer, xFormat );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single cell into xml.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="dWidth"> Represents Cell width.</param>
    /// <param name="strCellValue">Cell value to serialize.</param>
    /// <param name="iCellId">Represents Cell id.</param>
    private void SerializeCell( XmlWriter writer, double dWidth,
      string strCellValue, int iCellId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strCellValue == null )
        throw new ArgumentNullException( "strCellValue" );

      writer.WriteStartElement( DEF_CELL_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, iCellId.ToString() );
      WriteAttribute( writer, DEF_WIDTH_ATTRIBUTE, dWidth );

      writer.WriteStartElement( DEF_PARAGRAPHS_START );
      writer.WriteStartElement( DEF_PARAGRAPH_START );

      writer.WriteStartElement( DEF_PARAGRAPH_FORMAT_START );
      writer.WriteAttributeString( DEF_HALIGNMENT_ATTRIBUTE, DEF_ALIGN_CENTER );
      writer.WriteEndElement();

      writer.WriteStartElement( DEF_ITEMS_START );
      writer.WriteStartElement( DEF_ITEM_START );
      writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, DEF_TEXT_RANGE_ATTRIBUTE );
      writer.WriteStartElement( DEF_TEXT_START );

      writer.WriteString( strCellValue );

      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();

      WriteHeadingsBorders( writer );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes table format.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="xFormat">Format to serialize.</param>
    private void SerializeTableFormat( XmlWriter writer, ExtendedFormatImpl xFormat )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( xFormat == null )
        throw new ArgumentNullException( "xFormat" );

      // NOTE: here we have to serialize borders and maybe some other properties.
      writer.WriteStartElement( DEF_TABLE_FORMAT_START );

      string strAlign = GetVAlignment( xFormat.VerticalAlignment );
      WriteAttribute( writer, DEF_VALIGNMENT_ATTRIBUTE, strAlign );

      if( !xFormat.IsDefaultColor )
      {
        string strColor = GetColorString( xFormat.Color );
        writer.WriteAttributeString( DEF_TABLE_SHADOW_COLOR_ATTRIBUTE, strColor );
      }

      SerializeBorders( writer, xFormat );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes borders collection.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="xFormat">Extended format that contains borders to serialize.</param>
    private void SerializeBorders( XmlWriter writer, ExtendedFormatImpl xFormat )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( xFormat == null )
        throw new ArgumentNullException( "xFormat" );

      writer.WriteStartElement( DEF_BORDERS_START );
      BorderImpl border = null;

      //foreach( IBorder border in borders )
      for( int i = 0, len = DEF_DLS_BORDERS.Length; i < len; i++ )
      {
        ExcelBordersIndex borderIndex = DEF_DLS_BORDERS[ i ];

        if( border == null )
        {
          border = new BorderImpl( xFormat.Application, xFormat, xFormat, borderIndex );
        }
        else
        {
          border.BorderIndex = borderIndex;
        }

        writer.WriteStartElement( DEF_DLS_BORDER_NAMES[ i ] );

        if( border.LineStyle != ExcelLineStyle.None )
        {
          writer.WriteAttributeString( DEF_COLOR_ATTRIBUTE, GetColorString( border.ColorRGB ) );
          writer.WriteAttributeString( DEF_LINE_WIDTH_ATTRIBUTE, GetLineWidth( border ) );
        }
        
        writer.WriteAttributeString( DEF_BORDER_TYPE_ATTRIBUTE, GetBorderType( border ) );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Returns string representing vertical alignment in dls format.
    /// </summary>
    /// <param name="align">Vertical alignment.</param>
    /// <returns>String representing vertical alignment in dls format.</returns>
    private string GetVAlignment( ExcelVAlign align )
    {
      switch( align )
      {
        case ExcelVAlign.VAlignTop:    return DEF_ALIGN_TOP;
        case ExcelVAlign.VAlignBottom: return DEF_ALIGN_BOTTOM;

        case ExcelVAlign.VAlignDistributed:
        case ExcelVAlign.VAlignCenter: return DEF_ALIGN_MIDDLE;

        case ExcelVAlign.VAlignJustify:
          //return "ERROR-UNKNOWN";
          return null;

        default:
          throw new ArgumentOutOfRangeException( "align" );
      }
    }
    /// <summary>
    /// Returns string representing horizontal alignment in dls format.
    /// </summary>
    /// <param name="align">Horizontal alignment.</param>
    /// <returns>String representing horizontal alignment in dls format.</returns>
    private string GetHAlignment( ExcelHAlign align )
    {
      switch( align )
      {
        case ExcelHAlign.HAlignCenter:
        case ExcelHAlign.HAlignCenterAcrossSelection:
          return DEF_ALIGN_CENTER;

        case ExcelHAlign.HAlignLeft:    return DEF_ALIGN_LEFT;
        case ExcelHAlign.HAlignJustify: return DEF_ALIGN_JUSTIFY;
        case ExcelHAlign.HAlignRight :  return DEF_ALIGN_RIGHT;
      }

      return null;
    }
    /// <summary>
    /// Returns line width for dls format.
    /// </summary>
    /// <param name="border">Border to get width from.</param>
    /// <returns>Line width for dls format.</returns>
    private string GetLineWidth( IBorder border )
    {
      switch( border.LineStyle )
      {
        case ExcelLineStyle.Hair:
          return DEF_BORDER_WIDTH_HAIR;

        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Dotted:
        case ExcelLineStyle.Double:
        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Dash_dot_dot:
        case ExcelLineStyle.Slanted_dash_dot:
          return DEF_BORDER_WIDTH_THIN;

        case ExcelLineStyle.Medium:
        case ExcelLineStyle.Medium_dashed:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          return DEF_BORDER_WIDTH_MEDIUM;

        case ExcelLineStyle.Thick:
          return DEF_BORDER_WIDTH_THICK;

        case ExcelLineStyle.None:
        default:
          return DEF_BORDER_WIDTH_NONE;
      }
    }
    /// <summary>
    /// Returns border type for dls format.
    /// </summary>
    /// <param name="border">Border to get type from.</param>
    /// <returns>Border type for dls format.</returns>
    private string GetBorderType( IBorder border )
    {
      switch( border.LineStyle )
      {
        case ExcelLineStyle.Hair:
        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Medium:
          return DEF_BORDER_TYPE_SIGNLE;

        case ExcelLineStyle.Double:
          return DEF_BORDER_TYPE_DOUBLE;

        case ExcelLineStyle.Dotted:
          return DEF_BORDER_TYPE_DOT;

        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Medium_dashed:
          return DEF_BORDER_TYPE_DASH_SMALL;

        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Slanted_dash_dot:
          return DEF_BORDER_TYPE_DOT_DASH;

        case ExcelLineStyle.Dash_dot_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          return DEF_BORDER_TYPE_DOT_DOT_DASH;

        case ExcelLineStyle.Thick:
          return DEF_BORDER_TYPE_THICK;

        case ExcelLineStyle.None:
        default:
          return DEF_BORDER_TYPE_NONE;
      }
    }
    /// <summary>
    /// Serializes rtf string.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="rtfString">RTF string to serialize.</param>
    /// <param name="xFormat">Cell extended format.</param>
    private void SerializeRichTextString( XmlWriter writer, RichTextString rtfString, ExtendedFormatImpl xFormat )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( rtfString == null )
        return;

      // TODO: finish rtf string serialization. Now no formatting data is serialized.
      // TODO: hidden symbols are visible in the resulting string.

      writer.WriteStartElement( DEF_PARAGRAPHS_START );
      writer.WriteStartElement( DEF_PARAGRAPH_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );

      writer.WriteStartElement( DEF_PARAGRAPH_FORMAT_START );

      string strAlign = GetHAlignment( xFormat.HorizontalAlignment );
      WriteAttribute( writer, DEF_HALIGNMENT_ATTRIBUTE, strAlign );

      writer.WriteEndElement();

      writer.WriteStartElement( DEF_ITEMS_START );

      TextWithFormat text = rtfString.TextObject;
      int iFormattingRunsCount = text.FormattingRunsCount;
      string strText = text.Text;
      string strTextPart;
      WorkbookImpl book = rtfString.Workbook;
      FontsCollection arrFonts = book.InnerFonts;
      int iItemId = 0;

      if( iFormattingRunsCount > 0 )
      {
        int iLength = text.Text.Length;
        int i = 0;

        int iCurPos = text.GetPositionByIndex( 0 );

        if( iCurPos != 0 )
        {
          // Here we have to serialize start of the string.
          strTextPart = strText.Substring( 0, iCurPos );
          WriteText( writer, strTextPart, rtfString.DefaultFont, iItemId );
          iItemId++;
        }

        for( ; i < iFormattingRunsCount; i++, iItemId++ )
        {
          int iFontIndex = text.GetFontByIndex( i );
          iCurPos = text.GetPositionByIndex( i );
          int iEndPos = ( i != iFormattingRunsCount - 1 )
            ? text.GetPositionByIndex( i + 1 )
            : iLength;

          strTextPart = strText.Substring( iCurPos, iEndPos - iCurPos );
          IFont font = arrFonts[ iFontIndex ];
          WriteText( writer, strTextPart, font, iItemId );
        }
      }
      else
      {
        WriteText( writer, strText, rtfString.DefaultFont, iItemId );
      }

      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    /// <summary>
    /// Writes text block into XmlWriter.
    /// </summary>
    /// <param name="writer">Writer to save into.</param>
    /// <param name="strText">Text to write.</param>
    /// <param name="font">Font of the text to write.</param>
    /// <param name="id">Represents Item id.</param>
    private void WriteText( XmlWriter writer, string strText, IFont font, int id )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strText == null )
        throw new ArgumentNullException( "strText" );

      writer.WriteStartElement( DEF_ITEM_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, id.ToString() );

      writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, DEF_TEXT_RANGE_ATTRIBUTE );

      if( font != null && strText.Length > 0 )
      {
        WriteFont( writer, font );
        // TODO: Here we have to write font into writer.
      }

      writer.WriteStartElement( DEF_TEXT_START );
      writer.WriteString( strText );
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Writes font into xml writer.
    /// </summary>
    /// <param name="writer">Writer to save font into.</param>
    /// <param name="font">Font to save.</param>
    private void WriteFont( XmlWriter writer, IFont font )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( font == null )
        throw new ArgumentNullException( "font" );

      writer.WriteStartElement( DEF_CHARACTER_FORMAT_START );//DEF_FORMAT_START );
      writer.WriteAttributeString( DEF_FONT_NAME_ATTRIBUTE, font.FontName.ToString() );
      writer.WriteAttributeString( DEF_FONT_SIZE_ATTRIBUTE, font.Size.ToString() );
      writer.WriteAttributeString( DEF_TEXT_COLOR_ATTRIBUTE, GetColorString( font.RGBColor ) );
      SerializeBoolAttribute( writer, DEF_BOLD_ATTRIBUTE, font.Bold );
      SerializeBoolAttribute( writer, DEF_ITALIC_ATTRIBUTE, font.Italic );
      writer.WriteAttributeString( DEF_UNDERLINE_ATTRIBUTE, GetUnderlineString( font.Underline ) );
      writer.WriteAttributeString( DEF_SUBSUPERSCRIPT_ATTRIBUTE, GetSubSuperScript( font ) );
      SerializeBoolAttribute( writer, DEF_STRIKEOUT_ATTRIBUTE, font.Strikethrough );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes boolean attribute.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="strAttributeName">Attribute name.</param>
    /// <param name="bValue">Attribute value.</param>
    private void SerializeBoolAttribute( XmlWriter writer, string strAttributeName, bool bValue )
    {
      if( bValue )
      {
        if( writer == null )
          throw new ArgumentNullException( "writer" );

        if( strAttributeName == null )
          throw new ArgumentNullException( "strAttributeName" );

        if( strAttributeName.Length == 0 )
          throw new ArgumentException( "strAttributeName - string cannot be empty." );

        writer.WriteAttributeString( strAttributeName, DEF_TRUE_STRING );
      }
    }
    /// <summary>
    /// Converts color into string that is understandable by dls.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <returns>Color string representation in dls format.</returns>
    private string GetColorString( Color color )
    {
      int iColor = color.ToArgb();
      return DEF_COLOR_PREFIX + iColor.ToString( "X" );
    }

    /// <summary>
    /// Converts excel underline into string in dls format.
    /// </summary>
    /// <param name="underline">Underline type to convert.</param>
    /// <returns>String corresponding to the underline.</returns>
    private string GetUnderlineString( ExcelUnderline underline )
    {
      switch( underline )
      {
        case ExcelUnderline.Double:
        case ExcelUnderline.DoubleAccounting:
          return DEF_UNDERLINE_DOUBLE;

        case ExcelUnderline.Single:
        case ExcelUnderline.SingleAccounting:
          return DEF_UNDERLINE_SINGLE;

        case ExcelUnderline.None:
        default:
          return DEF_UNDERLINE_NONE;
      }
    }
    /// <summary>
    /// Converts font into SubSuperScript string.
    /// </summary>
    /// <param name="font">Font to get settings from.</param>
    /// <returns>SubSuperScript string in dls format.</returns>
    private string GetSubSuperScript( IFont font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( font.Subscript ) return DEF_SUBCRIPT;

      if( font.Superscript ) return DEF_SUPSCRIPT;

      return DEF_NO_SUBSUPERSCIRPT;
    }
    /// <summary>
    /// Writes attribute string, converts value into points.
    /// </summary>
    /// <param name="writer">Writer to write attribute into.</param>
    /// <param name="strAttributeName">Attribute name.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="units">Units in which value is specified.</param>
    private void WriteAttribute( XmlWriter writer, string strAttributeName, double value, MeasureUnits units )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strAttributeName == null )
        throw new ArgumentNullException( "strAttributeName" );

      if( strAttributeName.Length == 0 )
        throw new ArgumentException( "strAttributeName - string cannot be empty." );

      value = ApplicationImpl.ConvertUnitsStatic( value, units, MeasureUnits.Point );
      string strValue = value.ToString( DLSCulture );
      writer.WriteAttributeString( strAttributeName, strValue );
    }
    /// <summary>
    /// Writes attribute string.
    /// </summary>
    /// <param name="writer">Writer to write attribute into.</param>
    /// <param name="strAttributeName">Attribute name.</param>
    /// <param name="strValue">Value to write.</param>
    private void WriteAttribute( XmlWriter writer, string strAttributeName, string strValue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strAttributeName == null )
        throw new ArgumentNullException( "strAttributeName" );

      if( strAttributeName.Length == 0 )
        throw new ArgumentException( "strAttributeName - string cannot be empty." );

      if( strValue == null || strValue.Length == 0 ) return;
      writer.WriteAttributeString( strAttributeName, strValue );
    }
    /// <summary>
    /// Writes attribute string.
    /// </summary>
    /// <param name="writer">Writer to write attribute into.</param>
    /// <param name="strAttributeName">Attribute name.</param>
    /// <param name="value">Value to write.</param>
    private void WriteAttribute( XmlWriter writer, string strAttributeName, double value )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strAttributeName == null )
        throw new ArgumentNullException( "strAttributeName" );

      if( strAttributeName.Length == 0 )
        throw new ArgumentException( "strAttributeName - string cannot be empty." );

      string strValue = value.ToString( DLSCulture );
      writer.WriteAttributeString( strAttributeName, strValue );
    }
    /// <summary>
    /// Serializes header and footer into XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="sheet">Sheet with footer/header to serialize.</param>
    /// <param name="dPageWidth">Available page width.</param>
    private void SerializeHeaderFooter( XmlWriter writer, IWorksheet sheet, double dPageWidth )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      writer.WriteStartElement( DEF_HEADERS_FOOTERS_START );

      IPageSetup pageSetup = sheet.PageSetup;
      string[] arrStrings = new string[ 3 ];
      arrStrings[ 0 ] = pageSetup.LeftHeader;
      arrStrings[ 1 ] = pageSetup.RightHeader;
      arrStrings[ 2 ] = pageSetup.CenterHeader;

      writer.WriteStartElement( DEF_ODD_HEADER_START );
      SerializeHeaderFooter( writer, arrStrings, dPageWidth );
      writer.WriteEndElement();

      arrStrings[ 0 ] = pageSetup.LeftFooter;
      arrStrings[ 1 ] = pageSetup.RightFooter;
      arrStrings[ 2 ] = pageSetup.CenterFooter;

      writer.WriteStartElement( DEF_ODD_FOOTER_START );
      SerializeHeaderFooter( writer, arrStrings, dPageWidth );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes header footer.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="arrValues">Header/footer values.</param>
    /// <param name="dPageWidth">Page width.</param>
    private void SerializeHeaderFooter( XmlWriter writer, string[] arrValues,
      double dPageWidth )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( arrValues == null )
        throw new ArgumentNullException( "arrValues" );

      int iCount = arrValues.Length;

      if( iCount == 0 ) return;
      int iFullLength = 0;

      for( int i = 0; i < iCount; i++ )
      {
        iFullLength += arrValues[ i ].Length;
      }

      if( iFullLength == 0 ) return;

      writer.WriteStartElement( DEF_PARAGRAPHS_START );
      writer.WriteStartElement( DEF_PARAGRAPH_START );
      writer.WriteStartElement( DEF_ITEMS_START );

      writer.WriteStartElement( DEF_ITEM_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );
      writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, DEF_ITEM_TYPE_TABLE );
      writer.WriteAttributeString( DEF_COLUMNS_COUNT_ATTRIBUTE, iCount.ToString() );

      WriteEmptyBorders( writer );

      writer.WriteStartElement( DEF_ROWS_START );
      writer.WriteStartElement( DEF_ROW_START );
      writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );

      writer.WriteStartElement( DEF_CELLS_START );
      double dWidth = dPageWidth / iCount;

      for( int i = 0; i < iCount; i++ )
      {
        writer.WriteStartElement( DEF_CELL_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, i.ToString() );
        WriteAttribute( writer, DEF_WIDTH_ATTRIBUTE, dWidth );

        writer.WriteStartElement( DEF_PARAGRAPHS_START );
        writer.WriteStartElement( DEF_PARAGRAPH_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );
        
        writer.WriteStartElement( DEF_ITEMS_START );
        writer.WriteStartElement( DEF_ITEM_START );
        writer.WriteAttributeString( DEF_ID_ATTRIBUTE, "0" );
        writer.WriteAttributeString( DEF_TYPE_ATTRIBUTE, DEF_TEXT_RANGE_ATTRIBUTE );
        writer.WriteStartElement( DEF_TEXT_START );
        writer.WriteString( arrValues[ i ] );
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Evaluates column width.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="iColumn">Column index.</param>
    /// <returns>Column width in points.</returns>
    private double GetColumnWidth( IWorksheet sheet, int iColumn )
    {
      int iPixels = sheet.GetColumnWidthInPixels( iColumn );
      // NOTE: we are adding some constant to column width because it looks like
      // MS Excel and OpenOffice reserves some place for border.
      double dPoints = ApplicationImpl.ConvertUnitsStatic( iPixels,
        MeasureUnits.Pixel, MeasureUnits.Point ) + DEF_BORDER_WIDTH;

      return dPoints;
    }
    #endregion
  }
}
