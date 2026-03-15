#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;
using System.Text;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record is stored on a separate stream that maintains information about
  /// each PivotTable cache. The record is followed by a single CacheDataExRecord
  /// and several FDB records, one for each field in the PivotTable.
  /// SXDBEX in Excel specification.
  /// </summary>
  [ Biff( TBIFFRecord.CacheData ) ]
  [ CLSCompliant( false ) ]
  public class CacheDataRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Offset to the user name field.
    /// </summary>
    private const int DEF_USERNAME_OFFSET = 20;
    #endregion

    #region Class members
    /// <summary>
    /// Number of records in database.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iRecordsNumber;
    /// <summary>
    /// Identifies the stream.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usStreamId;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Indicates whether data is being saved with table layout.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bSaveData;
    /// <summary>
    /// Indicates whether the PivotTable must be refreshed before next update.
    /// </summary>
    [ BiffRecordPos( 6, 1, TFieldType.Bit ) ]
    private bool m_bInvalid;
    /// <summary>
    /// Indicates whether the PivotTable will be refreshed on load.
    /// </summary>
    [ BiffRecordPos( 6, 2, TFieldType.Bit ) ]
    private bool m_bRefreshOnLoad;
    /// <summary>
    /// Indicates whether the cache is optimized to use the least amount of memory.
    /// </summary>
    [ BiffRecordPos( 6, 3, TFieldType.Bit ) ]
    private bool m_bOptimizeCache;
    /// <summary>
    /// Indicates whether results of the query are obtained in the background.
    /// </summary>
    [ BiffRecordPos( 6, 4, TFieldType.Bit ) ]
    private bool m_bBackgroundQuery;
    /// <summary>
    /// Indicates whether refresh is enabled.
    /// </summary>
    [ BiffRecordPos( 6, 5, TFieldType.Bit ) ]
    private bool m_bEnableRefresh;

    /// <summary>
    /// Number of records for each database block.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usRecordsInBlock;
    /// <summary>
    /// Number of base fields in databases.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usBaseFieldsCount;
    /// <summary>
    /// Number of base fields, grouped fields, and calculated fields.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usFieldsNumber;
    /// <summary>
    /// This value is not used and can be set to zero.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usReserved;
    /// <summary>
    /// Data source is one of:
    /// 1 - Excel worksheet,
    /// 2 - external data,
    /// 4 - consolidation,
    /// 8 - scenario PivotTable.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usSourceType;
    /// <summary>
    /// Number of characters in the string containing the name of the user who
    /// last refreshed the PivotTable.
    /// </summary>
    [ BiffRecordPos( 18, 2 ) ]
    private ushort m_usUserNameSize;

    /// <summary>
    /// The user who last refreshed the PivotTable.
    /// </summary>
    private string m_strUserName;
    private bool m_bUserName16Bit;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  CacheDataRecord()
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
    public  CacheDataRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CacheDataRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Number of records in database.
    /// </summary>
    public int RecordsNumber
    {
      get
      {
        return m_iRecordsNumber;
      }
      set
      {
        m_iRecordsNumber = value;
      }
    }
    /// <summary>
    /// Identifies the stream.
    /// </summary>
    public ushort StreamId
    {
      get
      {
        return m_usStreamId;
      }
      set
      {
        m_usStreamId = value;
      }
    }
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
    /// Indicates whether data is being saved with table layout.
    /// </summary>
    public bool IsSaveData
    {
      get
      {
        return m_bSaveData;
      }
      set
      {
        m_bSaveData = value;
      }
    }
    /// <summary>
    /// Indicates whether the PivotTable must be refreshed before next update.
    /// </summary>
    public bool IsInvalid
    {
      get
      {
        return m_bInvalid;
      }
      set
      {
        m_bInvalid = value;
      }
    }
    /// <summary>
    /// Indicates whether the PivotTable will be refreshed on load.
    /// </summary>
    public bool IsRefreshOnLoad
    {
      get
      {
        return m_bRefreshOnLoad;
      }
      set
      {
        m_bRefreshOnLoad = value;
      }
    }
    /// <summary>
    /// Indicates whether the cache is optimized to use the least amount of memory.
    /// </summary>
    public bool IsOptimizeCache
    {
      get
      {
        return m_bOptimizeCache;
      }
      set
      {
        m_bOptimizeCache = value;
      }
    }
    /// <summary>
    /// Indicates whether results of the query are obtained in the background.
    /// </summary>
    public bool IsBackgroundQuery
    {
      get
      {
        return m_bBackgroundQuery;
      }
      set
      {
        m_bBackgroundQuery = value;
      }
    }
    /// <summary>
    /// Indicates whether refresh is enabled.
    /// </summary>
    public bool IsEnableRefresh
    {
      get
      {
        return m_bEnableRefresh;
      }
      set
      {
        m_bEnableRefresh = value;
      }
    }

    /// <summary>
    /// Number of records for each database block.
    /// </summary>
    public ushort RecordsInBlock
    {
      get
      {
        return m_usRecordsInBlock;
      }
      set
      {
        m_usRecordsInBlock = value;
      }
    }
    /// <summary>
    /// Number of base fields in databases.
    /// </summary>
    public ushort BaseFieldsCount
    {
      get
      {
        return m_usBaseFieldsCount;
      }
      set
      {
        m_usBaseFieldsCount = value;
      }
    }
    /// <summary>
    /// Number of base fields, grouped fields, and calculated fields.
    /// </summary>
    public ushort FieldsNumber
    {
      get
      {
        return m_usFieldsNumber;
      }
      set
      {
        m_usFieldsNumber = value;
      }
    }
    /// <summary>
    /// This value is not used and can be set to zero. Read-only.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
