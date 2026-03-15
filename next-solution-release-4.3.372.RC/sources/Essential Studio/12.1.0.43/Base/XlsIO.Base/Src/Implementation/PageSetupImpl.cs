#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This Class allows the user to configure Print settings of a document.
  /// </summary>
  public class PageSetupImpl
    : PageSetupBaseImpl
    , IPageSetup
  {
    #region Class constants
    /// <summary>
    /// Represents default print area.
    /// </summary>
    internal static readonly string DEF_AREA_XlS = NameRecord.PREDEFINED_NAMES[ 6 ];
    internal static readonly string DEF_AREA_XlSX = NameRecord.PREDEFINED_NAMES[15];
    /// <summary>
    /// Represents default print title.
    /// </summary>
    private static readonly string DEF_TITLE_XLS = NameRecord.PREDEFINED_NAMES[ 7 ];
    private static readonly string DEF_TITLE_XLSX = NameRecord.PREDEFINED_NAMES[14];
    /// <summary>
    /// Represents default print area tokens.
    /// </summary>
    private static readonly FormulaToken[] DEF_PRINT_AREA_TOKENS = new FormulaToken[]
    {
      FormulaToken.tRef3d1,
      FormulaToken.tRef3d2,
      FormulaToken.tRef3d3,
      FormulaToken.tArea3d1,
      FormulaToken.tArea3d2,
      FormulaToken.tArea3d3,
      FormulaToken.tCellRangeList,
    };
    #endregion

    #region Class members
    /// <summary>
    /// This record contains information as to whether or not the the row/column headers have to be printed.
    /// </summary>
    private ushort                  m_usPrintHeaders;
    /// <summary>
    /// This record defines whether or not to print the gridlines.
    /// </summary>
    private ushort                  m_usPrintGridlines;
    /// <summary>
    /// This record specifies if the option to print sheet grid lines
    /// (PrintGridlinesRecord)has ever been changed.
    /// </summary>
    private ushort                  m_usGridset = 1;
    /// <summary>
    /// Contains information about the layout of outline symbols.
    /// </summary>
    private GutsRecord              m_Guts;
    /// <summary>
    /// Row height for rows with undefined or inexplicitly defined heights.
    /// </summary>
    private DefaultRowHeightRecord  m_DefRowHeight;
    /// <summary>
    /// This record stores a 16-bit value with Boolean options for the current sheet.
    /// </summary>
    private WSBoolRecord            m_WSBool;
    /// <summary>
    /// Parent worksheet for this page setup.
    /// </summary>
    private WorksheetImpl           m_worksheet;
    /// <summary>
    /// Represents horizontal page break.
    /// </summary>
    private HPageBreaksCollection   m_hPageBreaks;
    /// <summary>
    /// Represents vertical page break.
    /// </summary>
    private VPageBreaksCollection   m_vPageBreaks;
    /// <summary>
    /// Id of the printer settings part.
    /// </summary>
    private string m_strRelationId;
    #endregion

    #region Class properties
    /// <summary>
    /// True if cell gridlines are printed on the page. Applies only to
    /// worksheets. Read/write Boolean.
    /// </summary>
    public bool   PrintGridlines
    {
      get
      {
        return ( m_usPrintGridlines == 1 );
      }
      set
      {
        ushort newValue = ( value ) ? ( ushort ) 1 : ( ushort ) 0;
        if( m_usPrintGridlines != newValue )
        {
          m_usPrintGridlines = newValue;
          m_usGridset = 1;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if row and column headings are printed with this page. Applies
    /// only to worksheets. Read/write Boolean.
    /// </summary>
    public bool   PrintHeadings
    {
      get
      {
        return ( m_usPrintHeaders != 0 );
      }
      set
      {
        ushort newValue = ( value ) ? ( ushort ) 1 : ( ushort ) 0;
        if( m_usPrintHeaders != newValue )
        {
          m_usPrintHeaders = newValue;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets horizontal page break.
    /// </summary>
    public HPageBreaksCollection HPageBreaks
    {
      get
      {
        if( m_hPageBreaks == null )
          m_hPageBreaks = new HPageBreaksCollection( Application, this );

        return m_hPageBreaks;
      }
    }
    /// <summary>
    /// Gets vertical page break.
    /// </summary>
    public VPageBreaksCollection VPageBreaks
    {
      get
      {
        if( m_vPageBreaks == null )
          m_vPageBreaks = new VPageBreaksCollection( Application, this );

        return m_vPageBreaks;
      }
    }
    /// <summary>
    /// Returns or sets the range to be printed, as a string using A1-style
    /// references in the language of the macro. Read/write String.
    /// </summary>
    public string PrintArea
    {
      get
      {
        return ExtractPrintArea();
      }
      set
      {
        if( value != ExtractPrintArea() )
        {
          ParsePrintAreaExpression( value );
        }
      }
    }
    /// <summary>
    /// Returns or sets the columns that contain the cells to be repeated
    /// on the left side of each page, as a string in A1-style notation
    /// in the language of the macro. Read/write String.
    /// </summary>
    public string PrintTitleColumns
    {
      get
      {
        return ExtractPrintTitleRowColumn( false );
      }
      set
      {
        if( value != ExtractPrintTitleRowColumn( false ) )
        {
          ParsePrintTitleColumns( value );
        }
      }
    }
    /// <summary>
    /// Returns or sets the rows that contain the cells to be repeated at
    /// the top of each page, as a string in A1-style notation in the
    /// language of the macro. Read/write String.
    /// </summary>
    public string PrintTitleRows
    {
      get
      {
        return ExtractPrintTitleRowColumn( true );
      }
      set
      {
        if( value != ExtractPrintTitleRowColumn( true ) )
        {
          ParsePrintTitleRows( value );
        }
      }
    }
    /// <summary>
    /// Indicates whether fit to page mode is selected.
    /// </summary>
    public override bool IsFitToPage
    {
      get
      {
        return m_WSBool.IsFitToPage;
      }
      set
      {
        if( m_WSBool.IsFitToPage != value )
        {
          m_WSBool.IsFitToPage = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Indicates whether summary rows will appear below detail in outlines.
    /// </summary>
    public bool IsSummaryRowBelow
    {
      get
      {
        return m_WSBool.IsRowSumsBelow;
      }
      set
      {
        m_WSBool.IsRowSumsBelow = value;
      }
    }
    /// <summary>
    /// Indicates whether summary columns will appear right of the detail in outlines.
    /// </summary>
    public bool IsSummaryColumnRight
    {
      get
      {
        return m_WSBool.IsRowSumsRight;
      }
      set
      {
        m_WSBool.IsRowSumsRight = value;
      }
    }
    /// <summary>
    /// Gets / sets default row height.
    /// </summary>
    public int DefaultRowHeight
    {
      get
      {
        return m_DefRowHeight.Height;
      }
      set
      {
        m_DefRowHeight.Height = ( ushort )value;
      }
    }
    /// <summary>
    /// Gets / sets default row height option flag.
    /// </summary>
    public bool DefaultRowHeightFlag
    {
      get
      {
        return ( m_DefRowHeight.OptionFlags & 1 ) == 1;
      }
      set
      {
        if ( value )
        {
          m_DefRowHeight.OptionFlags = ( ushort )( m_DefRowHeight.OptionFlags | 1 );
        }
        else
        {
            if (m_worksheet.IsZeroHeight)
                m_DefRowHeight.OptionFlags = (ushort)(m_DefRowHeight.OptionFlags | 2);
            else
                m_DefRowHeight.OptionFlags = (ushort)(m_DefRowHeight.OptionFlags & 0);
        }
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    /// <param name="version">Represents Excel version.</param>
    public override int GetStoreSize( ExcelVersion version )
    {
      FillGutsRecord();
      int iResult = base.GetStoreSize( version )
        + /*m_PrintHeaders.GetStoreSize( version )*/2 + BiffRecordRaw.DEF_HEADER_SIZE
        + /*m_PrintGridlines.GetStoreSize( version )*/2 + BiffRecordRaw.DEF_HEADER_SIZE
        + /*m_Gridset.GetStoreSize( version )*/2 + BiffRecordRaw.DEF_HEADER_SIZE
        + m_Guts.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE
        + m_DefRowHeight.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE
        + m_WSBool.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;

      if( m_hPageBreaks != null )
        iResult += m_hPageBreaks.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;

      if( m_vPageBreaks != null )
        iResult += m_vPageBreaks.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;

      return iResult;
    }
    /// <summary>
    /// Gets / sets relation id to the printer settings part.
    /// </summary>
    public string RelationId
    {
      get
      {
        return m_strRelationId;
      }
      set
      {
        m_strRelationId = value;
      }
    }
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    public PageSetupImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //FindParents();
      InitializeCollections();
      CreateNecessaryRecords();
    }
    /// <summary>
    /// Recovers page setup from the stream and sets its Application and Parent fields.
    /// Current record in the stream must be PrintHeadersRecord.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="reader">BiffReader that contains page setup records.</param>
    [ CLSCompliant( false ) ]
    public PageSetupImpl( IApplication application, object parent, BiffReader reader )
      : base( application, parent )
    {
      //FindParents();
      InitializeCollections();
      Parse( reader );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from position
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="data">Array of Biff Records that contains all needed records.</param>
    /// <param name="position">Position of PrintHeadersRecord in the array.</param>
    [ CLSCompliant( false ) ]
    public PageSetupImpl( IApplication application, object parent, BiffRecordRaw[] data, int position )
      : base( application, parent )
    {
      //FindParents();
      InitializeCollections();
      Parse( data, position );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records List starting from position.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="data">List which contains Biff Records.</param>
    /// <param name="position">Position of PrintHeadersRecord in the array.</param>
    public PageSetupImpl( IApplication application, object parent, List<BiffRecordRaw> data, int position )
      : base( application, parent )
    {
      InitializeCollections();
      Parse( data, position );

      CreateNecessaryRecords();
    }
    /// <summary>
    /// Find parent worksheet.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// When can't find parent worksheet.
    /// </exception>
    protected override void FindParents()
    {
      base.FindParents();

      // Find worksheet
      object result = FindParent( typeof( WorksheetImpl ) );

      if( result == null )
        throw new ArgumentException( "PageSetup class must be a leaf of Worksheet object tree" );

      m_worksheet = ( WorksheetImpl )result;
    }
    /// <summary>
    /// Creates necessary records.
    /// </summary>
    private void CreateNecessaryRecords()
    {
      if( m_Guts == null )
        m_Guts = ( GutsRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Guts );

      if( m_DefRowHeight == null )
        m_DefRowHeight = ( DefaultRowHeightRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DefaultRowHeight );
      else
      {
          if (m_DefRowHeight.OptionFlags == 2)
              m_worksheet.IsZeroHeight = true;
      }
      if( m_WSBool == null )
        m_WSBool = ( WSBoolRecord )BiffRecordFactory.GetRecord( TBIFFRecord.WSBool );
    }
    #endregion

    #region Class reader methods
    /// <summary>
    /// Parses record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <returns>True if record was successfully parsed, false otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected override bool ParseRecord( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      bool bResult = base.ParseRecord( record );

      if( !bResult )
      {
        bResult = true;

        switch( record.TypeCode )
        {
          case TBIFFRecord.PrintHeaders:
            PrintHeadersRecord printHeaders = ( PrintHeadersRecord )record;
            m_usPrintHeaders = printHeaders.IsPrintHeaders;
            break;

          case TBIFFRecord.PrintGridlines:
            PrintGridlinesRecord printGridlines = ( PrintGridlinesRecord )record;
            m_usPrintGridlines = printGridlines.IsPrintGridlines;
            break;

          case TBIFFRecord.Gridset:
            GridsetRecord gridset = ( GridsetRecord )record;
            m_usGridset = gridset.GridsetFlag;
            break;

          case TBIFFRecord.Guts:
            m_Guts = ( GutsRecord )record;
            break;

          case TBIFFRecord.DefaultRowHeight:
            m_DefRowHeight = ( DefaultRowHeightRecord )record;
            break;

          case TBIFFRecord.WSBool:
            m_WSBool = ( WSBoolRecord )record;
            break;

          case TBIFFRecord.HorizontalPageBreaks:
            HorizontalPageBreaksRecord hbreaks = ( HorizontalPageBreaksRecord )record;
            HPageBreaks.Parse( hbreaks );
            break;

          case TBIFFRecord.VerticalPageBreaks:
            VerticalPageBreaksRecord vbreaks = ( VerticalPageBreaksRecord )record;
            VPageBreaks.Parse( vbreaks );
            break;

          default:
            bResult = false;
            break;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Recovers page setup from the stream, first record must be PrintHeadersRecord.
    /// </summary>
    /// <param name="reader">Stream that contains all needed records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      throw new NotImplementedException();
    }
    /// <summary>
    /// Skips unknown records.
    /// </summary>
    /// <param name="data">Data with records.</param>
    /// <param name="pos">Starting position.</param>
    private void SkipUnknownRecords( IList data, ref int pos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( pos < 0 || pos > data.Count )
        throw new ArgumentOutOfRangeException( "pos", "Value cannot be less than 0 and greater than data.Count" );

      while( data[ pos ] is UnknownRecord )
      {
        pos++;
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes some records before main page setup block.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeStartRecords( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_Guts == null )
        throw new ArgumentNullException( "m_Guts" );
      
      if( m_DefRowHeight == null )
        throw new ArgumentNullException( "m_DefRowHeight" );
      
      if( m_WSBool == null )
        throw new ArgumentNullException( "m_WSBool" );
      
      FillGutsRecord();
      
      PrintHeadersRecord printHeaders = ( PrintHeadersRecord )BiffRecordFactory.GetRecord( TBIFFRecord.PrintHeaders );
      printHeaders.IsPrintHeaders = m_usPrintHeaders;
      records.Add( printHeaders );

      PrintGridlinesRecord printGridlines = ( PrintGridlinesRecord )BiffRecordFactory.GetRecord( TBIFFRecord.PrintGridlines );
      printGridlines.IsPrintGridlines = m_usPrintGridlines;
      records.Add( printGridlines );

      GridsetRecord gridset = ( GridsetRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Gridset );
      gridset.GridsetFlag = m_usGridset;
      records.Add( gridset );
      records.Add( m_Guts );
      records.Add( m_DefRowHeight );
      records.Add( m_WSBool );

      if( m_hPageBreaks != null ) m_hPageBreaks.Serialize( records );
      if( m_vPageBreaks != null ) m_vPageBreaks.Serialize( records );
    }
    /// <summary>
    /// Fills stream with some records before main page setup records.
    /// </summary>
    /// <param name="writer">Writer to write records into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the serialized data.</returns>
    protected override int FillStreamStart( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      // This method will be called in StoreSize, so there is no need in calling it here.
      //FillGutsRecord();
      
      int iResult = WriteUShortRecord( writer, provider, encryptor, TBIFFRecord.PrintHeaders,
        m_usPrintHeaders, streamPosition );

      iResult += WriteUShortRecord( writer, provider, encryptor, TBIFFRecord.PrintGridlines,
        m_usPrintGridlines, streamPosition + iResult );

      iResult += WriteUShortRecord( writer, provider, encryptor, TBIFFRecord.Gridset,
        m_usGridset, streamPosition + iResult );

      iResult += m_Guts.FillStream( writer, provider, encryptor, streamPosition + iResult );
      iResult += m_DefRowHeight.FillStream( writer, provider, encryptor, streamPosition + iResult );
      iResult += m_WSBool.FillStream( writer, provider, encryptor, streamPosition + iResult );

      if( m_hPageBreaks != null )
        iResult += m_hPageBreaks.FillStream( writer, provider, encryptor, streamPosition + iResult );

      if( m_vPageBreaks != null )
        iResult += m_vPageBreaks.FillStream( writer, provider, encryptor, streamPosition + iResult );

      return iResult;
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Fills internal guts record with information from Rows
    /// and ColumnInfo records.
    /// </summary>
    protected void FillGutsRecord()
    {
      m_Guts.MaxRowLevel = 0;
      m_Guts.MaxColumnLevel = 0;

      //foreach( IOutline outline in m_worksheet.RowInformation.Values )
      int iFirstRow = m_worksheet.FirstRow;

      if( iFirstRow > 0 )
      {
        for( int i = iFirstRow, last = m_worksheet.LastRow; i <= last; i++ )
        {
          IOutline row = WorksheetHelper.GetRowOutline( m_worksheet, i );

          if( row != null && row.OutlineLevel > m_Guts.MaxRowLevel )
            m_Guts.MaxRowLevel = row.OutlineLevel;
        }
      }

      foreach( IOutline outline in m_worksheet.ColumnInformation )//.Values )
      {
        if( outline != null && outline.OutlineLevel > m_Guts.MaxColumnLevel )
        {
          m_Guts.MaxColumnLevel = outline.OutlineLevel;
        }
      }

      if( m_Guts.MaxRowLevel != 0 )
      {
        m_Guts.MaxRowLevel++;
        m_Guts.LeftRowGutter = ( ushort )(( m_Guts.MaxRowLevel * 14 ) - 1);
      }
      else
      {
        m_Guts.LeftRowGutter = 0;
      }

      if( m_Guts.MaxColumnLevel != 0 )
      {
        m_Guts.MaxColumnLevel++;
        m_Guts.TopColumnGutter = ( ushort )(( m_Guts.MaxColumnLevel * 14 ) - 1);
      }
      else
      {
        m_Guts.TopColumnGutter = 0;
      }
    }

    /// <summary>
    /// Initialize collections.
    /// </summary>
    private void InitializeCollections()
    {
//      m_hPageBreaks = new HPageBreaksCollection( Application, this );
//      m_vPageBreaks = new VPageBreaksCollection( Application, this );
    }
    /// <summary>
    /// Converts cell range to 3d Range name.
    /// </summary>
    /// <param name="value">cell range.</param>
    /// <returns>3d range name.</returns>
    protected string ConvertTo3dRangeName( string value )
    {
      Match match = FormulaUtil.CellRangeRegex.Match( value );

      if( match.Success )
      {
        return "'" + m_worksheet.Name + "'!" +
          match.Result( "${Column1}${Row1}:${Column2}${Row2}" );
      }

      Match m1 = FormulaUtil.CellRegex.Match( value );

      if( m1.Success )
      {
        return "'" + m_worksheet.Name + "'!" + m1.Result( "${Column1}${Row1}" );
      }

      match = FormulaUtil.FullRowRangeRegex.Match( value );

      if( match.Success )
      {
        return "'" + m_worksheet.Name + "'!" + value;
      }

      match = FormulaUtil.FullColumnRangeRegex.Match( value );

      if( match.Success )
      {
        return "'" + m_worksheet.Name + "'!" + value;
      }

      return null;
    }
    /// <summary>
    /// Parses print area expression.
    /// </summary>
    /// <param name="value">Value representing print area.</param>
    protected void ParsePrintAreaExpression( string value )
    {
      // Remove print area settings if string empty.
      if( value == null || value.Length == 0 )
      {
          m_worksheet.Names.Remove(DEF_AREA_XlS);
        return;
      }

      NameImpl name = m_worksheet.Workbook.Version== ExcelVersion.Excel97to2003 ?
          m_worksheet.InnerNames.GetOrCreateName( DEF_AREA_XlS ) : m_worksheet.InnerNames.GetOrCreateName(DEF_AREA_XlSX);
      NameRecord record = name.Record;
      //record.IndexOrGlobal = ( ushort )( m_worksheet.Index + 1 );

      int iCount = 0;
      WorkbookImpl book = m_worksheet.ParentWorkbook;
      FormulaUtil formulaParser = book.FormulaUtil;
      bool bR1C1 = book.CalculationOptions.R1C1ReferenceMode;
      int iSheetReference = book.AddSheetReference( m_worksheet );

      Dictionary<Type, ReferenceIndexAttribute> indexes = new Dictionary<Type, ReferenceIndexAttribute>();
      indexes.Add( typeof( Area3DPtg ), new ReferenceIndexAttribute( 1 ) );
      indexes.Add( typeof( Ref3DPtg ), new ReferenceIndexAttribute( 1 ) );
      indexes.Add( typeof( AreaPtg ), new ReferenceIndexAttribute( 1 ) );
      indexes.Add( typeof( RefPtg ), new ReferenceIndexAttribute( 1 ) );

      ExcelParseFormulaOptions options = bR1C1
        ? ExcelParseFormulaOptions.InName | ExcelParseFormulaOptions.UseR1C1
        : ExcelParseFormulaOptions.InName;

      Ptg[] arrDataTokens = formulaParser.ParseString( value, m_worksheet, indexes, 0, null,
        options, 0, 0 );

      int iLength = arrDataTokens.Length;
      Ptg[] arrResult = new Ptg[ iLength ];
      ExcelVersion version = m_worksheet.ParentWorkbook.Version;
      
      for( int i = 0; i < iLength; i++ )
      {
        Ptg token = arrDataTokens[ i ];
        IToken3D range = token as IToken3D;

        if( range != null )
        {
          token = range.Get3DToken( iSheetReference );
        }

        if( Array.IndexOf( DEF_PRINT_AREA_TOKENS, token.TokenCode ) == -1 )
          throw new ArgumentException( "Print area has incorrect format" );

        arrResult[ i /*+ 1*/ ] = token;
        iCount += token.GetSize( version );
      }

//      MemFuncPtg memPtg = new MemFuncPtg( iCount );
//      arrResult[ 0 ] = memPtg;
      record.FormulaTokens = arrResult;

      ( name as IParseable ).Parse();
    }
    /// <summary>
    /// Parses column print title string and makes appropriate changes to default title named range.
    /// </summary>
    /// <param name="value">Column print title string to set.</param>
    protected void ParsePrintTitleColumns( string value )
    {
      bool bValue = ( value != null && value.Length > 0 );

      NameImpl name = m_worksheet.Workbook.Version == ExcelVersion.Excel97to2003 ?
                                    m_worksheet.InnerNames.GetOrCreateName(DEF_TITLE_XLS)
                                    : m_worksheet.InnerNames.GetOrCreateName(DEF_TITLE_XLSX);
      NameRecord record = name.Record;

      Ptg[] recordTokens = record.FormulaTokens;
      Area3DPtg rowPtg = null;

      if( recordTokens != null )
      {
        int iLength = recordTokens.Length;

        if( iLength == 4 )
        {
          rowPtg = recordTokens[ 2 ] as Area3DPtg;
        }
        else if( iLength == 1 )
        {
          Area3DPtg ptg = recordTokens[ 0 ] as Area3DPtg;
          IWorkbook book = m_worksheet.Workbook;

          if( ptg.FirstRow != 0 || ptg.LastRow != book.MaxRowCount - 1 )
          {
            rowPtg = ptg;
          }
        }
      }

      Ptg column = null;

      if( bValue )
      {
        value = ConvertTo3dRangeName( value );
        WorkbookImpl book = ( WorkbookImpl )m_worksheet.ParentWorkbook;
        Ptg[] ptgTokens = book.FormulaUtil.ParseString( value );
        //column = new Area3DPtg( value, m_worksheet.Workbook );
        column = ptgTokens[ 0 ];
        column.TokenCode = FormulaToken.tArea3d1;
      }

      List<Ptg> tokens = new List<Ptg>();

      // all values set
      if( rowPtg != null && bValue )
      {
        WorkbookImpl book = m_worksheet.ParentWorkbook;
        FormulaUtil formulaUtil = book.FormulaUtil;
        ExcelVersion version = book.Version;

        Ptg delim = FormulaUtil.CreatePtg( FormulaToken.tCellRangeList, formulaUtil.OperandsSeparator );
        int count = column.GetSize( version ) + rowPtg.GetSize( version ) + delim.GetSize( version );
        tokens.AddRange( new Ptg[] { new MemFuncPtg( count ), column, rowPtg, delim } );
      }
      // set only rows
      else if( rowPtg != null && !bValue )
      {
        tokens.Add( rowPtg );
      }
      else if( bValue )
      {
        tokens.Add( column );
      }
      else
      {
        m_worksheet.Names.Remove( DEF_TITLE_XLS );
        return;
      }

      record.FormulaTokens = tokens.ToArray();
    }
    /// <summary>
    /// Parses row print title string and makes appropriate changes to default title named range.
    /// </summary>
    /// <param name="value">Row print title string to set.</param>
    protected void ParsePrintTitleRows( string value )
    {
      bool bValue = ( value != null && value.Length > 0 );

      NameImpl name =m_worksheet.Workbook.Version == ExcelVersion.Excel97to2003 ?
                                    m_worksheet.InnerNames.GetOrCreateName( DEF_TITLE_XLS )
                                    : m_worksheet.InnerNames.GetOrCreateName(DEF_TITLE_XLSX);
      NameRecord record = name.Record;

      Ptg[] recordTokens = record.FormulaTokens;
      Area3DPtg columnPtg = null;

      if( recordTokens != null )
      {

        int iLength = recordTokens.Length;

        if( iLength == 4 )
        {
          columnPtg = recordTokens[ 1 ] as Area3DPtg;
        }
        else if( iLength == 1 )
        {
          Area3DPtg ptg = recordTokens[ 0 ] as Area3DPtg;
          IWorkbook book = m_worksheet.Workbook;

          if( ptg.FirstRow == 0 || ptg.LastRow == book.MaxRowCount - 1 )
          {
            columnPtg = ptg;
          }
        }
      }

      Ptg row = null;

      if( bValue )
      {
        value = ConvertTo3dRangeName( value );
        WorkbookImpl book = ( WorkbookImpl )m_worksheet.ParentWorkbook;
        Ptg[] ptgTokens = book.FormulaUtil.ParseString( value );
        //row = new Area3DPtg( value, m_worksheet.Workbook );
        row = ptgTokens[ 0 ];
        row.TokenCode = FormulaToken.tArea3d1;
      }

      List<Ptg> tokens = new List<Ptg>();

      // All values set.
      if( columnPtg != null && bValue )
      {
        WorkbookImpl book = m_worksheet.ParentWorkbook;
        FormulaUtil formulaUtil = book.FormulaUtil;
        ExcelVersion version = book.Version;

        Ptg delim = FormulaUtil.CreatePtg( FormulaToken.tCellRangeList, formulaUtil.OperandsSeparator );
        int count = columnPtg.GetSize( version ) + row.GetSize( version ) + delim.GetSize( version );
        tokens.AddRange( new Ptg[] { new MemFuncPtg( count ), columnPtg, row, delim } );
      }
      // Set only rows.
      else if( columnPtg != null && !bValue )
      {
        tokens.Add( columnPtg );
      }
      else if( bValue )
      {
        tokens.Add( row );
      }
      else
      {
        m_worksheet.Names.Remove( DEF_TITLE_XLS );
        return;
      }

      record.FormulaTokens = tokens.ToArray();
    }
    /// <summary>
    /// Extracts print area string.
    /// </summary>
    /// <returns>Print area string.</returns>
    //protected string ExtractPrintArea()
    protected string ExtractPrintArea()
    {
      INames names = m_worksheet.Names;
      NameImpl area = m_worksheet.Workbook.Version== ExcelVersion.Excel97to2003 ?
          ( NameImpl )names[ DEF_AREA_XlS ] : (NameImpl)names[DEF_AREA_XlSX];

      if( area != null )
      {
        NameRecord record = ( ( NameImpl )area ).Record;
        return GetAddressGlobalWithoutName( record.FormulaTokens );
      }

      return null;
    }
    /// <summary>
    /// Extracts row or column print title from default title named range.
    /// </summary>
    /// <param name="bRowExtract">Defines which print title must be extracted.</param>
    /// <returns>Row/column print title string.</returns>
    //protected string ExtractPrintTitleRowColumn( bool bRowExtract )
    protected string ExtractPrintTitleRowColumn( bool bRowExtract )
    {
      INames names = m_worksheet.Names;
      NameImpl title = m_worksheet.Workbook.Version== ExcelVersion.Excel97to2003?
                          ( NameImpl )names[ DEF_TITLE_XLS ]
                          :(NameImpl)names[DEF_TITLE_XLSX];

      if( title != null )
      {
        NameRecord titleRec = ( ( NameImpl )title ).Record;
        Ptg[] tokens = titleRec.FormulaTokens;

        if( tokens.Length <= 0 || tokens.Length > 4 )
          throw new ArgumentOutOfRangeException( "Print_Titles Name record", "Print_Titles Name record has wrong quantity of formula tokens." );

        if( tokens.Length == 4 )
        {
          if( bRowExtract )
          {
            return GetAddressGlobalWithoutName( new Ptg[] { tokens[ 2 ] } );
          }
          else
          {
            return GetAddressGlobalWithoutName( new Ptg[] { tokens[ 1 ] } );
          }
        }
        else if (tokens.Length == 3)
        {
            if (bRowExtract)
            {
                return GetAddressGlobalWithoutName(new Ptg[] { tokens[1] });
            }
            else
            {
                return GetAddressGlobalWithoutName(new Ptg[] { tokens[0] });
            }

        }
        else if( tokens.Length == 1 )
        {
          string value = GetAddressGlobalWithoutName( tokens );
          Area3DPtg ptg = tokens[ 0 ] as Area3DPtg;

          if( bRowExtract )
          {
              if (ptg.FirstRow == 0 && ptg.LastRow == m_worksheet.ParentWorkbook.MaxRowCount - 1)
            {
              return null;
            }
            else
            {
              return value;
            }
          }
          else
          {
              if (ptg.FirstRow == 0 && ptg.LastRow == m_worksheet.ParentWorkbook.MaxRowCount - 1)
            {
              return value;
            }
            else
            {
              return null;
            }
          }
        }
      }

      return null;
    }
    /// <summary>
    /// Returns global address without sheet name.
    /// </summary>
    /// <param name="token">Formula token.</param>
    /// <returns>Global address without sheet name.</returns>
    protected string GetAddressGlobalWithoutName( Ptg[] token )
    {
      WorkbookImpl book = m_worksheet.ParentWorkbook;
      FormulaUtil formulaParser = book.FormulaUtil;

      string strResult = formulaParser.ParsePtgArray( token, 0, 0, false, null, true );
      //RangeImpl.GetWorksheetName( ref strResult );

      return strResult;
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent for the new instance.</param>
    /// <returns>A clone of the current instance.</returns>
    public PageSetupImpl Clone( object parent )
    {
      PageSetupImpl result = ( PageSetupImpl )MemberwiseClone();

      result.SetParent( parent );
      result.FindParents();

      m_Guts = ( GutsRecord )CloneUtils.CloneCloneable( m_Guts );
      m_DefRowHeight = ( DefaultRowHeightRecord )CloneUtils.CloneCloneable( m_DefRowHeight );
      m_WSBool = ( WSBoolRecord )CloneUtils.CloneCloneable( m_WSBool );
      m_unknown = ( PrinterSettingsRecord )CloneUtils.CloneCloneable( m_unknown );
      m_setup = ( PrintSetupRecord )CloneUtils.CloneCloneable( m_setup );
      if (m_headerFooter!=null)
      m_headerFooter = (HeaderAndFooterRecord)CloneUtils.CloneCloneable(m_headerFooter);
      m_arrHeaders = CloneUtils.CloneStringArray( m_arrHeaders );
      m_arrFooters = CloneUtils.CloneStringArray( m_arrFooters );

      if( m_hPageBreaks != null )
        result.m_hPageBreaks = ( HPageBreaksCollection )m_hPageBreaks.Clone( result );

      if( m_vPageBreaks != null )
        result.m_vPageBreaks = ( VPageBreaksCollection )m_vPageBreaks.Clone( result );

      //result.ExtractPrintAreaTitle();

      return result;
    }
    #endregion

      #region Dispose Methods
    public override void Dispose()
    {
        base.Dispose();
        if (m_hPageBreaks != null)
        {
            this.m_hPageBreaks.Clear();
  }
        if (m_vPageBreaks != null)
        {
            this.m_vPageBreaks.Clear();
        }
        //this.m_worksheet = null;
        if (dictPaperHeight != null)
        {
            dictPaperHeight.Clear();
            dictPaperHeight = null;
        }
        if (dictPaperWidth != null)
        {
            dictPaperWidth.Clear();
            dictPaperWidth = null;
        }
        this.m_arrHeaders = null;
        if (m_backgroundImage != null)
        {
            this.m_backgroundImage.Dispose();
        }
        m_bIsDisposed = true;
    }
      #endregion
  }
}
