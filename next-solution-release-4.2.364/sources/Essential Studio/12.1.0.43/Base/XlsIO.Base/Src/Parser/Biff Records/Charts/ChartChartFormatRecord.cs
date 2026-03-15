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
  /// This record is the parent record for the chart group format 
  /// description. Each chart group will have a separate CHARTFORMAT 
  /// record, followed by a BEGIN record, the chart-group description, 
  /// and an END record.
  /// </summary>
  [ Biff( TBIFFRecord.ChartChartFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartChartFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 20;
    #endregion

    #region Class members
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iReserved0 = 0;
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iReserved1 = 0;
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iReserved2 = 0;
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iReserved3 = 0;
    /// <summary>
    /// Holder of record options.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    [ BiffRecordPos( 16, 0, TFieldType.Bit ) ]
    private bool m_bIsVaryColor;
    /// <summary>
    /// Drawing order (0 = bottom of the z-order).
    /// </summary>
    [ BiffRecordPos( 18, 2 ) ]
    private ushort m_usZOrder;
    #endregion

    #region Class properties
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    public int Reserved0
    {
      get
      {
        return m_iReserved0;
      }
    }
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    public int Reserved1
    {
      get
      {
        return m_iReserved1;
      }
    }
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    public int Reserved2
    {
      get
      {
        return m_iReserved2;
      }
    }
    /// <summary>
    /// Field reserved by Microsoft for own use. Read-only.
    /// </summary>
    public int Reserved3
    {
      get
      {
        return m_iReserved3;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    public bool IsVaryColor
    {
      get
      {
        return m_bIsVaryColor;
      }
      set
      {
        m_bIsVaryColor = value;
      }
    }
    /// <summary>
    /// Drawing order (0 = bottom of the z-order).
    /// </summary>
    public ushort DrawingZOrder
    {
      get
      {
        return m_usZOrder;
      }
      set
      {
        if( value != m_usZOrder )
        {
          m_usZOrder = value;
        }
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
        return DEF_RECORD_SIZE;
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
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartChartFormatRecord()
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
    public  ChartChartFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartChartFormatRecord( int iReserve )
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

      m_iReserved0 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iReserved1 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iReserved2 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iReserved3 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bIsVaryColor = provider.ReadBit( iOffset, 0 );
      iOffset += 2;

      m_usZOrder = provider.ReadUInt16( iOffset );
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
      // Reserved values must be set to 0.
      m_iReserved0 = m_iReserved1 = m_iReserved2 = m_iReserved3 = 0;

      m_usOptions &= 0x01;

      m_iLength = GetStoreSize( version );

      provider.WriteInt32( iOffset, m_iReserved0 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iReserved1 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iReserved2 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iReserved3 );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bIsVaryColor, 0 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usZOrder );
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion

    internal bool EqualsWithoutOrder( ChartChartFormatRecord chartFormatRecord )
    {
      return ( m_usOptions == chartFormatRecord.m_usOptions &&
        m_bIsVaryColor == chartFormatRecord.m_bIsVaryColor );
    }
  }
}