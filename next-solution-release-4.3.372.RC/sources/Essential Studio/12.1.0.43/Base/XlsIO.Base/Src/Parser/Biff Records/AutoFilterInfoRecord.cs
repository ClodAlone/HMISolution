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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for AutoFilterInfoRecord.
  /// </summary>
  [ Biff( TBIFFRecord.AutoFilterInfo ) ]
  [ CLSCompliant( false ) ]
  public class AutoFilterInfoRecord : BiffRecordRaw
  {
    #region Class constants
    private const int DEF_RECORD_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Number of AutoFilter drop-down arrows on the sheet.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usArrowsCount;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Number of AutoFilter drop-down arrows on the sheet.
    /// </summary>
    public ushort ArrowsCount
    {
      get
      {
        return m_usArrowsCount;
      }
      set
      {
        m_usArrowsCount = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  AutoFilterInfoRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  AutoFilterInfoRecord( Stream stream, out int itemSize )
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
    public  AutoFilterInfoRecord( int iReserve )
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
      m_usArrowsCount = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usArrowsCount );
      m_iLength = ExcelConstants.ShortSize;
    }

    #endregion
  }
}
