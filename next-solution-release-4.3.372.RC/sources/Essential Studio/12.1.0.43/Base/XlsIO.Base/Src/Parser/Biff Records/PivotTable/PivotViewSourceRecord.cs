#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record contains an integer that defines the data source for a PivotTable.
  /// </summary>
  [ Biff( TBIFFRecord.PivotViewSource ) ]
  [ CLSCompliant( false ) ]
  public class PivotViewSourceRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 2;
    /// <summary>
    /// Possible data sources.
    /// </summary>
    public enum DataSourceTypes
    {
      /// <summary>
      /// Microsoft Excel list or database.
      /// </summary>
      MSExcelOrDB = 1,
      /// <summary>
      /// External data source (Microsoft Query).
      /// </summary>
      External = 2,
      /// <summary>
      /// Multiple consolidation ranges.
      /// </summary>
      ConsolidationRanges = 4,
      /// <summary>
      /// Another PivotTable.
      /// </summary>
      PivotTable = 8,
      /// <summary>
      /// A Scenario Manager summary report.
      /// </summary>
      ScenarioManager = 16,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Data source.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usDataSource;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotViewSourceRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  PivotViewSourceRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotViewSourceRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Data source.
    /// </summary>
    public DataSourceTypes DataSource
    {
      get
      {
        return ( DataSourceTypes )m_usDataSource;
      }
      set
      {
        m_usDataSource = ( ushort )value;
      }
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
      m_usDataSource = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usDataSource );
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
