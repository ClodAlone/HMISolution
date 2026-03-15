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
using System.Collections;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  ///<exclude/>
  /// <summary>
  /// Occurs right after BOF, gives information where the DBCELL records are for a sheet.
  /// Important for locating cells.
  /// </summary>
  [ Biff( TBIFFRecord.Index ) ]
//  [ BiffOffsetsRecords( TBIFFRecord.BOF )
//  , BiffOffsetsRecords( TBIFFRecord.DBCell )
//  , BiffOffsetsRecords( TBIFFRecord.DefaultColWidth ) ]
//  [ BiffOffsetOrder( TBIFFRecord.BOF, TBIFFRecord.DBCell, TBIFFRecord.DefaultColWidth ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class IndexRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 16;
    /// <summary>
    /// Subitem size.
    /// </summary>
    private const int DEF_SUB_ITEM_SIZE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 0, 4 , true ) ]
    private int m_iReserved0 = 0;

    /// <summary>
    /// Index to first used row.
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iFirstRow = 0;

    /// <summary>
    /// Index to first row of unused tail of sheet.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iLastRowAdd1 = 0;

    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 12, 4 , true ) ]
    private int m_iReserved1 = 0;

    /// <summary>
    /// Array of absolute stream positions to the DBCELL record.
    /// </summary>
    private int[] m_arrDbCells = null;

    /// <summary>
    /// Array with DBCell records that must be referenced by this record.
    /// </summary>
    private List<DBCellRecord> m_arrDBCellRecords;
    #endregion

    #region Class properties
    /// <summary>
    /// Index to first used row.
    /// </summary>
    public int    FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }

    /// <summary>
    /// Index to first row of unused tail of sheet.
    /// </summary>
    public int    LastRow
    {
      get
      {
        return m_iLastRowAdd1;
      }
      set
      {
        m_iLastRowAdd1 = value;
      }
    }

    /// <summary>
    /// Array of nm absolute stream positions to the DBCELL record.
    /// </summary>
    public int[]  DbCells
    {
      get
      {
        return m_arrDbCells;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException();

        if( value.Length > 2048 )
          throw new ArgumentOutOfRangeException( "Worksheet cannot contain more than 2048 DBCells." );

        m_arrDbCells = value;

      }
    }
    /// <summary>
    /// Read-only. Not used.
    /// </summary>
    public int    Reserved0
    {
      get
      {
        return m_iReserved0;
      }
    }
    /// <summary>
    /// Read-only. Not used.
    /// </summary>
    public int    Reserved1
    {
      get
      {
        return m_iReserved1;
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
        return 16;
      }
    }

    /// <summary>
    /// Gets / sets array with DBCell records that must be referenced by this record.
    /// </summary>
    internal List<DBCellRecord> DbCellRecords
    {
      get
      {
        return m_arrDBCellRecords;
      }
      set
      {
        m_arrDBCellRecords = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  IndexRecord()
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
    public  IndexRecord( Stream stream, out int itemSize )
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
    public  IndexRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns></returns>
    public override int FillStream( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      UpdateOffsets();
      return base.FillStream( writer, provider, encryptor, streamPosition );
    }

    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_iReserved0 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iFirstRow = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iLastRowAdd1 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iReserved1 = provider.ReadInt32( iOffset );
      iOffset += 4;

      InternalDataIntegrityCheck();

      m_arrDbCells = new int[ ( m_iLength - 16 ) / 4 ];

      for( int j = 0; iOffset < m_iLength; iOffset += 4, j++ )
      {
        m_arrDbCells[ j ] = provider.ReadInt32( iOffset );
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
      m_iLength = GetStoreSize( version );

      provider.WriteInt32( iOffset, m_iReserved0 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iFirstRow );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iLastRowAdd1 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iReserved1 );
      iOffset += 4;

      if( m_arrDbCells != null )
      {
        int iDBCellLen = m_arrDbCells.Length;

        for( int j = 0; j < iDBCellLen; iOffset += DEF_SUB_ITEM_SIZE, j++ )
        {
          provider.WriteInt32( iOffset, m_arrDbCells[ j ] );
        }
      }
    }

    /// <summary>
    /// This method checks record's internal data array for integrity.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    private void InternalDataIntegrityCheck()
    {
      // First we should check if the string's last byte is the last byte of the record.
      if( m_iLength % 4 != 0 )
        throw new WrongBiffRecordDataException( "IndexRecord" );
    }

    /// <summary>
    /// Method update fields of record which must contain stream offset
    /// or other data. This method must be called before save operation
    /// when all records placed in array on own positions and offsets can
    /// be freely calculated.
    /// </summary>
    public void UpdateOffsets()
    {
      if( m_arrDBCellRecords == null ) return;

      for( int i = 0, len = m_arrDBCellRecords.Count; i < len; i++ )
      {
        DBCellRecord dbcell = m_arrDBCellRecords[ i ];
        long lStreamPos = dbcell.StreamPos;
        m_arrDbCells[ i ] = ( int )lStreamPos;
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iLength = ( m_arrDbCells != null )
        ? m_arrDbCells.Length
        : 0;

      return DEF_FIXED_SIZE + iLength * DEF_SUB_ITEM_SIZE;
    }
    #endregion
  }
}
