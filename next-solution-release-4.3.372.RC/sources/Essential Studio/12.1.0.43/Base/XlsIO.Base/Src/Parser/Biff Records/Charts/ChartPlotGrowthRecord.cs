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
  /// This record stores scale factors for font scaling.
  /// </summary>
  [ Biff( TBIFFRecord.ChartPlotGrowth ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartPlotGrowthRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Horizontal growth of plot area for font scaling.
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private uint m_uiHorzGrowth = 65536;
    /// <summary>
    /// Vertical growth of plot area for font scaling.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiVertGrowth = 65536;
    #endregion

    #region Class properties
    /// <summary>
    /// Horizontal growth of plot area for font scaling.
    /// </summary>
    public uint HorzGrowth
    {
      get
      {
        return m_uiHorzGrowth;
      }
      set
      {
        m_uiHorzGrowth = value;
      }
    }
    /// <summary>
    /// Vertical growth of plot area for font scaling.
    /// </summary>
    public uint VertGrowth
    {
      get
      {
        return m_uiVertGrowth;
      }
      set
      {
        m_uiVertGrowth = value;
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
    public  ChartPlotGrowthRecord()
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
    public  ChartPlotGrowthRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartPlotGrowthRecord( int iReserve )
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

      m_uiHorzGrowth = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiVertGrowth = provider.ReadUInt32( iOffset );
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
      m_iLength = GetStoreSize( version );

      provider.WriteUInt32( iOffset, m_uiHorzGrowth );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiVertGrowth );

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
