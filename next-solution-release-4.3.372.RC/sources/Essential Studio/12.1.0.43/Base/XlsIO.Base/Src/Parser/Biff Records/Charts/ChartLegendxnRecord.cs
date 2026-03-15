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
  /// This record specifies information about a legend entry which has 
  /// been changed from the default legend-entry settings.
  /// </summary>
  [ Biff( TBIFFRecord.ChartLegendxn ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartLegendxnRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DefaultRecordSize = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Legend-entry index.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usLegendEntityIndex = 65535;
    /// <summary>
    /// Record flags holder.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True if the legend entry has been deleted.
    /// </summary>
    [ BiffRecordPos( 2, 0, TFieldType.Bit ) ]
    private bool m_bIsDeleted;
    /// <summary>
    /// True if the legend entry has been formatted.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.Bit ) ]
    private bool m_bIsFormatted;
    #endregion

    #region Class properties
    /// <summary>
    /// Legend-entry index.
    /// </summary>
    public ushort LegendEntityIndex
    {
      get
      {
        return m_usLegendEntityIndex;
      }
      set
      {
        if( value != m_usLegendEntityIndex )
        {
          m_usLegendEntityIndex = value;
        }
      }
    }
    /// <summary>
    /// Options bit flags holder.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// True if the legend entry has been deleted.
    /// </summary>
    public bool IsDeleted
    {
      get
      {
        return m_bIsDeleted;
      }
      set
      {
        m_bIsDeleted = value;
      }
    }
    /// <summary>
    /// True if the legend entry has been formatted.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        return m_bIsFormatted;
      }
      set
      {
        m_bIsFormatted = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartLegendxnRecord()
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
    public  ChartLegendxnRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartLegendxnRecord( int iReserve )
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
      m_usLegendEntityIndex = provider.ReadUInt16( iOffset + 0 );
      m_usOptions = provider.ReadUInt16( iOffset + 2 );
      m_bIsDeleted = provider.ReadBit( iOffset + 2, 0 );
      m_bIsFormatted = provider.ReadBit( iOffset + 2, 1 );
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
      m_usOptions &= 0x03;

      provider.WriteUInt16( iOffset + 0, m_usLegendEntityIndex );
      provider.WriteUInt16( iOffset + 2, m_usOptions );
      provider.WriteBit( iOffset + 2, m_bIsDeleted, 0 );
      provider.WriteBit( iOffset + 2, m_bIsFormatted, 1 );
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