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
  /// This record contains a SQL datatype identifier.
  /// </summary>
  [ Biff( TBIFFRecord.SQLDataTypeId ) ]
  [ CLSCompliant( false ) ]
  public class SQLDataTypeIdRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// The SQL datatypes.
    /// </summary>
    public enum SQLDataType
    {
      /// <summary>
      /// Represents the SQL_UNKNOWN_TYPE SQL data type.
      /// </summary>
      SQL_UNKNOWN_TYPE   = 0,
      /// <summary>
      /// Represents the SQL_CHAR SQL data type.
      /// </summary>
      SQL_CHAR           = 1,
      /// <summary>
      /// Represents the SQL_NUMERIC SQL data type.
      /// </summary>
      SQL_NUMERIC        = 2,
      /// <summary>
      /// Represents the SQL_DECIMAL SQL data type.
      /// </summary>
      SQL_DECIMAL        = 3,
      /// <summary>
      /// Represents the SQL_INTEGER SQL data type.
      /// </summary>
      SQL_INTEGER        = 4,
      /// <summary>
      /// Represents the SQL_SMALLINT SQL data type.
      /// </summary>
      SQL_SMALLINT       = 5,
      /// <summary>
      /// Represents the SQL_FLOAT SQL data type.
      /// </summary>
      SQL_FLOAT          = 6,
      /// <summary>
      /// Represents the SQL_REAL SQL data type.
      /// </summary>
      SQL_REAL           = 7,
      /// <summary>
      /// Represents the SQL_DOUBLE SQL data type.
      /// </summary>
      SQL_DOUBLE         = 8,
      /// <summary>
      /// Represents the SQL_DATETIME SQL data type.
      /// </summary>
      SQL_DATETIME       = 9,
      /// <summary>
      /// Represents the SQL_VARCHAR SQL data type.
      /// </summary>
      SQL_VARCHAR        = 12,
    }
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DefaultRecordSize = 2;
    #endregion

    #region Class members
    /// <summary>
    /// The SQL datatype of the field described in the immediately preceding SXFDB record.
    /// These are the same values as found in the ODBC SDK. See the SQL datatypes in SQL.H.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usDataType;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  SQLDataTypeIdRecord()
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
    public  SQLDataTypeIdRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  SQLDataTypeIdRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// The SQL datatype of the field described in the immediately preceding SXFDB record.
    /// </summary>
    public SQLDataType DataType
    {
      get
      {
        return ( SQLDataType )m_usDataType;
      }
      set
      {
        m_usDataType = ( ushort )value;
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
      m_usDataType = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usDataType );
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
