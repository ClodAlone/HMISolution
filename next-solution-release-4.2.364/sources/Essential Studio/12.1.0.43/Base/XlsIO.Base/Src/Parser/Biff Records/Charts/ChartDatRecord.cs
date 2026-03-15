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
  /// This record stores options for the chart data table.
  /// </summary>
  [ Biff( TBIFFRecord.ChartDat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartDatRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DefaultRecordSize = ExcelConstants.ShortSize;
    #endregion

    #region Class members
    /// <summary>
    /// Holder of record options.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True if data table has horizontal borders.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bHasHorizontalBorders;
    /// <summary>
    /// True if data table has vertical borders.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bHasVerticalBorders;
    /// <summary>
    /// True if data table has a border.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bHasBorders;
    /// <summary>
    /// True if data table shows series keys.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bShowSeriesKeys;
    #endregion

    #region Class properties
    /// <summary>
    /// Holds record flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True if data table has horizontal borders.
    /// </summary>
    public bool HasHorizontalBorders
    {
      get
      {
        return m_bHasHorizontalBorders;
      }
      set
      {
        m_bHasHorizontalBorders = value;
      }
    }
    /// <summary>
    /// True if data table has vertical borders.
    /// </summary>
    public bool HasVerticalBorders
    {
      get
      {
        return m_bHasVerticalBorders;
      }
      set
      {
        m_bHasVerticalBorders = value;
      }
    }
    /// <summary>
    /// True if data table has a border.
    /// </summary>
    public bool HasBorders
    {
      get
      {
        return m_bHasBorders;
      }
      set
      {
        m_bHasBorders = value;
      }
    }
    /// <summary>
    /// True if data table shows series keys.
    /// </summary>
    public bool ShowSeriesKeys
    {
      get
      {
        return m_bShowSeriesKeys;
      }
      set
      {
        m_bShowSeriesKeys = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartDatRecord()
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
    public  ChartDatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartDatRecord( int iReserve )
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
      m_bHasHorizontalBorders = provider.ReadBit( iOffset + 0, 0 );
      m_bHasVerticalBorders = provider.ReadBit( iOffset + 0, 1 );
      m_bHasBorders = provider.ReadBit( iOffset + 0, 2 );
      m_bShowSeriesKeys = provider.ReadBit( iOffset + 0, 3 );
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
      m_usOptions &= 0x0f;

      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bHasHorizontalBorders, 0 );
      provider.WriteBit( iOffset + 0, m_bHasVerticalBorders, 1 );
      provider.WriteBit( iOffset + 0, m_bHasBorders, 2 );
      provider.WriteBit( iOffset + 0, m_bShowSeriesKeys, 3 );

      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}