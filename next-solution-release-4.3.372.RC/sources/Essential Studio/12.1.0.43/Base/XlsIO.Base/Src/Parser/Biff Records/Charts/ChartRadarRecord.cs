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
  /// This record defines a radar chart group.
  /// </summary>
  [ Biff( TBIFFRecord.ChartRadar ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ChartRadarRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions = 0;
    /// <summary>
    /// True if the chart contains radar axis labels.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bRadarAxisLabel;
    /// <summary>
    /// True if this radar series has a shadow.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bHasShadow;
    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usReserved = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True if the chart contains radar axis labels.
    /// </summary>
    public bool IsRadarAxisLabel
    {
      get
      {
        return m_bRadarAxisLabel;
      }
      set
      {
        m_bRadarAxisLabel = value;
      }
    }
    /// <summary>
    /// True if this radar series has a shadow.
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
    /// Field reserved by Microsoft.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
      set
      {
        m_usReserved = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
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
    public  ChartRadarRecord()
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
    public  ChartRadarRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartRadarRecord( int iReserve )
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
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bRadarAxisLabel = provider.ReadBit( iOffset + 0, 0 );
      m_bHasShadow = provider.ReadBit( iOffset + 0, 1 );
      m_usReserved = provider.ReadUInt16( iOffset + 2 );
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
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bRadarAxisLabel, 0 );
      provider.WriteBit( iOffset + 0, m_bHasShadow, 1 );
      provider.WriteUInt16( iOffset + 2, m_usReserved );
      m_iLength = 4;
    }
    #endregion
  }
}
