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
  /// Extended SST table info subrecord
  /// contains the elements of "info" in the SST's array field.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.ExtSSTInfoSub ) ]
  [ CLSCompliant( false ) ]
  public class ExtSSTInfoSubRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Absolute stream position of first string of the portion.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int     m_iStreamPos = 0;
    /// <summary>
    /// Position of first string of the portion inside the current record,
    /// including record header. This counter restarts at zero, if the SST
    /// record is continued with a CONTINUE record.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort  m_usBucketSSTOffset = 0;
    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort  m_usReserved = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Absolute stream position of first string of the portion.
    /// </summary>
    public int    StreamPosition
    {
      get
      {
        return m_iStreamPos;
      }
      set
      {
        m_iStreamPos = value;
      }
    }

    /// <summary>
    /// Position of first string of the portion inside of current record,
    /// including record header. This counter restarts at zero, if the SST
    /// record is continued with a CONTINUE record.
    /// </summary>
    public ushort BucketSSTOffset
    {
      get
      {
        return m_usBucketSSTOffset;
      }
      set
      {
        if( value > DEF_RECORD_MAX_SIZE + 4 )
        {
          throw new ArgumentOutOfRangeException( "BucketSSTOffset",
            "Bucket SST Offset cannot be larger then MAX record size. " +
            "On each Continue Record offset must be started from zero." );
        }

        m_usBucketSSTOffset = value;
      }
    }
    /// <summary>
    /// Read-only. Reserved.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 8;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return 8;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  ExtSSTInfoSubRecord()
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
    public  ExtSSTInfoSubRecord( Stream stream, out int itemSize )
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
    public  ExtSSTInfoSubRecord( int iReserve )
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
      m_iStreamPos = provider.ReadInt32( iOffset + 0 );
      m_usBucketSSTOffset = provider.ReadUInt16( iOffset + 4 );
      m_usReserved = provider.ReadUInt16( iOffset + 6 );
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
      provider.WriteInt32( iOffset + 0, m_iStreamPos );
      provider.WriteUInt16( iOffset + 4, m_usBucketSSTOffset );
      provider.WriteUInt16( iOffset + 6, m_usReserved );
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
