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
  /// The CHART record marks the start of the chart data substream in the 
  /// workbook BIFF stream. This record defines the location of the chart 
  /// on the display and its overall size. The X and Y fields define the 
  /// position of the upper-left corner of the bounding rectangle that 
  /// encompasses the chart. The position of the chart is referenced to the page.
  /// The Width and Height fields define the overall size (the bounding rectangle) of 
  /// the chart, including title, pointing arrows, axis labels, etc.
  /// The position and size are specified in points (1/72 inch), using a fixed 
  /// point format (two bytes integer, two bytes fraction).
  /// </summary>
  [ Biff( TBIFFRecord.ChartChart ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartChartRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 16;
    #endregion

    #region Class members
    /// <summary>
    /// X-position of upper-left corner.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iTopLeftX;
    /// <summary>
    /// Y-position of upper-left corner.
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iTopLeftY;
    /// <summary>
    /// X-size.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iWidth;
    /// <summary>
    /// Y-size.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iHeight;
    #endregion

    #region Class properties
    /// <summary>
    /// X-position of upper-left corner.
    /// </summary>
    public int X
    {
      get
      {
        return m_iTopLeftX;
      }
      set
      {
        if( value != m_iTopLeftX )
        {
          m_iTopLeftX = value;
        }
      }
    }

    /// <summary>
    /// Y-position of upper-left corner.
    /// </summary>
    public int Y
    {
      get
      {
        return m_iTopLeftY;
      }
      set
      {
        if( value != m_iTopLeftY )
        {
          m_iTopLeftY = value;
        }
      }
    }

    /// <summary>
    /// X-size.
    /// (1/72 inch), used a fixed point format (two bytes integer, two bytes fraction).
    /// </summary>
    public int Width
    {
      get
      {
        return m_iWidth;
      }
      set
      {
        if( value != m_iWidth )
        {
          m_iWidth = value;
        }
      }
    }

    /// <summary>
    /// Y-size. 
    /// (1/72 inch), used a fixed point format (two bytes integer, two bytes fraction).
    /// </summary>
    public int Height
    {
      get
      {
        return m_iHeight;
      }
      set
      {
        if( value != m_iHeight )
        {
          m_iHeight = value;
        }
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
    public  ChartChartRecord()
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
    public  ChartChartRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartChartRecord( int iReserve )
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

      m_iTopLeftX = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iTopLeftY = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iWidth = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iHeight = provider.ReadInt32( iOffset );
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

      provider.WriteInt32( iOffset, m_iTopLeftX );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iTopLeftY );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iWidth );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iHeight );
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