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
using System.IO;
using System.Text;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The begin record defines the start of a block of records for a (Graphing)
  /// data object. This record is matched with a corresponding EndRecord.
  /// </summary>
  [ Biff( TBIFFRecord.Name ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class NameRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask of the Function group.
    /// </summary>
    public const ushort FunctionGroupBitMask = 0x0FC0;
    /// <summary>
    /// Predefined names:
    /// </summary>
    public static readonly string[] PREDEFINED_NAMES = new string[]
    {
      "Consolidate_Area",      // 0
      "Auto_Open",             // 1
      "Auto_Close",            // 2
      "Extract",               // 3
      "Database",              // 4
      "Criteria",              // 5
      "Print_Area",            // 6
      "Print_Titles",          // 7
      "Recorder",              // 8
      "Data_Form",             // 9
      "Auto_Activate",         // a
      "Auto_Deactivate",       // b
      "Sheet_Title",           // c
      "_FilterDatabase",       // d - AutoFilter
      "_xlnm.Print_Titles",
      "_xlnm.Print_Area",
    };
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 14;

    private const string XLNM_ExtensionName = "_xlnm.";
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;

    #region Options bit fields

    /// <summary>
    /// True if name is hidden.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bNameHidden;

    /// <summary>
    /// True if name is a function.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bNameFunction;

    /// <summary>
    /// True if name is a command.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bNameCommand;

    /// <summary>
    /// True if function macro or command macro.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bFCMacro;

    /// <summary>
    /// True if complex function (array formula or user defined).
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bComplexFunction;

    /// <summary>
    /// True if built-in name.
    /// </summary>
    [ BiffRecordPos( 0, 5, TFieldType.Bit ) ]
    private bool m_bBuinldInName;

    /// <summary>
    /// True if name contains binary data.
    /// </summary>
    [ BiffRecordPos( 1, 4, TFieldType.Bit ) ]
    private bool m_bBinaryData;
    #endregion

    /// <summary>
    /// Keyboard shortcut.
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte   m_bKeyboardShortcut;
    /// <summary>
    /// Length of the name.
    /// </summary>
    [ BiffRecordPos( 3, 1 ) ]
    private byte   m_bNameLength;
    /// <summary>
    /// Size of the formula data.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usFormulaDataSize;
    /// <summary>
    /// Reserved.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usReserved = 0;
    /// <summary>
    /// 0 if global name; otherwise index to sheet (one-based).
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usIndexOrGlobal;
    /// <summary>
    /// Length of menu text.
    /// </summary>
    [ BiffRecordPos( 10, 1 ) ]
    private byte m_bMenuTextLength;
    /// <summary>
    /// Length of description text.
    /// </summary>
    [ BiffRecordPos( 11, 1 ) ]
    private byte m_bDescriptionLength;
    /// <summary>
    /// Length of help topic text.
    /// </summary>
    [ BiffRecordPos( 12, 1 ) ]
    private byte m_bHelpTextLength;
    /// <summary>
    /// Length of status bar text.
    /// </summary>
    [ BiffRecordPos( 13, 1 ) ]
    private byte m_bStatusTextLength;
    /// <summary>
    /// Name (Unicode string without length field).
    /// </summary>
    private string m_strName = string.Empty;
    /// <summary>
    /// Formula data (RPN token array without size field).
    /// </summary>
    private byte[] m_arrFormulaData = null;
    /// <summary>
    /// Optional. Menu text (Unicode string without length field).
    /// </summary>
    private string m_strMenuText = string.Empty;
    /// <summary>
    /// Optional. Description text (Unicode string without length field).
    /// </summary>
    private string m_strDescription = string.Empty;
    /// <summary>
    /// Optional. Help topic text (Unicode string without length field).
    /// </summary>
    private string m_strHelpText = string.Empty;
    /// <summary>
    /// Optional. Status bar text (Unicode string without length field).
    /// </summary>
    private string m_strStatusText = string.Empty;
    /// <summary>
    /// Parsed formula expression.
    /// </summary>
    private Ptg[] m_arrToken = null;
    #endregion

    #region Class properties
    /// <summary>
    /// True if name is hidden.
    /// </summary>
    public bool     IsNameHidden
    {
      get
      {
        return m_bNameHidden;
      }
      set
      {
        m_bNameHidden = value;
      }
    }

    /// <summary>
    /// True if name is a function.
    /// </summary>
    public bool     IsNameFunction
    {
      get
      {
        return m_bNameFunction;
      }
      set
      {
        m_bNameFunction = value;
      }
    }

    /// <summary>
    /// True if name is a command.
    /// </summary>
    public bool     IsNameCommand
    {
      get
      {
        return m_bNameCommand;
      }
      set
      {
        m_bNameCommand = value;
      }
    }

    /// <summary>
    /// True if function macro or command macro.
    /// </summary>
    public bool     IsFunctionOrCommandMacro
    {
      get
      {
        return m_bFCMacro;
      }
      set
      {
        m_bFCMacro = value;
      }
    }

    /// <summary>
    /// True if complex function (array formula or user defined).
    /// </summary>
    public bool     IsComplexFunction
    {
      get
      {
        return m_bComplexFunction;
      }
      set
      {
        m_bComplexFunction = value;
      }
    }

    /// <summary>
    /// True if built-in name.
    /// </summary>
    public bool     IsBuinldInName
    {
      get
      {
        return m_bBuinldInName;
      }
      set
      {
        m_bBuinldInName = value;
      }
    }

    /// <summary>
    /// True if name contains binary data.
    /// </summary>
    public bool     HasBinaryData
    {
      get
      {
        return m_bBinaryData;
      }
      set
      {
        m_bBinaryData = value;
      }
    }

    /// <summary>
    /// Index to function group.
    /// Changes bits of m_usOptions member.
    /// </summary>
    public ushort   FunctionGroupIndex
    {
      get
      {
        return (ushort)( GetUInt16BitsByMask( m_usOptions, FunctionGroupBitMask ) >> 6 );
      }
      set
      {
        if( value > 0x3F )
          throw new ArgumentOutOfRangeException( "FunctionGroupIndex too large." );

        SetUInt16BitsByMask( ref m_usOptions, FunctionGroupBitMask, (ushort) (value << 6 ) );
      }
    }
    /// <summary>
    /// Keyboard shortcut.
    /// </summary>
    public byte     KeyboardShortcut
    {
      get
      {
        return m_bKeyboardShortcut;
      }
      set
      {
        m_bKeyboardShortcut = value;
      }
    }

    /// <summary>
    /// Read-only. Length of the name.
    /// </summary>
    public byte     NameLength
    {
      get
      {
        return m_bNameLength;
      }
    }

    /// <summary>
    /// Read-only. Size of the formula data.
    /// </summary>
    public ushort   FormulaDataSize
    {
      get
      {
        return m_usFormulaDataSize;
      }
    }

    /// <summary>
    /// 0 if global name; otherwise index to sheet (one-based).
    /// </summary>
    public ushort   IndexOrGlobal
    {
      get
      {
        return m_usIndexOrGlobal;
      }
      set
      {
        m_usIndexOrGlobal = value;
      }
    }

    /// <summary>
    /// Read-only. Length of menu text.
    /// </summary>
    public byte     MenuTextLength
    {
      get
      {
        return m_bKeyboardShortcut;
      }
    }

    /// <summary>
    /// Read-only. Length of description text.
    /// </summary>
    public byte     DescriptionLength
    {
      get
      {
        return m_bDescriptionLength;
      }
    }

    /// <summary>
    /// Read-only. Length of help topic text.
    /// </summary>
    public byte     HelpTextLength
    {
      get
      {
        return m_bHelpTextLength;
      }
    }

    /// <summary>
    /// Read-only. Length of status bar text.
    /// </summary>
    public byte     StatusTextLength
    {
      get
      {
        return m_bStatusTextLength;
      }
    }

    /// <summary>
    /// Name (Unicode string without length field).
    /// </summary>
    public string   Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
        m_bNameLength = ( IsPredefinedName( m_strName ) ) ? ( byte )1 : 
          ( ( m_strName != null ) ? ( byte )m_strName.Length : ( byte )0 );
      }
    }

    /// <summary>
    /// Formula data (RPN token array without size field).
    /// </summary>
    public Ptg[]    FormulaTokens
    {
      get
      {
        return m_arrToken;
      }
      set
      {
        m_arrToken = value;

        if( value != null )
        {
          int Len;
          m_arrFormulaData = FormulaUtil.PtgArrayToByteArray( value, out Len, ExcelVersion.Excel2007 );
          m_usFormulaDataSize = ( ushort ) Len;
        }
        else
        {
          m_usFormulaDataSize = ( ushort ) 0;
          m_arrFormulaData = null;
        }
      }
    }

    /// <summary>
    /// Optional. Menu text (Unicode string without length field).
    /// </summary>
    public string   MenuText
    {
      get
      {
        return m_strMenuText;
      }
      set
      {
        m_strMenuText = value;
        m_bMenuTextLength = ( m_strMenuText != null ) ?
          (byte) m_strMenuText.Length : (byte)0;
      }
    }

    /// <summary>
    /// Optional. Description text (Unicode string without length field).
    /// </summary>
    public string   Description
    {
      get
      {
        return m_strDescription;
      }
      set
      {
        m_strDescription = value;
        m_bDescriptionLength = ( m_strDescription != null ) ?
          (byte) m_strDescription.Length : (byte)0;
      }
    }

    /// <summary>
    /// Optional. Help topic text (Unicode string without length field).
    /// </summary>
    public string   HelpText
    {
      get
      {
        return m_strHelpText;
      }
      set
      {
        m_strHelpText = value;
        m_bHelpTextLength = ( m_strHelpText != null ) ?
          (byte) m_strHelpText.Length : (byte)0;
      }
    }

    /// <summary>
    /// Optional. Status bar text (Unicode string without length field).
    /// </summary>
    public string   StatusText
    {
      get
      {
        return m_strStatusText;
      }
      set
      {
        m_strStatusText = value;
        m_bStatusTextLength = ( m_strStatusText != null ) ?
          (byte) m_strStatusText.Length : (byte)0;
      }
    }
    /// <summary>
    /// Read-only. Reserved (not used).
    /// </summary>
    public ushort   Reserved
    {
      get
      {
        return m_usReserved;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_FIXED_PART_SIZE;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  NameRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  NameRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  NameRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <exception cref="WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      ParseFixedPart( provider, iOffset );

      int offset = iOffset + DEF_FIXED_PART_SIZE;

      if( m_bNameLength != 0 )
      {
        m_strName = provider.ReadStringUpdateOffset( ref offset, m_bNameLength );
      }
      else
      {
        offset++;
      }
      
      // extract default name of Name record
      if( IsBuinldInName && m_strName.Length == 1 )
      {
        m_strName = PREDEFINED_NAMES[ ( int )m_strName[ 0 ] ];
      }

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_strName, "NameRecord" );

      try
      {
        FormulaTokens = FormulaUtil.ParseExpression( provider, offset, 
          m_usFormulaDataSize, out offset, version );
      }
      catch( Exception ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Can't parse formula", "Exception" );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
        throw;
      }

      m_strMenuText     = provider.ReadStringUpdateOffset( ref offset, m_bMenuTextLength );
      m_strDescription  = provider.ReadStringUpdateOffset( ref offset, m_bDescriptionLength );
      m_strHelpText     = provider.ReadStringUpdateOffset( ref offset, m_bHelpTextLength );
      m_strStatusText   = provider.ReadStringUpdateOffset( ref offset, m_bStatusTextLength );

      if( offset != m_iLength )
      {
        throw new WrongBiffRecordDataException( "NameRecord" );
      }
    }

    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      int iLength;

      if (m_strName.Equals(PREDEFINED_NAMES[14]))
      {
          m_strName=m_strName.Substring(XLNM_ExtensionName.Length);
      }
      bool bUnicode = !BiffRecordRawWithArray.IsAsciiString( m_strName );

      if( m_arrToken != null && m_arrToken.Length > 0 )
      {
        m_arrFormulaData = FormulaUtil.PtgArrayToByteArray( m_arrToken, out iLength, version );
        m_usFormulaDataSize = ( ushort )iLength;
      }
      else
      {
        m_arrFormulaData = null;
        m_usFormulaDataSize = 0;
      }

      m_iLength = GetStoreSize( version );

      InfillFixedPart( provider, iOffset );
      iOffset += DEF_FIXED_PART_SIZE;

      if( IsBuinldInName )
      {
        provider.WriteByte( iOffset, 0 );
      
        int index = PredefinedIndex( m_strName );
        
        if( index != -1 )
        {
          byte btValue = ( index < 0 ) ? ( byte )m_strName[ 0 ] : ( byte )index;
          provider.WriteByte( iOffset + 1, btValue );
          iOffset += 2;
        }
        else
        {
          provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strName, bUnicode );
        }
      }
      else
      {
        provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strName, bUnicode );
      }

      if( m_bNameLength == 0 )
      {
        provider.WriteByte( iOffset, 0 );
        iOffset++;
      }

      if( m_arrFormulaData != null )
      {
        provider.WriteBytes( iOffset, m_arrFormulaData, 0, m_arrFormulaData.Length );
        iOffset += m_arrFormulaData.Length;
      }

      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strMenuText );
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strDescription );
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strHelpText );
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strStatusText );

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_strName, "Serializing name" );
    }

    /// <summary>
    /// Infill fixed part of the record.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Starting offset.</param>
    private void InfillFixedPart( DataProvider provider, int iOffset)
    {
      provider.WriteUInt16( iOffset, m_usOptions );

      provider.WriteBit( iOffset, m_bNameHidden, 0 );
      provider.WriteBit( iOffset, m_bNameFunction, 1 );
      provider.WriteBit( iOffset, m_bNameCommand, 2 );
      provider.WriteBit( iOffset, m_bFCMacro, 3 );
      provider.WriteBit( iOffset, m_bComplexFunction, 4 );
      provider.WriteBit( iOffset, m_bBuinldInName, 5 );
      iOffset++;

      provider.WriteBit( iOffset, m_bBinaryData, 4 );
      iOffset++;

      provider.WriteByte( iOffset, m_bKeyboardShortcut );
      iOffset++;

      provider.WriteByte( iOffset, m_bNameLength );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usFormulaDataSize );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usReserved );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usIndexOrGlobal );
      iOffset += 2;

      provider.WriteByte( iOffset, m_bMenuTextLength );
      iOffset++;

      provider.WriteByte( iOffset, m_bDescriptionLength );
      iOffset++;

      provider.WriteByte( iOffset, m_bHelpTextLength );
      iOffset++;

      provider.WriteByte( iOffset, m_bStatusTextLength );
      iOffset++;
    }
    /// <summary>
    /// Infill fixed part of the record.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the fixed part data.</param>
    private void ParseFixedPart( DataProvider provider, int iOffset )
    {
      m_usOptions = provider.ReadUInt16( iOffset );

      m_bNameHidden = provider.ReadBit( iOffset, 0 );
      m_bNameFunction = provider.ReadBit( iOffset, 1 );
      m_bNameCommand = provider.ReadBit( iOffset, 2 );
      m_bFCMacro = provider.ReadBit( iOffset, 3 );
      m_bComplexFunction = provider.ReadBit( iOffset, 4 );
      m_bBuinldInName = provider.ReadBit( iOffset, 5 );
      m_bBinaryData = provider.ReadBit( iOffset + 1, 4 );

      m_bKeyboardShortcut = provider.ReadByte( iOffset + 2 );
      m_bNameLength = provider.ReadByte( iOffset + 3 );
      m_usFormulaDataSize = provider.ReadUInt16( iOffset + 4 );
      m_usReserved = provider.ReadUInt16( iOffset + 6 );
      m_usIndexOrGlobal = provider.ReadUInt16( iOffset + 8 );
      m_bMenuTextLength = provider.ReadByte( iOffset + 10 );
      m_bDescriptionLength = provider.ReadByte( iOffset + 11 );
      m_bHelpTextLength = provider.ReadByte( iOffset + 12 );
      m_bStatusTextLength = provider.ReadByte( iOffset + 13 );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iSize = DEF_FIXED_PART_SIZE;
      bool bAscii = BiffRecordRawWithArray.IsAsciiString( m_strName );
      Encoding unicode = !bAscii ?
        Encoding.Unicode :
        Encoding.UTF8;

      if( IsBuinldInName )
      {
        int index = Array.IndexOf( PREDEFINED_NAMES, m_strName );
        
        if( index != -1 )
        {
          iSize += 2;
        }
        else
        {
          iSize += unicode.GetByteCount( m_strName ) + 1;
        }
      }
      else
      {
        iSize += unicode.GetByteCount( m_strName ) + 1;
      }

      iSize += DVRecord.GetFormulaSize( m_arrToken, version, true );//m_arrFormulaData.Length;

      iSize += GetByteCount( m_strMenuText );
      iSize += GetByteCount( m_strDescription );
      iSize += GetByteCount( m_strHelpText );
      iSize += GetByteCount( m_strStatusText );

      return iSize;
    }

    /// <summary>
    /// Determines number of bytes needed to store string without length field.
    /// </summary>
    /// <param name="strValue">Value to measure.</param>
    /// <returns>Number of bytes needed to store string without length field.</returns>
    private int GetByteCount( string strValue )
    {
      return ( strValue != null && strValue.Length > 0 )
        ? Encoding.Unicode.GetByteCount( strValue ) + 1
        : 0;
    }
    /// <summary>
    /// Method checks name of NameRecord and determines whether it is a default or not.
    /// </summary>
    /// <param name="value">Name to check.</param>
    /// <returns>True if name is Predefined name; otherwise False.</returns>
    public static bool IsPredefinedName( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      return ( PredefinedIndex( value ) >= 0 );
    }
    /// <summary>
    /// Looks for name in the predefined names table.
    /// </summary>
    /// <param name="value">Value to look for.</param>
    /// <returns>Index in the predefined names array.</returns>
    public static int PredefinedIndex( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      int iFoundIndex = -1;

      if( value.Length > 0 )
      {
        for( int i = 0, iCount = PREDEFINED_NAMES.Length; i < iCount; i++ )
        {
          string strName = PREDEFINED_NAMES[ i ];

          if( value.StartsWith( strName ) )
          {
            iFoundIndex = i;
            break;
          }
        }
      }

      return iFoundIndex;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
      NameRecord result = ( NameRecord )base.Clone();
      result.FormulaTokens = CloneUtils.ClonePtgArray( m_arrToken );

      return result;
    }
    public override void ClearData()
    {
        m_arrFormulaData = null;
        m_arrToken = null;        
    }
    #endregion
  }
}
