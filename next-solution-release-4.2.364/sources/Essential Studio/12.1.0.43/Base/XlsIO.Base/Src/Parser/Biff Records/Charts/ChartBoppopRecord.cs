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
  /// This record stores options for a bar of pie or pie of pie chart;
  /// these are two of the pie chart subtypes.
  /// </summary>
  [ Biff( TBIFFRecord.ChartBoppop ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartBoppopRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DefaultRecordSize = 18;
    #endregion

    #region Class members
    /// <summary>
    /// 0 = normal pie chart
    /// 1 = pie of pie chart
    /// 2 = bar of pie chart
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte    m_PieType;
    /// <summary>
    /// Holder of DefaultSplitValue property flag.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte    m_UseDefaultSplit;
    /// <summary>
    /// True to use default split value; otherwise False.
    /// </summary>
    [ BiffRecordPos( 1, 0, TFieldType.Bit ) ]
    private bool    m_bUseDefaultSplit;
    /// <summary>
    /// Split type:
    /// 0 = Position
    /// 1 = Value
    /// 2 = Percent
    /// 3 = Custom
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort  m_usSplitType;
    /// <summary>
    /// For split = 0, which positions should go to the other pie / bar.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort  m_usSplitPos;
    /// <summary>
    /// For split = 2, what percentage should go to the other pie / bar.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort  m_usSplitPercent;
    /// <summary>
    /// Size of the second pie as a percentage of the first.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort  m_usPie2Size;
    /// <summary>
    /// Space between the first pie and the second.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort  m_usGap;
    /// <summary>
    /// For split = 1, what values should go to the other pie / bar.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int     m_uiNumSplitValue;
    /// <summary>
    /// Holder of HasShadow property.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort  m_usHasShadow;
    /// <summary>
    /// True if the second bar / pie has a shadow; otherwise False.
    /// </summary>
    [ BiffRecordPos( 16, 0, TFieldType.Bit ) ]
    private bool    m_bHasShadow;
    /// <summary>
    /// Represents LeaderLines of datalabels
	/// This property is included as the support in enabled after 2003 formats
    /// </summary>
    private Boolean m_bShowLeaderLines;
    #endregion

    #region Class properties
    /// <summary>
    /// 0 = normal pie chart
    /// 1 = pie of pie chart
    /// 2 = bar of pie chart
    /// </summary>
    public ExcelPieType PieChartType
    {
      get
      {
        return ( ExcelPieType )m_PieType;
      }
      set
      {
        m_PieType = ( byte )value;
      }
    }
    /// <summary>
    /// True to use default split value; otherwise False.
    /// </summary>
    public bool   UseDefaultSplitValue
    {
      get
      {
        return m_bUseDefaultSplit;
      }
      set
      {
        m_bUseDefaultSplit = value;
      }
    }
    /// <summary>
    /// Split type:
    /// 0 = Position
    /// 1 = Value
    /// 2 = Percent
    /// 3 = Custom
    /// </summary>
    public ExcelSplitType ChartSplitType
    {
      get
      {
        return ( ExcelSplitType )m_usSplitType;
      }
      set
      {
        m_usSplitType = ( ushort )value;
      }
    }
    /// <summary>
    /// For split = 0, which positions should go to the other pie / bar.
    /// </summary>
    public ushort SplitPosition
    {
      get
      {
        return m_usSplitPos;
      }
      set
      {
        if( value != m_usSplitPos )
        {
          m_usSplitPos = value;
        }
      }
    }
    /// <summary>
    /// For split = 2, what percentage should go to the other pie / bar.
    /// </summary>
    public ushort SplitPercent
    {
      get
      {
        return m_usSplitPercent;
      }
      set
      {
        if( value != m_usSplitPercent )
        {
          m_usSplitPercent = value;
        }
      }
    }
    /// <summary>
    /// Size of the second pie as a percentage of the first.
    /// </summary>
    public ushort Pie2Size
    {
      get
      {
        return m_usPie2Size;
      }
      set
      {
        if( value != m_usPie2Size )
        {
          m_usPie2Size = value;
        }
      }
    }
    /// <summary>
    /// Space between the first pie and the second.
    /// </summary>
    public ushort Gap
    {
      get
      {
        return m_usGap;
      }
      set
      {
        if( value != m_usGap )
        {
          m_usGap = value;
        }
      }
    }
    /// <summary>
    /// For split = 1, what values should go to the other pie / bar.
    /// </summary>
    public int    NumSplitValue
    {
      get
      {
        return m_uiNumSplitValue;
      }
      set
      {
        if( value != m_uiNumSplitValue )
        {
          m_uiNumSplitValue = value;
        }
      }
    }
    /// <summary>
    /// 1 = the second pie / bar has a shadow.
    /// </summary>
    public bool   HasShadow
    {
      get
      {
        return m_bHasShadow;
      }
      set
      {
        m_bHasShadow = value;
      }
    }
    /// <summary>
    /// True to show leader lines to data labels.
    /// </summary>
    public bool ShowLeaderLines
    {
        get
        {
            return m_bShowLeaderLines;
        }
        set
        {
            m_bShowLeaderLines = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartBoppopRecord()
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
    public  ChartBoppopRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartBoppopRecord( int iReserve )
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
      m_PieType = provider.ReadByte( iOffset + 0 );
      m_UseDefaultSplit = provider.ReadByte( iOffset + 1 );
      m_bUseDefaultSplit = provider.ReadBit( iOffset + 1, 0 );
      m_usSplitType = provider.ReadUInt16( iOffset + 2 );
      m_usSplitPos = provider.ReadUInt16( iOffset + 4 );
      m_usSplitPercent = provider.ReadUInt16( iOffset + 6 );
      m_usPie2Size = provider.ReadUInt16( iOffset + 8 );
      m_usGap = provider.ReadUInt16( iOffset + 10 );
      m_uiNumSplitValue = provider.ReadInt32( iOffset + 12 );
      m_usHasShadow = provider.ReadUInt16( iOffset + 16 );
      m_bHasShadow = provider.ReadBit( iOffset + 16, 0 );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_usHasShadow &= 0x01;
      m_UseDefaultSplit &= 0x01;

      provider.WriteByte( iOffset + 0, m_PieType );
      provider.WriteByte( iOffset + 1, m_UseDefaultSplit );
      provider.WriteBit( iOffset + 1, m_bUseDefaultSplit, 0 );
      provider.WriteUInt16( iOffset + 2, m_usSplitType );
      provider.WriteUInt16( iOffset + 4, m_usSplitPos );
      provider.WriteUInt16( iOffset + 6, m_usSplitPercent );
      provider.WriteUInt16( iOffset + 8, m_usPie2Size );
      provider.WriteUInt16( iOffset + 10, m_usGap );
      provider.WriteInt32( iOffset + 12, m_uiNumSplitValue );
      provider.WriteUInt16( iOffset + 16, m_usHasShadow );
      provider.WriteBit( iOffset + 16, m_bHasShadow, 0 );

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