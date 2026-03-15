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
using System.IO;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// 
  /// </summary>
  [ Biff( TBIFFRecord.ExternName ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ExternNameRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Possible options flags.
    /// </summary>
    [ Flags ]
    private enum OptionFlags
    {
      BuiltIn = 1,
      WantAdvise = 2,
      WantPicture = 4,
      Ole = 8,
      OleLink = 16,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private OptionFlags m_options = 0;
    /*
    /// <summary>
    /// Indicates whether the name is a built-in name.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bBuiltin;
    /// <summary>
    /// False for manual OLE / DDE links, True - automatic.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bWantAdvise;
    /// <summary>
    /// Indicates whether Microsoft Excel wants a cfPict clipboard format
    /// representation of the data. OBJ and IMDATA records store the image.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bWantPic;
    /// <summary>
    /// Indicates whether this record stores the OLE StdDocumentName identifier.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bOle;
    /// <summary>
    /// Indicates whether name is OLE link.
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bOleLink;
*/
    /// <summary>
    /// One-based index to sheet in preceding SUPBOOK record, 0
    /// for global defined names (for external names), or
    /// Not used (for add-in functions).
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usSheetId = 0;

    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usWord2 = 0;

    /// <summary>
    /// External name (Unicode string, 8-bit string length) or
    /// Add-in function name (Unicode string, 8-bit string length).
    /// </summary>
    [ BiffRecordPos( 6, TFieldType.String ) ]
    private string m_strName = string.Empty;

    /// <summary>
    /// Size of the formula data.
    /// </summary>
    private ushort m_usFormulaSize;
    /// <summary>
    /// Formula data or
    /// 02H 00H 1CH 17H (formula representing the #REF! error code).
    /// </summary>
    private byte[] m_arrFormulaData = null;
    private bool m_isAddIn;
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Option flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return ( ushort )m_options;
      }
      set
      {
        m_options = ( OptionFlags )value;
      }
    }
    /// <summary>
    /// Read-only. One-based index to sheet in preceding SUPBOOK
    /// record, 0 for global defined names (for external names), or
    /// Not used (for add-in functions).
    /// </summary>
    public ushort SheetId
    {
      get
      {
        return m_usSheetId;
      }
    }
    /// <summary>
    /// Read-only. Not used.
    /// </summary>
    public ushort Word2
    {
      get
      {
        return m_usWord2;
      }
    }
    /// <summary>
    /// Read-only. Formula data or
    /// 02H 00H 1CH 17H (formula representing the #REF! error code).
    /// </summary>
    public byte[] FormulaData
    {
      get
      {
        return m_arrFormulaData;
      }
    }
    /// <summary>
    /// External name (Unicode string, 8-bit string length) or
    /// Add-in function name (Unicode string, 8-bit string length).
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ushort FormulaSize
    {
      get
      {
        return m_usFormulaSize;
      }
      set
      {
        m_usFormulaSize = value;
      }
    }
    /// <summary>
    /// Indicates whether data array is needed to store record data. Read-only.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return ( m_options != 0 && m_options != OptionFlags.BuiltIn );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool BuiltIn
    {
      get
      {
        return ( m_options & OptionFlags.BuiltIn ) != 0;
      }
      set
      {
        SetFlag( OptionFlags.BuiltIn, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool WantAdvise
    {
      get
      {
        return ( m_options & OptionFlags.WantAdvise ) != 0;
      }
      set
      {
        SetFlag( OptionFlags.WantAdvise, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool WantPicture
    {
      get
      {
        return ( m_options & OptionFlags.WantPicture ) != 0;
      }
      set
      {
        SetFlag( OptionFlags.WantPicture, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool Ole
    {
      get
      {
        return ( m_options & OptionFlags.Ole ) != 0;
      }
      set
      {
        SetFlag( OptionFlags.Ole, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool OleLink
    {
      get
      {
        return ( m_options & OptionFlags.OleLink ) != 0;
      }
      set
      {
        SetFlag( OptionFlags.OleLink, value );
      }
    }
    public bool IsAddIn
    {
        get
        {
            return m_isAddIn;
        }
        set
        {
            m_isAddIn = value;
        }
    }
    private void SetFlag( OptionFlags flag, bool value )
    {
      if( value )
      {
        m_options |= flag;
      }
      else
      {
        m_options &= ~flag;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  ExternNameRecord()
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
    public  ExternNameRecord( Stream stream, out int itemSize )
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
    public  ExternNameRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      int iOffset = 0;

      m_options = ( OptionFlags )GetUInt16( iOffset );
      iOffset += 2;

      m_usSheetId = GetUInt16( iOffset );
      iOffset += 2;

      m_usWord2 = GetUInt16( iOffset );
      iOffset += 2;

      int iBytes;
      m_strName = GetStringByteLen( iOffset, out iBytes );
      iOffset += iBytes + 2;

      if( BuiltIn )
      {
        m_usFormulaSize = GetUInt16( iOffset );
        iOffset += 2;
      }
      // Indicates that parses dde link.
      else if( !OleLink )
      {
        m_usFormulaSize = ( ushort )( m_iLength - iOffset );
      }

      m_arrFormulaData = new byte[ m_usFormulaSize ];
      Buffer.BlockCopy( m_data, iOffset, m_arrFormulaData, 0, m_usFormulaSize );
      iOffset += m_usFormulaSize;
    }

    /// <summary>
     /// In this method, a class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      if( !OleLink && !BuiltIn )
      {
        InfillDDELink();
      }
      else if( m_options == 0 || m_options == OptionFlags.BuiltIn && m_usFormulaSize == 0 )
      {
        bool bAutoGrow = AutoGrowData;
        AutoGrowData = true;
        
        SetUInt16( 0, ( ushort )m_options );
        SetUInt16( 2, m_usSheetId );
        SetUInt16( 4, m_usWord2 );
        m_iLength = 6;
        SetString16BitUpdateOffset( ref m_iLength, m_strName );

        if( m_options == 0 || m_options == OptionFlags.BuiltIn )
        {
          SetUInt16( m_iLength, m_usFormulaSize );
          m_iLength += 2;

          if( m_usFormulaSize > 0 )
          {
            SetBytes( m_iLength, m_arrFormulaData, 0, m_usFormulaSize );
            m_iLength += m_usFormulaSize;
          }
        }

        AutoGrowData = bAutoGrow;
      }
    }
    /// <summary>
    /// Infills internal data in the case of DDE link extern name.
    /// </summary>
    private void InfillDDELink()
    {
      bool bAutoGrow = AutoGrowData;
      AutoGrowData = true;
      m_iLength = 0;

      SetUInt16( m_iLength, ( ushort )m_options );
      m_iLength += 2;

      SetInt32( m_iLength, 0 );
      m_iLength += 4;

      m_iLength += SetStringByteLen( m_iLength, m_strName );

      if (m_arrFormulaData != null && m_arrFormulaData.Length > 0)
      {
          SetBytes(m_iLength, m_arrFormulaData);
          m_iLength += m_arrFormulaData.Length;
      }
      else if(m_isAddIn)
      {
          m_arrFormulaData = new byte[] { 0x2, 0x0, 0x1C, 0x17 };
          SetBytes(m_iLength, m_arrFormulaData);
          m_iLength += m_arrFormulaData.Length;
      }

      AutoGrowData = bAutoGrow;
    }

    #endregion
  }
}