#if DEBUG
      set
      {
        m_usReserved = value;
      }
#endif
    }
    /// <summary>
    /// Data source is one of:
    /// 1 - Excel worksheet,
    /// 2 - external data,
    /// 4 - consolidation,
    /// 8 - scenario PivotTable.
    /// </summary>
    public ExcelDataSourceType SourceType
    {
      get
      {
        return ( ExcelDataSourceType )m_usSourceType;
      }
      set
      {
        m_usSourceType = ( ushort )value;
      }
    }
    /// <summary>
    /// Number of characters in the string containing the name of the user who
    /// last refreshed the PivotTable. Read-only.
    /// </summary>
    public ushort UserNameSize
    {
      get
      {
        return m_usUserNameSize;
      }
    }
    /// <summary>
    /// The user who last refreshed the PivotTable.
    /// </summary>
    public string UserName
    {
      get
      {
        return m_strUserName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strUserName = value;
        m_usUserNameSize = ( ushort )m_strUserName.Length;
        m_bUserName16Bit = !BiffRecordRawWithArray.IsAsciiString( value );
      }
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
      m_iRecordsNumber = provider.ReadInt32( iOffset + 0 );
      m_usStreamId = provider.ReadUInt16( iOffset + 4 );
      m_usOptions = provider.ReadUInt16( iOffset + 6 );
      m_bSaveData = provider.ReadBit( iOffset + 6, 0 );
      m_bInvalid = provider.ReadBit( iOffset + 6, 1 );
      m_bRefreshOnLoad = provider.ReadBit( iOffset + 6, 2 );
      m_bOptimizeCache = provider.ReadBit( iOffset + 6, 3 );
      m_bBackgroundQuery = provider.ReadBit( iOffset + 6, 4 );
      m_bEnableRefresh = provider.ReadBit( iOffset + 6, 5 );
      m_usRecordsInBlock = provider.ReadUInt16( iOffset + 8 );
      m_usBaseFieldsCount = provider.ReadUInt16( iOffset + 10 );
      m_usFieldsNumber = provider.ReadUInt16( iOffset + 12 );
      m_usReserved = provider.ReadUInt16( iOffset + 14 );
      m_usSourceType = provider.ReadUInt16( iOffset + 16 );
      m_usUserNameSize = provider.ReadUInt16( iOffset + 18 );

      iOffset += DEF_USERNAME_OFFSET;
      int iSize;
      m_strUserName = provider.ReadString( iOffset, m_usUserNameSize, out iSize, false );

      m_bUserName16Bit = ( m_strUserName.Length * 2 + 3 >= iSize );
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
      provider.WriteInt32( iOffset + 0, m_iRecordsNumber );
      provider.WriteUInt16( iOffset + 4, m_usStreamId );
      provider.WriteUInt16( iOffset + 6, m_usOptions );
      provider.WriteBit( iOffset + 6, m_bSaveData, 0 );
      provider.WriteBit( iOffset + 6, m_bInvalid, 1 );
      provider.WriteBit( iOffset + 6, m_bRefreshOnLoad, 2 );
      provider.WriteBit( iOffset + 6, m_bOptimizeCache, 3 );
      provider.WriteBit( iOffset + 6, m_bBackgroundQuery, 4 );
      provider.WriteBit( iOffset + 6, m_bEnableRefresh, 5 );
      provider.WriteUInt16( iOffset + 8, m_usRecordsInBlock );
      provider.WriteUInt16( iOffset + 10, m_usBaseFieldsCount );
      provider.WriteUInt16( iOffset + 12, m_usFieldsNumber );
      provider.WriteUInt16( iOffset + 14, m_usReserved );
      provider.WriteUInt16( iOffset + 16, m_usSourceType );
      provider.WriteUInt16( iOffset + 18, m_usUserNameSize );
      iOffset += DEF_USERNAME_OFFSET;
      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strUserName, m_bUserName16Bit );
      m_iLength = iOffset - iStartOffset;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int stringLen = m_strUserName.Length;
      int result = DEF_USERNAME_OFFSET + 
        /*Encoding.ASCII.GetByteCount( */stringLen /*)*/ + 1;

      if( m_bUserName16Bit )
        result += stringLen;

      return result;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Fills cache with data from the range.
    /// </summary>
    /// <param name="dataRange">Data range to fill.</param>
    public void FillCache( IRange dataRange )
    {
      if( dataRange == null )
        throw new ArgumentNullException( "dataRange" );
    }
    #endregion
  }
}
