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
  /// This record specifies chart sheet properties.
  /// </summary>
  [ Biff( TBIFFRecord.ChartShtprops ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartShtpropsRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 4;
    /// <summary>
    /// Represents minimum record size.
    /// </summary>
    public const int DEF_MIN_RECORD_SIZE = 3;
    #endregion

    #region Class members
    /// <summary>
    /// Property flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFlags = 0;
    /// <summary>
    /// True if chart type has been manually formatted (changed from the default).
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bManSerAlloc;
    /// <summary>
    /// True to plot only visible cells.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bPlotVisOnly = true;
    /// <summary>
    /// True not to size chart with window.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bNotSizeWith = true;
    /// <summary>
    /// False to use default plot area dimensions;
    /// True if POS record describes plot-area dimensions.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bManPlotArea = true;
    /// <summary>
    /// True if user has modified chart enough that fManPlotArea should be set to 0.
    /// </summary>
    [ BiffRecordPos( 0, 4, TFieldType.Bit ) ]
    private bool m_bAlwaysAutoPlotArea;
    /// <summary>
    /// Empty cells plotted as:
    /// 0 = not plotted
    /// 1 = zero
    /// 2 = interpolated
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte m_plotBlank;
    /// <summary>
    /// This field is not used.
    /// </summary>
    [ BiffRecordPos( 3, 1 ) ]
    private byte m_notUsed = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Property flags. Read-only.
    /// </summary>
    public ushort Flags
    {
      get
      {
        return m_usFlags;
      }
    }

    /// <summary>
    /// True if chart type has been manually formatted (changed from the default).
    /// </summary>
    public bool IsManSerAlloc
    {
      get
      {
        return m_bManSerAlloc;
      }
      set
      {
        m_bManSerAlloc = value;
      }
    }
    /// <summary>
    /// True to plot visible cells only.
    /// </summary>
    public bool IsPlotVisOnly
    {
      get
      {
        return m_bPlotVisOnly;
      }
      set
      {
        m_bPlotVisOnly = value;
      }
    }
    /// <summary>
    /// True not to size chart with window.
    /// </summary>
    public bool IsNotSizeWith
    {
      get
      {
        return m_bNotSizeWith;
      }
      set
      {
        m_bNotSizeWith = value;
      }
    }
    /// <summary>
    /// False to use default plot area dimensions;
    /// True if POS record describes plot-area dimensions.
    /// </summary>
    public bool IsManPlotArea
    {
      get
      {
        return m_bManPlotArea;
      }
      set
      {
        m_bManPlotArea = value;
      }
    }
    /// <summary>
    /// True if user has modified chart enough that fManPlotArea should be set to 0.
    /// </summary>
    public bool IsAlwaysAutoPlotArea
    {
      get
      {
        return m_bAlwaysAutoPlotArea;
      }
      set
      {
        m_bAlwaysAutoPlotArea = value;
      }
    }
    /// <summary>
    /// Empty cells plotted as:
    /// 0 = not plotted
    /// 1 = zero
    /// 2 = interpolated
    /// </summary>
    public ExcelChartPlotEmpty PlotBlank
    {
      get
      {
        return ( ExcelChartPlotEmpty )m_plotBlank;
      }
      set
      {
        m_plotBlank = ( byte )value;
      }
    }
    /// <summary>
    /// Reserved by Microsoft.
    /// </summary>
    public byte Reserved
    {
      get
      {
        return m_notUsed;
      }
      set
      {
        m_notUsed = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_MIN_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Maximum possible size of the record.
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
    public  ChartShtpropsRecord()
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
    public  ChartShtpropsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartShtpropsRecord( int iReserve )
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
      // TODO: check correctness of data
      m_usFlags = provider.ReadUInt16( iOffset );

      m_bManSerAlloc = provider.ReadBit( iOffset, 0 );
      m_bPlotVisOnly = provider.ReadBit( iOffset, 1 );
      m_bNotSizeWith = provider.ReadBit( iOffset, 2 );
      m_bManPlotArea = provider.ReadBit( iOffset, 3 );
      m_bAlwaysAutoPlotArea = provider.ReadBit( iOffset, 4 );

      m_plotBlank = provider.ReadByte( iOffset + 2 );

      if( iLength > 3 )
        m_notUsed = provider.ReadByte( iOffset + 3 );

      //AutoExtractFields();
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteUInt16( iOffset, m_usFlags );

      provider.WriteBit( iOffset, m_bManSerAlloc, 0 );
      provider.WriteBit( iOffset, m_bPlotVisOnly, 1 );
      provider.WriteBit( iOffset, m_bNotSizeWith, 2 );
      provider.WriteBit( iOffset, m_bManPlotArea, 3 );
      provider.WriteBit( iOffset, m_bAlwaysAutoPlotArea, 4 );
      iOffset += 2;

      provider.WriteByte( iOffset, m_plotBlank );
      iOffset++;

      provider.WriteByte( iOffset, m_notUsed );
      //iOffset++;

      m_iLength = GetStoreSize( version );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}
