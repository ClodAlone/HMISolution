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
  /// This record defines a pie chart group and specifies pie chart options.
  /// </summary>
  [ Biff( TBIFFRecord.ChartPie ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartPieRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DefaultRecordSize = 6;
    #endregion

    #region Class members
    /// <summary>
    /// Angle of the first pie slice expressed in degrees.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usStartAngle;
    /// <summary>
    /// 0 = True pie chart
    /// Non-zero = size of center hole in a donut chart (as a percentage).
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usDonutHoleSize;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True if this pie has a shadow.
    /// </summary>
    [ BiffRecordPos( 4, 0, TFieldType.Bit ) ]
    private bool m_bHasShadow;
    /// <summary>
    /// True to show leader lines to data labels.
    /// </summary>
    [ BiffRecordPos( 4, 1, TFieldType.Bit ) ]
    private bool m_bShowLeaderLines;
    #endregion

    #region Class properties
    /// <summary>
    /// Angle of the first pie slice expressed in degrees.
    /// </summary>
    public ushort StartAngle
    {
      get
      {
        return m_usStartAngle;
      }
      set
      {
        m_usStartAngle = value;
      }
    }
    /// <summary>
    /// 0 = True pie chart
    /// Non-zero = size of center hole in a donut chart (as a percentage).
    /// </summary>
    public ushort DonutHoleSize
    {
      get
      {
        return m_usDonutHoleSize;
      }
      set
      {
        m_usDonutHoleSize = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
      set
      {
        m_usOptions = value;
      }
    }
    /// <summary>
    /// True if this pie has a shadow.
    /// </summary>
    public bool HasShadow
    {
      get
      {
        return m_bHasShadow;
      }
      set
      {
        m_bHasShadow = value;
      }
    }
    /// <summary>
    /// True to show leader lines to data labels.
    /// </summary>
    public bool ShowLeaderLines
    {
      get
      {
        return m_bShowLeaderLines;
      }
      set
      {
        m_bShowLeaderLines = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }
    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartPieRecord()
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
    public  ChartPieRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartPieRecord( int iReserve )
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
      m_usStartAngle = provider.ReadUInt16( iOffset + 0 );
      m_usDonutHoleSize = provider.ReadUInt16( iOffset + 2 );
      m_usOptions = provider.ReadUInt16( iOffset + 4 );
      m_bHasShadow = provider.ReadBit( iOffset + 4, 0 );
      m_bShowLeaderLines = provider.ReadBit( iOffset + 4, 1 );
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
      provider.WriteUInt16( iOffset + 0, m_usStartAngle );
      provider.WriteUInt16( iOffset + 2, m_usDonutHoleSize );
      provider.WriteUInt16( iOffset + 4, m_usOptions );
      provider.WriteBit( iOffset + 4, m_bHasShadow, 0 );
      provider.WriteBit( iOffset + 4, m_bShowLeaderLines, 1 );
      m_iLength = DefaultRecordSize;
    }
    #endregion
  }
}