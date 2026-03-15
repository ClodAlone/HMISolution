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

using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record is subordinate to the second CHARTFORMAT (overlay)
  /// record in a file and defines the series that are displayed as
  /// the overlay to the main chart. The first CHARTFORMAT (main chart)
  /// record in a file does not require a SERIESLIST record because all
  /// series, except those specified for the overlay, are included in
  /// the main chart.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSeriesList ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ChartSeriesListRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Minimum size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Count of series.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usCount;
    /// <summary>
    /// List of series numbers (words).
    /// </summary>
    private ushort[] m_arrSeries;
    #endregion

    #region Class properties
    /// <summary>
    /// Count of series. Read-only.
    /// </summary>
    public ushort SeriesCount
    {
      get
      {
        return m_usCount;
      }
    }
    /// <summary>
    /// List of series numbers (words).
    /// </summary>
    public ushort[] Series
    {
      get
      {
        return m_arrSeries;
      }
      set
      {
        m_arrSeries = value;
        m_usCount = (ushort)(( value != null ) ? value.Length : 0);
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
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartSeriesListRecord()
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
    public  ChartSeriesListRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSeriesListRecord( int iReserve )
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

      m_usCount = provider.ReadUInt16( iOffset + 0 );
      iOffset += ExcelConstants.ShortSize;

      if( m_usCount * ExcelConstants.ShortSize + ExcelConstants.ShortSize != m_iLength )
        throw new WrongBiffRecordDataException( "ChartListRecord" );

      m_arrSeries = new ushort[ m_usCount ];

      for( int i = 0; i < m_usCount; i++, iOffset += ExcelConstants.ShortSize )
      {
        m_arrSeries[ i ] = provider.ReadUInt16( iOffset );
      }
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
      provider.WriteUInt16( iOffset + 0, m_usCount );
      m_iLength = ExcelConstants.ShortSize;

      for( int i = 0; i < m_usCount; i++, m_iLength += ExcelConstants.ShortSize )
      {
        provider.WriteUInt16( iOffset + m_iLength, m_arrSeries[ i ] );
      }
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ExcelConstants.ShortSize + ExcelConstants.ShortSize * m_usCount;
    }
    public static bool operator ==( ChartSeriesListRecord record1, ChartSeriesListRecord record2 )
    {
      bool bFirstNull = object.Equals( record1, null );
      bool bSecondNull = object.Equals( record2, null );

      if( bFirstNull && bSecondNull )
        return true;

      if( bFirstNull || bSecondNull )
        return false;

      bool result = ( record1.m_usCount == record2.m_usCount );

      for( int i = 0, len = record1.m_usCount; i < len && result; i++ )
      {
        result = record1.m_arrSeries[ i ] == record2.m_arrSeries[ i ];
      }

      return result;
    }
    public static bool operator !=( ChartSeriesListRecord record1, ChartSeriesListRecord record2 )
    {
      return !( record1 == record2 );
    }
    #endregion

    #region ICloneable method
    /// <summary>
    /// Clones current record.
    /// </summary>
    /// <returns>Returns cloned record.</returns>
    public override object Clone()
    {
      ChartSeriesListRecord result = ( ChartSeriesListRecord )base.Clone();

      result.m_arrSeries = CloneUtils.CloneUshortArray( m_arrSeries );

      return result;
    }
    #endregion
  }
}
