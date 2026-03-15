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
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains relative offsets to calculate the stream position of
  /// the first cell record for each row.
  /// This record is written once in a row block.
  /// </summary>
  [ Biff( TBIFFRecord.DBCell ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DBCellRecord
#if DEBUG
    : BiffRecordRaw
#else
    : BiffRecordWithStreamPos
#endif
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 4;
    /// <summary>
    /// Subitem size.
    /// </summary>
    private const int DEF_SUB_ITEM_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Relative offset to first row record in the row block
    /// (difference between record position of this record and the row record;
    /// positive offset for an earlier stream position).
    /// </summary>
    [ BiffRecordPos( 0, 4, false ) ]
    private int m_iRowOffset = 0;

    /// <summary>
    /// Relative offset to first row record in the row block
    /// (difference between record position of this record and the row record;
    /// positive offset for an earlier stream position).
    /// </summary>
    private ushort[] m_arrCellOffset = new ushort[]{ 0 };
    #endregion

    #region Class properties
    /// <summary>
    /// Relative offset to first row record in the row block
    /// (difference between record position of this record and the row record;
    /// positive offset for an earlier stream position).
    /// </summary>
    public int RowOffset
    {
      get
      {
        return m_iRowOffset;
      }
      set
      {
        m_iRowOffset = value;
      }
    }

    /// <summary>
    /// Relative offset to first row record in the row block
    /// (difference between record position of this record and the row record;
    /// positive offset for an earlier stream position).
    /// </summary>
    public ushort[] CellOffsets
    {
      get
      {
        return m_arrCellOffset;
      }
      set
      {
        m_arrCellOffset = value;
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
        return 4;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  DBCellRecord()
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
    public  DBCellRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DBCellRecord( int iReserve )
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
      m_iRowOffset = provider.ReadInt32( iOffset );
      iOffset += 4;

      InternalDataIntegrityCheck();

      int iCount = ( m_iLength - 4 ) / 2;
      m_arrCellOffset = new ushort[ iCount ];

      for( int i = 0; i < iCount; i++, iOffset += 2 )
      {
        m_arrCellOffset[ i ] = provider.ReadUInt16( iOffset );
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
      int iOffsetLength = m_arrCellOffset.Length;
      m_iLength = GetStoreSize( version );

      provider.WriteInt32( iOffset, m_iRowOffset );
      iOffset += 4;

      for( int i = 0; i < iOffsetLength; iOffset += 2, i++ )
      {
        provider.WriteUInt16( iOffset, m_arrCellOffset[ i ] );
      }
    }

    /// <summary>
    /// This method checks the record's internal data array for integrity.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">If there is any internal error.</exception>
    private void InternalDataIntegrityCheck()
    {
      if( ( Length - 2 ) % 2 != 0 )
        throw new WrongBiffRecordDataException( "DBCellRecord" );
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_SIZE + m_arrCellOffset.Length * DEF_SUB_ITEM_SIZE;
    }
    #endregion
  }
}