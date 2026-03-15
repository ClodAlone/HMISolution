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

using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the last settings from the "Sort" dialog
  /// for each sheet. These settings are not attached to a cell range
  /// in the sheet, so it is not possible to determine
  /// the cell range sorted with the settings of this record.
  /// </summary>
  [ Biff( TBIFFRecord.Sort ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class SortRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for the table index.
    /// </summary>
    private const ushort TableIndexBitMask = 0x03E0;
    /// <summary>
    /// Start bit for the table index.
    /// </summary>
    private const int    TableIndexStartBit = 5;
    /// <summary>
    /// Size of the fixed part size.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 5;
    #endregion

    #region Class members

    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort  m_usOptions = 0;

    #region Options bit fields

    /// <summary>
    /// False to sort rows (top to bottom);
    /// True to sort columns (left to right).
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bSortColumns = false;

    /// <summary>
    /// False to sort first key in ascending order;
    /// True to sort first key in descending order.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bFirstDesc = false;

    /// <summary>
    /// False to sort second key in ascending order;
    /// True to sort second key in descending order.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bSecondDesc = false;

    /// <summary>
    /// False to sort third key in ascending order;
    /// True to sort third key in descending order.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bThirdDesc = false;

    /// <summary>
    /// False to sort case-insensitive;
    /// True to sort case-sensitive.
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bCaseSensitive = false;
    #endregion

    /// <summary>
    /// Length of first sort key.
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte    m_FirstKeyLen = 0;

    /// <summary>
    /// Length of second sort key.
    /// </summary>
    [ BiffRecordPos( 3, 1 ) ]
    private byte    m_SecondKeyLen = 0;

    /// <summary>
    /// Length of third sort key.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte    m_ThirdKeyLen = 0;

    /// <summary>
    /// First sort key.
    /// </summary>
    private string  m_strFirstKey = string.Empty;

    /// <summary>
    /// Second sort key.
    /// </summary>
    private string  m_strSecondKey = string.Empty;

    /// <summary>
    /// Third sort key.
    /// </summary>
    private string  m_strThirdKey = string.Empty;

    #endregion

    #region Class properties

    #region Option bit fields

    /// <summary>
    /// False to sort rows (top to bottom);
    /// True to sort columns (left to right).
    /// </summary>
    public bool IsSortColumns
    {
      get
      {
        return m_bSortColumns;
      }
      set
      {
        m_bSortColumns = value;
      }
    }

    /// <summary>
    /// False to sort first key in ascending order;
    /// True to sort first key in descending order.
    /// </summary>
    public bool IsFirstDesc
    {
      get
      {
        return m_bFirstDesc;
      }
      set
      {
        m_bFirstDesc = value;
      }
    }

    /// <summary>
    /// False to sort second key in ascending order;
    /// True to sort second key in descending order.
    /// </summary>
    public bool IsSecondDesc
    {
      get
      {
        return m_bSecondDesc;
      }
      set
      {
        m_bSecondDesc = value;
      }
    }

    /// <summary>
    /// False to sort third key in ascending order;
    /// True to sort third key in descending order.
    /// </summary>
    public bool IsThirdDesc
    {
      get
      {
        return m_bThirdDesc;
      }
      set
      {
        m_bThirdDesc = value;
      }
    }

    /// <summary>
    /// False to sort case-insensitive;
    /// True to sort case-sensitive.
    /// </summary>
    public bool IsCaseSensitive
    {
      get
      {
        return m_bCaseSensitive;
      }
      set
      {
        m_bCaseSensitive = value;
      }
    }

    /// <summary>
    /// One-based index into the table of defined sort lists,
    /// or 0 for sorting without a list.
    /// This property changes some bits of m_usOptions.
    /// </summary>
    public ushort TableIndex
    {
      get
      {
        return ( ushort ) ( GetUInt16BitsByMask( m_usOptions, TableIndexBitMask )
          >> TableIndexStartBit );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, TableIndexBitMask,
          ( ushort ) ( value << TableIndexStartBit ) );
      }
    }
    #endregion
    /// <summary>
    /// Read-only. Length of first sort key.
    /// </summary>
    public byte   FirstKeyLen
    {
      get
      {
        return m_FirstKeyLen;
      }
    }

    /// <summary>
    /// Read-only. Length of second sort key.
    /// </summary>
    public byte   SecondKeyLen
    {
      get
      {
        return m_SecondKeyLen;
      }
    }

    /// <summary>
    /// Read-only. Length of third sort key.
    /// </summary>
    public byte   ThirdKeyLen
    {
      get
      {
        return m_ThirdKeyLen;
      }
    }

    /// <summary>
    /// First sort key.
    /// </summary>
    public string FirstKey
    {
      get
      {
        return m_strFirstKey;
      }
      set
      {
        m_strFirstKey = value;
        m_FirstKeyLen = ( value != null ) ? ( byte ) value.Length : ( byte ) 0;
      }
    }

    /// <summary>
    /// Second sort key.
    /// </summary>
    public string SecondKey
    {
      get
      {
        return m_strSecondKey;
      }
      set
      {
        m_strSecondKey = value;
        m_SecondKeyLen = ( value != null ) ? ( byte ) value.Length : ( byte ) 0;
      }
    }

    /// <summary>
    /// Third sort key.
    /// </summary>
    public string ThirdKey
    {
      get
      {
        return m_strThirdKey;
      }
      set
      {
        m_strThirdKey = value;
        m_ThirdKeyLen = ( value != null ) ? ( byte ) value.Length : ( byte ) 0;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 5;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  SortRecord()
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
    public  SortRecord( Stream stream, out int itemSize )
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
    public  SortRecord( int iReserve )
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
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bSortColumns = provider.ReadBit( iOffset + 0, 0 );
      m_bFirstDesc = provider.ReadBit( iOffset + 0, 1 );
      m_bSecondDesc = provider.ReadBit( iOffset + 0, 2 );
      m_bThirdDesc = provider.ReadBit( iOffset + 0, 3 );
      m_bCaseSensitive = provider.ReadBit( iOffset + 0, 4 );
      m_FirstKeyLen = provider.ReadByte( iOffset + 2 );
      m_SecondKeyLen = provider.ReadByte( iOffset + 3 );
      m_ThirdKeyLen = provider.ReadByte( iOffset + 4 );

      int iStartOffset = iOffset;
      iOffset += 5;

      m_strFirstKey = provider.ReadStringUpdateOffset( ref iOffset, m_FirstKeyLen );
      m_strSecondKey = provider.ReadStringUpdateOffset( ref iOffset, m_SecondKeyLen );
      m_strThirdKey = provider.ReadStringUpdateOffset( ref iOffset, m_ThirdKeyLen );

      //if( offset + 1 != m_iLength )
      //{
      //  throw new WrongBiffRecordDataException();
      //}
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      int iStartOffset = iOffset;
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bSortColumns, 0 );
      provider.WriteBit( iOffset + 0, m_bFirstDesc, 1 );
      provider.WriteBit( iOffset + 0, m_bSecondDesc, 2 );
      provider.WriteBit( iOffset + 0, m_bThirdDesc, 3 );
      provider.WriteBit( iOffset + 0, m_bCaseSensitive, 4 );
      provider.WriteByte( iOffset + 2, m_FirstKeyLen );
      provider.WriteByte( iOffset + 3, m_SecondKeyLen );
      provider.WriteByte( iOffset + 4, m_ThirdKeyLen );
      iOffset += 5;

      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strFirstKey );
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strSecondKey );
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strThirdKey );
      provider.WriteByte( iOffset, 0 );
      iOffset++;
      m_iLength = iOffset - iStartOffset;
    }

    /// <summary>
    /// Returns size of the string
    /// </summary>
    /// <param name="strValue"></param>
    /// <returns></returns>
    private int GetStringSize( string strValue )
    {
      if( strValue == null || strValue.Length == 0 ) return 0;

      return Encoding.Unicode.GetByteCount( strValue ) + 1;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_PART_SIZE + GetStringSize( m_strFirstKey )
        + GetStringSize( m_strSecondKey ) + GetStringSize( m_strThirdKey ) + 1;
    }
    #endregion
  }
}
