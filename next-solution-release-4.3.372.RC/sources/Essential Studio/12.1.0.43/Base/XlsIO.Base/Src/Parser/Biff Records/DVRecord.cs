#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

#region file using directives
using System;
using System.IO;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record is part of the Data Validity Table. It stores data validity settings
  /// and a list of cell ranges which contain these settings. The "prompt box" appears
  /// while editing such a cell. The "error box" appears if the entered value does not
  /// fit the conditions. The data validity settings of a sheet are stored in a sequential
  /// list of DV records. This list is preluded by a DVAL record. If a string is empty
  /// and the default text should appear in the prompt box or error box, the string
  /// must contain a single zero character (string length will be 1).
  /// </summary>
  [ Biff( TBIFFRecord.DV ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DVRecord :
      BiffRecordRawWithArray,
      ICloneable
  {
    #region Class constants
    /// <summary>
    /// Bit mask for data type.
    /// </summary>
    public const uint DataTypeBitMask     = 0x0000000F;
    /// <summary>
    /// Bit mask for error style.
    /// </summary>
    public const uint ErrorStyleBitMask   = 0x00000070;
    /// <summary>
    /// Bit mask for condition.
    /// </summary>
    public const uint ConditionBitMask    = 0x00F00000;
    /// <summary>
    /// Start bit of the error style in options.
    /// </summary>
    public const int  ErrorStyleStartBit  = 4;
    /// <summary>
    /// Start bit of the condition in options.
    /// </summary>
    public const int  ConditionStartBit   = 20;
    /// <summary>
    /// 
    /// </summary>
    public const string StringEmpty       = "\0";
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private uint m_uiOptions = 0;

    #region Options bit fields
    //TODO: Figure out a name for the variable.
    /// <summary>
    /// True if in list type validity, the string list is explicitly given in the formula.
    /// </summary>
    [ BiffRecordPos( 0, 7, TFieldType.Bit ) ]
    private bool m_bStrListExplicit = false;

    /// <summary>
    /// True if empty cells are allowed.
    /// </summary>
    [ BiffRecordPos( 1, 0, TFieldType.Bit ) ]
    private bool m_bEmptyCell = true;

    /// <summary>
    /// True to suppress the drop-down arrow in list type validity.
    /// </summary>
    [ BiffRecordPos( 1, 1, TFieldType.Bit ) ]
    private bool m_bSuppressArrow = false;

    /// <summary>
    /// True to show prompt box if cell selected.
    /// </summary>
    [ BiffRecordPos( 2, 2, TFieldType.Bit ) ]
    private bool m_bShowPromptBox = true;

    /// <summary>
    /// True to show error box if invalid values are entered.
    /// </summary>
    [ BiffRecordPos( 2, 3, TFieldType.Bit ) ]
    private bool m_bShowErrorBox = true;

    #endregion

    /// <summary>
    /// Title of the prompt box (Unicode string, 16-bit string length).
    /// </summary>
    private string m_strPromtBoxTitle = string.Empty;
    private bool m_bPromptBoxShort;

    /// <summary>
    /// Title of the error box (Unicode string, 16-bit string length).
    /// </summary>
    private string m_strErrorBoxTitle = string.Empty;
    private bool m_bErrorBoxShort;

    /// <summary>
    /// Text of the prompt box (Unicode string, 16-bit string length).
    /// </summary>
    private string m_strPromtBoxText = string.Empty;
    private bool m_bPromptBoxTextShort;

    /// <summary>
    /// Text of the error box (Unicode string, 16-bit string length)
    /// </summary>
    private string m_strErrorBoxText = string.Empty;
    private bool m_bErrorBoxTextShort;

    /// <summary>
    /// Number of cell range addresses.
    /// </summary>
    private ushort m_usAddrListSize = 0;

    /// <summary>
    /// Cell range address list with all affected ranges.
    /// </summary>
    private List<TAddr> m_arrAddrList = new List<TAddr>();
    /// <summary>
    /// Tokens of the first formula.
    /// </summary>
    private Ptg[] m_arrFirstFormulaTokens = null;
    /// <summary>
    /// Tokens of the second formula.
    /// </summary>
    private Ptg[] m_arrSecondFormulaTokens = null;

    #endregion

    #region Class properties

    /// <summary>
    /// Option flags.
    /// </summary>
    public uint Options
    {
      get
      {
        return m_uiOptions;
      }
    }


    #region Options bit fields
    /// <summary>
    /// True if in list type validity, the string list is explicitly given in the formula.
    /// </summary>
    public bool IsStrListExplicit
    {
      get
      {
        return m_bStrListExplicit;
      }
      set
      {
        m_bStrListExplicit = value;
      }
    }

    /// <summary>
    /// True if empty cells are allowed.
    /// </summary>
    public bool IsEmptyCell
    {
      get
      {
        return m_bEmptyCell;
      }
      set
      {
        m_bEmptyCell = value;
      }
    }

    /// <summary>
    /// True to suppress the drop-down arrow in list type validity.
    /// </summary>
    public bool IsSuppressArrow
    {
      get
      {
        return m_bSuppressArrow;
      }
      set
      {
        m_bSuppressArrow = value;
      }
    }

    /// <summary>
    /// True to show prompt box if cell is selected.
    /// </summary>
    public bool IsShowPromptBox
    {
      get
      {
        return m_bShowPromptBox;
      }
      set
      {
        m_bShowPromptBox = value;
      }
    }

    /// <summary>
    /// True to show error box if invalid values are entered.
    /// </summary>
    public bool IsShowErrorBox
    {
      get
      {
        return m_bShowErrorBox;
      }
      set
      {
        m_bShowErrorBox = value;
      }
    }

    /// <summary>
    /// Data type:
    /// Changes bits of m_uiOptions private member.
    /// </summary>
    public ExcelDataType DataType
    {
      get
      {
        return ( ExcelDataType ) GetUInt32BitsByMask( m_uiOptions, DataTypeBitMask );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiOptions, DataTypeBitMask, ( uint ) value );
      }
    }

    /// <summary>
    /// Error style:
    /// Changes bits of m_uiOptions private member.
    /// </summary>
    public ExcelErrorStyle ErrorStyle
    {
      get
      {
        return ( ExcelErrorStyle ) ( GetUInt32BitsByMask( m_uiOptions, ErrorStyleBitMask )
          >> ErrorStyleStartBit );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiOptions, ErrorStyleBitMask,
           ( uint )value << ErrorStyleStartBit );
      }
    }
    /// <summary>
    /// Condition operator:
    /// Changes bits of m_uiOptions private member.
    /// </summary>
    public ExcelDataValidationComparisonOperator Condition
    {
      get
      {
        return ( ExcelDataValidationComparisonOperator )( GetUInt32BitsByMask( m_uiOptions,
          ConditionBitMask ) >> ConditionStartBit );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiOptions, ConditionBitMask,
          ( uint ) value << ConditionStartBit );
      }
    }
    #endregion

    /// <summary>
    /// Title of the prompt box (Unicode string, 16-bit string length).
    /// </summary>
    public string PromtBoxTitle
    {
      get
      {
        return m_strPromtBoxTitle;
      }
      set
      {
        m_strPromtBoxTitle = value;
      }
    }

    /// <summary>
    /// Title of the error box (Unicode string, 16-bit string length).
    /// </summary>
    public string ErrorBoxTitle
    {
      get
      {
        return m_strErrorBoxTitle;
      }
      set
      {
        m_strErrorBoxTitle = value;
      }
    }

    /// <summary>
    /// Text of the prompt box (Unicode string, 16-bit string length).
    /// </summary>
    public string PromtBoxText
    {
      get
      {
        return m_strPromtBoxText;
      }
      set
      {
        m_strPromtBoxText = value;
      }
    }

    /// <summary>
    /// Text of the error box (Unicode string, 16-bit string length).
    /// </summary>
    public string ErrorBoxText
    {
      get
      {
        return m_strErrorBoxText;
      }
      set
      {
        m_strErrorBoxText = value;
      }
    }

    /// <summary>
    /// Formula data for the first condition (RPN token array without size field).
    /// </summary>
    public Ptg[] FirstFormulaTokens
    {
      get
      {
        return m_arrFirstFormulaTokens;
      }
      set
      {
        m_arrFirstFormulaTokens = value;
      }
    }
    /// <summary>
    /// Formula data for the second condition (RPN token array without size field).
    /// </summary>
    public Ptg[] SecondFormulaTokens
    {
      get
      {
        return m_arrSecondFormulaTokens;
      }
      set
      {
        m_arrSecondFormulaTokens = value;
      }
    }

    /// <summary>
    /// Read-only. Number of cell range addresses.
    /// </summary>
    public ushort AddrListSize
    {
      get
      {
        return m_usAddrListSize;
      }
    }

    /// <summary>
    /// Cell range address list with all the affected ranges.
    /// </summary>
    public TAddr[] AddrList
    {
      get
      {
          return m_arrAddrList.ToArray();
      }
      set
      {
        m_arrAddrList.Clear();

        if( value != null )
          m_arrAddrList.AddRange( value );

        m_usAddrListSize = ( ushort )m_arrAddrList.Count;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 12;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  DVRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  DVRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DVRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// When last byte of read data is not the last byte of data after parsing.
    /// </exception>
    public override void ParseStructure()
    {
      m_uiOptions = GetUInt32( 0 );
      m_bStrListExplicit = GetBit( 0, 7 );
      m_bEmptyCell = GetBit( 1, 0 );
      m_bSuppressArrow = GetBit( 1, 1 );
      m_bShowPromptBox = GetBit( 2, 2 );
      m_bShowErrorBox = GetBit( 2, 3 );

      int offset = 4;
      PromtBoxTitle = CreateEmptyString( GetString16BitUpdateOffset( ref offset, out m_bPromptBoxShort ) );
      ErrorBoxTitle = CreateEmptyString( GetString16BitUpdateOffset( ref offset, out m_bErrorBoxShort ) );
      PromtBoxText = CreateEmptyString( GetString16BitUpdateOffset( ref offset, out m_bPromptBoxTextShort ) );
      ErrorBoxText = CreateEmptyString( GetString16BitUpdateOffset( ref offset, out m_bErrorBoxTextShort ) );

      ushort usFstCondFormulaSize = GetUInt16( offset );
      offset += 4;   //two bytes are not used
      byte[] arrFstCondFormula = GetBytes( offset, usFstCondFormulaSize );
      offset += usFstCondFormulaSize;

      ushort usSndCondFormulaSize = GetUInt16( offset );
      offset += 4;   //two bytes are not used
      byte[] arrSndCondFormula = GetBytes( offset, usSndCondFormulaSize );
      offset += usSndCondFormulaSize;

      // This method can only be called when parsing binary data from Excel 97 format(we don't
      // support binary format for Excel2007) that's why we can use ExcelVersion.Excel97to2003 here.
      ByteArrayDataProvider provider = new ByteArrayDataProvider( arrFstCondFormula );
      m_arrFirstFormulaTokens = FormulaUtil.ParseExpression( provider,
        usFstCondFormulaSize, ExcelVersion.Excel97to2003 );

      provider.SetBuffer( arrSndCondFormula );
      m_arrSecondFormulaTokens = FormulaUtil.ParseExpression( provider,
        usSndCondFormulaSize, ExcelVersion.Excel97to2003 );

      m_usAddrListSize = GetUInt16( offset );
      offset += 2;
      m_arrAddrList.Clear();

      for( int i = 0; i < m_usAddrListSize; i++, offset += 8 )
      {
        m_arrAddrList.Add( GetAddr( offset ) );
      }

      if( offset != m_iLength )
        throw new WrongBiffRecordDataException();
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      m_data = new byte[ m_iLength ];

      SetUInt32( 0, m_uiOptions );
      SetBit( 0, m_bStrListExplicit, 7 );
      SetBit( 1, m_bEmptyCell, 0 );
      SetBit( 1, m_bSuppressArrow, 1 );
      SetBit( 2, m_bShowPromptBox, 2 );
      SetBit( 2, m_bShowErrorBox, 3 );

      int iOffset = 4;

      SetString16BitUpdateOffset( ref iOffset, CreateNotEmptyString( m_strPromtBoxTitle ), m_bPromptBoxShort );
      SetString16BitUpdateOffset( ref iOffset, CreateNotEmptyString( m_strErrorBoxTitle ), m_bErrorBoxShort );
      SetString16BitUpdateOffset( ref iOffset, CreateNotEmptyString( m_strPromtBoxText ), m_bPromptBoxTextShort );
      SetString16BitUpdateOffset( ref iOffset, CreateNotEmptyString( m_strErrorBoxText ), m_bErrorBoxTextShort );

      byte[] arrFstCondFormula = FormulaUtil.PtgArrayToByteArray( m_arrFirstFormulaTokens, version );
      ushort usFstCondFormulaSize = ( ushort )( ( arrFstCondFormula != null )
        ? arrFstCondFormula.Length
        : 0 );

      SetUInt16( iOffset, usFstCondFormulaSize );
      iOffset += 2;
      SetUInt16( iOffset, 0 );
      iOffset += 2;

      if( usFstCondFormulaSize > 0 )
      {
        SetBytes( iOffset, arrFstCondFormula, 0, usFstCondFormulaSize );
        iOffset += usFstCondFormulaSize;
      }

      byte[] arrSndCondFormula = FormulaUtil.PtgArrayToByteArray( m_arrSecondFormulaTokens, version );
      ushort usSndCondFormulaSize = ( ushort )( ( arrSndCondFormula != null )
        ? arrSndCondFormula.Length
        : 0 );

      SetUInt16( iOffset, usSndCondFormulaSize );
      iOffset += 2;
      SetUInt16( iOffset, 0 );
      iOffset += 2;

      if( usSndCondFormulaSize > 0 )
      {
        SetBytes( iOffset, arrSndCondFormula, 0, usSndCondFormulaSize );
        iOffset += usSndCondFormulaSize;
      }

      SetUInt16( iOffset, m_usAddrListSize );
      iOffset += 2;

      for( int i = 0; i < m_usAddrListSize; i++, iOffset += 8 )
      {
        SetAddr( iOffset, m_arrAddrList[ i ] );
      }
    }

    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds new range to the list of validation ranges.
    /// </summary>
    /// <param name="addrToAdd">Range to add.</param>
    public void Add( TAddr addrToAdd )
    {
      m_arrAddrList.Add( addrToAdd );
      m_usAddrListSize++;
    }
    /// <summary>
    /// Adds new ranges to the list of validation ranges.
    /// </summary>
    /// <param name="addrToAdd">Array of ranges to add.</param>
    public void AddRange( TAddr[] addrToAdd )
    {
      m_arrAddrList.AddRange( addrToAdd );
      m_usAddrListSize = ( ushort )m_arrAddrList.Count;
//      TAddr[] buffer = new TAddr[ m_arrAddrList.Count + addrToAdd.Length ];
//
//      m_arrAddrList.CopyTo( buffer, 0 );
//      addrToAdd.CopyTo( buffer, m_arrAddrList.Count );
//
//      this.AddrList = buffer;
    }
    /// <summary>
    /// Adds new ranges to the list of validation ranges.
    /// </summary>
    /// <param name="addrToAdd">Collection of ranges to add.</param>
    public void AddRange( ICollection<TAddr> addrToAdd )
    {
      m_arrAddrList.AddRange( addrToAdd );
      m_usAddrListSize = ( ushort )m_arrAddrList.Count;
    }
    /// <summary>
    /// Evaluates size of the formula in bytes.
    /// </summary>
    /// <param name="arrTokens">Tokens to get size from.</param>
    /// <param name="version">Excel version that should be used to infill data.</param>
    /// <param name="addAdditionalDataSize">Indicates whether we should add size of the additional data.</param>
    /// <returns>Formula size in bytes.</returns>
    public static int GetFormulaSize( Ptg[] arrTokens, ExcelVersion version, bool addAdditionalDataSize )
    {
      if( arrTokens == null ) return 0;

      int iLength = arrTokens.Length;

      if( iLength == 0 ) return 0;

      int iResult = 0;

      for( int i = 0; i < iLength; i++ )
      {
        Ptg token = arrTokens[ i ];
        iResult += token.GetSize( version );

        if( addAdditionalDataSize )
        {
          IAdditionalData additional = token as IAdditionalData;

          if( additional != null )
          {
            iResult += additional.AdditionalDataSize;
          }
        }
      }

      return iResult;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_PART_SIZE
        + Get16BitStringSize( CreateNotEmptyString( m_strPromtBoxTitle ), m_bPromptBoxShort )
        + Get16BitStringSize( CreateNotEmptyString( m_strErrorBoxTitle ), m_bErrorBoxShort )
        + Get16BitStringSize( CreateNotEmptyString( m_strPromtBoxText ), m_bPromptBoxTextShort )
        + Get16BitStringSize( CreateNotEmptyString( m_strErrorBoxText ), m_bErrorBoxTextShort )
        + GetFormulaSize( m_arrFirstFormulaTokens, version, true )
        + GetFormulaSize( m_arrSecondFormulaTokens, version, true )
        + m_usAddrListSize * 8;
    }
    /// <summary>
    /// Returns StringEmpty if income string is empty or null.
    /// </summary>
    /// <param name="strToModify">String to modify.</param>
    /// <returns></returns>
    private string CreateNotEmptyString( string strToModify )
    {
      if( strToModify == null || strToModify.Length == 0 )
      {
        strToModify = StringEmpty;
      }

      return strToModify;
    }
    /// <summary>
    /// Returns empty string is income string is StringEmpty.
    /// </summary>
    /// <param name="strToModify">String to modify.</param>
    /// <returns></returns>
    private string CreateEmptyString( string strToModify )
    {
      if( strToModify == StringEmpty )
      {
        strToModify = string.Empty;
      }

      return strToModify;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Clone current record.
    /// </summary>
    /// <returns>Returns clone of the current object.</returns>
    new public object Clone()
    {
      DVRecord result = ( DVRecord )base.Clone();
      
      result.m_arrFirstFormulaTokens = CloneUtils.ClonePtgArray( m_arrFirstFormulaTokens );
      result.m_arrSecondFormulaTokens = CloneUtils.ClonePtgArray( m_arrSecondFormulaTokens );

      int iLen = m_arrAddrList.Count;

      result.m_arrAddrList = new List<TAddr>( iLen );

      for( int i = 0; i < iLen; i++ )
      {
        result.m_arrAddrList.Add( m_arrAddrList[ i ] );
      }

      return result;
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      DVRecord dv = obj as DVRecord;

      if( dv == null ) return false;

      bool iResult = ( dv.IsStrListExplicit == IsStrListExplicit
        && dv.IsEmptyCell == IsEmptyCell
        && dv.IsSuppressArrow == IsSuppressArrow
        && dv.IsShowPromptBox == IsShowPromptBox
        && dv.IsShowErrorBox == IsShowErrorBox
        && dv.DataType == DataType
        && dv.ErrorStyle == ErrorStyle
        && dv.Condition == Condition
        && dv.PromtBoxTitle == PromtBoxTitle
        && dv.ErrorBoxTitle == ErrorBoxTitle
        && dv.PromtBoxText == PromtBoxText
        && dv.ErrorBoxText == ErrorBoxText
//        && dv.FstCondFormulaSize == FstCondFormulaSize
//        && dv.SndCondFormulaSize == SndCondFormulaSize
        //&& dv.AddrListSize == AddrListSize
        && Ptg.CompareArrays( dv.FirstFormulaTokens, FirstFormulaTokens )
        && Ptg.CompareArrays( dv.SecondFormulaTokens, SecondFormulaTokens )
        //        && CompareArrays( dv.m_arrAddrList, m_arrAddrList )
        );

      return iResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      int iFirstFormulaSize = ( m_arrFirstFormulaTokens == null )
        ? 0
        : m_arrFirstFormulaTokens.Length;

      int iSecondFormulaSize = ( m_arrSecondFormulaTokens == null )
        ? 0
        : m_arrSecondFormulaTokens.Length;

      int listValueHash = 0;

      for (int i = 0; i < (AddrList.Length); i++)
      {
          listValueHash += AddrList.GetValue(i).GetHashCode();
      }


      int iHashCode = IsStrListExplicit.GetHashCode()
        ^ IsEmptyCell.GetHashCode()
        ^ IsSuppressArrow.GetHashCode()
        ^ IsShowPromptBox.GetHashCode()
        ^ IsShowErrorBox.GetHashCode()
        ^ DataType.GetHashCode()
        ^ ErrorStyle.GetHashCode()
        ^ Condition.GetHashCode()
        ^ PromtBoxTitle.GetHashCode()
        ^ ErrorBoxTitle.GetHashCode()
        ^ PromtBoxText.GetHashCode()
        ^ ErrorBoxText.GetHashCode()
        ^ iFirstFormulaSize.GetHashCode()
        ^ iSecondFormulaSize.GetHashCode()
        ^ AddrListSize.GetHashCode()
        ^ listValueHash.GetHashCode();

      return iHashCode;
    }

    #endregion
  }
}
