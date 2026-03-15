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
  /// The DATAFORMAT record contains the zero-based numbers of the data 
  /// point and series. The subordinate records determine the format of 
  /// the series or point defined by the DATAFORMAT record.
  /// </summary>
  [ Biff( TBIFFRecord.ChartDataFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartDataFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Point number (FFFFh means entire series).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usPointNumber;
    /// <summary>
    /// Series index (file relative).
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usSeriesIndex;
    /// <summary>
    /// Series number (as shown in name box -- S1, S2, etc.). This can 
    /// be different from yi if the series order has been changed.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usSeriesNumber;
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True to use Microsoft Excel 4.0 colors for automatic formatting.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bUseXL4Color;
    #endregion

    #region Class properties
    /// <summary>
    /// Point number (FFFFh means entire series).
    /// </summary>
    public ushort PointNumber
    {
      get
      {
        return m_usPointNumber;
      }
      set
      {
        if( value != m_usPointNumber )
        {
          m_usPointNumber = value;
        }
      }
    }
    /// <summary>
    /// Series index (file relative).
    /// </summary>
    public ushort SeriesIndex
    {
      get
      {
        return m_usSeriesIndex;
      }
      set
      {
        if( value != m_usSeriesIndex )
        {
          m_usSeriesIndex = value;
        }
      }
    }

    /// <summary>
    /// Series number (as shown in name box -- S1, S2, etc.). This can 
    /// be different from yi if the series order has been changed.
    /// </summary>
    public ushort SeriesNumber
    {
      get
      {
        return m_usSeriesNumber;
      }
      set
      {
        if( value != m_usSeriesNumber )
        {
          m_usSeriesNumber = value;
        }
      }
    }
    /// <summary>
    /// Options holder.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True to use Microsoft Excel 4.0 colors for automatic formatting.
    /// </summary>
    public bool UserExcel4Colors
    {
      get
      {
        return m_bUseXL4Color;
      }
      set
      {
        m_bUseXL4Color = value;
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
    public  ChartDataFormatRecord()
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
    public  ChartDataFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartDataFormatRecord( int iReserve )
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
      m_usPointNumber = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usSeriesIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usSeriesNumber = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bUseXL4Color = provider.ReadBit( iOffset, 0 );
      //iOffset += 2;
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
      m_usOptions &= 0x01;
      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usPointNumber );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usSeriesIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usSeriesNumber );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bUseXL4Color, 0 );
      //iOffset += 2;
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