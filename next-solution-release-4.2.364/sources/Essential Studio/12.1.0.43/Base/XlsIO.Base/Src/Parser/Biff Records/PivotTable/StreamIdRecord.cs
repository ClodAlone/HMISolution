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
  /// This record is a header record for a group of SXVS, SXEXT, and SXSTRING records
  /// that describe the PivotTable streams in the SX DB storage (the PivotTable cache storage).
  /// The StreamId field identifies the stream.
  /// </summary>
  [ Biff( TBIFFRecord.StreamId ) ]
  [ CLSCompliant( false ) ]
  public class StreamIdRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Stream Id.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usStreamId;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  StreamIdRecord()
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
    public  StreamIdRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  StreamIdRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion
    
    #region Class properties
    /// <summary>
    /// Stream Id.
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
      m_usStreamId = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usStreamId );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Size of the required storage space.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
