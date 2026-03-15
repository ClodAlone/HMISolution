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
  /// This record defines the appearance of a line, such as an axis line or border.
  /// </summary>
  [ Biff( TBIFFRecord.ChartLineFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartLineFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 12;
    #endregion

    #region Class members
    /// <summary>
    /// Color of line; RGB value high byte must be set to zero.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int    m_rgbColor;
    /// <summary>
    /// Pattern of line:
    /// 0 = solid
    /// 1 = dash
    /// 2 = dot
    /// 3 = dash-dot
    /// 4 = dash dot-dot
    /// 5 = none
    /// 6 = dark gray pattern
    /// 7 = medium gray pattern
    /// 8 = light gray pattern
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usLinePattern;
    /// <summary>
    /// Weight of line:
    /// �1 or 0xffff = hairline
    /// 0 = narrow (single)
    /// 1 = medium (double)
    /// 2 = wide (triple)
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usLineWeight;
    /// <summary>
    /// Format flags holder.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Automatic format.
    /// </summary>
    [ BiffRecordPos( 8, 0, TFieldType.Bit ) ]
    private bool   m_bAutoFormat = true;
    /// <summary>
    /// True to draw tick labels on this axis.
    /// </summary>
    [ BiffRecordPos( 8, 2, TFieldType.Bit ) ]
    private bool   m_bDrawTickLabels;
    /// <summary>
    /// True to draw line by custom style.
    /// </summary>
    [ BiffRecordPos( 8, 3, TFieldType.Bit ) ]
    private bool m_bIsAutoLineColor = true;
    /// <summary>
    /// Index to color of line.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usColorIndex;
    #endregion

    #region Class properties
    /// <summary>
    /// Color of line; RGB value high byte must be set to zero.
    /// </summary>
    public int LineColor
    {
      get
      {
        return m_rgbColor;
      }
      set
      {
        if( value != m_rgbColor )
        {
          m_rgbColor = value;
        }
      }
    }
    /// <summary>
    /// Line pattern.
    /// </summary>
    public ExcelChartLinePattern LinePattern
    {
      get
      {
        return ( ExcelChartLinePattern )m_usLinePattern;
      }
      set
      {
        m_usLinePattern = ( ushort )value;
      }
    }
    /// <summary>
    /// Weight of line.
    /// </summary>
    public ExcelChartLineWeight LineWeight
    {
      get
      {
        return ( ExcelChartLineWeight )m_usLineWeight;
      }
      set
      {
        m_usLineWeight = ( ushort )value;
      }
    }
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    /// <summary>
    /// If true - default format; otherwise custom.
    /// </summary>
    public bool AutoFormat
    {
      get
      {
        return m_bAutoFormat;
      }
      set
      {
        m_bAutoFormat = value;
      }
    }
    /// <summary>
    /// True to draw tick labels on this axis.
    /// </summary>
    public bool DrawTickLabels
    {
      get
      {
        return m_bDrawTickLabels;
      }
      set
      {
        m_bDrawTickLabels = value;
      }
    }
    /// <summary>
    /// Custom format for line color.
    /// </summary>
    public bool IsAutoLineColor
    {
      get
      {
        return m_bIsAutoLineColor;
      }
      set
      {
        m_bIsAutoLineColor = value;
      }
    }
    /// <summary>
    /// Line color index..
    /// </summary>
    public ushort ColorIndex
    {
      get
      {
        return m_usColorIndex;
      }
      set
      {
        m_usColorIndex = value;
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
    public  ChartLineFormatRecord()
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
    public  ChartLineFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartLineFormatRecord( int iReserve )
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

      //AutoExtractFields();
      m_rgbColor = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_usLinePattern = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLineWeight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bAutoFormat = provider.ReadBit( iOffset, 0 );
      m_bDrawTickLabels = provider.ReadBit( iOffset, 2 );
      m_bIsAutoLineColor = provider.ReadBit( iOffset, 3 );
      iOffset += 2;
      
      m_usColorIndex = provider.ReadUInt16( iOffset );
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
      //m_usOptions &= 0x05;
      //m_usOptions &= 0xD;

      provider.WriteInt32( iOffset, m_rgbColor );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usLinePattern );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLineWeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bAutoFormat, 0 );
      provider.WriteBit( iOffset, m_bDrawTickLabels, 2 );
      provider.WriteBit( iOffset, m_bIsAutoLineColor, 3 );
      iOffset += 2;
      
      provider.WriteUInt16( iOffset, m_usColorIndex );
      m_iLength = GetStoreSize( version );
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