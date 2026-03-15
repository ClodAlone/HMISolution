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
  /// This record stores the scatter chart properties.
  /// </summary>
  [ Biff( TBIFFRecord.ChartScatter ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartScatterRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DefaultRecordSize = 6;
    #endregion

    #region Class members
    /// <summary>
    /// Percent of largest bubble compared to chart in general.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usBubleSizeRation;
    /// <summary>
    /// Bubble size.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usBubleSize;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// True if this a bubble series.
    /// </summary>
    [ BiffRecordPos( 4, 0, TFieldType.Bit ) ]
    private bool m_bBubbles;
    /// <summary>
    /// True to show negative bubbles.
    /// </summary>
    [ BiffRecordPos( 4, 1,TFieldType.Bit ) ]
    private bool m_bShowNegBubbles;
    /// <summary>
    /// True if bubble series has a shadow.
    /// </summary>
    [ BiffRecordPos( 4, 2, TFieldType.Bit ) ]
    private bool m_bHasShadow;
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }

    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DefaultRecordSize;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Percent of largest bubble compared to chart in general.
    /// </summary>
    public ushort BubleSizeRation
    {
      get
      {
        return m_usBubleSizeRation;
      }
      set
      {
        m_usBubleSizeRation = value;
      }
    }
    /// <summary>
    /// Bubble size.
    /// </summary>
    public ExcelBubbleSize BubleSize
    {
      get
      {
        return (ExcelBubbleSize) m_usBubleSize;
      }
      set
      {
        m_usBubleSize = (ushort) value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
      set
      {
        m_usOptions = value;
      }
    }
    /// <summary>
    /// True if this a bubble series.
    /// </summary>
    public bool IsBubbles
    {
      get
      {
        return m_bBubbles;
      }
      set
      {
        m_bBubbles = value;
      }
    }
    /// <summary>
    /// True to show negative bubbles.
    /// </summary>
    public bool IsShowNegBubbles
    {
      get
      {
        return m_bShowNegBubbles;
      }
      set
      {
        m_bShowNegBubbles = value;
      }
    }
    /// <summary>
    /// True if bubble series has a shadow.
    /// </summary>
    public bool HasShadow
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
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartScatterRecord()
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
    public  ChartScatterRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartScatterRecord( int iReserve )
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
      m_usBubleSizeRation = provider.ReadUInt16( iOffset + 0 );
      m_usBubleSize = provider.ReadUInt16( iOffset + 2 );
      m_usOptions = provider.ReadUInt16( iOffset + 4 );
      m_bBubbles = provider.ReadBit( iOffset + 4, 0 );
      m_bShowNegBubbles = provider.ReadBit( iOffset + 4, 1 );
      m_bHasShadow = provider.ReadBit( iOffset + 4, 2 );
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
      provider.WriteUInt16( iOffset + 0, m_usBubleSizeRation );
      provider.WriteUInt16( iOffset + 2, m_usBubleSize );
      provider.WriteUInt16( iOffset + 4, m_usOptions );
      provider.WriteBit( iOffset + 4, m_bBubbles, 0 );
      provider.WriteBit( iOffset + 4, m_bShowNegBubbles, 1 );
      provider.WriteBit( iOffset + 4, m_bHasShadow, 2 );
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