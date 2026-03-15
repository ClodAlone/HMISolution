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
  /// This record stores information about the SQL query string that retrieves
  /// external data for a PivotTable. The record is followed by SXSTRING records
  /// that contain the SQL strings and then by a SXSTRING record that contains
  /// the SQL server connection string.
  /// </summary>
  [ Biff( TBIFFRecord.ExternalSourceInfo ) ]
  [ CLSCompliant( false ) ]
  public class ExternalSourceInfoRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Bit mask for DataSourceType property.
    /// </summary>
    private const ushort DEF_DATASOURCETYPE_BITMASK = 0x0007;
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 12;
    #endregion

    #region Class members
    /// <summary>
    /// Options flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True for ODBC connection.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bOdbcConnection;
    /// <summary>
    /// True for SQL statement or URL.
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bSql;
    /// <summary>
    /// True for server-based page fields.
    /// </summary>
    [ BiffRecordPos( 0, 5, TFieldType.Bit ) ]
    private bool m_bSqlSav;
    /// <summary>
    /// True for Web (WWW) query.
    /// </summary>
    [ BiffRecordPos( 0, 6, TFieldType.Bit ) ]
    private bool m_bWeb;
    /// <summary>
    /// Indicates whether save password option is on.
    /// </summary>
    [ BiffRecordPos( 0, 7, TFieldType.Bit ) ]
    private bool m_bSavePassword;
    /// <summary>
    /// Indicates whether save tables in HTML only option is on.
    /// </summary>
    [ BiffRecordPos( 1, 0, TFieldType.Bit ) ]
    private bool m_bTablesOnlyHtml;
    /// <summary>
    /// Number of parameter strings.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usParamsCount;
    /// <summary>
    /// Number of strings for SQL statement or URL.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usQueryCount;
    /// <summary>
    /// Number of strings for post method of Web query.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usWebPostCount;
    /// <summary>
    /// Number of strings for SQL statement for server-based page fields.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usSQLSavCount;
    /// <summary>
    /// Number of strings for ODBC connection string.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usOdbcConnectionCount;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  ExternalSourceInfoRecord()
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
    public  ExternalSourceInfoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ExternalSourceInfoRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Options flags. Read-only.
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
    /// 1 - ODBC data source,
    /// 2 - DAO recordset (no actual information about the recordset is saved),
    /// 3 - reserved,
    /// 4 - Web (WWW) query.
    /// </summary>
    public ushort DataSourceType
    {
      get
      {
        return GetUInt16BitsByMask( m_usOptions, DEF_DATASOURCETYPE_BITMASK );
      }
      set
      {
        if( value < 1 || value > 4 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 4" );

        SetUInt16BitsByMask( ref m_usOptions, DEF_DATASOURCETYPE_BITMASK, value );
      }
    }
    /// <summary>
    /// True for ODBC connection.
    /// </summary>
    public bool IsOdbcConnection
    {
      get
      {
        return m_bOdbcConnection;
      }
      set
      {
        m_bOdbcConnection = value;
      }
    }
    /// <summary>
    /// True for SQL statement or URL.
    /// </summary>
    public bool IsSql
    {
      get
      {
        return m_bSql;
      }
      set
      {
        m_bSql = value;
      }
    }
    /// <summary>
    /// True for server-based page fields.
    /// </summary>
    public bool IsSqlSav
    {
      get
      {
        return m_bSqlSav;
      }
      set
      {
        m_bSqlSav = value;
      }
    }
    /// <summary>
    /// True for Web (WWW) query.
    /// </summary>
    public bool IsWeb
    {
      get
      {
        return m_bWeb;
      }
      set
      {
        m_bWeb = value;
      }
    }
    /// <summary>
    /// Indicates whether save password option is on.
    /// </summary>
    public bool IsSavePassword
    {
      get
      {
        return m_bSavePassword;
      }
      set
      {
        m_bSavePassword = value;
      }
    }
    /// <summary>
    /// Indicates whether save tables in HTML only option is on.
    /// </summary>
    public bool IsTablesOnlyHtml
    {
      get
      {
        return m_bTablesOnlyHtml;
      }
      set
      {
        m_bTablesOnlyHtml = value;
      }
    }
    /// <summary>
    /// Number of parameter strings.
    /// </summary>
    public ushort ParamsCount
    {
      get
      {
        return m_usParamsCount;
      }
      set
      {
        m_usParamsCount = value;
      }
    }
    /// <summary>
    /// Number of strings for SQL statement or URL.
    /// </summary>
    public ushort QueryCount
    {
      get
      {
        return m_usQueryCount;
      }
      set
      {
        m_usQueryCount = value;
      }
    }
    /// <summary>
    /// Number of strings for post method of Web query.
    /// </summary>
    public ushort WebPostCount
    {
      get
      {
        return m_usWebPostCount;
      }
      set
      {
        m_usWebPostCount = value;
      }
    }
    /// <summary>
    /// Number of strings for SQL statement for server-based page fields.
    /// </summary>
    public ushort SQLSavCount
    {
      get
      {
        return m_usSQLSavCount;
      }
      set
      {
        m_usSQLSavCount = value;
      }
    }
    /// <summary>
    /// Number of strings for ODBC connection string.
    /// </summary>
    public ushort OdbcConnectionCount
    {
      get
      {
        return m_usOdbcConnectionCount;
      }
      set
      {
        m_usOdbcConnectionCount = value;
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
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bOdbcConnection = provider.ReadBit( iOffset + 0, 3 );
      m_bSql = provider.ReadBit( iOffset + 0, 4 );
      m_bSqlSav = provider.ReadBit( iOffset + 0, 5 );
      m_bWeb = provider.ReadBit( iOffset + 0, 6 );
      m_bSavePassword = provider.ReadBit( iOffset + 0, 7 );
      m_bTablesOnlyHtml = provider.ReadBit( iOffset + 1, 0 );
      m_usParamsCount = provider.ReadUInt16( iOffset + 2 );
      m_usQueryCount = provider.ReadUInt16( iOffset + 4 );
      m_usWebPostCount = provider.ReadUInt16( iOffset + 6 );
      m_usSQLSavCount = provider.ReadUInt16( iOffset + 8 );
      m_usOdbcConnectionCount = provider.ReadUInt16( iOffset + 10 );
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
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bOdbcConnection, 3 );
      provider.WriteBit( iOffset + 0, m_bSql, 4 );
      provider.WriteBit( iOffset + 0, m_bSqlSav, 5 );
      provider.WriteBit( iOffset + 0, m_bWeb, 6 );
      provider.WriteBit( iOffset + 0, m_bSavePassword, 7 );
      provider.WriteBit( iOffset + 1, m_bTablesOnlyHtml, 0 );
      provider.WriteUInt16( iOffset + 2, m_usParamsCount );
      provider.WriteUInt16( iOffset + 4, m_usQueryCount );
      provider.WriteUInt16( iOffset + 6, m_usWebPostCount );
      provider.WriteUInt16( iOffset + 8, m_usSQLSavCount );
      provider.WriteUInt16( iOffset + 10, m_usOdbcConnectionCount );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
