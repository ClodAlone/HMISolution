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
  /// This record specifies the location and size of the chart axes, 
  /// in units of 1/4000 of the chart area.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAxisParent ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAxisParentRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 18;
    #endregion

    #region Class members
    /// <summary>
    /// Axis index (0 = main, 1 = secondary).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usAxisIndex;
    /// <summary>
    /// X coordinate of top left corner.
    /// </summary>
    [ BiffRecordPos( 2, 4, true ) ]
    private int m_iTopLeftX;
    /// <summary>
    /// Y coordinate of top left corner.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_iTopLeftY;
    /// <summary>
    /// Length of x axis.
    /// </summary>
    [ BiffRecordPos( 10, 4, true ) ]
    private int m_iXLength;
    /// <summary>
    /// Length of y axis.
    /// </summary>
    [ BiffRecordPos( 14, 4, true ) ]
    private int m_iYLength;
    #endregion

    #region Class properties
    /// <summary>
    /// Axis index (0 = main, 1 = secondary).
    /// </summary>
    public ushort AxesIndex
    {
      get
      {
        return m_usAxisIndex;
      }
      set
      {
        if( value != m_usAxisIndex )
        {
          m_usAxisIndex = value;
        }
      }
    }

    /// <summary>
    /// X coordinate of top left corner.
    /// </summary>
    public int TopLeftX
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
    /// Y coordinate of top left corner.
    /// </summary>
    public int TopLeftY
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
    /// Length of x axis.
    /// </summary>
    public int XAxisLength
    {
      get
      {
        return m_iXLength;
      }
      set
      {
        if( value != m_iXLength )
        {
          m_iXLength = value;
        }
      }
    }

    /// <summary>
    /// Length of y axis.
    /// </summary>
    public int YAxisLength
    {
      get
      {
        return m_iYLength;
      }
      set
      {
        if( value != m_iYLength )
        {
          m_iYLength = value;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAxisParentRecord()
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
    public  ChartAxisParentRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAxisParentRecord( int iReserve )
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

      m_usAxisIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_iTopLeftX = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iTopLeftY = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iXLength = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iYLength = provider.ReadInt32( iOffset );
      iOffset += 4;
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

      provider.WriteUInt16( iOffset, m_usAxisIndex );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iTopLeftX );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iTopLeftY );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iXLength );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iYLength );
      iOffset += 4;
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