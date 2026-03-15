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
  [ Biff( TBIFFRecord.UnkMarker ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class UnknownMarkerRecord  : BiffRecordRaw
  {
    #region Class constants
    private const ushort DEF_RESERVED1 = 0x37;
    #endregion

    #region Class members

    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usReserved0 = 0;

    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usReserved1 = 0x37;

    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usReserved2 = 0;
    #endregion

    #region Class properties

    /// <summary>
    /// Reserved.
    /// </summary>
    public ushort Reserved0
    {
      get
      {
        return m_usReserved0;
      }
      set
      {
        m_usReserved0 = value;
      }
    }

    /// <summary>
    /// Reserved.
    /// </summary>
    public ushort Reserved1
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
    /// Reserved.
    /// </summary>
    public ushort Reserved2
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
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 6;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return 6;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  UnknownMarkerRecord()
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
    public  UnknownMarkerRecord( Stream stream, out int itemSize )
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
    public  UnknownMarkerRecord( int iReserve )
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usReserved0 = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usReserved1 = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usReserved2 = provider.ReadUInt16( iOffset );
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
      m_usReserved0 = 0;
      m_usReserved1 = DEF_RESERVED1;
      m_usReserved2 = 0;

      provider.WriteUInt16( iOffset, m_usReserved0 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usReserved1 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usReserved2 );
    }
    #endregion
  }
}
