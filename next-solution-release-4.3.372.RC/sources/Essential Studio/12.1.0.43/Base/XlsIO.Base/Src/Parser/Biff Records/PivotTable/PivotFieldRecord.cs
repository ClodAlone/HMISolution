#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// Summary description for PivotFieldRecord.
  /// </summary>
  [ Biff( TBIFFRecord.PivotField ) ]
  [ CLSCompliant( false ) ]
  public class PivotFieldRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bInIndexList;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bNotInList;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 5, TFieldType.Bit ) ]
    private bool m_bDouble;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 6, TFieldType.Bit ) ]
    private bool m_bDoubleInt;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 7, TFieldType.Bit ) ]
    private bool m_bString;
    /// <summary>
    /// Maybe m_bNotAny.
    /// </summary>
    [ BiffRecordPos( 1, 0, TFieldType.Bit ) ]
    private bool m_bUnknown;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 1, 1, TFieldType.Bit ) ]
    private bool m_bLongIndex;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 1, 2, TFieldType.Bit ) ]
    private bool m_bUnknown2;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 1, 3, TFieldType.Bit ) ]
    private bool m_bDate;
    /// <summary>
    /// Unknown.
    /// </summary>
    [ BiffRecordPos( 2, 4 ) ]
    private uint m_usReserved1;
    /// <summary>
    /// First item count.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usItemCount1;
    /// <summary>
    /// Unknown.
    /// </summary>
    [ BiffRecordPos( 8, 4 ) ]
    private uint m_usReserved2;
    /// <summary>
    /// Second item count.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usItemCount2;
    /// <summary>
    /// Field name.
    /// </summary>
    [ BiffRecordPos( 14, TFieldType.String16Bit ) ]
    private string m_strFieldName;
    private bool m_bFieldName16Bit;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotFieldRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  PivotFieldRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotFieldRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Option flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsInIndexList
    {
      get
      {
        return m_bInIndexList;
      }
      set
      {
        m_bInIndexList = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsNotInList
    {
      get
      {
        return m_bNotInList;
      }
      set
      {
        m_bNotInList = value;
      }
    }
    /// <summary>
    /// Indicates whether field can contain double values.
    /// </summary>
    public bool IsDouble
    {
      get
      {
        return m_bDouble;
      }
      set
      {
        m_bDouble = value;
      }
    }
    /// <summary>
    /// Indicates whether field can contain double values without fraction.
    /// </summary>
    public bool IsDoubleInt
    {
      get
      {
        return m_bDoubleInt;
      }
      set
      {
        m_bDoubleInt = value;
      }
    }
    /// <summary>
    /// Indicates whether field can contain string values.
    /// </summary>
    public bool IsString
    {
      get
      {
        return m_bString;
      }
      set
      {
        m_bString = value;
      }
    }
    /// <summary>
    /// Unknown flag.
    /// </summary>
    public bool IsUnknown
    {
      get
      {
        return m_bUnknown;
      }
      set
      {
        m_bUnknown = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsLongIndex
    {
      get
      {
        return m_bLongIndex;
      }
      set
      {
        m_bLongIndex = value;
      }
    }
    /// <summary>
    /// Unknown flag.
    /// </summary>
    public bool IsUnknown2
    {
      get
      {
        return m_bUnknown2;
      }
      set
      {
        m_bUnknown2 = value;
      }
    }
    /// <summary>
    /// Indicates whether field can contain date values.
    /// </summary>
    public bool IsDate
    {
      get
      {
        return m_bDate;
      }
      set
      {
        m_bDate = value;
      }
    }
    /// <summary>
    /// Unknown.
    /// </summary>
    public uint Reserved1
    {
      get
      {
        return m_usReserved1;
      }
      set
      {
        m_usReserved1 = value;
      }
    }
    /// <summary>
    /// First item count.
    /// </summary>
    public ushort ItemCount1
    {
      get
      {
        return m_usItemCount1;
      }
      set
      {
        m_usItemCount1 = value;
      }
    }
    /// <summary>
    /// Unknown.
    /// </summary>
    public uint Reserved2
    {
      get
      {
        return m_usReserved2;
      }
      set
      {
        m_usReserved2 = value;
      }
    }
    /// <summary>
    /// Second item count.
    /// </summary>
    public ushort ItemCount2
    {
      get
      {
        return m_usItemCount2;
      }
      set
      {
        m_usItemCount2 = value;
      }
    }
    /// <summary>
    /// Field name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strFieldName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strFieldName = value;
        m_bFieldName16Bit = !BiffRecordRawWithArray.IsAsciiString( value );
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
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bInIndexList = provider.ReadBit( iOffset + 0, 0 );
      m_bNotInList = provider.ReadBit( iOffset + 0, 1 );
      m_bDouble = provider.ReadBit( iOffset + 0, 5 );
      m_bDoubleInt = provider.ReadBit( iOffset + 0, 6 );
      m_bString = provider.ReadBit( iOffset + 0, 7 );
      m_bUnknown = provider.ReadBit( iOffset + 1, 0 );
      m_bLongIndex = provider.ReadBit( iOffset + 1, 1 );
      m_bUnknown2 = provider.ReadBit( iOffset + 1, 2 );
      m_bDate = provider.ReadBit( iOffset + 1, 3 );
      m_usReserved1 = provider.ReadUInt32( iOffset + 2 );
      m_usItemCount1 = provider.ReadUInt16( iOffset + 6 );
      m_usReserved2 = provider.ReadUInt32( iOffset + 8 );
      m_usItemCount2 = provider.ReadUInt16( iOffset + 12 );

      int iFullLength;
      m_strFieldName = provider.ReadString16Bit( iOffset + 14, out iFullLength );

      m_bFieldName16Bit = ( m_strFieldName.Length * 2 + 3 >= iFullLength );
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
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bInIndexList, 0 );
      provider.WriteBit( iOffset + 0, m_bNotInList, 1 );
      provider.WriteBit( iOffset + 0, m_bDouble, 5 );
      provider.WriteBit( iOffset + 0, m_bDoubleInt, 6 );
      provider.WriteBit( iOffset + 0, m_bString, 7 );
      provider.WriteBit( iOffset + 1, m_bUnknown, 0 );
      provider.WriteBit( iOffset + 1, m_bLongIndex, 1 );
      provider.WriteBit( iOffset + 1, m_bUnknown2, 2 );
      provider.WriteBit( iOffset + 1, m_bDate, 3 );
      provider.WriteUInt32( iOffset + 2, m_usReserved1 );
      provider.WriteUInt16( iOffset + 6, m_usItemCount1 );
      provider.WriteUInt32( iOffset + 8, m_usReserved2 );
      provider.WriteUInt16( iOffset + 12, m_usItemCount2 );
      provider.WriteString16Bit( iOffset + 14, m_strFieldName, m_bFieldName16Bit );

      m_iLength = GetStoreSize( version );
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int stringLen = m_strFieldName.Length;
      int result = 14 + 3 + stringLen /** 2*/;

      if( m_bFieldName16Bit )
        result += stringLen;

      return result;
    }
    #endregion
  }
}
