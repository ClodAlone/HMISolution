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
  /// The FRAME record defines the border that is present around a 
  /// displayed label as a rectangle. A displayed label can include 
  /// the chart title, the legend (if not a regular rectangle), a 
  /// category name, or a value amount.
  /// </summary>
  [ Biff( TBIFFRecord.ChartFrame ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartFrameRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// 0 = regular rectangle/no border
    /// 1�3 (reserved)
    /// 4 = rectangle with shadow
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRectStyle;
    /// <summary>
    /// Record flags holder.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions = 0;
    /// <summary>
    /// Microsoft Excel calculates size.
    /// </summary>
    [ BiffRecordPos( 2, 0, TFieldType.Bit ) ]
    private bool m_bAutoSize = true;
    /// <summary>
    /// Microsoft Excel calculates position.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.Bit ) ]
    private bool m_bAutoPosition = true;
    #endregion

    #region Class properties
    /// <summary>
    /// Record flags holder.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// 0 = regular rectangle/no border
    /// 1�3 (reserved)
    /// 4 = rectangle with shadow
    /// </summary>
    public ExcelRectangleStyle Rectangle
    {
      get
      {
        return ( ExcelRectangleStyle )m_usRectStyle;
      }
      set
      {
        m_usRectStyle = ( ushort )value;
      }
    }
    /// <summary>
    /// Microsoft Excel calculates size.
    /// </summary>
    public bool AutoSize
    {
      get
      {
        return m_bAutoSize;
      }
      set
      {
        m_bAutoSize = value;
      }
    }
    /// <summary>
    /// Microsoft Excel calculates position.
    /// </summary>
    public bool AutoPosition
    {
      get
      {
        return m_bAutoPosition;
      }
      set
      {
        m_bAutoPosition = value;
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
    public  ChartFrameRecord()
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
    public  ChartFrameRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartFrameRecord( int iReserve )
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

      m_usRectStyle = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bAutoSize = provider.ReadBit( iOffset, 0 );
      m_bAutoPosition = provider.ReadBit( iOffset, 1 );
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

      provider.WriteUInt16( iOffset, m_usRectStyle );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bAutoSize, 0 );
      provider.WriteBit( iOffset, m_bAutoPosition, 1 );
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
