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
  /// The LEGEND record defines the location of the legend on the 
  /// display and its overall size. The displayed legend contains 
  /// all series on the chart.
  /// </summary>
  [ Biff( TBIFFRecord.ChartLegend ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartLegendRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DefaultRecordSize = 20;
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
    /// <summary>
    /// Type:
    /// 0 = bottom
    /// 1 = corner
    /// 2 = top
    /// 3 = right
    /// 4 = left
    /// 7 = not docked or inside the plot area
    /// </summary>
    [ BiffRecordPos( 16, 1 ) ]
    private byte m_wType = 3;
    /// <summary>
    /// Spacing:
    /// 0 = close
    /// 1 = medium
    /// 2 = open
    /// </summary>
    [ BiffRecordPos( 17, 1 ) ]
    private byte m_wSpacing = 1;
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    [ BiffRecordPos( 18, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Automatic positioning (True if legend is docked).
    /// </summary>
    [ BiffRecordPos( 18, 0, TFieldType.Bit ) ]
    private bool m_bAutoPosition = true;
    /// <summary>
    /// Automatic series distribution (True in Microsoft Excel 5.0).
    /// </summary>
    [ BiffRecordPos( 18, 1, TFieldType.Bit ) ]
    private bool m_bAutoSeries = true;
    /// <summary>
    /// X positioning is automatic.
    /// </summary>
    [ BiffRecordPos( 18, 2, TFieldType.Bit ) ]
    private bool m_bAutoPosX = true;
    /// <summary>
    /// Y positioning is automatic.
    /// </summary>
    [ BiffRecordPos( 18, 3, TFieldType.Bit ) ]
    private bool m_bAutoPosY = true;
    /// <summary>
    /// True if vertical legend (a single column of entries);
    /// False if horizontal legend (multiple columns of entries).
    /// Manual-sized legends always have this bit set to False.
    /// </summary>
    [ BiffRecordPos( 18, 4, TFieldType.Bit ) ]
    private bool m_bIsVerticalLegend = true;
    /// <summary>
    /// True if the chart contains a data table.
    /// </summary>
    [ BiffRecordPos( 18, 5, TFieldType.Bit ) ]
    private bool m_bContainsDataTable;
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
    /// Type:
    /// 0 = bottom
    /// 1 = corner
    /// 2 = top
    /// 3 = right
    /// 4 = left
    /// 7 = not docked or inside the plot area
    /// </summary>
    public ExcelLegendPosition Position
    {
      get
      {
        return ( ExcelLegendPosition )m_wType;
      }
      set
      {
        m_wType = ( byte )value;
      }
    }

    /// <summary>
    /// Spacing:
    /// 0 = close
    /// 1 = medium
    /// 2 = open
    /// </summary>
    public ExcelLegendSpacing Spacing
    {
      get
      {
        return ( ExcelLegendSpacing )m_wSpacing;
      }
      set
      {
        m_wSpacing = ( byte )value;
      }
    }
    /// <summary>
    /// Automatic positioning (True if legend is docked).
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
    /// Automatic series distribution (True in Microsoft Excel 5.0).
    /// </summary>
    public bool AutoSeries
    {
      get
      {
        return m_bAutoSeries;
      }
      set
      {
        m_bAutoSeries = value;
      }
    }
    /// <summary>
    /// X positioning is automatic.
    /// </summary>
    public bool AutoPositionX
    {
      get
      {
        return m_bAutoPosX;
      }
      set
      {
        m_bAutoPosX = value;
      }
    }
    /// <summary>
    /// Y positioning is automatic.
    /// </summary>
    public bool AutoPositionY
    {
      get
      {
        return m_bAutoPosY;
      }
      set
      {
        m_bAutoPosY = value;
      }
    }
    /// <summary>
    /// True if vertical legend (a single column of entries);
    /// False if horizontal legend (multiple columns of entries).
    /// Manual-sized legends always have this bit set to False.
    /// </summary>
    public bool IsVerticalLegend
    {
      get
      {
        return m_bIsVerticalLegend;
      }
      set
      {
        m_bIsVerticalLegend = value;
      }
    }
    /// <summary>
    /// True if chart contains data table.
    /// </summary>
    public bool ContainsDataTable
    {
      get
      {
        return m_bContainsDataTable;
      }
      set
      {
        m_bContainsDataTable = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartLegendRecord()
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
    public  ChartLegendRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartLegendRecord( int iReserve )
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
      m_iTopLeftX = provider.ReadInt32( iOffset + 0 );
      m_iTopLeftY = provider.ReadInt32( iOffset + 4 );
      m_iWidth = provider.ReadInt32( iOffset + 8 );
      m_iHeight = provider.ReadInt32( iOffset + 12 );
      m_wType = provider.ReadByte( iOffset + 16 );
      m_wSpacing = provider.ReadByte( iOffset + 17 );
      m_usOptions = provider.ReadUInt16( iOffset + 18 );
      m_bAutoPosition = provider.ReadBit( iOffset + 18, 0 );
      m_bAutoSeries = provider.ReadBit( iOffset + 18, 1 );
      m_bAutoPosX = provider.ReadBit( iOffset + 18, 2 );
      m_bAutoPosY = provider.ReadBit( iOffset + 18, 3 );
      m_bIsVerticalLegend = provider.ReadBit( iOffset + 18, 4 );
      m_bContainsDataTable = provider.ReadBit( iOffset + 18, 5 );
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
      m_usOptions &= 0x3F;

      provider.WriteInt32( iOffset + 0, m_iTopLeftX );
      provider.WriteInt32( iOffset + 4, m_iTopLeftY );
      provider.WriteInt32( iOffset + 8, m_iWidth );
      provider.WriteInt32( iOffset + 12, m_iHeight );
      provider.WriteByte( iOffset + 16, m_wType );
      provider.WriteByte( iOffset + 17, m_wSpacing );
      provider.WriteUInt16( iOffset + 18, m_usOptions );
      provider.WriteBit( iOffset + 18, m_bAutoPosition, 0 );
      provider.WriteBit( iOffset + 18, m_bAutoSeries, 1 );
      provider.WriteBit( iOffset + 18, m_bAutoPosX, 2 );
      provider.WriteBit( iOffset + 18, m_bAutoPosY, 3 );
      provider.WriteBit( iOffset + 18, m_bIsVerticalLegend, 4 );
      provider.WriteBit( iOffset + 18, m_bContainsDataTable, 5 );

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