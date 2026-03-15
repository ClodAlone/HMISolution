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
  /// This record defines manual position information for the main-axis
  /// plot area, legend, and attached text (data labels, axis labels,
  /// and chart title). The record data depends on the record's use, 
  /// as shown in the following sections.
  /// </summary>
  [ Biff( TBIFFRecord.ChartPos ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartPosRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 20;
    #endregion

    #region Class members
    /// <summary>
    /// For plot area and text must be 2,
    /// For legend must be 5.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usTopLeft;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usBottomRight = 2;
    /// <summary>
    /// X coordinate of the top left corner.
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iX1;
    /// <summary>
    /// Y coordinate of the top left corner.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iY1;
    /// <summary>
    /// X coordinate of the bottom right corner.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iX2;
    /// <summary>
    /// Y coordinate of the bottom right corner.
    /// </summary>
    [ BiffRecordPos( 16, 4, true ) ]
    private int m_iY2;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public ushort TopLeft
    {
      get
      {
        return m_usTopLeft;
      }
      set
      {
        m_usTopLeft = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort BottomRight
    {
      get
      {
        return m_usBottomRight;
      }
      set
      {
        m_usBottomRight = value;
      }
    }
    
    /// <summary>
    /// X coordinate of the top left corner.
    /// </summary>
    public int X1
    {
      get
      {
        return m_iX1;
      }
      set
      {
        m_iX1 = value;
      }
    }
    /// <summary>
    /// Y coordinate of the top left corner.
    /// </summary>
    public int Y1
    {
      get
      {
        return m_iY1;
      }
      set
      {
        m_iY1 = value;
      }
    }
    /// <summary>
    /// X coordinate of the bottom right corner.
    /// </summary>
    public int X2
    {
      get
      {
        return m_iX2;
      }
      set
      {
        m_iX2 = value;
      }
    }
    /// <summary>
    /// Y coordinate of the bottom right corner.
    /// </summary>
    public int Y2
    {
      get
      {
        return m_iY2;
      }
      set
      {
        m_iY2 = value;
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
    public  ChartPosRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / Initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  ChartPosRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartPosRecord( int iReserve )
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

      m_usTopLeft = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usBottomRight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_iX1 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iY1 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iX2 = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iY2 = provider.ReadInt32( iOffset );
      //iOffset += 4;
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

      provider.WriteUInt16( iOffset, m_usTopLeft );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBottomRight );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iX1 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iY1 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iX2 );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iY2 );
      //iOffset += 4;
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
