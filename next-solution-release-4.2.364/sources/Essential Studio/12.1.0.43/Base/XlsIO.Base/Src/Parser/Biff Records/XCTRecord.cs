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
  /// XCT � CRN Count:
  /// This record stores the number of immediately following CRN records.
  /// These records are used to store the cell contents of external references.
  /// </summary>
  [ CLSCompliant( false ) ]
  [ Biff( TBIFFRecord.XCT ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class XCTRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Number of following CRN records.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usCRNCount = 0;

    /// <summary>
    /// Index into sheet table of the involved SUPBOOK record.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usSheetTableIndex = 0;

    #endregion

    #region Class properties

    /// <summary>
    /// Number of following CRN records.
    /// </summary>
    public ushort CRNCount
    {
      get
      {
        return m_usCRNCount;
      }
      set
      {
        m_usCRNCount = value;
      }
    }

    /// <summary>
    /// Index into sheet table of the SUPBOOK record.
    /// </summary>
    public ushort SheetTableIndex
    {
      get
      {
        return m_usSheetTableIndex;
      }
      set
      {
        m_usSheetTableIndex = value;
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
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  XCTRecord()
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
    public  XCTRecord( Stream stream, out int itemSize )
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
    public  XCTRecord( int iReserve )
      : base( iReserve )
    {
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
      m_usCRNCount = provider.ReadUInt16( iOffset );
      m_usSheetTableIndex = provider.ReadUInt16( iOffset + 2 );
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
      m_iLength = DEF_RECORD_SIZE;
      provider.WriteUInt16( iOffset, m_usCRNCount );
      provider.WriteUInt16( iOffset + 2, m_usSheetTableIndex );
    }
    #endregion
  }
}
