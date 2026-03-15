#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains the token array and the result of a formula cell.
  /// </summary>
  [ Biff( TBIFFRecord.Formula ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class FormulaRecord
    : CellPositionBase
    , ICellPositionFormat
    , ICloneable
    , IDoubleValue
    , IFormulaRecord
  {
    #region Class constants
    /// <summary>
    /// Represents first mask
    /// </summary>
    public const ulong DEF_FIRST_MASK = 0xffff0000000000ff;
    /// <summary>
    /// Represents boolean mask.
    /// </summary>
    public const ulong DEF_BOOL_MASK = 0xffff000000000001;
    /// <summary>
    /// Represents error mask.
    /// </summary>
    public const ulong DEF_ERROR_MASK = 0xffff000000000002;
    /// <summary>
    /// Represents error mask.
    /// </summary>
    public const ulong DEF_BLANK_MASK = 0xffff000000000003;
    /// <summary>
    /// String mask.
    /// </summary>
    //public const ulong DEF_STRING_MASK = 0xFFFFFF00000000FF;
    public const ulong DEF_STRING_MASK = 0xFFFF000000000000;
    /// <summary>
    /// String mask.
    /// </summary>
    public const ulong DEF_STRING_MASK_VALUE = 0xFFFF070000000000;
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 22;
    /// <summary>
    /// Formula value in the case of string value.
    /// </summary>
    private const ulong DEF_STRING_VALUE_ULONG = 0xFFFF07DA00180000;
    /// <summary>
    /// Formula value in the case of blank value.
    /// </summary>
    private const ulong DEF_BLANK_VALUE_ULONG = 0xFFFF030064007D07;
    /// <summary>
    /// 
    /// </summary>
    public static readonly long DEF_STRING_VALUE_LONG;
    /// <summary>
    /// 
    /// </summary>
    public static readonly long DEF_BLANK_VALUE_LONG;
    /// <summary>
    /// 
    /// </summary>
    public static readonly double DEF_STRING_VALUE;
    /// <summary>
    /// Offset to the formula value.
    /// </summary>
    private const int FormulaValueOffset = BiffRecordRaw.DEF_HEADER_SIZE + 6;
    /// <summary>
    /// Size of the data before expression that belongs only to formula (without row, column, xf indexes).
    /// </summary>
    private const int DataSizeBeforeExpression = 22 - 6;
    #endregion

    #region Class members
    /// <summary>
    /// The calculated value of the formula.
    /// </summary>
    [ BiffRecordPos( 6, 8, TFieldType.Float ) ]
    private double m_dbValue = 0;
    /// <summary>
    /// The option flags.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usOptions = 0;

    #region Options bit flags

    /// <summary>
    /// True to always recalculate.
    /// </summary>
    [ BiffRecordPos( 14, 0, TFieldType.Bit ) ]
    private bool m_bRecalculateAlways = false;

    /// <summary>
    /// True to calculate on open.
    /// </summary>
    [ BiffRecordPos( 14, 1, TFieldType.Bit ) ]
    private bool m_bCalculateOnOpen = false;

    /// <summary>
    /// True if part of a shared formula.
    /// </summary>
    [ BiffRecordPos( 14, 3, TFieldType.Bit ) ]
    private bool m_bPartOfSharedFormula = false;

    #endregion

    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 16, 4, true ) ]
    private int    m_iReserved = 0;
    /// <summary>
    /// Size of the following formula data.
    /// </summary>
    [ BiffRecordPos( 20, 2 ) ]
    private ushort m_usExpressionLen = 0;
    /// <summary>
    /// Formula data (RPN token array).
    /// </summary>
    private byte[] m_expression = null;
    /// <summary>
    /// Array that contains all parsed tokens.
    /// </summary>
    private Ptg[]  m_arrParsedExpression = null;
    /// <summary>
    /// 
    /// </summary>
    private bool  m_bFillFromExpression;
    #endregion

    #region Class properties
    /// <summary>
    /// The calculated value of the formula.
    /// </summary>
    public double Value
    {
      get
      {
        return m_dbValue;
      }
      set
      {
        m_dbValue = value;
      }
    }

    /// <summary>
    ///  Option flags of the formula.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
      set
      {
        m_usOptions = value;
      }
    }

    /// <summary>
    /// True to always recalculate.
    /// </summary>
    public bool RecalculateAlways
    {
      get
      {
        return m_bRecalculateAlways;
      }
      set
      {
        m_bRecalculateAlways = value;
      }
    }

    /// <summary>
    /// True to calculate on open.
    /// </summary>
    public bool CalculateOnOpen
    {
      get
      {
        return m_bCalculateOnOpen;
      }
      set
      {
        m_bCalculateOnOpen = value;
      }
    }

    /// <summary>
    /// True if part of a shared formula.
    /// </summary>
    public bool PartOfSharedFormula
    {
      get
      {
        return m_bPartOfSharedFormula;
      }
      set
      {
        m_bPartOfSharedFormula = value;
      }
    }

    /// <summary>
    /// Array that contains all parsed tokens.
    /// </summary>
    public Ptg[] ParsedExpression
    {
      get
      {
        return m_arrParsedExpression;
      }
      set
      {
        m_arrParsedExpression = value;

        if( value != null )
        {
          int Len;
          m_expression = FormulaUtil.PtgArrayToByteArray( value, out Len, ExcelVersion.Excel2007 );
          m_usExpressionLen = ( ushort ) Len;
        }
        else
        {
          m_expression = null;
          m_usExpressionLen = 0;
        }
      }
    }
    /// <summary>
    /// Read-only. Reserved.
    /// </summary>
    public int Reserved
    {
      get
      {
        return m_iReserved;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 24;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFillFromExpression
    {
      get
      {
        return m_bFillFromExpression;
      }
      set
      {
        m_bFillFromExpression = value;
      }
    }
    /// <summary>
    /// The calculated value of the formula.
    /// </summary>
    public double DoubleValue
    {
      get
      {
        return m_dbValue;
      }
    }
    /// <summary>
    /// Indicates if formula record contain bool value. Read-only.
    /// </summary>
    public bool IsBool
    {
      get
      {
        ulong lValue = ( ulong )BitConverterGeneral.DoubleToInt64Bits( m_dbValue );

        return ( lValue & DEF_FIRST_MASK ) == DEF_BOOL_MASK;
      }
    }
    /// <summary>
    /// Indicates if formula record contain error value. Read-only.
    /// </summary>
    public bool IsError
    {
      get
      {
        ulong lValue = ( ulong )BitConverterGeneral.DoubleToInt64Bits( m_dbValue );

        return ( lValue & DEF_FIRST_MASK ) == DEF_ERROR_MASK;
      }
    }
    public bool IsBlank
    {
      get
      {
        ulong lValue = ( ulong )BitConverterGeneral.DoubleToInt64Bits( m_dbValue );
        return ( lValue & DEF_FIRST_MASK ) == DEF_BLANK_MASK;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool HasString
    {
      get
      {
        ulong ulValue = ( ulong )BitConverterGeneral.DoubleToInt64Bits( m_dbValue );
        return ( ( ulValue & DEF_FIRST_MASK ) == DEF_STRING_MASK );
        
        //return m_dbValue == DEF_STRING_VALUE;//BitConverterGeneral.DoubleToInt64Bits( m_dbValue ) == DEF_STRING_VALUE;
      }
      set
      {
        m_dbValue = DEF_STRING_VALUE;//BitConverterGeneral.Int64BitsToDouble( DEF_STRING_VALUE );
      }
    }
    /// <summary>
    /// The calculated boolean value of the formula.
    /// </summary>
    public bool BooleanValue
    {
      get
      {
        if( IsBool )
        {
          long lResult = BitConverterGeneral.DoubleToInt64Bits( m_dbValue );

          lResult = lResult & 0xff0000;

          return lResult > 0;
        }

        return false;
      }
      set
      {
        SetBoolErrorValue( value ? ( byte )1 : ( byte )0, false );
      }
    }
    /// <summary>
    /// The calculated error value of the formula.
    /// </summary>
    public byte ErrorValue
    {
      get
      {
        if( IsError )
        {
          long lResult = BitConverterGeneral.DoubleToInt64Bits( m_dbValue );

          lResult = lResult & 0xff0000;

          return ( byte )( lResult >> 16 );
        }

        return 0;
      }
      set
      {
        SetBoolErrorValue( value, true );
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    static FormulaRecord()
    {
      ulong ulValue = DEF_STRING_VALUE_ULONG;
      DEF_STRING_VALUE_LONG = ( long )ulValue;
      DEF_STRING_VALUE = BitConverterGeneral.Int64BitsToDouble( DEF_STRING_VALUE_LONG );

      ulValue = DEF_BLANK_VALUE_ULONG;
      DEF_BLANK_VALUE_LONG = ( long )ulValue;
    }
    /// <summary>
    /// Default constructor
    /// </summary>
    public  FormulaRecord()
      : base()
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void ParseCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_dbValue = provider.ReadDouble( iOffset );
      iOffset += ExcelConstants.DoubleSize;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bPartOfSharedFormula = provider.ReadBit( iOffset, 3 );
      m_bCalculateOnOpen = provider.ReadBit( iOffset, 1 );
      m_bRecalculateAlways = provider.ReadBit( iOffset, 0 );
      iOffset += ExcelConstants.ShortSize;

      m_iReserved = provider.ReadInt32( iOffset );
      iOffset += ExcelConstants.IntSize;

      m_usExpressionLen = provider.ReadUInt16( iOffset );
      iOffset += ExcelConstants.ShortSize;

      m_expression = new byte[ m_usExpressionLen ];
      provider.ReadArray( iOffset, m_expression );
      ParseFormula( provider, iOffset, version );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void InfillCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      PrepareExpression( version );
      //CalculateOnOpen = true;
      //RecalculateAlways = true;

      provider.WriteDouble( iOffset, m_dbValue );
      iOffset += ExcelConstants.DoubleSize;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bPartOfSharedFormula, 3 );
      provider.WriteBit( iOffset, m_bCalculateOnOpen, 1 );
      provider.WriteBit( iOffset, m_bRecalculateAlways, 0 );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt32( iOffset, m_iReserved );
      iOffset += ExcelConstants.IntSize;

      provider.WriteUInt16( iOffset, m_usExpressionLen );
      iOffset += ExcelConstants.ShortSize;

      //m_iLength = 22;

      if( m_usExpressionLen != 0 )
      {
        //SetBytes( arrBuffer, iOffset + 22, m_expression, 0, m_expression.Length );
        //Buffer.BlockCopy( m_expression, 0, arrBuffer, iOffset + 22, m_expression.Length );
        provider.WriteBytes( iOffset, m_expression, 0, m_expression.Length );
        //m_iLength += m_expression.Length;
      }
    }
    /// <summary>
    /// Parses formula expression and fill and provides internal integrity data check.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Record's data offset.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    private void ParseFormula( DataProvider provider, int iOffset, ExcelVersion version )
    {
      int iCorrectOffset = iOffset;// + DEF_FIXED_SIZE;
      int finalOffset;

      try
      {
        //m_arrParsedExpression = FormulaUtil.ParseExpression( new ByteArrayDataProvider( m_expression ), 0, m_usExpressionLen, out finalOffset );
        ParsedExpression = FormulaUtil.ParseExpression( provider, iCorrectOffset,
          m_usExpressionLen, out finalOffset, version );
      }
      catch( Exception ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message, "Exception message" );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "StackTrace" );

        finalOffset = 0;
      }

      // NOTE: this exception is not quite correct, it can be uncommented if necessary.
//      if( finalOffset != iOffset + m_iLength - DEF_FIXED_SIZE )
//      {
//        throw new WrongBiffRecordDataException( "FormulaRecord" );
//      }
    }
    /// <summary>
    /// Prepares expression field.
    /// </summary>
    private void PrepareExpression( ExcelVersion version )
    {
      //if( m_bFillFromExpression )
      {
        int iFormulaLen;

        if( m_arrParsedExpression != null && m_arrParsedExpression.Length > 0 )
        {
          m_expression = FormulaUtil.PtgArrayToByteArray( ParsedExpression, out iFormulaLen, version );
          m_usExpressionLen = ( ushort )iFormulaLen;
        }
        else
        {
          m_expression = null;
          m_usExpressionLen = 0;
        }

        m_bFillFromExpression = false;
      }
    }
    /// <summary>
    /// Sets bool or error value.
    /// </summary>
    /// <param name="value">Represents bool or error byte value.</param>
    /// <param name="bIsError">Indicates if this is error.</param>
    private void SetBoolErrorValue( byte value, bool bIsError )
    {
      m_dbValue = GetBoolErrorValue( value, bIsError );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DEF_FIXED_SIZE + DVRecord.GetFormulaSize( m_arrParsedExpression, version, true );

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    /// <summary>
    /// Converts error or bool value to formula double value.
    /// </summary>
    /// <param name="value">Represents error or bool value.</param>
    /// <param name="bIsError">Indicates is error or bool.</param>
    /// <returns>Returns formula value.</returns>
    public static double GetBoolErrorValue( byte value, bool bIsError )
    {
      long result = 0xffff000000;
      result = result << 8;
      result += value;
      result = result << 16;
      result += bIsError ? 2 : 1;

      return BitConverterGeneral.Int64BitsToDouble( result );
    }
    /// <summary>
    /// Sets flags so MS Excel understands that formula returns string.
    /// </summary>
    /// <param name="dataProvider">Object that provides access to the data.</param>
    /// <param name="iFormulaOffset">Offset to the FormulaRecord..</param>
    public static void SetStringValue( DataProvider dataProvider, int iFormulaOffset, ExcelVersion version )
    {
      if( version != ExcelVersion.Excel97to2003 )
      {
        iFormulaOffset += ExcelConstants.IntSize; // 2 bytes for row, 2 bytes for column
      }

      dataProvider.WriteInt64( iFormulaOffset + FormulaValueOffset, DEF_STRING_VALUE_LONG );
    }
    /// <summary>
    /// Sets flags so MS Excel understands that formula returns string.
    /// </summary>
    /// <param name="dataProvider">Object that provides access to the data.</param>
    /// <param name="iFormulaOffset">Offset to the FormulaRecord..</param>
    public static void SetBlankValue( DataProvider dataProvider, int iFormulaOffset, ExcelVersion version )
    {
      if( version != ExcelVersion.Excel97to2003 )
      {
        iFormulaOffset += ExcelConstants.IntSize; // 2 bytes for row, 2 bytes for column
      }

      dataProvider.WriteInt64( iFormulaOffset + FormulaValueOffset, DEF_BLANK_VALUE_LONG );
    }
    /// <summary>
    /// Reads record's value from the data provider.
    /// </summary>
    /// <param name="provider">Provider to read data from.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version used to infill data.</param>
    /// <returns>Record's value.</returns>
    public static Ptg[] ReadValue( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += DEF_HEADER_SIZE + ExcelConstants.IntSize + ExcelConstants.ShortSize; // row, column + xf

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize;
      }

      recordStart += DataSizeBeforeExpression - ExcelConstants.ShortSize;
      int iExpressionSize = provider.ReadUInt16( recordStart );
      recordStart += ExcelConstants.ShortSize;
      int iFinalOffset;

      return FormulaUtil.ParseExpression( provider, recordStart, iExpressionSize,
        out iFinalOffset, version );
    }
    /// <summary>
    /// Reads record's value from the data provider.
    /// </summary>
    /// <param name="provider">Provider to read data from.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version used to infill data.</param>
    /// <returns>Record's value.</returns>
    public static long ReadInt64Value( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += FormulaValueOffset;

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize; // 2 bytes for row, 2 bytes for column
      }

      return provider.ReadInt64( recordStart );
    }
    /// <summary>
    /// Reads record's value from the data provider.
    /// </summary>
    /// <param name="provider">Provider to read data from.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version used to infill data.</param>
    /// <returns>Record's value.</returns>
    public static double ReadDoubleValue( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += FormulaValueOffset;

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize; // 2 bytes for row, 2 bytes for column
      }

      return provider.ReadDouble( recordStart );
    }
    /// <summary>
    /// Writes record's value into the data provider.
    /// </summary>
    /// <param name="provider">Provider to write data into.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version used to infill data.</param>
    /// <param name="value">Record's value.</param>
    public static void WriteDoubleValue( DataProvider provider, int recordStart,
      ExcelVersion version, double value )
    {
      recordStart += FormulaValueOffset;

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize; // 2 bytes for row, 2 bytes for column
      }

      provider.WriteDouble( recordStart, value );
    }
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="data"></param>
    //public static void UpdateOptions( byte[] data )
    //{
    //  const int OptionsOffset = ExcelConstants.DoubleSize + BiffRecordRaw.DEF_HEADER_SIZE + 6;
    //  byte btOptions = data[ OptionsOffset ];
    //  btOptions |= 3;
    //  btOptions &= 0xFF-8;
    //  data[ OptionsOffset ] = btOptions;
    //  //byte[] bytes = BitConverter.GetBytes( btOptions );
    //  //Buffer.BlockCopy( bytes, 0, data, OptionsOffset, bytes.Length );
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public static void UpdateOptions( DataProvider provider, int iOffset )
    {
      const int OptionsOffset = ExcelConstants.DoubleSize + BiffRecordRaw.DEF_HEADER_SIZE + 6;
      byte btOptions = provider.ReadByte( iOffset + OptionsOffset );
      btOptions |= 3;
      btOptions &= 0xFF - 8;
      provider.WriteByte( iOffset + OptionsOffset, btOptions );
      //byte[] bytes = BitConverter.GetBytes( btOptions );
      //Buffer.BlockCopy( bytes, 0, data, OptionsOffset, bytes.Length );
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    new public object Clone()
    {
      FormulaRecord result = ( FormulaRecord )base.Clone();

      if( m_expression != null )
      {
        int len = m_expression.Length;
        m_expression = new byte[ len ];

        for( int i = len - 1; i >= 0; i-- )
        {
          m_expression[ i ] = result.m_expression[ i ];
        }
      }

      if( m_arrParsedExpression != null )
      {
        int len = m_arrParsedExpression.Length;
        m_arrParsedExpression = new Ptg[ len ];

        for( int i = len - 1; i >= 0; i-- )
        {
          m_arrParsedExpression[ i ] = ( Ptg )result.m_arrParsedExpression[ i ].Clone();
        }
      }

      return result;
    }
    #endregion

    #region IFormulaRecord Members
    /// <summary>
    /// Array that contains all parsed tokens.
    /// </summary>
    public Ptg[] Formula
    {
      get
      {
        return ParsedExpression;
      }
      set
      {
        ParsedExpression = value;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Converts formula tokens from Excel97to2003 to Excel2007 version and vice versa.
    /// </summary>
    /// <param name="tokens">Formula tokens.</param>
    /// <param name="bFromExcel07To97">Defines what conversion must be applied.</param>
    public static void ConvertFormulaTokens( Ptg[] tokens, bool bFromExcel07To97 )
    {
      if( tokens == null )
        return;

      for( int i = 0; i < tokens.Length; i++ )
      {
        AttrPtg attrPtg = tokens[ i ] as AttrPtg;

        if( attrPtg != null && ( attrPtg.HasOptGoto || attrPtg.HasOptimizedIf ) )
        {
          ConvertFormulaGotoToken( tokens, i, bFromExcel07To97 );
        }

        AreaPtg areaPtg = tokens[ i ] as AreaPtg;

        if( areaPtg != null )
        {
          tokens[ i ] = areaPtg.ConvertFullRowColumnAreaPtgs( bFromExcel07To97 );
          if (bFromExcel07To97)
          {
              if (tokens[i].TokenCode == FormulaToken.tArea3d2)
              {
                  tokens[i].TokenCode = FormulaToken.tArea3d1;
              }
          }
        }
      }
    }
    /// <summary>
    /// Converts formula GOTO and IF token from Excel97to2003 to Excel2007 version and vice versa.
    /// </summary>
    /// <param name="formulaTokens">Formula tokens.</param>
    /// <param name="iGotoTokenIndex">GOTO token index.</param>
    /// <param name="bFromExcel07To97">Defines what conversion must be applied.</param>
    private static void ConvertFormulaGotoToken( Ptg[] formulaTokens, int iGotoTokenIndex, bool bFromExcel07To97 )
    {
      ushort usCurrentSize = 0;
      AttrPtg attrPtg = ( AttrPtg )formulaTokens[ iGotoTokenIndex ];
      ushort usSize = attrPtg.AttrData;
      int iCurrentsToken = iGotoTokenIndex + 1;
      ushort usNewAttrData = 0;

      ExcelVersion currentVersion;
      ExcelVersion newVersion;

      if( bFromExcel07To97 )
      {
        currentVersion = ExcelVersion.Excel2007;
        newVersion = ExcelVersion.Excel97to2003;
      }
      else
      {
        currentVersion = ExcelVersion.Excel97to2003;
        newVersion = ExcelVersion.Excel2007;
      }
      
      do
      {
        Ptg token = formulaTokens[ iCurrentsToken ];
        usNewAttrData += ( ushort )token.GetSize( newVersion );
        usCurrentSize += ( ushort )token.GetSize( currentVersion );
        iCurrentsToken++;
      }
      while( usCurrentSize < usSize );

      if( attrPtg.HasOptimizedIf )
      {
        attrPtg.AttrData = usNewAttrData;
      }
      else
      {
        attrPtg.AttrData = ( ushort )( usNewAttrData - 1 );
      }
    }
    #endregion
  }
}
