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

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// 
  /// </summary>
  [ Biff( TBIFFRecord.Chart3DDataFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class Chart3DDataFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_RECORD_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_DataFormatBase;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_DataFormatTop;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public ExcelBaseFormat DataFormatBase
    {
      get
      {
        return ( ExcelBaseFormat )m_DataFormatBase;
      }
      set
      {
        m_DataFormatBase = ( byte )value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelTopFormat DataFormatTop
    {
      get
      {
        return ( ExcelTopFormat )m_DataFormatTop;
      }
      set
      {
        m_DataFormatTop = ( byte )value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, sets all fields default values.
    /// </summary>
    public  Chart3DDataFormatRecord()
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
    public  Chart3DDataFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  Chart3DDataFormatRecord( int iReserve )
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
      // TODO: check correctness of data

      m_DataFormatBase = provider.ReadByte( iOffset );
      m_DataFormatTop = provider.ReadByte( iOffset + 1 );
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

      provider.WriteByte( iOffset, m_DataFormatBase );
      provider.WriteByte( iOffset + 1, m_DataFormatTop );
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}
