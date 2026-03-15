#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Globalization;
using System.Xml;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using CellType = Syncfusion.XlsIO.Implementation.XmlSerialization.WorkbookXmlSerializator.XmlSerializationCellType;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Text;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Implementation.XmlReaders
{
  /// <summary>
  /// This class is responsible for xml spreadsheets parsing.
  /// </summary>
  public class MSXmlReader
    : CommonObject
  {
    #region Class constants
    /// <summary>
    /// Represents current xml version.
    /// </summary>
    private const string DEF_VERSION_STRING = "version=\"1.0\"";
    /// <summary>
    /// Represents current xml version.
    /// </summary>
    private const string DEF_XML_STRING = "xml";
    /// <summary>
    /// Represents current application version.
    /// </summary>
    private const string DEF_APPLICATION_STRING = "progid=\"Excel.Sheet\"";
    /// <summary>
    /// Represents current application node name.
    /// </summary>
    private const string DEF_APPLICATION_NAME_STRING = "mso-application";
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
    /// Represents xml subscript value.
    /// </summary>
    private const string DEF_SUBSCRIPT = "Subscript";
    /// <summary>
    /// Represents xml superscript value.
    /// </summary>
    private const string DEF_SUPERSCRIPT = "Superscript";
    /// <summary>
    /// Represents rtf bold.
    /// </summary>
    private const string DEF_RTF_BOLD = "B";
    /// <summary>
    /// Represents rtf Italic.
    /// </summary>
    private const string DEF_RTF_ITALIC = "I";
    /// <summary>
    /// Represents rtf Underline.
    /// </summary>
    private const string DEF_RTF_UNDERLINE = "U";
    /// <summary>
    /// Represents rtf strikethrough.
    /// </summary>
    private const string DEF_RTF_STRIKETHROUGH = "S";
    /// <summary>
    /// Represents rtf span.
    /// </summary>
    private const string DEF_RTF_SPAN = "Span";
    /// <summary>
    /// Represents rtf sub.
    /// </summary>
    private const string DEF_RTF_SUB = "Sub";
    /// <summary>
    /// Represents rtf sup.
    /// </summary>
    private const string DEF_RTF_SUP = "Sup";
    /// <summary>
    /// Represents rtf font.
    /// </summary>
    private const string DEF_RTF_FONT = "Font";
    /// <summary>
    /// Represents default font size.
    /// </summary>
    private const int DEF_SIZE_FONT = 10;
    /// <summary>
    /// Represents none constant.
    /// </summary>
    private const string DEF_NONE = "None";
    /// <summary>
    /// Default font name. Used when font name is not specified.
    /// </summary>
    private const string DefaultFontName = "Arial";
    /// <summary>
    /// Represents current version of xml file
    /// </summary>
    private const string VersionAttribute = "version";
    /// <summary>
    /// represent default Xml Version
    /// </summary>
    private const string DefaultVersion = "1.0";
    #endregion

    #region Class members
    /// <summary>
    /// Represents style dictionary.
    /// Key - Recovered from xml stream unique id, value - XF index in workbook collection.
    /// </summary>
    private Dictionary<string, int> m_hashStyle = new Dictionary<string, int>();
    /// <summary>
    /// Represents parent workbook.
    /// </summary>
    private WorkbookImpl m_parentBook;
    /// <summary>
    /// Represents array list, that contain formula string to reparse.
    /// </summary>
    private List<string> m_arrNames = new List<string>();
    /// <summary>
    /// Represents array with formulas to reparse.
    /// </summary>
    private Dictionary<long, FormulaData> m_hashFormula = new Dictionary<long, FormulaData>();
    /// <summary>
    /// Formula utils.
    /// </summary>
    private FormulaUtil m_formulaUtil;
    #endregion

    #region Class static members
    /// <summary>
    /// Represents dictionary with alignment horizontal types.
    /// </summary>
    private static Dictionary<string, ExcelHAlign> m_hashHorizontalAll = new Dictionary<string, ExcelHAlign>( 9 );
    /// <summary>
    /// Represents hashtable with alignment vertical types.
    /// </summary>
    private static Dictionary<string, ExcelVAlign> m_hashVerticalAll = new Dictionary<string, ExcelVAlign>( 7 );
    /// <summary>
    /// Represents dictionary with number format types.
    /// </summary>
    private static Dictionary<string, string> m_hashNumberFormat = new Dictionary<string, string>( 10 );
    #endregion

    #region Class static constructors
    /// <summary>
    /// Initialize all static collections.
    /// </summary>
    static MSXmlReader()
    {
      #region Init Allign hash.
      //Init alignment horizontal hash.
      m_hashHorizontalAll.Add( "Automatic", ExcelHAlign.HAlignGeneral );
      m_hashHorizontalAll.Add( "Left", ExcelHAlign.HAlignLeft );
      m_hashHorizontalAll.Add( "Center", ExcelHAlign.HAlignCenter );
      m_hashHorizontalAll.Add( "Right", ExcelHAlign.HAlignRight );
      m_hashHorizontalAll.Add( "Fill", ExcelHAlign.HAlignFill );
      m_hashHorizontalAll.Add( "Justify", ExcelHAlign.HAlignJustify );
      m_hashHorizontalAll.Add( "CenterAcrossSelection", ExcelHAlign.HAlignCenterAcrossSelection );
      m_hashHorizontalAll.Add( "Distributed", ExcelHAlign.HAlignDistributed );
      m_hashHorizontalAll.Add( "JustifyDistributed", ExcelHAlign.HAlignGeneral );

      //Nit alignment vertical hash.
      m_hashVerticalAll.Add( "Automatic", ExcelVAlign.VAlignBottom );
      m_hashVerticalAll.Add( "Top", ExcelVAlign.VAlignTop );
      m_hashVerticalAll.Add( "Bottom", ExcelVAlign.VAlignBottom );
      m_hashVerticalAll.Add( "Center", ExcelVAlign.VAlignCenter );
      m_hashVerticalAll.Add( "Justify", ExcelVAlign.VAlignJustify );
      m_hashVerticalAll.Add( "Distributed", ExcelVAlign.VAlignDistributed );
      m_hashVerticalAll.Add( "JustifyDistributed", ExcelVAlign.VAlignBottom );

      #endregion

      //Inin number format hash.
      m_hashNumberFormat.Add( "Fixed", @"0.00" );
      m_hashNumberFormat.Add( "Standard", @"#,##0.00" );
      m_hashNumberFormat.Add( "Percent", @"0.00%" );
      m_hashNumberFormat.Add( "Scientific", @"0.00E+0" );
      m_hashNumberFormat.Add( "Short Date", @"m/d/yyyy" );
      m_hashNumberFormat.Add( "Medium Date", @"d\-mmm\-yy" );
      m_hashNumberFormat.Add( "Medium Time", @"h:mm AM/PM" );
      m_hashNumberFormat.Add( "Long Time", @"h:mm:ss AM/PM" );
      m_hashNumberFormat.Add( "Short Time", @"h:mm" );
      m_hashNumberFormat.Add( "General Date", @"m/d/yy h:mm" );
    }
    #endregion

    #region Class initialization methods
    /// <summary>
    /// Initialize new instance of current class.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object for class.</param>
    public MSXmlReader( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class reading methods

    #region Worksheet reading methods
    /// <summary>
    /// Gets worksheet from xml. 
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="book">Workbook to fill.</param>
    /// <returns>Returns new instance of WorkSheet.</returns>
    private void ReadWorksheet( XmlReader reader, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      WorksheetImpl sheet;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_NAME_PREF, DEF_SS_NAMESPACE )
        && reader.Value != null && reader.Value.Length > 0 )
      {
        sheet = ( WorksheetImpl )book.Worksheets.Create( reader.Value );
      }
      else
      {
        throw new XmlReadingException( "Worksheet", "Worksheet name attribute isn't specified." );
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_RIGHTTOLEFT_PREF, DEF_SS_NAMESPACE ) )
        sheet.IsRightToLeft = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_TABLE_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadTable( reader, sheet );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_NAMES_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadNames( reader, sheet.Names, sheet.Index + 1 );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_WORKSHEET_OPTIONS_PREF && reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadWorksheetOptions( reader, sheet );

        if (reader.LocalName == WorkbookXmlSerializator.DEF_DATAVALIDATION_PREF) 
            ReadDataValidation(reader, sheet);

        if (reader.LocalName == WorkbookXmlSerializator.DEF_CONDITIONAL_FORMATTING_PREF)
            ReadConditionalFormats(reader, sheet);

        reader.Skip();
      }
    }
    /// <summary>
    /// Read table from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadTable( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      //sheet.FirstColumn = 1;
      //sheet.LastColumn = sheet.Workbook.MaxColumnCount - 1;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_DEFAULTROWHEIGHT_PREF, DEF_SS_NAMESPACE ) )
      {
        sheet.StandardHeight = XmlConvert.ToDouble( reader.Value );
        sheet.StandardHeightFlag = true;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_DEFAULTCOLUMNWIDTH_PREF, DEF_SS_NAMESPACE ) )
      {
        double dWidth = XmlConvert.ToDouble( reader.Value );
        dWidth = Application.ConvertUnits( ( float )dWidth, MeasureUnits.Point, MeasureUnits.Pixel );
        dWidth = sheet.PixelsToColumnWidth((int)dWidth);
        sheet.StandardWidth = dWidth;
      }

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      int iRowIndex = 0;
      int iColIndex = 0;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_ROW_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          iRowIndex = ReadRow( reader, sheet, iRowIndex );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_COLUMN_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          iColIndex = ReadColumn( reader, sheet, iColIndex );

        reader.Skip();
      }
    }
    /// <summary>
    /// Reads row from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Sheet that contain current row.</param>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <returns>Returns updated row index.</returns>
    private int ReadRow( XmlReader reader,  WorksheetImpl sheet, int iRowIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      bool bAutoFit = false;
      int iSpan = 0;
      double dRowHeight = sheet.StandardHeight;
      int iStyleIndex = sheet.ParentWorkbook.DefaultXFIndex;
      int iColumnIndex = 0;

      iRowIndex = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_INDEX_PREF, DEF_SS_NAMESPACE ) )
        ? XmlConvert.ToInt32( reader.Value ) : ++iRowIndex;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HEIGHT_PREF, DEF_SS_NAMESPACE ) )
        dRowHeight = XmlConvert.ToDouble( reader.Value );

      bool bHidden = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HIDDEN_PREF, DEF_SS_NAMESPACE ) )
        ? XmlConvert.ToBoolean( reader.Value ) : false;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_STYLEID_PREF, DEF_SS_NAMESPACE ) )
        iStyleIndex = m_hashStyle[ reader.Value ];

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_AUTOFIT_HEIGHT_PREF, DEF_SS_NAMESPACE ) )
        bAutoFit = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SPAN_PREF, DEF_SS_NAMESPACE ) )
        iSpan = XmlConvert.ToInt32( reader.Value );

      for( int i = iRowIndex, iLen = iRowIndex + iSpan; i <= iLen; i++ )
      {
        RowStorage rowStorage = WorksheetHelper.GetOrCreateRow( sheet, i - 1, true );
        //RowRecord row = rowStorage.RowInformation;
        //rowStorage.RowNumber = ( ushort )( i - 1 );
        //rowStorage.FirstColumn = WorkbookXmlSerializator.DEF_MIN_COLUMN;
        //rowStorage.LastColumn = WorkbookXmlSerializator.DEF_MAX_COLUMN;
        dRowHeight = Math.Min( dRowHeight, RowRecord.DEF_MAX_HEIGHT );
        rowStorage.Height = ( ushort )( dRowHeight * WorkbookXmlSerializator.DEF_ROW_DIV );
        rowStorage.IsBadFontHeight = true;
        rowStorage.ExtendedFormatIndex = ( ushort )iStyleIndex;
        rowStorage.IsHidden = bHidden;

        if( sheet.FirstRow < 0 || sheet.FirstRow > i ) sheet.FirstRow = i;
        if( sheet.LastRow < i ) sheet.LastRow = i;

        //sheet.RowInformation.Add( i, row );
        //row = ( RowRecord )row.Clone();
      }

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return iRowIndex + iSpan;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_CELL_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          iColumnIndex = ReadCell( reader, sheet, iRowIndex, iColumnIndex );

        reader.Skip();
      }

      if( bAutoFit && dRowHeight == sheet.StandardHeight )
        sheet.AutofitRow( iRowIndex );

      return iRowIndex + iSpan;
    }
    /// <summary>
    /// Read column info.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Represents current worksheet.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Returns updated column index.</returns>
    private int ReadColumn( XmlReader reader,  WorksheetImpl sheet, int iColumnIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      double dWidth = sheet.StandardWidth * WorkbookXmlSerializator.DEF_COLUMN_DIV;
      bool bIsHidden = false;
      bool bIsAutoFit = false;
      int iSpan = 0;
      int iStyleIndex = sheet.ParentWorkbook.DefaultXFIndex;

      iColumnIndex = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_INDEX_PREF, DEF_SS_NAMESPACE ) )
        ? XmlConvert.ToInt32( reader.Value ) : ++iColumnIndex;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_WIDTH_PREF, DEF_SS_NAMESPACE ) )
      {
        dWidth = XmlConvert.ToDouble( reader.Value );
        dWidth = Application.ConvertUnits( ( float )dWidth, MeasureUnits.Point, MeasureUnits.Pixel );
        dWidth = sheet.PixelsToColumnWidth( ( int )dWidth ) * WorkbookXmlSerializator.DEF_COLUMN_DIV;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_AUTOFIT_WIDTH_PREF, DEF_SS_NAMESPACE ) )
        bIsAutoFit = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HIDDEN_PREF, DEF_SS_NAMESPACE ) )
        bIsHidden = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_STYLEID_PREF, DEF_SS_NAMESPACE ) )
        iStyleIndex = m_hashStyle[ reader.Value ];

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SPAN_PREF, DEF_SS_NAMESPACE ) )
        iSpan = XmlConvert.ToInt32( reader.Value );

      for( int i = iColumnIndex, iLen = iColumnIndex + iSpan; i <= iLen; i++ )
      {
        ColumnInfoRecord coll = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
        coll.FirstColumn = ( ushort )( i - 1 );
        coll.LastColumn = ( ushort )( i - 1 );
        coll.IsHidden = bIsHidden;
        coll.ExtendedFormatIndex = ( ushort )iStyleIndex;
        coll.ColumnWidth = ( ushort )dWidth;

        //sheet.ColumnInformation.Add( i, coll );
        sheet.ColumnInformation[ i ] = coll;
      }

      if( bIsAutoFit && dWidth == sheet.StandardWidth )
        sheet.AutofitColumn( iColumnIndex );

      reader.MoveToElement();

      return iColumnIndex + iSpan;
    }
    /// <summary>
    /// Read cell from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Current sheet.</param>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <param name="iIndex">Represents column index.</param>
    private int ReadCell( XmlReader reader, WorksheetImpl sheet, int iRowIndex, int iIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      iIndex = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_INDEX_PREF, DEF_SS_NAMESPACE ) )
        ? XmlConvert.ToInt32( reader.Value ) : ++iIndex;

      int iStyleIndex = sheet.ParentWorkbook.DefaultXFIndex;
      CellRecordCollection cells = sheet.CellRecords;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_STYLEID_PREF, DEF_SS_NAMESPACE ) )
      {
        iStyleIndex = GetXFIndex( sheet, reader.Value );
      }
      else
      {
        ColumnInfoRecord columnInfoRecord = sheet.ColumnInformation[iIndex];
        int rowStyleIndex = (sheet.GetDefaultRowStyle(iRowIndex) as ExtendedFormatWrapper).XFormatIndex;
        if( rowStyleIndex!=iStyleIndex)
        {
           iStyleIndex = (sheet.GetDefaultRowStyle(iRowIndex) as ExtendedFormatWrapper).XFormatIndex;
        }
        else if (columnInfoRecord != null)
        {
            iStyleIndex = columnInfoRecord.ExtendedFormatIndex;
        }
        
      }
      int iMergeAcross = ReadMerge( reader, sheet, iRowIndex - 1, iIndex - 1, iStyleIndex );

      string strFormula = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_FORMULA_PREF, DEF_SS_NAMESPACE ) ) ?
        reader.Value :
        null;

      ParseHyperlink( reader, sheet, iRowIndex, iIndex );
      reader.MoveToElement();

      CellType cellType = CellType.Number;
      TextWithFormat rtf = null;
      string cellValue = null;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == WorkbookXmlSerializator.DEF_DATA_PREF
            && reader.NamespaceURI == DEF_SS_NAMESPACE )
          {
            cellValue = ReadData( reader, cells, iStyleIndex, out cellType, out rtf );
          }

          if( reader.LocalName == WorkbookXmlSerializator.DEF_COMMENT_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          {
            CommentShapeImpl comment = ( CommentShapeImpl )sheet.InnerComments.AddComment( iRowIndex, iIndex );
            ReadComment( reader, comment, iStyleIndex );
          }

          reader.Skip();
        }
      }

      if( strFormula != null )
      {
        ParseFormula( sheet, iRowIndex, iIndex, strFormula, iStyleIndex, cellValue, cellType );
      }
      else if( cellValue != null )
      {
        SetCellRecord( cellType, cellValue, cells, iRowIndex, iIndex, iStyleIndex, rtf );
      }
      else
      {
        cells.SetBlank( iRowIndex, iIndex, iStyleIndex );
      }

      return iIndex + iMergeAcross;
    }
    /// <summary>
    /// Parses hyperlink data.
    /// </summary>
    /// <param name="reader">XmlReader to get necessary data from.</param>
    /// <param name="sheet">Worksheet that is being parsed.</param>
    /// <param name="row">Zero-based row index of the cell.</param>
    /// <param name="column">Zero-based column index of the cell.</param>
    private void ParseHyperlink( XmlReader reader, WorksheetImpl sheet, int row, int column )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      string hLinkAddress = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HREF_PREF, DEF_SS_NAMESPACE ) ) ?
        reader.Value :
        null;

      string hLinkScreenTip = ( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HYPRER_TIP_PREF, DEF_X_NAMESPACE ) ) ?
        reader.Value :
        null;  

      if( hLinkAddress != null )
      {
        IRange range = sheet[ row, column ];
        HyperLinkImpl link = ( HyperLinkImpl )sheet.HyperLinks.Add( range );
        string strTemp;

        if( FormulaUtil.IsCell3D( hLinkAddress, false, out strTemp, out strTemp, out strTemp ) ||
          m_formulaUtil.IsCellRange3D( hLinkAddress, false, out strTemp, out strTemp, out strTemp, out strTemp, out strTemp ) )
        {
          link.Type = ExcelHyperLinkType.Workbook;
        }
        else if( hLinkAddress.StartsWith( @"\\" ) )
        {
          link.Type = ExcelHyperLinkType.Unc;
        }
        else if( ( hLinkAddress.StartsWith( "mailto" ) ) || ( hLinkAddress.IndexOf( @"://" ) != -1 ) )
        {
          link.Type = ExcelHyperLinkType.Url;
        }
        else
        {
          link.Type = ExcelHyperLinkType.File;
        }
        link.SetAddress( hLinkAddress, false );
        link.ScreenTip = hLinkScreenTip;
      }
    }
    /// <summary>
    /// Reads record from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="cells">Represents cell records collection.</param>
    /// <param name="iXFIndex">Represents XF index.</param>
    /// <param name="type">Type of the extracted data.</param>
    /// <param name="rtf">Object to store extracted rtf string inside.</param>
    /// <returns>Returns created record.</returns>
    private string ReadData( XmlReader reader, CellRecordCollection cells,
      int iXFIndex, out CellType type, out TextWithFormat rtf )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strType;
      string strValue = null;

      FontsCollection fonts = m_parentBook.InnerFonts;
      int iFontIndex = m_parentBook.InnerExtFormats[ iXFIndex ].FontIndex;

      rtf = new TextWithFormat( iFontIndex );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_TYPE_PREF, DEF_SS_NAMESPACE ) )
      {
        strType = reader.Value;
      }
      else
      {
        throw new XmlReadingException( "table", "Undefined cell type." );
      }

      type = ( CellType )Enum.Parse( typeof( CellType ), strType, true );
      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        strValue = ReadRTF( reader, iXFIndex, rtf );
      }

      return strValue;
    }
    /// <summary>
    /// If can adds merge record to collection.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Current sheet.</param>
    /// <param name="iRow">Row index.( Zero based )</param>
    /// <param name="iCol">Column index.( Zero based )</param>
    /// <param name="iXFIndex">Represents XF index.</param>
    /// <returns>Number of cells merged across.</returns>
    private int ReadMerge( XmlReader reader, WorksheetImpl sheet, int iRow, int iCol, int iXFIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      int iMergeAcross = 0;
      int iMergeDown = 0;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MERGE_ACROSS_PREF, DEF_SS_NAMESPACE ) )
        iMergeAcross = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MERGE_DOWN_PREF, DEF_SS_NAMESPACE ) )
        iMergeDown = XmlConvert.ToInt32( reader.Value );

      if( iMergeAcross != 0 || iMergeDown != 0 )
      {
        CellRecordCollection cells = sheet.CellRecords;

        sheet.MergeCells.AddMerge( iRow, iRow + iMergeDown, iCol, iCol + iMergeAcross, ExcelMergeOperation.Leave );

        for( int i = iRow; i <= iRow + iMergeDown; i++ )
        {
          for( int j = iCol; j <= iCol + iMergeAcross; j++ )
          {
            cells.SetBlank( i + 1, j + 1, iXFIndex );
          }
        }

        cells.Remove( iCol + 1, iRow + 1 );
      }

      return iMergeAcross;
    }
    #endregion

    #region Style reading methods
    /// <summary>
    /// Reads styles from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="book">Current workbook.</param>
    private void ReadStyles( XmlReader reader, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_STYLE_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadStyle( reader, book );
        
        reader.Skip();
      }
    }
    /// <summary>
    /// Reads custom style from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="book">Current workbook.</param>
    private void ReadStyle( XmlReader reader, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      ExtendedFormatsCollection XFColl = book.InnerExtFormats;
      string strIndex = null;
      string strName = null;
      string strParent = null;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_ID_PREF, DEF_SS_NAMESPACE ) )
        strIndex = reader.Value;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_PARENT_PREF, DEF_SS_NAMESPACE ) )
        strParent = reader.Value;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_NAME_PREF, DEF_SS_NAMESPACE ) )
        strName = reader.Value;

      ExtendedFormatImpl format = GetExtendedFormat( XFColl, strIndex, strName, strParent );
      if (strIndex == WorkbookXmlSerializator.DEF_STYLE_NAME)
          format.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
      reader.MoveToElement();

      if( reader.IsEmptyElement )
      {
        AddXFToCollection( book, XFColl, format, strIndex, strName );
        return;
      }

      reader.Read();

      if( strName != null )
      {
        format.IncludeAlignment = true;
        format.IncludeFont = true;
        format.IncludePatterns = true;
        format.IncludeNumberFormat = true;
        format.IncludeProtection = true;
        format.IncludeBorder = true;
      }

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_ALIGNMENT_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadAlignment( reader, format );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FONT_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadFont( reader, format );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_INTERIOR_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadInterior( reader, format );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_NUMBERFORMAT_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadNumberFormat( reader, format );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PROTECTION_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadProtection( reader, format );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_BORDERS_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadBorders( reader, format );

        reader.Skip();
      }
      if (strIndex != WorkbookXmlSerializator.DEF_STYLE_NAME)
          AddXFToCollection(book, XFColl, format, strIndex, strName);
      else
          format.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
    }
    /// <summary>
    /// Reads alignment properties from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">XF format to fill.</param>
    private void ReadAlignment( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.AttributeCount == 0 )
        return;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_ROTATE_PREF, DEF_SS_NAMESPACE ) )
      {
          double iRotation = 0;
          double.TryParse(reader.Value, out iRotation);
          format.Rotation = (iRotation < 0) ? (int)(WorkbookXmlSerializator.DEF_STYLE_ROTATION - iRotation) : (int)iRotation;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_WRAPTEXT_PREF, DEF_SS_NAMESPACE ) )
        format.WrapText = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_INDENT_PREF, DEF_SS_NAMESPACE ) )
        format.IndentLevel = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_READINGORDER_PREF, DEF_SS_NAMESPACE ) )
        format.ReadingOrder = ( ExcelReadingOrderType )Enum.Parse( typeof( ExcelReadingOrderType ), reader.Value, true );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SHRINKTOFIT_PREF, DEF_SS_NAMESPACE ) )
        format.ShrinkToFit = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_VERTICALTEXT_PREF, DEF_SS_NAMESPACE )
        && XmlConvert.ToBoolean( reader.Value ) )
      {
        format.Rotation = WorkbookXmlSerializator.DEF_ROTATION_TEXT;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HORIZONTAL_PREF, DEF_SS_NAMESPACE ) )
        format.HorizontalAlignment = m_hashHorizontalAll[ reader.Value ];

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_VERTICAL_PREF, DEF_SS_NAMESPACE ) )
        format.VerticalAlignment = ( ExcelVAlign )m_hashVerticalAll[ reader.Value ];

      reader.MoveToElement();

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludeAlignment = false;

      return;
    }
    /// <summary>
    /// Reads font properties from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Represents XF format, which fill from xml stream.</param>
    private void ReadFont( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.AttributeCount == 0 )
        return;

      IFont font = new FontImpl( Application, format.Workbook.InnerFonts );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_BOLD_PREF, DEF_SS_NAMESPACE ) )
        font.Bold = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_COLOR_PREF, DEF_SS_NAMESPACE ) )
        font.RGBColor = GetColor( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_FONTNAME_PREF, DEF_SS_NAMESPACE ) )
      {
        font.FontName = reader.Value;
      }
      else
      {
        font.FontName = DefaultFontName;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_ITALIC_PREF, DEF_SS_NAMESPACE ) )
        font.Italic = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_OUTLINE_PREF, DEF_SS_NAMESPACE ) )
        font.MacOSOutlineFont = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SHADOW_PREF, DEF_SS_NAMESPACE ) )
        font.MacOSShadow = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SIZE_PREF, DEF_SS_NAMESPACE ) )
        font.Size = ( int )XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_STRIKETHROUGH_PREF, DEF_SS_NAMESPACE ) )
        font.Strikethrough = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_UNDERLINE_PREF, DEF_SS_NAMESPACE ) )
        font.Underline = ( ExcelUnderline )Enum.Parse( typeof( ExcelUnderline ), reader.Value, true );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_VERTICAL_ALIGN_PREF, DEF_SS_NAMESPACE ) )
        font = SetFontAllign( font, reader.Value );
      
      if( format.Index == 0 )
      {
        FontImpl fontTmp = ( FontImpl )font;
        fontTmp.CopyTo( ( FontImpl )format.Workbook.InnerFonts[ 0 ] );
        format.FontIndex = ( ( IInternalFont )format.Workbook.InnerFonts[ 0 ] ).Index;
      }
      else
      {      
        font = format.Workbook.InnerFonts.Add( font );
        format.FontIndex = ( ( IInternalFont )font ).Index;
      }

      reader.MoveToElement();

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludeFont = false;

      return;
    }
    /// <summary>
    /// Reads interior from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Parent XF format.</param>
    private void ReadInterior( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.AttributeCount == 0 )
        return;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_COLOR_PREF, DEF_SS_NAMESPACE ) )
        format.Color = GetColor( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_PATTERNCOLOR_PREF, DEF_SS_NAMESPACE ) )
        format.PatternColor = GetColor( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_PATTERN_PREF, DEF_SS_NAMESPACE ) )
      {
        format.FillPattern = ( ExcelPattern )Array.IndexOf( 
          WorkbookXmlSerializator.DEF_PATTERN_STRING, reader.Value );
      }

      reader.MoveToElement();

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludePatterns = false;

      return;
    }
    /// <summary>
    /// Reads number format from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Parent XF format.</param>
    private void ReadNumberFormat( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      format.NumberFormat = RangeImpl.DEF_GENERAL_FORMAT;

      if( reader.AttributeCount == 0 )
        return;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_FORMAT_PREF, DEF_SS_NAMESPACE ) )
      {
        string strValue = reader.Value;

        if( m_hashNumberFormat.ContainsKey( strValue ) )
          strValue = m_hashNumberFormat[ strValue ];

        format.NumberFormat = strValue;
      }

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludeNumberFormat = false;

      reader.MoveToElement();

      return;
    }
    /// <summary>
    /// Reads protection properties from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Parent XF format.</param>
    private void ReadProtection( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.AttributeCount == 0 )
        return;

      format.IncludeProtection = true;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_PROTECTED_PREF, DEF_SS_NAMESPACE ) )
        format.Locked = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HIDEFORMULA_PREF, DEF_X_NAMESPACE ) )
        format.FormulaHidden = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludeProtection = false;
    }
    /// <summary>
    /// Reads borders from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Current xf format.</param>
    private void ReadBorders( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_BORDER_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadBorder( reader, format );
        
        reader.Skip();
      }

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
        format.IncludeBorder = false;
    }
    /// <summary>
    /// Reads border from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="format">Current extended format.</param>
    private void ReadBorder( XmlReader reader, ExtendedFormatImpl format )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( reader.AttributeCount == 0 )
        return;

      format.IncludeBorder = true;
      reader.MoveToAttribute( WorkbookXmlSerializator.DEF_POSITION_PREF, DEF_SS_NAMESPACE );
      string strType = reader.Value;
      string strLineStyle = DEF_NONE;
      string strWeight = "0";

      int iIndex = Array.IndexOf( WorkbookXmlSerializator.DEF_BORDER_POSITION_STRING, strType );
      IBorder border = format.Borders[ ( ExcelBordersIndex )iIndex ];

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_COLOR_PREF, DEF_SS_NAMESPACE ) )
        border.ColorRGB = GetColor( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_LINE_STYLE_PREF, DEF_SS_NAMESPACE ) )
        strLineStyle = reader.Value;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_WEIGHT_PREF, DEF_SS_NAMESPACE ) )
        strWeight = reader.Value;

      border.LineStyle = GetLineStyle( strLineStyle, strWeight );

      reader.MoveToElement();
    }
    #endregion

    #region Worksheet options reading methods
    /// <summary>
    /// Reads worksheet options from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadWorksheetOptions ( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();
      
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_TABCOLOR_INDEX_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {  
          reader.Read();
          sheet.TabColor = ( ExcelKnownColors )Enum.Parse( typeof( ExcelKnownColors ), reader.Value, true );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PAGE_SETUP_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadPageSetup( reader, sheet );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_ACTIVE_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.ActivePane = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PANES_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadPanes( reader, sheet );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_SPLIT_HORIZONTAL_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.HorizontalSplit = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_TOPROW_BOTTOM_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.FirstVisibleRow = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FIRST_VISIBLE_ROW_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.WindowTwo.TopRow = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_SPLIT_VERTICAL_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.VerticalSplit = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_LEFTCOLUMN_RIGHT_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.FirstVisibleColumn = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FREEZE_PANES_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          sheet.WindowTwo.IsFreezePanes = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FROZEN_NOSPLIT_PANES_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          sheet.WindowTwo.IsFreezePanesNoSplit = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_ZOOM_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          sheet.Zoom = XmlConvert.ToInt32( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PRINT_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadPrint( reader, sheet );

        if (reader.LocalName == WorkbookXmlSerializator.DEF_DISPLAY_GRIDLINES_PREF &&
            reader.NamespaceURI == DEF_X_NAMESPACE)
          sheet.IsGridLinesVisible = false;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_VISIBLE_PREF )
        {
          ParseVisibility( reader, sheet );
        }
        else
        {
          reader.Skip();
        }
      }
    }
    #region DataValidation  reading methods
    /// <summary>
    /// Reads DataValidation from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadDataValidation(XmlReader reader, WorksheetImpl sheet)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        DataValidationCollection dvCollection;

        if (sheet.DVTable.Count == 0)
        {

            DValRecord dValRecord = (DValRecord)BiffRecordFactory.GetRecord(TBIFFRecord.DVal);

            dvCollection = sheet.DVTable.Add(dValRecord);
        }
        else
        {
            dvCollection = sheet.DVTable[0];
            
        }


        ParseDataValidations(reader, dvCollection,sheet);
    }

    /// <summary>
    /// Reads DataValidation from XML
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private  void ParseDataValidations(XmlReader reader, DataValidationCollection dvCollection,WorksheetImpl sheet)
    {
        DataValidationImpl dataValidation = new DataValidationImpl(dvCollection);
        TAddr taddr =new TAddr ();

        reader.MoveToElement();

        if (reader.IsEmptyElement) return;

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            reader.MoveToElement();
            switch (reader.LocalName)
            {
                case DV.DEF_RANGE_PREF:
                    {
                        string[] ranges = reader.ReadElementContentAsString().Split(',');

                        IRange Range = null;

                        foreach (string range in ranges)
                        {
                            taddr = GetRangeForDVOrAF(range, sheet,ref Range);

                            dataValidation.AddRange(taddr);
                        }
                        //dataValidation.Range = Range;
                       
                        break;
                    }
                case DV.DEF_TYPE_PREF:
                    {
                        dataValidation.AllowType = ConvertDataValidationType(reader.ReadElementContentAsString());
                        break;
                    }

                case DV.DEF_MIN_PREF:
                    {
                        dataValidation.SetFormulaValue(reader.ReadElementContentAsString(), m_formulaUtil,taddr, true);
                        break;
                    }

                case DV.DEF_MAX_PREF:
                    {
                        dataValidation.SetFormulaValue(reader.ReadElementContentAsString(), m_formulaUtil,taddr, false);
                        break;
                    }
                case DV.DEF_INPUTTITLE_PREF:
                    {
                        dataValidation.PromptBoxTitle = reader.ReadElementContentAsString();
                        break;
                    }
                case DV.DEF_INPUTMESSAGE_PREF:
                    {
                        dataValidation.PromptBoxText = reader.ReadElementContentAsString();
                        break;
                    }
                case DV.DEF_ERRORSTYLE_PREF:
                    {
                        dataValidation.ErrorStyle = ConvertDataValidationErrorStyle(reader.ReadElementContentAsString());
                        break;
                    }
                case DV.DEF_ERRORMESSAGE_PREF:
                    {
                        dataValidation.ErrorBoxText = reader.ReadElementContentAsString();
                        break;
                    }
                case DV.DEF_ERRORTITLE_PREF:
                    {
                        dataValidation.ErrorBoxTitle = reader.ReadElementContentAsString();
                        break;
                    }

                case DV.DEF_VALUE_PREF:
                    {
                        dataValidation.SetFormulaValue(reader.ReadElementContentAsString(), m_formulaUtil,taddr, true);
                        break;

                    }
                case DV.DEF_QUALIFIER_PREF:
                    {
                        dataValidation.CompareOperator = (ExcelDataValidationComparisonOperator)Enum.Parse(typeof(ExcelDataValidationComparisonOperator), reader.ReadElementContentAsString(), true);
                        break;
                    }
                case DV.DEF_CELLRANGELIST_PREF:
                    {
                        reader.Skip();
                        break;
                    }
                default:
                    {
                        reader.Skip();
                        break;
                    }
            }
        }
        
        dvCollection.Add(dataValidation);
    }

    /// <summary>
    /// Converts Excel 2007 data validation type to Excel 97-03.
    /// </summary>
    /// <param name="dataValidationType">Excel 2007 data validation type.</param>
    /// <returns>Excel 97-03 data validation type.</returns>
    private ExcelDataType ConvertDataValidationType(string dataValidationType)
    {
        if (dataValidationType == null || dataValidationType == string.Empty)
            throw new ArgumentNullException("strErrorStyle");

        int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_ALLOWTYPE_STRING, dataValidationType);

        return (iIndex > 0) ? (ExcelDataType)iIndex : ExcelDataType.Any;
        
        
    }
    /// <summary>
    /// Returns DV error style.
    /// </summary>
    /// <param name="strErrorStyle">DV error style name.</param>
    /// <returns>DV error style.</returns>
    private ExcelErrorStyle ConvertDataValidationErrorStyle(string strErrorStyle)
    {
        if (strErrorStyle == null || strErrorStyle == string.Empty)
            throw new ArgumentNullException("strErrorStyle");

        int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_ERRORSTYLE, strErrorStyle);

        return (iIndex > 0) ? (ExcelErrorStyle)iIndex : ExcelErrorStyle.Stop;
        
    }
    /// <summary>
    /// Gets the Ranges for the DataValidations
    /// </summary>
    /// <param name="strRange">Ranges in R1C1 format</param>
    /// <param name="sheet">Worksheet to fill.</param>
    /// <param name="range">ranges to be added</param>
    private TAddr GetRangeForDVOrAF(string strRange,WorksheetImpl sheet,ref IRange range )
    {
        if (strRange == null || strRange == string.Empty)
            throw new ArgumentNullException("strRange");

        string strFirstRow = string.Empty;
        string strFirstColumn = string.Empty;
        string strLastRow = string.Empty;
        string strLastColumn = string.Empty;

        TAddr tAddr = new TAddr();

        if (FormulaUtil.IsCell(strRange, true, out strFirstRow, out strFirstColumn))
        {
            int iRow = Convert.ToInt32(strFirstRow.Remove(0,1)) ;
            int iCol = Convert.ToInt32(strFirstColumn.Remove(0, 1)) ;
            tAddr = new TAddr(iRow-1, iCol-1, iRow-1, iCol-1);
            range = sheet.Range[iRow, iCol, iRow, iCol];
            
        }
        if (m_formulaUtil.IsCellRange(strRange, true, out strFirstRow, out strFirstColumn, out strLastRow, out strLastColumn))
        {
            int iFirstRow = Convert.ToInt32(strFirstRow.Remove(0, 1)) ;
            int iFirstColumn = Convert.ToInt32(strFirstColumn.Remove(0, 1));
            int iLastRow = Convert.ToInt32(strLastRow.Remove(0, 1)) ;
            int iLastColumn = Convert.ToInt32(strLastColumn.Remove(0, 1)) ;
            tAddr = new TAddr(iFirstRow-1, iFirstColumn-1, iLastRow-1, iLastColumn-1);
            range = sheet.Range[iFirstRow, iFirstColumn, iLastRow, iLastColumn];
        }

        return tAddr;
    }
    #endregion

    #region Conditonal Formatting reading methods
    /// <summary>
    /// Reads the conditional formats
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadConditionalFormats(XmlReader reader, WorksheetImpl sheet)
    {

        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (reader.NodeType == XmlNodeType.None)
            return;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.LocalName == WorkbookXmlSerializator.DEF_CONDITIONAL_FORMATTING_PREF)
            {
                ConditionalFormats conditionalFormats = new ConditionalFormats(sheet.Application, sheet.ConditionalFormats);
                bool isAdd = ParseConditionalFormatting(reader, conditionalFormats, sheet);

                if (isAdd && conditionalFormats.Count != 0)
                    sheet.ConditionalFormats.Add(conditionalFormats);
            }
            else
            {
                reader.Skip();
            }
        }

    }
    /// <summary>
    /// Pareses the conditions
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="conditionalFormats">ConditonalFormats</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private bool ParseConditionalFormatting(XmlReader reader, ConditionalFormats conditionalFormats, WorksheetImpl sheet)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (conditionalFormats == null)
            throw new ArgumentNullException("conditionalFormats");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        IRange range = null;

        reader.MoveToElement();

        if (reader.IsEmptyElement) return false;

        reader.Read();

        if (reader.NodeType == XmlNodeType.Whitespace)
            reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            reader.MoveToElement();
            if (reader.LocalName == WorkbookXmlSerializator.DEF_RANGE_PREF)
            {
                reader.Read();
                TAddr taddr = new TAddr();

                string[] values = reader.Value.Split(',');


                foreach (string value in values)
                {
                    taddr = GetRangeForDVOrAF(value, sheet, ref range);
                    conditionalFormats.AddRange(range);
                }

                reader.Skip();
            }
        }

        ConditionalFormats format = conditionalFormats;

        ParseConditionalFormat(reader, format, range);

        return true;
    }
    /// <summary>
    /// Parses each elements in the condtion
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="formats">ConditonalFormats</param>
    /// <param name="Range">Ranges to fill</param>
    private void ParseConditionalFormat(XmlReader reader, ConditionalFormats formats, IRange Range)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (formats == null)
            throw new ArgumentNullException("formats");

        if (Range == null)
            throw new ArgumentNullException("Range");

        ConditionalFormatImpl conditionalFormat = new ConditionalFormatImpl(formats.Application, formats.Parent);
        conditionalFormat.Range = Range;

        reader.MoveToElement();

        if (reader.IsEmptyElement) return;

        reader.Read();
        if (reader.LocalName == WorkbookXmlSerializator.DEF_CONDITIONAL_PREF)
        {
            reader.MoveToElement();
            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.MoveToElement();

                switch (reader.LocalName)
                {
                    case WorkbookXmlSerializator.DEF_QUALIFIER_PREF:
                        {
                            string value = reader.ReadElementContentAsString();
                            if (value != null)
                            {
                                conditionalFormat.Operator = ConvertCFOperator(value);
                            }
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_VALUE1_PREF:
                        {
                            string value = reader.ReadElementContentAsString();
                            if (value != null)
                            {
                                conditionalFormat.FirstFormulaR1C1 = value;
                            }

                            break;
                        }
                    case WorkbookXmlSerializator.DEF_VALUE2_PREF:
                        {
                            string value = reader.ReadElementContentAsString();
                            if (value != null)
                            {
                                conditionalFormat.SecondFormulaR1C1 = value;
                            }
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_FORMAT_PREF:
                        {
                            //Checks for the Attribute NameSpaces.
                            if (reader.MoveToAttribute(WorkbookXmlSerializator.DEF_STYLE_PREF))
                            {
                                ParseConditionalFormatString(reader.Value, conditionalFormat);
                            }
                            else
                            {
                                reader.MoveToAttribute(WorkbookXmlSerializator.DEF_STYLE_PREF, DEF_X_NAMESPACE);
                                ParseConditionalFormatString(reader.Value, conditionalFormat);
                            }
                            reader.Read();
                            break;
                        }
                    default:
                        {
                            reader.Skip();
                            break;
                        }
                }

            }

            reader.Skip();
            formats.Add(conditionalFormat);

        }

    }
    /// <summary>
    /// Parses Each conditional format settings from JSON string.
    /// </summary>
    /// <param name="conditionalFormat">FormattingValues represented in JSON type</param>
    /// <param name="formattingValues">CondtionalFormatImpl</param>
    private void ParseConditionalFormatString(string formattingValues, ConditionalFormatImpl conditionalFormat)
    {
        if (formattingValues == null || formattingValues.Length == 0)
            throw new ArgumentNullException("formattingValues");

        if (conditionalFormat == null)
            throw new ArgumentNullException("conditionalFormat");


        string[] values = formattingValues.Split(WorkbookXmlSerializator.DEF_SEMICOLON[0]);

        for (int i = 0; i < values.Length; i++)
        {
            string[] items = values[i].Split(WorkbookXmlSerializator.DEF_COLON[0]);

            for (int j = 0; j < items.Length; j++)
            {
                switch (items[j])
                {
                    case WorkbookXmlSerializator.DEF_FONT_COLOR_CF:
                        {
                            conditionalFormat.IsFontFormatPresent = true;
                            conditionalFormat.IsFontColorPresent = true;
                            conditionalFormat.FontColorRGB = GetColor(items[++j]);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_FONT_WEIGHT_CF:
                        {
                            if (items[++j] == WorkbookXmlSerializator.DEF_FONT_BOLD_CF)
                                conditionalFormat.IsBold = true;
                            else
                                conditionalFormat.IsBold = false;
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_FONT_STYLE_CF:
                        {
                            conditionalFormat.IsItalic = true;
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_FONT_STRIKETHROUGH_CF:
                        {
                            if (items[++j] == WorkbookXmlSerializator.DEF_FONT_STRIKETHROUGH_SINGLE_CF)
                                conditionalFormat.IsStrikeThrough = true;
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_FONT_UNDERLINE_CF:
                        {
                            conditionalFormat.Underline = (ExcelUnderline)Enum.Parse(typeof(ExcelUnderline), items[++j],true);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_PATTERN_BACKGROUND_CF:
                        {
                            conditionalFormat.BackColorRGB = GetColor(items[++j]);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_BORDERTOP_CF:
                        {
                            string[] BorderSettings = GetBorderSetting(items[++j]);
                            conditionalFormat.TopBorderStyle = GetBroderLineStyle(BorderSettings);
                            conditionalFormat.TopBorderColorRGB = GetColor(BorderSettings[0]);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_BORDERBOTTOM_CF:
                        {
                            string[] BorderSettings = GetBorderSetting(items[++j]);
                            conditionalFormat.BottomBorderStyle = GetBroderLineStyle(BorderSettings);
                            conditionalFormat.BottomBorderColorRGB = GetColor(BorderSettings[0]);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_BORDERLEFT_CF:
                        {
                            string[] BorderSettings = GetBorderSetting(items[++j]);
                            conditionalFormat.LeftBorderStyle = GetBroderLineStyle(BorderSettings);
                            conditionalFormat.LeftBorderColorRGB = GetColor(BorderSettings[0]);
                            break;
                        }
                    case WorkbookXmlSerializator.DEF_BORDERRIGHT_CF:
                        {
                            string[] BorderSettings = GetBorderSetting(items[++j]);
                            conditionalFormat.RightBorderStyle = GetBroderLineStyle(BorderSettings);
                            conditionalFormat.RightBorderColorRGB = GetColor(BorderSettings[0]);
                            break;
                        }

                    case WorkbookXmlSerializator.DEF_PATTERN_FILL_CF:
                        {
                            //To do more Fill Patterns
                            string[] fillpartters = items[++j].Split(' ');
                            if (fillpartters.Length > 1 && fillpartters[1] != null && fillpartters[1].Length > 0)
                            {
                                int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_PATTERN_STRING_CF, fillpartters[1]);

                                conditionalFormat.FillPattern = (iIndex > 0) ? (ExcelPattern)iIndex : ExcelPattern.None;
                            }
                            else if(fillpartters.Length>=1 && fillpartters[0]!=null && fillpartters[0].Length>0)
                            {
                                int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_PATTERN_STRING_CF, fillpartters[0]);

                                conditionalFormat.FillPattern = (iIndex > 0) ? (ExcelPattern)iIndex : ExcelPattern.None;
                            }
                            break;
                        }
                }

            }

        }

    }
    /// <summary>
    /// Reverses the Array
    /// </summary>
    /// <param name="borderSetting">Bordersettings</param>
    private string[] GetBorderSetting(string borderSetting)
    {
        if (borderSetting == null || borderSetting.Length == 0)
            throw new ArgumentNullException("borderSetting");

        string[] result = borderSetting.Split(' ');
        string[] temp = new string[result.Length];

        for (int i = 0, j = result.Length - 1; i < result.Length; j--, i++)
        {
            temp[i] = result[j];
        }
        return temp;
    }
    /// <summary>
    /// Get the ExcelLineStyle for border
    /// </summary>
    /// <param name="style">array of styles</param>
    private ExcelLineStyle GetBroderLineStyle(string[] style)
    {
        if (style == null || style.Length == 0)
            throw new ArgumentNullException("style");

        string borderLineStyle = null;

        if (style[1] != "none")
        {
            borderLineStyle = style[2] + " " + style[1];
        }
        else
        {
            borderLineStyle = style[1];
        }

        int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_BORDER_LINE_CF, borderLineStyle);

        return (iIndex > 0) ? (ExcelLineStyle)iIndex : ExcelLineStyle.None;
    }

    /// <summary>
    /// Gets ExcelComparisonOperator 
    /// </summary>
    /// <param name="strOperator">Operator</param>
    /// <returns>ExcelComparisonOperator</returns>
    private ExcelComparisonOperator ConvertCFOperator(string strOperator)
    {
        if (strOperator == null || strOperator.Length == 0)
            throw new ArgumentNullException("strOperator");

        int iIndex = Array.IndexOf(WorkbookXmlSerializator.DEF_COMPARISION_OPERATORS_PREF, strOperator);

        return (iIndex > 0) ? (ExcelComparisonOperator)iIndex : ExcelComparisonOperator.None;
    }
    #endregion
    /// <summary>
    /// Parses worksheet visibility option.
    /// </summary>
    /// <param name="reader">XmlReader to get option from.</param>
    /// <param name="sheet">Worksheet to set option to.</param>
    private void ParseVisibility( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      string strContent = reader.ReadElementContentAsString();
      int iIndex = Array.IndexOf( WorkbookXmlSerializator.DEF_VISIBLE_STRING, strContent );

      if( iIndex < 0 )
        throw new XmlException();

      sheet.Visibility = ( WorksheetVisibility )iIndex;
    }
    /// <summary>
    /// Reads page setup from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadPageSetup( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( ( ( reader.LocalName == WorkbookXmlSerializator.DEF_FOOTER_PREF ) || 
          ( reader.LocalName == WorkbookXmlSerializator.DEF_HEADER_PREF ) ) &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadHeaderFooter( reader, sheet );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_LAYOUT_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadLayout( reader, sheet );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PAGE_MARGINS_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadPageMargins( reader, sheet );

        reader.Skip();
      }
    }
    /// <summary>
    /// Reads header/footer from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadHeaderFooter( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      string strLocalName = reader.LocalName;
      PageSetupImpl pageSetup = ( PageSetupImpl )sheet.PageSetup;

      if ( strLocalName == WorkbookXmlSerializator.DEF_FOOTER_PREF )
      {
        if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_PREF, DEF_X_NAMESPACE ) )
          pageSetup.FooterMargin = XmlConvert.ToDouble( reader.Value );

        if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_DATA_PREF, DEF_X_NAMESPACE ) )
          pageSetup.FullFooterString = reader.Value;
      }
      else
      {
        if ( strLocalName == WorkbookXmlSerializator.DEF_HEADER_PREF )
        {
          if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_PREF, DEF_X_NAMESPACE ) )
            pageSetup.HeaderMargin = XmlConvert.ToDouble( reader.Value );

          if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_DATA_PREF, DEF_X_NAMESPACE ) )
            pageSetup.FullHeaderString = reader.Value;
        }
      }
    }
    /// <summary>
    /// Reads page setup layout from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>>
    private void ReadLayout( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      IPageSetup pageSetup = sheet.PageSetup;
      pageSetup.Orientation = ExcelPageOrientation.Portrait;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_START_PAGE_NUMBER_PREF, DEF_X_NAMESPACE ) )
      {
        pageSetup.AutoFirstPageNumber = false;
        uint firstPageNumber = (uint)XmlConvert.ToInt32(reader.Value);
        pageSetup.FirstPageNumber = (short)firstPageNumber;
      }

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_ORIENTATION_PREF, DEF_X_NAMESPACE ) )
        pageSetup.Orientation = ( ExcelPageOrientation )Enum.Parse( typeof( ExcelPageOrientation ), reader.Value, true );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_CENTER_HORIZONTAL_PREF, DEF_X_NAMESPACE ) )
        pageSetup.CenterHorizontally = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_CENTER_VERTICAL_PREF, DEF_X_NAMESPACE ) )
        pageSetup.CenterVertically = XmlConvert.ToBoolean( reader.Value );
    }
    /// <summary>
    /// Reads page margins from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadPageMargins( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      PageSetupImpl pageSetup = ( PageSetupImpl )sheet.PageSetup;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_RIGHT_PREF, DEF_X_NAMESPACE ) )
        pageSetup.RightMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_LEFT_PREF, DEF_X_NAMESPACE ) )
        pageSetup.LeftMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_BOTTOM_PREF, DEF_X_NAMESPACE ) )
        pageSetup.BottomMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_MARGIN_TOP_PREF, DEF_X_NAMESPACE ) )
        pageSetup.TopMargin = XmlConvert.ToDouble( reader.Value );
    }
    /// <summary>
    /// Reads panes from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadPanes( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );
 
      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_PANE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          ReadSelectionPane( reader, sheet );
        
        reader.Skip();
      }

      //reader.Skip();
    }
    /// <summary>
    /// Reads selection pane from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadSelectionPane( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      SelectionRecord record = ( SelectionRecord ) BiffRecordFactory.GetRecord( TBIFFRecord.Selection );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_NUMBER_PANE_PREF && reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          record.Pane = XmlConvert.ToByte( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_ACTIVECOL_PANE_PREF && reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          record.ColumnActiveCell = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_ACTIVEROW_PANE_PREF && reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          record.RowActiveCell = XmlConvert.ToUInt16( reader.Value );
          reader.Skip();
        }

        reader.Skip();
      }

      record.Addr = new SelectionRecord.TAddr[ 1 ] { new SelectionRecord.TAddr(
                                                   ( ushort )record.RowActiveCell,
                                                   ( ushort )record.RowActiveCell,
                                                   ( byte )record.ColumnActiveCell,
                                                   ( byte )record.ColumnActiveCell ) };
      sheet.Selections.Add( record );
    }
    /// <summary>
    /// Reads print options from xml.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="sheet">Worksheet to fill.</param>
    private void ReadPrint( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      reader.MoveToElement();

      if( reader.IsEmptyElement ) return;

      reader.Read();

      PageSetupImpl pageSetup = ( PageSetupImpl )sheet.PageSetup;
      pageSetup.PaperSize = ExcelPaperSize.PaperLetter;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_NUMBER_OF_COPIES_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.Copies = XmlConvert.ToInt32( reader.Value );
          reader.Read();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_HORIZONTAL_RESOLUTION_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.PrintQuality = XmlConvert.ToInt32( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PAPER_SIZE_INDEX_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.PaperSize = ( ExcelPaperSize )( XmlConvert.ToInt16( reader.Value ) );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FIT_WIDTH_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.FitToPagesWide = XmlConvert.ToInt32( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FIT_HEIGHT_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.FitToPagesTall = XmlConvert.ToInt32( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_SCALE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          pageSetup.Zoom = XmlConvert.ToInt32( reader.Value );
          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_GRIDLINES_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          pageSetup.PrintGridlines = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_BLACK_AND_WHITE_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          pageSetup.BlackAndWhite = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_DRAFT_QUALITY_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          pageSetup.Draft = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_ROWCOL_HEADINGS_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
          pageSetup.PrintHeadings = true;

        if( reader.LocalName == WorkbookXmlSerializator.DEF_COMMENTS_LAYOUT_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          int iIndex = Array.IndexOf( WorkbookXmlSerializator.DEF_PRINT_LOCATION_STRING, reader.Value );

          if( iIndex != -1 )
            pageSetup.PrintComments = ( ExcelPrintLocation )iIndex;

          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_PRINT_ERRORS_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.Read();
          int iIndex = Array.IndexOf( WorkbookXmlSerializator.DEF_PRINT_ERROR_STRING, reader.Value );

          if( iIndex != -1 )
            pageSetup.PrintErrors = ( ExcelPrintErrors )iIndex;

          reader.Skip();
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_LEFT_TO_RIGHT_PREF && reader.NamespaceURI == DEF_X_NAMESPACE )
          sheet.PageSetup.Order = ExcelOrder.OverThenDown;

        reader.Skip();
      }
    }
    #endregion

    #region Named Range reading methods
    /// <summary>
    /// Reads named ranges from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="namesColl">Collection of named ranges.</param>
    /// <param name="iSheetIndex">Represents sheet index ( One based ).Zero if global name.</param>
    private void ReadNames( XmlReader reader, INames namesColl, int iSheetIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( namesColl == null )
        throw new ArgumentNullException( "namesColl" );

      if( reader.IsEmptyElement ) return;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_NAMEDRANGE_PREF
          && reader.NamespaceURI == DEF_SS_NAMESPACE )
        {
          ReadName( reader, namesColl, iSheetIndex );
        }
        
        reader.Skip();
      }

      IName name = namesColl[ AutoFiltersCollection.DEF_AUTOFILTER_NAMEDRANGE ];
      IWorksheet sheet = namesColl.ParentWorksheet;

      if ( name != null && sheet != null )
        sheet.AutoFilters.FilterRange = name.RefersToRange;
    }
    /// <summary>
    /// Reads single named range from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="namesColl">Represents collection, of named ranges.</param>
    /// <param name="sheetIndex">Represents sheet index ( One based ).Zero if global name.</param>
    private void ReadName( XmlReader reader, INames namesColl, int sheetIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( namesColl == null )
        throw new ArgumentNullException( "namesColl" );

      if( sheetIndex < 0 )
        throw new ArgumentNullException( "sheetIndex" );

      NameRecord record = ( NameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Name );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_HIDDEN_PREF, DEF_SS_NAMESPACE ) )
        record.IsNameHidden = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToAttribute( WorkbookXmlSerializator.DEF_NAME_PREF, DEF_SS_NAMESPACE );
      record.Name = reader.Value;

      reader.MoveToAttribute( WorkbookXmlSerializator.DEF_REFERSTO_PREF, DEF_SS_NAMESPACE );
      m_arrNames.Add( reader.Value );

      record.IndexOrGlobal = ( ushort )sheetIndex;

      NameImpl name = new NameImpl( Application, namesColl, record );
      
      //name.ValueR1C1 = reader.Value;
      
      string strValue = reader.Value;

      if( strValue[ 0 ] == '=' )
        strValue = reader.Value.Substring( 1 );

      name.SetValue( m_formulaUtil.ParseString( strValue, null, null, 0, 0, true ) );
     
      namesColl.Add( name );
      reader.MoveToElement();
    }
    #endregion

    #region Comment reading methods
    /// <summary>
    /// Reads comment from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="comment">Current comment to fill from xml stream.</param>
    /// <param name="iStyleIndex">Represents default XF index.</param>
    private void ReadComment( XmlReader reader, CommentShapeImpl comment, int iStyleIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( comment == null )
        throw new ArgumentNullException( "comment" );

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_AUTHOR_PREF, DEF_SS_NAMESPACE ) )
        comment.Author = reader.Value;

      if( reader.MoveToAttribute( WorkbookXmlSerializator.DEF_SHOWALWAYS_PREF, DEF_SS_NAMESPACE ) )
        comment.IsVisible = XmlConvert.ToBoolean( reader.Value );

      comment.ShapeType = ExcelShapeType.Comment;

      reader.MoveToElement();

      if( reader.IsEmptyElement )
        return;
      
      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_DATA_PREF && reader.NamespaceURI == DEF_SS_NAMESPACE )
        {
          TextWithFormat commentText = ( ( RichTextString )comment.RichText ).TextObject;

          ReadCommentData( reader, iStyleIndex, commentText );
        }

        reader.Skip();
      }
    }
    /// <summary>
    /// Reads comment rtf text.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="iXFIndex">XF index.</param>
    /// <param name="rtf">Initialize text with format, by read from xml stream rtf properties.</param>
    private void ReadCommentData( XmlReader reader, int iXFIndex, TextWithFormat rtf )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      reader.MoveToElement();

      if( reader.IsEmptyElement )
        return;

      ReadRTF( reader, iXFIndex, rtf );
    }
    #endregion

    /// <summary>
    /// Reads workbook from xml
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="book">Book to fill.</param>
    /// <returns>Returns error code.</returns>
    public void FillWorkbook( XmlReader reader, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      bool bIsError = false;

      if( !reader.EOF )
      {
        reader.Read();
        bIsError = !( reader.NodeType == XmlNodeType.XmlDeclaration );
        bIsError = bIsError || !( reader.Name == DEF_XML_STRING );

        if( reader.MoveToAttribute( VersionAttribute ) )
          bIsError |= !( XmlConvert.DecodeName( reader.Value ) == DefaultVersion );
      }
      else
      {
        bIsError = true;
      }

      if( !reader.EOF )
      {
        reader.Read();

        if (reader.NodeType == XmlNodeType.Whitespace)
            reader.Read();

        if( reader.NodeType == XmlNodeType.Element )
        {
          bIsError = bIsError || ( reader.LocalName != WorkbookXmlSerializator.DEF_WORKBOOK_PREF );
        }
        else
        {
          bIsError = bIsError || !( reader.NodeType == XmlNodeType.ProcessingInstruction );
          bIsError = bIsError || !( reader.Name == DEF_APPLICATION_NAME_STRING );
          bIsError = bIsError || !( XmlConvert.DecodeName( reader.Value ) == DEF_APPLICATION_STRING );
        }
      }
      else
        bIsError = true;

      if( bIsError )
        throw new XmlReadingException( "strict", "Wrong file header. File is likely to be corrupt." );

      ReadWorkbook( reader, book );
    }

    /// <summary>
    /// Read workbook node.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="book">Book to fill.</param>
    private void ReadWorkbook( XmlReader reader, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      m_parentBook = book;

      m_formulaUtil = new FormulaUtil( m_parentBook.Application, m_parentBook, NumberFormatInfo.InvariantInfo,
        ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR );

      reader.MoveToContent();

      if( reader.LocalName != WorkbookXmlSerializator.DEF_WORKBOOK_PREF || reader.IsEmptyElement )
        throw new XmlReadingException( "book", "Workbook node is empty." );

      reader.Read();
      bool bIsSheet = false;
      m_arrNames.Clear();
      int iActiveSheet = 0;
      int iFirstVisibleSheet = 0;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == WorkbookXmlSerializator.DEF_EXCELWORKBOOK_PREF &&
          reader.NamespaceURI == DEF_X_NAMESPACE )
        {
          reader.MoveToElement();

          if( !reader.IsEmptyElement )
          {
            reader.Read();

            while( reader.NodeType != XmlNodeType.EndElement )
            {
              if( reader.LocalName == WorkbookXmlSerializator.DEF_ACTIVE_SHEET_PREF &&
                reader.NamespaceURI == DEF_X_NAMESPACE )
              {
                reader.Read();
                iActiveSheet = XmlConvert.ToInt32( reader.Value );
                reader.Skip();
              }

              if( reader.LocalName == WorkbookXmlSerializator.DEF_FIRST_VISIBLE_SHEET_PREF &&
                reader.NamespaceURI == DEF_X_NAMESPACE )
              {
                reader.Read();
                iFirstVisibleSheet = XmlConvert.ToInt32( reader.Value );
                reader.Skip();
              }

              reader.Skip();
            }
          }
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_WORKSHEET_PREF &&
          reader.NamespaceURI == DEF_SS_NAMESPACE )
        {
          ReadWorksheet( reader, book );
          bIsSheet = true;
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_STYLES_PREF &&
          reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadStyles( reader, book );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_NAMES_PREF &&
          reader.NamespaceURI == DEF_SS_NAMESPACE )
          ReadNames( reader, book.Names, 0 );

        reader.Skip();
      }

      if( !bIsSheet )
        throw new XmlReadingException( "book", "Cannot find worksheet node." );

      ReparseNames( book );
      ReparseFormula( book );
      book.ActiveSheetIndex = iActiveSheet;
      book.DisplayedTab = iFirstVisibleSheet;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Gets extended format from xml value.
    /// </summary>
    /// <param name="strIndex">Unique id.</param>
    /// <param name="coll">Represents parent XF collection.</param>
    /// <param name="strName">Name of style.</param>
    /// <param name="strParent">Parent name.</param>
    /// <returns></returns>
    private ExtendedFormatImpl GetExtendedFormat( ExtendedFormatsCollection coll,
      string strIndex, string strName, string strParent )
    {
      if( strIndex == null )
        throw new XmlReadingException( "style", "File is corrupt - wrong style index." );

      if( strIndex == WorkbookXmlSerializator.DEF_STYLE_NAME)
      {
        m_hashStyle.Add( WorkbookXmlSerializator.DEF_STYLE_NAME, coll.ParentWorkbook.DefaultXFIndex );
        return coll[ WorkbookXmlSerializator.DEF_STYLE_ZERO ];
      }

      ExtendedFormatImpl format;

      if( strParent != null )
      {
        int iParIndex = m_hashStyle[ strParent ];
        ExtendedFormatImpl parentXF = coll[ iParIndex ];
        format = ( ExtendedFormatImpl )parentXF.Clone();
        format.ParentIndex = iParIndex;
      }
      else if( strName == null )
      {
        ExtendedFormatImpl parentXF = coll[ 0 ];
        format = parentXF.CreateChildFormat( false );
      }
      else
      {
        format = ( ExtendedFormatImpl )coll[ WorkbookXmlSerializator.DEF_STYLE_ZERO ].Clone();
        format.ParentIndex = format.Workbook.MaxXFCount;
      }

      format.XFType = ( strName == null || strName == WorkbookXmlSerializator.DEF_STYLE_NAME )
        ? ExtendedFormatRecord.TXFType.XF_STYLE : ExtendedFormatRecord.TXFType.XF_CELL;

      return format;
    }
    /// <summary>
    /// Adds format to collection.
    /// </summary>
    /// <param name="format">Format to add.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="coll">Parent XF collections.</param>
    /// <param name="strID">Unique xml id of XF format.</param>
    /// <param name="strName">Unique xml name of XF format.</param>
    private void AddXFToCollection( WorkbookImpl book, ExtendedFormatsCollection coll,
      ExtendedFormatImpl format, string strID, string strName )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      if( format == null )
        throw new ArgumentNullException( "format" );

      if( coll == null )
        throw new ArgumentNullException( "coll" );

      if( strID != WorkbookXmlSerializator.DEF_STYLE_NAME )
      {
        format = coll.Add( format );
        m_hashStyle.Add( strID, format.XFormatIndex );
      }

      if( format.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
      {
        StylesCollection styles = book.InnerStyles;

        if( styles.ContainsName( strName ) )
        {
          StyleImpl existingStyle = ( StyleImpl ) (styles[ strName ] );
          ExtendedFormatImpl existingFormat = coll[ existingStyle.Index ];
          format.CopyTo( existingFormat );
          ExtendedFormatWrapper formatWrapper = ( ExtendedFormatWrapper )styles[ strName ];
          formatWrapper.UpdateFont();
        }
        else
        {
          StyleRecord record = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
          record.ExtendedFormatIndex = ( ushort )format.XFormatIndex;
          record.StyleName = strName;
          StyleImpl style = AppImplementation.CreateStyle( book, record );
          styles.Add( style );
        }
      }
    }
    /// <summary>
    /// Gets color from xml value.
    /// </summary>
    /// <param name="strColor">Represents xml color.</param>
    /// <returns>Returns color.</returns>
    private Color GetColor( string strColor )
    {
      if( strColor == null || strColor.Length == 0 )
        throw new ArgumentNullException( "strColor" );

      int iPos = strColor.IndexOf( WorkbookXmlSerializator.DEF_COLOR_STRING );

      if( iPos != -1 )
      {
        strColor = strColor.Substring( iPos + 1 );
        int iColor = int.Parse( strColor, System.Globalization.NumberStyles.HexNumber );

        return ColorExtension.FromArgb( iColor );
      }

#if !SILVERLIGHT && !WINRT && !WP
      return Color.FromName( strColor );
#else
      throw new NotImplementedException();
#endif
    }
    /// <summary>
    /// Sets font alignment.
    /// </summary>
    /// <param name="strAlling">Represents font alignment.</param>
    /// <param name="font">Current font.</param>
    /// <returns>Returns initialized font.</returns>
    private IFont SetFontAllign( IFont font, string strAlling )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( strAlling == DEF_SUBSCRIPT )
        font.Subscript = true;

      if( strAlling == DEF_SUPERSCRIPT )
        font.Superscript = true;

      return font;
    }
    /// <summary>
    /// Creates cell record.
    /// </summary>
    /// <param name="type">Cell type.</param>
    /// <param name="strValue">Record value.</param>
    /// <param name="cells">Current cells collection.</param>
    /// <param name="iColumn">Represents column index.</param>
    /// <param name="iRow">Represents row index.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    /// <param name="rtf">Represents Rich Text Format to set.</param>
    private void SetCellRecord( CellType type, string strValue, CellRecordCollection cells
      , int iRow, int iColumn, int iXFIndex, TextWithFormat rtf )
    {
      if( strValue == null || strValue.Length == 0 )
        return;

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      switch( type )
      {
        case CellType.Number:
          cells.SetNumberValue( iRow, iColumn, XmlConvert.ToDouble( strValue ), iXFIndex );
          break;

        case CellType.DateTime:
          DateTime time =
#if ( WINRT )
 XmlConvert.ToDateTimeOffset(strValue).Date;
#else
              XmlConvert.ToDateTime( strValue, XmlDateTimeSerializationMode.Unspecified );
#endif
          double timeInNumber = UtilityMethods.ConvertDateTimeToNumber( time );
          cells.SetNumberValue( iRow, iColumn, timeInNumber, iXFIndex );
          break;

        case CellType.Boolean:
          cells.SetBooleanValue( iRow, iColumn, XmlConvert.ToBoolean( strValue ), iXFIndex );
          break;

        case CellType.Error:
          cells.SetErrorValue( iRow, iColumn, strValue, iXFIndex );
          break;

        default:
          cells.SetRTF( iRow, iColumn, iXFIndex, rtf );
          break;
      }
    }
    /// <summary>
    /// Gets XF index for current cell.
    /// </summary>
    /// <param name="sheet">Parent sheet.</param>
    /// <param name="strValue">Value, that represents current xf id.</param>
    /// <returns>Returns valid index in Inner ExtFormats collection.</returns>
    private int GetXFIndex( WorksheetImpl sheet, string strValue )
    {
      if( strValue == null || strValue.Length == 0 )
        throw new ArgumentNullException( "strValue" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      ExtendedFormatsCollection coll = sheet.ParentWorkbook.InnerExtFormats;
      ExtendedFormatImpl format = coll[ m_hashStyle[ strValue ] ];
      format = format.CreateChildFormat();

      int iXFIndex = format.XFormatIndex;

      m_hashStyle[ strValue ] = iXFIndex;

      return iXFIndex;
    }
    /// <summary>
    /// Gets line style by style and weight.
    /// </summary>
    /// <param name="style">Represents one of xml line style type.</param>
    /// <param name="weight">Represents weight of line.</param>
    /// <returns>Returns excel line style.</returns>
    private ExcelLineStyle GetLineStyle( string style, string weight )
    {
      if( style == null || style.Length == 0 )
        throw new ArgumentNullException( "style" );

      if( weight == null || weight.Length == 0 )
        throw new ArgumentNullException( "weight" );

      if( weight != "0" )
        style = weight + " " + style;

      int iIndex = Array.IndexOf( WorkbookXmlSerializator.DEF_BORDER_LINE_TYPE_STRING, style );

      return ( iIndex > 0 ) ? ( ExcelLineStyle )iIndex : ExcelLineStyle.None;
    }
    /// <summary>
    /// Reparses names read from xml stream.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    private void ReparseNames( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      WorkbookNamesCollection names = book.InnerNamesColection;

      for( int i = 0, iLen = m_arrNames.Count; i < iLen; i++ )
      {
        NameRecord name = names.GetNameRecordByIndex( i );
        string strFormula = m_arrNames[ i ];

        //if( strFormula.IndexOf( WorkbookXmlSerializator.DEF_BAD_REF ) != -1 )
        //  strFormula = WorkbookXmlSerializator.DEF_BAD_FORMULA;

        if( strFormula[ 0 ] == '=' )
          strFormula = strFormula.Substring( 1 );
        
        name.FormulaTokens = m_formulaUtil.ParseString( strFormula, null, null, 0, 0, true );
      }
    }
    /// <summary>
    /// Reads rtf from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="iXFIndex">Index to XF record.</param>
    /// <param name="rtf">Represents rtf holder.</param>
    /// <returns>Returns read from stream string value.</returns>
    private string ReadRTF( XmlReader reader, int iXFIndex, TextWithFormat rtf )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      FontsCollection fonts = m_parentBook.InnerFonts;
      int iFontIndex = m_parentBook.InnerExtFormats[ iXFIndex ].FontIndex;

      FontImpl font = ( FontImpl )fonts[ iFontIndex ];
      StringBuilder builder = new StringBuilder();

      font = font.Clone( m_parentBook.InnerFonts );
      reader.Read( );

      while( reader.NodeType != XmlNodeType.EndElement || reader.LocalName
        != WorkbookXmlSerializator.DEF_DATA_PREF )
      {
        if( reader.NodeType == XmlNodeType.Text )
        {
          font = ( FontImpl )fonts.Add( font );
          rtf.FormattingRuns.Add( builder.Length, font.Index );
          builder.Append( reader.Value );
        }

        if( reader.NodeType == XmlNodeType.Element )
          font = UpdateFont( reader, font.Clone( fonts ), reader.LocalName, false );

        if( reader.NodeType == XmlNodeType.EndElement )
          font = UpdateFont( reader, font.Clone( fonts ), reader.LocalName, true );

        reader.Read();
      }

      string strResult = rtf.Text = builder.ToString();

      return builder.Length > 0 ? strResult : null;
    }
    /// <summary>
    /// Updates font reading from xml stream.
    /// </summary>
    /// <param name="reader">Represents xml reader.</param>
    /// <param name="font">Current font to update.</param>
    /// <param name="strNodeName">Local rtf node name.</param>
    /// <param name="bEndElement">if true - update endelement.</param>
    /// <returns>Returns updated font.</returns>
    private FontImpl UpdateFont( XmlReader reader, FontImpl font, string strNodeName, bool bEndElement )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      switch( strNodeName )
      {
        case DEF_RTF_BOLD:
          font.Bold = !bEndElement;
          break;

        case DEF_RTF_ITALIC:
          font.Italic = !bEndElement;
          break;

        case DEF_RTF_UNDERLINE:
          font.Underline = ( bEndElement ) ? ExcelUnderline.None : ExcelUnderline.Single;
          break;

        case DEF_RTF_SUB:
          font.Subscript = !bEndElement;
          break;

        case DEF_RTF_SUP:
          font.Superscript = !bEndElement;
          break;

        case DEF_RTF_STRIKETHROUGH:
          font.Strikethrough = !bEndElement;
          break;

        case DEF_RTF_FONT:
          font = ReadRTFFont( reader, font );
          break;

        default:
          break;
      }

      return font;
    }
    /// <summary>
    /// Read rtf font.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="font">Font to update.</param>
    /// <returns>Returns updated font.</returns>
    private FontImpl ReadRTFFont( XmlReader reader, FontImpl font )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( font == null )
        throw new ArgumentNullException( "font" );

      font.RGBColor = ColorExtension.Black;
      font.Size = DEF_SIZE_FONT;

      for( int i = 0, iLen = reader.AttributeCount; i < iLen; i++ )
      {
        reader.MoveToAttribute( i );

        if( reader.LocalName == WorkbookXmlSerializator.DEF_COLOR_PREF )
        {
          font.RGBColor = GetColor( reader.Value );
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_SIZE_PREF )
        {
          font.Size = XmlConvert.ToDouble( reader.Value );
        }

        if( reader.LocalName == WorkbookXmlSerializator.DEF_FACE_PREF )
          font.FontName = reader.Value;
      }

      reader.MoveToElement();

      return font;
    }
    /// <summary>
    /// Parse formula from xml stream.
    /// </summary>
    /// <param name="sheet">Current sheet.</param>
    /// <param name="iRowIndex">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <param name="strFormula">Represents string formula in R1C1 style.</param>
    /// <param name="iXFIndex">Represents XFindex.</param>
    /// <param name="cellValue">String representation of the formula value.</param>
    /// <param name="cellType">Type of the value in the formula.</param>
    private void ParseFormula( WorksheetImpl sheet, int iRowIndex, int iCol, string strFormula
      , int iXFIndex, string cellValue, CellType cellType )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( strFormula == null || strFormula.Length == 0 )
        throw new ArgumentNullException( "strFormula" );

      CellRecordCollection cells = sheet.CellRecords;

      // Formula potentially has references to unparsed worksheet, in this case we'd better delay its parsing.
      if( strFormula.IndexOf( '!' ) != -1 )
      {
        //cells.SetFormula( iRowIndex, iCol, strFormula, iXFIndex, true, false, NumberFormatInfo.InvariantInfo );

        long lIndex = WorkbookXmlSerializator.GetUniqueID( sheet.Index
          , RangeImpl.GetCellIndex( iCol, iRowIndex ) );

        m_hashFormula.Add( lIndex, new FormulaData( strFormula, cellValue, cellType, iXFIndex ) );
      }
      else
      {
        SetFormula( sheet, iRowIndex, iCol, strFormula, cellValue, cellType, iXFIndex );
      }
    }
    /// <summary>
    /// Sets formula to the specified cell.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    /// <param name="strFormula">String represetation of the formula.</param>
    /// <param name="cellValue">String representation of the formula value.</param>
    /// <param name="cellType">Type of the formula value.</param>
    private void SetFormula( WorksheetImpl sheet, int row, int column, string strFormula,
      string cellValue, CellType cellType, int iXFIndex )
    {
      CellRecordCollection cells = sheet.CellRecords;

      FormulaRecord formulaRecord = ( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );

      if( strFormula.StartsWith( "=" ) )
        strFormula = UtilityMethods.RemoveFirstCharUnsafe( strFormula );

      formulaRecord.ParsedExpression = m_formulaUtil.ParseString( strFormula, sheet, null,
        row - 1, column - 1, true );
      formulaRecord.Row = row - 1;
      formulaRecord.Column = column - 1;
      formulaRecord.ExtendedFormatIndex = ( ushort )iXFIndex;
      cells.SetCellRecord( row, column, formulaRecord );
      //sheet.SetFormula( row, column, strFormula );
      SetFormulaValue( sheet, row, column, cellValue, cellType );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    /// <param name="cellValue">String representation of the formula value.</param>
    /// <param name="cellType">Type of the formula value.</param>
    private void SetFormulaValue( WorksheetImpl sheet, int row, int column,
      string cellValue, CellType cellType )
    {
      if( cellValue == null )
        return;

      switch( cellType )
      {
        case CellType.Number:
          sheet.SetFormulaNumberValue( row, column, XmlConvert.ToDouble( cellValue ) );
          break;

        case CellType.String:
          sheet.SetFormulaStringValue( row, column, cellValue );
          break;

        case CellType.Boolean:
          sheet.SetFormulaBoolValue( row, column, XmlConvert.ToBoolean( cellValue ) );
          break;

        case CellType.Error:
          sheet.SetFormulaErrorValue( row, column, cellValue );
          break;

        case CellType.DateTime:
          DateTime dateTime =
#if ( WINRT )
              XmlConvert.ToDateTimeOffset( cellValue).Date;
#else
                       XmlConvert.ToDateTime( cellValue, XmlDateTimeSerializationMode.Unspecified );
#endif
          sheet[ row, column ].FormulaDateTime = dateTime;
          break;

        default:
          throw new XmlException();
      }
    }
    /// <summary>
    /// Reparse formula.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    private void ReparseFormula( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      FormulaUtil frmUtil = book.FormulaUtil;
      WorksheetsCollection sheets = book.InnerWorksheets;
      frmUtil.NumberFormat = NumberFormatInfo.InvariantInfo;

      foreach( KeyValuePair<long, FormulaData> entry in m_hashFormula )
      {
        long lIndex = entry.Key;

        int iSheetIndex = WorkbookXmlSerializator.GetSheetIndexByUniqueId( lIndex );
        long lCellIndex = WorkbookXmlSerializator.GetCellIndexByUniqueId( lIndex );
        int iRow = RangeImpl.GetRowFromCellIndex( lCellIndex );
        int iCol = RangeImpl.GetColumnFromCellIndex( lCellIndex );

        FormulaData formulaData = entry.Value;
        string strFormula = formulaData.Formula;
        
        if( strFormula[ 0 ] == '=' )
          strFormula = UtilityMethods.RemoveFirstCharUnsafe( strFormula );

        WorksheetImpl sheet = ( WorksheetImpl )sheets[ iSheetIndex ];
        SetFormula( sheet, iRow, iCol, strFormula, formulaData.Value, formulaData.ValueType, formulaData.XFIndex );
      }

      frmUtil.NumberFormat = null;
    }
    
    #endregion

	#region Internal classes
    private class FormulaData
    {
      public string Formula;
      public string Value;
      public CellType ValueType;
      public int XFIndex;
      public FormulaData( string formula, string value, CellType type, int xfIndex )
      {
        Formula = formula;
        Value = value;
        ValueType = type;
        XFIndex = xfIndex;
      }
    }
    #endregion
  }
}
